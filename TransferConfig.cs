
namespace kCura.WinEDDS.Core
{
	public class TransferConfig : ITransferConfig
	{
		public int ImportBatchSize => Config.ImportBatchSize;
		public int ConnectionTimeout => 30;
	}
}
