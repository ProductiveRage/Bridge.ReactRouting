using System;

namespace ProductiveRage.ReactRouting
{
	public interface IInteractWithBrowserRouting
	{
		void RegisterForNavigatedCallback(Action<UrlDetails> callback);
		void NavigateTo(UrlDetails url);
		void RaiseNavigateToForCurrentLocation();
	}
}
