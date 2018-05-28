using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting
{
	public interface IInitiateNavigations
	{
		void NavigateTo(UrlDetails url);

		/// <summary>
		/// This property should be updated just before each NavigateTo call is processed and records the current URL at that time. A comprehensive navigation history is not maintained by this library
		/// and this property does not change when the User navigates using the browser controls (back, forward), it is ONLY when NavigateTo is called. Even so, it may still have value as it may be
		/// used to record a where-to-return-to value for when a particular Store is mounted due to a navigation event (for example, the XzyStore might record the value when it is mounted and then
		/// the XyzContainer component might navigate to that Url when its 'Back' button is clicked - if the XyzStore recorded a Missing value then the XyzContainer will need to have a default
		/// URL that it back to).
		/// </summary>
		Optional<UrlDetails> LastNavigatedToUrl { get; }
	}
}
