using System;
using Bridge;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting
{
	public static class RouteBuilderExtensions_String
	{
		[IgnoreGeneric]
		public static RouteBuilder.IBuildRoutesWithVariablesToMatch<NonBlankTrimmedString> String(this RouteBuilder source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return source.Variable(valueExtender: segment => segment, parser: segment => Optional.For(segment));
		}

		[IgnoreGeneric]
		public static RouteBuilder.IBuildRoutesWithVariablesToMatch<TValues> String<TValues>(this RouteBuilder source, Func<NonBlankTrimmedString, TValues> valueExtender)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (valueExtender == null)
				throw new ArgumentNullException("valueExtender");

			return source.Variable(valueExtender, parser: segment => Optional.For(segment));
		}

		[IgnoreGeneric]
		public static RouteBuilder.IBuildRoutesWithVariablesToMatch<TValuesExpanded> String<TValues, TValuesExpanded>(
			this RouteBuilder.IBuildRoutesWithVariablesToMatch<TValues> source,
			Func<TValues, NonBlankTrimmedString, TValuesExpanded> valueExtender)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (valueExtender == null)
				throw new ArgumentNullException("valueExtender");

			return source.Variable(valueExtender, parser: segment => Optional.For(segment));
		}
	}
}
