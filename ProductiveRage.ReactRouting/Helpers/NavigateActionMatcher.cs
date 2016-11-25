using System;
using System.Linq;
using Bridge.React;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting.Helpers
{
	/// <summary>
	/// Once you have logic that defines routes and maps them to INavigationDispatcherAction instances, you need something that will match these actions to particular React
	/// elements (generally stateful &quot;container&quot; components) and that will listen out for those actions and render the appropriate element. One convenient way to
	/// do that is to build a list of action-to-element mappings using this class and to then use it to instantiate a RoutingStoreActivatorContainer, which registers with
	/// the dispatcher and listens out to the supported actions (and uses that information to mount the appropriate React element).
	/// </summary>
	public sealed class NavigateActionMatcher
	{
		public static readonly NavigateActionMatcher Empty = new NavigateActionMatcher(NonNullList<Matcher>.Empty);

		/// <summary>
		/// Because ReactElement is External, we can't have a property that is Optional of ReactElement because of how Bridge works (it needs a real
		/// type to initialise an Optional instance) but we can make it an Optional of a Func of ReactElement
		/// </summary>
		private delegate Optional<Func<ReactElement>> Matcher(INavigationDispatcherAction action);

		private readonly NonNullList<Matcher> _navigateActionMatchers;
		private NavigateActionMatcher(NonNullList<Matcher> navigateActionMatchers)
		{
			if (navigateActionMatchers == null)
				throw new ArgumentNullException("navigateActionMatchers");

			_navigateActionMatchers = navigateActionMatchers;
		}

		public NavigateActionMatcher AddFor<T>(ReactElement element) where T : class, INavigationDispatcherAction
		{
			if (element == null)
				throw new ArgumentNullException("element");

			return AddFor<T>(action => element);
		}

		public NavigateActionMatcher AddFor<T>(Func<T, ReactElement> elementGenerator) where T : class, INavigationDispatcherAction
		{
			if (elementGenerator == null)
				throw new ArgumentNullException("elementGenerator");

			return new NavigateActionMatcher(_navigateActionMatchers.Add(
				action => (action is T) ? GetElementGenerator(elementGenerator, (T)action) : null
			));
		}

		public NavigateActionMatcher AddFor<T>(Func<T, bool> condition, Func<T, ReactElement> elementGenerator) where T : class, INavigationDispatcherAction
		{
			if (condition == null)
				throw new ArgumentNullException("condition");
			if (elementGenerator == null)
				throw new ArgumentNullException("elementGenerator");

			return new NavigateActionMatcher(_navigateActionMatchers.Add(
				action => (action is T) && condition((T)action) ? GetElementGenerator(elementGenerator, (T)action) : null
			));
		}

		public Optional<Func<ReactElement>> MatchAction(INavigationDispatcherAction action)
		{
			if (action == null)
				throw new ArgumentNullException("action");

			return _navigateActionMatchers
				.Select(navigateActionMatcher => navigateActionMatcher(action))
				.FirstOrDefault(newContainerGenerator => newContainerGenerator.IsDefined);
		}

		private static Func<ReactElement> GetElementGenerator<T>(Func<T, ReactElement> elementGenerator, T action) where T : class, INavigationDispatcherAction
		{
			if (elementGenerator == null)
				throw new ArgumentNullException("elementGenerator");
			if (action == null)
				throw new ArgumentNullException("action");

			return () => elementGenerator(action);
		}
	}
}