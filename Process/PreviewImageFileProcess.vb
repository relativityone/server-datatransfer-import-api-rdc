Imports kCura.Windows.Process

Namespace kCura.WinEDDS

	Public Class PreviewImageFileProcess
		Inherits ProcessBase

		Public LoadFile As ImageLoadFile
		Private WithEvents _imageFilePreviewer As ImageFilePreviewer
		Private _startTime As DateTime
		Private _errorCount As Int32
		Private _warningCount As Int32

		Public Property TimeZoneOffset As Int32

		Protected Overrides Sub Execute()
			_startTime = DateTime.Now
			_warningCount = 0
			_errorCount = 0
			_imageFilePreviewer = New ImageFilePreviewer(ProcessController, True, New ImageValidator.ImageValidator)

			_imageFilePreviewer.ReadFile(LoadFile.FileName)
			Me.ProcessObserver.RaiseProcessCompleteEvent()
		End Sub

		Private Sub _imageFileImporter_StatusMessage(ByVal e As StatusEventArgs) Handles _imageFilePreviewer.StatusMessage
			Select Case e.EventType
				Case EventType.Error
					_errorCount += 1
					Me.ProcessObserver.RaiseErrorEvent(e.CurrentRecordIndex.ToString, e.Message)
				Case EventType.Progress
					Me.ProcessObserver.RaiseStatusEvent(e.CurrentRecordIndex.ToString, e.Message)
					Me.ProcessObserver.RaiseProgressEvent(e.TotalRecords, e.CurrentRecordIndex, _warningCount, _errorCount, _startTime, DateTime.Now, 0, 0, Me.ProcessID)
				Case EventType.Status
					Me.ProcessObserver.RaiseStatusEvent(e.CurrentRecordIndex.ToString, e.Message)
				Case EventType.Warning
					_warningCount += 1
					Me.ProcessObserver.RaiseWarningEvent(e.CurrentRecordIndex.ToString, e.Message)
			End Select
		End Sub

	End Class

End Namespace