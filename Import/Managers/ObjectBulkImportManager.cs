using System;
using System.Net;
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
				throw new ArgumentException("NativeLoadInfo can't be cast to ObjectLoadInfo.");
			}
			return _bulkImportManager.BulkImportObjects(importContext.Settings.LoadFile.CaseInfo.ArtifactID, objectLoadInfo,
				importContext.Settings.LoadFile.CopyFilesToDocumentRepository);
		}

		public bool NativeRunHasErrors(int appId, string runId)
		{
			return _bulkImportManager.NativeRunHasErrors(appId, runId);
		}

		public ErrorFileKey GenerateNonImageErrorFiles(int appId, string runId, int artifactTypeId, bool writeHeader, int keyFieldId)
		{
			return _bulkImportManager.GenerateNonImageErrorFiles(appId, runId, artifactTypeId, writeHeader, keyFieldId);
		}

		public ICredentials Credentials => _bulkImportManager.Credentials;
		public CookieContainer CookieContainer => _bulkImportManager.CookieContainer;
	}
}