Imports kCura.WinEDDS
Imports Monitoring
Imports Relativity.DataExchange.Process
Imports Relativity.DataExchange.Service
Imports Relativity.DataExchange.Transfer
Imports Relativity.DataTransfer.MessageService

Public MustInherit Class MonitoredProcessBase
	Inherits ProcessBase2

	Private ReadOnly _messageThrottling As TimeSpan
	Protected Property JobGuid As System.Guid = System.Guid.NewGuid()
	Protected Property StartTime As System.DateTime
	Protected Property EndTime As System.DateTime
	Protected Property TotalRecords As Long
	Protected Property CompletedRecordsCount As Long
	Protected Property InitialTapiClientName As String
	Protected MustOverride ReadOnly Property JobType As String
	Protected MustOverride ReadOnly Property TapiClientName As String
	Protected ReadOnly Property MessageService As IMessageService
	Protected _hasFatalErrorOccured As Boolean
	Protected _tapiClientName As String = TapiClient.None.ToString()

	Public Property CaseInfo As CaseInfo

    Public Property ExecutionSource As ExecutionSource = ExecutionSource.Unknown

    Public Property ApplicationName As String = Nothing

	Public Sub New(messageService As IMessageService)
		Me.MessageService = messageService
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

	Protected Sub SendTransferJobStartedMessage()
		If InitialTapiClientName Is Nothing Then
            Dim message As TransferJobStartedMessage = New TransferJobStartedMessage
		    BuildApmBaseMessage(message)
		    MessageService.Send(message)
            InitialTapiClientName = TapiClientName
		End If
	End Sub

	Protected Sub SendTransferJobFailedMessage()
	    Dim message As TransferJobFailedMessage = New TransferJobFailedMessage
	    BuildApmBaseMessage(message)
	    MessageService.Send(message)
    End Sub

	Protected Sub SendTransferJobCompletedMessage()
	    Dim message As TransferJobCompletedMessage = New TransferJobCompletedMessage
	    BuildApmBaseMessage(message)
	    MessageService.Send(message)
    End Sub

	Protected Sub SendThroughputStatistics(metadataThroughput As Double, fileThroughput As Double)
		Dim message As TransferJobProgressMessage = New TransferJobProgressMessage With {.MetadataThroughput = metadataThroughput, .FileThroughput = fileThroughput}
		BuildApmBaseMessage(message)
		MessageService.Send(message)
	End Sub

	Protected Sub SendJobStatistics(statistics As Statistics)
		SendJobThroughputMessage(statistics)
		SendJobTotalRecordsCountMessage()
		SendJobCompletedRecordsCountMessage()
		SendJobSize(statistics)
	End Sub

	Protected Sub SendJobThroughputMessage(statistics As Statistics)
		If CompletedRecordsCount = 0 Then
			Return
		End If
		Dim duration As System.TimeSpan = EndTime - StartTime
		Dim recordsPerSecond As Double
		Dim bytesPerSecond As Double
		If duration.TotalSeconds = 0 Then
			recordsPerSecond = 0
			bytesPerSecond = 0
		Else
			recordsPerSecond = CompletedRecordsCount / duration.TotalSeconds
			bytesPerSecond = (statistics.FileBytes + statistics.MetadataBytes) / duration.TotalSeconds
		End If

	    Dim message As TransferJobThroughputMessage = New TransferJobThroughputMessage With {.RecordsPerSecond = recordsPerSecond, .BytesPerSecond = bytesPerSecond}
	    BuildApmBaseMessage(message)
	    MessageService.Send(message)
    End Sub

	Protected Sub SendJobTotalRecordsCountMessage()
        Dim message As TransferJobTotalRecordsCountMessage = New TransferJobTotalRecordsCountMessage With {.TotalRecords = TotalRecords}
	    BuildApmBaseMessage(message)
	    MessageService.Send(message)
    End Sub

	Protected Sub SendJobCompletedRecordsCountMessage()
        Dim message As TransferJobCompletedRecordsCountMessage = New TransferJobCompletedRecordsCountMessage With {.CompletedRecords = CompletedRecordsCount}
	    BuildApmBaseMessage(message)
	    MessageService.Send(message)
    End Sub

	Private Sub SendJobSize(statistics As Statistics)
		Dim message As TransferJobStatisticsMessage = New TransferJobStatisticsMessage With {.MetadataBytes = statistics.MetadataBytes, .FileBytes = statistics.FileBytes, .JobSizeInBytes = statistics.MetadataBytes + statistics.FileBytes}
		BuildApmBaseMessage(message)
		MessageService.Send(message)
	End Sub

	Private Sub BuildApmBaseMessage(message As TransferJobMessageBase)
		message.JobType = JobType
		message.TransferMode = TapiClientName
		message.CorrelationID = JobGuid.ToString()
		message.CustomData.Add(TelemetryConstants.KeyName.USE_OLD_EXPORT, Me.AppSettings.UseOldExport)
		message.UnitOfMeasure = "Bytes(s)"
        message.ApplicationName = GetApplicationName()
		If Not (CaseInfo Is Nothing) Then
			message.WorkspaceID = CaseInfo.ArtifactID
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
