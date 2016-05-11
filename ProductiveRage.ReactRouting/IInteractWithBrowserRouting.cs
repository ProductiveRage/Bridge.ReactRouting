using System;

namespace ProductiveRage.ReactRouting
{
	public interface IInteractWithBrowserRouting
	{
		UrlDetails CurrentLocation { get; }
		void RegisterForNavigatedCallback(Action<UrlDetails> callback);
		void NavigateTo(UrlDetails url);
		void RaiseNavigateToForCurrentLocation();
	}
}
