﻿Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.IO
Imports System.Reflection
Imports System.Threading
Imports System.Threading.Tasks
Imports Castle.Windsor
Imports kCura.WinEDDS.Container
Imports kCura.WinEDDS.Exporters
Imports kCura.WinEDDS.Exporters.Validator
Imports kCura.WinEDDS.FileNaming.CustomFileNaming
Imports kCura.WinEDDS.LoadFileEntry
Imports kCura.WinEDDS.Service.Export
Imports Relativity.DataExchange
Imports Relativity.DataExchange.Process
Imports Relativity.DataExchange.Service
Imports Relativity.DataExchange.Transfer
Imports Relativity.Logging

Namespace kCura.WinEDDS
	Public Class Exporter
		Implements IExporter
		Implements IStatus
		Implements IServiceNotification

#Region "Members"

		Private ReadOnly _logger As ILog
		Private ReadOnly _loadFileFormatterFactory As ILoadFileHeaderFormatterFactory
		Private ReadOnly _exportConfig As IExportConfig
		Private ReadOnly _fieldProviderCache As IFieldProviderCache
		Private ReadOnly _searchManager As Service.Export.ISearchManager
		Private ReadOnly _productionManager As Service.Export.IProductionManager
		Private ReadOnly _auditManager As Service.Export.IAuditManager
		Private ReadOnly _fieldManager As Service.Export.IFieldManager
		Public Property ExportManager As Service.Export.IExportManager
		Private _exportFile As kCura.WinEDDS.ExportFile
		Private _columns As System.Collections.ArrayList
		Public TotalExportArtifactCount As Int32
		Private WithEvents _processContext As ProcessContext
		Private _downloadHandler As Service.Export.IExportFileDownloader
		Private WithEvents _downloadModeStatus As Service.Export.IExportFileDownloaderStatus
		Private _volumeManager As VolumeManager
		Private _exportNativesToFileNamedFrom As kCura.WinEDDS.ExportNativeWithFilenameFrom
		Private _beginBatesColumn As String = ""
		Private _timekeeper As New Timekeeper2
		Private _productionArtifactIDs As Int32()
		Private _lastStatusMessageTs As Long = System.DateTime.Now.Ticks
		Private _lastDocumentsExportedCountReported As Int32 = 0
		Public Property Statistics As New kCura.WinEDDS.ExportStatistics
		Private _lastStatisticsSnapshot As IDictionary
		Private _stopwatch As System.Diagnostics.Stopwatch = New System.Diagnostics.Stopwatch()
		Private _warningCount As Int32 = 0
		Private _errorCount As Int32 = 0
		Private _fileCount As Int64 = 0
		Private _productionExportProduction As kCura.EDDS.WebAPI.ProductionManagerBase.ProductionInfo
		Private _productionLookup As New System.Collections.Generic.Dictionary(Of Int32, kCura.EDDS.WebAPI.ProductionManagerBase.ProductionInfo)
		Private _productionPrecedenceIds As Int32()
		Private _tryToNameNativesAndTextFilesAfterPrecedenceBegBates As Boolean = False
		Private _linesToWriteDat As ConcurrentDictionary(Of Int32, ILoadFileEntry)
		Private _linesToWriteOpt As ConcurrentBag(Of KeyValuePair(Of String, String))
		Protected FieldLookupService As IFieldLookupService
		Private _errorFile As IErrorFile
		Private ReadOnly _cancellationTokenSource As CancellationTokenSource
		Private _syncLock As Object = New Object
		Private _originalFileNameProvider As OriginalFileNameProvider
		Private _cancelledByUser As Boolean

#End Region

#Region "Accessors"
		''' <summary>
		''' This is not moved over to the Settings object yet
		''' </summary>
		''' <returns></returns>
		Public Property NameTextAndNativesAfterBegBates() As Boolean = False
		Public Property Settings() As kCura.WinEDDS.ExportFile
			Get
				Return _exportFile
			End Get
			Set(ByVal value As kCura.WinEDDS.ExportFile)
				_exportFile = value
			End Set
		End Property

		Public ReadOnly Property ExportConfig() As IExportConfig
			Get
				Return _exportConfig
			End Get
		End Property

		Public Property Columns() As System.Collections.ArrayList
			Get
				Return _columns
			End Get
			Set(ByVal value As System.Collections.ArrayList)
				_columns = value
			End Set
		End Property

		Public Property ExportNativesToFileNamedFrom() As kCura.WinEDDS.ExportNativeWithFilenameFrom
			Get
				Return _exportNativesToFileNamedFrom
			End Get
			Set(ByVal value As kCura.WinEDDS.ExportNativeWithFilenameFrom)
				_exportNativesToFileNamedFrom = value
			End Set
		End Property

		Public ReadOnly Property ErrorLogFileName() As String
			Get
				If UseOldExport Then
					Return _volumeManager?.ErrorLogFileName
				End If
				If _errorFile?.IsErrorFileCreated() Then
					Return _errorFile.Path()
				Else
					Return Nothing
				End If
			End Get
		End Property

		Friend ReadOnly Property IsCancelledByUser As Boolean
			Get
				Return _cancelledByUser
			End Get
		End Property

		Protected Overridable ReadOnly Property NumberOfRetries() As Int32
			Get
				Return _exportConfig.ExportErrorNumberOfRetries
			End Get
		End Property

		Protected Overridable ReadOnly Property WaitTimeBetweenRetryAttempts() As Int32
			Get
				Return _exportConfig.ExportErrorWaitTime
			End Get
		End Property

		Protected Overridable ReadOnly Property UseOldExport() As Boolean
			Get
				Return _exportConfig.UseOldExport
			End Get
		End Property

		Public Property FileHelper() As Global.Relativity.DataExchange.Io.IFile = Global.Relativity.DataExchange.Io.FileSystem.Instance.File
		Public Property DirectoryHelper() As Global.Relativity.DataExchange.Io.IDirectory = Global.Relativity.DataExchange.Io.FileSystem.Instance.Directory

		Private _fileNameProvider As IFileNameProvider
		Public Property FileNameProvider() As IFileNameProvider
			Get
				If _fileNameProvider Is Nothing Then
					_fileNameProvider = BuildFileNameProvider()
				End If

				Return _fileNameProvider
			End Get
			Set
				_fileNameProvider = Value
			End Set
		End Property

#End Region

		Public Event ShutdownEvent()
		Public Sub Shutdown()
			_logger.LogError("Prematurely shutting down export due to a serious configuration or validation issue.")
			RaiseEvent ShutdownEvent()
		End Sub

#Region "Constructors"

		<Obsolete("This constructor is marked for deprecation. Please use the constructor that requires a logger instance.")>
		Public Sub New(exportFile As kCura.WinEDDS.ExportFile, 
		               context As ProcessContext,
		               loadFileFormatterFactory As ILoadFileHeaderFormatterFactory)
			Me.New(exportFile, context, New Service.Export.WebApiServiceFactory(exportFile), loadFileFormatterFactory, New ExportConfig, RelativityLogger.Instance, CancellationToken.None)
		End Sub

		<Obsolete("This constructor is marked for deprecation. Please use the constructor that requires a logger instance.")>
		Public Sub New(exportFile As kCura.WinEDDS.ExportFile,
		               context As ProcessContext,
		               serviceFactory As Service.Export.IServiceFactory,
		               loadFileFormatterFactory As ILoadFileHeaderFormatterFactory,
		               exportConfig As IExportConfig)
			Me.New(exportFile, context, serviceFactory, loadFileFormatterFactory, exportConfig, RelativityLogger.Instance, CancellationToken.None)
		End Sub

		Public Sub New(exportFile As kCura.WinEDDS.ExportFile,
		               context As ProcessContext,
		               serviceFactory As Service.Export.IServiceFactory,
		               loadFileFormatterFactory As ILoadFileHeaderFormatterFactory,
		               exportConfig As IExportConfig,
		               logger As ILog,
		               cancellationToken As CancellationToken)
			_searchManager = serviceFactory.CreateSearchManager()
			_downloadHandler = serviceFactory.CreateExportFileDownloader()
			_downloadModeStatus = _downloadHandler
			_productionManager = serviceFactory.CreateProductionManager()
			_auditManager = serviceFactory.CreateAuditManager()
			_fieldManager = serviceFactory.CreateFieldManager()
			ExportManager = serviceFactory.CreateExportManager()

			_fieldProviderCache = New FieldProviderCache(exportFile.Credential, exportFile.CookieContainer)
			_cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken)
			_processContext = context
			DocumentsExported = 0
			TotalExportArtifactCount = 1
			Settings = exportFile
			Settings.FolderPath = Me.Settings.FolderPath + "\"
			ExportNativesToFileNamedFrom = exportFile.ExportNativesToFileNamedFrom
			_loadFileFormatterFactory = loadFileFormatterFactory
			_logger = If(logger, new NullLogger())
			_exportConfig = exportConfig
		End Sub

