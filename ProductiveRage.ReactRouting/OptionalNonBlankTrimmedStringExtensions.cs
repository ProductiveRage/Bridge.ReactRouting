using System;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting
{
	public static class OptionalNonBlankTrimmedStringExtensions // TODO: Move into ProductiveRage.Immutable.Extensions NuGet packages
	{
		/// <summary>
		/// TODO
		/// </summary>
		public static string ToNullableString(this Optional<NonBlankTrimmedString> source)
		{
			return source.IsDefined ? source.Value.Value : null;
		}

		/// <summary>
		/// TODO
		/// </summary>
		public static Optional<NonBlankTrimmedString> Add(this Optional<NonBlankTrimmedString> source, Optional<NonBlankTrimmedString> other)
		{
			return source.Add(" ", other);
		}

		/// <summary>
		/// TODO
		/// </summary>
		public static Optional<NonBlankTrimmedString> Add(this Optional<NonBlankTrimmedString> source, string delimiter, Optional<NonBlankTrimmedString> other)
		{
			if (delimiter == null)
				throw new ArgumentNullException("delimiter");

			if (!source.IsDefined && !other.IsDefined)
				return Optional<NonBlankTrimmedString>.Missing;
			else if (!source.IsDefined)
				return other;
			else if (!other.IsDefined)
				return source;

			return new NonBlankTrimmedString(source.Value.Value + delimiter + other.Value.Value);
		}
	}
}
