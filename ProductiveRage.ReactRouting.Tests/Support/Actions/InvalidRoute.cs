using Bridge.React;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting.Tests.Support.Actions
{
	public sealed class InvalidRoute : IDispatcherAction, IAmImmutable
	{
		public InvalidRoute(UrlPathDetails url)
		{
			this.CtorSet(_ => _.Url, url);
		}
		public UrlPathDetails Url { get; private set; }
	}
}
