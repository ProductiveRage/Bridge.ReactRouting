using Bridge;
using Bridge.QUnit;
using ProductiveRage.Immutable;

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
		public delegate bool ResultComparer<TValues>(TValues actual, TValues expected);

		[IgnoreGeneric]
		public static void RouteMatched<TValues>(
			this Assert assert,
			RouteBuilder.IBuildRoutesWithVariablesToMatch<TValues> routeInfo,
			UrlPathDetails url,
			TValues expectedValue,
			ResultComparer<TValues> comparer)
		{
			var routeMatched = false;
			var route = routeInfo.ToRoute(extractedValue => routeMatched = comparer(extractedValue, expectedValue));
			route.ExecuteCallbackIfUrlMatches(url);
			assert.Ok(routeMatched);
		}

		[IgnoreGeneric]
		public static void RouteNotMatched<TValues>(
			this Assert assert,
			RouteBuilder.IBuildRoutesWithVariablesToMatch<TValues> routeInfo,
			UrlPathDetails url)
		{
			var routeWasMatched = false;
			var route = routeInfo.ToRoute(extractedValue => routeWasMatched = true);
			route.ExecuteCallbackIfUrlMatches(url);
			assert.NotOk(routeWasMatched);
		}

		/// <summary>
		/// Unfortunately, naming this Equal will not get it used when assert.Equal is called - for some reason, the Equals(object, object) method
		/// on the Assert base class is considered to be more specified
		/// </summary>
		public static void OptionalEqual<T>(this Assert assert, Optional<T> actual, Optional<T> expected)
		{
			assert.Equal(actual.IsDefined, expected.IsDefined);
			assert.Equal(actual.Value, expected.Value);
		}
	}
}
