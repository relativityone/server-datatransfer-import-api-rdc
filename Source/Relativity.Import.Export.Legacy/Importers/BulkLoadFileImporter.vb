Imports System.Collections.Generic
Imports System.Threading
Imports System.Threading.Tasks
Imports kCura.WinEDDS.Api
Imports Relativity.Import.Export
Imports Relativity.Import.Export.Data
Imports Relativity.Import.Export.Io
Imports Relativity.Import.Export.Process
Imports Relativity.Import.Export.Transfer
Imports Relativity.Import.Export.Service

Namespace kCura.WinEDDS
	Public Class BulkLoadFileImporter
		Inherits LoadFileBase
		Implements IImportJob

#Region "Const Fields"

		Public Shared ReadOnly RestartTimeEventMsg As String = "Reset time for import rolling average"
		Public Shared ReadOnly CancelEventMsg As String = "cancel import"

#End Region

#Region "Members"

		Private Const _COPY_TEXT_FILE_BUFFER_SIZE As Int32 = 40000
		Private Const _UNKNOWN_PARENT_FOLDER_ID As Int32 = -9
		Private Const _LENGTH_OF_FOLDER_ALLOWED As Integer = 255
		Public Const DATA_GRID_ID_FIELD_NAME As String = "DataGridID"
		Public Const ERROR_MESSAGE_FOLDER_NAME_TOO_LONG As String = "Error occurred when importing the document. The folder name is longer than 255 characters."

		Private ReadOnly _copyFileToRepository As Boolean
		Private ReadOnly _caseInfo As CaseInfo
		Private ReadOnly _usePipeliningForNativeAndObjectImports As Boolean
		Private ReadOnly _createFoldersInWebApi As Boolean
		Private ReadOnly _createErrorForEmptyNativeFile As Boolean
		Private ReadOnly _enforceDocumentLimit As Boolean

		Protected Overwrite As ImportOverwriteType
		Protected AuditManager As Service.AuditManager
		Protected RelativityManager As Service.RelativityManager

		Protected RecordCount As Int64 = -1
		Private _allFields As kCura.EDDS.WebAPI.DocumentManagerBase.Field()
		Private _fieldsForCreate As kCura.EDDS.WebAPI.DocumentManagerBase.Field()
		Protected ProcessedDocumentIdentifiers As Collections.Specialized.NameValueCollection
		Protected WithEvents Context As ProcessContext
		Protected Offset As Int32 = 0
		Protected FirstTimeThrough As Boolean
		Private _importBatchSize As Int32?
		Private _jobCompleteBatchSize As Int32?
		Private _importBatchVolume As Int32?
		Private _minimumBatchSize As Int32?
		Protected DestinationFolderColumnIndex As Int32 = -1
		Protected FolderCache As FolderCache
		Private _defaultDestinationFolderPath As String = String.Empty
		Private _oixFileLookup As System.Collections.Specialized.HybridDictionary
		Private _fieldArtifactIds As Int32()
		Protected OutputFileWriter As OutputFileWriter
		Protected OverlayArtifactId As Int32
		Protected RunId As String = System.Guid.NewGuid.ToString.Replace("-", "_")
		Private _lastRunMetadataImport As Int64 = 0
		Private _timekeeper As ITimeKeeperManager
		Private _filePath As String
		Private _batchCounter As Int32 = 0
		Private _jobCompleteNativeCount As Int32 = 0
		Private _jobCompleteMetadataCount As Int32 = 0
		Private _errorMessageFileLocation As String = String.Empty
		Private _errorLinesFileLocation As String = String.Empty

		Public MaxNumberOfErrorsInGrid As Int32 = AppSettings.Instance.DefaultMaxErrorCount
		Private _errorCount As Int32 = 0
		Private _prePushErrorLineNumbersFileName As String = String.Empty
		Private _processId As Guid
		Private _parentArtifactTypeId As Int32?
		Private _unmappedRelationalFields As System.Collections.ArrayList

		Protected BulkLoadFileFieldDelimiter As String

		Protected Property LinkDataGridRecords As Boolean
		Public Property Timekeeper As ITimeKeeperManager
			Get
				If (_timekeeper Is Nothing) Then
					_timekeeper = New DefaultTimeKeeperManager()
				End If
				Return _timekeeper
			End Get
			Set(value As ITimeKeeperManager)
				_timekeeper = value
			End Set
		End Property

#End Region

#Region "Accessors"

		Public Property DisableNativeValidation As Boolean = AppSettings.Instance.DisableOutsideInFileIdentification
		Public Property DisableUserSecurityCheck As Boolean
		Public Property AuditLevel As kCura.EDDS.WebAPI.BulkImportManagerBase.ImportAuditLevel = Config.AuditLevel
		Public ReadOnly Property BatchSizeHistoryList As System.Collections.Generic.List(Of Int32)

		Protected Overridable ReadOnly Property NumberOfRetries As Int32
			Get
				Return AppSettings.Instance.IoErrorNumberOfRetries
			End Get
		End Property

		Protected Overridable ReadOnly Property WaitTimeBetweenRetryAttempts As Int32
			Get
				Return AppSettings.Instance.IoErrorWaitTimeInSeconds
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
					For Each field As kCura.EDDS.WebAPI.DocumentManagerBase.Field In Me.AllFields(ArtifactType.Document)
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
					If field.FieldCategoryID = FieldCategory.FileInfo Then retVal = field
				Next
				Return retVal
			End Get
		End Property

		Public ReadOnly Property FullTextField(ByVal artifactTypeID As Int32) As kCura.EDDS.WebAPI.DocumentManagerBase.Field
			Get
				For Each field As kCura.EDDS.WebAPI.DocumentManagerBase.Field In Me.AllFields(artifactTypeID)
					If field.FieldCategory = EDDS.WebAPI.DocumentManagerBase.FieldCategory.FullText Then
						Return field
					End If
				Next
				Return Nothing
			End Get
		End Property

		Public Overridable ReadOnly Property HasErrors() As Boolean
			Get
				Return _errorCount > 0
			End Get
		End Property

		Public ReadOnly Property UploadConnection() As TapiClient
			Get
				Return Me.FileTapiClient
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
						If Not item.DocumentField Is Nothing AndAlso item.DocumentField.FieldCategory = FieldCategory.Relational AndAlso item.DocumentField.ImportBehavior = kCura.EDDS.WebAPI.DocumentManagerBase.ImportBehaviorChoice.ReplaceBlankValuesWithIdentifier Then
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
				If Not _parentArtifactTypeId.HasValue Then
					Dim parentQuery As New Service.ObjectTypeManager(_settings.Credentials, _settings.CookieContainer)
					_parentArtifactTypeId = CType(parentQuery.RetrieveParentArtifactTypeID(_settings.CaseInfo.ArtifactID, _settings.ArtifactTypeID).Tables(0).Rows(0)("ParentArtifactTypeID"), Int32)
				End If
				Return _parentArtifactTypeId.Value
			End Get
		End Property

		Protected Overridable Property MinimumBatchSize As Int32
			Get
				If Not _minimumBatchSize.HasValue Then _minimumBatchSize = AppSettings.Instance.MinBatchSize
				Return _minimumBatchSize.Value
			End Get
			Set(ByVal value As Int32)
				_minimumBatchSize = value
			End Set
		End Property

		Protected Property ImportBatchSize As Int32
			Get
				If Not _importBatchSize.HasValue Then _importBatchSize = AppSettings.Instance.ImportBatchSize
				Return _importBatchSize.Value
			End Get
			Set(ByVal value As Int32)
				_importBatchSize = If(value > MinimumBatchSize, value, MinimumBatchSize)
			End Set
		End Property

		Protected Property JobCompleteBatchSize As Int32
			Get
				If Not _jobCompleteBatchSize.HasValue Then _jobCompleteBatchSize = AppSettings.Instance.JobCompleteBatchSize
				Return _jobCompleteBatchSize.Value
			End Get
			Set(ByVal value As Int32)
				_jobCompleteBatchSize = If(value > MinimumBatchSize, value, MinimumBatchSize)
			End Set
		End Property

		Protected Property ImportBatchVolume As Int32
			Get
				If Not _importBatchVolume.HasValue Then _importBatchVolume = AppSettings.Instance.ImportBatchMaxVolume
				Return _importBatchVolume.Value
			End Get
			Set(ByVal value As Int32)
				_importBatchVolume = value
			End Set
		End Property

		Protected Overridable ReadOnly Property BatchResizeEnabled As Boolean
			Get
				Return AppSettings.Instance.DynamicBatchResizingOn
			End Get
		End Property

#End Region

