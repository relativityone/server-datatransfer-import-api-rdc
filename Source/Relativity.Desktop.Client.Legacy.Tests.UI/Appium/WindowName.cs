using System;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	internal abstract class WindowName
	{
		private readonly string value;

		protected WindowName(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentException("Argument cannot be null or empty.", nameof(value));
			}

			this.value = value;
		}

		public static implicit operator string(WindowName w)
		{
			return w.value;
		}

		public override string ToString()
		{
			return value;
		}

		public override bool Equals(object other)
		{
			var otherName = other as WindowName;
			return !ReferenceEquals(null, otherName)
			       && string.Equals(value, otherName.value);
		}

		public override int GetHashCode()
		{
			return value.GetHashCode();
		}

		public static bool operator ==(WindowName name1, WindowName name2)
		{
			if (ReferenceEquals(name1, name2))
			{
				return true;
			}

			return !ReferenceEquals(null, name1) && name1.Equals(name2);
		}

		public static bool operator !=(WindowName name1, WindowName name2)
		{
			return !(name1 == name2);
		}
	}
}