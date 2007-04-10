Imports System.IO
Namespace kCura.WinEDDS
	Public Class ImageFileImporter
		Inherits kCura.Utility.DelimitedFileImporter

#Region "Members"

		Private _docManager As kCura.WinEDDS.Service.DocumentManager
		Private _fieldQuery As kCura.WinEDDS.Service.FieldQuery
		Private _folderManager As kCura.WinEDDS.Service.FolderManager
		Private _fileUploader As kCura.WinEDDS.FileUploader
		Private _fileManager As kCura.WinEDDS.Service.FileManager
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
		Private _errorLogFileName As String
		Private _errorLogWriter As System.IO.StreamWriter

		Private WithEvents _processController As kCura.Windows.Process.Controller

#End Region

#Region "Accessors"

		Friend WriteOnly Property FilePath() As String
			Set(ByVal value As String)
				_filePath = value
			End Set
		End Property

		Public ReadOnly Property ErrorLogFileName() As String
			Get
				Return _errorLogFileName
			End Get
		End Property

		Public ReadOnly Property HasErrors() As Boolean
			Get
				Return _errorLogFileName <> ""
			End Get
		End Property

		Protected ReadOnly Property Continue() As Boolean
			Get
				Return Not MyBase.HasReachedEOF AndAlso _continue
			End Get
		End Property

#End Region