#Region "Constructors"

		''' <summary>
		''' Constructs a new importer that will prepare a bulk load file from a provided file.
		''' </summary>
		''' <param name="args">Information about the file being loaded</param>
		''' <param name="context">The process context</param>
		''' <param name="reporter">The object that performs I/O operations and publishes messages during retry operations.</param>
		''' <param name="logger">The Relativity logger.</param>
		''' <param name="timeZoneOffset">The running context's time zone offset from UTC</param>
		''' <param name="initializeUploaders">Sets whether or not the uploaders should be initialized
		''' for use</param>
		''' <param name="processID">The identifier of the process running</param>
		''' <param name="bulkLoadFileFieldDelimiter">Sets the field delimiter to use when writing
		''' out the bulk load file. Line delimiters will be this value plus a line feed.</param>
		''' <param name="executionSource">Optional parameter that states where the import
		''' is coming from.</param>
		''' <exception cref="ArgumentNullException">Thrown if <paramref name="bulkLoadFileFieldDelimiter"/>
		''' is <c>null</c> or <c>String.Empty</c>.</exception>
		Public Sub New(args As LoadFile, _
		               context As ProcessContext, _
		               reporter As IIoReporter, _
		               logger As Global.Relativity.Logging.ILog, _
		               timeZoneOffset As Int32, _
		               initializeUploaders As Boolean, _
		               processID As Guid, _
		               doRetryLogic As Boolean, _
		               bulkLoadFileFieldDelimiter As String, _
		               enforceDocumentLimit As Boolean, _
		               tokenSource As CancellationTokenSource, _
					   ByVal Optional executionSource As ExecutionSource = ExecutionSource.Unknown)
			Me.New(args, _
			       context, _
			       reporter, _
			       logger, _
			       timeZoneOffset, _
			       True, _
			       initializeUploaders, _
			       processID, _
			       doRetryLogic, _
			       bulkLoadFileFieldDelimiter, _
			       enforceDocumentLimit, _
			       tokenSource, _
			       initializeArtifactReader:=True, _
			       executionSource:=executionSource)
		End Sub

		''' <summary>
		''' Constructs a new importer that will prepare a bulk load file from a provided file.
		''' </summary>
		''' <param name="args">Information about the file being loaded</param>
		''' <param name="context">The process context</param>
		''' <param name="reporter">The object that performs I/O operations and publishes messages during retry operations.</param>
		''' <param name="logger">The Relativity logger.</param>
		''' <param name="timeZoneOffset">The running context's time zone offset from UTC</param>
		''' <param name="initializeUploaders">Sets whether or not the uploaders should be initialized
		''' for use</param>
		''' <param name="processID">The identifier of the process running</param>
		''' <param name="bulkLoadFileFieldDelimiter">Sets the field delimiter to use when writing
		''' out the bulk load file. Line delimiters will be this value plus a line feed.</param>
		''' <param name="executionSource">Optional parameter that states where the import
		''' is coming from.</param>
		''' <exception cref="ArgumentNullException">Thrown if <paramref name="bulkLoadFileFieldDelimiter"/>
		''' is <c>null</c> or <c>String.Empty</c>.</exception>
		Public Sub New(args As LoadFile, _
		               context As ProcessContext, _
		               reporter As IIoReporter, _
		               logger As Global.Relativity.Logging.ILog, _
		               timeZoneOffset As Int32, _
		               autoDetect As Boolean, _
		               initializeUploaders As Boolean, _
		               processID As Guid, _
		               doRetryLogic As Boolean, _
		               bulkLoadFileFieldDelimiter As String, _
		               enforceDocumentLimit As Boolean, _
		               tokenSource As CancellationTokenSource, _
		               ByVal Optional executionSource As ExecutionSource = ExecutionSource.Unknown)
			Me.New(args, _
			       context, _
			       reporter, _
			       logger, _
			       timeZoneOffset, _
			       autoDetect, _
			       initializeUploaders, _
			       processID, _
			       doRetryLogic,
			       bulkLoadFileFieldDelimiter, _
			       enforceDocumentLimit, _
			       tokenSource, _
			       initializeArtifactReader:=True, _
			       executionSource:=executionSource)
		End Sub

		''' <summary>
		''' Constructs a new importer that will prepare a bulk load file from a provided file.
		''' </summary>
		''' <param name="args">Information about the file being loaded</param>
		''' <param name="context">The process context</param>
		''' <param name="reporter">The object that performs I/O operations and publishes messages during retry operations.</param>
		''' <param name="logger">The Relativity logger.</param>
		''' <param name="timeZoneOffset">The running context's time zone offset from UTC</param>
		''' <param name="initializeUploaders">Sets whether or not the uploaders should be initialized
		''' for use</param>
		''' <param name="processID">The identifier of the process running</param>
		''' <param name="bulkLoadFileFieldDelimiter">Sets the field delimiter to use when writing
		''' out the bulk load file. Line delimiters will be this value plus a line feed.</param>
		''' <param name="executionSource">Optional parameter that states where the import
		''' is coming from.</param>
		''' <exception cref="ArgumentNullException">Thrown if <paramref name="bulkLoadFileFieldDelimiter"/>
		''' is <c>null</c> or <c>String.Empty</c>.</exception>
		Public Sub New(args As LoadFile, _
		               context As ProcessContext, _
		               reporter As IIoReporter, _
		               logger As Global.Relativity.Logging.ILog, _
		               timeZoneOffset As Int32, _
		               autoDetect As Boolean, _
		               initializeUploaders As Boolean, _
		               processID As Guid, _
		               doRetryLogic As Boolean, _
		               bulkLoadFileFieldDelimiter As String, _
		               enforceDocumentLimit As Boolean, _
		               tokenSource As CancellationTokenSource,
		               initializeArtifactReader As Boolean,
		               ByVal Optional executionSource As ExecutionSource = ExecutionSource.Unknown)
			MyBase.New(args, _
			           reporter, _
			           logger, _
			           timeZoneOffset, _
			           doRetryLogic, _
			           autoDetect, _
			           tokenSource, _
			           initializeArtifactReader, _
			           executionSource:=executionSource)

			' Avoid excessive concurrent dictionary hits by caching frequently used config settings.
			_usePipeliningForNativeAndObjectImports = AppSettings.Instance.UsePipeliningForNativeAndObjectImports
			_createFoldersInWebApi = AppSettings.Instance.CreateFoldersInWebApi
			_createErrorForEmptyNativeFile = AppSettings.Instance.CreateErrorForEmptyNativeFile

			' get an instance of the specific type of artifact reader so we can get the fieldmapped event
			_enforceDocumentLimit = enforceDocumentLimit

			ShouldImport = True
			If (String.IsNullOrEmpty(args.OverwriteDestination)) Then
				Overwrite = ImportOverwriteType.Append
			Else
				Overwrite = CType([Enum].Parse(GetType(ImportOverwriteType), args.OverwriteDestination, True), ImportOverwriteType)
			End If
			If args.CopyFilesToDocumentRepository Then
				'DEFECT: SF#226211, repositories without trailing \ caused import to fail. Changed to use Path.Combine. -tmh
				Dim lastHalfPath As String = "EDDS" & args.CaseInfo.ArtifactID & "\"
				_defaultDestinationFolderPath = System.IO.Path.Combine(args.SelectedCasePath, lastHalfPath)
				If args.ArtifactTypeID <> ArtifactType.Document Then
					For Each item As LoadFileFieldMap.LoadFileFieldMapItem In args.FieldMap
						If Not item.DocumentField Is Nothing AndAlso item.NativeFileColumnIndex > -1 AndAlso item.DocumentField.FieldTypeID = FieldType.File Then
							_defaultDestinationFolderPath &= "File" & item.DocumentField.FieldID & "\"
						End If
					Next
				End If
			End If
			If initializeUploaders Then
				CreateUploaders(args)
			End If
			_copyFileToRepository = args.CopyFilesToDocumentRepository

			If Not _createFoldersInWebApi Then
				'Client side folder creation (added back for Dominus# 1127879)
				If autoDetect Then _folderManager.Read(args.CaseInfo.ArtifactID, args.CaseInfo.RootFolderID)
			End If

			Me.Context = context
			FirstTimeThrough = True
			_caseInfo = args.CaseInfo
			_settings = args
			_processId = processID
			_startLineNumber = args.StartLineNumber
			OverlayArtifactId = args.IdentityFieldId

			If String.IsNullOrEmpty(bulkLoadFileFieldDelimiter) Then
				Throw New ArgumentNullException("bulkLoadFileFieldDelimiter")
			End If

			Me.BulkLoadFileFieldDelimiter = bulkLoadFileFieldDelimiter

			BatchSizeHistoryList = New System.Collections.Generic.List(Of Int32)

		End Sub


		Protected Overridable Sub CreateUploaders(ByVal args As LoadFile)
			Dim gateway As Service.FileIO = New Service.FileIO(args.Credentials, args.CookieContainer)
			Dim nativeParameters As UploadTapiBridgeParameters2 = New UploadTapiBridgeParameters2
			nativeParameters.Application = AppSettings.Instance.ApplicationName
			nativeParameters.BcpFileTransfer = False
			nativeParameters.AsperaBcpRootFolder = String.Empty

			' This will tie both native and BCP to a single unique identifier.
			nativeParameters.ClientRequestId = Guid.NewGuid()
			nativeParameters.Credentials = If(args.TapiCredentials, args.Credentials)
			nativeParameters.AsperaDocRootLevels = AppSettings.Instance.TapiAsperaNativeDocRootLevels
			nativeParameters.FileShare = args.CaseInfo.DocumentPath
			nativeParameters.ForceAsperaClient = AppSettings.Instance.TapiForceAsperaClient
			nativeParameters.ForceClientCandidates = AppSettings.Instance.TapiForceClientCandidates
			nativeParameters.ForceFileShareClient = AppSettings.Instance.TapiForceFileShareClient
			nativeParameters.ForceHttpClient = AppSettings.Instance.ForceWebUpload OrElse AppSettings.Instance.TapiForceHttpClient
			nativeParameters.LargeFileProgressEnabled = AppSettings.Instance.TapiLargeFileProgressEnabled
			nativeParameters.LogConfigFile = AppSettings.Instance.LogConfigXmlFileName
			nativeParameters.MaxFilesPerFolder = gateway.RepositoryVolumeMax
			nativeParameters.MaxJobParallelism = AppSettings.Instance.TapiMaxJobParallelism
			nativeParameters.MaxJobRetryAttempts = Me.NumberOfRetries
			nativeParameters.MinDataRateMbps = AppSettings.Instance.TapiMinDataRateMbps
			nativeParameters.PreserveFileTimestamps = AppSettings.Instance.TapiPreserveFileTimestamps
			nativeParameters.SubmitApmMetrics = AppSettings.Instance.TapiSubmitApmMetrics
			nativeParameters.TargetPath = Me._defaultDestinationFolderPath
			nativeParameters.TargetDataRateMbps = AppSettings.Instance.TapiTargetDataRateMbps
			nativeParameters.TimeoutSeconds = AppSettings.Instance.HttpTimeoutSeconds
			nativeParameters.TransferLogDirectory = AppSettings.Instance.TapiTransferLogDirectory
			nativeParameters.WaitTimeBetweenRetryAttempts = Me.WaitTimeBetweenRetryAttempts
			nativeParameters.WebCookieContainer = args.CookieContainer
			nativeParameters.WebServiceUrl = AppSettings.Instance.WebApiServiceUrl
			nativeParameters.WorkspaceId = args.CaseInfo.ArtifactID
			nativeParameters.PermissionErrorsRetry = AppSettings.Instance.PermissionErrorsRetry
			nativeParameters.BadPathErrorsRetry = AppSettings.Instance.TapiBadPathErrorsRetry

			' Copying the parameters and tweaking just a few BCP specific parameters.
			Dim bcpParameters As UploadTapiBridgeParameters2 = nativeParameters.ShallowCopy()
			bcpParameters.BcpFileTransfer = True
			bcpParameters.AsperaBcpRootFolder = AppSettings.Instance.TapiAsperaBcpRootFolder
			bcpParameters.FileShare = gateway.GetBcpSharePath(args.CaseInfo.ArtifactID)
			bcpParameters.SupportCheckPath = bcpParameters.FileShare
			bcpParameters.SortIntoVolumes = False
			bcpParameters.ForceHttpClient = bcpParameters.ForceHttpClient Or AppSettings.Instance.TapiForceBcpHttpClient

			' Never preserve timestamps for BCP load files.
			bcpParameters.PreserveFileTimestamps = false
			CreateTapiBridges(nativeParameters, bcpParameters)
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

		''' <summary>
		''' Safely deletes all temporary load files that are auto-generated when this object is created.
		''' </summary>
		''' <remarks>
		''' API users are responsible for calling this method when <see cref="ReadFile"/> is not executed.
		''' </remarks>
		Public Sub DeleteTempLoadFiles()
			If (Not OutputFileWriter Is Nothing) Then
				Me.OutputFileWriter.DeleteFiles()
			End If
		End Sub

		Public Sub WriteCodeLineToTempFile(ByVal documentIdentifier As String, ByVal codeArtifactID As Int32, ByVal codeTypeID As Int32)
			If (Me.OutputFileWriter Is Nothing) Then
				Throw New InvalidOperationException($"The code artifact '{codeArtifactID}' associated with '{documentIdentifier}' cannot be added to the load file because the file no longer exists. Try again. If the problem persists, please contact your system administrator for assistance.")
			End If

			Me.OutputFileWriter.OutputCodeFileWriter.WriteLine(String.Format("{1}{0}{2}{0}{3}{0}", BulkLoadFileFieldDelimiter, documentIdentifier, codeArtifactID, codeTypeID))
		End Sub

		Public Sub WriteObjectLineToTempFile(ByVal ownerIdentifier As String, ByVal objectName As String, ByVal artifactID As Int32, ByVal objectTypeArtifactID As Int32, ByVal fieldID As Int32)
			If (Me.OutputFileWriter Is Nothing) Then
				Throw New InvalidOperationException($"The object artifact '{artifactID}' associated with '{ownerIdentifier}' cannot be added to the load file because the file no longer exists. Try again. If the problem persists, please contact your system administrator for assistance.")
			End If

			Me.OutputFileWriter.OutputObjectFileWriter.WriteLine(String.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}", BulkLoadFileFieldDelimiter, ownerIdentifier, objectName, artifactID, objectTypeArtifactID, fieldID))
		End Sub

#End Region

#Region "Main"

		Private Sub PublishUploadModeEvent()
			Dim retval As New List(Of String)
			If Not BulkLoadTapiBridge Is Nothing Then
				retval.Add("Metadata: " & BulkLoadTapiClientName)
			End If

			If _settings.CopyFilesToDocumentRepository AndAlso _settings.NativeFilePathColumn IsNot Nothing Then
				If Not String.IsNullOrEmpty(FileTapiClientName) Then
					retval.Add("Files: " & FileTapiClientName)
				End If
			Else
				retval.Add("Files: not copied")
			End If
			If retval.Any() Then
				Dim uploadStatus As String = String.Join(" - ", retval.ToArray())

				' Note: single vs. bulk mode is a vestige. Bulk mode is always true.
				OnUploadModeChangeEvent(uploadStatus, True)
			End If
		End Sub

		''' <summary>
		''' Loads all the documents in a load file
		''' </summary>
		''' <param name="path">The load file which contains information about the document being loaded</param>
		''' <returns>True indicates success.  False or Nothing indicates failure.</returns>
		''' <remarks></remarks>
		Public Overridable Function ReadFile(ByVal path As String) As Object Implements IImportJob.ReadFile
			Dim line As Api.ArtifactFieldCollection
			_filePath = path
			Try
				Using Timekeeper.CaptureTime("TOTAL")
					OnStartFileImport()
					Using Timekeeper.CaptureTime("ReadFile_InitializeMembers")
						If Not InitializeMembers(path) Then
							Return False
						End If
						ProcessedDocumentIdentifiers = New Collections.Specialized.NameValueCollection
					End Using

					If (_enforceDocumentLimit) Then
						If (Overwrite = ImportOverwriteType.Append And _artifactTypeID = ArtifactType.Document) Then
							Dim currentDocCount As Int32 = _documentManager.RetrieveDocumentCount(_caseInfo.ArtifactID)
							Dim docLimit As Int32 = _documentManager.RetrieveDocumentLimit(_caseInfo.ArtifactID)
							Dim fileLineStart As Long = _startLineNumber
							If _startLineNumber <= 0 Then fileLineStart = 1
							Dim countAfterJob As Long = currentDocCount + (RecordCount - (fileLineStart - 1))
							If (docLimit <> 0 And countAfterJob > docLimit) Then
								Dim errorMessage As String = String.Format("The document import was canceled.  It would have exceeded the workspace's document limit of {1} by {0} documents.", countAfterJob - docLimit, docLimit)
								Throw New Exception(errorMessage)
								Return False
							End If
						End If
					End If

					Me.LogInformation("Preparing to import documents via WinEDDS.")
					Using Timekeeper.CaptureTime("ReadFile_ProcessDocuments")
						_columnHeaders = _artifactReader.GetColumnNames(_settings)
						If _firstLineContainsColumnNames Then Offset = -1
						Statistics.BatchSize = Me.ImportBatchSize
						JobCounter = 1
						Me.TotalTransferredFilesCount = 0
						Using fileService As OutsideInFileIdService = New OutsideInFileIdService()
							While ShouldImport AndAlso _artifactReader.HasMoreRecords
								Try
									If Me.CurrentLineNumber < _startLineNumber Then
										Me.AdvanceLine()

										' This will ensure progress takes into account the start line number
										FileTapiProgressCount += 1
									Else
										Using Timekeeper.CaptureTime("ReadFile_GetLine")
											Statistics.DocCount += 1
											'The EventType.Count is used as an 'easy' way for the ImportAPI to eventually get a record count.
											' It could be done in DataReaderClient in other ways, but those ways turned out to be pretty messy.
											' -Phil S. 06/12/2012
											WriteStatusLine(EventType.Count, String.Empty)
											line = _artifactReader.ReadArtifact
										End Using
										Dim lineStatus As Int32 = 0
										'If line.Count <> _columnHeaders.Length Then
										'	lineStatus += ImportStatus.ColumnMismatch								 'Throw New ColumnCountMismatchException(Me.CurrentLineNumber, _columnHeaders.Length, line.Length)
										'End If

										Dim id As String
										Using Timekeeper.CaptureTime("ReadFile_ManageDocument")
											id = ManageDocument(fileService, line, lineStatus)
										End Using

										Using Timekeeper.CaptureTime("ReadFile_IdTrack")
											ProcessedDocumentIdentifiers.Add(id, CurrentLineNumber.ToString)
										End Using
									End If
								Catch ex As LoadFileBase.CodeCreationException
									If ex.IsFatal Then
										WriteFatalError(Me.CurrentLineNumber, ex)
										Me.LogFatal(ex, "A fatal code operation error has occurred managing an import document.")
									Else
										WriteError(Me.CurrentLineNumber, ex.Message)
										Me.LogError(ex, "A serious code operation error has occurred managing an import document.")
									End If
								Catch ex As System.IO.PathTooLongException
									WriteError(Me.CurrentLineNumber, ERROR_MESSAGE_FOLDER_NAME_TOO_LONG)
									Me.LogError(ex, "An import error has occured because of invalid document path - the path is too long.")
								Catch ex As ImporterException
									WriteError(Me.CurrentLineNumber, ex.Message)
									Me.LogError(ex, "An import data error has occurred managing an import document.")
								Catch ex As FileInfoInvalidPathException
									WriteError(Me.CurrentLineNumber, ex.Message)
									Me.LogError(ex, "An import error has occured because of invalid document path - illegal characters in path.")
								Catch ex As System.IO.FileNotFoundException
									WriteError(Me.CurrentLineNumber, ex.Message)
									Me.LogError(ex, "A file not found error has occurred managing an import document.")
								Catch ex As FileIdException
									WriteError(Me.CurrentLineNumber, ex.Message)
									Me.LogError(ex, "An error occured identifying type of native file.")
								Catch ex As System.UnauthorizedAccessException
									WriteFatalError(Me.CurrentLineNumber, ex)
									Me.LogFatal(ex, "A fatal import error has occurred because the user doesn't have authorized access to the document.")
								Catch ex As System.Exception
									WriteFatalError(Me.CurrentLineNumber, ex)
									Me.LogFatal(ex, "A fatal unexpected error has occurred managing an import document.")
								End Try
							End While

							' Dump OI details.
							' Preserving existing behavior where this can never fail.
							Try
								Dim FileIdConfiguration As FileIdConfiguration = fileService.Configuration
								If FileIdConfiguration.HasError Then
									Me.LogError("OI Configuration Info")
									Me.LogError("OI version: {Version}", FileIdConfiguration.Version)
									Me.LogError("OI idle worker timeout: {Timeout} seconds", FileIdConfiguration.Timeout)
									Me.LogError("OI install path: {InstallPath}", FileIdConfiguration.InstallDirectory)
									If Not FileIdConfiguration.Exception Is Nothing Then
										Me.LogError(FileIdConfiguration.Exception, "OI runtime exception.", FileIdConfiguration.Exception)
									End If
								Else
									Me.LogInformation("OI Configuration Info")
									Me.LogInformation("OI version: {Version}", FileIdConfiguration.Version)
									Me.LogInformation("OI idle worker timeout: {Timeout}", FileIdConfiguration.Timeout)
									Me.LogInformation("OI install path: {InstallPath}", FileIdConfiguration.InstallDirectory)
								End If
							Catch ex As FileIDException
								Me.LogError(ex, "Failed to retrieve OI configuration info.")
							End Try
						End Using

						If Not _task Is Nothing AndAlso _task.Status.In(
							Threading.Tasks.TaskStatus.Running,
							Threading.Tasks.TaskStatus.WaitingForActivation,
							Threading.Tasks.TaskStatus.WaitingForChildrenToComplete,
							Threading.Tasks.TaskStatus.WaitingToRun) Then
							WaitOnPushBatchTask()
						End If
					End Using
					Using Timekeeper.CaptureTime("ReadFile_OtherFinalization")
						Me.TryPushNativeBatch(True, True, True)
						WaitOnPushBatchTask()
						RaiseEvent EndFileImport(RunId)
						WriteEndImport("Finish")
						_artifactReader.Close()
					End Using
				End Using
				Timekeeper.GenerateCsvReportItemsAsRows("_winedds", "C:\")
				Me.LogInformation("Successfully imported {ImportCount} documents via WinEDDS.", Me.FileTapiProgressCount)
				Me.DumpStatisticsInfo()
				Return True
			Catch ex As System.Exception
				Me.WriteFatalError(Me.CurrentLineNumber, ex)
				Me.LogFatal(ex, "A serious unexpected error has occurred importing documents.")
				Me.DumpStatisticsInfo()
			Finally
				Using Timekeeper.CaptureTime("ReadFile_CleanupTempTables")
					DestroyTapiBridges()
					CleanupTempTables()

					' Deletes all temp load files too.
					If Not Me.OutputFileWriter Is Nothing Then
						Me.OutputFileWriter.Dispose()
						Me.OutputFileWriter = Nothing
					End If
				End Using
			End Try
			Return Nothing
		End Function

		Private Function InitializeMembers(ByVal path As String) As Boolean
			RecordCount = _artifactReader.CountRecords
			If RecordCount = -1 Then
				OnStatusMessage(New StatusEventArgs(EventType.Progress, CurrentLineNumber, CurrentLineNumber, CancelEventMsg, CurrentStatisticsSnapshot, Statistics))
				Return False
			End If

			Me.InitializeFolderManagement()
			Me.InitializeFieldIdList()
			Me.DeleteTempLoadFiles()
			Me.OpenFileWriters()
			Me.OnStatusMessage(New StatusEventArgs(EventType.ResetStartTime, 0, RecordCount, RestartTimeEventMsg, Nothing, Statistics))

			' Counting all lines increments progress to 100%.
			' This will reset progress back to zero instead of waiting for the first transfer to complete.
			Me.OnStatusMessage(New StatusEventArgs(EventType.ResetProgress, 0, RecordCount, "Starting import...", Nothing, Statistics))
			Return True
		End Function

		Protected Overrides Sub InitializeManagers(ByVal args As LoadFile)
			MyBase.InitializeManagers(args)
			AuditManager = New Service.AuditManager(args.Credentials, args.CookieContainer)
			_documentManager = New Service.DocumentManager(args.Credentials, args.CookieContainer)
			RelativityManager = New Service.RelativityManager(args.Credentials, args.CookieContainer)
		End Sub

		Protected Sub InitializeFolderManagement()
			If _createFolderStructure Then
				If Not _createFoldersInWebApi Then
					'Client side folder creation (added back for Dominus# 1127879)
					If _artifactTypeID = ArtifactType.Document Then
						FolderCache = New FolderCache(Me.Logger, _folderManager, _folderID, _caseArtifactID)
					End If
				End If
				Dim openParenIndex As Int32 = _destinationFolder.LastIndexOf("("c) + 1
				Dim closeParenIndex As Int32 = _destinationFolder.LastIndexOf(")"c)
				DestinationFolderColumnIndex = Int32.Parse(_destinationFolder.Substring(openParenIndex, closeParenIndex - openParenIndex)) - 1
			End If
		End Sub

		Private Sub InitializeFieldIdList()
			Dim fieldIdList As New System.Collections.ArrayList
			For Each item As LoadFileFieldMap.LoadFileFieldMapItem In _fieldMap
				If Not item.DocumentField Is Nothing AndAlso Not item.NativeFileColumnIndex = -1 Then
					'If item.DocumentField.FieldCategoryID <> FieldCategory.FullText Then fieldIdList.Add(item.DocumentField.FieldID)
					fieldIdList.Add(item.DocumentField.FieldID)
				End If
			Next
			fieldIdList.Add(Me.FileInfoField(_artifactTypeID).ArtifactID)
			_fieldArtifactIds = DirectCast(fieldIdList.ToArray(GetType(Int32)), Int32())
		End Sub

		Private Function ManageDocument(ByVal fileService As OutsideInFileIdService, ByVal record As Api.ArtifactFieldCollection, ByVal lineStatus As Int64) As String
			Dim filename As String = String.Empty
			Dim originalFilename As String = String.Empty
			Dim fileGuid As String = String.Empty
			Dim uploadFile As Boolean = record.FieldList(FieldType.File).Length > 0 AndAlso Not record.FieldList(FieldType.File)(0).Value Is Nothing
			Dim fileExists As Boolean
			Dim identityValue As String = String.Empty
			Dim parentFolderID As Int32
			Dim fullFilePath As String = String.Empty
			Dim oixFileIdInfo As FileIdInfo = Nothing
			Dim destinationVolume As String = Nothing
			Dim injectableContainer As Api.IInjectableFieldCollection = TryCast(record, Api.IInjectableFieldCollection)

			Dim injectableContainerIsNothing As Boolean = injectableContainer Is Nothing

			Const retry As Boolean = True
			Using Timekeeper.CaptureTime("ManageDocument_Filesystem")
				If uploadFile AndAlso _artifactTypeID = ArtifactType.Document Then
					filename = record.FieldList(FieldType.File)(0).Value.ToString
					If filename.Length > 1 AndAlso filename.Chars(0) = "\" AndAlso filename.Chars(1) <> "\" Then
						filename = "." & filename
					End If
					originalFilename = filename

					If Me.DisableNativeLocationValidation Then
						fileExists = True
					Else
						Dim foundFileName As String = Me.GetExistingFilePath(filename, retry)
						fileExists = Not String.IsNullOrEmpty(foundFileName)

						If fileExists AndAlso (Not String.Equals(filename, foundFileName))
							WriteWarning($"File {filename} does not exist. File {foundFileName} will be used instead.")
							filename = foundFileName
						End If
					End If

					If filename.Trim.Equals(String.Empty) Then
						fileExists = False
					End If

					If filename <> String.Empty AndAlso Not fileExists Then
						lineStatus += ImportStatus.FileSpecifiedDne
					End If
					If fileExists AndAlso Not Me.DisableNativeLocationValidation Then
						If Me.GetFileLength(filename, retry) = 0 Then
							If _createErrorForEmptyNativeFile Then
								lineStatus += ImportStatus.EmptyFile
							Else
								WriteWarning($"Note that file {filename} has been detected as empty, metadata and the native file will be loaded.")
							End If
						End If
					End If
					fullFilePath = filename
					If fileExists Then
						Dim now As Date = Date.Now

						Try
							If Me.DisableNativeValidation Then
								oixFileIdInfo = Nothing
							Else
								Dim idDataExtractor As Api.IHasOixFileType = Nothing
								If (Not injectableContainerIsNothing) Then
									idDataExtractor = injectableContainer.FileIdInfo
								End If

								If (idDataExtractor Is Nothing) Then
									' REL-165493: Added OI resiliency and properly address FileNotFoundException scenarios.
									Dim maxRetryAttempts As Integer = Me.NumberOfRetries
									Dim currentRetryAttempt As Integer = 0
									Dim policy As IWaitAndRetryPolicy = Me.CreateWaitAndRetryPolicy()
									oixFileIdInfo = policy.WaitAndRetry(
										Function(exception)
											Dim outsideInException As FileIdException = TryCast(exception, FileIdException)
											If (Not outsideInException Is Nothing)
												If (outsideInException.Error = FileIdError.Permissions) Then
													' Only perform a retry operation if configured to do so.
													Return Me.RetryOptions.HasFlag(RetryOptions.Permissions)
												End If

												' All other OI exceptions are retried.
												Return True
											End If

											' Otherwise, rely on the standard retry mechanism.
											Dim retryResult As Boolean = RetryExceptionHelper.IsRetryable(exception, Me.RetryOptions)
											Return retryResult
										End Function,
										Function(count)
											' Force OI to get reinitialized in the event the runtime configuration is invalid.
											currentRetryAttempt = count
											If count > 1 Then
												fileService.Reinitialize()
											End If
											Return TimeSpan.FromSeconds(Me.WaitTimeBetweenRetryAttempts)
										End Function,
										Sub(exception, span)
											Me.PublishIoRetryMessage(exception, span, currentRetryAttempt, maxRetryAttempts)
										End Sub,
										Function() As FileIdInfo
											Return fileService.Identify(fullFilePath)
										End Function,
										Me.CancellationToken)
								Else
									oixFileIdInfo = idDataExtractor.GetFileIdInfo()
								End If
							End If

							If _copyFileToRepository Then
								If Not Me.DisableNativeLocationValidation AndAlso fileExists OrElse Me.GetFileExists(filename, retry) Then
									Dim guid As String = System.Guid.NewGuid().ToString()
									Me.ImportFilesCount += 1
									_jobCompleteNativeCount += 1
									fileGuid = FileTapiBridge.AddPath(filename, guid, Me.CurrentLineNumber)
									destinationVolume = FileTapiBridge.TargetFolderName
								Else
									WriteWarning($"File {filename} does not exist and will be not uploaded")
								End If
							Else
								fileGuid = System.Guid.NewGuid.ToString
							End If

							If (Not injectableContainerIsNothing AndAlso injectableContainer.HasFileName()) Then
								filename = injectableContainer.FileName.GetFileName()
							Else
								filename = System.IO.Path.GetFileName(originalFilename)
							End If
						Catch ex As System.IO.FileNotFoundException
							If Me.DisableNativeLocationValidation Then
								'Don't do anything. This exception can only happen if DisableNativeLocationValidation is turned on
							Else
								Throw
							End If
						End Try

						' Status must be handled the pre-TAPI way whenever the copy repository option is disabled.
						If ShouldImport AndAlso Not _copyFileToRepository Then
							WriteStatusLine(EventType.Status, String.Format("End upload file. ({0}ms)", DateTime.op_Subtraction(DateTime.Now, now).Milliseconds))
						End If
					End If
				End If
			End Using

			Dim folderPath As String = String.Empty
			Using Timekeeper.CaptureTime("ManageDocument_Folder")
				If _createFolderStructure Then
					If _artifactTypeID = ArtifactType.Document Then
						Dim value As String = NullableTypesHelper.ToEmptyStringOrValue(NullableTypesHelper.DBNullString(record.FieldList(FieldCategory.ParentArtifact)(0).Value))
						If _createFoldersInWebApi Then
							'Server side folder creation
							Dim cleanFolderPath As String = CleanDestinationFolderPath(value)
							If (String.IsNullOrWhiteSpace(cleanFolderPath)) Then
								parentFolderID = _folderID
							ElseIf InnerRelativityFolderPathsAreTooLarge(cleanFolderPath) Then
								Throw New System.IO.PathTooLongException("Error occurred when importing the document. The folder name is longer than 255 characters.")
							Else
								folderPath = cleanFolderPath
								'We're creating the structure on the server side, so it'll get a number then
								parentFolderID = _UNKNOWN_PARENT_FOLDER_ID
							End If
						Else
							'Client side folder creation (added back for Dominus# 1127879)
						parentFolderID = FolderCache.GetFolderId(CleanDestinationFolderPath(value))
						End If
					Else
						'TODO: If we are going to do this for more than documents, fix this as well...
						Dim textIdentifier As String = NullableTypesHelper.ToEmptyStringOrValue(NullableTypesHelper.DBNullString(record.FieldList(FieldCategory.ParentArtifact)(0).Value.ToString))
						If textIdentifier = "" Then
							If Overwrite = ImportOverwriteType.Overlay OrElse Overwrite = ImportOverwriteType.AppendOverlay Then
								parentFolderID = -1
							End If
							Throw New ParentObjectReferenceRequiredException(Me.CurrentLineNumber, DestinationFolderColumnIndex)
						Else
							Dim parentObjectTable As System.Data.DataTable = _objectManager.RetrieveArtifactIdOfMappedParentObject(_caseArtifactID,
																																   textIdentifier, _artifactTypeID).Tables(0)
							If parentObjectTable.Rows.Count > 1 Then
								Throw New DuplicateObjectReferenceException(Me.CurrentLineNumber, DestinationFolderColumnIndex, "Parent Info")
							ElseIf parentObjectTable.Rows.Count = 0 Then
								Throw New NonExistentParentException(Me.CurrentLineNumber, DestinationFolderColumnIndex, "Parent Info")
							Else
								parentFolderID = CType(parentObjectTable.Rows(0)("ArtifactID"), Int32)
							End If
						End If
					End If
				Else
					'If we're not creating the structure, all documents go in the root folder (aka _folderId)
					If _artifactTypeID = ArtifactType.Document OrElse Me.ParentArtifactTypeID = ArtifactType.Case Then
						parentFolderID = _folderID
					Else
						parentFolderID = -1
					End If
				End If
			End Using

			Dim markPrepareFields As DateTime = DateTime.Now
			identityValue = PrepareFieldCollectionAndExtractIdentityValue(record)
			If identityValue = String.Empty Then
				'lineStatus += ImportStatus.EmptyIdentifier				'
				Throw New IdentityValueNotSetException
			ElseIf Not ProcessedDocumentIdentifiers(identityValue) Is Nothing Then
				'lineStatus += ImportStatus.IdentifierOverlap				'
				Throw New IdentifierOverlapException(identityValue, ProcessedDocumentIdentifiers(identityValue))
			End If

			Dim dataGridID As String = Nothing
			Dim dataGridIDField As Api.ArtifactField = record.FieldList(FieldType.Varchar).FirstOrDefault(Function(x) x.DisplayName = DATA_GRID_ID_FIELD_NAME)
			If (dataGridIDField IsNot Nothing) Then
				dataGridID = dataGridIDField.ValueAsString
			End If

			Dim doc As MetaDocument
			Dim fileSizeExtractor As Api.IHasFileSize = Nothing
			If (Not injectableContainerIsNothing) Then
				fileSizeExtractor = injectableContainer.FileSize
			End If
			If fileSizeExtractor Is Nothing Then
				doc = New MetaDocument(fileGuid, identityValue, fileExists AndAlso uploadFile AndAlso (fileGuid <> String.Empty OrElse Not _copyFileToRepository), filename, fullFilePath, uploadFile, CurrentLineNumber, parentFolderID, record, oixFileIdInfo, lineStatus, destinationVolume, folderPath, dataGridID)
			Else
				doc = New SizedMetaDocument(fileGuid, identityValue, fileExists AndAlso uploadFile AndAlso (fileGuid <> String.Empty OrElse Not _copyFileToRepository), filename, fullFilePath, uploadFile, CurrentLineNumber, parentFolderID, record, oixFileIdInfo, lineStatus, destinationVolume, fileSizeExtractor.GetFileSize(), folderPath, dataGridID)
			End If

			Using Timekeeper.CaptureTime("ManageDocument_ManageDocumentMetadata")
				ManageDocumentMetaData(doc)
			End Using

			Return identityValue
		End Function

		Public Shared Function CleanDestinationFolderPath(ByVal path As String) As String
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

		Public Shared Function InnerRelativityFolderPathsAreTooLarge(ByVal cleanFolderPath As String) As Boolean
			If String.IsNullOrEmpty(cleanFolderPath) Then
				Return False
			End If
			If (Not cleanFolderPath.Contains("\") AndAlso cleanFolderPath.Length <= _LENGTH_OF_FOLDER_ALLOWED) Then
				Return False
			End If
			Dim paths As String() = cleanFolderPath.Split("\"(0))
			Return paths.Any(Function(path) path.Length > _LENGTH_OF_FOLDER_ALLOWED)
		End Function

#End Region

#Region "WebService Calls"

		Public Overrides Function LookupArtifactIDForName(objectName As String, associatedObjectTypeID As Integer) As Integer
			Return If(associatedObjectTypeID = ArtifactType.Document, MyBase.LookupArtifactIDForName(objectName, associatedObjectTypeID), -1)
		End Function

		Public Overrides Function LookupNameForArtifactID(objectArtifactID As Integer, associatedObjectTypeID As Integer) As String
			Return If(associatedObjectTypeID = ArtifactType.Document, MyBase.LookupNameForArtifactID(objectArtifactID, associatedObjectTypeID), String.Empty)
		End Function

		Private Sub ManageDocumentMetaData(ByVal metaDoc As MetaDocument)
			Try
				Using Timekeeper.CaptureTime("ManageDocumentMetadata_ManageDocumentLine")
					ManageDocumentLine(metaDoc)
				End Using

				_batchCounter += 1

				Using Timekeeper.CaptureTime("ManageDocumentMetadata_WserviceCall")
					If Me.OutputFileWriter.CombinedStreamLength > ImportBatchVolume OrElse _batchCounter > ImportBatchSize - 1 Then
						Me.TryPushNativeBatch(False, _jobCompleteNativeCount >= JobCompleteBatchSize, _jobCompleteMetadataCount >= JobCompleteBatchSize)
					End If
				End Using
			Catch ex As ImporterException
				WriteError(metaDoc.LineNumber, ex.Message)
				Me.LogError(ex, "A serious import error has occurred managing document {file} metadata.", metaDoc.FullFilePath)
			Catch ex As FileInfoInvalidPathException
				WriteError(metaDoc.LineNumber, ex.Message)
				Me.LogError(ex, "An import error has occured because of invalid document path - illegal characters in path {0}", metaDoc.FullFilePath)
			Catch ex As System.UnauthorizedAccessException
				WriteFatalError(metaDoc.LineNumber, ex)
				Me.LogFatal(ex, "A fatal import error has occurred because the user doesn't have authorized access to the document {file} metadata.", metaDoc.FullFilePath)
			Catch ex As System.Exception
				WriteFatalError(metaDoc.LineNumber, ex)
				Me.LogFatal(ex, "A fatal unexpected error has occurred managing document {file} metadata.", metaDoc.FullFilePath)
			End Try

			' Let TAPI handle progress as long as we're transferring the native. See the TAPI progress event below.
			If _copyFileToRepository AndAlso metaDoc.IndexFileInDB Then
				Using Timekeeper.CaptureTime("ManageDocumentMetadata_StatusEvent")
					WriteStatusLine(EventType.Status, $"Item '{metaDoc.IdentityValue}' file '{metaDoc.FileGuid}' processed.", metaDoc.LineNumber)
				End Using
			Else
				Using Timekeeper.CaptureTime("ManageDocumentMetadata_ProgressEvent")
					FileTapiProgressCount += 1
					WriteStatusLine(EventType.Progress, $"Item '{metaDoc.IdentityValue}' file '{metaDoc.FileGuid}'  processed.", metaDoc.LineNumber)
				End Using
			End If
		End Sub

		Protected Function BulkImport(ByVal settings As kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo, ByVal includeExtractedTextEncoding As Boolean) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			If BatchSizeHistoryList.Count = 0 Then BatchSizeHistoryList.Add(ImportBatchSize)
			Dim totalTries As Int32 = NumberOfRetries
			Dim tries As Int32 = totalTries
			Dim retval As New kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			While tries > 0
				Try
					retval = BatchBulkImport(settings, includeExtractedTextEncoding)
					Exit While
				Catch ex As Exception
					tries -= 1
					If tries = 0 Then
						Me.LogFatal(ex, "The native bulk import service call failed and exceeded the max retry attempts.")
						Throw
					ElseIf IsTimeoutException(ex) Then
						' A timeout exception can be retried.
						Me.LogError(ex, "A SQL or HTTP timeout error has occurred bulk importing the native batch.")
						Throw
					ElseIf Not ShouldImport Then
						' Don't log cancel requests
						Throw
					ElseIf IsBulkImportSqlException(ex) Then
						Me.LogFatal(ex, "A fatal SQL error has occurred bulk importing the native batch.")
						Throw
					ElseIf IsInsufficientPermissionsForImportException(ex) Then
						Me.LogFatal(ex, "A fatal insufficient permissions error has occurred bulk importing the native batch.")
						Throw
					Else
						Me.LogWarning(ex, "A serious error has occurred bulk importing the native batch. Retry info: {Count} of {TotalRetry}.", totalTries - tries, totalTries)
						Me.RaiseWarningAndPause(ex, WaitTimeBetweenRetryAttempts, totalTries - tries, totalTries)
					End If
				End Try
			End While
			Return retval
		End Function

		Private Function BatchBulkImport(ByVal settings As kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo, ByVal includeExtractedTextEncoding As Boolean) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Dim retval As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			If TypeOf settings Is kCura.EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo Then
				retval = Me.BulkImportManager.BulkImportObjects(_caseInfo.ArtifactID, DirectCast(settings, kCura.EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo), _copyFileToRepository)
			Else
				retval = Me.BulkImportManager.BulkImportNative(_caseInfo.ArtifactID, settings, _copyFileToRepository, includeExtractedTextEncoding)
			End If
			Return retval
		End Function

		Protected Overridable Sub LowerBatchLimits()
			Dim oldBatchSize As Int32 = Me.ImportBatchSize
			Me.ImportBatchSize -= 100
			Me.Statistics.BatchSize = Me.ImportBatchSize
			Me.BatchSizeHistoryList.Add(Me.ImportBatchSize)
			Me.LogWarning("Lowered the native batch limits from {OldBatchSize} to {NewBatchSize}.", oldBatchSize, Me.ImportBatchSize)
		End Sub

		Private Function GetSettingsObject() As kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo
			Dim retval As kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo = Nothing
			If _artifactTypeID = ArtifactType.Document Then
				retval = New kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo With {.DisableUserSecurityCheck = Me.DisableUserSecurityCheck, .AuditLevel = Me.AuditLevel, .OverlayArtifactID = OverlayArtifactId}
				If _createFoldersInWebApi Then
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

			retval.LinkDataGridRecords = LinkDataGridRecords

			OnSettingsObjectCreate(retval)
			Return retval
		End Function

		Public Overridable Sub OnSettingsObjectCreate(settings As kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo)
			'Do Nothing
		End Sub

		Private Sub TryPushNativeBatch(ByVal lastRun As Boolean, ByVal shouldCompleteNativeJob As Boolean, ByVal shouldCompleteMetadataJob As Boolean)
			CloseFileWriters()
			Dim outputNativePath As String = Me.OutputFileWriter.OutputNativeFilePath

			If (shouldCompleteNativeJob Or lastRun) And _jobCompleteNativeCount > 0 Then
				_jobCompleteNativeCount = 0
				CompletePendingPhysicalFileTransfers("Waiting for the native file job to complete...", "Native file job completed.", "Failed to complete all pending native file transfers.")
			End If

			' REL-157042: Prevent importing bad data into Relativity or honor stoppage.
			If ShouldImport Then
				Try
					If ShouldImport AndAlso _copyFileToRepository AndAlso FileTapiBridge.TransfersPending Then
						WaitForPendingFileUploads()
						JobCounter += 1

						' The sync progress addresses an issue with TAPI clients that fail to raise progress when a failure occurs but successfully transfer all files via job retry (Aspera).
						Dim expectedProcessCount As Int32 = Me.CurrentLineNumber + Offset
						If ShouldImport AndAlso FileTapiProgressCount <> expectedProcessCount Then
							FileTapiProgressCount = expectedProcessCount
							Me.WriteTapiProgressMessage("Synchronized process count.", Me.CurrentLineNumber)
						End If
					End If

					Dim start As Int64 = System.DateTime.Now.Ticks

					If ShouldImport Then
						Me.PushNativeBatch(outputNativePath, shouldCompleteMetadataJob, lastRun)
					End If

					Me.Statistics.FileWaitTime += System.Math.Max((System.DateTime.Now.Ticks - start), 1)
				Catch ex As Exception
					If BatchResizeEnabled AndAlso IsTimeoutException(ex) AndAlso ShouldImport Then
						Me.LogWarning(ex, "A SQL or HTTP timeout error has occurred bulk importing the native batch and the batch will be resized.")
						Dim originalBatchSize As Int32 = Me.ImportBatchSize
						LowerBatchLimits()
						Me.RaiseWarningAndPause(ex, WaitTimeBetweenRetryAttempts)
						If Not ShouldImport Then Throw 'after the pause
						Me.LowerBatchSizeAndRetry(outputNativePath, originalBatchSize)
					Else
						If ShouldImport AndAlso Not BatchResizeEnabled Then
							Me.LogError("Pushing the native batch failed but lowering the batch and performing a retry is disabled.", ex)
						End If

						If ShouldImport AndAlso BatchResizeEnabled Then
							Me.LogError("Pushing the native batch failed but lowering the batch isn't supported because the error isn't timeout related.", ex)
						End If

						Throw
					End If
				End Try
			End If

			Me.DeleteTempLoadFiles()
			If Not lastRun Then
				Me.OpenFileWriters()
			End If
		End Sub

		Private Sub LowerBatchSizeAndRetry(ByVal oldNativeFilePath As String, ByVal totalRecords As Int32)
			'NOTE: we are not cutting a new/smaller data grid bulk file because it will be chunked as it is loaded into the data grid
			Dim newNativeFilePath As String = TempFileBuilder.GetTempFileName(TempFileConstants.NativeLoadFileNameSuffix)
			Dim limit As String = BulkLoadFileFieldDelimiter & vbCrLf
			Dim last As New System.Collections.Generic.Queue(Of Char)
			Dim recordsProcessed As Int32 = 0
			Dim charactersSuccessfullyProcessed As Int64 = 0
			Dim hasReachedEof As Boolean = False
			Dim tries As Int32 = 1 'already starts at 1 retry
			While totalRecords > recordsProcessed AndAlso Not hasReachedEof AndAlso ShouldImport
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
					Me.PushNativeBatch(newNativeFilePath, False, True)
					recordsProcessed += i
					charactersSuccessfullyProcessed += charactersProcessed
				Catch ex As Exception
					If tries < NumberOfRetries AndAlso BatchResizeEnabled AndAlso IsTimeoutException(ex) AndAlso ShouldImport Then
						Me.LogWarning(ex, "A serious SQL or HTTP timeout error has occurred and the native batch will be resized.")
						LowerBatchLimits()
						Me.RaiseWarningAndPause(ex, WaitTimeBetweenRetryAttempts, tries, NumberOfRetries)
						If Not ShouldImport Then Throw 'after the pause
						tries += 1
						hasReachedEof = False
					Else
						Global.Relativity.Import.Export.Io.FileSystem.Instance.File.Delete(newNativeFilePath)
						Throw
					End If
				End Try
			End While
			Global.Relativity.Import.Export.Io.FileSystem.Instance.File.Delete(newNativeFilePath)
		End Sub

		Private Sub AdvanceStream(ByVal sr As System.IO.StreamReader, ByVal count As Int64)
			Dim i As Int32
			If count > 0 Then
				For j As Int64 = 0 To count - 1
					i = sr.Read()
				Next
			End If
		End Sub

		Private Sub PushNativeBatch(ByVal outputNativePath As String, ByVal shouldCompleteJob As Boolean, ByVal lastRun As Boolean)
			If _lastRunMetadataImport > 0 Then
				Me.Statistics.MetadataWaitTime += System.DateTime.Now.Ticks - _lastRunMetadataImport
			End If

			If _batchCounter = 0 OrElse Not ShouldImport Then
				If _jobCompleteMetadataCount > 0 Then
					_jobCompleteMetadataCount = 0
					CompletePendingBulkLoadFileTransfers()
				End If
				Exit Sub
			End If
			_batchCounter = 0

			If shouldCompleteJob And _jobCompleteMetadataCount > 0 Then
				_jobCompleteMetadataCount = 0
				CompletePendingBulkLoadFileTransfers()
			End If

			Dim settings As kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo = Me.GetSettingsObject
			settings.UseBulkDataImport = True
			Dim nativeFileUploadKey As String
			Dim codeFileUploadKey As String
			Dim objectFileUploadKey As String
			Dim dataGridFileUploadKey As String

			Try
				nativeFileUploadKey = BulkLoadTapiBridge.AddPath(outputNativePath, Guid.NewGuid().ToString(), 1)
				codeFileUploadKey = BulkLoadTapiBridge.AddPath(Me.OutputFileWriter.OutputCodeFilePath, Guid.NewGuid().ToString(), 2)
				objectFileUploadKey = BulkLoadTapiBridge.AddPath(Me.OutputFileWriter.OutputObjectFilePath, Guid.NewGuid().ToString(), 3)
				dataGridFileUploadKey = BulkLoadTapiBridge.AddPath(Me.OutputFileWriter.OutputDataGridFilePath, Guid.NewGuid().ToString(), 4)

				' keep track of the total count of added files
				MetadataFilesCount += 4
				_jobCompleteMetadataCount += 4

				If lastRun Then
					CompletePendingBulkLoadFileTransfers()
				Else
					WaitForPendingMetadataUploads()
				End If
			Catch ex As Exception
				' Note: Retry and potential HTTP fallback automatically kick in. Throwing a similar exception if a failure occurs.
				Throw New BcpPathAccessException("Error accessing BCP Path, could be caused by network connectivity issues: " & ex.Message)
			End Try

			_lastRunMetadataImport = System.DateTime.Now.Ticks

			' Account for possible cancellation during the BCP transfers.
			If Not ShouldImport Then
				Return
			End If

			If _artifactTypeID = ArtifactType.Document Then
				settings.Repository = _defaultDestinationFolderPath
				If settings.Repository = String.Empty Then settings.Repository = _caseInfo.DocumentPath
			Else
				settings.Repository = _caseInfo.DocumentPath
			End If

			settings.RunID = RunId
			settings.CodeFileName = codeFileUploadKey
			settings.DataFileName = nativeFileUploadKey
			settings.ObjectFileName = objectFileUploadKey
			settings.DataGridFileName = dataGridFileUploadKey
			settings.MappedFields = Me.GetMappedFields(_artifactTypeID, _settings.ObjectFieldIdListContainsArtifactId)
			settings.KeyFieldArtifactID = _keyFieldID
			settings.BulkLoadFileFieldDelimiter = BulkLoadFileFieldDelimiter
			settings.OverlayBehavior = Me.GetMassImportOverlayBehavior(_settings.OverlayBehavior)
			settings.MoveDocumentsInAppendOverlayMode = _settings.MoveDocumentsInAppendOverlayMode
			Select Case Overwrite
				Case ImportOverwriteType.Overlay
					settings.Overlay = EDDS.WebAPI.BulkImportManagerBase.OverwriteType.Overlay
				Case ImportOverwriteType.AppendOverlay
					settings.Overlay = EDDS.WebAPI.BulkImportManagerBase.OverwriteType.Both
				Case Else
					settings.Overlay = EDDS.WebAPI.BulkImportManagerBase.OverwriteType.Append
			End Select
			settings.UploadFiles = _filePathColumnIndex <> -1 AndAlso _settings.LoadNativeFiles
			settings.LoadImportedFullTextFromServer = Me.LoadImportedFullTextFromServer
			settings.ExecutionSource = CType(_executionSource, kCura.EDDS.WebAPI.BulkImportManagerBase.ExecutionSource)
			settings.Billable = _settings.Billable
			If _usePipeliningForNativeAndObjectImports AndAlso Not _task Is Nothing Then
				WaitOnPushBatchTask()
				_task = Nothing
			End If
			Dim makeServiceCalls As Action =
					Sub()
						Dim start As Int64 = DateTime.Now.Ticks
						Dim runResults As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults = Me.BulkImport(settings, _fullTextColumnMapsToFileLocation)

						Statistics.ProcessRunResults(runResults)
						Statistics.SqlTime += (DateTime.Now.Ticks - start)

						UpdateStatisticsSnapshot(DateTime.Now)
						Me.ManageErrors(_artifactTypeID)
					End Sub
			If _usePipeliningForNativeAndObjectImports Then
				Dim f As New System.Threading.Tasks.TaskFactory()
				_task = f.StartNew(makeServiceCalls)
			Else
				makeServiceCalls()
			End If

			Me.TotalTransferredFilesCount = Me.FileTapiProgressCount
		End Sub

		Private Sub WaitOnPushBatchTask()
			If _task Is Nothing Then Return
			Try
				Task.WaitAll(_task)
			Catch ex As AggregateException
				Me.LogFatal(ex, "A fatal error occurred while waiting on the batch task")

				ex.Handle(Function(e)
							  Throw e
						  End Function)
			End Try
		End Sub

		Private _task As System.Threading.Tasks.Task = Nothing
		Private _isRunOccurring As Boolean = False
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

		Protected Sub OpenFileWriters()
			If Me.OutputFileWriter Is Nothing Then
				Me.OutputFileWriter = New OutputFileWriter(Me.FileSystem)
			End If

			Me.OutputFileWriter.Open()
		End Sub

		Protected Sub CloseFileWriters()
			If Not Me.OutputFileWriter Is Nothing Then
				Me.OutputFileWriter.Close()
			End If
		End Sub

		Public Function GetMappedFields(ByVal artifactTypeId As Int32, ByVal ObjectFieldIdListContainsArtifactId As IList(Of Int32)) As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo()
			Dim retval As New System.Collections.ArrayList
			For Each item As WinEDDS.LoadFileFieldMap.LoadFileFieldMapItem In _fieldMap
				If Not item.DocumentField Is Nothing Then
					Dim i As Integer = retval.Add(item.DocumentField.ToFieldInfo)
					If Not ObjectFieldIdListContainsArtifactId Is Nothing Then
						If (CType(retval(i), kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo).Type = kCura.EDDS.WebAPI.BulkImportManagerBase.FieldType.Object _
							Or CType(retval(i), kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo).Type = kCura.EDDS.WebAPI.BulkImportManagerBase.FieldType.Objects) _
						   AndAlso ObjectFieldIdListContainsArtifactId.Contains(CType(retval(i), kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo).ArtifactID) Then
							CType(retval(i), kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo).ImportBehavior = kCura.EDDS.WebAPI.BulkImportManagerBase.ImportBehaviorChoice.ObjectFieldContainsArtifactId
						End If
					End If
				End If
			Next
			retval.Sort(New WebServiceFieldInfoNameComparer)
			If artifactTypeId = ArtifactType.Document Then
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

		Protected Sub ManageDocumentLine(ByVal mdoc As MetaDocument)
			Dim chosenEncoding As System.Text.Encoding = Nothing

			OutputFileWriter.MarkRollbackPosition()
			OutputFileWriter.OutputNativeFileWriter.Write("0" & BulkLoadFileFieldDelimiter) 'kCura_Import_ID
			OutputFileWriter.OutputNativeFileWriter.Write(mdoc.LineStatus.ToString & BulkLoadFileFieldDelimiter)   'kCura_Import_Status
			OutputFileWriter.OutputNativeFileWriter.Write("0" & BulkLoadFileFieldDelimiter) 'kCura_Import_IsNew
			OutputFileWriter.OutputNativeFileWriter.Write("0" & BulkLoadFileFieldDelimiter) 'ArtifactID
			OutputFileWriter.OutputNativeFileWriter.Write(mdoc.LineNumber & BulkLoadFileFieldDelimiter) 'kCura_Import_OriginalLineNumber

			If mdoc.UploadFile And mdoc.IndexFileInDB Then
				OutputFileWriter.OutputNativeFileWriter.Write(mdoc.FileGuid & BulkLoadFileFieldDelimiter)  'kCura_Import_FileGuid
				OutputFileWriter.OutputNativeFileWriter.Write(mdoc.Filename & BulkLoadFileFieldDelimiter)  'kCura_Import_FileName
				If _settings.CopyFilesToDocumentRepository Then
					OutputFileWriter.OutputNativeFileWriter.Write(_defaultDestinationFolderPath & mdoc.DestinationVolume & "\" & mdoc.FileGuid & BulkLoadFileFieldDelimiter)  'kCura_Import_Location
					OutputFileWriter.OutputNativeFileWriter.Write(mdoc.FullFilePath & BulkLoadFileFieldDelimiter) 'kCura_Import_OriginalFileLocation
				Else
					OutputFileWriter.OutputNativeFileWriter.Write(mdoc.FullFilePath & BulkLoadFileFieldDelimiter) 'kCura_Import_Location
					OutputFileWriter.OutputNativeFileWriter.Write(mdoc.FullFilePath & BulkLoadFileFieldDelimiter) 'kCura_Import_OriginalFileLocation
				End If
				Dim fileSizeExtractor As Api.IHasFileSize = TryCast(mdoc, Api.IHasFileSize)
				If (fileSizeExtractor Is Nothing) Then
					Const retry As Boolean = True
					If Me.GetFileExists(mdoc.FullFilePath, retry) Then
						OutputFileWriter.OutputNativeFileWriter.Write(Me.GetFileLength(mdoc.FullFilePath, retry) & BulkLoadFileFieldDelimiter) 'kCura_Import_FileSize
					Else
						OutputFileWriter.OutputNativeFileWriter.Write(0 & BulkLoadFileFieldDelimiter)
					End If
				Else
					OutputFileWriter.OutputNativeFileWriter.Write(fileSizeExtractor.GetFileSize() & BulkLoadFileFieldDelimiter) 'kCura_Import_FileSize
				End If

			Else
				OutputFileWriter.OutputNativeFileWriter.Write(BulkLoadFileFieldDelimiter)   'kCura_Import_FileGuid
				OutputFileWriter.OutputNativeFileWriter.Write(BulkLoadFileFieldDelimiter)   'kCura_Import_FileName
				OutputFileWriter.OutputNativeFileWriter.Write(BulkLoadFileFieldDelimiter)   'kCura_Import_Location
				OutputFileWriter.OutputNativeFileWriter.Write(BulkLoadFileFieldDelimiter)   'kCura_Import_OriginalFileLocation
				OutputFileWriter.OutputNativeFileWriter.Write("0" & BulkLoadFileFieldDelimiter) 'kCura_Import_FileSize
			End If
			OutputFileWriter.OutputNativeFileWriter.Write(mdoc.ParentFolderID & BulkLoadFileFieldDelimiter) 'kCura_Import_ParentFolderID

			Dim foundDataGridField As Boolean = False

			' If we're mapping, always write out to the data grid file writer
			If (Not String.IsNullOrEmpty(mdoc.DataGridID)) Then
				OutputFileWriter.OutputDataGridFileWriter.Write(mdoc.IdentityValue & BulkLoadFileFieldDelimiter & mdoc.DataGridID & BulkLoadFileFieldDelimiter)
				foundDataGridField = True
			End If

			For Each field As Api.ArtifactField In mdoc.Record
				Try
					Select Case field.EnableDataGrid

						Case False
							WriteDocumentField(chosenEncoding, field, OutputFileWriter.OutputNativeFileWriter, _fullTextColumnMapsToFileLocation, BulkLoadFileFieldDelimiter, _artifactTypeID, _extractedTextFileEncoding)

						Case True
							If Not foundDataGridField Then
								'write the data grid identity field as the first column and an empty data grid id as the second column, but only when we have data grid fields
								OutputFileWriter.OutputDataGridFileWriter.Write(mdoc.IdentityValue & BulkLoadFileFieldDelimiter & String.Empty & BulkLoadFileFieldDelimiter)
								foundDataGridField = True
							End If

							'TODO: do we want to set/update the "chosenEncoding" property if the extracted text is going to the data grid?
							WriteDocumentField(chosenEncoding, field, OutputFileWriter.OutputDataGridFileWriter, _fullTextColumnMapsToFileLocation, BulkLoadFileFieldDelimiter, _artifactTypeID, _extractedTextFileEncoding)

					End Select
				Catch ex As ExtractedTextFileNotFoundException
					OutputFileWriter.RollbackDocumentLineWrites()
					Throw
				Catch ex As ExtractedTextTooLargeException
					OutputFileWriter.RollbackDocumentLineWrites()
					Throw
				Catch ex As kCura.WinEDDS.Exceptions.ImportIOException
					OutputFileWriter.RollbackDocumentLineWrites()
					Throw
				End Try
			Next

			If _artifactTypeID = ArtifactType.Document Then
				WriteDocumentNativeInfo(mdoc)
			End If

			'_fullTextColumnMapsToFileLocation and chosenEncoding indicate that extracted text is being mapped
			'In that case, we need a field (it can be empty but it needs the field delimiter) for the extracted text encoding
			If chosenEncoding IsNot Nothing Then
				OutputFileWriter.OutputNativeFileWriter.Write(chosenEncoding.CodePage.ToString() & BulkLoadFileFieldDelimiter)
			ElseIf _fullTextColumnMapsToFileLocation Then
				OutputFileWriter.OutputNativeFileWriter.Write(BulkLoadFileFieldDelimiter)
			End If

			If _createFoldersInWebApi Then
				'Server side folder creation

				'For documents only, we need a field to contain the folder path, so that WebAPI can create the folder (if needed)
				'If we are using client-side folder creation, the folder path will be an empty string, but we still need to
				'add it so that the number of fields set by the client equals the number of fields that the server expects to find.
				'If we are using client-side folder creation, this folder path field will not be used by the server because of the
				'settings object -- NativeFileInfo.RootFolderID will be 0.
				If _artifactTypeID = ArtifactType.Document Then
					'Last column is the folder path (ONLY IF THIS IS A DOCUMENT LOAD... adding this to object imports will break them)
					OutputFileWriter.OutputNativeFileWriter.Write(mdoc.FolderPath & BulkLoadFileFieldDelimiter)
				End If
			End If
			OutputFileWriter.OutputNativeFileWriter.Write(BulkLoadFileFieldDelimiter)   'kCura_DataGrid_Exception
			OutputFileWriter.OutputNativeFileWriter.Write(BulkLoadFileFieldDelimiter)   'kCura_Import_ErrorData
			OutputFileWriter.OutputNativeFileWriter.Write(vbCrLf)
			If foundDataGridField Then
				OutputFileWriter.OutputDataGridFileWriter.Write(vbCrLf)
			End If
		End Sub

		Private Sub WriteDocumentNativeInfo(mdoc As MetaDocument)
			If _filePathColumnIndex <> -1 AndAlso mdoc.UploadFile AndAlso mdoc.IndexFileInDB Then
				Dim supportedByViewerProvider As IHasSupportedByViewer = TryCast(mdoc.FileIdInfo, IHasSupportedByViewer)

				If supportedByViewerProvider Is Nothing
					WriteDocumentNativeInfo(Me.IsSupportedRelativityFileType(mdoc.FileIdInfo), mdoc.GetFileType(), True)
				Else
					WriteDocumentNativeInfo(supportedByViewerProvider.SupportedByViewer(), mdoc.GetFileType(), True)
				End If
			Else
				WriteDocumentNativeInfo(False, String.Empty, False)
			End If
		End Sub

		Private Sub WriteDocumentNativeInfo(supportedByViewer As Boolean, relativityNativeType As String, hasNative As Boolean)
			Dim supportedByViewerAsString As String = ConvertToString(supportedByViewer)
			Dim hasNativeAsString As String = ConvertToString(hasNative)

			OutputFileWriter.OutputNativeFileWriter.Write(supportedByViewerAsString & BulkLoadFileFieldDelimiter)   'SupportedByViewer
			OutputFileWriter.OutputNativeFileWriter.Write(relativityNativeType & BulkLoadFileFieldDelimiter)        'RelativityNativeType
			OutputFileWriter.OutputNativeFileWriter.Write(hasNativeAsString & BulkLoadFileFieldDelimiter)           'HasNative
		End Sub

		Private Function ConvertToString(booleanValue As Boolean) As String
			Return If(booleanValue, "1", "0")
		End Function


		Private Sub WriteDocumentField(ByRef chosenEncoding As System.Text.Encoding, field As Api.ArtifactField, ByVal outputWriter As Global.Relativity.Import.Export.Io.IStreamWriter, ByVal fileBasedfullTextColumn As Boolean, ByVal delimiter As String, ByVal artifactTypeID As Int32, ByVal extractedTextEncoding As System.Text.Encoding)
			If field.Type = FieldType.MultiCode OrElse field.Type = FieldType.Code Then
				outputWriter.Write(field.Value)
				outputWriter.Write(delimiter)
			ElseIf field.Type = FieldType.File AndAlso artifactTypeID <> ArtifactType.Document Then
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
			ElseIf field.Type = FieldType.File AndAlso artifactTypeID = ArtifactType.Document Then
				'do nothing
			ElseIf field.Category = FieldCategory.ParentArtifact Then
				'do nothing
			ElseIf field.ArtifactID <= 0 Then
				' do nothing, this is a catch-all for all "virtual fields" that are added to pass information
				' from the load file to the file writers that shouldn't be imported as actual object field values
			Else
				Dim fieldShouldReadFromTextFile As Boolean = FieldValueContainsTextFileLocation(field)
				If fieldShouldReadFromTextFile Then

					Try
						If Not field.ValueAsString = String.Empty Then
							' Prevent nested retry operations.
							Const retry As Boolean = False
							Dim maxRetryAttempts As Integer = Me.NumberOfRetries
							Dim currentRetryAttempt As Integer = 0

							' REL-272765: Added I/O resiliency and support document level errors.
							Dim policy As IWaitAndRetryPolicy = Me.CreateWaitAndRetryPolicy()
							' Note: a lambda can't modify a ref param; therefore, a policy block return value is used.
							Dim returnEncoding As System.Text.Encoding = policy.WaitAndRetry(
								RetryExceptionHelper.CreateRetryPredicate(Me.RetryOptions),
								Function(count)
									currentRetryAttempt = count
									Return TimeSpan.FromSeconds(Me.WaitTimeBetweenRetryAttempts)
								End Function,
								Sub(exception, span)
									Me.PublishIoRetryMessage(exception, span, currentRetryAttempt, maxRetryAttempts)
								End Sub,
								Function(token) As System.Text.Encoding
									Dim encoding As System.Text.Encoding = extractedTextEncoding
									Dim fileStream As System.IO.Stream
									Dim fileSize As Long = Me.GetFileLength(field.ValueAsString, retry)
									If fileSize > GetMaxExtractedTextLength(encoding) Then
										Throw New ExtractedTextTooLargeException
									End If

									If Me.LoadImportedFullTextFromServer Then
										If Not SkipExtractedTextEncodingCheck Then
											Dim determinedEncodingStream As DeterminedEncodingStream = Utility.DetectEncoding(field.ValueAsString, False)
											fileStream = determinedEncodingStream.UnderlyingStream

											Dim textField As kCura.EDDS.WebAPI.DocumentManagerBase.Field = Me.FullTextField(_settings.ArtifactTypeID)
											Dim expectedEncoding As System.Text.Encoding = If(textField IsNot Nothing AndAlso textField.UseUnicodeEncoding, System.Text.Encoding.Unicode, Nothing)
											Dim detectedEncoding As System.Text.Encoding = determinedEncodingStream.DeterminedEncoding
											If Not System.Text.Encoding.Equals(expectedEncoding, detectedEncoding) Then
												WriteWarning("The extracted text file's encoding was not detected to be the same as the extracted text field. The imported data may be incorrectly encoded.")
											End If
											If detectedEncoding IsNot Nothing Then
												encoding = detectedEncoding
											End If
											Try
												fileStream.Close()
											Catch
											End Try
										End If
										outputWriter.Write(field.Value)
									Else
										'This logic exists as an attempt to improve import speeds.  The DetectEncoding call first checks if the file
										' exists, followed by a read of the first few bytes. The File.Exists check can be very expensive when going
										' across the network for the file, so this override allows that check to be skipped.
										' -Phil S. 07/27/2012
										If Not SkipExtractedTextEncodingCheck Then
											Dim determinedEncodingStream As DeterminedEncodingStream = Utility.DetectEncoding(field.ValueAsString, False)
											fileStream = determinedEncodingStream.UnderlyingStream

											Dim detectedEncoding As System.Text.Encoding = determinedEncodingStream.DeterminedEncoding
											If detectedEncoding IsNot Nothing Then
												encoding = detectedEncoding
											End If
										Else
											fileStream = New System.IO.FileStream(field.ValueAsString, System.IO.FileMode.Open, System.IO.FileAccess.Read)
										End If

										Dim sr As New System.IO.StreamReader(fileStream, encoding)
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
									Return encoding
								End Function,
								Me.CancellationToken)
							' Only update the ref parameter when a non-null value is returned.
							If Not returnEncoding Is Nothing Then
								chosenEncoding = returnEncoding
							End If
						End If
					Catch ex As System.IO.FileNotFoundException
						Throw New ExtractedTextFileNotFoundException()
					Catch ex As System.IO.IOException
						' Running out of disk space should always be treated as a fatal exception.
						If ExceptionHelper.IsOutOfDiskSpaceException(ex) Then
							Throw
						End If

						Dim message As String = $"An I/O error occurred reading the file associated with the '{field.DisplayName}' full text field."
						Throw New kCura.WinEDDS.Exceptions.ImportIOException(message, ex)
					End Try
				ElseIf field.Type = FieldType.Boolean Then
					If field.ValueAsString <> String.Empty Then
						If Boolean.Parse(field.ValueAsString) Then
							outputWriter.Write("1")
						Else
							outputWriter.Write("0")
						End If
					End If
				ElseIf field.Type = FieldType.Decimal OrElse
					   field.Type = FieldType.Currency Then
					If field.ValueAsString <> String.Empty Then
						Dim d As String = CDec(field.Value).ToString(System.Globalization.CultureInfo.InvariantCulture)
						outputWriter.Write(d)
					End If
				ElseIf field.Type = FieldType.Text OrElse
					   field.Type = FieldType.OffTableText Then
					If TypeOf field.Value Is System.IO.Stream Then
						Dim stream As System.IO.Stream = CType(field.Value, System.IO.Stream)
						outputWriter.Flush()
						stream.CopyTo(outputWriter.BaseStream)
					Else
						outputWriter.Write(field.Value)
					End If
				Else
					outputWriter.Write(field.Value)
				End If
				outputWriter.Write(delimiter)
			End If
		End Sub

		Private Function GetIsSupportedRelativityFileTypeField() As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo
			For Each field As kCura.EDDS.WebAPI.DocumentManagerBase.Field In AllFields(_artifactTypeID)
				If field.DisplayName.ToLower = "supported by viewer" Then
					Return Me.FieldDtoToFieldInfo(field)
				End If
			Next
			Return Nothing
		End Function

		Private Function GetRelativityFileTypeField() As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo
			For Each field As kCura.EDDS.WebAPI.DocumentManagerBase.Field In AllFields(_artifactTypeID)
				If field.DisplayName.ToLower = "relativity native type" Then
					Return Me.FieldDtoToFieldInfo(field)
				End If
			Next
			Return Nothing
		End Function

		Private Function GetHasNativesField() As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo
			For Each field As kCura.EDDS.WebAPI.DocumentManagerBase.Field In AllFields(_artifactTypeID)
				If field.DisplayName.ToLower = "has native" Then
					Return Me.FieldDtoToFieldInfo(field)
				End If
			Next
			Return Nothing
		End Function

		Private Function GetObjectFileField() As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo
			For Each field As kCura.EDDS.WebAPI.DocumentManagerBase.Field In AllFields(_artifactTypeID)
				If field.FieldTypeID = FieldType.File Then
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
			retval.EnableDataGrid = input.EnableDataGrid
			Return retval
		End Function

		Private Function ConvertFieldTypeEnum(ByVal fieldtypeID As Int32) As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldType
			Dim ft As FieldType = CType(fieldtypeID, FieldType)
			Return CType(System.Enum.Parse(GetType(kCura.EDDS.WebAPI.BulkImportManagerBase.FieldType), ft.ToString), kCura.EDDS.WebAPI.BulkImportManagerBase.FieldType)
		End Function


		Private Function IsSupportedRelativityFileType(ByVal fileData As FileIdInfo) As Boolean
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
			Return Not _oixFileLookup.Contains(fileData.Id)
		End Function

#End Region

#Region "Field Preparation"

		Protected Function PrepareFieldCollectionAndExtractIdentityValue(ByVal record As Api.ArtifactFieldCollection) As String
			SyncLock OutputFileWriter.OutputNativeFileWriter
			SyncLock OutputFileWriter.OutputCodeFileWriter
			SyncLock OutputFileWriter.OutputObjectFileWriter
			SyncLock OutputFileWriter.OutputDataGridFileWriter
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
				If Not ProcessedDocumentIdentifiers(identityValue) Is Nothing Then Throw New IdentifierOverlapException(identityValue, ProcessedDocumentIdentifiers(identityValue))
				For Each item In _fieldMap
					If FirstTimeThrough Then
						If item.DocumentField Is Nothing Then
							WriteStatusLine(EventType.Warning, String.Format("File column '{0}' will be unmapped", item.NativeFileColumnIndex + 1), 0)
						End If
						If item.NativeFileColumnIndex = -1 Then
							WriteStatusLine(EventType.Warning, String.Format("Field '{0}' will be unmapped", item.DocumentField.FieldName), 0)
						End If
					End If
					If Not item.DocumentField Is Nothing Then
						If item.DocumentField.FieldTypeID = FieldType.File Then
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
				FirstTimeThrough = False
				Return identityValue
			End SyncLock
			End SyncLock
			End SyncLock
			End SyncLock
		End Function

		Private Sub ManageFileField(ByVal fileField As Api.ArtifactField)
			If fileField Is Nothing Then Exit Sub
			If fileField.Value Is Nothing Then Exit Sub
			If fileField.Value.ToString = String.Empty Then Exit Sub
			Dim localFilePath As String = fileField.Value.ToString
			Dim fileSize As Int64
			Const retry As Boolean = True
			If Me.GetFileExists(localFilePath, retry) Then
				fileSize = Me.GetFileLength(localFilePath, retry)
				Dim fileName As String = System.IO.Path.GetFileName(localFilePath).Replace(ChrW(11), "_")
				Dim location As String
				If FileTapiBridge.TargetFolderName = "" Then
					location = localFilePath
				Else
					Dim guid As String = FileTapiBridge.AddPath(localFilePath, System.Guid.NewGuid().ToString(), Me.CurrentLineNumber)
					location = FileTapiBridge.TargetPath & FileTapiBridge.TargetFolderName & "\" & guid
				End If
				location = System.Web.HttpUtility.UrlEncode(location)
				fileField.Value = String.Format("{1}{0}{2}{0}{3}", ChrW(11), fileName, fileSize, location)
			Else
				Throw New System.IO.FileNotFoundException(String.Format("File '{0}' not found.", localFilePath))
			End If
		End Sub

#End Region


#Region "Status Window"

		Private Sub WriteTapiProgressMessage(ByVal message As String, ByVal lineNumber As Int32)
			message = GetLineMessage(message, lineNumber)
			Dim lineProgress As Int32 = FileTapiProgressCount
			OnStatusMessage(New StatusEventArgs(EventType.Progress, lineProgress, RecordCount, message, CurrentStatisticsSnapshot, Statistics))
		End Sub

		Protected Sub WriteStatusLine(ByVal et As EventType, ByVal line As String)
			WriteStatusLine(et, line, Me.CurrentLineNumber)
		End Sub

		Private Sub WriteStatusLine(ByVal et As EventType, ByVal line As String, ByVal lineNumber As Int32)
			' Avoid displaying potential negative numbers.
			Dim recordNumber As Int32 = lineNumber
			If recordNumber <> TapiConstants.NoLineNumber Then
				recordNumber = recordNumber + Offset
			End If

			' Prevent unnecessary crashes due to to ArgumentException (IE progress).
			If recordNumber < 0 Then
				recordNumber = 0
			End If

			line = GetLineMessage(line, lineNumber)
			OnStatusMessage(New StatusEventArgs(et, recordNumber, RecordCount, line, CurrentStatisticsSnapshot, Statistics))
		End Sub

		''' <summary>
		''' Writes a fatal error and stops the import.
		''' </summary>
		''' <param name="lineNumber">
		''' The line number.
		''' </param>
		''' <param name="exception">
		''' The fatal exception.
		''' </param>
		Private Sub WriteFatalError(ByVal lineNumber As Int32, ByVal exception As System.Exception)
			_artifactReader.OnFatalErrorState()
			StopImport()
			OnFatalError($"Error processing line:{lineNumber.ToString}", exception, RunId)
		End Sub

		Private Sub WriteError(ByVal currentLineNumber As Int32, ByVal line As String)
			If _prePushErrorLineNumbersFileName = "" Then
				_prePushErrorLineNumbersFileName = TempFileBuilder.GetTempFileName(TempFileConstants.ErrorsFileNameSuffix)
			End If

			Dim sw As New System.IO.StreamWriter(_prePushErrorLineNumbersFileName, True, System.Text.Encoding.Default)
			sw.WriteLine(currentLineNumber)
			sw.Flush()
			sw.Close()
			Dim ht As New System.Collections.Hashtable
			ht.Add("Message", line)
			ht.Add("Line Number", currentLineNumber)
			ht.Add("Identifier", _artifactReader.SourceIdentifierValue)
			RaiseReportError(ht, currentLineNumber, _artifactReader.SourceIdentifierValue, "client")
			WriteStatusLine(EventType.Error, line)
		End Sub

		Private Sub RaiseReportError(ByVal row As System.Collections.Hashtable, ByVal lineNumber As Int32, ByVal identifier As String, ByVal type As String)
			_errorCount += 1
			If String.IsNullOrEmpty(_errorMessageFileLocation) Then
				_errorMessageFileLocation = TempFileBuilder.GetTempFileName(TempFileConstants.ErrorsFileNameSuffix)
			End If

			Dim errorMessageFileWriter As New System.IO.StreamWriter(_errorMessageFileLocation, True, System.Text.Encoding.Default)
			If _errorCount < MaxNumberOfErrorsInGrid Then
				OnReportErrorEvent(row)
			ElseIf _errorCount = MaxNumberOfErrorsInGrid Then
				Dim moretobefoundMessage As New System.Collections.Hashtable
				moretobefoundMessage.Add("Message", "Maximum number of errors for display reached.  Export errors to view full list.")
				OnReportErrorEvent(moretobefoundMessage)
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

		Protected Sub WriteWarning(ByVal line As String)
			WriteStatusLine(EventType.Warning, line)
		End Sub

		Private Sub WriteEndImport(ByVal line As String)
			' When a fatal error occurs or the user stops, provide an accurate count due to async nature of TAPI.
			If ShouldImport Then
				WriteStatusLine(EventType.End, line)
			ElseIf CancellationToken.IsCancellationRequested Then
				WriteStatusLine(EventType.Status, "Job has been finalized.", TapiConstants.NoLineNumber)
			Else
				WriteStatusLine(EventType.End, line, FileTapiProgressCount)
			End If
		End Sub

		Protected Overrides Sub OnWriteStatusMessage(ByVal eventType As EventType, ByVal message As String, ByVal progressLineNumber As Int32, ByVal physicalLineNumber As Int32)
			Select Case eventType
				Case EventType.Error
					WriteError(progressLineNumber, message)
				Case EventType.Warning, EventType.Status, EventType.Progress, EventType.Statistics
					WriteStatusLine(eventType, message, progressLineNumber)
			End Select
		End Sub

		Protected Overrides Sub OnWriteFatalError(ByVal exception As Exception)
			WriteFatalError(CurrentLineNumber, exception)
			MyBase.OnWriteFatalError(exception)
		End Sub
		Private Sub LegacyUploader_UploadStatusEvent(ByVal s As String)
			WriteStatusLine(EventType.Status, s)
		End Sub
