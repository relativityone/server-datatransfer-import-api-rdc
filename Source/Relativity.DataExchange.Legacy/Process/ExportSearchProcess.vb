Imports System.Threading
Imports kCura.WinEDDS.Exporters
Imports kCura.WinEDDS.Service.Export
Imports Monitoring
Imports Monitoring.Sinks
Imports Relativity.DataExchange
Imports Relativity.DataExchange.Logger
Imports Relativity.DataExchange.Process
Imports Relativity.DataExchange.Transfer
Imports Relativity.Logging

Namespace kCura.WinEDDS
	Public Class ExportSearchProcess
		Inherits MonitoredProcessBase

		Private ReadOnly _loadFileHeaderFormatterFactory As ILoadFileHeaderFormatterFactory
		Private ReadOnly _exportConfig As IExportConfig
		Public ExportFile As ExportFile
		Private WithEvents _searchExporter As kCura.WinEDDS.Exporter
		Private _errorCount As Int32
		Private _warningCount As Int32
		Private _uploadModeText As String = Nothing
		Private _hasErrors As Boolean
		Private _tapiClient As TapiClient = TapiClient.None

		Protected Overrides ReadOnly Property TransferDirection As TelemetryConstants.TransferDirection = TelemetryConstants.TransferDirection.Export

		Protected Overrides ReadOnly Property TapiClient As TapiClient
			Get
				Return _tapiClient
			End Get
		End Property

		Protected Overrides ReadOnly Property Statistics As Statistics
			Get
				Return _searchExporter.Statistics
			End Get
		End Property

		Public Property UserNotification As Exporters.IUserNotification
		Public Property UserNotificationFactory As Func(Of Exporter, IUserNotification)

		<Obsolete("This constructor is marked for deprecation. Please use the constructor that requires a logger instance.")>
		Public Sub New(loadFileHeaderFormatterFactory As ILoadFileHeaderFormatterFactory, exportConfig As IExportConfig, correlationIdFunc As Func(Of String))
			Me.New(loadFileHeaderFormatterFactory, exportConfig, RelativityLogger.Instance, New RunningContext(), correlationIdFunc)
		End Sub

		Public Sub New(loadFileHeaderFormatterFactory As ILoadFileHeaderFormatterFactory, exportConfig As IExportConfig, logger As ILog, runningContext As IRunningContext, correlationIdFunc As Func(Of String))
			Me.New(loadFileHeaderFormatterFactory, exportConfig, New MetricService(New ImportApiMetricSinkConfig), runningContext, logger, correlationIdFunc)
		End Sub

		<Obsolete("This constructor is marked for deprecation. Please use the constructor that requires a logger instance.")>
		Public Sub New(loadFileHeaderFormatterFactory As ILoadFileHeaderFormatterFactory, exportConfig As IExportConfig, metricService As IMetricService, correlationIdFunc As Func(Of String))
			Me.New(loadFileHeaderFormatterFactory, exportConfig, metricService, New RunningContext(), RelativityLogger.Instance, correlationIdFunc)
		End Sub

		Public Sub New(loadFileHeaderFormatterFactory As ILoadFileHeaderFormatterFactory, exportConfig As IExportConfig, metricService As IMetricService, runningContext As IRunningContext, logger As ILog, correlationIdFunc As Func(Of String))
			MyBase.New(metricService, runningContext, logger, correlationIdFunc)
			_loadFileHeaderFormatterFactory = loadFileHeaderFormatterFactory
			_exportConfig = exportConfig
		End Sub

		''' <inheritdoc/>
		Protected Overrides Function GetTotalRecordsCount() As Long
			Return _searchExporter.TotalExportArtifactCount
		End Function

		''' <inheritdoc/>
		Protected Overrides Function GetCompletedRecordsCount() As Long
			Return _searchExporter.DocumentsExported
		End Function

		Protected Overrides Sub OnSuccess()
			MyBase.OnSuccess()
			Me.Context.PublishStatusEvent("", "Export completed")
			Me.Context.PublishProcessCompleted()
		End Sub

		Protected Overrides Sub OnHasErrors()
			MyBase.OnHasErrors()
			Me.Context.PublishProcessCompleted(False, _searchExporter.ErrorLogFileName, True)
		End Sub

		Protected Overrides Function HasErrors() As Boolean
			Return _hasErrors
		End Function

		Protected Overrides Function IsCancelled() As Boolean
			Return _searchExporter.IsCancelledByUser
		End Function

		Private Function GetExporter() As Exporter
			If (ExportFile.UseCustomFileNaming) Then
				Return _
					New ExtendedExporter(TryCast(Me.ExportFile, ExtendedExportFile), Me.Context,
										 New WebApiServiceFactory(Me.ExportFile),
										 _loadFileHeaderFormatterFactory, _exportConfig, Me.Logger, Me.CancellationTokenSource.Token, AddressOf GetCorrelationId) With {.InteractionManager = UserNotification}
			Else
				Return _
					New Exporter(Me.ExportFile, Me.Context,
										 New WebApiServiceFactory(Me.ExportFile),
										 _loadFileHeaderFormatterFactory, _exportConfig, Me.Logger, Me.CancellationTokenSource.Token, AddressOf GetCorrelationId) With {.InteractionManager = UserNotification}

			End If
		End Function

		Protected Overrides Sub Initialize()
			MyBase.Initialize()
			_warningCount = 0
			_errorCount = 0
			_searchExporter = GetExporter()
			If Not UserNotificationFactory Is Nothing Then
				Dim un As IUserNotification = UserNotificationFactory(_searchExporter)
				If Not un Is Nothing Then
					_searchExporter.InteractionManager = un
					UserNotification = un
				End If
			End If
		End Sub

		Protected Overrides Function Run() As Boolean
			_hasErrors = Not _searchExporter.ExportSearch()
			Return Not _hasFatalErrorOccured
		End Function

		Protected Overrides Function BuildEndMetric(jobStatus As TelemetryConstants.JobStatus) As MetricJobEndReport
			Dim metric As MetricJobEndReport = MyBase.BuildEndMetric(jobStatus)
			metric.ExportedNativeCount = _searchExporter.Statistics.ExportedNativeCount
			metric.ExportedPdfCount = _searchExporter.Statistics.ExportedPdfCount
			metric.ExportedImageCount = _searchExporter.Statistics.ExportedImageCount
			metric.ExportedLongTextCount = _searchExporter.Statistics.ExportedLongTextCount
			Return metric
		End Function

		Private Sub _searchExporter_FileTransferModeChangeEvent(ByVal sender As Object, ByVal args As Global.Relativity.DataExchange.Transfer.TapiMultiClientEventArgs) Handles _searchExporter.FileTransferMultiClientModeChangeEvent
			If _uploadModeText Is Nothing Then
				_uploadModeText = TapiModeHelper.BuildDocText()
			End If

			Dim nativeFilesCopied As Boolean = (Me.ExportFile.ExportImages AndAlso
			                                    Me.ExportFile.VolumeInfo.CopyImageFilesFromRepository) OrElse
			                                   (Me.ExportFile.ExportNative AndAlso
			                                    Me.ExportFile.VolumeInfo.CopyNativeFilesFromRepository)
			Dim statusBarText As String = TapiModeHelper.BuildExportStatusText(nativeFilesCopied, args.TransferClients)
			_tapiClient = TapiModeHelper.GetTapiClient(args.TransferClients)
			
			OnTapiClientChanged()
			Me.Context.PublishStatusBarChanged(statusBarText, _uploadModeText)
			Me.Logger.LogInformation("Export transfer mode changed: {@TransferClients}", args.TransferClients)
		End Sub

		Private Sub _productionExporter_StatusMessage(ByVal e As ExportEventArgs) Handles _searchExporter.StatusMessage
			Select Case e.EventType
				Case EventType2.Error
					Interlocked.Increment(_errorCount)
					Statistics.DocsErrorsCount = _errorCount
					Me.Context.PublishErrorEvent(e.DocumentsExported.ToString, e.Message)
				Case EventType2.Progress
					SendMetricJobProgress(e.Statistics, checkThrottling := True)
					Me.Context.PublishStatusEvent("", e.Message)
				Case EventType2.Statistics
					SendMetricJobProgress(e.Statistics, checkThrottling := True)
				Case EventType2.Status
					Me.Context.PublishStatusEvent(e.DocumentsExported.ToString, e.Message)
				Case EventType2.Warning
					Interlocked.Increment(_warningCount)
					Me.Context.PublishWarningEvent(e.DocumentsExported.ToString, e.Message)
				Case EventType2.ResetStartTime
					SetStartTime()
			End Select
			Dim statDict As IDictionary = Nothing
			If Not e.AdditionalInfo Is Nothing AndAlso TypeOf e.AdditionalInfo Is IDictionary Then
				statDict = DirectCast(e.AdditionalInfo, IDictionary)
			End If

			Me.Context.PublishProgress(e.TotalDocuments, e.DocumentsExported, _warningCount, _errorCount, StartTime, New DateTime, e.Statistics.MetadataTransferThroughput, e.Statistics.FileTransferThroughput, Me.ProcessID, Nothing, Nothing, statDict)
		End Sub

		Private Sub _productionExporter_FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception) Handles _searchExporter.FatalErrorEvent
			Me.Context.PublishFatalException(ex)
			_hasFatalErrorOccured = True
		End Sub

		Private Sub _searchExporter_ShutdownEvent() Handles _searchExporter.ShutdownEvent
			Me.Context.PublishShutdown()
		End Sub
	End Class
End Namespace