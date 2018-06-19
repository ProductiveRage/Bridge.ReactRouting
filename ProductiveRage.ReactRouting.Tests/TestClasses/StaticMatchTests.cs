﻿using Bridge;
using Bridge.QUnit;
using ProductiveRage.ReactRouting.Tests.Support;

namespace ProductiveRage.ReactRouting.Tests.TestClasses
{
	public static class StaticMatchTests
	{
		[Ready]
		public static void Go()
		{
			QUnit.Module("Static Matches");

			QUnit.Test("Match '/'", assert =>
			{
				var routeInfo = RouteBuilder.Empty;
				var url = UrlDetailsCreator.New();
				assert.RouteMatched(routeInfo, url);
			});

			QUnit.Test("Match '/home'", assert =>
			{
				var routeInfo = RouteBuilder.Empty.Fixed("home");
				var url = UrlDetailsCreator.New("home");
				assert.RouteMatched(routeInfo, url);
			});

			QUnit.Test("Match '/home/info'", assert =>
			{
				var routeInfo = RouteBuilder.Empty.Fixed("home", "info");
				var url = UrlDetailsCreator.New("home", "info");
				assert.RouteMatched(routeInfo, url);
			});

			QUnit.Test("NotFound '/home' if only '/other' route specified", assert =>
			{
				var routeInfo = RouteBuilder.Empty.Fixed("other");
				var url = UrlDetailsCreator.New("home");
				assert.RouteNotMatched(routeInfo, url);
			});

			QUnit.Test("NotFound '/home' if only '/home/info' route specified", assert =>
			{
				var routeInfo = RouteBuilder.Empty.Fixed("home", "info");
				var url = UrlDetailsCreator.New("home");
				assert.RouteNotMatched(routeInfo, url);
			});
		}
	}
}
