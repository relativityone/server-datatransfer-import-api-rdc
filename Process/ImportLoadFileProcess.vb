Imports System.Threading
Imports kCura.WinEDDS.TApi
Imports Relativity.DataTransfer.MessageService

Namespace kCura.WinEDDS

	Public Class ImportLoadFileProcess
		Inherits MonitoredProcessBase

		Public LoadFile As LoadFile
		Protected WithEvents _loadFileImporter As BulkLoadFileImporter
		Protected WithEvents _ioWarningPublisher As IoWarningPublisher
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

		Public Property ExecutionSource() As Relativity.ExecutionSource

		Public Property TimeZoneOffset() As Int32
			Get
				Return _timeZoneOffset
			End Get
			Set(ByVal value As Int32)
				_timeZoneOffset = value
			End Set
		End Property

		Public Overridable Function GetImporter() As kCura.WinEDDS.BulkLoadFileImporter
			Dim tokenSource As CancellationTokenSource = New CancellationTokenSource()
			_ioWarningPublisher = New IoWarningPublisher()

			Dim logger As Relativity.Logging.ILog = RelativityLogFactory.CreateLog(RelativityLogFactory.WinEDDSSubSystem)
			Dim reporter As IIoReporter = IoReporterFactory.CreateIoReporter(
				kCura.Utility.Config.IOErrorNumberOfRetries, _
				kCura.Utility.Config.IOErrorWaitTimeInSeconds, _ 
				WinEDDS.Config.DisableNativeLocationValidation, _
				logger, _
				_ioWarningPublisher, _
				tokenSource.Token)
			Dim returnImporter As BulkLoadFileImporter = New kCura.WinEDDS.BulkLoadFileImporter(
				LoadFile, _
				ProcessController, _
				reporter, _
				logger, _
				_timeZoneOffset, _
				True, _
				Me.ProcessID, _
				True, _
				BulkLoadFileFieldDelimiter, _
				EnforceDocumentLimit, _
				tokenSource, _
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
			Me.ProcessObserver.RaiseProcessCompleteEvent(False, "", True)
		End Sub

		Protected Overrides Sub OnHasErrors()
			MyBase.OnHasErrors()
			SendJobStatistics(_loadFileImporter.Statistics)
			SendTransferJobCompletedMessage()
			Me.ProcessObserver.RaiseProcessCompleteEvent(False, System.Guid.NewGuid.ToString, True)
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
			_loadFileImporter.SkipExtractedTextEncodingCheck = (_loadFileImporter.SkipExtractedTextEncodingCheck OrElse Config.DisableTextFileEncodingCheck)

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
			_loadFileImporter.LoadImportedFullTextFromServer = (Me.LoadImportedFullTextFromServer OrElse Config.LoadImportedFullTextFromServer)
			Me.ProcessObserver.InputArgs = LoadFile.FilePath
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
				If LoadFile.ArtifactTypeID <> Relativity.ArtifactType.Document Then retval.DestinationFolderArtifactID = -1
				Dim fieldMap As New System.Collections.ArrayList
				Dim mappedExtractedText As Boolean = False
				For Each item As WinEDDS.LoadFileFieldMap.LoadFileFieldMapItem In LoadFile.FieldMap
					If Not item.DocumentField Is Nothing AndAlso item.NativeFileColumnIndex > -1 Then
						fieldMap.Add(New Int32() {item.NativeFileColumnIndex, item.DocumentField.FieldID})
						If item.DocumentField.FieldCategory = Relativity.FieldCategory.FullText Then mappedExtractedText = True
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
				Select Case CType([Enum].Parse(GetType(Relativity.ImportOverwriteType), LoadFile.OverwriteDestination, True), Relativity.ImportOverwriteType)
					Case Relativity.ImportOverwriteType.Overlay
						retval.Overwrite = EDDS.WebAPI.AuditManagerBase.OverwriteType.Overlay
					Case Relativity.ImportOverwriteType.AppendOverlay
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
						Case TApi.TapiClient.Direct
							retval.RepositoryConnection = EDDS.WebAPI.AuditManagerBase.RepositoryConnectionType.Direct
						Case TApi.TapiClient.Aspera
						Case TApi.TapiClient.Web
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
			Me.ProcessObserver.RaiseFieldMapped(sourceField, workspaceField)
		End Sub

		Private Sub _loadFileImporter_StatusMessage(ByVal e As StatusEventArgs) Handles _loadFileImporter.StatusMessage
			SyncLock Me.ProcessObserver
				Dim statisticsDictionary As IDictionary = Nothing
				If Not e.AdditionalInfo Is Nothing Then statisticsDictionary = DirectCast(e.AdditionalInfo, IDictionary)
				Select Case e.EventType
					Case kCura.Windows.Process.EventType.End
						Me.ProcessObserver.RaiseProgressEvent(e.TotalRecords, e.CurrentRecordIndex, _warningCount, _errorCount, StartTime, System.DateTime.Now, e.Statistics.MetadataThroughput, e.Statistics.FileThroughput, Me.ProcessID, Nothing, Nothing, statisticsDictionary)
						Me.ProcessObserver.RaiseStatusEvent(e.CurrentRecordIndex.ToString, e.Message)
						Me.ProcessObserver.RaiseEndEvent(e.Statistics.FileBytes, e.Statistics.MetadataBytes)
					Case kCura.Windows.Process.EventType.Error
						_errorCount += 1
						Me.ProcessObserver.RaiseProgressEvent(e.TotalRecords, e.CurrentRecordIndex, _warningCount, _errorCount, StartTime, New DateTime, e.Statistics.MetadataThroughput, e.Statistics.FileThroughput, Me.ProcessID, Nothing, Nothing, statisticsDictionary)
						Me.ProcessObserver.RaiseErrorEvent(e.CurrentRecordIndex.ToString, e.Message)
					Case kCura.Windows.Process.EventType.Progress
						TotalRecords = e.TotalRecords
						CompletedRecordsCount = e.CurrentRecordIndex
						Me.ProcessObserver.RaiseRecordProcessed(e.CurrentRecordIndex)
						Me.ProcessObserver.RaiseProgressEvent(e.TotalRecords, e.CurrentRecordIndex, _warningCount, _errorCount, StartTime, New DateTime, e.Statistics.MetadataThroughput, e.Statistics.FileThroughput, Me.ProcessID, Nothing, Nothing, statisticsDictionary)
						Me.ProcessObserver.RaiseStatusEvent(e.CurrentRecordIndex.ToString, e.Message)
					Case kCura.Windows.Process.EventType.Statistics
						SendThroughputStatistics(e.Statistics.MetadataThroughput, e.Statistics.FileThroughput)
					Case kCura.Windows.Process.EventType.ResetProgress
						' Do NOT raise RaiseRecordProcessed for this event. 
						CompletedRecordsCount = 0
						Me.ProcessObserver.RaiseProgressEvent(e.TotalRecords, e.CurrentRecordIndex, _warningCount, _errorCount, StartTime, New DateTime, e.Statistics.MetadataThroughput, e.Statistics.FileThroughput, Me.ProcessID, Nothing, Nothing, statisticsDictionary)
						Me.ProcessObserver.RaiseStatusEvent(e.CurrentRecordIndex.ToString, e.Message)
					Case kCura.Windows.Process.EventType.Status
						Me.ProcessObserver.RaiseStatusEvent(e.CurrentRecordIndex.ToString, e.Message)
					Case kCura.Windows.Process.EventType.Warning
						_warningCount += 1
						Me.ProcessObserver.RaiseWarningEvent(e.CurrentRecordIndex.ToString, e.Message)
					Case Windows.Process.EventType.ResetStartTime
						SetStartTime()
					Case Windows.Process.EventType.Count
						Me.ProcessObserver.RaiseCountEvent()
				End Select
			End SyncLock
			'Me.ProcessObserver.RaiseProgressEvent(e.TotalLines, e.CurrentRecordIndex, 0, 0, _startTime, System.DateTime.Now)
		End Sub

		Private Sub _loadFileImporter_FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception, ByVal runID As String) Handles _loadFileImporter.FatalErrorEvent
			SyncLock Me.ProcessObserver
				Me.ProcessObserver.RaiseFatalExceptionEvent(ex)
				'TODO: _loadFileImporter.ErrorLogFileName
				Me.ProcessObserver.RaiseProcessCompleteEvent(False, "", True)
				_hasFatalErrorOccured = True
			End SyncLock
			Me.AuditRun(False, runID)
		End Sub

		Private Sub _loadFileImporter_UploadModeChangeEvent(ByVal statusBarText As String, ByVal tapiClientName As String, ByVal isBulkEnabled As Boolean) Handles _loadFileImporter.UploadModeChangeEvent
			If _uploadModeText Is Nothing Then
				_uploadModeText = Config.FileTransferModeExplanationText(True)
			End If
			Dim statusBarMessage As String = $"{statusBarText} - SQL Insert Mode: {If(isBulkEnabled, "Bulk", "Single")}"

			SendTransferJobStartedMessage()

			Me.ProcessObserver.RaiseStatusBarEvent(statusBarMessage, _uploadModeText)
		End Sub



		Private Sub _loadFileImporter_DataSourcePrepEvent(ByVal e As Api.DataSourcePrepEventArgs) Handles _loadFileImporter.DataSourcePrepEvent
			SyncLock Me.ProcessObserver
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
						Me.ProcessObserver.RaiseProgressEvent(e.TotalBytes, e.TotalBytes, 0, 0, e.StartTime, System.DateTime.Now, 0, 0, Me.ProcessID, totaldisplay, processeddisplay)
					Case Api.DataSourcePrepEventArgs.EventType.Open
						Me.ProcessObserver.RaiseProgressEvent(e.TotalBytes, e.BytesRead, 0, 0, e.StartTime, System.DateTime.Now, 0, 0, Me.ProcessID, totaldisplay, processeddisplay)
						Me.ProcessObserver.RaiseStatusEvent("", "Preparing file for import")
					Case Api.DataSourcePrepEventArgs.EventType.ReadEvent
						Me.ProcessObserver.RaiseProgressEvent(e.TotalBytes, e.BytesRead, 0, 0, e.StartTime, System.DateTime.Now, 0, 0, Me.ProcessID, totaldisplay, processeddisplay)
						Me.ProcessObserver.RaiseStatusEvent("", "Preparing file for import")
				End Select
			End SyncLock
		End Sub

		Private Sub _loadFileImporter_ReportErrorEvent(ByVal row As System.Collections.IDictionary) Handles _loadFileImporter.ReportErrorEvent
			Me.ProcessObserver.RaiseReportErrorEvent(row)
		End Sub

		Private Sub _loadFileImporter_IoErrorEvent(ByVal sender As Object, ByVal e As IoWarningEventArgs) Handles _ioWarningPublisher.IoWarningEvent
			SyncLock Me.ProcessObserver
				Me.ProcessObserver.RaiseWarningEvent((e.CurrentLineNumber + 1).ToString, e.Message)
			End SyncLock
		End Sub

		Private Sub _loadFileImporter_EndFileImport(ByVal runID As String) Handles _loadFileImporter.EndFileImport
			Me.AuditRun(True, runID)
		End Sub
	End Class

End Namespace