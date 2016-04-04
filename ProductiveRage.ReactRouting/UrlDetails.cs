using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting
{
	public sealed class UrlDetails : IAmImmutable
	{
		public UrlDetails(Set<NonBlankTrimmedString> segments) // TODO: QueryString? Hash?
		{
			this.CtorSet(_ => _.Segments, segments);
		}
		public Set<NonBlankTrimmedString> Segments { get; private set; }

		public override string ToString()
		{
			return "/" + string.Join("/", Segments); // TODO: Include QueryString (and Hash, if supported) here
		}
	}
}
