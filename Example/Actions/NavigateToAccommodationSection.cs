using ProductiveRage.Immutable;
using ProductiveRage.ReactRouting;

namespace Example.Actions
{
	public sealed class NavigateToAccommodationSection : INavigationDispatcherAction
	{
		public NavigateToAccommodationSection(NonBlankTrimmedString segment)
		{
			Segment = segment;
		}

		public NonBlankTrimmedString Segment { get; private set; }
	}
}