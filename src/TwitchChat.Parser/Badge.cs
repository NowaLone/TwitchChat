using System;
using System.Collections.Generic;

namespace TwitchChat.Parser
{
	/// <summary>
	/// Represents a badge in the Twitch chat.
	/// </summary>
	public class Badge : IEquatable<Badge>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Badge"/> class.
		/// </summary>
		public Badge()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Badge"/> class with the specified name and version.
		/// </summary>
		/// <param name="name">The name of the badge.</param>
		/// <param name="version">The version of the badge.</param>
		public Badge(string name, int version)
		{
			Name = name;
			Version = version;
		}

		/// <summary>
		/// Gets or sets the name of the badge.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the version of the badge.
		/// </summary>
		public int Version { get; set; }

		/// <inheritdoc/>
		public override bool Equals(object obj)
		{
			return Equals(obj as Badge);
		}

		/// <inheritdoc/>
		public bool Equals(Badge other)
		{
			return !(other is null) &&
Name == other.Name &&
Version == other.Version;
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			int hashCode = 2112831277;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
			hashCode = hashCode * -1521134295 + Version.GetHashCode();
			return hashCode;
		}

		/// <summary>
		/// Determines whether two specified instances of <see cref="Badge"/> are equal.
		/// </summary>
		/// <param name="left">The first badge to compare.</param>
		/// <param name="right">The second badge to compare.</param>
		/// <returns>true if the two badges are equal; otherwise, false.</returns>
		public static bool operator ==(Badge left, Badge right)
		{
			return EqualityComparer<Badge>.Default.Equals(left, right);
		}

		/// <summary>
		/// Determines whether two specified instances of <see cref="Badge"/> are not equal.
		/// </summary>
		/// <param name="left">The first badge to compare.</param>
		/// <param name="right">The second badge to compare.</param>
		/// <returns>true if the two badges are not equal; otherwise, false.</returns>
		public static bool operator !=(Badge left, Badge right)
		{
			return !(left == right);
		}
	}
}