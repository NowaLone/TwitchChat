using IrcNet;
using IrcNet.Client;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using TwitchChat.Client;
using TwitchChat.Parser;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
	/// <summary>
	/// Extension methods for setting up Twitch client services in an <see cref="IServiceCollection" />.
	/// </summary>
	public static class TwitchClientServiceCollectionExtensions
	{
		/// <summary>
		/// Adds the Twitch client services to the specified <see cref="IServiceCollection" />.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection" /> to add the services to.</param>
		/// <param name="setupAction">An optional action to configure the <see cref="TwitchClient.Options" />.</param>
		/// <returns>The <see cref="IServiceCollection" /> so that additional calls can be chained.</returns>
		public static IServiceCollection AddTwitchClient(this IServiceCollection services, Action<TwitchClient.Options> setupAction = null)
		{
			services.TryAddTransient<ITwitchClient, TwitchClient>();
			services.TryAddTransient<IIrcParser<TwitchMessage>, TwitchParser>();
			services.TryAddTransient(s => s.GetRequiredService<IOptions<IrcClientWebSocket.Options>>().Value);
			services.AddIrcWebSocketClient(o => o.Uri = new Uri(TwitchClient.Options.wssUrlSSL));

			if (setupAction != null)
			{
				services.Configure(setupAction);
			}

			return services;
		}
	}
}