#End Region

#Region "Public Events"

		Public Event FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception, ByVal runID As String)
		Public Event StatusMessage(ByVal args As StatusEventArgs)
		Public Event EndFileImport(ByVal runID As String)
		Public Event StartFileImport()

		Public Event ReportErrorEvent(ByVal row As System.Collections.IDictionary)
		Public Event DataSourcePrepEvent(ByVal e As Api.DataSourcePrepEventArgs)
		Public Event FieldMapped(ByVal sourceField As String, ByVal workspaceField As String)

#End Region

#Region "Event Handlers"

		Protected Overrides Sub OnTapiClientChanged()
			PublishUploadModeEvent()
			MyBase.OnTapiClientChanged()
		End Sub

		Protected Overridable Sub _processContext_HaltProcessEvent(ByVal sender As Object, ByVal e As CancellationRequestEventArgs) Handles Context.CancellationRequest
			If e.ProcessId.ToString = _processId.ToString Then
				StopImport()
			End If
		End Sub

		Protected Overrides Sub OnStopImport()
			If Not _artifactReader Is Nothing Then
				_artifactReader.Halt()
			End If
			WriteStatusLine(EventType.Progress, $"Job has been stopped - {Me.TotalTransferredFilesCount} documents have been transferred.", CType(Me.TotalTransferredFilesCount + 1, Integer))
			WriteStatusLine(EventType.Status, "Finalizing job...", TapiConstants.NoLineNumber)
		End Sub

		Protected Overridable Sub _processContext_ExportServerErrors(ByVal sender As Object, e As ExportErrorEventArgs) Handles Context.ExportServerErrors
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
			Dim rootFilePath As String = e.Path & rootFileName
			Dim datetimeNow As System.DateTime = System.DateTime.Now
			Dim errorFilePath As String = rootFilePath & "_ErrorLines_" & datetimeNow.Ticks & defaultExtension
			Dim errorReportPath As String = rootFilePath & "_ErrorReport_" & datetimeNow.Ticks & ".csv"
			Const retry As Boolean = True
			If Not _errorLinesFileLocation Is Nothing AndAlso Not _errorLinesFileLocation = String.Empty AndAlso Me.GetFileExists(_errorLinesFileLocation, retry) Then
				Me.CopyFile(_errorLinesFileLocation, errorFilePath, retry)
			End If

			Me.CopyFile(_errorMessageFileLocation, errorReportPath, retry)
			_errorMessageFileLocation = ""
		End Sub

		Private Sub _processContext_ExportErrorReportEvent(ByVal sender As Object, e As ExportErrorEventArgs) Handles Context.ExportErrorReport
			If String.IsNullOrEmpty(_errorMessageFileLocation) Then
				' write out a blank file if there is no error message file
				Dim fileWriter As System.IO.StreamWriter = System.IO.File.CreateText(e.Path)
				fileWriter.Close()

				Exit Sub
			End If

			Const retry As Boolean = True
			Me.CopyFile(_errorMessageFileLocation, e.Path, True, retry)
		End Sub

		Private Sub _processContext_ExportErrorFileEvent(ByVal sender As Object, e As ExportErrorEventArgs) Handles Context.ExportErrorFile
			Const retry As Boolean = True
			If _errorMessageFileLocation Is Nothing OrElse _errorMessageFileLocation = "" Then Exit Sub
			If _errorLinesFileLocation Is Nothing OrElse _errorLinesFileLocation = "" OrElse Not Me.GetFileExists(_errorLinesFileLocation, retry) Then
				_errorLinesFileLocation = _artifactReader.ManageErrorRecords(_errorMessageFileLocation, _prePushErrorLineNumbersFileName)
			End If
			If _errorLinesFileLocation Is Nothing Then
				Exit Sub
			End If

			Me.CopyFile(_errorLinesFileLocation, e.Path, True, retry)
		End Sub

