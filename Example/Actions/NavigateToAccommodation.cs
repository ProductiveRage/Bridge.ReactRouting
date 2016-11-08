using ProductiveRage.Immutable;
using ProductiveRage.ReactRouting;

namespace Example.Actions
{
	public sealed class NavigateToAccommodation : INavigationDispatcherAction
	{
		public NavigateToAccommodation(Optional<NonBlankTrimmedString> segment)
		{
			Segment = segment;
		}
		public Optional<NonBlankTrimmedString> Segment { get; private set; }
	}
}