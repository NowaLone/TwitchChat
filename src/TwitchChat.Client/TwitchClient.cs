using IrcNet;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TwitchChat.EventsArgs;
using TwitchChat.Parser;

namespace TwitchChat.Client
{
	/// <summary>
	/// Represents a client for connecting to Twitch chat.
	/// </summary>
	public class TwitchClient : ITwitchClient
	{
		#region Fields

		private readonly IIrcClientWebSocket client;
		private readonly IIrcParser<TwitchMessage> ircParser;
		private readonly IOptionsMonitor<Options> options;
		private readonly ILogger logger;

		private readonly HashSet<string> joinedChannelsSet;
		private readonly Queue<string> joinChannelQueue;

		private CancellationTokenSource joinQueuecancellationTokenSource;

		#endregion Fields

		#region Events

		/// <inheritdoc/>
		public event EventHandler<ConnectedEventArgs> OnConnected;

		/// <inheritdoc/>
		public event EventHandler<DisconnectedEventArgs> OnDisconnected;

		/// <inheritdoc/>
		public event EventHandler<MessageReceivedEventArgs> OnMessageReceived;

		/// <inheritdoc/>
		public event EventHandler<MessageReceivedEventArgs> OnAuthorized;

		/// <inheritdoc/>
		public event EventHandler<ConnectionStateChangedEventArgs> OnConnectionStateChanged;

		/// <inheritdoc/>
		public event EventHandler<MessageSentEventArgs> OnMessageSent;

		#endregion Events

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="TwitchClient"/> class.
		/// </summary>
		/// <param name="client">The IRC client web socket.</param>
		/// <param name="ircParser">The IRC parser.</param>
		/// <param name="options">The options monitor.</param>
		/// <param name="logger">The logger.</param>
		public TwitchClient(IIrcClientWebSocket client, IIrcParser<TwitchMessage> ircParser, IOptionsMonitor<Options> options, ILogger<TwitchClient> logger = default)
		{
			this.client = client;
			this.ircParser = ircParser;
			this.options = options;
			this.logger = logger ?? NullLogger<TwitchClient>.Instance;

			joinedChannelsSet = new HashSet<string>();
			joinChannelQueue = new Queue<string>();
		}

		#endregion Constructors

		#region Methods

		/// <inheritdoc/>
		public void JoinChannel(params string[] channels)
		{
			JoinChannel(channels.AsEnumerable());
		}

		/// <inheritdoc/>
		public void JoinChannel(IEnumerable<string> channels)
		{
			foreach (var channel in channels)
			{
				var correctChannel = channel.StartsWith("#", StringComparison.Ordinal) ? channel : '#' + channel;
				logger.LogInformation("Add channel {channel} to join queue.", channel);
				joinChannelQueue.Enqueue(correctChannel);
			}
		}

		/// <inheritdoc/>
		public Task PartChannelAsync(CancellationToken cancellationToken = default, params string[] channels)
		{
			return PartChannelAsync(channels.AsEnumerable(), cancellationToken);
		}

		/// <inheritdoc/>
		public async Task PartChannelAsync(IEnumerable<string> channels, CancellationToken cancellationToken = default)
		{
			foreach (var channel in channels)
			{
				var correctChannel = channel.StartsWith("#", StringComparison.Ordinal) ? channel : '#' + channel;
				logger.LogInformation("Leave from {channel}.", channel);
				joinedChannelsSet.Remove(channel);
				await client.SendAsync($"{IrcCommand.PART} {correctChannel}", cancellationToken).ConfigureAwait(false);
			}
		}

		/// <inheritdoc/>
		public Task SendMessageAsync(string message, CancellationToken cancellationToken = default)
		{
			return client.SendAsync(message, cancellationToken);
		}

		/// <inheritdoc/>
		public Task ConnectAsync(CancellationToken cancellationToken = default)
		{
			if (client.IsConnected)
			{
				throw new InvalidOperationException("Already connected, disconnect first.");
			}

			if (string.IsNullOrWhiteSpace(options.CurrentValue.Nickname))
			{
				throw new InvalidOperationException("Nickname cannot be null or whitespace.");
			}

			logger.LogInformation("Connecting...");

			client.OnConnected += Client_OnConnected;
			client.OnConnectionStateChanged += Client_OnConnectionStateChanged;
			client.OnDisconnected += Client_OnDisconnected;
			client.OnMessageReceived += Client_OnMessageReceived;
			client.OnMessageSent += Client_OnMessageSent;

			return client.OpenAsync(cancellationToken);
		}

		/// <inheritdoc/>
		public Task DisconnectAsync(CancellationToken cancellationToken = default)
		{
			if (!client.IsConnected)
			{
				throw new InvalidOperationException("Already disconnected, connect first.");
			}

			logger.LogInformation("Disconnecting...");

			client.OnConnected -= Client_OnConnected;
			client.OnConnectionStateChanged -= Client_OnConnectionStateChanged;
			client.OnDisconnected -= Client_OnDisconnected;
			client.OnMessageReceived -= Client_OnMessageReceived;
			client.OnMessageSent -= Client_OnMessageSent;

			return client.CloseAsync(CloseMode.Irc, cancellationToken);
		}

