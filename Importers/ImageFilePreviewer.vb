Imports System.IO
Imports kCura.Utility
Imports kCura.Windows.Process
Imports kCura.WinEDDS.Api

Namespace kCura.WinEDDS
	Public Class ImageFilePreviewer
		Inherits DelimitedFileImporter
		
		Private _fileLineCount As Int32
		Private _continue As Boolean
		Private WithEvents _processController As Controller

		Private Enum Columns
			DocumentArtifactID = 0
			FileLocation = 2
			MultiPageIndicator = 3
		End Enum

		Private ReadOnly Property [Continue]() As Boolean
			Get
				Return Not MyBase.HasReachedEOF AndAlso _continue
			End Get
		End Property

		Public Event StatusMessage(ByVal args As StatusEventArgs)

		Public Sub New(ByVal controller As Controller, ByVal doRetryLogic As Boolean)
			MyBase.New(New Char() {","c}, doRetryLogic)
			
			_processController = controller
			_continue = True
		End Sub


		Public Overloads Overrides Function ReadFile(ByVal path As String) As Object
			Try
				_fileLineCount = kCura.Utility.File.Instance.CountLinesInFile(path)
				Reader = New StreamReader(path)
				RaiseStatusEvent(EventType.Progress, "Begin Image Upload")

				While [Continue]
					Try
						DoFileUpload()
					Catch ex As Exception
						Dim txt As String = ex.ToString.ToLower
						If txt.IndexOf("ix_") <> -1 AndAlso txt.IndexOf("duplicate") <> -1 Then
							txt = "Error creating document - identifier field isn't being properly filled.  Please choose a different 'key' field."
						Else
							txt = ex.Message
						End If
						RaiseStatusEvent(EventType.Error, txt)
					End Try
				End While
				Me.Reader.Close()
				RaiseStatusEvent(EventType.Progress, "End Image Upload")
			Catch ex As Exception
				RaiseStatusEvent(EventType.Error, ex.Message)
				RaiseFatalError(ex)
			End Try
			Return Nothing
		End Function

		Private Sub DoFileUpload()
			Dim valuearray As String() = Me.GetLine

			While Not valuearray Is Nothing AndAlso _continue
				Dim record As New ImageRecord
				record.OriginalIndex = Me.CurrentLineNumber
				record.IsNewDoc = GetValue(valuearray, Columns.MultiPageIndicator).ToLower = "Y"
				record.FileLocation =  GetValue(valuearray, Columns.FileLocation)
				record.BatesNumber =  GetValue(valuearray, Columns.DocumentArtifactID)

				Dim filePath As String = BulkImageFileImporter.GetFileLocation(record)
				If Not filePath = "" Then
					filePath = Me.CheckFile(filePath)
				End If

				If record.BatesNumber = "" Then
					Me.RaiseStatusEvent(EventType.Progress, $"Line {CurrentLineNumber} improperly formatted. Invalid bates number.")
				ElseIf filePath = "" Then
					Me.RaiseStatusEvent(EventType.Progress, $"Record '{record.BatesNumber}' processed - file info error.")
				Else
					Me.RaiseStatusEvent(EventType.Progress, $"Record '{record.BatesNumber}' processed.")
				End If

				If MyBase.HasReachedEOF Then
					valuearray = Nothing
				Else
					valuearray = Me.GetLine
				End If
			End While
		End Sub
		
		Private Function GetValue(ByVal valuearray As String(), index As Int32) As String
			Dim retval As String
			Try
				retval = valuearray(index)
				Return retval
			Catch ex As IndexOutOfRangeException
				Me.RaiseStatusEvent(EventType.Error, $"Line {CurrentLineNumber} is invalid format. No column at index {index}.")
				Return ""
			End Try
		End Function

		Private Function CheckFile(ByVal path As String) As String
			If Not System.IO.File.Exists(path) Then
				Me.RaiseStatusEvent(EventType.Error, $"File '{path}' does not exist.")
				Return ""
			End If

			Return path
		End Function

#Region "Events and Event Handling"

		Public Event FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception)

		Private Sub RaiseFatalError(ByVal ex As System.Exception)
			RaiseEvent FatalErrorEvent("Error processing line: " + CurrentLineNumber.ToString, ex)
		End Sub

		Private Sub RaiseStatusEvent(ByVal et As EventType, ByVal line As String)
			'TODO: track stats
			RaiseEvent StatusMessage(New StatusEventArgs(et, Me.CurrentLineNumber, _fileLineCount, line, Nothing, Nothing))
		End Sub

#End Region

		Private Sub _processObserver_CancelImport(ByVal processID As Guid) Handles _processController.HaltProcessEvent
			_continue = False
		End Sub

	End Class
End Namespace