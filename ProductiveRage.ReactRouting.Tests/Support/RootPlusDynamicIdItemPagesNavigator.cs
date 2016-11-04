using System;
using Bridge.React;
using ProductiveRage.Immutable;
using ProductiveRage.ReactRouting.Tests.Support.Actions;

namespace ProductiveRage.ReactRouting.Tests.Support
{
	public sealed class RootPlusDynamicIdItemPagesNavigator<T> : Navigator
	{
		private readonly Func<UrlPathDetails> _getRoot;
		private readonly Func<NonBlankTrimmedString, UrlPathDetails> _getItem;
		public RootPlusDynamicIdItemPagesNavigator(Set<NonBlankTrimmedString> parentSegments, AppDispatcher dispatcher) : base(parentSegments, dispatcher)
		{
			if (dispatcher == null)
				throw new ArgumentNullException("dispatcher");

			AddRelativeRoute(
				Set<string>.Empty,
				new NavigateToRoot<T>()
			);
			_getRoot = () => GetPath();

			AddRelativeRoute(
				RouteBuilder.Empty.Fixed("item").String(),
				matchedValue => new NavigateToItem<T>(matchedValue.Item1)
			);
			_getItem = segment => GetPath("item", segment.Value);
		}
		public RootPlusDynamicIdItemPagesNavigator(AppDispatcher dispatcher) : this(Set<NonBlankTrimmedString>.Empty, dispatcher) { }

		public UrlPathDetails Root() { return _getRoot(); }
		public UrlPathDetails Item(NonBlankTrimmedString name) { return _getItem(name); }
	}
}
