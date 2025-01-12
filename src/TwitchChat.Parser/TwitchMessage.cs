using IrcNet;
using IrcNet.Parser.V3;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TwitchChat.Parser
{
	/// <summary>
	/// Represents a Twitch message with various properties extracted from IRC tags.
	/// </summary>
	public class TwitchMessage : IrcV3Message, IIrcMessage
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TwitchMessage"/> class.
		/// </summary>
		public TwitchMessage() : base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TwitchMessage"/> class using an existing <see cref="IIrcMessage"/>.
		/// </summary>
		/// <param name="message">The <see cref="IIrcMessage"/> to initialize the instance with.</param>
		public TwitchMessage(IIrcMessage message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TwitchMessage"/> class using an existing <see cref="IrcV3Message"/>.
		/// </summary>
		/// <param name="message">The <see cref="IrcV3Message"/> to initialize the instance with.</param>
		public TwitchMessage(IrcV3Message message) : base(message)
		{
			Tags = message.Tags;
		}

		/// <summary>
		/// The message includes this tag if the user was put in a timeout instead of a ban. The tag contains the duration of the timeout, in seconds.
		/// </summary>
		public TimeSpan? BanDuration => Tags.TryGetValue("ban-duration", out var result) && double.TryParse(result, out var seconds) ? TimeSpan.FromSeconds(seconds) : default(TimeSpan?);

		/// <summary>
		/// The ID of the channel where the messages were removed from.
		/// </summary>
		public int? RoomId => Tags.TryGetValue("room-id", out var result) && int.TryParse(result, out var roomId) ? roomId : default(int?);

		/// <summary>
		/// The User ID of the user that was banned or put in a timeout.
		/// </summary>
		public int? TargetUserId => Tags.TryGetValue("target-user-id", out var result) && int.TryParse(result, out var targetUserId) ? targetUserId : default(int?);

		/// <summary>
		/// The UNIX timestamp.
		/// </summary>
		public DateTimeOffset? TmiSentTs => Tags.TryGetValue("tmi-sent-ts", out var result) && long.TryParse(result, out var milliseconds) ? DateTimeOffset.FromUnixTimeMilliseconds(milliseconds) : default(DateTimeOffset?);

		/// <summary>
		/// The name of the user who sent the message.
		/// </summary>
		public string Login => Tags.TryGetValue("login", out var result) ? result : null;

		/// <summary>
		/// The ID of the message. In UUID format.
		/// </summary>
		public Guid? TargetMsgId => Tags.TryGetValue("target-msg-id", out var result) && Guid.TryParse(result, out var id) ? id : default(Guid?);

		/// <summary>
		/// Contains metadata related to the chat badges in the badges tag.
		/// </summary>
		/// <remarks>Currently, this tag contains metadata only for subscriber badges, to indicate the number of months the user has been a subscriber.</remarks>
		public BadgeInfo BadgeInfo
		{
			get
			{
				if (Tags.TryGetValue("badge-info", out var result))
				{
					var parts = result.Split('/');
					return new BadgeInfo(parts[0], int.Parse(parts[1]));
				}
				return null;
			}
		}

		/// <summary>
		/// List of chat badges.
		/// </summary>
		/// <remarks>Most badges have only 1 version, but some badges like subscriber badges offer different versions of the badge depending on how long the user has subscribed.</remarks>
		public IEnumerable<Badge> Badges
		{
			get
			{
				if (Tags.TryGetValue("badges", out var result))
				{
					return result.Split(',').Select(badgeInfo =>
					{
						var parts = result.Split('/');
						return new Badge(parts[0], int.Parse(parts[1]));
					});
				}
				else
				{
					return Enumerable.Empty<Badge>();
				}
			}
		}

		/// <summary>
		/// The color of the user’s name in the chat room.
		/// </summary>
		public Color? Color => Tags.TryGetValue("color", out var result) ? System.Drawing.Color.FromArgb(int.Parse(result.Substring(1, 2), System.Globalization.NumberStyles.HexNumber), int.Parse(result.Substring(3, 2), System.Globalization.NumberStyles.HexNumber), int.Parse(result.Substring(5, 2), System.Globalization.NumberStyles.HexNumber)) : default(Color?);

		/// <summary>
		/// The user’s display name.
		/// </summary>
		public string DisplayName => Tags.TryGetValue("display-name", out var result) ? result : null;

		/// <summary>
		/// List of IDs that identify the emote sets that the user has access to. Is always set to at least zero (0).
		/// </summary>
		public IEnumerable<int> EmoteSets => Tags.TryGetValue("emote-sets", out var result) ? result.Split(',').Select(int.Parse) : Enumerable.Empty<int>();

		/// <summary>
		/// A Boolean value that indicates whether the user has site-wide commercial free mode enabled. Is <see langword="true"/> if enabled; otherwise, <see langword="false"/>.
		/// </summary>
		public bool? Turbo => Tags.TryGetValue("turbo", out var result) ? result == "1" : default(bool?);

		/// <summary>
		/// The User ID of the relevant user.
		/// </summary>
		public int? UserId => Tags.TryGetValue("user-id", out var result) && int.TryParse(result, out var userId) ? userId : default(int?);

		/// <summary>
		/// The type of user.
		/// </summary>
		public UserType? UserType => Tags.TryGetValue("user-type", out var result) && Enum.TryParse<UserType>(result, true, out var userType) ? userType : default(UserType?);

		/// <summary>
		/// An ID that you can use to programmatically determine the action’s outcome.
		/// </summary>
		public MessageId? MsgId => Tags.TryGetValue("msg-id", out var result) && Enum.TryParse<MessageId>(result, true, out var msgId) ? msgId : default(MessageId?);

		/// <summary>
		/// The amount of Bits the user cheered.
		/// </summary>
		/// <remarks>Only a Bits cheer message includes this tag.</remarks>
		public int? Bits => Tags.TryGetValue("bits", out var result) && int.TryParse(result, out var bits) ? bits : default(int?);

		/// <summary>
		/// List of emotes and their positions in the message.
		/// </summary>
		/// <remarks>The position indices are zero-based.</remarks>
		public IEnumerable<Emote> Emotes
		{
			get
			{
				if (Tags.TryGetValue("emotes", out var result))
				{
					return result.Split('/').Select(emote =>
					{
						var emoteIdAndPositions = emote.Split(':');
						var positions = emoteIdAndPositions[1].Split(',').Select(pos =>
						{
							var parts = pos.Split('-');
							return new Emote.Position(int.Parse(parts[0]), int.Parse(parts[1]));
						});

						return new Emote(emoteIdAndPositions[0], positions);
					});
				}
				else
				{
					return Enumerable.Empty<Emote>();
				}
			}
		}

		/// <summary>
		/// An ID that uniquely identifies the message.
		/// </summary>
		public Guid? Id => Tags.TryGetValue("id", out var result) && Guid.TryParse(result, out var id) ? id : default(Guid?);

		/// <summary>
		/// A Boolean value that determines whether the user is a moderator. Is <see langword="true"/> if the user is a moderator; otherwise, <see langword="false"/>.
		/// </summary>
		public bool? Mod => Tags.TryGetValue("mod", out var result) ? result == "1" : default(bool?);

		/// <summary>
		/// An ID that uniquely identifies the direct parent message that this message is replying to.
		/// </summary>
		/// <remarks>The message does not include this tag if this message is not a reply.</remarks>
		public Guid? ReplyParentMsgId => Tags.TryGetValue("reply-parent-msg-id", out var result) && Guid.TryParse(result, out var id) ? id : default(Guid?);

		/// <summary>
		/// An ID that identifies the sender of the direct parent message.
		/// </summary>
		/// <remarks>The message does not include this tag if this message is not a reply.</remarks>
		public int? ReplyParentUserId => Tags.TryGetValue("reply-parent-user-id", out var result) && int.TryParse(result, out var userId) ? userId : default(int?);

		/// <summary>
		/// The login name of the sender of the direct parent message.
		/// </summary>
		/// <remarks>The message does not include this tag if this message is not a reply.</remarks>
		public string ReplyParentUserLogin => Tags.TryGetValue("reply-parent-user-login", out var result) ? result : null;

		/// <summary>
		/// The display name of the sender of the direct parent message.
		/// </summary>
		/// <remarks>The message does not include this tag if this message is not a reply.</remarks>
		public string ReplyParentDisplayName => Tags.TryGetValue("reply-parent-display-name", out var result) ? result : null;

		/// <summary>
		/// The text of the direct parent message.
		/// </summary>
		/// <remarks>The message does not include this tag if this message is not a reply.</remarks>
		public string ReplyParentMsgBody => Tags.TryGetValue("reply-parent-msg-body", out var result) ? result : null;

		/// <summary>
		/// An ID that uniquely identifies the top-level parent message of the reply thread that this message is replying to.
		/// </summary>
		/// <remarks>The message does not include this tag if this message is not a reply.</remarks>
		public MessageId? ReplyThreadParentMsgId => Tags.TryGetValue("reply-thread-parent-msg-id", out var result) && Enum.TryParse<MessageId>(result, true, out var msgId) ? msgId : default(MessageId?);

		/// <summary>
		/// The login name of the sender of the top-level parent message.
		/// </summary>
		/// <remarks>The message does not include this tag if this message is not a reply.</remarks>
		public string ReplyThreadParentUserLogin => Tags.TryGetValue("reply-thread-parent-user-login", out var result) ? result : null;

		/// <summary>
		/// List of chat badges for the chatter in the room the message was sent from.
		/// </summary>
		/// <remarks>This uses the same format as the <see cref="Badges"/>.</remarks>
		public IEnumerable<Badge> SourceBadges
		{
			get
			{
				if (Tags.TryGetValue("source-badges", out var result))
				{
					return result.Split(',').Select(badgeInfo =>
					{
						var parts = result.Split('/');
						return new Badge(parts[0], int.Parse(parts[1]));
					});
				}
				else
				{
					return Enumerable.Empty<Badge>();
				}
			}
		}

		/// <summary>
		/// Contains metadata related to the chat badges in the <see cref="SourceBadges"/>.
		/// </summary>
		public BadgeInfo SourceBadgeInfo
		{
			get
			{
				if (Tags.TryGetValue("source-badge-info", out var result))
				{
					var parts = result.Split('/');
					return new BadgeInfo(parts[0], int.Parse(parts[1]));
				}
				return null;
			}
		}

		/// <summary>
		/// A UUID that identifies the source message from the channel the message was sent from.
		/// </summary>
		public Guid? SourceId => Tags.TryGetValue("source-id", out var result) && Guid.TryParse(result, out var id) ? id : default(Guid?);

		/// <summary>
		/// An ID that identifies the chat room (channel) the message was sent from.
		/// </summary>
		public int? SourceRoomId => Tags.TryGetValue("source-room-id", out var result) && int.TryParse(result, out var roomId) ? roomId : default(int?);

		/// <summary>
		/// The value of <see cref="MsgId"/>> from the USERNOTICE sent to the source channel.
		/// </summary>
		public MessageId? SourceMsgId => Tags.TryGetValue("source-msg-id", out var result) && Enum.TryParse<MessageId>(result, true, out var msgId) ? msgId : default(MessageId?);

		/// <summary>
		/// A Boolean value that determines whether the user is a subscriber. Is <see langword="true"/> if the user is a subscriber; otherwise, <see langword="false"/>.
		/// </summary>
		public bool? Subscriber => Tags.TryGetValue("subscriber", out var result) ? result == "1" : default(bool?);

		/// <summary>
		/// A Boolean value that determines whether the user that sent the chat is a VIP.
		/// </summary>
		/// <remarks>The message includes this tag if the user is a VIP; otherwise, the message doesn’t include this tag (check for the presence of the tag instead of whether the tag is set to <see langword="true"/> or <see langword="false"/>).</remarks>
		public bool? Vip => Tags.TryGetValue("vip", out var result) ? result == "1" : default(bool?);

		/// <summary>
		/// A Boolean value that determines whether the chat room allows only messages with emotes. Is <see langword="true"/> if only emotes are allowed; otherwise, <see langword="false"/>.
		/// </summary>
		public bool? EmoteOnly => Tags.TryGetValue("emote-only", out var result) ? result == "1" : default(bool?);

		/// <summary>
		/// An integer value that determines whether only followers can post messages in the chat room.
		/// </summary>
		/// <remarks>The value indicates how long, in minutes, the user must have followed the broadcaster before posting chat messages. If the value is -1, the chat room is not restricted to followers only.</remarks>
		public int? FollowersOnly => Tags.TryGetValue("followers-only", out var result) && int.TryParse(result, out var roomId) ? roomId : default(int?);

		/// <summary>
		/// A Boolean value that determines whether a user’s messages must be unique. Applies only to messages with more than 9 characters. Is <see langword="true"/> if users must post unique messages; otherwise, <see langword="false"/>.
		/// </summary>
		public bool? R9k => Tags.TryGetValue("r9k", out var result) ? result == "1" : default(bool?);

		/// <summary>
		/// An integer value that determines how long, in seconds, users must wait between sending messages.
		/// </summary>
		public int? Slow => Tags.TryGetValue("slow", out var result) && int.TryParse(result, out var roomId) ? roomId : default(int?);

		/// <summary>
		/// A Boolean value that determines whether only subscribers and moderators can chat in the chat room. Is <see langword="true"/> if only subscribers and moderators can chat; otherwise, <see langword="false"/>.
		/// </summary>
		public bool? SubsOnly => Tags.TryGetValue("subs-only", out var result) ? result == "1" : default(bool?);

		/// <summary>
		/// The message Twitch shows in the chat room for this notice.
		/// </summary>
		public string SystemMsg => Tags.TryGetValue("system-msg", out var result) ? result : null;

		/// <summary>
		/// The total number of months the user has subscribed. This is the same as <see cref="MsgParamMonths"/> but sent for different types of user notices.
		/// </summary>
		/// <remarks>Included only with sub and resub notices.</remarks>
		public int? MsgParamCumulativeMonths => Tags.TryGetValue("msg-param-cumulative-months", out var result) && int.TryParse(result, out var roomId) ? roomId : default(int?);

		/// <summary>
		/// The display name of the broadcaster raiding this channel.
		/// </summary>
		/// <remarks>Included only with raid notices.</remarks>
		public string MsgParamDisplayName => Tags.TryGetValue("msg-param-displayName", out var result) ? result : null;

		/// <summary>
		///The login name of the broadcaster raiding this channel.
		/// </summary>
		/// <remarks>Included only with raid notices.</remarks>
		public string MsgParamLogin => Tags.TryGetValue("msg-param-login", out var result) ? result : null;

		/// <summary>
		/// The total number of months the user has subscribed. This is the same as <see cref="MsgParamCumulativeMonths"/> but sent for different types of user notices.
		/// </summary>
		/// <remarks>Included only with subgift notices.</remarks>
		public int? MsgParamMonths => Tags.TryGetValue("msg-param-months", out var result) && int.TryParse(result, out var roomId) ? roomId : default(int?);

		/// <summary>
		/// The number of gifts the gifter has given during the promo indicated by <see cref="MsgParamPromoName"/>.
		/// </summary>
		/// <remarks>Included only with anongiftpaidupgrade and giftpaidupgrade notices.</remarks>
		public int? MsgParamPromoGiftTotal => Tags.TryGetValue("msg-param-promo-gift-total", out var result) && int.TryParse(result, out var roomId) ? roomId : default(int?);

		/// <summary>
		/// The subscriptions promo, if any, that is ongoing (for example, Subtember 2018).
		/// </summary>
		/// <remarks>Included only with anongiftpaidupgrade and giftpaidupgrade notices.</remarks>
		public string MsgParamPromoName => Tags.TryGetValue("msg-param-promo-name", out var result) ? result : null;

		/// <summary>
		/// The display name of the subscription gift recipient.
		/// </summary>
		/// <remarks>Included only with subgift notices.</remarks>
		public string MsgParamRecipientDisplayName => Tags.TryGetValue("msg-param-recipient-display-name", out var result) ? result : null;

		/// <summary>
		/// The user ID of the subscription gift recipient.
		/// </summary>
		/// <remarks>Included only with subgift notices.</remarks>
		public int? MsgParamRecipientId => Tags.TryGetValue("msg-param-recipient-id", out var result) && int.TryParse(result, out var userId) ? userId : default(int?);

		/// <summary>
		/// The user name of the subscription gift recipient.
		/// </summary>
		/// <remarks>Included only with subgift notices.</remarks>
		public string MsgParamRecipientUserName => Tags.TryGetValue("msg-param-recipient-user-name", out var result) ? result : null;

		/// <summary>
		/// The login name of the user who gifted the subscription.
		/// </summary>
		/// <remarks>Included only with giftpaidupgrade notices.</remarks>
		public string MsgParamSenderLogin => Tags.TryGetValue("msg-param-sender-login", out var result) ? result : null;

		/// <summary>
		/// The display name of the user who gifted the subscription.
		/// </summary>
		/// <remarks>Include only with giftpaidupgrade notices.</remarks>
		public string MsgParamSenderName => Tags.TryGetValue("msg-param-sender-name", out var result) ? result : null;

		/// <summary>
		/// A Boolean value that indicates whether the user wants their streaks shared.
		/// </summary>
		/// <remarks>Included only with sub and resub notices.</remarks>
		public bool? MsgParamShouldShareStreak => Tags.TryGetValue("msg-param-should-share-streak", out var result) ? result == "1" : default(bool?);

		/// <summary>
		/// The number of consecutive months the user has subscribed. This is zero (0) if <see cref="MsgParamShouldShareStreak"/>> is 0.
		/// </summary>
		/// <remarks>Included only with sub and resub notices.</remarks>
		public int? MsgParamStreakMonths => Tags.TryGetValue("msg-param-streak-months", out var result) && int.TryParse(result, out var userId) ? userId : default(int?);

		/// <summary>
		/// The type of subscription plan being used.
		/// </summary>
		/// <remarks>Included only with sub, resub and subgift notices.</remarks>
		public SubPlan? MsgParamSubPlan => Tags.TryGetValue("msg-param-sub-plan", out var result) && Enum.TryParse<SubPlan>(result, true, out var subPlan) ? subPlan : default(SubPlan?);

		/// <summary>
		/// The display name of the subscription plan. This may be a default name or one created by the channel owner.
		/// </summary>
		/// <remarks>Included only with sub, resub, and subgift notices.</remarks>
		public string MsgParamSubPlanName => Tags.TryGetValue("msg-param-sub-plan-name", out var result) ? result : null;

		/// <summary>
		/// The number of viewers raiding this channel from the broadcaster’s channel.
		/// </summary>
		/// <remarks>Included only with raid notices.</remarks>
		public int? MsgParamViewerCount => Tags.TryGetValue("msg-param-viewerCount", out var result) && int.TryParse(result, out var userId) ? userId : default(int?);

		/// <summary>
		/// The tier of the Bits badge the user just earned. For example, 100, 1000, or 10000.
		/// </summary>
		/// <remarks>Included only with bitsbadgetier notices.</remarks>
		public int? MsgParamThreshold => Tags.TryGetValue("msg-param-threshold", out var result) && int.TryParse(result, out var userId) ? userId : default(int?);

		/// <summary>
		/// The number of months gifted as part of a single, multi-month gift.
		/// </summary>
		/// <remarks>Included only with subgift notices.</remarks>
		public int? MsgParamGiftMonths => Tags.TryGetValue("msg-param-gift-months", out var result) && int.TryParse(result, out var userId) ? userId : default(int?);
	}
}