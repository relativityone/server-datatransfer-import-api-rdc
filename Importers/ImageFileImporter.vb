Imports System.IO
Namespace kCura.WinEDDS
	Public Class ImageFileImporter
		Inherits kCura.Utility.DelimitedFileImporter

		Private _docManager As kCura.WinEDDS.Service.DocumentManager
		Private _fieldQuery As kCura.WinEDDS.Service.FieldQuery
		Private _folderManager As kCura.WinEDDS.Service.FolderManager
		Private _fileUploader As kCura.WinEDDS.FileUploader
		Private _fileManager As kCura.WinEDDS.Service.FileManager
		Private _folderID As Int32
		Private _overwrite As Boolean
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

		Protected ReadOnly Property Continue() As Boolean
			Get
				Return Not MyBase.HasReachedEOF AndAlso _continue
			End Get
		End Property

		Public Event StatusMessage(ByVal args As kCura.Windows.Process.StatusEventArgs)

		Public Sub New(ByVal folderID As Int32, ByVal args As ImageLoadFile, ByVal controller As kCura.Windows.Process.Controller)
			MyBase.New(New Char() {","c})
			_docManager = New kCura.WinEDDS.Service.DocumentManager(args.Credential, args.CookieContainer, args.Identity)
			_fieldQuery = New kCura.WinEDDS.Service.FieldQuery(args.Credential, args.CookieContainer, args.Identity)
			_folderManager = New kCura.WinEDDS.Service.FolderManager(args.Credential, args.CookieContainer, args.Identity)
			_fileManager = New kCura.WinEDDS.Service.FileManager(args.Credential, args.CookieContainer, args.Identity)
			_fileUploader = New kCura.WinEDDS.FileUploader(args.Credential, _docManager.GetDocumentDirectoryByCaseArtifactID(args.CaseInfo.ArtifactID) & "\", args.CookieContainer)
			_folderID = folderID
			_overwrite = args.Overwrite
			_replaceFullText = args.ReplaceFullText
			_selectedIdentifierField = args.ControlKeyField
			_processController = controller
			_continue = True
		End Sub

		Public Overloads Overrides Function ReadFile(ByVal path As String) As Object
			Try
				Dim documentIdentifier As String = String.Empty
				_fileLineCount = kCura.Utility.File.CountLinesInFile(path)
				'Dim documents As New kCura.Data.DataView(_docManager.GetAllDocumentsForCase(_folderID).Tables(0))
				'Dim fields As New kCura.Data.DataView(_fieldQuery.RetrieveAllMappable(_folderID).Tables(0))
				Reader = New StreamReader(path)
				RaiseStatusEvent(kCura.Windows.Process.EventType.Progress, "Begin Image Upload")

				_csvwriter = New System.Text.StringBuilder
				_csvwriter.Append("DocInitialization" & Microsoft.VisualBasic.ControlChars.Tab)
				_csvwriter.Append("OW Check Delete Existing" & Microsoft.VisualBasic.ControlChars.Tab)
				_csvwriter.Append("File Upload" & Microsoft.VisualBasic.ControlChars.Tab)
				_csvwriter.Append("DocumentManager.Create" & Microsoft.VisualBasic.ControlChars.Tab)
				_csvwriter.Append("FileManager.Create" & Microsoft.VisualBasic.ControlChars.Tab)
				_csvwriter.Append("UpdateFullText" & Microsoft.VisualBasic.ControlChars.Tab)
				_csvwriter.Append("File size" & Microsoft.VisualBasic.ControlChars.Tab)
				_csvwriter.Append(System.Environment.NewLine)
				While Continue
					Try
						DoFileUpload()
					Catch ex As System.Exception
						RaiseStatusEvent(kCura.Windows.Process.EventType.Error, ex.Message)
					End Try
				End While
				Me.Reader.Close()
				'Dim outputfile As String = "C:\UploadMetrics\" & DateTime.Now.Year.ToString & DateTime.Now.Month.ToString & DateTime.Now.Day.ToString & "_" & System.Guid.NewGuid.ToString & ".txt"
				'Dim sr As New System.IO.StreamWriter(outputfile)
				'sr.Write(_csvwriter.ToString)
				'sr.Close()
				RaiseStatusEvent(kCura.Windows.Process.EventType.Progress, "End Image Upload")
			Catch ex As System.Exception
				RaiseFatalError(ex)
			End Try
		End Function

		Private Sub DoFileUpload()
			Dim valuearray As String()
			valuearray = Me.GetLine
			While Not valuearray Is Nothing
				valuearray = BuildDocument(valuearray)
			End While
		End Sub

		Private Function BuildDocument(ByVal valueArray As String()) As String()
			'Dim fullTextBuilder As New System.Text.StringBuilder
			Dim fullTextBuilder As New kCura.EDDS.Types.FullTextBuilder
			Dim fileGuids As New ArrayList
			Dim fileNames As New ArrayList
			Dim fileDTOs As New ArrayList
			Dim fullTextFileGuid As String
			Dim retval As String()
			Dim documentIdentifier As String = String.Copy(valueArray(Columns.DocumentArtifactID))
			Dim fileLocation As String = String.Copy(valueArray(Columns.FileLocation))
			Dim multipageindicator As String = String.Copy(valueArray(Columns.MultiPageIndicator))

			Dim currentDocumentArtifactID As Int32 = _docManager.GetDocumentArtifactIDFromIdentifier(documentIdentifier, _selectedIdentifierField, _folderID)
			If currentDocumentArtifactID > 0 Then
				If _overwrite Then
					GetImageForDocument(fileLocation, fileDTOs, fullTextBuilder)
					retval = GetImagesForDocument(fileDTOs, fullTextBuilder)
					If _replaceFullText Then fullTextFileGuid = _fileUploader.UploadTextAsFile(fullTextBuilder.FullText, _folderID, System.Guid.NewGuid.ToString)
					_docManager.ClearImagesFromDocument(currentDocumentArtifactID)
					If _replaceFullText Then _docManager.AddFullTextToDocumentFromFile(currentDocumentArtifactID, fullTextFileGuid, fullTextBuilder)
					'Update Document
				Else
					Throw New OverwriteException
				End If
			Else
				'Create Document
				GetImageForDocument(fileLocation, fileDTOs, fullTextBuilder)
				retval = GetImagesForDocument(fileDTOs, fullTextBuilder)
				If _replaceFullText Then
					fullTextFileGuid = _fileUploader.UploadTextAsFile(fullTextBuilder.FullText, _folderID, System.Guid.NewGuid.ToString)
				Else
					fullTextFileGuid = _fileUploader.UploadTextAsFile(String.Empty, _folderID, System.Guid.NewGuid.ToString)
				End If
				currentDocumentArtifactID = CreateDocument(documentIdentifier, fullTextFileGuid, fullTextBuilder)
			End If
			_fileManager.CreateImages(DirectCast(fileDTOs.ToArray(GetType(kCura.EDDS.WebAPI.FileManagerBase.FileInfoBase)), kCura.EDDS.WebAPI.FileManagerBase.FileInfoBase()), currentDocumentArtifactID, _folderID)
			'For i = 0 To fileNames.Count - 1
			'	_fileManager.CreateFile(_folderID, currentDocID, fileNames(i).ToString, fileGuids(i).ToString, i, kCura.EDDS.Types.FileType.Tif)
			'Next
			Return retval
		End Function

		Private Function GetImagesForDocument(ByVal fileDTOs As ArrayList, ByVal fullTextBuilder As kCura.EDDS.Types.FullTextBuilder) As String()
			Dim valueArray As String()
			While Continue
				valueArray = Me.GetLine
				If valueArray(Columns.MultiPageIndicator).ToLower = "y" Then Return valueArray
				GetImageForDocument(valueArray(Columns.FileLocation), fileDTOs, fullTextBuilder)
			End While
		End Function

		Private Sub GetImageForDocument(ByVal imageFileName As String, ByVal fileDTOs As ArrayList, ByVal fullTextBuilder As kCura.EDDS.Types.FullTextBuilder)
			Dim filename As String = imageFileName.Substring(imageFileName.LastIndexOf("\") + 1)
			RaiseStatusEvent(kCura.Windows.Process.EventType.Progress, String.Format("Uploading File '{0}'.", filename))
			Dim extractedTextFileName As String = imageFileName.Substring(0, imageFileName.LastIndexOf("."c) + 1) & "txt"
			Dim file As New kCura.EDDS.WebAPI.FileManagerBase.FileInfoBase
			file.FileGuid = _fileUploader.UploadFile(imageFileName, _folderID)
			file.FileName = filename
			fileDTOs.Add(file)
			If _replaceFullText AndAlso System.IO.File.Exists(extractedTextFileName) Then
				'Dim sr As New System.IO.FileStream(extractedTextFileName, FileMode.Open)
				'Dim b As Int32 = sr.ReadByte
				'While Not b = -1
				'	If b = 160 Then
				'		b = 32
				'	End If
				'	fullTextBuilder.Append(ChrW(b))
				'	b = sr.ReadByte
				'End While
				'sr.Close()
				Dim sr As New System.IO.StreamReader(extractedTextFileName, System.Text.Encoding.Default, True)
				fullTextBuilder.AppendPage(sr.ReadToEnd)
				sr.Close()
			Else
				If Not _replaceFullText Then
					RaiseStatusEvent(kCura.Windows.Process.EventType.Warning, String.Format("File '{0}' not found.  No text updated.", extractedTextFileName))
				End If
			End If
		End Sub

		Private Function CreateDocument(ByVal identifier As String, ByVal fullTextFileName As String, ByVal fullTextBuilder As kCura.EDDS.Types.FullTextBuilder) As Int32
			Dim fieldID As Int32
			Dim encoder As New System.Text.ASCIIEncoding
			Try
				Return _docManager.CreateEmptyDocument(_folderID, encoder.GetBytes(identifier), fullTextFileName, _selectedIdentifierField, fullTextBuilder)
			Catch ex As System.Exception
				Throw New CreateDocumentException(ex)
			End Try
		End Function

#Region "Events and Event Handling"

		Public Event FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception)

		Private Sub RaiseFatalError(ByVal ex As System.Exception)
			RaiseEvent FatalErrorEvent("Error processing line: " + CurrentLineNumber.ToString, ex)
		End Sub

		Private Sub RaiseStatusEvent(ByVal et As kCura.Windows.Process.EventType, ByVal line As String)
			RaiseEvent StatusMessage(New kCura.Windows.Process.StatusEventArgs(et, Me.CurrentLineNumber, _fileLineCount, line))
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