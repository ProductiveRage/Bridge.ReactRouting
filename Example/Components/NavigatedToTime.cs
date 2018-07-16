using System;
using Bridge.React;
using ProductiveRage.Immutable;

namespace HostBridge.Components
{
	public sealed class NavigatedToTime : PureComponent<NavigatedToTime.Props>
	{
		public NavigatedToTime(DateTime requestedAt) : base(new Props(requestedAt)) { }

		public override ReactElement Render()
		{
			return DOM.Div(new Attributes { ClassName = "requested-at" },
				"Page navigated to at ",
				props.RequestedAt.ToString("HH:mm:ss")
			);
		}

		public sealed class Props : IAmImmutable
		{
			public Props(DateTime requestedAt)
			{
				this.CtorSet(_ => _.RequestedAt, requestedAt);
			}

			public DateTime RequestedAt { get; }
		}
	}
}