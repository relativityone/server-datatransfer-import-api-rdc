Namespace kCura.WinEDDS
	Public Class ImportImageFileProcess
		Inherits kCura.Windows.Process.ProcessBase

		Public ImageLoadFile As ImageLoadFile
		Private WithEvents _imageFileImporter As kCura.WinEDDS.BulkImageFileImporter
		Private _startTime As DateTime
		Private _errorCount As Int32
		Private _warningCount As Int32
		Private _hasRunProcessComplete As Boolean = False
		Private _uploadModeText As String = Nothing

		Private _disableUserSecurityCheck As Boolean
		Private _importAuditLevel As kCura.EDDS.WebAPI.BulkImportManagerBase.ImportAuditLevel = WinEDDS.Config.AuditLevel
		Private _disableImageTypeValidation As Boolean?
		Private _disableImageLocationValidation As Boolean?

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

		Public Property MaximumErrorCount As Int32?

		Public Property SkipExtractedTextEncodingCheck As Boolean?

		Public Property CloudInstance As Boolean

		Public Property ExecutionSource As Relativity.ExecutionSource

		Protected Overrides Sub Execute()
			_startTime = DateTime.Now
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
			_imageFileImporter.ReadFile(ImageLoadFile.FileName)
			If Not _hasRunProcessComplete Then
				Dim exportFilePath As String = ""
				If _imageFileImporter.HasErrors Then exportFilePath = System.Guid.NewGuid.ToString
				Me.ProcessObserver.RaiseProcessCompleteEvent(False, exportFilePath, True)
			End If
		End Sub

		Protected Overridable Function GetImageFileImporter() As kCura.WinEDDS.BulkImageFileImporter
			Dim returnImporter As BulkImageFileImporter = New kCura.WinEDDS.BulkImageFileImporter(ImageLoadFile.DestinationFolderID, ImageLoadFile, ProcessController, Me.ProcessID, True, CloudInstance, ExecutionSource)
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
					Case FileUploader.Type.Web
						retval.RepositoryConnection = EDDS.WebAPI.AuditManagerBase.RepositoryConnectionType.Web
					Case FileUploader.Type.Direct
						retval.RepositoryConnection = EDDS.WebAPI.AuditManagerBase.RepositoryConnectionType.Direct
				End Select
				retval.RunTimeInMilliseconds = CType(System.DateTime.Now.Subtract(_startTime).TotalMilliseconds, Int32)
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

		Private Sub _imageFileImporter_StatusMessage(ByVal e As kCura.Windows.Process.StatusEventArgs) Handles _imageFileImporter.StatusMessage
			System.Threading.Monitor.Enter(Me.ProcessObserver)
			Dim additionalInfo As IDictionary = Nothing
			If Not e.AdditionalInfo Is Nothing Then additionalInfo = DirectCast(e.AdditionalInfo, IDictionary)
			Select Case e.EventType
				Case kCura.Windows.Process.EventType.Error
					If e.CountsTowardsTotal Then _errorCount += 1
					Me.ProcessObserver.RaiseErrorEvent(e.CurrentRecordIndex.ToString, e.Message)
				Case kCura.Windows.Process.EventType.Progress
					Me.ProcessObserver.RaiseStatusEvent(e.CurrentRecordIndex.ToString, e.Message)
					Me.ProcessObserver.RaiseProgressEvent(e.TotalRecords, e.CurrentRecordIndex, _warningCount, _errorCount, _startTime, New System.DateTime, Nothing, Nothing, additionalInfo)
					Me.ProcessObserver.RaiseRecordProcessed(e.CurrentRecordIndex)
				Case kCura.Windows.Process.EventType.Status
					Me.ProcessObserver.RaiseStatusEvent(e.CurrentRecordIndex.ToString, e.Message)
				Case kCura.Windows.Process.EventType.Warning
					If e.CountsTowardsTotal Then _warningCount += 1
					Me.ProcessObserver.RaiseWarningEvent(e.CurrentRecordIndex.ToString, e.Message)
				Case Windows.Process.EventType.Count
					Me.ProcessObserver.RaiseCountEvent()
			End Select
			System.Threading.Monitor.Exit(Me.ProcessObserver)
		End Sub

		Private Sub _imageFileImporter_FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception) Handles _imageFileImporter.FatalErrorEvent
			System.Threading.Monitor.Enter(Me.ProcessObserver)
			Me.ProcessObserver.RaiseFatalExceptionEvent(ex)
			Me.ProcessObserver.RaiseProcessCompleteEvent(False, _imageFileImporter.ErrorLogFileName, True)
			_hasRunProcessComplete = True
			System.Threading.Monitor.Exit(Me.ProcessObserver)
		End Sub

		Private Sub _imageFileImporter_ReportErrorEvent(ByVal row As System.Collections.IDictionary) Handles _imageFileImporter.ReportErrorEvent
			Me.ProcessObserver.RaiseReportErrorEvent(row)
		End Sub

		Private Sub _loadFileImporter_UploadModeChangeEvent(ByVal mode As String, ByVal isBulkEnabled As Boolean) Handles _imageFileImporter.UploadModeChangeEvent
			If _uploadModeText Is Nothing Then
				_uploadModeText = Config.FileTransferModeExplanationText(True)
			End If
			Dim statusBarMessage As String = String.Format("{0} - SQL Insert Mode: {1}", mode, If(isBulkEnabled, "Bulk", "Single"))
			Me.ProcessObserver.RaiseStatusBarEvent(statusBarMessage, _uploadModeText)
		End Sub

		Private Sub _imageFileImporter_IoErrorEvent(ByVal e As kCura.Utility.DelimitedFileImporter.IoWarningEventArgs) Handles _imageFileImporter.IoWarningEvent
			System.Threading.Monitor.Enter(Me.ProcessObserver)
			Dim message As New System.Text.StringBuilder
			Select Case e.Type
				Case kCura.Utility.DelimitedFileImporter.IoWarningEventArgs.TypeEnum.Message
					message.Append(e.Message)
				Case Else
					message.Append("Error accessing opticon file - retrying")
					If e.WaitTime > 0 Then message.Append(" in " & e.WaitTime & " seconds")
					message.Append(vbNewLine)
					message.Append("Actual error: " & e.Exception.ToString)
			End Select
			Me.ProcessObserver.RaiseWarningEvent((e.CurrentLineNumber + 1).ToString, message.ToString)
			System.Threading.Monitor.Exit(Me.ProcessObserver)
		End Sub

		Private Sub _imageFileImporter_EndRun(ByVal success As Boolean, ByVal runID As String) Handles _imageFileImporter.EndRun
			Me.AuditRun(success, runID)
		End Sub
	End Class
End Namespace