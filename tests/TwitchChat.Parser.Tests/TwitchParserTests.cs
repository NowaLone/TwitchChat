using IrcNet;
using IrcNet.Parser.Rfc1459;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TwitchChat.Parser.Tests
{
	[TestClass]
	[TestCategory(nameof(TwitchParser))]
	public class TwitchParserTests
	{
		#region ParseMessage

		[DataTestMethod]
		[DynamicData(nameof(ClearchatTags))]
		[TestCategory(nameof(TwitchParser.ParseMessage))]
		public void ParseMessage_ClearchatTags_ShouldGetTagCorrectly(string message, TimeSpan? banDuration, int? roomId, int? targetUserId, DateTimeOffset? tmiSentTs)
		{
			// Arrange
			var parser = new TwitchParser();

			// Act
			var result = parser.ParseMessage(message);

			// Assert
			Assert.AreEqual(banDuration, result.BanDuration);
			Assert.AreEqual(roomId, result.RoomId);
			Assert.AreEqual(targetUserId, result.TargetUserId);
			Assert.AreEqual(tmiSentTs, result.TmiSentTs);
		}

		[DataTestMethod]
		[DynamicData(nameof(ClearmsgTags))]
		[TestCategory(nameof(TwitchParser.ParseMessage))]
		public void ParseMessage_ClearmsgTags_ShouldGetTagCorrectly(string message, string login, int? roomId, Guid? targetMsgId, DateTimeOffset? tmiSentTs)
		{
			// Arrange
			var parser = new TwitchParser();

			// Act
			var result = parser.ParseMessage(message);

			// Assert
			Assert.AreEqual(login, result.Login);
			Assert.AreEqual(roomId, result.RoomId);
			Assert.AreEqual(targetMsgId, result.TargetMsgId);
			Assert.AreEqual(tmiSentTs, result.TmiSentTs);
		}

		[DataTestMethod]
		[DynamicData(nameof(GlobaluserstateTags))]
		[TestCategory(nameof(TwitchParser.ParseMessage))]
		public void ParseMessage_GlobaluserstateTags_ShouldGetTagCorrectly(string message, BadgeInfo badgeInfo, List<Badge> badges, Color? color, string displayName, List<int> emoteSets, bool? turbo, int? userId, UserType? userType)
		{
			// Arrange
			var parser = new TwitchParser();

			// Act
			var result = parser.ParseMessage(message);

			// Assert
			Assert.AreEqual(badgeInfo, result.BadgeInfo);
			CollectionAssert.AreEquivalent(badges, result.Badges.ToList());
			Assert.AreEqual(color, result.Color);
			Assert.AreEqual(displayName, result.DisplayName);
			CollectionAssert.AreEqual(emoteSets, result.EmoteSets.ToList());
			Assert.AreEqual(turbo, result.Turbo);
			Assert.AreEqual(userId, result.UserId);
			Assert.AreEqual(userType, result.UserType);
		}

		[DataTestMethod]
		[DynamicData(nameof(NoticeTags))]
		[TestCategory(nameof(TwitchParser.ParseMessage))]
		public void ParseMessage_NoticeTags_ShouldGetTagCorrectly(string message, MessageId? messageId, int? targetUserId)
		{
			// Arrange
			var parser = new TwitchParser();

			// Act
			var result = parser.ParseMessage(message);

			// Assert
			Assert.AreEqual(messageId, result.MsgId);
			Assert.AreEqual(targetUserId, result.TargetUserId);
		}

		#endregion ParseMessage

		#region BuildMessage

		[TestMethod]
		[TestCategory(nameof(TwitchParser.BuildMessage))]
		public void BuildMessage_WithTags_ShouldReturnRawWithTags()
		{
			// Arrange
			var parser = new TwitchParser();

			var message = new TwitchMessage(new IrcMessage
			{
				Command = IrcCommand.PRIVMSG,
			})
			{
				Tags = new Dictionary<string, string>
				{
					{ "tag1", "value1" },
					{ "tag2", "value2" }
				},
			};

			// Act
			var result = parser.BuildMessage(message);

			// Assert
			Assert.AreEqual("@tag1=value1;tag2=value2 PRIVMSG\r\n", result);
		}

		[TestMethod]
		[TestCategory(nameof(TwitchParser.BuildMessage))]
		public void BuildMessage_WithEmptyTags_ShouldReturnRawWithoutTags()
		{
			// Arrange
			var parser = new TwitchParser();

			var message = new TwitchMessage();

			// Act
			var result = parser.BuildMessage(message);

			// Assert
			Assert.AreEqual("UNKNOWN\r\n", result);
		}

		[TestMethod]
		[TestCategory(nameof(TwitchParser.BuildMessage))]
		public void BuildMessage_JoinChannelCommand_ShouldReturnRawWithoutTags()
		{
			// Arrange
			var parser = new TwitchParser();

			var message = new TwitchMessage
			{
				Command = IrcCommand.JOIN,
				Parameters = new List<string>
				{
					"#twitch",
				}
			};

			// Act
			var result = parser.BuildMessage(message);

			// Assert
			Assert.AreEqual("JOIN #twitch\r\n", result);
		}

		[TestMethod]
		[TestCategory(nameof(TwitchParser.BuildMessage))]
		public void BuildMessage_PartChannelCommand_ShouldReturnRawWithoutTags()
		{
			// Arrange
			var parser = new TwitchParser();

			var message = new TwitchMessage
			{
				Command = IrcCommand.PART,
				Parameters = new List<string>
				{
					"#twitch",
				}
			};

			// Act
			var result = parser.BuildMessage(message);

			// Assert
			Assert.AreEqual("PART #twitch\r\n", result);
		}

		#endregion BuildMessage

		private static IEnumerable<object[]> ClearchatTags
		{
			get
			{
				return new[]
				{
					new object[]
					{
						"@room-id=12345678;target-user-id=87654321;tmi-sent-ts=1642715756806 :tmi.twitch.tv CLEARCHAT #dallas :ronni\r\n",
						null,
						12345678,
						87654321,
						DateTimeOffset.FromUnixTimeMilliseconds(1642715756806),
					},
					new object[]
					{
						"@room-id=12345678;tmi-sent-ts=1642715695392 :tmi.twitch.tv CLEARCHAT #dallas\r\n",
						null,
						12345678,
						null,
						DateTimeOffset.FromUnixTimeMilliseconds(1642715695392),
					},
					new object[]
					{
						"@ban-duration=350;room-id=12345678;target-user-id=87654321;tmi-sent-ts=1642719320727 :tmi.twitch.tv CLEARCHAT #dallas :ronni\r\n",
						TimeSpan.FromSeconds(350),
						12345678,
						87654321,
						DateTimeOffset.FromUnixTimeMilliseconds(1642719320727),
					},
				};
			}
		}

		private static IEnumerable<object[]> ClearmsgTags
		{
			get
			{
				return new[]
				{
					new object[]
					{
						"@login=ronni;room-id=;target-msg-id=770DFD1E-C3C4-4798-8005-C6B41553A698;tmi-sent-ts=1642720582342 :tmi.twitch.tv CLEARMSG #dallas :HeyGuys\r\n",
						"ronni",
						null,
						Guid.Parse("770DFD1E-C3C4-4798-8005-C6B41553A698"),
						DateTimeOffset.FromUnixTimeMilliseconds(1642720582342),
					},
				};
			}
		}

		private static IEnumerable<object[]> GlobaluserstateTags
		{
			get
			{
				return new[]
				{
					new object[]
					{
						"@badge-info=subscriber/8;badges=subscriber/6;color=#0D4200;display-name=dallas;emote-sets=0,33,50,237,793,2126,3517,4578,5569,9400,10337,12239;turbo=0;user-id=12345678;user-type=admin :tmi.twitch.tv GLOBALUSERSTATE\r\n",
						new BadgeInfo("subscriber", 8),
						new List<Badge>{ new Badge("subscriber", 6) },
						Color.FromArgb(0x0D,0x42,0x00),
						"dallas",
						new List<int> {0,33,50,237,793,2126,3517,4578,5569,9400,10337,12239},
						false,
						12345678,
						UserType.Admin
					},
				};
			}
		}

		private static IEnumerable<object[]> NoticeTags
		{
			get
			{
				return new[]
				{
					new object[]
					{
						"@msg-id=emote_only_off :tmi.twitch.tv NOTICE #bar :The message from foo is now deleted.\r\n",
						MessageId.Emote_Only_Off,
						null,
					},
					new object[]
					{
						"@msg-id=followers_on;target-user-id=12345678 :tmi.twitch.tv NOTICE #bar :Your settings prevent you from sending this whisper.\r\n",
						MessageId.Followers_On,
						12345678,
					},
				};
			}
		}
	}
}