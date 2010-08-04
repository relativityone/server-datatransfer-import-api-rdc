Namespace kCura.EDDS.WinForm
	Public Class CommandLineProcessRunner
		Private WithEvents _observer As kCura.Windows.Process.ProcessObserver
		Private WithEvents _controller As kCura.Windows.Process.Controller
		Private _lastUpdated As Long = 0
		Private _hasReceivedFatalError As Boolean = False
		Private _hasReceivedLineError As Boolean = False
		Private _hasReceivedLineWarning As Boolean = False
		Private _exportErrorReportLocation As String = ""
		Private _exportErrorFileLocation As String = ""

		Public Enum ParsableLineType
			Status
			Warning
			LineError
			FatalError
		End Enum

		Public Sub New(ByVal observer As kCura.Windows.Process.ProcessObserver, ByVal controller As kCura.Windows.Process.Controller, ByVal exportErrorFileLocation As String, ByVal exportErrorReportLocation As String)
			_observer = observer
			_controller = controller
			If Not exportErrorFileLocation Is Nothing Then _exportErrorFileLocation = exportErrorFileLocation
			If Not exportErrorReportLocation Is Nothing Then _exportErrorReportLocation = exportErrorReportLocation
		End Sub

		Private Sub _observer_OnProcessEvent(ByVal evt As kCura.Windows.Process.ProcessEvent) Handles _observer.OnProcessEvent
			Select Case evt.Type
				Case kCura.Windows.Process.ProcessEventTypeEnum.Status
					WriteLine(evt.Message + " " + evt.RecordInfo, ParsableLineType.Status)
				Case kCura.Windows.Process.ProcessEventTypeEnum.Error
					WriteLine(evt.Message, ParsableLineType.LineError)
					_hasReceivedLineError = True
				Case kCura.Windows.Process.ProcessEventTypeEnum.Warning
					WriteLine(evt.Message, ParsableLineType.Warning)
					_hasReceivedLineWarning = True
			End Select
		End Sub

		Private Sub _observer_OnProcessProgressEvent(ByVal evt As kCura.Windows.Process.ProcessProgressEvent) Handles _observer.OnProcessProgressEvent
			Dim now As Long = System.DateTime.Now.Ticks
			If now - _lastUpdated > 10000000 Then
				_lastUpdated = now
				WriteLine(vbTab & evt.TotalRecordsProcessedDisplay + " of " + evt.TotalRecordsDisplay + " processed", ParsableLineType.Status)
			End If
		End Sub

		Private Sub _observer_OnProcessComplete(ByVal closeForm As Boolean, ByVal exportFilePath As String, ByVal exportLog As Boolean) Handles _observer.OnProcessComplete
			If _hasReceivedFatalError Then
				WriteLine("Fatal Exception Encountered", ParsableLineType.Status)
			ElseIf Not _hasReceivedLineError AndAlso Not _hasReceivedLineWarning Then
				WriteLine("All records have been successfully processed", ParsableLineType.Status)
			Else
				Dim x As String = ""
				If _hasReceivedLineWarning Then
					x &= "Some records were processed with warnings"
				End If
				If _hasReceivedLineError Then
					x &= "Some records were not processed due to errors"
				End If
				WriteLine(x, ParsableLineType.Status)
			End If
			If _hasReceivedLineError Then
				If _exportErrorFileLocation <> "" Then _controller.ExportErrorFile(_exportErrorFileLocation)
				If _exportErrorReportLocation <> "" Then _controller.ExportErrorReport(_exportErrorReportLocation)
			End If
			'If exportFilePath <> "" Then
			'	WriteLine("Errors have occurred. Export error files? (y/n)")
			'	Dim resp As String = ChrW(Console.Read)
			'	If resp.ToLower = "y" Then
			'		Dim folderPath As String = System.IO.Directory.GetCurrentDirectory
			'		If Not folderPath = "" Then
			'			folderPath = folderPath.TrimEnd("\"c) & "\"
			'			_controller.ExportServerErrors(folderPath)
			'		End If
			'	End If
			'Else
			'End If
		End Sub

		Private Sub _observer_OnProcessFatalException(ByVal ex As System.Exception) Handles _observer.OnProcessFatalException
			WriteLine("Fatal Exception Encountered", ParsableLineType.FatalError)
			WriteLine(ex.ToString, ParsableLineType.FatalError)
			_hasReceivedFatalError = True
		End Sub

		Private Sub _observer_ErrorReportEvent(ByVal row As System.Collections.IDictionary) Handles _observer.ErrorReportEvent
			'Forms only
		End Sub

		Private Sub _processObserver_StatusBarEvent(ByVal message As String, ByVal popupText As String) Handles _observer.StatusBarEvent
			WriteLine(message, ParsableLineType.Status)
		End Sub

		Private Sub WriteLine(ByVal line As String, ByVal lineType As ParsableLineType)
			Dim stringBuilder As New System.Text.StringBuilder
			Dim lineTypeString As String = Nothing
			If lineType = ParsableLineType.Status Then
				lineTypeString = "[Status]"
			ElseIf lineType = ParsableLineType.Warning Then
				lineTypeString = "[Warning]"
			ElseIf lineType = ParsableLineType.LineError Then
				lineTypeString = "[Error:Line]"
			ElseIf lineType = ParsableLineType.FatalError Then
				lineTypeString = "[Error:Fatal]"
			End If
			stringBuilder.Append("""").Append("[").Append(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff").Replace("Z", "")).Append("]").Append(""",")
			stringBuilder.Append("""").Append(lineTypeString).Append(""",").ToString()
			stringBuilder.Append("""").Append(line.Replace("""", """""").Replace(vbNewLine, ChrW(10))).Append("""")
			Console.WriteLine(stringBuilder.ToString)
		End Sub

	End Class
End Namespace