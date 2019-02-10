using System;

namespace kCura.Relativity.ImportAPI.IntegrationTests.Tests
{
	[Serializable]
	public class ImportApiTestException : Exception
	{
		public ImportApiTestException(): base()
		{
		
		}

		public ImportApiTestException(string message): base(message)
		{
		
		}

		public ImportApiTestException(string message, System.Exception innerException): base(message, innerException)
		{
		
		}

		protected ImportApiTestException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context): base(info, context)
		{
		
		}
	}
}
