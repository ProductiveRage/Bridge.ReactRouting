using System;
using System.Linq;
using Bridge.Html5;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting
{
	public sealed class Html5HistoryRouter : IInteractWithBrowserRouting
	{
		public static readonly Html5HistoryRouter Instance = new Html5HistoryRouter();

		private NonNullList<Action<UrlDetails>> _navigatedCallbacks;
		private Html5HistoryRouter()
		{
			_navigatedCallbacks = NonNullList<Action<UrlDetails>>.Empty;

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

			// 2017-12-08 DWR: Since navigations fire Dispatcher actions, it means that there will be difficulties if using a Dispatcher which does not allow the handling of one action to dispatch
			// another action (which is something that recommended by Facebook - to not let one action fire another because this could lead to a difficult-to-reason-about chain of events) if you
			// want to have a successful update send the User to a success page (because the update-successfully-applied action would cause a navigation, which would dispatch another action).
			// As a workaround (which, granted, feels a little dirty), we'll apply a SetTimeout to the history manipulation to "break" the potential Dispatcher action chain. This doesn't feel
			// SO filthy since requesting a location change and waiting for the browser to apply it and a corresponding Dispatcher method to be fired could understandably be an async process..
			// though, really, we only want this for convenience.
			Window.SetTimeout(() =>
			{
				Window.History.PushState(state: null, title: null, url: url.ToString());
				RaiseCallbacks(url);
			});
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
					.ToNonNullList(),
				(rawQueryStringContent == "") ? Optional<QueryString>.Missing : QueryString.Parse(rawQueryStringContent)
			);
		}
	}
}
