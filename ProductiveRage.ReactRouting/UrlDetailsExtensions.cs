using System;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting
{
	public static class UrlDetailsExtensions
	{
		/// <summary>
		/// Add a new QueryString segment to a UrlDetails instance. The value will have ToString called on it to generate the QueryString segment value. This will throw an exception for a null source, a
		/// null-or-blank-or-whitespace-only key or a null value.
		/// </summary>
		public static UrlDetails AddToQuery(this UrlDetails source, string key, object value)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			if (string.IsNullOrWhiteSpace(key))
				throw new ArgumentException($"Null/blank {nameof(key)} specified");
			if (value == null)
				throw new ArgumentNullException(nameof(value));

			// 2019-01-08: Unless/until https://forums.bridge.net/forum/community/help/6001 is accepted as a bug and fixed, we can't rely upon the ToString implementation of
			// NonBlankTrimmedString since it became annotated with [ObjectLiteral] (for deserialisation performance improvements) - ObjectLiteralToStringSupport is required 
			return new UrlDetails(
				source.Segments,
				source.QueryString.GetValueOrDefault(new QueryString(NonNullList<QueryString.Segment>.Empty)).Add(key, ObjectLiteralToStringSupport.ToString(value))
			);
		}

		/// <summary>
		/// Add a new QueryString segment to a UrlDetails instance if the specified value is defined (if a Missing value is provided then the source UrlDetails instance will be returned unaltered).
		/// The value will have ToString called on it to generate the QueryString segment value. This will throw an exception for a null source or null-or-blank-or-whitespace-only key.
		/// </summary>
		public static UrlDetails AddToQueryIfDefined<T>(this UrlDetails source, string key, Optional<T> value)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			if (string.IsNullOrWhiteSpace(key))
				throw new ArgumentException($"Null/blank {nameof(key)} specified");

			return value.IsDefined ? AddToQuery(source, key, value.Value) : source;
		}
	}
}