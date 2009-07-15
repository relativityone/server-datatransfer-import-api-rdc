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
		Public Property TimeZoneOffset() As Int32
			Get
				Return _timeZoneOffset
			End Get
			Set(ByVal value As Int32)
				_timeZoneOffset = value
			End Set
		End Property

		Protected Overrides Sub Execute()
			_startTime = DateTime.Now
			_warningCount = 0
			_errorCount = 0
			_loadFileImporter = New kCura.WinEDDS.BulkLoadFileImporter(LoadFile, ProcessController, _timeZoneOffset, True, Me.ProcessID, True)
			_newlineCounter = New kCura.Utility.File.LineCounter
			_newlineCounter.Path = LoadFile.FilePath
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

		Private Sub _loadFileImporter_StatusMessage(ByVal e As kCura.Windows.Process.StatusEventArgs) Handles _loadFileImporter.StatusMessage
			System.Threading.Monitor.Enter(Me.ProcessObserver)
			Select Case e.EventType
				Case kCura.Windows.Process.EventType.End
					Me.ProcessObserver.RaiseProgressEvent(e.TotalRecords, e.CurrentRecordIndex, _warningCount, _errorCount, _startTime, System.DateTime.Now)
					Me.ProcessObserver.RaiseStatusEvent(e.CurrentRecordIndex.ToString, e.Message)
				Case kCura.Windows.Process.EventType.Error
					_errorCount += 1
					Me.ProcessObserver.RaiseProgressEvent(e.TotalRecords, e.CurrentRecordIndex, _warningCount, _errorCount, _startTime, New DateTime)
					Me.ProcessObserver.RaiseErrorEvent(e.CurrentRecordIndex.ToString, e.Message)
				Case kCura.Windows.Process.EventType.Progress
					Me.ProcessObserver.RaiseProgressEvent(e.TotalRecords, e.CurrentRecordIndex, _warningCount, _errorCount, _startTime, New DateTime)
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

		Private Sub _loadFileImporter_FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception) Handles _loadFileImporter.FatalErrorEvent
			System.Threading.Monitor.Enter(Me.ProcessObserver)
			Me.ProcessObserver.RaiseFatalExceptionEvent(ex)
			Me.ProcessObserver.RaiseProcessCompleteEvent(False, _loadFileImporter.ErrorLogFileName, True)
			_hasRunPRocessComplete = True
			System.Threading.Monitor.Exit(Me.ProcessObserver)
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

		Private Sub _loadFileImporter_FilePrepEvent(ByVal e As BulkLoadFileImporter.FilePrepEventArgs) Handles _loadFileImporter.FilePrepEvent
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
				Case BulkLoadFileImporter.FilePrepEventArgs.FilePrepEventType.CloseFile
					Me.ProcessObserver.RaiseProgressEvent(e.TotalBytes, e.TotalBytes, 0, 0, e.StartTime, System.DateTime.Now, totaldisplay, processeddisplay)
					'Me.ProcessObserver.RaiseProcessCompleteEvent()
				Case BulkLoadFileImporter.FilePrepEventArgs.FilePrepEventType.OpenFile
					Me.ProcessObserver.RaiseProgressEvent(e.TotalBytes, e.BytesRead, 0, 0, e.StartTime, System.DateTime.Now, totaldisplay, processeddisplay)
					Me.ProcessObserver.RaiseStatusEvent("", "Preparing file for import")
				Case BulkLoadFileImporter.FilePrepEventArgs.FilePrepEventType.ReadEvent
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
					message.Append("Actual error: " & e.Exception.ToString)
			End Select
			Me.ProcessObserver.RaiseWarningEvent((e.CurrentLineNumber + 1).ToString, message.ToString)
			System.Threading.Monitor.Exit(Me.ProcessObserver)
		End Sub

	End Class

End Namespace