namespace TwitchChat.Parser
{
	/// <summary>
	/// Lists the symbolic constants that the msg-id tag may be set to when the Twitch IRC server sends you a NOTICE message.
	/// </summary>
	public enum MessageId
	{
		#region NOTICE

		/// <summary>
		/// This room is no longer in emote-only mode.
		/// </summary>
		Emote_Only_Off,

		/// <summary>
		/// This room is now in emote-only mode.
		/// </summary>
		Emote_Only_On,

		/// <summary>
		/// This room is no longer in followers-only mode.
		/// </summary>
		Followers_Off,

		/// <summary>
		/// This room is now in &lt;duration&gt; followers-only mode.
		/// </summary>
		Followers_On,

		/// <summary>
		/// This room is now in followers-only mode.
		/// </summary>
		Followers_On_Zero,

		/// <summary>
		/// You are permanently banned from talking in &lt;channel&gt;.
		/// </summary>
		Msg_Banned,

		/// <summary>
		/// Your message was not sent because it contained too many unprocessable characters. If you believe this is an error, please rephrase and try again.
		/// </summary>
		Msg_Bad_Characters,

		/// <summary>
		/// Your message was not sent because your account is not in good standing in this channel.
		/// </summary>
		Msg_Channel_Blocked,

		/// <summary>
		/// This channel does not exist or has been suspended.
		/// </summary>
		Msg_Channel_Suspended,

		/// <summary>
		/// Your message was not sent because it is identical to the previous one you sent, less than 30 seconds ago.
		/// </summary>
		Msg_Duplicate,

		/// <summary>
		/// This room is in emote-only mode. You can find your currently available emoticons using the smiley in the chat text area.
		/// </summary>
		Msg_Emoteonly,

		/// <summary>
		/// This room is in &lt;duration&gt; followers-only mode. Follow&lt;channel&gt; to join the community! Note: These msg_followers tags are kickbacks to a user who does not meet the criteria; that is, does not follow or has not followed long enough.
		/// </summary>
		Msg_Followersonly,

		/// <summary>
		/// This room is in &lt;duration1&gt; followers-only mode. You have been following for &lt;duration2&gt;. Continue following to chat!
		/// </summary>
		Msg_Followersonly_Followed,

		/// <summary>
		/// This room is in followers-only mode. Follow&lt;channel&gt; to join the community!
		/// </summary>
		Msg_Followersonly_Zero,

		/// <summary>
		/// This room is in unique-chat mode and the message you attempted to send is not unique.
		/// </summary>
		Msg_R9k,

		/// <summary>
		/// Your message was not sent because you are sending messages too quickly.
		/// </summary>
		Msg_Ratelimit,

		/// <summary>
		/// Hey! Your message is being checked by mods and has not been sent.
		/// </summary>
		Msg_Rejected,

		/// <summary>
		/// Your message wasn’t posted due to conflicts with the channel’s moderation settings.
		/// </summary>
		Msg_Rejected_Mandatory,

		/// <summary>
		/// A verified phone number is required to chat in this channel. Please visit <a href="https://www.twitch.tv/settings/security">https://www.twitch.tv/settings/security</a> to verify your phone number.
		/// </summary>
		Msg_Requires_Verified_Phone_Number,

		/// <summary>
		/// This room is in slow mode and you are sending messages too quickly. You will be able to talk again in &lt;number&gt; seconds.
		/// </summary>
		Msg_Slowmode,

		/// <summary>
		/// This room is in subscribers only mode. To talk, purchase a channel subscription at <a href="https://www.twitch.tv/products/&lt;broadcaster login name&gt;/ticket?ref=subscriber_only_mode_chat">https://www.twitch.tv/products/&lt;broadcaster login name&gt;/ticket?ref=subscriber_only_mode_chat</a>.
		/// </summary>
		Msg_Subsonly,

		/// <summary>
		/// You don’t have permission to perform that action.
		/// </summary>
		Msg_Suspended,

		/// <summary>
		/// You are timed out for &lt;number&gt; more seconds.
		/// </summary>
		Msg_Timedout,

		/// <summary>
		/// This room requires a verified account to chat. Please verify your account at <a href="https://www.twitch.tv/settings/security">https://www.twitch.tv/settings/security</a>.
		/// </summary>
		Msg_Verified_Email,

		/// <summary>
		/// This room is no longer in slow mode.
		/// </summary>
		Slow_Off,

		/// <summary>
		/// This room is now in slow mode. You may send messages every &lt;number&gt; seconds.
		/// </summary>
		Slow_On,

		/// <summary>
		/// This room is no longer in subscribers-only mode.
		/// </summary>
		Subs_Off,

		/// <summary>
		/// This room is now in subscribers-only mode.
		/// </summary>
		Subs_On,

		/// <summary>
		/// The community has closed channel &lt;channel&gt; due to Terms of Service violations.
		/// </summary>
		Tos_Ban,

		/// <summary>
		/// Unrecognized command: &lt;command&gt;
		/// </summary>
		Unrecognized_Cmd,

		#endregion NOTICE

		#region USERNOTICE

		Sub,
		Resub,
		Subgift,
		Submysterygift,
		Giftpaidupgrade,
		Rewardgift,
		Anongiftpaidupgrade,
		Raid,
		Unraid,
		Bitsbadgetier,
		Sharedchatnotice,

		#endregion USERNOTICE
	}
}