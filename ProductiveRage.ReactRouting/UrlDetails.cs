using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting
{
	public sealed class UrlDetails : IAmImmutable
	{
		public UrlDetails(NonNullList<NonBlankTrimmedString> segments, Optional<QueryString> queryString = new Optional<QueryString>())
		{
			this.CtorSet(_ => _.Segments, segments);
			this.CtorSet(_ => _.QueryString, queryString);
		}

		public NonNullList<NonBlankTrimmedString> Segments { get; private set; }
		public Optional<QueryString> QueryString { get; private set; }

		public UrlPathDetails ToUrlPathDetails()
		{
			return new UrlPathDetails(Segments);
		}

		public override string ToString()
		{
			return "/" + string.Join("/", Segments) + (QueryString.IsDefined ? ("?" + QueryString.ToString()) : "");
		}
	}
}
