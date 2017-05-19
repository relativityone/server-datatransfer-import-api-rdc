using System;
using kCura.EDDS.WebAPI.BulkImportManagerBase;

namespace kCura.WinEDDS.Core.Import.Managers
{
	public class ObjectBulkImportManager : IBulkImportManager
	{
		private readonly BulkImportManager _bulkImportManager;

		public ObjectBulkImportManager(BulkImportManager bulkImportManager)
		{
			_bulkImportManager = bulkImportManager;
		}

		public MassImportResults BulkImport(NativeLoadInfo loadInfo, ImportContext importContext)
		{
			var objectLoadInfo = loadInfo as ObjectLoadInfo;
			if (objectLoadInfo == null)
			{
				throw new Exception();
			}
			return _bulkImportManager.BulkImportObjects(importContext.Settings.LoadFile.CaseInfo.ArtifactID, objectLoadInfo, 
				importContext.Settings.LoadFile.CopyFilesToDocumentRepository);
		}
	}
}