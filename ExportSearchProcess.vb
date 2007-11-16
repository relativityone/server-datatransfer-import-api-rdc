Namespace kCura.WinEDDS
	Public Class ExportSearchProcess
    Inherits kCura.Windows.Process.ProcessBase

		Public ExportFile As ExportFile
		Private WithEvents _searchExporter As kCura.WinEDDS.Exporter
		Private _startTime As System.DateTime
		Private _errorCount As Int32
		Private _warningCount As Int32

		Protected Overrides Sub Execute()
			_startTime = DateTime.Now
			_warningCount = 0
			_errorCount = 0
			_searchExporter = New Exporter(Me.ExportFile, Me.ProcessController)


			If Not _searchExporter.ExportSearch() Then
				Me.ProcessObserver.RaiseProcessCompleteEvent(False, _searchExporter.ErrorLogFileName)
			Else
				Me.ProcessObserver.RaiseStatusEvent("", "Export completed")
			End If
		End Sub

		Private Sub _searchExporter_FileTransferModeChangeEvent(ByVal mode As String) Handles _searchExporter.FileTransferModeChangeEvent
			Me.ProcessObserver.RaiseStatusBarEvent("File Transfer Mode: " & mode)
		End Sub

		Private Sub _productionExporter_StatusMessage(ByVal e As ExportEventArgs) Handles _searchExporter.StatusMessage
			Select Case e.EventType
				Case kCura.Windows.Process.EventType.Error
					_errorCount += 1
					Me.ProcessObserver.RaiseErrorEvent(e.DocumentsExported.ToString, e.Message)
				Case kCura.Windows.Process.EventType.Progress
					Me.ProcessObserver.RaiseStatusEvent(e.DocumentsExported.ToString, e.Message)
				Case kCura.Windows.Process.EventType.Status
					Me.ProcessObserver.RaiseStatusEvent(e.DocumentsExported.ToString, e.Message)
				Case kCura.Windows.Process.EventType.Warning
					_warningCount += 1
					Me.ProcessObserver.RaiseWarningEvent(e.DocumentsExported.ToString, e.Message)
			End Select
			Me.ProcessObserver.RaiseProgressEvent(e.TotalDocuments, e.DocumentsExported, _warningCount, _errorCount, _startTime, New DateTime)
		End Sub

		Private Sub _productionExporter_FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception) Handles _searchExporter.FatalErrorEvent
			Me.ProcessObserver.RaiseFatalExceptionEvent(ex)
		End Sub
	End Class
End Namespace