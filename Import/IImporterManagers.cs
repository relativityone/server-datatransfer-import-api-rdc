using kCura.WinEDDS.Core.Import.Managers;
using kCura.WinEDDS.Service;

namespace kCura.WinEDDS.Core.Import
{
	public interface IImporterManagers
	{
		IBulkImportManager BulkImportManager { get; }

		FolderManager FolderManager { get; }
	}
}