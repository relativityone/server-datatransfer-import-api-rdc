using Relativity.DataExchange;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	internal abstract class WindowName
	{
		private readonly string value;

		protected WindowName(string value)
		{
			
			this.value = value.ThrowIfNullOrEmpty(nameof(value));
		}

		public static implicit operator string(WindowName windowName)
		{
			return windowName.value;
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