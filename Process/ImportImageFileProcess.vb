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
			_imageFileImporter = New kCura.WinEDDS.BulkImageFileImporter(ImageLoadFile.DestinationFolderID, ImageLoadFile, ProcessController, Me.ProcessID)
			_imageFileImporter.ReadFile(ImageLoadFile.FileName)
			If Not _hasRunProcessComplete Then
				Dim exportFilePath As String = ""
				If _imageFileImporter.HasErrors Then exportFilePath = System.Guid.NewGuid.ToString
				Me.ProcessObserver.RaiseProcessCompleteEvent(False, exportFilePath, True)
			End If

		End Sub

		Private Sub _imageFileImporter_StatusMessage(ByVal e As kCura.Windows.Process.StatusEventArgs) Handles _imageFileImporter.StatusMessage
			System.Threading.Monitor.Enter(Me.ProcessObserver)
			Select Case e.EventType
				Case kCura.Windows.Process.EventType.Error
					If e.CountsTowardsTotal Then _errorCount += 1
					Me.ProcessObserver.RaiseErrorEvent(e.CurrentRecordIndex.ToString, e.Message)
				Case kCura.Windows.Process.EventType.Progress
					Me.ProcessObserver.RaiseStatusEvent(e.CurrentRecordIndex.ToString, e.Message)
					Me.ProcessObserver.RaiseProgressEvent(e.TotalRecords, e.CurrentRecordIndex, _warningCount, _errorCount, _startTime, New System.DateTime)
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
			Me.ProcessObserver.RaiseProcessCompleteEvent(False, "", True)
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



	End Class
End Namespace