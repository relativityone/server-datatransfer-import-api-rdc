Namespace kCura.Windows.Process.Generic
	Public Class ProcessObserver(Of T)

#Region "Members"

		Private _tempFileName As String = ""
		Private _errorsFileName As String
		Private _outputWriter As System.IO.StreamWriter
		Private _errorsWriter As System.IO.StreamWriter
		Private _getMessage As Func(Of T, String)
		Private _getRecordInfo As Func(Of T, String)

		Public Property InSafeMode() As Boolean
		Public Property InputArgs() As Object

#End Region

#Region "Events"

		Public Event OnProcessFatalException(ByVal ex As Exception)
		Public Event OnProcessEvent(ByVal evt As ProcessEvent(Of T))
		Public Event OnProcessProgressEvent(ByVal evt As ProcessProgressEvent)
		Public Event OnProcessComplete(ByVal closeForm As Boolean, ByVal exportFilePath As String, ByVal exportLogs As Boolean)
		Public Event StatusBarEvent(ByVal message As String, ByVal popupText As String)
		Public Event ShowReportEvent(ByVal datasource As System.Data.DataTable, ByVal maxlengthExceeded As Boolean)
		Public Event ErrorReportEvent(ByVal row As System.Collections.IDictionary)
		Public Event ShutdownEvent()
#End Region

		Public Sub New(ByVal getMessage As Func(Of T, String), ByVal getRecordInfo As Func(Of T, String))
			_getMessage = getMessage
			_getRecordInfo = getRecordInfo
		End Sub

#Region "Event Throwers"

		Public Sub Shutdown()
			RaiseEvent ShutdownEvent()
		End Sub

		Public Sub RaiseStatusEvent(ByVal result As T)
			Dim evt As New ProcessEvent(Of T)(ProcessEventTypeEnum.Status, result, _getMessage, _getRecordInfo)
			RaiseEvent OnProcessEvent(evt)
			WriteToFile(evt)
		End Sub

		Public Sub RaiseWarningEvent(ByVal result As T)
			Dim evt As New ProcessEvent(Of T)(ProcessEventTypeEnum.Warning, result, _getMessage, _getRecordInfo)
			RaiseEvent OnProcessEvent(evt)
			WriteToFile(evt)
		End Sub

		Public Sub RaiseErrorEvent(ByVal result As T)
			Dim evt As New ProcessEvent(Of T)(ProcessEventTypeEnum.Error, result, _getMessage, _getRecordInfo)
			RaiseEvent OnProcessEvent(evt)
			WriteToFile(evt)
			WriteError(_getRecordInfo(result), _getMessage(result))
		End Sub

		Public Sub RaiseProgressEvent(ByVal totalRecords As Int64, ByVal totalRecordsProcessed As Int64, ByVal totalRecordsProccessedWithWarnings As Int64, ByVal totalRecordsProcessedWithErrors As Int64, ByVal startTime As DateTime, ByVal endTime As DateTime, Optional ByVal totalRecordsDisplay As String = Nothing, Optional ByVal totalRecordsProcessedDisplay As String = Nothing, Optional ByVal args As IDictionary = Nothing)
			RaiseEvent OnProcessProgressEvent(New ProcessProgressEvent(totalRecords, totalRecordsProcessed, totalRecordsProccessedWithWarnings, totalRecordsProcessedWithErrors, startTime, endTime, totalRecordsDisplay, totalRecordsProcessedDisplay, args))
		End Sub

		Public Sub RaiseProcessCompleteEvent(Optional ByVal closeForm As Boolean = False, Optional ByVal exportFilePath As String = "", Optional ByVal exportLogs As Boolean = False)
			RaiseEvent OnProcessComplete(closeForm, exportFilePath, exportLogs)
			If Not _errorsWriter Is Nothing Then _errorsWriter.Close()
			If Not _outputWriter Is Nothing Then _outputWriter.Close()
			If Not closeForm AndAlso _errorsFileName <> String.Empty Then
				Dim o As Object() = Me.BuildErrorReportDatasource()
				RaiseEvent ShowReportEvent(DirectCast(o(0), System.Data.DataTable), CType(o(1), Boolean))
			End If
		End Sub

		Public Sub RaiseFatalExceptionEvent(ByVal ex As Exception)
			RaiseEvent OnProcessFatalException(ex)
			WriteError("FATAL ERROR", ex.ToString)
			Dim pair As New DefaultResult With {.Message = ex.ToString, .RecordInfo = String.Empty}
			Dim evt As New ProcessEvent(Of DefaultResult)(ProcessEventTypeEnum.Error, pair, Function(res As DefaultResult) res.Message, Function(res As DefaultResult) res.RecordInfo)
			WriteToFile(evt)
		End Sub

		Public Sub RaiseStatusBarEvent(ByVal message As String, ByVal popupText As String)
			RaiseEvent StatusBarEvent(message, popupText)
		End Sub

		Public Sub RaiseReportErrorEvent(ByVal row As System.Collections.IDictionary)
			RaiseEvent ErrorReportEvent(row)
		End Sub

