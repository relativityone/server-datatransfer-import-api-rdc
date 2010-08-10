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
		Private _genericTimestamp As System.DateTime
		Private _number As Int64 = 0
		Private _destinationFolderColumnIndex As Int32 = -1
		Private _folderCache As FolderCache
		Private _fullTextField As kCura.EDDS.WebAPI.DocumentManagerBase.Field
		Private _defaultDestinationFolderPath As String = ""
		Private _defaultTextFolderPath As String = ""
		Private _copyFileToRepository As Boolean
		Private _oixFileLookup As System.Collections.Specialized.HybridDictionary
		Private _fieldArtifactIds As Int32()
		Private _outputNativeFileWriter As System.IO.StreamWriter
		Private _outputCodeFileWriter As System.IO.StreamWriter
		Private _outputObjectFileWriter As System.IO.StreamWriter
		Private _caseInfo As kCura.EDDS.Types.CaseInfo

		Private _runID As String = ""
		Private _uploadKey As String

		Private _outputNativeFilePath As String = System.IO.Path.GetTempFileName
		Private _outputCodeFilePath As String = System.IO.Path.GetTempFileName
		Private _outputObjectFilePath As String = System.IO.Path.GetTempFileName
		Private _filePath As String
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
		Private _unmappedRelationalFields As System.Collections.ArrayList

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

		Public ReadOnly Property UnmappedRelationalFields() As System.Collections.ArrayList
			Get
				If _unmappedRelationalFields Is Nothing Then
					Dim mappedRelationalFieldIds As New System.Collections.ArrayList
					For Each item As LoadFileFieldMap.LoadFileFieldMapItem In _fieldMap
						If Not item.DocumentField Is Nothing AndAlso item.DocumentField.FieldCategory = DynamicFields.Types.FieldCategory.Relational Then
							mappedRelationalFieldIds.Add(item.DocumentField.FieldID)
						End If
					Next
					_unmappedRelationalFields = New System.Collections.ArrayList
					For Each field As kCura.EDDS.WebAPI.DocumentManagerBase.Field In Me.AllFields(_artifactTypeID)
						If field.FieldCategory = EDDS.WebAPI.DocumentManagerBase.FieldCategory.Relational And Not mappedRelationalFieldIds.Contains(field.ArtifactID) Then
							_unmappedRelationalFields.Add(field)
						End If
					Next
				End If
				Return _unmappedRelationalFields
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
				If args.ArtifactTypeID <> kCura.EDDS.Types.ArtifactType.Document Then
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
			If autoDetect Then _parentFolderDTO = _folderManager.Read(args.CaseInfo.ArtifactID, args.CaseInfo.RootFolderID)
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

		Public Function GetColumnNames(ByVal args As Object) As String()
			Dim columnNames As String() = _artifactReader.GetColumnNames(args)
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
			Return columnNames
		End Function

		Public Sub WriteCodeLineToTempFile(ByVal documentIdentifier As String, ByVal codeArtifactID As Int32, ByVal codeTypeID As Int32)
			_outputCodeFileWriter.WriteLine(String.Format("{1}{0}{2}{0}{3}{0}", Constants.NATIVE_FIELD_DELIMITER, documentIdentifier, codeArtifactID, codeTypeID))
		End Sub

		Public Sub WriteObjectLineToTempFile(ByVal ownerIdentifier As String, ByVal objectName As String, ByVal artifactID As Int32, ByVal objectTypeArtifactID As Int32, ByVal fieldID As Int32)
			_outputObjectFileWriter.WriteLine(String.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}", Constants.NATIVE_FIELD_DELIMITER, ownerIdentifier, objectName, artifactID, objectTypeArtifactID, fieldID))
		End Sub

#End Region

