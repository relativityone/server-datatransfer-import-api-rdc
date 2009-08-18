Imports kCura.EDDS.Types.MassImport
Namespace kCura.WinEDDS
	Public Class BulkLoadFileImporter

		Inherits kCura.WinEDDS.LoadFileBase


#Region "Members"
		Private _overwrite As String
		Private WithEvents _uploader As kCura.WinEDDS.FileUploader
		Private WithEvents _bcpuploader As kCura.WinEDDS.FileUploader
		'Private _textUploader As kCura.WinEDDS.FileUploader
		Private _path As String
		Private _pathIsSet As Boolean = False
		Private _selectedIdentifier As DocumentField
		Private _docFieldCollection As DocumentFieldCollection
		Private _parentFolderDTO As kCura.EDDS.WebAPI.FolderManagerBase.Folder
		Private _auditManager As kCura.WinEDDS.Service.AuditManager

		Private _recordCount As Int64 = -1
		Private _extractFullTextFromNative As Boolean
		Private _allFields As kCura.EDDS.WebAPI.DocumentManagerBase.Field()
		Private _fieldsForCreate As kCura.EDDS.WebAPI.DocumentManagerBase.Field()
		Protected _continue As Boolean
		Protected _processedDocumentIdentifiers As Collections.Specialized.NameValueCollection
		Protected WithEvents _processController As kCura.Windows.Process.Controller
		Protected _offset As Int32 = 0
		Protected _firstTimeThrough As Boolean
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
		Private _batchCounter As Int32 = 0
		Private _errorMessageFileLocation As String = ""
		Private _errorLinesFileLocation As String = ""

		Public Const MaxNumberOfErrorsInGrid As Int32 = 1000
		Private _errorCount As Int32 = 0
		Private _prePushErrorLineNumbersFileName As String = ""
		Private _isAuditingEnabled As Boolean
		Private _processID As Guid
		Private _parentArtifactTypeID As Int32
		Private _statistics As New kCura.WinEDDS.Statistics
		Private _timekeeper As New kCura.Utility.Timekeeper
		Private _currentStatisticsSnapshot As IDictionary
		Private _statisticsLastUpdated As System.DateTime = System.DateTime.Now
		Private _start As System.DateTime
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

		Public ReadOnly Property AllFields(ByVal artifactTypeID As Int32) As kCura.EDDS.WebAPI.DocumentManagerBase.Field()
			Get
				If _allFields Is Nothing Then
					_allFields = _fieldQuery.RetrieveAllAsArray(_caseArtifactID, artifactTypeID, True)
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
					For Each field As kCura.EDDS.WebAPI.DocumentManagerBase.Field In Me.AllFields(10)
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

		Public ReadOnly Property FileInfoField(ByVal artifactTypeID As Int32) As kCura.EDDS.WebAPI.DocumentManagerBase.Field
			Get
				Dim retVal As New kCura.EDDS.WebAPI.DocumentManagerBase.Field
				For Each field As kCura.EDDS.WebAPI.DocumentManagerBase.Field In Me.AllFields(artifactTypeID)
					If field.FieldCategoryID = kCura.DynamicFields.Types.FieldCategory.FileInfo Then retVal = field
				Next
				Return retVal
			End Get
		End Property

		Public ReadOnly Property FullTextField() As kCura.EDDS.WebAPI.DocumentManagerBase.Field
			Get
				For Each field As kCura.EDDS.WebAPI.DocumentManagerBase.Field In Me.AllFields(10)
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
				Return _errorCount > 0
			End Get
		End Property

		Public ReadOnly Property UploadConnection() As kCura.WinEDDS.FileUploader.Type
			Get
				Return _uploader.UploaderType
			End Get
		End Property

		Public ReadOnly Property Statistics() As kCura.WinEDDS.Statistics
			Get
				Return _statistics
			End Get
		End Property

		Public ReadOnly Property FoldersCreated() As Int32
			Get
				Return _folderManager.CreationCount
			End Get
		End Property

		Public ReadOnly Property CodesCreated() As Int32
			Get
				Dim retval As Int32
				If Me.MulticodeMatrix Is Nothing Then Return 0
				For Each cache As NestedArtifactCache In Me.MulticodeMatrix.Values
					retval += cache.CreationCount
				Next
				Return retval
			End Get
		End Property

#End Region

