using System;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting.Tests.Support
{
	public sealed class MockBrowserNavigator : IInteractWithBrowserRouting
	{
		private Set<Action<UrlDetails>> _navigatedCallbacks;
		public MockBrowserNavigator()
		{
			_navigatedCallbacks = Set<Action<UrlDetails>>.Empty;
		}

		public void NavigateTo(UrlDetails url)
		{
			if (url == null)
				throw new ArgumentNullException("url");

			Bridge.Html5.Console.WriteLine("MockBrowserNavigator: NavigateTo " + url);
			foreach (var callback in _navigatedCallbacks)
				callback(url);
		}

		public void RegisterForNavigatedCallback(Action<UrlDetails> callback)
		{
			if (callback == null)
				throw new ArgumentNullException("callback");
			_navigatedCallbacks = _navigatedCallbacks.Add(callback);
		}
	}
}
