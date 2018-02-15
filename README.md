# A Bridge.NET routing solution for React applications

The intentions of this library are twofold:

**1. To handle routing in a "strongly-typed" way that ensures that routes are only defined in one placed and that this information is used to generate links to those routes in the application** (for example, if a route is configured to map "/accommodation" to an AccommodationContainer component and then, one day in the future, the route is changed to "/where-to-stay", there should only be one place that needs to be updated, there should be no worry that there may be many anchor tags throughout the application with hard-coded URLs that all need changing from the old "/accommodation" URL to the new "/where-to-stay" format)

**2. To decouple the aspects of the routing library to make it easy to configure and easy to test** -

* This requires a "Navigator" which defines routes and maps them on to actions (actions in the context of a Flux-like architecture that will be passed through a dispatcher). This will also expose properties and/or methods for generating URLs that correspond to those routes so that other code may request URLs from this "one authoritative source of routes" instead of URLs having to be hand coded. In this project, this is implemented by the ExampleNavigator.
* It also requires a navigation-to-action matcher" whose role is to map the navigation actions to React elements - the premise being that when a navigation action is received, a different area of the application will be displayed (or a different page within the same area). This may be implemented in any way that you see fit but the ReactRouting library includes a NavigateActionMatcher class that is helpful for constructing mappings from actions to components and it includes a RoutingStoreActivatorContainer that will take a NavigateActionMatcher instance and ensure that the appropriate component is rendered within it, depending upon the last navigation action.
* Finally, something is required to listen to navigation events - in this example that is the ReactRouting's Html5HistoryRouter, which means that navigation events are published and subscribed to/from the browser's HTML5 history (pushState) API but it could be any implementation of the IInteractWithBrowserRouting interface (for example, the unit tests use a "MockHistoryHandler" that allow navigation events to be raised and received without having to try to read/change the URL of the browser hosting the tests).

*(Note: Routes are only matched against the URL path, there is no support for QueryString-based routing - though you can still extract values from the QueryString to configure the navigation action that should be dispatched, see the "QueryString Data" section further down)*

Below is an example of a "Navigator" class that defines three routes and exposes public methods so that URLs may be generated that correspond to those routes -

	public sealed class ExampleNavigator : Navigator
	{
		private readonly Func<UrlPathDetails> _getHome, _getAccommodation;
		private readonly Func<NonBlankTrimmedString, UrlPathDetails> _getAccommodationWithSegment;
		public ExampleNavigator(AppDispatcher dispatcher) : base(dispatcher)
		{
			// Register home
			_getHome = AddRelativeRoute(
				segments: NonNullList<string>.Empty,
				routeAction: new NavigateToHome(),
				urlGenerator: () => GetPath()
			);

			// Register "/Accommodation"
			const string accommodationSegment = "Accommodation";
			_getAccommodation = AddRelativeRoute(
				segment: accommodationSegment,
				routeAction: new NavigateToAccommodation(Optional<NonBlankTrimmedString>.Missing),
				urlGenerator: () => GetPath(accommodationSegment)
			);

			// Register "/Accommodation/{string}"
			_getAccommodationWithSegment = AddRelativeRoute(
				routeDetails: RouteBuilder.Empty.Fixed(accommodationSegment).String(),
				routeActionGenerator: matchedValue => new NavigateToAccommodation(matchedValue),
				urlGenerator: segment => GetPath(accommodationSegment, segment)
			);
		}

		public UrlPathDetails Home()
		{
			return _getHome();
		}
		public UrlPathDetails Accommodation()
		{
			return _getAccommodation();
		}
		public UrlPathDetails Accommodation(NonBlankTrimmedString segment)
		{
			return _getAccommodationWithSegment(segment);
		}
	}

Having Navigator methods that generate the URLs mean that if a route needs to be changed at any point, these changes will not have to applied in many other places throughout the code (which is the case with some other routers because the code that renders links is generally removed from the code that is responsible for recognising routes).

