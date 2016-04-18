using System;
using Bridge.Html5;
using Bridge.QUnit;
using Bridge.React;
using ProductiveRage.Immutable;
using ProductiveRage.ReactRouting.Tests.Support;
using ProductiveRage.ReactRouting.Tests.Support.Actions;
using ProductiveRage.ReactRouting.Tests.Support.RouteDataTypes;

namespace ProductiveRage.ReactRouting.Tests.TestClasses
{
	public static class NavigatorTests
	{
		[Ready]
		public static void Go()
		{
			QUnit.Module("Navigator");
			QUnit.Test("Single level: Static root plus dynamic 'item' id routes [static navigation methods]", TestHotelRouterWithStaticNavigationCalls);
			QUnit.Test("Single level: Static root plus dynamic 'item' id routes [via historyHandler NavigateTo changes]", TestHotelRouterWithDynamicNavigationCalls);
			QUnit.Test("Nested Navigators: Root plus static-root-plus-dynamic-'item'-id-routes for 'hotel' and 'restaurant' [static navigation methods]", TestNestedRouterWithStaticNavigationCalls);
			QUnit.Test("Nested Navigators: Root plus static-root-plus-dynamic-'item'-id-routes for 'hotel' and 'restaurant' [via historyHandler NavigateTo changes]", TestNestedRouterWithDynamiccNavigationCalls);
		}

		/// <summary>
		/// The navigation actions in this test are all triggered by using the static methods (Navigator.Root() and Navigator.Item(id)), which
		/// is how most navigation actions are expected to be initiated (since it means that typos are caught by the compiler, rather than being
		/// compiled alright but resulting in a different route being followed than expected)
		/// </summary>
		private static void TestHotelRouterWithStaticNavigationCalls(Assert assert)
		{
			if (assert == null)
				throw new ArgumentNullException("assert");

			var navigatorTestWrapper = GetNavigatorToTest(
				initialUrl: new UrlDetails(Set<NonBlankTrimmedString>.Empty),
				assert: assert,
				navigatorGenerator: (historyHandler, dispatcher) => new RootPlusDynamicIdItemPagesNavigator<Hotel>(historyHandler, dispatcher)
			);

			// The navigator initialisation will have executed match-current-route logic, so there should be a single action already present
			// corresponding to the initialUrl specified above
			navigatorTestWrapper.AssertActionRecorded<NavigateToRoot<Hotel>>();

			// Explicitly navigate again to the same URL we're already on - even though the route hasn't changed, this action should result
			// in the route being re-analysed and another action being recorded
			navigatorTestWrapper.Navigator.Root();
			navigatorTestWrapper.AssertActionRecorded<NavigateToRoot<Hotel>>();

			// Now navigate to a different route: /item/abc
			navigatorTestWrapper.Navigator.Item(new NonBlankTrimmedString("abc"));
			navigatorTestWrapper.AssertActionRecorded<NavigateToItem<Hotel>>(action => action.Id.Value == "abc");
		}

		/// <summary>
		/// The navigation actions in this test are all triggered by using the static methods
		/// </summary>
		private static void TestNestedRouterWithStaticNavigationCalls(Assert assert)
		{
			if (assert == null)
				throw new ArgumentNullException("assert");

			var navigatorTestWrapper = GetNavigatorToTest(
				initialUrl: new UrlDetails(Set<NonBlankTrimmedString>.Empty),
				assert: assert,
				navigatorGenerator: (historyHandler, dispatcher) => new RootPlusHotelAndRestaurantSectionsNavigator(historyHandler, dispatcher)
			);

			// The navigator initialisation will have executed match-current-route logic, so there should be a single action already present
			// corresponding to the initialUrl specified above
			navigatorTestWrapper.AssertActionRecorded<NavigateToRoot>();

			// Explicitly navigate again to the same URL we're already on - even though the route hasn't changed, this action should result
			// in the route being re-analysed and another action being recorded
			navigatorTestWrapper.Navigator.Root();
			navigatorTestWrapper.AssertActionRecorded<NavigateToRoot>();

			// Now navigate to the root of the "Hotels" section..
			navigatorTestWrapper.Navigator.Hotels.Root();
			navigatorTestWrapper.AssertActionRecorded<NavigateToRoot<Hotel>>();

			// .. and then to an items within hotels
			navigatorTestWrapper.Navigator.Hotels.Item(new NonBlankTrimmedString("abc"));
			navigatorTestWrapper.AssertActionRecorded<NavigateToItem<Hotel>>(action => action.Id.Value == "abc");

			// Now navigate to the root of the "Restaurants" section..
			navigatorTestWrapper.Navigator.Restaurants.Root();
			navigatorTestWrapper.AssertActionRecorded<NavigateToRoot<Restaurant>>();

			// .. and then to an items within restaurants
			navigatorTestWrapper.Navigator.Restaurants.Item(new NonBlankTrimmedString("xyz"));
			navigatorTestWrapper.AssertActionRecorded<NavigateToItem<Restaurant>>(action => action.Id.Value == "xyz");
		}

