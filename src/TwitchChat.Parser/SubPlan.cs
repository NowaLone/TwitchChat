namespace TwitchChat.Parser
{
	/// <summary>
	/// The type of subscription plan being used.
	/// </summary>
	public enum SubPlan
	{
		/// <summary>
		/// Amazon Prime subscription.
		/// </summary>
		Prime,
		/// <summary>
		/// First level of paid subscription.
		/// </summary>
		First = 1000,
		/// <summary>
		/// Second level of paid subscription.
		/// </summary>
		Second = 2000,
		/// <summary>
		/// Third level of paid subscription.
		/// </summary>
		Third = 3000,
	}
}