Imports System.IO
Imports System.Collections.Generic
Imports System.Threading
Imports kCura.EDDS.WebAPI.BulkImportManagerBase
Imports kCura.Utility.Extensions.Enumerable
Imports kCura.WinEDDS.Service
Imports kCura.Utility
Imports kCura.WinEDDS.TApi
Imports Relativity

Namespace kCura.WinEDDS
	Public Class BulkImageFileImporter
		Inherits ImportExportTapiBase

#Region "Members"
		Private _imageReader As kCura.WinEDDS.Api.IImageReader
		Protected _fieldQuery As kCura.WinEDDS.Service.FieldQuery
		Protected _productionManager As kCura.WinEDDS.Service.ProductionManager
		Protected _bulkImportManager As kCura.WinEDDS.Service.BulkImportManager
		Protected _documentManager As kCura.WinEDDS.Service.DocumentManager
		Protected _relativityManager As kCura.WinEDDS.Service.RelativityManager
		Private _folderID As Int32
		Private _productionArtifactID As Int32
		Private _overwrite As Relativity.ImportOverwriteType
		Private _filePath As String
		Private _recordCount As Int64
		Private _replaceFullText As Boolean
		Private _importBatchSize As Int32?
		Private _jobCompleteBatchSize As Int32?
		Private _importBatchVolume As Int32?
		Private _minimumBatchSize As Int32?
		Private _batchSizeHistoryList As System.Collections.Generic.List(Of Int32)
		Private _autoNumberImages As Boolean
		Private _copyFilesToRepository As Boolean
		Private _defaultDestinationFolderPath As String
		Private _caseInfo As Relativity.CaseInfo
		Private _overlayArtifactID As Int32
		Private _executionSource As Relativity.ExecutionSource
		Private _lastRunMetadataImport As Int64 = 0

		Private WithEvents _processController As kCura.Windows.Process.Controller
		Protected _keyFieldDto As kCura.EDDS.WebAPI.FieldManagerBase.Field
		Protected _fullTextStorageIsInSql As Boolean = True
		Private _bulkLoadFileWriter As System.IO.StreamWriter
		Private _dataGridFileWriter As System.IO.StreamWriter
		Private _uploadKey As String = ""
		Private _uploadDataGridKey As String = ""
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
		Public Property MaxNumberOfErrorsInGrid As Int32 = Config.DefaultMaximumErrorCount
		Private _totalValidated As Long
		Private _totalProcessed As Long
		Private _startLineNumber As Int64
		Private _enforceDocumentLimit As Boolean

		Private _timekeeper As New kCura.Utility.Timekeeper
		Private _doRetryLogic As Boolean
		Private _verboseErrorCollection As New ClientSideErrorCollection
		Public Property SkipExtractedTextEncodingCheck As Boolean
		Public Property OIFileIdMapped As Boolean
		Public Property OIFileIdColumnName As String
		Public Property OIFileTypeColumnName As String
		Public Property FileNameColumn As String
#End Region

#Region "Accessors"

		Public Property DisableImageTypeValidation As Boolean = Config.DisableImageTypeValidation
		Public Property DisableImageLocationValidation As Boolean = Config.DisableImageLocationValidation
		Public Property DisableUserSecurityCheck As Boolean
		Public Property AuditLevel As kCura.EDDS.WebAPI.BulkImportManagerBase.ImportAuditLevel = Config.AuditLevel

		Public ReadOnly Property BatchSizeHistoryList As System.Collections.Generic.List(Of Int32)
			Get
				Return _batchSizeHistoryList
			End Get
		End Property

		Friend WriteOnly Property FilePath() As String
			Set(ByVal value As String)
				_filePath = value
			End Set
		End Property

		Public ReadOnly Property HasErrors() As Boolean
			Get
				Return _errorCount > 0
			End Get
		End Property

		Protected ReadOnly Property [Continue]() As Boolean
			Get
				Return _imageReader.HasMoreRecords AndAlso ShouldImport
			End Get
		End Property

		Public ReadOnly Property UploadConnection() As TApi.TapiClient
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
				If Not _minimumBatchSize.HasValue Then _minimumBatchSize = kCura.WinEDDS.Config.MinimumBatchSize
				Return _minimumBatchSize.Value
			End Get
			Set(ByVal value As Int32)
				_minimumBatchSize = value
			End Set
		End Property

		Protected Overridable Property ImportBatchSize As Int32
			Get
				If Not _importBatchSize.HasValue Then _importBatchSize = Config.ImportBatchSize
				Return _importBatchSize.Value
			End Get
			Set(ByVal value As Int32)
				_importBatchSize = If(value > MinimumBatchSize, value, MinimumBatchSize)
			End Set
		End Property

		Protected Property JobCompleteBatchSize As Int32
			Get
				If Not _jobCompleteBatchSize.HasValue Then _jobCompleteBatchSize = Config.JobCompleteBatchSize
				Return _jobCompleteBatchSize.Value
			End Get
			Set(ByVal value As Int32)
				_jobCompleteBatchSize = If(value > MinimumBatchSize, value, MinimumBatchSize)
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

		Protected Overridable ReadOnly Property BatchResizeEnabled As Boolean
			Get
				Return kCura.WinEDDS.Config.DynamicBatchResizingOn
			End Get
		End Property

#End Region