#Region "Constructors"

		Public Sub New(ByVal args As LoadFile, ByVal processController As kCura.Windows.Process.Controller, ByVal timeZoneOffset As Int32, ByVal initializeUploaders As Boolean, ByVal processID As Guid, ByVal doRetryLogic As Boolean)
			Me.New(args, processController, timeZoneOffset, True, initializeUploaders, processID, doRetryLogic)
		End Sub

		Public Sub New(ByVal args As LoadFile, ByVal processController As kCura.Windows.Process.Controller, ByVal timeZoneOffset As Int32, ByVal autoDetect As Boolean, ByVal initializeUploaders As Boolean, ByVal processID As Guid, ByVal doRetryLogic As Boolean)
			MyBase.New(args, timeZoneOffset, doRetryLogic, autoDetect)
			_overwrite = args.OverwriteDestination
			_auditManager = New kCura.WinEDDS.Service.AuditManager(args.Credentials, args.CookieContainer)
			If args.CopyFilesToDocumentRepository Then
				_defaultDestinationFolderPath = args.SelectedCasePath & "EDDS" & args.CaseInfo.ArtifactID & "\"
				If args.ArtifactTypeID <> 10 Then
					For Each item As LoadFileFieldMap.LoadFileFieldMapItem In args.FieldMap
						If Not item.DocumentField Is Nothing AndAlso item.NativeFileColumnIndex > -1 AndAlso item.DocumentField.FieldTypeID = kCura.DynamicFields.Types.FieldTypeHelper.FieldType.File Then
							_defaultDestinationFolderPath &= "File" & item.DocumentField.FieldID & "\"
						End If
					Next
				End If
			End If
			_defaultTextFolderPath = args.CaseDefaultPath & "EDDS" & args.CaseInfo.ArtifactID & "\"
			If initializeUploaders Then
				_uploader = New kCura.WinEDDS.FileUploader(args.Credentials, args.CaseInfo.ArtifactID, _defaultDestinationFolderPath, args.CookieContainer)
				_bcpuploader = New kCura.WinEDDS.FileUploader(args.Credentials, args.CaseInfo.ArtifactID, _defaultDestinationFolderPath, args.CookieContainer, False)
				'_textUploader = New kCura.WinEDDS.FileUploader(args.Credentials, args.CaseInfo.ArtifactID, _defaultTextFolderPath, args.CookieContainer, False)
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
			_isAuditingEnabled = New kCura.WinEDDS.Service.RelativityManager(args.Credentials, args.CookieContainer).IsAuditingEnabled
			_processID = processID
			_startLineNumber = args.StartLineNumber
			Dim parentQuery As New kCura.WinEDDS.Service.ObjectTypeManager(args.Credentials, args.CookieContainer)
			_parentArtifactTypeID = CType(parentQuery.RetrieveParentArtifactTypeID(args.CaseInfo.ArtifactID, args.ArtifactTypeID).Tables(0).Rows(0)("ParentArtifactTypeID"), Int32)
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
			_start = System.DateTime.Now
			_timekeeper.MarkStart("TOTAL")
			Try
				RaiseEvent StartFileImport()
				_timekeeper.MarkStart("ReadFile_InitializeMembers")
				_lineCounter = New kCura.Utility.File.LineCounter
				Dim validateBcp As FileUploadReturnArgs = _bcpuploader.ValidateBcpPath(_caseInfo.ArtifactID, _outputNativeFilePath)
				If validateBcp.Type = FileUploadReturnArgs.FileUploadReturnType.UploadError And Not Config.EnableSingleModeImport Then
					Throw New BcpPathAccessException(validateBcp.Value)
				Else
					RaiseEvent UploadModeChangeEvent(_uploader.UploaderType.ToString, _bcpuploader.IsBulkEnabled)
				End If
				InitializeMembers(path)
				_timekeeper.MarkEnd("ReadFile_InitializeMembers")
				_timekeeper.MarkStart("ReadFile_ProcessDocuments")
				Dim isError As Boolean = False
				While _continue AndAlso Not HasReachedEOF
					Try
						If Me.CurrentLineNumber < _startLineNumber Then
							Me.AdvanceLine()
						Else
							_timekeeper.MarkStart("ReadFile_GetLine")
							_statistics.DocCount += 1
							line = Me.GetLine
							_timekeeper.MarkEnd("ReadFile_GetLine")
							Dim lineStatus As Int32 = 0
							If line.Length <> _columnHeaders.Length Then
								lineStatus += ImportStatus.ColumnMismatch								 'Throw New ColumnCountMismatchException(Me.CurrentLineNumber, _columnHeaders.Length, line.Length)
							End If

							_timekeeper.MarkStart("ReadFile_ManageDocument")
							Dim id As String = ManageDocument(line, lineStatus)
							_timekeeper.MarkEnd("ReadFile_ManageDocument")

							_timekeeper.MarkStart("ReadFile_IdTrack")
							_processedDocumentIdentifiers.Add(id, CurrentLineNumber.ToString)
							_timekeeper.MarkEnd("ReadFile_IdTrack")
						End If
					Catch ex As LoadFileBase.CodeCreationException
						If ex.IsFatal Then
							_continue = False
							isError = True
							WriteFatalError(Me.CurrentLineNumber, ex, line)
						Else
							WriteError(Me.CurrentLineNumber, ex.Message)
						End If
					Catch ex As kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
						WriteError(Me.CurrentLineNumber, ex.Message)
					Catch ex As System.IO.FileNotFoundException
						WriteError(Me.CurrentLineNumber, ex.Message)
					Catch ex As System.Exception
						WriteFatalError(Me.CurrentLineNumber, ex, line)
					End Try
				End While
				_timekeeper.MarkEnd("ReadFile_ProcessDocuments")
				_timekeeper.MarkStart("ReadFile_OtherFinalization")
				Me.PushNativeBatch(True)
				RaiseEvent EndFileImport(_runID)
				WriteEndImport("Finish")
				Me.Close()
				Try
					_errorLogWriter.Close()
				Catch ex As System.Exception
				End Try
				_timekeeper.MarkEnd("ReadFile_OtherFinalization")
				_timekeeper.MarkStart("ReadFile_CleanupTempTables")
				Me.CleanupTempTables()
				_timekeeper.MarkEnd("ReadFile_CleanupTempTables")
				_timekeeper.MarkEnd("TOTAL")
				_timekeeper.GenerateCsvReportItemsAsRows("_winedds", "C:\")
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
			kCura.Utility.File.Delete(_outputNativeFilePath)
			kCura.Utility.File.Delete(_outputCodeFilePath)
			_outputNativeFileWriter = New System.IO.StreamWriter(_outputNativeFilePath, False, System.Text.Encoding.Unicode)
			_outputCodeFileWriter = New System.IO.StreamWriter(_outputCodeFilePath, False, System.Text.Encoding.Unicode)
			RaiseEvent StatusMessage(New kCura.Windows.Process.StatusEventArgs(Windows.Process.EventType.ResetStartTime, 0, _recordCount, "Reset time for import rolling average", Nothing))
		End Sub

		Private Sub InitializeLineCounter(ByVal path As String)
			_lineCounter.Path = path
			_lineCounter.CountLines(_sourceFileEncoding, New kCura.Utility.File.LineCounter.LineCounterArgs(Me.Bound, Me.Delimiter))
		End Sub

		Private Sub InitializeFolderManagement()
			If _createFolderStructure Then
				If _artifactTypeID = 10 Then _folderCache = New FolderCache(_folderManager, _folderID, _caseArtifactID)
				Dim openParenIndex As Int32 = _destinationFolder.LastIndexOf("("c) + 1
				Dim closeParenIndex As Int32 = _destinationFolder.LastIndexOf(")"c)
				_destinationFolderColumnIndex = Int32.Parse(_destinationFolder.Substring(openParenIndex, closeParenIndex - openParenIndex)) - 1
			End If
		End Sub

		Private Sub InitializeFieldIdList()
			Dim fieldIdList As New System.Collections.ArrayList
			For Each item As LoadFileFieldMap.LoadFileFieldMapItem In _fieldMap
				If Not item.DocumentField Is Nothing AndAlso Not item.NativeFileColumnIndex = -1 Then
					'If item.DocumentField.FieldCategoryID <> kCura.DynamicFields.Types.FieldCategory.FullText Then fieldIdList.Add(item.DocumentField.FieldID)
					fieldIdList.Add(item.DocumentField.FieldID)
				End If
			Next
			fieldIdList.Add(Me.FileInfoField(_artifactTypeID).ArtifactID)
			_fieldArtifactIds = DirectCast(fieldIdList.ToArray(GetType(Int32)), Int32())
		End Sub

		Private Function ManageDocument(ByVal values As String(), ByVal lineStatus As Int32) As String
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
			Dim destinationVolume As String
			_timekeeper.MarkStart("ManageDocument_Filesystem")
			If uploadFile AndAlso _artifactTypeID = 10 Then
				filename = values(_filePathColumnIndex)
				If filename.Length > 1 AndAlso filename.Chars(0) = "\" AndAlso filename.Chars(1) <> "\" Then
					filename = "." & filename
				End If

				fileExists = System.IO.File.Exists(filename)
				If filename <> String.Empty AndAlso Not fileExists Then lineStatus += kCura.EDDS.Types.MassImport.ImportStatus.FileSpecifiedDne 'Throw New InvalidFilenameException(filename)
				If fileExists Then
					Dim now As DateTime = DateTime.Now
					Dim tries As Int32 = kCura.Utility.Config.Settings.IoErrorNumberOfRetries
					If Me.GetFileLength(filename) = 0 Then lineStatus += kCura.EDDS.Types.MassImport.ImportStatus.EmptyFile 'Throw New EmptyNativeFileException(filename)
					oixFileIdData = kCura.OI.FileID.Manager.Instance.GetFileIDDataByFilePath(filename)
					If _copyFileToRepository Then
						Dim start As Int64 = System.DateTime.Now.Ticks
						Dim updateCurrentStats As Boolean = (start - _statisticsLastUpdated.Ticks) > 10000000
						_statistics.FileBytes += Me.GetFileLength(filename)
						fileGuid = _uploader.UploadFile(filename, _caseArtifactID)
						_statistics.FileTime += System.DateTime.Now.Ticks - start
						destinationVolume = _uploader.CurrentDestinationDirectory
						If updateCurrentStats Then
							_currentStatisticsSnapshot = _statistics.ToDictionary
							_statisticsLastUpdated = New System.DateTime(start)
						End If
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
			_timekeeper.MarkEnd("ManageDocument_Filesystem")

			_timekeeper.MarkStart("ManageDocument_Folder")
			If _createFolderStructure Then
				If _artifactTypeID = 10 Then
					parentFolderID = _folderCache.FolderID(Me.CleanDestinationFolderPath(values(_destinationFolderColumnIndex)))
				Else
					Dim textIdentifier As String = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(kCura.Utility.NullableTypesHelper.ToNullableString(values(_destinationFolderColumnIndex)))

					If textIdentifier = "" Then
						If _overwrite.ToLower = "strict" OrElse _overwrite.ToLower = "append" Then
							parentFolderID = -1
						End If
						Throw New ParentObjectReferenceRequiredException(Me.CurrentLineNumber, _destinationFolderColumnIndex)
					Else
						Dim parentObjectTable As System.Data.DataTable = _objectManager.RetrieveArtifactIdOfMappedParentObject(_caseArtifactID, _
						textIdentifier, _artifactTypeID).Tables(0)
						If parentObjectTable.Rows.Count > 1 Then
							Throw New DuplicateObjectReferenceException(Me.CurrentLineNumber, _destinationFolderColumnIndex, "Parent Info")
						ElseIf parentObjectTable.Rows.Count = 0 Then
							Throw New NonExistentParentException(Me.CurrentLineNumber, _destinationFolderColumnIndex, "Parent Info")
						Else
							parentFolderID = CType(parentObjectTable.Rows(0)("ArtifactID"), Int32)
						End If
					End If
				End If
			Else
				If _artifactTypeID = 10 OrElse _parentArtifactTypeID = 8 Then
					parentFolderID = _folderID
				Else
					parentFolderID = -1
				End If
			End If
			_timekeeper.MarkEnd("ManageDocument_Folder")
			Dim markPrepareFields As DateTime = DateTime.Now
			identityValue = PrepareFieldCollectionAndExtractIdentityValue(fieldCollection, values)
			If identityValue = String.Empty Then
				'lineStatus += ImportStatus.EmptyIdentifier				'
				Throw New IdentityValueNotSetException
			ElseIf Not _processedDocumentIdentifiers(identityValue) Is Nothing Then
				'lineStatus += ImportStatus.IdentifierOverlap				'	
				Throw New IdentifierOverlapException(identityValue, _processedDocumentIdentifiers(identityValue))
			End If
			Dim metadoc As New MetaDocument(fileGuid, identityValue, fieldCollection, fileExists AndAlso uploadFile AndAlso (fileGuid <> String.Empty OrElse Not _copyFileToRepository), filename, fullFilePath, uploadFile, CurrentLineNumber, parentFolderID, md5hash, values, oixFileIdData, lineStatus, destinationVolume)
			'_docsToProcess.Push(metadoc)
			_timekeeper.MarkStart("ManageDocument_ManageDocumentMetadata")
			ManageDocumentMetaData(metadoc)
			_timekeeper.MarkEnd("ManageDocument_ManageDocumentMetadata")
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

#Region "WebService Calls"

		Private Sub ManageDocumentMetaData(ByVal metaDoc As MetaDocument)
			_number += 1
			Try
				_timekeeper.MarkStart("ManageDocumentMetadata_ManageDocumentLine")
				ManageDocumentLine(metaDoc, _extractFullTextFromNative)
				_timekeeper.MarkEnd("ManageDocumentMetadata_ManageDocumentLine")
				_batchCounter += 1
				_timekeeper.MarkStart("ManageDocumentMetadata_WserviceCall")
				If _outputNativeFileWriter.BaseStream.Length > Config.ImportBatchMaxVolume OrElse _batchCounter > Config.ImportBatchSize - 1 Then
					Me.PushNativeBatch()
				End If
				_timekeeper.MarkEnd("ManageDocumentMetadata_WserviceCall")
			Catch ex As kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
				WriteError(metaDoc.LineNumber, ex.Message)
			Catch ex As System.Exception
				WriteFatalError(metaDoc.LineNumber, ex, metaDoc.SourceLine)
			End Try
			_timekeeper.MarkStart("ManageDocumentMetadata_ProgressEvent")
			WriteStatusLine(Windows.Process.EventType.Progress, String.Format("Document '{0}' processed.", metaDoc.IdentityValue), metaDoc.LineNumber)
			_timekeeper.MarkEnd("ManageDocumentMetadata_ProgressEvent")
		End Sub

		Private Function BulkImport(ByVal settings As kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Dim tries As Int32 = kCura.Utility.Config.Settings.IoErrorNumberOfRetries
			While tries > 0
				Try
					Dim retval As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
					If TypeOf settings Is kCura.EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo Then
						Return _bulkImportManager.BulkImportObjects(_caseInfo.ArtifactID, DirectCast(settings, kCura.EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo))
					Else
						Return _bulkImportManager.BulkImportNative(_caseInfo.ArtifactID, settings)
					End If
				Catch ex As Exception
					tries -= 1
					If tries = 0 Then
						Throw
					ElseIf tries = kCura.Utility.Config.Settings.IoErrorNumberOfRetries - 1 Then
						Me.RaiseIoWarning(New kCura.Utility.DelimitedFileImporter.IoWarningEventArgs(kCura.Utility.Config.Settings.IoErrorWaitTimeInSeconds, ex, Me.CurrentLineNumber))
						System.Threading.Thread.CurrentThread.Join(1000 * kCura.Utility.Config.Settings.IoErrorWaitTimeInSeconds)
					End If
				End Try
			End While
		End Function

		Private Function GetSettingsObject() As kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo
			If _artifactTypeID = 10 Then
				Return New kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo
			Else
				Dim settings As New kCura.EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo
				settings.ArtifactTypeID = _artifactTypeID
				Return settings
			End If
		End Function

		Private Function PushNativeBatch(Optional ByVal lastRun As Boolean = False) As Object
			_outputNativeFileWriter.Close()
			_outputCodeFileWriter.Close()
			If _batchCounter = 0 Then Return Nothing
			_batchCounter = 0
			Dim settings As kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo = Me.GetSettingsObject
			settings.UseBulkDataImport = True
			_bcpuploader.DoRetry = True
			Dim uploadBcp As FileUploadReturnArgs = _bcpuploader.UploadBcpFile(_caseInfo.ArtifactID, _outputNativeFilePath)
			If uploadBcp Is Nothing Then Return Nothing
			Dim nativeFileUploadKey As String = uploadBcp.Value
			Dim codebcp As FileUploadReturnArgs = _bcpuploader.UploadBcpFile(_caseInfo.ArtifactID, _outputCodeFilePath)
			If codebcp Is Nothing Then Return Nothing
			Dim codeFileUploadKey As String = codebcp.Value
			If _artifactTypeID = 10 Then
				settings.Repository = _defaultDestinationFolderPath
				If settings.Repository = "" Then settings.Repository = _caseInfo.DocumentPath
			Else
				settings.Repository = _caseInfo.DocumentPath
			End If
			Dim start As Int64 = System.DateTime.Now.Ticks
			If uploadBcp.Type = FileUploadReturnArgs.FileUploadReturnType.UploadError Then
				If Config.EnableSingleModeImport Then
					RaiseEvent UploadModeChangeEvent(_uploader.UploaderType.ToString, _bcpuploader.IsBulkEnabled)
					_uploader.DestinationFolderPath = settings.Repository
					_bcpuploader.DestinationFolderPath = settings.Repository
					nativeFileUploadKey = _bcpuploader.UploadFile(_outputNativeFilePath, _caseInfo.ArtifactID)
					codeFileUploadKey = _bcpuploader.UploadFile(_outputCodeFilePath, _caseInfo.ArtifactID)
					settings.UseBulkDataImport = False
				Else
					Throw New BcpPathAccessException(uploadBcp.Value)
				End If
			End If
			_statistics.MetadataTime += System.Math.Max((System.DateTime.Now.Ticks - start), 1)
			_statistics.MetadataBytes += (Me.GetFileLength(_outputCodeFilePath) + Me.GetFileLength(_outputNativeFilePath))
			settings.RunID = _runID
			settings.CodeFileName = codeFileUploadKey
			settings.DataFileName = nativeFileUploadKey
			settings.MappedFields = Me.GetMappedFields(_artifactTypeID)
			settings.KeyFieldArtifactID = _keyFieldID
			Select Case _overwrite.ToLower
				Case "strict"
					settings.Overlay = EDDS.WebAPI.BulkImportManagerBase.OverwriteType.Overlay
				Case "none"
					settings.Overlay = EDDS.WebAPI.BulkImportManagerBase.OverwriteType.Append
				Case Else
					settings.Overlay = EDDS.WebAPI.BulkImportManagerBase.OverwriteType.Both
			End Select
			start = System.DateTime.Now.Ticks
			settings.UploadFiles = _filePathColumnIndex <> -1 AndAlso _settings.LoadNativeFiles
			Dim runResults As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults = Me.BulkImport(settings)
			_statistics.ProcessRunResults(runResults)
			_runID = runResults.RunID
			_statistics.SqlTime += (System.DateTime.Now.Ticks - start)

			kCura.Utility.File.Delete(_outputNativeFilePath)
			kCura.Utility.File.Delete(_outputCodeFilePath)
			_currentStatisticsSnapshot = _statistics.ToDictionary
			_statisticsLastUpdated = System.DateTime.Now
			If Not lastRun Then
				_outputNativeFileWriter = New System.IO.StreamWriter(_outputNativeFilePath, False, System.Text.Encoding.Unicode)
				_outputCodeFileWriter = New System.IO.StreamWriter(_outputCodeFilePath, False, System.Text.Encoding.Unicode)
			End If
			Me.ManageErrors(_artifactTypeID)
		End Function

		Private Function GetMappedFields(ByVal artifactTypeID As Int32) As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo()
			Dim retval As New System.Collections.ArrayList
			For Each item As WinEDDS.LoadFileFieldMap.LoadFileFieldMapItem In _fieldMap
				If Not item.DocumentField Is Nothing Then
					retval.Add(item.DocumentField.ToFileInfo)
				End If
			Next
			retval.Sort(New WebServiceFieldInfoNameComparer)
			If artifactTypeID = 10 Then
				retval.Add(Me.GetIsSupportedRelativityFileTypeField)
				retval.Add(Me.GetRelativityFileTypeField)
				retval.Add(Me.GetHasNativesField)
			Else
				'If (_filePathColumnIndex <> -1) AndAlso _uploadFiles Then
				'	retval.Add(Me.GetObjectFileField())
				'End If
			End If
			Return DirectCast(retval.ToArray(GetType(kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo)), kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo())
		End Function

		Private Function ManageDocumentLine(ByVal mdoc As MetaDocument, ByVal extractText As Boolean) As Int32
			Return ManageDocumentLine(mdoc.FieldCollection, mdoc.IdentityValue, mdoc.FileGuid <> String.Empty AndAlso extractText, mdoc.Filename, mdoc.FileGuid, mdoc)
		End Function

		Private Function ManageDocumentLine(ByVal fieldCollection As DocumentFieldCollection, ByVal identityValue As String, ByVal extractText As Boolean, ByVal filename As String, ByVal fileguid As String, ByVal mdoc As MetaDocument) As Int32
			_outputNativeFileWriter.Write(mdoc.LineStatus.ToString & Constants.NATIVE_FIELD_DELIMITER)
			_outputNativeFileWriter.Write("0" & Constants.NATIVE_FIELD_DELIMITER)
			_outputNativeFileWriter.Write("0" & Constants.NATIVE_FIELD_DELIMITER)
			_outputNativeFileWriter.Write(mdoc.LineNumber & Constants.NATIVE_FIELD_DELIMITER)
			If mdoc.UploadFile And mdoc.IndexFileInDB Then
				_outputNativeFileWriter.Write(fileguid & Constants.NATIVE_FIELD_DELIMITER)
				_outputNativeFileWriter.Write(filename & Constants.NATIVE_FIELD_DELIMITER)
				If _settings.CopyFilesToDocumentRepository Then
					_outputNativeFileWriter.Write(_defaultDestinationFolderPath & mdoc.DestinationVolume & "\" & fileguid & Constants.NATIVE_FIELD_DELIMITER)
					_outputNativeFileWriter.Write(mdoc.FullFilePath & Constants.NATIVE_FIELD_DELIMITER)
				Else
					_outputNativeFileWriter.Write(mdoc.FullFilePath & Constants.NATIVE_FIELD_DELIMITER)
					_outputNativeFileWriter.Write(mdoc.FullFilePath & Constants.NATIVE_FIELD_DELIMITER)
				End If
				_outputNativeFileWriter.Write(Me.GetFileLength(mdoc.FullFilePath) & Constants.NATIVE_FIELD_DELIMITER)
			Else
				_outputNativeFileWriter.Write(Constants.NATIVE_FIELD_DELIMITER)
				_outputNativeFileWriter.Write(Constants.NATIVE_FIELD_DELIMITER)
				_outputNativeFileWriter.Write(Constants.NATIVE_FIELD_DELIMITER)
				_outputNativeFileWriter.Write(Constants.NATIVE_FIELD_DELIMITER)
				_outputNativeFileWriter.Write("0" & Constants.NATIVE_FIELD_DELIMITER)
			End If
			_outputNativeFileWriter.Write(mdoc.ParentFolderID & Constants.NATIVE_FIELD_DELIMITER)
			For Each docField As DocumentField In fieldCollection.AllFields
				If docField.FieldTypeID = kCura.DynamicFields.Types.FieldTypeHelper.FieldType.MultiCode OrElse docField.FieldTypeID = kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Code Then
					_outputNativeFileWriter.Write(docField.Value)
					_outputNativeFileWriter.Write(Constants.NATIVE_FIELD_DELIMITER)
				ElseIf docField.FieldTypeID = kCura.DynamicFields.Types.FieldTypeHelper.FieldType.File Then
					Dim fileFieldValues() As String = System.Web.HttpUtility.UrlDecode(docField.Value).Split(Chr(11))
					If fileFieldValues.Length > 1 Then
						_outputNativeFileWriter.Write(fileFieldValues(0))
						_outputNativeFileWriter.Write(Constants.NATIVE_FIELD_DELIMITER)
						_outputNativeFileWriter.Write(fileFieldValues(1))
						_outputNativeFileWriter.Write(Constants.NATIVE_FIELD_DELIMITER)
						_outputNativeFileWriter.Write(fileFieldValues(2))
						_outputNativeFileWriter.Write(Constants.NATIVE_FIELD_DELIMITER)
					Else
						_outputNativeFileWriter.Write("")
						_outputNativeFileWriter.Write(Constants.NATIVE_FIELD_DELIMITER)
						_outputNativeFileWriter.Write("")
						_outputNativeFileWriter.Write(Constants.NATIVE_FIELD_DELIMITER)
						_outputNativeFileWriter.Write("")
						_outputNativeFileWriter.Write(Constants.NATIVE_FIELD_DELIMITER)
					End If
				Else
					If docField.FieldCategory = DynamicFields.Types.FieldCategory.FullText AndAlso _fullTextColumnMapsToFileLocation Then
						If Not docField.Value = String.Empty Then
							Dim sr As New System.IO.StreamReader(docField.Value, _extractedTextFileEncoding)
							Dim count As Int32 = 1
							Do
								Dim buff(1000000) As Char
								count = sr.ReadBlock(buff, 0, 1000000)
								If count > 0 Then _outputNativeFileWriter.Write(buff, 0, count)
							Loop Until count = 0
						End If
					ElseIf docField.FieldTypeID = kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Boolean Then
						If docField.Value <> "" Then
							If Boolean.Parse(docField.Value) Then
								_outputNativeFileWriter.Write("1")
							Else
								_outputNativeFileWriter.Write("0")
							End If
						End If
					Else
						_outputNativeFileWriter.Write(docField.Value)
					End If
					_outputNativeFileWriter.Write(Constants.NATIVE_FIELD_DELIMITER)
				End If
			Next
			If _artifactTypeID = 10 Then
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

		Private Function GetObjectFileField() As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo
			For Each field As kCura.EDDS.WebAPI.DocumentManagerBase.Field In _allFields
				If field.FieldTypeID = kCura.DynamicFields.Types.FieldTypeHelper.FieldType.File Then
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
			retval.IsUnicodeEnabled = input.UseUnicodeEncoding
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
			System.Threading.Monitor.Enter(_outputNativeFileWriter)
			System.Threading.Monitor.Enter(_outputCodeFileWriter)
			Dim item As LoadFileFieldMap.LoadFileFieldMapItem
			Dim identityValue As String = String.Empty
			Dim docfield As DocumentField
			For Each item In _fieldMap
				If Not item.DocumentField Is Nothing AndAlso item.DocumentField.FieldCategory = DynamicFields.Types.FieldCategory.Identifier Then
					identityValue = values(item.NativeFileColumnIndex)
				End If
				If Not item.DocumentField Is Nothing Then
					If _keyFieldID > 0 Then
						If item.DocumentField.FieldID = _keyFieldID Then
							identityValue = values(item.NativeFileColumnIndex)
							Exit For
						End If
					Else
						If item.DocumentField.FieldCategory = DynamicFields.Types.FieldCategory.Identifier Then
							identityValue = values(item.NativeFileColumnIndex)
							Exit For
						End If
					End If
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
					If item.DocumentField.FieldTypeID = kCura.DynamicFields.Types.FieldTypeHelper.FieldType.File AndAlso values(item.NativeFileColumnIndex) <> "" AndAlso item.NativeFileColumnIndex <> -1 Then
						Dim localFilePath As String = values(item.NativeFileColumnIndex)
						Dim fileSize As Int64
						If System.IO.File.Exists(localFilePath) Then
							fileSize = Me.GetFileLength(localFilePath)
							Dim fileName As String = System.IO.Path.GetFileName(localFilePath).Replace(ChrW(11), "_")
							Dim location As String
							If _uploader.DestinationFolderPath = "" Then
								location = localFilePath
							Else
								Dim guid As String = _uploader.UploadFile(localFilePath, _caseArtifactID)
								location = _uploader.DestinationFolderPath & _uploader.CurrentDestinationDirectory & "\" & guid
							End If
							location = System.Web.HttpUtility.UrlEncode(location)
							docfield.Value = String.Format("{1}{0}{2}{0}{3}", ChrW(11), fileName, fileSize, location)
						Else
							Throw New System.IO.FileNotFoundException(String.Format("File '{0}' not found.", localFilePath))
						End If
					Else
						MyBase.SetFieldValue(docfield, values, item.NativeFileColumnIndex, identityValue)
					End If
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
			System.Threading.Monitor.Exit(_outputNativeFileWriter)
			System.Threading.Monitor.Exit(_outputCodeFileWriter)
		End Function

#End Region

#Region "Status Window"

		Private Sub WriteStatusLine(ByVal et As kCura.Windows.Process.EventType, ByVal line As String, ByVal lineNumber As Int32)
			line = line & String.Format(" [line {0}]", lineNumber)
			'If Not _status Is Nothing AndAlso Not _status.IsDisposed Then
			'	_status.UpdateStatusWindow(line)
			'End If
			RaiseEvent StatusMessage(New kCura.Windows.Process.StatusEventArgs(et, lineNumber + _offset, _recordCount, line, _currentStatisticsSnapshot))
		End Sub

		Private Sub WriteStatusLine(ByVal et As kCura.Windows.Process.EventType, ByVal line As String)
			WriteStatusLine(et, line, Me.CurrentLineNumber)
		End Sub

		Private Sub WriteFatalError(ByVal lineNumber As Int32, ByVal ex As System.Exception, ByVal sourceLine As String())
			_continue = False
			Me.DoRetryLogic = False
			_uploader.DoRetry = False
			_bcpuploader.DoRetry = False

			RaiseEvent FatalErrorEvent("Error processing line: " + lineNumber.ToString, ex, _runID)
		End Sub

		Private Sub WriteError(ByVal currentLineNumber As Int32, ByVal line As String)
			If _prePushErrorLineNumbersFileName = "" Then _prePushErrorLineNumbersFileName = System.IO.Path.GetTempFileName
			Dim sw As New System.IO.StreamWriter(_prePushErrorLineNumbersFileName, True, System.Text.Encoding.Default)
			sw.WriteLine(currentLineNumber)
			sw.Flush()
			sw.Close()
			Dim ht As New System.Collections.Hashtable
			ht.Add("Message", line)
			ht.Add("Line Number", currentLineNumber)
			RaiseReportError(ht, currentLineNumber, "", "client")
			WriteStatusLine(kCura.Windows.Process.EventType.Error, line)
		End Sub

		Private Sub RaiseReportError(ByVal row As System.Collections.Hashtable, ByVal lineNumber As Int32, ByVal identifier As String, ByVal type As String)
			_errorCount += 1
			If _errorMessageFileLocation = "" Then _errorMessageFileLocation = System.IO.Path.GetTempFileName
			Dim errorMessageFileWriter As New System.IO.StreamWriter(_errorMessageFileLocation, True, System.Text.Encoding.Default)
			If _errorCount < Me.MaxNumberOfErrorsInGrid Then
				RaiseEvent ReportErrorEvent(row)
			ElseIf _errorCount = Me.MaxNumberOfErrorsInGrid Then
				Dim moretobefoundMessage As New System.Collections.Hashtable
				moretobefoundMessage.Add("Message", "Maximum number of errors for display reached.  Export errors to view full list.")
				RaiseEvent ReportErrorEvent(moretobefoundMessage)
			End If
			errorMessageFileWriter.WriteLine("""" & row("Line Number").ToString & """,""" & row("Message").ToString & """,""" & identifier & """,""" & type & """")
			errorMessageFileWriter.Close()
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

		Private Sub _uploader_UploadStatusEvent(ByVal s As String) Handles _uploader.UploadStatusEvent, _bcpuploader.UploadStatusEvent
			WriteStatusLine(kCura.Windows.Process.EventType.Status, s)
		End Sub

		Private Sub _uploader_UploadWarningEvent(ByVal s As String) Handles _uploader.UploadWarningEvent, _bcpuploader.UploadWarningEvent
			WriteStatusLine(kCura.Windows.Process.EventType.Warning, s)
		End Sub

#End Region

#Region "Public Events"

		Public Event FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception, ByVal runID As String)
		Public Event StatusMessage(ByVal args As kCura.Windows.Process.StatusEventArgs)
		Public Event EndFileImport(ByVal runID As String)
		Public Event StartFileImport()
		Public Event UploadModeChangeEvent(ByVal mode As String, ByVal isBulkEnabled As Boolean)
		Public Event ReportErrorEvent(ByVal row As System.Collections.IDictionary)
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

		Private Sub _uploader_UploadModeChangeEvent(ByVal mode As String, ByVal isBulkEnabled As Boolean) Handles _uploader.UploadModeChangeEvent
			RaiseEvent UploadModeChangeEvent(mode, _bcpuploader.IsBulkEnabled)
		End Sub

		Private Sub _bcpuploader_UploadModeChangeEvent(ByVal mode As String, ByVal isBulkEnabled As Boolean) Handles _bcpuploader.UploadModeChangeEvent
			RaiseEvent UploadModeChangeEvent(_uploader.UploaderType.ToString, isBulkEnabled)
		End Sub

		Private Sub _processController_HaltProcessEvent(ByVal processID As System.Guid) Handles _processController.HaltProcessEvent
			If processID.ToString = _processID.ToString Then
				_continue = False
				_lineCounter.StopCounting()
				_uploader.DoRetry = False
				_bcpuploader.DoRetry = False
			End If
		End Sub

		Private Sub BuildErrorLinesFile()
			RaiseEvent StatusMessage(New kCura.Windows.Process.StatusEventArgs(Windows.Process.EventType.Status, Me.CurrentLineNumber, Me.CurrentLineNumber, "Generating error line file.", _currentStatisticsSnapshot))
			Dim allErrors As New kCura.Utility.GenericCsvReader(_errorMessageFileLocation, System.Text.Encoding.Default, True)
			Dim clientErrors As System.IO.StreamReader
			'Me.Reader.BaseStream.Seek(0, IO.SeekOrigin.Begin)
			Me.Reader = New System.IO.StreamReader(_filePath, _sourceFileEncoding, True)
			Me.ResetLineCounter()
			If _prePushErrorLineNumbersFileName = "" Then
				clientErrors = New System.IO.StreamReader(System.IO.Path.GetTempFileName, System.Text.Encoding.Default)
			Else
				clientErrors = New System.IO.StreamReader(_prePushErrorLineNumbersFileName, System.Text.Encoding.Default)
			End If
			Dim advanceClient As Boolean = True
			Dim advanceAll As Boolean = True
			Dim allErrorsLine As Int32
			Dim clientErrorsLine As Int32
			_errorLinesFileLocation = System.IO.Path.GetTempFileName
			Dim sw As New System.IO.StreamWriter(_errorLinesFileLocation, False, _sourceFileEncoding)
			If _settings.FirstLineContainsHeaders Then
				sw.WriteLine(Me.ToDelimetedLine(Me.GetLine))
			End If
			If _prePushErrorLineNumbersFileName = "" Then
				clientErrorsLine = Int32.MaxValue
			Else
				clientErrorsLine = Int32.Parse(clientErrors.ReadLine)
			End If
			If Not allErrors.Eof Then
				Dim e As String() = allErrors.ReadLine
				If e(3) <> "client" Then
					allErrorsLine = Int32.Parse(e(0))
				Else
					While Not e Is Nothing AndAlso e(3) = "client"
						e = allErrors.ReadLine
					End While
					If e Is Nothing Then
						allErrorsLine = Int32.MaxValue
					Else
						allErrorsLine = Int32.Parse(e(0))
					End If
				End If
			Else
				allErrorsLine = Int32.MaxValue
			End If
			Dim line As String()
			Dim currentLine As String()
			Dim continue As Boolean = True And Not Me.Reader.Peek = -1
			While continue
				If Me.CurrentLineNumber < System.Math.Min(clientErrorsLine, allErrorsLine) Then
					If Me.Reader.Peek = -1 Then
						continue = False
					Else
						line = Me.GetLine()
					End If
				Else
					sw.WriteLine(Me.ToDelimetedLine(line))
					If Me.CurrentLineNumber = clientErrorsLine Then
						If clientErrors.Peek = -1 Then
							clientErrorsLine = Int32.MaxValue
						Else
							clientErrorsLine = Int32.Parse(clientErrors.ReadLine)
						End If
					End If
					If Me.CurrentLineNumber = allErrorsLine Then
						If allErrors.Eof Then
							allErrorsLine = Int32.MaxValue
						Else
							allErrorsLine = Int32.Parse(allErrors.ReadLine(0))
						End If
					End If
				End If

				continue = ((Not allErrors.Eof Or clientErrors.Peek <> -1 Or Me.CurrentLineNumber <= System.Math.Min(clientErrorsLine, allErrorsLine)) And continue)
			End While
			Me.Close()
			sw.Close()
			allErrors.Close()
			clientErrors.Close()
			RaiseEvent StatusMessage(New kCura.Windows.Process.StatusEventArgs(Windows.Process.EventType.Status, Me.CurrentLineNumber, Me.CurrentLineNumber, "Error line file generation complete.", _currentStatisticsSnapshot))
		End Sub

		Private Sub _processController_ExportServerErrors(ByVal exportLocation As String) Handles _processController.ExportServerErrorsEvent
			Me.BuildErrorLinesFile()
			Dim rootFileName As String = _filePath
			Dim defaultExtension As String
			If Not rootFileName.IndexOf(".") = -1 Then
				defaultExtension = rootFileName.Substring(rootFileName.LastIndexOf("."))
				rootFileName = rootFileName.Substring(0, rootFileName.LastIndexOf("."))
			Else
				defaultExtension = ".txt"
			End If
			rootFileName.Trim("\"c)
			If rootFileName.IndexOf("\") <> -1 Then
				rootFileName = rootFileName.Substring(rootFileName.LastIndexOf("\") + 1)
			End If
			Dim rootFilePath As String = exportLocation & rootFileName
			Dim datetimeNow As System.DateTime = System.DateTime.Now
			Dim errorFilePath As String = rootFilePath & "_ErrorLines_" & datetimeNow.Ticks & defaultExtension
			Dim errorReportPath As String = rootFilePath & "_ErrorReport_" & datetimeNow.Ticks & ".csv"
			Try
				System.IO.File.Copy(_errorLinesFileLocation, errorFilePath)
			Catch
				System.IO.File.Copy(_errorLinesFileLocation, errorFilePath)
			End Try
			Try
				System.IO.File.Copy(_errorMessageFileLocation, errorReportPath)
			Catch
				System.IO.File.Copy(_errorMessageFileLocation, errorReportPath)
			End Try
			_errorMessageFileLocation = ""
		End Sub

		Private Sub _processController_ExportErrorReportEvent(ByVal exportLocation As String) Handles _processController.ExportErrorReportEvent
			If _errorMessageFileLocation Is Nothing OrElse _errorMessageFileLocation = "" Then Exit Sub
			Try
				System.IO.File.Copy(_errorMessageFileLocation, exportLocation, True)
			Catch ex As Exception
				System.IO.File.Copy(_errorMessageFileLocation, exportLocation, True)
			End Try
		End Sub

		Private Sub _processController_ExportErrorFileEvent(ByVal exportLocation As String) Handles _processController.ExportErrorFileEvent
			'_errorLinesFileLocation()
			If _errorMessageFileLocation Is Nothing OrElse _errorMessageFileLocation = "" Then Exit Sub
			If _errorLinesFileLocation Is Nothing OrElse _errorLinesFileLocation = "" OrElse Not System.IO.File.Exists(_errorLinesFileLocation) Then
				Me.BuildErrorLinesFile()
			End If
			Try
				System.IO.File.Copy(_errorLinesFileLocation, exportLocation, True)
			Catch ex As Exception
				System.IO.File.Copy(_errorLinesFileLocation, exportLocation, True)
			End Try
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

		Private Sub IoWarningHandler(ByVal e As IoWarningEventArgs)
			MyBase.RaiseIoWarning(e)
		End Sub

		Private Sub ManageErrors(ByVal artifactTypeID As Int32)
			If Not _bulkImportManager.NativeRunHasErrors(_caseInfo.ArtifactID, _runID) Then Exit Sub
			Dim sr As kCura.Utility.GenericCsvReader
			Try
				With _bulkImportManager.GenerateNativeErrorFiles(_caseInfo.ArtifactID, _runID, artifactTypeID, True, _keyFieldID)
					Me.WriteStatusLine(Windows.Process.EventType.Status, "Retrieving errors from server")
					Dim downloader As New FileDownloader(DirectCast(_bulkImportManager.Credentials, System.Net.NetworkCredential), _caseInfo.DocumentPath, _caseInfo.DownloadHandlerURL, _bulkImportManager.CookieContainer, kCura.WinEDDS.Service.Settings.AuthenticationToken)
					Dim errorsLocation As String = System.IO.Path.GetTempFileName
					downloader.MoveTempFileToLocal(errorsLocation, .LogKey, _caseInfo)
					sr = New kCura.Utility.GenericCsvReader(errorsLocation, True)
					AddHandler sr.IoWarningEvent, AddressOf Me.IoWarningHandler
					Dim line As String() = sr.ReadLine
					While Not line Is Nothing
						_errorCount += 1
						Dim ht As New System.Collections.Hashtable
						ht.Add("Message", line(1))
						ht.Add("Identifier", line(2))
						ht.Add("Line Number", Int32.Parse(line(0)))
						RaiseReportError(ht, Int32.Parse(line(0)), line(2), "server")
						RaiseEvent StatusMessage(New kCura.Windows.Process.StatusEventArgs(Windows.Process.EventType.Error, Int32.Parse(line(0)) - 1, _recordCount, "[Line " & line(0) & "]" & line(1), _currentStatisticsSnapshot))
						line = sr.ReadLine
					End While
					RemoveHandler sr.IoWarningEvent, AddressOf Me.IoWarningHandler
				End With
			Catch ex As Exception
				Try
					sr.Close()
					RemoveHandler sr.IoWarningEvent, AddressOf Me.IoWarningHandler
				Catch
				End Try
				Throw
			End Try
		End Sub

		Private Sub _processController_ParentFormClosingEvent(ByVal processID As Guid) Handles _processController.ParentFormClosingEvent
			If processID.ToString = _processID.ToString Then CleanupTempTables()
		End Sub

		Private Sub CleanupTempTables()
			If Not _runID Is Nothing AndAlso _runID <> "" Then
				Try
					_bulkImportManager.DisposeTempTables(_caseInfo.ArtifactID, _runID)
				Catch
				End Try
			End If
		End Sub

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