		private async Task JoinQueueAsync(CancellationToken cancellationToken = default)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				if (client.IsConnected && joinChannelQueue.Count > 0)
				{
					var channel = joinChannelQueue.Dequeue().ToLower();

					logger.LogInformation("Join to {channel}", channel);
					joinedChannelsSet.Add(channel);
					await client.SendAsync($"{IrcCommand.JOIN} {channel}", cancellationToken).ConfigureAwait(false);
				}
				else
				{
					await Task.Delay(200, cancellationToken).ConfigureAwait(false);
				}
			}
		}

		#region Handlers

		private async void Client_OnConnected(object sender, IrcNet.EventsArgs.ConnectedEventArgs e)
		{
			logger.LogInformation("Connected to {uri}.", e.ConnectionUrl.AbsoluteUri);

			var oauth = options.CurrentValue.OAuthToken = !options.CurrentValue.OAuthToken.StartsWith("oauth:")
					? options.CurrentValue.OAuthToken.StartsWith(":")
						? "oauth" + options.CurrentValue.OAuthToken
						: "oauth:" + options.CurrentValue.OAuthToken
					: options.CurrentValue.OAuthToken;

			var nickname = options.CurrentValue.Nickname.ToLower();

			await client.SendAsync($"{IrcCommand.PASS} {oauth}").ConfigureAwait(false);
			await client.SendAsync($"{IrcCommand.NICK} {nickname}").ConfigureAwait(false);

			foreach (var capability in options.CurrentValue.Capabilities)
			{
				await client.SendAsync($"CAP REQ {capability}").ConfigureAwait(false);
			}

			JoinChannel(joinedChannelsSet);
			joinQueuecancellationTokenSource = new CancellationTokenSource();
			JoinQueueAsync(joinQueuecancellationTokenSource.Token);

			OnConnected?.Invoke(this, new ConnectedEventArgs(e.ConnectionUrl));
		}

		private void Client_OnConnectionStateChanged(object sender, IrcNet.EventsArgs.ConnectionStateChangedEventArgs e)
		{
			OnConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(e.CurrentState, e.LastState));
		}

		private void Client_OnDisconnected(object sender, IrcNet.EventsArgs.DisconnectedEventArgs e)
		{
			logger.LogInformation("Disconnected from {uri}.", e.ConnectionUrl.AbsoluteUri);

			joinQueuecancellationTokenSource.Cancel();
			joinQueuecancellationTokenSource.Dispose();

			OnDisconnected?.Invoke(this, new DisconnectedEventArgs(e.ConnectionUrl));
		}

		private void Client_OnMessageReceived(object sender, IrcNet.EventsArgs.MessageReceivedEventArgs e)
		{
			foreach (var line in e.Message.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
			{
				if (!string.IsNullOrWhiteSpace(line))
				{
					var msg = ircParser.ParseMessage(line);

					OnMessageReceived?.Invoke(this, new MessageReceivedEventArgs(msg));

					switch (msg.Command)
					{
						case IrcCommand.PING:
							client.SendAsync($"{IrcCommand.PONG} {string.Join(" ", msg.Parameters)}").ConfigureAwait(false);
							break;

						case IrcCommand.RPL_WELCOME:
							logger.LogInformation("Authorized.");
							OnAuthorized?.Invoke(this, new MessageReceivedEventArgs(msg));
							break;
					}
				}
			}
		}

		private void Client_OnMessageSent(object sender, IrcNet.EventsArgs.MessageSentEventArgs e)
		{
			OnMessageSent?.Invoke(this, new MessageSentEventArgs(ircParser.ParseMessage(e.Message)));
		}

		#endregion Handlers

		#endregion Methods

		/// <summary>
		/// Represents the options for the <see cref="TwitchClient"/>.
		/// </summary>
		public class Options
		{
			/// <summary>
			/// Twitch IRC server WebSocket Non-SSL URI.
			/// </summary>
			public const string wssUrl = "ws://irc-ws.chat.twitch.tv:80";

			/// <summary>
			/// Twitch IRC server WebSocket SSL URI.
			/// </summary>
			public const string wssUrlSSL = "wss://irc-ws.chat.twitch.tv:443";

			/// <summary>
			/// Twitch IRC server IRC Non-SSL URI.
			/// </summary>
			public const string ircUrl = "irc://irc.chat.twitch.tv:6667";

			/// <summary>
			/// Twitch IRC server IRC SSL URI.
			/// </summary>
			public const string ircUrlSSL = "irc://irc.chat.twitch.tv:6697";

			/// <summary>
			/// OAuth token authorized through Twitch API.
			/// </summary>
			public string OAuthToken { get; set; } = string.Empty;

			/// <summary>
			/// Your Twitch nickname in lowercase.
			/// </summary>
			public string Nickname { get; set; } = "justinfan123";

			/// <summary>
			/// Twitch IRC Capabilities.
			/// </summary>
			public IEnumerable<string> Capabilities { get; set; } = new List<string> { "twitch.tv/commands", "twitch.tv/membership", "twitch.tv/tags", };
		}
	}
}