
namespace kCura.WinEDDS.Core
{
	public interface ITransferConfig
	{
		int ImportBatchSize { get; }
		long ImportBatchMaxVolume { get; }
		bool DisableNativeValidation { get; }
		bool DisableNativeLocationValidation { get; }
		bool CreateErrorForEmptyNativeFile { get; }
		int ConnectionTimeout { get; }
		int IoErrorNumberOfRetries { get; }
		int IoErrorWaitTimeInSeconds { get; }
		int DefaultMaximumErrorCount { get; }
		bool CreateFoldersInWebAPI { get; }
		string RestUrl { get; }
		string ServicesUrl { get; }
		string BcpPathRootFolder { get; }
		string NativeFilesRootFolder { get; }
	}
}
