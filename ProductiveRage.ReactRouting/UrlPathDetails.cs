using System.Linq;
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
		public UrlPathDetails(NonNullList<NonBlankTrimmedString> segments)
		{
			this.CtorSet(_ => _.Segments, segments);
		}
		public NonNullList<NonBlankTrimmedString> Segments { get; }

		public UrlDetails ToUrlDetails(Optional<QueryString> queryString)
		{
			return new UrlDetails(Segments, queryString);
		}

		// It's probably taking a little bit of a liberty allowing a UrlPathDetails to be accepted anywhere that a UrlDetails is required but I'm
		// making changes (Jan 2017) to IMatchRoutes such that ExecuteCallbackIfUrlMatches now gets a UrlDetails where previously it accepted a
		// UrlPathDetails and having implicit conversions should mean that older code (that expects just UrlPathDetails) continues to work.
		public static implicit operator UrlDetails(UrlPathDetails urlPath)
		{
			return urlPath?.ToUrlDetails(Optional<QueryString>.Missing);
		}
		public static implicit operator UrlPathDetails(UrlDetails url)
		{
			return url?.ToUrlPathDetails();
		}

		public override string ToString()
		{
			return "/" + string.Join("/", Segments.Select(s => s.Value));
		}
	}
}