#End Region

#Region "Exceptions"

		''' <summary>
		''' The exception thrown when the identity field value is null or empty.
		''' </summary>
		<Serializable>
		Public Class IdentityValueNotSetException
			Inherits ImporterException

			''' <summary>
			''' Initializes a new instance of the <see cref="IdentityValueNotSetException"/> class.
			''' </summary>
			Public Sub New()
				MyBase.New("Identity value not set")
			End Sub

			''' <inheritdoc />
			<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
			Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
				MyBase.New(info, context)
			End Sub
		End Class

		''' <summary>
		''' The exception thrown when the extracted text file does not exist.
		''' </summary>
		<Serializable>
		Public Class ExtractedTextFileNotFoundException
			Inherits ImporterException

			''' <summary>
			''' Initializes a new instance of the <see cref="ExtractedTextFileNotFoundException"/> class.
			''' </summary>
			Public Sub New()
				MyBase.New("Error occurred when importing the document. Extracted text is missing.")
			End Sub

			''' <inheritdoc />
			<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
			Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
				MyBase.New(info, context)
			End Sub
		End Class

		''' <summary>
		''' The exception thrown when the extracted text file length exceeds the max extracted text length.
		''' When the encoding is not specified or is <see cref="System.Text.Encoding.UTF8"/>, the max length is 1GB;
		''' otherwise, the max length is <see cref="System.Int32.MaxValue"/>.
		''' </summary>
		<Serializable>
		Public Shadows Class ExtractedTextTooLargeException
			Inherits ImporterException

			''' <summary>
			''' Initializes a new instance of the <see cref="ExtractedTextTooLargeException"/> class.
			''' </summary>
			Public Sub New()
				MyBase.New("Error occurred when importing the document. Extracted text is greater than 2 GB.")
			End Sub

			''' <inheritdoc />
			<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
			Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
				MyBase.New(info, context)
			End Sub
		End Class
#End Region

		Private Sub _artifactReader_DataSourcePrep(ByVal e As Api.DataSourcePrepEventArgs) Handles _artifactReader.DataSourcePrep
			OnDataSourcePrepEvent(e)
		End Sub

		Private Sub _artifactReader_StatusMessage(ByVal message As String) Handles _artifactReader.StatusMessage
			OnStatusMessage(New StatusEventArgs(EventType.Status, _artifactReader.CurrentLineNumber, RecordCount, message, False, CurrentStatisticsSnapshot, Statistics))
		End Sub

		Private Sub _artifactReader_FieldMapped(ByVal sourceField As String, ByVal workspaceField As String) Handles _artifactReader.FieldMapped
			OnFieldMapped(sourceField, workspaceField)
		End Sub

		Private Sub IoWarningHandler(ByVal sender As Object, e As IoWarningEventArgs)
			Dim ioWarningEventArgs As New IoWarningEventArgs(e.Message, e.CurrentLineNumber)
			Me.PublishIoWarningEvent(ioWarningEventArgs)
		End Sub

		Private Sub ManageErrors(ByVal artifactTypeID As Int32)
			If Not Me.BulkImportManager.NativeRunHasErrors(_caseInfo.ArtifactID, RunId) Then Exit Sub
			Dim sr As GenericCsvReader = Nothing
			Dim downloader As FileDownloader = Nothing
			Try
				With Me.BulkImportManager.GenerateNonImageErrorFiles(_caseInfo.ArtifactID, RunId, artifactTypeID, True, _keyFieldID)
					Me.WriteStatusLine(EventType.Status, "Retrieving errors from server")
					downloader = New FileDownloader(DirectCast(Me.BulkImportManager.Credentials, System.Net.NetworkCredential), _caseInfo.DocumentPath, _caseInfo.DownloadHandlerURL, Me.BulkImportManager.CookieContainer)
					AddHandler downloader.UploadStatusEvent, AddressOf LegacyUploader_UploadStatusEvent
					Dim errorsLocation As String = TempFileBuilder.GetTempFileName(TempFileConstants.ErrorsFileNameSuffix)
					sr = AttemptErrorFileDownload(downloader, errorsLocation, .LogKey, _caseInfo)

					If sr Is Nothing Then
						'If we're here and still have an empty response, we can at least notify
						'the user that there was an error retrieving all errors.
						' -Phil S. 08/13/2012
						Const message As String = "There was an error while attempting to retrieve the errors from the server."

						OnFatalError(message, New Exception(message), RunId)
					Else
						AddHandler sr.Context.IoWarningEvent, AddressOf Me.IoWarningHandler
						Dim line As String() = sr.ReadLine

						While Not line Is Nothing
							_errorCount += 1
							Dim ht As New System.Collections.Hashtable
							ht.Add("Message", line(1))
							ht.Add("Identifier", line(2))
							ht.Add("Line Number", Int32.Parse(line(0)))
							RaiseReportError(ht, Int32.Parse(line(0)), line(2), "server")
							OnStatusMessage(New StatusEventArgs(EventType.Error, Int32.Parse(line(0)) - 1, RecordCount, "[Line " & line(0) & "]" & line(1), CurrentStatisticsSnapshot, Statistics))
							line = sr.ReadLine
						End While
						RemoveHandler sr.Context.IoWarningEvent, AddressOf Me.IoWarningHandler
					End If

					RemoveHandler downloader.UploadStatusEvent, AddressOf LegacyUploader_UploadStatusEvent
				End With
			Catch ex As Exception
				Try
					If downloader IsNot Nothing Then RemoveHandler downloader.UploadStatusEvent, AddressOf LegacyUploader_UploadStatusEvent
					sr.Close()
					RemoveHandler sr.Context.IoWarningEvent, AddressOf Me.IoWarningHandler
				Catch
				End Try
				Throw
			End Try
		End Sub

		Private Function AttemptErrorFileDownload(ByVal downloader As FileDownloader, ByVal errorFileOutputPath As String, ByVal logKey As String, ByVal caseInfo As CaseInfo) As GenericCsvReader
			Dim triesLeft As Integer = 3
			Dim sr As GenericCsvReader = Nothing

			While triesLeft > 0
				downloader.MoveTempFileToLocal(errorFileOutputPath, logKey, caseInfo, False)
				sr = New GenericCsvReader(errorFileOutputPath, System.Text.Encoding.UTF8, True)
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

		Private Sub _processContext_ParentFormClosingEvent(ByVal sender As Object, e As ParentFormClosingEventArgs) Handles Context.ParentFormClosing
			If e.ProcessId.ToString = _processId.ToString Then
				CleanupTempTables()
			End If
		End Sub

		Protected Sub CleanupTempTables()
			If Not RunId Is Nothing AndAlso RunId <> "" Then
				Try
					Me.BulkImportManager.DisposeTempTables(_caseInfo.ArtifactID, RunId)
				Catch ex As Exception
					Me.LogWarning(ex, "Failed to drop the {RunId} SQL temp tables.", RunId)
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
			Return New kCura.WinEDDS.LoadFileReader(_settings, False, _executionSource)
		End Function

		Protected Sub OnFatalError(message As String, ex As Exception, runID As String)
			RaiseEvent FatalErrorEvent(message, ex, runID)
		End Sub

		Protected Sub OnStatusMessage(args As StatusEventArgs)
			RaiseEvent StatusMessage(args)
		End Sub

		Protected Sub OnEndFileImport(ByVal runID As String)
			RaiseEvent EndFileImport(runID)
		End Sub

		Protected Sub OnStartFileImport()
			RaiseEvent StartFileImport()
		End Sub

		Protected Sub OnDataSourcePrepEvent(args As Api.DataSourcePrepEventArgs)
			RaiseEvent DataSourcePrepEvent(args)
		End Sub

		Protected Sub OnReportErrorEvent(row As IDictionary)
			RaiseEvent ReportErrorEvent(row)
		End Sub

		Protected Sub OnFieldMapped(sourceField As String, workspaceField As String)
			RaiseEvent FieldMapped(sourceField, workspaceField)
		End Sub

	End Class

	Public Class WebServiceFieldInfoNameComparer
		Implements IComparer

		Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements System.Collections.IComparer.Compare
			Return String.Compare(DirectCast(x, kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo).DisplayName, DirectCast(y, kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo).DisplayName)
		End Function
	End Class
End Namespace