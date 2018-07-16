using System;
using Bridge.React;
using Example.Navigation;
using ProductiveRage.Immutable;
using ProductiveRage.ReactRouting;

namespace HostBridge.Components
{
	/// <summary>
	/// This is not a PureComponent as the Link component is not a PureComponent (as it relies upon an external history handler and so its
	/// current state is not fully described by its props - if a Links container does not need to vary its styling or class names based
	/// upon the current URL then it may be a PureComponent, which is always preferable to a StatelessComponent where it may be used)
	/// </summary>
	public sealed class NavigationLinks : StatelessComponent<NavigationLinks.Props>
	{
		public NavigationLinks(ExampleNavigator navigator, Optional<ClassName> className = new Optional<ClassName>())
			: base(new Props(navigator, className)) { }

		public override ReactElement Render()
		{
			return DOM.OL(new OListAttributes { ClassName = props.ClassName.ToNullableString() },
				DOM.Li(null, GetLink("Home", props.Navigator.Home(), className: new ClassName("home"))),
				DOM.Li(null,
					GetLink("Accommodation", props.Navigator.Accommodation()),
					DOM.OL(null,
						DOM.Li(null, GetLink("B&Bs", props.Navigator.Accommodation(new NonBlankTrimmedString("BedAndBreakfast")))),
						DOM.Li(null, GetLink("Hotels", props.Navigator.Accommodation(new NonBlankTrimmedString("Hotels")))),
						DOM.Li(null, GetLink("Self Catering", props.Navigator.Accommodation(new NonBlankTrimmedString("SelfCatering"))))
					)
				)
			);
		}

		private Link GetLink(string text, UrlPathDetails url, Optional<ClassName> className = new Optional<ClassName>())
		{
			if (string.IsNullOrWhiteSpace(text))
				throw new ArgumentException("Null/blank text specified");
			if (url == null)
				throw new ArgumentNullException("url");

			return new Link(
				url: url.ToUrlDetails(Optional<QueryString>.Missing),
				text: new NonBlankTrimmedString(text),
				caseSensitiveUrlMatching: false,
				name: Optional<NonBlankTrimmedString>.Missing,
				target: Optional<NonBlankTrimmedString>.Missing,
				className: className,
				ancestorClassName: new ClassName("ancestor"),
				selectedClassName: new ClassName("selected")
			);
		}

		public sealed class Props : IAmImmutable
		{
			public Props(ExampleNavigator navigator, Optional<ClassName> className)
			{
				this.CtorSet(_ => _.Navigator, navigator);
				this.CtorSet(_ => _.ClassName, className);
			}
			public ExampleNavigator Navigator { get; }
			public Optional<ClassName> ClassName { get; }
		}
	}
}
