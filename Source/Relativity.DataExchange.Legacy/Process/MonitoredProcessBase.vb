Imports kCura.WinEDDS
Imports Monitoring
Imports Monitoring.Sinks
Imports Relativity.DataExchange.Process
Imports Relativity.DataExchange.Service
Imports Relativity.DataExchange.Transfer

Public MustInherit Class MonitoredProcessBase
	Inherits ProcessBase2

	Private ReadOnly _lockObject As Object = new Object
	Private ReadOnly _metricThrottling As TimeSpan
	Private _lastSendTime As DateTime
	Protected Property JobGuid As System.Guid = System.Guid.NewGuid()
	Protected Property StartTime As System.DateTime
	Protected Property EndTime As System.DateTime
	Protected Property InitialTapiClientName As String
	Protected MustOverride ReadOnly Property JobType As String
	Protected MustOverride ReadOnly Property TapiClientName As String
	Protected ReadOnly Property MetricService() As IMetricService
	Protected _hasFatalErrorOccured As Boolean
	Protected _tapiClientName As String = TapiClient.None.ToString()

	Public Property CaseInfo As CaseInfo

	Public Property ExecutionSource As ExecutionSource = ExecutionSource.Unknown

	''' <summary>
	''' Gets or sets name of application executing import/export job. This property is used to support telemetry.
	''' </summary>
	''' <returns>The application name</returns>
	Public Property ApplicationName As String = Nothing

	Public Sub New(metricService As IMetricService)
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
		SetEndTime()
		Me.Context.PublishStatusEvent("", $"{JobType} aborted")
	End Sub

	Protected Overridable Sub OnSuccess()
		SetEndTime()
	End Sub

	Protected Overridable Sub OnHasErrors()
		SetEndTime()
	End Sub

	Protected MustOverride Function HasErrors() As Boolean

	Protected Overridable Sub Initialize()
		SetStartTime()
	End Sub

	Protected MustOverride Function Run() As Boolean

	Protected Sub SendMetricJobStarted()
		If InitialTapiClientName Is Nothing Then
			Dim message As MetricJobStarted = New MetricJobStarted
			BuildMetricBase(message)
			MetricService.Log(message)
			InitialTapiClientName = TapiClientName
		End If
	End Sub

	Protected Sub SendMetricJobEndReport(jobStatus As String, statistics As Statistics)
		Dim totalRecordsCount As Long = GetTotalRecordsCount()
		Dim completedRecordsCount As Long = GetCompletedRecordsCount()
		Dim metric As MetricJobEndReport = New MetricJobEndReport() With {.JobStatus = jobStatus, .TotalSizeBytes = (statistics.MetadataBytes + statistics.FileBytes),
				.FileSizeBytes = statistics.FileBytes, .MetadataSizeBytes = statistics.MetadataBytes,
				.TotalRecords = totalRecordsCount, .CompletedRecords = completedRecordsCount,
				.ThroughputBytesPerSecond = CalculateThroughput(statistics.FileBytes + statistics.MetadataBytes),
				.ThroughputRecordsPerSecond = CalculateThroughput(completedRecordsCount)}
		BuildMetricBase(metric)
		MetricService.Log(metric)
	End Sub

	Protected Sub SendMetricJobProgress(metadataThroughput As Double, fileThroughput As Double)
		Dim currentTime As DateTime = DateTime.Now
		SyncLock _lockObject
			If currentTime - _lastSendTime < _metricThrottling Then Return
			_lastSendTime = currentTime
		End SyncLock
		Dim message As MetricJobProgress = New MetricJobProgress With {.MetadataThroughput = metadataThroughput, .FileThroughput = fileThroughput}
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

	Private Sub BuildMetricBase(metric As MetricBase)
		metric.JobType = JobType
		metric.TransferMode = TapiClientName
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

	Private Function CalculateThroughput(size As Long) As Double
		Dim duration As System.TimeSpan = EndTime - StartTime
		Return CDbl(IIf(duration.TotalSeconds.Equals(0.0), 0.0, size/duration.TotalSeconds))
	End Function
End Class
