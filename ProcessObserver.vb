Namespace kCura.Windows.Process
  Public Class ProcessObserver

#Region "Members"

    Private _tempFileName As String
    Private _inSafeMode As Boolean
    Private _sw As System.IO.StreamWriter

    Public Property InSafeMode() As Boolean
      Get
        Return _inSafeMode
      End Get
      Set(ByVal value As Boolean)
        _inSafeMode = value
      End Set
    End Property

#End Region

#Region "Events"

    Public Event OnProcessFatalException(ByVal ex As Exception)
    Public Event OnProcessEvent(ByVal evt As ProcessEvent)
    Public Event OnProcessProgressEvent(ByVal evt As ProcessProgressEvent)
		Public Event OnProcessComplete(ByVal closeForm As Boolean)
    Public Event StatusBarEvent(ByVal message As String)

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
    End Sub

		Public Sub RaiseProgressEvent(ByVal totalRecords As Int32, ByVal totalRecordsProcessed As Int32, ByVal totalRecordsProccessedWithWarnings As Int32, ByVal totalRecordsProcessedWithErrors As Int32, ByVal startTime As DateTime, ByVal endTime As DateTime, Optional ByVal totalRecordsDisplay As String = Nothing, Optional ByVal totalRecordsProcessedDisplay As String = Nothing)
			RaiseEvent OnProcessProgressEvent(New ProcessProgressEvent(totalRecords, totalRecordsProcessed, totalRecordsProccessedWithWarnings, totalRecordsProcessedWithErrors, startTime, endTime, totalRecordsDisplay, totalRecordsProcessedDisplay))
		End Sub

		Public Sub RaiseProcessCompleteEvent(Optional ByVal closeForm As Boolean = False)
			RaiseEvent OnProcessComplete(closeForm)
			If Not _sw Is Nothing Then
				_sw.Close()
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

#Region "File IO"

    Public Sub SaveOutputFile(ByVal fileName As String)
      System.IO.File.Move(_tempFileName, fileName)
    End Sub

    Private Sub WriteToFile(ByVal evt As ProcessEvent)

      If Not Me.InSafeMode Then
        Dim serializer As New System.xml.Serialization.XmlSerializer(evt.GetType)
				If _sw Is Nothing OrElse _sw.BaseStream Is Nothing Then
					Me.OpenFile()
				End If
				serializer.Serialize(_sw.BaseStream, evt)
			End If
    End Sub

    Private Sub OpenFile()
      _tempFileName = System.IO.Path.GetTempFileName()
      _sw = New System.IO.StreamWriter(_tempFileName, False)
      _sw.WriteLine("<ProcessEvents>")
    End Sub

    Private Sub CloseFile()
      _sw.WriteLine("</ProcessEvents>")
      If Not _sw Is Nothing Then
        _sw.Close()
      End If
    End Sub

#End Region

  End Class
End Namespace