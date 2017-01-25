using System;
using Bridge.Html5;
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
		public RoutingStoreActivatorContainer(IDispatcher dispatcher, NavigateActionMatcher navigateActionMatcher, bool redispatchNavigationMessagesAfterStateAsynchronouslyUpdated = true)
			: base(new RoutingStoreActivatorContainer.Props(dispatcher, navigateActionMatcher, redispatchNavigationMessagesAfterStateAsynchronouslyUpdated)) { }

		protected override State GetInitialState()
		{
			return State.Empty;
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
				{
					var container = newContainerRetriever.Value();
					var newState = state.WithCurrentContainer(container);
					if (newState == state)
						return; // Do nothing if there is no change to the current container component

					SetState(
						newState,
						() =>
						{
							// When a navigation action occurs that results in the current container being changed, we may have a problem due to React's async state-updating
							// nature. When container components are mounted/unmounted they will subscribe/unsubscribe to change events in relevant stores - if one of the stores
							// receives this navigation action and uses it to retrieve some initial data and then fires its Change event, the container component that would be
							// responsible for receiving that Change event and rendering the data may not currently be listening because it may have been unmounted, even though
							// the navigation action that will result in its mounting has been dispatched (because the component-mounting is performed asynchronously and may be
							// delayed sufficiently that the store has done its data retrieval before the container subscribes to its Change event). One way to workaround this
							// is to allow this component to re-issue the navigation message after the container has been updated - doing that here means that there is only
							// one place that hack is required. One alternative would be for the container component to initiate work after it has been mounted; the container
							// could issue a subsequent dispatcher message (which would also be a hack because one action - the navigation action in this case - should not
							// directly result in another dispatcher message being issued). Alternatively, the container could have a way to query the store and request that
							// it update its data to the latest (which would be hack because the stores should instruct the containers when data has changed, rather than
							// containers poking stores to update their content). So it seems like the least of all evils is to re-issue the navigation message here (which
							// is also not ideal because it means that the handling of one dispatcher message is resulting in another dispatch-message call and it might
							// seem odd to anyone watching the queue; "why are these messages appearing twice?") - it's optional, though, so if the consumer disagrees
							// then they can set redispatchNavigationMessagesAfterStateAsynchronouslyUpdated to false.
							if (props.RedispatchNavigationMessagesAfterStateAsynchronouslyUpdated)
							{
								// In some cases, SetState will be performed SYNCHRONOUSLY and so we need to re-dispatch the message using a SetTimeout call to ensure that
								// the Dispatcher doesn't realise that the handler for a message is also dispatching a message
								Window.SetTimeout(() =>
								{
									if (message.Source == MessageSourceOptions.Server)
										props.Dispatcher.HandleServerAction(message.Action);
									else
										props.Dispatcher.HandleViewAction(message.Action);
								});
							}
						}
					);
				}
			});
		}

		public override ReactElement Render()
		{
			return state.CurrentContainerRetrieverIfAny;
		}
		public sealed class Props : IAmImmutable
		{
			public Props(IDispatcher dispatcher, NavigateActionMatcher navigateActionMatcher, bool redispatchNavigationMessagesAfterStateAsynchronouslyUpdated)
			{
				this.CtorSet(_ => _.Dispatcher, dispatcher);
				this.CtorSet(_ => _.NavigateActionMatcher, navigateActionMatcher);
				this.CtorSet(_ => _.RedispatchNavigationMessagesAfterStateAsynchronouslyUpdated, redispatchNavigationMessagesAfterStateAsynchronouslyUpdated);
			}
			public IDispatcher Dispatcher { get; }
			public NavigateActionMatcher NavigateActionMatcher { get; }
			public bool RedispatchNavigationMessagesAfterStateAsynchronouslyUpdated { get; }
		}

		// Note: Can't use Optional<ReactElement> because ReactElement is [External], which means that it can't be used with Optional<>
		public sealed class State
		{
			public static State Empty { get; } = new State(null);
			private State(ReactElement currentContainerRetrieverIfAny)
			{
				CurrentContainerRetrieverIfAny = currentContainerRetrieverIfAny;
			}
			public ReactElement CurrentContainerRetrieverIfAny { get; }
			public State WithCurrentContainer(ReactElement container)
			{
				if (container == null)
					throw new ArgumentNullException(nameof(container));
				return (container == CurrentContainerRetrieverIfAny) ? this : new State(container);
			}
		}
	}
}
