using Bridge.React;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting.Tests.Support.Actions
{
	public sealed class NavigateToItem : IDispatcherAction, IAmImmutable
	{
		public NavigateToItem(NonBlankTrimmedString id)
		{
			this.CtorSet(_ => _.Id, id);
		}
		public NonBlankTrimmedString Id { get; private set; }
	}
}
