Imports Aspera.Transfer
Imports Relativity.DataExchange.Process

Namespace Relativity.Desktop.Client
	Public Class CommandLineProcessRunner
		Private WithEvents _context As ProcessContext
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

		Public Sub New(ByVal context As ProcessContext, ByVal exportErrorFileLocation As String, ByVal exportErrorReportLocation As String)
			If context Is Nothing Then
				Throw New ArgumentNullException("context")
			End If

			_context = context

			If Not exportErrorFileLocation Is Nothing Then
				_exportErrorFileLocation = exportErrorFileLocation
			End If

			If Not exportErrorReportLocation Is Nothing Then
				_exportErrorReportLocation = exportErrorReportLocation
			End If
		End Sub

		Private Sub _context_OnProcessEvent(ByVal sender As Object, ByVal e As ProcessEventArgs) Handles _context.ProcessEvent
			Select Case e.EventType
				Case ProcessEventType.Status
					WriteLine(e.Message + " " + e.RecordInfo, ParsableLineType.Status)
				Case ProcessEventType.Error
					WriteLine(e.Message, ParsableLineType.LineError)
					_hasReceivedLineError = True
				Case ProcessEventType.Warning
					WriteLine(e.Message, ParsableLineType.Warning)
					_hasReceivedLineWarning = True
			End Select
		End Sub

		Private Sub _context_OnProcessProgressEvent(ByVal sender As Object, ByVal e As ProgressEventArgs) Handles _context.Progress
			Dim now As Long = System.DateTime.Now.Ticks
			If now - _lastUpdated > 10000000 Then
				_lastUpdated = now
				WriteLine(vbTab & e.TotalProcessedRecordsDisplay + " of " + e.TotalRecordsDisplay + " processed", ParsableLineType.Status)
			End If
		End Sub

		Private Sub _context_OnProcessComplete(ByVal sender As Object, ByVal e As ProcessCompleteEventArgs) Handles _context.ProcessCompleted
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
				If _exportErrorFileLocation <> "" Then _context.PublishExportErrorFile(_exportErrorFileLocation)
				If _exportErrorReportLocation <> "" Then _context.PublishExportErrorReport(_exportErrorReportLocation)
			End If
			Try
				FaspManager.destroy()
			Catch ex As NullReferenceException
				' catch NullReferenceException which occurs when FaspManager is was not created by TAPI
				' this is a case when transfer mode is not "Aspera"
				' this is needed in CLI flow to enable the process to exit, becaue FaspManager thread is blocking the process from ending
			End Try
		End Sub

		Private Sub _context_OnProcessFatalException(ByVal sender As Object, ByVal e As FatalExceptionEventArgs) Handles _context.FatalException
			WriteLine("Fatal Exception Encountered", ParsableLineType.FatalError)
			WriteLine(e.ToString, ParsableLineType.FatalError)
			_hasReceivedFatalError = True
		End Sub

		Private Sub _context_ErrorReportEvent(ByVal sender As Object, ByVal e As ErrorReportEventArgs) Handles _context.ErrorReport
			'Forms only
		End Sub

		Private Sub _processObserver_StatusBarEvent(ByVal sender As Object, ByVal e As StatusBarEventArgs) Handles _context.StatusBarChanged
			WriteLine(e.Message, ParsableLineType.Status)
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