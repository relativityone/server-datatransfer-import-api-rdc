Imports System.IO
Imports kCura.WinEDDS.Api
Imports kCura.WinEDDS.Helpers
Imports Relativity.DataExchange.Data
Imports Relativity.DataExchange.Media
Imports Relativity.DataExchange.Process

Namespace kCura.WinEDDS
	Public Class ImageFilePreviewer
		Inherits DelimitedFileImporter2

		Private _fileLineCount As Int32
		Private _continue As Boolean
		Private WithEvents _processContext As ProcessContext
		Private ReadOnly _filePathHelper As IFilePathHelper = New ConfigurableFilePathHelper()
		Private Property _imageValidator As IImageValidator = New ImageValidator()
		Private Property _tiffValidator As ITiffValidator = New TiffValidator()
		Private Property _fileInspector As IFileInspector = New FileInspector()

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

		Public Sub New(ByVal context As ProcessContext, ByVal doRetryLogic As Boolean)
			MyBase.New(","c, doRetryLogic)
			
			_processContext = context
			_continue = True
		End Sub


		Public Overloads Overrides Function ReadFile(ByVal path As String) As Object
			Try
				_fileLineCount = Global.Relativity.DataExchange.Io.FileSystem.Instance.File.CountLinesInFile(path)
				Reader = New StreamReader(path)
				RaiseStatusEvent(EventType2.Progress, "Begin Image Upload")

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
						RaiseStatusEvent(EventType2.Error, txt)
					End Try
				End While
				Me.Reader.Close()
				RaiseStatusEvent(EventType2.Progress, "End Image Upload")
			Catch ex As Exception
				RaiseStatusEvent(EventType2.Error, ex.Message)
				RaiseFatalError(ex)
			End Try
			Return Nothing
		End Function

		Private Sub DoFileUpload()
			Dim valuearray As String() = Me.GetLine

			While Not valuearray Is Nothing AndAlso _continue
				Dim record As ImageRecord = CreateImageRecord(valuearray)
				Dim filePath As String = GetFilePathAndValidate(record)

				If record.BatesNumber = "" Then
					Me.RaiseStatusEvent(EventType2.Progress, $"Line {CurrentLineNumber} improperly formatted. Invalid bates number.")
				ElseIf filePath = "" Then
					Me.RaiseStatusEvent(EventType2.Progress, $"Record '{record.BatesNumber}' processed - file info error.")
				Else
					Me.RaiseStatusEvent(EventType2.Progress, $"Record '{record.BatesNumber}' processed.")
				End If

				If MyBase.HasReachedEOF Then
					valuearray = Nothing
				Else
					valuearray = Me.GetLine
				End If
			End While
		End Sub

		Private Function CreateImageRecord(valuearray As String()) As ImageRecord

			Dim record As New ImageRecord
			record.OriginalIndex = Me.CurrentLineNumber
			record.IsNewDoc = GetValue(valuearray, Columns.MultiPageIndicator).ToLower = "Y"
			record.FileLocation =  GetValue(valuearray, Columns.FileLocation)
			record.BatesNumber =  GetValue(valuearray, Columns.DocumentArtifactID)
			Return record
		End Function

		Private Function GetFilePathAndValidate(record As ImageRecord) As String
			Dim filename As String = BulkImageFileImporter.GetFileLocation(record)
			If filename = String.Empty
				Return filename
			End If

			Dim foundFileName As String = _filePathHelper.GetExistingFilePath(filename)
			Dim fileExists As Boolean = Not String.IsNullOrEmpty(foundFileName)
			If Not fileExists Then
				Me.RaiseStatusEvent(EventType2.Error, $"File '{filename}' does not exist.")
				Return String.Empty
			End If

			If Not String.Equals(filename, foundFileName)
				Me.RaiseStatusEvent(EventType2.Warning, $"File '{filename}' does not exist. File {foundFileName} will be used instead.")
			End If

			If Not ValidateImage(foundFileName) Then
				Return String.Empty
			End If

			Return foundFileName
		End Function

		Private Function GetValue(ByVal valuearray As String(), index As Int32) As String
			Dim retval As String
			Try
				retval = valuearray(index)
				Return retval
			Catch ex As IndexOutOfRangeException
				Me.RaiseStatusEvent(EventType2.Error, $"Line {CurrentLineNumber} is invalid format. No column at index {index}.")
				Return ""
			End Try
		End Function

		Private Function ValidateImage(path As String) As Boolean
			Try
				Dim result As ImageValidationResult = _imageValidator.IsImageValid(path, _tiffValidator, _fileInspector)
				If(Not result.IsValid)
					Throw New ImageFileValidationException(result.Message)
				End If
				Return True
			Catch ex As Exception
				RaiseStatusEvent(EventType2.Error, ex.Message)
				Return False
			End Try
		End Function

#Region "Events and Event Handling"

		Public Event FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception)

		Private Sub RaiseFatalError(ByVal ex As System.Exception)
			RaiseEvent FatalErrorEvent("Error processing line: " + CurrentLineNumber.ToString, ex)
		End Sub

		Private Sub RaiseStatusEvent(ByVal et As EventType2, ByVal line As String)
			'TODO: track stats
			RaiseEvent StatusMessage(New StatusEventArgs(et, Me.CurrentLineNumber, _fileLineCount, line, Nothing, Nothing))
		End Sub

#End Region

		Private Sub _processObserver_CancelImport(ByVal sender As Object, ByVal e As CancellationRequestEventArgs) Handles _processContext.CancellationRequest
			_continue = False
		End Sub

	End Class
End Namespace