using System;
using System.Linq;
using Bridge.QUnit;
using Bridge.React;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting.Tests.Support
{
	public sealed class NavigatorTestingDetails<TNavigator>
	{
		private readonly IInteractWithBrowserRouting _historyHandler;
		private readonly Assert _assert;
		private Set<INavigationDispatcherAction> _receivedNavigationActions;
		private int _actionsConfirmedSoFar;
		public NavigatorTestingDetails(TNavigator navigator, IInteractWithBrowserRouting historyHandler, IDispatcher dispatcher, Assert assert)
		{
			if (navigator == null)
				throw new ArgumentNullException("navigator");
			if (historyHandler == null)
				throw new ArgumentNullException("historyHandler");
			if (dispatcher == null)
				throw new ArgumentNullException("dispatcher");
			if (assert == null)
				throw new ArgumentNullException("assert");

			_receivedNavigationActions = Set<INavigationDispatcherAction>.Empty;
			dispatcher.Register(message => {
				var navigationDispatcherAction = message.Action as INavigationDispatcherAction;
				if (navigationDispatcherAction != null)
					_receivedNavigationActions = _receivedNavigationActions.Add(navigationDispatcherAction);
			});

			_historyHandler = historyHandler;
			_assert = assert;
			_actionsConfirmedSoFar = 0;
			Navigator = navigator;
		}

		public TNavigator Navigator { get; private set; }

		public void NavigateTo(UrlPathDetails url)
		{
			if (url == null)
				throw new ArgumentNullException("url");
			_historyHandler.NavigateTo(url.ToUrlDetails(Optional<QueryString>.Missing));
		}

		public void AssertActionRecorded<TAction>(Predicate<TAction> optionalConditionThatActionMustMeet = null)
		{
			_assert.Equal(_receivedNavigationActions.Count, _actionsConfirmedSoFar + 1);
			_assert.Ok(_receivedNavigationActions.Last().Is<TAction>(), "Expected the last-recorded action to be of type " + typeof(TAction).Name);
			if (optionalConditionThatActionMustMeet != null)
				_assert.Ok(optionalConditionThatActionMustMeet((TAction)_receivedNavigationActions.Last()), "The last-recorded action was of the expected type but did not meet the other condition(s)");
			_actionsConfirmedSoFar++;
		}
	}
}
