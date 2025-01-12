using IrcNet;
using IrcNet.Parser.V3;

namespace TwitchChat.Parser
{
	/// <summary>
	/// Parses and builds Twitch messages using the IRC V3 protocol.
	/// </summary>
	public class TwitchParser : IrcV3Parser, IIrcParser<TwitchMessage>
	{
		/// <summary>
		/// Builds a message from a TwitchMessage object.
		/// </summary>
		/// <param name="message">The TwitchMessage object to build the message from.</param>
		/// <param name="useNumeric">Indicates whether to use numeric values in the message.</param>
		/// <returns>The built message as a string.</returns>
		public string BuildMessage(TwitchMessage message, bool useNumeric = false)
		{
			return base.BuildMessage(message, useNumeric);
		}

		/// <summary>
		/// Parses a message string into a TwitchMessage object.
		/// </summary>
		/// <param name="message">The message string to parse.</param>
		/// <returns>The parsed TwitchMessage object.</returns>
		public new TwitchMessage ParseMessage(string message)
		{
			var ircMessage = base.ParseMessage(message);

			return new TwitchMessage(ircMessage);
		}
	}
}