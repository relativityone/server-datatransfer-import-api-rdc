Imports System.Threading
Imports kCura.WinEDDS
Imports Monitoring
Imports Monitoring.Sinks
Imports Relativity.DataExchange
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
	Protected MustOverride ReadOnly Property JobType As TelemetryConstants.JobType
	Protected MustOverride ReadOnly Property Statistics As Statistics
	Protected ReadOnly Property MetricService() As IMetricService
	Protected _hasFatalErrorOccured As Boolean
	Private _jobStartedMetricSent As Boolean = False
	Protected MustOverride ReadOnly Property TapiClient As TapiClient

	Public Property CaseInfo As CaseInfo

	Public Property ExecutionSource As ExecutionSource = ExecutionSource.Unknown

	''' <summary>
	''' Gets or sets name of application executing import/export job. This property is used to support telemetry.
	''' </summary>
	''' <returns>The application name</returns>
	Public Property ApplicationName As String = Nothing

	<Obsolete("This constructor is marked for deprecation. Please use the constructor that requires a logger instance.")>
	Public Sub New(metricService As IMetricService)
		Me.New(metricService, RelativityLogger.Instance)
	End Sub

	Public Sub New(metricService As IMetricService, logger As ILog)
		Me.New(metricService, logger, New CancellationTokenSource())
	End Sub

	Public Sub New(metricService As IMetricService, logger As ILog, tokenSource As CancellationTokenSource)
		MyBase.New(logger, tokenSource)
		Me.MetricService = metricService
		_metricThrottling = metricService.MetricSinkConfig.ThrottleTimeout
	End Sub

	Protected Overrides Sub OnExecute()
		Initialize()
		If Run() Then
			If HasErrors() Then
				OnHasErrors()
			Else
				OnSuccess()
			End If
		Else
			OnFatalError()
		End If

	End Sub

	Protected Overridable Sub SetEndTime()
		EndTime = DateTime.Now
	End Sub

	Protected Overridable Sub SetStartTime()
		StartTime = DateTime.Now
	End Sub

	Protected Overridable Sub OnFatalError()
		OnJobEndWithStatus(TelemetryConstants.JobStatus.Failed)
		Me.Context.PublishStatusEvent("", $"{JobType} aborted")
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

	Protected Sub SendMetricJobStarted()
		If Not _jobStartedMetricSent And TapiClient <> TapiClient.None Then
			Dim message As MetricJobStarted = New MetricJobStarted
			BuildMetricBase(message)
			MetricService.Log(message)
			_jobStartedMetricSent = True
		End If
	End Sub

	Protected Sub SendMetricJobEndReport(jobStatus As TelemetryConstants.JobStatus)
		Dim totalRecordsCount As Long = GetTotalRecordsCount()
		Dim completedRecordsCount As Long = GetCompletedRecordsCount()
		Dim jobDuration As Double = (EndTime - StartTime).TotalSeconds
		Dim metric As MetricJobEndReport = New MetricJobEndReport() With {
				.JobStatus = jobStatus,
				.TotalSizeBytes = (Statistics.TotalBytes),
				.FileSizeBytes = Statistics.FileBytes,
				.MetadataSizeBytes = Statistics.MetadataBytes,
				.TotalRecords = totalRecordsCount,
				.CompletedRecords = completedRecordsCount,
				.ThroughputBytesPerSecond = Statistics.CalculateThroughput(Statistics.TotalBytes, jobDuration),
				.ThroughputRecordsPerSecond = Statistics.CalculateThroughput(completedRecordsCount, jobDuration),
				.SqlBulkLoadThroughputRecordsPerSecond = Statistics.CalculateThroughput(Statistics.DocumentsCreated + Statistics.DocumentsUpdated, Statistics.SqlTime/TimeSpan.TicksPerSecond),
				.BulkImportType = Statistics.BulkImportType}
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
		Dim message As MetricJobProgress = New MetricJobProgress With {
			.MetadataThroughputBytesPerSecond = statistics.MetadataThroughput,
			.FileThroughputBytesPerSecond = statistics.FileThroughput,
			.SqlBulkLoadThroughputRecordsPerSecond = Statistics.CalculateThroughput(statistics.DocumentsCreated + statistics.DocumentsUpdated, statistics.SqlTime/TimeSpan.TicksPerSecond),
			.BulkImportType = statistics.BulkImportType}
		BuildMetricBase(message)
		MetricService.Log(message)
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

	Private Sub BuildMetricBase(metric As MetricJobBase)
		metric.JobType = JobType
		metric.TransferMode = TapiClient
		metric.CorrelationID = JobGuid.ToString()
		metric.UseOldExport = Me.AppSettings.UseOldExport
		metric.UnitOfMeasure = TelemetryConstants.Values.NOT_APPLICABLE
		metric.ApplicationName = GetApplicationName()
		If Not (CaseInfo Is Nothing) Then
			metric.WorkspaceID = CaseInfo.ArtifactID
		End If
	End Sub

	''' <summary>
	''' Provides application name for telemetry purpose
	''' </summary>
	''' <returns>The application name</returns>
	Private Function GetApplicationName() As String
		If Not String.IsNullOrEmpty(ApplicationName) Then
			Return ApplicationName
		ElseIf Not String.IsNullOrEmpty(AppSettings.ApplicationName) Then
			Return AppSettings.ApplicationName
		Else
			Return ExecutionSource.ToString()
		End If
	End Function
End Class
