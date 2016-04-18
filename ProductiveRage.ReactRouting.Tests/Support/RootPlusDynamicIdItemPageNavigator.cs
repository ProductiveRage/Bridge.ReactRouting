using System;
using Bridge.React;
using ProductiveRage.Immutable;
using ProductiveRage.ReactRouting.Tests.Support.Actions;

namespace ProductiveRage.ReactRouting.Tests.Support
{
	public sealed class RootPlusDynamicIdItemPagesNavigator<T> : Navigator
	{
		private readonly Action _navigateToRoot;
		private readonly Action<NonBlankTrimmedString> _navigateToItem;
		public RootPlusDynamicIdItemPagesNavigator(Set<NonBlankTrimmedString> parentSegments, IInteractWithBrowserRouting historyHandler, AppDispatcher dispatcher)
			: base(parentSegments, historyHandler, dispatcher)
		{
			if (dispatcher == null)
				throw new ArgumentNullException("dispatcher");

			AddRelativeRoute(
				Set<string>.Empty,
				new NavigateToRoot<T>()
			);
			_navigateToRoot = () => NavigateTo();

			AddRelativeRoute(
				RouteBuilder.Empty.Fixed("item").String(),
				matchedValue => new NavigateToItem<T>(matchedValue.Item1)
			);
			_navigateToItem = segment => NavigateTo("item", segment.Value);
		}
		public RootPlusDynamicIdItemPagesNavigator(IInteractWithBrowserRouting historyHandler, AppDispatcher dispatcher)
			: this(Set<NonBlankTrimmedString>.Empty, historyHandler, dispatcher)
		{ }

		public void Root() { _navigateToRoot(); }
		public void Item(NonBlankTrimmedString name) { _navigateToItem(name); }
	}
}