		/// <summary>
		/// The navigation actions in this test are all triggered by requests to the history handler to directly change the current URL
		/// </summary>
		private static void TestNestedRouterWithDynamiccNavigationCalls(Assert assert)
		{
			if (assert == null)
				throw new ArgumentNullException("assert");

			var navigatorTestWrapper = GetNavigatorToTest(
				initialUrl: new UrlDetails(Set<NonBlankTrimmedString>.Empty),
				assert: assert,
				navigatorGenerator: (historyHandler, dispatcher) => new RootPlusHotelAndRestaurantSectionsNavigator(historyHandler, dispatcher)
			);

			// The navigator initialisation will have executed match-current-route logic, so there should be a single action already present
			// corresponding to the initialUrl specified above
			navigatorTestWrapper.AssertActionRecorded<NavigateToRoot>();

			// Explicitly navigate again to the same URL we're already on - even though the route hasn't changed, this action should result
			// in the route being re-analysed and another action being recorded
			navigatorTestWrapper.NavigateTo(UrlDetailsCreator.New());
			navigatorTestWrapper.AssertActionRecorded<NavigateToRoot>();

			// Now navigate to the root of the "Hotels" section..
			navigatorTestWrapper.NavigateTo(UrlDetailsCreator.New("hotel"));
			navigatorTestWrapper.AssertActionRecorded<NavigateToRoot<Hotel>>();

			// .. and then to an items with hotels
			navigatorTestWrapper.NavigateTo(UrlDetailsCreator.New("hotel", "item", "abc"));
			navigatorTestWrapper.AssertActionRecorded<NavigateToItem<Hotel>>(action => action.Id.Value == "abc");

			// Now navigate to the root of the "Restaurants" section..
			navigatorTestWrapper.NavigateTo(UrlDetailsCreator.New("restaurant"));
			navigatorTestWrapper.AssertActionRecorded<NavigateToRoot<Restaurant>>();

			// .. and then to an items within restaurants
			navigatorTestWrapper.NavigateTo(UrlDetailsCreator.New("restaurant", "item", "xyz"));
			navigatorTestWrapper.AssertActionRecorded<NavigateToItem<Restaurant>>(action => action.Id.Value == "xyz");
		}

		/// <summary>
		/// The navigation actions in this test are all triggered by requests to the history handler to directly change the current URL - this
		/// is not how navigation is expected to work in most cases but it's possible that a project might be getting migrated bit by bit and
		/// so some navigations will may be triggered by direct calls to HTML5 history's pushState in some parts of the application, rather
		/// than the static methods being used in all cases. This is still supported, even if it is not encouraged.
		/// </summary>
		private static void TestHotelRouterWithDynamicNavigationCalls(Assert assert)
		{
			if (assert == null)
				throw new ArgumentNullException("assert");

			var navigatorTestWrapper = GetNavigatorToTest(
				initialUrl: new UrlDetails(Set<NonBlankTrimmedString>.Empty),
				assert: assert,
				navigatorGenerator: (historyHandler, dispatcher) => new RootPlusDynamicIdItemPagesNavigator<Hotel>(historyHandler, dispatcher)
			);

			// The navigator initialisation will have executed match-current-route logic, so there should be a single action already present
			// corresponding to the initialUrl specified above
			navigatorTestWrapper.AssertActionRecorded<NavigateToRoot<Hotel>>();

			// Explicitly navigate again to the same URL we're already on - even though the route hasn't changed, this action should result
			// in the route being re-analysed and another action being recorded
			navigatorTestWrapper.NavigateTo(UrlDetailsCreator.New());
			navigatorTestWrapper.AssertActionRecorded<NavigateToRoot<Hotel>>();

			// Now navigate to a different route: /item/abc
			navigatorTestWrapper.NavigateTo(UrlDetailsCreator.New("item", "abc"));
			navigatorTestWrapper.AssertActionRecorded<NavigateToItem<Hotel>>(action => action.Id.Value == "abc");
		}

		private static NavigatorTestingDetails<TNavigator> GetNavigatorToTest<TNavigator>(
			UrlDetails initialUrl,
			Assert assert,
			Func<IInteractWithBrowserRouting, AppDispatcher, TNavigator> navigatorGenerator)
				where TNavigator : Navigator
		{
			if (initialUrl == null)
				throw new ArgumentNullException("initialUrl");
			if (assert == null)
				throw new ArgumentNullException("assert");
			if (navigatorGenerator == null)
				throw new ArgumentNullException("navigatorGenerator");

			var dispatcher = new AppDispatcher();
			var historyHandler = new MockHistoryHandler(initialUrl: new UrlDetails(Set<NonBlankTrimmedString>.Empty));
			var navigator = navigatorGenerator(historyHandler, dispatcher);
			RouteCombiner.StartListening(
				historyHandler,
				navigator.Routes,
				url => dispatcher.HandleViewAction(new InvalidRoute(url))
			);
			var navigatorTestingDetails = new NavigatorTestingDetails<TNavigator>(navigator, historyHandler, dispatcher, assert);
			historyHandler.RaiseNavigateToForCurrentLocation();
			return navigatorTestingDetails;
		}
	}
}
