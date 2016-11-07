using Bridge.React;

namespace ProductiveRage.ReactRouting
{
	/// <summary>
	/// This specialised version of the IDispatcherAction interface must be used for all actions that arise from a navigation since it may be important for parts of an
	/// application to know that an action has been dispatched that is relates to a navigation event (for example, if the application displays a login form when the user
	/// tries to perform an action that requires them to be authenticated, if they then click on something that navigates them away - maybe to a different page that does
	/// not require them to be authenticated or maybe just to a forgot-my-password form - then the login form should be hidden again; this is easily achieved if a login
	/// form that is being displayed may be hidden after any INavigationDispatcherAction is raised).
	/// </summary>
	public interface INavigationDispatcherAction : IDispatcherAction { }
}
