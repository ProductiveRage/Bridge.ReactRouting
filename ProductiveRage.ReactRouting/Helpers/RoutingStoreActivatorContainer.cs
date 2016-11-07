using System;
using Bridge.React;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting.Helpers
{
	/// <summary>
	/// This container is used to render the appropriate container component for the current URL, based upon the information in the provided NavigateActionMatcher (it waits
	/// for known navigation actions to be dispatched and then renders the corresponding store). This component should be rendered in React and is used to ensure that the
	/// correct store is shown within for the current URL.
	/// </summary>
	public sealed class RoutingStoreActivatorContainer : Component<RoutingStoreActivatorContainer.Props, RoutingStoreActivatorContainer.State>
	{
		public RoutingStoreActivatorContainer(AppDispatcher dispatcher, NavigateActionMatcher navigateActionMatcher)
			: base(new Props(dispatcher, navigateActionMatcher)) { }

		protected override State GetInitialState()
		{
			return new State(currentContainerRetriever: null);
		}

		protected override void ComponentWillMount()
		{
			props.Dispatcher.Register(message =>
			{
				var navigationAction = message.Action as INavigationDispatcherAction;
				if (navigationAction == null)
					return;

				var newContainerRetriever = props.NavigateActionMatcher.MatchAction(navigationAction);
				if (newContainerRetriever.IsDefined)
					SetState(state.With(_ => _.CurrentContainerRetriever, newContainerRetriever.Value));
			});
		}

		public override ReactElement Render()
		{
			return state.CurrentContainerRetriever.IsDefined ? state.CurrentContainerRetriever.Value() : null;
		}

		public sealed class Props : IAmImmutable
		{
			public Props(AppDispatcher dispatcher, NavigateActionMatcher navigateActionMatcher)
			{
				this.CtorSet(_ => _.Dispatcher, dispatcher);
				this.CtorSet(_ => _.NavigateActionMatcher, navigateActionMatcher);
			}
			public AppDispatcher Dispatcher { get; private set; }
			public NavigateActionMatcher NavigateActionMatcher { get; private set; }
		}

		public sealed class State : IAmImmutable
		{
			public State(Optional<Func<ReactElement>> currentContainerRetriever)
			{
				this.CtorSet(_ => _.CurrentContainerRetriever, currentContainerRetriever);
			}
			public Optional<Func<ReactElement>> CurrentContainerRetriever { get; private set; }
		}
	}
}
