using IrcNet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TwitchChat.Parser;

namespace TwitchChat.Client.Extensions.Tests
{
	[TestClass]
	[TestCategory(nameof(TwitchClientServiceCollectionExtensions))]
	public sealed class IrcWebSocketClientServiceCollectionExtensionsTests
	{
		private ServiceProvider provider;

		[TestCleanup]
		public void Cleanup()
		{
			provider.Dispose();
		}

		#region AddTwitchClient

		[TestMethod]
		[TestCategory(nameof(TwitchClientServiceCollectionExtensions.AddTwitchClient))]
		public void AddTwitchClient_WithSetupAction_ShouldAddServicesAndSetup()
		{
			// Arrange
			var serviceDescriptors = new ServiceCollection();
			var nickname = nameof(AddTwitchClient_WithSetupAction_ShouldAddServicesAndSetup);
			var action = new Action<TwitchClient.Options>((o) => o.Nickname = nickname);

			// Act
			provider = serviceDescriptors.AddTwitchClient(action).BuildServiceProvider();

			// Assert
			Assert.IsNotNull(provider.GetService<ITwitchClient>());
			Assert.IsNotNull(provider.GetServices<IIrcParser<TwitchMessage>>());
			Assert.IsNotNull(provider.GetService<IIrcClientWebSocket>());
			Assert.IsNotNull(provider.GetService<IOptionsMonitor<TwitchClient.Options>>());
			Assert.AreEqual(nickname, provider.GetService<IOptionsMonitor<TwitchClient.Options>>().CurrentValue.Nickname);
		}

		[TestMethod]
		[TestCategory(nameof(TwitchClientServiceCollectionExtensions.AddTwitchClient))]
		public void AddTwitchClient_WithoutSetupAction_ShouldAddServicesWithoutSetup()
		{
			// Arrange
			var serviceDescriptors = new ServiceCollection();

			// Act
			provider = serviceDescriptors.AddTwitchClient().BuildServiceProvider();

			// Assert
			Assert.IsNotNull(provider.GetService<ITwitchClient>());
			Assert.IsNotNull(provider.GetServices<IIrcParser<TwitchMessage>>());
			Assert.IsNotNull(provider.GetService<IIrcClientWebSocket>());
			Assert.IsNotNull(provider.GetService<IOptionsMonitor<TwitchClient.Options>>());
			Assert.AreEqual(new TwitchClient.Options().Nickname, provider.GetService<IOptionsMonitor<TwitchClient.Options>>().CurrentValue.Nickname);
		}

		#endregion AddTwitchClient
	}
}