Imports System.IO
Namespace kCura.WinEDDS
	Public Class ImageFilePreviewer
		Inherits kCura.Utility.DelimitedFileImporter

		Protected _docManager As kCura.WinEDDS.Service.DocumentManager
		Protected _fieldQuery As kCura.WinEDDS.Service.FieldQuery
		Protected _folderManager As kCura.WinEDDS.Service.FolderManager
		Private _fileUploader As kCura.WinEDDS.FileUploader
		Protected _fileManager As kCura.WinEDDS.Service.FileManager
		Private _folderID As Int32
		Private _overwrite As String
		Private _filePath As String
		Private _selectedIdentifierField As String
		Private _fileLineCount As Int32
		Private _continue As Boolean
		Private _overwriteOK As Boolean
		Private _replaceFullText As Boolean
		Private _order As Int32
		Private _csvwriter As System.Text.StringBuilder
		Private _nextLine As String()
		Private WithEvents _processController As kCura.Windows.Process.Controller

		Private Enum Columns
			DocumentArtifactID = 0
			FileLocation = 2
			MultiPageIndicator = 3
		End Enum

		Friend WriteOnly Property FilePath() As String
			Set(ByVal value As String)
				_filePath = value
			End Set
		End Property

		Public Overloads Sub ReadFile()
			Me.ReadFile(_filePath)
		End Sub

		Protected ReadOnly Property [Continue]() As Boolean
			Get
				Return Not MyBase.HasReachedEOF AndAlso _continue
			End Get
		End Property

		Public Event StatusMessage(ByVal args As kCura.Windows.Process.StatusEventArgs)

		Public Sub New(ByVal args As ImageLoadFile, ByVal controller As kCura.Windows.Process.Controller, ByVal doRetryLogic As Boolean)
			MyBase.New(New Char() {","c}, doRetryLogic)

			_docManager = New kCura.WinEDDS.Service.DocumentManager(args.Credential, args.CookieContainer)
			_fieldQuery = New kCura.WinEDDS.Service.FieldQuery(args.Credential, args.CookieContainer)
			_folderManager = New kCura.WinEDDS.Service.FolderManager(args.Credential, args.CookieContainer)
			_fileManager = New kCura.WinEDDS.Service.FileManager(args.Credential, args.CookieContainer)
			InitializeUploaders(args)

			_overwrite = args.Overwrite
			_replaceFullText = args.ReplaceFullText
			_selectedIdentifierField = args.ControlKeyField
			_processController = controller
			_continue = True
		End Sub

		Protected Overridable Sub InitializeUploaders(ByVal args As ImageLoadFile)
			_fileUploader = New kCura.WinEDDS.FileUploader(args.Credential, args.CaseInfo.ArtifactID, _docManager.GetDocumentDirectoryByCaseArtifactID(args.CaseInfo.ArtifactID) & "\", args.CookieContainer)
		End Sub

		Public Overloads Overrides Function ReadFile(ByVal path As String) As Object
			Try
				Dim documentIdentifier As String = String.Empty
				_fileLineCount = kCura.Utility.File.CountLinesInFile(path)
				Reader = New StreamReader(path)
				RaiseStatusEvent(kCura.Windows.Process.EventType.Progress, "Begin Image Upload")

				While [Continue]
					Try
						DoFileUpload()
					Catch ex As System.Exception
						Dim txt As String = ex.ToString.ToLower
						If txt.IndexOf("ix_") <> -1 AndAlso txt.IndexOf("duplicate") <> -1 Then
							txt = "Error creating document - identifier field isn't being properly filled.  Please choose a different 'key' field."
						Else
							txt = ex.Message
						End If
						RaiseStatusEvent(kCura.Windows.Process.EventType.Error, txt)
					End Try
				End While
				Me.Reader.Close()
				RaiseStatusEvent(kCura.Windows.Process.EventType.Progress, "End Image Upload")
			Catch ex As System.Exception
				RaiseFatalError(ex)
			End Try
			Return Nothing
		End Function

		Private Sub DoFileUpload()
			Dim valuearray As String()
			valuearray = Me.GetLine
			Dim filePath As String
			While Not valuearray Is Nothing AndAlso _continue
				Dim record As New Api.ImageRecord
				record.OriginalIndex = Me.CurrentLineNumber
				record.IsNewDoc = valuearray(Columns.MultiPageIndicator).ToLower = "Y"
				filePath = BulkImageFileImporter.GetFileLocation(record)
				If Not filePath = "" Then filePath = Me.CheckFile(filePath)
				record.BatesNumber = Me.GetBatesNumber(valuearray)
				If record.BatesNumber = "" Then
					Me.RaiseStatusEvent(Windows.Process.EventType.Progress, "Line improperly formatted.")
				ElseIf filePath = "" Then
					Me.RaiseStatusEvent(Windows.Process.EventType.Progress, String.Format("Record '{0}' processed - file info error.", record.BatesNumber))
				Else
					Me.RaiseStatusEvent(Windows.Process.EventType.Progress, String.Format("Record '{0}' processed.", record.BatesNumber))
				End If
				If MyBase.HasReachedEOF Then
					valuearray = Nothing
				Else
					valuearray = Me.GetLine
				End If
			End While
		End Sub

		Public Function GetFilePath(ByVal valuearray As String()) As String
			Dim retval As String
			Try
				retval = valuearray(Columns.FileLocation)
				Return retval
			Catch ex As IndexOutOfRangeException
				Me.RaiseStatusEvent(Windows.Process.EventType.Error, "Invalid line format - no file specified in line.")
				Return ""
			End Try
		End Function

		Public Function GetBatesNumber(ByVal valuearray As String()) As String
			Dim retval As String
			Try
				retval = valuearray(Columns.DocumentArtifactID)
				Return retval
			Catch ex As IndexOutOfRangeException
				Me.RaiseStatusEvent(Windows.Process.EventType.Error, "Invalid line format - Bates number specified in line.")
				Return ""
			End Try
		End Function

		Public Function CheckFile(ByVal path As String) As String
			'See if exists
			If Not System.IO.File.Exists(path) Then
				Me.RaiseStatusEvent(Windows.Process.EventType.Error, String.Format("File '{0}' does not exist.", path))
				Return ""
			End If

			Dim validator As New kCura.ImageValidator.ImageValidator
			Try
				'validator.ValidateImage(path)
			Catch ex As System.Exception
				Me.RaiseStatusEvent(Windows.Process.EventType.Error, String.Format("Error in '{0}': {1}", path, ex.Message))
				Return ""
			End Try
			Return path
		End Function

#Region "Events and Event Handling"

		Public Event FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception)

		Private Sub RaiseFatalError(ByVal ex As System.Exception)
			RaiseEvent FatalErrorEvent("Error processing line: " + CurrentLineNumber.ToString, ex)
		End Sub

		Private Sub RaiseStatusEvent(ByVal et As kCura.Windows.Process.EventType, ByVal line As String)
			'TODO: track stats
			RaiseEvent StatusMessage(New kCura.Windows.Process.StatusEventArgs(et, Me.CurrentLineNumber, _fileLineCount, line, Nothing))
		End Sub

#End Region

#Region "Exceptions"
		Public Class FileLoadException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New()
				MyBase.New("Error uploading file.  Skipping line.")
			End Sub
		End Class

		Public Class CreateDocumentException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal parentException As System.Exception)
				MyBase.New("Error creating new document.  Skipping line: " & parentException.Message, parentException)
			End Sub
		End Class

		Public Class OverwriteException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New()
				MyBase.New("Document exists - upload aborted.")
			End Sub
		End Class

#End Region

		Private Sub _processObserver_CancelImport(ByVal processID As System.Guid) Handles _processController.HaltProcessEvent
			_continue = False
		End Sub

	End Class
End Namespace