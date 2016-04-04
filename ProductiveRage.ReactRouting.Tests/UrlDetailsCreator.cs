using System;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting.Tests
{
	public static class UrlDetailsCreator
	{
		public static UrlDetails New(params string[] segments)
		{
			if (segments == null)
				throw new ArgumentNullException("segments");

			var nonBlankTrimmedStrings = Set<NonBlankTrimmedString>.Empty;
			foreach (var value in segments)
			{
				if (string.IsNullOrWhiteSpace(value))
					throw new ArgumentException("Null/blank/whitespace-only value specified in values set");
				nonBlankTrimmedStrings = nonBlankTrimmedStrings.Add(new NonBlankTrimmedString(value));
			}
			return new UrlDetails(nonBlankTrimmedStrings);
		}
	}
}