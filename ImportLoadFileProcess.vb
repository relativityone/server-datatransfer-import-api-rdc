Namespace kCura.WinEDDS

	Public Class ImportLoadFileProcess
    Inherits kCura.Windows.Process.ProcessBase

		Public LoadFile As LoadFile
		Protected WithEvents _loadFileImporter As LoadFileImporter
		Private _startTime As System.DateTime
		Private _errorCount As Int32
		Private _warningCount As Int32
		Private _timeZoneOffset As Int32
		Private WithEvents _newlineCounter As kCura.Utility.File.LineCounter
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
			_loadFileImporter = New kCura.WinEDDS.LoadFileImporter(LoadFile, ProcessController, _timeZoneOffset)
			_newlineCounter = New kCura.Utility.File.LineCounter
			_newlineCounter.Path = LoadFile.FilePath
			If (CType(_loadFileImporter.ReadFile(LoadFile.FilePath), Boolean)) Then
				Me.ProcessObserver.RaiseProcessCompleteEvent()
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
					Me.ProcessObserver.RaiseErrorEvent(e.CurrentRecordIndex.ToString, e.Message)
				Case kCura.Windows.Process.EventType.Progress
					Me.ProcessObserver.RaiseProgressEvent(e.TotalRecords, e.CurrentRecordIndex, _warningCount, _errorCount, _startTime, New DateTime)
					Me.ProcessObserver.RaiseStatusEvent(e.CurrentRecordIndex.ToString, e.Message)
				Case kCura.Windows.Process.EventType.Status
					Me.ProcessObserver.RaiseStatusEvent(e.CurrentRecordIndex.ToString, e.Message)
				Case kCura.Windows.Process.EventType.Warning
					_warningCount += 1
					Me.ProcessObserver.RaiseWarningEvent(e.CurrentRecordIndex.ToString, e.Message)
			End Select
			System.Threading.Monitor.Exit(Me.ProcessObserver)
			'Me.ProcessObserver.RaiseProgressEvent(e.TotalLines, e.CurrentRecordIndex, 0, 0, _startTime, System.DateTime.Now)
    End Sub

    Private Sub _loadFileImporter_FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception) Handles _loadFileImporter.FatalErrorEvent
      Me.ProcessObserver.RaiseFatalExceptionEvent(ex)
    End Sub

		Private Sub _loadFileImporter_UploadModeChangeEvent(ByVal mode As String) Handles _loadFileImporter.UploadModeChangeEvent
			Me.ProcessObserver.RaiseStatusBarEvent("Upload mode: " & mode)
		End Sub

		Private Sub _loadFileImporter_FilePrepEvent(ByVal e As LoadFileImporter.FilePrepEventArgs) Handles _loadFileImporter.FilePrepEvent
			System.Threading.Monitor.Enter(Me.ProcessObserver)
			Dim totaldisplay As String
			Dim processeddisplay As String
			If e.TotalBytes >= 104857600 Then
				totaldisplay = (e.TotalBytes / 1048576).ToString("N4") & "MB"
				processeddisplay = (e.BytesRead / 1048576).ToString("N4") & "MB"
			ElseIf e.TotalBytes < 104857600 AndAlso e.TotalBytes >= 102400 Then
				totaldisplay = (e.TotalBytes / 1024).ToString("N4") & "KB"
				processeddisplay = (e.BytesRead / 1024).ToString("N4") & "KB"
			Else
				totaldisplay = e.TotalBytes.ToString & "B"
				processeddisplay = e.BytesRead.ToString & "B"
			End If
			Select Case e.Type
				Case LoadFileImporter.FilePrepEventArgs.FilePrepEventType.CloseFile
					Me.ProcessObserver.RaiseProgressEvent(e.TotalBytes, e.TotalBytes, 0, 0, e.StartTime, System.DateTime.Now, totaldisplay, processeddisplay)
					'Me.ProcessObserver.RaiseProcessCompleteEvent()
				Case LoadFileImporter.FilePrepEventArgs.FilePrepEventType.OpenFile
					Me.ProcessObserver.RaiseProgressEvent(e.TotalBytes, e.BytesRead, 0, 0, e.StartTime, System.DateTime.Now, totaldisplay, processeddisplay)
					Me.ProcessObserver.RaiseStatusEvent("", "Preparing file for import")
				Case LoadFileImporter.FilePrepEventArgs.FilePrepEventType.ReadEvent
					Me.ProcessObserver.RaiseProgressEvent(e.TotalBytes, e.BytesRead, 0, 0, e.StartTime, System.DateTime.Now, totaldisplay, processeddisplay)
					Me.ProcessObserver.RaiseStatusEvent("", "Preparing file for import")
			End Select
			System.Threading.Monitor.Exit(Me.ProcessObserver)
		End Sub
	End Class

End Namespace