using IrcNet;
using System;
using TwitchChat.Parser;

namespace TwitchChat.EventsArgs
{
	/// <summary>
	/// Provides data for the MessageSent event.
	/// </summary>
	public class MessageSentEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MessageSentEventArgs"/> class.
		/// </summary>
		/// <param name="message">The message that was sent.</param>
		public MessageSentEventArgs(TwitchMessage message)
		{
			this.Message = message;
		}

		/// <summary>
		/// Gets the message that was sent.
		/// </summary>
		public TwitchMessage Message { get; }
	}
}