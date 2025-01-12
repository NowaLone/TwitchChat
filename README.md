# TwitchChat

TwitchChat is a .NET library for connecting and interacting with Twitch chat using the IRC protocol. It provides a client for handling connection, messaging, and channel management, as well as parsing Twitch chat messages.

## Download

[![Nuget](https://img.shields.io/nuget/v/NowaLone.TwitchChat.Client.svg?logo=nuget&label=NowaLone.TwitchChat.Client)](https://www.nuget.org/packages/NowaLone.TwitchChat.Client)
[![Nuget](https://img.shields.io/nuget/v/NowaLone.TwitchChat.Client.svg?logo=nuget&label=NowaLone.TwitchChat.Client)](https://www.nuget.org/packages/NowaLone.TwitchChat.Client)
[![Nuget](https://img.shields.io/nuget/v/NowaLone.TwitchChat.Client.Extensions.svg?logo=nuget&label=NowaLone.TwitchChat.Client.Extensions)](https://www.nuget.org/packages/NowaLone.TwitchChat.Client.Extensions)

## Features

- Connect to Twitch chat using WebSocket.
- Send and receive messages.
- Join and leave channels.
- Handle various Twitch chat events.
- Parse Twitch chat messages.

## Installation

You can install the TwitchChat library via NuGet:

```sh
dotnet add package NowaLone.TwitchChat.Client
dotnet add package NowaLone.TwitchChat.Parser
dotnet add package NowaLone.TwitchChat.Client.Extensions
```

## Usage

### Connecting to Twitch Chat

```csharp
using Microsoft.Extensions.DependencyInjection;
using TwitchChat.Client;

var services = new ServiceCollection();
services.AddTwitchClient(options =>
{
    options.OAuthToken = "your-oauth-token";
    options.Nickname = "your-nickname";
});

var provider = services.BuildServiceProvider();
var client = provider.GetRequiredService<ITwitchClient>();

await client.ConnectAsync();
```

### Sending a Message

```csharp
await client.SendMessageAsync("Hello, Twitch!");
```

### Joining a Channel

```csharp
client.JoinChannel("channel-name");
```

### Handling Events

```csharp
client.OnMessageReceived += (sender, e) =>
{
    Console.WriteLine($"Message received: {e.Message.Raw}");
};

client.OnConnected += (sender, e) =>
{
    Console.WriteLine($"Connected to {e.ConnectionUrl}");
};
```

## Building the Project

To build the project, run the following command:

```sh
dotnet build
```

## Running Tests

To run the tests, use the following command:

```sh
dotnet test
```
