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
		private readonly Func<NonBlankTrimmedString, int, UrlPathDetails> _getItemSomething;
		public RootPlusDynamicIdItemPagesNavigator(Set<NonBlankTrimmedString> parentSegments, IDispatcher dispatcher) : base(parentSegments, dispatcher)
		{
			if (dispatcher == null)
				throw new ArgumentNullException("dispatcher");

			_getRoot = AddRelativeRoute(
				segments: Set<string>.Empty,
				routeAction: new NavigateToRoot<T>(),
				urlGenerator: () => GetPath()
			);

			_getItem = AddRelativeRoute(
				routeDetails: RouteBuilder.Empty.Fixed("item").String(),
				routeActionGenerator: matchedValue => new NavigateToItem<T>(matchedValue),
				urlGenerator: matchedValue => GetPath("item", matchedValue)
			);

			_getItemSomething = AddRelativeRoute(
				routeDetails: RouteBuilder.Empty.Fixed("item").String().Int(),
				routeActionGenerator: match => new NavigateToItem<T>(match.Item1),
				urlGenerator: (name, index) => GetPath("item", name, index)
			);
		}
		public RootPlusDynamicIdItemPagesNavigator(IDispatcher dispatcher) : this(Set<NonBlankTrimmedString>.Empty, dispatcher) { }

		public UrlPathDetails Root() { return _getRoot(); }
		public UrlPathDetails Item(NonBlankTrimmedString name) { return _getItem(name); }

		public Func<NonBlankTrimmedString, int, UrlPathDetails> ItemSomething { get; }

		private sealed class EmptyNavigateToItemSomething : IAmImmutable
		{
			public EmptyNavigateToItemSomething(Optional<NonBlankTrimmedString> name = new Optional<NonBlankTrimmedString>(), int index = 0)
			{
				this.CtorSet(_ => _.Name, name);
				this.CtorSet(_ => _.Index, index);
			}
			public Optional<NonBlankTrimmedString> Name { get; }
			public int Index { get; }
		}
	}
}
