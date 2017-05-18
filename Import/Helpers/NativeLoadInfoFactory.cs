using kCura.EDDS.WebAPI.BulkImportManagerBase;

namespace kCura.WinEDDS.Core.Import.Helpers
{
	public class NativeLoadInfoFactory : INativeLoadInfoFactory
	{
		private readonly ITransferConfig _transferConfig;
		private readonly ImportContext _importContext;

		public NativeLoadInfoFactory(ITransferConfig transferConfig, ImportContext importContext)
		{
			_transferConfig = transferConfig;
			_importContext = importContext;
		}

		public NativeLoadInfo Create(MetadataFilesInfo metadataFilesInfo)
		{
			return null;
		}
	}
}