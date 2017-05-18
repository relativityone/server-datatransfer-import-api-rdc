﻿using kCura.EDDS.WebAPI.BulkImportManagerBase;

namespace kCura.WinEDDS.Core.Import.Managers
{
	public class DocumentBulkImportManager : IBulkImportManager
	{
		private readonly BulkImportManager _bulkImportManager;
		private readonly ImportContext _importContext;

		public DocumentBulkImportManager(BulkImportManager bulkImportManager, ImportContext importContext)
		{
			_bulkImportManager = bulkImportManager;
			_importContext = importContext;
		}

		public MassImportResults BulkImport(NativeLoadInfo loadInfo)
		{
			return _bulkImportManager.BulkImportNative(_importContext.Settings.CaseInfo.ArtifactID, loadInfo, _importContext.Settings.CopyFilesToDocumentRepository,
				_importContext.Settings.FullTextColumnContainsFileLocation);
		}
	}
}