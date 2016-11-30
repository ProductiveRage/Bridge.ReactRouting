using System;
using Bridge.React;
using Example.Actions;
using ProductiveRage.Immutable;
using ProductiveRage.ReactRouting;

namespace Example.Navigation
{
	public sealed class AccommodationNavigator : Navigator
	{
		private readonly Func<UrlPathDetails> _getRoot;
		private readonly Func<NonBlankTrimmedString, UrlPathDetails> _getWithSegment;
		private readonly Func<NonBlankTrimmedString, int, UrlPathDetails> _getWithSegmentAndIndex;
		public AccommodationNavigator(NonNullList<NonBlankTrimmedString> parentSegments, AppDispatcher dispatcher) : base(parentSegments, dispatcher)
		{
			_getRoot = AddRelativeRoute(
				segments: NonNullList<string>.Empty,
				routeAction: new NavigateToAccommodation(),
				urlGenerator: () => GetPath("")
			);

			_getWithSegment = AddRelativeRoute(
				routeDetails: RouteBuilder.Empty.String(),
				routeActionGenerator: segment => new NavigateToAccommodation(segment),
				urlGenerator: segment => GetPath("Accommodation", segment)
			);

			/* TODO
			_getWithSegmentAndIndex = AddRelativeRoute(
				routeDetails: RouteBuilder.Empty.String().Int(),
				routeActionGenerator: (segment, index) => new NavigateToAccommodation(segment, index),
				urlGenerator: (segment, index) => GetPath("Accommodation", segment, index)
			);
			*/
		}

		public UrlPathDetails Root() { return _getRoot(); }
		public UrlPathDetails Segment(NonBlankTrimmedString segment)
		{
			return _getWithSegment(segment);
		}
		/* TODO
		public UrlPathDetails SegmentAndIndex(NonBlankTrimmedString segment, int index)
		{
			return _getWithSegmentAndIndex(segment, index);
		}
		*/
	}
}