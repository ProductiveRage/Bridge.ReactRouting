using System;
using System.Linq;
using Bridge;
using Bridge.React;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting
{
	/// <summary>
	/// This will render an anchor link for the specified URL but any non-modified left clicks (ie. clicks that do not occur while Shift or Control keys are depressed)
	/// will be translated into calls to a history handler's NavigateTo method, rather than allowing the browser to following the link. If no HistoryHandler props value
	/// is provided then the static Html5HistoryRouter instance will be used. This history handler is also used to determine whether to add the optional Ancestor or
	/// Selected class names to the current link - if the current URL matches the link's URL precisely, then the Selected class name is added. If the link's URL
	/// appears to be a parent of the current URL (eg. the link URL is /home and the current URL is /home/info) then the Ancestor class name is added. As this component
	/// has to query a history handler to determine what class names to add to the anchor, it is not a PureComponent - as such, it should not be contained within a
	/// PureComponent, otherwise some updates may not be applied to it (if the Ancestor and Selected classes are not applicable, and are set to missing values, then
	/// this may be treated as a PureComponent).
	/// </summary>
	public sealed class Link : StatelessComponent<Link.Props>
	{
		public Link(
			UrlDetails url,
			NonBlankTrimmedString text,
			bool caseSensitiveUrlMatching = false,
			Optional<NonBlankTrimmedString> name = new Optional<NonBlankTrimmedString>(),
			Optional<NonBlankTrimmedString> target = new Optional<NonBlankTrimmedString>(),
			Optional<ClassName> className = new Optional<ClassName>(),
			Optional<ClassName> ancestorClassName = new Optional<ClassName>(),
			Optional<ClassName> selectedClassName = new Optional<ClassName>(),
			Optional<Action<MouseEvent<Bridge.Html5.HTMLAnchorElement>>> onClick = new Optional<Action<MouseEvent<Bridge.Html5.HTMLAnchorElement>>>(),
			Optional<IInteractWithBrowserRouting> historyHandlerOverride = new Optional<IInteractWithBrowserRouting>())
			: base(new Props(url, text, caseSensitiveUrlMatching, name, target, className, ancestorClassName, selectedClassName, onClick, historyHandlerOverride))
		{ }

		public Link(
			UrlDetails url,
			ReactElement text,
			bool caseSensitiveUrlMatching = false,
			Optional<NonBlankTrimmedString> name = new Optional<NonBlankTrimmedString>(),
			Optional<NonBlankTrimmedString> target = new Optional<NonBlankTrimmedString>(),
			Optional<ClassName> className = new Optional<ClassName>(),
			Optional<ClassName> ancestorClassName = new Optional<ClassName>(),
			Optional<ClassName> selectedClassName = new Optional<ClassName>(),
			Optional<Action<MouseEvent<Bridge.Html5.HTMLAnchorElement>>> onClick = new Optional<Action<MouseEvent<Bridge.Html5.HTMLAnchorElement>>>(),
			Optional<IInteractWithBrowserRouting> historyHandlerOverride = new Optional<IInteractWithBrowserRouting>())
			: base(new Props(url, text, caseSensitiveUrlMatching, name, target, className, ancestorClassName, selectedClassName, onClick, historyHandlerOverride))
		{ }

		public override ReactElement Render()
		{
			var historyHandler = props.HistoryHandlerOverride.GetValueOrDefault(Html5HistoryRouter.Instance);
			var currentUrl = historyHandler.CurrentLocation;
			bool isAncestor;
			if (historyHandler.CurrentLocation.Segments.Count >= props.Url.Segments.Count)
			{
				var stringComparison = props.CaseSensitiveUrlMatching ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
				isAncestor = props.Url.Segments
					.Zip(historyHandler.CurrentLocation.Segments.Take((int)props.Url.Segments.Count), (x, y) => new { CurrentUrlSegment = x, PropsUrlSegment = y })
					.All(segment => segment.CurrentUrlSegment.Value.Equals(segment.PropsUrlSegment.Value, stringComparison));
			}
			else
				isAncestor = false;
			bool isSelected;
			if (isAncestor && (historyHandler.CurrentLocation.Segments.Count == props.Url.Segments.Count))
			{
				isSelected = true;
				isAncestor = false; // Don't identify a URL as "ancestor" AND "selected", "selected" should take precedent
			}
			else
				isSelected = false;
			var className = props.ClassName;
			if (isAncestor)
				className = className.Add(props.AncestorClassName);
			if (isSelected)
				className = className.Add(props.SelectedClassName);
			return DOM.A(
				new AnchorAttributes
				{
					Name = props.Name.ToNullableString(),
					Target = props.Target.ToNullableString(),
					ClassName = className.ToNullableString(),
					Href = props.Url.ToString(),
					OnClick = e =>
					{
						if (props.OnClick.IsDefined)
							props.OnClick.Value(e);

						// ONLY intercept left-click and ONLY if there's no holding-Control-or-Shift-of-whatever, so that we don't mess with people trying to
						// open in a new tab, window, etc.. Note that the button values are reversed for left-handed mice, so button 0 is always the primary
						// click button (except on IE8 and earlier, where it's a different value, but we don't care about those browsers). For more details,
						// see http://www.w3schools.com/jsref/event_button.asp.

						// If a target has been specified and it's not "_self" (i.e. "_blank", "_parent", "_top" or "{TARGET NAME}") then we also want to allow 
						// the default browser behaviour to take over and open in a new frame.
						// For more details, see https://www.w3.org/TR/html4/types.html#type-frame-target
						var openInNewTarget = props.Target.IsDefined && !props.Target.Value.Value.Equals("_self", StringComparison.OrdinalIgnoreCase);

						if ((e.Button != 0) || e.AltKey || e.CtrlKey || e.MetaKey || e.ShiftKey || openInNewTarget)
							return;
						historyHandler.NavigateTo(props.Url);
						e.PreventDefault();
					}
				},
				props.Text
			);
		}

		public sealed class Props : IAmImmutable
		{
			public Props(
				UrlDetails url,
				Union<ReactElement, string> text,
				bool caseSensitiveUrlMatching,
				Optional<NonBlankTrimmedString> name,
				Optional<NonBlankTrimmedString> target,
				Optional<ClassName> className,
				Optional<ClassName> ancestorClassName,
				Optional<ClassName> selectedClassName,
				Optional<Action<MouseEvent<Bridge.Html5.HTMLAnchorElement>>> onClick,
				Optional<IInteractWithBrowserRouting> historyHandlerOverride)
			{
				this.CtorSet(_ => _.Url, url);
				this.CtorSet(_ => _.Text, text);
				this.CtorSet(_ => _.CaseSensitiveUrlMatching, caseSensitiveUrlMatching);
				this.CtorSet(_ => _.Name, name);
				this.CtorSet(_ => _.Target, target);
				this.CtorSet(_ => _.ClassName, className);
				this.CtorSet(_ => _.AncestorClassName, ancestorClassName);
				this.CtorSet(_ => _.SelectedClassName, selectedClassName);
				this.CtorSet(_ => _.OnClick, onClick);
				this.CtorSet(_ => _.HistoryHandlerOverride, historyHandlerOverride);
			}
			public UrlDetails Url { get; }
			public Union<ReactElement, string> Text { get; }
			public bool CaseSensitiveUrlMatching { get; }
			public Optional<NonBlankTrimmedString> Name { get; }
			public Optional<NonBlankTrimmedString> Target { get; }
			public Optional<ClassName> ClassName { get; }
			public Optional<ClassName> AncestorClassName { get; }
			public Optional<ClassName> SelectedClassName { get; }
			public Optional<Action<MouseEvent<Bridge.Html5.HTMLAnchorElement>>> OnClick { get; }
			public Optional<IInteractWithBrowserRouting> HistoryHandlerOverride { get; }
		}
	}
}