
namespace kCura.WinEDDS.Core
{
	public class TransferConfig : ITransferConfig
	{
		public int ImportBatchSize => Config.ImportBatchSize;

		public bool DisableNativeValidation => Config.DisableNativeValidation;
		public int ConnectionTimeout => 30;
	}
}
