namespace TwitchChat.Parser
{
	/// <summary>
	/// The type of user.
	/// </summary>
	public enum UserType
	{
		/// <summary>
		/// A normal user.
		/// </summary>
		Normal,

		/// <summary>
		/// A Twitch administrator.
		/// </summary>
		Admin,

		/// <summary>
		/// A global moderator.
		/// </summary>
		Global_Mod,

		/// <summary>
		/// A Twitch employee.
		/// </summary>
		Staff
	}
}