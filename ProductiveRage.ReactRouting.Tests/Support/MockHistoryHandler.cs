using System;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting.Tests.Support
{
	public sealed class MockHistoryHandler : IInteractWithBrowserRouting
	{
		private NonNullList<Action<UrlDetails>> _navigatedCallbacks;
		public MockHistoryHandler(UrlDetails initialUrl)
		{
			if (initialUrl == null)
				throw new ArgumentNullException("initialUrl");

			_navigatedCallbacks = NonNullList<Action<UrlDetails>>.Empty;
			CurrentLocation = initialUrl;
		}

		public UrlDetails CurrentLocation { get; private set; }

		public void RaiseNavigateToForCurrentLocation()
		{
			RaiseCallbacks(CurrentLocation);
		}

		public void NavigateTo(UrlDetails url)
		{
			if (url == null)
				throw new ArgumentNullException("url");

			CurrentLocation = url;
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
