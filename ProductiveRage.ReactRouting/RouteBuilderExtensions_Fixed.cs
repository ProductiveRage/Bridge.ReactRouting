using System;
using System.Collections.Generic;
using System.Linq;
using Bridge;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting
{
	public static class RouteBuilderExtensions_Fixed
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

			var validatedSegments = NonNullList<NonBlankTrimmedString>.Empty;
			foreach (var segment in ((IEnumerable<string>)segments).Reverse())
			{
				if (string.IsNullOrWhiteSpace(segment))
					throw new ArgumentException("Null/blank/whitespace-only value encountered within segments set");
				validatedSegments = validatedSegments.Insert(new NonBlankTrimmedString(segment));
			}
			return source.Fixed(validatedSegments);
		}

		[IgnoreGeneric]
		public static RouteBuilder.IBuildRoutesWithVariablesToMatch<TValues> Fixed<TValues>(this RouteBuilder.IBuildRoutesWithVariablesToMatch<TValues> source, string segment)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (string.IsNullOrWhiteSpace(segment))
				throw new ArgumentException("Null/blank/whitespace-only segment specified");

			return source.Fixed(new NonBlankTrimmedString(segment));
		}
	}
}
