using System;
using Bridge.React;
using ProductiveRage.Immutable;
using ProductiveRage.ReactRouting.Tests.Support.Actions;

namespace ProductiveRage.ReactRouting.Tests.Support
{
	public sealed class RootPlusDynamicIdItemPageNavigator : Navigator
	{
		private readonly Action _navigateToRoot;
		private readonly Action<NonBlankTrimmedString> _navigateToItem;
		public RootPlusDynamicIdItemPageNavigator(IInteractWithBrowserRouting historyHandler, AppDispatcher dispatcher)
			: base(historyHandler, dispatcher)
		{
			if (dispatcher == null)
				throw new ArgumentNullException("dispatcher");

			AddRelativeRoute(
				Set<string>.Empty,
				new NavigateToRoot()
			);
			_navigateToRoot = () => NavigateTo();

			AddRelativeRoute(
				RouteBuilder.Empty.Fixed("item").String(),
				matchedValue => new NavigateToItem(matchedValue.Item1)
			);
			_navigateToItem = segment => NavigateTo("item", segment.Value);
		}

		public void Root() { _navigateToRoot(); }
		public void Item(NonBlankTrimmedString name) { _navigateToItem(name); }
	}
}
