using System;
using Bridge.React;
using Example.Navigation;
using HostBridge.Stores;
using ProductiveRage.Immutable;

namespace HostBridge.Components
{
	public sealed class AccommodationListContainer : Component<AccommodationListContainer.Props, AccommodationListContainer.State>
	{
		public AccommodationListContainer(AccommodationListStore store, ExampleNavigator navigator) : base(new Props(store, navigator)) { }

		protected override State GetInitialState() => new State(props.Store.RequestedAt);
		protected override void ComponentDidMount() => props.Store.Change += StoreChanged;
		protected override void ComponentWillUnmount() => props.Store.Change -= StoreChanged;
		private void StoreChanged() => SetState(new State(props.Store.RequestedAt));

		public override ReactElement Render()
		{
			if (!state.RequestedAt.IsDefined)
				return null;

			return DOM.Div(null,
				new NavigationLinks(props.Navigator, new ClassName("main-nav")),
				DOM.Div(new Attributes { ClassName = "content" }, "This is an introduction page for the Accommodation section"),
				new NavigatedToTime(state.RequestedAt.Value)
			);
		}

		public sealed class Props : IAmImmutable
		{
			public Props(AccommodationListStore store, ExampleNavigator navigator)
			{
				this.CtorSet(_ => _.Store, store);
				this.CtorSet(_ => _.Navigator, navigator);
			}
			public AccommodationListStore Store { get; }
			public ExampleNavigator Navigator { get; }
		}

		public sealed class State : IAmImmutable
		{
			public State(Optional<DateTime> requestedAt)
			{
				this.CtorSet(_ => _.RequestedAt, requestedAt);
			}
			public Optional<DateTime> RequestedAt { get; }
		}
	}
}