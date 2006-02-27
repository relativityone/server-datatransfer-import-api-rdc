Namespace kCura.WinEDDS
	Public Class ImportImageFileProcess
    Inherits kCura.Windows.Process.ProcessBase

		Public ImageLoadFile As ImageLoadFile
		Private WithEvents _imageFileImporter As kCura.WinEDDS.ImageFileImporter
		Private _startTime As DateTime
		Private _errorCount As Int32
		Private _warningCount As Int32

		Protected Overrides Sub Execute()
			_startTime = DateTime.Now
			_warningCount = 0
			_errorCount = 0
			_imageFileImporter = New kCura.WinEDDS.ImageFileImporter(ImageLoadFile.DestinationFolderID, ImageLoadFile, ProcessController)
			_imageFileImporter.ReadFile(ImageLoadFile.FileName)
			Me.ProcessObserver.RaiseProcessCompleteEvent()
		End Sub

    Private Sub _imageFileImporter_StatusMessage(ByVal e As kCura.Windows.Process.StatusEventArgs) Handles _imageFileImporter.StatusMessage
      Select Case e.EventType
        Case kCura.Windows.Process.EventType.Error
          _errorCount += 1
          Me.ProcessObserver.RaiseErrorEvent(e.CurrentRecordIndex.ToString, e.Message)
        Case kCura.Windows.Process.EventType.Progress
          Me.ProcessObserver.RaiseStatusEvent(e.CurrentRecordIndex.ToString, e.Message)
          Me.ProcessObserver.RaiseProgressEvent(e.TotalRecords, e.CurrentRecordIndex, _warningCount, _errorCount, _startTime, DateTime.Now)
        Case kCura.Windows.Process.EventType.Status
          Me.ProcessObserver.RaiseStatusEvent(e.CurrentRecordIndex.ToString, e.Message)
        Case kCura.Windows.Process.EventType.Warning
          _warningCount += 1
          Me.ProcessObserver.RaiseWarningEvent(e.CurrentRecordIndex.ToString, e.Message)
      End Select
    End Sub

    Private Sub _imageFileImporter_FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception) Handles _imageFileImporter.FatalErrorEvent
      Me.ProcessObserver.RaiseFatalExceptionEvent(ex)
    End Sub
  End Class
End Namespace