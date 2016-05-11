using System;
using System.Collections.Generic;
using System.Linq;
using Bridge;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting
{
	public static class RouteBuilderExtensions
	{
		public static RouteBuilder Fixed(this RouteBuilder source, string segment)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (string.IsNullOrWhiteSpace(segment))
				throw new ArgumentException("Null/blank/whitespace-only segment specified");

			return source.Fixed(new NonBlankTrimmedString(segment));
		}

		public static RouteBuilder Fixed(this RouteBuilder source, params string[] segments)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (segments == null)
				throw new ArgumentNullException("segments");

			var validatedSegments = Set<NonBlankTrimmedString>.Empty;
			foreach (var segment in ((IEnumerable<string>)segments).Reverse())
			{
				if (string.IsNullOrWhiteSpace(segment))
					throw new ArgumentException("Null/blank/whitespace-only value encountered within segments set");
				validatedSegments = validatedSegments.Insert(new NonBlankTrimmedString(segment));
			}
			return source.Fixed(validatedSegments);
		}

		[IgnoreGeneric]
		public static RouteBuilder.IBuildRoutesWithVariablesToMatch<TValues> Fixed<TValues>(
			this RouteBuilder.IBuildRoutesWithVariablesToMatch<TValues> source,
			string segment) where TValues : class
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (string.IsNullOrWhiteSpace(segment))
				throw new ArgumentException("Null/blank/whitespace-only segment specified");

			return source.Fixed(new NonBlankTrimmedString(segment));
		}

		[IgnoreGeneric]
		public static RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<NonBlankTrimmedString>> String(this RouteBuilder source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return source.String(segment => Tuple.Create(segment));
		}

		[IgnoreGeneric]
		public static RouteBuilder.IBuildRoutesWithVariablesToMatch<TValues> String<TValues>(
			this RouteBuilder source,
			Func<NonBlankTrimmedString, TValues> valueExtender) where TValues : class
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (valueExtender == null)
				throw new ArgumentNullException("valueExtender");

			return source.Variable(valueExtender, segment => Optional.For(segment));
		}

		[IgnoreGeneric]
		public static RouteBuilder.IBuildRoutesWithVariablesToMatch<TValuesExpanded> String<TValues, TValuesExpanded>(
			this RouteBuilder.IBuildRoutesWithVariablesToMatch<TValues> source,
			Func<TValues, NonBlankTrimmedString, TValuesExpanded> valueExtender) where TValues : class where TValuesExpanded : class
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (valueExtender == null)
				throw new ArgumentNullException("valueExtender");

			return source.Variable(valueExtender, segment => Optional.For(segment));
		}

		[IgnoreGeneric]
		public static RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<int>> Int(this RouteBuilder source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return source.Int(segment => Tuple.Create(segment));
		}

		[IgnoreGeneric]
		public static RouteBuilder.IBuildRoutesWithVariablesToMatch<TValues> Int<TValues>(
			this RouteBuilder source,
			Func<int, TValues> valueExtender) where TValues : class
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
			Func<TValues, int, TValuesExpanded> valueExtender) where TValues : class where TValuesExpanded : class
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
