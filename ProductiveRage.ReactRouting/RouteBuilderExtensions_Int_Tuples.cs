using System;
using Bridge;

namespace ProductiveRage.ReactRouting
{
	// TODO: Most of these methods could be simplified down to just "return source.Int(TupleExtensions.Extend);" but there is a Bridge bug that needs fixing that is currently preventing this
	// (see http://forums.bridge.net/forum/bridge-net-pro/bugs/3096)
	public static class RouteBuilderExtensions_Int_Tuples
	{
		/// <summary>
		/// Extend a route matcher (that records match data from a partial URL in a single value) by trying to match the next segment of the URL as an integer and producing a route matcher
		/// that records match data in a two-element tuple (where the first item is the provided route matcher's value and the second item is an integer)
		/// </summary>
		[IgnoreGeneric]
		public static RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, int>> Int<T1>(this RouteBuilder.IBuildRoutesWithVariablesToMatch<T1> source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return source.Int((matchSoFar, intValue) => Tuple.Create(matchSoFar, intValue));
		}

		/// <summary>
		/// Extend a route matcher (that records match data from a partial URL into a two-item tuple) by trying to match the next segment of the URL as an integer and producing a route
		/// matcher that records match data in a three-element tuple (where the first items are from the provided route matcher's values and the last item is an integer)
		/// </summary>
		[IgnoreGeneric]
		public static RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, T2, int>> Int<T1, T2>(this RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, T2>> source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return source.Int((matchSoFar, intValue) => TupleExtensions.Extend(matchSoFar, intValue));
		}

		/// <summary>
		/// Extend a route matcher (that records match data from a partial URL into a three-item tuple) by trying to match the next segment of the URL as an integer and producing a route
		/// matcher that records match data in a four-element tuple (where the first items are from the provided route matcher's values and the last item is an integer)
		/// </summary>
		[IgnoreGeneric]
		public static RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, T2, T3, int>> Int<T1, T2, T3>(this RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, T2, T3>> source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return source.Int((matchSoFar, intValue) => TupleExtensions.Extend(matchSoFar, intValue));
		}

		/// <summary>
		/// Extend a route matcher (that records match data from a partial URL into a four-item tuple) by trying to match the next segment of the URL as an integer and producing a route
		/// matcher that records match data in a five-element tuple (where the first items are from the provided route matcher's values and the last item is an integer)
		/// </summary>
		[IgnoreGeneric]
		public static RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, T2, T3, T4, int>> Int<T1, T2, T3, T4>(this RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, T2, T3, T4>> source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return source.Int((matchSoFar, intValue) => TupleExtensions.Extend(matchSoFar, intValue));
		}

		/// <summary>
		/// Extend a route matcher (that records match data from a partial URL into a five-item tuple) by trying to match the next segment of the URL as an integer and producing a route
		/// matcher that records match data in a six-element tuple (where the first items are from the provided route matcher's values and the last item is an integer)
		/// </summary>
		[IgnoreGeneric]
		public static RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, T2, T3, T4, T5, int>> Int<T1, T2, T3, T4, T5>(this RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, T2, T3, T4, T5>> source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return source.Int((matchSoFar, intValue) => TupleExtensions.Extend(matchSoFar, intValue));
		}

		/// <summary>
		/// Extend a route matcher (that records match data from a partial URL into a six-item tuple) by trying to match the next segment of the URL as an integer and producing a route
		/// matcher that records match data in a seven-element tuple (where the first items are from the provided route matcher's values and the last item is an integer)
		/// </summary>
		[IgnoreGeneric]
		public static RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, T2, T3, T4, T5, T6, int>> Int<T1, T2, T3, T4, T5, T6>(this RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, T2, T3, T4, T5, T6>> source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return source.Int((matchSoFar, intValue) => TupleExtensions.Extend(matchSoFar, intValue));
		}

		/// <summary>
		/// Extend a route matcher (that records match data from a partial URL into a seven-item tuple) by trying to match the next segment of the URL as an integer and producing a route
		/// matcher that records match data in an eight-element tuple (where the first items are from the provided route matcher's values and the last item is an integer)
		/// </summary>
		[IgnoreGeneric]
		public static RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, T2, T3, T4, T5, T6, T7, int>> Int<T1, T2, T3, T4, T5, T6, T7>(this RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, T2, T3, T4, T5, T6, T7>> source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return source.Int((matchSoFar, intValue) => TupleExtensions.Extend(matchSoFar, intValue));
		}
	}
}
