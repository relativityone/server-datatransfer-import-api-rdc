Imports RelativityDataTransferLegacySDK = Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models
Imports RelativityDataExchange = Relativity.DataExchange.Service

Namespace kCura.WinEDDS.Mapping

	Public Class KeplerTypeMapper

		Public Shared Function Map(source As kCura.EDDS.WebAPI.AuditManagerBase.ImageImportStatistics) As RelativityDataTransferLegacySDK.ImageImportStatistics
			If source Is Nothing Then
				Return Nothing
			End If
			
			Return New RelativityDataTransferLegacySDK.ImageImportStatistics() With {
				.BatchSizes = source.BatchSizes,
				.RepositoryConnection = CType([Enum].Parse(GetType(RelativityDataTransferLegacySDK.RepositoryConnectionType), source.RepositoryConnection.ToString()), RelativityDataTransferLegacySDK.RepositoryConnectionType),
				.Overwrite = CType([Enum].Parse(GetType(RelativityDataTransferLegacySDK.OverwriteType), source.Overwrite.ToString()), RelativityDataTransferLegacySDK.OverwriteType),
				.OverlayIdentifierFieldArtifactID = source.OverlayIdentifierFieldArtifactID,
				.DestinationFolderArtifactID = source.DestinationFolderArtifactID,
				_
				.LoadFileName = source.LoadFileName,
				.StartLine = source.StartLine,
				.FilesCopiedToRepository = source.FilesCopiedToRepository,
				.TotalFileSize = source.TotalFileSize,
				.TotalMetadataBytes = source.TotalMetadataBytes,
				_
				.NumberOfDocumentsCreated = source.NumberOfDocumentsCreated,
				.NumberOfDocumentsUpdated = source.NumberOfDocumentsUpdated,
				.NumberOfFilesLoaded = source.NumberOfFilesLoaded,
				.NumberOfErrors = source.NumberOfErrors,
				.NumberOfWarnings = source.NumberOfWarnings,
				_
				.RunTimeInMilliseconds = source.RunTimeInMilliseconds,
				.SendNotification = source.SendNotification,
				.OverlayBehavior = If(source.OverlayBehavior Is Nothing, Nothing, CType([Enum].Parse(GetType(RelativityDataTransferLegacySDK.OverlayBehavior), source.OverlayBehavior.ToString()), RelativityDataTransferLegacySDK.OverlayBehavior?)),
				.ExtractedTextReplaced = source.ExtractedTextReplaced,
				.SupportImageAutoNumbering = source.SupportImageAutoNumbering,
				_
				.DestinationProductionArtifactID = source.DestinationProductionArtifactID,
				.ExtractedTextDefaultEncodingCodePageID = source.ExtractedTextDefaultEncodingCodePageID
			}
		End Function

		Public Shared Function Map(source As kCura.EDDS.WebAPI.AuditManagerBase.ObjectImportStatistics) As RelativityDataTransferLegacySDK.ObjectImportStatistics
			If source Is Nothing Then
				Return Nothing
			End If
			
			Return New RelativityDataTransferLegacySDK.ObjectImportStatistics() With {
				.BatchSizes = source.BatchSizes,
				.RepositoryConnection = CType([Enum].Parse(GetType(RelativityDataTransferLegacySDK.RepositoryConnectionType), source.RepositoryConnection.ToString()), RelativityDataTransferLegacySDK.RepositoryConnectionType),
				.Overwrite = CType([Enum].Parse(GetType(RelativityDataTransferLegacySDK.OverwriteType), source.Overwrite.ToString()), RelativityDataTransferLegacySDK.OverwriteType),
				.OverlayIdentifierFieldArtifactID = source.OverlayIdentifierFieldArtifactID,
				.DestinationFolderArtifactID = source.DestinationFolderArtifactID,
				_
				.LoadFileName = source.LoadFileName,
				.StartLine = source.StartLine,
				.FilesCopiedToRepository = source.FilesCopiedToRepository,
				.TotalFileSize = source.TotalFileSize,
				.TotalMetadataBytes = source.TotalMetadataBytes,
				_
				.NumberOfDocumentsCreated = source.NumberOfDocumentsCreated,
				.NumberOfDocumentsUpdated = source.NumberOfDocumentsUpdated,
				.NumberOfFilesLoaded = source.NumberOfFilesLoaded,
				.NumberOfErrors = source.NumberOfErrors,
				.NumberOfWarnings = source.NumberOfWarnings,
				_
				.RunTimeInMilliseconds = source.RunTimeInMilliseconds,
				.SendNotification = source.SendNotification, 
				.OverlayBehavior = If(source.OverlayBehavior Is Nothing, Nothing, CType([Enum].Parse(GetType(RelativityDataTransferLegacySDK.OverlayBehavior), source.OverlayBehavior.ToString()), RelativityDataTransferLegacySDK.OverlayBehavior?)),
				.ArtifactTypeID = source.ArtifactTypeID,
				.Delimiter = source.Delimiter,
				_
				.Bound = source.Bound,
				.NewlineProxy = source.NewlineProxy,
				.MultiValueDelimiter = source.MultiValueDelimiter,
				.LoadFileEncodingCodePageID = source.LoadFileEncodingCodePageID,
				.ExtractedTextFileEncodingCodePageID = source.ExtractedTextFileEncodingCodePageID,
				_
				.FolderColumnName = source.FolderColumnName,
				.FileFieldColumnName = source.FileFieldColumnName,
				.ExtractedTextPointsToFile = source.ExtractedTextPointsToFile,
				.NumberOfChoicesCreated = source.NumberOfChoicesCreated,
				.NumberOfFoldersCreated = source.NumberOfFoldersCreated,
				_
				.FieldsMapped = source.FieldsMapped,
				.NestedValueDelimiter = source.NestedValueDelimiter
			}
		End Function

		Public Shared Function Map(source As kCura.EDDS.WebAPI.AuditManagerBase.ExportStatistics) As RelativityDataTransferLegacySDK.ExportStatistics
			If source Is Nothing Then
				Return Nothing
			End If
			
			Return New RelativityDataTransferLegacySDK.ExportStatistics() With {
				.Type = source.Type,
				.Fields = source.Fields,
				.DestinationFilesystemFolder = source.DestinationFilesystemFolder,
				.OverwriteFiles = source.OverwriteFiles,
				.VolumePrefix = source.VolumePrefix,
				_
				.VolumeMaxSize = source.VolumeMaxSize,
				.SubdirectoryImagePrefix = source.SubdirectoryImagePrefix,
				.SubdirectoryNativePrefix = source.SubdirectoryNativePrefix,
				.SubdirectoryTextPrefix = source.SubdirectoryTextPrefix,
				.SubdirectoryStartNumber = source.SubdirectoryStartNumber,
				_
				.SubdirectoryMaxFileCount = source.SubdirectoryMaxFileCount,
				.FilePathSettings = source.FilePathSettings,
				.Delimiter = source.Delimiter,
				.Bound = source.Bound,
				.NewlineProxy = source.NewlineProxy,
				_
				.MultiValueDelimiter = source.MultiValueDelimiter,
				.NestedValueDelimiter = source.NestedValueDelimiter,
				.TextAndNativeFilesNamedAfterFieldID = source.TextAndNativeFilesNamedAfterFieldID,
				.AppendOriginalFilenames = source.AppendOriginalFilenames,
				.ExportImages = source.ExportImages,
				_
				.ImageLoadFileFormat = CType([Enum].Parse(GetType(RelativityDataTransferLegacySDK.ImageLoadFileFormatType), source.ImageLoadFileFormat.ToString()), RelativityDataTransferLegacySDK.ImageLoadFileFormatType),
				.ImageFileType = CType([Enum].Parse(GetType(RelativityDataTransferLegacySDK.ImageFileExportType), source.ImageFileType.ToString()), RelativityDataTransferLegacySDK.ImageFileExportType),
				.ExportNativeFiles = source.ExportNativeFiles,
				.MetadataLoadFileFormat = CType([Enum].Parse(GetType(RelativityDataTransferLegacySDK.LoadFileFormat), source.MetadataLoadFileFormat.ToString()), RelativityDataTransferLegacySDK.LoadFileFormat),
				.MetadataLoadFileEncodingCodePage = source.MetadataLoadFileEncodingCodePage,
				_
				.ExportTextFieldAsFiles = source.ExportTextFieldAsFiles,
				.ExportedTextFileEncodingCodePage = source.ExportedTextFileEncodingCodePage,
				.ExportedTextFieldID = source.ExportedTextFieldID,
				.ExportMultipleChoiceFieldsAsNested = source.ExportMultipleChoiceFieldsAsNested,
				.TotalFileBytesExported = source.TotalFileBytesExported,
				_
				.TotalMetadataBytesExported = source.TotalMetadataBytesExported,
				.ErrorCount = source.ErrorCount,
				.WarningCount = source.WarningCount,
				.DocumentExportCount = source.DocumentExportCount,
				.FileExportCount = source.FileExportCount,
				_
				.ImagesToExport = CType([Enum].Parse(GetType(RelativityDataTransferLegacySDK.ImagesToExportType), source.ImagesToExport.ToString()), RelativityDataTransferLegacySDK.ImagesToExportType),
				.ProductionPrecedence = source.ProductionPrecedence,
				.DataSourceArtifactID = source.DataSourceArtifactID,
				.SourceRootFolderID = source.SourceRootFolderID,
				.RunTimeInMilliseconds = source.RunTimeInMilliseconds,
				_
				.CopyFilesFromRepository = source.CopyFilesFromRepository,
				.StartExportAtDocumentNumber = source.StartExportAtDocumentNumber,
				.VolumeStartNumber = source.VolumeStartNumber,
				.ArtifactTypeID = source.ArtifactTypeID,
				.SubdirectoryPDFPrefix = source.SubdirectoryPDFPrefix,
				_
				.ExportSearchablePDFs = source.ExportSearchablePDFs
			}
		End Function

		Public Shared Function Map(source As kCura.EDDS.WebAPI.BulkImportManagerBase.ImageLoadInfo) As RelativityDataTransferLegacySDK.ImageLoadInfo
			If source Is Nothing Then
				Return Nothing
			End If
			
			Return New RelativityDataTransferLegacySDK.ImageLoadInfo() With {
				.DisableUserSecurityCheck = source.DisableUserSecurityCheck,
				.RunID = source.RunID,
				.Overlay = CType([Enum].Parse(GetType(RelativityDataTransferLegacySDK.OverwriteType), source.Overlay.ToString()), RelativityDataTransferLegacySDK.OverwriteType),
				.Repository = source.Repository,
				.UseBulkDataImport = source.UseBulkDataImport,
				_
				.UploadFullText = source.UploadFullText,
				.BulkFileName = source.BulkFileName,
				.DataGridFileName = source.DataGridFileName,
				.KeyFieldArtifactID = source.KeyFieldArtifactID,
				.DestinationFolderArtifactID = source.DestinationFolderArtifactID,
				_
				.AuditLevel = CType([Enum].Parse(GetType(RelativityDataTransferLegacySDK.ImportAuditLevel), source.AuditLevel.ToString()), RelativityDataTransferLegacySDK.ImportAuditLevel),
				.OverlayArtifactID = source.OverlayArtifactID,
				.ExecutionSource = CType([Enum].Parse(GetType(RelativityDataTransferLegacySDK.ExecutionSource), source.ExecutionSource.ToString()), RelativityDataTransferLegacySDK.ExecutionSource),
				.Billable = source.Billable
			}
		End Function

		Public Shared Function Map(source As kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo) As RelativityDataTransferLegacySDK.NativeLoadInfo
			If source Is Nothing Then
				Return Nothing
			End If
			
			Return New RelativityDataTransferLegacySDK.NativeLoadInfo() With {
				.Range = Map(source.Range),
				.MappedFields = source.MappedFields?.Select(Function(mappedField As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo) Map(mappedField))?.ToArray(),
				.Overlay = CType([Enum].Parse(GetType(RelativityDataTransferLegacySDK.OverwriteType), source.Overlay.ToString()), RelativityDataTransferLegacySDK.OverwriteType),
				.Repository = source.Repository,
				.RunID = source.RunID,
				_
				.DataFileName = source.DataFileName,
				.UseBulkDataImport = source.UseBulkDataImport,
				.UploadFiles = source.UploadFiles,
				.CodeFileName = source.CodeFileName,
				.ObjectFileName = source.ObjectFileName,
				_
				.DataGridFileName = source.DataGridFileName, 
				.DataGridOffsetFileName = Nothing, 'No matching property in source
				.DisableUserSecurityCheck = source.DisableUserSecurityCheck,
				.OnBehalfOfUserToken = source.OnBehalfOfUserToken,
				.AuditLevel = CType([Enum].Parse(GetType(RelativityDataTransferLegacySDK.ImportAuditLevel), source.AuditLevel.ToString()), RelativityDataTransferLegacySDK.ImportAuditLevel),
				_
				.BulkLoadFileFieldDelimiter = source.BulkLoadFileFieldDelimiter,
				.OverlayArtifactID = source.OverlayArtifactID,
				.OverlayBehavior = CType([Enum].Parse(GetType(RelativityDataTransferLegacySDK.OverlayBehavior), source.OverlayBehavior.ToString()), RelativityDataTransferLegacySDK.OverlayBehavior),
				.LinkDataGridRecords = source.LinkDataGridRecords,
				.LoadImportedFullTextFromServer = source.LoadImportedFullTextFromServer,
				_
				.KeyFieldArtifactID = source.KeyFieldArtifactID,
				.RootFolderID = source.RootFolderID,
				.MoveDocumentsInAppendOverlayMode = source.MoveDocumentsInAppendOverlayMode,
				.ExecutionSource = CType([Enum].Parse(GetType(RelativityDataTransferLegacySDK.ExecutionSource), source.ExecutionSource.ToString()), RelativityDataTransferLegacySDK.ExecutionSource),
				.Billable = source.Billable
			}
		End Function

		Public Shared Function Map(source As kCura.EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo) As RelativityDataTransferLegacySDK.ObjectLoadInfo
			If source Is Nothing Then
				Return Nothing
			End If
			
			Return New RelativityDataTransferLegacySDK.ObjectLoadInfo() With {
				.Range = Map(source.Range),
				.MappedFields = source.MappedFields?.Select(Function(mappedField As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo) Map(mappedField))?.ToArray(),
				.Overlay = CType([Enum].Parse(GetType(RelativityDataTransferLegacySDK.OverwriteType), source.Overlay.ToString()), RelativityDataTransferLegacySDK.OverwriteType),
				.Repository = source.Repository,
				.RunID = source.RunID,
				_
				.DataFileName = source.DataFileName,
				.UseBulkDataImport = source.UseBulkDataImport,
				.UploadFiles = source.UploadFiles,
				.CodeFileName = source.CodeFileName,
				.ObjectFileName = source.ObjectFileName,
				_
				.DataGridFileName = source.DataGridFileName,
				.DataGridOffsetFileName = Nothing, 'No matching property in source
				.DisableUserSecurityCheck = source.DisableUserSecurityCheck,
				.OnBehalfOfUserToken = source.OnBehalfOfUserToken,
				.AuditLevel = CType([Enum].Parse(GetType(RelativityDataTransferLegacySDK.ImportAuditLevel), source.AuditLevel.ToString()), RelativityDataTransferLegacySDK.ImportAuditLevel),
				_
				.BulkLoadFileFieldDelimiter = source.BulkLoadFileFieldDelimiter,
				.OverlayArtifactID = source.OverlayArtifactID,
				.OverlayBehavior = CType([Enum].Parse(GetType(RelativityDataTransferLegacySDK.OverlayBehavior), source.OverlayBehavior.ToString()), RelativityDataTransferLegacySDK.OverlayBehavior),
				.LinkDataGridRecords = source.LinkDataGridRecords,
				.LoadImportedFullTextFromServer = source.LoadImportedFullTextFromServer,
				_
				.KeyFieldArtifactID = source.KeyFieldArtifactID,
				.RootFolderID = source.RootFolderID,
				.MoveDocumentsInAppendOverlayMode = source.MoveDocumentsInAppendOverlayMode,
				.ExecutionSource = CType([Enum].Parse(GetType(RelativityDataTransferLegacySDK.ExecutionSource), source.ExecutionSource.ToString()), RelativityDataTransferLegacySDK.ExecutionSource),
				.Billable = source.Billable,
				_
				.ArtifactTypeID = source.ArtifactTypeID
			}
		End Function

		Public Shared Function Map(source As kCura.EDDS.WebAPI.BulkImportManagerBase.LoadRange) As RelativityDataTransferLegacySDK.LoadRange
			If source Is Nothing Then
				Return Nothing
			End If
			
			Return New RelativityDataTransferLegacySDK.LoadRange() With {
				.StartIndex = source.StartIndex,
				.Count = source.Count
			}
		End Function

		Public Shared Function Map(source As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo) As RelativityDataTransferLegacySDK.FieldInfo
			If source Is Nothing Then
				Return Nothing
			End If
			
			Return New RelativityDataTransferLegacySDK.FieldInfo() With {
				.ArtifactID = source.ArtifactID,
				.Category = CType([Enum].Parse(GetType(RelativityDataTransferLegacySDK.FieldCategory), source.Category.ToString()), RelativityDataTransferLegacySDK.FieldCategory),
				.Type = CType([Enum].Parse(GetType(RelativityDataTransferLegacySDK.FieldType), source.Type.ToString()), RelativityDataTransferLegacySDK.FieldType),
				.DisplayName = source.DisplayName,
				.TextLength = source.TextLength,
				_
				.CodeTypeID = source.CodeTypeID,
				.EnableDataGrid = source.EnableDataGrid,
				.FormatString = source.FormatString,
				.IsUnicodeEnabled = source.IsUnicodeEnabled,
				.ImportBehavior = If(source.ImportBehavior Is Nothing, Nothing, CType([Enum].Parse(GetType(RelativityDataTransferLegacySDK.ImportBehaviorChoice), source.ImportBehavior.ToString()), RelativityDataTransferLegacySDK.ImportBehaviorChoice?))
			}

		End Function

		Public Shared Function Map(source As kCura.EDDS.WebAPI.CodeManagerBase.Code) As RelativityDataTransferLegacySDK.Code
			If source Is Nothing Then
				Return Nothing
			End If
			
			Return New RelativityDataTransferLegacySDK.Code() With {
				.ArtifactID = source.ArtifactID,
				.ArtifactTypeID = source.ArtifactTypeID,
				.ParentArtifactID = source.ParentArtifactID,
				.ContainerID = source.ContainerID,
				.AccessControlListID = source.AccessControlListID,
				_
				.AccessControlListIsInherited = source.AccessControlListIsInherited,
				.Keywords = source.Keywords,
				.Notes = source.Notes,
				.TextIdentifier = source.TextIdentifier,
				.LastModifiedOn = source.LastModifiedOn,
				_
				.LastModifiedBy = source.LastModifiedBy,
				.CreatedBy = source.CreatedBy,
				.CreatedOn = source.CreatedOn,
				.DeleteFlag = source.DeleteFlag,
				.Guids = source.Guids?.ToList(),
				_
				.CodeType = source.CodeType,
				.Name = source.Name,
				.Order = source.Order,
				.IsActive = source.IsActive,
				.UpdateInSearchEngine = source.UpdateInSearchEngine,
				_
				.OIHiliteStyleID = source.OIHiliteStyleID,
				.KeyboardShortcut = Map(source.KeyboardShortcut),
				.RelativityApplications = source.RelativityApplications
			}
		End Function

		Public Shared Function Map(source As kCura.EDDS.WebAPI.CodeManagerBase.KeyboardShortcut) As RelativityDataTransferLegacySDK.KeyboardShortcut
			If source Is Nothing Then
				Return Nothing
			End If
			
			Return New RelativityDataTransferLegacySDK.KeyboardShortcut() With {
				.Id = source.Id,
				.Shift = source.Shift,
				.Ctrl = source.Ctrl,
				.Alt = source.Alt,
				.Key = source.Key
			}
		End Function

		Public Shared Function Map(source As RelativityDataTransferLegacySDK.MassImportResults) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			If source Is Nothing Then
				Return Nothing
			End If
			
			Return New kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults() With {
				.FilesProcessed = source.FilesProcessed,
				.ArtifactsCreated = source.ArtifactsCreated,
				.ArtifactsUpdated = source.ArtifactsUpdated,
				.RunID = source.RunID,
				.ExceptionDetail = Map(source.ExceptionDetail)
			}
		End Function

		Public Shared Function Map(source As RelativityDataTransferLegacySDK.SoapExceptionDetail) As kCura.EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail
			If source Is Nothing Then
				Return Nothing
			End If
			
			Return New kCura.EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail() With {
				.ExceptionType = source.ExceptionType,
				.ExceptionMessage = source.ExceptionMessage,
				.ExceptionTrace = source.ExceptionTrace,
				.ExceptionFullText = source.ExceptionFullText,
				.Details = source.Details?.ToArray()
			}
		End Function

		Public Shared Function Map(source As RelativityDataTransferLegacySDK.ErrorFileKey) As RelativityDataExchange.ErrorFileKey
			If source Is Nothing Then
				Return Nothing
			End If
			
			Return New RelativityDataExchange.ErrorFileKey() With {
				.OpticonKey = source.OpticonKey,
				.LogKey = source.LogKey
			}
		End Function

		Public Shared Function Map(source As RelativityDataTransferLegacySDK.CaseInfo) As RelativityDataExchange.CaseInfo
			If source Is Nothing Then
				Return Nothing
			End If
			
			Return New RelativityDataExchange.CaseInfo() With {
				.ArtifactID = source.ArtifactID,
				.Name = source.Name,
				.MatterArtifactID = source.MatterArtifactID,
				.StatusCodeArtifactID = source.StatusCodeArtifactID,
				.EnableDataGrid = source.EnableDataGrid,
				_
				.RootFolderID = source.RootFolderID,
				.RootArtifactID = source.RootArtifactID,
				.DownloadHandlerURL = source.DownloadHandlerURL,
				.AsImportAllowed = source.AsImportAllowed,
				.ExportAllowed = source.ExportAllowed,
				_
				.DocumentPath = source.DocumentPath
			}
		End Function

		Public Shared Function Map(source As RelativityDataTransferLegacySDK.ChoiceInfo) As RelativityDataExchange.ChoiceInfo
			If source Is Nothing Then
				Return Nothing
			End If
			
			Return New RelativityDataExchange.ChoiceInfo() With {
				.Order = source.Order,
				.CodeTypeID = source.CodeTypeID,
				.Name = source.Name,
				.ArtifactID = source.ArtifactID,
				.ParentArtifactID = source.ParentArtifactID
			}
		End Function

		Public Shared Function MapToCodeManagerBaseKeyboardShortcut(source As RelativityDataTransferLegacySDK.KeyboardShortcut) As kCura.EDDS.WebAPI.CodeManagerBase.KeyboardShortcut
			If source Is Nothing Then
				Return Nothing
			End If
			
			Return New kCura.EDDS.WebAPI.CodeManagerBase.KeyboardShortcut() With {
				.Id = source.Id,
				.Shift = source.Shift,
				.Ctrl = source.Ctrl,
				.Alt = source.Alt,
				.Key = source.Key
			}

		End Function

		Public Shared Function Map(source As RelativityDataTransferLegacySDK.InitializationResults) As kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults
			If source Is Nothing Then
				Return Nothing
			End If
			
			Return New kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults() With {
				.RunId = source.RunId,
				.RowCount = source.RowCount,
				.ColumnNames = source.ColumnNames
			}
		End Function

		Public Shared Function MapToFieldManagerBaseField(source As RelativityDataTransferLegacySDK.Field) As kCura.EDDS.WebAPI.FieldManagerBase.Field
			If source Is Nothing Then
				Return Nothing
			End If
			
			Return New kCura.EDDS.WebAPI.FieldManagerBase.Field() With {
				.ArtifactID = source.ArtifactID,
				.ArtifactTypeID = source.ArtifactTypeID,
				.ParentArtifactID = source.ParentArtifactID,
				.ContainerID = source.ContainerID,
				.AccessControlListID = source.AccessControlListID,
				_
				.AccessControlListIsInherited = source.AccessControlListIsInherited,
				.Keywords = source.Keywords,
				.Notes = source.Notes,
				.TextIdentifier = source.TextIdentifier,
				.LastModifiedOn = source.LastModifiedOn,
				_
				.LastModifiedBy = source.LastModifiedBy,
				.CreatedBy = source.CreatedBy,
				.CreatedOn = source.CreatedOn,
				.DeleteFlag = source.DeleteFlag,
				.Guids = source.Guids?.ToArray(),
				_
				.FieldArtifactTypeID = source.FieldArtifactTypeID,
				.DisplayName = source.DisplayName,
				.FieldTypeID = source.FieldTypeID,
				.FieldType = CType([Enum].Parse(GetType(kCura.EDDS.WebAPI.FieldManagerBase.FieldType), source.FieldType.ToString()), kCura.EDDS.WebAPI.FieldManagerBase.FieldType),
				.FieldCategoryID = source.FieldCategoryID,
				_
				.FieldCategory = CType([Enum].Parse(GetType(kCura.EDDS.WebAPI.FieldManagerBase.FieldCategory), source.FieldCategory.ToString()), kCura.EDDS.WebAPI.FieldManagerBase.FieldCategory),
				.ArtifactViewFieldID = source.ArtifactViewFieldID,
				.CodeTypeID = source.CodeTypeID,
				.MaxLength = source.MaxLength,
				.IsRequired = source.IsRequired,
				_
				.IsRemovable = source.IsRemovable,
				.IsEditable = source.IsEditable, 'IsUpdatable - no matching property in result
				.IsVisible = source.IsVisible,
				.IsArtifactBaseField = source.IsArtifactBaseField,
				_
				.Value = source.Value,
				.TableName = source.TableName,
				.ColumnName = source.ColumnName,
				.IsReadOnlyInLayout = source.IsReadOnlyInLayout,
				.FilterType = source.FilterType,
				_
				.FieldDisplayTypeID = source.FieldDisplayTypeID,
				.Rows = source.Rows,
				.IsLinked = source.IsLinked,
				.FormatString = source.FormatString,
				.RepeatColumn = source.RepeatColumn,
				_
				.AssociativeArtifactTypeID = source.AssociativeArtifactTypeID,
				.IsAvailableToAssociativeObjects = source.IsAvailableToAssociativeObjects,
				.IsAvailableInChoiceTree = source.IsAvailableInChoiceTree,
				.IsGroupByEnabled = source.IsGroupByEnabled,
				.IsIndexEnabled = source.IsIndexEnabled,
				_
				.DisplayValueTrue = source.DisplayValueTrue,
				.DisplayValueFalse = source.DisplayValueFalse,
				.Width = source.Width,
				.Wrapping = source.Wrapping,
				.LinkLayoutArtifactID = source.LinkLayoutArtifactID,
				_
				.NameValue = source.NameValue,
				.LinkType = source.LinkType,
				.UseUnicodeEncoding = source.UseUnicodeEncoding,
				.AllowHtml = source.AllowHtml,
				.IsSortable = source.IsSortable,
				_
				.FriendlyName = source.FriendlyName,
				.ImportBehavior = If(source.ImportBehavior Is Nothing, Nothing, CType([Enum].Parse(GetType(kCura.EDDS.WebAPI.FieldManagerBase.ImportBehaviorChoice), source.ImportBehavior.ToString()), kCura.EDDS.WebAPI.FieldManagerBase.ImportBehaviorChoice?)),
				.EnableDataGrid = source.EnableDataGrid,
				.OverlayBehavior = source.OverlayBehavior,
				.RelationalIndexViewArtifactID = source.RelationalIndexViewArtifactID,
				_
				.ObjectsFieldArgs = MapToFieldManagerBaseObjectsFieldParameters(source.ObjectsFieldArgs),
				.AllowGroupBy = source.AllowGroupBy,
				.AllowPivot = source.AllowPivot,
				.PopupPickerView = source.PopupPickerView,
				.FieldTreeView = source.FieldTreeView,
				_
				.KeyboardShortcut = MapToFieldManagerBaseKeyboardShortcut(source.KeyboardShortcut),
				.AvailableInViewer = source.AvailableInViewer,
				.RelativityApplications = source.RelativityApplications,
				.RelationalPane = MapToFieldManagerBaseRelationalFieldPane(source.RelationalPane),
				.AutoAddChoices = source.AutoAddChoices 'IsReflected, IsSystemField, IsSystemOrRelationalField, IsRelationalField - no matching properties in result
			}
		End Function

		Public Shared Function MapToFieldManagerBaseKeyboardShortcut(source As RelativityDataTransferLegacySDK.KeyboardShortcut) As kCura.EDDS.WebAPI.FieldManagerBase.KeyboardShortcut
			If source Is Nothing Then
				Return Nothing
			End If
			
			Return New kCura.EDDS.WebAPI.FieldManagerBase.KeyboardShortcut() With {
				.Id = source.Id,
				.Shift = source.Shift,
				.Ctrl = source.Ctrl,
				.Alt = source.Alt,
				.Key = source.Key
			}
		End Function

		Public Shared Function MapToFieldManagerBaseRelationalFieldPane(source As RelativityDataTransferLegacySDK.RelationalFieldPane) As kCura.EDDS.WebAPI.FieldManagerBase.RelationalFieldPane
			If source Is Nothing Then
				Return Nothing
			End If
			
			Return New kCura.EDDS.WebAPI.FieldManagerBase.RelationalFieldPane() With {
				.PaneOrder = source.PaneOrder,
				.IconFilename = source.IconFilename,
				.ColumnName = source.ColumnName,
				.FieldArtifactID = source.FieldArtifactID,
				.RelationalViewArtifactID = source.RelationalViewArtifactID,
				_
				.HeaderText = source.HeaderText,
				.IconFileData = source.IconFileData,
				.PaneID = source.PaneID
			}
		End Function

		Public Shared Function MapToFieldManagerBaseObjectsFieldParameters(source As RelativityDataTransferLegacySDK.ObjectsFieldParameters) As kCura.EDDS.WebAPI.FieldManagerBase.ObjectsFieldParameters
			If source Is Nothing Then
				Return Nothing
			End If
			
			Return New kCura.EDDS.WebAPI.FieldManagerBase.ObjectsFieldParameters() With {
				.SiblingFieldName = source.SiblingFieldName,
				.FieldSchemaColumnName = source.FieldSchemaColumnName,
				.SiblingFieldSchemaColumnName = source.SiblingFieldSchemaColumnName,
				.RelationalTableSchemaName = source.RelationalTableSchemaName,
				.CreateForeignKeys = source.CreateForeignKeys
			}
		End Function

		Public Shared Function MapToDocumentManagerBaseKeyboardShortcut(source As RelativityDataTransferLegacySDK.KeyboardShortcut) As kCura.EDDS.WebAPI.DocumentManagerBase.KeyboardShortcut
			If source Is Nothing Then
				Return Nothing
			End If
			
			Return New kCura.EDDS.WebAPI.DocumentManagerBase.KeyboardShortcut() With {
				.Id = source.Id,
				.Shift = source.Shift,
				.Ctrl = source.Ctrl,
				.Alt = source.Alt,
				.Key = source.Key
			}
		End Function

		Public Shared  Function MapToDocumentManagerBaseRelationalFieldPane(source As RelativityDataTransferLegacySDK.RelationalFieldPane) As kCura.EDDS.WebAPI.DocumentManagerBase.RelationalFieldPane
			If source Is Nothing Then
				Return Nothing
			End If
			
			Return New kCura.EDDS.WebAPI.DocumentManagerBase.RelationalFieldPane() With {
				.PaneOrder = source.PaneOrder,
				.IconFilename = source.IconFilename,
				.ColumnName = source.ColumnName,
				.FieldArtifactID = source.FieldArtifactID,
				.RelationalViewArtifactID = source.RelationalViewArtifactID,
				_
				.HeaderText = source.HeaderText,
				.IconFileData = source.IconFileData,
				.PaneID = source.PaneID
			}
		End Function

		Public Shared Function MapToDocumentManagerBaseObjectsFieldParameters(source As RelativityDataTransferLegacySDK.ObjectsFieldParameters) As kCura.EDDS.WebAPI.DocumentManagerBase.ObjectsFieldParameters
			If source Is Nothing Then
				Return Nothing
			End If
			
			Return New kCura.EDDS.WebAPI.DocumentManagerBase.ObjectsFieldParameters() With {
				.ManageFieldExposureForSingleObjectField = source.ManageFieldExposureForSingleObjectField,
				.SiblingFieldName = source.SiblingFieldName,
				.FieldSchemaColumnName = source.FieldSchemaColumnName,
				.SiblingFieldSchemaColumnName = source.SiblingFieldSchemaColumnName, 'SiblingFieldGuids - no matching property in result
				_
				.RelationalTableSchemaName = source.RelationalTableSchemaName,
				.CreateForeignKeys = source.CreateForeignKeys,
				.ManageFieldExposure = source.ManageFieldExposure
			}
		End Function

		Public Shared Function Map(source As RelativityDataTransferLegacySDK.Folder) As kCura.EDDS.WebAPI.FolderManagerBase.Folder
			If source Is Nothing Then
				Return Nothing
			End If
			
			Return New kCura.EDDS.WebAPI.FolderManagerBase.Folder() With {
				.ArtifactID = source.ArtifactID,
				.ArtifactTypeID = source.ArtifactTypeID,
				.ParentArtifactID = source.ParentArtifactID,
				.ContainerID = source.ContainerID,
				.AccessControlListID = source.AccessControlListID,
				_
				.AccessControlListIsInherited = source.AccessControlListIsInherited,
				.Keywords = source.Keywords,
				.Notes = source.Notes,
				.TextIdentifier = source.TextIdentifier,
				.LastModifiedOn = source.LastModifiedOn,
				_
				.LastModifiedBy = source.LastModifiedBy,
				.CreatedBy = source.CreatedBy,
				.CreatedOn = source.CreatedOn,
				.DeleteFlag = source.DeleteFlag,
				.Guids = source.Guids?.ToArray(),
				_
				.Name = source.Name
			}
		End Function

		Public Shared Function Map(source As RelativityDataTransferLegacySDK.ProductionInfo) As kCura.EDDS.WebAPI.ProductionManagerBase.ProductionInfo
			If source Is Nothing Then
				Return Nothing
			End If
			
			Return New kCura.EDDS.WebAPI.ProductionManagerBase.ProductionInfo() With {
				.BatesNumbering = source.BatesNumbering,
				.BeginBatesReflectedFieldId = source.BeginBatesReflectedFieldId,
				.DocumentsHaveRedactions = source.DocumentsHaveRedactions,
				.IncludeImageLevelNumberingForDocumentLevelNumbering = source.IncludeImageLevelNumberingForDocumentLevelNumbering,
				.Name = source.Name,
				_
				.UseDocumentLevelNumbering = source.UseDocumentLevelNumbering
			}
		End Function
	End Class
End Namespace