namespace Relativity.DataExchange.Export.VolumeManagerV2
{
	using System;

	public partial class FileShareSettingsService
	{
		[Serializable]
        public class FileShareSettingsServiceException : Exception
        {
            public FileShareSettingsServiceException(string message): base(message)
            {
            }

            public FileShareSettingsServiceException(): base()
            {
            }

	        public FileShareSettingsServiceException(string message, System.Exception innerException) : base(message,
		        innerException)
	        {
	        }

	        protected FileShareSettingsServiceException(System.Runtime.Serialization.SerializationInfo info,
		        System.Runtime.Serialization.StreamingContext context) : base(info, context)
	        {
	        }
        }
	
	}
}