The ExampleNavigator class would be integrated into an application with code such as the following:

	// The AppDispatcher is part of the Bridge.React library, which this depends upon
	var dispatcher = new AppDispatcher();
	var navigator = new ExampleNavigator(dispatcher);
	
	// These are the components that should be displayed based upon the navigation actions that come through the
	// dispatcher
	// - The NavigateActionMatcher class just offers a simple way to build up the mappings from navigation actions
	//   to ReactElement-to-display (the NavigateActionMatcher instance will be passed to the
	//   RoutingStoreActivatorContainer that will ensure that the appropriate ReactElement
	//   is rendered to whatever container is specified, see below..)
	var navigateActionMatchers = NavigateActionMatcher.Empty
	  .AddFor<NavigateToHome>(new HomeContainer(navigator))
	  .AddFor<NavigateToAccommodation>(
	    condition: action => action.Segment.IsDefined,
	    elementGenerator: action => new AccommodationListContainer(navigator, action.Segment.Value)
	  )
	  .AddFor<NavigateToAccommodation>(new AccommodationContentContainer(navigator))
	  .AddFor<InvalidRoute>(new NotFoundContainer(navigator));
	
	// Render the application state (since no navigiation events have been raised yet, this will not display anything
	// but the RoutingStoreActivatorContainer will be waiting to receive navigation actions so that the appropriate
	// content for the URL can be displayed in a moment)
	React.Render(
	  new RoutingStoreActivatorContainer(dispatcher, navigateActionMatchers),
	  Document.GetElementById("main")
	);
	
	// Start handling routes (calling RaiseNavigateToForCurrentLocation will result in an action being raised for the
	// current URL, so the RoutingStoreActivatorContainer component can mount the appropriate container component)
    // - Note: an InvalidRoute action will be sent through the dispatcher if a URL is encountered that does not
    //   match any routes
	var browserHistoryHandler = Html5HistoryRouter.Instance;
	RouteCombiner.StartListening(browserHistoryHandler, navigator.Routes, dispatcher);
	browserHistoryHandler.RaiseNavigateToForCurrentLocation();
    
*(Note: The action classes and container components would be defined within the application code, though the InvalidRoute is part of the ReactRouting library)*

## Rendering links

Routes are matched the application whenever the history handler (which uses the HTML5 history API by default) indicates that the current URL has changed. This means that, in order to prevent page reloads, that anchor tags need to change the URL in a way that the history handler can identify, rather than generating tags that will instruct the browser to treat the URL as requiring a new page request. Presuming that you are using HTML5 history, this just means changing anchor tag behaviour to call "pushState" rather than allowing the browser to navigate away.

To make this as simple as possible, the library include a "Link" component that takes a "UrlDetails" instance and a text string and renders an anchor tag that translates left clicks into pushState requests (it *only* intercepts left clicks and only if no modifier keys - such as [Shift] or [Ctrl] - are being held down, so that open-in-new-tab clicks work as the user expects).

The Link component has other optional configuration properties, such as "name" and "target" (which will be applied to the anchor tag) and "className", "ancestorClassName" and "selectedClassName" - the first is always applied, the second is applied if the Link's URL is a parent of the current URL and the third is applied if the Link's URL is a precise match to the current URL. 

## More complicated route-matching

In the above example code, there are only two types of route matched - fixed routes (such as "Home", which has zero URL segments, and "Accommodation", which has a single URL segment) and single-variable routes ("/Accommodation/{string}"). There are several ways to generate routes with more variable segments. The first, and simplest, is to build up a list of Tuples for each variable segment - eg.

	RouteBuilder.Empty.Fixed("Accommodation").String().Int()
  
This will match routes such as "/Accommodation/Hotels/123" and the variable segments will be tracked internally by the route builder using a Tuple (where the Item1 is a NonBlankTrimmedString and Item2 is an int). If the ExampleNavigator (from above) was updated to handle this route then it would generate a new Func<NonBlankTrimmedString, int, UrlPathDetails> using the following:

	_getAccommodationWithSegmentAndId = AddRelativeRoute(
		routeDetails: RouteBuilder.Empty.Fixed("Accommodation").String().Int(),
		routeActionGenerator: (segment, index) => new NavigateToAccommodation(segment, index),
		urlGenerator: (segment, index) => GetPath("Accommodation", segment, index)
	);
    
Note that the "routeActionGenerator" and "urlGenerator" delegates unwrap the Tuple's values so that it's possible to name the individual values with something more descriptive than Item1 and Item2 - this is because the library has some special behaviour for Tuple-based routes since they're so convenient to use.

