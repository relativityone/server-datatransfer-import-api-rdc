Imports System.Threading
Imports System.Collections.Generic
Imports kCura.WinEDDS.Api

Imports kCura.WinEDDS.Service
Imports Monitoring

Imports Relativity.DataExchange
Imports Relativity.DataExchange.Data
Imports Relativity.DataExchange.Io
Imports Relativity.DataExchange.Logging
Imports Relativity.DataExchange.Logger
Imports Relativity.DataExchange.Media
Imports Relativity.DataExchange.Process
Imports Relativity.DataExchange.Service
Imports Relativity.DataExchange.Transfer

Namespace kCura.WinEDDS
	Public Class BulkImageFileImporter
		Inherits ImportTapiBase

#Region "Members"
		Protected _imageReader As Api.IImageReader
		Protected _fieldQuery As FieldQuery
		Protected _productionManager As ProductionManager
		Protected _bulkImportManager As IBulkImportManager
		Private _folderID As Int32
		Private _productionArtifactID As Int32
		Private _overwrite As ImportOverwriteType
		Private _filePath As String
		Private _recordCount As Int64
		Private _replaceFullText As Boolean
		Private _importBatchSize As Int32?
		Private _jobCompleteBatchSize As Int32?
		Private _importBatchVolume As Int32?
		Private _minimumBatchSize As Int32?
		Private _autoNumberImages As Boolean
		Private _copyFilesToRepository As Boolean
		Private _defaultDestinationFolderPath As String
		Private _caseInfo As CaseInfo
		Private _overlayArtifactID As Int32
		Private _executionSource As ExecutionSource
		Private _lastRunMetadataImport As Int64 = 0

		Private WithEvents _processContext As ProcessContext
		Protected _keyFieldDto As kCura.EDDS.WebAPI.FieldManagerBase.Field
		Protected _fullTextStorageIsInSql As Boolean = True
		Private _bulkLoadFileWriter As System.IO.StreamWriter
		Private _dataGridFileWriter As System.IO.StreamWriter
		Private _uploadKey As String = ""
		Private _uploadDataGridKey As String = ""

		'TODO: rmove _localRunId  https://jira.kcura.com/browse/REL-414969
		Private _localRunId As String = System.Guid.NewGuid.ToString.Replace("-", "_") 
		Private _runId As String = ""
		Private _settings As ImageLoadFile
		Private _batchCount As Int32 = 0
		Private _jobCompleteImageCount As Int32 = 0
		Private _jobCompleteMetadataCount As Int32 = 0
		Private _errorCount As Int32 = 0
		Private _errorMessageFileLocation As String = ""
		Private _errorRowsFileLocation As String = ""
		Private _fileIdentifierLookup As System.Collections.Hashtable

		Private _processID As Guid
		Public Property MaxNumberOfErrorsInGrid As Int32 = AppSettings.Instance.DefaultMaxErrorCount
		Private _totalValidated As Long
		Private _totalProcessed As Long
		Private _startLineNumber As Int64

		Private _timekeeper As New Timekeeper2
		Private _doRetryLogic As Boolean
		Private _verboseErrorCollection As New ClientSideErrorCollection
		Private _prePushErrors As New List(Of Tuple(Of ImageRecord, String))
		Private _cancelledByUser As Boolean = False

		Private Property _imageValidator As IImageValidator = New ImageValidator()
		Private Property _tiffValidator As ITiffValidator = New TiffValidator()
		Private Property _fileInspector As IFileInspector = New FileInspector()

		Public Property SkipExtractedTextEncodingCheck As Boolean
		Public Property OIFileIdMapped As Boolean
		Public Property OIFileIdColumnName As String
		Public Property OIFileTypeColumnName As String
		Public Property FileNameColumn As String

		''' <summary>
		''' Gets total number of records. This property is used in our telemetry system.
		''' </summary>
		''' <returns>Total number of records.</returns>
		Friend ReadOnly Overridable Property TotalRecords As Long
			Get
				' check if _recordCount has already been updated to avoid unnecessary file I/O operation
				If _recordCount <= 0 Then _recordCount = _imageReader.CountRecords.GetValueOrDefault()
				Return _recordCount
			End Get
		End Property

		''' <summary>
		''' Gets number of completed records. This property is used in our telemetry system.
		''' </summary>
		''' <returns>Number of completed records.</returns>
		Friend ReadOnly Property CompletedRecords As Long
			Get
				Return TotalTransferredFilesCount
			End Get
		End Property
#End Region

#Region "Accessors"

		Public Property DisableImageTypeValidation As Boolean = AppSettings.Instance.DisableImageTypeValidation
		Public Property DisableImageLocationValidation As Boolean = AppSettings.Instance.DisableImageLocationValidation
		Public Property DisableUserSecurityCheck As Boolean
		Public Property AuditLevel As kCura.EDDS.WebAPI.BulkImportManagerBase.ImportAuditLevel = Config.AuditLevel

		Public ReadOnly Property BatchSizeHistoryList As List(Of Int32)

		Friend WriteOnly Property FilePath() As String
			Set(ByVal value As String)
				_filePath = value
			End Set
		End Property
		Friend ReadOnly Property IsCancelledByUser As Boolean
			Get
				Return _cancelledByUser
			End Get
		End Property

		Public ReadOnly Property HasErrors() As Boolean
			Get
				Return _errorCount > 0
			End Get
		End Property

		Public ReadOnly Property RunId As String
			Get
				Return _runId
			End Get
		End Property

		Protected ReadOnly Property [Continue]() As Boolean
			Get
				Return _imageReader.HasMoreRecords AndAlso ShouldImport
			End Get
		End Property

		Public ReadOnly Property UploadConnection() As TapiClient
			Get
				Return Me.FileTapiClient
			End Get
		End Property

		Public ReadOnly Property ErrorLogFileName() As String
			Get
				Return _errorMessageFileLocation
			End Get
		End Property

		Protected Overrides ReadOnly Property CurrentLineNumber() As Integer
			Get
				Return _imageReader.CurrentRecordNumber
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

		Protected Overridable Property ImportBatchSize As Int32
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

		Protected Overridable ReadOnly Property NumberOfRetries() As Int32
			Get
				Return AppSettings.Instance.IoErrorNumberOfRetries
			End Get
		End Property

		Protected Overridable ReadOnly Property WaitTimeBetweenRetryAttempts() As Int32
			Get
				Return AppSettings.Instance.IoErrorWaitTimeInSeconds
			End Get
		End Property

		Protected Overridable ReadOnly Property BatchResizeEnabled As Boolean
			Get
				Return AppSettings.Instance.DynamicBatchResizingOn
			End Get
		End Property

#End Region

