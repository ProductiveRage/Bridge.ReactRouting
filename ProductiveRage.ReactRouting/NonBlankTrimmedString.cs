using System;
using Bridge;
using Bridge.React;

namespace ProductiveRage.ReactRouting
{
	public sealed class NonBlankTrimmedString
	{
		public NonBlankTrimmedString(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				throw new ArgumentException("Null, blank or whitespace-only value specified");

			Value = value.Trim();
		}

		/// <summary>
		/// This will never be null, blank or have any leading or trailing whitespace
		/// </summary>
		public string Value { get; private set; }

		/// <summary>
		/// It's convenient to be able to pass a NonBlankTrimmedString instance as any argument
		/// that requires a string
		/// </summary>
		public static implicit operator string(NonBlankTrimmedString value)
		{
			if (value == null)
				throw new ArgumentNullException("value");
			return value.Value;
		}

		/// <summary>
		/// It's convenient to be able to pass a NonBlankTrimmedString instance as any argument
		/// that requires a ReactElement-or-string, such as for the children array of the React
		/// DOM component factories
		/// </summary>
		public static implicit operator Any<ReactElement, string>(NonBlankTrimmedString value)
		{
			if (value == null)
				throw new ArgumentNullException("value");
			return value.Value;
		}

		public override bool Equals(object o)
		{
			var otherNonBlankTrimmedString = o as NonBlankTrimmedString;
			return
			  (otherNonBlankTrimmedString != null) &&
			  (otherNonBlankTrimmedString.Value == Value);
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public override string ToString()
		{
			return Value;
		}
	}
}
