using System.Net;
using kCura.EDDS.WebAPI.BulkImportManagerBase;

namespace kCura.WinEDDS.Core.Import.Managers
{
	public class DocumentBulkImportManager : IBulkImportManager
	{
		private readonly BulkImportManager _bulkImportManager;

		public DocumentBulkImportManager(BulkImportManager bulkImportManager)
		{
			_bulkImportManager = bulkImportManager;
		}

		public MassImportResults BulkImport(NativeLoadInfo loadInfo, ImportContext importContext)
		{
			return _bulkImportManager.BulkImportNative(importContext.Settings.LoadFile.CaseInfo.ArtifactID, loadInfo, importContext.Settings.LoadFile.CopyFilesToDocumentRepository,
				importContext.Settings.LoadFile.FullTextColumnContainsFileLocation);
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