#Region "Constructors"
		Public Sub New(ByVal folderID As Int32, ByVal args As ImageLoadFile, ByVal controller As kCura.Windows.Process.Controller, ByVal ioReporterInstance As IIoReporter, 
					   ByVal logger As Logging.ILog, ByVal processID As Guid, ByVal doRetryLogic As Boolean,  ByVal enforceDocumentLimit As Boolean, ByVal tokenSource As CancellationTokenSource,
					   Optional ByVal executionSource As Relativity.ExecutionSource = Relativity.ExecutionSource.Unknown)
			MyBase.New(ioReporterInstance, logger, tokenSource)

			_executionSource = executionSource
			_enforceDocumentLimit = enforceDocumentLimit
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
			InitializeDTOs(args)
			If(args.Overwrite.IsNullOrEmpty)
				_overwrite = Relativity.ImportOverwriteType.Append
			Else 
				_overwrite = CType([Enum].Parse(GetType(Relativity.ImportOverwriteType),args.Overwrite, True), Relativity.ImportOverwriteType)
			End If
			_replaceFullText = args.ReplaceFullText
			_processController = controller
			_copyFilesToRepository = args.CopyFilesToDocumentRepository
			ShouldImport = True
			_autoNumberImages = args.AutoNumberImages
			_caseInfo = args.CaseInfo
			_settings = args
			_processID = processID
			_startLineNumber = args.StartLineNumber
			_overlayArtifactID = args.IdentityFieldId

			_batchSizeHistoryList = New System.Collections.Generic.List(Of Int32)

			If args.ReplaceFullText Then
				_fullTextStorageIsInSql = (_fieldQuery.RetrieveAllAsDocumentFieldCollection(args.CaseInfo.ArtifactID, Relativity.ArtifactType.Document).FullText.EnableDataGrid = False)
			End If

		End Sub

		Protected Overridable Sub InitializeUploaders(ByVal args As ImageLoadFile)
			Dim gateway As kCura.WinEDDS.Service.FileIO = New kCura.WinEDDS.Service.FileIO(args.Credential, args.CookieContainer)
			Dim nativeParameters As TApi.UploadTapiBridgeParameters = New TApi.UploadTapiBridgeParameters
			nativeParameters.BcpFileTransfer = False
			nativeParameters.AsperaBcpRootFolder = String.Empty

			' This will tie both native and BCP to a single unique identifier.
			nativeParameters.ClientRequestId = Guid.NewGuid()
			nativeParameters.Credentials = args.Credential
			nativeParameters.AsperaDocRootLevels = Config.TapiAsperaNativeDocRootLevels
			nativeParameters.FileShare = args.CaseInfo.DocumentPath
			nativeParameters.ForceAsperaClient = Config.TapiForceAsperaClient
			nativeParameters.ForceClientCandidates = Config.TapiForceClientCandidates
			nativeParameters.ForceFileShareClient = Config.TapiForceFileShareClient
			nativeParameters.ForceHttpClient = Config.ForceWebUpload OrElse Config.TapiForceHttpClient
			nativeParameters.LargeFileProgressEnabled = Config.TapiLargeFileProgressEnabled
			nativeParameters.LogConfigFile = Config.LogConfigFile
			nativeParameters.MaxFilesPerFolder = gateway.RepositoryVolumeMax
			nativeParameters.MaxJobParallelism = Config.TapiMaxJobParallelism
			nativeParameters.MaxJobRetryAttempts = Me.NumberOfRetries
			nativeParameters.MinDataRateMbps = Config.TapiMinDataRateMbps
			nativeParameters.TargetPath = Me._defaultDestinationFolderPath
			nativeParameters.TargetDataRateMbps = Config.TapiTargetDataRateMbps
			nativeParameters.TransferLogDirectory = Config.TapiTransferLogDirectory
			nativeParameters.WaitTimeBetweenRetryAttempts = Me.WaitTimeBetweenRetryAttempts
			nativeParameters.WebCookieContainer = args.CookieContainer
			nativeParameters.WebServiceUrl = Config.WebServiceURL
			nativeParameters.WorkspaceId = args.CaseInfo.ArtifactID
			nativeParameters.PermissionErrorsRetry = Config.PermissionErrorsRetry
			nativeParameters.BadPathErrorsRetry = Config.BadPathErrorsRetry

			' Copying the parameters and tweaking just a few BCP specific parameters.
			Dim bcpParameters As TApi.UploadTapiBridgeParameters = nativeParameters.ShallowCopy()
			bcpParameters.BcpFileTransfer = True
			bcpParameters.AsperaBcpRootFolder = Config.TapiAsperaBcpRootFolder
			bcpParameters.FileShare = gateway.GetBcpSharePath(args.CaseInfo.ArtifactID)
			bcpParameters.SortIntoVolumes = False
			bcpParameters.ForceHttpClient = bcpParameters.ForceHttpClient Or Config.TapiForceBcpHttpClient

			' Ensure that one instance is used for both TAPI objects.
			CreateTapiBridges(nativeParameters, bcpParameters)
		End Sub

		Protected Overridable Sub InitializeDTOs(ByVal args As ImageLoadFile)
			Dim fieldManager As FieldManager = New kCura.WinEDDS.Service.FieldManager(args.Credential, args.CookieContainer)

			' slm- 10/10/2011 - fixed both of these to check for ID greater than zero
			If _productionArtifactID > 0 Then
				_keyFieldDto = fieldManager.Read(args.CaseInfo.ArtifactID, args.BeginBatesFieldArtifactID)
			ElseIf args.IdentityFieldId > 0 Then
				_keyFieldDto = fieldManager.Read(args.CaseInfo.ArtifactID, args.IdentityFieldId)
			Else
				Dim fieldID As Int32 = _fieldQuery.RetrieveAllAsDocumentFieldCollection(args.CaseInfo.ArtifactID, Relativity.ArtifactType.Document).IdentifierFields(0).FieldID
				_keyFieldDto = fieldManager.Read(args.CaseInfo.ArtifactID, fieldID)
			End If
		End Sub

		Protected Overridable Sub InitializeManagers(ByVal args As ImageLoadFile)
			_fieldQuery = New kCura.WinEDDS.Service.FieldQuery(args.Credential, args.CookieContainer)
			_productionManager = New kCura.WinEDDS.Service.ProductionManager(args.Credential, args.CookieContainer)
			_bulkImportManager = New kCura.WinEDDS.Service.BulkImportManager(args.Credential, args.CookieContainer)
			_documentManager = New kCura.WinEDDS.Service.DocumentManager(args.Credential, args.CookieContainer)
			_relativityManager = New kCura.WinEDDS.Service.RelativityManager(args.Credential, args.CookieContainer)
		End Sub

