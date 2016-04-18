using Bridge.React;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting.Tests.Support.Actions
{
	public sealed class InvalidRoute : IDispatcherAction, IAmImmutable
	{
		public InvalidRoute(UrlDetails url)
		{
			this.CtorSet(_ => _.Url, url);
		}
		public UrlDetails Url { get; private set; }
	}
}
