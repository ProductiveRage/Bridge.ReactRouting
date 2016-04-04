using System;
using System.Collections.Generic;
using System.Linq;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting
{
	public static class IEnumerableExtensions
	{
		public static Set<T> ToSet<T>(this IEnumerable<T> values)
		{
			if (values == null)
				throw new ArgumentNullException("values");

			return Set.Of(values.ToArray());
		}
	}
}
