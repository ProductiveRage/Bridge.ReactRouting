using Bridge;
using Bridge.QUnit;
using ProductiveRage.Immutable;
using ProductiveRage.ReactRouting.Tests.Support;

namespace ProductiveRage.ReactRouting.Tests.TestClasses
{
	public static class QueryStringTests
	{
		[Ready]
		public static void Go()
		{
			QUnit.Module("QueryString Parsing");

			QUnit.Test("Match ''", assert =>
			{
				var rawQueryString = "";
				var queryString = QueryString.Parse(rawQueryString);
				assert.Equal(queryString["x"].Count, 0);
				assert.Equal(queryString.ToString(), rawQueryString);
			});

			QUnit.Test("Match 'x=1'", assert =>
			{
				var rawQueryString = "x=1";
				var queryString = QueryString.Parse(rawQueryString);
				assert.Equal(queryString["x"].Count, 1);
				assert.OptionalEqual(queryString["x"][0], "1");
				assert.Equal(queryString.ToString(), rawQueryString);
			});

			QUnit.Test("Match '?x=1'", assert =>
			{
				assert.Throws(() => QueryString.Parse("?x=1"), "The QueryString should not include the leading '?' character");
			});

			QUnit.Test("Match 'x=1&x=2'", assert =>
			{
				var rawQueryString = "x=1&x=2";
				var queryString = QueryString.Parse(rawQueryString);
				assert.Equal(queryString["x"].Count, 2);
				assert.OptionalEqual(queryString["x"][0], "1");
				assert.OptionalEqual(queryString["x"][1], "2");
				assert.Equal(queryString.ToString(), rawQueryString);
			});

			QUnit.Test("Match 'x=1&y=&z'", assert =>
			{
				var rawQueryString = "x=1&y=&z";
				var queryString = QueryString.Parse(rawQueryString);
				assert.Equal(queryString["x"].Count, 1);
				assert.OptionalEqual(queryString["x"][0], "1");
				assert.Equal(queryString["y"].Count, 1);
				assert.OptionalEqual(queryString["y"][0], "");
				assert.Equal(queryString["z"].Count, 1);
				assert.OptionalEqual(queryString["z"][0], Optional<string>.Missing);
				assert.Equal(queryString.ToString(), rawQueryString);
			});

			QUnit.Test("Match 'x=test%20value'", assert =>
			{
				var rawQueryString = "x=test%20value";
				var queryString = QueryString.Parse(rawQueryString);
				assert.Equal(queryString["x"].Count, 1);
				assert.OptionalEqual(queryString["x"][0], "test value");
				assert.Equal(queryString.ToString(), rawQueryString);
			});

			QUnit.Test("Match '%20%20x=test%20value'", assert =>
			{
				var rawQueryString = "%20%20x=test%20value";
				var queryString = QueryString.Parse(rawQueryString);
				assert.Equal(queryString["x"].Count, 0);
				assert.Equal(queryString["  x"].Count, 1);
				assert.OptionalEqual(queryString["  x"][0], "test value");
				assert.Equal(queryString.ToString(), rawQueryString);
			});

			QUnit.Module("QueryString Manipulating");

			QUnit.Test("Add 'x=2' to 'x=1'", assert =>
			{
				var queryString = QueryString.Parse("x=1").Add("x", "2");
				assert.Equal(queryString["x"].Count, 2);
				assert.OptionalEqual(queryString["x"][0], "1");
				assert.OptionalEqual(queryString["x"][1], "2");
				assert.Equal(queryString.ToString(), "x=1&x=2");
			});

			QUnit.Test("Remove 'x' from 'x=1&x=2'", assert =>
			{
				var queryString = QueryString.Parse("x=1&x=2").RemoveIfPresent("x");
				assert.Equal(queryString["x"].Count, 0);
				assert.Equal(queryString.ToString(), "");
			});
		}
	}
}
