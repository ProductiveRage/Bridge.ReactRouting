using System;
using System.Linq;
using Bridge.Html5;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting
{
	public sealed class Html5HistoryRouter : IInteractWithBrowserRouting
	{
		public static readonly Html5HistoryRouter Instance = new Html5HistoryRouter();

		private Set<Action<UrlDetails>> _navigatedCallbacks;
		private Html5HistoryRouter()
		{
			_navigatedCallbacks = Set<Action<UrlDetails>>.Empty;

			Window.AddEventListener(EventType.PopState, e => RaiseNavigateToForCurrentLocation());
		}

		public UrlDetails CurrentLocation { get { return GetCurrentLocation(); } }

		public void RaiseNavigateToForCurrentLocation()
		{
			RaiseCallbacks(GetCurrentLocation());
		}

		public void NavigateTo(UrlDetails url)
		{
			if (url == null)
				throw new ArgumentNullException("url");

			Window.History.PushState(state: null, title: null, url: url.ToString());
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

		private static UrlDetails GetCurrentLocation()
		{
			var rawQueryStringContent = (Window.Location.Search ?? "").Trim().TrimStart('?');
			return new UrlDetails(
				Window.Location.PathName
					.Split('/')
					.Where(segment => !string.IsNullOrWhiteSpace(segment))
					.Select(segment => new NonBlankTrimmedString(segment))
					.ToSet(),
				(rawQueryStringContent == "") ? Optional<NonBlankTrimmedString>.Missing : new NonBlankTrimmedString(rawQueryStringContent)
			);
		}
	}
}