#End Region

		Public Property DocumentsExported As Integer Implements IExporter.DocumentsExported
			Get
				SyncLock _syncLock
					Return Me.Statistics.DocCount
				End SyncLock
			End Get
			Set
				SyncLock _syncLock
					Me.Statistics.DocCount = value
				End SyncLock
			End Set
		End Property

		Private _interactionManager As IUserNotification = New Exporters.NullUserNotification
		Public Property InteractionManager As Exporters.IUserNotification Implements IExporter.InteractionManager
			Get
				Return If(_interactionManager, New NullUserNotification)
			End Get
			Set(value As Exporters.IUserNotification)
				_interactionManager = value
			End Set
		End Property

		Public Function ExportSearch() As Boolean Implements IExporter.ExportSearch
			Try
				_stopwatch.Restart()
				_logger.LogInformation("Preparing to execute the export search.")
				Me.Search()
				_logger.LogInformation("Successfully executed the export search.")
			Catch ex As System.Exception
				Me.WriteFatalError($"A fatal error occurred on document #{Me.DocumentsExported}", ex)
				If Not _volumeManager Is Nothing Then
					_volumeManager.Close()
				End If
			Finally
				_stopwatch.Stop()
				RaiseEvent StatusMessage(New ExportEventArgs(Me.DocumentsExported, Me.TotalExportArtifactCount, "", EventType2.End, _lastStatisticsSnapshot, Statistics))
			End Try

			Me.LogStatistics()
			Dim result As Boolean = String.IsNullOrWhiteSpace(Me.ErrorLogFileName)
			Me.LogExportSearchResults(result)
			Return result
		End Function

		Protected Overridable Function GetHeaderColName(fieldInfo As ViewFieldInfo) As String
			Return fieldInfo.DisplayName
		End Function


		Private Function IsExtractedTextSelected() As Boolean
			For Each vfi As ViewFieldInfo In Me.Settings.SelectedViewFields
				If vfi.Category = FieldCategory.FullText Then Return True
			Next
			Return False
		End Function

		Private Function ExtractedTextField() As ViewFieldInfo
			For Each v As ViewFieldInfo In Me.Settings.AllExportableFields
				If v.Category = FieldCategory.FullText Then Return v
			Next
			Throw New System.Exception("Full text field somehow not in all fields")
		End Function

		Private Sub LogApiVersionInfo()
			Dim dict As Dictionary(Of String, Object) = New Dictionary(Of String, Object) From
				    {
					    {"SDK", GetType(Global.Relativity.DataExchange.IAppSettings).Assembly.GetName().Version},
					    {"TAPI", GetType(Global.Relativity.Transfer.ITransferClient).Assembly.GetName().Version}
				    }
			Me._logger.LogInformation("API Versions = @{ApiVersions}", dict)
		End Sub

		Private Sub LogExportSettings()
			_logger.LogObjectAsDictionary("Export Settings = {@ExportConfiguration}", _exportConfig, AddressOf LogObjectAsDictionaryFilter)
			_logger.LogObjectAsDictionary("Export File Settings = {@ExportFile}", _exportFile, AddressOf LogObjectAsDictionaryFilter)
			_logger.LogObjectAsDictionary("Volume Info Settings = {@ExportFileVolumeInfo}", _exportFile.VolumeInfo, AddressOf LogObjectAsDictionaryFilter)
		End Sub

		Private Function LogObjectAsDictionaryFilter(info As PropertyInfo) As Boolean
			' Reference types are intentionally limited due to size concerns.
			Dim supported As Boolean = info.GetIndexParameters().Length = 0 AndAlso
			                           Not String.Equals(info.Name, NameOf(_exportFile.FolderPath)) AndAlso
			                           (info.PropertyType.IsValueType OrElse
			                            Not Nullable.GetUnderlyingType(info.PropertyType) Is Nothing OrElse
			                            info.PropertyType = GetType(String) OrElse
			                            info.PropertyType = GetType(System.Text.Encoding))
			Return supported
		End Function

		Private Sub LogWebApiServiceException(exception As Exception)
			_logger.LogError(exception, "A serious error occurred calling a WebAPI service.")
		End Sub

		Private Sub LogStatistics()
			Dim statisticsDict As IDictionary(Of String, Object) = Me.Statistics.ToDictionaryForLogs()
			_logger.LogInformation("Export statistics: {@Statistics}", statisticsDict)
		End Sub

		Private Sub LogExportSearchResults(jobSuccessful As Boolean)
			Dim results As Dictionary(Of String, Object) = New Dictionary(Of String, Object) From
				{
				    {"ElapsedTime", _stopwatch.Elapsed},
				    {"Errors", Me._errorCount},
				    {"JobSuccessful", jobSuccessful},
				    {"JobCancelled", _cancellationTokenSource.IsCancellationRequested},
				    {"Warnings", Me._warningCount}
				}
			_logger.LogInformation("Exporter search results: {@ExporterSearchResults}", results)
		End Sub

		Private Sub InitializeExportProcess()
			_productionPrecedenceIds = (From p In If(Settings.ImagePrecedence, New Pair() {}) Where Not String.IsNullOrEmpty(p.Value) Select CInt(p.Value)).ToArray()
			_tryToNameNativesAndTextFilesAfterPrecedenceBegBates = ShouldTextAndNativesBeNamedAfterPrecedenceBegBates()
			InitializeFieldIndentifier()
		End Sub

		Protected Overridable Function GetAvfIds() As List(Of Int32)

			Dim allAvfIds As New List(Of Int32)
			For i As Int32 = 0 To _columns.Count - 1
				If Not TypeOf _columns(i) Is CoalescedTextViewField Then
					allAvfIds.Add(Me.Settings.SelectedViewFields(i).AvfId)
				End If
			Next

			If Me.Settings.TypeOfExport = ExportFile.ExportType.Production Then
				With _fieldManager.Read(Me.Settings.CaseArtifactID, _productionExportProduction.BeginBatesReflectedFieldId)
					_beginBatesColumn = SqlNameHelper.GetSqlFriendlyName(.DisplayName)
					If Not allAvfIds.Contains(.ArtifactViewFieldID) Then allAvfIds.Add(.ArtifactViewFieldID)
				End With
			End If

			If Me.Settings.ExportImages AndAlso Me.Settings.LogFileFormat = LoadFileType.FileFormat.IPRO_FullText Then
				If Not Me.IsExtractedTextSelected Then
					allAvfIds.Add(Me.ExtractedTextField.AvfId)
				End If
			End If

			Return allAvfIds
		End Function

		Private Sub Search()
			Me.LogApiVersionInfo()
			Me.LogExportSettings()
			InitializeExportProcess()
			
			Dim maxTries As Int32 = NumberOfRetries + 1
			Dim typeOfExportDisplayString As String = ""
			Dim errorOutputFilePath As String = _exportFile.FolderPath & "\" & _exportFile.LoadFilesPrefix & "_img_errors.txt"
			If FileHelper.Exists(errorOutputFilePath) AndAlso _exportFile.Overwrite Then FileHelper.Delete(errorOutputFilePath)
			Me.WriteUpdate("Retrieving export data from the server...")
			Dim startTicks As Int64 = System.DateTime.Now.Ticks
			Dim exportInitializationArgs As kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults = Nothing
			Dim columnHeaderString As String = Me.LoadColumns
			Dim production As kCura.EDDS.WebAPI.ProductionManagerBase.ProductionInfo = Nothing

			If Me.Settings.TypeOfExport = ExportFile.ExportType.Production Then

				Dim tries As Int32 = 0
				While tries < maxTries
					tries += 1
					Try
						_logger.LogVerbose("Preparing to retrieve the {ArtifactId} production from the {WorkspaceId} workspace.", Me.Settings.ArtifactID, Me.Settings.CaseArtifactID)
						production = _productionManager.Read(Me.Settings.CaseArtifactID, Me.Settings.ArtifactID)
						_logger.LogVerbose("Successfully retrieved the {ArtifactId} production from the {WorkspaceId} workspace.", Me.Settings.ArtifactID, Me.Settings.CaseArtifactID)
						Exit While
					Catch ex As System.Exception
						Me.LogWebApiServiceException(ex)
						If tries < maxTries AndAlso Not (TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("Need To Re Login") <> -1) Then
							Me.WriteStatusLine(EventType2.Status, "Error occurred, attempting retry number " & tries & ", in " & WaitTimeBetweenRetryAttempts & " seconds...", True)
							System.Threading.Thread.CurrentThread.Join(WaitTimeBetweenRetryAttempts * 1000)
						Else
							Throw
						End If
					End Try
				End While

				_productionExportProduction = production
			End If
			Dim allAvfIds As List(Of Int32) = GetAvfIds()
			Dim isFileNamePresent As Boolean = OriginalFileNameProvider.ExtendFieldRequestByFileNameIfNecessary(Me.Settings.AllExportableFields, allAvfIds)

			_logger.LogInformation("Preparing to initialize the {TypeOfExport} export search.", Me.Settings.TypeOfExport)
			Select Case Me.Settings.TypeOfExport
				Case ExportFile.ExportType.ArtifactSearch
					typeOfExportDisplayString = "search"
					exportInitializationArgs = CallServerWithRetry(Function() Me.ExportManager.InitializeSearchExport(_exportFile.CaseInfo.ArtifactID, Me.Settings.ArtifactID, allAvfIds.ToArray, Me.Settings.StartAtDocumentNumber + 1), maxTries)

				Case ExportFile.ExportType.ParentSearch
					typeOfExportDisplayString = "folder"
					exportInitializationArgs = CallServerWithRetry(Function() Me.ExportManager.InitializeFolderExport(Me.Settings.CaseArtifactID, Me.Settings.ViewID, Me.Settings.ArtifactID, False, allAvfIds.ToArray, Me.Settings.StartAtDocumentNumber + 1, Me.Settings.ArtifactTypeID), maxTries)

				Case ExportFile.ExportType.AncestorSearch
					typeOfExportDisplayString = "folder and subfolder"
					exportInitializationArgs = CallServerWithRetry(Function() Me.ExportManager.InitializeFolderExport(Me.Settings.CaseArtifactID, Me.Settings.ViewID, Me.Settings.ArtifactID, True, allAvfIds.ToArray, Me.Settings.StartAtDocumentNumber + 1, Me.Settings.ArtifactTypeID), maxTries)

				Case ExportFile.ExportType.Production
					typeOfExportDisplayString = "production"
					exportInitializationArgs = CallServerWithRetry(Function() Me.ExportManager.InitializeProductionExport(_exportFile.CaseInfo.ArtifactID, Me.Settings.ArtifactID, allAvfIds.ToArray, Me.Settings.StartAtDocumentNumber + 1), maxTries)

			End Select

			_logger.LogInformation("Successfully initialized the {TypeOfExport} export search and returned {TotalArtifactCount} total artifacts and {RunId} run identifier.",
			                       Me.Settings.TypeOfExport,
			                       exportInitializationArgs.RowCount,
			                       exportInitializationArgs.RunId)
			Me.TotalExportArtifactCount = CType(exportInitializationArgs.RowCount, Int32)
			If Me.TotalExportArtifactCount - 1 < Me.Settings.StartAtDocumentNumber Then
				Dim msg As String = String.Format("The chosen start item number ({0}) exceeds the number of {2} items in the export ({1}).  Export halted.", Me.Settings.StartAtDocumentNumber + 1, Me.TotalExportArtifactCount, vbNewLine)
				_logger.LogWarning(msg)
				InteractionManager.AlertCriticalError(msg)
				Me.Shutdown()
				Return
			End If

			Me.TotalExportArtifactCount -= Me.Settings.StartAtDocumentNumber

			Statistics.MetadataTime += System.Math.Max(System.DateTime.Now.Ticks - startTicks, 1)

			Using container As IWindsorContainer = ContainerFactoryProvider.ContainerFactory.Create(Me, exportInitializationArgs.ColumnNames, UseOldExport, _loadFileFormatterFactory)
				Dim batch As IBatch = Nothing
				Dim objectExportableSize As IObjectExportableSize = Nothing

				If UseOldExport Then
					_volumeManager = New VolumeManager(Me.Settings, Me.TotalExportArtifactCount, Me, _downloadHandler, _timekeeper, exportInitializationArgs.ColumnNames, Statistics, FileHelper, DirectoryHelper, FileNameProvider, _logger)
					FieldLookupService = _volumeManager
					_volumeManager.ColumnHeaderString = columnHeaderString
				Else
					Dim validator As IExportValidation = container.Resolve(Of IExportValidation)
					If (Not validator.ValidateExport(Settings, TotalExportArtifactCount)) Then
						_logger.LogWarning("The export failed due to a validation failure.")
						Shutdown()
						Return
					End If
					objectExportableSize = container.Resolve(Of IObjectExportableSize)
					FieldLookupService = container.Resolve(Of IFieldLookupService)
					_errorFile = container.Resolve(Of IErrorFile)
					batch = container.Resolve(Of IBatch)
					_downloadModeStatus = container.Resolve(Of IExportFileDownloaderStatus)
				End If

				CreateOriginalFileNameProviderInstance(isFileNamePresent)

				If _exportFile.AppendOriginalFileName AndAlso Not isFileNamePresent Then
					WriteWarningWithoutShowingExportedDocumentsCount("Filename column does not exist for this workspace and the filename from the file table will be used")
				End If

				Me.WriteStatusLine(EventType2.Status, "Created search log file.", True)
				Me.WriteUpdate($"Data retrieved. Beginning {typeOfExportDisplayString} export...")

				RaiseEvent StatusMessage(New ExportEventArgs(Me.DocumentsExported, Me.TotalExportArtifactCount, "", EventType2.ResetStartTime, _lastStatisticsSnapshot, Statistics))
				RaiseEvent FileTransferMultiClientModeChangeEvent(Me, New TapiMultiClientEventArgs(_downloadModeStatus.TransferModes))

				Dim records As Object() = Nothing
				Dim nextRecordIndex As Int32 = 0
				Dim lastRecordCount As Int32 = -1
				Me.Statistics.BatchCount = 0
				While lastRecordCount <> 0
					_timekeeper.MarkStart("Exporter_GetDocumentBlock")
					Me.Statistics.BatchCount += 1
					startTicks = System.DateTime.Now.Ticks
					Dim textPrecedenceAvfIds As Int32() = Nothing
					If Not Me.Settings.SelectedTextFields Is Nothing AndAlso Me.Settings.SelectedTextFields.Count > 0 Then textPrecedenceAvfIds = Me.Settings.SelectedTextFields.Select(Of Int32)(Function(f As ViewFieldInfo) f.AvfId).ToArray

					_logger.LogVerbose("Preparing to retrieve the next batch of artifacts for starting record index {NextRecordIndex} and batch {BatchNumber}.", nextRecordIndex, Me.Statistics.BatchCount)

					If Me.Settings.TypeOfExport = ExportFile.ExportType.Production Then
						records = CallServerWithRetry(Function() Me.ExportManager.RetrieveResultsBlockForProductionStartingFromIndex(Me.Settings.CaseInfo.ArtifactID, exportInitializationArgs.RunId, Me.Settings.ArtifactTypeID, allAvfIds.ToArray, _exportConfig.ExportBatchSize, Me.Settings.MulticodesAsNested, Me.Settings.MultiRecordDelimiter, Me.Settings.NestedValueDelimiter, textPrecedenceAvfIds, Me.Settings.ArtifactID, nextRecordIndex), maxTries)
					Else
						records = CallServerWithRetry(Function() Me.ExportManager.RetrieveResultsBlockStartingFromIndex(Me.Settings.CaseInfo.ArtifactID, exportInitializationArgs.RunId, Me.Settings.ArtifactTypeID, allAvfIds.ToArray, _exportConfig.ExportBatchSize, Me.Settings.MulticodesAsNested, Me.Settings.MultiRecordDelimiter, Me.Settings.NestedValueDelimiter, textPrecedenceAvfIds, nextRecordIndex), maxTries)
					End If

					If records Is Nothing Then
						_logger.LogVerbose("Exiting the export batch loop because the export results block returned null for batch {BatchNumber}.", Me.Statistics.BatchCount)
						Exit While
					End If

					_logger.LogVerbose("Successfully retrieved {TotalArtifactCount} artifacts for batch {BatchNumber}.", records.Length, Me.Statistics.BatchCount)
					If Me.Settings.TypeOfExport = ExportFile.ExportType.Production AndAlso production IsNot Nothing AndAlso production.DocumentsHaveRedactions Then
						WriteStatusLineWithoutDocCount(EventType2.Warning, "Please Note - Documents in this production were produced with redactions applied.  Ensure that you have exported text that was generated via OCR of the redacted documents.", True)
					End If

					lastRecordCount = records.Length
					Statistics.MetadataTime += System.Math.Max(System.DateTime.Now.Ticks - startTicks, 1)
					_timekeeper.MarkEnd("Exporter_GetDocumentBlock")
					Dim artifactIDs As New List(Of Int32)
					Dim artifactIdOrdinal As Int32 = FieldLookupService.GetOrdinalIndex("ArtifactID")
					
					If records.Length > 0 Then
						For Each artifactMetadata As Object() In records
							artifactIDs.Add(CType(artifactMetadata(artifactIdOrdinal), Int32))
						Next

						Dim batchStart As DateTime = DateTime.Now
						_logger.LogInformation("Preparing to export {TotalArtifactCount} chunked artifacts for batch {BatchNumber}.", records.Length, Me.Statistics.BatchCount)
						ExportChunk(artifactIDs.ToArray(), records, objectExportableSize, batch)
						Dim batchTicks As Int64 = System.Math.Max(System.DateTime.Now.Ticks - batchStart.Ticks, 1)
						_logger.LogInformation("Successfully exported {TotalArtifactCount} chunked artifacts for batch {BatchNumber} in {BatchTimeSpan}.", records.Length, Me.Statistics.BatchCount, TimeSpan.FromTicks(batchTicks))
						nextRecordIndex += records.Length
						artifactIDs.Clear()
						records = Nothing
					End If
					If lastRecordCount = 0 Then
						_logger.LogVerbose("Exiting the export batch loop because the next batch contains zero artifacts.")
					End If

					If _cancellationTokenSource.IsCancellationRequested Then
						_logger.LogVerbose("Exiting the export batch loop because cancellation is requested.")
						Exit While
					End If
				End While

				' REL-292896: nextRecordIndex represents the total number of records and should match.
				ValidateExportedRecordCount(nextRecordIndex, Me.TotalExportArtifactCount)

				Me.WriteStatusLine(EventType2.Status, FileDownloader.TotalWebTime.ToString, True)
				_timekeeper.GenerateCsvReportItemsAsRows()

				If UseOldExport Then
					_volumeManager.Finish()
				End If

				Me.AuditRun(True)
				Return
			End Using
		End Sub

		Private Sub ValidateExportedRecordCount(actualExportedRecordsCount As Int32, expectedExportedRecordsCount As Long)
			' We don't want to display validation message on the cancelled job
			If actualExportedRecordsCount <> expectedExportedRecordsCount AndAlso Not _cancellationTokenSource.IsCancellationRequested Then
				WriteError($"Total items processed ({actualExportedRecordsCount}) is different than expected total records count ({expectedExportedRecordsCount}).")
			End If
		End Sub

		Private Sub CreateOriginalFileNameProviderInstance(isFileNamePresent As Boolean)
			Dim emptyAction As Action(Of String) = Sub(y)
												   End Sub

			Dim shouldWriteWarning As Boolean = Settings.AppendOriginalFileName
			Dim warningWriter As Action(Of String) = If(shouldWriteWarning,
				CType(AddressOf WriteWarningWithoutShowingExportedDocumentsCount, Action(Of String)),
				emptyAction
			)
			_originalFileNameProvider = New OriginalFileNameProvider(isFileNamePresent, FieldLookupService, warningWriter)
		End Sub

		Private Function CallServerWithRetry(Of T)(f As Func(Of T), ByVal maxTries As Int32) As T
			Dim tries As Integer
			Dim records As T

			tries = 0
			While tries < maxTries
				tries += 1
				Try
					records = f()
					Exit While
				Catch ex As System.Exception
					Me.LogWebApiServiceException(ex)
					If TypeOf (ex) Is System.InvalidOperationException AndAlso ex.Message.Contains("empty response") Then
						Throw New Exception("Communication with the WebAPI server has failed, possibly because values for MaximumLongTextSizeForExportInCell and/or MaximumTextVolumeForExportChunk are too large.  Please lower them and try again.", ex)
					ElseIf tries < maxTries AndAlso Not (TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("Need To Re Login") <> -1) Then
						Me.WriteStatusLine(EventType2.Status, "Error occurred, attempting retry number " & tries & ", in " & WaitTimeBetweenRetryAttempts & " seconds...", True)
						System.Threading.Thread.CurrentThread.Join(WaitTimeBetweenRetryAttempts * 1000)
					Else
						Throw
					End If
				End Try
			End While
			Return records
		End Function

