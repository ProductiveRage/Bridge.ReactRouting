using System;

namespace ProductiveRage.ReactRouting
{
	public static class TupleExtensions
	{
		public static Tuple<T1, T2> Extend<T1, T2>(this Tuple<T1> tuple, T2 value)
		{
			return Tuple.Create(tuple.Item1, value);
		}
		public static Tuple<T1, T2, T3> Extend<T1, T2, T3>(this Tuple<T1, T2> tuple, T3 value)
		{
			return Tuple.Create(tuple.Item1, tuple.Item2, value);
		}
		public static Tuple<T1, T2, T3, T4> Extend<T1, T2, T3, T4>(this Tuple<T1, T2, T3> tuple, T4 value)
		{
			return Tuple.Create(tuple.Item1, tuple.Item2, tuple.Item3, value);
		}
		public static Tuple<T1, T2, T3, T4, T5> Extend<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4> tuple, T5 value)
		{
			return Tuple.Create(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, value);
		}
		public static Tuple<T1, T2, T3, T4, T5, T6> Extend<T1, T2, T3, T4, T5, T6>(this Tuple<T1, T2, T3, T4, T5> tuple, T6 value)
		{
			return Tuple.Create(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, value);
		}
		public static Tuple<T1, T2, T3, T4, T5, T6, T7> Extend<T1, T2, T3, T4, T5, T6, T7>(this Tuple<T1, T2, T3, T4, T5, T6> tuple, T7 value)
		{
			return Tuple.Create(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, value);
		}
		public static Tuple<T1, T2, T3, T4, T5, T6, T7, T8> Extend<T1, T2, T3, T4, T5, T6, T7, T8>(this Tuple<T1, T2, T3, T4, T5, T6, T7> tuple, T8 value)
		{
			return Tuple.Create(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, value);
		}
	}
}