#Region "Main"

		Public Function ReadFile(ByVal path As String) As Object
			Dim line As Api.ArtifactFieldCollection
			'Dim path As String = _path
			_filePath = path
			_start = System.DateTime.Now
			_timekeeper.MarkStart("TOTAL")
			Try
				RaiseEvent StartFileImport()
				_timekeeper.MarkStart("ReadFile_InitializeMembers")
				Dim validateBcp As FileUploadReturnArgs = _bcpuploader.ValidateBcpPath(_caseInfo.ArtifactID, _outputNativeFilePath)
				If validateBcp.Type = FileUploadReturnArgs.FileUploadReturnType.UploadError And Not Config.EnableSingleModeImport Then
					Throw New BcpPathAccessException(validateBcp.Value)
				Else
					RaiseEvent UploadModeChangeEvent(_uploader.UploaderType.ToString, _bcpuploader.IsBulkEnabled)
				End If
				InitializeMembers(path)
				_processedDocumentIdentifiers = New Collections.Specialized.NameValueCollection
				_timekeeper.MarkEnd("ReadFile_InitializeMembers")
				_timekeeper.MarkStart("ReadFile_ProcessDocuments")
				_columnHeaders = _artifactReader.GetColumnNames(_settings)
				If _firstLineContainsColumnNames Then _offset = -1
				Dim isError As Boolean = False
				While _continue AndAlso _artifactReader.HasMoreRecords
					Try
						If Me.CurrentLineNumber < _startLineNumber Then
							Me.AdvanceLine()
						Else
							_timekeeper.MarkStart("ReadFile_GetLine")
							_statistics.DocCount += 1
							line = _artifactReader.ReadArtifact
							_timekeeper.MarkEnd("ReadFile_GetLine")
							Dim lineStatus As Int32 = 0
							'If line.Count <> _columnHeaders.Length Then
							'	lineStatus += ImportStatus.ColumnMismatch								 'Throw New ColumnCountMismatchException(Me.CurrentLineNumber, _columnHeaders.Length, line.Length)
							'End If

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
							WriteFatalError(Me.CurrentLineNumber, ex)
						Else
							WriteError(Me.CurrentLineNumber, ex.Message)
						End If
					Catch ex As kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
						WriteError(Me.CurrentLineNumber, ex.Message)
					Catch ex As System.IO.FileNotFoundException
						WriteError(Me.CurrentLineNumber, ex.Message)
					Catch ex As System.Exception
						WriteFatalError(Me.CurrentLineNumber, ex)
					End Try
				End While
				_timekeeper.MarkEnd("ReadFile_ProcessDocuments")
				_timekeeper.MarkStart("ReadFile_OtherFinalization")
				Me.PushNativeBatch(True)
				RaiseEvent EndFileImport(_runID)
				WriteEndImport("Finish")
				_artifactReader.Close()
				_timekeeper.MarkEnd("ReadFile_OtherFinalization")
				_timekeeper.MarkStart("ReadFile_CleanupTempTables")
				Me.CleanupTempTables()
				_timekeeper.MarkEnd("ReadFile_CleanupTempTables")
				_timekeeper.MarkEnd("TOTAL")
				_timekeeper.GenerateCsvReportItemsAsRows("_winedds", "C:\")
				Return True
			Catch ex As System.Exception
				WriteFatalError(Me.CurrentLineNumber, ex)
			End Try
		End Function

		Private Sub InitializeMembers(ByVal path As String)
			_recordCount = _artifactReader.CountRecords
			Me.InitializeFolderManagement()
			Me.InitializeFieldIdList()
			kCura.Utility.File.Delete(_outputNativeFilePath)
			kCura.Utility.File.Delete(_outputCodeFilePath)
			kCura.Utility.File.Delete(_outputObjectFilePath)
			_outputNativeFileWriter = New System.IO.StreamWriter(_outputNativeFilePath, False, System.Text.Encoding.Unicode)
			_outputCodeFileWriter = New System.IO.StreamWriter(_outputCodeFilePath, False, System.Text.Encoding.Unicode)
			_outputObjectFileWriter = New System.IO.StreamWriter(_outputObjectFilePath, False, System.Text.Encoding.Unicode)
			RaiseEvent StatusMessage(New kCura.Windows.Process.StatusEventArgs(Windows.Process.EventType.ResetStartTime, 0, _recordCount, "Reset time for import rolling average", Nothing))
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

		Private Function ManageDocument(ByVal record As Api.ArtifactFieldCollection, ByVal lineStatus As Int32) As String
			Dim filename As String = Nothing
			Dim fileGuid As String = String.Empty
			Dim uploadFile As Boolean = record.FieldList(DynamicFields.Types.FieldTypeHelper.FieldType.File).Length > 0 AndAlso Not record.FieldList(DynamicFields.Types.FieldTypeHelper.FieldType.File)(0).Value Is Nothing
			Dim fileExists As Boolean
			Dim fieldCollection As New DocumentFieldCollection
			Dim identityValue As String = String.Empty
			Dim markUploadStart As DateTime = DateTime.Now
			Dim parentFolderID As Int32
			Dim md5hash As String = ""
			Dim fullFilePath As String = ""
			Dim oixFileIdData As OI.FileID.FileIDData = Nothing
			Dim destinationVolume As String = Nothing
			_timekeeper.MarkStart("ManageDocument_Filesystem")
			If uploadFile AndAlso _artifactTypeID = kCura.EDDS.Types.ArtifactType.Document Then
				filename = record.FieldList(DynamicFields.Types.FieldTypeHelper.FieldType.File)(0).Value.ToString
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
				If _artifactTypeID = kCura.EDDS.Types.ArtifactType.Document Then
					parentFolderID = _folderCache.FolderID(Me.CleanDestinationFolderPath(record.FieldList(DynamicFields.Types.FieldCategory.ParentArtifact)(0).Value.ToString))
				Else
					Dim textIdentifier As String = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(kCura.Utility.NullableTypesHelper.DBNullString(record.FieldList(DynamicFields.Types.FieldCategory.ParentArtifact)(0).Value.ToString))
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
				If _artifactTypeID = kCura.EDDS.Types.ArtifactType.Document OrElse _parentArtifactTypeID = kCura.EDDS.Types.ArtifactType.Case Then
					parentFolderID = _folderID
				Else
					parentFolderID = -1
				End If
			End If
			_timekeeper.MarkEnd("ManageDocument_Folder")
			Dim markPrepareFields As DateTime = DateTime.Now
			identityValue = PrepareFieldCollectionAndExtractIdentityValue(record)
			If identityValue = String.Empty Then
				'lineStatus += ImportStatus.EmptyIdentifier				'
				Throw New IdentityValueNotSetException
			ElseIf Not _processedDocumentIdentifiers(identityValue) Is Nothing Then
				'lineStatus += ImportStatus.IdentifierOverlap				'	
				Throw New IdentifierOverlapException(identityValue, _processedDocumentIdentifiers(identityValue))
			End If
			Dim metadoc As New MetaDocument(fileGuid, identityValue, fileExists AndAlso uploadFile AndAlso (fileGuid <> String.Empty OrElse Not _copyFileToRepository), filename, fullFilePath, uploadFile, CurrentLineNumber, parentFolderID, md5hash, record, oixFileIdData, lineStatus, destinationVolume)
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
				WriteFatalError(metaDoc.LineNumber, ex)
			End Try
			_timekeeper.MarkStart("ManageDocumentMetadata_ProgressEvent")
			WriteStatusLine(Windows.Process.EventType.Progress, String.Format("Document '{0}' processed.", metaDoc.IdentityValue), metaDoc.LineNumber)
			_timekeeper.MarkEnd("ManageDocumentMetadata_ProgressEvent")
		End Sub

		Private Function BulkImport(ByVal settings As kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Dim tries As Int32 = kCura.Utility.Config.Settings.IoErrorNumberOfRetries
			While tries > 0
				Try
					If TypeOf settings Is kCura.EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo Then
						Return _bulkImportManager.BulkImportObjects(_caseInfo.ArtifactID, DirectCast(settings, kCura.EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo), _copyFileToRepository)
					Else
						Return _bulkImportManager.BulkImportNative(_caseInfo.ArtifactID, settings, _copyFileToRepository)
					End If
				Catch ex As Exception
					tries -= 1
					If tries = 0 OrElse TypeOf ex Is kCura.WinEDDS.Service.BulkImportManager.BulkImportSqlException OrElse _continue = False Then
						Throw
					ElseIf tries < kCura.Utility.Config.Settings.IoErrorNumberOfRetries Then
						Me.RaiseIoWarning(New kCura.Utility.DelimitedFileImporter.IoWarningEventArgs(kCura.Utility.Config.Settings.IoErrorWaitTimeInSeconds, ex, Me.CurrentLineNumber))
						System.Threading.Thread.CurrentThread.Join(1000 * kCura.Utility.Config.Settings.IoErrorWaitTimeInSeconds)
					End If
				End Try
			End While
			Return Nothing
		End Function

		Private Function GetSettingsObject() As kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo
			If _artifactTypeID = kCura.EDDS.Types.ArtifactType.Document Then
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
			_outputObjectFileWriter.Close()
			Dim start As Int64 = System.DateTime.Now.Ticks
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

			Dim objectbcp As FileUploadReturnArgs = _bcpuploader.UploadBcpFile(_caseInfo.ArtifactID, _outputObjectFilePath)
			If objectbcp Is Nothing Then Return Nothing
			Dim objectFileUploadKey As String = objectbcp.Value

			If _artifactTypeID = kCura.EDDS.Types.ArtifactType.Document Then
				settings.Repository = _defaultDestinationFolderPath
				If settings.Repository = "" Then settings.Repository = _caseInfo.DocumentPath
			Else
				settings.Repository = _caseInfo.DocumentPath
			End If
			If uploadBcp.Type = FileUploadReturnArgs.FileUploadReturnType.UploadError Then
				If Config.EnableSingleModeImport Then
					RaiseEvent UploadModeChangeEvent(_uploader.UploaderType.ToString, _bcpuploader.IsBulkEnabled)
					_uploader.DestinationFolderPath = settings.Repository
					_bcpuploader.DestinationFolderPath = settings.Repository
					nativeFileUploadKey = _bcpuploader.UploadFile(_outputNativeFilePath, _caseInfo.ArtifactID)
					codeFileUploadKey = _bcpuploader.UploadFile(_outputCodeFilePath, _caseInfo.ArtifactID)
					objectFileUploadKey = _bcpuploader.UploadFile(_outputObjectFilePath, _caseInfo.ArtifactID)
					settings.UseBulkDataImport = False
				Else
					Throw New BcpPathAccessException(uploadBcp.Value)
				End If
			End If
			_statistics.MetadataTime += System.Math.Max((System.DateTime.Now.Ticks - start), 1)
			_statistics.MetadataBytes += (Me.GetFileLength(_outputCodeFilePath) + Me.GetFileLength(_outputNativeFilePath) + Me.GetFileLength(_outputObjectFilePath))
			settings.RunID = _runID
			settings.CodeFileName = codeFileUploadKey
			settings.DataFileName = nativeFileUploadKey
			settings.ObjectFileName = objectFileUploadKey
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
			kCura.Utility.File.Delete(_outputObjectFilePath)
			_currentStatisticsSnapshot = _statistics.ToDictionary
			_statisticsLastUpdated = System.DateTime.Now
			If Not lastRun Then
				_outputNativeFileWriter = New System.IO.StreamWriter(_outputNativeFilePath, False, System.Text.Encoding.Unicode)
				_outputCodeFileWriter = New System.IO.StreamWriter(_outputCodeFilePath, False, System.Text.Encoding.Unicode)
				_outputObjectFileWriter = New System.IO.StreamWriter(_outputObjectFilePath, False, System.Text.Encoding.Unicode)
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
			If artifactTypeID = kCura.EDDS.Types.ArtifactType.Document Then
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
			Return ManageDocumentLine(mdoc.IdentityValue, mdoc.FileGuid <> String.Empty AndAlso extractText, mdoc.Filename, mdoc.FileGuid, mdoc)
		End Function

		Private Function ManageDocumentLine(ByVal identityValue As String, ByVal extractText As Boolean, ByVal filename As String, ByVal fileguid As String, ByVal mdoc As MetaDocument) As Int32
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
			For Each field As Api.ArtifactField In mdoc.Record
				If field.Type = kCura.DynamicFields.Types.FieldTypeHelper.FieldType.MultiCode OrElse field.Type = kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Code Then
					_outputNativeFileWriter.Write(field.Value)
					_outputNativeFileWriter.Write(Constants.NATIVE_FIELD_DELIMITER)
				ElseIf field.Type = kCura.DynamicFields.Types.FieldTypeHelper.FieldType.File AndAlso _artifactTypeID <> kCura.EDDS.Types.ArtifactType.Document Then
					Dim fileFieldValues() As String = System.Web.HttpUtility.UrlDecode(field.ValueAsString).Split(Chr(11))
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
				ElseIf field.Type = kCura.DynamicFields.Types.FieldTypeHelper.FieldType.File AndAlso _artifactTypeID = kCura.EDDS.Types.ArtifactType.Document Then
					'do nothing
				ElseIf field.Category = DynamicFields.Types.FieldCategory.ParentArtifact Then
					'do nothing
				Else
					If field.Category = DynamicFields.Types.FieldCategory.FullText AndAlso _fullTextColumnMapsToFileLocation Then
						If Not field.ValueAsString = String.Empty Then
							Dim sr As New System.IO.StreamReader(field.ValueAsString, _extractedTextFileEncoding)
							Dim count As Int32 = 1
							Do
								Dim buff(1000000) As Char
								count = sr.ReadBlock(buff, 0, 1000000)
								If count > 0 Then _outputNativeFileWriter.Write(buff, 0, count)
							Loop Until count = 0
						End If
					ElseIf field.Type = kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Boolean Then
						If field.ValueAsString <> "" Then
							If Boolean.Parse(field.ValueAsString) Then
								_outputNativeFileWriter.Write("1")
							Else
								_outputNativeFileWriter.Write("0")
							End If
						End If
					Else
						_outputNativeFileWriter.Write(field.Value)
					End If
					_outputNativeFileWriter.Write(Constants.NATIVE_FIELD_DELIMITER)
				End If
			Next
			If _artifactTypeID = kCura.EDDS.Types.ArtifactType.Document Then
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
			If Not input.CodeTypeID Is Nothing Then retval.CodeTypeID = input.CodeTypeID.Value
			retval.DisplayName = input.DisplayName
			If Not input.MaxLength Is Nothing Then retval.TextLength = input.MaxLength.Value
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

		Private Function PrepareFieldCollectionAndExtractIdentityValue(ByVal record As Api.ArtifactFieldCollection) As String
			System.Threading.Monitor.Enter(_outputNativeFileWriter)
			System.Threading.Monitor.Enter(_outputCodeFileWriter)
			System.Threading.Monitor.Enter(_outputObjectFileWriter)
			Dim item As LoadFileFieldMap.LoadFileFieldMapItem
			Dim identityValue As String = String.Empty
			Dim keyField As Api.ArtifactField
			If _keyFieldID > 0 Then
				keyField = record(_keyFieldID)
			Else
				keyField = record.IdentifierField
			End If
			If Not keyField Is Nothing AndAlso Not keyField.Value Is Nothing Then identityValue = keyField.Value.ToString
			If identityValue Is Nothing OrElse identityValue = String.Empty Then Throw New IdentityValueNotSetException
			If Not _processedDocumentIdentifiers(identityValue) Is Nothing Then Throw New IdentifierOverlapException(identityValue, _processedDocumentIdentifiers(identityValue))
			For Each item In _fieldMap
				If _firstTimeThrough Then
					If item.DocumentField Is Nothing Then
						WriteStatusLine(Windows.Process.EventType.Warning, String.Format("File column '{0}' will be unmapped", item.NativeFileColumnIndex + 1), 0)
					End If
					If item.NativeFileColumnIndex = -1 Then
						WriteStatusLine(Windows.Process.EventType.Warning, String.Format("Field '{0}' will be unmapped", item.DocumentField.FieldName), 0)
					End If
				End If
				If Not item.DocumentField Is Nothing Then
					If item.DocumentField.FieldTypeID = kCura.DynamicFields.Types.FieldTypeHelper.FieldType.File Then
						Me.ManageFileField(record(item.DocumentField.FieldID))
					Else
						MyBase.SetFieldValue(record(item.DocumentField.FieldID), item.NativeFileColumnIndex, False, identityValue, 0)
					End If
				End If
			Next
			For Each fieldDTO As kCura.EDDS.WebAPI.DocumentManagerBase.Field In Me.UnmappedRelationalFields
				Dim field As New Api.ArtifactField(fieldDTO)
				field.Value = identityValue
				Me.SetFieldValue(field, -1, False, identityValue, 0)
			Next
			_firstTimeThrough = False
			System.Threading.Monitor.Exit(_outputNativeFileWriter)
			System.Threading.Monitor.Exit(_outputCodeFileWriter)
			Return identityValue
		End Function

		Private Sub ManageFileField(ByVal fileField As Api.ArtifactField)
			If fileField Is Nothing Then Exit Sub
			If fileField.Value Is Nothing Then Exit Sub
			If fileField.Value.ToString = String.Empty Then Exit Sub
			Dim localFilePath As String = fileField.Value.ToString
			Dim fileSize As Int64
			If System.IO.File.Exists(localFilePath) Then
				fileSize = Me.GetFileLength(localFilePath)
				Dim fileName As String = System.IO.Path.GetFileName(localFilePath).Replace(ChrW(11), "_")
				Dim location As String
				If _uploader.DestinationFolderPath = "" Then
					location = localFilePath
				Else
					Dim start As Int64 = System.DateTime.Now.Ticks
					_statistics.FileBytes += Me.GetFileLength(localFilePath)
					Dim guid As String = _uploader.UploadFile(localFilePath, _caseArtifactID)
					location = _uploader.DestinationFolderPath & _uploader.CurrentDestinationDirectory & "\" & guid
					Dim updateCurrentStats As Boolean = (start - _statisticsLastUpdated.Ticks) > 10000000
					_statistics.FileTime += System.DateTime.Now.Ticks - start
					If updateCurrentStats Then
						_currentStatisticsSnapshot = _statistics.ToDictionary
						_statisticsLastUpdated = New System.DateTime(start)
					End If

				End If
				location = System.Web.HttpUtility.UrlEncode(location)
				fileField.Value = String.Format("{1}{0}{2}{0}{3}", ChrW(11), fileName, fileSize, location)
			Else
				Throw New System.IO.FileNotFoundException(String.Format("File '{0}' not found.", localFilePath))
			End If
		End Sub

#End Region


#Region "Status Window"

		Private Sub WriteStatusLine(ByVal et As kCura.Windows.Process.EventType, ByVal line As String, ByVal lineNumber As Int32)
			line = line & String.Format(" [line {0}]", lineNumber)
			RaiseEvent StatusMessage(New kCura.Windows.Process.StatusEventArgs(et, lineNumber + _offset, _recordCount, line, _currentStatisticsSnapshot))
		End Sub

		Private Sub WriteStatusLine(ByVal et As kCura.Windows.Process.EventType, ByVal line As String)
			WriteStatusLine(et, line, Me.CurrentLineNumber)
		End Sub

		Private Sub WriteFatalError(ByVal lineNumber As Int32, ByVal ex As System.Exception)
			_continue = False
			_artifactReader.OnFatalErrorState()
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
			If _errorCount < BulkLoadFileImporter.MaxNumberOfErrorsInGrid Then
				RaiseEvent ReportErrorEvent(row)
			ElseIf _errorCount = BulkLoadFileImporter.MaxNumberOfErrorsInGrid Then
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
		Public Event DataSourcePrepEvent(ByVal e As Api.DataSourcePrepEventArgs)

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
				_artifactReader.Halt()
				_uploader.DoRetry = False
				_bcpuploader.DoRetry = False
			End If
		End Sub

		Private Sub _processController_ExportServerErrors(ByVal exportLocation As String) Handles _processController.ExportServerErrorsEvent
			_errorLinesFileLocation = _artifactReader.ManageErrorRecords(_errorMessageFileLocation, _prePushErrorLineNumbersFileName)
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
			If Not _errorLinesFileLocation Is Nothing AndAlso Not _errorLinesFileLocation = String.Empty AndAlso System.IO.File.Exists(_errorLinesFileLocation) Then
				Try
					System.IO.File.Copy(_errorLinesFileLocation, errorFilePath)
				Catch
					System.IO.File.Copy(_errorLinesFileLocation, errorFilePath)
				End Try
			End If
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
				_errorLinesFileLocation = _artifactReader.ManageErrorRecords(_errorMessageFileLocation, _prePushErrorLineNumbersFileName)
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

#End Region

		Private Sub _artifactReader_DataSourcePrep(ByVal e As Api.DataSourcePrepEventArgs) Handles _artifactReader.DataSourcePrep
			RaiseEvent DataSourcePrepEvent(e)
		End Sub

		Private Sub _artifactReader_StatusMessage(ByVal message As String) Handles _artifactReader.StatusMessage
			RaiseEvent StatusMessage(New kCura.Windows.Process.StatusEventArgs(Windows.Process.EventType.Status, _artifactReader.CurrentLineNumber, _recordCount, message, False, _currentStatisticsSnapshot))
		End Sub

		Private Sub IoWarningHandler(ByVal e As IoWarningEventArgs)
			MyBase.RaiseIoWarning(e)
		End Sub

		Private Sub ManageErrors(ByVal artifactTypeID As Int32)
			If Not _bulkImportManager.NativeRunHasErrors(_caseInfo.ArtifactID, _runID) Then Exit Sub
			Dim sr As kCura.Utility.GenericCsvReader = Nothing
			Dim downloader As FileDownloader = Nothing
			Try
				With _bulkImportManager.GenerateNativeErrorFiles(_caseInfo.ArtifactID, _runID, artifactTypeID, True, _keyFieldID)
					Me.WriteStatusLine(Windows.Process.EventType.Status, "Retrieving errors from server")
					downloader = New FileDownloader(DirectCast(_bulkImportManager.Credentials, System.Net.NetworkCredential), _caseInfo.DocumentPath, _caseInfo.DownloadHandlerURL, _bulkImportManager.CookieContainer, kCura.WinEDDS.Service.Settings.AuthenticationToken)
					AddHandler downloader.UploadStatusEvent, AddressOf _uploader_UploadStatusEvent
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
					RemoveHandler downloader.UploadStatusEvent, AddressOf _uploader_UploadStatusEvent
				End With
			Catch ex As Exception
				Try
					If downloader IsNot Nothing Then RemoveHandler downloader.UploadStatusEvent, AddressOf _uploader_UploadStatusEvent
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

		Protected Overrides Function GetSingleCodeValidator() As CodeValidator.Base
			Return New CodeValidator.SingleImporter(_settings.CaseInfo, _codeManager)
		End Function
		Protected Overrides Function GetArtifactReader() As Api.IArtifactReader
			Return New kCura.WinEDDS.LoadFileReader(_settings, False)
		End Function



	End Class

	Public Class WebServiceFieldInfoNameComparer
		Implements IComparer

		Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements System.Collections.IComparer.Compare
			Return String.Compare(DirectCast(x, kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo).DisplayName, DirectCast(y, kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo).DisplayName)
		End Function
	End Class

End Namespace