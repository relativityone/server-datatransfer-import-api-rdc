using kCura.EDDS.WebAPI.BulkImportManagerBase;
using Relativity;
using ExecutionSource = kCura.EDDS.WebAPI.BulkImportManagerBase.ExecutionSource;
using NativeLoadInfo = kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo;

namespace kCura.WinEDDS.Core.Import.Helpers
{
	public class NativeLoadInfoFactory : INativeLoadInfoFactory
	{
		private readonly ITransferConfig _transferConfig;
		private readonly ImportContext _importContext;

		public NativeLoadInfoFactory(ITransferConfig transferConfig, ImportContext importContext)
		{
			_transferConfig = transferConfig;
			_importContext = importContext;
		}

		public NativeLoadInfo Create(MetadataFilesInfo metadataFilesInfo)
		{
			var importerSettings = _importContext.Settings;

			var settings = new NativeLoadInfo();
			settings.DisableUserSecurityCheck = importerSettings.DisableUserSecurityCheck;
			settings.AuditLevel = importerSettings.AuditLevel;
			settings.OverlayArtifactID = importerSettings.OverlayArtifactID;
			settings.LinkDataGridRecords = importerSettings.LinkDataGridRecords;
			settings.UseBulkDataImport = true;
			settings.RunID = importerSettings.RunId;
			settings.KeyFieldArtifactID = importerSettings.KeyFieldId;
			settings.BulkLoadFileFieldDelimiter = importerSettings.BulkLoadFileFieldDelimiter;
			settings.MoveDocumentsInAppendOverlayMode = importerSettings.LoadFile.MoveDocumentsInAppendOverlayMode;
			settings.UploadFiles = importerSettings.FilePathColumnIndex != -1 && importerSettings.LoadFile.LoadNativeFiles;
			settings.LoadImportedFullTextFromServer = importerSettings.LoadImportedFullTextFromServer;
			settings.ExecutionSource = (ExecutionSource) importerSettings.ExecutionSource;
			settings.Billable = importerSettings.LoadFile.Billable;
			settings.Overlay = GetOverlayType(importerSettings.Overwrite);
			settings.OverlayBehavior = GetMassImportOverlayBehavior(importerSettings.LoadFile.OverlayBehavior);
			settings.MappedFields = importerSettings.GetMappedFields(importerSettings.LoadFile.ArtifactTypeID, importerSettings.LoadFile.ObjectFieldIdListContainsArtifactId);

			settings.CodeFileName = metadataFilesInfo.CodeFilePath.FileGuid;
			settings.DataFileName = metadataFilesInfo.NativeFilePath.FileGuid;
			settings.ObjectFileName = metadataFilesInfo.ObjectFilePath.FileGuid;
			settings.DataGridFileName = metadataFilesInfo.DataGridFilePath.FileGuid;

			//TODO
			settings.Repository = "TODO";
			if (string.IsNullOrEmpty(settings.Repository))
			{
				settings.Repository = importerSettings.LoadFile.CaseInfo.DocumentPath;
			}

			if (_transferConfig.CreateFoldersInWebAPI)
			{
				settings.RootFolderID = importerSettings.FolderId != 0 ? importerSettings.FolderId : -1;
			}
			else
			{
				settings.RootFolderID = 0;
			}

			return null;
		}

		private OverlayBehavior GetMassImportOverlayBehavior(LoadFile.FieldOverlayBehavior? behavior)
		{
			switch (behavior)
			{
				case LoadFile.FieldOverlayBehavior.MergeAll:
					return OverlayBehavior.MergeAll;
				case LoadFile.FieldOverlayBehavior.ReplaceAll:
					return OverlayBehavior.ReplaceAll;
				default:
					return OverlayBehavior.UseRelativityDefaults;
			}
		}

		private OverwriteType GetOverlayType(ImportOverwriteType importOverwriteType)
		{
			switch (importOverwriteType)
			{
				case ImportOverwriteType.Overlay:
					return OverwriteType.Overlay;
				case ImportOverwriteType.AppendOverlay:
					return OverwriteType.Both;
				default:
					return OverwriteType.Append;
			}
		}
	}
}