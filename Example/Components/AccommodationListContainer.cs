using Bridge.React;
using Example.Navigation;
using ProductiveRage.Immutable;

namespace HostBridge.Components
{
	public sealed class AccommodationListContainer : Component<AccommodationListContainer.Props, AccommodationListContainer.State>
	{
		public AccommodationListContainer(ExampleNavigator navigator, NonBlankTrimmedString id) : base(new Props(navigator, id)) { }

		public override ReactElement Render()
		{
			return DOM.Div(null,
				new NavigationLinks(props.Navigator, new ClassName("main-nav")),
				DOM.Div(new Attributes { ClassName = "content" }, "Accommodation: ", props.Id)
			);
		}

		public sealed class Props : IAmImmutable
		{
			public Props(ExampleNavigator navigator, NonBlankTrimmedString id)
			{
				this.CtorSet(_ => _.Navigator, navigator);
				this.CtorSet(_ => _.Id, id);
			}
			public ExampleNavigator Navigator { get; private set; }
			public NonBlankTrimmedString Id { get; private set; }
		}

		public sealed class State : IAmImmutable { }
	}
}