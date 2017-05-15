
namespace kCura.WinEDDS.Core
{
	public interface ITransferConfig
	{
		int ImportBatchSize { get; }

		bool DisableNativeValidation { get; }

		int ConnectionTimeout { get; }
	}
}
