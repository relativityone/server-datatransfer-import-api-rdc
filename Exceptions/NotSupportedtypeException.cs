using System;

namespace kCura.WinEDDS.Core.Exceptions
{
	public class NotSupportedTypeException : Exception
	{
		public NotSupportedTypeException(string message) : base(message)
		{
		}
	}
}
