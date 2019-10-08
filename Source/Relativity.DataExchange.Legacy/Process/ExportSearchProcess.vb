Imports System.Threading
Imports kCura.WinEDDS.Exporters
Imports kCura.WinEDDS.Service.Export
Imports Monitoring
Imports Monitoring.Sinks
Imports Relativity.DataExchange
Imports Relativity.DataExchange.Process
Imports Relativity.DataExchange.Transfer

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

		Protected Overrides ReadOnly Property JobType As String = "Export"

		Protected Overrides ReadOnly Property CurrentTapiClient As TapiClient
			Get
				Return _tapiClient
			End Get
		End Property

		Public Property UserNotification As Exporters.IUserNotification
		Public Property UserNotificationFactory As Func(Of Exporter, IUserNotification)

		Public Sub New(loadFileHeaderFormatterFactory As ILoadFileHeaderFormatterFactory, exportConfig As IExportConfig)
			MyBase.New(New MetricService(New ImportApiMetricSinkConfig))
			_loadFileHeaderFormatterFactory = loadFileHeaderFormatterFactory
			_exportConfig = exportConfig
		End Sub

		Public Sub New(loadFileHeaderFormatterFactory As ILoadFileHeaderFormatterFactory, exportConfig As IExportConfig, metricService As IMetricService)
			MyBase.New(metricService)
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
			SendMetricJobEndReport(TelemetryConstants.JobStatus.COMPLETED, _searchExporter.Statistics)
			' This is to ensure we send non-zero JobProgressMessage even with small job
			SendMetricJobProgress(_searchExporter.Statistics.MetadataThroughput, _searchExporter.Statistics.FileThroughput, forceSend := True)
			Me.Context.PublishStatusEvent("", "Export completed")
			Me.Context.PublishProcessCompleted()
		End Sub

		Protected Overrides Sub OnFatalError()
			MyBase.OnFatalError()
			SendMetricJobEndReport(TelemetryConstants.JobStatus.FAILED, _searchExporter.Statistics)
			' This is to ensure we send non-zero JobProgressMessage even with small job
			SendMetricJobProgress(_searchExporter.Statistics.MetadataThroughput, _searchExporter.Statistics.FileThroughput, forceSend := True)
		End Sub

		Protected Overrides Sub OnHasErrors()
			MyBase.OnHasErrors()
			SendMetricJobEndReport(TelemetryConstants.JobStatus.COMPLETED, _searchExporter.Statistics)
			' This is to ensure we send non-zero JobProgressMessage even with small job
			SendMetricJobProgress(_searchExporter.Statistics.MetadataThroughput, _searchExporter.Statistics.FileThroughput, forceSend := True)
			Me.Context.PublishProcessCompleted(False, _searchExporter.ErrorLogFileName, True)
		End Sub

		Protected Overrides Function HasErrors() As Boolean
			Return _hasErrors
		End Function

		Private Function GetExporter() As Exporter
			If (ExportFile.UseCustomFileNaming) Then
				Return _
					New ExtendedExporter(TryCast(Me.ExportFile, ExtendedExportFile), Me.Context,
										 New WebApiServiceFactory(Me.ExportFile),
										 _loadFileHeaderFormatterFactory, _exportConfig) With {.InteractionManager = UserNotification}
			Else
				Return _
					New Exporter(Me.ExportFile, Me.Context,
										 New WebApiServiceFactory(Me.ExportFile),
										 _loadFileHeaderFormatterFactory, _exportConfig) With {.InteractionManager = UserNotification}

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

		Private Sub _searchExporter_FileTransferModeChangeEvent(ByVal sender As Object, ByVal args As Global.Relativity.DataExchange.Transfer.TapiMultiClientEventArgs) Handles _searchExporter.FileTransferMultiClientModeChangeEvent
			If _uploadModeText Is Nothing Then
				_uploadModeText = TapiModeHelper.BuildDocText()
			End If

			Dim statusBarText As String = TapiModeHelper.BuildExportStatusText(args.TransferClients)

			If Not args.TransferClients Is Nothing AndAlso args.TransferClients.Count <> 0 Then
				_tapiClient = args.TransferClients.Last()
			End If
			SendMetricJobStarted()
			Me.Context.PublishStatusBarChanged(statusBarText, _uploadModeText)
		End Sub

		Private Sub _productionExporter_StatusMessage(ByVal e As ExportEventArgs) Handles _searchExporter.StatusMessage
			Select Case e.EventType
				Case EventType2.Error
					Interlocked.Increment(_errorCount)
					Me.Context.PublishErrorEvent(e.DocumentsExported.ToString, e.Message)
				Case EventType2.Progress
					SendMetricJobProgress(e.Statistics.MetadataThroughput, e.Statistics.FileThroughput, forceSend := False)
					Me.Context.PublishStatusEvent("", e.Message)
				Case EventType2.Statistics
					SendMetricJobProgress(e.Statistics.MetadataThroughput, e.Statistics.FileThroughput, forceSend := False)
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

			Me.Context.PublishProgress(e.TotalDocuments, e.DocumentsExported, _warningCount, _errorCount, StartTime, New DateTime, e.Statistics.MetadataThroughput, e.Statistics.FileThroughput, Me.ProcessID, Nothing, Nothing, statDict)
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