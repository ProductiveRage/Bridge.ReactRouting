using Bridge.React;
using Example.Navigation;
using ProductiveRage.Immutable;

namespace HostBridge.Components
{
	public sealed class AccommodationContentContainer : Component<AccommodationContentContainer.Props, AccommodationContentContainer.State>
	{
		public AccommodationContentContainer(ExampleNavigator navigator) : base(new Props(navigator)) { }

		public override ReactElement Render()
		{
			return DOM.Div(null,
				new NavigationLinks(props.Navigator, new ClassName("main-nav")),
				DOM.Div(new Attributes { ClassName = "content" }, "This is an introduction page for the Accommodation section")
			);
		}

		public sealed class Props : IAmImmutable
		{
			public Props(ExampleNavigator navigator)
			{
				this.CtorSet(_ => _.Navigator, navigator);
			}
			public ExampleNavigator Navigator { get; }
		}

		public sealed class State : IAmImmutable { }
	}
}