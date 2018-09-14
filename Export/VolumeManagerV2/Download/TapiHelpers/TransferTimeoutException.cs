using System;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers
{
	[Serializable]
	public class TransferTimeoutException : Exception
	{
		public TimeSpan Timeout { get; }
		public TransferTimeoutException(TimeSpan timeout): base()
		{
			Timeout = timeout;
		}

		public TransferTimeoutException(TimeSpan timeout, string message): base(message)
		{
			Timeout = timeout;
		}

		public TransferTimeoutException(TimeSpan timeout, string message, Exception innerException): base(message, innerException)
		{
			Timeout = timeout;
		}

		protected TransferTimeoutException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context): base(info, context)
		{
		}
	}
}