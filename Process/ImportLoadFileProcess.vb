
Namespace kCura.WinEDDS

	Public Class ImportLoadFileProcess
		Inherits kCura.Windows.Process.ProcessBase

		Public LoadFile As LoadFile
		Protected WithEvents _loadFileImporter As BulkLoadFileImporter
		Private _startTime As System.DateTime
		Private _errorCount As Int32
		Private _warningCount As Int32
		Private _timeZoneOffset As Int32
		Private WithEvents _newlineCounter As kCura.Utility.File.LineCounter
		Private _hasRunPRocessComplete As Boolean = False
		Private _uploadModeText As String = Nothing

		Private _disableUserSecutityCheck As Boolean
		Private _disableNativeValidation As Boolean?
		Private _disableNativeLocationValidation As Boolean?
		Private _auditLevel As kCura.EDDS.WebAPI.BulkImportManagerBase.ImportAuditLevel = Config.AuditLevel

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

		''' <summary>
		''' Gets or sets the delimiter to use to separate fields in the bulk
		''' file created in this process. Line delimiters will be this value plus a line feed.
		''' </summary>
		Public Property BulkLoadFileFieldDelimiter As String

		Public Property TimeZoneOffset() As Int32
			Get
				Return _timeZoneOffset
			End Get
			Set(ByVal value As Int32)
				_timeZoneOffset = value
			End Set
		End Property

		Public Overridable Function GetImporter() As kCura.WinEDDS.BulkLoadFileImporter
			Dim returnImporter As BulkLoadFileImporter = New kCura.WinEDDS.BulkLoadFileImporter(LoadFile, ProcessController, _timeZoneOffset, True, Me.ProcessID, True, BulkLoadFileFieldDelimiter)

			Return returnImporter
		End Function

		Protected Overrides Sub Execute()
			_startTime = DateTime.Now
			_warningCount = 0
			_errorCount = 0
			_loadFileImporter = Me.GetImporter()

			If _disableNativeValidation.HasValue Then _loadFileImporter.DisableNativeValidation = _disableNativeValidation.Value
			If _disableNativeLocationValidation.HasValue Then _loadFileImporter.DisableNativeLocationValidation = _disableNativeLocationValidation.Value
			_loadFileImporter.AuditLevel = _auditLevel
			_loadFileImporter.DisableUserSecurityCheck = _disableUserSecutityCheck

			'_newlineCounter = New kCura.Utility.File.Instance.LineCounter
			'_newlineCounter.Path = LoadFile.FilePath
			Me.ProcessObserver.InputArgs = LoadFile.FilePath
			If (CType(_loadFileImporter.ReadFile(LoadFile.FilePath), Boolean)) AndAlso Not _hasRunPRocessComplete Then
				If _loadFileImporter.HasErrors Then
					Me.ProcessObserver.RaiseProcessCompleteEvent(False, System.Guid.NewGuid.ToString, True)
				Else
					Me.ProcessObserver.RaiseProcessCompleteEvent(False, "", True)
				End If
			Else
				Me.ProcessObserver.RaiseStatusEvent("", "Import aborted")
			End If
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
				Select Case LoadFile.OverwriteDestination.ToLower
					Case "strict"
						retval.Overwrite = EDDS.WebAPI.AuditManagerBase.OverwriteType.Overlay
					Case "none"
						retval.Overwrite = EDDS.WebAPI.AuditManagerBase.OverwriteType.Append
					Case Else
						retval.Overwrite = EDDS.WebAPI.AuditManagerBase.OverwriteType.Both
				End Select
				If LoadFile.CopyFilesToDocumentRepository Then
					Select Case _loadFileImporter.UploadConnection
						Case FileUploader.Type.Direct
							retval.RepositoryConnection = EDDS.WebAPI.AuditManagerBase.RepositoryConnectionType.Direct
						Case FileUploader.Type.Web
							retval.RepositoryConnection = EDDS.WebAPI.AuditManagerBase.RepositoryConnectionType.Web
					End Select
					retval.TotalFileSize = _loadFileImporter.Statistics.FileBytes
				End If
				retval.RunTimeInMilliseconds = CType(System.DateTime.Now.Subtract(_startTime).TotalMilliseconds, Int32)
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

		Private Sub _loadFileImporter_StatusMessage(ByVal e As kCura.Windows.Process.StatusEventArgs) Handles _loadFileImporter.StatusMessage
			System.Threading.Monitor.Enter(Me.ProcessObserver)
			Dim statisticsDictionary As IDictionary = Nothing
			If Not e.AdditionalInfo Is Nothing Then statisticsDictionary = DirectCast(e.AdditionalInfo, IDictionary)
			Select Case e.EventType
				Case kCura.Windows.Process.EventType.End
					Me.ProcessObserver.RaiseProgressEvent(e.TotalRecords, e.CurrentRecordIndex, _warningCount, _errorCount, _startTime, System.DateTime.Now, Nothing, Nothing, statisticsDictionary)
					Me.ProcessObserver.RaiseStatusEvent(e.CurrentRecordIndex.ToString, e.Message)
				Case kCura.Windows.Process.EventType.Error
					_errorCount += 1
					Me.ProcessObserver.RaiseProgressEvent(e.TotalRecords, e.CurrentRecordIndex, _warningCount, _errorCount, _startTime, New DateTime, Nothing, Nothing, statisticsDictionary)
					Me.ProcessObserver.RaiseErrorEvent(e.CurrentRecordIndex.ToString, e.Message)
				Case kCura.Windows.Process.EventType.Progress
					Me.ProcessObserver.RaiseRecordProcessed(e.CurrentRecordIndex)
					Me.ProcessObserver.RaiseProgressEvent(e.TotalRecords, e.CurrentRecordIndex, _warningCount, _errorCount, _startTime, New DateTime, Nothing, Nothing, statisticsDictionary)
					Me.ProcessObserver.RaiseStatusEvent(e.CurrentRecordIndex.ToString, e.Message)
				Case kCura.Windows.Process.EventType.Status
					Me.ProcessObserver.RaiseStatusEvent(e.CurrentRecordIndex.ToString, e.Message)
				Case kCura.Windows.Process.EventType.Warning
					_warningCount += 1
					Me.ProcessObserver.RaiseWarningEvent(e.CurrentRecordIndex.ToString, e.Message)
				Case Windows.Process.EventType.ResetStartTime
					_startTime = System.DateTime.Now
			End Select
			System.Threading.Monitor.Exit(Me.ProcessObserver)
			'Me.ProcessObserver.RaiseProgressEvent(e.TotalLines, e.CurrentRecordIndex, 0, 0, _startTime, System.DateTime.Now)
		End Sub

		Private Sub _loadFileImporter_FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception, ByVal runID As String) Handles _loadFileImporter.FatalErrorEvent
			System.Threading.Monitor.Enter(Me.ProcessObserver)
			Me.ProcessObserver.RaiseFatalExceptionEvent(ex)
			'TODO: _loadFileImporter.ErrorLogFileName
			Me.ProcessObserver.RaiseProcessCompleteEvent(False, "", True)
			_hasRunPRocessComplete = True
			System.Threading.Monitor.Exit(Me.ProcessObserver)
			Me.AuditRun(False, runID)
		End Sub

		Private Sub _loadFileImporter_UploadModeChangeEvent(ByVal mode As String, ByVal isBulkEnabled As Boolean) Handles _loadFileImporter.UploadModeChangeEvent
			If _uploadModeText Is Nothing Then
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

		Private Sub _loadFileImporter_DataSourcePrepEvent(ByVal e As Api.DataSourcePrepEventArgs) Handles _loadFileImporter.DataSourcePrepEvent
			System.Threading.Monitor.Enter(Me.ProcessObserver)
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
					Me.ProcessObserver.RaiseProgressEvent(e.TotalBytes, e.TotalBytes, 0, 0, e.StartTime, System.DateTime.Now, totaldisplay, processeddisplay)
				Case Api.DataSourcePrepEventArgs.EventType.Open
					Me.ProcessObserver.RaiseProgressEvent(e.TotalBytes, e.BytesRead, 0, 0, e.StartTime, System.DateTime.Now, totaldisplay, processeddisplay)
					Me.ProcessObserver.RaiseStatusEvent("", "Preparing file for import")
				Case Api.DataSourcePrepEventArgs.EventType.ReadEvent
					Me.ProcessObserver.RaiseProgressEvent(e.TotalBytes, e.BytesRead, 0, 0, e.StartTime, System.DateTime.Now, totaldisplay, processeddisplay)
					Me.ProcessObserver.RaiseStatusEvent("", "Preparing file for import")
			End Select
			System.Threading.Monitor.Exit(Me.ProcessObserver)
		End Sub

		Private Sub _loadFileImporter_ReportErrorEvent(ByVal row As System.Collections.IDictionary) Handles _loadFileImporter.ReportErrorEvent
			Me.ProcessObserver.RaiseReportErrorEvent(row)
		End Sub

		Private Sub _imageFileImporter_IoErrorEvent(ByVal e As kCura.Utility.DelimitedFileImporter.IoWarningEventArgs) Handles _loadFileImporter.IoWarningEvent
			System.Threading.Monitor.Enter(Me.ProcessObserver)
			Dim message As New System.Text.StringBuilder
			Select Case e.Type
				Case kCura.Utility.DelimitedFileImporter.IoWarningEventArgs.TypeEnum.Message
					message.Append(e.Message)
				Case Else
					message.Append("Error accessing load file - retrying")
					If e.WaitTime > 0 Then message.Append(" in " & e.WaitTime & " seconds")
					message.Append(vbNewLine)
					message.Append("Actual error: " & e.Exception.Message)
			End Select
			Me.ProcessObserver.RaiseWarningEvent((e.CurrentLineNumber + 1).ToString, message.ToString)
			System.Threading.Monitor.Exit(Me.ProcessObserver)
		End Sub

		Private Sub _loadFileImporter_EndFileImport(ByVal runID As String) Handles _loadFileImporter.EndFileImport
			Me.AuditRun(True, runID)
		End Sub
	End Class

End Namespace