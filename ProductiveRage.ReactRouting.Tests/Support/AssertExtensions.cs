using Bridge;
using Bridge.QUnit;

namespace ProductiveRage.ReactRouting.Tests.Support
{
	public static class AssertExtensions
	{
		public static void RouteMatched(this Assert assert, RouteBuilder routeInfo, UrlPathDetails url)
		{
			var routeMatched = false;
			var route = routeInfo.ToRoute(() => routeMatched = true);
			route.ExecuteCallbackIfUrlMatches(url);
			assert.Ok(routeMatched);
		}

		public static void RouteNotMatched(this Assert assert, RouteBuilder routeInfo, UrlPathDetails url)
		{
			var routeMatched = false;
			var route = routeInfo.ToRoute(() => routeMatched = true);
			route.ExecuteCallbackIfUrlMatches(url);
			assert.NotOk(routeMatched);
		}

		[IgnoreGeneric]
		public static void RouteMatched<TValues>(
			this Assert assert,
			RouteBuilder.IBuildRoutesWithVariablesToMatch<TValues> routeInfo,
			UrlPathDetails url,
			TValues expectedValue) where TValues : class
		{
			var route = routeInfo.ToRoute(extractedValue => assert.DeepEqual(extractedValue, expectedValue));
			route.ExecuteCallbackIfUrlMatches(url);
		}

		[IgnoreGeneric]
		public static void RouteNotMatched<TValues>(
			this Assert assert,
			RouteBuilder.IBuildRoutesWithVariablesToMatch<TValues> routeInfo,
			UrlPathDetails url) where TValues : class
		{
			var routeWasMatched = false;
			var route = routeInfo.ToRoute(extractedValue => routeWasMatched = true);
			route.ExecuteCallbackIfUrlMatches(url);
			assert.NotOk(routeWasMatched);
		}
	}
}
