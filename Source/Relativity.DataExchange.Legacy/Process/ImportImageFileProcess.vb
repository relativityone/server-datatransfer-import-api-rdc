Imports Monitoring
Imports Monitoring.Sinks
Imports Relativity.DataExchange
Imports Relativity.DataExchange.Io
Imports Relativity.DataExchange.Logger
Imports Relativity.DataExchange.Process
Imports Relativity.DataExchange.Service
Imports Relativity.DataExchange.Transfer

Namespace kCura.WinEDDS
	Public Class ImportImageFileProcess
		Inherits MonitoredProcessBase

		Public ImageLoadFile As ImageLoadFile
		Private WithEvents _imageFileImporter As kCura.WinEDDS.BulkImageFileImporter
		Protected WithEvents _ioReporterContext As IoReporterContext
		Private _errorCount As Int32
		Private _warningCount As Int32
		Private _uploadModeText As String = Nothing

		Private _disableUserSecurityCheck As Boolean
		Private _importAuditLevel As kCura.EDDS.WebAPI.BulkImportManagerBase.ImportAuditLevel = Config.AuditLevel
		Private _disableImageTypeValidation As Boolean?
		Private _disableImageLocationValidation As Boolean?

		<Obsolete("This constructor is marked for deprecation. Please use the constructor that requires a logger instance.")>
		Public Sub New()
			Me.New(new MetricService(New ImportApiMetricSinkConfig))
		End Sub

		<Obsolete("This constructor is marked for deprecation. Please use the constructor that requires a logger instance.")>
		Public Sub New (metricService As IMetricService)
			Me.New(metricService, RelativityLogger.Instance)
		End Sub

		Public Sub New (metricService As IMetricService, logger As Global.Relativity.Logging.ILog)
			MyBase.New(metricService, logger)
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

		Protected Overrides ReadOnly Property TransferDirection As TelemetryConstants.TransferDirection = TelemetryConstants.TransferDirection.Import

		Protected Overrides ReadOnly Property TapiClient As TapiClient
			Get
				If _imageFileImporter Is Nothing Then
					Return TapiClient.None
				Else
					Return _imageFileImporter.TapiClient
				End If
			End Get
		End Property

		Protected Overrides ReadOnly Property Statistics As Statistics
			Get
				Return _imageFileImporter.Statistics
			End Get
		End Property

		Public Property MaximumErrorCount As Int32?

		Public Property SkipExtractedTextEncodingCheck As Boolean?

		Public Property CloudInstance As Boolean

		Public Property EnforceDocumentLimit As Boolean

		Protected Overrides Function Run() As Boolean
			_imageFileImporter.ReadFile(ImageLoadFile.FileName)
			Return Not _hasFatalErrorOccured
		End Function

		Protected Overrides Function HasErrors() As Boolean
			Return _imageFileImporter.HasErrors
		End Function

		Protected Overrides Function IsCancelled() As Boolean
			Return _imageFileImporter.IsCancelledByUser
		End Function

		''' <inheritdoc/>
		Protected Overrides Function GetTotalRecordsCount() As Long
			Return _imageFileImporter.TotalRecords
		End Function

		''' <inheritdoc/>
		Protected Overrides Function GetCompletedRecordsCount() As Long
			Return __imageFileImporter.CompletedRecords
		End Function

		Protected Overrides Sub OnSuccess()
			MyBase.OnSuccess()
			Me.Context.PublishProcessCompleted(False, "", True)
		End Sub

		Protected Overrides Sub OnHasErrors()
			MyBase.OnHasErrors()
			Me.Context.PublishProcessCompleted(False, System.Guid.NewGuid.ToString, True)
		End Sub

		Protected Overrides Sub Initialize()

			MyBase.Initialize()
			_warningCount = 0
			_errorCount = 0
			Me.Context.InputArgs = ImageLoadFile.FileName
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
		    _ioReporterContext = New IoReporterContext(Me.FileSystem, Me.AppSettings, New WaitAndRetryPolicy(Me.AppSettings))
		    Dim reporter As IIoReporter = Me.CreateIoReporter(_ioReporterContext)
            Dim returnImporter As BulkImageFileImporter = New kCura.WinEDDS.BulkImageFileImporter(
	            ImageLoadFile.DestinationFolderID, _
	            ImageLoadFile, _
	            Me.Context, _
	            reporter, _
	            logger, _
	            Me.ProcessID, _
	            True, _
	            EnforceDocumentLimit, _
	            Me.CancellationTokenSource, _
	            ExecutionSource)
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
				Select Case CType([Enum].Parse(GetType(ImportOverwriteType), ImageLoadFile.Overwrite, True), ImportOverwriteType)
					Case ImportOverwriteType.AppendOverlay
						retval.Overwrite = EDDS.WebAPI.AuditManagerBase.OverwriteType.Both
					Case ImportOverwriteType.Overlay
						retval.Overwrite = EDDS.WebAPI.AuditManagerBase.OverwriteType.Overlay
					Case Else
						retval.Overwrite = EDDS.WebAPI.AuditManagerBase.OverwriteType.Append
				End Select
				Select Case _imageFileImporter.UploadConnection
					Case TapiClient.Aspera
					Case TapiClient.Web
						retval.RepositoryConnection = EDDS.WebAPI.AuditManagerBase.RepositoryConnectionType.Web
					Case TapiClient.Direct
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
			SyncLock Me.Context
				Dim additionalInfo As IDictionary = Nothing
				If Not e.AdditionalInfo Is Nothing Then additionalInfo = DirectCast(e.AdditionalInfo, IDictionary)
				Select Case e.EventType
					Case EventType2.Error
						If e.CountsTowardsTotal Then _errorCount += 1
						Me.Context.PublishProgress(e.TotalRecords, e.CurrentRecordIndex, _warningCount, _errorCount, StartTime, New System.DateTime, e.Statistics.MetadataThroughput, e.Statistics.FileThroughput, Me.ProcessID, Nothing, Nothing, additionalInfo)
						Me.Context.PublishErrorEvent(e.CurrentRecordIndex.ToString, e.Message)
					Case EventType2.Progress
						Me.Context.PublishStatusEvent(e.CurrentRecordIndex.ToString, e.Message)
						Me.Context.PublishProgress(e.TotalRecords, e.CurrentRecordIndex, _warningCount, _errorCount, StartTime, New System.DateTime, e.Statistics.MetadataThroughput, e.Statistics.FileThroughput, Me.ProcessID, Nothing, Nothing, additionalInfo)
						Me.Context.PublishRecordProcessed(e.CurrentRecordIndex)
					Case EventType2.Statistics
						SendMetricJobProgress(e.Statistics, checkThrottling := True)
					Case EventType2.Status
						Me.Context.PublishStatusEvent(e.CurrentRecordIndex.ToString, e.Message)
					Case EventType2.Warning
						If e.CountsTowardsTotal Then _warningCount += 1
						Me.Context.PublishWarningEvent(e.CurrentRecordIndex.ToString, e.Message)
					Case EventType2.Count
						Me.Context.PublishRecordCountIncremented()
					Case EventType2.ResetStartTime
						SetStartTime()
				End Select
			End SyncLock
		End Sub

		Private Sub _imageFileImporter_FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception) Handles _imageFileImporter.FatalErrorEvent
			SyncLock Me.Context
				Me.Context.PublishFatalException(ex)
				Me.Context.PublishProcessCompleted(False, _imageFileImporter.ErrorLogFileName, True)
				_hasFatalErrorOccured = True
			End SyncLock
		End Sub

		Private Sub _imageFileImporter_ReportErrorEvent(ByVal row As System.Collections.IDictionary) Handles _imageFileImporter.ReportErrorEvent
			Me.Context.PublishErrorReport(row)
		End Sub

		Private Sub _loadFileImporter_UploadModeChangeEvent(ByVal statusBarText As String) Handles _imageFileImporter.UploadModeChangeEvent
			If _uploadModeText Is Nothing Then
				_uploadModeText = TapiModeHelper.BuildDocText()
			End If

			OnTapiClientChanged()
			Me.Context.PublishStatusBarChanged(statusBarText, _uploadModeText)
		End Sub

		Private Sub _imageFileImporter_IoErrorEvent(ByVal sender As Object, ByVal e As IoWarningEventArgs) Handles _ioReporterContext.IoWarningEvent
			SyncLock Me.Context
				Me.Context.PublishWarningEvent((e.CurrentLineNumber + 1).ToString, e.Message)
			End SyncLock
		End Sub

		Private Sub _imageFileImporter_EndRun(ByVal success As Boolean, ByVal runID As String) Handles _imageFileImporter.EndRun
			Me.AuditRun(success, runID)
		End Sub
	End Class
End Namespace