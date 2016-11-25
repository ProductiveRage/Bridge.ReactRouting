using System;
using Bridge.React;
using Example.Actions;
using ProductiveRage.Immutable;
using ProductiveRage.ReactRouting;

namespace Example.Navigation
{
	/// <summary>
	/// This is responsible for defining routes and then trying to match them after each history change. Since it deals for defining the routes, it is also
	/// responsible for telling everyone else how to construct URLs for each route (for each, a component that needs to include a Link element would not try
	/// to build up the URL itself - instead it would call one of the methods on this class, which would return the URL that should be linked to).
	/// </summary>
	public sealed class ExampleNavigator : Navigator
	{
		private readonly Func<UrlPathDetails> _getHome, _getAccommodation;
		private readonly Func<NonBlankTrimmedString, UrlPathDetails> _getAccommodationWithSegment;
		public ExampleNavigator(AppDispatcher dispatcher) : base(dispatcher)
		{
			if (dispatcher == null)
				throw new ArgumentNullException("dispatcher");

			// Declare pairs of routes and actions which will send the user along these routes. Doing both halves of the route management (registering and
			// following) together keeps each route's code localised while still allowing a static navigation interface to be created. When the user needs
			// to be sent to the an "Accommodation" sub-page, there won't be code to send them to "/Accommodation/xyz", a call will instead to be made to
			// the "Accommodation" method of this class, providing the sub-page id, and the target URL will be returned. This ensures that there is no
			// chance of changes to routes here not being reflected in other code (if "Accommodation" was changed to "WhereToStay", for example, then
			// no other code need be changed since it will all be calling the "Accommodation" method; if the other code was manually trying to go to
			// "/Accommodation/xyz" then that code would also have to be changed to "/WhereToStay/xyz" in all places and any mistakes wouldn't be
			// caught until runtime).

			// Register home
			_getHome = AddRelativeRoute(
				segments: Set<string>.Empty,
				routeAction: new NavigateToHome(),
				urlGenerator: () => GetPath()
			);

			// Register "/Accommodation"
			_getAccommodation = AddRelativeRoute(
				segment: "Accommodation",
				routeAction: new NavigateToAccommodation(Optional<NonBlankTrimmedString>.Missing),
				urlGenerator: () => GetPath("Accommodation")
			);

			// Register "/Accommodation/{string}"
			_getAccommodationWithSegment = AddRelativeRoute(
				routeDetails: RouteBuilder.Empty.Fixed("Accommodation").String(),
				routeActionGenerator: matchedValue => new NavigateToAccommodation(matchedValue),
				urlGenerator: segment => GetPath("Accommodation", segment)
			);
		}

		public UrlPathDetails Home() { return _getHome(); }
		public UrlPathDetails Accommodation() { return _getAccommodation(); }
		public UrlPathDetails Accommodation(NonBlankTrimmedString segment) { return _getAccommodationWithSegment(segment); }
	}
}