This is very simple and type-safe but it does have a limitation - namely that the maximum number of variable URL segments that may be matched is eight because that's as many type arguments as the Tuple class will accept.

One alternative is to build route match data using anonymous types. There are overloads for the variable segment matching methods that take a Func that maps from the current match data (if any) to a new value that incorporates the current segment's content - eg.

	RouteBuilder.Empty
		.Fixed("Accommodation")
		.String(category => new { Category = category })
		.Int((matchSoFar, index) => new { Category = matchSoFar.Category, Index = index })
    
This approach avoids the disadvantage of the Tuple approach since there is no limit to how many variables may be matched but at the cost of having to write more code and having to repeat the property names each time another variable segment is matched.

A third alternative is to define a type that will contain the route match data that implements IAmImmutable (part of the [ProductiveRage.Immutable](https://github.com/ProductiveRage/Bridge.Immutable/) library, which this route depends upon) and to build this up from each matched segment. Something like:

	private sealed class AccommodationRouteInfo : IAmImmutable
	{
		public AccommodationRouteInfo(
			Optional<NonBlankTrimmedString> segment = new Optional<NonBlankTrimmedString>(),
			Optional<int> index = new Optional<int>())
		{
			this.CtorSet(_ => _.Segment, segment);
			this.CtorSet(_ => _.Index, index);
		}
		public Optional<NonBlankTrimmedString> Segment { get; }
		public Optional<int> Index { get; }
	}

The "/Accommodation/{string}" route definition from the ExampleNavigator above could be changed to the following to make use of that class:

	// Register "/Accommodation/{string}"
	_getAccommodationWithSegment = AddRelativeRoute(
		routeDetails: RouteBuilder.Empty
			.Fixed(accommodationSegment)
			.String(),
		routeActionGenerator: matchedValue => new NavigateToAccommodation(matchedValue),
		urlGenerator: segment => GetPath(accommodationSegment, segment)
	);
  
 And defining another route for "/Accommodation/{string}/{int}" would look like this:

	// Register "/Accommodation/{string}/{int}"
	_getAccommodationWithSegmentAndIndex = AddRelativeRoute(
		routeDetails: RouteBuilder.Empty
			.Fixed(accommodationSegment)
			.String(segment => new AccommodationRouteInfo(segment))
			.Int((matchSoFar, index) => matchSoFar.With(_ => _.Index, index)),
		routeActionGenerator: matchedValue => new NavigateToAccommodation(matchedValue.Segment, matchedValue.Index),
		urlGenerator: routeInfo => GetPath(accommodationSegment, routeInfo.Segment, routeInfo.Index)
	);

This is also more verbose than the Tuple approach but it has the anonymous-type-approach's advantage of not limiting how many segments may be matches and it doesn't have the *disadvantage* around the duplication of property names within each variable route segment's matching logic,.

I suspect that Tuples will offer the most convenient and succinct code in many cases but there are alternatives to consider for when you want to be more expressive.

## Nesting routes

When segregating code, it may be desirable to have one "module" that is responsible for handling routes within a section of an application but which doesn't know what the route into that section is. For example, instead of the ExampleNavigator defining routes for home *and* for the root of the Accommodation section *and* for the single-variable route within Accommodation, it may be make sense to move the Accommodation routes into its own Navigator class -

	public sealed class AccommodationNavigator : Navigator
	{
		private readonly Func<UrlPathDetails> _getRoot;
		private readonly Func<NonBlankTrimmedString, UrlPathDetails> _getWithSegment;
		private readonly Func<NonBlankTrimmedString, int, UrlPathDetails> _getWithSegmentAndIndex;
		public AccommodationNavigator(NonNullList<NonBlankTrimmedString> parentSegments, AppDispatcher dispatcher)
            : base(parentSegments, dispatcher)
		{
			_getRoot = AddRelativeRoute(
				segments: NonNullList<string>.Empty,
				routeAction: new NavigateToAccommodation(),
				urlGenerator: () => GetPath()
			);

			_getWithSegment = AddRelativeRoute(
				routeDetails: RouteBuilder.Empty.String(),
				routeActionGenerator: segment => new NavigateToAccommodation(segment),
				urlGenerator: segment => GetPath("Accommodation", segment)
			);
		}

		public UrlPathDetails Root() { return _getRoot(); }
		public UrlPathDetails Segment(NonBlankTrimmedString segment)
		{
			return _getWithSegment(segment);
		}
	}

Note that the routes are defined as if they start from the root of the site (there is no mention of the fixed "Accommodation" segment) and, instead, a "parentSegments" list is passed into the constructor.

The ExampleNavigator becomes:

	public sealed class ExampleNavigator : Navigator
	{
		private readonly Func<UrlPathDetails> _getHome;
		public ExampleNavigator(AppDispatcher dispatcher) : base(dispatcher)
		{
			_getHome = AddRelativeRoute(
				segments: NonNullList<string>.Empty,
				routeAction: new NavigateToHome(),
				urlGenerator: () => GetPath()
			);

			PullInRoutesFrom(Accommodation = new AccommodationNavigator(
				parentSegments: NonNullList.Of(new NonBlankTrimmedString("Accommodation")),
				dispatcher: dispatcher
			));
		}

		public UrlPathDetails Home() { return _getHome(); }
		public AccommodationNavigator Accommodation { get; }
	}

Now, given a reference "navigator" to an ExampleNavigator instance, to get to "/Accommodation/Hotels" you would call the method

    var hotelsUrl = navigator.Accommodation.Segment(new NonBlankTrimmedString("Hotels"));

**Important:** You need to be sure to pass all "child navigators" through the "PullInRoutesFrom" method because a Navigator implementation neesd to be able to declare all of the results that it (and any child navigators) are responsible for and "PullInRoutesFrom" adds all of the routes from the child navigator to the internal list maintained by the parent navigator.

## QueryString Data

Although routes may only be defined by the URL path, it may be desirable to include some extra information in the QueryString and to use QueryString content to configure the navigation action. To return an earlier example -

	// Register "/Accommodation/{string}"
	_getAccommodationWithSegment = AddRelativeRoute(
		routeDetails: RouteBuilder.Empty.Fixed("Accommodation").String(),
		routeActionGenerator: matchedValue => new NavigateToAccommodation(matchedValue),
		urlGenerator: segment => GetPath("Accommodation", segment)
	);

.. if we wanted to include a "UserId" value in the QueryString that would be passed to the **NavigateToAccommodation** action as an **Optional&lt;int&gt;** then we could use a different "AddRelativeRoute" method overload that takes a "routeActionGenerator" delegate that receives a "matchedValue" reference *and* a "queryString" reference -

	// Register "/Accommodation/{string}"
	_getAccommodationWithSegment = AddRelativeRoute(
		routeDetails: RouteBuilder.Empty.Fixed("Accommodation").String(),
		routeActionGenerator: (matchedValue, queryString) => new NavigateToAccommodation(
			matchedValue,
			queryString.Int("UserId")
		),
		urlGenerator: segment => GetPath("Accommodation", segment)
	);

If there is no "UserId" QueryString (or if it is not parseable into an integer) then queryString.Int will return **Optional&lt;int&gt;**.Missing and the **Optional&lt;int&gt;** action must be designed to take this into account.

There is also a "String" method available on the queryString reference (that returns a **Optional&lt;NonBlankTrimmedString&gt;**) and it should be easy for you to use this to parse QueryString values into any other data type that you might need.

If you are reading values from the QueryString when processing routes then you will also want to be populating the QueryString. To do that, change the functions that build URLs such as this:

	public UrlPathDetails Accommodation(NonBlankTrimmedString segment)
	{
		return _getAccommodationWithSegment(segment);
	}
		
.. and make it return a **UrlDetails** instance (instead of **UrlPathDetail** - **UrlDetails** is just a **UrlPathDetails** with an optional QueryString).

	public UrlDetails Accommodation(NonBlankTrimmedString segment, Optional<int> userId)
	{
		return _getAccommodationWithSegment(segment)
			.ToUrlDetails(queryString: null)
			.AddToQueryIfDefined("UserId", userId);
	}
	
The "ToUrlDetails" function appends the specified "queryString" reference (if any) to the **UrlPathDetails** instance to create a **UrlDetails** instance.

The "AddToQueryIfDefined" function will extend the **UrlDetails**'s QueryString with the specified value if it is not {Missing} (so if the "userId" is {Missing} then no QueryString will be generated but if "userId" has a value then a QueryString *will* be generated of the form "UserId=123").























