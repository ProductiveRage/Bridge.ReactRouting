using System;
using Bridge;

namespace ProductiveRage.ReactRouting
{
	public static class RouteBuilderExtensions
	{
		/// <summary>
		/// This method overload may be used if the QueryString of the URL that gets matched is not important
		/// </summary>
		public static IMatchRoutes ToRoute(this RouteBuilder source, Action ifMatched)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			if (ifMatched == null)
				throw new ArgumentNullException("ifMatched");

			return source.ToRoute(ifMatched: queryString => ifMatched());
		}

		/// <summary>
		/// This method overload may be used if the QueryString of the URL that gets matched is not important
		/// </summary>
		[IgnoreGeneric]
		public static IMatchRoutes ToRoute<TValues>(this RouteBuilder.IBuildRoutesWithVariablesToMatch<TValues> source, Action<TValues> ifMatched)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			if (ifMatched == null)
				throw new ArgumentNullException("ifMatched");

			return source.ToRoute(ifMatched: (values, queryString) => ifMatched(values));
		}
	}
}
