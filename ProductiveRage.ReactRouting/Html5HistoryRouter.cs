using System;
using System.Linq;
using Bridge.Html5;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting
{
	public sealed class Html5HistoryRouter : IInteractWithBrowserRouting
	{
		public static readonly Html5HistoryRouter Instance = new Html5HistoryRouter();

		private Set<Action<UrlPathDetails>> _navigatedCallbacks;
		private Html5HistoryRouter()
		{
			_navigatedCallbacks = Set<Action<UrlPathDetails>>.Empty;

			Window.AddEventListener(EventType.PopState, e => RaiseNavigateToForCurrentLocation());
		}

		public UrlPathDetails CurrentLocation { get { return GetCurrentLocation(); } }

		public void RaiseNavigateToForCurrentLocation()
		{
			RaiseCallbacks(GetCurrentLocation());
		}

		public void NavigateTo(UrlPathDetails url)
		{
			if (url == null)
				throw new ArgumentNullException("url");

			Window.History.PushState(state: null, title: null, url: url.ToString());
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

		private static UrlPathDetails GetCurrentLocation()
		{
			// Can't use Window.Location.PathName directly until http://forums.bridge.net/forum/bridge-net-pro/bugs/2016 is fixed
			string currentLocationPathName = ((dynamic)Window.Location).pathname;
			return new UrlPathDetails(
				currentLocationPathName
					.Split('/')
					.Where(segment => !string.IsNullOrWhiteSpace(segment))
					.Select(segment => new NonBlankTrimmedString(segment))
					.ToSet()
			);
		}
	}
}
