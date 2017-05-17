
namespace kCura.WinEDDS.Core
{
	public interface ITransferConfig
	{
		int ImportBatchSize { get; }
		bool DisableNativeValidation { get; }
		bool DisableNativeLocationValidation { get; }
		bool CreateErrorForEmptyNativeFile { get; }
		int ConnectionTimeout { get; }

		int DefaultMaximumErrorCount { get; }
	}
}
