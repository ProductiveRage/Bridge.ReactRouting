using System;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting.Tests.Support
{
	public sealed class MockHistoryHandler : IInteractWithBrowserRouting
	{
		private Set<Action<UrlPathDetails>> _navigatedCallbacks;
		public MockHistoryHandler(UrlPathDetails initialUrl)
		{
			if (initialUrl == null)
				throw new ArgumentNullException("initialUrl");

			_navigatedCallbacks = Set<Action<UrlPathDetails>>.Empty;
			CurrentLocation = initialUrl;
		}

		public UrlPathDetails CurrentLocation { get; private set; }

		public void RaiseNavigateToForCurrentLocation()
		{
			RaiseCallbacks(CurrentLocation);
		}

		public void NavigateTo(UrlPathDetails url)
		{
			if (url == null)
				throw new ArgumentNullException("url");

			CurrentLocation = url;
			RaiseCallbacks(url);
		}

		public void RegisterForNavigatedCallback(Action<UrlPathDetails> callback)
		{
			if (callback == null)
				throw new ArgumentNullException("callback");
			_navigatedCallbacks = _navigatedCallbacks.Add(callback);
		}

		private void RaiseCallbacks(UrlPathDetails url)
		{
			if (url == null)
				throw new ArgumentNullException("url");

			foreach (var callback in _navigatedCallbacks)
				callback(url);
		}
	}
}
