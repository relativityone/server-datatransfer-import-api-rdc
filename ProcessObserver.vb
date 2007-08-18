Namespace kCura.Windows.Process
  Public Class ProcessObserver

#Region "Members"

		Private _tempFileName As String = ""
		Private _errorsFileName As String
		Private _inSafeMode As Boolean
		Private _outputWriter As System.IO.StreamWriter
		Private _errorsWriter As System.IO.StreamWriter
		Private _inputArgs As Object

    Public Property InSafeMode() As Boolean
      Get
        Return _inSafeMode
      End Get
      Set(ByVal value As Boolean)
        _inSafeMode = value
      End Set
    End Property
		Public Property InputArgs() As Object
			Get
				Return _inputArgs
			End Get
			Set(ByVal value As Object)
				_inputArgs = value
			End Set
		End Property

#End Region

#Region "Events"

    Public Event OnProcessFatalException(ByVal ex As Exception)
    Public Event OnProcessEvent(ByVal evt As ProcessEvent)
    Public Event OnProcessProgressEvent(ByVal evt As ProcessProgressEvent)
		Public Event OnProcessComplete(ByVal closeForm As Boolean, ByVal exportFilePath As String)
		Public Event StatusBarEvent(ByVal message As String)
		Public Event ShowReportEvent(ByVal datasource As System.Data.DataTable, ByVal maxlengthExceeded As Boolean)

#End Region

#Region "Event Throwers"

    Public Sub RaiseStatusEvent(ByVal recordInfo As String, ByVal message As String)
      Dim evt As New ProcessEvent(ProcessEventTypeEnum.Status, recordInfo, message)
      RaiseEvent OnProcessEvent(evt)
      WriteToFile(evt)
    End Sub

    Public Sub RaiseWarningEvent(ByVal recordInfo As String, ByVal message As String)
      Dim evt As New ProcessEvent(ProcessEventTypeEnum.Warning, recordInfo, message)
      RaiseEvent OnProcessEvent(evt)
      WriteToFile(evt)
    End Sub

    Public Sub RaiseErrorEvent(ByVal recordInfo As String, ByVal message As String)
      Dim evt As New ProcessEvent(ProcessEventTypeEnum.Error, recordInfo, message)
      RaiseEvent OnProcessEvent(evt)
			WriteToFile(evt)
			WriteError(recordInfo, message)
    End Sub

		Public Sub RaiseProgressEvent(ByVal totalRecords As Int64, ByVal totalRecordsProcessed As Int64, ByVal totalRecordsProccessedWithWarnings As Int64, ByVal totalRecordsProcessedWithErrors As Int64, ByVal startTime As DateTime, ByVal endTime As DateTime, Optional ByVal totalRecordsDisplay As String = Nothing, Optional ByVal totalRecordsProcessedDisplay As String = Nothing)
			RaiseEvent OnProcessProgressEvent(New ProcessProgressEvent(totalRecords, totalRecordsProcessed, totalRecordsProccessedWithWarnings, totalRecordsProcessedWithErrors, startTime, endTime, totalRecordsDisplay, totalRecordsProcessedDisplay))
		End Sub

		Public Sub RaiseProcessCompleteEvent(Optional ByVal closeForm As Boolean = False, Optional ByVal exportFilePath As String = "")
			RaiseEvent OnProcessComplete(closeForm, exportFilePath)
			If Not _errorsWriter Is Nothing Then _errorsWriter.Close()
			If Not _outputWriter Is Nothing Then _outputWriter.Close()
			If Not closeForm AndAlso _errorsFileName <> String.Empty Then
				Dim o As Object() = Me.BuildErrorReportDatasource()
				RaiseEvent ShowReportEvent(DirectCast(o(0), System.Data.DataTable), CType(o(1), Boolean))
			End If
		End Sub

		Public Sub RaiseFatalExceptionEvent(ByVal ex As Exception)
			RaiseEvent OnProcessFatalException(ex)
			Dim evt As New ProcessEvent(ProcessEventTypeEnum.Error, "", ex.ToString)
			WriteToFile(evt)
		End Sub

		Public Sub RaiseStatusBarEvent(ByVal message As String)
			RaiseEvent StatusBarEvent(message)
		End Sub

#End Region

		Private Function BuildErrorReportDatasource() As Object()
			Dim reader As New ErrorFileReader
			Return DirectCast(reader.ReadFile(_errorsFileName), Object())
		End Function

#Region "File IO"

		Public Sub SaveOutputFile(ByVal fileName As String)
			System.IO.File.Move(_tempFileName, fileName)
		End Sub

		Public Sub ExportErrorReport(ByVal filename As String)
			If Not _errorsWriter Is Nothing Then
				_errorsWriter.Flush()
				_errorsWriter.Close()
			End If
			System.IO.File.Copy(_errorsFileName, filename)
		End Sub

		Private Sub WriteToFile(ByVal evt As ProcessEvent)
			If Not Config.LogAllEvents Then Exit Sub
			If Not Me.InSafeMode Then
				Dim serializer As New System.xml.Serialization.XmlSerializer(evt.GetType)
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