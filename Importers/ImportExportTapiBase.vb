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
        Private ReadOnly _syncRoot As Object = New Object
        Private ReadOnly _cancellationToken As CancellationTokenSource
        Private ReadOnly _statistics As New Statistics

        Private WithEvents _bulkLoadTapiBridge As TapiBridge
        Private WithEvents _fileTapiBridge As TapiBridge

        Private _bulkLoadTapiClientName As String

		Private _fileTapiClient As TapiClient = TapiClient.None
		Private _fileTapiClientName As String

		Private _statisticsLastUpdated As DateTime = DateTime.Now

		Protected IoReporter As IIoReporter

        Protected MustOverride ReadOnly Property CurrentLineNumber() As Integer
        private ReadOnly _logger As ILog
#End Region

#Region "Constructor"
        Public Sub New(ByRef ioReporter As IIoReporter, ByRef logger As ILog)
            _logger = logger
            _cancellationToken = New CancellationTokenSource()

            Me.IoReporter = ioReporter
        End Sub

#End Region

#Region "Properties"
        Public ReadOnly Property Statistics As Statistics
            Get
                Return _statistics
            End Get
        End Property

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

        Protected Sub CompletePendingNativeFileTransfers()
            Me.OnWriteStatusMessage(kCura.Windows.Process.EventType.Status, "Waiting for all files to upload...", 0, 0)
            Me.FileTapiBridge.WaitForTransferJob()
            Me.OnWriteStatusMessage(kCura.Windows.Process.EventType.Status, "File uploads completed.", 0, 0)
        End Sub

        Protected Sub CompletePendingBulkLoadFileTransfers()
            Me.OnWriteStatusMessage(kCura.Windows.Process.EventType.Status, "Waiting for all bulk load files to upload...", 0, 0)
            Me.BulkLoadTapiBridge.WaitForTransferJob()
            Me.OnWriteStatusMessage(kCura.Windows.Process.EventType.Status, "Bulk load file uploads completed.", 0, 0)
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

        Protected Sub UpdateStatisticsSnapshot(time As System.DateTime, Optional byval force As Boolean = false)
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

        Protected Overridable Sub RaiseWarningAndPause(ByVal ex As Exception, ByVal timeoutSeconds As Int32)
            IoReporter.IOWarningPublisher?.OnIoWarningEvent(New IoWarningEventArgs(TApi.IoReporter.BuildIoReporterWarningMessage(ex), CurrentLineNumber))
            System.Threading.Thread.CurrentThread.Join(1000 * timeoutSeconds)
        End Sub
    End Class
End Namespace