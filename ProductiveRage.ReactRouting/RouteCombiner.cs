using System;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting
{
	public static class RouteCombiner
	{
		public static void StartListening(
			IInteractWithBrowserRouting historyHandler,
			Set<IMatchRoutes> routes,
			Optional<Action<UrlPathDetails>> routeNotMatched = new Optional<Action<UrlPathDetails>>())
		{
			if (historyHandler == null)
				throw new ArgumentNullException("historyHandler");
			if (routes == null)
				throw new ArgumentNullException("routes");

			historyHandler.RegisterForNavigatedCallback(url => MatchRoute(url.ToUrlPathDetails(), routes, routeNotMatched));
		}

		public static void StartListening(IInteractWithBrowserRouting historyHandler, Set<IMatchRoutes> routes, Action<UrlPathDetails> routeNotMatched)
		{
			if (routeNotMatched == null)
				throw new ArgumentNullException("routeNotMatched");
			if (historyHandler == null)
				throw new ArgumentNullException("historyHandler");
			if (routes == null)
				throw new ArgumentNullException("routes");

			StartListening(historyHandler, routes, Optional.For(routeNotMatched));
		}

		private static void MatchRoute(UrlPathDetails url, Set<IMatchRoutes> routes, Optional<Action<UrlPathDetails>> routeNotMatched)
		{
			if (url == null)
				throw new ArgumentNullException("url");
			if (routes == null)
				throw new ArgumentNullException("routes");

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