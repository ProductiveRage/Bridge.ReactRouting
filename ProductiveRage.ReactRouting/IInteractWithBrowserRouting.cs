using System;

namespace ProductiveRage.ReactRouting
{
	public interface IInteractWithBrowserRouting
	{
		UrlPathDetails CurrentLocation { get; }
		void RegisterForNavigatedCallback(Action<UrlPathDetails> callback);
		void NavigateTo(UrlPathDetails url);
		void RaiseNavigateToForCurrentLocation();
	}
}
