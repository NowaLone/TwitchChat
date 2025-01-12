namespace TwitchChat.Client
{
	/// <summary>
	/// Twitch-specific IRC Commands.
	/// </summary>
	public enum TwitchCommand
	{
		/// <summary>
		/// Sent when the bot or moderator removes all messages from the chat room or removes all messages for the specified user.
		/// </summary>
		CLEARCHAT,

		/// <summary>
		/// Sent when the bot removes a single message from the chat room.
		/// </summary>
		CLEARMSG,

		/// <summary>
		/// Sent when the bot authenticates with the server.
		/// </summary>
		GLOBALUSERSTATE,

		/// <summary>
		/// Sent when a channel starts or stops hosting viewers from another channel.
		/// </summary>
		HOSTTARGET,

		/// <summary>
		/// Sent when the Twitch IRC server needs to terminate the connection.
		/// </summary>
		RECONNECT,

		/// <summary>
		/// Sent when the bot joins a channel or when the channel’s chat room settings change.
		/// </summary>
		ROOMSTATE,

		/// <summary>
		/// Sent when events like someone subscribing to the channel occurs.
		/// </summary>
		USERNOTICE,

		/// <summary>
		/// Sent when the bot joins a channel or sends a PRIVMSG message.
		/// </summary>
		USERSTATE,

		/// <summary>
		/// Sent when someone sends your bot a whisper message.
		/// </summary>
		WHISPER,
	}
}