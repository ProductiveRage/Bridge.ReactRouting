using System;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting
{
	public static class RouteCombiner
	{
		public static void StartListening(
			IInteractWithBrowserRouting historyHandler,
			Set<IMatchRoutes> routes,
			Optional<Action<UrlDetails>> routeNotMatched = new Optional<Action<UrlDetails>>())
		{
			if (historyHandler == null)
				throw new ArgumentNullException("historyHandler");
			if (routes == null)
				throw new ArgumentNullException("routes");

			historyHandler.RegisterForNavigatedCallback(url => MatchRoute(url, routes, routeNotMatched));
		}

		public static void StartListening(IInteractWithBrowserRouting historyHandler, Set<IMatchRoutes> routes, Action<UrlDetails> routeNotMatched)
		{
			if (routeNotMatched == null)
				throw new ArgumentNullException("routeNotMatched");
			if (historyHandler == null)
				throw new ArgumentNullException("historyHandler");
			if (routes == null)
				throw new ArgumentNullException("routes");

			StartListening(historyHandler, routes, Optional.For(routeNotMatched));
		}

		private static void MatchRoute(UrlDetails url, Set<IMatchRoutes> routes, Optional<Action<UrlDetails>> routeNotMatched)
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

			// TODO: Only required until http://forums.bridge.net/forum/bridge-net-pro/bugs/1993 is fixed
			if (routeNotMatched.IsDefined)
			{
				var x = routeNotMatched.Value;
				x(url);
			}
		}
	}
}