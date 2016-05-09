using System;
using Bridge.Html5;
using Bridge.QUnit;
using ProductiveRage.Immutable.Extensions;
using ProductiveRage.ReactRouting.Tests.Support;

namespace ProductiveRage.ReactRouting.Tests.TestClasses
{
	public static class VariableMatchTests
	{
		[Ready]
		public static void Go()
		{
			QUnit.Module("Variable Matches");

			QUnit.Test("Match '/home/info' for route '/home/{name:String}'", assert =>
			{
				var routeInfo = RouteBuilder.Empty
					.Fixed("home")
					.String(segment => new { Name = segment });
				var url = UrlDetailsCreator.New("home", "info");
				assert.RouteMatched(routeInfo, url, new { Name = new NonBlankTrimmedString("info") });
			});

			QUnit.Test("Match '/home/123' for route '/home/{key:Int}'", assert =>
			{
				var routeInfo = RouteBuilder.Empty
					.Fixed("home")
					.Int(value => new { Key = value });
				var url = UrlDetailsCreator.New("home", "123");
				assert.RouteMatched(routeInfo, url, new { Key = 123 });
			});

			QUnit.Test("NotFound '/home/info' if only '/home/{key:Int}' route specified", assert =>
			{
				var routeInfo = RouteBuilder.Empty
					.Fixed("home")
					.Int(value => new { Key = value });
				var url = UrlDetailsCreator.New("home", "info");
				assert.RouteNotMatched(routeInfo, url);
			});

			QUnit.Test("Match '/product/toy/123' for route '/product/{type:string}/{key:Int}'", assert =>
			{
				var routeInfo = RouteBuilder.Empty
					.Fixed("product")
					.String(type => new { Type = type })
					.Int((dataSoFar, key) => new { Type = dataSoFar.Type, Key = key });
				var url = UrlDetailsCreator.New("product", "toy", "123");
				assert.RouteMatched(routeInfo, url, new { Type = new NonBlankTrimmedString("toy"), Key = 123 });
			});

			QUnit.Test("Match '/product/toy/123' for route '/product/{type:string}/{key:Int}' using Tuples", assert =>
			{
				var routeInfo = RouteBuilder.Empty
					.Fixed("product")
					.String()
					.Int((valuesSoFar, newValue) => valuesSoFar.Extend(newValue));
				var url = UrlDetailsCreator.New("product", "toy", "123");
				assert.RouteMatched(routeInfo, url, Tuple.Create(new NonBlankTrimmedString("toy"), 123));
			});
		}
	}
}
