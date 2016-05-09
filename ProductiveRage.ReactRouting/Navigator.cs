using System;
using Bridge.React;
using ProductiveRage.Immutable;
using ProductiveRage.Immutable.Extensions;

namespace ProductiveRage.ReactRouting
{
	public abstract class Navigator
	{
		private readonly Set<NonBlankTrimmedString> _parentSegments;
		private readonly IInteractWithBrowserRouting _historyHandler;
		private readonly AppDispatcher _dispatcher;
		private Set<IMatchRoutes> _routes;

		protected Navigator(Set<NonBlankTrimmedString> parentSegments, IInteractWithBrowserRouting historyHandler, AppDispatcher dispatcher)
		{
			if (parentSegments == null)
				throw new ArgumentNullException("parentSegments");
			if (historyHandler == null)
				throw new ArgumentNullException("historyHandler");
			if (dispatcher == null)
				throw new ArgumentNullException("dispatcher");

			_parentSegments = parentSegments;
			_historyHandler = historyHandler;
			_dispatcher = dispatcher;
			_routes = Set<IMatchRoutes>.Empty;
		}
		protected Navigator(IInteractWithBrowserRouting historyHandler, AppDispatcher dispatcher)
			: this(Set<NonBlankTrimmedString>.Empty, historyHandler, dispatcher) { }

		public Set<IMatchRoutes> Routes { get { return _routes; } }

		/// <summary>
		/// The single segment must be a non-null, non-blank and non-whitespace-only value. The method signature only allows string rather than
		/// NonBlankTrimmedString to make the route-declaring code in derived classes as clean as possible (at the cost of a less descriptive
		/// method signature). Since the segment value will be wrapped in a NonBlankTrimmedString instance, any leading or trailing whitespace
		/// will be ignored. If any parent segments were passed to the constructor of this instance, they will automatically be injected before
		/// the segment specified here.
		/// </summary>
		protected void AddRelativeRoute(string segment, IDispatcherAction routeAction)
		{
			if (string.IsNullOrWhiteSpace(segment))
				throw new ArgumentException("Null, blank or whitespace-only segment specified");
			if (routeAction == null)
				throw new ArgumentNullException("routeAction");

			AddRelativeRoute(Set.Of(segment), routeAction);
		}

		/// <summary>
		/// All of the segments must be non-blank and non-whitespace-only values. The method signature only allows strings rather than
		/// NonBlankTrimmedStrings to make the route-declaring code in derived classes as clean as possible (at the cost of a less descriptive
		/// method signature). Since the segment values will be wrapped in NonBlankTrimmedString instances, any leading or trailing whitespace
		/// will be ignored. If any parent segments were passed to the constructor of this instance, they will automatically be injected before
		/// the segments specified here.
		/// </summary>
		protected void AddRelativeRoute(Set<string> segments, IDispatcherAction routeAction)
		{
			if (segments == null)
				throw new ArgumentNullException("segments");
			if (routeAction == null)
				throw new ArgumentNullException("routeAction");

			var relativeRouteSegments = RouteBuilder.Empty;
			foreach (var segment in segments)
			{
				if (segment.Trim() == "") // No need to check for null, a Set never contains null references
					throw new ArgumentException("Blank or whitespace-only segment specified in fixed route");
				relativeRouteSegments = relativeRouteSegments.Fixed(new NonBlankTrimmedString(segment));
			}
			AddRelativeRoute(
				relativeRouteSegments.ToRoute(() => _dispatcher.HandleViewAction(routeAction))
			);
		}

		/// <summary>
		/// This is a convenience method so that the derived class may declare variable routes in a structure that is similar to the way way in
		/// which the fixed routes may be declared through the alternate AddRelativeRoute signatures - where the details of the route are specified
		/// by the first argument and the Dispatcher action to raise if the route is matched is defined using the second argument. If any parent
		/// segments were passed to the constructor of this instance, they will automatically be injected into the route described here.
		/// </summary>
		protected void AddRelativeRoute<TMatchedValue>(
			RouteBuilder.IBuildRoutesWithVariablesToMatch<TMatchedValue> routeDetails,
			Func<TMatchedValue, IDispatcherAction> routeActionGenerator) where TMatchedValue : class
		{
			if (routeDetails == null)
				throw new ArgumentNullException("routeDetails");
			if (routeActionGenerator == null)
				throw new ArgumentNullException("routeActionGenerator");

			AddRelativeRoute(
				routeDetails.ToRoute(matchedValues => _dispatcher.HandleViewAction(routeActionGenerator(matchedValues)))
			);
		}

		/// <summary>
		/// If any parent segments were passed to the constructor of this instance, they will automatically be injected into the route provided here
		/// </summary>
		protected void AddRelativeRoute(IMatchRoutes route)
		{
			if (route == null)
				throw new ArgumentNullException("route");

			_routes = _routes.Add(route.MakeRelativeTo(_parentSegments));
		}

		/// <summary>
		/// This is a convenience method to make the calling code cleaner, at the cost of the method signature being less descriptive - each
		/// segment must be a non-null, non-blank and non-whitespace-only value, they will all be wrapped in NonBlankTrimmedString instances
		/// and so any leading or trailing whitespace will be ignored. The values will be translated into a UrlDetails instance, which will
		/// be passed to the history handler's NavigateTo method.
		/// </summary>
		protected UrlDetails GetPath(params string[] segments)
		{
			if (segments == null)
				throw new ArgumentNullException("segment");

			var nonBlankSegments = Set<NonBlankTrimmedString>.Empty;
			foreach (var segment in _parentSegments)
				nonBlankSegments = nonBlankSegments.Add(new NonBlankTrimmedString(segment));
			foreach (var segment in segments)
			{
				if (string.IsNullOrWhiteSpace(segment))
					throw new ArgumentException("Null/blank/whitespace-only string encountered in segments array");
				nonBlankSegments = nonBlankSegments.Add(new NonBlankTrimmedString(segment));
			}
			return new UrlDetails(nonBlankSegments);
		}
	}
}