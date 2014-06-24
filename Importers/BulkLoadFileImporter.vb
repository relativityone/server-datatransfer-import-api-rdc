Imports System.IO
Imports kCura.OI.FileID
Imports kCura.EDDS.WebAPI.BulkImportManagerBase
Imports Relativity

Imports Relativity.MassImport
Imports Microsoft.VisualBasic
Imports System.Collections.Generic

Namespace kCura.WinEDDS
	Public Class BulkLoadFileImporter
		Inherits kCura.WinEDDS.LoadFileBase

#Region "Members"
		Private _overwrite As String
		Private WithEvents _uploader As kCura.WinEDDS.FileUploader
		Private WithEvents _bcpuploader As kCura.WinEDDS.FileUploader
		Private _parentFolderDTO As kCura.EDDS.WebAPI.FolderManagerBase.Folder
		Protected _auditManager As kCura.WinEDDS.Service.AuditManager

		Private _recordCount As Int64 = -1
		Private _allFields As kCura.EDDS.WebAPI.DocumentManagerBase.Field()
		Private _fieldsForCreate As kCura.EDDS.WebAPI.DocumentManagerBase.Field()
		Protected _continue As Boolean
		Protected _processedDocumentIdentifiers As Collections.Specialized.NameValueCollection
		Protected WithEvents _processController As kCura.Windows.Process.Controller
		Protected _offset As Int32 = 0
		Protected _firstTimeThrough As Boolean
		Private _importBatchSize As Int32?
		Private _importBatchVolume As Int32?
		Private _minimumBatchSize As Int32?
		Private _batchSizeHistoryList As System.Collections.Generic.List(Of Int32)
		Private _destinationFolderColumnIndex As Int32 = -1
		Private _folderCache As FolderCache
		Private _defaultDestinationFolderPath As String = String.Empty
		Private _copyFileToRepository As Boolean
		Private _oixFileLookup As System.Collections.Specialized.HybridDictionary
		Private _fieldArtifactIds As Int32()
		Private _outputNativeFileWriter As System.IO.StreamWriter
		Private _outputCodeFileWriter As System.IO.StreamWriter
		Private _outputObjectFileWriter As System.IO.StreamWriter
		Private _outputDataGridFileWriter As System.IO.StreamWriter
		Private _caseInfo As Relativity.CaseInfo
		Private _overlayArtifactID As Int32

		Private _runID As String = System.Guid.NewGuid.ToString.Replace("-", "_")

		Private _outputNativeFilePath As String = System.IO.Path.GetTempFileName
		Private _outputCodeFilePath As String = System.IO.Path.GetTempFileName
		Private _outputObjectFilePath As String = System.IO.Path.GetTempFileName
		Private _outputDataGridFilePath As String = System.IO.Path.GetTempFileName
		Private _filePath As String
		Private _batchCounter As Int32 = 0
		Private _errorMessageFileLocation As String = String.Empty
		Private _errorLinesFileLocation As String = String.Empty

		Public MaxNumberOfErrorsInGrid As Int32 = Config.DefaultMaximumErrorCount
		Private _errorCount As Int32 = 0
		Private _prePushErrorLineNumbersFileName As String = String.Empty
		Private _processID As Guid
		Private _parentArtifactTypeID As Int32?
		Private _statistics As New kCura.WinEDDS.Statistics
		Private _timekeeper As New kCura.Utility.Timekeeper
		Private _currentStatisticsSnapshot As IDictionary
		Private _statisticsLastUpdated As System.DateTime = System.DateTime.Now
		Private _unmappedRelationalFields As System.Collections.ArrayList

		Private _bulkLoadFileFieldDelimiter As String

		Private Const _COPY_TEXT_FILE_BUFFER_SIZE As Int32 = 40000
		Private Const _UNKNOWN_PARENT_FOLDER_ID As Int32 = -9
#End Region

#Region "Accessors"

		Public Property DisableNativeValidation As Boolean = Config.DisableNativeValidation
		Public Property DisableNativeLocationValidation As Boolean = Config.DisableNativeLocationValidation
		Public Property DisableUserSecurityCheck As Boolean
		Public Property AuditLevel As kCura.EDDS.WebAPI.BulkImportManagerBase.ImportAuditLevel = WinEDDS.Config.AuditLevel
		Public ReadOnly Property BatchSizeHistoryList As System.Collections.Generic.List(Of Int32)
			Get
				Return _batchSizeHistoryList
			End Get
		End Property
		
		Protected Overridable ReadOnly Property NumberOfRetries() As Int32
			Get
				Return kCura.Utility.Config.IOErrorNumberOfRetries
			End Get
		End Property

		Protected Overridable ReadOnly Property WaitTimeBetweenRetryAttempts() As Int32
			Get
				Return kCura.Utility.Config.IOErrorWaitTimeInSeconds
			End Get
		End Property

		Protected Overridable ReadOnly Property MaxDataGridRecordSize As Int64
			Get
				Return kCura.Utility.Config.MaximumDataGridRecordSizeInBytes
			End Get
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
						If System.Array.IndexOf(_fieldArtifactIds, field.ArtifactID) <> -1 Then
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
					If field.FieldCategoryID = Relativity.FieldCategory.FileInfo Then retVal = field
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
				Return Nothing
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
						If Not item.DocumentField Is Nothing AndAlso item.DocumentField.FieldCategory = Relativity.FieldCategory.Relational AndAlso item.DocumentField.ImportBehavior = kCura.EDDS.WebAPI.DocumentManagerBase.ImportBehaviorChoice.ReplaceBlankValuesWithIdentifier Then
							mappedRelationalFieldIds.Add(item.DocumentField.FieldID)
						End If
					Next
					_unmappedRelationalFields = New System.Collections.ArrayList
					For Each field As kCura.EDDS.WebAPI.DocumentManagerBase.Field In Me.AllFields(_artifactTypeID)
						If field.FieldCategory = EDDS.WebAPI.DocumentManagerBase.FieldCategory.Relational And Not mappedRelationalFieldIds.Contains(field.ArtifactID) AndAlso field.ImportBehavior = kCura.EDDS.WebAPI.DocumentManagerBase.ImportBehaviorChoice.ReplaceBlankValuesWithIdentifier Then
							_unmappedRelationalFields.Add(field)
						End If
					Next
				End If
				Return _unmappedRelationalFields
			End Get
		End Property

		Protected Overridable ReadOnly Property ParentArtifactTypeID As Int32
			Get
				If Not _parentArtifactTypeID.HasValue Then
					Dim parentQuery As New kCura.WinEDDS.Service.ObjectTypeManager(_settings.Credentials, _settings.CookieContainer)
					_parentArtifactTypeID = CType(parentQuery.RetrieveParentArtifactTypeID(_settings.CaseInfo.ArtifactID, _settings.ArtifactTypeID).Tables(0).Rows(0)("ParentArtifactTypeID"), Int32)
				End If
				Return _parentArtifactTypeID.Value
			End Get
		End Property

		Protected Overridable Property MinimumBatchSize As Int32
			Get
				If Not _minimumBatchSize.HasValue Then _minimumBatchSize = kCura.WinEDDS.Config.MinimumBatchSize
				Return _minimumBatchSize.Value
			End Get
			Set(ByVal value As Int32)
				_minimumBatchSize = value
			End Set
		End Property

		Protected Property ImportBatchSize As Int32
			Get
				If Not _importBatchSize.HasValue Then _importBatchSize = Config.ImportBatchSize
				Return _importBatchSize.Value
			End Get
			Set(ByVal value As Int32)
				_importBatchSize = If(value > MinimumBatchSize, value, MinimumBatchSize)
			End Set
		End Property

		Protected Property ImportBatchVolume As Int32
			Get
				If Not _importBatchVolume.HasValue Then _importBatchVolume = Config.ImportBatchMaxVolume
				Return _importBatchVolume.Value
			End Get
			Set(ByVal value As Int32)
				_importBatchVolume = value
			End Set
		End Property

		Protected Overridable ReadOnly Property BatchResizeEnabled As Boolean
			Get
				Return kCura.WinEDDS.Config.DynamicBatchResizingOn
			End Get
		End Property

#End Region

