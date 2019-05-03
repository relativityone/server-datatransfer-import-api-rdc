Imports Relativity.DataTransfer.MessageService
Imports Relativity.Import.Export
Imports Relativity.Import.Export.Io
Imports Relativity.Import.Export.Process
Imports Relativity.Import.Export.Transfer
Imports Relativity.Import.Export.Service

Namespace kCura.WinEDDS

	Public Class ImportLoadFileProcess
		Inherits MonitoredProcessBase

		Public LoadFile As LoadFile
		Protected WithEvents _loadFileImporter As BulkLoadFileImporter
		Protected WithEvents _IoReporterContext As IoReporterContext
		Private _errorCount As Int32
		Private _warningCount As Int32
		Private _timeZoneOffset As Int32

		Private _uploadModeText As String = Nothing

		Private _disableUserSecutityCheck As Boolean
		Private _disableNativeValidation As Boolean?
		Private _disableNativeLocationValidation As Boolean?
		Private _auditLevel As kCura.EDDS.WebAPI.BulkImportManagerBase.ImportAuditLevel = Config.AuditLevel

		Public Sub New()
			MyBase.New(New MessageService())
		End Sub

		Public Sub New(messageService As IMessageService)
			MyBase.New(messageService)
		End Sub

		Public WriteOnly Property DisableNativeValidation As Boolean
			Set(ByVal value As Boolean)
				_disableNativeValidation = value
			End Set
		End Property

		Public WriteOnly Property DisableNativeLocationValidation As Boolean
			Set(ByVal value As Boolean)
				_disableNativeLocationValidation = value
			End Set
		End Property

		Protected Overrides ReadOnly Property JobType As String = "Import"

		Protected Overrides ReadOnly Property TapiClientName As String
			Get
				If _loadFileImporter Is Nothing Then
					Return _tapiClientName
				Else
					Return _loadFileImporter.TapiClientName
				End If
			End Get
		End Property

		Public Property OIFileIdMapped As Boolean
		Public Property OIFileIdColumnName As String
		Public Property OIFileTypeColumnName As String
		Public Property FileSizeMapped As Boolean
		Public Property FileSizeColumn As String
		Public Property FileNameColumn As String
		Public Property SupportedByViewerColumn As String

		Public WriteOnly Property DisableUserSecurityCheck As Boolean
			Set(ByVal value As Boolean)
				_disableUserSecutityCheck = value
			End Set
		End Property

		Public WriteOnly Property AuditLevel As kCura.EDDS.WebAPI.BulkImportManagerBase.ImportAuditLevel
			Set(ByVal value As kCura.EDDS.WebAPI.BulkImportManagerBase.ImportAuditLevel)
				_auditLevel = value
			End Set
		End Property

		Public Property DisableExtractedTextFileLocationValidation As Boolean

		Public Property MaximumErrorCount As Int32?

		Public Property SkipExtractedTextEncodingCheck As Boolean?

		Public Property LoadImportedFullTextFromServer As Boolean

		''' <summary>
		''' Gets or sets the delimiter to use to separate fields in the bulk
		''' file created in this process. Line delimiters will be this value plus a line feed.
		''' </summary>
		Public Property BulkLoadFileFieldDelimiter As String

		Public Property CloudInstance() As Boolean

		Public Property EnforceDocumentLimit() As Boolean

		Public Property ExecutionSource() As ExecutionSource

		Public Property TimeZoneOffset() As Int32
			Get
				Return _timeZoneOffset
			End Get
			Set(ByVal value As Int32)
				_timeZoneOffset = value
			End Set
		End Property

		Public Overridable Function GetImporter() As kCura.WinEDDS.BulkLoadFileImporter
			_IoReporterContext = New IoReporterContext(Me.FileSystem, Me.AppSettings, New WaitAndRetryPolicy(Me.AppSettings))
			Dim reporter As IIoReporter = Me.CreateIoReporter(_IoReporterContext)
			Dim returnImporter As BulkLoadFileImporter = New kCura.WinEDDS.BulkLoadFileImporter(
				LoadFile, _
				Me.Context, _
				reporter, _
				logger, _
				_timeZoneOffset, _
				True, _
				Me.ProcessID, _
				True, _
				BulkLoadFileFieldDelimiter, _
				EnforceDocumentLimit, _
				Me.CancellationTokenSource, _
				ExecutionSource)
			Return returnImporter
		End Function

		Protected Overrides Sub OnFatalError()
			MyBase.OnFatalError()
			SendJobStatistics(_loadFileImporter.Statistics)
			SendTransferJobFailedMessage()
		End Sub

		Protected Overrides Sub OnSuccess()
			MyBase.OnSuccess()
			SendJobStatistics(_loadFileImporter.Statistics)
			SendTransferJobCompletedMessage()
			Me.Context.PublishProcessCompleted(False, "", True)
		End Sub

		Protected Overrides Sub OnHasErrors()
			MyBase.OnHasErrors()
			SendJobStatistics(_loadFileImporter.Statistics)
			SendTransferJobCompletedMessage()
			Me.Context.PublishProcessCompleted(False, System.Guid.NewGuid.ToString, True)
		End Sub

		Protected Overrides Function HasErrors() As Boolean

			Return _loadFileImporter.HasErrors
		End Function

		Protected Overrides Function Run() As Boolean
			Return (CType(_loadFileImporter.ReadFile(LoadFile.FilePath), Boolean)) AndAlso Not _hasFatalErrorOccured
		End Function

		Protected Overrides Sub Initialize()

			MyBase.Initialize()
			_warningCount = 0
			_errorCount = 0
			_loadFileImporter = Me.GetImporter()

			If _disableNativeValidation.HasValue Then _loadFileImporter.DisableNativeValidation = _disableNativeValidation.Value
			If _disableNativeLocationValidation.HasValue Then _loadFileImporter.DisableNativeLocationValidation = _disableNativeLocationValidation.Value
			If MaximumErrorCount.HasValue AndAlso MaximumErrorCount.Value > 0 AndAlso MaximumErrorCount.Value < Int32.MaxValue Then
				'The '+1' is because the 'MaxNumberOfErrorsInGrid' is actually 1 more (because the
				' final error is simply 'Maximum # of errors' error) than the *actual* maximum, but
				' we don't want to change BulkImageFileImporter's behavior.
				' -Phil S. 07/10/2012
				_loadFileImporter.MaxNumberOfErrorsInGrid = MaximumErrorCount.Value + 1
			End If

			If SkipExtractedTextEncodingCheck.HasValue AndAlso SkipExtractedTextEncodingCheck Then
				_loadFileImporter.SkipExtractedTextEncodingCheck = True
			End If
			_loadFileImporter.SkipExtractedTextEncodingCheck = (_loadFileImporter.SkipExtractedTextEncodingCheck OrElse Me.AppSettings.DisableTextFileEncodingCheck)

			_loadFileImporter.DisableExtractedTextFileLocationValidation = DisableExtractedTextFileLocationValidation
			_loadFileImporter.AuditLevel = _auditLevel
			_loadFileImporter.DisableUserSecurityCheck = _disableUserSecutityCheck
			_loadFileImporter.OIFileIdColumnName = OIFileIdColumnName
			_loadFileImporter.OIFileIdMapped = OIFileIdMapped
			_loadFileImporter.OIFileTypeColumnName = OIFileTypeColumnName
			_loadFileImporter.FileSizeColumn = FileSizeColumn
			_loadFileImporter.FileSizeMapped = FileSizeMapped
			_loadFileImporter.FileNameColumn = FileNameColumn
			_loadFileImporter.SupportedByViewerColumn = SupportedByViewerColumn
			_loadFileImporter.LoadImportedFullTextFromServer = (Me.LoadImportedFullTextFromServer OrElse Me.AppSettings.LoadImportedFullTextFromServer)
			Me.Context.InputArgs = LoadFile.FilePath
		End Sub

		Private Sub AuditRun(ByVal success As Boolean, ByVal runID As String)
			Try
				Dim retval As New kCura.EDDS.WebAPI.AuditManagerBase.ObjectImportStatistics
				retval.ArtifactTypeID = LoadFile.ArtifactTypeID
				retval.BatchSizes = _loadFileImporter.BatchSizeHistoryList.ToArray
				retval.Bound = LoadFile.QuoteDelimiter
				retval.Delimiter = LoadFile.RecordDelimiter
				retval.NestedValueDelimiter = LoadFile.HierarchicalValueDelimiter
				retval.DestinationFolderArtifactID = LoadFile.DestinationFolderID
				If LoadFile.ArtifactTypeID <> ArtifactType.Document Then retval.DestinationFolderArtifactID = -1
				Dim fieldMap As New System.Collections.ArrayList
				Dim mappedExtractedText As Boolean = False
				For Each item As WinEDDS.LoadFileFieldMap.LoadFileFieldMapItem In LoadFile.FieldMap
					If Not item.DocumentField Is Nothing AndAlso item.NativeFileColumnIndex > -1 Then
						fieldMap.Add(New Int32() {item.NativeFileColumnIndex, item.DocumentField.FieldID})
						If item.DocumentField.FieldCategory = FieldCategory.FullText Then mappedExtractedText = True
					End If
				Next
				retval.ExtractedTextPointsToFile = LoadFile.FullTextColumnContainsFileLocation AndAlso mappedExtractedText
				If LoadFile.CopyFilesToDocumentRepository Then
					retval.FilesCopiedToRepository = LoadFile.SelectedCasePath
				Else
					retval.FilesCopiedToRepository = String.Empty
				End If
				retval.FieldsMapped = DirectCast(fieldMap.ToArray(GetType(Int32())), Int32()())
				If LoadFile.LoadNativeFiles Then
					retval.FileFieldColumnName = LoadFile.NativeFilePathColumn
				Else
					retval.FileFieldColumnName = String.Empty
				End If
				If LoadFile.CreateFolderStructure Then
					retval.FolderColumnName = LoadFile.FolderStructureContainedInColumn
				Else
					retval.FolderColumnName = ""
				End If
				If retval.FolderColumnName Is Nothing Then retval.FolderColumnName = String.Empty
				retval.LoadFileEncodingCodePageID = LoadFile.SourceFileEncoding.CodePage
				retval.LoadFileName = System.IO.Path.GetFileName(LoadFile.FilePath)
				retval.MultiValueDelimiter = LoadFile.MultiRecordDelimiter
				retval.NewlineProxy = LoadFile.NewlineDelimiter
				retval.NumberOfChoicesCreated = _loadFileImporter.CodesCreated + _loadFileImporter.SingleCodesCreated
				retval.NumberOfDocumentsCreated = _loadFileImporter.Statistics.DocumentsCreated
				retval.NumberOfDocumentsUpdated = _loadFileImporter.Statistics.DocumentsUpdated
				retval.NumberOfErrors = _errorCount
				retval.NumberOfFilesLoaded = _loadFileImporter.Statistics.FilesProcessed
				retval.NumberOfFoldersCreated = _loadFileImporter.FoldersCreated
				retval.NumberOfWarnings = _warningCount
				retval.OverlayIdentifierFieldArtifactID = LoadFile.IdentityFieldId
				If Not LoadFile.ExtractedTextFileEncoding Is Nothing Then
					retval.ExtractedTextFileEncodingCodePageID = LoadFile.ExtractedTextFileEncoding.CodePage
				End If
				Select Case CType([Enum].Parse(GetType(ImportOverwriteType), LoadFile.OverwriteDestination, True), ImportOverwriteType)
					Case ImportOverwriteType.Overlay
						retval.Overwrite = EDDS.WebAPI.AuditManagerBase.OverwriteType.Overlay
					Case ImportOverwriteType.AppendOverlay
						retval.Overwrite = EDDS.WebAPI.AuditManagerBase.OverwriteType.Both
					Case Else
						retval.Overwrite = EDDS.WebAPI.AuditManagerBase.OverwriteType.Append
				End Select

				If LoadFile.OverlayBehavior.HasValue Then
					Select Case LoadFile.OverlayBehavior.Value
						Case WinEDDS.LoadFile.FieldOverlayBehavior.MergeAll
							retval.OverlayBehavior = EDDS.WebAPI.AuditManagerBase.OverlayBehavior.MergeAll
						Case WinEDDS.LoadFile.FieldOverlayBehavior.ReplaceAll
							retval.OverlayBehavior = EDDS.WebAPI.AuditManagerBase.OverlayBehavior.ReplaceAll
						Case Else
							retval.OverlayBehavior = EDDS.WebAPI.AuditManagerBase.OverlayBehavior.UseRelativityDefaults
					End Select
				End If


				If LoadFile.CopyFilesToDocumentRepository Then
					Select Case _loadFileImporter.UploadConnection
						Case TapiClient.Direct
							retval.RepositoryConnection = EDDS.WebAPI.AuditManagerBase.RepositoryConnectionType.Direct
						Case TapiClient.Aspera
						Case TapiClient.Web
							retval.RepositoryConnection = EDDS.WebAPI.AuditManagerBase.RepositoryConnectionType.Web
					End Select
					retval.TotalFileSize = _loadFileImporter.Statistics.FileBytes
				End If
				retval.RunTimeInMilliseconds = CType(System.DateTime.Now.Subtract(StartTime).TotalMilliseconds, Int32)
				retval.StartLine = CType(System.Math.Min(LoadFile.StartLineNumber, Int32.MaxValue), Int32)
				retval.TotalMetadataBytes = _loadFileImporter.Statistics.MetadataBytes
				retval.SendNotification = LoadFile.SendEmailOnLoadCompletion
				Dim auditManager As New kCura.WinEDDS.Service.AuditManager(LoadFile.Credentials, LoadFile.CookieContainer)

				auditManager.AuditObjectImport(LoadFile.CaseInfo.ArtifactID, runID, Not success, retval)
			Catch
			End Try
		End Sub

		Private Sub _loadFileImporter_FieldMapped(ByVal sourceField As String, ByVal workspaceField As String) Handles _loadFileImporter.FieldMapped
			Me.Context.PublishFieldMapped(sourceField, workspaceField)
		End Sub

		Private Sub _loadFileImporter_StatusMessage(ByVal e As StatusEventArgs) Handles _loadFileImporter.StatusMessage
			SyncLock Me.Context
				Dim statisticsDictionary As IDictionary = Nothing
				If Not e.AdditionalInfo Is Nothing Then statisticsDictionary = DirectCast(e.AdditionalInfo, IDictionary)
				Select Case e.EventType
					Case EventType.End
						Me.Context.PublishProgress(e.TotalRecords, e.CurrentRecordIndex, _warningCount, _errorCount, StartTime, System.DateTime.Now, e.Statistics.MetadataThroughput, e.Statistics.FileThroughput, Me.ProcessID, Nothing, Nothing, statisticsDictionary)
						Me.Context.PublishStatusEvent(e.CurrentRecordIndex.ToString, e.Message)
						Me.Context.PublishProcessEnded(e.Statistics.FileBytes, e.Statistics.MetadataBytes)
					Case EventType.Error
						_errorCount += 1
						Me.Context.PublishProgress(e.TotalRecords, e.CurrentRecordIndex, _warningCount, _errorCount, StartTime, New DateTime, e.Statistics.MetadataThroughput, e.Statistics.FileThroughput, Me.ProcessID, Nothing, Nothing, statisticsDictionary)
						Me.Context.PublishErrorEvent(e.CurrentRecordIndex.ToString, e.Message)
					Case EventType.Progress
						TotalRecords = e.TotalRecords
						CompletedRecordsCount = e.CurrentRecordIndex
						Me.Context.PublishRecordProcessed(e.CurrentRecordIndex)
						Me.Context.PublishProgress(e.TotalRecords, e.CurrentRecordIndex, _warningCount, _errorCount, StartTime, New DateTime, e.Statistics.MetadataThroughput, e.Statistics.FileThroughput, Me.ProcessID, Nothing, Nothing, statisticsDictionary)
						Me.Context.PublishStatusEvent(e.CurrentRecordIndex.ToString, e.Message)
					Case EventType.Statistics
						SendThroughputStatistics(e.Statistics.MetadataThroughput, e.Statistics.FileThroughput)
					Case EventType.ResetProgress
						' Do NOT raise RaiseRecordProcessed for this event. 
						CompletedRecordsCount = 0
						Me.Context.PublishProgress(e.TotalRecords, e.CurrentRecordIndex, _warningCount, _errorCount, StartTime, New DateTime, e.Statistics.MetadataThroughput, e.Statistics.FileThroughput, Me.ProcessID, Nothing, Nothing, statisticsDictionary)
						Me.Context.PublishStatusEvent(e.CurrentRecordIndex.ToString, e.Message)
					Case EventType.Status
						Me.Context.PublishStatusEvent(e.CurrentRecordIndex.ToString, e.Message)
					Case EventType.Warning
						_warningCount += 1
						Me.Context.PublishWarningEvent(e.CurrentRecordIndex.ToString, e.Message)
					Case EventType.ResetStartTime
						SetStartTime()
					Case EventType.Count
						Me.Context.PublishRecordCountIncremented()
				End Select
			End SyncLock
			'Me.Context.PublishProgress(e.TotalLines, e.CurrentRecordIndex, 0, 0, _startTime, System.DateTime.Now)
		End Sub

		Private Sub _loadFileImporter_FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception, ByVal runID As String) Handles _loadFileImporter.FatalErrorEvent
			SyncLock Me.Context
				Me.Context.PublishFatalException(ex)
				'TODO: _loadFileImporter.ErrorLogFileName
				Me.Context.PublishProcessCompleted(False, "", True)
				_hasFatalErrorOccured = True
			End SyncLock
			Me.AuditRun(False, runID)
		End Sub

		Private Sub _loadFileImporter_UploadModeChangeEvent(ByVal statusBarText As String, ByVal tapiClientName As String, ByVal isBulkEnabled As Boolean) Handles _loadFileImporter.UploadModeChangeEvent
			If _uploadModeText Is Nothing Then
				Dim tapiObjectService As ITapiObjectService = New TapiObjectService
				_uploadModeText = tapiObjectService.BuildFileTransferModeDocText(True)
			End If
			Dim statusBarMessage As String = $"{statusBarText} - SQL Insert Mode: {If(isBulkEnabled, "Bulk", "Single")}"

			SendTransferJobStartedMessage()

			Me.Context.PublishStatusBarChanged(statusBarMessage, _uploadModeText)
		End Sub



		Private Sub _loadFileImporter_DataSourcePrepEvent(ByVal e As Api.DataSourcePrepEventArgs) Handles _loadFileImporter.DataSourcePrepEvent
			SyncLock Me.Context
				Dim totaldisplay As String
				Dim processeddisplay As String
				If e.TotalBytes >= 1048576 Then
					totaldisplay = (e.TotalBytes / 1048576).ToString("N0") & " MB"
					processeddisplay = (e.BytesRead / 1048576).ToString("N0") & " MB"
				ElseIf e.TotalBytes < 1048576 AndAlso e.TotalBytes >= 102400 Then
					totaldisplay = (e.TotalBytes / 1024).ToString("N0") & " KB"
					processeddisplay = (e.BytesRead / 1024).ToString("N0") & " KB"
				Else
					totaldisplay = e.TotalBytes.ToString & " B"
					processeddisplay = e.BytesRead.ToString & " B"
				End If
				Select Case e.Type
					Case Api.DataSourcePrepEventArgs.EventType.Close
						Me.Context.PublishProgress(e.TotalBytes, e.TotalBytes, 0, 0, e.StartTime, System.DateTime.Now, 0, 0, Me.ProcessID, totaldisplay, processeddisplay)
					Case Api.DataSourcePrepEventArgs.EventType.Open
						Me.Context.PublishProgress(e.TotalBytes, e.BytesRead, 0, 0, e.StartTime, System.DateTime.Now, 0, 0, Me.ProcessID, totaldisplay, processeddisplay)
						Me.Context.PublishStatusEvent("", "Preparing file for import")
					Case Api.DataSourcePrepEventArgs.EventType.ReadEvent
						Me.Context.PublishProgress(e.TotalBytes, e.BytesRead, 0, 0, e.StartTime, System.DateTime.Now, 0, 0, Me.ProcessID, totaldisplay, processeddisplay)
						Me.Context.PublishStatusEvent("", "Preparing file for import")
				End Select
			End SyncLock
		End Sub

		Private Sub _loadFileImporter_ReportErrorEvent(ByVal row As System.Collections.IDictionary) Handles _loadFileImporter.ReportErrorEvent
			Me.Context.PublishErrorReport(row)
		End Sub

		Private Sub _loadFileImporter_IoErrorEvent(ByVal sender As Object, ByVal e As IoWarningEventArgs) Handles _IoReporterContext.IoWarningEvent
			SyncLock Me.Context
				Me.Context.PublishWarningEvent((e.CurrentLineNumber + 1).ToString, e.Message)
			End SyncLock
		End Sub

		Private Sub _loadFileImporter_EndFileImport(ByVal runID As String) Handles _loadFileImporter.EndFileImport
			Me.AuditRun(True, runID)
		End Sub
	End Class

End Namespace