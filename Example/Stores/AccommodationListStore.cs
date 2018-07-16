using System;
using Bridge.React;
using Example.Actions;
using ProductiveRage.Immutable;

namespace HostBridge.Stores
{
	public sealed class AccommodationListStore : Store
	{
		public AccommodationListStore(IDispatcher dispatcher)
		{
			RequestedAt = Optional<DateTime>.Missing;
			dispatcher.Receive(a => a
				.If<NavigateToAccommodationList>(action => RequestedAt = DateTime.Now)
				.IfAnyMatched(OnChange)
			);
		}

		public Optional<DateTime> RequestedAt { get; private set; }
	}
}