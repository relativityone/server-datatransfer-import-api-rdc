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

		Protected Overrides Sub Execute()
			_startTime = DateTime.Now
			_warningCount = 0
			_errorCount = 0
			Me.ProcessObserver.InputArgs = ImageLoadFile.FileName
			_imageFileImporter = Me.GetImageFileImporter
			_imageFileImporter.ReadFile(ImageLoadFile.FileName)
			If Not _hasRunProcessComplete Then
				Dim exportFilePath As String = ""
				If _imageFileImporter.HasErrors Then exportFilePath = System.Guid.NewGuid.ToString
				Me.ProcessObserver.RaiseProcessCompleteEvent(False, exportFilePath, True)
			End If
		End Sub

		Protected Overridable Function GetImageFileImporter() As kCura.WinEDDS.BulkImageFileImporter
			Dim returnImporter As BulkImageFileImporter = New kCura.WinEDDS.BulkImageFileImporter(ImageLoadFile.DestinationFolderID, ImageLoadFile, ProcessController, Me.ProcessID, True)

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
				Select Case ImageLoadFile.Overwrite.ToLower
					Case "none", "append"
						retval.Overwrite = EDDS.WebAPI.AuditManagerBase.OverwriteType.Append
					Case "strict", "overlay"
						retval.Overwrite = EDDS.WebAPI.AuditManagerBase.OverwriteType.Overlay
					Case Else
						retval.Overwrite = EDDS.WebAPI.AuditManagerBase.OverwriteType.Both
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
				Dim sb As New System.Text.StringBuilder
				_uploadModeText = Config.FileTransferModeExplanationText(True)
			End If
			Dim statusBarMessage As String = "File Transfer Mode: " & mode
			If isBulkEnabled Then
				statusBarMessage &= " - SQL Insert Mode: " & "Bulk"
			Else
				statusBarMessage &= " - SQL Insert Mode: " & "Single"
			End If
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