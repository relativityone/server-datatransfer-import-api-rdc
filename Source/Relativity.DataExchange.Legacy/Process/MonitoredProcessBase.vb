Imports System.Threading
Imports Helpers
Imports kCura.WinEDDS
Imports Monitoring
Imports Monitoring.Sinks
Imports Relativity.DataExchange
Imports Relativity.DataExchange.Logger
Imports Relativity.DataExchange.Process
Imports Relativity.DataExchange.Service
Imports Relativity.DataExchange.Transfer
Imports Relativity.Logging

Public MustInherit Class MonitoredProcessBase
	Inherits ProcessBase2

	Private ReadOnly Property LockObject As Object = new Object
	Private ReadOnly Property MetricThrottling As TimeSpan
	Private Property LastSendTime As DateTime
	Protected Property JobGuid As System.Guid = System.Guid.NewGuid()
	Protected Property StartTime As System.DateTime
	Protected Property EndTime As System.DateTime
	Protected MustOverride ReadOnly Property TransferDirection As TelemetryConstants.TransferDirection
	Protected MustOverride ReadOnly Property Statistics As Statistics
	Protected ReadOnly Property RunningContext As IRunningContext
	Protected ReadOnly Property MetricService() As IMetricService
	Protected _hasFatalErrorOccured As Boolean
	Private _jobStartedMetricSent As Boolean = False
	Protected MustOverride ReadOnly Property TapiClient As TapiClient
	Protected _initialTapiClient As TapiClient = TapiClient.None
	Protected Property RunId As String
	Private _jobStatus As TelemetryConstants.JobStatus
	Protected _correlationIdFunc As Func(Of String)

	Public Property CaseInfo As CaseInfo

	<Obsolete("This constructor is marked for deprecation. Please use the constructor that requires a logger instance.")>
	Public Sub New(metricService As IMetricService, runningContext As IRunningContext, correlationIdFunc As Func(Of String))
		Me.New(metricService, runningContext, RelativityLogger.Instance, correlationIdFunc)
	End Sub

	Public Sub New(metricService As IMetricService, runningContext As IRunningContext, logger As ILog, correlationIdFunc As Func(Of String))
		Me.New(metricService, runningContext, logger, New CancellationTokenSource(), correlationIdFunc)
	End Sub

	Public Sub New(metricService As IMetricService, runningContext As IRunningContext, logger As ILog, tokenSource As CancellationTokenSource, correlationIdFunc As Func(Of String))
		MyBase.New(logger, tokenSource)
		Me.MetricService = metricService
		_MetricThrottling = metricService.MetricSinkConfig.ThrottleTimeout
		Me.RunningContext = runningContext
		_correlationIdFunc = correlationIdFunc
	End Sub

	Protected Overrides Sub OnExecute()
		Try
			Initialize()
		Catch ex As Exception
			OnInitializationError(ex)
			throw
		End Try
		Try
			If Run() Then
				If HasErrors() Then
					OnHasErrors()
				Else
					OnSuccess()
				End If
			Else
				OnFatalError()
			End If
		Catch ex As Exception
			OnFatalError()
			throw
		Finally
			SendSummaryMetrics()
		End Try
	End Sub

	Protected Overridable Sub SetEndTime()
		EndTime = DateTime.Now
	End Sub

	Protected Overridable Sub SetStartTime()
		StartTime = DateTime.Now
	End Sub

	Protected Overridable Sub OnFatalError()
		Me.SetEndTime()
		Me._jobStatus = TelemetryConstants.JobStatus.Failed
		Me.Context.PublishStatusEvent("", $"{TransferDirection} aborted")
	End Sub

	Protected Overridable Sub OnSuccess()
		Me.SetEndTime()
		Me._jobStatus = CType(IIf(IsCancelled, TelemetryConstants.JobStatus.Cancelled, TelemetryConstants.JobStatus.Completed), TelemetryConstants.JobStatus)
	End Sub

	Protected Overridable Sub OnHasErrors()
		Me.SetEndTime()
		Me._jobStatus = CType(IIf(IsCancelled, TelemetryConstants.JobStatus.Cancelled, TelemetryConstants.JobStatus.Completed), TelemetryConstants.JobStatus)
	End Sub

	Protected MustOverride Function HasErrors() As Boolean

	Protected MustOverride Function IsCancelled() As Boolean

	Protected Overridable Sub Initialize()
		SetStartTime()
	End Sub

	Protected MustOverride Function Run() As Boolean

	Protected Sub OnTapiClientChanged()
		If Not _jobStartedMetricSent And TapiClient <> TapiClient.None Then
			SendMetricJobStarted()
			_jobStartedMetricSent = True
			_initialTapiClient = TapiClient
		End If
	End Sub

	Protected Sub SendMetricJobStarted()
		Dim metric As MetricJobStarted = BuildStartMetric()
		MetricService.Log(metric)
	End Sub

	Protected Sub SendMetricJobEndReport(jobStatus As TelemetryConstants.JobStatus)
		Dim metric As MetricJobEndReport = BuildEndMetric(jobStatus)
		MetricService.Log(metric)
	End Sub

	Protected Sub SendMetricJobBatch(batchInformation As BatchInformation)
		Dim metric As MetricJobBatch = BuildBatchMetric(batchInformation)
		MetricService.Log(metric)
	End Sub

	Protected Sub SendMetricJobProgress(statistics As Statistics, checkThrottling As Boolean)
		' Don't send metrics with no transfer mode
		If TapiClient = TapiClient.None Then Return
		Dim currentTime As DateTime = DateTime.Now
		SyncLock LockObject
			If currentTime - LastSendTime < MetricThrottling And checkThrottling Then Return
			LastSendTime = currentTime
		End SyncLock
		
		Dim metric As MetricJobProgress = BuildProgressMetric(statistics)
		MetricService.Log(metric)
	End Sub

	''' <summary>
	''' Returns total number of records to import/export. This value is used by our telemetry system.
	''' </summary>
	''' <returns>Total number of records.</returns>
	Protected MustOverride Function GetTotalRecordsCount() As Long

	''' <summary>
	''' Returns number of completed records. This value is used by our telemetry system.
	''' </summary>
	''' <returns>Number of completed records.</returns>
	Protected MustOverride Function GetCompletedRecordsCount() As Long

	Private Sub SendSummaryMetrics()
		SendMetricJobEndReport(Me._jobStatus)
		' This is to ensure we send non-zero JobProgressMessage even with small job
		SendMetricJobProgress(Statistics, checkThrottling := False)
	End Sub

	Private Sub OnInitializationError(exception As Exception)
		Logger.LogError(exception, "Error occurred during initialization")

		Try
			' send only a basic version of metric since objects may not be initialized correctly
			Dim metric As MetricJobEndReport = New MetricJobEndReport() With { .JobStatus = TelemetryConstants.JobStatus.Failed }
			SetBaseMetrics(metric)
			MetricService.Log(metric)
		Catch ex As Exception
			Logger.LogError(ex, "Error occurred while sending metric for failed job.")
		End Try
	End Sub

	Protected Overridable Sub SetBaseMetrics(metric As MetricJobBase)
		' To be overriden in import or export to add Metrics from ImportStatistics or ExportStatistics
		metric.TransferDirection = TransferDirection
		metric.TransferMode = TapiClient
		metric.CorrelationID = JobGuid.ToString()
		metric.UnitOfMeasure = TelemetryConstants.Values.NOT_APPLICABLE
		metric.ApplicationName = GetApplicationName()
		metric.ImportApiVersion = RunningContext.ImportApiSdkVersion.ToString()
		metric.RelativityVersion = RunningContext.RelativityVersion.ToString()
		If Not (CaseInfo Is Nothing) Then
			metric.WorkspaceID = CaseInfo.ArtifactID
		End If
	End Sub

	Protected Overridable Function BuildStartMetric() As MetricJobStarted
		' To be overriden in import or export to add Metrics from ImportStatistics or ExportStatistics
		Dim metric As MetricJobStarted = New MetricJobStarted()
		SetBaseMetrics(metric)
		SetClientInformation(metric)
		Return metric
	End Function

	Protected Overridable Function BuildBatchMetric(batchInformation As BatchInformation) As MetricJobBatch
		' To be overriden in import or export to add Metrics from ImportStatistics or ExportStatistics
		Dim metric As MetricJobBatch = New MetricJobBatch()

		metric.MassImportDurationMilliseconds = batchInformation.MassImportDuration.TotalMilliseconds
		metric.BatchNumber = batchInformation.OrdinalNumber
		metric.NumberOfRecords = batchInformation.NumberOfRecords
		metric.NumberOfRecordsWithErrors = batchInformation.NumberOfRecordsWithErrors
		metric.NumberOfFiles = batchInformation.NumberOfFilesProcessed

		SetBaseMetrics(metric)

		Return metric
	End Function

	Protected Overridable Function BuildProgressMetric(statistics As Statistics) As MetricJobProgress
		' To be overriden in import or export to add Metrics from ImportStatistics or ExportStatistics
		Dim metric As MetricJobProgress = New MetricJobProgress()

		metric.MetadataThroughputBytesPerSecond = statistics.MetadataTransferThroughput
		metric.FileThroughputBytesPerSecond = statistics.FileTransferThroughput

		SetBaseMetrics(metric)

		Return metric
	End Function

	Protected Overridable Function BuildEndMetric(jobStatus As TelemetryConstants.JobStatus) As MetricJobEndReport
		' To be overriden in import or export to add Metrics from ImportStatistics or ExportStatistics
		Dim metric As MetricJobEndReport = New MetricJobEndReport()

		Dim totalRecordsCount As Long = GetTotalRecordsCount()
		Dim completedRecordsCount As Long = GetCompletedRecordsCount()
		Dim jobDuration As Double = (EndTime - StartTime).TotalSeconds
		Dim totalTransferredBytes As Long = Statistics.FileTransferredBytes + Statistics.MetadataTransferredBytes
		Dim jobStartTimeStamp As Double = EpochDateConverter.ConvertDateTimeToEpoch(StartTime)
		Dim jobEndTimeStamp As Double = EpochDateConverter.ConvertDateTimeToEpoch(EndTime)

		metric.JobStatus = jobStatus
		metric.TotalSizeBytes = totalTransferredBytes
		metric.FileSizeBytes = Statistics.FileTransferredBytes
		metric.MetadataSizeBytes = Statistics.MetadataTransferredBytes
		metric.TotalRecords = totalRecordsCount
		metric.CompletedRecords = completedRecordsCount
		metric.RecordsWithErrors = Statistics.DocsErrorsCount
		metric.ThroughputBytesPerSecond = Statistics.CalculateThroughput(totalTransferredBytes, jobDuration)
		metric.ThroughputRecordsPerSecond = Statistics.CalculateThroughput(completedRecordsCount, jobDuration)
		metric.JobDurationInSeconds = jobDuration
		metric.InitialTransferMode = _initialTapiClient
		metric.JobStartTimeStamp = jobStartTimeStamp
		metric.JobEndTimeStamp = jobEndTimeStamp
		metric.JobRunId = RunId

		SetBaseMetrics(metric)

		Return metric
	End Function

	Private Sub SetClientInformation(metric As MetricJobStarted)
		Try
			Dim clientInfo As ClientInformationHelper = new ClientInformationHelper()
			metric.TotalPhysicalMemory = clientInfo.TotalPhysicalMemory
			metric.AvailablePhysicalMemory = clientInfo.AvailablePhysicalMemory
			metric.OperatingSystemName = clientInfo.OperatingSystemName
			metric.OperatingSystemVersion = clientInfo.OperatingSystemVersion
			metric.Is64BitOperatingSystem = clientInfo.Is64BitOperatingSystem
			metric.Is64BitProcess = clientInfo.Is64BitProcess
			metric.CpuCount = clientInfo.CpuCount
			metric.CallingAssembly = Me.RunningContext.CallingAssembly
		Catch ex As Exception
			Me.Logger.LogError("Exception occurred when trying to add client information metrics", ex)
		End Try
		
	End Sub

	''' <summary>
	''' Provides application name for telemetry purpose
	''' </summary>
	''' <returns>The application name</returns>
	Private Function GetApplicationName() As String
		If Not String.IsNullOrEmpty(RunningContext.ApplicationName) Then
			Return RunningContext.ApplicationName
		ElseIf Not String.IsNullOrEmpty(AppSettings.ApplicationName) Then
			Return AppSettings.ApplicationName
		Else
			Return RunningContext.ExecutionSource.ToString()
		End If
	End Function

	Protected Function GetCorrelationId() As String
		' Return job id if already set
		If Not JobGuid = Guid.Empty Then
			Return JobGuid.ToString()
		End If

		' Return 'injected' correlationIdFunc (passed from desktop application or import api)
		Return _correlationIdFunc?.Invoke()
	End Function
End Class
