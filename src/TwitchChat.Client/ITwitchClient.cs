using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TwitchChat.EventsArgs;

namespace TwitchChat.Client
{
	/// <summary>
	/// Interface for Twitch client to handle connection, messaging, and channel management.
	/// </summary>
	public interface ITwitchClient
	{
		/// <summary>
		/// Event triggered when the client is authorized.
		/// </summary>
		event EventHandler<MessageReceivedEventArgs> OnAuthorized;

		/// <summary>
		/// Event triggered when the client is connected.
		/// </summary>
		event EventHandler<ConnectedEventArgs> OnConnected;

		/// <summary>
		/// Event triggered when the connection state changes.
		/// </summary>
		event EventHandler<ConnectionStateChangedEventArgs> OnConnectionStateChanged;

		/// <summary>
		/// Event triggered when the client is disconnected.
		/// </summary>
		event EventHandler<DisconnectedEventArgs> OnDisconnected;

		/// <summary>
		/// Event triggered when a message is received.
		/// </summary>
		event EventHandler<MessageReceivedEventArgs> OnMessageReceived;

		/// <summary>
		/// Event triggered when a message is sent.
		/// </summary>
		event EventHandler<MessageSentEventArgs> OnMessageSent;

		/// <summary>
		/// Connects the client asynchronously.
		/// </summary>
		/// <param name="cancellationToken">Token to cancel the operation.</param>
		/// <returns>A task representing the asynchronous operation.</returns>
		Task ConnectAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// Disconnects the client asynchronously.
		/// </summary>
		/// <param name="cancellationToken">Token to cancel the operation.</param>
		/// <returns>A task representing the asynchronous operation.</returns>
		Task DisconnectAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// Joins the specified channels.
		/// </summary>
		/// <param name="channels">The channels to join.</param>
		void JoinChannel(IEnumerable<string> channels);

		/// <summary>
		/// Joins the specified channels.
		/// </summary>
		/// <param name="channels">The channels to join.</param>
		void JoinChannel(params string[] channels);

		/// <summary>
		/// Parts the specified channels asynchronously.
		/// </summary>
		/// <param name="channels">The channels to part.</param>
		/// <param name="cancellationToken">Token to cancel the operation.</param>
		/// <returns>A task representing the asynchronous operation.</returns>
		Task PartChannelAsync(IEnumerable<string> channels, CancellationToken cancellationToken = default);

		/// <summary>
		/// Parts the specified channels asynchronously.
		/// </summary>
		/// <param name="cancellationToken">Token to cancel the operation.</param>
		/// <param name="channels">The channels to part.</param>
		/// <returns>A task representing the asynchronous operation.</returns>
		Task PartChannelAsync(CancellationToken cancellationToken = default, params string[] channels);

		/// <summary>
		/// Sends a message asynchronously.
		/// </summary>
		/// <param name="message">The message to send.</param>
		/// <param name="cancellationToken">Token to cancel the operation.</param>
		/// <returns>A task representing the asynchronous operation.</returns>
		Task SendMessageAsync(string message, CancellationToken cancellationToken = default);
	}
}