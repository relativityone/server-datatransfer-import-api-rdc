
namespace kCura.WinEDDS.Core
{
	public class TransferConfig : ITransferConfig
	{
		public int ImportBatchSize => Config.ImportBatchSize;

		public bool DisableNativeValidation => Config.DisableNativeValidation;

		public bool DisableNativeLocationValidation => Config.DisableNativeLocationValidation;

		public bool CreateErrorForEmptyNativeFile => Config.CreateErrorForEmptyNativeFile;

		public int ConnectionTimeout => 30;
	}
}