#Region "Constructors"
		Public Sub New(folderID As Int32, _
		               args As ImageLoadFile, _
		               context As ProcessContext, _
		               reporter As IIoReporter, _
		               logger As Global.Relativity.Logging.ILog, _
		               processID As Guid, _
		               doRetryLogic As Boolean, _
		               tokenSource As CancellationTokenSource, _
		               Optional ByVal executionSource As ExecutionSource = ExecutionSource.Unknown)
			MyBase.New(reporter, logger, tokenSource)

			_executionSource = executionSource
			
			'TODO: generate runId  https://jira.kcura.com/browse/REL-414969
			'_runId = System.Guid.NewGuid.ToString.Replace("-", "_")

			_doRetryLogic = doRetryLogic
			InitializeManagers(args)
			Dim suffix As String = "\EDDS" & args.CaseInfo.ArtifactID & "\"
			If args.SelectedCasePath = "" Then
				_defaultDestinationFolderPath = args.CaseDefaultPath.TrimEnd("\"c) & suffix
			Else
				_defaultDestinationFolderPath = args.SelectedCasePath.TrimEnd("\"c) & suffix
			End If

			InitializeUploaders(args)
			_folderID = folderID
			_productionArtifactID = args.ProductionArtifactID
			Statistics.ImportObjectType = CType(IIf(_productionArtifactID = 0, TelemetryConstants.ImportObjectType.Image, TelemetryConstants.ImportObjectType.ProductionImage), TelemetryConstants.ImportObjectType)
			InitializeDTOs(args)
			If(args.Overwrite.IsNullOrEmpty)
				_overwrite = ImportOverwriteType.Append
			Else 
				_overwrite = CType([Enum].Parse(GetType(ImportOverwriteType),args.Overwrite, True), ImportOverwriteType)
			End If
			_replaceFullText = args.ReplaceFullText
			_processContext = context
			_copyFilesToRepository = args.CopyFilesToDocumentRepository
			ShouldImport = True
			_autoNumberImages = args.AutoNumberImages
			_caseInfo = args.CaseInfo
			_settings = args
			_processID = processID
			_startLineNumber = args.StartLineNumber
			_overlayArtifactID = args.IdentityFieldId

			BatchSizeHistoryList = New List(Of Int32)

			If args.ReplaceFullText Then
				_fullTextStorageIsInSql = (_fieldQuery.RetrieveAllAsDocumentFieldCollection(args.CaseInfo.ArtifactID, ArtifactType.Document).FullText.EnableDataGrid = False)
			End If
		End Sub

		Protected Overridable Sub InitializeUploaders(ByVal args As ImageLoadFile)
			Dim gateway As FileIO = New FileIO(args.Credential, args.CookieContainer)
			Dim nativeParameters As UploadTapiBridgeParameters2 = New UploadTapiBridgeParameters2
			nativeParameters.BcpFileTransfer = False
			nativeParameters.AsperaBcpRootFolder = String.Empty

			' This will tie both native and BCP to a single unique identifier.
			nativeParameters.Application = AppSettings.Instance.ApplicationName
			nativeParameters.ClientRequestId = Guid.NewGuid()
			nativeParameters.Credentials = args.Credential
			nativeParameters.AsperaDocRootLevels = AppSettings.Instance.TapiAsperaNativeDocRootLevels
			nativeParameters.AsperaDatagramSize = AppSettings.Instance.TapiAsperaDatagramSize
			nativeParameters.FileShare = args.CaseInfo.DocumentPath
			nativeParameters.ForceAsperaClient = AppSettings.Instance.TapiForceAsperaClient
			nativeParameters.ForceClientCandidates = AppSettings.Instance.TapiForceClientCandidates
			nativeParameters.ForceFileShareClient = AppSettings.Instance.TapiForceFileShareClient
			nativeParameters.ForceHttpClient = AppSettings.Instance.ForceWebUpload OrElse AppSettings.Instance.TapiForceHttpClient
			nativeParameters.LargeFileProgressEnabled = AppSettings.Instance.TapiLargeFileProgressEnabled
			nativeParameters.LogConfigFile = AppSettings.Instance.LogConfigXmlFileName
			nativeParameters.MaxFilesPerFolder = gateway.RepositoryVolumeMax
			nativeParameters.MaxInactivitySeconds = AppSettings.Instance.TapiMaxInactivitySeconds
			nativeParameters.MaxJobParallelism = AppSettings.Instance.TapiMaxJobParallelism
			nativeParameters.MaxJobRetryAttempts = Me.NumberOfRetries
			nativeParameters.MinDataRateMbps = AppSettings.Instance.TapiMinDataRateMbps
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
			nativeParameters.PreserveFileTimestamps = AppSettings.Instance.TapiPreserveFileTimestamps
			nativeParameters.BadPathErrorsRetry = AppSettings.Instance.TapiBadPathErrorsRetry
			nativeParameters.FileNotFoundErrorsDisabled = AppSettings.Instance.TapiFileNotFoundErrorsDisabled
			nativeParameters.FileNotFoundErrorsRetry = AppSettings.Instance.TapiFileNotFoundErrorsRetry

			' Copying the parameters and tweaking just a few BCP specific parameters.
			Dim bcpParameters As UploadTapiBridgeParameters2 = nativeParameters.ShallowCopy()
			bcpParameters.BcpFileTransfer = True
			bcpParameters.AsperaBcpRootFolder = AppSettings.Instance.TapiAsperaBcpRootFolder
			bcpParameters.FileShare = gateway.GetBcpSharePath(args.CaseInfo.ArtifactID)
			bcpParameters.SortIntoVolumes = False
			bcpParameters.ForceHttpClient = bcpParameters.ForceHttpClient Or AppSettings.Instance.TapiForceBcpHttpClient

			' Never preserve timestamps for BCP load files.
			bcpParameters.PreserveFileTimestamps = false
			CreateTapiBridges(nativeParameters, bcpParameters, args.WebApiCredential.TokenProvider)
		End Sub

		Protected Overridable Sub InitializeDTOs(ByVal args As ImageLoadFile)
			Dim fieldManager As FieldManager = New kCura.WinEDDS.Service.FieldManager(args.Credential, args.CookieContainer)

			' slm- 10/10/2011 - fixed both of these to check for ID greater than zero
			If _productionArtifactID > 0 Then
				_keyFieldDto = fieldManager.Read(args.CaseInfo.ArtifactID, args.BeginBatesFieldArtifactID)
			ElseIf args.IdentityFieldId > 0 Then
				_keyFieldDto = fieldManager.Read(args.CaseInfo.ArtifactID, args.IdentityFieldId)
			Else
				Dim fieldID As Int32 = _fieldQuery.RetrieveAllAsDocumentFieldCollection(args.CaseInfo.ArtifactID, ArtifactType.Document).IdentifierFields(0).FieldID
				_keyFieldDto = fieldManager.Read(args.CaseInfo.ArtifactID, fieldID)
			End If
		End Sub

		Protected Overridable Sub InitializeManagers(ByVal args As ImageLoadFile)
			_fieldQuery = New FieldQuery(args.Credential, args.CookieContainer)
			_productionManager = New ProductionManager(args.Credential, args.CookieContainer)
			_bulkImportManager = New BulkImportManager(args.Credential, args.CookieContainer)
		End Sub

#End Region

#Region "Main"

		Public Sub ReadFile()
			Me.ReadFile(_filePath)
		End Sub

		Private Sub ProcessList(ByVal al As List(Of Api.ImageRecord), ByRef status As Int64, ByVal bulkLoadFilePath As String, ByVal dataGridFilePath As String)
			If al.Count = 0 Then Exit Sub
			Me.ProcessDocument(al, status)
			al.Clear()
			status = 0
			If (_bulkLoadFileWriter.BaseStream.Length + _dataGridFileWriter.BaseStream.Length > ImportBatchVolume) OrElse _batchCount > ImportBatchSize - 1 Then
				Me.TryPushImageBatch(bulkLoadFilePath, dataGridFilePath, False, _jobCompleteImageCount >= JobCompleteBatchSize, _jobCompleteMetadataCount >= JobCompleteBatchSize)
			End If
		End Sub

		Public Function RunBulkImport(ByVal overwrite As kCura.EDDS.WebAPI.BulkImportManagerBase.OverwriteType, ByVal useBulk As Boolean) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Dim totalTries As Int32 = NumberOfRetries
			Dim tries As Int32 = totalTries
			Dim retval As New kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			While tries > 0
				Try
					retval = BulkImport(overwrite, useBulk)
					Exit While
				Catch ex As Exception
					tries -= 1
					If tries = 0
						Me.LogFatal(ex, "The image bulk import service call failed and exceeded the max retry attempts.")
						Throw
					Else If IsTimeoutException(ex)
						' A timeout exception can be retried.
						Me.LogError(ex, "A fatal SQL or HTTP timeout error has occurred bulk importing the image batch.")
						Throw
					Else If Not ShouldImport
						' Don't log cancel requests
						Throw
					Else If IsBulkImportSqlException(ex)
						Me.LogFatal(ex, "A fatal SQL error has occurred bulk importing the image batch.")
						Throw
					Else If IsInsufficientPermissionsForImportException(ex)
						Me.LogFatal(ex, "A fatal insufficient permissions error has occurred bulk importing the image batch.")
						Throw
					Else
						Me.LogWarning(ex, "A serious error has occurred bulk importing the image batch. Retry info: {Count} of {TotalRetry}.", totalTries - tries, totalTries)
						Me.RaiseWarningAndPause(ex, WaitTimeBetweenRetryAttempts, totalTries - tries, totalTries)
					End If
				End Try
			End While
			Return retval
		End Function

		Private Function BulkImport(ByVal overwrite As kCura.EDDS.WebAPI.BulkImportManagerBase.OverwriteType, ByVal useBulk As Boolean) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Dim retval As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Dim settings As kCura.EDDS.WebAPI.BulkImportManagerBase.ImageLoadInfo = Me.GetSettingsObject
			settings.UseBulkDataImport = useBulk
			settings.Overlay = overwrite
			settings.Billable = _settings.Billable

			If _productionArtifactID = 0 Then
				retval = _bulkImportManager.BulkImportImage(_caseInfo.ArtifactID, settings, _copyFilesToRepository)
			Else
				retval = _bulkImportManager.BulkImportProductionImage(_caseInfo.ArtifactID, settings, _productionArtifactID, _copyFilesToRepository)
			End If
			Return retval
		End Function

		Protected Overridable Sub LowerBatchLimits()
			Dim oldBatchSize As Int32 = Me.ImportBatchSize
			Me.ImportBatchSize -= 100
			Me.Statistics.BatchSize = Me.ImportBatchSize
			Me.BatchSizeHistoryList.Add(Me.ImportBatchSize)
			Me.LogWarning("Lowered the image batch limits from {OldBatchSize} to {NewBatchSize}.", oldBatchSize, Me.ImportBatchSize)
		End Sub

		Private Function GetSettingsObject() As kCura.EDDS.WebAPI.BulkImportManagerBase.ImageLoadInfo
			Dim settings As New kCura.EDDS.WebAPI.BulkImportManagerBase.ImageLoadInfo With {
			.RunID = _runId,
			.DestinationFolderArtifactID = _folderID,
			.BulkFileName = _uploadKey,
			.DataGridFileName = _uploadDataGridKey,
			.KeyFieldArtifactID = _keyFieldDto.ArtifactID,
			.Repository = _defaultDestinationFolderPath,
			.UploadFullText = _replaceFullText,
			.DisableUserSecurityCheck = Me.DisableUserSecurityCheck,
			.AuditLevel = Me.AuditLevel,
			.OverlayArtifactID = _overlayArtifactID,
			.ExecutionSource = CType(_executionSource, kCura.EDDS.WebAPI.BulkImportManagerBase.ExecutionSource)
			}
			Return settings
		End Function

		Private Sub TryPushImageBatch(ByVal bulkLoadFilePath As String, ByVal dataGridFilePath As String, ByVal isFinal As Boolean, ByVal shouldCompleteImageJob As Boolean, ByVal shouldCompleteMetadataJob As Boolean)
			_bulkLoadFileWriter.Close()
			_dataGridFileWriter.Close()
			_fileIdentifierLookup.Clear()

			If (shouldCompleteImageJob Or isFinal) And _jobCompleteImageCount > 0 Then
				_jobCompleteImageCount = 0
				Me.AwaitPendingPhysicalFileUploadsForJob()
			End If

			Try
				If ShouldImport AndAlso _copyFilesToRepository AndAlso Me.FileTapiBridge.TransfersPending Then
					Me.AwaitPendingPhysicalFileUploadsForBatch()
					Me.JobCounter += 1
				End If

				Dim start As Int64 = System.DateTime.Now.Ticks

				If ShouldImport
					PushImageBatch(bulkLoadFilePath, dataGridFilePath, shouldCompleteMetadataJob, isFinal)
				End If

				Me.Statistics.FileWaitDuration += New TimeSpan(System.Math.Max((System.DateTime.Now.Ticks - start), 1))
			Catch ex As Exception
				If BatchResizeEnabled AndAlso IsTimeoutException(ex) AndAlso ShouldImport Then
					Me.LogWarning(ex, "A SQL or HTTP timeout error has occurred bulk importing the image batch and the batch will be resized.")
					Dim originalBatchSize As Int32 = Me.ImportBatchSize
					LowerBatchLimits()
					Me.RaiseWarningAndPause(ex, WaitTimeBetweenRetryAttempts)
					If Not ShouldImport Then
						Throw
					End If

					Me.LowerBatchSizeAndRetry(bulkLoadFilePath, dataGridFilePath, originalBatchSize)
				Else
					If ShouldImport AndAlso Not BatchResizeEnabled Then
						Me.LogFatal(ex, "Pushing the image batch failed but lowering the batch and performing a retry is disabled.")
					End If

					If ShouldImport AndAlso BatchResizeEnabled Then
						Me.LogFatal(ex, "Pushing the image batch failed but lowering the batch isn't supported because the error isn't timeout related.")
					End If

					Throw
				End If
			End Try
			
			DeleteFiles(bulkLoadFilePath, dataGridFilePath)
			If Not isFinal Then
				Try
					_bulkLoadFileWriter = New System.IO.StreamWriter(bulkLoadFilePath, False, System.Text.Encoding.Unicode)
					_dataGridFileWriter = New System.IO.StreamWriter(dataGridFilePath, False, System.Text.Encoding.Unicode)
				Catch ex As Exception
					Me.LogWarning(ex, "Failed to create new image bulk load files. Preparing to retry...")
					_bulkLoadFileWriter = New System.IO.StreamWriter(bulkLoadFilePath, False, System.Text.Encoding.Unicode)
					_dataGridFileWriter = New System.IO.StreamWriter(dataGridFilePath, False, System.Text.Encoding.Unicode)
				End Try
			End If

			UpdateStatisticsSnapshot(System.DateTime.Now, True)
		End Sub

		Protected Sub LowerBatchSizeAndRetry(ByVal oldBulkLoadFilePath As String, ByVal dataGridFilePath As String, ByVal totalRecords As Int32)
			'NOTE: we are not cutting a new/smaller data grid bulk file because it will be chunked as it is loaded into the data grid
			Dim newBulkLoadFilePath As String = TempFileBuilder.GetTempFileName(TempFileConstants.NativeLoadFileNameSuffix)
			Dim limit As String = ServiceConstants.ENDLINETERMSTRING
			Dim last As New System.Collections.Generic.Queue(Of Char)
			Dim recordsProcessed As Int32 = 0
			Dim charactersSuccessfullyProcessed As Int64 = 0
			Dim hasReachedEof As Boolean = False
			Dim tries As Int32 = 1 'already starts at 1 retry
			While totalRecords > recordsProcessed AndAlso Not hasReachedEof AndAlso ShouldImport
				Dim i As Int32 = 0
				Dim charactersProcessed As Int64 = 0
				Using sr As System.IO.TextReader = CreateStreamReader(oldBulkLoadFilePath), sw As System.IO.TextWriter = CreateStreamWriter(newBulkLoadFilePath)
					Me.AdvanceStream(sr, charactersSuccessfullyProcessed)
					Dim tempBatchSize As Int32 = Me.ImportBatchSize
					While (Not hasReachedEof AndAlso i < tempBatchSize)
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
						If Not i < tempBatchSize AndAlso sr.Peek = AscW("0") Then
							tempBatchSize += 1
						End If
					End While
					sw.Flush()
				End Using
				Try
					recordsProcessed = DoLogicAndPushImageBatch(totalRecords, recordsProcessed, newBulkLoadFilePath, dataGridFilePath, charactersSuccessfullyProcessed, i, charactersProcessed)
				Catch ex As Exception
					If tries < NumberOfRetries AndAlso BatchResizeEnabled AndAlso IsTimeoutException(ex) AndAlso ShouldImport Then
						LowerBatchLimits()
						Me.RaiseWarningAndPause(ex, WaitTimeBetweenRetryAttempts, tries, NumberOfRetries)
						If Not ShouldImport Then Throw
						'after the pause
						tries += 1
						hasReachedEof = False
					Else
						Global.Relativity.DataExchange.Io.FileSystem.Instance.File.Delete(newBulkLoadFilePath)
						Throw
					End If
				End Try
			End While
			Global.Relativity.DataExchange.Io.FileSystem.Instance.File.Delete(newBulkLoadFilePath)
		End Sub

		Protected Overridable Function DoLogicAndPushImageBatch(ByVal totalRecords As Integer, ByVal recordsProcessed As Integer, ByVal bulkLocation As String, ByVal dataGridLocation As String, ByRef charactersSuccessfullyProcessed As Long, ByVal i As Integer, ByVal charactersProcessed As Long) As Integer
			_batchCount = i
			RaiseStatusEvent(EventType2.Warning, "Begin processing sub-batch of size " & i & ".", CType((_totalValidated + _totalProcessed) / 2, Int64))
			Me.PushImageBatch(bulkLocation, dataGridLocation, False, True)
			RaiseStatusEvent(EventType2.Warning, "End processing sub-batch of size " & i & ".  " & recordsProcessed & " of " & totalRecords & " in the original batch processed", CType((_totalValidated + _totalProcessed) / 2, Int64))
			recordsProcessed += i
			charactersSuccessfullyProcessed += charactersProcessed
			Return recordsProcessed
		End Function

		Private Sub DeleteFiles(ByVal bulkFilePath As String, ByVal datagridFilePath As String)
			Global.Relativity.DataExchange.Io.FileSystem.Instance.File.Delete(bulkFilePath)
			Global.Relativity.DataExchange.Io.FileSystem.Instance.File.Delete(datagridFilePath)
		End Sub

		Protected Overridable Function CreateStreamWriter(ByVal tmpLocation As String) As System.IO.TextWriter
			Return New System.IO.StreamWriter(tmpLocation, False, System.Text.Encoding.Unicode)
		End Function

		Protected Overridable Function CreateStreamReader(ByVal outputPath As String) As System.IO.TextReader
			Return New System.IO.StreamReader(outputPath, System.Text.Encoding.Unicode)
		End Function

		Private Sub AdvanceStream(ByVal sr As System.IO.TextReader, ByVal count As Int64)
			If count > 0 Then
				For j As Int64 = 0 To count - 1
					sr.Read()
				Next
			End If
		End Sub

		Public Sub PushImageBatch(ByVal bulkLoadFilePath As String, ByVal dataGridFilePath As String, ByVal shouldCompleteJob As Boolean, ByVal lastRun As Boolean)
			ManagePrePushErrors()
			If _lastRunMetadataImport > 0 Then
				Me.Statistics.MetadataWaitDuration += New TimeSpan(System.DateTime.Now.Ticks - _lastRunMetadataImport)
			End If

			If _batchCount = 0 Then
				If _jobCompleteMetadataCount > 0 Then
					_jobCompleteMetadataCount = 0
					Me.AwaitPendingBulkLoadFileUploadsForJob()
				End If
				Return
			End If

			If shouldCompleteJob And _jobCompleteMetadataCount > 0 Then
				_jobCompleteMetadataCount = 0
				Me.AwaitPendingBulkLoadFileUploadsForJob()
			End If

			_batchCount = 0
			Const retry As Boolean = True
			Me.Statistics.MetadataTransferredBytes += (Me.GetFileLength(bulkLoadFilePath, retry) + Me.GetFileLength(dataGridFilePath, retry))
			
			_uploadKey = Me.BulkLoadTapiBridge.AddPath(bulkLoadFilePath, Guid.NewGuid().ToString(), 1)
			_uploadDataGridKey = Me.BulkLoadTapiBridge.AddPath(dataGridFilePath, Guid.NewGuid().ToString(), 2)

			' keep track of the total count of added files
			MetadataFilesCount += 2
			_jobCompleteMetadataCount += 2

			If lastRun Then
				Me.AwaitPendingBulkLoadFileUploadsForJob()
			Else
				Me.AwaitPendingBulkLoadFileUploadsForBatch()
			End If

			_lastRunMetadataImport = System.DateTime.Now.Ticks

			Dim overwrite As kCura.EDDS.WebAPI.BulkImportManagerBase.OverwriteType
			Select Case _overwrite
				Case ImportOverwriteType.AppendOverlay
					overwrite = EDDS.WebAPI.BulkImportManagerBase.OverwriteType.Both
				Case ImportOverwriteType.Overlay
					overwrite = EDDS.WebAPI.BulkImportManagerBase.OverwriteType.Overlay
				Case Else
					overwrite = EDDS.WebAPI.BulkImportManagerBase.OverwriteType.Append
			End Select

			If ShouldImport Then
				Dim start As Int64 = System.DateTime.Now.Ticks
				Dim runResults As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults = Me.RunBulkImport(overwrite, True)
				Me.Statistics.ProcessMassImportResults(runResults)

				'TODO: remove runId set up and mapping log https://jira.kcura.com/browse/REL-414969
				If String.IsNullOrWhiteSpace(_runId) Then
					_runId = runResults.RunID
					Logger.LogWarning("CorrelationId mapping [{localId}] - [{runId}]", _localRunId, _runId)
				End If

				Dim numberOfTicks As Long = System.DateTime.Now.Ticks - start
				Dim batchDuration As TimeSpan = New TimeSpan(numberOfTicks)
				Me.Statistics.MassImportDuration += batchDuration
				Me.Statistics.BatchCount += 1

				Logger.LogInformation("Duration of mass import processing: {durationInMilliseconds}, batch: {numberOfBatch}", batchDuration.TotalMilliseconds, Me.Statistics.BatchCount)
				ManageErrors()

				Me.TotalTransferredFilesCount = Me.FileTapiProgressCount

				Dim batchInformation As New BatchInformation With {
						.OrdinalNumber = Statistics.BatchCount,
						.NumberOfRecords = runResults.FilesProcessed,
						.MassImportDuration = batchDuration
						}
				MyBase.OnBatchCompleted(batchInformation)
			End If
		End Sub

		Public Overridable Function GetImageReader() As kCura.WinEDDS.Api.IImageReader
			Return New OpticonFileReader(_folderID, _settings, Nothing, Nothing, _doRetryLogic)
		End Function

		Public Sub AdvanceRecord()
			_imageReader.AdvanceRecord()
		End Sub

		Public Sub ReadFile(ByVal path As String)
			_timekeeper.MarkStart("TOTAL")

			Using _logger.LogImportContextPushProperties(New LogContext(_localRunId, _settings.CaseInfo.ArtifactID))
				_logger.LogUserContextInformation("Start import process", _settings.Credential)

				Dim bulkLoadFilePath As String = TempFileBuilder.GetTempFileName(TempFileConstants.NativeLoadFileNameSuffix)
				Dim dataGridFilePath As String = TempFileBuilder.GetTempFileName(TempFileConstants.DatagridLoadFileNameSuffix)

				_fileIdentifierLookup = New System.Collections.Hashtable
				_totalProcessed = 0
				_totalValidated = 0
				Me.TotalTransferredFilesCount = 0
				Me.JobCounter = 1
				Me.FileTapiProgressCount = 0
				DeleteFiles(bulkLoadFilePath, dataGridFilePath)
				_bulkLoadFileWriter = New System.IO.StreamWriter(bulkLoadFilePath, False, System.Text.Encoding.Unicode)
				_dataGridFileWriter = New System.IO.StreamWriter(dataGridFilePath, False, System.Text.Encoding.Unicode)

				Try
					_timekeeper.MarkStart("ReadFile_Init")
					_filePath = path
					_imageReader = Me.GetImageReader
					_imageReader.Initialize()
					_recordCount = _imageReader.CountRecords.GetValueOrDefault()

					RaiseStatusEvent(EventType2.Progress, "Begin Image Upload", 0)
					RaiseStatusEvent(EventType2.ResetStartTime, "", 0)
					Dim al As New List(Of Api.ImageRecord)
					Dim status As Int64 = 0
					_timekeeper.MarkEnd("ReadFile_Init")

					_timekeeper.MarkStart("ReadFile_Main")
				
					' This will safely force the status bar to update immediately.
					Me.OnTapiClientChanged()
					Me.LogInformation("Preparing to import images via WinEDDS.")
					Me.Statistics.BatchSize = Me.ImportBatchSize
					If _productionArtifactID <> 0 Then _productionManager.DoPreImportProcessing(_caseInfo.ArtifactID, _productionArtifactID)
					While Me.[Continue]
						If Me.CurrentLineNumber < _startLineNumber Then
							Me.AdvanceRecord()

							' This will ensure progress takes into account the start line number
							Me.FileTapiProgressCount = Me.FileTapiProgressCount + 1
						Else
							'The EventType.Count is used as an 'easy' way for the ImportAPI to eventually get a record count.
							' It could be done in DataReaderClient in other ways, but those ways turned out to be pretty messy.
							' -Phil S. 06/12/2012
							RaiseStatusEvent(EventType2.Count, String.Empty, 0)
							Dim record As Api.ImageRecord = _imageReader.GetImageRecord
							record.OriginalIndex = _imageReader.CurrentRecordNumber
							If (record.IsNewDoc) Then
								Me.ProcessList(al, status, bulkLoadFilePath, dataGridFilePath)
							End If
							status = status Or Me.ProcessImageLine(record)
							Try
								ValidateImageRecord(record)
								al.Add(record)
							Catch ex As ImageDataValidationException
								_verboseErrorCollection.AddError(record.OriginalIndex, ex)
								_prePushErrors.Add(New Tuple(Of ImageRecord, String)(record, ex.Message))
							End Try

							If Not Me.[Continue] Then
								Me.ProcessList(al, status, bulkLoadFilePath, dataGridFilePath)
								Exit While
							End If
						End If
					End While
					_timekeeper.MarkEnd("ReadFile_Main")
					_timekeeper.MarkStart("ReadFile_Cleanup")
					Me.TryPushImageBatch(bulkLoadFilePath, dataGridFilePath, True, True, False)
					Me.LogInformation("Successfully imported {ImportCount} images via WinEDDS.", Me.FileTapiProgressCount)
					Me.CompleteSuccess()
					_timekeeper.MarkEnd("ReadFile_Cleanup")
					_timekeeper.MarkEnd("TOTAL")
					_timekeeper.GenerateCsvReportItemsAsRows("_winedds_image", "C:\")
				Catch ex As Exception
					Me.LogFatal(ex, "A serious unexpected error has occurred importing images.")
					Me.CompleteError(ex)
				Finally
					RaiseEvent EndRun(_runId)
					'has to be called after Raise EndRun event
					Me.LogStatistics()
					_timekeeper.MarkStart("ReadFile_CleanupTempTables")
					DestroyTapiBridges()
					CleanupTempTables()
					_logger.LogUserContextInformation("Import process completed", _settings.Credential)
					_timekeeper.MarkEnd("ReadFile_CleanupTempTables")
				End Try
			End Using
		End Sub

		Public Event EndRun(ByVal runID As String)

		Private Sub CompleteSuccess()
			If Not _imageReader Is Nothing Then _imageReader.Close()
			If _productionArtifactID <> 0 Then _productionManager.DoPostImportProcessing(Me.FileTapiBridge.WorkspaceId, _productionArtifactID)

			If CancellationToken.IsCancellationRequested Then
				OnWriteStatusMessage(EventType2.Status, "Job has been finalized", TapiConstants.NoLineNumber, TapiConstants.NoLineNumber)
			Else
				OnWriteStatusMessage(EventType2.Status, "End Image Upload")
			End If
		End Sub

		Private Sub CompleteError(ByVal exception As System.Exception)

			Try
				_bulkLoadFileWriter.Close()
				_dataGridFileWriter.Close()
			Catch ex As Exception
				Me.LogWarning(ex, "Failed to close the image load file writers before raising the image import fatal error.")
			End Try
			Try
				If Not _imageReader Is Nothing Then
					_imageReader.Close()
				End If
			Catch ex As Exception
				Me.LogWarning(ex, "Failed to close the image reader before raising the image import fatal error.")
			End Try
			Try
				Me.ManageErrors()
			Catch ex As Exception
				Me.LogWarning(ex, "Failed to manage errors before raising the image import fatal error.")
			End Try

			RaiseFatalError(exception)
		End Sub

		Private Sub ProcessDocument(ByVal al As List(Of Api.ImageRecord), ByVal status As Int64)
			GetImagesForDocument(al, status)
			'We want DocCount to represent the how many documents have been sent to import
			'in case of BulkImageFileImporter this is represented best by count of IsNewDoc
			Me.Statistics.DocumentsCount += 1
		End Sub

#End Region

#Region "Worker Methods"

		Public Function ProcessImageLine(ByVal imageRecord As Api.ImageRecord) As ImportStatus
			_totalValidated += 1			
			'check for existence
			If imageRecord.BatesNumber.Trim = "" Then
				Me.RaiseStatusEvent(EventType2.Error, "No image file or identifier specified on line.", CType((_totalValidated + _totalProcessed) / 2, Int64))
				Return ImportStatus.NoImageSpecifiedOnLine
			End If

			Dim imageFilePath As String = BulkImageFileImporter.GetFileLocation(imageRecord)

			If Not Me.DisableImageLocationValidation Then
				Const retry As Boolean = True
				Dim foundFileName As String = Me.GetExistingFilePath(imageFilePath, retry)
				Dim fileExists As Boolean = Not String.IsNullOrEmpty(foundFileName)

				If Not fileExists
					Me.RaiseStatusEvent(EventType2.Error, $"Image file specified ( {imageRecord.FileLocation} ) does not exist.", CType((_totalValidated + _totalProcessed) / 2, Int64))
					Return ImportStatus.FileSpecifiedDne
				End If

				If Not String.Equals(imageFilePath, foundFileName)
					Me.RaiseStatusEvent(EventType2.Warning ,$"File {imageFilePath} does not exist. File {foundFileName} will be used instead.", CType((_totalValidated + _totalProcessed) / 2, Int64))
					imageFilePath = foundFileName
				End If
			End If

			Dim retval As ImportStatus = ImportStatus.Pending

			Try
				If Not Me.DisableImageTypeValidation Then
					Dim result As ImageValidationResult = _imageValidator.IsImageValid(imageFilePath, _tiffValidator, _fileInspector)
					If(Not result.IsValid)
						Throw New ImageFileValidationException(result.Message)
					End If
				End If

				Me.RaiseStatusEvent(EventType2.Status, $"Image file ( {imageRecord.FileLocation} ) validated.", CType((_totalValidated + _totalProcessed) / 2, Int64))
			Catch ex As ImageFileValidationException
				Me.LogError(ex, "Failed to validate the {Path} image.", imageFilePath.Secure())
				retval = ImportStatus.InvalidImageFormat
				_verboseErrorCollection.AddError(imageRecord.OriginalIndex, ex)
			Catch ex As Exception
				Me.LogFatal(ex, "Unexpected failure to validate the {Path} image file.", imageFilePath.Secure())
					Throw
			End Try

			Return retval
		End Function

		Private Sub ValidateImageRecord(ByVal record As Api.ImageRecord)
			If record.BatesNumber?.IndexOf(",") > -1 Then
				Throw New ImageDataValidationException("Bates number contains unsupported char ','")
			End If
			If record.FileName?.IndexOf(",") > -1 Then
				Throw New ImageDataValidationException("File name contains unsupported char ','")
			End If
			If record.FileLocation?.IndexOf(",") > -1 Then
				Throw New ImageDataValidationException("File location contains unsupported char ','")
			End If
		End Sub

		Public Shared Function GetFileLocation(ByVal record As Api.ImageRecord) As String
			Dim fileLocation As String = record.FileLocation
			If fileLocation <> "" AndAlso fileLocation.Chars(0) = "\" AndAlso fileLocation.Chars(1) <> "\" Then
				fileLocation = "." & fileLocation
			End If
			Return fileLocation
		End Function


		Private Sub GetImagesForDocument(ByVal lines As List(Of Api.ImageRecord), ByVal status As Int64)
			Me.AutoNumberImages(lines)
			Dim hasFileIdentifierProblem As Boolean = False
			For Each line As Api.ImageRecord In lines
				If _fileIdentifierLookup.ContainsKey(line.BatesNumber.Trim) Then
					hasFileIdentifierProblem = True
				Else
					_fileIdentifierLookup.Add(line.BatesNumber.Trim, line.BatesNumber.Trim)
				End If
			Next
			If hasFileIdentifierProblem Then status += ImportStatus.IdentifierOverlap

			Dim record As Api.ImageRecord = lines(0)
			Dim textFileList As New System.Collections.ArrayList
			Dim documentId As String = record.BatesNumber
			Dim offset As Int64 = 0
			For i As Int32 = 0 To lines.Count - 1
				If Not ShouldImport Then 
					Exit For
				End If
				record = lines(i)

				Const retry As Boolean = True
				Dim imageFilePath As String = BulkImageFileImporter.GetFileLocation(record)
				Dim foundFileName As String = Me.GetExistingFilePath(imageFilePath, retry)
				Dim originalFileName As String = lines(i).FileName

				If Not (foundFileName Is Nothing)
					imageFilePath = foundFileName
				End If

				Me.GetImageForDocument(imageFilePath, originalFileName, record.BatesNumber, documentId, i, offset, textFileList, i < lines.Count - 1, Convert.ToInt32(record.OriginalIndex), status, i = 0)
			Next

			Dim lastDivider As String = If(_fullTextStorageIsInSql, ",", String.Empty)

			If _replaceFullText Then
				If Not _fullTextStorageIsInSql Then
					'datagrid metadata including a blank data grid id
					_dataGridFileWriter.Write(documentId & "," & String.Empty & ",")
				End If

				If textFileList.Count = 0 Then
					'no extracted text encodings, write "-1"
					_bulkLoadFileWriter.Write($"{(-1)}{lastDivider}")
				ElseIf textFileList.Count > 0 Then
					_bulkLoadFileWriter.Write("{0}{1}", Me.GetExtractedTextEncodings(textFileList).ToDelimitedString("|"), lastDivider)
				End If


				Dim fullTextWriter As System.IO.StreamWriter = If(_fullTextStorageIsInSql, _bulkLoadFileWriter, _dataGridFileWriter)
				For Each filename As String In textFileList
					Dim chosenEncoding As System.Text.Encoding = _settings.FullTextEncoding
					Dim fileStream As System.IO.Stream

					If Not SkipExtractedTextEncodingCheck Then
						'We pass in 'False' as the final parameter to DetectEncoding to have it skip the File.Exists check. This
						' check can be very expensive when going across the network, so this is an attempt to improve performance.
						' We're ok skipping this check, because a few lines earlier in GetImageForDocument that existence check
						' is already made.
						' -Phil S. 07/27/2012
						Dim determinedEncodingStream As DeterminedEncodingStream = kCura.WinEDDS.Utility.DetectEncoding(filename, False, False)
						fileStream = determinedEncodingStream.UnderlyingStream

						Dim detectedEncoding As System.Text.Encoding = determinedEncodingStream.DeterminedEncoding
						If detectedEncoding IsNot Nothing Then
							chosenEncoding = detectedEncoding
						End If
					Else
						fileStream = New System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read)
					End If

					With New System.IO.StreamReader(fileStream, chosenEncoding, True)
						fullTextWriter.Write(.ReadToEnd)
						.Close()
						Try
							fileStream.Close()
						Catch
						End Try
					End With
				Next
			Else
				'no extracted text encodings, write "-1"
				_bulkLoadFileWriter.Write($"{(-1)}{lastDivider}")
			End If
				
			_bulkLoadFileWriter.Write(ServiceConstants.ENDLINETERMSTRING)
			If _replaceFullText AndAlso Not _fullTextStorageIsInSql Then
				_dataGridFileWriter.Write(ServiceConstants.ENDLINETERMSTRING)
			End If
		End Sub

		Private Function GetExtractedTextEncodings(ByVal textFileList As ArrayList) As List(Of Int32)
			Dim encodingList As New Generic.List(Of Int32)
			For Each filename As String In textFileList
				Dim chosenEncoding As System.Text.Encoding = _settings.FullTextEncoding

				If Not SkipExtractedTextEncodingCheck Then
					'We pass in 'False' as the final parameter to DetectEncoding to have it skip the File.Exists check. This
					' check can be very expensive when going across the network, so this is an attempt to improve performance.
					' We're ok skipping this check, because a few lines earlier in GetImageForDocument that existence check
					' is already made.
					' -Phil S. 07/27/2012
					Dim determinedEncodingStream As DeterminedEncodingStream = kCura.WinEDDS.Utility.DetectEncoding(filename, True, False)
					Dim detectedEncoding As System.Text.Encoding = determinedEncodingStream.DeterminedEncoding
					If detectedEncoding IsNot Nothing Then
						chosenEncoding = detectedEncoding
					End If
				End If

				If Not encodingList.Contains(chosenEncoding.CodePage) Then encodingList.Add(chosenEncoding.CodePage)
			Next
			Return encodingList
		End Function


		Private Sub AutoNumberImages(ByVal lines As List(Of Api.ImageRecord))
			If Not _autoNumberImages OrElse lines.Count <= 1 Then Exit Sub
			Dim allsame As Boolean = True
			Dim batesnumber As String = lines(0).BatesNumber
			For i As Int32 = 0 To lines.Count - 1
				allsame = allsame AndAlso batesnumber = lines(i).BatesNumber
				If Not allsame Then Exit Sub
			Next
			For i As Int32 = 1 To lines.Count - 1
				lines(i).BatesNumber = batesnumber & "_" & i.ToString.PadLeft(lines.Count.ToString.Length, "0"c)
			Next
		End Sub

		Private Sub GetImageForDocument(ByVal imageFile As String, ByVal originalFileName As String, ByVal batesNumber As String, ByVal documentIdentifier As String, ByVal order As Int32, ByRef offset As Int64, ByVal fullTextFiles As System.Collections.ArrayList, ByVal writeLineTermination As Boolean, ByVal originalLineNumber As Int32, ByVal status As Int64, ByVal isStartRecord As Boolean)
			_totalProcessed += 1
			
			Dim filename As String = If(Not originalFileName.IsNullOrEmpty(), originalFileName, imageFile.Substring(imageFile.LastIndexOf("\") + 1))
			Dim extractedTextFileName As String = imageFile.Substring(0, imageFile.LastIndexOf("."c) + 1) & "txt"
			Dim fileGuid As String = ""
			Dim fileLocation As String = imageFile
			Dim fileSize As Int64 = 0
			Const retry As Boolean = True
			_batchCount += 1
			If status = 0 Then
				If _copyFilesToRepository AndAlso ShouldImport Then
					Me.ImportFilesCount += 1
					_jobCompleteImageCount += 1
					fileGuid = Me.FileTapiBridge.AddPath(imageFile, Guid.NewGuid().ToString(), originalLineNumber)
					fileLocation = Me.FileTapiBridge.TargetPath.TrimEnd("\"c) & "\" & Me.FileTapiBridge.TargetFolderName & "\" & fileGuid
				Else
					WriteStatusLine(EventType2.Progress, $"Processing image '{batesNumber}'.", originalLineNumber)
					fileGuid = System.Guid.NewGuid.ToString
					Me.FileTapiProgressCount = Me.FileTapiProgressCount + 1
				End If

				If Me.GetFileExists(imageFile) Then
					fileSize = Me.GetFileLength(imageFile, retry)
				End If
				If _replaceFullText AndAlso Me.GetFileExists(extractedTextFileName) AndAlso Not fullTextFiles Is Nothing Then
					fullTextFiles.Add(extractedTextFileName)
				Else
					If _replaceFullText AndAlso Not Me.GetFileExists(extractedTextFileName) Then
						RaiseStatusEvent(EventType2.Warning, $"File '{extractedTextFileName}' not found.  No text updated.", CType((_totalValidated + _totalProcessed) / 2, Int64))
					End If
				End If
			End If
			If _replaceFullText AndAlso Me.GetFileExists(extractedTextFileName) AndAlso Not fullTextFiles Is Nothing Then
				offset += Me.GetFileLength(extractedTextFileName, retry)
			End If
			_bulkLoadFileWriter.Write(If(isStartRecord, "1,", "0,"))
			_bulkLoadFileWriter.Write(status & ",")
			_bulkLoadFileWriter.Write("0,")	'IsNew
			_bulkLoadFileWriter.Write("0,")	'ArtifactID
			_bulkLoadFileWriter.Write(originalLineNumber & ",")
			_bulkLoadFileWriter.Write(documentIdentifier & ",")
			_bulkLoadFileWriter.Write(batesNumber & ",")
			_bulkLoadFileWriter.Write(fileGuid & ",")
			_bulkLoadFileWriter.Write(filename & ",")
			_bulkLoadFileWriter.Write(order & ",")
			_bulkLoadFileWriter.Write(offset & ",")
			_bulkLoadFileWriter.Write(fileSize & ",")
			_bulkLoadFileWriter.Write(fileLocation & ",")
			_bulkLoadFileWriter.Write(imageFile & ",")
			_bulkLoadFileWriter.Write(",") 'kCura_Import_DataGridException
			If _replaceFullText AndAlso writeLineTermination Then
				_bulkLoadFileWriter.Write("-1,")
			End If
			If writeLineTermination Then
				_bulkLoadFileWriter.Write(ServiceConstants.ENDLINETERMSTRING)
			End If
		End Sub
#End Region

#Region "Events and Event Handling"

		Public Event FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception)
		Public Event StatusMessage(ByVal args As StatusEventArgs)
		Public Event ReportErrorEvent(ByVal row As System.Collections.IDictionary)

		Private Sub RaiseFatalError(ByVal ex As System.Exception)
			RaiseEvent FatalErrorEvent($"Error processing line: {CurrentLineNumber}", ex)
		End Sub

		Private Sub RaiseStatusEvent(ByVal et As EventType2, ByVal message As String, ByVal progressLineNumber As Int64)
			RaiseEvent StatusMessage(New StatusEventArgs(et, progressLineNumber, _recordCount, message, et = EventType2.Warning, Me.CurrentStatisticsSnapshot, Statistics))
		End Sub

		Private Sub _processObserver_CancelImport(ByVal sender As Object, ByVal e As CancellationRequestEventArgs) Handles _processContext.CancellationRequest
			If e.ProcessId.ToString = _processID.ToString Then
				_cancelledByUser = e.RequestByUser
				StopImport(_cancelledByUser)
			End If
		End Sub
		Protected Sub OnStatusMessage(args As StatusEventArgs)
			RaiseEvent StatusMessage(args)
		End Sub

		Protected Overrides Sub OnStopImport()
			If Not _imageReader Is Nothing Then
				_imageReader.Cancel()
			End If
			RaiseStatusEvent(EventType2.Progress, $"Job has been stopped by the user - {Me.TotalTransferredFilesCount} images have been transferred.", Me.TotalTransferredFilesCount)
			OnWriteStatusMessage(EventType2.Status, "Finalizing job…", TapiConstants.NoLineNumber, TapiConstants.NoLineNumber)
		End Sub

		Protected Overrides Sub OnTapiClientChanged()
			MyBase.OnTapiClientChanged()
			MyBase.PublishUploadModeChangeEvent(_settings.CopyFilesToDocumentRepository)
		End Sub

		Protected Overrides Sub OnWriteStatusMessage(ByVal eventType As EventType2, ByVal message As String)
			Me.RaiseStatusEvent(EventType2.Status, message, Me.CurrentLineNumber)
		End Sub

		Protected Overrides Sub OnWriteStatusMessage(ByVal eventType As EventType2, ByVal message As String, ByVal progressLineNumber As Int32, ByVal physicalLineNumber As Int32)
			message = GetLineMessage(message, physicalLineNumber)
			Me.RaiseStatusEvent(eventType, message, progressLineNumber)
		End Sub

		Private Sub WriteStatusLine(ByVal et As EventType2, ByVal line As String, ByVal lineNumber As Int32)
			' Avoid displaying potential negative numbers.
			Dim recordNumber As Int32 = lineNumber
			If recordNumber <> TapiConstants.NoLineNumber Then
				recordNumber = recordNumber
			End If

			' Prevent unnecessary crashes due to to ArgumentException (IE progress).
			If recordNumber < 0 Then
				recordNumber = 0
			End If

			line = GetLineMessage(line, lineNumber)
			OnStatusMessage(New StatusEventArgs(et, recordNumber, _recordCount, line, CurrentStatisticsSnapshot, Statistics))
		End Sub

		Protected Overrides Sub OnWriteFatalError(ByVal exception As Exception)
			Me.RaiseFatalError(exception)
		End Sub

		Private Sub RaiseReportError(ByVal row As System.Collections.Hashtable)
			_errorCount += 1
			If _errorMessageFileLocation = "" Then
				_errorMessageFileLocation = TempFileBuilder.GetTempFileName(TempFileConstants.ErrorsFileNameSuffix)
			End If
			Dim errorMessageFileWriter As New System.IO.StreamWriter(_errorMessageFileLocation, True, System.Text.Encoding.Default)
			If _errorCount < MaxNumberOfErrorsInGrid Then
				RaiseEvent ReportErrorEvent(row)
			ElseIf _errorCount = MaxNumberOfErrorsInGrid Then
				Dim moreToBeFoundMessage As New System.Collections.Hashtable
				moreToBeFoundMessage.Add("Message", "Maximum number of errors for display reached.  Export errors to view full list.")
				RaiseEvent ReportErrorEvent(moreToBeFoundMessage)
			End If
			errorMessageFileWriter.WriteLine(String.Format("{0},{1},{2},{3}",
														   CSVFormat(row("Line Number").ToString),
														   CSVFormat(row("DocumentID").ToString),
														   CSVFormat(row("FileID").ToString),
														   CSVFormat(row("Message").ToString)))
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

#End Region
		
		Private Sub IoWarningHandler(ByVal sender As Object, e As IoWarningEventArgs)
			Dim ioWarningEventArgs As New IoWarningEventArgs(e.Message, e.CurrentLineNumber)
			Me.PublishIoWarningEvent(ioWarningEventArgs)
		End Sub

		Private Sub ManagePrePushErrors()
			If Not _prePushErrors.Any Then Exit Sub
			InitializeErrorFiles

			Using stream As System.IO.StreamWriter = New System.IO.StreamWriter(_errorRowsFileLocation, True, System.Text.Encoding.Default)
				For Each prePushError As Tuple(Of ImageRecord, String) In _prePushErrors
					Dim record As ImageRecord = prePushError.Item1
					Dim errorMessage As String = prePushError.Item2
					Dim line As String() = new String() { record.OriginalIndex.ToString(), record.BatesNumber, record.FileName, errorMessage }
					ProcessError(line)
					Dim newRecordMarker As String = If(record.IsNewDoc, "Y", String.Empty)
					' must be the same format as returned from server 
					stream.WriteLine(String.Format("{0},{1},{2},{3},,,", record.BatesNumber, RunId, record.FileLocation, newRecordMarker))
				Next
			End Using

			_prePushErrors = New List(Of Tuple(Of ImageRecord, String))
		End Sub

		Private Sub ManageErrors()
			If Not _bulkImportManager.ImageRunHasErrors(_caseInfo.ArtifactID, _runId) Then Exit Sub
			InitializeErrorFiles 

			Dim w As System.IO.StreamWriter = Nothing
			Dim r As System.IO.StreamReader = Nothing

			Dim sr As GenericCsvReader2 = Nothing
			Try
				With _bulkImportManager.GenerateImageErrorFiles(_caseInfo.ArtifactID, _runId, True, _keyFieldDto.ArtifactID)
					Me.RaiseStatusEvent(EventType2.Status, "Retrieving errors from server", Me.CurrentLineNumber)
					Dim errorFileService As New ErrorFileService(DirectCast(_bulkImportManager.Credentials, System.Net.NetworkCredential), _caseInfo.DownloadHandlerURL, _bulkImportManager.CookieContainer)
					Dim errorsLocation As String = TempFileBuilder.GetTempFileName(TempFileConstants.ErrorsFileNameSuffix)
					sr = AttemptErrorFileDownload(errorFileService, errorsLocation, .LogKey, _caseInfo)

					If sr Is Nothing Then
						'If we're here and still have an empty response, we can at least notify
						'the user that there was an error retrieving all errors.
						' -Phil S. 08/13/2012
						Const message As String = "There was an error while attempting to retrieve the errors from the server."

						RaiseEvent FatalErrorEvent(message, New Exception(message))
					Else
						AddHandler sr.Context.IoWarningEvent, AddressOf Me.IoWarningHandler
						Dim line As String() = sr.ReadLine
						While Not line Is Nothing
							ProcessError(line)
							line = sr.ReadLine
						End While
						sr.Close()
						Dim tmp As String = TempFileBuilder.GetTempFileName(TempFileConstants.ErrorsFileNameSuffix)
						errorFileService.DownloadErrorFile(tmp, .OpticonKey, _caseInfo)
						w = New System.IO.StreamWriter(_errorRowsFileLocation, True, System.Text.Encoding.Default)
						r = New System.IO.StreamReader(tmp, System.Text.Encoding.Default)
						Dim c As Int32 = r.Read
						While Not c = -1
							w.Write(ChrW(c))
							c = r.Read
						End While
						w.Close()
						r.Close()
						Global.Relativity.DataExchange.Io.FileSystem.Instance.File.Delete(tmp)
						RemoveHandler sr.Context.IoWarningEvent, AddressOf Me.IoWarningHandler
					End If
				End With
			Catch ex As Exception

				' Be careful with these streams - they can be null if a download error occurs.
				Me.LogWarning(ex, "Failed to manage the image import errors.")

				Try
					If sr IsNot Nothing Then
						sr.Close()
					End If
					RemoveHandler sr.Context.IoWarningEvent, AddressOf Me.IoWarningHandler
				Catch ex2 As Exception
					Me.LogWarning(ex2, "Failed to close the image import CSV stream.")
				End Try
				Try
					If w IsNot Nothing Then
						w.Close()
					End If
				Catch ex2 As Exception
					Me.LogWarning(ex2, "Failed to close the image import error lines stream.")
				End Try
				Try
					If r IsNot Nothing Then
						r.Close()
					End If
				Catch ex2 As Exception
					Me.LogWarning(ex2, "Failed to close the downloaded image import errors stream.")
				End Try
			Finally
				_verboseErrorCollection.Clear()
			End Try
		End Sub

		Private Sub InitializeErrorFiles 
			If _errorMessageFileLocation = "" Then
				_errorMessageFileLocation = TempFileBuilder.GetTempFileName(TempFileConstants.ErrorsFileNameSuffix)
			End If

			If _errorRowsFileLocation = "" Then
				_errorRowsFileLocation = TempFileBuilder.GetTempFileName(TempFileConstants.ErrorsFileNameSuffix)
			End If
		End Sub

		Private Sub ProcessError(ByVal line As String())
			Dim originalIndex As Int64 = Int64.Parse(line(0))
			Dim ht As New System.Collections.Hashtable
			ht.Add("Line Number", Ctype(originalIndex,Int32))
			ht.Add("DocumentID", line(1))
			ht.Add("FileID", line(2))
			Dim errorMessages As String = line(3)
			If _verboseErrorCollection.ContainsLine(originalIndex) Then
				Dim sb As New System.Text.StringBuilder
				For Each message As String In _verboseErrorCollection(originalIndex)
					sb.Append(ImportStatusHelper.ConvertToMessageLineInCell(message))
				Next
				errorMessages = sb.ToString.TrimEnd(ChrW(10))
			End If
			ht.Add("Message", errorMessages)
			RaiseReportError(ht)
			'TODO: track stats
			Dim recordNumber As Long = originalIndex + ImportFilesCount
			RaiseEvent StatusMessage(New StatusEventArgs(EventType2.Error, recordNumber - 1, _recordCount, "[Line " & line(0) & "]" & errorMessages, Nothing, Statistics))
		End Sub
		
		Private Function AttemptErrorFileDownload(ByVal errorFileService As ErrorFileService, ByVal errorFileOutputPath As String, ByVal logKey As String, ByVal caseInfo As CaseInfo) As GenericCsvReader2
			Dim triesLeft As Integer = 3
			Dim sr As GenericCsvReader2 = Nothing

			While triesLeft > 0
				errorFileService.DownloadErrorFile(errorFileOutputPath, logKey, caseInfo, False)
				sr = New GenericCsvReader2(errorFileOutputPath, True)
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

			errorFileService.RemoveErrorFile(logKey, caseInfo)
			Return sr
		End Function

		Private Sub _processContext_ExportServerErrorsEvent(ByVal sender As Object, e As ExportErrorEventArgs) Handles _processContext.ExportServerErrors
			Dim rootFileName As String = _filePath
			Dim defaultExtension As String
			If Not rootFileName.IndexOf(".") = -1 Then
				defaultExtension = rootFileName.Substring(rootFileName.LastIndexOf("."))
				rootFileName = rootFileName.Substring(0, rootFileName.LastIndexOf("."))
			Else
				defaultExtension = ".opt"
			End If

			If rootFileName.IndexOf("\") <> -1 Then
				rootFileName = rootFileName.Substring(rootFileName.LastIndexOf("\") + 1)
			End If

			Dim rootFilePath As String = e.Path & rootFileName
			Dim datetimeNow As System.DateTime = System.DateTime.Now
			Dim errorFilePath As String = rootFilePath & "_ErrorLines_" & datetimeNow.Ticks & defaultExtension
			Dim errorReportPath As String = rootFilePath & "_ErrorReport_" & datetimeNow.Ticks & ".csv"
			Me.CopyFile(_errorRowsFileLocation, errorFilePath)
			Me.CopyFile(_errorMessageFileLocation, errorReportPath)
		End Sub

		Private Sub _processContext_ExportErrorFileEvent(ByVal sender As Object, e As ExportErrorEventArgs) Handles _processContext.ExportErrorFile
			Try
				If Me.GetFileExists(_errorRowsFileLocation) Then
					Me.CopyFile(_errorRowsFileLocation, e.Path, True)
				End If
			Catch ex As Exception
				Me.LogWarning(ex, "Failed to copy the image import error rows file. Going to retry the copy...")
				If Me.GetFileExists(_errorRowsFileLocation) Then
					Me.CopyFile(_errorRowsFileLocation, e.Path, True)
					Me.LogInformation("Successfully copied the image import error rows file on retry.")
				End If
			End Try
		End Sub

		Private Sub _processContext_ExportErrorReportEvent(ByVal sender As Object, e As ExportErrorEventArgs) Handles _processContext.ExportErrorReport
			If String.IsNullOrEmpty(_errorMessageFileLocation) Then
				' write out a blank file if there is no error message file
				Dim fileWriter As System.IO.StreamWriter = System.IO.File.CreateText(e.Path)
				fileWriter.Close()

				Exit Sub
			End If
			Try
				If Me.GetFileExists(_errorMessageFileLocation) Then
					Me.CopyFile(_errorMessageFileLocation, e.Path, True)
				End If
			Catch ex As Exception
				Me.LogWarning(ex, "Failed to copy the image import error location file. Going to retry the copy...")
				If Me.GetFileExists(_errorMessageFileLocation) Then
					Me.CopyFile(_errorMessageFileLocation, e.Path, True)
					Me.LogInformation("Successfully copied the image import error location file.")
				End If
			End Try
		End Sub

		Private Sub _processContext_ParentFormClosingEvent(ByVal sender As Object, e As ParentFormClosingEventArgs) Handles _processContext.ParentFormClosing
			If e.ProcessId.ToString = _processID.ToString Then Me.CleanupTempTables()
		End Sub

		Private Sub CleanupTempTables()
			If Not _runId Is Nothing AndAlso _runId <> "" Then
				Try
					_bulkImportManager.DisposeTempTables(_caseInfo.ArtifactID, _runId)
				Catch e As Exception
					Me.LogWarning(e, "Failed to drop the {RunId} SQL temp tables.", _runId)
				End Try
			End If
		End Sub
	End Class
End Namespace