Imports kCura.EDDS.Types.MassImport
Namespace kCura.WinEDDS
	Public Class BulkLoadFileImporter

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
		Private _docsToProcess As ImportHelpers.MetaDocQueue
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
		Private _outputNativeFileWriter As System.IO.StreamWriter
		Private _outputCodeFileWriter As System.IO.StreamWriter
		Private _caseInfo As kCura.EDDS.Types.CaseInfo

		Private _runID As String = ""
		Private _uploadKey As String

		Private _outputNativeFilePath As String = System.IO.Path.GetTempFileName
		Private _outputCodeFilePath As String = System.IO.Path.GetTempFileName
		Private _filePath As String
		Private _settings As kCura.WinEDDS.LoadFile

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
			_caseInfo = args.CaseInfo
			_settings = args
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

		Public Sub WriteCodeLineToTempFile(ByVal documentIdentifier As String, ByVal codeArtifactID As Int32, ByVal codeTypeID As Int32)
			_outputCodeFileWriter.WriteLine(String.Format("{1}{0}{2}{0}{3}{0}", Constants.NATIVE_FIELD_DELIMITER, documentIdentifier, codeArtifactID, codeTypeID))
		End Sub

#End Region

#Region "Main"

		Public Overloads Sub ReadFile()
			ReadFile(_path)
		End Sub

		Public Overloads Overrides Function ReadFile(ByVal path As String) As Object
			Dim line As String()
			_filePath = path
			Try
				RaiseEvent StartFileImport()
				Dim markStart As DateTime = DateTime.Now
				InitializeMembers(path)
				StartMassProcessor()
				While _continue AndAlso Not HasReachedEOF
					Try
						line = Me.GetLine
						Dim lineStatus As Int32 = 0
						If line.Length <> _columnHeaders.Length Then
							lineStatus += ImportStatus.ColumnMismatch					 'Throw New ColumnCountMismatchException(Me.CurrentLineNumber, _columnHeaders.Length, line.Length)
						End If
						_processedDocumentIdentifiers.Add(ManageDocument(line, lineStatus), CurrentLineNumber.ToString)
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
				Me.PushNativeBatch(True)
				RaiseEvent EndFileImport()
				WriteEndImport("Finish")
				Me.Close()
				Try
					_errorLogWriter.Close()
				Catch ex As System.Exception
				End Try
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
			_outputNativeFileWriter = New System.IO.StreamWriter(_outputNativeFilePath, False, System.Text.Encoding.Unicode)
			_outputCodeFileWriter = New System.IO.StreamWriter(_outputCodeFilePath, False, System.Text.Encoding.Unicode)
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

		Private Function ManageDocument(ByVal values As String(), ByVal lineStatus As Int32) As String
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
				If filename <> String.Empty AndAlso Not fileExists Then lineStatus += kCura.EDDS.Types.MassImport.ImportStatus.FileSpecifiedDne 'Throw New InvalidFilenameException(filename)
				If fileExists Then
					Dim now As DateTime = DateTime.Now
					If New IO.FileInfo(filename).Length = 0 Then lineStatus += kCura.EDDS.Types.MassImport.ImportStatus.EmptyFile 'Throw New EmptyNativeFileException(filename)
					oixFileIdData = kCura.OI.FileID.Manager.Instance.GetFileIDDataByFilePath(filename)
					If _copyFileToRepository Then
						fileGuid = _uploader.UploadFile(filename, _caseArtifactID)
					Else
						fileGuid = System.Guid.NewGuid.ToString
					End If
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
			Dim markPrepareFields As DateTime = DateTime.Now
			identityValue = PrepareFieldCollectionAndExtractIdentityValue(fieldCollection, values)
			If identityValue = String.Empty Then
				lineStatus += ImportStatus.EmptyIdentifier		'Throw New IdentityValueNotSetException
			ElseIf Not _processedDocumentIdentifiers(identityValue) Is Nothing Then
				lineStatus += ImportStatus.IdentifierOverlap		 '	Throw New IdentifierOverlapException(identityValue, _processedDocumentIdentifiers(identityValue))
			End If
			Dim metadoc As New MetaDocument(fileGuid, identityValue, fieldCollection, fileExists AndAlso uploadFile AndAlso (fileGuid <> String.Empty OrElse Not _copyFileToRepository), filename, fullFilePath, uploadFile, CurrentLineNumber, parentFolderID, md5hash, values, oixFileIdData, lineStatus)
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
				ManageDocumentLine(metaDoc, _extractFullTextFromNative)
				If _outputNativeFileWriter.BaseStream.Length > Config.BulkImportBatchSize Then
					Me.PushNativeBatch()
				End If
			Catch ex As kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
				WriteError(ex.Message, metaDoc.SourceLine)
			Catch ex As System.Exception
				WriteFatalError(metaDoc.LineNumber, ex, metaDoc.SourceLine)
			End Try
			WriteStatusLine(Windows.Process.EventType.Progress, String.Format("Document '{0}' processed.", metaDoc.IdentityValue), metaDoc.LineNumber)
		End Sub

		Private Function PushNativeBatch(Optional ByVal lastRun As Boolean = False) As Object
			_outputNativeFileWriter.Close()
			_outputCodeFileWriter.Close()
			Dim nativeFileUploadKey As String = _uploader.UploadBcpFile(_caseInfo.ArtifactID, _outputNativeFilePath)
			Dim codeFileUploadKey As String = _uploader.UploadBcpFile(_caseInfo.ArtifactID, _outputCodeFilePath)
			Dim settings As New kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo
			settings.RunID = _runID
			settings.CodeFileName = codeFileUploadKey
			settings.DataFileName = nativeFileUploadKey
			settings.MappedFields = Me.GetMappedFields
			Select Case _overwrite.ToLower
				Case "strict"
					settings.Overlay = EDDS.WebAPI.BulkImportManagerBase.OverwriteType.Overlay
				Case "none"
					settings.Overlay = EDDS.WebAPI.BulkImportManagerBase.OverwriteType.Append
				Case Else
					settings.Overlay = EDDS.WebAPI.BulkImportManagerBase.OverwriteType.Both
			End Select
			settings.Repository = _defaultDestinationFolderPath
			settings.UploadFiles = _filePathColumnIndex <> -1
			settings.UseBulkDataImport = True
			_runID = _bulkImportManager.BulkImportNative(_caseInfo.ArtifactID, settings).ToString
			If Not lastRun Then
				_outputNativeFileWriter = New System.IO.StreamWriter(_outputNativeFilePath, False, System.Text.Encoding.Unicode)
				_outputCodeFileWriter = New System.IO.StreamWriter(_outputCodeFilePath, False, System.Text.Encoding.Unicode)
			End If
		End Function

		Private Function GetMappedFields() As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo()
			Dim retval As New System.Collections.ArrayList
			For Each item As WinEDDS.LoadFileFieldMap.LoadFileFieldMapItem In _fieldMap
				If Not item.DocumentField Is Nothing Then
					retval.Add(item.DocumentField.ToFileInfo)
				End If
			Next
			retval.Sort(New WebServiceFieldInfoNameComparer)
			retval.Add(Me.GetIsSupportedRelativityFileTypeField)
			retval.Add(Me.GetRelativityFileTypeField)
			retval.Add(Me.GetHasNativesField)
			Return DirectCast(retval.ToArray(GetType(kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo)), kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo())
		End Function

		Private Function ManageDocumentLine(ByVal mdoc As MetaDocument, ByVal extractText As Boolean) As Int32
			Return ManageDocumentLine(mdoc.FieldCollection, mdoc.IdentityValue, mdoc.FileGuid <> String.Empty AndAlso extractText, mdoc.Filename, mdoc.FileGuid, mdoc)
		End Function

		Private Function ManageDocumentLine(ByVal fieldCollection As DocumentFieldCollection, ByVal identityValue As String, ByVal extractText As Boolean, ByVal filename As String, ByVal fileguid As String, ByVal mdoc As MetaDocument) As Int32
			'[kCura_Import_Status] INT NOT NULL,
			'[kCura_Import_IsNew] BIT NOT NULL,
			'[ArtifactID] INT NOT NULL,
			'[kCura_Import_OriginalLineNumber] INT NOT NULL,
			'[kCura_Import_FileGuid] NVARCHAR(100) NOT NULL,
			'[kCura_Import_Filename] NVARCHAR(200) NOT NULL,
			'[kCura_Import_Location] NVARCHAR(2000),
			'[kCura_Import_OriginalFileLocation] NVARCHAR(2000){1}
			'[kCura_Import_ParentFolderID] INT NOT NULL,
			_outputNativeFileWriter.Write(mdoc.LineStatus.ToString & Constants.NATIVE_FIELD_DELIMITER)
			_outputNativeFileWriter.Write("0" & Constants.NATIVE_FIELD_DELIMITER)
			_outputNativeFileWriter.Write("0" & Constants.NATIVE_FIELD_DELIMITER)
			_outputNativeFileWriter.Write(mdoc.LineNumber & Constants.NATIVE_FIELD_DELIMITER)
			If mdoc.UploadFile And mdoc.IndexFileInDB Then
				_outputNativeFileWriter.Write(fileguid & Constants.NATIVE_FIELD_DELIMITER)
				_outputNativeFileWriter.Write(filename & Constants.NATIVE_FIELD_DELIMITER)
				If _settings.CopyFilesToDocumentRepository Then
					_outputNativeFileWriter.Write(_defaultDestinationFolderPath & fileguid & Constants.NATIVE_FIELD_DELIMITER)
					_outputNativeFileWriter.Write(mdoc.FullFilePath & Constants.NATIVE_FIELD_DELIMITER)
				Else
					_outputNativeFileWriter.Write(mdoc.FullFilePath & Constants.NATIVE_FIELD_DELIMITER)
					_outputNativeFileWriter.Write(mdoc.FullFilePath & Constants.NATIVE_FIELD_DELIMITER)
				End If
			Else
				_outputNativeFileWriter.Write(Constants.NATIVE_FIELD_DELIMITER)
				_outputNativeFileWriter.Write(Constants.NATIVE_FIELD_DELIMITER)
				_outputNativeFileWriter.Write(Constants.NATIVE_FIELD_DELIMITER)
				_outputNativeFileWriter.Write(Constants.NATIVE_FIELD_DELIMITER)
			End If
			_outputNativeFileWriter.Write(mdoc.ParentFolderID & Constants.NATIVE_FIELD_DELIMITER)
			For Each docField As DocumentField In fieldCollection.AllFields
				_outputNativeFileWriter.Write(docField.Value)
				_outputNativeFileWriter.Write(Constants.NATIVE_FIELD_DELIMITER)
			Next
			If _filePathColumnIndex <> -1 AndAlso mdoc.UploadFile AndAlso mdoc.IndexFileInDB Then
				Dim boolString As String = "0"
				If Me.IsSupportedRelativityFileType(mdoc.FileIdData) Then boolString = "1"
				_outputNativeFileWriter.Write(boolString & Constants.NATIVE_FIELD_DELIMITER)
				_outputNativeFileWriter.Write(mdoc.FileIdData.FileType & Constants.NATIVE_FIELD_DELIMITER)
				_outputNativeFileWriter.Write("1" & Constants.NATIVE_FIELD_DELIMITER)
			Else
				_outputNativeFileWriter.Write("0" & Constants.NATIVE_FIELD_DELIMITER)
				_outputNativeFileWriter.Write(Constants.NATIVE_FIELD_DELIMITER)
				_outputNativeFileWriter.Write("0" & Constants.NATIVE_FIELD_DELIMITER)
			End If

			_outputNativeFileWriter.Write(vbNewLine)
		End Function

		Private Function GetIsSupportedRelativityFileTypeField() As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo
			For Each field As kCura.EDDS.WebAPI.DocumentManagerBase.Field In _allFields
				If field.DisplayName.ToLower = "supported by viewer" Then
					Return Me.FieldDtoToFieldInfo(field)
				End If
			Next
		End Function

		Private Function GetRelativityFileTypeField() As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo
			For Each field As kCura.EDDS.WebAPI.DocumentManagerBase.Field In _allFields
				If field.DisplayName.ToLower = "relativity native type" Then
					Return Me.FieldDtoToFieldInfo(field)
				End If
			Next
		End Function

		Private Function GetHasNativesField() As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo
			For Each field As kCura.EDDS.WebAPI.DocumentManagerBase.Field In _allFields
				If field.DisplayName.ToLower = "has native" Then
					Return Me.FieldDtoToFieldInfo(field)
				End If
			Next
		End Function

		Private Function FieldDtoToFieldInfo(ByVal input As kCura.EDDS.WebAPI.DocumentManagerBase.Field) As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo
			Dim retval As New kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo
			retval.ArtifactID = input.ArtifactID
			retval.Category = CType(input.FieldCategoryID, kCura.EDDS.WebAPI.BulkImportManagerBase.FieldCategory)
			If Not input.CodeTypeID.IsNull Then retval.CodeTypeID = input.CodeTypeID.Value
			retval.DisplayName = input.DisplayName
			If Not input.MaxLength.IsNull Then retval.TextLength = input.MaxLength.Value
			retval.Type = CType(input.FieldTypeID, kCura.EDDS.WebAPI.BulkImportManagerBase.FieldType)
			Return retval
		End Function

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
			For Each item In _fieldMap
				If Not item.DocumentField Is Nothing AndAlso item.DocumentField.FieldCategory = DynamicFields.Types.FieldCategory.Identifier Then
					identityValue = values(item.NativeFileColumnIndex)
				End If
			Next
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
					MyBase.SetFieldValue(docfield, values, item.NativeFileColumnIndex, identityValue)
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

		Private Sub _processController_ExportServerErrors(ByVal exportLocation As String) Handles _processController.ExportServerErrorsEvent
			With _bulkImportManager.GenerateNativeErrorFiles(_caseInfo.ArtifactID, _runID, True)
				Dim downloader As New FileDownloader(DirectCast(_bulkImportManager.Credentials, System.Net.NetworkCredential), _caseInfo.DocumentPath, _caseInfo.DownloadHandlerURL, _bulkImportManager.CookieContainer, kCura.WinEDDS.Service.Settings.AuthenticationToken)
				Dim rowsLocation As String = System.IO.Path.GetTempFileName
				Dim errorsLocation As String = System.IO.Path.GetTempFileName
				downloader.DownloadFile(rowsLocation, .OpticonKey, _caseInfo.ArtifactID.ToString)
				downloader.DownloadFile(errorsLocation, .LogKey, _caseInfo.ArtifactID.ToString)
				Dim rootFileName As String = _filePath
				Dim defaultExtension As String
				If Not rootFileName.IndexOf(".") = -1 Then
					defaultExtension = rootFileName.Substring(rootFileName.LastIndexOf("."))
					rootFileName = rootFileName.Substring(0, rootFileName.LastIndexOf("."))
				Else
					defaultExtension = ".opt"
				End If
				rootFileName.Trim("\"c)
				If rootFileName.IndexOf("\") <> -1 Then
					rootFileName = rootFileName.Substring(rootFileName.LastIndexOf("\") + 1)
				End If

				Dim rootFilePath As String = exportLocation & rootFileName
				Dim datetimeNow As System.DateTime = System.DateTime.Now
				Dim errorFilePath As String = rootFilePath & "_ErrorLines_" & datetimeNow.Ticks & defaultExtension
				Dim errorReportPath As String = rootFilePath & "_ErrorReport_" & datetimeNow.Ticks & ".csv"
				System.IO.File.Move(rowsLocation, errorFilePath)
				System.IO.File.Move(errorsLocation, errorReportPath)
			End With

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
					_docsToProcess = New ImportHelpers.MetaDocQueue
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
			End Select
		End Sub

#End Region


		Public Class Settings
			Public Const MAX_STRING_FIELD_LENGTH As Int32 = 1048576			'2^20 = 1 meg * 2 B/char binary = 2 meg max
		End Class

		Protected Overrides ReadOnly Property UseTimeZoneOffset() As Boolean
			Get
				Return True
			End Get
		End Property
	End Class

	Public Class WebServiceFieldInfoNameComparer
		Implements IComparer

		Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements System.Collections.IComparer.Compare
			Return String.Compare(DirectCast(x, kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo).DisplayName, DirectCast(y, kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo).DisplayName)
		End Function
	End Class

End Namespace