using System;
using Bridge.React;
using ProductiveRage.Immutable;
using ProductiveRage.ReactRouting.Tests.Support.Actions;
using ProductiveRage.ReactRouting.Tests.Support.RouteDataTypes;

namespace ProductiveRage.ReactRouting.Tests.Support
{
	public sealed class RootPlusHotelAndRestaurantSectionsNavigator : Navigator
	{
		private readonly Func<UrlPathDetails> _getRoot;
		public RootPlusHotelAndRestaurantSectionsNavigator(IDispatcher dispatcher) : base(Set<NonBlankTrimmedString>.Empty, dispatcher)
		{
			if (dispatcher == null)
				throw new ArgumentNullException("dispatcher");

			// This is the only route that this class directly creates..
			_getRoot = AddRelativeRoute(
				segments: Set<string>.Empty,
				routeAction: new NavigateToRoot(),
				urlGenerator: () => GetPath()
			);

			// .. however it does also create some other navigators that will declare routes under the "hotel" section and the "restaurant" section
			// (and we'll need to add the routes that these navigators declare to this instance's total set of known routes()
			Hotels = new RootPlusDynamicIdItemPagesNavigator<Hotel>(
				parentSegments: Set.Of(new NonBlankTrimmedString("hotel")),
				dispatcher: dispatcher
			);
			Restaurants = new RootPlusDynamicIdItemPagesNavigator<Restaurant>(
				parentSegments: Set.Of(new NonBlankTrimmedString("restaurant")),
				dispatcher: dispatcher
			);
			PullInRoutesFrom(Hotels);
			PullInRoutesFrom(Restaurants);
		}

		public UrlPathDetails Root() { return _getRoot(); }
		public RootPlusDynamicIdItemPagesNavigator<Hotel> Hotels { get; private set; }
		public RootPlusDynamicIdItemPagesNavigator<Restaurant> Restaurants { get; private set; }
	}
}
