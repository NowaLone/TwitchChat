﻿using System;
using TwitchChat.Parser;

namespace TwitchChat.EventsArgs
{
	/// <summary>
	/// Provides data for the MessageReceived event.
	/// </summary>
	public class MessageReceivedEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MessageReceivedEventArgs"/> class.
		/// </summary>
		/// <param name="message">The message that was received.</param>
		public MessageReceivedEventArgs(TwitchMessage message)
		{
			Message = message;
		}

		/// <summary>
		/// Gets the message that was received.
		/// </summary>
		public TwitchMessage Message { get; }
	}
}