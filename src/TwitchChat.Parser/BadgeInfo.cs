using System;
using System.Collections.Generic;

namespace TwitchChat.Parser
{
	/// <summary>
	/// Represents information about a badge in Twitch chat.
	/// </summary>
	public class BadgeInfo : IEquatable<BadgeInfo>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BadgeInfo"/> class.
		/// </summary>
		public BadgeInfo()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BadgeInfo"/> class with the specified name and months.
		/// </summary>
		/// <param name="name">The name of the badge.</param>
		/// <param name="months">The number of months associated with the badge.</param>
		public BadgeInfo(string name, int months)
		{
			Name = name;
			Months = months;
		}

		/// <summary>
		/// Gets or sets the name of the badge.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the number of months associated with the badge.
		/// </summary>
		public int Months { get; set; }

		/// <inheritdoc/>
		public override bool Equals(object obj)
		{
			return Equals(obj as BadgeInfo);
		}

		/// <inheritdoc/>
		public bool Equals(BadgeInfo other)
		{
			return !(other is null) &&
Name == other.Name &&
Months == other.Months;
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			int hashCode = 672087000;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
			hashCode = hashCode * -1521134295 + Months.GetHashCode();
			return hashCode;
		}

		/// <summary>
		/// Determines whether two specified instances of <see cref="BadgeInfo"/> are equal.
		/// </summary>
		/// <param name="left">The first <see cref="BadgeInfo"/> to compare.</param>
		/// <param name="right">The second <see cref="BadgeInfo"/> to compare.</param>
		/// <returns>true if the two <see cref="BadgeInfo"/> instances are equal; otherwise, false.</returns>
		public static bool operator ==(BadgeInfo left, BadgeInfo right)
		{
			return EqualityComparer<BadgeInfo>.Default.Equals(left, right);
		}

		/// <summary>
		/// Determines whether two specified instances of <see cref="BadgeInfo"/> are not equal.
		/// </summary>
		/// <param name="left">The first <see cref="BadgeInfo"/> to compare.</param>
		/// <param name="right">The second <see cref="BadgeInfo"/> to compare.</param>
		/// <returns>true if the two <see cref="BadgeInfo"/> instances are not equal; otherwise, false.</returns>
		public static bool operator !=(BadgeInfo left, BadgeInfo right)
		{
			return !(left == right);
		}
	}
}