Imports System.IO
Imports System.Collections.Generic
Imports kCura.EDDS.WebAPI.BulkImportManagerBase
Imports kCura.Utility.Extensions.CollectionExtension
Imports kCura.WinEDDS.Service
Imports kCura.Utility
Imports Relativity

Namespace kCura.WinEDDS
	Public Class BulkImageFileImporter
		Inherits kCura.Utility.RobustIoReporter

#Region "Members"
		Private _imageReader As kCura.WinEDDS.Api.IImageReader
		Protected _fieldQuery As kCura.WinEDDS.Service.FieldQuery
		Private WithEvents _fileUploader As kCura.WinEDDS.FileUploader
		Private WithEvents _bcpuploader As kCura.WinEDDS.FileUploader
		Protected _productionManager As kCura.WinEDDS.Service.ProductionManager
		Protected _bulkImportManager As kCura.WinEDDS.Service.BulkImportManager
		Private _folderID As Int32
		Private _productionArtifactID As Int32
		Private _overwrite As String
		Private _filePath As String
		Private _fileLineCount As Int64
		Private _continue As Boolean
		Private _replaceFullText As Boolean
		Private _importBatchSize As Int32?
		Private _importBatchVolume As Int32?
		Private _minimumBatchSize As Int32?
		Private _batchSizeHistoryList As System.Collections.Generic.List(Of Int32)
		Private _autoNumberImages As Boolean
		Private _copyFilesToRepository As Boolean
		Private _repositoryPath As String
		Private _caseInfo As Relativity.CaseInfo
		Private _overlayArtifactID As Int32

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
		Private _errorCount As Int32 = 0
		Private _errorMessageFileLocation As String = ""
		Private _errorRowsFileLocation As String = ""
		Private _fileIdentifierLookup As System.Collections.Hashtable

		Private _processID As Guid
		Public Property MaxNumberOfErrorsInGrid As Int32 = Config.DefaultMaximumErrorCount
		Private _totalValidated As Long
		Private _totalProcessed As Long
		Private _startLineNumber As Int64

		Private _statistics As New kCura.WinEDDS.Statistics
		Private _currentStatisticsSnapshot As IDictionary
		Private _snapshotLastModifiedOn As System.DateTime = System.DateTime.Now
		Private _timekeeper As New kCura.Utility.Timekeeper
		Private _doRetryLogic As Boolean
		Private _verboseErrorCollection As New ClientSideErrorCollection
		Public Property SkipExtractedTextEncodingCheck As Boolean
		Public Property OIFileIdMapped As Boolean
		Public Property OIFileIdColumnName As String
		Public Property OIFileTypeColumnName As String
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
				Return _imageReader.HasMoreRecords AndAlso _continue
			End Get
		End Property

		Public ReadOnly Property Statistics() As kCura.WinEDDS.Statistics
			Get
				Return _statistics
			End Get
		End Property

		Public ReadOnly Property UploadConnection() As FileUploader.Type
			Get
				Return _fileUploader.UploaderType
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
		Public Sub New(ByVal folderID As Int32, ByVal args As ImageLoadFile, ByVal controller As kCura.Windows.Process.Controller, ByVal processID As Guid, ByVal doRetryLogic As Boolean)
			MyBase.New()

			_doRetryLogic = doRetryLogic
			InitializeManagers(args)
			Dim suffix As String = "\EDDS" & args.CaseInfo.ArtifactID & "\"
			If args.SelectedCasePath = "" Then
				_repositoryPath = args.CaseDefaultPath.TrimEnd("\"c) & suffix
			Else
				_repositoryPath = args.SelectedCasePath.TrimEnd("\"c) & suffix
			End If
			Dim lastHalfPath As String = "EDDS" & args.CaseInfo.ArtifactID & "\"
			'_textRepositoryPath = Path.Combine(args.CaseDefaultPath, lastHalfPath)
			InitializeUploaders(args)
			_folderID = folderID
			_productionArtifactID = args.ProductionArtifactID
			InitializeDTOs(args)
			_overwrite = args.Overwrite
			_replaceFullText = args.ReplaceFullText
			_processController = controller
			_copyFilesToRepository = args.CopyFilesToDocumentRepository
			_continue = True
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
			_fileUploader = New kCura.WinEDDS.FileUploader(args.Credential, args.CaseInfo.ArtifactID, _repositoryPath, args.CookieContainer)
			_bcpuploader = New kCura.WinEDDS.FileUploader(args.Credential, args.CaseInfo.ArtifactID, _repositoryPath, args.CookieContainer, False)
			_bcpuploader.SetUploaderTypeForBcp()
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
		End Sub

#End Region

#Region "Main"

		Public Sub ReadFile()
			Me.ReadFile(_filePath)
		End Sub

		Private Sub ProcessList(ByVal al As System.Collections.Generic.List(Of Api.ImageRecord), ByRef status As Int64, ByVal bulkLoadFilePath As String, ByVal dataGridFilePath As String)
			Try
				If al.Count = 0 Then Exit Sub
				Me.ProcessDocument(al, status)
				al.Clear()
				status = 0
				If (_bulkLoadFileWriter.BaseStream.Length + _dataGridFileWriter.BaseStream.Length > ImportBatchVolume) OrElse _batchCount > ImportBatchSize - 1 Then
					Me.TryPushImageBatch(bulkLoadFilePath, dataGridFilePath, False)
				End If
			Catch ex As Exception
				Throw
			End Try
		End Sub

		Public Function RunBulkImport(ByVal overwrite As kCura.EDDS.WebAPI.BulkImportManagerBase.OverwriteType, ByVal useBulk As Boolean) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Dim tries As Int32 = NumberOfRetries
			Dim retval As New kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			While tries > 0
				Try
					retval = BulkImport(overwrite, useBulk)
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

		Private Function BulkImport(ByVal overwrite As OverwriteType, ByVal useBulk As Boolean) As MassImportResults
			Dim retval As MassImportResults
			Dim settings As kCura.EDDS.WebAPI.BulkImportManagerBase.ImageLoadInfo = Me.GetSettingsObject
			settings.UseBulkDataImport = useBulk
			settings.Overlay = overwrite

			If _productionArtifactID = 0 Then
				retval = _bulkImportManager.BulkImportImage(_caseInfo.ArtifactID, settings, _copyFilesToRepository)
			Else
				retval = _bulkImportManager.BulkImportProductionImage(_caseInfo.ArtifactID, settings, _productionArtifactID, _copyFilesToRepository)
			End If
			Return retval
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
			.Repository = _repositoryPath,
			.UploadFullText = _replaceFullText,
			.DisableUserSecurityCheck = Me.DisableUserSecurityCheck,
			.AuditLevel = Me.AuditLevel,
			.OverlayArtifactID = _overlayArtifactID
			}
			Return settings
		End Function

		Private Sub TryPushImageBatch(ByVal bulkLoadFilePath As String, ByVal dataGridFilePath As String, ByVal isFinal As Boolean)
			_bulkLoadFileWriter.Close()
			_dataGridFileWriter.Close()
			_fileIdentifierLookup.Clear()
			Try
				PushImageBatch(bulkLoadFilePath, dataGridFilePath)
			Catch ex As Exception
				If BatchResizeEnabled AndAlso ExceptionIsTimeoutRelated(ex) AndAlso _continue Then
					Dim originalBatchSize As Int32 = Me.ImportBatchSize
					LowerBatchLimits()
					Me.RaiseWarningAndPause(ex, WaitTimeBetweenRetryAttempts)
					If Not _continue Then Throw 'after the pause
					Me.LowerBatchSizeAndRetry(bulkLoadFilePath, dataGridFilePath, originalBatchSize)
				Else
					Throw
				End If
			End Try
			
			DeleteFiles(bulkLoadFilePath, dataGridFilePath)
			If Not isFinal Then
				Try
					_bulkLoadFileWriter = New System.IO.StreamWriter(bulkLoadFilePath, False, System.Text.Encoding.Unicode)
					_dataGridFileWriter = New System.IO.StreamWriter(dataGridFilePath, False, System.Text.Encoding.Unicode)
				Catch
					_bulkLoadFileWriter = New System.IO.StreamWriter(bulkLoadFilePath, False, System.Text.Encoding.Unicode)
					_dataGridFileWriter = New System.IO.StreamWriter(dataGridFilePath, False, System.Text.Encoding.Unicode)
				End Try
			End If
			_currentStatisticsSnapshot = _statistics.ToDictionary
			_snapshotLastModifiedOn = System.DateTime.Now
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
			While totalRecords > recordsProcessed AndAlso Not hasReachedEof AndAlso _continue
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
					If tries < NumberOfRetries AndAlso BatchResizeEnabled AndAlso ExceptionIsTimeoutRelated(ex) AndAlso _continue Then
						LowerBatchLimits()
						Me.RaiseWarningAndPause(ex, WaitTimeBetweenRetryAttempts)
						If Not _continue Then Throw
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
			Me.PushImageBatch(bulkLocation, dataGridLocation)
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
		
		Public Sub PushImageBatch(ByVal bulkLoadFilePath As String, ByVal dataGridFilePath As String)
			If _batchCount = 0 Then Return
			PublishUploadModeEvent()
			_batchCount = 0
			_statistics.MetadataBytes += (Me.GetFileLength(bulkLoadFilePath) + Me.GetFileLength(dataGridFilePath))
			Dim start As Int64 = System.DateTime.Now.Ticks

			Dim validateBcp As FileUploadReturnArgs = _bcpuploader.UploadBcpFile(_caseInfo.ArtifactID, bulkLoadFilePath)
			If validateBcp Is Nothing Then Exit Sub
			
			Dim validateDataGridBcp As FileUploadReturnArgs =  _bcpuploader.UploadBcpFile(_caseInfo.ArtifactID, dataGridFilePath)
			If validateDataGridBcp Is Nothing Then Exit Sub

			_statistics.MetadataTime += System.Math.Max((System.DateTime.Now.Ticks - start), 1)

			_uploadKey = validateBcp.Value
			_uploadDataGridKey = validateDataGridBcp.Value

			Dim overwrite As kCura.EDDS.WebAPI.BulkImportManagerBase.OverwriteType
			Select Case _overwrite.ToLower
				Case "none"
					overwrite = EDDS.WebAPI.BulkImportManagerBase.OverwriteType.Append
				Case "strict"
					overwrite = EDDS.WebAPI.BulkImportManagerBase.OverwriteType.Overlay
				Case Else
					overwrite = EDDS.WebAPI.BulkImportManagerBase.OverwriteType.Both
			End Select
			If validateBcp.Type = FileUploadReturnArgs.FileUploadReturnType.ValidUploadKey AndAlso validateDataGridBcp.Type = FileUploadReturnArgs.FileUploadReturnType.ValidUploadKey Then
				start = System.DateTime.Now.Ticks
				Dim runResults As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults = Me.RunBulkImport(overwrite, True)
				_statistics.ProcessRunResults(runResults)
				_runId = runResults.RunID
				_statistics.SqlTime += System.Math.Max(System.DateTime.Now.Ticks - start, 1)
			ElseIf Config.EnableSingleModeImport Then
				PublishUploadModeEvent()
				start = System.DateTime.Now.Ticks
				Dim oldDestinationFolderPath As String = System.String.Copy(_bcpuploader.DestinationFolderPath)

				_bcpuploader.DestinationFolderPath = _caseInfo.DocumentPath.TrimEnd("\"c) & "\" & "EDDS" & _caseInfo.ArtifactID & "\"
				_uploadKey = _bcpuploader.UploadFile(bulkLoadFilePath, _caseInfo.ArtifactID)
				_uploadDataGridKey = _bcpuploader.UploadFile(dataGridFilePath, _caseInfo.ArtifactID)

				_statistics.MetadataTime += System.Math.Max(System.DateTime.Now.Ticks - start, 1)
				_bcpuploader.DestinationFolderPath = oldDestinationFolderPath
				start = System.DateTime.Now.Ticks
				Dim runResults As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults = Me.RunBulkImport(overwrite, False)
				_statistics.ProcessRunResults(runResults)
				_runId = runResults.RunID
				_statistics.SqlTime += System.Math.Max(System.DateTime.Now.Ticks - start, 1)
			Else
				If validateBcp.Type <> FileUploadReturnArgs.FileUploadReturnType.ValidUploadKey Then
					Throw New kCura.WinEDDS.LoadFileBase.BcpPathAccessException(validateBcp.Value)
				Else
					Throw New kCura.WinEDDS.LoadFileBase.BcpPathAccessException(validateDataGridBcp.Value)
				End If
			End If
			PublishUploadModeEvent()
			ManageErrors()
		End Sub

		Public Overridable Function GetImageReader() As kCura.WinEDDS.Api.IImageReader
			Return New OpticonFileReader(_folderID, _settings, Nothing, Nothing, _doRetryLogic)
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
			'TODO: same check as the other place
			DeleteFiles(bulkLoadFilePath, dataGridFilePath)
			_bulkLoadFileWriter = New System.IO.StreamWriter(bulkLoadFilePath, False, System.Text.Encoding.Unicode)
			_dataGridFileWriter = New System.IO.StreamWriter(dataGridFilePath, False, System.Text.Encoding.Unicode)
			Try
				_timekeeper.MarkStart("ReadFile_Init")
				_filePath = path
				_imageReader = Me.GetImageReader
				_imageReader.Initialize()
				_fileLineCount = _imageReader.CountRecords
				RaiseStatusEvent(kCura.Windows.Process.EventType.Progress, "Begin Image Upload", 0, 0)
				Dim al As New System.Collections.Generic.List(Of Api.ImageRecord)
				Dim status As Int64 = 0
				_timekeeper.MarkEnd("ReadFile_Init")

				_timekeeper.MarkStart("ReadFile_Main")
				Dim validateBcp As FileUploadReturnArgs = _bcpuploader.ValidateBcpPath(_caseInfo.ArtifactID, bulkLoadFilePath)
				Dim validateDataGridBcp As FileUploadReturnArgs = _bcpuploader.ValidateBcpPath(_caseInfo.ArtifactID, dataGridFilePath)

				If (validateBcp.Type = FileUploadReturnArgs.FileUploadReturnType.UploadError OrElse validateDataGridBcp.Type = FileUploadReturnArgs.FileUploadReturnType.UploadError) And Not Config.EnableSingleModeImport Then
					If validateBcp.Type = FileUploadReturnArgs.FileUploadReturnType.UploadError Then
						Throw New kCura.WinEDDS.LoadFileBase.BcpPathAccessException(validateBcp.Value)
					Else
						Throw New kCura.WinEDDS.LoadFileBase.BcpPathAccessException(validateDataGridBcp.Value)
					End If
				Else
					PublishUploadModeEvent()
				End If

				Me.Statistics.BatchSize = Me.ImportBatchSize
				If _productionArtifactID <> 0 Then _productionManager.DoPreImportProcessing(_caseInfo.ArtifactID, _productionArtifactID)
				While Me.[Continue]
					If Me.CurrentLineNumber < _startLineNumber Then
						Me.AdvanceRecord()
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
				Me.TryPushImageBatch(bulkLoadFilePath, dataGridFilePath, True)
				Me.CompleteSuccess()
				_timekeeper.MarkEnd("ReadFile_Cleanup")
				_timekeeper.MarkEnd("TOTAL")
				_timekeeper.GenerateCsvReportItemsAsRows("_winedds_image", "C:\")
			Catch ex As System.Exception
				Me.CompleteError(ex)
			Finally
				_timekeeper.MarkStart("ReadFile_CleanupTempTables")
				CleanupTempTables()
				_timekeeper.MarkEnd("ReadFile_CleanupTempTables")
			End Try
		End Sub

		Public Event EndRun(ByVal success As Boolean, ByVal runID As String)

		Private Sub CompleteSuccess()
			If Not _imageReader Is Nothing Then _imageReader.Close()
			If _productionArtifactID <> 0 Then _productionManager.DoPostImportProcessing(_fileUploader.CaseArtifactID, _productionArtifactID)
			Try
				RaiseEvent EndRun(True, _runId)
			Catch
			End Try
			RaiseStatusEvent(kCura.Windows.Process.EventType.Progress, "End Image Upload", Me.CurrentLineNumber, Me.CurrentLineNumber)
		End Sub

		Private Sub CompleteError(ByVal ex As System.Exception)
			Try
				_bulkLoadFileWriter.Close()
			Catch x As System.Exception
			End Try
			Try
				If Not _imageReader Is Nothing Then _imageReader.Close()
			Catch x As System.Exception
			End Try
			Try
				Me.ManageErrors()
			Catch
			End Try
			Try
				RaiseEvent EndRun(False, _runId)
			Catch
			End Try
			RaiseFatalError(ex)
		End Sub

		Private Sub ProcessDocument(ByVal al As System.Collections.Generic.List(Of Api.ImageRecord), ByVal status As Int64)
			Try
				GetImagesForDocument(al, status)
				_statistics.DocCount += 1
			Catch ex As System.Exception
				'Me.LogErrorInFile(al)
				Throw
			End Try
		End Sub

#End Region

#Region "Worker Methods"

		Public Function ProcessImageLine(ByVal imageRecord As Api.ImageRecord) As Relativity.MassImport.ImportStatus
			Try
				_totalValidated += 1
				Dim globalStart As System.DateTime = System.DateTime.Now
				Dim retval As Relativity.MassImport.ImportStatus = Relativity.MassImport.ImportStatus.Pending
				'check for existence
				If imageRecord.BatesNumber.Trim = "" Then
					Me.RaiseStatusEvent(Windows.Process.EventType.Error, String.Format("No image file or identifier specified on line."), CType((_totalValidated + _totalProcessed) / 2, Int64), Me.CurrentLineNumber)
					retval = Relativity.MassImport.ImportStatus.NoImageSpecifiedOnLine
				ElseIf Not Me.DisableImageLocationValidation AndAlso Not System.IO.File.Exists(BulkImageFileImporter.GetFileLocation(imageRecord)) Then
					Me.RaiseStatusEvent(Windows.Process.EventType.Error, String.Format("Image file specified ( {0} ) does not exist.", imageRecord.FileLocation), CType((_totalValidated + _totalProcessed) / 2, Int64), Me.CurrentLineNumber)
					retval = Relativity.MassImport.ImportStatus.FileSpecifiedDne
				Else
					Dim validator As New kCura.ImageValidator.ImageValidator
					Dim path As String = BulkImageFileImporter.GetFileLocation(imageRecord)
					Try
						If Not Me.DisableImageTypeValidation Then validator.ValidateImage(path)
						Me.RaiseStatusEvent(Windows.Process.EventType.Progress, String.Format("Image file ( {0} ) validated.", imageRecord.FileLocation), CType((_totalValidated + _totalProcessed) / 2, Int64), Me.CurrentLineNumber)
					Catch ex As System.Exception
						If TypeOf ex Is kCura.ImageValidator.Exception.Base Then
							retval = Relativity.MassImport.ImportStatus.InvalidImageFormat
							_verboseErrorCollection.AddError(imageRecord.OriginalIndex, ex)
						Else
							Throw
						End If
						'Me.RaiseStatusEvent(Windows.Process.EventType.Error, String.Format("Error in '{0}': {1}", path, ex.Message))
					End Try
				End If
				Return retval
			Catch ex As Exception
				Throw
			End Try
			'check to make sure image is good
		End Function

		'Private Sub LogErrorInFile(ByVal lines As System.Collections.ArrayList)
		'	If lines Is Nothing Then Exit Sub
		'	If _errorLogFileName = "" Then
		'		_errorLogFileName = System.IO.Path.GetTempFileName()
		'		_errorLogWriter = New System.IO.StreamWriter(_errorLogFileName, False, System.Text.Encoding.Default)
		'	End If
		'	Dim line As String()
		'	For Each line In lines
		'		_errorLogWriter.WriteLine(kCura.Utility.Array.StringArrayToCsv(line))
		'	Next
		'End Sub

		Public Shared Function GetFileLocation(ByVal record As Api.ImageRecord) As String
			Dim fileLocation As String = record.FileLocation
			If fileLocation <> "" AndAlso fileLocation.Chars(0) = "\" AndAlso fileLocation.Chars(1) <> "\" Then
				fileLocation = "." & fileLocation
			End If
			Return fileLocation
		End Function


		Private Sub GetImagesForDocument(ByVal lines As System.Collections.Generic.List(Of Api.ImageRecord), ByVal status As Int64)
			Try
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
					record = lines(i)
					Me.GetImageForDocument(BulkImageFileImporter.GetFileLocation(record), record.BatesNumber, documentId, i, offset, textFileList, i < lines.Count - 1, record.OriginalIndex.ToString, status, lines.Count, i = 0)
				Next

				Dim lastDivider As String = If(_fullTextStorageIsInSql, ",", String.Empty)

				If _replaceFullText Then
					If Not _fullTextStorageIsInSql Then
						'datagrid metadata including a blank data grid id
						_dataGridFileWriter.Write(documentId & "," & String.Empty & ",")
					End If

					If textFileList.Count = 0 Then
						'no extracted text encodings, write "-1"
						_bulkLoadFileWriter.Write(String.Format("{0}{1}", -1, lastDivider))
					ElseIf textFileList.Count > 0 Then
						_bulkLoadFileWriter.Write("{0}{1}", kCura.Utility.List.ToDelimitedString(Me.GetextractedTextEncodings(textFileList), "|"), lastDivider)
					End If


					Dim fullTextWriter As System.IO.StreamWriter = If(_fullTextStorageIsInSql, _bulkLoadFileWriter, _dataGridFileWriter)
					For Each filename As String In textFileList
						Dim chosenEncoding As System.Text.Encoding = _settings.FullTextEncoding
						Dim fileStream As IO.Stream

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
					_bulkLoadFileWriter.Write(String.Format("{0}{1}", -1, lastDivider))
				End If
				
				_bulkLoadFileWriter.Write(Relativity.Constants.ENDLINETERMSTRING)
				If _replaceFullText AndAlso Not _fullTextStorageIsInSql Then
					_dataGridFileWriter.Write(Relativity.Constants.ENDLINETERMSTRING)
				End If
			Catch ex As Exception
				Throw
			End Try
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


		Private Sub GetImageForDocument(ByVal imageFileName As String, ByVal batesNumber As String, ByVal documentIdentifier As String, ByVal order As Int32, ByRef offset As Int64, ByVal fullTextFiles As System.Collections.ArrayList, ByVal writeLineTermination As Boolean, ByVal originalLineNumber As String, ByVal status As Int64, ByVal totalForDocument As Int32, ByVal isStartRecord As Boolean)
			Try
				_totalProcessed += 1
				Dim filename As String = imageFileName.Substring(imageFileName.LastIndexOf("\") + 1)
				Dim extractedTextFileName As String = imageFileName.Substring(0, imageFileName.LastIndexOf("."c) + 1) & "txt"
				Dim fileGuid As String = ""
				Dim fileLocation As String = imageFileName
				Dim fileSize As Int64 = 0
				_batchCount += 1
				If status = 0 Then
					If _copyFilesToRepository Then
						RaiseStatusEvent(kCura.Windows.Process.EventType.Progress, String.Format("Uploading File '{0}'.", filename), CType((_totalValidated + _totalProcessed) / 2, Int64), Int64.Parse(originalLineNumber))
						_statistics.FileBytes += Me.GetFileLength(imageFileName)
						Dim start As Int64 = System.DateTime.Now.Ticks
						fileGuid = _fileUploader.UploadFile(imageFileName, _folderID)
						Dim now As Int64 = System.DateTime.Now.Ticks
						_statistics.FileTime += System.Math.Max(now - start, 1)
						fileLocation = _fileUploader.DestinationFolderPath.TrimEnd("\"c) & "\" & _fileUploader.CurrentDestinationDirectory & "\" & fileGuid
						If now - _snapshotLastModifiedOn.Ticks > 10000000 Then
							_currentStatisticsSnapshot = _statistics.ToDictionary
							_snapshotLastModifiedOn = New System.DateTime(now)
						End If
					Else
						RaiseStatusEvent(kCura.Windows.Process.EventType.Progress, String.Format("Processing image '{0}'.", batesNumber), CType((_totalValidated + _totalProcessed) / 2, Int64), Int64.Parse(originalLineNumber))
						fileGuid = System.Guid.NewGuid.ToString
					End If
					If System.IO.File.Exists(imageFileName) Then fileSize = Me.GetFileLength(imageFileName)
					If _replaceFullText AndAlso System.IO.File.Exists(extractedTextFileName) AndAlso Not fullTextFiles Is Nothing Then
						fullTextFiles.Add(extractedTextFileName)
					Else
						If _replaceFullText AndAlso Not System.IO.File.Exists(extractedTextFileName) Then
							RaiseStatusEvent(kCura.Windows.Process.EventType.Warning, String.Format("File '{0}' not found.  No text updated.", extractedTextFileName), CType((_totalValidated + _totalProcessed) / 2, Int64), Int64.Parse(originalLineNumber))
						End If
					End If
				End If
				If _replaceFullText AndAlso System.IO.File.Exists(extractedTextFileName) AndAlso Not fullTextFiles Is Nothing Then
					offset += Me.GetFileLength(extractedTextFileName)
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
				_bulkLoadFileWriter.Write(imageFileName & ",")
				_bulkLoadFileWriter.Write(",") 'kCura_Import_DataGridException
				If _replaceFullText AndAlso writeLineTermination Then
					_bulkLoadFileWriter.Write("-1,")
				End If
				If writeLineTermination Then
					_bulkLoadFileWriter.Write(Relativity.Constants.ENDLINETERMSTRING)
				End If
			Catch ex As Exception
				Throw
			End Try
		End Sub
#End Region

#Region "Events and Event Handling"

		Public Event FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception)
		Public Event StatusMessage(ByVal args As kCura.Windows.Process.StatusEventArgs)
		Public Event ReportErrorEvent(ByVal row As System.Collections.IDictionary)
		Public Event UploadModeChangeEvent(ByVal mode As String, ByVal isBulkEnabled As Boolean)

		Private Sub PublishUploadModeEvent()
			Dim retval As New List(Of String)
			Dim isBulkEnabled As Boolean = True
			If Not _bcpuploader Is Nothing Then
				retval.Add("Metadata: " & _bcpuploader.UploaderType.ToString())
				isBulkEnabled = _bcpuploader.IsBulkEnabled
			End If
			If _settings.CopyFilesToDocumentRepository Then
				If Not _fileUploader Is Nothing Then
					retval.Add("Files: " & _fileUploader.UploaderType.ToString())
				End If
			Else
				retval.Add("Files: not copied")
			End If
			If retval.Any() Then
				Dim uploadStatus As String = String.Join(" - ", retval.ToArray())
				RaiseEvent UploadModeChangeEvent(uploadStatus, isBulkEnabled)
			End If
		End Sub



		Private Sub RaiseFatalError(ByVal ex As System.Exception)
			RaiseEvent FatalErrorEvent("Error processing line: " + CurrentLineNumber.ToString, ex)
		End Sub

		Private Sub RaiseStatusEvent(ByVal et As kCura.Windows.Process.EventType, ByVal line As String, ByVal progressLineNumber As Int64, ByVal physicalLineNumber As Int64)
			RaiseEvent StatusMessage(New kCura.Windows.Process.StatusEventArgs(et, progressLineNumber, _fileLineCount, line & String.Format(" [line {0}]", physicalLineNumber), et = Windows.Process.EventType.Warning, _currentStatisticsSnapshot))
		End Sub

		Private Sub _processObserver_CancelImport(ByVal processID As System.Guid) Handles _processController.HaltProcessEvent
			If processID.ToString = _processID.ToString Then
				_continue = False
				If Not _imageReader Is Nothing Then _imageReader.Cancel()
				If Not _fileUploader Is Nothing Then _fileUploader.DoRetry = False
				If Not _bcpuploader Is Nothing Then _bcpuploader.DoRetry = False
			End If
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
			errorMessageFileWriter.WriteLine(String.Format("{0},{1},{2},{3}", CSVFormat(row("Line Number").ToString), CSVFormat(row("DocumentID").ToString), CSVFormat(row("FileID").ToString), CSVFormat(row("Message").ToString)))
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

		Private Sub _uploader_UploadModeChangeEvent(ByVal mode As String, ByVal isBulkEnabled As Boolean) Handles _fileUploader.UploadModeChangeEvent
			PublishUploadModeEvent()
		End Sub

		Private Sub _bcpuploader_UploadModeChangeEvent(ByVal mode As String, ByVal isBulkEnabled As Boolean) Handles _bcpuploader.UploadModeChangeEvent
			PublishUploadModeEvent()
		End Sub

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
				MyBase.New(String.Format("Document '{0}' exists - upload aborted.", docIdentifier))
			End Sub
		End Class

		Public Class OverwriteStrictException
			Inherits ImporterExceptionBase
			Public Sub New(ByVal docIdentifier As String)
				MyBase.New(String.Format("Document '{0}' does not exist - upload aborted.", docIdentifier))
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
				MyBase.New(String.Format("Document '{0}' belongs to one or more productions.  Document skipped.", identifier))
			End Sub
		End Class

		Public Class RedactionOverwriteException
			Inherits ImporterExceptionBase
			Public Sub New(ByVal identifier As String)
				MyBase.New(String.Format("The one or more images for document '{0}' have redactions.  Document skipped.", identifier))
			End Sub
		End Class

		Public Class InvalidIdentifierKeyException
			Inherits ImporterExceptionBase
			Public Sub New(ByVal identifier As String, ByVal fieldName As String)
				MyBase.New(String.Format("More than one document contains '{0}' as its '{1}' value.  Document skipped.", identifier, fieldName))
			End Sub
		End Class


#End Region

#Region "Exceptions - Fatal"
		Public Class InvalidBatesFormatException
			Inherits System.Exception
			Public Sub New(ByVal batesNumber As String, ByVal productionName As String, ByVal batesPrefix As String, ByVal batesSuffix As String, ByVal batesFormat As String)
				MyBase.New(String.Format("The image with production number {0} cannot be imported into production '{1}' because the prefix and/or suffix do not match the values specified in the production. Expected prefix: '{2}'. Expected suffix: '{3}'. Expected format: '{4}'.", batesNumber, productionName, batesPrefix, batesSuffix, batesFormat))
			End Sub
		End Class

#End Region

		Private Sub IoWarningHandler(ByVal e As IoWarningEventArgs)
			MyBase.RaiseIoWarning(e)
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
					Dim downloader As New FileDownloader(DirectCast(_bulkImportManager.Credentials, System.Net.NetworkCredential), _caseInfo.DocumentPath, _caseInfo.DownloadHandlerURL, _bulkImportManager.CookieContainer, kCura.WinEDDS.Service.Settings.AuthenticationToken)
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
							RaiseEvent StatusMessage(New kCura.Windows.Process.StatusEventArgs(Windows.Process.EventType.Error, Int32.Parse(line(0)) - 1, _fileLineCount, "[Line " & line(0) & "]" & errorMessages, Nothing))
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
				Try
					sr.Close()
					RemoveHandler sr.IoWarningEvent, AddressOf Me.IoWarningHandler
				Catch
				End Try
				Try
					w.Close()
				Catch
				End Try
				Try
					r.Close()
				Catch
				End Try
				Throw
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
			If _errorRowsFileLocation Is Nothing Then Exit Sub
			Try
				If System.IO.File.Exists(_errorRowsFileLocation) Then System.IO.File.Copy(_errorRowsFileLocation, exportLocation, True)
			Catch ex As Exception
				If System.IO.File.Exists(_errorRowsFileLocation) Then System.IO.File.Copy(_errorRowsFileLocation, exportLocation, True)
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
				If System.IO.File.Exists(_errorMessageFileLocation) Then System.IO.File.Copy(_errorMessageFileLocation, exportLocation, True)
			Catch ex As Exception
				If System.IO.File.Exists(_errorMessageFileLocation) Then System.IO.File.Copy(_errorMessageFileLocation, exportLocation, True)
			End Try
		End Sub

		Private Sub _fileUploader_UploadStatusEvent(ByVal s As String) Handles _fileUploader.UploadStatusEvent
			RaiseStatusEvent(kCura.Windows.Process.EventType.Status, s, 0, 0)
		End Sub

		Private Sub _fileUploader_UploadWarningEvent(ByVal s As String) Handles _fileUploader.UploadWarningEvent
			RaiseStatusEvent(kCura.Windows.Process.EventType.Warning, s, 0, 0)
		End Sub

		Private Sub _processController_ParentFormClosingEvent(ByVal processID As Guid) Handles _processController.ParentFormClosingEvent
			If processID.ToString = _processID.ToString Then Me.CleanupTempTables()
		End Sub

		Private Sub CleanupTempTables()
			If Not _runId Is Nothing AndAlso _runId <> "" Then
				Try
					_bulkImportManager.DisposeTempTables(_caseInfo.ArtifactID, _runId)
				Catch
				End Try
			End If
		End Sub

		Private Sub _bcpuploader_UploadWarningEvent(ByVal message As String) Handles _bcpuploader.UploadWarningEvent
			Me.RaiseStatusEvent(Windows.Process.EventType.Warning, message, CType((_totalValidated + _totalProcessed) / 2, Int64), Me.CurrentLineNumber)
		End Sub
	End Class
End Namespace