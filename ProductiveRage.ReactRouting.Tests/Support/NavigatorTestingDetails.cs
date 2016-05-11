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
		private Set<IDispatcherAction> _receivedActions;
		private int _actionsConfirmedSoFar;
		public NavigatorTestingDetails(TNavigator navigator, IInteractWithBrowserRouting historyHandler, AppDispatcher dispatcher, Assert assert)
		{
			if (navigator == null)
				throw new ArgumentNullException("navigator");
			if (historyHandler == null)
				throw new ArgumentNullException("historyHandler");
			if (dispatcher == null)
				throw new ArgumentNullException("dispatcher");
			if (assert == null)
				throw new ArgumentNullException("assert");

			_receivedActions = Set<IDispatcherAction>.Empty;
			dispatcher.Register(
				message => _receivedActions = _receivedActions.Add(message.Action)
			);

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
			_historyHandler.NavigateTo(url);
		}

		public void AssertActionRecorded<TAction>(Predicate<TAction> optionalConditionThatActionMustMeet = null)
		{
			_assert.Equal(_receivedActions.Count, _actionsConfirmedSoFar + 1);
			_assert.Ok(_receivedActions.Last().Is<TAction>(), "Expected the last-recorded action to be of type " + typeof(TAction).GetClassName());
			if (optionalConditionThatActionMustMeet != null)
				_assert.Ok(optionalConditionThatActionMustMeet((TAction)_receivedActions.Last()), "The last-recorded action was of the expected type but did not meet the other condition(s)");
			_actionsConfirmedSoFar++;
		}
	}
}
