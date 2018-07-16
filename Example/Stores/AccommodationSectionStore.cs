using System;
using Bridge.React;
using Example.Actions;
using ProductiveRage.Immutable;

namespace HostBridge.Stores
{
	public sealed class AccommodationSectionStore : Store
	{
		public AccommodationSectionStore(IDispatcher dispatcher)
		{
			RequestedAccommodationSection = null;
			dispatcher.Receive(a => a
				.If<NavigateToAccommodationSection>(
					action => RequestedAccommodationSection = new RequestedAccommodationSectionDetails(action.Segment, DateTime.Now)
				)
				.IfAnyMatched(OnChange)
			);
		}

		public RequestedAccommodationSectionDetails RequestedAccommodationSection { get; private set; }

		public sealed class RequestedAccommodationSectionDetails : IAmImmutable
		{
			public RequestedAccommodationSectionDetails(NonBlankTrimmedString segment, DateTime requestedAt)
			{
				this.CtorSet(_ => _.Segment, segment);
				this.CtorSet(_ => _.RequestedAt, requestedAt);
			}
			public NonBlankTrimmedString Segment { get; }
			public DateTime RequestedAt { get; }
		}
	}
}