using System;
using Bridge.React;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting
{
	public static class RouteCombiner
	{
		/// <summary>
		/// This method overload will fire an InvalidRoute action to the specified dispatcher if a URL is encountered that does not match any of the provided route (this should be used in most cases since
		/// it means that all URLs result in a particular action being sent, whether the URL may be matched to a route or whether it was non-matched)
		/// </summary>
		public static void StartListening(IInteractWithBrowserRouting historyHandler, NonNullList<IMatchRoutes> routes, IDispatcher dispatcherForInvalidRouteActions)
		{
			if (historyHandler == null)
				throw new ArgumentNullException(nameof(historyHandler));
			if (routes == null)
				throw new ArgumentNullException(nameof(routes));
			if (dispatcherForInvalidRouteActions == null)
				throw new ArgumentNullException(nameof(dispatcherForInvalidRouteActions));

			StartListening(historyHandler, routes, url => dispatcherForInvalidRouteActions.HandleViewAction(new InvalidRoute(url)));
		}

		/// <summary>
		/// This method overload will execute the specified action if a URL is encountered that does not match any of the provided route (for cases where custom behaviour is desired, other than firing
		/// an InvalidRoute action at the dispatcher if an non-matching URL is found)
		/// </summary>
		public static void StartListening(IInteractWithBrowserRouting historyHandler, NonNullList<IMatchRoutes> routes, Action<UrlPathDetails> routeNotMatched)
		{
			if (historyHandler == null)
				throw new ArgumentNullException(nameof(historyHandler));
			if (routes == null)
				throw new ArgumentNullException(nameof(routes));
			if (routeNotMatched == null)
				throw new ArgumentNullException(nameof(routeNotMatched));

			StartListening(historyHandler, routes, Optional.For(routeNotMatched));
		}

		/// <summary>
		/// This method overload will optionally accept an action for cases where a route is found that can not be matched, if no routeNotMatched action is provided then invalid URLs will be ignored
		/// </summary>
		public static void StartListening(IInteractWithBrowserRouting historyHandler, NonNullList<IMatchRoutes> routes, Optional<Action<UrlPathDetails>> routeNotMatched = new Optional<Action<UrlPathDetails>>())
		{
			if (historyHandler == null)
				throw new ArgumentNullException(nameof(historyHandler));
			if (routes == null)
				throw new ArgumentNullException(nameof(routes));

			historyHandler.RegisterForNavigatedCallback(url => MatchRoute(url, routes, routeNotMatched));
		}

		private static void MatchRoute(UrlDetails url, NonNullList<IMatchRoutes> routes, Optional<Action<UrlPathDetails>> routeNotMatched)
		{
			if (url == null)
				throw new ArgumentNullException(nameof(url));
			if (routes == null)
				throw new ArgumentNullException(nameof(routes));

			foreach (var route in routes)
			{
				if (route.ExecuteCallbackIfUrlMatches(url))
					return;
			}
			if (routeNotMatched.IsDefined)
				routeNotMatched.Value(url);
		}
	}
}