#Region "Constructors"

		Public Sub New(ByVal folderID As Int32, ByVal args As ImageLoadFile, ByVal controller As kCura.Windows.Process.Controller)
			MyBase.New(New Char() {","c})
			_docManager = New kCura.WinEDDS.Service.DocumentManager(args.Credential, args.CookieContainer)
			_fieldQuery = New kCura.WinEDDS.Service.FieldQuery(args.Credential, args.CookieContainer)
			_folderManager = New kCura.WinEDDS.Service.FolderManager(args.Credential, args.CookieContainer)
			_fileManager = New kCura.WinEDDS.Service.FileManager(args.Credential, args.CookieContainer)
			_fileUploader = New kCura.WinEDDS.FileUploader(args.Credential, args.CaseInfo.ArtifactID, _docManager.GetDocumentDirectoryByCaseArtifactID(args.CaseInfo.ArtifactID) & "\", args.CookieContainer)
			_folderID = folderID
			_overwrite = args.Overwrite
			_replaceFullText = args.ReplaceFullText
			_selectedIdentifierField = args.ControlKeyField
			_processController = controller
			_continue = True
		End Sub

#End Region

#Region "Enumerations"

		Private Enum Columns
			BatesNumber = 0
			FileLocation = 2
			MultiPageIndicator = 3
		End Enum

#End Region

#Region "Main"

		Public Overloads Sub ReadFile()
			Me.ReadFile(_filePath)
		End Sub

		Private Sub ProcessList(ByVal al As System.Collections.ArrayList, ByRef sectionHasErrors As Boolean)
			If al.Count = 0 Then Exit Sub
			If Not sectionHasErrors Then
				Me.ProcessDocument(al)
			Else
				Me.LogErrorInFile(al)
			End If
			al.Clear()
			sectionHasErrors = False
		End Sub

		Public Overloads Overrides Function ReadFile(ByVal path As String) As Object
			Try
				Dim documentIdentifier As String = String.Empty
				_fileLineCount = kCura.Utility.File.CountLinesInFile(path)
				Reader = New StreamReader(path)
				RaiseStatusEvent(kCura.Windows.Process.EventType.Progress, "Begin Image Upload")
				Dim al As New System.collections.ArrayList
				Dim line As String()
				Dim sectionHasErrors As Boolean = False
				While Me.Continue
					Try
						line = Me.GetLine
						If (line(Columns.MultiPageIndicator).ToUpper = "Y") Then
							Me.ProcessList(al, sectionHasErrors)
						End If
						sectionHasErrors = sectionHasErrors Or Not Me.ProcessImageLine(line)
						al.Add(line)
						If Not Me.Continue Then
							Me.ProcessList(al, sectionHasErrors)
							Exit While
						End If
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
				If Not _errorLogWriter Is Nothing Then
					_errorLogWriter.Close()
				End If
				Me.Reader.Close()
				RaiseStatusEvent(kCura.Windows.Process.EventType.Progress, "End Image Upload")
			Catch ex As System.Exception
				RaiseFatalError(ex)
			End Try
		End Function

		Public Function ProcessDocument(ByVal al As System.Collections.ArrayList) As Object
			Try
				Dim fullTextBuilder As New kCura.EDDS.Types.FullTextBuilder
				Dim fileGuids As New ArrayList
				Dim fileNames As New ArrayList
				Dim fileDTOs As New ArrayList
				Dim fullTextFileGuid As String
				Dim retval As String()
				Dim identifierRow As String() = DirectCast(al(0), String())
				Dim documentIdentifier As String = String.Copy(identifierRow(Columns.BatesNumber))
				Dim fileLocation As String = String.Copy(identifierRow(Columns.FileLocation))
				If fileLocation.Chars(0) = "\" AndAlso fileLocation.Chars(1) <> "\" Then
					fileLocation = "." & fileLocation
				End If
				Dim multipageindicator As String = String.Copy(identifierRow(Columns.MultiPageIndicator))

				Dim currentDocumentArtifactID As Int32 = _docManager.GetDocumentArtifactIDFromIdentifier(_fileUploader.CaseArtifactID, documentIdentifier, _selectedIdentifierField)
				If currentDocumentArtifactID > 0 Then
					If _overwrite.ToLower = "strict" OrElse _overwrite.ToLower = "append" Then
						GetImagesForDocument(al, fileDTOs, fullTextBuilder)
						If _replaceFullText Then fullTextFileGuid = _fileUploader.UploadTextAsFile(fullTextBuilder.FullText, _folderID, System.Guid.NewGuid.ToString)
						_docManager.ClearImagesFromDocument(_fileUploader.CaseArtifactID, currentDocumentArtifactID)
						If _replaceFullText Then
							_docManager.AddFullTextToDocumentFromFile(_fileUploader.CaseArtifactID, currentDocumentArtifactID, fullTextFileGuid, fullTextBuilder)
						End If
						'Update Document
					Else
						Throw New OverwriteNoneException
					End If
				Else
					'Create Document
					If _overwrite.ToLower = "strict" Then
						Throw New OverwriteStrictException
					End If
					GetImagesForDocument(al, fileDTOs, fullTextBuilder)
					If _replaceFullText Then
						fullTextFileGuid = _fileUploader.UploadTextAsFile(fullTextBuilder.FullText, _folderID, System.Guid.NewGuid.ToString)
					Else
						fullTextFileGuid = _fileUploader.UploadTextAsFile(String.Empty, _folderID, System.Guid.NewGuid.ToString)
					End If
					currentDocumentArtifactID = CreateDocument(documentIdentifier, fullTextFileGuid, fullTextBuilder)
				End If
				_fileManager.CreateImages(_fileUploader.CaseArtifactID, DirectCast(fileDTOs.ToArray(GetType(kCura.EDDS.WebAPI.FileManagerBase.FileInfoBase)), kCura.EDDS.WebAPI.FileManagerBase.FileInfoBase()), currentDocumentArtifactID)
				Return retval
			Catch ex As System.Exception
				Me.LogErrorInFile(al)
				Throw
			End Try
		End Function

#End Region

#Region "Worker Methods"

		Public Function ProcessImageLine(ByVal values As String()) As Boolean
			Dim retval As Boolean = True
			'check for existence
			If values(Columns.BatesNumber).Trim = "" Then
				Me.RaiseStatusEvent(Windows.Process.EventType.Error, String.Format("Bates number '{0}'cannot be empty.", values(Columns.BatesNumber)))
				retval = False
			End If
			If values(Columns.BatesNumber).Trim = "" Then
				Me.RaiseStatusEvent(Windows.Process.EventType.Error, String.Format("No image file specified on line."))
				retval = False
			ElseIf Not System.IO.File.Exists(Me.GetFileLocation(values)) Then
				Me.RaiseStatusEvent(Windows.Process.EventType.Error, String.Format("Image file specified ( {0} ) does not exist.", values(Columns.FileLocation)))
				retval = False
			End If
			Return retval
			'check to make sure image is good
		End Function

		Private Sub LogErrorInFile(ByVal lines As System.Collections.ArrayList)
			If lines Is Nothing Then Exit Sub
			If _errorLogFileName = "" Then
				_errorLogFileName = System.IO.Path.GetTempFileName()
				_errorLogWriter = New System.IO.StreamWriter(_errorLogFileName, False, System.Text.Encoding.Default)
			End If
			Dim line As String()
			For Each line In lines
				_errorLogWriter.WriteLine(kCura.Utility.Array.StringArrayToCsv(line))
			Next
		End Sub

		Public Shared Function GetFileLocation(ByVal line As String()) As String
			Dim fileLocation As String = line(Columns.FileLocation)
			If fileLocation.Chars(0) = "\" AndAlso fileLocation.Chars(1) <> "\" Then
				fileLocation = "." & fileLocation
			End If
			Return fileLocation
		End Function

		Private Function GetImagesForDocument(ByVal lines As ArrayList, ByVal fileDTOs As ArrayList, ByVal fullTextBuilder As kCura.EDDS.Types.FullTextBuilder) As String()
			Dim valueArray As String()
			For Each valueArray In lines
				GetImageForDocument(GetFileLocation(valueArray), valueArray(Columns.BatesNumber), fileDTOs, fullTextBuilder)
			Next
		End Function

		Private Sub GetImageForDocument(ByVal imageFileName As String, ByVal batesNumber As String, ByVal fileDTOs As ArrayList, ByVal fullTextBuilder As kCura.EDDS.Types.FullTextBuilder)
			Dim filename As String = imageFileName.Substring(imageFileName.LastIndexOf("\") + 1)
			RaiseStatusEvent(kCura.Windows.Process.EventType.Progress, String.Format("Uploading File '{0}'.", filename))
			Dim extractedTextFileName As String = imageFileName.Substring(0, imageFileName.LastIndexOf("."c) + 1) & "txt"
			Dim file As New kCura.EDDS.WebAPI.FileManagerBase.FileInfoBase
			file.FileGuid = _fileUploader.UploadFile(imageFileName, _folderID)
			file.FileName = filename
			file.Identifier = batesNumber
			fileDTOs.Add(file)
			If _replaceFullText AndAlso System.IO.File.Exists(extractedTextFileName) Then
				Dim sr As New System.IO.StreamReader(extractedTextFileName, System.Text.Encoding.Default, True)
				fullTextBuilder.AppendPage(sr.ReadToEnd)
				sr.Close()
			Else
				If _replaceFullText AndAlso Not System.IO.File.Exists(extractedTextFileName) Then
					RaiseStatusEvent(kCura.Windows.Process.EventType.Warning, String.Format("File '{0}' not found.  No text updated.", extractedTextFileName))
				End If
			End If
		End Sub

		Private Function CreateDocument(ByVal identifier As String, ByVal fullTextFileName As String, ByVal fullTextBuilder As kCura.EDDS.Types.FullTextBuilder) As Int32
			Dim fieldID As Int32
			Dim encoder As New System.Text.ASCIIEncoding
			Try
				Return _docManager.CreateEmptyDocument(_fileUploader.CaseArtifactID, _folderID, encoder.GetBytes(identifier), fullTextFileName, _selectedIdentifierField, fullTextBuilder)
			Catch ex As System.Exception
				If kCura.WinEDDS.Config.UsesWebAPI Then
					Throw New CreateDocumentException(ex)
				Else
					Throw
				End If
			End Try
		End Function

#End Region

#Region "Events and Event Handling"

		Public Event FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception)
		Public Event StatusMessage(ByVal args As kCura.Windows.Process.StatusEventArgs)

		Private Sub RaiseFatalError(ByVal ex As System.Exception)
			RaiseEvent FatalErrorEvent("Error processing line: " + CurrentLineNumber.ToString, ex)
		End Sub

		Private Sub RaiseStatusEvent(ByVal et As kCura.Windows.Process.EventType, ByVal line As String)
			RaiseEvent StatusMessage(New kCura.Windows.Process.StatusEventArgs(et, Me.CurrentLineNumber, _fileLineCount, line))
		End Sub

		Private Sub _processObserver_CancelImport(ByVal processID As System.Guid) Handles _processController.HaltProcessEvent
			_continue = False
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

		Public Class OverwriteNoneException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New()
				MyBase.New("Document exists - upload aborted.")
			End Sub
		End Class

		Public Class OverwriteStrictException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New()
				MyBase.New("Document does not exist - upload aborted.")
			End Sub
		End Class

#End Region

	End Class
End Namespace