#Region "Constructors"

		''' <summary>
		''' Constructs a new importer that will prepare a bulk load file from a provided file.
		''' </summary>
		''' <param name="args">Information about the file being loaded</param>
		''' <param name="processController">The process that is running</param>
		''' <param name="timeZoneOffset">The running context's time zone offset from UTC</param>
		''' <param name="initializeUploaders">Sets whether or not the uploaders should be initialized
		''' for use</param>
		''' <param name="processID">The identifier of the process running</param>
		''' <param name="bulkLoadFileFieldDelimiter">Sets the field delimiter to use when writing
		''' out the bulk load file. Line delimiters will be this value plus a line feed.</param>
		''' <exception cref="ArgumentNullException">Thrown if <paramref name="bulkLoadFileFieldDelimiter"/>
		''' is <c>null</c> or <c>String.Empty</c>.</exception>
		Public Sub New(ByVal args As LoadFile, ByVal processController As kCura.Windows.Process.Controller, ByVal timeZoneOffset As Int32, ByVal initializeUploaders As Boolean, ByVal processID As Guid, ByVal doRetryLogic As Boolean, ByVal bulkLoadFileFieldDelimiter As String)
			Me.New(args, processController, timeZoneOffset, True, initializeUploaders, processID, doRetryLogic, bulkLoadFileFieldDelimiter, InitializeArtifactReader:=True)
		End Sub

		''' <summary>
		''' Constructs a new importer that will prepare a bulk load file from a provided file.
		''' </summary>
		''' <param name="args">Information about the file being loaded</param>
		''' <param name="processController">The process that is running</param>
		''' <param name="timeZoneOffset">The running context's time zone offset from UTC</param>
		''' <param name="initializeUploaders">Sets whether or not the uploaders should be initialized
		''' for use</param>
		''' <param name="processID">The identifier of the process running</param>
		''' <param name="bulkLoadFileFieldDelimiter">Sets the field delimiter to use when writing
		''' out the bulk load file. Line delimiters will be this value plus a line feed.</param>
		''' <exception cref="ArgumentNullException">Thrown if <paramref name="bulkLoadFileFieldDelimiter"/>
		''' is <c>null</c> or <c>String.Empty</c>.</exception>
		Public Sub New(ByVal args As LoadFile, ByVal processController As kCura.Windows.Process.Controller, ByVal timeZoneOffset As Int32, ByVal autoDetect As Boolean, ByVal initializeUploaders As Boolean, ByVal processID As Guid, ByVal doRetryLogic As Boolean, ByVal bulkLoadFileFieldDelimiter As String)
			Me.New(args, processController, timeZoneOffset, autoDetect, initializeUploaders, processID, doRetryLogic, bulkLoadFileFieldDelimiter, InitializeArtifactReader:=True)
		End Sub

		''' <summary>
		''' Constructs a new importer that will prepare a bulk load file from a provided file.
		''' </summary>
		''' <param name="args">Information about the file being loaded</param>
		''' <param name="processController">The process that is running</param>
		''' <param name="timeZoneOffset">The running context's time zone offset from UTC</param>
		''' <param name="initializeUploaders">Sets whether or not the uploaders should be initialized
		''' for use</param>
		''' <param name="processID">The identifier of the process running</param>
		''' <param name="bulkLoadFileFieldDelimiter">Sets the field delimiter to use when writing
		''' out the bulk load file. Line delimiters will be this value plus a line feed.</param>
		''' <exception cref="ArgumentNullException">Thrown if <paramref name="bulkLoadFileFieldDelimiter"/>
		''' is <c>null</c> or <c>String.Empty</c>.</exception>
		Public Sub New(args As LoadFile, processController As kCura.Windows.Process.Controller, timeZoneOffset As Int32, autoDetect As Boolean, initializeUploaders As Boolean, processID As Guid, doRetryLogic As Boolean, bulkLoadFileFieldDelimiter As String, initializeArtifactReader As Boolean)
			MyBase.New(args, timeZoneOffset, doRetryLogic, autoDetect, initializeArtifactReader)

			' get an instance of the specific type of artifact reader so we can get the fieldmapped event

			_overwrite = args.OverwriteDestination
			If args.CopyFilesToDocumentRepository Then
				'DEFECT: SF#226211, repositories without trailing \ caused import to fail. Changed to use Path.Combine. -tmh
				Dim lastHalfPath As String = "EDDS" & args.CaseInfo.ArtifactID & "\"
				_defaultDestinationFolderPath = Path.Combine(args.SelectedCasePath, lastHalfPath)
				If args.ArtifactTypeID <> Relativity.ArtifactType.Document Then
					For Each item As LoadFileFieldMap.LoadFileFieldMapItem In args.FieldMap
						If Not item.DocumentField Is Nothing AndAlso item.NativeFileColumnIndex > -1 AndAlso item.DocumentField.FieldTypeID = Relativity.FieldTypeHelper.FieldType.File Then
							_defaultDestinationFolderPath &= "File" & item.DocumentField.FieldID & "\"
						End If
					Next
				End If
			End If
			If initializeUploaders Then
				CreateUploaders(args)
			End If
			_copyFileToRepository = args.CopyFilesToDocumentRepository
			If Not kCura.WinEDDS.Config.CreateFoldersInWebAPI Then
				'Client side folder creation (added back for Dominus# 1127879)
				If autoDetect Then _parentFolderDTO = _folderManager.Read(args.CaseInfo.ArtifactID, args.CaseInfo.RootFolderID)
			End If
			_processController = processController
			_continue = True
			_firstTimeThrough = True
			_caseInfo = args.CaseInfo
			_settings = args
			_processID = processID
			_startLineNumber = args.StartLineNumber
			_overlayArtifactID = args.IdentityFieldId

			If String.IsNullOrEmpty(bulkLoadFileFieldDelimiter) Then
				Throw New ArgumentNullException("bulkLoadFileFieldDelimiter")
			End If

			_bulkLoadFileFieldDelimiter = bulkLoadFileFieldDelimiter

			_batchSizeHistoryList = New System.Collections.Generic.List(Of Int32)
			_DisableNativeLocationValidation = Config.DisableNativeLocationValidation
		End Sub

		Protected Overridable Sub CreateUploaders(ByVal args As LoadFile)
			_uploader = New kCura.WinEDDS.FileUploader(args.Credentials, args.CaseInfo.ArtifactID, _defaultDestinationFolderPath, args.CookieContainer)
			_bcpuploader = New kCura.WinEDDS.FileUploader(args.Credentials, args.CaseInfo.ArtifactID, _defaultDestinationFolderPath, args.CookieContainer, False)
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
			_outputCodeFileWriter.WriteLine(String.Format("{1}{0}{2}{0}{3}{0}", _bulkLoadFileFieldDelimiter, documentIdentifier, codeArtifactID, codeTypeID))
		End Sub

		Public Sub WriteObjectLineToTempFile(ByVal ownerIdentifier As String, ByVal objectName As String, ByVal artifactID As Int32, ByVal objectTypeArtifactID As Int32, ByVal fieldID As Int32)
			_outputObjectFileWriter.WriteLine(String.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}", _bulkLoadFileFieldDelimiter, ownerIdentifier, objectName, artifactID, objectTypeArtifactID, fieldID))
		End Sub

#End Region

#Region "Main"

		Public Function ReadFile(ByVal path As String) As Object
			Dim line As Api.ArtifactFieldCollection
			_filePath = path
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
				If Not InitializeMembers(path) Then
					Return False
				End If
				_processedDocumentIdentifiers = New Collections.Specialized.NameValueCollection
				_timekeeper.MarkEnd("ReadFile_InitializeMembers")
				_timekeeper.MarkStart("ReadFile_ProcessDocuments")
				_columnHeaders = _artifactReader.GetColumnNames(_settings)
				If _firstLineContainsColumnNames Then _offset = -1
				Dim isError As Boolean = False
				_statistics.BatchSize = Me.ImportBatchSize
				While _continue AndAlso _artifactReader.HasMoreRecords
					Try
						If Me.CurrentLineNumber < _startLineNumber Then
							Me.AdvanceLine()
						Else
							_timekeeper.MarkStart("ReadFile_GetLine")
							_statistics.DocCount += 1
							'The EventType.Count is used as an 'easy' way for the ImportAPI to eventually get a record count.
							' It could be done in DataReaderClient in other ways, but those ways turned out to be pretty messy.
							' -Phil S. 06/12/2012
							WriteStatusLine(Windows.Process.EventType.Count, String.Empty)
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
					Catch ex As kCura.Utility.ImporterExceptionBase
						WriteError(Me.CurrentLineNumber, ex.Message)
					Catch ex As System.IO.FileNotFoundException
						WriteError(Me.CurrentLineNumber, ex.Message)
					Catch ex As System.Exception
						WriteFatalError(Me.CurrentLineNumber, ex)
					End Try
				End While
				_timekeeper.MarkEnd("ReadFile_ProcessDocuments")
				_timekeeper.MarkStart("ReadFile_OtherFinalization")
				Me.TryPushNativeBatch(True)
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
			Return Nothing
		End Function

		Private Function InitializeMembers(ByVal path As String) As Boolean
			_recordCount = _artifactReader.CountRecords
			If _recordCount = -1 Then
				RaiseEvent StatusMessage(New kCura.Windows.Process.StatusEventArgs(Windows.Process.EventType.Progress, CurrentLineNumber, CurrentLineNumber, "cancel import", _currentStatisticsSnapshot))
				Return False
			End If

			Me.InitializeFolderManagement()
			Me.InitializeFieldIdList()
			DeleteFiles()
			OpenFileWriters()
			RaiseEvent StatusMessage(New kCura.Windows.Process.StatusEventArgs(Windows.Process.EventType.ResetStartTime, 0, _recordCount, "Reset time for import rolling average", Nothing))
			Return True
		End Function

		Protected Overrides Sub InitializeManagers(ByVal args As LoadFile)
			MyBase.InitializeManagers(args)
			_auditManager = New kCura.WinEDDS.Service.AuditManager(args.Credentials, args.CookieContainer)
		End Sub

		Private Sub DeleteFiles()
			kCura.Utility.File.Instance.Delete(_outputNativeFilePath)
			kCura.Utility.File.Instance.Delete(_outputCodeFilePath)
			kCura.Utility.File.Instance.Delete(_outputObjectFilePath)
			kCura.Utility.File.Instance.Delete(_outputDataGridFilePath)
		End Sub

		Private Sub InitializeFolderManagement()
			If _createFolderStructure Then
				If Not kCura.WinEDDS.Config.CreateFoldersInWebAPI Then
					'Client side folder creation (added back for Dominus# 1127879)
					If _artifactTypeID = 10 Then _folderCache = New FolderCache(_folderManager, _folderID, _caseArtifactID)
				End If
				Dim openParenIndex As Int32 = _destinationFolder.LastIndexOf("("c) + 1
				Dim closeParenIndex As Int32 = _destinationFolder.LastIndexOf(")"c)
				_destinationFolderColumnIndex = Int32.Parse(_destinationFolder.Substring(openParenIndex, closeParenIndex - openParenIndex)) - 1
			End If
		End Sub

		Private Sub InitializeFieldIdList()
			Dim fieldIdList As New System.Collections.ArrayList
			For Each item As LoadFileFieldMap.LoadFileFieldMapItem In _fieldMap
				If Not item.DocumentField Is Nothing AndAlso Not item.NativeFileColumnIndex = -1 Then
					'If item.DocumentField.FieldCategoryID <> Relativity.FieldCategory.FullText Then fieldIdList.Add(item.DocumentField.FieldID)
					fieldIdList.Add(item.DocumentField.FieldID)
				End If
			Next
			fieldIdList.Add(Me.FileInfoField(_artifactTypeID).ArtifactID)
			_fieldArtifactIds = DirectCast(fieldIdList.ToArray(GetType(Int32)), Int32())
		End Sub

		Private Function ManageDocument(ByVal record As Api.ArtifactFieldCollection, ByVal lineStatus As Int32) As String
			Dim filename As String = Nothing
			Dim fileGuid As String = String.Empty
			Dim uploadFile As Boolean = record.FieldList(Relativity.FieldTypeHelper.FieldType.File).Length > 0 AndAlso Not record.FieldList(Relativity.FieldTypeHelper.FieldType.File)(0).Value Is Nothing
			Dim fileExists As Boolean
			Dim identityValue As String = String.Empty
			Dim parentFolderID As Int32
			Dim fullFilePath As String = ""
			Dim oixFileIdData As OI.FileID.FileIDData = Nothing
			Dim destinationVolume As String = Nothing
			_timekeeper.MarkStart("ManageDocument_Filesystem")
			If uploadFile AndAlso _artifactTypeID = Relativity.ArtifactType.Document Then
				filename = record.FieldList(Relativity.FieldTypeHelper.FieldType.File)(0).Value.ToString
				If filename.Length > 1 AndAlso filename.Chars(0) = "\" AndAlso filename.Chars(1) <> "\" Then
					filename = "." & filename
				End If

				If Me.DisableNativeLocationValidation Then
					'assuming that file exists and not validating it
					fileExists = True
				Else
					fileExists = System.IO.File.Exists(filename)
				End If

				If filename.Trim.Equals(String.Empty) Then
					fileExists = False
				End If

				If filename <> String.Empty AndAlso Not fileExists Then lineStatus += Relativity.MassImport.ImportStatus.FileSpecifiedDne 'Throw New InvalidFilenameException(filename)
				If fileExists AndAlso Not Me.DisableNativeLocationValidation Then
					If Me.GetFileLength(filename) = 0 Then
						If Config.CreateErrorForEmptyNativeFile Then
							lineStatus += Relativity.MassImport.ImportStatus.EmptyFile 'Throw New EmptyNativeFileException(filename)
						Else
							WriteWarning("The file " & filename & " is empty; only metadata will be loaded for this record.")
							fileExists = False
							filename = String.Empty
						End If
					End If
				End If
				If fileExists Then
					Try
						Dim now As DateTime = DateTime.Now
						If Me.DisableNativeValidation Then
							oixFileIdData = Nothing
						Else
							Dim idDataExtractor As kCura.WinEDDS.Api.IHasOixFileType = TryCast(record, kCura.WinEDDS.Api.IHasOixFileType)
							If idDataExtractor Is Nothing Then
								oixFileIdData = kCura.OI.FileID.Manager.Instance.GetFileIDDataByFilePath(filename)
							Else
								oixFileIdData = idDataExtractor.GetFileIDData()
							End If
						End If

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
						fullFilePath = filename
						filename = filename.Substring(filename.LastIndexOf("\") + 1)
						WriteStatusLine(Windows.Process.EventType.Status, String.Format("End upload file. ({0}ms)", DateTime.op_Subtraction(DateTime.Now, now).Milliseconds))
					Catch ex As System.IO.FileNotFoundException
						If Me.DisableNativeLocationValidation Then
							'Don't do anything. This exception can only happen if DisableNativeLocationValidation is turned on
						Else
							Throw
						End If
					End Try
				End If
			End If
			_timekeeper.MarkEnd("ManageDocument_Filesystem")

			_timekeeper.MarkStart("ManageDocument_Folder")
			Dim folderPath As String = String.Empty
			If _createFolderStructure Then
				If _artifactTypeID = Relativity.ArtifactType.Document Then
					Dim value As String = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(kCura.Utility.NullableTypesHelper.DBNullString(record.FieldList(Relativity.FieldCategory.ParentArtifact)(0).Value))
					If kCura.WinEDDS.Config.CreateFoldersInWebAPI Then
						'Server side folder creation
						Dim cleanFolderPath As String = Me.CleanDestinationFolderPath(value)
						If (String.IsNullOrWhiteSpace(cleanFolderPath)) Then
							parentFolderID = _folderID
						Else
							folderPath = cleanFolderPath
							'We're creating the structure on the server side, so it'll get a number then
							parentFolderID = _UNKNOWN_PARENT_FOLDER_ID
						End If
					Else
						'Client side folder creation (added back for Dominus# 1127879)
						parentFolderID = _folderCache.FolderID(Me.CleanDestinationFolderPath(value))
					End If
				Else
					'TODO: If we are going to do this for more than documents, fix this as well...
					Dim textIdentifier As String = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(kCura.Utility.NullableTypesHelper.DBNullString(record.FieldList(Relativity.FieldCategory.ParentArtifact)(0).Value.ToString))
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
				'If we're not creating the structure, all documents go in the root folder (aka _folderId)
				If _artifactTypeID = Relativity.ArtifactType.Document OrElse Me.ParentArtifactTypeID = Relativity.ArtifactType.Case Then
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
			Dim doc As MetaDocument
			Dim fileSizeExtractor As kCura.WinEDDS.Api.IHasFileSize = TryCast(record, kCura.WinEDDS.Api.IHasFileSize)
			If fileSizeExtractor Is Nothing Then
				doc = New MetaDocument(fileGuid, identityValue, fileExists AndAlso uploadFile AndAlso (fileGuid <> String.Empty OrElse Not _copyFileToRepository), filename, fullFilePath, uploadFile, CurrentLineNumber, parentFolderID, record, oixFileIdData, lineStatus, destinationVolume, folderPath)
			Else
				doc = New SizedMetaDocument(fileGuid, identityValue, fileExists AndAlso uploadFile AndAlso (fileGuid <> String.Empty OrElse Not _copyFileToRepository), filename, fullFilePath, uploadFile, CurrentLineNumber, parentFolderID, record, oixFileIdData, lineStatus, destinationVolume, fileSizeExtractor.GetFileSize(), folderPath)
			End If
			_timekeeper.MarkStart("ManageDocument_ManageDocumentMetadata")
			ManageDocumentMetaData(doc)
			_timekeeper.MarkEnd("ManageDocument_ManageDocumentMetadata")
			Return identityValue
		End Function

		Protected Function CleanDestinationFolderPath(ByVal path As String) As String
			path = path.Trim()
			While path.Contains(".\")
				path = path.Replace(".\", "\")
			End While
			While path.Contains("\\")
				path = path.Replace("\\", "\")
			End While
			path = path.Replace(":", "_")
			If Not path.Length = 0 Then
				If path.Chars(0) <> "\"c Then
					path = "\" & path
				End If
			End If
			path = path.TrimEnd(New Char() {"\"c})
			If String.IsNullOrWhiteSpace(path) Then
				path = "\"
			End If
			Return path
		End Function

#End Region

#Region "WebService Calls"

		Public Overrides Function LookupArtifactIDForName(objectName As String, associatedObjectTypeID As Integer) As Integer
			Return If(associatedObjectTypeID = Relativity.ArtifactType.Document, MyBase.LookupArtifactIDForName(objectName, associatedObjectTypeID), -1)
		End Function

		Public Overrides Function LookupNameForArtifactID(objectArtifactID As Integer, associatedObjectTypeID As Integer) As String
			Return If(associatedObjectTypeID = Relativity.ArtifactType.Document, MyBase.LookupNameForArtifactID(objectArtifactID, associatedObjectTypeID), String.Empty)
		End Function

		Private Sub ManageDocumentMetaData(ByVal metaDoc As MetaDocument)
			Try
				_timekeeper.MarkStart("ManageDocumentMetadata_ManageDocumentLine")
				ManageDocumentLine(metaDoc)
				_timekeeper.MarkEnd("ManageDocumentMetadata_ManageDocumentLine")
				_batchCounter += 1
				_timekeeper.MarkStart("ManageDocumentMetadata_WserviceCall")
				If _outputNativeFileWriter.BaseStream.Length > ImportBatchVolume OrElse _batchCounter > ImportBatchSize - 1 Then
					Me.TryPushNativeBatch()
				End If
				_timekeeper.MarkEnd("ManageDocumentMetadata_WserviceCall")
			Catch ex As kCura.Utility.ImporterExceptionBase
				WriteError(metaDoc.LineNumber, ex.Message)
			Catch ex As System.Exception
				WriteFatalError(metaDoc.LineNumber, ex)
			End Try
			_timekeeper.MarkStart("ManageDocumentMetadata_ProgressEvent")
			WriteStatusLine(Windows.Process.EventType.Progress, String.Format("Item '{0}' processed.", metaDoc.IdentityValue), metaDoc.LineNumber)
			_timekeeper.MarkEnd("ManageDocumentMetadata_ProgressEvent")
		End Sub

		Protected Function BulkImport(ByVal settings As kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo, ByVal includeExtractedTextEncoding As Boolean) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			If BatchSizeHistoryList.Count = 0 Then BatchSizeHistoryList.Add(ImportBatchSize)
			Dim tries As Int32 = NumberOfRetries
			Dim retval As New kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			While tries > 0
				Try
					retval = BatchBulkImport(settings, includeExtractedTextEncoding)
					Exit While
				Catch ex As Exception
					tries -= 1
					If tries = 0 OrElse ExceptionIsTimeoutRelated(ex) OrElse _continue = False OrElse ex.GetType = GetType(Service.BulkImportManager.BulkImportSqlException) OrElse ex.GetType = GetType(Service.BulkImportManager.InsufficientPermissionsForImportException) Then
						Throw
					Else
						Me.RaiseWarningAndPause(ex, WaitTimeBetweenRetryAttempts)
					End If
				End Try
			End While
			Return retval
		End Function

		Private Function BatchBulkImport(ByVal settings As kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo, ByVal includeExtractedTextEncoding As Boolean) As MassImportResults
			Dim retval As MassImportResults
			If TypeOf settings Is kCura.EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo Then
				retval = Me.BulkImportManager.BulkImportObjects(_caseInfo.ArtifactID, DirectCast(settings, kCura.EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo), _copyFileToRepository)
			Else
				retval = Me.BulkImportManager.BulkImportNative(_caseInfo.ArtifactID, settings, _copyFileToRepository, includeExtractedTextEncoding)
			End If
			Return retval
		End Function

		Private Function ExceptionIsTimeoutRelated(ByVal ex As Exception) As Boolean
			If ex.GetType = GetType(Service.BulkImportManager.BulkImportSqlTimeoutException) Then
				Return True
			ElseIf TypeOf ex Is System.Net.WebException AndAlso ex.Message.ToString.Contains("timed out") Then
				Return True
			Else
				Return False
			End If
		End Function

		Protected Overridable Sub LowerBatchLimits()
			Me.ImportBatchSize -= 100
			Me.Statistics.BatchSize = Me.ImportBatchSize
			Me.BatchSizeHistoryList.Add(Me.ImportBatchSize)
		End Sub

		Protected Overridable Sub RaiseWarningAndPause(ByVal ex As Exception, ByVal timeoutSeconds As Int32)
			Me.RaiseIoWarning(New kCura.Utility.DelimitedFileImporter.IoWarningEventArgs(timeoutSeconds, ex, Me.CurrentLineNumber))
			System.Threading.Thread.CurrentThread.Join(1000 * timeoutSeconds)
		End Sub

		Private Function GetSettingsObject() As kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo
			Dim retval As kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo = Nothing
			If _artifactTypeID = Relativity.ArtifactType.Document Then
				retval = New kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo With {.DisableUserSecurityCheck = Me.DisableUserSecurityCheck, .AuditLevel = Me.AuditLevel, .OverlayArtifactID = _overlayArtifactID}
				If kCura.WinEDDS.Config.CreateFoldersInWebAPI Then
					'Server side folder creation
					retval.RootFolderID = _folderID
					If retval.RootFolderID = 0 Then
						' -1 signifies unset, sent by a client using server-side folder creation (in the WebAPI)
						retval.RootFolderID = -1
					End If
				Else
					'Client side folder creation (added back for Dominus# 1127879)
					' 0 is the default value on the settings object which signifies client-side folder creation
					retval.RootFolderID = 0
				End If
			Else
				Dim settings As New kCura.EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo With {.DisableUserSecurityCheck = Me.DisableUserSecurityCheck, .AuditLevel = Me.AuditLevel}
				settings.ArtifactTypeID = _artifactTypeID
				retval = settings
			End If
			OnSettingsObjectCreate(retval)
			Return retval
		End Function

		Public Overridable Sub OnSettingsObjectCreate(settings As kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo)
			'Do Nothing
		End Sub

		Private Sub TryPushNativeBatch(Optional ByVal lastRun As Boolean = False)
			CloseFileWriters()
			Dim outputNativePath As String = _outputNativeFilePath
			Try
				PushNativeBatch(outputNativePath)
			Catch ex As Exception
				If BatchResizeEnabled AndAlso ExceptionIsTimeoutRelated(ex) AndAlso _continue Then
					Dim originalBatchSize As Int32 = Me.ImportBatchSize
					LowerBatchLimits()
					Me.RaiseWarningAndPause(ex, WaitTimeBetweenRetryAttempts)
					If Not _continue Then Throw 'after the pause
					Me.LowerBatchSizeAndRetry(outputNativePath, originalBatchSize)
				Else
					Throw
				End If
			End Try
			DeleteFiles()
			If Not lastRun Then OpenFileWriters()
		End Sub

		Private Sub LowerBatchSizeAndRetry(ByVal oldNativeFilePath As String, ByVal totalRecords As Int32)
			'NOTE: we are not cutting a new/smaller data grid bulk file because it will be chunked as it is loaded into the data grid
			Dim newNativeFilePath As String = System.IO.Path.GetTempFileName
			Dim limit As String = _bulkLoadFileFieldDelimiter & vbCrLf
			Dim last As New System.Collections.Generic.Queue(Of Char)
			Dim recordsProcessed As Int32 = 0
			Dim charactersSuccessfullyProcessed As Int64 = 0
			Dim hasReachedEof As Boolean = False
			Dim tries As Int32 = 1 'already starts at 1 retry
			While totalRecords > recordsProcessed AndAlso Not hasReachedEof AndAlso _continue
				Dim i As Int32 = 0
				Dim charactersProcessed As Int64 = 0
				Using sr As New System.IO.StreamReader(oldNativeFilePath, System.Text.Encoding.Unicode), sw As New System.IO.StreamWriter(newNativeFilePath, False, System.Text.Encoding.Unicode)
					Me.AdvanceStream(sr, charactersSuccessfullyProcessed)
					While (i < Me.ImportBatchSize AndAlso Not hasReachedEof)
						Dim c As Char = ChrW(sr.Read)
						last.Enqueue(c)
						If last.Count > limit.Length Then last.Dequeue()
						If New String(last.ToArray) = limit Then
							sw.Flush()
							i += 1
						End If
						sw.Write(c)
						charactersProcessed += 1
						hasReachedEof = (sr.Peek = -1)
					End While
					sw.Flush()
				End Using
				Try
					_batchCounter = i
					Me.WriteWarning("Processing sub-batch of size " & Me.ImportBatchSize & ".  " & recordsProcessed & " of " & totalRecords & " in the original batch processed")
					Me.PushNativeBatch(newNativeFilePath)
					recordsProcessed += i
					charactersSuccessfullyProcessed += charactersProcessed
				Catch ex As Exception
					If tries < NumberOfRetries AndAlso BatchResizeEnabled AndAlso ExceptionIsTimeoutRelated(ex) AndAlso _continue Then
						LowerBatchLimits()
						Me.RaiseWarningAndPause(ex, WaitTimeBetweenRetryAttempts)
						If Not _continue Then Throw 'after the pause
						tries += 1
						hasReachedEof = False
					Else
						kCura.Utility.File.Instance.Delete(newNativeFilePath)
						Throw
					End If
				End Try
			End While
			kCura.Utility.File.Instance.Delete(newNativeFilePath)
		End Sub

		Private Sub AdvanceStream(ByVal sr As System.IO.StreamReader, ByVal count As Int64)
			Dim i As Int32
			If count > 0 Then
				For j As Int64 = 0 To count - 1
					i = sr.Read()
				Next
			End If
		End Sub

		Private Sub PushNativeBatch(ByVal outputNativePath As String)
			Dim start As Int64 = System.DateTime.Now.Ticks
			If _batchCounter = 0 Then Exit Sub
			_batchCounter = 0
			Dim settings As kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo = Me.GetSettingsObject
			settings.UseBulkDataImport = True
			_bcpuploader.DoRetry = True

			Dim uploadBcp As FileUploadReturnArgs = _bcpuploader.UploadBcpFile(_caseInfo.ArtifactID, outputNativePath)
			If uploadBcp Is Nothing Then Exit Sub

			Dim codebcp As FileUploadReturnArgs = _bcpuploader.UploadBcpFile(_caseInfo.ArtifactID, _outputCodeFilePath)
			If codebcp Is Nothing Then Exit Sub

			Dim objectbcp As FileUploadReturnArgs = _bcpuploader.UploadBcpFile(_caseInfo.ArtifactID, _outputObjectFilePath)
			If objectbcp Is Nothing Then Exit Sub

			Dim datagridbcp As FileUploadReturnArgs = _bcpuploader.UploadBcpFile(_caseInfo.ArtifactID, _outputDataGridFilePath)
			If datagridbcp Is Nothing Then Exit Sub

			Dim nativeFileUploadKey As String = uploadBcp.Value
			Dim codeFileUploadKey As String = codebcp.Value
			Dim objectFileUploadKey As String = objectbcp.Value
			Dim dataGridFileUploadKey As String = datagridbcp.Value

			If _artifactTypeID = Relativity.ArtifactType.Document Then
				settings.Repository = _defaultDestinationFolderPath
				If settings.Repository = String.Empty Then settings.Repository = _caseInfo.DocumentPath
			Else
				settings.Repository = _caseInfo.DocumentPath
			End If
			If uploadBcp.Type = FileUploadReturnArgs.FileUploadReturnType.UploadError Then
				If Config.EnableSingleModeImport Then
					RaiseEvent UploadModeChangeEvent(_uploader.UploaderType.ToString, _bcpuploader.IsBulkEnabled)
					_uploader.DestinationFolderPath = settings.Repository
					_bcpuploader.DestinationFolderPath = settings.Repository
					nativeFileUploadKey = _bcpuploader.UploadFile(outputNativePath, _caseInfo.ArtifactID)
					codeFileUploadKey = _bcpuploader.UploadFile(_outputCodeFilePath, _caseInfo.ArtifactID)
					objectFileUploadKey = _bcpuploader.UploadFile(_outputObjectFilePath, _caseInfo.ArtifactID)
					dataGridFileUploadKey = _bcpuploader.UploadFile(_outputDataGridFilePath, _caseInfo.ArtifactID)
					settings.UseBulkDataImport = False
				Else
					Throw New BcpPathAccessException(uploadBcp.Value)
				End If
			End If

			settings.RunID = _runID
			settings.CodeFileName = codeFileUploadKey
			settings.DataFileName = nativeFileUploadKey
			settings.ObjectFileName = objectFileUploadKey
			settings.DataGridFileName = dataGridFileUploadKey
			settings.MappedFields = Me.GetMappedFields(_artifactTypeID, _settings.ObjectFieldIdListContainsArtifactId)
			settings.KeyFieldArtifactID = _keyFieldID
			settings.BulkLoadFileFieldDelimiter = _bulkLoadFileFieldDelimiter
			settings.OverlayBehavior = Me.GetMassImportOverlayBehavior(_settings.OverlayBehavior)
			Select Case _overwrite.ToLower
				Case "strict"
					settings.Overlay = EDDS.WebAPI.BulkImportManagerBase.OverwriteType.Overlay
				Case "none"
					settings.Overlay = EDDS.WebAPI.BulkImportManagerBase.OverwriteType.Append
				Case Else
					settings.Overlay = EDDS.WebAPI.BulkImportManagerBase.OverwriteType.Both
			End Select
			settings.UploadFiles = _filePathColumnIndex <> -1 AndAlso _settings.LoadNativeFiles

			_statistics.MetadataTime += System.Math.Max((System.DateTime.Now.Ticks - start), 1)
			_statistics.MetadataBytes += (Me.GetFileLength(_outputCodeFilePath) + Me.GetFileLength(outputNativePath) + Me.GetFileLength(_outputObjectFilePath) + Me.GetFileLength(_outputDataGridFilePath))
			start = System.DateTime.Now.Ticks

			Dim runResults As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults = Me.BulkImport(settings, _fullTextColumnMapsToFileLocation)

			_statistics.ProcessRunResults(runResults)
			_statistics.SqlTime += (System.DateTime.Now.Ticks - start)

			_currentStatisticsSnapshot = _statistics.ToDictionary
			_statisticsLastUpdated = System.DateTime.Now
			Me.ManageErrors(_artifactTypeID)
		End Sub

		Protected Function GetMassImportOverlayBehavior(ByVal inputOverlayType As LoadFile.FieldOverlayBehavior?) As kCura.EDDS.WebAPI.BulkImportManagerBase.OverlayBehavior
			Select Case inputOverlayType
				Case LoadFile.FieldOverlayBehavior.MergeAll
					Return EDDS.WebAPI.BulkImportManagerBase.OverlayBehavior.MergeAll

				Case LoadFile.FieldOverlayBehavior.ReplaceAll
					Return EDDS.WebAPI.BulkImportManagerBase.OverlayBehavior.ReplaceAll

				Case Else
					Return EDDS.WebAPI.BulkImportManagerBase.OverlayBehavior.UseRelativityDefaults
			End Select
		End Function

		Private Sub OpenFileWriters()
			_outputNativeFileWriter = New System.IO.StreamWriter(_outputNativeFilePath, False, System.Text.Encoding.Unicode)
			_outputCodeFileWriter = New System.IO.StreamWriter(_outputCodeFilePath, False, System.Text.Encoding.Unicode)
			_outputObjectFileWriter = New System.IO.StreamWriter(_outputObjectFilePath, False, System.Text.Encoding.Unicode)
			_outputDataGridFileWriter = New System.IO.StreamWriter(_outputDataGridFilePath, False, System.Text.Encoding.Unicode)
		End Sub

		Private Sub CloseFileWriters()
			_outputNativeFileWriter.Close()
			_outputCodeFileWriter.Close()
			_outputObjectFileWriter.Close()
			_outputDataGridFileWriter.Close()
		End Sub

		Public Function GetMappedFields(ByVal artifactTypeID As Int32, ByVal ObjectFieldIdListContainsArtifactId As IList(Of Int32)) As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo()
			Dim retval As New System.Collections.ArrayList
			For Each item As WinEDDS.LoadFileFieldMap.LoadFileFieldMapItem In _fieldMap
				If Not item.DocumentField Is Nothing Then
					Dim i As Integer = retval.Add(item.DocumentField.ToFieldInfo)
					If Not ObjectFieldIdListContainsArtifactId Is Nothing Then
						If (CType(retval(i), kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo).Type = FieldType.Object _
						 Or CType(retval(i), kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo).Type = FieldType.Objects) _
						AndAlso ObjectFieldIdListContainsArtifactId.Contains(CType(retval(i), kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo).ArtifactID) Then
							CType(retval(i), kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo).ImportBehavior = ImportBehaviorChoice.ObjectFieldContainsArtifactId
						End If
					End If
				End If
			Next
			retval.Sort(New WebServiceFieldInfoNameComparer)
			If artifactTypeID = Relativity.ArtifactType.Document Then
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

		Private Sub ManageDocumentLine(ByVal mdoc As MetaDocument)
			Dim chosenEncoding As System.Text.Encoding = Nothing

			_outputNativeFileWriter.Write("0" & _bulkLoadFileFieldDelimiter) 'kCura_Import_ID
			_outputNativeFileWriter.Write(mdoc.LineStatus.ToString & _bulkLoadFileFieldDelimiter)	'kCura_Import_Status
			_outputNativeFileWriter.Write("0" & _bulkLoadFileFieldDelimiter) 'kCura_Import_IsNew
			_outputNativeFileWriter.Write("0" & _bulkLoadFileFieldDelimiter) 'ArtifactID
			_outputNativeFileWriter.Write(mdoc.LineNumber & _bulkLoadFileFieldDelimiter) 'kCura_Import_OriginalLineNumber

			If mdoc.UploadFile And mdoc.IndexFileInDB Then
				_outputNativeFileWriter.Write(mdoc.FileGuid & _bulkLoadFileFieldDelimiter)	'kCura_Import_FileGuid
				_outputNativeFileWriter.Write(mdoc.Filename & _bulkLoadFileFieldDelimiter)	'kCura_Import_FileName
				If _settings.CopyFilesToDocumentRepository Then
					_outputNativeFileWriter.Write(_defaultDestinationFolderPath & mdoc.DestinationVolume & "\" & mdoc.FileGuid & _bulkLoadFileFieldDelimiter)	'kCura_Import_Location
					_outputNativeFileWriter.Write(mdoc.FullFilePath & _bulkLoadFileFieldDelimiter) 'kCura_Import_OriginalFileLocation
				Else
					_outputNativeFileWriter.Write(mdoc.FullFilePath & _bulkLoadFileFieldDelimiter) 'kCura_Import_Location
					_outputNativeFileWriter.Write(mdoc.FullFilePath & _bulkLoadFileFieldDelimiter) 'kCura_Import_OriginalFileLocation
				End If
				Dim fileSizeExtractor As kCura.WinEDDS.Api.IHasFileSize = TryCast(mdoc, kCura.WinEDDS.Api.IHasFileSize)
				If (fileSizeExtractor Is Nothing) Then
					_outputNativeFileWriter.Write(Me.GetFileLength(mdoc.FullFilePath) & _bulkLoadFileFieldDelimiter) 'kCura_Import_FileSize
				Else
					_outputNativeFileWriter.Write(fileSizeExtractor.GetFileSize() & _bulkLoadFileFieldDelimiter) 'kCura_Import_FileSize
				End If

			Else
				_outputNativeFileWriter.Write(_bulkLoadFileFieldDelimiter) 'kCura_Import_FileGuid
				_outputNativeFileWriter.Write(_bulkLoadFileFieldDelimiter) 'kCura_Import_FileName
				_outputNativeFileWriter.Write(_bulkLoadFileFieldDelimiter) 'kCura_Import_Location
				_outputNativeFileWriter.Write(_bulkLoadFileFieldDelimiter) 'kCura_Import_OriginalFileLocation
				_outputNativeFileWriter.Write("0" & _bulkLoadFileFieldDelimiter) 'kCura_Import_FileSize
			End If
			_outputNativeFileWriter.Write(mdoc.ParentFolderID & _bulkLoadFileFieldDelimiter) 'kCura_Import_ParentFolderID

			Dim foundDataGridField As Boolean = False

			For Each field As Api.ArtifactField In mdoc.Record
				Select Case field.StorageLocation

					Case Relativity.FieldInfo.StorageLocationChoice.SQL
						WriteDocumentField(chosenEncoding, field, _outputNativeFileWriter, _fullTextColumnMapsToFileLocation, _bulkLoadFileFieldDelimiter, _artifactTypeID, _extractedTextFileEncoding)

					Case Relativity.FieldInfo.StorageLocationChoice.DataGrid
						If Not foundDataGridField Then
							'write the data grid identity field as the first column, but only when we have data grid fields
							_outputDataGridFileWriter.Write(mdoc.IdentityValue & _bulkLoadFileFieldDelimiter)
							foundDataGridField = True
						End If

						'TODO: do we want to set/update the "chosenEncoding" property if the extracted text is going to the data grid?
						WriteDocumentField(chosenEncoding, field, _outputDataGridFileWriter, _fullTextColumnMapsToFileLocation, _bulkLoadFileFieldDelimiter, _artifactTypeID, _extractedTextFileEncoding)

				End Select
			Next

			If _artifactTypeID = Relativity.ArtifactType.Document Then
				If _filePathColumnIndex <> -1 AndAlso mdoc.UploadFile AndAlso mdoc.IndexFileInDB Then
					Dim boolString As String = "0"
					If Me.IsSupportedRelativityFileType(mdoc.FileIdData) Then boolString = "1"
					_outputNativeFileWriter.Write(boolString & _bulkLoadFileFieldDelimiter)
					Dim fieldType As String = mdoc.GetFileType()

					_outputNativeFileWriter.Write(fieldType & _bulkLoadFileFieldDelimiter)
					_outputNativeFileWriter.Write("1" & _bulkLoadFileFieldDelimiter)
				Else
					_outputNativeFileWriter.Write("0" & _bulkLoadFileFieldDelimiter)
					_outputNativeFileWriter.Write(_bulkLoadFileFieldDelimiter)
					_outputNativeFileWriter.Write("0" & _bulkLoadFileFieldDelimiter)
				End If
			End If

			'_fullTextColumnMapsToFileLocation and chosenEncoding indicate that extracted text is being mapped
			'In that case, we need a field (it can be empty but it needs the field delimiter) for the extracted text encoding
			If chosenEncoding IsNot Nothing Then
				_outputNativeFileWriter.Write(chosenEncoding.CodePage.ToString() & _bulkLoadFileFieldDelimiter)
			ElseIf _fullTextColumnMapsToFileLocation Then
				_outputNativeFileWriter.Write(_bulkLoadFileFieldDelimiter)
			End If

			If kCura.WinEDDS.Config.CreateFoldersInWebAPI Then
				'Server side folder creation

				'For documents only, we need a field to contain the folder path, so that WebAPI can create the folder (if needed)
				'If we are using client-side folder creation, the folder path will be an empty string, but we still need to 
				'add it so that the number of fields set by the client equals the number of fields that the server expects to find.
				'If we are using client-side folder creation, this folder path field will not be used by the server because of the 
				'settings object -- NativeFileInfo.RootFolderID will be 0.
				If _artifactTypeID = Relativity.ArtifactType.Document Then
					'Last column is the folder path (ONLY IF THIS IS A DOCUMENT LOAD... adding this to object imports will break them)
					_outputNativeFileWriter.Write(mdoc.FolderPath & _bulkLoadFileFieldDelimiter)
				End If
			End If
			_outputNativeFileWriter.Write(vbCrLf)
			If foundDataGridField Then
				_outputDataGridFileWriter.Write(vbCrLf)
			End If
		End Sub

		Private Sub WriteDocumentField(ByRef chosenEncoding As System.Text.Encoding, field As Api.ArtifactField, ByVal outputWriter As System.IO.StreamWriter, ByVal fileBasedfullTextColumn As Boolean, ByVal delimiter As String, ByVal artifactTypeID As Int32, ByVal extractedTextEncoding As System.Text.Encoding)
			If field.Type = Relativity.FieldTypeHelper.FieldType.MultiCode OrElse field.Type = Relativity.FieldTypeHelper.FieldType.Code Then
				outputWriter.Write(field.Value)
				outputWriter.Write(delimiter)
			ElseIf field.Type = Relativity.FieldTypeHelper.FieldType.File AndAlso artifactTypeID <> Relativity.ArtifactType.Document Then
				Dim fileFieldValues() As String = System.Web.HttpUtility.UrlDecode(field.ValueAsString).Split(Chr(11))
				If fileFieldValues.Length > 1 Then
					outputWriter.Write(fileFieldValues(0))
					outputWriter.Write(delimiter)
					outputWriter.Write(fileFieldValues(1))
					outputWriter.Write(delimiter)
					outputWriter.Write(fileFieldValues(2))
					outputWriter.Write(delimiter)
				Else
					outputWriter.Write("")
					outputWriter.Write(delimiter)
					outputWriter.Write("")
					outputWriter.Write(delimiter)
					outputWriter.Write("")
					outputWriter.Write(delimiter)
				End If
			ElseIf field.Type = Relativity.FieldTypeHelper.FieldType.File AndAlso artifactTypeID = Relativity.ArtifactType.Document Then
				'do nothing
			ElseIf field.Category = Relativity.FieldCategory.ParentArtifact Then
				'do nothing
			Else
				If field.Category = Relativity.FieldCategory.FullText AndAlso fileBasedfullTextColumn Then
					If Not field.ValueAsString = String.Empty Then
						chosenEncoding = extractedTextEncoding
						Dim fileStream As Stream

						'This logic exists as an attempt to improve import speeds.  The DetectEncoding call first checks if the file
						' exists, followed by a read of the first few bytes. The File.Exists check can be very expensive when going
						' across the network for the file, so this override allows that check to be skipped.
						' -Phil S. 07/27/2012
						If Not SkipExtractedTextEncodingCheck Then
							Dim determinedEncodingStream As DeterminedEncodingStream = kCura.WinEDDS.Utility.DetectEncoding(field.ValueAsString, False)
							fileStream = determinedEncodingStream.UnderlyingStream

							Dim detectedEncoding As System.Text.Encoding = determinedEncodingStream.DeterminedEncoding
							If detectedEncoding IsNot Nothing Then
								chosenEncoding = detectedEncoding
							End If
						Else
							fileStream = New FileStream(field.ValueAsString, FileMode.Open, FileAccess.Read)
						End If

						Dim sr As New System.IO.StreamReader(fileStream, chosenEncoding)
						Dim count As Int32 = 1
						Dim buff(_COPY_TEXT_FILE_BUFFER_SIZE) As Char
						Do
							count = sr.ReadBlock(buff, 0, _COPY_TEXT_FILE_BUFFER_SIZE)
							If count > 0 Then
								outputWriter.Write(buff, 0, count)
								outputWriter.Flush()
							End If
						Loop Until count = 0

						sr.Close()
						Try
							fileStream.Close()
						Catch
						End Try
					End If
				ElseIf field.Type = Relativity.FieldTypeHelper.FieldType.Boolean Then
					If field.ValueAsString <> String.Empty Then
						If Boolean.Parse(field.ValueAsString) Then
							outputWriter.Write("1")
						Else
							outputWriter.Write("0")
						End If
					End If
				ElseIf field.Type = Relativity.FieldTypeHelper.FieldType.Decimal OrElse
				 field.Type = Relativity.FieldTypeHelper.FieldType.Currency Then
					If field.ValueAsString <> String.Empty Then
						Dim d As String = CDec(field.Value).ToString(System.Globalization.CultureInfo.InvariantCulture)
						outputWriter.Write(d)
					End If
				Else
					outputWriter.Write(field.Value)
				End If
				outputWriter.Write(delimiter)
			End If
		End Sub

		Private Function GetIsSupportedRelativityFileTypeField() As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo
			For Each field As kCura.EDDS.WebAPI.DocumentManagerBase.Field In _allFields
				If field.DisplayName.ToLower = "supported by viewer" Then
					Return Me.FieldDtoToFieldInfo(field)
				End If
			Next
			Return Nothing
		End Function

		Private Function GetRelativityFileTypeField() As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo
			For Each field As kCura.EDDS.WebAPI.DocumentManagerBase.Field In _allFields
				If field.DisplayName.ToLower = "relativity native type" Then
					Return Me.FieldDtoToFieldInfo(field)
				End If
			Next
			Return Nothing
		End Function

		Private Function GetHasNativesField() As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo
			For Each field As kCura.EDDS.WebAPI.DocumentManagerBase.Field In _allFields
				If field.DisplayName.ToLower = "has native" Then
					Return Me.FieldDtoToFieldInfo(field)
				End If
			Next
			Return Nothing
		End Function

		Private Function GetObjectFileField() As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo
			For Each field As kCura.EDDS.WebAPI.DocumentManagerBase.Field In _allFields
				If field.FieldTypeID = Relativity.FieldTypeHelper.FieldType.File Then
					Return Me.FieldDtoToFieldInfo(field)
				End If
			Next
			Return Nothing
		End Function

		Private Function FieldDtoToFieldInfo(ByVal input As kCura.EDDS.WebAPI.DocumentManagerBase.Field) As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo
			Dim retval As New kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo
			retval.ArtifactID = input.ArtifactID
			retval.Category = CType(input.FieldCategoryID, kCura.EDDS.WebAPI.BulkImportManagerBase.FieldCategory)
			If Not input.CodeTypeID Is Nothing Then retval.CodeTypeID = input.CodeTypeID.Value
			retval.DisplayName = input.DisplayName
			If Not input.MaxLength Is Nothing Then retval.TextLength = input.MaxLength.Value
			retval.IsUnicodeEnabled = input.UseUnicodeEncoding
			retval.Type = Me.ConvertFieldTypeEnum(input.FieldTypeID)
			retval.StorageLocation = ConvertStorageLocationEnum(input.StorageLocation)
			Return retval
		End Function

		Private Function ConvertFieldTypeEnum(ByVal fieldtypeID As Int32) As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldType
			Dim ft As Relativity.FieldTypeHelper.FieldType = CType(fieldtypeID, Relativity.FieldTypeHelper.FieldType)
			Return CType(System.Enum.Parse(GetType(kCura.EDDS.WebAPI.BulkImportManagerBase.FieldType), ft.ToString), kCura.EDDS.WebAPI.BulkImportManagerBase.FieldType)
		End Function

		Private Function ConvertStorageLocationEnum(ByVal storageLocation As kCura.EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice) As kCura.EDDS.WebAPI.BulkImportManagerBase.StorageLocationChoice
			Return CType(System.Enum.Parse(GetType(kCura.EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice), storageLocation.ToString), kCura.EDDS.WebAPI.BulkImportManagerBase.StorageLocationChoice)
		End Function

		Private Function IsSupportedRelativityFileType(ByVal fileData As OI.FileID.FileIDData) As Boolean
			If fileData Is Nothing Then
				If Me.DisableNativeValidation Then
					Return True
				Else
					Return False
				End If
			End If
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
			System.Threading.Monitor.Enter(_outputDataGridFileWriter)
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
					If item.DocumentField.FieldTypeID = Relativity.FieldTypeHelper.FieldType.File Then
						Me.ManageFileField(record(item.DocumentField.FieldID))
					Else

						MyBase.SetFieldValue(record(item.DocumentField.FieldID), item.NativeFileColumnIndex, False, identityValue, 0, item.DocumentField.ImportBehavior)
					End If
				End If
			Next
			For Each fieldDTO As kCura.EDDS.WebAPI.DocumentManagerBase.Field In Me.UnmappedRelationalFields
				If fieldDTO.ImportBehavior = EDDS.WebAPI.DocumentManagerBase.ImportBehaviorChoice.ReplaceBlankValuesWithIdentifier Then
					Dim field As New Api.ArtifactField(fieldDTO)
					field.Value = identityValue
					Me.SetFieldValue(field, -1, False, identityValue, 0, fieldDTO.ImportBehavior)
				End If
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
			ht.Add("Identifier", _artifactReader.SourceIdentifierValue)
			RaiseReportError(ht, currentLineNumber, _artifactReader.SourceIdentifierValue, "client")
			WriteStatusLine(kCura.Windows.Process.EventType.Error, line)
		End Sub

		Private Sub RaiseReportError(ByVal row As System.Collections.Hashtable, ByVal lineNumber As Int32, ByVal identifier As String, ByVal type As String)
			_errorCount += 1
			If String.IsNullOrEmpty(_errorMessageFileLocation) Then
				_errorMessageFileLocation = System.IO.Path.GetTempFileName
			End If

			Dim errorMessageFileWriter As New System.IO.StreamWriter(_errorMessageFileLocation, True, System.Text.Encoding.Default)
			If _errorCount < MaxNumberOfErrorsInGrid Then
				RaiseEvent ReportErrorEvent(row)
			ElseIf _errorCount = MaxNumberOfErrorsInGrid Then
				Dim moretobefoundMessage As New System.Collections.Hashtable
				moretobefoundMessage.Add("Message", "Maximum number of errors for display reached.  Export errors to view full list.")
				RaiseEvent ReportErrorEvent(moretobefoundMessage)
			End If
			errorMessageFileWriter.WriteLine(String.Format("{0},{1},{2},{3}", CSVFormat(row("Line Number").ToString), CSVFormat(row("Message").ToString), CSVFormat(identifier), CSVFormat(type)))
			errorMessageFileWriter.Close()
		End Sub

		''' <summary>
		''' CSVFormat will take in a string, replace a double quote characters with a pair of double quote characters, then surround the string with double quote characters
		''' This preps it for being written as a field in a CSV file
		''' </summary>
		''' <param name="fieldValue">The string to convert to CSV format</param>
		''' <returns>
		''' the converted data
		''' </returns>
		Private Function CSVFormat(ByVal fieldValue As String) As String
			Return ControlChars.Quote + fieldValue.Replace(ControlChars.Quote, ControlChars.Quote + ControlChars.Quote) + ControlChars.Quote
		End Function

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
		Public Event FieldMapped(ByVal sourceField As String, ByVal workspaceField As String)

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
			If String.IsNullOrEmpty(_errorMessageFileLocation) Then
				' write out a blank file if there is no error message file
				Dim fileWriter As StreamWriter = System.IO.File.CreateText(exportLocation)
				fileWriter.Close()

				Exit Sub
			End If

			' not sure why the following is done twice.  It seems silly.
			Try
				System.IO.File.Copy(_errorMessageFileLocation, exportLocation, True)
			Catch ex As Exception
				System.IO.File.Copy(_errorMessageFileLocation, exportLocation, True)
			End Try
		End Sub

		Private Sub _processController_ExportErrorFileEvent(ByVal exportLocation As String) Handles _processController.ExportErrorFileEvent
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
			Inherits kCura.Utility.ImporterExceptionBase
			Public Sub New()
				MyBase.New("Identity value not set")
			End Sub
		End Class

		Public Class IdentityValueNotFoundException
			Inherits kCura.Utility.ImporterExceptionBase
			Public Sub New(ByVal value As String)
				MyBase.New(String.Format("Identity value '{0}' not found", value))
			End Sub
		End Class

		Public Class DocumentDomainException
			Inherits kCura.Utility.ImporterExceptionBase
			Public Sub New(ByVal ex As System.Exception)
				MyBase.New("Error accessing document information in domain layer: " & ex.Message.Replace("System.Web.Services.Protocols.SoapException: Server was unable to process request. ---> ", ""), ex)
			End Sub
		End Class

		Public Class DocumentReadException
			Inherits kCura.Utility.ImporterExceptionBase
			Public Sub New(ByVal parentException As System.Exception)
				MyBase.New("Error retrieving document information from EDDS: [" & parentException.Message & "]", parentException)
			End Sub
		End Class

		Public Class DocumentOverwriteException
			Inherits kCura.Utility.ImporterExceptionBase
			Public Sub New()
				MyBase.New("Identifier points to document that would be overwritten")
			End Sub
		End Class

		Public Class InvalidFilenameException
			Inherits kCura.Utility.ImporterExceptionBase
			Public Sub New(ByVal filename As String)
				MyBase.New(String.Format("File '{0}' not found.", filename))
			End Sub
		End Class

		Public Class EmptyNativeFileException
			Inherits kCura.Utility.ImporterExceptionBase
			Public Sub New(ByVal filename As String)
				MyBase.New(String.Format("File '{0}' contains 0 bytes.", filename))
			End Sub
		End Class

		Public Class FileUploadFailedException
			Inherits kCura.Utility.ImporterExceptionBase
			Public Sub New()
				MyBase.New(String.Format("File upload failed.  Either the access to the path is denied or there is no disk space available."))
			End Sub
		End Class

#End Region

		Private Sub _artifactReader_DataSourcePrep(ByVal e As Api.DataSourcePrepEventArgs) Handles _artifactReader.DataSourcePrep
			RaiseEvent DataSourcePrepEvent(e)
		End Sub

		Private Sub _artifactReader_StatusMessage(ByVal message As String) Handles _artifactReader.StatusMessage
			RaiseEvent StatusMessage(New kCura.Windows.Process.StatusEventArgs(Windows.Process.EventType.Status, _artifactReader.CurrentLineNumber, _recordCount, message, False, _currentStatisticsSnapshot))
		End Sub

		Private Sub _artifactReader_FieldMapped(ByVal sourceField As String, ByVal workspaceField As String) Handles _artifactReader.FieldMapped
			RaiseEvent FieldMapped(sourceField, workspaceField)
		End Sub



		Private Sub IoWarningHandler(ByVal e As IoWarningEventArgs)
			MyBase.RaiseIoWarning(e)
		End Sub

		Private Sub ManageErrors(ByVal artifactTypeID As Int32)
			If Not Me.BulkImportManager.NativeRunHasErrors(_caseInfo.ArtifactID, _runID) Then Exit Sub
			Dim sr As kCura.Utility.GenericCsvReader = Nothing
			Dim downloader As FileDownloader = Nothing
			Try
				With Me.BulkImportManager.GenerateNonImageErrorFiles(_caseInfo.ArtifactID, _runID, artifactTypeID, True, _keyFieldID)
					Me.WriteStatusLine(Windows.Process.EventType.Status, "Retrieving errors from server")
					downloader = New FileDownloader(DirectCast(Me.BulkImportManager.Credentials, System.Net.NetworkCredential), _caseInfo.DocumentPath, _caseInfo.DownloadHandlerURL, Me.BulkImportManager.CookieContainer, kCura.WinEDDS.Service.Settings.AuthenticationToken)
					AddHandler downloader.UploadStatusEvent, AddressOf _uploader_UploadStatusEvent
					Dim errorsLocation As String = System.IO.Path.GetTempFileName
					sr = AttemptErrorFileDownload(downloader, errorsLocation, .LogKey, _caseInfo)

					If sr Is Nothing Then
						'If we're here and still have an empty response, we can at least notify
						'the user that there was an error retrieving all errors.
						' -Phil S. 08/13/2012
						Const message As String = "There was an error while attempting to retrieve the errors from the server."

						RaiseEvent FatalErrorEvent(message, New Exception(message), _runID)
					Else
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
					End If

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

		Private Function AttemptErrorFileDownload(ByVal downloader As FileDownloader, ByVal errorFileOutputPath As String, ByVal logKey As String, ByVal caseInfo As CaseInfo) As kCura.Utility.GenericCsvReader
			Dim triesLeft As Integer = 3
			Dim sr As kCura.Utility.GenericCsvReader = Nothing

			While triesLeft > 0
				downloader.MoveTempFileToLocal(errorFileOutputPath, logKey, caseInfo, False)
				sr = New kCura.Utility.GenericCsvReader(errorFileOutputPath, True)
				Dim firstChar As Int32 = sr.Peek()

				If firstChar = -1 Then
					'Try again--assuming an empty error file is invalid, try the download one more time. The motivation
					' behind the retry is a rare SQL error that caused the DownloadHandler (used by the supplied instance
					' of FileDownloader) to return an empty response.
					' -Phil S. 08/13/2012
					triesLeft -= 1
					sr.Close()

					sr = Nothing
				Else
					Exit While
				End If
			End While

			downloader.RemoveRemoteTempFile(logKey, caseInfo)
			Return sr
		End Function

		Private Sub _processController_ParentFormClosingEvent(ByVal processID As Guid) Handles _processController.ParentFormClosingEvent
			If processID.ToString = _processID.ToString Then CleanupTempTables()
		End Sub

		Private Sub CleanupTempTables()
			If Not _runID Is Nothing AndAlso _runID <> "" Then
				Try
					Me.BulkImportManager.DisposeTempTables(_caseInfo.ArtifactID, _runID)
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