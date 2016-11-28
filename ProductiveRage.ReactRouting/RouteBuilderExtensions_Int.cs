using System;
using Bridge;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting
{
	public static class RouteBuilderExtensions_Int
	{
		/// <summary>
		/// This will add an integer variable segment to a route builder that does not yet contain any variables
		/// </summary>
		[IgnoreGeneric]
		public static RouteBuilder.IBuildRoutesWithVariablesToMatch<int> Int(this RouteBuilder source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return source.Variable(valueExtender: number => number, parser: ParseInt);
		}

		/// <summary>
		/// This will add an integer variable segment to a route builder that does not yet contain any variables, the value will be used to populate an object of type TValues rather than
		/// being an int value on the route builder
		/// </summary>
		[IgnoreGeneric]
		public static RouteBuilder.IBuildRoutesWithVariablesToMatch<TValues> Int<TValues>(this RouteBuilder source, Func<int, TValues> valueExtender)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (valueExtender == null)
				throw new ArgumentNullException("valueExtender");

			return source.Variable(valueExtender, ParseInt);
		}

		/// <summary>
		/// This will add an integer variable segment to a route builder that already matches some variable content - a Func is required to combine the integer value from the matched URL
		/// segment with the current TValues instance
		/// </summary>
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
