using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting
{
	/// <summary>
	/// This content describes the current URL path, it does not contain any QueryString or Fragment Identifier content. Route matching in this library
	/// is built around this concept (if HTML5 routing is used then the path corresponds to the current browser location but it may alternatively be
	/// encoded in the Fragment Identifier if HTMl5 is not available and an IInteractWithBrowserRouting implementation built around the Fragment
	/// Identified is used)
	/// </summary>
	public sealed class UrlPathDetails : IAmImmutable
	{
		public UrlPathDetails(Set<NonBlankTrimmedString> segments)
		{
			this.CtorSet(_ => _.Segments, segments);
		}
		public Set<NonBlankTrimmedString> Segments { get; private set; }

		public UrlDetails ToUrlDetails(Optional<NonBlankTrimmedString> queryString)
		{
			return new UrlDetails(Segments, queryString);
		}

		public override string ToString()
		{
			return "/" + string.Join("/", Segments);
		}
	}
}
