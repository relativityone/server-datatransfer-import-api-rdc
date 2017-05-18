using System;
using kCura.EDDS.WebAPI.BulkImportManagerBase;

namespace kCura.WinEDDS.Core.Import.Managers
{
	public class ObjectBulkImportManager : IBulkImportManager
	{
		private readonly BulkImportManager _bulkImportManager;
		private readonly ImportContext _importContext;

		public ObjectBulkImportManager(BulkImportManager bulkImportManager, ImportContext importContext)
		{
			_bulkImportManager = bulkImportManager;
			_importContext = importContext;
		}

		public MassImportResults BulkImport(NativeLoadInfo loadInfo)
		{
			var objectLoadInfo = loadInfo as ObjectLoadInfo;
			if (objectLoadInfo == null)
			{
				throw new Exception();
			}
			return _bulkImportManager.BulkImportObjects(_importContext.Args.CaseInfo.ArtifactID, objectLoadInfo, _importContext.Args.CopyFilesToDocumentRepository);
		}
	}
}