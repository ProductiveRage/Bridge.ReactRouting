using System;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting
{
	public sealed class UrlDetails : IAmImmutable
	{
		public UrlDetails(Set<NonBlankTrimmedString> segments, Optional<NonBlankTrimmedString> queryString)
		{
			this.CtorSet(_ => _.Segments, segments);
			this.CtorSet(_ => _.QueryString, queryString);
		}

		public Set<NonBlankTrimmedString> Segments { get; private set; }
		public Optional<NonBlankTrimmedString> QueryString { get; private set; }

		public static implicit operator UrlDetails(UrlPathDetails url)
		{
			if (url == null)
				throw new ArgumentNullException("url");

			return new UrlDetails(url.Segments, Optional<NonBlankTrimmedString>.Missing);
		}

		public override string ToString()
		{
			return "/" + string.Join("/", Segments) + (QueryString.IsDefined ? ("?" + QueryString.Value) : "");
		}
	}
}
