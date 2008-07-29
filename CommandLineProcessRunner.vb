Namespace kCura.EDDS.WinForm
	Public Class CommandLineProcessRunner
		Private WithEvents _observer As kCura.Windows.Process.ProcessObserver
		Private WithEvents _controller As kCura.Windows.Process.Controller
		Private _lastUpdated As Long = 0
		Private _hasReceivedFatalError As Boolean = False
		Public Sub New(ByVal observer As kCura.Windows.Process.ProcessObserver, ByVal controller As kCura.Windows.Process.Controller)
			_observer = observer
			_controller = controller
		End Sub

		Private Sub _observer_OnProcessEvent(ByVal evt As kCura.Windows.Process.ProcessEvent) Handles _observer.OnProcessEvent
			Select Case evt.Type
				Case kCura.Windows.Process.ProcessEventTypeEnum.Status
					WriteLine(evt.Message + " " + evt.RecordInfo)
				Case kCura.Windows.Process.ProcessEventTypeEnum.Error
					WriteLine("[Line Error] " & evt.Message)
				Case kCura.Windows.Process.ProcessEventTypeEnum.Warning
					WriteLine("[Line Warning] " & evt.Message)
			End Select
		End Sub

		Private Sub _observer_OnProcessProgressEvent(ByVal evt As kCura.Windows.Process.ProcessProgressEvent) Handles _observer.OnProcessProgressEvent
			Dim now As Long = System.DateTime.Now.Ticks
			If now - _lastUpdated > 10000000 Then
				_lastUpdated = now
				WriteLine(vbTab & evt.TotalRecordsProcessedDisplay + " of " + evt.TotalRecordsDisplay + " processed")
			End If
		End Sub

		Private Sub _observer_OnProcessComplete(ByVal closeForm As Boolean, ByVal exportFilePath As String, ByVal exportLog As Boolean) Handles _observer.OnProcessComplete
			If _hasReceivedFatalError Then
				WriteLine("Fatal Exception Encountered")
			Else
				WriteLine("All records have been processed")
			End If
			If exportFilePath <> "" Then
				WriteLine("Errors have occurred. Export error files? (y/n)")
				Dim resp As String = ChrW(Console.Read)
				If resp.ToLower = "y" Then
					Dim folderPath As String = System.IO.Directory.GetCurrentDirectory
					If Not folderPath = "" Then
						folderPath = folderPath.TrimEnd("\"c) & "\"
						_controller.ExportServerErrors(folderPath)
					End If
				End If
			Else
			End If
		End Sub

		Private Sub _observer_OnProcessFatalException(ByVal ex As System.Exception) Handles _observer.OnProcessFatalException
			WriteLine("Fatal Exception Encountered")
			WriteLine(ex.ToString)
			_hasReceivedFatalError = True
		End Sub

		Private Sub _observer_ErrorReportEvent(ByVal row As System.Collections.IDictionary) Handles _observer.ErrorReportEvent
			'Forms only
		End Sub

		Private Sub _processObserver_StatusBarEvent(ByVal message As String, ByVal popupText As String) Handles _observer.StatusBarEvent
			WriteLine(message)
		End Sub

		Private Sub WriteLine(ByVal line As String)
			WriteLine("[" & System.DateTime.Now.ToString("u").Replace("Z", "") & "]" & vbTab)
			WriteLine(line)
		End Sub

	End Class
End Namespace


