Imports System.IO
Namespace kCura.WinEDDS
	Public Class ImageFileImporter
		Inherits kCura.Utility.DelimitedFileImporter

#Region "Members"

		Private _docManager As kCura.WinEDDS.Service.DocumentManager
		Private _fieldQuery As kCura.WinEDDS.Service.FieldQuery
		Private _folderManager As kCura.WinEDDS.Service.FolderManager
		Private _fileUploader As kCura.WinEDDS.FileUploader
		Private _textUploader As kCura.WinEDDS.FileUploader
		Private _fileManager As kCura.WinEDDS.Service.FileManager
		Private _productionManager As kCura.WinEDDS.Service.ProductionManager
		Private _folderID As Int32
		Private _productionArtifactID As Int32
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
		Private _autoNumberImages As Boolean
		Private _copyFilesToRepository As Boolean
		Private _repositoryPath As String
		Private _textRepositoryPath As String

		Private WithEvents _processController As kCura.Windows.Process.Controller
		Private _productionDTO As kCura.EDDS.WebAPI.ProductionManagerBase.Production
		Private _keyFieldDto As kCura.EDDS.WebAPI.FieldManagerBase.Field
		Private _timekeeper As New kCura.Utility.Timekeeper
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

		Public Sub New(ByVal folderID As Int32, ByVal args As ImageLoadFile, ByVal controller As kCura.Windows.Process.Controller, ByVal doRetryLogic As Boolean)
			MyBase.New(New Char() {","c}, doRetryLogic)
			_docManager = New kCura.WinEDDS.Service.DocumentManager(args.Credential, args.CookieContainer)
			_fieldQuery = New kCura.WinEDDS.Service.FieldQuery(args.Credential, args.CookieContainer)
			_folderManager = New kCura.WinEDDS.Service.FolderManager(args.Credential, args.CookieContainer)
			_fileManager = New kCura.WinEDDS.Service.FileManager(args.Credential, args.CookieContainer)
			_productionManager = New kCura.WinEDDS.Service.ProductionManager(args.Credential, args.CookieContainer)
			_repositoryPath = args.SelectedCasePath & "EDDS" & args.CaseInfo.ArtifactID & "\"
			_textRepositoryPath = args.CaseDefaultPath & "EDDS" & args.CaseInfo.ArtifactID & "\"
			_fileUploader = New kCura.WinEDDS.FileUploader(args.Credential, args.CaseInfo.ArtifactID, _repositoryPath, args.CookieContainer)
			_textUploader = New kCura.WinEDDS.FileUploader(args.Credential, args.CaseInfo.ArtifactID, _textRepositoryPath, args.CookieContainer)
			_folderID = folderID

			_productionArtifactID = args.ProductionArtifactID
			If _productionArtifactID <> 0 Then
				_productionDTO = _productionManager.Read(args.CaseInfo.ArtifactID, _productionArtifactID)
				_keyFieldDto = New kCura.WinEDDS.Service.FieldManager(args.Credential, args.CookieContainer).Read(args.CaseInfo.ArtifactID, args.BeginBatesFieldArtifactID)
			End If
			_overwrite = args.Overwrite
			_replaceFullText = args.ReplaceFullText
			_selectedIdentifierField = args.ControlKeyField
			_processController = controller
			_copyFilesToRepository = args.CopyFilesToDocumentRepository
			_continue = True
			_autoNumberImages = args.AutoNumberImages
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
			Try
				If al.Count = 0 Then Exit Sub
				Try
					If Not sectionHasErrors Then
						If _productionArtifactID = 0 Then
							_timekeeper.MarkStart("ReadFile_ProcessList_ProcessDocument")
							Me.ProcessDocument(al)
							_timekeeper.MarkEnd("ReadFile_ProcessList_ProcessDocument")
						Else
							Me.ProcessDocumentForProduction(al)
						End If
					Else
						Me.LogErrorInFile(al)
					End If
				Catch ex As System.Exception
					Me.LogErrorInFile(al)
					Dim txt As String = ex.ToString.ToLower
					If txt.IndexOf("kcurafatalexception") <> -1 Then
						txt = ex.Message.Replace("System.Web.Services.Protocols.SoapException: Server was unable to process request. ---> System.Exception: kCuraFatalException:", "")
						Dim sr As New System.IO.StringReader(txt)
						Dim batesNumber As String = sr.ReadLine
						sr.Close()
						txt = "Error: the bates number '" & batesNumber & "' already exists in the system."
					ElseIf txt.IndexOf("ix_") <> -1 AndAlso txt.IndexOf("duplicate") <> -1 Then
						If txt.IndexOf("eddsdbo.file") <> -1 Then
							txt = "Error creating document - one or more image's bates numbers in the document to import already exist in the system."
						Else
							txt = "Error creating document - identifier field isn't being properly filled.  Please choose a different 'key' field."
						End If
					Else
						txt = ex.ToString
					End If
					sectionHasErrors = True
					RaiseStatusEvent(kCura.Windows.Process.EventType.Error, txt, 1)
				End Try
				al.Clear()
				sectionHasErrors = False
			Catch ex As Exception
				Throw
			End Try
		End Sub

		Public Overloads Overrides Function ReadFile(ByVal path As String) As Object
			_timekeeper.MarkStart("ReadFile")
			Try
				Dim documentIdentifier As String = String.Empty
				_timekeeper.MarkStart("ReadFile_CountLinesInFile")
				_fileLineCount = kCura.Utility.File.CountLinesInFile(path)
				_timekeeper.MarkEnd("ReadFile_CountLinesInFile")
				Reader = New StreamReader(path)
				RaiseStatusEvent(kCura.Windows.Process.EventType.Progress, "Begin Image Upload")
				Dim al As New System.collections.ArrayList
				Dim line As String()
				Dim sectionHasErrors As Boolean = False
				While Me.Continue
					line = Me.GetLine
					If (line(Columns.MultiPageIndicator).ToUpper = "Y") Then
						_timekeeper.MarkStart("ReadFile_ProcessList")
						Me.ProcessList(al, sectionHasErrors)
						_timekeeper.MarkEnd("ReadFile_ProcessList")
					End If
					sectionHasErrors = sectionHasErrors Or Not Me.ProcessImageLine(line)
					al.Add(line)
					If Not Me.Continue Then
						Me.ProcessList(al, sectionHasErrors)
						Exit While
					End If
				End While
				If Not _errorLogWriter Is Nothing Then
					_errorLogWriter.Close()
				End If
				Me.Reader.Close()
				If _productionArtifactID <> 0 Then _productionManager.DoPostImportProcessing(_fileUploader.CaseArtifactID, _productionArtifactID)
				RaiseStatusEvent(kCura.Windows.Process.EventType.Progress, "End Image Upload")
			Catch ex As System.Exception
				Try
					_errorLogWriter.Close()
				Catch x As System.Exception
				End Try
				Try
					Me.Reader.Close()
				Catch x As System.Exception
				End Try
				RaiseFatalError(ex)
			End Try
			_timekeeper.MarkEnd("ReadFile")
			_timekeeper.GenerateCsvReportItemsAsRows()
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
				_timekeeper.MarkStart("ReadFile_ProcessList_ProcessDocument_GetDocumentArtifactIDFromIdentifier")
				Dim currentDocumentArtifactID As Int32 = _docManager.GetDocumentArtifactIDFromIdentifier(_fileUploader.CaseArtifactID, documentIdentifier, _selectedIdentifierField)
				_timekeeper.MarkEnd("ReadFile_ProcessList_ProcessDocument_GetDocumentArtifactIDFromIdentifier")
				If currentDocumentArtifactID > 0 Then
					_timekeeper.MarkStart("ReadFile_ProcessList_ProcessDocument_Update")
					If _overwrite.ToLower = "strict" OrElse _overwrite.ToLower = "append" Then
						_timekeeper.MarkStart("ReadFile_ProcessList_ProcessDocument_Update_GetImagesForDocument")
						GetImagesForDocument(al, fileDTOs, fullTextBuilder)
						_timekeeper.MarkEnd("ReadFile_ProcessList_ProcessDocument_Update_GetImagesForDocument")
						'If _replaceFullText Then fullTextFileGuid = _fileUploader.UploadTextAsFile(fullTextBuilder.FullText, _folderID, System.Guid.NewGuid.ToString)
						Try
							_timekeeper.MarkStart("ReadFile_ProcessList_ProcessDocument_Update_ClearImagesFromDocument")
							_docManager.ClearImagesFromDocument(_fileUploader.CaseArtifactID, currentDocumentArtifactID)
							_timekeeper.MarkEnd("ReadFile_ProcessList_ProcessDocument_Update_ClearImagesFromDocument")
						Catch ex As System.Exception
							If ex.ToString.IndexOf("FK_ProductionDocumentFile") <> -1 Then
								Throw New ProductionOverwriteException(DirectCast(fileDTOs(0), kCura.EDDS.WebAPI.FileManagerBase.FileInfoBase).Identifier)
							ElseIf ex.ToString.IndexOf("The DELETE statement conflicted with the REFERENCE constraint ""FK_Redaction_File""") <> -1 Then
								Throw New RedactionOverwriteException(DirectCast(fileDTOs(0), kCura.EDDS.WebAPI.FileManagerBase.FileInfoBase).Identifier)
							Else
								Throw
							End If
						End Try
						If _replaceFullText Then
							If fullTextBuilder.FullTextLength > kCura.WinEDDS.Service.Settings.MAX_STRING_FIELD_LENGTH Then
								fullTextBuilder.FilePointer = _textUploader.UploadTextAsFile(fullTextBuilder.FullTextString, _folderID, System.Guid.NewGuid.ToString)
								fullTextBuilder.FullText = ""
							End If
							_docManager.AddFullTextToDocument(_fileUploader.CaseArtifactID, currentDocumentArtifactID, fullTextBuilder)
						End If
						'Update Document
					Else
						Throw New OverwriteNoneException(documentIdentifier)
					End If
					_timekeeper.MarkEnd("ReadFile_ProcessList_ProcessDocument_Update")
				Else
					'Create Document
					_timekeeper.MarkStart("ReadFile_ProcessList_ProcessDocument_Create")
					If _overwrite.ToLower = "strict" Then
						Throw New OverwriteStrictException(documentIdentifier)
					End If
					GetImagesForDocument(al, fileDTOs, fullTextBuilder)
					currentDocumentArtifactID = CreateDocument(documentIdentifier, fullTextBuilder)
					_timekeeper.MarkEnd("ReadFile_ProcessList_ProcessDocument_Create")
				End If
				_timekeeper.MarkStart("ReadFile_ProcessList_ProcessDocument_CreateImages")
				_fileManager.CreateImages(_fileUploader.CaseArtifactID, DirectCast(fileDTOs.ToArray(GetType(kCura.EDDS.WebAPI.FileManagerBase.FileInfoBase)), kCura.EDDS.WebAPI.FileManagerBase.FileInfoBase()), currentDocumentArtifactID)
				_timekeeper.MarkEnd("ReadFile_ProcessList_ProcessDocument_CreateImages")
				Return retval
			Catch ex As System.Exception
				'Me.LogErrorInFile(al)
				Throw
			End Try
		End Function

		Public Sub ProcessDocumentForProduction(ByVal al As System.Collections.ArrayList)
			_timekeeper.MarkStart("ProcessDocumentForProduction")
			Try
				Dim identifierRow As String() = DirectCast(al(0), String())
				Dim endRow As String() = DirectCast(al(al.Count - 1), String())
				Dim documentIdentifier As String = String.Copy(identifierRow(Columns.BatesNumber))
				Dim currentDocumentArtifactID As Int32 = _docManager.GetDocumentArtifactIDFromIdentifier(_fileUploader.CaseArtifactID, documentIdentifier, _keyFieldDto.DisplayName)
				Dim imageFileGuids As String()

				If currentDocumentArtifactID > 0 Then
					If _overwrite.ToLower = "none" Then Throw New OverwriteNoneException(documentIdentifier)
					imageFileGuids = _fileManager.ReturnFileGuidsForOriginalImages(_fileUploader.CaseArtifactID, currentDocumentArtifactID)
					If al.Count <> imageFileGuids.Length Then
						If imageFileGuids.Length > 0 Then
							Throw New ImageCountMismatchException
						Else
							Dim imageDTOs As New ArrayList
							GetImagesForDocument(al, imageDTOs, New kCura.EDDS.Types.FullTextBuilder)
							_fileManager.CreateImages(_fileUploader.CaseArtifactID, DirectCast(imageDTOs.ToArray(GetType(kCura.EDDS.WebAPI.FileManagerBase.FileInfoBase)), kCura.EDDS.WebAPI.FileManagerBase.FileInfoBase()), currentDocumentArtifactID)
							imageFileGuids = _fileManager.ReturnFileGuidsForOriginalImages(_fileUploader.CaseArtifactID, currentDocumentArtifactID)
						End If
					End If
				ElseIf currentDocumentArtifactID = -1 Then
					If _overwrite.ToLower = "strict" Then Throw New OverwriteStrictException(documentIdentifier)
					Dim fullTextFileGuid As String = ""					'_fileUploader.UploadTextAsFile(String.Empty, _folderID, System.Guid.NewGuid.ToString)
					currentDocumentArtifactID = CreateDocument(documentIdentifier, New kCura.EDDS.Types.FullTextBuilder)
					Dim imageDTOs As New ArrayList
					GetImagesForDocument(al, imageDTOs, New kCura.EDDS.Types.FullTextBuilder)
					_fileManager.CreateImages(_fileUploader.CaseArtifactID, DirectCast(imageDTOs.ToArray(GetType(kCura.EDDS.WebAPI.FileManagerBase.FileInfoBase)), kCura.EDDS.WebAPI.FileManagerBase.FileInfoBase()), currentDocumentArtifactID)
					imageFileGuids = _fileManager.ReturnFileGuidsForOriginalImages(_fileUploader.CaseArtifactID, currentDocumentArtifactID)
				Else
					Throw New InvalidIdentifierKeyException(documentIdentifier, _keyFieldDto.DisplayName)
				End If
				If Not _productionManager.AddDocumentToProduction(_fileUploader.CaseArtifactID, _productionArtifactID, currentDocumentArtifactID) Then
					Throw New DocumentInProductionException
				End If

				Dim fileDTOs As New ArrayList
				GetImagesForProductionDocument(al, fileDTOs)
				_fileManager.CreateProductionImages(_fileUploader.CaseArtifactID, DirectCast(fileDTOs.ToArray(GetType(kCura.EDDS.WebAPI.FileManagerBase.FileInfoBase)), kCura.EDDS.WebAPI.FileManagerBase.FileInfoBase()), currentDocumentArtifactID)

				Dim productionDocumentFiles As New ArrayList
				For c As Int32 = 0 To fileDTOs.Count - 1
					Dim productionDocumentFile As New kCura.EDDS.WebAPI.ProductionManagerBase.ProductionDocumentFileInfoBase
					productionDocumentFile.ImageGuid = CType(fileDTOs(c), kCura.EDDS.WebAPI.FileManagerBase.FileInfoBase).FileGuid
					productionDocumentFile.BatesNumber = DirectCast(al(c), String())(Columns.BatesNumber)
					productionDocumentFile.ImageSize = (New System.IO.FileInfo(GetFileLocation(DirectCast(al(c), String())))).Length
					productionDocumentFile.SourceGuid = imageFileGuids(c)
					productionDocumentFiles.Add(productionDocumentFile)
				Next
				_productionManager.CreateProductionDocumentFiles(_fileUploader.CaseArtifactID, DirectCast(productionDocumentFiles.ToArray(GetType(kCura.EDDS.WebAPI.ProductionManagerBase.ProductionDocumentFileInfoBase)), kCura.EDDS.WebAPI.ProductionManagerBase.ProductionDocumentFileInfoBase()), _productionArtifactID, currentDocumentArtifactID)
				_timekeeper.MarkEnd("ProcessDocumentForProduction")
			Catch ex As System.Exception
				_timekeeper.MarkEnd("ProcessDocumentForProduction")
				Me.LogErrorInFile(al)
				Throw
			End Try
		End Sub

