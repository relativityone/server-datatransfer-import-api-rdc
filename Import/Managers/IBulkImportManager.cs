using kCura.EDDS.WebAPI.BulkImportManagerBase;

namespace kCura.WinEDDS.Core.Import.Managers
{
	public interface IBulkImportManager
	{
		MassImportResults BulkImport(NativeLoadInfo loadInfo);
	}
}