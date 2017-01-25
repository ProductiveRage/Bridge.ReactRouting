using System;
using Bridge;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting
{
	public static class RouteBuilderExtensions_String_Tuples
	{
		/// <summary>
		/// Extend a route matcher (that records match data from a partial URL in a single value) by taking the next segment of the URL as a NonBlankTrimmedString and producing a route matcher
		/// that records match data in a two-element tuple (where the first item is the provided route matcher's value and the second item is a NonBlankTrimmedString)
		/// </summary>
		[IgnoreGeneric]
		public static RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, NonBlankTrimmedString>> String<T1>(this RouteBuilder.IBuildRoutesWithVariablesToMatch<T1> source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return source.String((matchSoFar, segment) => Tuple.Create(matchSoFar, segment));
		}

		/// <summary>
		/// Extend a route matcher (that records match data from a partial URL into a two-item tuple) by taking the next segment of the URL as a NonBlankTrimmedString and producing a route
		/// matcher that records match data in a three-element tuple (where the first items are from the provided route matcher's values and the last item is a NonBlankTrimmedString)
		/// </summary>
		[IgnoreGeneric]
		public static RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, T2, NonBlankTrimmedString>> String<T1, T2>(this RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, T2>> source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return source.String(TupleExtensions.Extend);
		}

		/// <summary>
		/// Extend a route matcher (that records match data from a partial URL into a three-item tuple) by taking the next segment of the URL as a NonBlankTrimmedString and producing a route
		/// matcher that records match data in a four-element tuple (where the first items are from the provided route matcher's values and the last item is a NonBlankTrimmedString)
		/// </summary>
		[IgnoreGeneric]
		public static RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, T2, T3, NonBlankTrimmedString>> String<T1, T2, T3>(this RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, T2, T3>> source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return source.String(TupleExtensions.Extend);
		}

		/// <summary>
		/// Extend a route matcher (that records match data from a partial URL into a four-item tuple) by taking the next segment of the URL as a NonBlankTrimmedString and producing a route
		/// matcher that records match data in a five-element tuple (where the first items are from the provided route matcher's values and the last item is a NonBlankTrimmedString)
		/// </summary>
		[IgnoreGeneric]
		public static RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, T2, T3, T4, NonBlankTrimmedString>> String<T1, T2, T3, T4>(this RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, T2, T3, T4>> source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return source.String(TupleExtensions.Extend);
		}

		/// <summary>
		/// Extend a route matcher (that records match data from a partial URL into a five-item tuple) by taking the next segment of the URL as a NonBlankTrimmedString and producing a route
		/// matcher that records match data in a six-element tuple (where the first items are from the provided route matcher's values and the last item is a NonBlankTrimmedString)
		/// </summary>
		[IgnoreGeneric]
		public static RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, T2, T3, T4, T5, NonBlankTrimmedString>> String<T1, T2, T3, T4, T5>(this RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, T2, T3, T4, T5>> source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return source.String(TupleExtensions.Extend);
		}

		/// <summary>
		/// Extend a route matcher (that records match data from a partial URL into a six-item tuple) by taking the next segment of the URL as a NonBlankTrimmedString and producing a route
		/// matcher that records match data in a seven-element tuple (where the first items are from the provided route matcher's values and the last item is a NonBlankTrimmedString)
		/// </summary>
		[IgnoreGeneric]
		public static RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, T2, T3, T4, T5, T6, NonBlankTrimmedString>> String<T1, T2, T3, T4, T5, T6>(this RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, T2, T3, T4, T5, T6>> source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return source.String(TupleExtensions.Extend);
		}

		/// <summary>
		/// Extend a route matcher (that records match data from a partial URL into a seven-item tuple) by taking the next segment of the URL as a NonBlankTrimmedString and producing a route
		/// matcher that records match data in an eight-element tuple (where the first items are from the provided route matcher's values and the last item is a NonBlankTrimmedString)
		/// </summary>
		[IgnoreGeneric]
		public static RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, T2, T3, T4, T5, T6, T7, NonBlankTrimmedString>> String<T1, T2, T3, T4, T5, T6, T7>(this RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, T2, T3, T4, T5, T6, T7>> source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return source.String(TupleExtensions.Extend);
		}
	}
}