#Region "Private Helper Functions"

		Friend Class BatesEntry
			Friend Property ProductionArtifactID As Int32
			Friend Property BegBates As String
		End Class

		Private Function GenerateBatesLookup(source As Object()()) As Dictionary(Of Int32, List(Of BatesEntry))
			Dim lookup As New Dictionary(Of Int32, List(Of BatesEntry))()
			For Each entry As Object() In source
				Dim documentId As Int32 = CInt(entry(0))
				Dim documentEntries As List(Of BatesEntry)
				If Not lookup.ContainsKey(documentId) Then
					documentEntries = New List(Of BatesEntry)
					lookup.Add(documentId, documentEntries)
				Else
					documentEntries = lookup(documentId)
				End If
				documentEntries.Add(New BatesEntry() With {.BegBates = entry(1).ToString(), .ProductionArtifactID = CInt(entry(2))})
			Next
			Return lookup
		End Function

		Private Sub InitializeFieldIndentifier()

			If String.IsNullOrEmpty(Settings.IdentifierColumnName) Then
				'This case is for Exporter class usage outside of RDC (WinForms project)

				Dim docFieldCollection As DocumentFieldCollection = _fieldProviderCache.CurrentFields(Settings.ArtifactTypeID,
																									  Settings.CaseArtifactID,
																									  True)
				Dim identifierList As String() = docFieldCollection.IdentifierFieldNames()
				If identifierList.IsNullOrEmpty() Then
					Dim errMsg As String = String.Format("Export process failed. Cannot find identifier field for workspace: {0}!", Settings.CaseInfo.Name)
					WriteError(errMsg)
				End If
				Settings.IdentifierColumnName = identifierList(0)
			End If
		End Sub

		Private Sub ExportChunk(documentArtifactIDs As Integer(), records As Object(), objectExportableSize As IObjectExportableSize, batch As IBatch)
			Dim maxTries As Int32 = NumberOfRetries + 1
			Dim natives As New System.Data.DataView
			Dim images As New System.Data.DataView
			Dim productionImages As New System.Data.DataView
			Dim i As Int32 = 0
			Dim productionArtifactID As Int32 = 0

			If Me.Settings.TypeOfExport = ExportFile.ExportType.Production Then productionArtifactID = Settings.ArtifactID

			Dim retrieveThreads As Task(Of System.Data.DataView)() = New Task(Of System.Data.DataView)(2) {}

			retrieveThreads(0) = RetrieveNatives(natives, productionArtifactID, documentArtifactIDs, maxTries)
			retrieveThreads(1) = RetrieveImages(images, documentArtifactIDs, maxTries)
			retrieveThreads(2) = RetrieveProductions(productionImages, documentArtifactIDs, maxTries)

			Task.WaitAll(retrieveThreads)

			natives = retrieveThreads(0).Result()
			images = retrieveThreads(1).Result()
			productionImages = retrieveThreads(2).Result()

			Dim beginBatesColumnIndex As Int32 = -1
			If FieldLookupService.ContainsFieldName(_beginBatesColumn) Then
				beginBatesColumnIndex = FieldLookupService.GetOrdinalIndex(_beginBatesColumn)
			End If
			Dim identifierColumnName As String = SqlNameHelper.GetSqlFriendlyName(Me.Settings.IdentifierColumnName)
			Dim identifierColumnIndex As Int32 = FieldLookupService.GetOrdinalIndex(identifierColumnName)
			'TODO: come back to this
			Dim productionPrecedenceArtifactIds As Int32() = Settings.ImagePrecedence.Select(Function(pair) CInt(pair.Value)).ToArray()
			Dim lookup As New Lazy(Of Dictionary(Of Int32, List(Of BatesEntry)))(Function() GenerateBatesLookup(_productionManager.RetrieveBatesByProductionAndDocument(Me.Settings.CaseArtifactID, productionPrecedenceArtifactIds, documentArtifactIDs)))

			_linesToWriteDat = New ConcurrentDictionary(Of Int32, ILoadFileEntry)
			_linesToWriteOpt = New ConcurrentBag(Of KeyValuePair(Of String, String))

			Dim artifacts(documentArtifactIDs.Length - 1) As Exporters.ObjectExportInfo
			Dim volumePredictions(documentArtifactIDs.Length - 1) As VolumePredictions

			Dim threads As Task() = Nothing
			If UseOldExport Then
				Dim threadCount As Integer = _exportConfig.ExportThreadCount - 1
				threads = New Task(threadCount) {}
				For i = 0 To threadCount
					threads(i) = Task.FromResult(0)
				Next
			End If

			For i = 0 To documentArtifactIDs.Length - 1
				Dim record As Object() = DirectCast(records(i), Object())
				Dim nativeRow As System.Data.DataRowView = GetNativeRow(natives, documentArtifactIDs(i))
				Dim prediction As VolumePredictions = New VolumePredictions()
				Dim artifact As ObjectExportInfo = CreateArtifact(record, documentArtifactIDs(i), nativeRow, images, productionImages, beginBatesColumnIndex, identifierColumnIndex, lookup, prediction)

				If UseOldExport Then
					_volumeManager.FinalizeVolumeAndSubDirPredictions(prediction, artifact)
				Else
					objectExportableSize.FinalizeSizeCalculations(artifact, prediction)
				End If

				volumePredictions(i) = prediction

				artifacts(i) = artifact
			Next

			If UseOldExport Then
				For i = 0 To documentArtifactIDs.Length - 1
					If _cancellationTokenSource.IsCancellationRequested Then Exit For
					Dim openThreadNumber As Integer = Task.WaitAny(threads, TimeSpan.FromDays(1))
					Dim volumeNum As Integer = _volumeManager.GetCurrentVolumeNumber(volumePredictions(i))
					Dim subDirNum As Integer = _volumeManager.GetCurrentSubDirectoryNumber(volumePredictions(i))
					threads(openThreadNumber) = ExportArtifactAsync(artifacts(i), maxTries, i, documentArtifactIDs.Length, openThreadNumber, volumeNum, subDirNum)
					DocumentsExported += 1
				Next

				Task.WaitAll(threads)
				_volumeManager.WriteDatFile(_linesToWriteDat, artifacts)
				_volumeManager.WriteOptFile(_linesToWriteOpt, artifacts)
			Else
				batch.ExportAsync(artifacts, volumePredictions, _cancellationTokenSource.Token).ConfigureAwait(false).GetAwaiter().GetResult()
			End If
		End Sub

		Private Async Function RetrieveNatives(ByVal natives As System.Data.DataView, ByVal productionArtifactID As Int32, ByVal documentArtifactIDs As Int32(), ByVal maxTries As Integer) As Task(Of System.Data.DataView)
			Return Await Task.Run(
					Function() As System.Data.DataView
						If Me.Settings.ExportNative Then
							_timekeeper.MarkStart("Exporter_GetNativesForDocumentBlock")
							Dim start As Int64
							start = System.DateTime.Now.Ticks
							If Me.Settings.TypeOfExport = ExportFile.ExportType.Production Then
								natives.Table = CallServerWithRetry(Function() _searchManager.RetrieveNativesForProduction(Me.Settings.CaseArtifactID, productionArtifactID, documentArtifactIDs.ToCsv()).Tables(0), maxTries)
							ElseIf Me.Settings.ArtifactTypeID = ArtifactType.Document Then
								natives.Table = CallServerWithRetry(Function() _searchManager.RetrieveNativesForSearch(Me.Settings.CaseArtifactID, documentArtifactIDs.ToCsv()).Tables(0), maxTries)
							Else
								Dim dt As System.Data.DataTable = CallServerWithRetry(Function() _searchManager.RetrieveFilesForDynamicObjects(Me.Settings.CaseArtifactID, Me.Settings.FileField.FieldID, documentArtifactIDs).Tables(0), maxTries)
								If dt Is Nothing Then
									natives = Nothing
								Else
									natives.Table = dt
								End If
							End If
							Statistics.MetadataTime += System.Math.Max(System.DateTime.Now.Ticks - start, 1)
							_timekeeper.MarkEnd("Exporter_GetNativesForDocumentBlock")
						End If
						Return natives
					End Function
				)
		End Function

		Private Async Function RetrieveImages(ByVal images As System.Data.DataView, ByVal documentArtifactIDs As Int32(), ByVal maxTries As Integer) As Task(Of System.Data.DataView)
			Return Await Task.Run(
				Function()
					If Me.Settings.ExportImages Then
						Dim start As Int64
						_timekeeper.MarkStart("Exporter_GetImagesForDocumentBlock")
						start = System.DateTime.Now.Ticks

						images.Table = CallServerWithRetry(Function() Me.RetrieveImagesForDocuments(documentArtifactIDs), maxTries)

						Statistics.MetadataTime += System.Math.Max(System.DateTime.Now.Ticks - start, 1)
						_timekeeper.MarkEnd("Exporter_GetImagesForDocumentBlock")
					End If
					Return images
				End Function
				)
		End Function

		Private Async Function RetrieveProductions(ByVal productionImages As System.Data.DataView, ByVal documentArtifactIDs As Int32(), ByVal maxTries As Integer) As Task(Of System.Data.DataView)
			Return Await Task.Run(
				Function()
					If Me.Settings.ExportImages Then
						Dim start As Int64
						_timekeeper.MarkStart("Exporter_GetProductionsForDocumentBlock")
						start = System.DateTime.Now.Ticks

						productionImages.Table = CallServerWithRetry(Function() Me.RetrieveProductionImagesForDocuments(documentArtifactIDs, Me.Settings.ImagePrecedence), maxTries)

						Statistics.MetadataTime += System.Math.Max(System.DateTime.Now.Ticks - start, 1)
						_timekeeper.MarkEnd("Exporter_GetProductionsForDocumentBlock")
					End If
					Return productionImages
				End Function
				)
		End Function

		Private Async Function ExportArtifactAsync(ByVal artifact As ObjectExportInfo, ByVal maxTries As Integer, ByVal docNum As Integer, ByVal numDocs As Integer, ByVal threadNumber As Integer, ByVal volumeNumber As Integer, ByVal subDirectoryNumber As Integer) As Task
			Await Task.Run(
				Sub()
					_fileCount += CallServerWithRetry(Function()
														  Dim retval As Long
														  retval = _volumeManager.ExportArtifact(artifact, _linesToWriteDat, _linesToWriteOpt, threadNumber, volumeNumber, subDirectoryNumber)
														  If retval >= 0 Then
															  WriteUpdate("Exported document " & docNum + 1, docNum = numDocs - 1)
															  UpdateStatisticsSnapshot(DateTime.Now)
															  Return retval
														  Else
															  Return 0
														  End If
													  End Function, maxTries)
				End Sub
				)
		End Function

		Protected Overridable Function CreateObjectExportInfo() As ObjectExportInfo
			Return New ObjectExportInfo
		End Function

		Private Function CreateArtifact(ByVal record As Object(), ByVal documentArtifactID As Int32, ByVal nativeRow As System.Data.DataRowView, ByVal images As System.Data.DataView, ByVal productionImages As System.Data.DataView, ByVal beginBatesColumnIndex As Int32,
																		ByVal identifierColumnIndex As Int32, ByRef lookup As Lazy(Of Dictionary(Of Int32, List(Of BatesEntry))), ByRef prediction As VolumePredictions) As Exporters.ObjectExportInfo

			Dim artifact As ObjectExportInfo = CreateObjectExportInfo()

			If beginBatesColumnIndex <> -1 Then
				artifact.ProductionBeginBates = record(beginBatesColumnIndex).ToString
			End If
			artifact.IdentifierValue = record(identifierColumnIndex).ToString
			artifact.Images = Me.PrepareImages(images, productionImages, documentArtifactID, artifact, Me.Settings.ImagePrecedence, prediction)
			If nativeRow Is Nothing Then
				artifact.NativeFileGuid = ""
				artifact.OriginalFileName = ""
				artifact.NativeSourceLocation = ""
			Else
				artifact.OriginalFileName = _originalFileNameProvider.GetOriginalFileName(record, nativeRow)
				artifact.NativeSourceLocation = nativeRow("Location").ToString
				If Me.Settings.ArtifactTypeID = ArtifactType.Document Then
					artifact.NativeFileGuid = nativeRow("Guid").ToString
				Else
					artifact.FileID = CType(nativeRow("FileID"), Int32)
				End If
			End If

			If nativeRow Is Nothing Then
				artifact.NativeExtension = ""
			ElseIf nativeRow("Filename").ToString.IndexOf(".") <> -1 Then
				artifact.NativeExtension = nativeRow("Filename").ToString.Substring(nativeRow("Filename").ToString.LastIndexOf(".") + 1)
			Else
				artifact.NativeExtension = ""
			End If
			artifact.ArtifactID = documentArtifactID
			artifact.Metadata = record
			SetProductionBegBatesFileName(artifact, lookup)

			prediction.NativeFileCount = artifact.NativeCount

			If (prediction.NativeFileCount > 0 AndAlso nativeRow?.Row?.Table?.Columns?.Contains("Size")) Then
				prediction.NativeFilesSize = CType(nativeRow("Size"), Long)
			Else
				prediction.NativeFilesSize = 0
			End If

			prediction.ImageFileCount = artifact.ImageCount

			Return artifact
		End Function

		Private Function ShouldTextAndNativesBeNamedAfterPrecedenceBegBates() As Boolean
			Dim isCustomFileNaming As Boolean = Settings.ExportNativesToFileNamedFrom = ExportNativeWithFilenameFrom.Custom
			Return If(isCustomFileNaming,
				ShouldTextAndNativesBeNamedAfterPrecedenceBegBatesForCustomFileNaming(),
				ShouldTextAndNativesBeNamedAfterPrecedenceBegBatesForSimpleFileNaming()
			)
		End Function

		Private Function ShouldTextAndNativesBeNamedAfterPrecedenceBegBatesForCustomFileNaming() As Boolean
			Return Settings.CustomFileNaming.FirstFieldDescriptorPart().IsProduction
		End Function

		Private Function ShouldTextAndNativesBeNamedAfterPrecedenceBegBatesForSimpleFileNaming() As Boolean
			Return NameTextAndNativesAfterBegBates AndAlso
				Settings.TypeOfExport <> ExportFile.ExportType.Production AndAlso
				ExportNativesToFileNamedFrom = ExportNativeWithFilenameFrom.Production AndAlso
				_productionPrecedenceIds.Any(Function(prodID) prodID > 0)
		End Function

		Private Sub SetProductionBegBatesFileName(artifact As ObjectExportInfo, bateslookup As Lazy(Of Dictionary(Of Int32, List(Of BatesEntry))))
			If Not _tryToNameNativesAndTextFilesAfterPrecedenceBegBates Then Return

			Dim lookup As Dictionary(Of Int32, List(Of BatesEntry)) = bateslookup.Value
			If Not lookup.ContainsKey(artifact.ArtifactID) Then Return 'Nothing to set up
			Dim documentProductions As List(Of BatesEntry) = lookup(artifact.ArtifactID)
			If artifact.CoalescedProductionID.HasValue Then
				artifact.ProductionBeginBates = (From r In documentProductions Where r.ProductionArtifactID = artifact.CoalescedProductionID.Value).First().BegBates
			Else
				For Each prodId As Int32 In _productionPrecedenceIds
					For Each entry As BatesEntry In documentProductions
						If entry.ProductionArtifactID = prodId Then
							artifact.ProductionBeginBates = entry.BegBates
							Return
						End If
					Next
				Next
			End If
		End Sub

		Private Function PrepareImagesForProduction(ByVal imagesView As System.Data.DataView, ByVal documentArtifactID As Int32, ByRef prediction As VolumePredictions) As System.Collections.ArrayList
			Dim retval As New System.Collections.ArrayList
			If Not Me.Settings.ExportImages Then Return retval
			Dim matchingRows As DataRow() = imagesView.Table.Select("DocumentArtifactID = " & documentArtifactID.ToString)
			Dim i As Int32 = 0
			'DAS034 There is at least one case where all production images for a document will end up with the same filename.
			'This happens when the production uses Existing production numbering, and the base production used Document numbering.
			'This case cannot be detected using current available information about the Production that we get from WebAPI.
			'To be on the safe side, keep track of the first image filename, and if another image has the same filename, add i + 1 onto it.
			Dim firstImageFileName As String = Nothing
			If matchingRows.Count > 0 Then
				Dim dr As System.Data.DataRow
				For Each dr In matchingRows
					Dim image As New Exporters.ImageExportInfo
					image.FileName = Global.Relativity.DataExchange.Io.FileSystem.Instance.Path.ConvertIllegalCharactersInFilename(dr("ImageFileName").ToString)
					image.FileGuid = dr("ImageGuid").ToString
					image.ArtifactID = documentArtifactID
					image.PageOffset = NullableTypesHelper.DBNullConvertToNullable(Of Int32)(dr("ByteRange"))
					image.BatesNumber = dr("BatesNumber").ToString
					image.SourceLocation = dr("Location").ToString
					Dim filenameExtension As String = ""
					If image.FileName.IndexOf(".") <> -1 Then
						filenameExtension = "." & image.FileName.Substring(image.FileName.LastIndexOf(".") + 1)
					End If
					Dim filename As String = image.BatesNumber
					If i = 0 Then
						firstImageFileName = filename
					End If
					If (i > 0) AndAlso
						(IsDocNumberOnlyProduction(_productionExportProduction) OrElse
						 filename.Equals(firstImageFileName, StringComparison.OrdinalIgnoreCase)) Then
						filename &= "_" & (i + 1).ToString
					End If
					image.FileName = Global.Relativity.DataExchange.Io.FileSystem.Instance.Path.ConvertIllegalCharactersInFilename(filename & filenameExtension)
					If Not image.FileGuid = "" Then
						retval.Add(image)
						prediction.ImageFilesSize += CType(dr("ImageSize"), Long)
					End If
					i += 1
				Next
			End If
			Return retval
		End Function

		Private Function GetProduction(ByVal productionArtifactId As String) As kCura.EDDS.WebAPI.ProductionManagerBase.ProductionInfo
			Dim id As Int32 = CInt(productionArtifactId)
			If Not _productionLookup.ContainsKey(id) Then
				_productionLookup.Add(id, _productionManager.Read(Me.Settings.CaseArtifactID, id))
			End If
			Return _productionLookup(id)
		End Function

		Private Function IsDocNumberOnlyProduction(ByVal production As kCura.EDDS.WebAPI.ProductionManagerBase.ProductionInfo) As Boolean
			Return Not production Is Nothing AndAlso production.BatesNumbering = False AndAlso production.UseDocumentLevelNumbering AndAlso Not production.IncludeImageLevelNumberingForDocumentLevelNumbering
		End Function

		Private Function PrepareImages(ByVal imagesView As System.Data.DataView, ByVal productionImagesView As System.Data.DataView, ByVal documentArtifactID As Int32, ByVal artifact As Exporters.ObjectExportInfo,
																	 ByVal productionOrderList As Pair(), ByRef prediction As VolumePredictions) As System.Collections.ArrayList
			Dim retval As New System.Collections.ArrayList
			If Not Me.Settings.ExportImages Then Return retval
			If Me.Settings.TypeOfExport = ExportFile.ExportType.Production Then
				productionImagesView.Sort = "DocumentArtifactID ASC, PageID ASC"
				Return Me.PrepareImagesForProduction(productionImagesView, documentArtifactID, prediction)
			End If
			Dim item As Pair
			For Each item In productionOrderList
				If item.Value = "-1" Then
					Return Me.PrepareOriginalImages(imagesView, documentArtifactID, artifact, prediction)
				Else
					productionImagesView.RowFilter = String.Format("DocumentArtifactID = {0} AND ProductionArtifactID = {1}", documentArtifactID, item.Value)
					Dim firstImageFileName As String = Nothing
					If productionImagesView.Count > 0 Then
						Dim drv As System.Data.DataRowView
						Dim i As Int32 = 0
						For Each drv In productionImagesView
							Dim image As New Exporters.ImageExportInfo
							image.FileName = Global.Relativity.DataExchange.Io.FileSystem.Instance.Path.ConvertIllegalCharactersInFilename(drv("ImageFileName").ToString)
							image.FileGuid = drv("ImageGuid").ToString
							If image.FileGuid <> "" Then
								image.ArtifactID = documentArtifactID
								image.BatesNumber = drv("BatesNumber").ToString
								image.PageOffset = NullableTypesHelper.DBNullConvertToNullable(Of Int32)(drv("ByteRange"))
								Dim filenameExtension As String = ""
								If image.FileName.IndexOf(".") <> -1 Then
									filenameExtension = "." & image.FileName.Substring(image.FileName.LastIndexOf(".") + 1)
								End If
								Dim filename As String = image.BatesNumber
								If i = 0 Then
									firstImageFileName = filename
								End If
								If (IsDocNumberOnlyProduction(Me.GetProduction(item.Value)) OrElse filename.Equals(firstImageFileName, StringComparison.OrdinalIgnoreCase)) AndAlso i > 0 Then filename &= "_" & (i + 1).ToString
								image.FileName = Global.Relativity.DataExchange.Io.FileSystem.Instance.Path.ConvertIllegalCharactersInFilename(filename & filenameExtension)
								image.SourceLocation = drv("Location").ToString
								retval.Add(image)
								prediction.ImageFilesSize += CType(drv("ImageSize"), Long)
								i += 1
							End If
						Next
						artifact.CoalescedProductionID = CInt(item.Value)
						Return retval
					End If
				End If
			Next
			Return retval
		End Function

		Private Function PrepareOriginalImages(ByVal imagesView As System.Data.DataView, ByVal documentArtifactID As Int32, ByVal artifact As Exporters.ObjectExportInfo, ByRef prediction As VolumePredictions) As System.Collections.ArrayList
			Dim retval As New System.Collections.ArrayList
			If Not Me.Settings.ExportImages Then Return retval
			imagesView.RowFilter = "DocumentArtifactID = " & documentArtifactID.ToString
			Dim i As Int32 = 0
			If imagesView.Count > 0 Then
				Dim drv As System.Data.DataRowView
				For Each drv In imagesView
					Dim image As New Exporters.ImageExportInfo
					image.FileName = Global.Relativity.DataExchange.Io.FileSystem.Instance.Path.ConvertIllegalCharactersInFilename(drv("Filename").ToString)
					image.FileGuid = drv("Guid").ToString
					image.ArtifactID = documentArtifactID
					image.PageOffset = NullableTypesHelper.DBNullConvertToNullable(Of Int32)(drv("ByteRange"))
					If i = 0 Then
						image.BatesNumber = artifact.IdentifierValue
					Else
						image.BatesNumber = drv("Identifier").ToString
						If image.BatesNumber.IndexOf(image.FileGuid) <> -1 Then
							image.BatesNumber = artifact.IdentifierValue & "_" & i.ToString.PadLeft(imagesView.Count.ToString.Length + 1, "0"c)
						End If
					End If
					'image.BatesNumber = drv("Identifier").ToString
					Dim filenameExtension As String = ""
					If image.FileName.IndexOf(".") <> -1 Then
						filenameExtension = "." & image.FileName.Substring(image.FileName.LastIndexOf(".") + 1)
					End If
					image.FileName = Global.Relativity.DataExchange.Io.FileSystem.Instance.Path.ConvertIllegalCharactersInFilename(image.BatesNumber.ToString & filenameExtension)
					image.SourceLocation = drv("Location").ToString
					retval.Add(image)
					prediction.ImageFilesSize += CType(drv("Size"), Long)
					i += 1
				Next
			End If
			Return retval
		End Function

		Private Function GetNativeRow(ByVal dv As System.Data.DataView, ByVal artifactID As Int32) As System.Data.DataRowView
			If Not Me.Settings.ExportNative Then Return Nothing
			If Me.Settings.ArtifactTypeID = 10 Then
				dv.RowFilter = "DocumentArtifactID = " & artifactID.ToString
			Else
				dv.RowFilter = "ObjectArtifactID = " & artifactID.ToString
			End If
			If dv.Count > 0 Then
				Return dv(0)
			Else
				Return Nothing
			End If
		End Function

		''' <summary>
		''' Sets the member variable _columns to contain an array of each Field which will be exported.
		''' _columns is an array of ViewFieldInfo, but for the "Text Precedence" column, the array item is
		''' a CoalescedTextViewField (a subclass of ViewFieldInfo).
		''' </summary>
		''' <returns>A string containing the contents of the export file header.  For example, if _exportFile.LoadFile is false,
		''' and the fields selected to export are (Control Number and ArtifactID), along with the Text Precendence which includes
		''' Extracted Text, then the following string would be returned: ""Control Number","Artifact ID","Text Precedence" "
		''' </returns>
		''' <remarks></remarks>
		Private Function LoadColumns() As String
			For Each field As WinEDDS.ViewFieldInfo In Me.Settings.SelectedViewFields
				Me.Settings.ExportFullText = Me.Settings.ExportFullText OrElse field.Category = FieldCategory.FullText
			Next
			_columns = New System.Collections.ArrayList(Me.Settings.SelectedViewFields)
			If Not Me.Settings.SelectedTextFields Is Nothing AndAlso Me.Settings.SelectedTextFields.Count > 0 Then
				Dim longTextSelectedViewFields As New List(Of ViewFieldInfo)()
				longTextSelectedViewFields.AddRange(Me.Settings.SelectedViewFields.Where(Function(f As ViewFieldInfo) f.FieldType = FieldType.Text OrElse f.FieldType = FieldType.OffTableText))
				If (Me.Settings.SelectedTextFields.Count = 1) AndAlso longTextSelectedViewFields.Exists(Function(f As ViewFieldInfo) f.Equals(Me.Settings.SelectedTextFields.First)) Then
					Dim selectedViewFieldToRemove As ViewFieldInfo = longTextSelectedViewFields.Find(Function(f As ViewFieldInfo) f.Equals(Me.Settings.SelectedTextFields.First))
					If selectedViewFieldToRemove IsNot Nothing Then
						Dim indexOfSelectedViewFieldToRemove As Int32 = _columns.IndexOf(selectedViewFieldToRemove)
						_columns.RemoveAt(indexOfSelectedViewFieldToRemove)
						_columns.Insert(indexOfSelectedViewFieldToRemove, New CoalescedTextViewField(Me.Settings.SelectedTextFields.First, True))
					Else

						_columns.Add(New CoalescedTextViewField(Me.Settings.SelectedTextFields.First, False))
					End If
				Else
					_columns.Add(New CoalescedTextViewField(Me.Settings.SelectedTextFields.First, False))
				End If
			End If

			Dim header As String = _loadFileFormatterFactory.Create(Settings).GetHeader(_columns.Cast(Of ViewFieldInfo)().ToList())
			Return header
		End Function

		Private Function RetrieveImagesForDocuments(ByVal documentArtifactIDs As Int32()) As System.Data.DataTable
			Select Case Me.Settings.TypeOfExport
				Case ExportFile.ExportType.Production
					Return Nothing
				Case Else
					Return _searchManager.RetrieveImagesForDocuments(Me.Settings.CaseArtifactID, documentArtifactIDs).Tables(0)
			End Select
		End Function

		Private Function RetrieveProductionImagesForDocuments(ByVal documentArtifactIDs As Int32(), ByVal productionOrderList As Pair()) As System.Data.DataTable
			Select Case Me.Settings.TypeOfExport
				Case ExportFile.ExportType.Production
					Return _searchManager.RetrieveImagesForProductionDocuments(Me.Settings.CaseArtifactID, documentArtifactIDs, Int32.Parse(productionOrderList(0).Value)).Tables(0)
				Case Else
					Dim productionIDs As Int32() = Me.GetProductionArtifactIDs(productionOrderList)
					If productionIDs.Length > 0 Then Return _searchManager.RetrieveImagesByProductionIDsAndDocumentIDsForExport(Me.Settings.CaseArtifactID, productionIDs, documentArtifactIDs).Tables(0)
			End Select
			Return Nothing
		End Function

		Private Function GetProductionArtifactIDs(ByVal productionOrderList As Pair()) As Int32()
			If _productionArtifactIDs Is Nothing Then
				Dim retval As New System.Collections.ArrayList
				Dim item As Pair
				For Each item In productionOrderList
					If item.Value <> "-1" Then
						retval.Add(Int32.Parse(item.Value))
					End If
				Next
				_productionArtifactIDs = DirectCast(retval.ToArray(GetType(Int32)), Int32())
			End If
			Return _productionArtifactIDs
		End Function

