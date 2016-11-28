using System;
using Bridge;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting
{
	public static class RouteBuilderExtensions_String
	{
		/// <summary>
		/// This will add a string variable segment to a route builder that does not yet contain any variables
		/// </summary>
		[IgnoreGeneric]
		public static RouteBuilder.IBuildRoutesWithVariablesToMatch<NonBlankTrimmedString> String(this RouteBuilder source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return source.Variable(valueExtender: segment => segment, parser: segment => Optional.For(segment));
		}

		/// <summary>
		/// This will add a string variable segment to a route builder that does not yet contain any variables, the value will be used to populate an object of type TValues rather than
		/// being a string value on the route builder
		/// </summary>
		[IgnoreGeneric]
		public static RouteBuilder.IBuildRoutesWithVariablesToMatch<TValues> String<TValues>(this RouteBuilder source, Func<NonBlankTrimmedString, TValues> valueExtender)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (valueExtender == null)
				throw new ArgumentNullException("valueExtender");

			return source.Variable(valueExtender, parser: segment => Optional.For(segment));
		}

		/// <summary>
		/// This will add a string variable segment to a route builder that already matches some variable content - a Func is required to combine the string value from the matched URL
		/// segment with the current TValues instance
		/// </summary>
		[IgnoreGeneric]
		public static RouteBuilder.IBuildRoutesWithVariablesToMatch<TValuesExpanded> String<TValues, TValuesExpanded>(
			this RouteBuilder.IBuildRoutesWithVariablesToMatch<TValues> source,
			Func<TValues, NonBlankTrimmedString, TValuesExpanded> valueExtender)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (valueExtender == null)
				throw new ArgumentNullException("valueExtender");

			return source.Variable(valueExtender, parser: segment => Optional.For(segment));
		}
	}
}
