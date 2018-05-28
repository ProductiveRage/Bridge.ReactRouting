using System;

namespace ProductiveRage.ReactRouting
{
	public interface IInteractWithBrowserRouting : IInitiateNavigations
	{
		UrlDetails CurrentLocation { get; }
		void RegisterForNavigatedCallback(Action<UrlDetails> callback);
		void RaiseNavigateToForCurrentLocation();
	}
}