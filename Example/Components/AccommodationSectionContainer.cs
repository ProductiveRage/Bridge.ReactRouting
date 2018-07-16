using Bridge.React;
using Example.Navigation;
using HostBridge.Stores;
using ProductiveRage.Immutable;

namespace HostBridge.Components
{
	public sealed class AccommodationSectionContainer : Component<AccommodationSectionContainer.Props, AccommodationSectionContainer.State>
	{
		public AccommodationSectionContainer(AccommodationSectionStore store, ExampleNavigator navigator) : base(new Props(store, navigator)) { }

		protected override State GetInitialState() => new State(props.Store.RequestedAccommodationSection);
		protected override void ComponentDidMount() => props.Store.Change += StoreChanged;
		protected override void ComponentWillUnmount() => props.Store.Change -= StoreChanged;
		private void StoreChanged() => SetState(new State(props.Store.RequestedAccommodationSection));

		public override ReactElement Render()
		{
			if (!state.RequestedAccommodationSegment.IsDefined)
				return null;

			return DOM.Div(null,
				new NavigationLinks(props.Navigator, new ClassName("main-nav")),
				DOM.Div(new Attributes { ClassName = "content" }, "Accommodation: ", state.RequestedAccommodationSegment.Value.Segment),
				new NavigatedToTime(state.RequestedAccommodationSegment.Value.RequestedAt)
			);
		}

		public sealed class Props : IAmImmutable
		{
			public Props(AccommodationSectionStore store, ExampleNavigator navigator)
			{
				this.CtorSet(_ => _.Store, store);
				this.CtorSet(_ => _.Navigator, navigator);
			}
			public AccommodationSectionStore Store { get; }
			public ExampleNavigator Navigator { get; }
		}

		public sealed class State : IAmImmutable
		{
			public State(Optional<AccommodationSectionStore.RequestedAccommodationSectionDetails> requestedAccommodationSegment)
			{
				this.CtorSet(_ => _.RequestedAccommodationSegment, requestedAccommodationSegment);
			}
			public Optional<AccommodationSectionStore.RequestedAccommodationSectionDetails> RequestedAccommodationSegment { get; }
		}
	}
}