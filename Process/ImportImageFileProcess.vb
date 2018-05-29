Imports System.Threading
Imports kCura.WinEDDS.TApi
Imports Relativity.DataTransfer.MessageService

Namespace kCura.WinEDDS
	Public Class ImportImageFileProcess
		Inherits MonitoredProcessBase

		Public ImageLoadFile As ImageLoadFile
		Private WithEvents _imageFileImporter As kCura.WinEDDS.BulkImageFileImporter
		Protected WithEvents _ioWarningPublisher As IoWarningPublisher
		Private _errorCount As Int32
		Private _warningCount As Int32
		Private _uploadModeText As String = Nothing

		Private _disableUserSecurityCheck As Boolean
		Private _importAuditLevel As kCura.EDDS.WebAPI.BulkImportManagerBase.ImportAuditLevel = WinEDDS.Config.AuditLevel
		Private _disableImageTypeValidation As Boolean?
		Private _disableImageLocationValidation As Boolean?

		Public Sub New ()
			MyBase.New(new MessageService())
		End Sub

		Public Sub New (messageService As IMessageService)
			MyBase.New(messageService)
		End Sub

        Public WriteOnly Property DisableImageTypeValidation As Boolean
			Set(value As Boolean)
				_disableImageTypeValidation = value
			End Set
		End Property

		Public WriteOnly Property DisableImageLocationValidation As Boolean
			Set(value As Boolean)
				_disableImageLocationValidation = value
			End Set
		End Property

		Public WriteOnly Property DisableUserSecurityCheck As Boolean
			Set(value As Boolean)
				_disableUserSecurityCheck = value
			End Set
		End Property

		Public WriteOnly Property AuditLevel As kCura.EDDS.WebAPI.BulkImportManagerBase.ImportAuditLevel
			Set(value As kCura.EDDS.WebAPI.BulkImportManagerBase.ImportAuditLevel)
				_importAuditLevel = value
			End Set
		End Property

		Protected Overrides ReadOnly Property JobType As String = "Import"
		Protected Overrides ReadOnly Property TapiClientName As String
			Get
				If _imageFileImporter Is Nothing Then
					Return _tapiClientName
				Else
					Return _imageFileImporter.TapiClientName
				End If
			End Get
		End Property

		Public Property MaximumErrorCount As Int32?

		Public Property SkipExtractedTextEncodingCheck As Boolean?

		Public Property CloudInstance As Boolean

		Public Property EnforceDocumentLimit As Boolean

		Public Property ExecutionSource As Relativity.ExecutionSource

		Protected Overrides Function Run() As Boolean
			_imageFileImporter.ReadFile(ImageLoadFile.FileName)
			Return Not _hasFatalErrorOccured
		End Function

		Protected Overrides Function HasErrors() As Boolean
			Return _imageFileImporter.HasErrors
		End Function

		Protected Overrides Sub OnFatalError()
			SendTransferJobFailedMessage()
			MyBase.OnFatalError()
			SendJobStatistics()
		End Sub

		Protected Overrides Sub OnSuccess()
			MyBase.OnFatalError()
			SendJobStatistics()
			SendTransferJobCompletedMessage()
			Me.ProcessObserver.RaiseProcessCompleteEvent(False, "", True)
		End Sub

		Protected Overrides Sub OnHasErrors()
			MyBase.OnFatalError()
			SendJobStatistics()
			SendTransferJobCompletedMessage()
			Me.ProcessObserver.RaiseProcessCompleteEvent(False, System.Guid.NewGuid.ToString, True)
		End Sub

		Protected Overrides Sub Initialize()

			MyBase.Initialize()
			_warningCount = 0
			_errorCount = 0
			Me.ProcessObserver.InputArgs = ImageLoadFile.FileName
			_imageFileImporter = Me.GetImageFileImporter

			If _disableImageTypeValidation.HasValue Then _imageFileImporter.DisableImageTypeValidation = _disableImageTypeValidation.Value
			If _disableImageLocationValidation.HasValue Then _imageFileImporter.DisableImageLocationValidation = _disableImageLocationValidation.Value
			_imageFileImporter.DisableUserSecurityCheck = _disableUserSecurityCheck
			_imageFileImporter.AuditLevel = _importAuditLevel

			If MaximumErrorCount.HasValue AndAlso MaximumErrorCount.Value > 0 AndAlso MaximumErrorCount.Value < Int32.MaxValue Then
				'The '+1' is because the 'MaxNumberOfErrorsInGrid' is actually 1 more (because the
				' final error is simply 'Maximum # of errors' error) than the *actual* maximum, but
				' we don't want to change BulkImageFileImporter's behavior.
				' -Phil S. 07/10/2012
				_imageFileImporter.MaxNumberOfErrorsInGrid = MaximumErrorCount.Value + 1
			End If

			_imageFileImporter.SkipExtractedTextEncodingCheck = SkipExtractedTextEncodingCheck.GetValueOrDefault(False)
		End Sub

		Protected Overridable Function GetImageFileImporter() As kCura.WinEDDS.BulkImageFileImporter
			Dim tokenSource As CancellationTokenSource = New CancellationTokenSource()
		    _ioWarningPublisher = New IoWarningPublisher()
		    Dim logger As Relativity.Logging.ILog = RelativityLogFactory.CreateLog("WinEDDS")
		    Dim ioReporter As IIoReporter = IoReporterFactory.CreateIoReporter(kCura.Utility.Config.IOErrorNumberOfRetries, kCura.Utility.Config.IOErrorWaitTimeInSeconds, 
		                                                                       WinEDDS.Config.DisableNativeLocationValidation, logger, _ioWarningPublisher, tokenSource.Token)

            Dim returnImporter As BulkImageFileImporter = New kCura.WinEDDS.BulkImageFileImporter(ImageLoadFile.DestinationFolderID, ImageLoadFile, ProcessController, ioReporter, logger, Me.ProcessID, True, EnforceDocumentLimit, tokenSource, ExecutionSource)
			Return returnImporter
		End Function

		Private Sub AuditRun(ByVal success As Boolean, ByVal runID As String)
			Try
				Dim retval As New kCura.EDDS.WebAPI.AuditManagerBase.ImageImportStatistics
				retval.DestinationFolderArtifactID = ImageLoadFile.DestinationFolderID
				retval.BatchSizes = _imageFileImporter.BatchSizeHistoryList.ToArray
				If ImageLoadFile.ProductionArtifactID > 0 Then retval.DestinationProductionArtifactID = ImageLoadFile.ProductionArtifactID
				retval.ExtractedTextReplaced = ImageLoadFile.ReplaceFullText
				retval.ExtractedTextDefaultEncodingCodePageID = 0
				If retval.ExtractedTextReplaced Then retval.ExtractedTextDefaultEncodingCodePageID = ImageLoadFile.FullTextEncoding.CodePage
				If ImageLoadFile.CopyFilesToDocumentRepository Then
					retval.FilesCopiedToRepository = ImageLoadFile.SelectedCasePath
				Else
					retval.FilesCopiedToRepository = String.Empty
				End If
				retval.LoadFileName = System.IO.Path.GetFileName(ImageLoadFile.FileName)
				retval.NumberOfDocumentsCreated = _imageFileImporter.Statistics.DocumentsCreated
				retval.NumberOfDocumentsUpdated = _imageFileImporter.Statistics.DocumentsUpdated
				retval.NumberOfErrors = _errorCount
				retval.NumberOfFilesLoaded = _imageFileImporter.Statistics.FilesProcessed
				retval.NumberOfWarnings = _warningCount
				retval.OverlayIdentifierFieldArtifactID = ImageLoadFile.IdentityFieldId
				If ImageLoadFile.ProductionArtifactID > 0 Then retval.OverlayIdentifierFieldArtifactID = ImageLoadFile.BeginBatesFieldArtifactID
				Select Case CType([Enum].Parse(GetType(Relativity.ImportOverwriteType), ImageLoadFile.Overwrite, True), Relativity.ImportOverwriteType)
					Case Relativity.ImportOverwriteType.AppendOverlay
						retval.Overwrite = EDDS.WebAPI.AuditManagerBase.OverwriteType.Both
					Case Relativity.ImportOverwriteType.Overlay
						retval.Overwrite = EDDS.WebAPI.AuditManagerBase.OverwriteType.Overlay
					Case Else
						retval.Overwrite = EDDS.WebAPI.AuditManagerBase.OverwriteType.Append
				End Select
				Select Case _imageFileImporter.UploadConnection
					Case TApi.TapiClient.Aspera
					Case TApi.TapiClient.Web
						retval.RepositoryConnection = EDDS.WebAPI.AuditManagerBase.RepositoryConnectionType.Web
					Case TApi.TapiClient.Direct
						retval.RepositoryConnection = EDDS.WebAPI.AuditManagerBase.RepositoryConnectionType.Direct
				End Select
				retval.RunTimeInMilliseconds = CType(System.DateTime.Now.Subtract(StartTime).TotalMilliseconds, Int32)
				retval.SupportImageAutoNumbering = ImageLoadFile.AutoNumberImages
				retval.StartLine = CType(System.Math.Min(ImageLoadFile.StartLineNumber, Int32.MaxValue), Int32)
				retval.TotalFileSize = _imageFileImporter.Statistics.FileBytes
				retval.TotalMetadataBytes = _imageFileImporter.Statistics.MetadataBytes
				retval.SendNotification = ImageLoadFile.SendEmailOnLoadCompletion
				Dim auditmanager As New kCura.WinEDDS.Service.AuditManager(ImageLoadFile.Credential, ImageLoadFile.CookieContainer)

				auditmanager.AuditImageImport(ImageLoadFile.CaseInfo.ArtifactID, runID, Not success, retval)
			Catch
			End Try
		End Sub

		Private Sub _imageFileImporter_StatusMessage(ByVal e As StatusEventArgs) Handles _imageFileImporter.StatusMessage
			System.Threading.Monitor.Enter(Me.ProcessObserver)
			Dim additionalInfo As IDictionary = Nothing
			If Not e.AdditionalInfo Is Nothing Then additionalInfo = DirectCast(e.AdditionalInfo, IDictionary)
			Select Case e.EventType
				Case kCura.Windows.Process.EventType.Error
					If e.CountsTowardsTotal Then _errorCount += 1
					Me.ProcessObserver.RaiseErrorEvent(e.CurrentRecordIndex.ToString, e.Message)
				Case kCura.Windows.Process.EventType.Progress
					TotalRecords = e.TotalRecords
					CompletedRecordsCount = e.CurrentRecordIndex
					Me.ProcessObserver.RaiseStatusEvent(e.CurrentRecordIndex.ToString, e.Message)
					Me.ProcessObserver.RaiseProgressEvent(e.TotalRecords, e.CurrentRecordIndex, _warningCount, _errorCount, StartTime, New System.DateTime, e.Statistics.MetadataThroughput, e.Statistics.FileThroughput, Me.ProcessID, Nothing, Nothing, additionalInfo)
					SendThroughputStatistics(e.Statistics.MetadataThroughput, e.Statistics.FileThroughput)
					Me.ProcessObserver.RaiseRecordProcessed(e.CurrentRecordIndex)
				Case kCura.Windows.Process.EventType.Status
					Me.ProcessObserver.RaiseStatusEvent(e.CurrentRecordIndex.ToString, e.Message)
				Case kCura.Windows.Process.EventType.Warning
					If e.CountsTowardsTotal Then _warningCount += 1
					Me.ProcessObserver.RaiseWarningEvent(e.CurrentRecordIndex.ToString, e.Message)
				Case Windows.Process.EventType.Count
					Me.ProcessObserver.RaiseCountEvent()
				Case Windows.Process.EventType.ResetStartTime
					SetStartTime()
			End Select
			System.Threading.Monitor.Exit(Me.ProcessObserver)
		End Sub

		Private Sub _imageFileImporter_FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception) Handles _imageFileImporter.FatalErrorEvent
			System.Threading.Monitor.Enter(Me.ProcessObserver)
			Me.ProcessObserver.RaiseFatalExceptionEvent(ex)
			Me.ProcessObserver.RaiseProcessCompleteEvent(False, _imageFileImporter.ErrorLogFileName, True)
			_hasFatalErrorOccured = True
			System.Threading.Monitor.Exit(Me.ProcessObserver)
		End Sub

		Private Sub _imageFileImporter_ReportErrorEvent(ByVal row As System.Collections.IDictionary) Handles _imageFileImporter.ReportErrorEvent
			Me.ProcessObserver.RaiseReportErrorEvent(row)
		End Sub

		Private Sub _loadFileImporter_UploadModeChangeEvent(ByVal statusBarText As String, ByVal tapiClientName As String, ByVal isBulkEnabled As Boolean) Handles _imageFileImporter.UploadModeChangeEvent
			If _uploadModeText Is Nothing Then
				_uploadModeText = Config.FileTransferModeExplanationText(True)
			End If
			Dim statusBarMessage As String = $"{statusBarText} - SQL Insert Mode: {If(isBulkEnabled, "Bulk", "Single")}"

			SendTransferJobStartedMessage()

			Me.ProcessObserver.RaiseStatusBarEvent(statusBarMessage, _uploadModeText)
		End Sub

		Private Sub _imageFileImporter_IoErrorEvent(ByVal sender As Object, ByVal e As IoWarningEventArgs) Handles _ioWarningPublisher.IoWarningEvent
		    System.Threading.Monitor.Enter(Me.ProcessObserver)
		    Me.ProcessObserver.RaiseWarningEvent((e.CurrentLineNumber + 1).ToString, e.Message)
		    System.Threading.Monitor.Exit(Me.ProcessObserver)
		End Sub

		Private Sub _imageFileImporter_EndRun(ByVal success As Boolean, ByVal runID As String) Handles _imageFileImporter.EndRun
			Me.AuditRun(success, runID)
		End Sub
	End Class
End Namespace