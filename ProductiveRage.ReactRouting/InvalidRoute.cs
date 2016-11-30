namespace ProductiveRage.ReactRouting
{
	public sealed class InvalidRoute : INavigationDispatcherAction
	{
		public InvalidRoute(UrlPathDetails url)
		{
			Url = Url;
		}
		public UrlPathDetails Url { get; }
	}
}