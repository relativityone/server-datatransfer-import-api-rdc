using kCura.EDDS.WebAPI.BulkImportManagerBase;

namespace kCura.WinEDDS.Core.Import.Helpers
{
	public interface INativeLoadInfoFactory
	{
		NativeLoadInfo Create(MetadataFilesInfo metadataFilesInfo);
	}
}