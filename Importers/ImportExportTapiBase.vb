' ----------------------------------------------------------------------------
' <copyright file="ImportExportTapiBase.vb" company="kCura Corp">
'   kCura Corp (C) 2017 All Rights Reserved.
' </copyright>
' ----------------------------------------------------------------------------

Imports System.Threading
Imports kCura.WinEDDS.TApi

Namespace kCura.WinEDDS

	''' <summary>
	''' Represents the base-class object for all TAPI-based import and export instances.
	''' </summary>
	Public MustInherit Class ImportExportTapiBase
		Inherits kCura.Utility.RobustIoReporter

		Private ReadOnly _syncRoot As Object = New Object
		Private ReadOnly _logger As Relativity.Logging.ILog
		Private ReadOnly _cancellationToken As System.Threading.CancellationTokenSource
		Private ReadOnly _statistics As New kCura.WinEDDS.Statistics

		Private WithEvents _bulkLoadTapiBridge As TApi.TapiBridge
		Private WithEvents _nativeTapiBridge As TApi.TapiBridge

		Private _bulkLoadTapiClientName As String
		Private _jobCounter As Int32
		Private _nativeTapiClientName As String
		Private _nativeTapiClient As TApi.TapiClient = TApi.TapiClient.None
		Private _nativeTapiProgressCount As Int32
		Private _shouldImport As Boolean
		Private _statisticsLastUpdated As System.DateTime = System.DateTime.Now
		Private _currentStatisticsSnapshot As IDictionary

		Public Sub New()
			' This must be constructed early. Do NOT arbitrarily move this call!
			_logger = RelativityLogFactory.CreateLog("WinEDDS")
			_cancellationToken = New CancellationTokenSource()
		End Sub

		Public ReadOnly Property Statistics As kCura.WinEDDS.Statistics
			Get
				Return _statistics
			End Get
		End Property

		Protected ReadOnly Property BulkLoadTapiBridge As TApi.TapiBridge
			Get
				Return _bulkLoadTapiBridge
			End Get
		End Property

		Protected ReadOnly Property BulkLoadTapiClientName As System.String
			Get
				Return _bulkLoadTapiClientName
			End Get
		End Property

		Protected ReadOnly Property CancellationToken As System.Threading.CancellationToken
			Get
				Return _cancellationToken.Token
			End Get
		End Property

		Protected Property CurrentStatisticsSnapshot As IDictionary
			Get
				Return _currentStatisticsSnapshot
			End Get
			Private Set
				_currentStatisticsSnapshot = value
			End Set
		End Property

		Protected Property JobCounter As Int32
			Get
				Return _jobCounter
			End Get
			Set
				_jobCounter = value
			End Set
		End Property

		Protected ReadOnly Property Logger As Relativity.Logging.ILog
			Get
				Return _logger
			End Get
		End Property

		Protected ReadOnly Property NativeTapiBridge As TApi.TapiBridge
			Get
				Return _nativeTapiBridge
			End Get
		End Property

		Protected ReadOnly Property NativeTapiClient As TApi.TapiClient
			Get
				Return _nativeTapiClient
			End Get
		End Property

		Protected ReadOnly Property NativeTapiClientName As System.String
			Get
				Return _nativeTapiClientName
			End Get
		End Property

		Protected Property NativeTapiProgressCount As Int32
			Get
				Return _nativeTapiProgressCount
			End Get
			Set
				_nativeTapiProgressCount = value
			End Set
		End Property

		Protected Property ShouldImport As Boolean
			Get
				Return _shouldImport
			End Get
			Set
				_shouldImport = value
			End Set
		End Property

		Protected Sub CompletePendingNativeFileTransfers()
			Me.OnWriteStatusMessage(kCura.Windows.Process.EventType.Status, "Waiting for all native files to upload...", 0, 0)
			Me.NativeTapiBridge.WaitForTransferJob()
			Me.OnWriteStatusMessage(kCura.Windows.Process.EventType.Status, "Native file uploads completed.", 0, 0)
		End Sub

		Protected Sub CompletePendingBulkLoadFileTransfers()
			Me.OnWriteStatusMessage(kCura.Windows.Process.EventType.Status, "Waiting for all bulk load files to upload...", 0, 0)
			Me.BulkLoadTapiBridge.WaitForTransferJob()
			Me.OnWriteStatusMessage(kCura.Windows.Process.EventType.Status, "Bulk load file uploads completed.", 0, 0)
		End Sub


		Protected Sub CreateTapiBridges(ByVal nativeParameters As TapiBridgeParameters, ByVal bulkLoadParameters As TapiBridgeParameters)
			_nativeTapiBridge = TApi.TapiBridgeFactory.CreateUploadBridge(nativeParameters, Me.Logger, Me.CancellationToken)
			AddHandler _nativeTapiBridge.TapiClientChanged, AddressOf NativeOnTapiClientChanged
			AddHandler _nativeTapiBridge.TapiFatalError, AddressOf OnTapiFatalError
			AddHandler _nativeTapiBridge.TapiProgress, AddressOf NativeOnTapiProgress
			AddHandler _nativeTapiBridge.TapiStatistics, AddressOf NativeOnTapiStatistics
			AddHandler _nativeTapiBridge.TapiStatusMessage, AddressOf OnTapiStatusEvent
			AddHandler _nativeTapiBridge.TapiErrorMessage, AddressOf OnTapiErrorMessage
			AddHandler _nativeTapiBridge.TapiWarningMessage, AddressOf OnTapiWarningMessage

			_bulkLoadTapiBridge = TApi.TapiBridgeFactory.CreateUploadBridge(bulkLoadParameters, Me.Logger, Me.CancellationToken)
			_bulkLoadTapiBridge.TargetPath = bulkLoadParameters.FileShare
			AddHandler _bulkLoadTapiBridge.TapiClientChanged, AddressOf BulkLoadOnTapiClientChanged
			AddHandler _bulkLoadTapiBridge.TapiStatistics, AddressOf BulkLoadOnTapiStatistics
			AddHandler _bulkLoadTapiBridge.TapiFatalError, AddressOf OnTapiFatalError
			AddHandler _bulkLoadTapiBridge.TapiStatusMessage, AddressOf OnTapiStatusEvent
			AddHandler _bulkLoadTapiBridge.TapiErrorMessage, AddressOf OnTapiErrorMessage
			AddHandler _bulkLoadTapiBridge.TapiWarningMessage, AddressOf OnTapiWarningMessage
		End Sub

		Protected Sub DestroyTapiBridges()
			If Not _nativeTapiBridge Is Nothing Then
				RemoveHandler _nativeTapiBridge.TapiClientChanged, AddressOf NativeOnTapiClientChanged
				RemoveHandler _nativeTapiBridge.TapiFatalError, AddressOf OnTapiFatalError
				RemoveHandler _nativeTapiBridge.TapiProgress, AddressOf NativeOnTapiProgress
				RemoveHandler _nativeTapiBridge.TapiStatistics, AddressOf NativeOnTapiStatistics
				RemoveHandler _nativeTapiBridge.TapiStatusMessage, AddressOf OnTapiStatusEvent
				RemoveHandler _nativeTapiBridge.TapiErrorMessage, AddressOf OnTapiErrorMessage
				RemoveHandler _nativeTapiBridge.TapiWarningMessage, AddressOf OnTapiWarningMessage
				_nativeTapiBridge.Dispose()
				_nativeTapiBridge = Nothing
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

		Protected MustOverride Sub OnStopImport()

		Protected MustOverride Sub OnTapiClientChanged()

		Protected MustOverride Sub OnWriteStatusMessage(ByVal eventType As kCura.Windows.Process.EventType, ByVal message As String)

		Protected MustOverride Sub OnWriteStatusMessage(ByVal eventType As kCura.Windows.Process.EventType, ByVal message As String, ByVal progressLineNumber As Int32, ByVal physicalLineNumber As Int32)

		Protected MustOverride Sub OnWriteFatalError(ByVal exception As Exception)

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

		Protected Sub UpdateStatisticsSnapshot(time As System.DateTime, force As Boolean)
			Dim updateCurrentStats As Boolean = (time.Ticks - _statisticsLastUpdated.Ticks) > 10000000
			If updateCurrentStats OrElse force Then
				CurrentStatisticsSnapshot = Me.Statistics.ToDictionary()
				_statisticsLastUpdated = time
			End If
		End Sub

		Private Sub BulkLoadOnTapiClientChanged(ByVal sender As Object, ByVal e As TApi.TapiClientEventArgs)
			Me._bulkLoadTapiClientName = e.Name
			Me.OnTapiClientChanged()
		End Sub

		Private Sub BulkLoadOnTapiStatistics(ByVal sender As Object, ByVal e As TApi.TapiStatisticsEventArgs)
			SyncLock _syncRoot
				_statistics.MetadataTime = e.TotalTransferTicks
				_statistics.MetadataBytes = e.TotalBytes
				Me.UpdateStatisticsSnapshot(System.DateTime.Now, False)
			End SyncLock
		End Sub

		Private Sub NativeOnTapiClientChanged(ByVal sender As Object, ByVal e As TApi.TapiClientEventArgs)
			Me._nativeTapiClient = e.Client
			Me._nativeTapiClientName = e.Name
			Me.OnTapiClientChanged()
		End Sub

		Private Sub NativeOnTapiProgress(ByVal sender As Object, ByVal e As TApi.TapiProgressEventArgs)
			SyncLock _syncRoot
				If ShouldImport AndAlso e.Status Then
					Me.NativeTapiProgressCount += 1
					WriteTapiProgressMessage($"End upload '{e.FileName}' file. ({System.DateTime.op_Subtraction(e.EndTime, e.StartTime).Milliseconds}ms)", e.LineNumber)
				End If
			End SyncLock
		End Sub

		Private Sub NativeOnTapiStatistics(ByVal sender As Object, ByVal e As TApi.TapiStatisticsEventArgs)
			SyncLock _syncRoot
				_statistics.FileTime = e.TotalTransferTicks
				_statistics.FileBytes = e.TotalBytes
				Me.UpdateStatisticsSnapshot(System.DateTime.Now, False)
			End SyncLock
		End Sub

		Private Sub OnTapiErrorMessage(ByVal sender As Object, ByVal e As TApi.TapiMessageEventArgs)
			SyncLock _syncRoot
				If ShouldImport Then
					' TODO: verify raising errors.
					Me.OnWriteStatusMessage(kCura.Windows.Process.EventType.Error, e.Message, e.LineNumber, e.LineNumber)
					Me.LogError(e.Message)
				End If
			End SyncLock
		End Sub

		Private Sub OnTapiWarningMessage(ByVal sender As Object, ByVal e As TApi.TapiMessageEventArgs)
			SyncLock _syncRoot
				If ShouldImport Then
					' TODO: verify raising warnings.
					Me.OnWriteStatusMessage(kCura.Windows.Process.EventType.Warning, e.Message, e.LineNumber, e.LineNumber)
					Me.LogWarning(e.Message)
				End If
			End SyncLock
		End Sub

		Private Sub OnTapiFatalError(ByVal sender As Object, ByVal e As TApi.TapiMessageEventArgs)
			SyncLock _syncRoot
				' TODO: verify raising fatal errors.
				Dim exception As Exception = New Exception(e.Message)
				OnWriteFatalError(exception)
				Me.LogFatal(exception, "A fatal error has occurred transferring files.")
			End SyncLock
		End Sub

		Private Sub OnTapiStatusEvent(ByVal sender As Object, ByVal e As TApi.TapiMessageEventArgs)
			SyncLock _syncRoot
				If ShouldImport Then
					' TODO: verify progress vs physical line number.
					Me.OnWriteStatusMessage(kCura.Windows.Process.EventType.Status, e.Message, e.LineNumber, e.LineNumber)
				End If
			End SyncLock
		End Sub

		Private Sub WriteTapiProgressMessage(ByVal message As String, ByVal lineNumber As Int32)
			message = GetLineMessage(message, lineNumber)
			Dim lineProgress As Int32 = NativeTapiProgressCount
			Me.OnWriteStatusMessage(kCura.Windows.Process.EventType.Progress, message, lineProgress, lineNumber)
		End Sub

		Private Function GetLineMessage(ByVal line As String, ByVal lineNumber As Int32) As String
			If lineNumber = kCura.WinEDDS.TApi.TapiConstants.NoLineNumber Then
				line = line & $" [job {Me.JobCounter}]"
			Else
				line = line & $" [line {lineNumber}]"
			End If
			Return line
		End Function
	End Class
End Namespace