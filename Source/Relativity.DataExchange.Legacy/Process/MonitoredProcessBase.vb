Imports System.Threading
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

	Private ReadOnly _lockObject As Object = new Object
	Private ReadOnly _metricThrottling As TimeSpan
	Private _lastSendTime As DateTime
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
	Private _initialTapiClient As TapiClient = TapiClient.None
	Protected Property RunId As String

	Public Property CaseInfo As CaseInfo

	<Obsolete("This constructor is marked for deprecation. Please use the constructor that requires a logger instance.")>
	Public Sub New(metricService As IMetricService, runningContext As IRunningContext)
		Me.New(metricService, runningContext, RelativityLogger.Instance)
	End Sub

	Public Sub New(metricService As IMetricService, runningContext As IRunningContext, logger As ILog)
		Me.New(metricService, runningContext, logger, New CancellationTokenSource())
	End Sub

	Public Sub New(metricService As IMetricService, runningContext As IRunningContext, logger As ILog, tokenSource As CancellationTokenSource)
		MyBase.New(logger, tokenSource)
		Me.MetricService = metricService
		_metricThrottling = metricService.MetricSinkConfig.ThrottleTimeout
		Me.RunningContext = runningContext
	End Sub

	Protected Overrides Sub OnExecute()
		Try
			Initialize()
		Catch ex As Exception
			OnInitializationError()
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
		End Try
	End Sub

	Protected Overridable Sub SetEndTime()
		EndTime = DateTime.Now
	End Sub

	Protected Overridable Sub SetStartTime()
		StartTime = DateTime.Now
	End Sub

	Protected Overridable Sub OnFatalError()
		OnJobEndWithStatus(TelemetryConstants.JobStatus.Failed)
		Me.Context.PublishStatusEvent("", $"{TransferDirection} aborted")
	End Sub

	Protected Overridable Sub OnSuccess()
		Dim jobStatus As TelemetryConstants.JobStatus = CType(IIf(IsCancelled, TelemetryConstants.JobStatus.Cancelled, TelemetryConstants.JobStatus.Completed), TelemetryConstants.JobStatus)
		OnJobEndWithStatus(jobStatus)
	End Sub

	Protected Overridable Sub OnHasErrors()
		Dim jobStatus As TelemetryConstants.JobStatus = CType(IIf(IsCancelled, TelemetryConstants.JobStatus.Cancelled, TelemetryConstants.JobStatus.Completed), TelemetryConstants.JobStatus)
		OnJobEndWithStatus(jobStatus)
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

	Private Sub SendMetricJobStarted()
			Dim metric As MetricJobStarted = New MetricJobStarted
			metric.ImportObjectType = Statistics.ImportObjectType
			BuildMetricBase(metric)
			MetricService.Log(metric)
	End Sub

	Protected Sub SendMetricJobEndReport(jobStatus As TelemetryConstants.JobStatus)
		Dim totalRecordsCount As Long = GetTotalRecordsCount()
		Dim completedRecordsCount As Long = GetCompletedRecordsCount()
		Dim jobDuration As Double = (EndTime - StartTime).TotalSeconds
		Dim totalTransferredBytes As Long = Statistics.FileTransferredBytes + Statistics.MetadataTransferredBytes
		Dim jobStartTimeStamp As Double = EpochDateConverter.ConvertDateTimeToEpoch(StartTime)
		Dim jobEndTimeStamp As Double = EpochDateConverter.ConvertDateTimeToEpoch(EndTime)

		Dim metric As MetricJobEndReport = New MetricJobEndReport() With {
				.JobStatus = jobStatus,
				.TotalSizeBytes = totalTransferredBytes,
				.FileSizeBytes = Statistics.FileTransferredBytes,
				.MetadataSizeBytes = Statistics.MetadataTransferredBytes,
				.TotalRecords = totalRecordsCount,
				.CompletedRecords = completedRecordsCount,
				.RecordsWithErrors = Statistics.RecordsWithErrorsCount,
				.ThroughputBytesPerSecond = Statistics.CalculateThroughput(totalTransferredBytes, jobDuration),
				.ThroughputRecordsPerSecond = Statistics.CalculateThroughput(completedRecordsCount, jobDuration),
				.SqlBulkLoadThroughputRecordsPerSecond = Statistics.CalculateThroughput(Statistics.DocumentsCreated + Statistics.DocumentsUpdated, Statistics.MassImportDuration.TotalSeconds),
				.ImportObjectType = Statistics.ImportObjectType,
				.JobDurationInSeconds = jobDuration,
				.InitialTransferMode = _initialTapiClient,
				.JobStartTimeStamp = jobStartTimeStamp,
				.JobEndTimeStamp = jobEndTimeStamp,
				.JobRunId = RunId}
		BuildMetricBase(metric)
		MetricService.Log(metric)
	End Sub

	Protected Sub SendMetricJobBatch(batchInformation As BatchInformation)
		Dim metric As MetricJobBatch = New MetricJobBatch() With {
					.ImportObjectType = Statistics.ImportObjectType,
					.MassImportDurationMilliseconds = batchInformation.MassImportDuration.TotalMilliseconds,
					.BatchNumber = batchInformation.OrdinalNumber,
					.NumberOfRecords = batchInformation.NumberOfRecords,
					.NumberOfRecordsWithErrors = batchInformation.NumberOfRecordsWithErrors
				}
		BuildMetricBase(metric)
		MetricService.Log(metric)
	End Sub

	Protected Sub SendMetricJobProgress(statistics As Statistics, checkThrottling As Boolean)
		' Don't send metrics with no transfer mode
		If TapiClient = TapiClient.None Then Return
		Dim currentTime As DateTime = DateTime.Now
		SyncLock _lockObject
			If currentTime - _lastSendTime < _metricThrottling And checkThrottling Then Return
			_lastSendTime = currentTime
		End SyncLock
		Dim metric As MetricJobProgress = New MetricJobProgress With {
			.MetadataThroughputBytesPerSecond = statistics.MetadataTransferThroughput,
			.FileThroughputBytesPerSecond = statistics.FileTransferThroughput,
			.SqlBulkLoadThroughputRecordsPerSecond = Statistics.CalculateThroughput(statistics.DocumentsCreated + statistics.DocumentsUpdated, statistics.MassImportDuration.TotalSeconds),
			.ImportObjectType = statistics.ImportObjectType}
		BuildMetricBase(metric)
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

	Private Sub OnJobEndWithStatus(jobStatus As TelemetryConstants.JobStatus)
		SetEndTime()
		SendMetricJobEndReport(jobStatus)
		' This is to ensure we send non-zero JobProgressMessage even with small job
		SendMetricJobProgress(Statistics, checkThrottling := False)
	End Sub

	Private Sub OnInitializationError()
		' send only a basic version of metric since objects may not be initialized correctly
		Dim metric As MetricJobEndReport = New MetricJobEndReport() With { .JobStatus = TelemetryConstants.JobStatus.Failed }
		BuildMetricBase(metric)
		MetricService.Log(metric)
	End Sub

	Private Sub BuildMetricBase(metric As MetricJobBase)
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
End Class
