
namespace kCura.WinEDDS.Core
{
	public class TransferConfig : ITransferConfig
	{
		public int ImportBatchSize => Config.ImportBatchSize;

		public bool DisableNativeValidation => Config.DisableNativeValidation;

		public bool DisableNativeLocationValidation => Config.DisableNativeLocationValidation;

		public bool CreateErrorForEmptyNativeFile => Config.CreateErrorForEmptyNativeFile;

		public int IoErrorNumberOfRetries => kCura.Utility.Config.IOErrorNumberOfRetries;

		public int IoErrorWaitTimeInSeconds => kCura.Utility.Config.IOErrorWaitTimeInSeconds;

		public int ConnectionTimeout => 30;
		public int DefaultMaximumErrorCount => Config.DefaultMaximumErrorCount;
		public bool CreateFoldersInWebAPI => Config.CreateFoldersInWebAPI;
	}
}
