using ProductiveRage.Immutable;
using ProductiveRage.Immutable.Extensions;

namespace ProductiveRage.ReactRouting
{
	/// <summary>
	/// This may contain QueryString and/or Hash content details in the future, if route matching based upon either or both are those (rather than
	/// just the path) is supported. At this time, though, this is only the individual segments of the URL path.
	/// </summary>
	public sealed class UrlDetails : IAmImmutable
	{
		public UrlDetails(Set<NonBlankTrimmedString> segments)
		{
			this.CtorSet(_ => _.Segments, segments);
		}
		public Set<NonBlankTrimmedString> Segments { get; private set; }

		public override string ToString()
		{
			return "/" + string.Join("/", Segments);
		}
	}
}
