using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting
{
	public interface IMatchRoutes
	{
		bool ExecuteCallbackIfUrlMatches(UrlDetails url);
		IMatchRoutes MakeRelativeTo(NonNullList<NonBlankTrimmedString> parentSegments);
	}
}
