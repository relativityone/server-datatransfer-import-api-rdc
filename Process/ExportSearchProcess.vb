Namespace kCura.WinEDDS
	Public Class ExportSearchProcess
    Inherits kCura.Windows.Process.ProcessBase

		Public ExportFile As ExportFile
		Private WithEvents _searchExporter As kCura.WinEDDS.Exporter
		Private _startTime As System.DateTime
		Private _errorCount As Int32
		Private _warningCount As Int32
		Private _uploadModeText As String = Nothing

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
			If _uploadModeText Is Nothing Then
				Dim sb As New System.Text.StringBuilder
				sb.Append("FILE TRANSFER MODES:" & vbNewLine)
				sb.Append(" • Web - The document repository is accessed through the Relativity web service API.  This is the slower of the two methods, but is globally available")
				sb.Append(vbNewLine & vbNewLine)
				sb.Append(" • Direct is significantly faster than Web mode.  To use Direct mode, you must:")
				sb.Append(vbNewLine)
				sb.Append(vbTab & " - Have Windows rights to the document repository.")
				sb.Append(vbNewLine)
				sb.Append(vbTab & " - Be logged into the document repository’s network.")
				sb.Append(vbNewLine & vbNewLine & "If you meet the above criteria, Relativity will automatically load in Direct mode.  If you are loading in Web mode and think you should have Direct mode, contact your Relativity Database Administrator to establish the correct rights.")
				_uploadModeText = sb.ToString
			End If
			Me.ProcessObserver.RaiseStatusBarEvent("File Transfer Mode: " & mode, _uploadModeText)
		End Sub

		Private Sub _productionExporter_StatusMessage(ByVal e As ExportEventArgs) Handles _searchExporter.StatusMessage
			Select Case e.EventType
				Case kCura.Windows.Process.EventType.Error
					_errorCount += 1
					Me.ProcessObserver.RaiseErrorEvent(e.DocumentsExported.ToString, e.Message)
				Case kCura.Windows.Process.EventType.Progress
					Me.ProcessObserver.RaiseStatusEvent("", e.Message)
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