Namespace kCura.WinEDDS
	Public Class ExportProductionProcess
    Inherits kCura.Windows.Process.ProcessBase

		Public ExportFile As ExportFile
		Private WithEvents _productionExporter As kCura.WinEDDS.ProductionExporter
		Private _startTime As System.DateTime
		Private _errorCount As Int32
		Private _warningCount As Int32

		Protected Overrides Sub Execute()
			_startTime = DateTime.Now
			_warningCount = 0
			_errorCount = 0
			_productionExporter = New ProductionExporter(ExportFile.Credential, ExportFile.ArtifactID, ExportFile.FolderPath, ExportFile.CaseInfo, ExportFile.Overwrite, Me.ProcessController)
			_productionExporter.CreateVolumes()
			Me.ProcessObserver.RaiseProcessCompleteEvent()
		End Sub

		Private Sub _productionExporter_StatusMessage(ByVal e As ExportEventArgs) Handles _productionExporter.StatusMessage
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
      Me.ProcessObserver.RaiseProgressEvent(e.TotalDocuments, e.DocumentsExported, _warningCount, _errorCount, _startTime, System.DateTime.Now)
		End Sub

		Private Sub _productionExporter_FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception) Handles _productionExporter.FatalErrorEvent
			Me.ProcessObserver.RaiseFatalExceptionEvent(ex)
		End Sub
	End Class
End Namespace