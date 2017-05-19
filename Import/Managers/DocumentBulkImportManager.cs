using kCura.EDDS.WebAPI.BulkImportManagerBase;

namespace kCura.WinEDDS.Core.Import.Managers
{
	public class DocumentBulkImportManager : IBulkImportManager
	{
		private readonly BulkImportManager _bulkImportManager;
		private readonly ImportContext _importContext;

		public DocumentBulkImportManager(BulkImportManager bulkImportManager)
		{
			_bulkImportManager = bulkImportManager;
		}

		public MassImportResults BulkImport(NativeLoadInfo loadInfo, ImportContext importContext)
		{
			return _bulkImportManager.BulkImportNative(_importContext.Settings.LoadFile.CaseInfo.ArtifactID, loadInfo, _importContext.Settings.LoadFile.CopyFilesToDocumentRepository,
				_importContext.Settings.LoadFile.FullTextColumnContainsFileLocation);
		}
	}
}