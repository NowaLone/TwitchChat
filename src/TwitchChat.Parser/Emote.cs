using System;
using System.Collections.Generic;

namespace TwitchChat.Parser
{
	/// <summary>
	/// Represents an emote in a Twitch chat message.
	/// </summary>
	public class Emote
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Emote"/> class.
		/// </summary>
		public Emote()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Emote"/> class with the specified ID and positions.
		/// </summary>
		/// <param name="id">The ID of the emote.</param>
		/// <param name="positions">The positions of the emote in the chat message.</param>
		public Emote(string id, IEnumerable<Position> positions)
		{
			Id = id;
			Positions = positions;
		}

		/// <summary>
		/// Gets or sets the ID of the emote.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the positions of the emote in the chat message.
		/// </summary>
		public IEnumerable<Position> Positions { get; set; }

		/// <summary>
		/// Represents the position of an emote in a chat message.
		/// </summary>
		public struct Position : IEquatable<Position>
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="Position"/> struct with the specified start and end positions.
			/// </summary>
			/// <param name="start">The start position of the emote.</param>
			/// <param name="end">The end position of the emote.</param>
			public Position(int start, int end)
			{
				Start = start;
				End = end;
			}

			/// <summary>
			/// Gets the start position of the emote.
			/// </summary>
			public int Start { get; }

			/// <summary>
			/// Gets the end position of the emote.
			/// </summary>
			public int End { get; }

			/// <inheritdoc/>
			public override bool Equals(object obj)
			{
				return obj is Position position && Equals(position);
			}

			/// <inheritdoc/>
			public bool Equals(Position other)
			{
				return Start == other.Start &&
End == other.End;
			}

			/// <inheritdoc/>
			public override int GetHashCode()
			{
				int hashCode = -1676728671;
				hashCode = hashCode * -1521134295 + Start.GetHashCode();
				hashCode = hashCode * -1521134295 + End.GetHashCode();
				return hashCode;
			}

			/// <summary>
			/// Determines whether two specified instances of <see cref="Position"/> are equal.
			/// </summary>
			/// <param name="left">The first <see cref="Position"/> to compare.</param>
			/// <param name="right">The second <see cref="Position"/> to compare.</param>
			/// <returns><see langword="true"/> if the two <see cref="Position"/> instances are equal; otherwise, <see langword="false"/>.</returns>
			public static bool operator ==(Position left, Position right)
			{
				return left.Equals(right);
			}

			/// <summary>
			/// Determines whether two specified instances of <see cref="Position"/> are not equal.
			/// </summary>
			/// <param name="left">The first <see cref="Position"/> to compare.</param>
			/// <param name="right">The second <see cref="Position"/> to compare.</param>
			/// <returns><see langword="true"/> if the two <see cref="Position"/> instances are not equal; otherwise, <see langword="false"/>.</returns>
			public static bool operator !=(Position left, Position right)
			{
				return !(left == right);
			}
		}
	}
}