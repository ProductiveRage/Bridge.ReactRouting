using ProductiveRage.ReactRouting;

namespace Example.Actions
{
	public sealed class InvalidRoute : INavigationDispatcherAction
	{
		public InvalidRoute(UrlPathDetails url)
		{
			Url = Url;
		}
		public UrlPathDetails Url { get; private set; }
		public override string ToString() { return "InvalidRoute"; }
	}
}