#End Region

#Region "Worker Methods"

		Public Function ProcessImageLine(ByVal values As String()) As Boolean
			_timekeeper.MarkStart("ProcessImageLine")
			Try
				Dim retval As Boolean = True
				'check for existence
				If values(Columns.BatesNumber).Trim = "" Then
					Me.RaiseStatusEvent(Windows.Process.EventType.Error, String.Format("No image file specified on line."))
					retval = False
				ElseIf Not System.IO.File.Exists(Me.GetFileLocation(values)) Then
					Me.RaiseStatusEvent(Windows.Process.EventType.Error, String.Format("Image file specified ( {0} ) does not exist.", values(Columns.FileLocation)))
					retval = False
				Else
					_timekeeper.MarkStart("ProcessImageLine_ImageValidation")
					Dim validator As New kCura.ImageValidator.ImageValidator
					Dim path As String = Me.GetFileLocation(values)
					Try
						validator.ValidateImage(path)
					Catch ex As System.Exception
						Me.RaiseStatusEvent(Windows.Process.EventType.Error, String.Format("Error in '{0}': {1}", path, ex.Message))
						retval = False
					End Try
					_timekeeper.MarkEnd("ProcessImageLine_ImageValidation")
				End If
				_timekeeper.MarkEnd("ProcessImageLine")
				Return retval
			Catch ex As Exception
				_timekeeper.MarkEnd("ProcessImageLine")
				Throw
			End Try
			'check to make sure image is good
		End Function

		Private Function IsNumber(ByVal value As String) As Boolean
			Try
				Dim x As Int32 = CType(value, Int32)
			Catch ex As Exception
				Return False
			End Try
			Return True
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
			If fileLocation <> "" AndAlso fileLocation.Chars(0) = "\" AndAlso fileLocation.Chars(1) <> "\" Then
				fileLocation = "." & fileLocation
			End If
			Return fileLocation
		End Function

		Private Function GetImagesForDocument(ByVal lines As ArrayList, ByVal fileDTOs As ArrayList, ByVal fullTextBuilder As kCura.EDDS.Types.FullTextBuilder) As String()
			_timekeeper.MarkStart("GetImagesForDocument")
			Try
				Dim valueArray As String()
				For Each valueArray In lines
					GetImageForDocument(GetFileLocation(valueArray), valueArray(Columns.BatesNumber), fileDTOs, fullTextBuilder)
				Next
				If _autoNumberImages Then
					Dim file As kCura.EDDS.WebAPI.FileManagerBase.FileInfoBase
					Dim last As String
					If fileDTOs.Count > 1 Then
						Dim allSame As Boolean = True
						last = DirectCast(fileDTOs(0), kCura.EDDS.WebAPI.FileManagerBase.FileInfoBase).Identifier
						Dim i As Int32
						For i = 0 To fileDTOs.Count - 1
							file = DirectCast(fileDTOs(i), kCura.EDDS.WebAPI.FileManagerBase.FileInfoBase)
							allSame = allSame AndAlso last = file.Identifier
							If Not allSame Then Exit For
							last = file.Identifier
						Next
						If allSame Then
							Dim baseIdentifier As String = DirectCast(fileDTOs(0), kCura.EDDS.WebAPI.FileManagerBase.FileInfoBase).Identifier
							For i = 1 To fileDTOs.Count - 1
								file = DirectCast(fileDTOs(i), kCura.EDDS.WebAPI.FileManagerBase.FileInfoBase)
								file.Identifier = baseIdentifier & "_" & i.ToString.PadLeft(fileDTOs.Count.ToString.Length, "0"c)
							Next
						End If
					End If
				End If
				_timekeeper.MarkEnd("GetImagesForDocument")
			Catch ex As Exception
				_timekeeper.MarkEnd("GetImagesForDocument")
				Throw
			End Try
		End Function

		Private Function GetImagesForProductionDocument(ByVal lines As ArrayList, ByVal fileDTOs As ArrayList) As String()
			Dim valueArray As String()
			For Each valueArray In lines
				GetImageForDocument(GetFileLocation(valueArray), _productionArtifactID.ToString & "_" & valueArray(Columns.BatesNumber), fileDTOs, New kCura.EDDS.Types.FullTextBuilder)
			Next
		End Function

		Private Sub GetImageForDocument(ByVal imageFileName As String, ByVal batesNumber As String, ByVal fileDTOs As ArrayList, ByVal fullTextBuilder As kCura.EDDS.Types.FullTextBuilder)
			_timekeeper.MarkStart("GetImageForDocument")
			Try
				_timekeeper.MarkStart("GetImageForDocument_GetImageFileName")
				Dim filename As String = imageFileName.Substring(imageFileName.LastIndexOf("\") + 1)
				_timekeeper.MarkEnd("GetImageForDocument_GetImageFileName")

				_timekeeper.MarkStart("GetImageForDocument_StatusUploadUpdate")
				RaiseStatusEvent(kCura.Windows.Process.EventType.Progress, String.Format("Uploading File '{0}'.", filename))
				_timekeeper.MarkEnd("GetImageForDocument_StatusUploadUpdate")

				_timekeeper.MarkStart("GetImageForDocument_GetExtractedTextFileName")
				Dim extractedTextFileName As String = imageFileName.Substring(0, imageFileName.LastIndexOf("."c) + 1) & "txt"
				_timekeeper.MarkEnd("GetImageForDocument_GetExtractedTextFileName")

				Dim file As New kCura.EDDS.WebAPI.FileManagerBase.FileInfoBase

				If _copyFilesToRepository Then
					_timekeeper.MarkStart("GetImageForDocument_GetFileGuidFromUpload")
					file.FileGuid = _fileUploader.UploadFile(imageFileName, _folderID)
					_timekeeper.MarkEnd("GetImageForDocument_GetFileGuidFromUpload")
				Else
					_timekeeper.MarkStart("GetImageForDocument_GetFileGuidFromNewGuid")
					file.FileGuid = System.Guid.NewGuid.ToString
					_timekeeper.MarkEnd("GetImageForDocument_GetFileGuidFromNewGuid")
				End If
				file.FileName = filename
				file.Identifier = batesNumber
				If _copyFilesToRepository Then
					_timekeeper.MarkStart("GetImageForDocument_GetFileLocationFromGuid")
					file.Location = _repositoryPath & file.FileGuid
					_timekeeper.MarkEnd("GetImageForDocument_GetFileLocationFromGuid")
				Else
					_timekeeper.MarkStart("GetImageForDocument_GetFileLocationFromNothingNoUpload")
					file.Location = imageFileName
					_timekeeper.MarkEnd("GetImageForDocument_GetFileLocationFromNothingNoUpload")
				End If

				_timekeeper.MarkStart("GetImageForDocument_AddNewDtoToArraylist")
				fileDTOs.Add(file)
				_timekeeper.MarkEnd("GetImageForDocument_AddNewDtoToArraylist")

				_timekeeper.MarkStart("GetImageForDocument_FullText")
				If _replaceFullText AndAlso System.IO.File.Exists(extractedTextFileName) Then
					Dim sr As New System.IO.StreamReader(extractedTextFileName, System.Text.Encoding.Default, True)
					fullTextBuilder.AppendPage(sr.ReadToEnd)
					sr.Close()
				Else
					If _replaceFullText AndAlso Not System.IO.File.Exists(extractedTextFileName) Then
						RaiseStatusEvent(kCura.Windows.Process.EventType.Warning, String.Format("File '{0}' not found.  No text updated.", extractedTextFileName))
					End If
				End If
				_timekeeper.MarkEnd("GetImageForDocument_FullText")

				_timekeeper.MarkEnd("GetImageForDocument")
			Catch ex As Exception
				_timekeeper.MarkEnd("GetImageForDocument")
				Throw
			End Try
		End Sub

		Private Function CreateDocument(ByVal identifier As String, ByVal fullTextBuilder As kCura.EDDS.Types.FullTextBuilder) As Int32
			_timekeeper.MarkStart("GetImageForDocument")
			Try
				Dim fieldID As Int32
				Dim encoder As New System.Text.ASCIIEncoding
				Try
					If fullTextBuilder.FullTextLength > kCura.WinEDDS.Service.Settings.MAX_STRING_FIELD_LENGTH Then
						fullTextBuilder.FilePointer = _textUploader.UploadTextAsFile(fullTextBuilder.FullTextString, _folderID, System.Guid.NewGuid.ToString)
						fullTextBuilder.FullText = ""
					End If
					Return _docManager.CreateEmptyDocument(_fileUploader.CaseArtifactID, _folderID, encoder.GetBytes(identifier), _selectedIdentifierField, fullTextBuilder)
				Catch ex As System.Exception
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Throw New CreateDocumentException(ex)
					Else
						Throw
					End If
				End Try
				_timekeeper.MarkEnd("GetImageForDocument")

			Catch ex As Exception
				_timekeeper.MarkEnd("GetImageForDocument")
				Throw
			End Try
		End Function

#End Region

#Region "Events and Event Handling"

		Public Event FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception)
		Public Event StatusMessage(ByVal args As kCura.Windows.Process.StatusEventArgs)

		Private Sub RaiseFatalError(ByVal ex As System.Exception)
			RaiseEvent FatalErrorEvent("Error processing line: " + CurrentLineNumber.ToString, ex)
		End Sub

		Private Sub RaiseStatusEvent(ByVal et As kCura.Windows.Process.EventType, ByVal line As String, Optional ByVal lineOffset As Int32 = 0)
			RaiseEvent StatusMessage(New kCura.Windows.Process.StatusEventArgs(et, Me.CurrentLineNumber - lineOffset, _fileLineCount, line))
		End Sub

		Private Sub _processObserver_CancelImport(ByVal processID As System.Guid) Handles _processController.HaltProcessEvent
			_continue = False
		End Sub

#End Region

#Region "Exceptions - Errors"
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
			Public Sub New(ByVal docIdentifier As String)
				MyBase.New(String.Format("Document '{0}' exists - upload aborted.", docIdentifier))
			End Sub
		End Class

		Public Class OverwriteStrictException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal docIdentifier As String)
				MyBase.New(String.Format("Document '{0}' does not exist - upload aborted.", docIdentifier))
			End Sub
		End Class

		Public Class ImageCountMismatchException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New()
				MyBase.New("Production and Document image counts don't match - upload aborted.")
			End Sub
		End Class

		Public Class DocumentInProductionException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New()
				MyBase.New("Document is already in specified production - upload aborted.")
			End Sub
		End Class

		Public Class ProductionOverwriteException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal identifier As String)
				MyBase.New(String.Format("Document '{0}' belongs to one or more productions.  Document skipped.", identifier))
			End Sub
		End Class

		Public Class RedactionOverwriteException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal identifier As String)
				MyBase.New(String.Format("The one or more images for document '{0}' have redactions.  Document skipped.", identifier))
			End Sub
		End Class

		Public Class InvalidIdentifierKeyException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal identifier As String, ByVal fieldName As String)
				MyBase.New(String.Format("More than one document contains '{0}' as its '{1}' value.  Document skipped.", identifier, fieldName))
			End Sub
		End Class


#End Region

#Region "Exceptions - Fatal"
		Public Class InvalidBatesFormatException
			Inherits System.Exception
			Public Sub New(ByVal batesNumber As String, ByVal productionName As String, ByVal batesPrefix As String, ByVal batesSuffix As String, ByVal batesFormat As String)
				MyBase.New(String.Format("The image with bates number {0} cannot be imported into production '{1}' because the prefix and/or suffix do not match the values specified in the production. Expected prefix: '{2}'. Expected suffix: '{3}'. Expected format: '{4}'.", batesNumber, productionName, batesPrefix, batesSuffix, batesFormat))
			End Sub
		End Class
#End Region

	End Class
End Namespace