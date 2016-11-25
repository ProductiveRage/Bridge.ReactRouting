using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting
{
	public interface IMatchRoutes
	{
		bool ExecuteCallbackIfUrlMatches(UrlPathDetails url);
		IMatchRoutes MakeRelativeTo(NonNullList<NonBlankTrimmedString> parentSegments);
	}
}
