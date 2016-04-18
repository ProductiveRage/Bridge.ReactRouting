using System;
using System.Linq;
using Bridge.Html5;
using Bridge.QUnit;
using Bridge.React;
using ProductiveRage.Immutable;
using ProductiveRage.ReactRouting.Tests.Support;
using ProductiveRage.ReactRouting.Tests.Support.Actions;

namespace ProductiveRage.ReactRouting.Tests.TestClasses
{
	public static class NavigatorTests
	{
		[Ready]
		public static void Go()
		{
			QUnit.Module("Navigator");
			QUnit.Test("Single level: Static root plus dynamic 'item' id routes [static navigation methods]", TestRouterWithStaticNavigationCalls);
			QUnit.Test("Single level: Static root plus dynamic 'item' id routes [via historyHandler NavigateTo changes]", TestRouterWithDynamicNavigationCalls);
		}

		/// <summary>
		/// The navigation actions in this test are all triggered by using the static methods (Navigator.Root() and Navigator.Item(id)), which
		/// is how most navigation actions are expected to be initiated (since it means that typos are caught by the compiler, rather than being
		/// compiled alright but resulting in a different route being followed than expected)
		/// </summary>
		private static void TestRouterWithStaticNavigationCalls(Assert assert)
		{
			if (assert == null)
				throw new ArgumentNullException("assert");

			var navigatorTestWrapper = new NavigatorTester(assert);

			// The navigator initialisation will have executed match-current-route logic, so there should be a single action already present
			// corresponding to the initialUrl specified above
			navigatorTestWrapper.AssertActionRecorded<NavigateToRoot>();

			// Explicitly navigate again to the same URL we're already on - even though the route hasn't changed, this action should result
			// in the route being re-analysed and another action being recorded
			navigatorTestWrapper.Navigator.Root();
			navigatorTestWrapper.AssertActionRecorded<NavigateToRoot>();

			// Now navigate to a different route: /item/abc
			navigatorTestWrapper.Navigator.Item(new NonBlankTrimmedString("abc"));
			navigatorTestWrapper.AssertActionRecorded<NavigateToItem>(action => action.Id.Value == "abc");
		}

		/// <summary>
		/// The navigation actions in this test are all triggered by requests to the history handler to directly change the current URL - this
		/// is not how navigation is expected to work in most cases but it's possible that a project might be getting migrated bit by bit and
		/// so some navigations will may be triggered by direct calls to HTML5 history's pushState in some parts of the application, rather
		/// than the static methods being used in all cases. This is still supported, even if it is not encouraged.
		/// </summary>
		private static void TestRouterWithDynamicNavigationCalls(Assert assert)
		{
			if (assert == null)
				throw new ArgumentNullException("assert");

			var navigatorTestWrapper = new NavigatorTester(assert);

			// The navigator initialisation will have executed match-current-route logic, so there should be a single action already present
			// corresponding to the initialUrl specified above
			navigatorTestWrapper.AssertActionRecorded<NavigateToRoot>();

			// Explicitly navigate again to the same URL we're already on - even though the route hasn't changed, this action should result
			// in the route being re-analysed and another action being recorded
			navigatorTestWrapper.NavigateTo(UrlDetailsCreator.New());
			navigatorTestWrapper.AssertActionRecorded<NavigateToRoot>();

			// Now navigate to a different route: /item/abc
			navigatorTestWrapper.NavigateTo(UrlDetailsCreator.New("item", "abc"));
			navigatorTestWrapper.AssertActionRecorded<NavigateToItem>(action => action.Id.Value == "abc");
		}

		private sealed class NavigatorTester
		{
			private readonly IInteractWithBrowserRouting _historyHandler;
			private readonly Assert _assert;
			private Set<IDispatcherAction> _receivedActions;
			private int _actionsConfirmedSoFar;
			public NavigatorTester(Assert assert)
			{
				if (assert == null)
					throw new ArgumentNullException("assert");

				var dispatcher = new AppDispatcher();
				_receivedActions = Set<IDispatcherAction>.Empty;
				dispatcher.Register(
					message => _receivedActions = _receivedActions.Add(message.Action)
				);

				_historyHandler = new MockHistoryHandler(initialUrl: new UrlDetails(Set<NonBlankTrimmedString>.Empty));
				Navigator = new RootPlusDynamicIdItemPageNavigator(_historyHandler, dispatcher);
				RouteCombiner.StartListening(
					_historyHandler,
					Navigator.Routes,
					url => dispatcher.HandleViewAction(new InvalidRoute(url))
				);
				_historyHandler.RaiseNavigateToForCurrentLocation();

				_assert = assert;
				_actionsConfirmedSoFar = 0;
			}

			public RootPlusDynamicIdItemPageNavigator Navigator { get; private set; }

			public void NavigateTo(UrlDetails url)
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
}
