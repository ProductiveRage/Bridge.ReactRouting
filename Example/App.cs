using Bridge.Html5;
using Bridge.React;
using Example.Actions;
using Example.Navigation;
using HostBridge.Components;
using ProductiveRage.ReactRouting;
using ProductiveRage.ReactRouting.Helpers;

namespace Example
{
	public static class App
	{
		[Ready]
		public static void Main()
		{
			// The primary intentions of this library are twofold:
			//
			//   1. To handle routing in a "strongly-typed" way that ensures that routes are only defined in one placed and that this information is used to
			//      generate links to those routes in the application (for example, if a route is configured to map "/accommodation" to an AccommodationContainer
			//      component and then, one day in the future, the route is changed to "/where-to-stay", there should only be one place that needs to be updated,
			//      there should be worry that there may be many anchor tags throughout the application with hard-coded URLs that all need changing from the old
			//      "/accommodation" URL to the new "/where-to-stay" format)
			//
			//   2. To decouple the aspects of the routing library to make it easy to configure and easy to test
			//      a. This requires a "Navigator" which defines routes and maps them on to actions (actions in the context of a Flux-like architecture that will
			//         be passed through a dispatcher). This will also expose properties and/or methods for generating URLs that correspond to those routes so
			//         that other code may request URLs from this "one authoritative source of routes" instead of URLs having to be hand coded. In this project,
			//         this is implemented by the ExampleNavigator.
			//      b. It also requires a navigation-to-action matcher" whose role is to map the navigation actions to React elements - the premise being that when
			//         a navigation action is received, a different area of the application will be displayed (or a different page within the same area). This may
			//         be implemented in any way that you see fit but the ReactRouting library includes a NavigateActionMatcher class that is helpful for constructing
			//         mappings from actions to components and it includes a RoutingStoreActivatorContainer that will take a NavigateActionMatcher instance and ensure
			//         that the appropriate component is rendered within it, depending upon the last navigation action.
			//      c. Finally, something is required to listen to navigation events - in this example that is the ReactRouting's Html5HistoryRouter, which means that
			//         navigation events are published and subscribed to/from the browser's HTML5 history (pushState) API but it could be any implementation of the
			//         IInteractWithBrowserRouting interface (for example, the unit tests use a "MockHistoryHandler" that allow navigation events to be raised and
			//         received without having to try to read/change the URL of the browser hosting the tests).
			//         
			// The navigator's methods "Home()", "Accommodation()" and "Accommodation(section)" return the appropriate URLs for each route (as a UrlPathDetails
			// instance). These URLs should be rendered using the ReactRouting's "Link" component rather than a simple anchor tag since navigation from an anchor tag
			// will directly change the browser's URL instead of using the HTML5 history API to raise a navigation event without actually reloading the page (the Link
			// component uses the Html5HistoryRouter by default but may be configured to use an alternative router if you need it to).
			// - The Link component has options to set additional class names if it relates to the current URL or if it relates to an ancester of the current URL (if
			//   it links to "/accommodation" and the current URL is "/accommodation/hotels", for example). If these options are used then the Link component should
			//   not be rendered within a PureComponent, since it varies by more than just its Props - it is fine for it to be within an ancestor chain of Stateless-
			//   Components or Components, though (for this reason, the NavigationLinks in this example is a StatelessComponent and not a PureComponent).
			//
			// Note that this library only supports route-matching by URL segments (eg. "/accommodation/hotels") and not QueryString (eg. "/accommodation?section=hotels").

			// The navigator exposes methods to navigate URLs and it sends actions through the dispatcher when the route changes
			var dispatcher = new AppDispatcher();
			var navigator = new ExampleNavigator(dispatcher);

			// These are the components that should be displayed based upon the navigation actions that come through the dispatcher
			// - The NavigateActionMatcher class just offers a simple way to build up the mappings from navigation actions to ReactElement-to-display (the
			//   NavigateActionMatcher instance will be passed to the RoutingStoreActivatorContainer that will ensure that the appropriate ReactElement is
			//   rendered to whatever container is specified, see below..)
			var navigateActionMatchers = NavigateActionMatcher.Empty
				.AddFor<NavigateToHome>(new HomeContainer(navigator))
				.AddFor<NavigateToAccommodation>(
					condition: action => action.Segment.IsDefined,
					elementGenerator: action => new AccommodationListContainer(navigator, action.Segment.Value)
				)
				.AddFor<NavigateToAccommodation>(new AccommodationContentContainer(navigator))
				.AddFor<InvalidRoute>(new NotFoundContainer(navigator));

			// Render the application state (since no navigiation events have been raised yet, this will not display anything - but the RoutingStoreActivatorContainer
			// will be waiting to receive navigation actions so that the appropriate content for the URL can be displayed in a moment)
			React.Render(
				new RoutingStoreActivatorContainer(dispatcher, navigateActionMatchers),
				Document.GetElementById("main")
			);

			// Start handling routes (calling RaiseNavigateToForCurrentLocation will result in an action being raised for the current URL, so the RoutingStoreActivatorContainer
			// component can mount the appropriate container component)
			var browserHistoryHandler = Html5HistoryRouter.Instance;
			RouteCombiner.StartListening(browserHistoryHandler, navigator.Routes, dispatcher);
			browserHistoryHandler.RaiseNavigateToForCurrentLocation();
		}
	}
}