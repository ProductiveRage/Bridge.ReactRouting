using System;
using Bridge.React;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting
{
	public abstract class Navigator
	{
		private readonly Set<NonBlankTrimmedString> _parentSegments;
		private readonly IDispatcher _dispatcher;
		private Set<IMatchRoutes> _routes;

		protected Navigator(Set<NonBlankTrimmedString> parentSegments, IDispatcher dispatcher)
		{
			if (parentSegments == null)
				throw new ArgumentNullException("parentSegments");
			if (dispatcher == null)
				throw new ArgumentNullException("dispatcher");

			_parentSegments = parentSegments;
			_dispatcher = dispatcher;
			_routes = Set<IMatchRoutes>.Empty;
		}
		protected Navigator(IDispatcher dispatcher) : this(Set<NonBlankTrimmedString>.Empty, dispatcher) { }

		public Set<IMatchRoutes> Routes { get { return _routes; } }

		/// <summary>
		/// For each of a Navigator's routes, it must define the route, map that route onto a dispatcher action and expose a method that will take values for any variables in a route
		/// and that will return a UrlPathDetails instance. The AddRelativeRoute methods require all of these things. It records the route details and the dispatcher action mapping
		/// logic and it takes a delegate for the route-variable-to-UrlPathDetails mapping - this delegate is passed back out, it is not recorded anywhere by the AddRelativeRoute
		/// call, it is only provided so that static analysis can ensure that it is of the correct form (and that its arguments match the route variables). This method overload
		/// defines a fixed route with a single segment (and so the urlGenerator delegate has zero arguments).
		/// </summary>
		protected Func<UrlPathDetails> AddRelativeRoute(string segment, INavigationDispatcherAction routeAction, Func<UrlPathDetails> urlGenerator)
		{
			if (string.IsNullOrWhiteSpace(segment))
				throw new ArgumentException("Null, blank or whitespace-only segment specified");
			if (routeAction == null)
				throw new ArgumentNullException("routeAction");
			if (urlGenerator == null)
				throw new ArgumentNullException(nameof(urlGenerator));

			return AddRelativeRoute(Set.Of(segment), routeAction, urlGenerator);
		}

		/// <summary>
		/// For each of a Navigator's routes, it must define the route, map that route onto a dispatcher action and expose a method that will take values for any variables in a route
		/// and that will return a UrlPathDetails instance. The AddRelativeRoute methods require all of these things. It records the route details and the dispatcher action mapping
		/// logic and it takes a delegate for the route-variable-to-UrlPathDetails mapping - this delegate is passed back out, it is not recorded anywhere by the AddRelativeRoute
		/// call, it is only provided so that static analysis can ensure that it is of the correct form (and that its arguments match the route variables). This method overload
		/// defines a fixed route with zero, one or multiple segments (the urlGenerator delegate has zero arguments because there are zero variable segments in the route).
		/// </summary>
		protected Func<UrlPathDetails> AddRelativeRoute(Set<string> segments, INavigationDispatcherAction routeAction, Func<UrlPathDetails> urlGenerator)
		{
			if (segments == null)
				throw new ArgumentNullException("segments");
			if (routeAction == null)
				throw new ArgumentNullException("routeAction");
			if (urlGenerator == null)
				throw new ArgumentNullException(nameof(urlGenerator));

			var relativeRouteSegments = RouteBuilder.Empty;
			foreach (var segment in segments)
			{
				if (segment.Trim() == "") // No need to check for null, a Set never contains null references
					throw new ArgumentException("Blank or whitespace-only segment specified in fixed route");
				relativeRouteSegments = relativeRouteSegments.Fixed(new NonBlankTrimmedString(segment));
			}
			AddRelativeRouteOnly(
				relativeRouteSegments.ToRoute(() => _dispatcher.HandleViewAction(routeAction))
			);
			return urlGenerator;
		}

		/// <summary>
		/// For each of a Navigator's routes, it must define the route, map that route onto a dispatcher action and expose a method that will take values for any variables in a route
		/// and that will return a UrlPathDetails instance. The AddRelativeRoute methods require all of these things. It records the route details and the dispatcher action mapping
		/// logic and it takes a delegate for the route-variable-to-UrlPathDetails mapping - this delegate is passed back out, it is not recorded anywhere by the AddRelativeRoute
		/// call, it is only provided so that static analysis can ensure that it is of the correct form (and that its arguments match the route variables). This method overload
		/// defines a route whose variables may all be fulfilled by a TMatchedValue instance (the urlGenerator delegate has a single argument since a single TMatchedValue may
		/// fully populate a route).
		/// </summary>
		protected Func<TMatchedValue, UrlPathDetails> AddRelativeRoute<TMatchedValue>(
			RouteBuilder.IBuildRoutesWithVariablesToMatch<TMatchedValue> routeDetails,
			Func<TMatchedValue, INavigationDispatcherAction> routeActionGenerator,
			Func<TMatchedValue, UrlPathDetails> urlGenerator)
		{
			if (routeDetails == null)
				throw new ArgumentNullException("routeDetails");
			if (routeActionGenerator == null)
				throw new ArgumentNullException("routeActionGenerator");
			if (urlGenerator == null)
				throw new ArgumentNullException(nameof(urlGenerator));

			AddRelativeRouteOnly(
				routeDetails.ToRoute(matchedValues => _dispatcher.HandleViewAction(routeActionGenerator(matchedValues)))
			);
			return urlGenerator;
		}

		/// <summary>
		/// For each of a Navigator's routes, it must define the route, map that route onto a dispatcher action and expose a method that will take values for any variables in a route
		/// and that will return a UrlPathDetails instance. The AddRelativeRoute methods require all of these things. It records the route details and the dispatcher action mapping
		/// logic and it takes a delegate for the route-variable-to-UrlPathDetails mapping - this delegate is passed back out, it is not recorded anywhere by the AddRelativeRoute
		/// call, it is only provided so that static analysis can ensure that it is of the correct form (and that its arguments match the route variables). Some routes may be
		/// configured to record all of the route variables in a single type instance while some may record each variable in a Tuple - this overload supports the case where
		/// a Tuple is used (with two elements). For convenience, the urlGenerator takes the unwrapped elements, rather than taking a single Tuple that wraps each value.
		/// </summary>
		protected Func<T1, T2, UrlPathDetails> AddRelativeRoute<T1, T2>(
			RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, T2>> routeDetails,
			Func<Tuple<T1, T2>, INavigationDispatcherAction> routeActionGenerator,
			Func<T1, T2, UrlPathDetails> urlGenerator)
		{
			if (routeDetails == null)
				throw new ArgumentNullException("routeDetails");
			if (routeActionGenerator == null)
				throw new ArgumentNullException("routeActionGenerator");
			if (urlGenerator == null)
				throw new ArgumentNullException(nameof(urlGenerator));

			AddRelativeRouteOnly(
				routeDetails.ToRoute(matchedValues => _dispatcher.HandleViewAction(routeActionGenerator(matchedValues)))
			);
			return urlGenerator;
		}

		/// <summary>
		/// For each of a Navigator's routes, it must define the route, map that route onto a dispatcher action and expose a method that will take values for any variables in a route
		/// and that will return a UrlPathDetails instance. The AddRelativeRoute methods require all of these things. It records the route details and the dispatcher action mapping
		/// logic and it takes a delegate for the route-variable-to-UrlPathDetails mapping - this delegate is passed back out, it is not recorded anywhere by the AddRelativeRoute
		/// call, it is only provided so that static analysis can ensure that it is of the correct form (and that its arguments match the route variables). Some routes may be
		/// configured to record all of the route variables in a single type instance while some may record each variable in a Tuple - this overload supports the case where
		/// a Tuple is used (with three elements). For convenience, the urlGenerator takes the unwrapped elements, rather than taking a single Tuple that wraps each value.
		/// </summary>
		protected Func<T1, T2, T3, UrlPathDetails> AddRelativeRoute<T1, T2, T3>(
			RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, T2, T3>> routeDetails,
			Func<Tuple<T1, T2, T3>, INavigationDispatcherAction> routeActionGenerator,
			Func<T1, T2, T3, UrlPathDetails> urlGenerator)
		{
			if (routeDetails == null)
				throw new ArgumentNullException("routeDetails");
			if (routeActionGenerator == null)
				throw new ArgumentNullException("routeActionGenerator");
			if (urlGenerator == null)
				throw new ArgumentNullException(nameof(urlGenerator));

			AddRelativeRouteOnly(
				routeDetails.ToRoute(matchedValues => _dispatcher.HandleViewAction(routeActionGenerator(matchedValues)))
			);
			return urlGenerator;
		}

		/// <summary>
		/// For each of a Navigator's routes, it must define the route, map that route onto a dispatcher action and expose a method that will take values for any variables in a route
		/// and that will return a UrlPathDetails instance. The AddRelativeRoute methods require all of these things. It records the route details and the dispatcher action mapping
		/// logic and it takes a delegate for the route-variable-to-UrlPathDetails mapping - this delegate is passed back out, it is not recorded anywhere by the AddRelativeRoute
		/// call, it is only provided so that static analysis can ensure that it is of the correct form (and that its arguments match the route variables). Some routes may be
		/// configured to record all of the route variables in a single type instance while some may record each variable in a Tuple - this overload supports the case where
		/// a Tuple is used (with four elements). For convenience, the urlGenerator takes the unwrapped elements, rather than taking a single Tuple that wraps each value.
		/// </summary>
		protected Func<T1, T2, T3, T4, UrlPathDetails> AddRelativeRoute<T1, T2, T3, T4>(
			RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, T2, T3, T4>> routeDetails,
			Func<Tuple<T1, T2, T3, T4>, INavigationDispatcherAction> routeActionGenerator,
			Func<T1, T2, T3, T4, UrlPathDetails> urlGenerator)
		{
			if (routeDetails == null)
				throw new ArgumentNullException("routeDetails");
			if (routeActionGenerator == null)
				throw new ArgumentNullException("routeActionGenerator");
			if (urlGenerator == null)
				throw new ArgumentNullException(nameof(urlGenerator));

			AddRelativeRouteOnly(
				routeDetails.ToRoute(matchedValues => _dispatcher.HandleViewAction(routeActionGenerator(matchedValues)))
			);
			return urlGenerator;
		}

		/// For each of a Navigator's routes, it must define the route, map that route onto a dispatcher action and expose a method that will take values for any variables in a route
		/// and that will return a UrlPathDetails instance. The AddRelativeRoute methods require all of these things. It records the route details and the dispatcher action mapping
		/// logic and it takes a delegate for the route-variable-to-UrlPathDetails mapping - this delegate is passed back out, it is not recorded anywhere by the AddRelativeRoute
		/// call, it is only provided so that static analysis can ensure that it is of the correct form (and that its arguments match the route variables). Some routes may be
		/// configured to record all of the route variables in a single type instance while some may record each variable in a Tuple - this overload supports the case where
		/// a Tuple is used (with five elements). For convenience, the urlGenerator takes the unwrapped elements, rather than taking a single Tuple that wraps each value.
		protected Func<T1, T2, T3, T4, T5, UrlPathDetails> AddRelativeRoute<T1, T2, T3, T4, T5>(
			RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, T2, T3, T4, T5>> routeDetails,
			Func<Tuple<T1, T2, T3, T4, T5>, INavigationDispatcherAction> routeActionGenerator,
			Func<T1, T2, T3, T4, T5, UrlPathDetails> urlGenerator)
		{
			if (routeDetails == null)
				throw new ArgumentNullException("routeDetails");
			if (routeActionGenerator == null)
				throw new ArgumentNullException("routeActionGenerator");
			if (urlGenerator == null)
				throw new ArgumentNullException(nameof(urlGenerator));

			AddRelativeRouteOnly(
				routeDetails.ToRoute(matchedValues => _dispatcher.HandleViewAction(routeActionGenerator(matchedValues)))
			);
			return urlGenerator;
		}

		/// For each of a Navigator's routes, it must define the route, map that route onto a dispatcher action and expose a method that will take values for any variables in a route
		/// and that will return a UrlPathDetails instance. The AddRelativeRoute methods require all of these things. It records the route details and the dispatcher action mapping
		/// logic and it takes a delegate for the route-variable-to-UrlPathDetails mapping - this delegate is passed back out, it is not recorded anywhere by the AddRelativeRoute
		/// call, it is only provided so that static analysis can ensure that it is of the correct form (and that its arguments match the route variables). Some routes may be
		/// configured to record all of the route variables in a single type instance while some may record each variable in a Tuple - this overload supports the case where
		/// a Tuple is used (with six elements). For convenience, the urlGenerator takes the unwrapped elements, rather than taking a single Tuple that wraps each value.
		protected Func<T1, T2, T3, T4, T5, T6, UrlPathDetails> AddRelativeRoute<T1, T2, T3, T4, T5, T6>(
			RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, T2, T3, T4, T5, T6>> routeDetails,
			Func<Tuple<T1, T2, T3, T4, T5, T6>, INavigationDispatcherAction> routeActionGenerator,
			Func<T1, T2, T3, T4, T5, T6, UrlPathDetails> urlGenerator)
		{
			if (routeDetails == null)
				throw new ArgumentNullException("routeDetails");
			if (routeActionGenerator == null)
				throw new ArgumentNullException("routeActionGenerator");
			if (urlGenerator == null)
				throw new ArgumentNullException(nameof(urlGenerator));

			AddRelativeRouteOnly(
				routeDetails.ToRoute(matchedValues => _dispatcher.HandleViewAction(routeActionGenerator(matchedValues)))
			);
			return urlGenerator;
		}

		/// For each of a Navigator's routes, it must define the route, map that route onto a dispatcher action and expose a method that will take values for any variables in a route
		/// and that will return a UrlPathDetails instance. The AddRelativeRoute methods require all of these things. It records the route details and the dispatcher action mapping
		/// logic and it takes a delegate for the route-variable-to-UrlPathDetails mapping - this delegate is passed back out, it is not recorded anywhere by the AddRelativeRoute
		/// call, it is only provided so that static analysis can ensure that it is of the correct form (and that its arguments match the route variables). Some routes may be
		/// configured to record all of the route variables in a single type instance while some may record each variable in a Tuple - this overload supports the case where
		/// a Tuple is used (with seven elements). For convenience, the urlGenerator takes the unwrapped elements, rather than taking a single Tuple that wraps each value.
		protected Func<T1, T2, T3, T4, T5, T6, T7, UrlPathDetails> AddRelativeRoute<T1, T2, T3, T4, T5, T6, T7>(
			RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, T2, T3, T4, T5, T6, T7>> routeDetails,
			Func<Tuple<T1, T2, T3, T4, T5, T6, T7>, INavigationDispatcherAction> routeActionGenerator,
			Func<T1, T2, T3, T4, T5, T6, T7, UrlPathDetails> urlGenerator)
		{
			if (routeDetails == null)
				throw new ArgumentNullException("routeDetails");
			if (routeActionGenerator == null)
				throw new ArgumentNullException("routeActionGenerator");
			if (urlGenerator == null)
				throw new ArgumentNullException(nameof(urlGenerator));

			AddRelativeRouteOnly(
				routeDetails.ToRoute(matchedValues => _dispatcher.HandleViewAction(routeActionGenerator(matchedValues)))
			);
			return urlGenerator;
		}

		/// For each of a Navigator's routes, it must define the route, map that route onto a dispatcher action and expose a method that will take values for any variables in a route
		/// and that will return a UrlPathDetails instance. The AddRelativeRoute methods require all of these things. It records the route details and the dispatcher action mapping
		/// logic and it takes a delegate for the route-variable-to-UrlPathDetails mapping - this delegate is passed back out, it is not recorded anywhere by the AddRelativeRoute
		/// call, it is only provided so that static analysis can ensure that it is of the correct form (and that its arguments match the route variables). Some routes may be
		/// configured to record all of the route variables in a single type instance while some may record each variable in a Tuple - this overload supports the case where
		/// a Tuple is used (with eight elements). For convenience, the urlGenerator takes the unwrapped elements, rather than taking a single Tuple that wraps each value.
		protected Func<T1, T2, T3, T4, T5, T6, T7, T8, UrlPathDetails> AddRelativeRoute<T1, T2, T3, T4, T5, T6, T7, T8>(
			RouteBuilder.IBuildRoutesWithVariablesToMatch<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> routeDetails,
			Func<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>, INavigationDispatcherAction> routeActionGenerator,
			Func<T1, T2, T3, T4, T5, T6, T7, T8, UrlPathDetails> urlGenerator)
		{
			if (routeDetails == null)
				throw new ArgumentNullException("routeDetails");
			if (routeActionGenerator == null)
				throw new ArgumentNullException("routeActionGenerator");
			if (urlGenerator == null)
				throw new ArgumentNullException(nameof(urlGenerator));

			AddRelativeRouteOnly(
				routeDetails.ToRoute(matchedValues => _dispatcher.HandleViewAction(routeActionGenerator(matchedValues)))
			);
			return urlGenerator;
		}

		/// <summary>
		/// This is a convenience method to make the calling code cleaner, at the cost of the method signature being less descriptive - each segment must be non-null and must return
		/// a non-null, non-blank and non-whitespace-only value when its ToString method is called. All string values will be wrapped in NonBlankTrimmedString instances and so any
		/// leading or trailing whitespace will be ignored. The values will be translated into a UrlDetails instance.
		/// </summary>
		protected UrlPathDetails GetPath(params object[] segments)
		{
			if (segments == null)
				throw new ArgumentNullException("segment");

			var nonBlankSegments = Set<NonBlankTrimmedString>.Empty;
			foreach (var segment in _parentSegments)
				nonBlankSegments = nonBlankSegments.Add(new NonBlankTrimmedString(segment));
			foreach (var segment in segments)
			{
				if (segment == null)
					throw new ArgumentException($"Null reference value encountered in {nameof(segments)} array");
				var segmentString = segment.ToString();
				if (string.IsNullOrWhiteSpace(segmentString))
					throw new ArgumentException($"Encountered value in {nameof(segment)} array that returns a null/blank/whitespace-only value from ToString");
				nonBlankSegments = nonBlankSegments.Add(new NonBlankTrimmedString(segmentString));
			}
			return new UrlPathDetails(nonBlankSegments);
		}

		/// <summary>
		/// If any parent segments were passed to the constructor of this instance, they will automatically be injected into the route provided here
		/// </summary>
		private void AddRelativeRouteOnly(IMatchRoutes route)
		{
			if (route == null)
				throw new ArgumentNullException("route");

			_routes = _routes.Add(route.MakeRelativeTo(_parentSegments));
		}

		protected void PullInRoutesFrom(Navigator otherNavigator)
		{
			if (otherNavigator == null)
				throw new ArgumentNullException(nameof(otherNavigator));

			foreach (var route in otherNavigator.Routes)
				AddRelativeRouteOnly(route);
		}
	}
}