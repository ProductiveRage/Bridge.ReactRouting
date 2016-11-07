using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting.Tests.Support.Actions
{
	public sealed class InvalidRoute : INavigationDispatcherAction, IAmImmutable
	{
		public InvalidRoute(UrlPathDetails url)
		{
			this.CtorSet(_ => _.Url, url);
		}
		public UrlPathDetails Url { get; private set; }
	}
}