#End Region

		Private Function BuildErrorReportDatasource() As Object()
			Dim reader As New ErrorFileReader(False)
			Return DirectCast(reader.ReadFile(_errorsFileName), Object())
		End Function

#Region "File IO"

		Public Sub SaveOutputFile(ByVal fileName As String)
			System.IO.File.Move(_tempFileName, fileName)
		End Sub

		Public Sub ExportErrorReport(ByVal filename As String)
			If Not _errorsWriter Is Nothing Then
				Try
					_errorsWriter.Flush()
				Catch
				End Try
				Try
					_errorsWriter.Close()
				Catch
				End Try
			End If
			If _errorsFileName Is Nothing OrElse _errorsFileName = "" OrElse Not System.IO.File.Exists(_errorsFileName) Then
				System.IO.File.Create(filename)
			Else
				System.IO.File.Copy(_errorsFileName, filename)
			End If
		End Sub

		Private Sub WriteToFile(ByVal evt As ProcessEvent(Of T))
			If Not Config.LogAllEvents Then Exit Sub
			If Not Me.InSafeMode Then
				Dim serializer As New System.Xml.Serialization.XmlSerializer(evt.GetType)
				If _outputWriter Is Nothing OrElse _outputWriter.BaseStream Is Nothing Then
					Me.OpenFile()
				End If
				serializer.Serialize(_outputWriter.BaseStream, evt)
			End If
		End Sub

		Private Sub WriteToFile(ByVal evt As ProcessEvent(Of DefaultResult))
			If Not Config.LogAllEvents Then Exit Sub
			If Not Me.InSafeMode Then
				Dim serializer As New System.Xml.Serialization.XmlSerializer(evt.GetType)
				If _outputWriter Is Nothing OrElse _outputWriter.BaseStream Is Nothing Then
					Me.OpenFile()
				End If
				serializer.Serialize(_outputWriter.BaseStream, evt)
			End If
		End Sub

		Private Sub WriteError(ByVal key As String, ByVal description As String)
			If _errorsFileName = "" Then _errorsFileName = System.IO.Path.GetTempFileName

			If _errorsWriter Is Nothing OrElse _errorsWriter.BaseStream Is Nothing Then
				_errorsWriter = New System.IO.StreamWriter(_errorsFileName, True)
			End If
			key = key.Replace("""", """""")
			description = description.Replace("""", """""")
			_errorsWriter.WriteLine(String.Format("""{1}{0}Error{0}{2}{0}{3}""", """,""", key, description, System.DateTime.Now.ToString))
		End Sub

		Private Sub OpenFile()
			_tempFileName = System.IO.Path.GetTempFileName()
			_outputWriter = New System.IO.StreamWriter(_tempFileName, False)
			_outputWriter.WriteLine("<ProcessEvents>")
		End Sub

		Private Sub CloseFile()
			_outputWriter.WriteLine("</ProcessEvents>")
			If Not _outputWriter Is Nothing Then
				_outputWriter.Close()
			End If
		End Sub

#End Region

	End Class
End Namespace