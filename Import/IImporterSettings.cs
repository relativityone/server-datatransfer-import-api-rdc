using System.Collections.Generic;
using Relativity;
using ExecutionSource = Relativity.ExecutionSource;
using FieldInfo = kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo;
using ImportAuditLevel = kCura.EDDS.WebAPI.BulkImportManagerBase.ImportAuditLevel;

namespace kCura.WinEDDS.Core.Import
{
	public interface IImporterSettings
	{
		LoadFile LoadFile { get; }
		string RunId { get; }

		int KeyFieldId { get; }
		bool DisableUserSecurityCheck { get; }
		ImportAuditLevel AuditLevel { get; }
		int OverlayArtifactID { get; }
		int FolderId { get; }
		bool LinkDataGridRecords { get; }
		string BulkLoadFileFieldDelimiter { get; }
		int FilePathColumnIndex { get; }
		bool LoadImportedFullTextFromServer { get; }
		ExecutionSource ExecutionSource { get; }
		ImportOverwriteType Overwrite { get; }
		FieldInfo[] GetMappedFields(int artifactTypeId, IList<int> objectFieldIdListContainsArtifactId);
	}
}