#End Region


#Region "Messaging"

		Private Sub AuditRun(ByVal success As Boolean)
			Dim args As New kCura.EDDS.WebAPI.AuditManagerBase.ExportStatistics
			args.AppendOriginalFilenames = Me.Settings.AppendOriginalFileName
			args.Bound = Me.Settings.QuoteDelimiter
			args.ArtifactTypeID = Me.Settings.ArtifactTypeID
			Select Case Me.Settings.TypeOfExport
				Case ExportFile.ExportType.AncestorSearch
					args.DataSourceArtifactID = Me.Settings.ViewID
				Case ExportFile.ExportType.ArtifactSearch
					args.DataSourceArtifactID = Me.Settings.ArtifactID
				Case ExportFile.ExportType.ParentSearch
					args.DataSourceArtifactID = Me.Settings.ViewID
				Case ExportFile.ExportType.Production
					args.DataSourceArtifactID = Me.Settings.ArtifactID
			End Select
			args.Delimiter = Me.Settings.RecordDelimiter
			args.DestinationFilesystemFolder = Me.Settings.FolderPath
			args.DocumentExportCount = Me.DocumentsExported
			args.ErrorCount = _errorCount
			If Not Me.Settings.SelectedTextFields Is Nothing AndAlso Me.Settings.SelectedTextFields.Any() Then args.ExportedTextFieldID = Me.Settings.SelectedTextFields.First().FieldArtifactId
			If Me.Settings.ExportFullTextAsFile Then
				args.ExportedTextFileEncodingCodePage = Me.Settings.TextFileEncoding.CodePage
				args.ExportTextFieldAsFiles = True
			Else
				args.ExportTextFieldAsFiles = False
			End If
			Dim fields As New System.Collections.ArrayList
			For Each field As ViewFieldInfo In Me.Settings.SelectedViewFields
				If Not fields.Contains(field.FieldArtifactId) Then fields.Add(field.FieldArtifactId)
			Next
			args.Fields = DirectCast(fields.ToArray(GetType(Int32)), Int32())
			args.ExportNativeFiles = Me.Settings.ExportNative
			If args.Fields.Length > 0 OrElse Me.Settings.ExportNative Then
				args.MetadataLoadFileEncodingCodePage = Me.Settings.LoadFileEncoding.CodePage
				Select Case Me.Settings.LoadFileExtension.ToLower
					Case "txt"
						args.MetadataLoadFileFormat = EDDS.WebAPI.AuditManagerBase.LoadFileFormat.Custom
					Case "csv"
						args.MetadataLoadFileFormat = EDDS.WebAPI.AuditManagerBase.LoadFileFormat.Csv
					Case "dat"
						args.MetadataLoadFileFormat = EDDS.WebAPI.AuditManagerBase.LoadFileFormat.Dat
					Case "html"
						args.MetadataLoadFileFormat = EDDS.WebAPI.AuditManagerBase.LoadFileFormat.Html
				End Select
				args.MultiValueDelimiter = Me.Settings.MultiRecordDelimiter
				args.ExportMultipleChoiceFieldsAsNested = Me.Settings.MulticodesAsNested
				args.NestedValueDelimiter = Me.Settings.NestedValueDelimiter
				args.NewlineProxy = Me.Settings.NewlineDelimiter
			End If
			Try
				args.FileExportCount = CType(_fileCount, Int32)
			Catch ex As System.Exception
				_logger.LogWarning(ex, "Failed to retrieve the file export count.")
			End Try
			Select Case Me.Settings.TypeOfExportedFilePath
				Case ExportFile.ExportedFilePathType.Absolute
					args.FilePathSettings = "Use Absolute Paths"
				Case ExportFile.ExportedFilePathType.Prefix
					args.FilePathSettings = "Use Prefix: " & Me.Settings.FilePrefix
				Case ExportFile.ExportedFilePathType.Relative
					args.FilePathSettings = "Use Relative Paths"
			End Select
			If Me.Settings.ExportImages Then
				args.ExportImages = True
				Select Case Me.Settings.TypeOfImage
					Case ExportFile.ImageType.MultiPageTiff
						args.ImageFileType = EDDS.WebAPI.AuditManagerBase.ImageFileExportType.MultiPageTiff
					Case ExportFile.ImageType.Pdf
						args.ImageFileType = EDDS.WebAPI.AuditManagerBase.ImageFileExportType.PDF
					Case ExportFile.ImageType.SinglePage
						args.ImageFileType = EDDS.WebAPI.AuditManagerBase.ImageFileExportType.SinglePage
				End Select
				Select Case Me.Settings.LogFileFormat
					Case LoadFileType.FileFormat.IPRO
						args.ImageLoadFileFormat = EDDS.WebAPI.AuditManagerBase.ImageLoadFileFormatType.Ipro
					Case LoadFileType.FileFormat.IPRO_FullText
						args.ImageLoadFileFormat = EDDS.WebAPI.AuditManagerBase.ImageLoadFileFormatType.IproFullText
					Case LoadFileType.FileFormat.Opticon
						args.ImageLoadFileFormat = EDDS.WebAPI.AuditManagerBase.ImageLoadFileFormatType.Opticon
				End Select
				Dim hasOriginal As Boolean = False
				Dim hasProduction As Boolean = False
				For Each pair As WinEDDS.Pair In Me.Settings.ImagePrecedence
					If pair.Value <> "-1" Then
						hasProduction = True
					Else
						hasOriginal = True
					End If
				Next
				If hasProduction AndAlso hasOriginal Then
					args.ImagesToExport = EDDS.WebAPI.AuditManagerBase.ImagesToExportType.Both
				ElseIf hasProduction Then
					args.ImagesToExport = EDDS.WebAPI.AuditManagerBase.ImagesToExportType.Produced
				Else
					args.ImagesToExport = EDDS.WebAPI.AuditManagerBase.ImagesToExportType.Original
				End If
			Else
				args.ExportImages = False
			End If
			args.OverwriteFiles = Me.Settings.Overwrite
			Dim preclist As New System.Collections.ArrayList
			For Each pair As WinEDDS.Pair In Me.Settings.ImagePrecedence
				preclist.Add(Int32.Parse(pair.Value))
			Next
			args.ProductionPrecedence = DirectCast(preclist.ToArray(GetType(Int32)), Int32())
			args.RunTimeInMilliseconds = CType(System.Math.Min(_stopwatch.ElapsedMilliseconds, Int32.MaxValue), Int32)
			If Me.Settings.TypeOfExport = ExportFile.ExportType.AncestorSearch OrElse Me.Settings.TypeOfExport = ExportFile.ExportType.ParentSearch Then
				args.SourceRootFolderID = Me.Settings.ArtifactID
			End If
			args.SubdirectoryImagePrefix = Me.Settings.VolumeInfo.SubdirectoryImagePrefix(False)
			args.SubdirectoryMaxFileCount = Me.Settings.VolumeInfo.SubdirectoryMaxSize
			args.SubdirectoryNativePrefix = Me.Settings.VolumeInfo.SubdirectoryNativePrefix(False)
			args.SubdirectoryStartNumber = Me.Settings.VolumeInfo.SubdirectoryStartNumber
			args.SubdirectoryTextPrefix = Me.Settings.VolumeInfo.SubdirectoryFullTextPrefix(False)
			'args.TextAndNativeFilesNamedAfterFieldID = Me.ExportNativesToFileNamedFrom
			If Me.ExportNativesToFileNamedFrom = ExportNativeWithFilenameFrom.Identifier Then
				For Each field As ViewFieldInfo In Me.Settings.AllExportableFields
					If field.Category = FieldCategory.Identifier Then
						args.TextAndNativeFilesNamedAfterFieldID = field.FieldArtifactId
						Exit For
					End If
				Next
			Else
				For Each field As ViewFieldInfo In Me.Settings.AllExportableFields
					If field.AvfColumnName.ToLower = _beginBatesColumn.ToLower Then
						args.TextAndNativeFilesNamedAfterFieldID = field.FieldArtifactId
						Exit For
					End If
				Next
			End If
			args.TotalFileBytesExported = Statistics.FileBytes
			args.TotalMetadataBytesExported = Statistics.MetadataBytes
			Select Case Me.Settings.TypeOfExport
				Case ExportFile.ExportType.AncestorSearch
					args.Type = "Folder and Subfolders"
				Case ExportFile.ExportType.ArtifactSearch
					args.Type = "Saved Search"
				Case ExportFile.ExportType.ParentSearch
					args.Type = "Folder"
				Case ExportFile.ExportType.Production
					args.Type = "Production Set"
			End Select
			args.VolumeMaxSize = Me.Settings.VolumeInfo.VolumeMaxSize
			args.VolumePrefix = Me.Settings.VolumeInfo.VolumePrefix
			args.VolumeStartNumber = Me.Settings.VolumeInfo.VolumeStartNumber
			args.StartExportAtDocumentNumber = Me.Settings.StartAtDocumentNumber + 1
			args.CopyFilesFromRepository = Me.Settings.VolumeInfo.CopyNativeFilesFromRepository OrElse Settings.VolumeInfo.CopyNativeFilesFromRepository
			args.WarningCount = _warningCount
			Try
				_auditManager.AuditExport(Me.Settings.CaseInfo.ArtifactID, Not success, args)
			Catch ex As System.Exception
				Me.LogWebApiServiceException(ex)
			End Try
		End Sub

		Friend Sub WriteFatalError(ByVal line As String, ByVal ex As System.Exception)
			Me.AuditRun(False)
			_logger.LogFatal(ex, "Export experienced a fatal error. Line: {Line}", line)
			RaiseEvent FatalErrorEvent(line, ex)
		End Sub

		Private Sub LogStatusLine(ByVal e As EventType2, ByVal line As String, ByVal isEssential As Boolean)
			Select Case e
				Case EventType2.Warning
					_logger.LogWarning(line)
				Case EventType2.Error
					_logger.LogError(line)
				Case EventType2.Statistics
					_logger.LogInformation(line)
				Case Else
					If isEssential Then
						_logger.LogInformation(line)
					Else
						_logger.LogVerbose(line)
					End If
			End Select
		End Sub

		Friend Sub WriteStatusLine(ByVal e As EventType2, ByVal line As String, ByVal isEssential As Boolean, ByVal showNumberOfExportedDocuments As Boolean)
			Dim now As Long = System.DateTime.Now.Ticks

			SyncLock _syncLock
				If now - _lastStatusMessageTs > TimeSpan.TicksPerSecond OrElse isEssential Then
					_lastStatusMessageTs = now
					Dim appendString As String = ""
					If showNumberOfExportedDocuments Then
						appendString = " ... " & Me.DocumentsExported - _lastDocumentsExportedCountReported & " document(s) exported."
						_lastDocumentsExportedCountReported = Me.DocumentsExported
					End If
					RaiseEvent StatusMessage(New ExportEventArgs(Me.DocumentsExported, Me.TotalExportArtifactCount, line & appendString, e, _lastStatisticsSnapshot, Statistics))
				End If

				Me.LogStatusLine(e, line, isEssential)
			End SyncLock
		End Sub

		Friend Sub WriteStatusLine(ByVal e As EventType2, ByVal line As String, ByVal isEssential As Boolean) Implements IStatus.WriteStatusLine
			WriteStatusLine(e, line, isEssential, True)
		End Sub

		Friend Sub WriteStatusLineWithoutDocCount(ByVal e As EventType2, ByVal line As String, ByVal isEssential As Boolean) Implements IStatus.WriteStatusLineWithoutDocCount
			Dim now As Long = System.DateTime.Now.Ticks

			SyncLock _syncLock
				If now - _lastStatusMessageTs > TimeSpan.TicksPerSecond OrElse isEssential Then
					_lastStatusMessageTs = now
					_lastDocumentsExportedCountReported = Me.DocumentsExported
					RaiseEvent StatusMessage(New ExportEventArgs(Me.DocumentsExported, Me.TotalExportArtifactCount, line, e, _lastStatisticsSnapshot, Statistics))
				End If

				Me.LogStatusLine(e, line, isEssential)
			End SyncLock
		End Sub

		Friend Sub WriteError(ByVal line As String) Implements IStatus.WriteError
			Interlocked.Increment(_errorCount)
			WriteStatusLine(EventType2.Error, line, True)
		End Sub

		Friend Sub WriteImgProgressError(ByVal artifact As Exporters.ObjectExportInfo, ByVal imageIndex As Int32, ByVal ex As System.Exception, Optional ByVal notes As String = "") Implements IStatus.WriteImgProgressError
			Dim sw As New System.IO.StreamWriter(_exportFile.FolderPath & "\" & _exportFile.LoadFilesPrefix & "_img_errors.txt", True, _exportFile.LoadFileEncoding)
			sw.WriteLine(System.DateTime.Now.ToString("s"))
			sw.WriteLine(String.Format("DOCUMENT: {0}", artifact.IdentifierValue))
			If imageIndex > -1 AndAlso artifact.Images.Count > 0 Then
				sw.WriteLine(String.Format("IMAGE: {0} ({1} of {2})", artifact.Images(imageIndex), imageIndex + 1, artifact.Images.Count))
			End If
			If Not notes = "" Then sw.WriteLine("NOTES: " & notes)
			sw.WriteLine("ERROR: " & ex.ToString)
			sw.WriteLine("")
			sw.Flush()
			sw.Close()
			Dim errorLine As String = String.Format("Error processing images for document {0}: {1}. Check {2}_img_errors.txt for details", artifact.IdentifierValue, ex.Message.TrimEnd("."c), _exportFile.LoadFilesPrefix)
			Me.WriteError(errorLine)
		End Sub
		Friend Sub WriteWarningWithoutShowingExportedDocumentsCount(line As String)
			Interlocked.Increment(_warningCount)
			WriteStatusLine(EventType2.Warning, line, True, False)
		End Sub

		Friend Sub WriteWarning(ByVal line As String) Implements IStatus.WriteWarning
			Interlocked.Increment(_warningCount)
			WriteStatusLine(EventType2.Warning, line, True)
		End Sub

		Friend Sub WriteWarningWithoutDocCount(ByVal line As String) Implements IStatus.WriteWarningWithoutDocCount
			Interlocked.Increment(_warningCount)
			WriteStatusLineWithoutDocCount(EventType2.Warning, line, True)
		End Sub

		Friend Sub WriteUpdate(ByVal line As String, Optional ByVal isEssential As Boolean = True) Implements IStatus.WriteUpdate
			WriteStatusLine(EventType2.Progress, line, isEssential)
		End Sub

		Dim _statisticsLastUpdated As Date
		Protected Sub UpdateStatisticsSnapshot(time As System.DateTime)
			Dim updateCurrentStats As Boolean = (time.Ticks - _statisticsLastUpdated.Ticks) > TimeSpan.TicksPerSecond
			If updateCurrentStats Then
				_lastStatisticsSnapshot = Statistics.ToDictionary()
				_statisticsLastUpdated = time
				RaiseEvent StatusMessage(New ExportEventArgs(Me.DocumentsExported, Me.TotalExportArtifactCount, "", EventType2.Statistics, _lastStatisticsSnapshot, Statistics))
			End If
		End Sub

		Sub UpdateDocumentExportedCount(count As Int32) Implements IStatus.UpdateDocumentExportedCount
			DocumentsExported = count
			UpdateStatisticsSnapshot(DateTime.Now)
		End Sub

		Sub NotifyError(message As String) Implements IServiceNotification.NotifyError
			Me.WriteError(message)
		End Sub

		Sub NotifyStatus(message As String) Implements IServiceNotification.NotifyStatus
			Me.WriteStatusLineWithoutDocCount(EventType2.Status, message, True)
		End Sub

		Sub NotifyWarning(message As String) Implements IServiceNotification.NotifyWarning
			Me.WriteWarningWithoutDocCount(message)
		End Sub

#End Region

#Region "Public Events"

		Public Event FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception) Implements IExporterStatusNotification.FatalErrorEvent
		Public Event StatusMessage(ByVal exportArgs As ExportEventArgs) Implements IExporterStatusNotification.StatusMessage
		Public Event FileTransferMultiClientModeChangeEvent(ByVal sender As Object, ByVal args As Global.Relativity.DataExchange.Transfer.TapiMultiClientEventArgs) Implements IExporterStatusNotification.FileTransferMultiClientModeChangeEvent

