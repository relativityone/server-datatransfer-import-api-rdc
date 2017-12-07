' ----------------------------------------------------------------------------
' <copyright file="ImportExportTapiBase.vb" company="kCura Corp">
'   kCura Corp (C) 2017 All Rights Reserved.
' </copyright>
' ----------------------------------------------------------------------------

Imports System.Threading
Imports kCura.WinEDDS.TApi
Imports Relativity.Logging

Namespace kCura.WinEDDS

	''' <summary>
	''' Represents the base-class object for all TAPI-based import and export instances.
	''' </summary>
	Public MustInherit Class ImportExportTapiBase

#Region "Members"
		Protected IoReporterInstance As IIoReporter
		Private ReadOnly _syncRoot As Object = New Object
		Private ReadOnly _cancellationToken As CancellationTokenSource
		Private ReadOnly _statistics As New Statistics
		Private WithEvents _bulkLoadTapiBridge As TapiBridge
		Private WithEvents _fileTapiBridge As TapiBridge
		Private _bulkLoadTapiClientName As String
		Private _fileTapiClient As TapiClient = TapiClient.None
		Private _fileTapiClientName As String
		Private _statisticsLastUpdated As DateTime = DateTime.Now
		Private ReadOnly _logger As ILog
#End Region

#Region "Constructor"
		Public Sub New(ByVal ioReporterInstance As IIoReporter, ByVal logger As ILog, cancellationToken As CancellationTokenSource)
			'There is no argument checks for ioReporterInstance and cancellationToken here as both of these are not used when the constructor is called in Application.vb for previewing the content of load file.
			If logger Is Nothing Then
				Throw New ArgumentNullException("logger")
			End If

			_logger = logger
			_cancellationToken = cancellationToken
			Me.IoReporterInstance = ioReporterInstance
		End Sub

#End Region

#Region "Properties"
		Public ReadOnly Property Statistics As Statistics
			Get
				Return _statistics
			End Get
		End Property

		Protected MustOverride ReadOnly Property CurrentLineNumber() As Integer

		Protected ReadOnly Property BulkLoadTapiBridge As TapiBridge
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
				Return _cancellationToken.Token
			End Get
		End Property

		Protected Property CurrentStatisticsSnapshot As IDictionary

		Protected Property JobCounter As Int32

		Protected ReadOnly Property Logger As ILog
			Get
				Return _logger
			End Get
		End Property

		Protected ReadOnly Property FileTapiBridge As TapiBridge
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

		Protected Property FileTapiProgressCount As Int32

		Protected Property ShouldImport As Boolean

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

		Protected Sub CompletePendingNativeFileTransfers()
			Try
				Me.OnWriteStatusMessage(kCura.Windows.Process.EventType.Status, "Waiting for all native files to upload...", 0, 0)
				Me.FileTapiBridge.WaitForTransferJob()
				Me.OnWriteStatusMessage(kCura.Windows.Process.EventType.Status, "Native file uploads completed.", 0, 0)
			Catch ex As Exception
				Me.LogError(ex, "Failed to complete all pending native file transfers.")
				Throw
			End Try
		End Sub

		Protected Sub CompletePendingBulkLoadFileTransfers()
			Try
				Me.OnWriteStatusMessage(kCura.Windows.Process.EventType.Status, "Waiting for all bulk load files to upload...", 0, 0)
				Me.BulkLoadTapiBridge.WaitForTransferJob()
				Me.OnWriteStatusMessage(kCura.Windows.Process.EventType.Status, "Bulk load file uploads completed.", 0, 0)
			Catch ex As Exception
				Me.LogError(ex, "Failed to complete all pending bulk load file transfers.")
				Throw
			End Try
		End Sub


		Protected Sub CreateTapiBridges(ByVal fileParameters As TapiBridgeParameters, ByVal bulkLoadParameters As TapiBridgeParameters)
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
			Logger.LogWarning("Import has been stopped")
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
				_cancellationToken.Cancel()
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
				If ShouldImport AndAlso e.Status Then
					Me.FileTapiProgressCount += 1
					WriteTapiProgressMessage($"End upload '{e.FileName}' file. ({System.DateTime.op_Subtraction(e.EndTime, e.StartTime).Milliseconds}ms)", e.LineNumber)
				End If
			End SyncLock
		End Sub

		Private Sub FileOnTapiStatistics(ByVal sender As Object, ByVal e As TapiStatisticsEventArgs)
			SyncLock _syncRoot
				_statistics.FileTime = e.TotalTransferTicks
				_statistics.FileBytes = e.TotalBytes
				Me.UpdateStatisticsSnapshot(System.DateTime.Now)
			End SyncLock
		End Sub

		Private Sub OnTapiErrorMessage(ByVal sender As Object, ByVal e As TapiMessageEventArgs)
			SyncLock _syncRoot
				If ShouldImport Then
					' TODO: verify raising errors.
					Me.OnWriteStatusMessage(kCura.Windows.Process.EventType.Error, e.Message, e.LineNumber, e.LineNumber)
					Me.LogError(e.Message)
				End If
			End SyncLock
		End Sub

		Private Sub OnTapiWarningMessage(ByVal sender As Object, ByVal e As TapiMessageEventArgs)
			SyncLock _syncRoot
				If ShouldImport Then
					' TODO: verify raising warnings.
					Me.OnWriteStatusMessage(kCura.Windows.Process.EventType.Warning, e.Message, e.LineNumber, e.LineNumber)
					Me.LogWarning(e.Message)
				End If
			End SyncLock
		End Sub

		Private Sub OnTapiFatalError(ByVal sender As Object, ByVal e As TapiMessageEventArgs)
			SyncLock _syncRoot
				' TODO: verify raising fatal errors.
				Dim exception As Exception = New Exception(e.Message)
				OnWriteFatalError(exception)
				Me.LogFatal(exception, "A fatal error has occurred transferring files.")
			End SyncLock
		End Sub

		Private Sub OnTapiStatusEvent(ByVal sender As Object, ByVal e As TapiMessageEventArgs)
			SyncLock _syncRoot
				If ShouldImport Then
					' TODO: verify progress vs physical line number.
					Me.OnWriteStatusMessage(kCura.Windows.Process.EventType.Status, e.Message, e.LineNumber, e.LineNumber)
				End If
			End SyncLock
		End Sub

		Private Sub WriteTapiProgressMessage(ByVal message As String, ByVal lineNumber As Int32)
			message = GetLineMessage(message, lineNumber)
			Me.OnWriteStatusMessage(kCura.Windows.Process.EventType.Progress, message, FileTapiProgressCount, lineNumber)
		End Sub

		Private Function GetLineMessage(ByVal line As String, ByVal lineNumber As Int32) As String
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
			Dim message As String = TApi.IoReporter.BuildIoReporterWarningMessage(exception, timeoutSeconds, retryCount, totalRetryCount)
			IoReporterInstance.IOWarningPublisher?.PublishIoWarningEvent(New IoWarningEventArgs(message, CurrentLineNumber))
			System.Threading.Thread.CurrentThread.Join(1000 * timeoutSeconds)
		End Sub
	End Class
End Namespace