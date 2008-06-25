Namespace kCura.WinEDDS
	Public Class LoadFileImporter

		Inherits kCura.WinEDDS.LoadFileBase


#Region "Members"
		Private _overwrite As String
		Private WithEvents _uploader As kCura.WinEDDS.FileUploader
		Private _textUploader As kCura.WinEDDS.FileUploader
		Private _path As String
		Private _pathIsSet As Boolean = False
		Private _selectedIdentifier As DocumentField
		Private _docFieldCollection As DocumentFieldCollection
		Private _parentFolderDTO As kCura.EDDS.WebAPI.FolderManagerBase.Folder
		Private _recordCount As Int64 = -1
		Private _extractFullTextFromNative As Boolean
		Private _allFields As kCura.EDDS.WebAPI.DocumentManagerBase.Field()
		Private _fieldsForCreate As kCura.EDDS.WebAPI.DocumentManagerBase.Field()
		Protected Shared _continue As Boolean
		Protected _processedDocumentIdentifiers As Collections.Specialized.NameValueCollection
		Protected WithEvents _processController As kCura.Windows.Process.Controller
		Protected _offset As Int32 = 0
		Protected _firstTimeThrough As Boolean
		Private _docsToAdd As ArrayList
		Private _docsToUpdate As ArrayList
		Private _filesToAdd As ArrayList
		Private _docsToProcess As MetaDocQueue
		Private _timeKeeper As TimeKeeper
		Private _killWorker As Boolean
		Private _workerRunning As Boolean
		Private WithEvents _lineCounter As kCura.Utility.File.LineCounter
		Private _genericTimestamp As System.DateTime
		Private _number As Int64 = 0
		Private _destinationFolderColumnIndex As Int32 = -1
		Private _folderCache As FolderCache
		Private _errorLogFileName As String = ""
		Private _errorLogWriter As System.IO.StreamWriter
		Private _fullTextField As kCura.EDDS.WebAPI.DocumentManagerBase.Field
		Private _defaultDestinationFolderPath As String = ""
		Private _defaultTextFolderPath As String = ""
		Private _copyFileToRepository As Boolean
		Private _oixFileLookup As System.Collections.Specialized.HybridDictionary
		Private _fieldArtifactIds As Int32()

#End Region

#Region "Accessors"

		Friend WriteOnly Property FilePath() As String
			Set(ByVal value As String)
				If System.IO.File.Exists(value) Then
					_path = value
					_pathIsSet = True
				End If
			End Set
		End Property

		Public ReadOnly Property AllDocumentFields() As kCura.EDDS.WebAPI.DocumentManagerBase.Field()
			Get
				If _allFields Is Nothing Then
					_allFields = _fieldQuery.RetrieveAllAsArray(_caseArtifactID, True)
				End If
				Dim field As kCura.EDDS.WebAPI.DocumentManagerBase.Field
				For Each field In _allFields
					field.Value = Nothing
					field.FieldCategory = CType(field.FieldCategoryID, kCura.EDDS.WebAPI.DocumentManagerBase.FieldCategory)
				Next
				Return _allFields
			End Get
		End Property

		Public ReadOnly Property DocumentFieldsForCreate() As kCura.EDDS.WebAPI.DocumentManagerBase.Field()
			Get
				If _fieldsForCreate Is Nothing Then
					Dim fieldsForCreate As New System.Collections.ArrayList
					For Each field As kCura.EDDS.WebAPI.DocumentManagerBase.Field In Me.AllDocumentFields
						If System.Array.IndexOf(_fieldArtifactIds, field.ArtifactID) <> -1 OrElse _
						field.FieldCategory = EDDS.WebAPI.DocumentManagerBase.FieldCategory.Relational Then
							fieldsForCreate.Add(field)
						End If
					Next
					_fieldsForCreate = DirectCast(fieldsForCreate.ToArray(GetType(kCura.EDDS.WebAPI.DocumentManagerBase.Field)), kCura.EDDS.WebAPI.DocumentManagerBase.Field())
				End If
				Return _fieldsForCreate
			End Get
		End Property

		Public ReadOnly Property FileInfoField() As kCura.EDDS.WebAPI.DocumentManagerBase.Field
			Get
				For Each field As kCura.EDDS.WebAPI.DocumentManagerBase.Field In Me.AllDocumentFields
					If field.FieldCategoryID = kCura.DynamicFields.Types.FieldCategory.FileInfo Then Return field
				Next
			End Get
		End Property

		Public ReadOnly Property FullTextField() As kCura.EDDS.WebAPI.DocumentManagerBase.Field
			Get
				For Each field As kCura.EDDS.WebAPI.DocumentManagerBase.Field In Me.AllDocumentFields
					If field.FieldCategory = EDDS.WebAPI.DocumentManagerBase.FieldCategory.FullText Then
						Return field
					End If
				Next
			End Get
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
#End Region

#Region "Constructors"

		Public Sub New(ByVal args As LoadFile, ByVal processController As kCura.Windows.Process.Controller, ByVal timeZoneOffset As Int32, ByVal initializeUploaders As Boolean)
			Me.New(args, processController, timeZoneOffset, True, initializeUploaders)
		End Sub

		Public Sub New(ByVal args As LoadFile, ByVal processController As kCura.Windows.Process.Controller, ByVal timeZoneOffset As Int32, ByVal autoDetect As Boolean, ByVal initializeUploaders As Boolean)
			MyBase.New(args, timeZoneOffset, autoDetect)
			_overwrite = args.OverwriteDestination
			If args.CopyFilesToDocumentRepository Then
				_defaultDestinationFolderPath = args.SelectedCasePath & "EDDS" & args.CaseInfo.ArtifactID & "\"
			End If
			_defaultTextFolderPath = args.CaseDefaultPath & "EDDS" & args.CaseInfo.ArtifactID & "\"
			If initializeUploaders Then
				_uploader = New kCura.WinEDDS.FileUploader(args.Credentials, args.CaseInfo.ArtifactID, _defaultDestinationFolderPath, args.CookieContainer)
				_textUploader = New kCura.WinEDDS.FileUploader(args.Credentials, args.CaseInfo.ArtifactID, _defaultTextFolderPath, args.CookieContainer)
			End If
			_extractFullTextFromNative = args.ExtractFullTextFromNativeFile
			_selectedIdentifier = args.SelectedIdentifierField
			_copyFileToRepository = args.CopyFilesToDocumentRepository
			_docFieldCollection = New DocumentFieldCollection(args.FieldMap.DocumentFields)
			If autoDetect Then _parentFolderDTO = _foldermanager.Read(args.CaseInfo.ArtifactID, args.CaseInfo.RootFolderID)
			_processController = processController
			_continue = True
			_firstTimeThrough = True
		End Sub