#End Region

        Private Sub _processContext_OnCancellationRequest(ByVal sender As Object, ByVal e As CancellationRequestEventArgs) Handles _processContext.CancellationRequest
			_cancellationTokenSource.Cancel()
	        _cancelledByUser = e.RequestByUser
			If e.RequestByUser Then
				_logger.LogInformation("The export cancellation request event has been raised by the user.")
			Else
				_logger.LogVerbose("The export cancellation request event has been raised by the search process to perform standard cleanup.")
			End If
			
			If Not _volumeManager Is Nothing Then
				_volumeManager.Halt = True
			End If
		End Sub

		Private Sub _processContext_OnExportServerErrors(ByVal sender As Object, e As ExportErrorEventArgs) Handles _processContext.ExportServerErrors
			Dim sourcePath As String
			If UseOldExport Then
				sourcePath = _volumeManager.ErrorDestinationPath
			Else
				sourcePath = _errorFile.Path()
			End If
			Dim destinationPath As String = Path.Combine(e.Path, $"{Settings.LoadFilesPrefix}_errors.csv")
			FileHelper.Move(sourcePath, destinationPath)
		End Sub

		Private Sub _downloadModeStatus_UploadModeChangeEvent(ByVal sender As Object, ByVal args As TapiMultiClientEventArgs) Handles _downloadModeStatus.TransferModesChangeEvent
			RaiseEvent FileTransferMultiClientModeChangeEvent(Me, args)
        End Sub

		Private Function BuildFileNameProvider() As IFileNameProvider
			Dim identifierExportFileNameProvider As IFileNameProvider = New IdentifierExportFileNameProvider(Settings)
			Dim productionExportFileNameProvider As IFileNameProvider = New ProductionExportFileNameProvider(Settings, NameTextAndNativesAfterBegBates)
			Dim customExportFileNameProvider As IFileNameProvider = New CustomFileNameProvider(Settings.CustomFileNaming?.DescriptorParts().ToList(), New FileNamePartProviderContainer(), _exportFile.AppendOriginalFileName)
			Dim fileNameProvidersDictionary As New Dictionary(Of ExportNativeWithFilenameFrom, IFileNameProvider) From
				{
					{ExportNativeWithFilenameFrom.Identifier, identifierExportFileNameProvider},
					{ExportNativeWithFilenameFrom.Production, productionExportFileNameProvider},
					{ExportNativeWithFilenameFrom.Custom, customExportFileNameProvider}
				}

			Return New FileNameProviderContainer(Settings, fileNameProvidersDictionary)
		End Function
	End Class
End Namespace