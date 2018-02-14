using System;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting
{
	public static class OptionalQueryStringExtensions
	{
		/// <summary>
		/// Try to get an int value from the QueryString (if it has a value) by taking the first QueryString value for the specified key (if it exists in the QueryString) and parsing as an int - if the
		/// QueryString is not defined or if it does not contain any values for the specified key or if the first value is not parseable as an int then a Missing value will be returned
		/// </summary>
		public static Optional<int> Int(this Optional<QueryString> source, string key)
		{
			var rawValue = String(source, key);
			if (!rawValue.IsDefined)
				return Optional<int>.Missing;

			int value;
			return int.TryParse(rawValue.Value, out value) ? value : Optional<int>.Missing;
		}

		/// <summary>
		/// Try to get a single string value from the QueryString (if it has a value) by taking the first QueryString value for the specified key (if it exists in the QueryString) - if the QueryString
		/// is not defined or if it does not contain any values for the specified key then a Missing value will be returned
		/// </summary>
		public static Optional<NonBlankTrimmedString> String(this Optional<QueryString> source, string key)
		{
			if (string.IsNullOrWhiteSpace(key))
				throw new ArgumentException($"Null/blank {nameof(key)} specified");

			if (!source.IsDefined)
				return Optional<NonBlankTrimmedString>.Missing;

			var rawValues = source.Value[key];
			if (rawValues.Count == 0)
				return Optional<NonBlankTrimmedString>.Missing;
			var rawValue = rawValues[0];
			return !rawValue.IsDefined || string.IsNullOrWhiteSpace(rawValue.Value) ? Optional<NonBlankTrimmedString>.Missing : new NonBlankTrimmedString(rawValue.Value);
		}
	}
}