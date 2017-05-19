
namespace kCura.WinEDDS.Core
{
	public interface ITransferConfig
	{
		int ImportBatchSize { get; }
		bool DisableNativeValidation { get; }
		bool DisableNativeLocationValidation { get; }
		bool CreateErrorForEmptyNativeFile { get; }
		int ConnectionTimeout { get; }
		int IoErrorNumberOfRetries { get; }
		int IoErrorWaitTimeInSeconds { get; }
		int DefaultMaximumErrorCount { get; }
		bool CreateFoldersInWebAPI { get; }

	}
}
