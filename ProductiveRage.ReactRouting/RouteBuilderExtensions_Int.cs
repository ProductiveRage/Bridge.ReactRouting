using System;
using Bridge;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting
{
	public static class RouteBuilderExtensions_Int
	{
		[IgnoreGeneric]
		public static RouteBuilder.IBuildRoutesWithVariablesToMatch<int> Int(this RouteBuilder source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return source.Variable(valueExtender: number => number, parser: ParseInt);
		}

		[IgnoreGeneric]
		public static RouteBuilder.IBuildRoutesWithVariablesToMatch<TValues> Int<TValues>(this RouteBuilder source, Func<int, TValues> valueExtender)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (valueExtender == null)
				throw new ArgumentNullException("valueExtender");

			return source.Variable(valueExtender, ParseInt);
		}

		[IgnoreGeneric]
		public static RouteBuilder.IBuildRoutesWithVariablesToMatch<TValuesExpanded> Int<TValues, TValuesExpanded>(
			this RouteBuilder.IBuildRoutesWithVariablesToMatch<TValues> source,
			Func<TValues, int, TValuesExpanded> valueExtender)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (valueExtender == null)
				throw new ArgumentNullException("valueExtender");

			return source.Variable(valueExtender, ParseInt);
		}

		private static Optional<int> ParseInt(NonBlankTrimmedString segment)
		{
			if (segment == null)
				throw new ArgumentNullException("segment");

			int value;
			return int.TryParse(segment.Value, out value) ? value : Optional<int>.Missing;
		}
	}
}
