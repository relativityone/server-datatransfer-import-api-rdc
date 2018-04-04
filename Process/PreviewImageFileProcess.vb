Namespace kCura.WinEDDS

	Public Class PreviewImageFileProcess
		Inherits kCura.Windows.Process.ProcessBase

		Public LoadFile As ImageLoadFile
		Public ReturnValueCollection As Hashtable
		Public ReturnValueKey As Guid
		Protected WithEvents _imageFilePreviewer As ImageFilePreviewer
		Private _startTime As System.DateTime
		Private _errorCount As Int32
		Private _warningCount As Int32

		Public Property TimeZoneOffset As Int32

		Public Property ErrorsOnly As Boolean

		Public Property Thrower As ValueThrower

		Protected Overrides Sub Execute()
			_startTime = DateTime.Now
			_warningCount = 0
			_errorCount = 0
			_imageFilePreviewer = New kCura.WinEDDS.ImageFilePreviewer(ProcessController, True)

			_imageFilePreviewer.ReadFile(LoadFile.FileName)
			Me.ProcessObserver.RaiseProcessCompleteEvent()
		End Sub

		Private Sub _imageFileImporter_StatusMessage(ByVal e As kCura.Windows.Process.StatusEventArgs) Handles _imageFilePreviewer.StatusMessage
			Select Case e.EventType
				Case kCura.Windows.Process.EventType.Error
					_errorCount += 1
					Me.ProcessObserver.RaiseErrorEvent(e.CurrentRecordIndex.ToString, e.Message)
				Case kCura.Windows.Process.EventType.Progress
					Me.ProcessObserver.RaiseStatusEvent(e.CurrentRecordIndex.ToString, e.Message)
					Me.ProcessObserver.RaiseProgressEvent(e.TotalRecords, e.CurrentRecordIndex, _warningCount, _errorCount, _startTime, DateTime.Now, Me.ProcessID)
				Case kCura.Windows.Process.EventType.Status
					Me.ProcessObserver.RaiseStatusEvent(e.CurrentRecordIndex.ToString, e.Message)
				Case kCura.Windows.Process.EventType.Warning
					_warningCount += 1
					Me.ProcessObserver.RaiseWarningEvent(e.CurrentRecordIndex.ToString, e.Message)
			End Select
		End Sub

	End Class

End Namespace