#End Region

#Region "Main"

		Public Sub ReadFile()
			Me.ReadFile(_filePath)
		End Sub

		Private Sub ProcessList(ByVal al As System.Collections.Generic.List(Of Api.ImageRecord), ByRef status As Int64, ByVal bulkLoadFilePath As String, ByVal dataGridFilePath As String)
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

		Private Function BulkImport(ByVal overwrite As OverwriteType, ByVal useBulk As Boolean) As MassImportResults
			Dim retval As MassImportResults
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
		
		Protected Function GetImageRecord() As Api.ImageRecord
			Return _imageReader.GetImageRecord
		End Function

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
				CompletePendingPhysicalFileTransfers("Waiting for the image file job to complete...", "Image file job completed.", "Failed to complete all pending image file transfers.")
			End If

			Try
				If ShouldImport AndAlso _copyFilesToRepository AndAlso Me.FileTapiBridge.TransfersPending Then
					WaitForPendingFileUploads()
					Me.JobCounter += 1
				End If

				Dim start As Int64 = System.DateTime.Now.Ticks

				If ShouldImport
					PushImageBatch(bulkLoadFilePath, dataGridFilePath, shouldCompleteMetadataJob, isFinal)
				End If

				Me.Statistics.FileWaitTime += System.Math.Max((System.DateTime.Now.Ticks - start), 1)
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
			Dim newBulkLoadFilePath As String = System.IO.Path.GetTempFileName
			Dim limit As String = Relativity.Constants.ENDLINETERMSTRING
			Dim last As New System.Collections.Generic.Queue(Of Char)
			Dim recordsProcessed As Int32 = 0
			Dim charactersSuccessfullyProcessed As Int64 = 0
			Dim hasReachedEof As Boolean = False
			Dim tries As Int32 = 1 'already starts at 1 retry
			While totalRecords > recordsProcessed AndAlso Not hasReachedEof AndAlso ShouldImport
				Dim i As Int32 = 0
				Dim charactersProcessed As Int64 = 0
				Using sr As TextReader = CreateStreamReader(oldBulkLoadFilePath), sw As TextWriter = CreateStreamWriter(newBulkLoadFilePath)
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
						kCura.Utility.File.Instance.Delete(newBulkLoadFilePath)
						Throw
					End If
				End Try
			End While
			kCura.Utility.File.Instance.Delete(newBulkLoadFilePath)
		End Sub

		Protected Overridable Function DoLogicAndPushImageBatch(ByVal totalRecords As Integer, ByVal recordsProcessed As Integer, ByVal bulkLocation As String, ByVal dataGridLocation As String, ByRef charactersSuccessfullyProcessed As Long, ByVal i As Integer, ByVal charactersProcessed As Long) As Integer
			_batchCount = i
			RaiseStatusEvent(kCura.Windows.Process.EventType.Warning, "Begin processing sub-batch of size " & i & ".", CType((_totalValidated + _totalProcessed) / 2, Int64), Me.CurrentLineNumber)
			Me.PushImageBatch(bulkLocation, dataGridLocation, False, True)
			RaiseStatusEvent(kCura.Windows.Process.EventType.Warning, "End processing sub-batch of size " & i & ".  " & recordsProcessed & " of " & totalRecords & " in the original batch processed", CType((_totalValidated + _totalProcessed) / 2, Int64), Me.CurrentLineNumber)
			recordsProcessed += i
			charactersSuccessfullyProcessed += charactersProcessed
			Return recordsProcessed
		End Function

		Private Sub DeleteFiles(ByVal bulkFilePath As String, ByVal datagridFilePath As String)
			kCura.Utility.File.Instance.Delete(bulkFilePath)
			kCura.Utility.File.Instance.Delete(datagridFilePath)
		End Sub

		Protected Overridable Function CreateStreamWriter(ByVal tmpLocation As String) As System.IO.TextWriter
			Return New System.IO.StreamWriter(tmpLocation, False, System.Text.Encoding.Unicode)
		End Function

		Protected Overridable Function CreateStreamReader(ByVal outputPath As String) As TextReader
			Return New System.IO.StreamReader(outputPath, System.Text.Encoding.Unicode)
		End Function

		Private Sub AdvanceStream(ByVal sr As System.IO.TextReader, ByVal count As Int64)
			Dim i As Int32
			If count > 0 Then
				For j As Int64 = 0 To count - 1
					i = sr.Read()
				Next
			End If
		End Sub

		Public Sub PushImageBatch(ByVal bulkLoadFilePath As String, ByVal dataGridFilePath As String, ByVal shouldCompleteJob As Boolean, ByVal lastRun As Boolean)
			If _lastRunMetadataImport > 0 Then
				Me.Statistics.MetadataWaitTime += System.DateTime.Now.Ticks - _lastRunMetadataImport
			End If

			If _batchCount = 0 Then
				If _jobCompleteMetadataCount > 0 Then
					_jobCompleteMetadataCount = 0
					CompletePendingBulkLoadFileTransfers()
				End If
				Return
			End If

			If shouldCompleteJob And _jobCompleteMetadataCount > 0 Then
				_jobCompleteMetadataCount = 0
				CompletePendingBulkLoadFileTransfers()
			End If

			_batchCount = 0
			Me.Statistics.MetadataBytes += (IoReporterInstance.GetFileLength(bulkLoadFilePath, Me.CurrentLineNumber) + IoReporterInstance.GetFileLength(dataGridFilePath, Me.CurrentLineNumber))

			try
				_uploadKey = Me.BulkLoadTapiBridge.AddPath(bulkLoadFilePath, Guid.NewGuid().ToString(), 1)
				_uploadDataGridKey = Me.BulkLoadTapiBridge.AddPath(dataGridFilePath, Guid.NewGuid().ToString(), 2)

				' keep track of the total count of added files
				MetadataFilesCount += 2
				_jobCompleteMetadataCount += 2

				If lastRun Then
					CompletePendingBulkLoadFileTransfers()
				Else
					WaitForPendingMetadataUploads()
				End If
			Catch ex As Exception
				' Note: Retry and potential HTTP fallback automatically kick in. Throwing a similar exception if a failure occurs.
				Throw New kCura.WinEDDS.LoadFilebase.BcpPathAccessException("Error accessing BCP Path, could be caused by network connectivity issues: " & ex.Message)
			End Try

			_lastRunMetadataImport = System.DateTime.Now.Ticks

			Dim overwrite As kCura.EDDS.WebAPI.BulkImportManagerBase.OverwriteType
			Select Case _overwrite
				Case Relativity.ImportOverwriteType.AppendOverlay
					overwrite = EDDS.WebAPI.BulkImportManagerBase.OverwriteType.Both
				Case Relativity.ImportOverwriteType.Overlay
					overwrite = EDDS.WebAPI.BulkImportManagerBase.OverwriteType.Overlay
				Case Else
					overwrite = EDDS.WebAPI.BulkImportManagerBase.OverwriteType.Append
			End Select

			If ShouldImport Then
				Dim start As Int64 = System.DateTime.Now.Ticks
				Dim runResults As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults = Me.RunBulkImport(overwrite, True)
				Me.Statistics.ProcessRunResults(runResults)
				_runId = runResults.RunID
				Me.Statistics.SqlTime += System.Math.Max(System.DateTime.Now.Ticks - start, 1)
				PublishUploadModeEvent()
				ManageErrors()

				Me.TotalTransferredFilesCount = Me.FileTapiProgressCount
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
			Dim bulkLoadFilePath As String = System.IO.Path.GetTempFileName
			Dim dataGridFilePath As String = System.IO.Path.GetTempFileName
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
				_recordCount = _imageReader.CountRecords

				If (_enforceDocumentLimit AndAlso _overwrite = Relativity.ImportOverwriteType.Append) Then

					Me.LogInformation("Preparing to determine the number of images to import...")
					Dim tempImageReader As OpticonFileReader = New OpticonFileReader(_folderID, _settings, Nothing, Nothing, _doRetryLogic)
					tempImageReader.Initialize()
					Dim newDocCount As Int32 = 0

					While tempImageReader.HasMoreRecords AndAlso tempImageReader.CurrentRecordNumber < _startLineNumber
						tempImageReader.AdvanceRecord()
					End While

					While tempImageReader.HasMoreRecords

						Dim record As Api.ImageRecord = tempImageReader.GetImageRecord
						If record.IsNewDoc Then
							newDocCount += 1
						End If

					End While
					tempImageReader.Close()

					Dim currentDocCount As Int32 = _documentManager.RetrieveDocumentCount(_caseInfo.ArtifactID)
					Dim docLimit As Int32 = _documentManager.RetrieveDocumentLimit(_caseInfo.ArtifactID)


					Dim countAfterJob As Long = currentDocCount + newDocCount
					Me.LogInformation("Successfully calculated the number of images to import: {ImageCount}, Doc limit: {DocLimit}", countAfterJob, docLimit)
					If (docLimit <> 0 And countAfterJob > docLimit) Then
						Dim errorMessage As String = $"The document import was canceled.  It would have exceeded the workspace's document limit of {docLimit} by {(countAfterJob - docLimit)} documents."
						Throw New Exception(errorMessage)
					End If
				End If

				RaiseStatusEvent(kCura.Windows.Process.EventType.Progress, "Begin Image Upload", 0, 0)
				RaiseStatusEvent(kCura.Windows.Process.EventType.ResetStartTime, "", 0, 0)
				Dim al As New System.Collections.Generic.List(Of Api.ImageRecord)
				Dim status As Int64 = 0
				_timekeeper.MarkEnd("ReadFile_Init")

				_timekeeper.MarkStart("ReadFile_Main")
				
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
						RaiseStatusEvent(Windows.Process.EventType.Count, String.Empty, 0, 0)
						Dim record As Api.ImageRecord = _imageReader.GetImageRecord
						record.OriginalIndex = _imageReader.CurrentRecordNumber
						If (record.IsNewDoc) Then
							Me.ProcessList(al, status, bulkLoadFilePath, dataGridFilePath)
						End If
						status = status Or Me.ProcessImageLine(record)
						al.Add(record)
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
				Me.DumpStatisticsInfo()
				Me.CompleteSuccess()
				_timekeeper.MarkEnd("ReadFile_Cleanup")
				_timekeeper.MarkEnd("TOTAL")
				_timekeeper.GenerateCsvReportItemsAsRows("_winedds_image", "C:\")
			Catch ex As Exception
				Me.LogFatal(ex, "A serious unexpected error has occurred importing images.")
				Me.DumpStatisticsInfo()
				Me.CompleteError(ex)
			Finally
				_timekeeper.MarkStart("ReadFile_CleanupTempTables")
				DestroyTapiBridges()
				CleanupTempTables()
				_timekeeper.MarkEnd("ReadFile_CleanupTempTables")
			End Try
		End Sub

		Public Event EndRun(ByVal success As Boolean, ByVal runID As String)

		Private Sub CompleteSuccess()
			If Not _imageReader Is Nothing Then _imageReader.Close()
			If _productionArtifactID <> 0 Then _productionManager.DoPostImportProcessing(Me.FileTapiBridge.WorkspaceId, _productionArtifactID)
			Try
				RaiseEvent EndRun(True, _runId)
			Catch
			End Try
			If CancellationToken.IsCancellationRequested Then
				OnWriteStatusMessage(kCura.Windows.Process.EventType.Status, "Job has been finalized", TapiConstants.NoLineNumber, TapiConstants.NoLineNumber)
			Else
				OnWriteStatusMessage(kCura.Windows.Process.EventType.Status, "End Image Upload")
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
			Try
				RaiseEvent EndRun(False, _runId)
			Catch ex As Exception
				Me.LogWarning(ex, "Failed to raise the EndRun event before raising the image import fatal error.")
			End Try
			RaiseFatalError(exception)
		End Sub

		Private Sub ProcessDocument(ByVal al As System.Collections.Generic.List(Of Api.ImageRecord), ByVal status As Int64)
			GetImagesForDocument(al, status)
			Me.Statistics.DocCount += 1
		End Sub

#End Region

#Region "Worker Methods"

		Public Function ProcessImageLine(ByVal imageRecord As Api.ImageRecord) As Relativity.MassImport.ImportStatus
			_totalValidated += 1
			Dim retval As Relativity.MassImport.ImportStatus = Relativity.MassImport.ImportStatus.Pending
			'check for existence
			If imageRecord.BatesNumber.Trim = "" Then
				Me.RaiseStatusEvent(Windows.Process.EventType.Error, "No image file or identifier specified on line.", CType((_totalValidated + _totalProcessed) / 2, Int64), Me.CurrentLineNumber)
				retval = Relativity.MassImport.ImportStatus.NoImageSpecifiedOnLine
			ElseIf Not Me.DisableImageLocationValidation AndAlso Not System.IO.File.Exists(BulkImageFileImporter.GetFileLocation(imageRecord)) Then
				Me.RaiseStatusEvent(Windows.Process.EventType.Error, $"Image file specified ( {imageRecord.FileLocation} ) does not exist.", CType((_totalValidated + _totalProcessed) / 2, Int64), Me.CurrentLineNumber)
				retval = Relativity.MassImport.ImportStatus.FileSpecifiedDne
			Else
				Dim validator As New kCura.ImageValidator.ImageValidator
				Dim path As String = BulkImageFileImporter.GetFileLocation(imageRecord)
				Try
					If Not Me.DisableImageTypeValidation Then
						validator.ValidateImage(path)
					End If

					Me.RaiseStatusEvent(Windows.Process.EventType.Status, $"Image file ( {imageRecord.FileLocation} ) validated.", CType((_totalValidated + _totalProcessed) / 2, Int64), Me.CurrentLineNumber)
				Catch ex As Exception
					If TypeOf ex Is kCura.ImageValidator.Exception.Base Then
						Me.LogError(ex, "Failed to validate the {Path} image.", path)
						retval = Relativity.MassImport.ImportStatus.InvalidImageFormat
						_verboseErrorCollection.AddError(imageRecord.OriginalIndex, ex)
					Else
						Me.LogFatal(ex, "Unexpected failure to validate the {Path} image file.", path)
						Throw
					End If
				End Try
			End If
			Return retval
		End Function

		Public Shared Function GetFileLocation(ByVal record As Api.ImageRecord) As String
			Dim fileLocation As String = record.FileLocation
			If fileLocation <> "" AndAlso fileLocation.Chars(0) = "\" AndAlso fileLocation.Chars(1) <> "\" Then
				fileLocation = "." & fileLocation
			End If
			Return fileLocation
		End Function


		Private Sub GetImagesForDocument(ByVal lines As System.Collections.Generic.List(Of Api.ImageRecord), ByVal status As Int64)
			Me.AutoNumberImages(lines)
			Dim hasFileIdentifierProblem As Boolean = False
			For Each line As Api.ImageRecord In lines
				If _fileIdentifierLookup.ContainsKey(line.BatesNumber.Trim) Then
					hasFileIdentifierProblem = True
				Else
					_fileIdentifierLookup.Add(line.BatesNumber.Trim, line.BatesNumber.Trim)
				End If
			Next
			If hasFileIdentifierProblem Then status += Relativity.MassImport.ImportStatus.IdentifierOverlap

			Dim record As Api.ImageRecord = lines(0)
			Dim textFileList As New System.Collections.ArrayList
			Dim documentId As String = record.BatesNumber
			Dim offset As Int64 = 0
			For i As Int32 = 0 To lines.Count - 1
				If Not ShouldImport Then 
					Exit For
				End If
				record = lines(i)
				Me.GetImageForDocument(BulkImageFileImporter.GetFileLocation(record), record.BatesNumber, documentId, i, offset, textFileList, i < lines.Count - 1, Convert.ToInt32(record.OriginalIndex), status, lines.Count, i = 0)
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
					_bulkLoadFileWriter.Write("{0}{1}", Me.GetextractedTextEncodings(textFileList).ToDelimitedString("|"), lastDivider)
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
						fileStream = New FileStream(filename, FileMode.Open, FileAccess.Read)
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
				
			_bulkLoadFileWriter.Write(Relativity.Constants.ENDLINETERMSTRING)
			If _replaceFullText AndAlso Not _fullTextStorageIsInSql Then
				_dataGridFileWriter.Write(Relativity.Constants.ENDLINETERMSTRING)
			End If
		End Sub

		Private Function GetextractedTextEncodings(ByVal textFileList As System.Collections.ArrayList) As Generic.List(Of Int32)
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


		Private Sub AutoNumberImages(ByVal lines As System.Collections.Generic.List(Of Api.ImageRecord))
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

		Private Sub GetImageForDocument(ByVal imageFile As String, ByVal batesNumber As String, ByVal documentIdentifier As String, ByVal order As Int32, ByRef offset As Int64, ByVal fullTextFiles As System.Collections.ArrayList, ByVal writeLineTermination As Boolean, ByVal originalLineNumber As Int32, ByVal status As Int64, ByVal totalForDocument As Int32, ByVal isStartRecord As Boolean)
			_totalProcessed += 1
			Dim filename As String = imageFile.Substring(imageFile.LastIndexOf("\") + 1)
			Dim extractedTextFileName As String = imageFile.Substring(0, imageFile.LastIndexOf("."c) + 1) & "txt"
			Dim fileGuid As String = ""
			Dim fileLocation As String = imageFile
			Dim fileSize As Int64 = 0
			_batchCount += 1
			If status = 0 Then
				If _copyFilesToRepository AndAlso ShouldImport Then
					Me.ImportFilesCount += 1
					_jobCompleteImageCount += 1
					fileGuid = Me.FileTapiBridge.AddPath(imageFile, Guid.NewGuid().ToString(), originalLineNumber)
					fileLocation = Me.FileTapiBridge.TargetPath.TrimEnd("\"c) & "\" & Me.FileTapiBridge.TargetFolderName & "\" & fileGuid
				Else
					WriteStatusLine(Windows.Process.EventType.Progress, $"Processing image '{batesNumber}'.", originalLineNumber)
					fileGuid = System.Guid.NewGuid.ToString
					Me.FileTapiProgressCount = Me.FileTapiProgressCount + 1
				End If

				If System.IO.File.Exists(imageFile) Then
					fileSize = IoReporterInstance.GetFileLength(imageFile, Me.CurrentLineNumber)
				End If
				If _replaceFullText AndAlso System.IO.File.Exists(extractedTextFileName) AndAlso Not fullTextFiles Is Nothing Then
					fullTextFiles.Add(extractedTextFileName)
				Else
					If _replaceFullText AndAlso Not System.IO.File.Exists(extractedTextFileName) Then
						RaiseStatusEvent(kCura.Windows.Process.EventType.Warning, $"File '{extractedTextFileName}' not found.  No text updated.", CType((_totalValidated + _totalProcessed) / 2, Int64), originalLineNumber)
					End If
				End If
			End If
			If _replaceFullText AndAlso System.IO.File.Exists(extractedTextFileName) AndAlso Not fullTextFiles Is Nothing Then
				offset += IoReporterInstance.GetFileLength(extractedTextFileName, Me.CurrentLineNumber)
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
				_bulkLoadFileWriter.Write(Relativity.Constants.ENDLINETERMSTRING)
			End If
		End Sub
#End Region

#Region "Events and Event Handling"

		Public Event FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception)
		Public Event StatusMessage(ByVal args As StatusEventArgs)
		Public Event ReportErrorEvent(ByVal row As System.Collections.IDictionary)

		Private Sub PublishUploadModeEvent()
			Dim retval As New List(Of String)
			If Not Me.BulkLoadTapiBridge Is Nothing Then
				retval.Add("Metadata: " & Me.BulkLoadTapiClientName)
			End If

			If _settings.CopyFilesToDocumentRepository Then
				If Not String.IsNullOrEmpty(Me.FileTapiClientName) Then
					retval.Add("Files: " & Me.FileTapiClientName)
				End If
			Else
				retval.Add("Files: not copied")
			End If
			If retval.Any() Then
				Dim uploadStatus As String = String.Join(" - ", retval.ToArray())

				' Note: single vs. bulk mode is a vestige. Bulk mode is always true.
				OnUploadModeChangeEvent(uploadStatus, true)
			End If
		End Sub

		Private Sub RaiseFatalError(ByVal ex As System.Exception)
			RaiseEvent FatalErrorEvent($"Error processing line: {CurrentLineNumber}", ex)
		End Sub

		Private Sub RaiseStatusEvent(ByVal et As kCura.Windows.Process.EventType, ByVal message As String, ByVal progressLineNumber As Int64, ByVal physicalLineNumber As Int64)
			RaiseEvent StatusMessage(New StatusEventArgs(et, progressLineNumber, _recordCount, message, et = Windows.Process.EventType.Warning, Me.CurrentStatisticsSnapshot, Statistics))
		End Sub

		Private Sub _processObserver_CancelImport(ByVal processID As System.Guid) Handles _processController.HaltProcessEvent
			If processID.ToString = _processID.ToString Then
				StopImport()
			End If
		End Sub
		Protected Sub OnStatusMessage(args As StatusEventArgs)
			RaiseEvent StatusMessage(args)
		End Sub

		Protected Overrides Sub OnStopImport()
			If Not _imageReader Is Nothing Then
				_imageReader.Cancel()
			End If
			RaiseStatusEvent(kCura.Windows.Process.EventType.Progress, $"Job has been stopped by the user - {Me.TotalTransferredFilesCount} images have been transferred.", Me.TotalTransferredFilesCount, Me.CurrentLineNumber)
			OnWriteStatusMessage(kCura.Windows.Process.EventType.Status, "Finalizing job…", TapiConstants.NoLineNumber, TapiConstants.NoLineNumber)
		End Sub

		Protected Overrides Sub OnTapiClientChanged()
			Me.PublishUploadModeEvent()
		End Sub

		Protected Overrides Sub OnWriteStatusMessage(ByVal eventType As kCura.Windows.Process.EventType, ByVal message As String)
			Me.RaiseStatusEvent(kCura.Windows.Process.EventType.Status, message, Me.CurrentLineNumber, Me.CurrentLineNumber)
		End Sub

		Protected Overrides Sub OnWriteStatusMessage(ByVal eventType As kCura.Windows.Process.EventType, ByVal message As String, ByVal progressLineNumber As Int32, ByVal physicalLineNumber As Int32)
			message = GetLineMessage(message, physicalLineNumber)
			Me.RaiseStatusEvent(eventType, message, progressLineNumber, physicalLineNumber)
		End Sub

		Private Sub WriteStatusLine(ByVal et As Windows.Process.EventType, ByVal line As String, ByVal lineNumber As Int32)
			' Avoid displaying potential negative numbers.
			Dim recordNumber As Int32 = lineNumber
			If recordNumber <> TApi.TapiConstants.NoLineNumber Then
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

		Private Sub RaiseReportError(ByVal row As System.Collections.Hashtable, ByVal lineNumber As Int32, ByVal identifier As String, ByVal type As String)
			_errorCount += 1
			If _errorMessageFileLocation = "" Then _errorMessageFileLocation = System.IO.Path.GetTempFileName
			Dim errorMessageFileWriter As New System.IO.StreamWriter(_errorMessageFileLocation, True, System.Text.Encoding.Default)
			If _errorCount < MaxNumberOfErrorsInGrid Then
				RaiseEvent ReportErrorEvent(row)
			ElseIf _errorCount = MaxNumberOfErrorsInGrid Then
				Dim moretobefoundMessage As New System.Collections.Hashtable
				moretobefoundMessage.Add("Message", "Maximum number of errors for display reached.  Export errors to view full list.")
				RaiseEvent ReportErrorEvent(moretobefoundMessage)
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

#Region "Exceptions - Errors"
		Public Class FileLoadException
			Inherits ImporterExceptionBase
			Public Sub New()
				MyBase.New("Error uploading file.  Skipping line.")
			End Sub
		End Class

		Public Class CreateDocumentException
			Inherits ImporterExceptionBase
			Public Sub New(ByVal parentException As System.Exception)
				MyBase.New("Error creating new document.  Skipping line: " & parentException.Message, parentException)
			End Sub
		End Class

		Public Class OverwriteNoneException
			Inherits ImporterExceptionBase
			Public Sub New(ByVal docIdentifier As String)
				MyBase.New($"Document '{docIdentifier}' exists - upload aborted.")
			End Sub
		End Class

		Public Class OverwriteStrictException
			Inherits ImporterExceptionBase
			Public Sub New(ByVal docIdentifier As String)
				MyBase.New($"Document '{docIdentifier}' does not exist - upload aborted.")
			End Sub
		End Class

		Public Class ImageCountMismatchException
			Inherits ImporterExceptionBase
			Public Sub New()
				MyBase.New("Production and Document image counts don't match - upload aborted.")
			End Sub
		End Class

		Public Class DocumentInProductionException
			Inherits ImporterExceptionBase
			Public Sub New()
				MyBase.New("Document is already in specified production - upload aborted.")
			End Sub
		End Class

		Public Class ProductionOverwriteException
			Inherits ImporterExceptionBase
			Public Sub New(ByVal identifier As String)
				MyBase.New($"Document '{identifier}' belongs to one or more productions.  Document skipped.")
			End Sub
		End Class

		Public Class RedactionOverwriteException
			Inherits ImporterExceptionBase
			Public Sub New(ByVal identifier As String)
				MyBase.New($"The one or more images for document '{identifier}' have redactions.  Document skipped.")
			End Sub
		End Class

		Public Class InvalidIdentifierKeyException
			Inherits ImporterExceptionBase
			Public Sub New(ByVal identifier As String, ByVal fieldName As String)
				MyBase.New($"More than one document contains '{identifier}' as its '{fieldName}' value.  Document skipped.")
			End Sub
		End Class


#End Region

#Region "Exceptions - Fatal"
		Public Class InvalidBatesFormatException
			Inherits System.Exception
			Public Sub New(ByVal batesNumber As String, ByVal productionName As String, ByVal batesPrefix As String, ByVal batesSuffix As String, ByVal batesFormat As String)
				MyBase.New(
					$"The image with production number {batesNumber} cannot be imported into production '{productionName _
					          }' because the prefix and/or suffix do not match the values specified in the production. Expected prefix: '{ _
					          batesPrefix}'. Expected suffix: '{batesSuffix}'. Expected format: '{batesFormat}'.")
			End Sub
		End Class

#End Region
		
		Private Sub IoWarningHandler(ByVal e As RobustIoReporter.IoWarningEventArgs)
			Dim ioWarningEventArgs As New IoWarningEventArgs(e.Message, e.CurrentLineNumber)
			IoReporterInstance.IOWarningPublisher?.PublishIoWarningEvent(ioWarningEventArgs)
		End Sub

		Private Sub ManageErrors()
			If Not _bulkImportManager.ImageRunHasErrors(_caseInfo.ArtifactID, _runId) Then Exit Sub
			If _errorMessageFileLocation = "" Then _errorMessageFileLocation = System.IO.Path.GetTempFileName
			If _errorRowsFileLocation = "" Then _errorRowsFileLocation = System.IO.Path.GetTempFileName
			Dim w As System.IO.StreamWriter = Nothing
			Dim r As System.IO.StreamReader = Nothing

			Dim sr As kCura.Utility.GenericCsvReader = Nothing
			Try
				With _bulkImportManager.GenerateImageErrorFiles(_caseInfo.ArtifactID, _runId, True, _keyFieldDto.ArtifactID)
					Me.RaiseStatusEvent(Windows.Process.EventType.Status, "Retrieving errors from server", Me.CurrentLineNumber, Me.CurrentLineNumber)
					Dim downloader As New FileDownloader(DirectCast(_bulkImportManager.Credentials, System.Net.NetworkCredential), _caseInfo.DocumentPath, _caseInfo.DownloadHandlerURL, _bulkImportManager.CookieContainer)
					Dim errorsLocation As String = System.IO.Path.GetTempFileName
					sr = AttemptErrorFileDownload(downloader, errorsLocation, .LogKey, _caseInfo)

					If sr Is Nothing Then
						'If we're here and still have an empty response, we can at least notify
						'the user that there was an error retrieving all errors.
						' -Phil S. 08/13/2012
						Const message As String = "There was an error while attempting to retrieve the errors from the server."

						RaiseEvent FatalErrorEvent(message, New Exception(message))
					Else
						AddHandler sr.IoWarningEvent, AddressOf Me.IoWarningHandler
						Dim line As String() = sr.ReadLine
						While Not line Is Nothing
							_errorCount += 1
							Dim originalIndex As Int64 = Int64.Parse(line(0))
							Dim ht As New System.Collections.Hashtable
							ht.Add("Line Number", Int32.Parse(line(0)))
							ht.Add("DocumentID", line(1))
							ht.Add("FileID", line(2))
							Dim errorMessages As String = line(3)
							If _verboseErrorCollection.ContainsLine(originalIndex) Then
								Dim sb As New System.Text.StringBuilder
								For Each message As String In _verboseErrorCollection(originalIndex)
									sb.Append(Relativity.MassImport.ImportStatusHelper.ConvertToMessageLineInCell(message))
								Next
								errorMessages = sb.ToString.TrimEnd(ChrW(10))
							End If
							ht.Add("Message", errorMessages)
							RaiseReportError(ht, Int32.Parse(line(0)), line(2), "server")
							'TODO: track stats
							RaiseEvent StatusMessage(New StatusEventArgs(Windows.Process.EventType.Error, Int32.Parse(line(0)) - 1, _recordCount, "[Line " & line(0) & "]" & errorMessages, Nothing, Statistics))
							line = sr.ReadLine
						End While
						sr.Close()
						Dim tmp As String = System.IO.Path.GetTempFileName
						downloader.MoveTempFileToLocal(tmp, .OpticonKey, _caseInfo)
						w = New System.IO.StreamWriter(_errorRowsFileLocation, True, System.Text.Encoding.Default)
						r = New System.IO.StreamReader(tmp, System.Text.Encoding.Default)
						Dim c As Int32 = r.Read
						While Not c = -1
							w.Write(ChrW(c))
							c = r.Read
						End While
						w.Close()
						r.Close()
						kCura.Utility.File.Instance.Delete(tmp)
						RemoveHandler sr.IoWarningEvent, AddressOf Me.IoWarningHandler
					End If
				End With
			Catch ex As Exception

				' Be careful with these streams - they can be null if a download error occurs.
				Me.LogWarning(ex, "Failed to manage the image import errors.")

				Try
					If sr IsNot Nothing Then
						sr.Close()
					End If
					RemoveHandler sr.IoWarningEvent, AddressOf Me.IoWarningHandler
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

		Private Sub _processController_ExportServerErrorsEvent(ByVal exportLocation As String) Handles _processController.ExportServerErrorsEvent
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
			System.IO.File.Copy(_errorRowsFileLocation, errorFilePath)
			System.IO.File.Copy(_errorMessageFileLocation, errorReportPath)
		End Sub

		Private Sub _processController_ExportErrorFileEvent(ByVal exportLocation As String) Handles _processController.ExportErrorFileEvent
			If _errorRowsFileLocation Is Nothing Then
				Exit Sub
			End If

			Try
				If System.IO.File.Exists(_errorRowsFileLocation) Then
					System.IO.File.Copy(_errorRowsFileLocation, exportLocation, True)
				End If
			Catch ex As Exception
				Me.LogWarning(ex, "Failed to copy the image import error rows file. Going to retry the copy...")
				If System.IO.File.Exists(_errorRowsFileLocation) Then
					System.IO.File.Copy(_errorRowsFileLocation, exportLocation, True)
					Me.LogInformation("Successfully copied the image import error rows file on retry.")
				End If
			End Try
		End Sub

		Private Sub _processController_ExportErrorReportEvent(ByVal exportLocation As String) Handles _processController.ExportErrorReportEvent
			If String.IsNullOrEmpty(_errorMessageFileLocation) Then
				' write out a blank file if there is no error message file
				Dim fileWriter As StreamWriter = System.IO.File.CreateText(exportLocation)
				fileWriter.Close()

				Exit Sub
			End If
			Try
				If System.IO.File.Exists(_errorMessageFileLocation) Then
					System.IO.File.Copy(_errorMessageFileLocation, exportLocation, True)
				End If
			Catch ex As Exception
				Me.LogWarning(ex, "Failed to copy the image import error location file. Going to retry the copy...")
				If System.IO.File.Exists(_errorMessageFileLocation) Then
					System.IO.File.Copy(_errorMessageFileLocation, exportLocation, True)
					Me.LogInformation("Successfully copied the image import error location file.")
				End If
			End Try
		End Sub

		Private Sub _processController_ParentFormClosingEvent(ByVal processID As Guid) Handles _processController.ParentFormClosingEvent
			If processID.ToString = _processID.ToString Then Me.CleanupTempTables()
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