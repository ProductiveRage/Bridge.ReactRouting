using System;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting.Tests.Support
{
	public sealed class MockHistoryHandler : IInteractWithBrowserRouting
	{
		private Set<Action<UrlDetails>> _navigatedCallbacks;
		private UrlDetails _currentUrl;
		public MockHistoryHandler(UrlDetails initialUrl)
		{
			if (initialUrl == null)
				throw new ArgumentNullException("initialUrl");

			_navigatedCallbacks = Set<Action<UrlDetails>>.Empty;
			_currentUrl = initialUrl;
		}

		public void RaiseNavigateToForCurrentLocation()
		{
			RaiseCallbacks(_currentUrl);
		}

		public void NavigateTo(UrlDetails url)
		{
			if (url == null)
				throw new ArgumentNullException("url");

			_currentUrl = url;
			RaiseCallbacks(url);
		}

		public void RegisterForNavigatedCallback(Action<UrlDetails> callback)
		{
			if (callback == null)
				throw new ArgumentNullException("callback");
			_navigatedCallbacks = _navigatedCallbacks.Add(callback);
		}

		private void RaiseCallbacks(UrlDetails url)
		{
			if (url == null)
				throw new ArgumentNullException("url");

			foreach (var callback in _navigatedCallbacks)
				callback(url);
		}
	}
}
