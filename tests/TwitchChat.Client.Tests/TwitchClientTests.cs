using IrcNet;
using IrcNet.EventsArgs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwitchChat.Parser;

namespace TwitchChat.Client.Tests
{
	[TestClass]
	[TestCategory(nameof(TwitchClient))]
	public class TwitchClientTests
	{
		private ILoggerFactory loggerFactory;
		private Mock<IIrcClientWebSocket> mockIrcClient;
		private Mock<IIrcParser<TwitchMessage>> mockIrcParser;
		private Mock<IOptionsMonitor<TwitchClient.Options>> mockOptionsMonitor;
		private TwitchClient.Options options;
		private TwitchClient twitchClient;
		public TestContext TestContext { get; set; }

		[TestInitialize]
		public void Initialize()
		{
			loggerFactory = LoggerFactory.Create(c => c.AddDebug().SetMinimumLevel(LogLevel.Trace));
			mockIrcClient = new Mock<IIrcClientWebSocket>();
			mockIrcParser = new Mock<IIrcParser<TwitchMessage>>();
			mockOptionsMonitor = new Mock<IOptionsMonitor<TwitchClient.Options>>();
			options = new TwitchClient.Options
			{
				OAuthToken = "oauth:token",
				Nickname = "testuser",
				Capabilities = new List<string> { "twitch.tv/commands", "twitch.tv/membership", "twitch.tv/tags" }
			};
			mockOptionsMonitor.Setup(o => o.CurrentValue).Returns(options);
			twitchClient = new TwitchClient(mockIrcClient.Object, mockIrcParser.Object, mockOptionsMonitor.Object, loggerFactory.CreateLogger<TwitchClient>());

			// Setup mockIrcParser to return a TwitchMessage with the same Raw property as the input parameter
			mockIrcParser.Setup(p => p.ParseMessage(It.IsAny<string>()))
				.Returns((string raw) => new TwitchMessage { Raw = raw });

			// Set cancellation token timeout
			TestContext.CancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));
		}

		[TestCleanup]
		public void Cleanup()
		{
			loggerFactory.Dispose();
		}

		#region Constructor

		[TestMethod]
		[TestCategory(nameof(TwitchClient))]
		public void Constructor_ShouldUseNullLoggerIfLoggerIsNull()
		{
			// Arrange
			var client = new TwitchClient(mockIrcClient.Object, mockIrcParser.Object, mockOptionsMonitor.Object, null);

			// Act & Assert
			Assert.IsNotNull(client);
		}

		#endregion Constructor

		#region ConnectAsync

		[TestMethod]
		[TestCategory(nameof(TwitchClient.ConnectAsync))]
		public async Task ConnectAsync_ShouldConnectSuccessfully()
		{
			// Arrange
			mockIrcClient.Setup(c => c.OpenAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

			// Act
			await twitchClient.ConnectAsync(TestContext.CancellationTokenSource.Token);

			// Assert
			mockIrcClient.Verify(c => c.OpenAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		[TestCategory(nameof(TwitchClient.ConnectAsync))]
		public async Task ConnectAsync_ShouldThrowIfAlreadyConnected()
		{
			// Arrange
			mockIrcClient.Setup(c => c.IsConnected).Returns(true);

			// Act & Assert
			await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => twitchClient.ConnectAsync(TestContext.CancellationTokenSource.Token));
		}

		[DataTestMethod]
		[TestCategory(nameof(TwitchClient.ConnectAsync))]
		[DataRow(null)]
		[DataRow("")]
		[DataRow(" ")]
		[DataRow("	")]
		[DataRow("\t")]
		[DataRow("\n")]
		[DataRow("\r")]
		[DataRow("\r\n")]
		[DataRow("\t ")]
		[DataRow(" \t")]
		[DataRow("\n ")]
		[DataRow(" \n")]
		public async Task ConnectAsync_ShouldThrowIfNicknameIsInvalid(string nickname)
		{
			// Arrange
			options.Nickname = nickname;

			// Act & Assert
			await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => twitchClient.ConnectAsync(TestContext.CancellationTokenSource.Token));
		}

		#endregion ConnectAsync

		#region DisconnectAsync

		[TestMethod]
		[TestCategory(nameof(TwitchClient.DisconnectAsync))]
		public async Task DisconnectAsync_ShouldDisconnectSuccessfully()
		{
			// Arrange
			mockIrcClient.Setup(c => c.CloseAsync(It.IsAny<CloseMode>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
			mockIrcClient.Setup(c => c.IsConnected).Returns(true);

			// Act
			await twitchClient.DisconnectAsync(TestContext.CancellationTokenSource.Token);

			// Assert
			mockIrcClient.Verify(c => c.CloseAsync(It.IsAny<CloseMode>(), It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		[TestCategory(nameof(TwitchClient.DisconnectAsync))]
		public async Task DisconnectAsync_ShouldThrowIfAlreadyDisconnected()
		{
			// Arrange
			mockIrcClient.Setup(c => c.IsConnected).Returns(false);

			// Act & Assert
			await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => twitchClient.DisconnectAsync(TestContext.CancellationTokenSource.Token));
		}

		#endregion DisconnectAsync

		#region JoinChannel

		[TestMethod]
		[TestCategory(nameof(TwitchClient.JoinChannel))]
		public void JoinChannel_ShouldAddChannelsToQueue()
		{
			// Arrange
			var channels = new[] { "channel1", "#channel2" };

			// Act
			twitchClient.JoinChannel(channels);

			// Assert
			// Verify that channels are added to the join queue
		}

		#endregion JoinChannel

		#region PartChannelAsync

		[TestMethod]
		[TestCategory(nameof(TwitchClient.PartChannelAsync))]
		public async Task PartChannelAsync_ShouldPartChannelsSuccessfully()
		{
			// Arrange
			var channels = new[] { "channel1", "#channel2" };
			mockIrcClient.Setup(c => c.SendAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

			// Act
			await twitchClient.PartChannelAsync(TestContext.CancellationTokenSource.Token, channels);

			// Assert
			mockIrcClient.Verify(c => c.SendAsync(It.Is<string>(s => s.StartsWith("PART")), It.IsAny<CancellationToken>()), Times.Exactly(channels.Length));
		}

		[TestMethod]
		[TestCategory(nameof(TwitchClient.PartChannelAsync))]
		public async Task PartChannelAsync_WithListParams_ShouldPartChannelsSuccessfully()
		{
			// Arrange
			var channels = new[] { "channel1", "#channel2" }.ToList();
			mockIrcClient.Setup(c => c.SendAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

			// Act
			await twitchClient.PartChannelAsync(channels, TestContext.CancellationTokenSource.Token);

			// Assert
			mockIrcClient.Verify(c => c.SendAsync(It.Is<string>(s => s.StartsWith("PART")), It.IsAny<CancellationToken>()), Times.Exactly(channels.Count));
		}

		#endregion PartChannelAsync

		#region SendMessageAsync

		[TestMethod]
		[TestCategory(nameof(TwitchClient.SendMessageAsync))]
		public async Task SendMessageAsync_ShouldSendMessageSuccessfully()
		{
			// Arrange
			var message = "Hello, world!";
			mockIrcClient.Setup(c => c.SendAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

			// Act
			await twitchClient.SendMessageAsync(message, TestContext.CancellationTokenSource.Token);

			// Assert
			mockIrcClient.Verify(c => c.SendAsync(It.Is<string>(s => s == message), It.IsAny<CancellationToken>()), Times.Once);
		}

		#endregion SendMessageAsync

		#region Event Handlers

		#region OnConnected

		[TestMethod]
		[TestCategory(nameof(TwitchClient.OnConnected))]
		public async Task Client_OnConnected_ShouldInvokeOnConnectedEvent()
		{
			// Arrange
			EventsArgs.ConnectedEventArgs eventArgs = null;
			twitchClient.OnConnected += (sender, e) => eventArgs = e;
			var connectedEventArgs = new ConnectedEventArgs(new Uri(TwitchClient.Options.ircUrlSSL));

			// Act
			await twitchClient.ConnectAsync(TestContext.CancellationTokenSource.Token);
			mockIrcClient.Raise(c => c.OnConnected += null, connectedEventArgs);

			// Assert
			Assert.IsNotNull(eventArgs);
			Assert.IsNotNull(eventArgs.ConnectionUrl);
			Assert.AreEqual(TwitchClient.Options.ircUrlSSL, eventArgs.ConnectionUrl.OriginalString);
		}

		#endregion OnConnected

		#region OnDisconnected

		[TestMethod]
		[TestCategory(nameof(TwitchClient.OnDisconnected))]
		public async Task Client_OnDisconnected_ShouldInvokeOnDisconnectedEvent()
		{
			// Arrange
			EventsArgs.DisconnectedEventArgs eventArgs = null;
			twitchClient.OnDisconnected += (sender, e) => eventArgs = e;
			var connectedEventArgs = new ConnectedEventArgs(new Uri(TwitchClient.Options.wssUrl));
			var disconnectedEventArgs = new DisconnectedEventArgs(new Uri(TwitchClient.Options.ircUrlSSL));

			// Act
			await twitchClient.ConnectAsync(TestContext.CancellationTokenSource.Token);
			mockIrcClient.Raise(c => c.OnConnected += null, connectedEventArgs);
			mockIrcClient.Raise(c => c.OnDisconnected += null, disconnectedEventArgs);

			// Assert
			Assert.IsNotNull(eventArgs);
			Assert.IsNotNull(eventArgs.ConnectionUrl);
			Assert.AreEqual(TwitchClient.Options.ircUrlSSL, eventArgs.ConnectionUrl.OriginalString);
		}

		#endregion OnDisconnected

		#region OnMessageReceived

		[TestMethod]
		[TestCategory(nameof(TwitchClient.OnMessageReceived))]
		public async Task Client_OnMessageReceived_ShouldInvokeOnMessageReceivedEvent()
		{
			// Arrange
			EventsArgs.MessageReceivedEventArgs eventArgs = null;
			var raw = nameof(Client_OnMessageReceived_ShouldInvokeOnMessageReceivedEvent);
			twitchClient.OnMessageReceived += (sender, e) => eventArgs = e;
			var messageReceivedEventArgs = new MessageReceivedEventArgs(raw);

			// Act
			await twitchClient.ConnectAsync(TestContext.CancellationTokenSource.Token);
			mockIrcClient.Raise(c => c.OnMessageReceived += null, messageReceivedEventArgs);

			// Assert
			Assert.IsNotNull(eventArgs);
			Assert.IsNotNull(eventArgs.Message);
			Assert.AreEqual(raw, eventArgs.Message.Raw);
		}

		#endregion OnMessageReceived

		#region OnMessageSent

		[TestMethod]
		[TestCategory(nameof(TwitchClient.OnMessageSent))]
		public async Task Client_OnMessageSent_ShouldInvokeOnMessageSentEvent()
		{
			// Arrange
			EventsArgs.MessageSentEventArgs eventArgs = null;
			var raw = nameof(Client_OnMessageSent_ShouldInvokeOnMessageSentEvent);
			twitchClient.OnMessageSent += (sender, e) => eventArgs = e;
			var messageSentEventArgs = new MessageSentEventArgs(Encoding.UTF8.GetBytes(raw));

			// Act
			await twitchClient.ConnectAsync(TestContext.CancellationTokenSource.Token);
			mockIrcClient.Raise(c => c.OnMessageSent += null, messageSentEventArgs);

			// Assert
			Assert.IsNotNull(eventArgs);
			Assert.IsNotNull(eventArgs.Message);
			Assert.AreEqual(raw, eventArgs.Message.Raw);
		}

		#endregion OnMessageSent

		#region OnConnectionStateChanged

		[TestMethod]
		[TestCategory(nameof(TwitchClient.OnConnectionStateChanged))]
		public async Task Client_OnConnectionStateChanged_ShouldInvokeOnConnectionStateChangedEvent()
		{
			// Arrange
			EventsArgs.ConnectionStateChangedEventArgs eventArgs = null;
			var currentState = WebSocketState.Open;
			var lastState = WebSocketState.Connecting;
			twitchClient.OnConnectionStateChanged += (sender, e) => eventArgs = e;
			var connectionStateChangedEventArgs = new ConnectionStateChangedEventArgs(currentState, lastState);

			// Act
			await twitchClient.ConnectAsync(TestContext.CancellationTokenSource.Token);
			mockIrcClient.Raise(c => c.OnConnectionStateChanged += null, connectionStateChangedEventArgs);

			// Assert
			Assert.IsNotNull(eventArgs);
			Assert.AreEqual(currentState, eventArgs.CurrentState);
			Assert.AreEqual(lastState, eventArgs.LastState);
		}

		#endregion OnConnectionStateChanged

		#endregion Event Handlers
	}
}