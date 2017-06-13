using System.Net;
using kCura.EDDS.WebAPI.BulkImportManagerBase;

namespace kCura.WinEDDS.Core.Import.Managers
{
	public interface IBulkImportManager
	{
		MassImportResults BulkImport(NativeLoadInfo loadInfo, ImportContext importContext);
		bool NativeRunHasErrors(int appId, string runId);
		ErrorFileKey GenerateNonImageErrorFiles(int appId, string runId, int artifactTypeId, bool writeHeader, int keyFieldId);
		ICredentials Credentials { get; }
		CookieContainer CookieContainer { get; }
	}
}