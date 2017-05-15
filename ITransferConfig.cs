
namespace kCura.WinEDDS.Core
{
	public interface ITransferConfig
	{
		int ImportBatchSize { get; }

		int ConnectionTimeout { get; }
	}
}
