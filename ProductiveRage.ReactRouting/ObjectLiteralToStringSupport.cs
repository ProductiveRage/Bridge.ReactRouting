using System;
using Bridge;

namespace ProductiveRage.ReactRouting
{
	internal static class ObjectLiteralToStringSupport
	{
		/// <summary>
		/// Relying upon ToString() on [ObjectLiteral] types does not work (hopefully the Bridge Team will accept and fix https://forums.bridge.net/forum/community/help/6001)
		/// and so the helper methods in this library that take object references and change them into strings will encounter problems if those object references are for types
		/// that have custom ToString methods that must be called as part of this process. This method 
		/// </summary>
		public static string ToString(object value)
		{
			if (value == null)
				throw new ArgumentNullException(nameof(value));

			// Note: Only apply the special processing to [ObjectLiteral] types to try to minimise the change that something nasty might break in an unforeseen manner
			// (whether today or in some future version of Bridge)
			var type = value.GetType();
			var identifiedAsObjectLiteral = Script.Write<bool>("{0}.$literal === true", type);
			if (identifiedAsObjectLiteral)
			{
				// If it's an [ObjectLiteral] then see if it has a custom "toString" method - all Bridge types (whether [ObjectLiteral] or not) will have a "toString"
				// method on their prototype because they're all objects and the browser will provide a default implementation if not overridden, but we want to ignore
				// that default implementation if that is all that is present. The easiest way to determine whether it's the default implementation is to compare it to
				// the "toString" method on the Object prototype. We don't need to worry about this method having a different method signature (which would be possible
				// if the C# code had the "new" keyword against the method) because the Bridge compiler would give that method a different name to ensure that it didn't
				// clash with the "toString" method that all types have.
				var toStringMethod = Script.Write<object>("{0}.prototype.toString", type);
				if (toStringMethod != Script.Write<object>("Object.prototype.toString"))
					return Script.Write<string>("{0}.apply({1})", toStringMethod, value);
			}
			return value.ToString();
		}
	}
}