using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting.Tests.Support.Actions
{
	public sealed class NavigateToItem<T> : INavigationDispatcherAction, IAmImmutable
	{
		public NavigateToItem(NonBlankTrimmedString id)
		{
			this.CtorSet(_ => _.Id, id);
		}
		public NonBlankTrimmedString Id { get; private set; }
	}
}