#End Region

#Region "Utility"

		Public Function GetColumnNames(ByVal path As String) As String()
			If _sourceFileEncoding Is Nothing Then _sourceFileEncoding = System.Text.Encoding.Default
			reader = New System.IO.StreamReader(path, _sourceFileEncoding, True)
			Dim columnNames As String() = GetLine
			If Not _firstLineContainsColumnNames Then
				Dim i As Int32
				For i = 0 To columnNames.Length - 1
					columnNames(i) = "Column (" & (i + 1).ToString & ")"
				Next
			Else
				Dim i As Int32
				For i = 0 To columnNames.Length - 1
					columnNames(i) &= String.Format(" ({0})", i + 1)
				Next
			End If
			reader.Close()
			Me.ResetLineCounter()
			Return columnNames
		End Function

#End Region

#Region "Main"

		Public Overloads Sub ReadFile()
			ReadFile(_path)
		End Sub

		Public Overloads Overrides Function ReadFile(ByVal path As String) As Object
			Dim line As String()
			Try
				RaiseEvent StartFileImport()
				Dim markStart As DateTime = DateTime.Now
				InitializeMembers(path)
				StartMassProcessor()
				While _continue AndAlso Not HasReachedEOF
					Try
						line = Me.GetLine
						If line.Length <> _columnHeaders.Length Then
							Throw New ColumnCountMismatchException(Me.CurrentLineNumber, _columnHeaders.Length, line.Length)
						End If
						_processedDocumentIdentifiers.Add(ManageDocument(line), CurrentLineNumber.ToString)
					Catch ex As LoadFileBase.CodeCreationException
						_continue = False
						WriteFatalError(Me.CurrentLineNumber, ex, line)
					Catch ex As kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
						WriteError(ex.Message, line)
					Catch ex As System.Exception
						WriteFatalError(Me.CurrentLineNumber, ex, line)
					End Try
				End While
				StopMassProcessor()
				While _workerRunning
					System.Threading.Thread.CurrentThread.Join(1000)
				End While
				RaiseEvent EndFileImport()
				WriteEndImport("Finish")
				Me.Close()
				Try
					_errorLogWriter.Close()
				Catch ex As System.Exception
				End Try
				_timeKeeper.Add("Total", DateTime.Now.Subtract(markStart).TotalMilliseconds)
				'Dim filenameFolder As String = "C:\UploadFileMetrics\"
				'Dim now As System.DateTime = System.DateTime.Now
				'Dim filename As String = String.Format("{0}{1}{2}_{3}{4}{5}.csv", now.Year, now.Month.ToString.PadLeft(2, "0"c), now.Day.ToString.PadLeft(2, "0"c), now.Hour.ToString.PadLeft(2, "0"c), now.Minute.ToString.PadLeft(2, "0"c), now.Second.ToString.PadLeft(2, "0"c))
				'Dim sw As New System.IO.StreamWriter(filenameFolder & filename)
				'sw.Write(_timeKeeper.ToCollectionString())
				'sw.Flush()
				'sw.Close()
				Return True
			Catch ex As System.Exception
				WriteFatalError(Me.CurrentLineNumber, ex, line)
			End Try
		End Function

		Private Sub LogErrorLine(ByVal values As String())
			If values Is Nothing Then Exit Sub
			If _errorLogFileName = "" Then
				_errorLogFileName = System.IO.Path.GetTempFileName()
				_errorLogWriter = New System.IO.StreamWriter(_errorLogFileName, False, _sourceFileEncoding)
				_errorLogWriter.WriteLine(Me.ToDelimetedLine(_columnHeaders))
			End If
			_errorLogWriter.WriteLine(Me.ToDelimetedLine(values))
		End Sub

		Private Function ToDelimetedLine(ByVal values As String()) As String
			Dim sb As New System.Text.StringBuilder
			Dim value As String
			For Each value In values
				sb.Append(Me.Bound & value & Me.Bound & Me.Delimiter)
			Next
			sb.Remove(sb.Length - 1, 1)
			Return sb.ToString
		End Function

		Private Sub InitializeMembers(ByVal path As String)
			Me.InitializeLineCounter(path)
			Me.InitializeFolderManagement()
			Me.InitializeFieldIdList()
			RaiseEvent StatusMessage(New kCura.Windows.Process.StatusEventArgs(Windows.Process.EventType.ResetStartTime, 0, _recordCount, "Reset time for import rolling average"))
		End Sub

		Private Sub InitializeLineCounter(ByVal path As String)
			_lineCounter = New kCura.Utility.File.LineCounter
			_lineCounter.Path = path
			_lineCounter.CountLines(_sourceFileEncoding, New kCura.Utility.File.LineCounter.LineCounterArgs(Me.Bound, Me.Delimiter))
		End Sub

		Private Sub InitializeFolderManagement()
			If _createFolderStructure Then
				_folderCache = New FolderCache(_folderManager, _folderID, _caseArtifactID)
				Dim openParenIndex As Int32 = _destinationFolder.LastIndexOf("("c) + 1
				Dim closeParenIndex As Int32 = _destinationFolder.LastIndexOf(")"c)
				_destinationFolderColumnIndex = Int32.Parse(_destinationFolder.Substring(openParenIndex, closeParenIndex - openParenIndex)) - 1
			End If
		End Sub

		Private Sub InitializeFieldIdList()
			Dim fieldIdList As New System.Collections.ArrayList
			For Each item As LoadFileFieldMap.LoadFileFieldMapItem In _fieldMap
				'If item.DocumentField.FieldCategoryID <> kCura.DynamicFields.Types.FieldCategory.FullText Then fieldIdList.Add(item.DocumentField.FieldID)
				fieldIdList.Add(item.DocumentField.FieldID)
			Next
			fieldIdList.Add(Me.FileInfoField.ArtifactID)
			_fieldArtifactIds = DirectCast(fieldIdList.ToArray(GetType(Int32)), Int32())
		End Sub

		Private Function ManageDocument(ByVal values As String()) As String
			If _docsToProcess.IsFull Then
				While Not _docsToProcess.CanAdd
					If _continue Then
						System.Threading.Thread.CurrentThread.Join(1000)
					Else
						Exit Function
					End If
				End While
			End If
			Dim markStart As DateTime = DateTime.Now
			Dim filename As String
			Dim fileGuid As String = String.Empty
			Dim uploadFile As Boolean = (_filePathColumnIndex <> -1) AndAlso _uploadFiles
			Dim fileExists As Boolean
			Dim fieldCollection As New DocumentFieldCollection
			Dim identityValue As String = String.Empty
			Dim documentArtifactID As Int32
			Dim markUploadStart As DateTime = DateTime.Now
			Dim parentFolderID As Int32
			Dim md5hash As String = ""
			Dim fullFilePath As String = ""
			Dim isSupportedFileType As Boolean
			Dim oixFileIdData As OI.FileID.FileIDData
			If uploadFile Then
				filename = values(_filePathColumnIndex)
				If filename.Length > 1 AndAlso filename.Chars(0) = "\" AndAlso filename.Chars(1) <> "\" Then
					filename = "." & filename
				End If

				fileExists = System.IO.File.Exists(filename)
				If filename <> String.Empty AndAlso Not fileExists Then Throw New InvalidFilenameException(filename)
				If fileExists Then
					Dim now As DateTime = DateTime.Now
					If New IO.FileInfo(filename).Length = 0 Then Throw New EmptyNativeFileException(filename)
					oixFileIdData = kCura.OI.FileID.Manager.Instance.GetFileIDDataByFilePath(filename)
					If _copyFileToRepository Then
						fileGuid = _uploader.UploadFile(filename, _caseArtifactID)
					Else
						fileGuid = System.Guid.NewGuid.ToString
					End If
					If fileGuid = "" Then Throw New FileUploadFailedException
					If _extractMd5Hash Then
						md5hash = kCura.Utility.File.GenerateMD5HashForFile(filename)
					End If
					fullFilePath = filename
					filename = filename.Substring(filename.LastIndexOf("\") + 1)
					WriteStatusLine(Windows.Process.EventType.Status, String.Format("End upload file. ({0}ms)", DateTime.op_Subtraction(DateTime.Now, now).Milliseconds))
				End If
			End If
			If _createFolderStructure Then
				parentFolderID = _folderCache.FolderID(Me.CleanDestinationFolderPath(values(_destinationFolderColumnIndex)))
			Else
				parentFolderID = _folderID
			End If
			_timeKeeper.Add("UploadFile", DateTime.Now.Subtract(markUploadStart).TotalMilliseconds)
			Dim markPrepareFields As DateTime = DateTime.Now
			identityValue = PrepareFieldCollectionAndExtractIdentityValue(fieldCollection, values)
			_timeKeeper.Add("PrepareFields", DateTime.Now.Subtract(markPrepareFields).TotalMilliseconds)
			If identityValue = String.Empty Then
				Throw New IdentityValueNotSetException
			ElseIf Not _processedDocumentIdentifiers(identityValue) Is Nothing Then
				Throw New IdentifierOverlapException(identityValue, _processedDocumentIdentifiers(identityValue))
			End If
			Dim metadoc As New MetaDocument(fileGuid, identityValue, fieldCollection, fileExists AndAlso uploadFile AndAlso (fileGuid <> String.Empty OrElse Not _copyFileToRepository), filename, fullFilePath, uploadFile, CurrentLineNumber, parentFolderID, md5hash, values, oixFileIdData)
			_docsToProcess.Push(metadoc)
			Return identityValue
		End Function

		Private Function CleanDestinationFolderPath(ByVal path As String) As String
			While path.IndexOf(".\") <> -1
				path = path.Replace(".\", "\")
			End While
			While path.IndexOf("\\") <> -1
				path = path.Replace("\\", "\")
			End While
			path = path.Replace(":", "_")
			If Not path.Length = 0 Then
				If path.Chars(0) <> "\"c Then
					path = "\" & path
				End If
			End If
			path = path.TrimEnd(New Char() {"\"c})
			If path = "" Then path = "\"
			Return path
		End Function

#End Region

#Region "Thread Management"

		Private Sub MassProcessWorker()
			_workerRunning = True
			While (Not _killWorker OrElse _docsToProcess.Length > 0) AndAlso _continue
				If Not _docsToProcess Is Nothing AndAlso _docsToProcess.Length > 0 Then
					Dim metaDoc As MetaDocument = _docsToProcess.Front
					If Not metaDoc Is Nothing Then
						metaDoc = _docsToProcess.Pop()
						GC.Collect(3)
						GC.WaitForPendingFinalizers()
						GC.Collect(3)
						Try
							ManageDocumentMetaData(metaDoc)
						Catch ex As System.Exception
							WriteFatalError(CurrentLineNumber, ex, metaDoc.SourceLine)
						End Try
					End If
				Else
					System.Threading.Thread.CurrentThread.Join(1000)
				End If
			End While
			_workerRunning = False
		End Sub

		Private Sub StartMassProcessor()
			_killWorker = False
			Dim thread As New System.Threading.Thread(AddressOf MassProcessWorker)
			thread.Start()
		End Sub

		Private Sub StopMassProcessor()
			_killWorker = True
		End Sub

#End Region

#Region "WebService Calls"

		Private Function ReadDocumentInfo(ByVal identityValue As String) As kCura.EDDS.WebAPI.DocumentManagerBase.FullDocumentInfo
			Try
				Return _documentManager.ReadFromIdentifierWithFileList(_caseArtifactID, _selectedIdentifier.FieldName, identityValue, _fieldArtifactIds)
			Catch ex As System.Exception
				If kCura.WinEDDS.Config.UsesWebAPI Then
					If TypeOf ex Is System.Web.Services.Protocols.SoapException Then
						If Not ex.InnerException Is Nothing Then
							ex = ex.InnerException
						End If
					End If
					Throw New DocumentReadException(ex)
				Else
					Throw
				End If
			End Try
		End Function

		Private Sub ManageDocumentMetaData(ByVal metaDoc As MetaDocument)
			_number += 1
			Dim sw As System.IO.StreamWriter
			Try
				Dim doc As kCura.EDDS.WebAPI.DocumentManagerBase.Document
				Dim documentArtifactID As Int32
				Dim markReadDoc As DateTime = DateTime.Now
				Dim files As kCura.EDDS.WebAPI.DocumentManagerBase.File()
				Select Case _overwrite.ToLower
					Case "strict"
						With Me.ReadDocumentInfo(metaDoc.IdentityValue)
							doc = .DocumentDTO
							files = .FileList
							If doc Is Nothing Then
								Throw New IdentityValueNotFoundException(metaDoc.IdentityValue)
							End If
						End With
					Case "append"
						With Me.ReadDocumentInfo(metaDoc.IdentityValue)
							doc = .DocumentDTO
							files = .FileList
						End With
				End Select
				_timeKeeper.Add("ReadUpload", DateTime.Now.Subtract(markReadDoc).TotalMilliseconds)
				markReadDoc = DateTime.Now
				If doc Is Nothing Then
					documentArtifactID = CreateDocument(metaDoc, _extractFullTextFromNative)
				Else
					documentArtifactID = UpdateDocument(doc, metaDoc, _extractFullTextFromNative, files)
				End If
				Dim o As New Object
				_timeKeeper.Add("Manage", DateTime.Now.Subtract(markReadDoc).TotalMilliseconds)
			Catch ex As kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
				If ex.GetBaseException.Message.IndexOf("Cannot insert duplicate key row") > -1 Then
					WriteError("A record with the selected identifier already exists.", metaDoc.SourceLine)
				Else
					WriteError(ex.Message, metaDoc.SourceLine)
				End If
			Catch ex As System.Exception
				WriteFatalError(metaDoc.LineNumber, ex, metaDoc.SourceLine)
			End Try
			WriteStatusLine(Windows.Process.EventType.Progress, String.Format("Document '{0}' processed.", metaDoc.IdentityValue), metaDoc.LineNumber)
		End Sub

		Private Function GetDocumentDtoForCreate(ByVal parentFolderId As Int32, ByVal extractText As Boolean) As kCura.EDDS.WebAPI.DocumentManagerBase.Document
			Dim documentDTO As New kCura.EDDS.WebAPI.DocumentManagerBase.Document
			documentDTO.Fields = Me.DocumentFieldsForCreate
			documentDTO.ParentArtifactID = New NullableTypes.NullableInt32(parentFolderId)
			documentDTO.ContainerID = _parentFolderDTO.ContainerID
			documentDTO.AccessControlListIsInherited = True
			documentDTO.AccessControlListID = _parentFolderDTO.AccessControlListID
			documentDTO.DocumentAgentFlags = New kCura.EDDS.WebAPI.DocumentManagerBase.DocumentAgentFlags
			documentDTO.DocumentAgentFlags.UpdateFullText = extractText
			documentDTO.DocumentAgentFlags.IndexStatus = kCura.EDDS.Types.IndexStatus.IndexLowPriority
			Return documentDTO
		End Function

		Private Function CreateDocument(ByVal mdoc As MetaDocument, ByVal extractText As Boolean) As Int32
			Return CreateDocument(mdoc.FieldCollection, mdoc.IdentityValue, mdoc.FileGuid <> String.Empty AndAlso extractText, mdoc.Filename, mdoc.FileGuid, mdoc)
		End Function

		Private Function CreateDocument(ByVal fieldCollection As DocumentFieldCollection, ByVal identityValue As String, ByVal extractText As Boolean, ByVal filename As String, ByVal fileguid As String, ByVal mdoc As MetaDocument) As Int32
			Dim documentDTO As kCura.EDDS.WebAPI.DocumentManagerBase.Document = Me.GetDocumentDtoForCreate(mdoc.ParentFolderID, extractText)
			Dim files As kCura.EDDS.WebAPI.DocumentManagerBase.File()
			Dim now As System.DateTime = System.DateTime.Now
			SetFieldValues(documentDTO, fieldCollection)
			If mdoc.Md5Hash <> "" Then
				Me.SetMd5HashValue(mdoc.Md5Hash, documentDTO)
			End If
			ManageRelationalFields(documentDTO)
			Dim field As kCura.EDDS.WebAPI.DocumentManagerBase.Field
			If mdoc.UploadFile And mdoc.IndexFileInDB Then
				Me.SetFileIdDataFields(documentDTO, mdoc.FileIdData)
				Dim fileDTO As kCura.EDDS.WebAPI.DocumentManagerBase.File = CreateFileDTO(filename, fileguid, _defaultDestinationFolderPath, mdoc.FullFilePath)
				files = New kCura.EDDS.WebApi.DocumentManagerBase.File() {fileDTO}
				'documentDTO.HasNative = True
			End If

			Try
				WriteStatusLine(Windows.Process.EventType.Status, String.Format("Creating document '{0}' in database.", identityValue))
				Return _documentManager.Create(_uploader.CaseArtifactID, documentDTO, files)
			Catch ex As System.Exception
				If kCura.WinEDDS.Config.UsesWebAPI Then
					If ex.ToString.IndexOf("NeedToReLoginException") <> -1 Then
						Throw
					Else
						Throw New DocumentDomainException(ex)
					End If
				Else
					Throw
				End If
			End Try
		End Function

		Private Function UpdateDocument(ByVal docDTO As kCura.EDDS.WebAPI.DocumentManagerBase.Document, ByVal mdoc As MetaDocument, ByVal extractText As Boolean, ByVal files As kCura.EDDS.WebAPI.DocumentManagerBase.File()) As Int32
			Return UpdateDocument(docDTO, mdoc.FieldCollection, mdoc.IdentityValue, mdoc.UploadFile AndAlso mdoc.FileGuid <> String.Empty, mdoc.FileGuid <> String.Empty AndAlso extractText, mdoc.Filename, mdoc.FileGuid, mdoc, files)
		End Function

		Private Function UpdateDocument(ByVal docDTO As kCura.EDDS.WebAPI.DocumentManagerBase.Document, ByVal fieldCollection As DocumentFieldCollection, ByVal identityValue As String, ByVal uploadFile As Boolean, ByVal extractText As Boolean, ByVal fileName As String, ByVal fileGuid As String, ByVal mdoc As MetaDocument, ByVal files As kCura.EDDS.WebAPI.DocumentManagerBase.File()) As Int32
			If Not _overwrite.ToLower = "none" Then
				WriteStatusLine(Windows.Process.EventType.Status, String.Format("Updating document '{0}' in database.", identityValue))
				docDTO.DocumentAgentFlags.UpdateFullText = extractText
				docDTO.DocumentAgentFlags.IndexStatus = kCura.EDDS.Types.IndexStatus.IndexLowPriority
				Dim al As New System.Collections.ArrayList
				al.AddRange(docDTO.Fields)
				al.Add(Me.FullTextField)
				docDTO.Fields = DirectCast(al.ToArray(GetType(kCura.EDDS.WebAPI.DocumentManagerBase.Field)), kCura.EDDS.WebAPI.DocumentManagerBase.Field())
				SetFieldValues(docDTO, fieldCollection)
				Dim fileList As New ArrayList
				If uploadFile OrElse mdoc.IndexFileInDB Then
					Dim oldFile As kCura.EDDS.WebAPI.DocumentManagerBase.File
					Dim hasOldFile As Boolean = False
					If Not files Is Nothing Then
						For Each oldFile In files
							If oldFile.Type = kCura.EDDS.Types.FileType.Native Then
								hasOldFile = True
								Exit For
							End If
						Next
					End If
					Dim fileDTO As kCura.EDDS.WebAPI.DocumentManagerBase.File = CreateFileDTO(fileName, fileGuid, _defaultDestinationFolderPath, mdoc.FullFilePath)
					If Not hasOldFile Then
						fileList.Add(fileDTO)
					Else
						fileList.Add(fileDTO)
						fileList.Add(oldFile)
					End If
					Me.SetFileIdDataFields(docDTO, mdoc.FileIdData)
				End If
				Dim fullTextFileDTO As kCura.EDDS.WebAPI.DocumentManagerBase.File
				If Not files Is Nothing Then
					For Each fullTextFileDTO In files
						If fullTextFileDTO.Type = 2 Then
							Exit For
						End If
					Next
				End If
				If Not fullTextFileDTO Is Nothing AndAlso fullTextFileDTO.Type = 2 AndAlso Not extractText Then
					fileList.Add(fullTextFileDTO)
				End If
				If fileList.Count = 0 Then
					files = Nothing
				Else
					files = DirectCast(fileList.ToArray(GetType(kCura.EDDS.WebAPI.DocumentManagerBase.File)), kCura.EDDS.WebAPI.DocumentManagerBase.File())
				End If
				If mdoc.Md5Hash <> "" Then
					Me.SetMd5HashValue(mdoc.Md5Hash, docDTO)
				End If
				ManageRelationalFields(docDTO)
				Try
					_documentManager.Update(_uploader.CaseArtifactID, docDTO, files)
				Catch ex As System.Exception
					If kCura.WinEDDS.Config.UsesWebAPI Then
						If ex.ToString.IndexOf("NeedToReLoginException") <> -1 Then
							Throw
						Else
							Throw New DocumentDomainException(ex)
						End If
					Else
						Throw
					End If
				End Try
				Return docDTO.ArtifactID
			Else
				Throw New DocumentOverwriteException
			End If
		End Function

		Private Function CreateFileDTO(ByVal filename As String, ByVal fileguid As String, ByVal documentDirectory As String, ByVal fullFilePath As String) As kCura.EDDS.WebAPI.DocumentManagerBase.File
			Dim fileDTO As New kCura.EDDS.WebAPI.DocumentManagerBase.File
			fileDTO.DocumentArtifactID = 0
			fileDTO.Filename = filename
			fileDTO.Guid = fileguid
			fileDTO.Order = 0
			fileDTO.Type = kCura.EDDS.Types.FileType.Native
			If _copyFileToRepository Then
				fileDTO.Location = documentDirectory & fileguid
			Else
				fileDTO.Location = fullFilePath
			End If
			Return fileDTO
		End Function

		Private Sub SetFileIdDataFields(ByVal document As kCura.EDDS.WebAPI.DocumentManagerBase.Document, ByVal oixFileIdData As OI.FileID.FileIDData)
			Dim isSupported As Boolean = Me.IsSupportedRelativityFileType(oixFileIdData)
			For Each field As kCura.EDDS.WebAPI.DocumentManagerBase.Field In document.Fields
				If field.DisplayName = "Supported By Viewer" Then field.Value = isSupported.ToString
				If field.DisplayName = "Relativity Native Type" Then field.Value = System.Text.Encoding.Unicode.GetBytes(oixFileIdData.FileType)
			Next
			document.HasNative = True
		End Sub

		Private Function IsSupportedRelativityFileType(ByVal fileData As OI.FileID.FileIDData) As Boolean
			If fileData Is Nothing Then Return False
			If _oixFileLookup Is Nothing Then
				_oixFileLookup = New System.Collections.Specialized.HybridDictionary
				For Each id As Int32 In _documentManager.RetrieveAllUnsupportedOiFileIds
					_oixFileLookup.Add(id, id)
				Next
			End If
			Return Not _oixFileLookup.Contains(fileData.FileID)
		End Function

#End Region

#Region "Field Preparation"

		Private Function PrepareFieldCollectionAndExtractIdentityValue(ByVal fieldCollection As DocumentFieldCollection, ByVal values As String()) As String
			Dim item As LoadFileFieldMap.LoadFileFieldMapItem
			Dim identityValue As String = String.Empty
			Dim docfield As DocumentField
			For Each item In _fieldmap
				If _firstTimeThrough Then
					If item.DocumentField Is Nothing Then
						WriteStatusLine(Windows.Process.EventType.Warning, String.Format("File column '{0}' will be unmapped", item.NativeFileColumnIndex + 1), 0)
					End If
					If item.NativeFileColumnIndex = -1 Then
						WriteStatusLine(Windows.Process.EventType.Warning, String.Format("Field '{0}' will be unmapped", item.DocumentField.FieldName), 0)
					End If
				End If
				If Not item.DocumentField Is Nothing Then
					docfield = New DocumentField(item.DocumentField)
					MyBase.SetFieldValue(docfield, values, item.NativeFileColumnIndex)
					If docfield.FieldName = _selectedIdentifier.FieldName Then
						identityValue = docfield.Value
					End If
					fieldCollection.Add(docfield)
				End If
			Next
			If Not fieldCollection.GroupIdentifier Is Nothing AndAlso fieldCollection.GroupIdentifier.Value = "" Then
				fieldCollection.GroupIdentifier.Value = identityValue
			End If
			_firstTimeThrough = False
			Return identityValue
		End Function

#End Region

#Region "Field Value Manipulation"

		Private Sub SetFieldValues(ByVal documentDTO As kCura.EDDS.WebAPI.DocumentManagerBase.Document, ByVal selectedFields As DocumentFieldCollection)
			Dim fieldDTO As kCura.EDDS.WebAPI.DocumentManagerBase.Field
			Dim docField As DocumentField
			Dim encoder As New System.Text.UnicodeEncoding
			Dim value As String
			Dim removeFullTextField As Boolean = False
			For Each fieldDTO In documentDTO.Fields
				docField = selectedFields.Item(fieldDTO.ArtifactID)
				If docField Is Nothing Then
					If fieldDTO.Value Is Nothing Then
						If fieldDTO.FieldCategory = EDDS.WebAPI.DocumentManagerBase.FieldCategory.FullText Then
							removeFullTextField = True
						End If
						Select Case fieldDTO.FieldType
							Case kCura.EDDS.WebAPI.DocumentManagerBase.FieldType.Text, kCura.EDDS.WebAPI.DocumentManagerBase.FieldType.Varchar
								fieldDTO.Value = encoder.GetBytes(String.Empty)
							Case Else
								fieldDTO.Value = String.Empty
						End Select
					End If
				Else
					Select Case fieldDTO.FieldType
						Case kCura.EDDS.WebAPI.DocumentManagerBase.FieldType.MultiCode, EDDS.WebAPI.DocumentManagerBase.FieldType.Code
							SetMultiCode(fieldDTO, docField)
						Case EDDS.WebAPI.DocumentManagerBase.FieldType.Code
							fieldDTO.Value = docField.Value
						Case EDDS.WebAPI.DocumentManagerBase.FieldType.Text, EDDS.WebAPI.DocumentManagerBase.FieldType.Varchar
							If fieldDTO.FieldCategory = EDDS.WebAPI.DocumentManagerBase.FieldCategory.FullText Then
								If _fullTextColumnMapsToFileLocation Then
									If docField.Value <> "" Then
										Dim fileLocation As String = docField.Value
										If fileLocation.Length > 1 AndAlso fileLocation.Chars(0) = "\" AndAlso fileLocation.Chars(1) <> "\" Then
											fileLocation = "." & fileLocation
										End If

										Dim finfo As New System.IO.FileInfo(fileLocation)
										Dim multiplier As Int32 = 2
										If TypeOf _sourceFileEncoding Is System.Text.UnicodeEncoding Then multiplier = 1

										If finfo.Length > Me.Settings.MAX_STRING_FIELD_LENGTH * multiplier Then
											fieldDTO.Value = _extractedTextFileEncodingName & ":" & _textUploader.UploadFile(fileLocation, _caseArtifactID)
										Else
											Dim sr As New System.IO.StreamReader(fileLocation, _sourceFileEncoding)
											fieldDTO.Value = encoder.GetBytes(sr.ReadToEnd)
											sr.Close()
										End If
									Else
										fieldDTO.Value = encoder.GetBytes("")
									End If
								Else
									If docField.Value.Length > Me.Settings.MAX_STRING_FIELD_LENGTH Then
										fieldDTO.Value = "unicode:" & _textUploader.UploadTextAsFile(docField.Value, _caseArtifactID, System.Guid.NewGuid.ToString)
									Else
										fieldDTO.Value = encoder.GetBytes(docField.Value)
									End If
								End If
							Else
								fieldDTO.Value = encoder.GetBytes(docField.Value)
							End If
						Case Else
							fieldDTO.Value = docField.Value
					End Select
				End If
			Next
			ManageRelationalFields(documentDTO)
			If removeFullTextField Then
				Dim al As New System.Collections.ArrayList
				al.AddRange(documentDTO.Fields)
				For Each field As kCura.EDDS.WebAPI.DocumentManagerBase.Field In documentDTO.Fields
					If field.FieldCategory = EDDS.WebAPI.DocumentManagerBase.FieldCategory.FullText Then
						al.Remove(field)
						Exit For
					End If
				Next
				documentDTO.Fields = DirectCast(al.ToArray(GetType(kCura.EDDS.WebAPI.DocumentManagerBase.Field)), kCura.EDDS.WebAPI.DocumentManagerBase.Field())
			End If
			fieldDTO = Nothing
			encoder = Nothing
			value = Nothing
		End Sub

		Private Sub ManageRelationalFields(ByVal document As kCura.EDDS.WebAPI.DocumentManagerBase.Document)
			Dim identifier As kCura.EDDS.WebAPI.DocumentManagerBase.Field
			For Each field As kCura.EDDS.WebAPI.DocumentManagerBase.Field In document.Fields
				If field.FieldCategory = EDDS.WebAPI.DocumentManagerBase.FieldCategory.Identifier Then
					identifier = field
					Exit For
				End If
			Next
			For Each field As kCura.EDDS.WebAPI.DocumentManagerBase.Field In document.Fields
				If field.FieldCategory = EDDS.WebAPI.DocumentManagerBase.FieldCategory.Relational AndAlso System.Text.Encoding.Unicode.GetString(DirectCast(field.Value, Byte())) = "" Then
					field.Value = identifier.Value
				End If
			Next
		End Sub

		Public Sub SetMultiCode(ByVal fieldDTO As kCura.EDDS.WebAPI.DocumentManagerBase.Field, ByVal docField As DocumentField)
			If fieldDTO.Value Is Nothing Then
				fieldDTO.Value = String.Empty
			End If
			Dim valueArray As String() = docField.Value.Split(";".ToCharArray)
			If valueArray.Length = 1 AndAlso valueArray(0) = String.Empty Then
				fieldDTO.Value = ""
			Else
				Dim codeArtifactIDs As New System.Collections.ArrayList
				Dim codeArtifactIdString As String
				For Each codeArtifactIdString In valueArray
					Try
						codeArtifactIDs.Add(Int32.Parse(codeArtifactIdString))
					Catch ex As System.Exception
					End Try
				Next
				fieldDTO.Value = kCura.Utility.Array.IntArrayToCSV(DirectCast(codeArtifactIDs.ToArray(GetType(Int32)), Int32())).Replace(",", ";")
			End If
		End Sub

#End Region

#Region "Status Window"

		Private Sub WriteStatusLine(ByVal et As kCura.Windows.Process.EventType, ByVal line As String, ByVal lineNumber As Int32)
			line = line & String.Format(" [line {0}]", lineNumber)
			'If Not _status Is Nothing AndAlso Not _status.IsDisposed Then
			'	_status.UpdateStatusWindow(line)
			'End If
			RaiseEvent StatusMessage(New kCura.Windows.Process.StatusEventArgs(et, lineNumber + _offset, _recordCount, line))
		End Sub

		Private Sub WriteStatusLine(ByVal et As kCura.Windows.Process.EventType, ByVal line As String)
			WriteStatusLine(et, line, Me.CurrentLineNumber)
		End Sub

		Private Sub WriteFatalError(ByVal lineNumber As Int32, ByVal ex As System.Exception, ByVal sourceLine As String())
			_continue = False
			Me.LogErrorLine(sourceLine)
			RaiseEvent FatalErrorEvent("Error processing line: " + lineNumber.ToString, ex)
		End Sub

		Private Sub WriteError(ByVal line As String, ByVal sourceLine As String())
			Me.LogErrorLine(sourceLine)
			WriteStatusLine(kCura.Windows.Process.EventType.Error, line)
		End Sub

		Private Sub WriteWarning(ByVal line As String)
			WriteStatusLine(kCura.Windows.Process.EventType.Warning, line)
		End Sub

		Private Sub WriteUpdate(ByVal line As String)
			WriteStatusLine(kCura.Windows.Process.EventType.Progress, line)
		End Sub

		Private Sub WriteEndImport(ByVal line As String)
			WriteStatusLine(kCura.Windows.Process.EventType.End, line)
		End Sub

		Private Sub _uploader_UploadStatusEvent(ByVal s As String) Handles _uploader.UploadStatusEvent
			WriteStatusLine(kCura.Windows.Process.EventType.Status, s)
		End Sub

#End Region

#Region "Public Events"

		Public Event FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception)
		Public Event StatusMessage(ByVal args As kCura.Windows.Process.StatusEventArgs)
		Public Event EndFileImport()
		Public Event StartFileImport()
		Public Event UploadModeChangeEvent(ByVal mode As String)

#Region "File Prep Event"
		Public Event FilePrepEvent(ByVal e As FilePrepEventArgs)
		Public Class FilePrepEventArgs
			Public Enum FilePrepEventType
				OpenFile
				CloseFile
				ReadEvent
			End Enum
			Private _type As FilePrepEventType
			Private _newlinesRead As Int64
			Private _bytesRead As Int64
			Private _totalBytes As Int64
			Private _stepSize As Int64
			Private _startTime As System.DateTime
			Private _endTime As System.DateTime

#Region "Accessors"

			Public ReadOnly Property Type() As FilePrepEventType
				Get
					Return _type
				End Get
			End Property

			Public ReadOnly Property NewlinesRead() As Int64
				Get
					Return _newlinesRead
				End Get
			End Property

			Public ReadOnly Property BytesRead() As Int64
				Get
					Return _bytesRead
				End Get
			End Property

			Public ReadOnly Property TotalBytes() As Int64
				Get
					Return _totalBytes
				End Get
			End Property

			Public ReadOnly Property StepSize() As Int64
				Get
					Return _stepSize
				End Get
			End Property

			Public ReadOnly Property StartTime() As System.DateTime
				Get
					Return _startTime
				End Get
			End Property

			Public ReadOnly Property EndTime() As System.DateTime
				Get
					Return _endTime
				End Get
			End Property

#End Region

			Public Sub New(ByVal eventType As FilePrepEventType, ByVal newlines As Int64, ByVal bytes As Int64, ByVal total As Int64, ByVal [step] As Int64, ByVal start As System.DateTime, ByVal [end] As System.DateTime)
				_type = eventType
				_newlinesRead = newlines
				_bytesRead = bytes
				_totalBytes = total
				_stepSize = [step]
				_startTime = start
				_endTime = [end]
			End Sub
		End Class
#End Region

#End Region

#Region "Event Handlers"

		Private Sub _uploader_UploadModeChangeEvent(ByVal mode As String) Handles _uploader.UploadModeChangeEvent
			RaiseEvent UploadModeChangeEvent(mode)
		End Sub

		Private Sub _processController_HaltProcessEvent(ByVal processID As System.Guid) Handles _processController.HaltProcessEvent
			_continue = False
			_lineCounter.StopCounting()
		End Sub

#End Region

#Region "Exceptions"

		Public Class IdentityValueNotSetException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New()
				MyBase.New("Identity value not set")
			End Sub
		End Class

		Public Class IdentityValueNotFoundException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal value As String)
				MyBase.New(String.Format("Identity value '{0}' not found", value))
			End Sub
		End Class

		Public Class DocumentDomainException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal ex As System.Exception)
				MyBase.New("Error accessing document information in domain layer: " & ex.Message.Replace("System.Web.Services.Protocols.SoapException: Server was unable to process request. ---> ", ""), ex)
			End Sub
		End Class

		Public Class DocumentReadException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal parentException As System.Exception)
				MyBase.New("Error retrieving document information from EDDS: [" & parentException.Message & "]", parentException)
			End Sub
		End Class

		Public Class DocumentOverwriteException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New()
				MyBase.New("Identifier points to document that would be overwritten")
			End Sub
		End Class

		Public Class InvalidFilenameException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal filename As String)
				MyBase.New(String.Format("File '{0}' not found.", filename))
			End Sub
		End Class

		Public Class EmptyNativeFileException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal filename As String)
				MyBase.New(String.Format("File '{0}' contains 0 bytes.", filename))
			End Sub
		End Class

		Public Class FileUploadFailedException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New()
				MyBase.New(String.Format("File upload failed.  Either the access to the path is denied or there is no disk space available."))
			End Sub
		End Class

#End Region

#Region "Reporting"

		Public Class TimeKeeper
			Private _hashtable As Hashtable
			Public Sub New()
				_hashtable = New Hashtable
			End Sub

			Public Sub Add(ByVal key As String, ByVal value As Double)
				Exit Sub
				If _hashtable(key) Is Nothing Then
					_hashtable.Add(key, value)
				Else
					_hashtable(key) = value + DirectCast(_hashtable(key), Double)
				End If
			End Sub

			Public Function ToCollectionString() As String
				Return String.Empty
				Dim sb As New System.Text.StringBuilder
				Dim nl As String = System.Environment.NewLine
				Dim tab As String = Microsoft.VisualBasic.ControlChars.Tab
				Dim key As String
				Dim i As Int32
				For Each key In _hashtable.Keys
					sb.Append(key & tab)
				Next
				sb.Append(nl)
				For Each key In _hashtable.Keys
					sb.Append("""" & (DirectCast(_hashtable(key), Double) / 1000).ToString & """,")
				Next
				sb.Append(nl)
				Return sb.ToString
			End Function
		End Class

#End Region

#Region "Preprocessing"

		Private Sub _lineCounter_OnEvent(ByVal e As kCura.Utility.File.LineCounter.EventArgs) Handles _lineCounter.OnEvent
			Select Case e.Type
				Case kCura.Utility.File.LineCounter.EventType.Begin
					_genericTimestamp = System.DateTime.Now
					RaiseEvent FilePrepEvent(New FilePrepEventArgs(FilePrepEventArgs.FilePrepEventType.OpenFile, 0, 0, e.TotalBytes, e.StepSize, _genericTimestamp, System.DateTime.Now))
				Case kCura.Utility.File.LineCounter.EventType.Progress
					RaiseEvent FilePrepEvent(New FilePrepEventArgs(FilePrepEventArgs.FilePrepEventType.ReadEvent, e.NewlinesRead, e.BytesRead, e.TotalBytes, e.StepSize, _genericTimestamp, System.DateTime.Now))
				Case kCura.Utility.File.LineCounter.EventType.Complete
					_recordCount = e.NewlinesRead
					RaiseEvent FilePrepEvent(New FilePrepEventArgs(FilePrepEventArgs.FilePrepEventType.ReadEvent, e.NewlinesRead, e.TotalBytes, e.TotalBytes, e.StepSize, _genericTimestamp, System.DateTime.Now))
					Dim path As String = _lineCounter.Path
					_columnHeaders = GetColumnNames(path)
					_processedDocumentIdentifiers = New Collections.Specialized.NameValueCollection
					Reader = New System.IO.StreamReader(path, _sourceFileEncoding)
					_docsToAdd = New ArrayList
					_docsToUpdate = New ArrayList
					_docsToProcess = New MetaDocQueue
					If _firstLineContainsColumnNames Then
						_columnHeaders = GetLine
						_recordCount -= 1
						_offset = -1
					End If
					If Not _filePathColumn Is Nothing Then
						Dim openParenIndex As Int32 = _filePathColumn.LastIndexOf("("c) + 1
						Dim closeParenIndex As Int32 = _filePathColumn.LastIndexOf(")"c)
						_filePathColumnIndex = Int32.Parse(_filePathColumn.Substring(openParenIndex, closeParenIndex - openParenIndex)) - 1
					Else
						_filePathColumnIndex = -1
					End If
					_timeKeeper = New TimeKeeper
			End Select
		End Sub

#End Region

		Private Sub SetMd5HashValue(ByVal md5Hash As String, ByVal doc As kCura.EDDS.WebAPI.DocumentManagerBase.Document)
			Dim field As kCura.EDDS.WebAPI.DocumentManagerBase.Field
			For Each field In doc.Fields
				'If field.FieldCategoryID = kCura.DynamicFields.Types.FieldCategory.DuplicateHash Then
				If field.DisplayName.ToLower = "md5 hash" Then
					field.Value = System.Text.Encoding.Unicode.GetBytes(md5Hash)
					Exit Sub
				End If
			Next
		End Sub

		Public Class Settings
			Public Shared MAX_STRING_FIELD_LENGTH As Int32 = 1048576			'2^20 = 1 meg * 2 B/char binary = 2 meg max
		End Class

		Protected Overrides ReadOnly Property UseTimeZoneOffset() As Boolean
			Get
				Return True
			End Get
		End Property
	End Class

#Region "MetaDocQueue"

	Public Class MetaDocQueue
		Implements IEnumerable
		Private _list As System.Collections.ArrayList
		Private _weight As Int64
		Private Shared QUEUE_LENGTH_MAX As Int32 = 100
		Private Shared QUEUE_WEIGHT_MAX As Int64 = 52428800
		Public Sub New()
			_list = New System.Collections.ArrayList
		End Sub

		Public Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
			Return _list.GetEnumerator
		End Function

		Public ReadOnly Property Front() As MetaDocument
			Get
				Return DirectCast(_list(0), MetaDocument)
			End Get
		End Property

		Public Sub Push(ByVal mdoc As MetaDocument)
			_weight += mdoc.Size
			_list.Add(mdoc)
		End Sub

		Public ReadOnly Property Weight() As Int64
			Get
				Return _weight
			End Get
		End Property

		Public ReadOnly Property Length() As Int32
			Get
				Return _list.Count
			End Get
		End Property

		Public ReadOnly Property IsFull() As Boolean
			Get
				Return Me.Weight > Me.QUEUE_WEIGHT_MAX OrElse Me.Length > Me.QUEUE_LENGTH_MAX
			End Get
		End Property
		Public ReadOnly Property CanAdd() As Boolean
			Get
				Return Not (Me.Weight > Me.QUEUE_WEIGHT_MAX / 2 OrElse Me.Length > Me.QUEUE_LENGTH_MAX / 2)
			End Get
		End Property

		Public ReadOnly Property Pop() As MetaDocument
			Get
				Dim retval As MetaDocument = Me.Front
				_list.RemoveAt(0)
				_weight -= retval.Size
				Return retval
			End Get
		End Property
	End Class

#End Region

End Namespace