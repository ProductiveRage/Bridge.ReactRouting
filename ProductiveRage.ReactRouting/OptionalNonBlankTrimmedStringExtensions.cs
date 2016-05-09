using System;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting
{
	public static class OptionalNonBlankTrimmedStringExtensions // TODO: Move into ProductiveRage.Immutable.Extensions NuGet packages
	{
		/// <summary>
		/// Translate an Optional NonBlankTrimmedString instance into a string - returning null if there is no value. Nulls are usually not desirable since it's
		/// difficult for the type system to describe where a null is and isn't acceptable (which is what the Optional struct is intended to help with) but, if
		/// an Optional NonBlankTrimmedString is to be used as a class name of an element then it will need to be reduced to a string instance again at some
		/// point since React elements have a string ClassName property (which may be null, meaning set no class attribute).
		/// </summary>
		public static string ToNullableString(this Optional<NonBlankTrimmedString> source)
		{
			return source.IsDefined ? source.Value.Value : null;
		}

		/// <summary>
		/// Combine two Optional NonBlankTrimmedString instances if both arguments have values, separating them with a single space. If only one argument has a
		/// value then that argument will be returned. If neither argument have a value then a missing value will be returned.
		/// </summary>
		public static Optional<NonBlankTrimmedString> Add(this Optional<NonBlankTrimmedString> source, Optional<NonBlankTrimmedString> other)
		{
			return source.Add(" ", other);
		}

		/// <summary>
		/// Combine two Optional NonBlankTrimmedString instances if both arguments have values, separating them with a specified delimiter string (which may be
		/// blank if desired). If only one argument has a value then that argument will be returned. If neither argument have a value then a missing value will
		/// be returned.
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
