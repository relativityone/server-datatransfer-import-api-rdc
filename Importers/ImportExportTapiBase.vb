' ----------------------------------------------------------------------------
' <copyright file="ImportExportTapiBase.vb" company="kCura Corp">
'   kCura Corp (C) 2017 All Rights Reserved.
' </copyright>
' ----------------------------------------------------------------------------

Imports System.Threading
Imports kCura.WinEDDS.Helpers
Imports kCura.WinEDDS.TApi
Imports Polly
Imports Relativity.Logging
Imports Relativity.Transfer

Namespace kCura.WinEDDS

	''' <summary>
	''' Represents the base-class object for all TAPI-based import and export instances.
	''' </summary>
	Public MustInherit Class ImportExportTapiBase

#Region "Members"
		Private ReadOnly _ioReporter As IIoReporter
		Private ReadOnly _syncRoot As Object = New Object
		Private ReadOnly _fileSystem As kCura.WinEDDS.TApi.IFileSystem
		Private ReadOnly _cancellationTokenSource As CancellationTokenSource
		Private ReadOnly _statistics As New Statistics
		Private WithEvents _bulkLoadTapiBridge As UploadTapiBridge
		Private WithEvents _fileTapiBridge As UploadTapiBridge
		Private _bulkLoadTapiClientName As String
		Private _fileTapiClient As TapiClient = TapiClient.None
		Private _fileTapiClientName As String
		Private _statisticsLastUpdated As DateTime = DateTime.Now
		Private _fileCheckRetryCount As Int32 = 6000
		Private _fileCheckWaitBetweenRetries As Int32 = 10
		Private _batchFileTapiProgressCount As Int32 = 0
		Private _batchMetadataTapiProgressCount As Int32 = 0
		Private ReadOnly _logger As ILog
		Private ReadOnly _filePathHelper As IFilePathHelper = New ConfigurableFilePathHelper()
#End Region

		Public Event UploadModeChangeEvent(ByVal statusBarText As String, ByVal tapiClientName As String, ByVal isBulkEnabled As Boolean)

#Region "Constructor"
		Public Sub New(ByVal reporter As IIoReporter, ByVal logger As ILog, cancellationTokenSource As CancellationTokenSource)

			' TODO: Refactor all core constructors to use a single config object
			' TODO: once IAPI/RDC is moved into the new repo.
			_fileSystem = kCura.WinEDDS.TApi.FileSystem.Instance.DeepCopy()
			If reporter Is Nothing Then
				reporter = New NullIoReporter(_fileSystem)
			End If

			If logger Is Nothing Then
				Throw New ArgumentNullException("logger")
			End If

			If cancellationTokenSource Is Nothing Then
				cancellationTokenSource = New CancellationTokenSource()
			End If

			_logger = logger
			_cancellationTokenSource = cancellationTokenSource
			_ioReporter = reporter
		End Sub

#End Region

#Region "Properties"
		Public ReadOnly Property Statistics As Statistics
			Get
				Return _statistics
			End Get
		End Property

		Protected MustOverride ReadOnly Property CurrentLineNumber() As Integer

		Protected ReadOnly Property BulkLoadTapiBridge As UploadTapiBridge
			Get
				Return _bulkLoadTapiBridge
			End Get
		End Property

		Protected ReadOnly Property BulkLoadTapiClientName As String
			Get
				Return _bulkLoadTapiClientName
			End Get
		End Property

		Protected ReadOnly Property CancellationToken As CancellationToken
			Get
				Return _cancellationTokenSource.Token
			End Get
		End Property

		Protected Property CurrentStatisticsSnapshot As IDictionary

		Protected Property JobCounter As Int32

		Protected ReadOnly Property Logger As ILog
			Get
				Return _logger
			End Get
		End Property

		Protected ReadOnly Property FileTapiBridge As UploadTapiBridge
			Get
				Return _fileTapiBridge
			End Get
		End Property

		Protected ReadOnly Property FileTapiClient As TapiClient
			Get
				Return _fileTapiClient
			End Get
		End Property

		Protected ReadOnly Property FileTapiClientName As String
			Get
				Return _fileTapiClientName
			End Get
		End Property

		Public ReadOnly Property TapiClientName As String
			Get
				Return If(If(FileTapiClientName, BulkLoadTapiClientName), TapiClient.None.ToString())
			End Get
		End Property


		Protected Property FileTapiProgressCount As Int32

		Protected Property TotalTransferredFilesCount As Long

		Protected Property ShouldImport As Boolean

		Protected Property ImportFilesCount As Int32 = 0

		Protected Property MetadataFilesCount As Int32 = 0
#End Region

		Protected Shared Function IsTimeoutException(ByVal ex As Exception) As Boolean
			If ex.GetType = GetType(Service.BulkImportManager.BulkImportSqlTimeoutException) Then
				Return True
			ElseIf TypeOf ex Is System.Net.WebException AndAlso ex.Message.ToString.Contains("timed out") Then
				Return True
			Else
				Return False
			End If
		End Function

		Protected Shared Function IsBulkImportSqlException(ByVal ex As Exception) As Boolean
			If ex.GetType = GetType(Service.BulkImportManager.BulkImportSqlException) Then
				Return True
			Else
				Return False
			End If
		End Function

		Protected Shared Function IsInsufficientPermissionsForImportException(ByVal ex As Exception) As Boolean
			If ex.GetType = GetType(Service.BulkImportManager.InsufficientPermissionsForImportException) Then
				Return True
			Else
				Return False
			End If
		End Function

		''' <summary>
		''' Copies an existing file to a new file. Overwriting a file of the same name is not allowed.
		''' </summary>
		''' <param name="sourceFileName">
		''' The file to copy.
		''' </param>
		''' <param name="destFileName">
		''' The name of the destination file. This cannot be a directory.
		''' </param>
		''' <param name="retry">
		''' <see langword="true" /> to retry <see cref="T:System.IO.IOException"/> and publish error messages; otherwise, <see langword="false" />.
		''' </param>
		Protected Sub CopyFile(sourceFileName As String, destFileName As String, retry As Boolean)
			Const overwrite As Boolean = False
			Me.CopyFile(sourceFileName, destFileName, overwrite, retry)
		End Sub

		''' <summary>
		''' Copies an existing file to a new file. Overwriting a file of the same name is allowed.
		''' </summary>
		''' <param name="sourceFileName">
		''' The file to copy.
		''' </param>
		''' <param name="destFileName">
		''' The name of the destination file. This cannot be a directory.
		''' </param>
		''' <param name="overwrite">
		''' <see langword="true" /> if the destination file can be overwritten; otherwise, <see langword="false" />.
		''' </param>
		''' <param name="retry">
		''' <see langword="true" /> to retry <see cref="T:System.IO.IOException"/> and publish error messages; otherwise, <see langword="false" />.
		''' </param>
		Protected Sub CopyFile(sourceFileName As String, destFileName As String, overwrite As Boolean, retry As Boolean)
			If Not retry
				_fileSystem.File.Copy(sourceFileName, destFileName, overwrite)
			Else
				_ioReporter.CopyFile(sourceFileName, destFileName, overwrite, Me.CurrentLineNumber)
			End If
		End Sub

		''' <summary>
		''' Retrieves the case-sensitive or case-insensitive file path and optionally retries all thrown <see cref="T:System.IO.IOException"/>.
		''' </summary>
		''' <param name="path">
		''' The path used to retrieve the case-sensitive or case-insensitive file path.
		''' </param>
		''' <param name="retry">
		''' <see langword="true" /> to retry <see cref="T:System.IO.IOException"/> and publish error messages; otherwise, <see langword="false" />.
		''' </param>
		''' <returns>
		''' The fil path.
		''' </returns>
		Protected Function GetExistingFilePath(path As String, retry As Boolean) As String
			If Not retry
				Return _filePathHelper.GetExistingFilePath(path)
			Else
				' REL-272765: Added I/O resiliency and support document level errors.
				Dim maxRetryAttempts As Integer = kCura.Utility.Config.IOErrorNumberOfRetries
				Dim currentRetryAttempt As Integer = 0
				Dim policy As IWaitAndRetryPolicy = New WaitAndRetryPolicy(
					maxRetryAttempts, _
					kCura.Utility.Config.IOErrorWaitTimeInSeconds)
				Dim returnExistingPath As String = policy.WaitAndRetry(Of String)(
					RetryPolicies.IoStandardPolicy,								
					Function(count)
						currentRetryAttempt = count
						Return TimeSpan.FromSeconds(kCura.Utility.Config.IOErrorWaitTimeInSeconds)
					End Function,
					Sub(exception, span)
						Me.PublishIoRetryMessage(exception, span, currentRetryAttempt, maxRetryAttempts)
					End Sub,
					Function(token) As String					
						Return _filePathHelper.GetExistingFilePath(path)
					End Function,
					Me.CancellationToken)
				Return returnExistingPath
			End If
		End Function

		''' <summary>
		''' Determines whether the specified file exists and optionally retries all thrown <see cref="T:System.IO.IOException"/>.
		''' </summary>
		''' <param name="path">
		''' The file to check.
		''' </param>
		''' <param name="retry">
		''' <see langword="true" /> to retry <see cref="T:System.IO.IOException"/> and publish error messages; otherwise, <see langword="false" />.
		''' </param>
		''' <returns>
		''' <see langword="true" /> if the caller has the required permissions and path contains the name of an existing file; otherwise, <see langword="false" />.
		''' </returns>
		Protected Function GetFileExists(path As String, retry As Boolean) As Boolean
			If Not retry
				Return _fileSystem.File.Exists(path)
			Else
				Return _ioReporter.GetFileExists(path, Me.CurrentLineNumber)
			End If
		End Function

		''' <summary>
		''' Retrieves the specified file length and optionally retries all thrown <see cref="T:System.IO.IOException"/>.
		''' </summary>
		''' <param name="path">
		''' The file to retrieve file length information.
		''' </param>
		''' <param name="retry">
		''' <see langword="true" /> to retry <see cref="T:System.IO.IOException"/> and publish error messages; otherwise, <see langword="false" />.
		''' </param>
		''' <returns>
		''' The file length.
		''' </returns>
		Protected Function GetFileLength(path As String, retry As Boolean) As Long
			If Not retry Then
				' Note: this is always non-null even if the file doesn't exist.
				' Note: always allow the System.IO.FileNotFoundException to throw.
				Dim fileInfo As kCura.WinEDDS.TApi.IFileInfo = _fileSystem.CreateFileInfo(path)
				Return fileInfo.Length
			Else
				Return _ioReporter.GetFileLength(path, Me.CurrentLineNumber)
			End If
		End Function

		''' <summary>
		''' Publishes a retry-based warning message and logs the exception.
		''' </summary>
		''' <param name="exception">
		''' The exception to publish and log.
		''' </param>
		''' <param name="timeSpan">
		''' The time span between retry attempts.
		''' </param>
		''' <param name="retryCount">
		''' The current retry count.
		''' </param>
		''' <param name="totalRetryCount">
		''' The total retry count.
		''' </param>
		Protected Sub PublishIoRetryMessage(exception As Exception, timeSpan As TimeSpan, retryCount As Integer, totalRetryCount As Integer)
			_ioReporter.PublishRetryMessage(exception, timeSpan, retryCount, totalRetryCount, Me.CurrentLineNumber)
		End Sub

		''' <summary>
		''' Publishes a raw warning message and logs the information.
		''' </summary>
		''' <param name="args">
		''' The warning event data.
		''' </param>
		Protected Sub PublishIoWarningEvent(args As TApi.IoWarningEventArgs)
			_ioReporter.PublishWarningMessage(args)
		End Sub

		Protected Sub CompletePendingPhysicalFileTransfers(waitingMessage As String, completedMessage As String, errorMessage As String)
			Try
				Me.OnWriteStatusMessage(kCura.Windows.Process.EventType.Status, waitingMessage, 0, 0)
				Me.FileTapiBridge.WaitForTransferJob()
				Me.OnWriteStatusMessage(kCura.Windows.Process.EventType.Status, completedMessage, 0, 0)
			Catch ex As Exception
				Me.LogError(ex, errorMessage)
				Throw
			End Try
		End Sub

		Protected Sub CompletePendingBulkLoadFileTransfers()
			Try
				Me.OnWriteStatusMessage(kCura.Windows.Process.EventType.Status, "Waiting for the bulk load job to complete...", 0, 0)
				Me.BulkLoadTapiBridge.WaitForTransferJob()
				Me.OnWriteStatusMessage(kCura.Windows.Process.EventType.Status, "Bulk load file job completed.", 0, 0)
			Catch ex As Exception
				Me.LogError(ex, "Failed to complete all pending bulk load file transfers.")
				Throw
			End Try
		End Sub


		Protected Sub CreateTapiBridges(ByVal fileParameters As UploadTapiBridgeParameters, ByVal bulkLoadParameters As UploadTapiBridgeParameters)
			_fileTapiBridge = TapiBridgeFactory.CreateUploadBridge(fileParameters, Me.Logger, Me.CancellationToken)
			AddHandler _fileTapiBridge.TapiClientChanged, AddressOf FileOnTapiClientChanged
			AddHandler _fileTapiBridge.TapiFatalError, AddressOf OnTapiFatalError
			AddHandler _fileTapiBridge.TapiProgress, AddressOf FileOnTapiProgress
			AddHandler _fileTapiBridge.TapiStatistics, AddressOf FileOnTapiStatistics
			AddHandler _fileTapiBridge.TapiStatusMessage, AddressOf OnTapiStatusEvent
			AddHandler _fileTapiBridge.TapiErrorMessage, AddressOf OnTapiErrorMessage
			AddHandler _fileTapiBridge.TapiWarningMessage, AddressOf OnTapiWarningMessage

			_bulkLoadTapiBridge = TapiBridgeFactory.CreateUploadBridge(bulkLoadParameters, Me.Logger, Me.CancellationToken)
			_bulkLoadTapiBridge.TargetPath = bulkLoadParameters.FileShare
			AddHandler _bulkLoadTapiBridge.TapiClientChanged, AddressOf BulkLoadOnTapiClientChanged
			AddHandler _bulkLoadTapiBridge.TapiStatistics, AddressOf BulkLoadOnTapiStatistics
			AddHandler _bulkLoadTapiBridge.TapiFatalError, AddressOf OnTapiFatalError
			AddHandler _bulkLoadTapiBridge.TapiStatusMessage, AddressOf OnTapiStatusEvent
			AddHandler _bulkLoadTapiBridge.TapiErrorMessage, AddressOf OnTapiErrorMessage
			AddHandler _bulkLoadTapiBridge.TapiWarningMessage, AddressOf OnTapiWarningMessage
			AddHandler _bulkLoadTapiBridge.TapiProgress, AddressOf BulkLoadOnTapiProgress

			' Dump native and bcp upload bridge
			Me.LogInformation("Begin dumping native parameters.")
			_fileTapiBridge.DumpInfo()

			Me.LogInformation("Begin dumping bcp parameters.")
			_bulkLoadTapiBridge.DumpInfo()
		End Sub

		Protected Sub DestroyTapiBridges()
			If Not _fileTapiBridge Is Nothing Then
				RemoveHandler _fileTapiBridge.TapiClientChanged, AddressOf FileOnTapiClientChanged
				RemoveHandler _fileTapiBridge.TapiFatalError, AddressOf OnTapiFatalError
				RemoveHandler _fileTapiBridge.TapiProgress, AddressOf FileOnTapiProgress
				RemoveHandler _fileTapiBridge.TapiStatistics, AddressOf FileOnTapiStatistics
				RemoveHandler _fileTapiBridge.TapiStatusMessage, AddressOf OnTapiStatusEvent
				RemoveHandler _fileTapiBridge.TapiErrorMessage, AddressOf OnTapiErrorMessage
				RemoveHandler _fileTapiBridge.TapiWarningMessage, AddressOf OnTapiWarningMessage
				_fileTapiBridge.Dispose()
				_fileTapiBridge = Nothing
			End If

			If Not _bulkLoadTapiBridge Is Nothing Then
				RemoveHandler _bulkLoadTapiBridge.TapiClientChanged, AddressOf BulkLoadOnTapiClientChanged
				RemoveHandler _bulkLoadTapiBridge.TapiStatistics, AddressOf BulkLoadOnTapiStatistics
				RemoveHandler _bulkLoadTapiBridge.TapiFatalError, AddressOf OnTapiFatalError
				RemoveHandler _bulkLoadTapiBridge.TapiStatusMessage, AddressOf OnTapiStatusEvent
				RemoveHandler _bulkLoadTapiBridge.TapiErrorMessage, AddressOf OnTapiErrorMessage
				RemoveHandler _bulkLoadTapiBridge.TapiWarningMessage, AddressOf OnTapiWarningMessage
				RemoveHandler _bulkLoadTapiBridge.TapiProgress, AddressOf BulkLoadOnTapiProgress
				_bulkLoadTapiBridge.Dispose()
				_bulkLoadTapiBridge = Nothing
			End If
		End Sub

		''' <summary>
		''' Dump the statistic object.
		''' </summary>
		Protected Sub DumpStatisticsInfo()
			Me.LogInformation("Statistics info:")
			Me.LogInformation("Document count: {DocCount}", _statistics.DocCount)
			Me.LogInformation("Documents created: {DocsCreated}", _statistics.DocumentsCreated)
			Me.LogInformation("Documents updated: {DocsUpdated}", _statistics.DocumentsUpdated)
			Me.LogInformation("Files processed: {FilesProcessed}", _statistics.FilesProcessed)

			Dim pair As DictionaryEntry
			For Each pair In _statistics.ToDictionary()
				Me.LogInformation("{StatsKey}: {StatsValue}", pair.Key, pair.Value)
			Next
		End Sub

		Protected Sub LogInformation(ByVal exception As System.Exception, ByVal messageTemplate As String, ParamArray propertyValues As Object())
			_logger.LogInformation(exception, messageTemplate, propertyValues)
		End Sub

		Protected Sub LogInformation(ByVal messageTemplate As String, ParamArray propertyValues As Object())
			_logger.LogInformation(messageTemplate, propertyValues)
		End Sub

		Protected Sub LogError(ByVal exception As System.Exception, ByVal messageTemplate As String, ParamArray propertyValues As Object())
			_logger.LogError(exception, messageTemplate, propertyValues)
		End Sub

		Protected Sub LogError(ByVal messageTemplate As String, ParamArray propertyValues As Object())
			_logger.LogError(messageTemplate, propertyValues)
		End Sub

		Protected Sub LogFatal(ByVal exception As System.Exception, ByVal messageTemplate As String, ParamArray propertyValues As Object())
			_logger.LogFatal(exception, messageTemplate, propertyValues)
		End Sub

		Protected Sub LogFatal(ByVal messageTemplate As String, ParamArray propertyValues As Object())
			_logger.LogFatal(messageTemplate, propertyValues)
		End Sub

		Protected Sub LogWarning(ByVal exception As System.Exception, ByVal messageTemplate As String, ParamArray propertyValues As Object())
			_logger.LogWarning(exception, messageTemplate, propertyValues)
		End Sub

		Protected Sub LogWarning(ByVal messageTemplate As String, ParamArray propertyValues As Object())
			_logger.LogWarning(messageTemplate, propertyValues)
		End Sub

		Protected Overridable Sub OnStopImport()
			Logger.LogWarning("Import has been stopped.")
		End Sub

		Protected Overridable Sub OnTapiClientChanged()
			Logger.LogWarning($"Tapi client has been changed.")
		End Sub

		Protected Overridable Sub OnWriteStatusMessage(ByVal eventType As kCura.Windows.Process.EventType, ByVal message As String)
			'TODO Log this action and call from the derivered class.
		End Sub

		Protected Overridable Sub OnWriteStatusMessage(ByVal eventType As kCura.Windows.Process.EventType, ByVal message As String, ByVal progressLineNumber As Int32, ByVal physicalLineNumber As Int32)
			'TODO Log this action and call from the derivered class.
		End Sub

		Protected Overridable Sub OnWriteFatalError(ByVal exception As Exception)
			Logger.LogFatal($"Fatal error occured in line {CurrentLineNumber}, exception message {exception.Message}")
		End Sub

		Protected Sub StopImport()
			Try
				ShouldImport = False
				OnStopImport()
				_cancellationTokenSource.Cancel()
			Catch ex As Exception
				OnWriteStatusMessage(kCura.Windows.Process.EventType.Status, "Error occured while stopping the job.")
				Throw
			End Try
		End Sub

		Protected Sub UpdateStatisticsSnapshot(time As System.DateTime, Optional ByVal force As Boolean = False)
			Dim updateCurrentStats As Boolean = (time.Ticks - _statisticsLastUpdated.Ticks) > 10000000
			If updateCurrentStats OrElse force Then
				CurrentStatisticsSnapshot = Me.Statistics.ToDictionary()
				_statisticsLastUpdated = time
				Me.OnWriteStatusMessage(kCura.Windows.Process.EventType.Statistics, "", 0, 0)
			End If
		End Sub

		Private Sub BulkLoadOnTapiClientChanged(ByVal sender As Object, ByVal e As TapiClientEventArgs)
			Me._bulkLoadTapiClientName = e.Name
			Me.OnTapiClientChanged()
		End Sub

		Private Sub BulkLoadOnTapiStatistics(ByVal sender As Object, ByVal e As TapiStatisticsEventArgs)
			SyncLock _syncRoot
				_statistics.MetadataTime = e.TotalTransferTicks
				_statistics.MetadataBytes = e.TotalBytes
				_statistics.MetadataThroughput = e.TransferRateBytes
				Me.UpdateStatisticsSnapshot(System.DateTime.Now)
			End SyncLock
		End Sub

		Private Sub FileOnTapiClientChanged(ByVal sender As Object, ByVal e As TapiClientEventArgs)
			Me._fileTapiClient = e.Client
			Me._fileTapiClientName = e.Name
			Me.OnTapiClientChanged()
		End Sub

		Private Sub FileOnTapiProgress(ByVal sender As Object, ByVal e As TapiProgressEventArgs)
			SyncLock _syncRoot
				If DidFileComplete(e.TransferStatus) Then
					_batchFileTapiProgressCount += 1
				End If

				If ShouldImport AndAlso e.DidTransferSucceed Then
					Me.FileTapiProgressCount += 1
					WriteTapiProgressMessage($"End upload '{e.FileName}' file. ({System.DateTime.op_Subtraction(e.EndTime, e.StartTime).Milliseconds}ms)", e.LineNumber)
				End If
			End SyncLock
		End Sub

		Private Sub BulkLoadOnTapiProgress(ByVal sender As Object, ByVal e As TapiProgressEventArgs)
			SyncLock _syncRoot
				If DidFileComplete(e.TransferStatus) Then
					_batchMetadataTapiProgressCount += 1
				End If
			End SyncLock
		End Sub

		Private Sub FileOnTapiStatistics(ByVal sender As Object, ByVal e As TapiStatisticsEventArgs)
			SyncLock _syncRoot
				_statistics.FileTime = e.TotalTransferTicks
				_statistics.FileBytes = e.TotalBytes
				_statistics.FileThroughput = e.TransferRateBytes
				Me.UpdateStatisticsSnapshot(System.DateTime.Now)
			End SyncLock
		End Sub

		Private Sub OnTapiErrorMessage(ByVal sender As Object, ByVal e As TapiMessageEventArgs)
			SyncLock _syncRoot
				If ShouldImport Then
					Me.OnWriteStatusMessage(kCura.Windows.Process.EventType.Error, e.Message, e.LineNumber, e.LineNumber)
					Me.LogError(e.Message)
				End If
			End SyncLock
		End Sub

		Private Sub OnTapiWarningMessage(ByVal sender As Object, ByVal e As TapiMessageEventArgs)
			SyncLock _syncRoot
				If ShouldImport Then
					Me.OnWriteStatusMessage(kCura.Windows.Process.EventType.Warning, e.Message, e.LineNumber, e.LineNumber)
					Me.LogWarning(e.Message)
				End If
			End SyncLock
		End Sub

		Private Sub OnTapiFatalError(ByVal sender As Object, ByVal e As TapiMessageEventArgs)
			SyncLock _syncRoot
				Dim exception As Exception = New Exception(e.Message)
				OnWriteFatalError(exception)
				Me.LogFatal(exception, "A fatal error has occurred transferring files.")
			End SyncLock
		End Sub

		Private Sub OnTapiStatusEvent(ByVal sender As Object, ByVal e As TapiMessageEventArgs)
			SyncLock _syncRoot
				If ShouldImport Then
					Me.OnWriteStatusMessage(kCura.Windows.Process.EventType.Status, e.Message, e.LineNumber, e.LineNumber)
				End If
			End SyncLock
		End Sub

		Private Sub WriteTapiProgressMessage(ByVal message As String, ByVal lineNumber As Int32)
			Me.OnWriteStatusMessage(kCura.Windows.Process.EventType.Progress, message, FileTapiProgressCount, lineNumber)
		End Sub

		Protected Function GetLineMessage(ByVal line As String, ByVal lineNumber As Int32) As String
			If lineNumber = TapiConstants.NoLineNumber Then
				line = line & $" [job {Me.JobCounter}]"
			Else
				line = line & $" [line {lineNumber}]"
			End If
			Return line
		End Function

		Protected Overridable Sub RaiseWarningAndPause(ByVal exception As Exception, ByVal timeoutSeconds As Int32)
			Me.RaiseWarningAndPause(exception, timeoutSeconds, -1, -1)
		End Sub

		Protected Overridable Sub RaiseWarningAndPause(ByVal exception As Exception, ByVal timeoutSeconds As Int32, ByVal retryCount As Int32, ByVal totalRetryCount As Int32)
			Me.PublishIoRetryMessage(exception, TimeSpan.FromSeconds(timeoutSeconds), retryCount, totalRetryCount)
			System.Threading.Thread.CurrentThread.Join(1000 * timeoutSeconds)
		End Sub

		Protected Sub OnUploadModeChangeEvent(statusBarText As String, isBulkEnabled As Boolean)
			RaiseEvent UploadModeChangeEvent(statusBarText, TapiClientName, isBulkEnabled)
		End Sub

		Public Sub WaitForPendingMetadataUploads()
			WaitForRetry(Function()
							 Return _batchMetadataTapiProgressCount >= Me.MetadataFilesCount
						 End Function,
						 "Waiting for all metadata files to upload...",
						 "Metadata file uploads completed.",
						 "Unable to successfully wait for pending metadata uploads",
						 _fileCheckRetryCount,
						 _fileCheckWaitBetweenRetries)

			_batchMetadataTapiProgressCount = 0
			Me.MetadataFilesCount = 0
		End Sub

		Public Sub WaitForPendingFileUploads()
			WaitForRetry(Function()
							 Return _batchFileTapiProgressCount >= Me.ImportFilesCount
						 End Function,
						 "Waiting for all files to upload...",
						 "File uploads completed.",
						 "Unable to successfully wait for pending uploads",
						 _fileCheckRetryCount,
						 _fileCheckWaitBetweenRetries)

			_batchFileTapiProgressCount = 0
			Me.ImportFilesCount = 0
		End Sub

		Public Sub WaitForRetry(ByVal retryFunction As Func(Of Boolean),
								ByVal startMessage As String,
								ByVal stopMessage As String,
								ByVal errorMessage As String,
								ByVal retryCount As Int32,
								ByVal waitBetweenRetries As Int32)
			Dim waitSuccess As Boolean = False

			Dim retryPolicy As Retry.RetryPolicy(Of Boolean) = Policy.HandleResult(False).WaitAndRetry(
				retryCount,
				Function(count) As TimeSpan
					Return TimeSpan.FromMilliseconds(waitBetweenRetries)
				End Function)

			Me.OnWriteStatusMessage(kCura.Windows.Process.EventType.Status, startMessage, 0, 0)

			retryPolicy.Execute(Function()
									waitSuccess = retryFunction()

									Return waitSuccess
								End Function, _cancellationTokenSource.Token)

			Me.OnWriteStatusMessage(kCura.Windows.Process.EventType.Status, stopMessage, 0, 0)

			If Not waitSuccess Then
				Me.LogWarning(errorMessage)
			End If
		End Sub

		Private Function DidFileComplete(ByVal status As TransferPathStatus) As Boolean
			If status = TransferPathStatus.Failed Or
				status = TransferPathStatus.FileNotFound Or
				status = TransferPathStatus.Skipped Or
				status = TransferPathStatus.Successful Or
				status = TransferPathStatus.Failed Then
				Return True
			End If

			Return False
		End Function
	End Class
End Namespace