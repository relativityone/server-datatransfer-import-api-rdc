﻿Imports kCura.WinEDDS
Imports kCura.WinEDDS.Monitoring
Imports kCura.WinEDDS.TApi
Imports Relativity.DataTransfer.MessageService

Public MustInherit Class MonitoredProcessBase
	Inherits kCura.Windows.Process.ProcessBase

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

	Public Property CaseInfo As Relativity.CaseInfo

	Public Sub New(messageService As IMessageService)
		Me.MessageService = messageService
	End Sub

	Protected Overrides Sub Execute()
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
		Me.ProcessObserver.RaiseStatusEvent("", $"{JobType} aborted")
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
			MessageService.Send(New TransferJobStartedMessage With {.JobType = JobType, .TransferMode = TapiClientName})
		End If
	End Sub

	Protected Sub SendTransferJobFailedMessage()
		MessageService.Send(New TransferJobFailedMessage With {.JobType = JobType, .TransferMode = TapiClientName})
	End Sub

	Protected Sub SendTransferJobCompletedMessage()
		MessageService.Send(New TransferJobCompletedMessage With {.JobType = JobType, .TransferMode = TapiClientName})
	End Sub

	Protected Sub SendThroughputStatistics(metadataThroughput As Double, fileThroughput As Double)
		Dim message As TransferJobApmThroughputMessage = New TransferJobApmThroughputMessage()
		BuildApmBaseMessage(message)
		message.CustomData.Add("MetadataThroughput", metadataThroughput)
		message.CustomData.Add("FileThroughput", fileThroughput)
		MessageService.Send(message)
	End Sub

	Protected Sub SendJobStatistics(statistics As Statistics)
		SendJobThroughputMessage()
		SendJobTotalRecordsCountMessage()
		SendJobCompletedRecordsCountMessage()
		SendJobSize(statistics)
	End Sub

	Protected Sub SendJobThroughputMessage()
		If CompletedRecordsCount = 0 Then
			Return
		End If
		Dim duration As System.TimeSpan = EndTime - StartTime
		Dim recordsPerSecond As Double = CompletedRecordsCount / duration.TotalSeconds
		MessageService.Send(New TransferJobThroughputMessage With {.JobType = JobType, .TransferMode = TapiClientName, .RecordsPerSecond = recordsPerSecond})
	End Sub

	Protected Sub SendJobTotalRecordsCountMessage()
		MessageService.Send(New TransferJobTotalRecordsCountMessage With {.JobType = JobType, .TransferMode = TapiClientName, .TotalRecords = TotalRecords})
	End Sub

	Protected Sub SendJobCompletedRecordsCountMessage()
		MessageService.Send(New TransferJobCompletedRecordsCountMessage With {.JobType = JobType, .TransferMode = TapiClientName, .CompletedRecords = CompletedRecordsCount})
	End Sub

	Private Sub SendJobSize(statistics As Statistics)
		Dim message As TransferJobSizeMessage = New TransferJobSizeMessage() With {
			    .JobType = JobType, 
			    .TransferMode = TapiClientName, 
			    .JobSize = statistics.MetadataBytes + statistics.FileBytes }

		message.CustomData.Add("MetadataBytes", statistics.MetadataBytes)
		message.CustomData.Add("FileBytes", statistics.FileBytes)
		BuildApmBaseMessage(message)
		MessageService.Send(message)
	End Sub
	
	Private Sub BuildApmBaseMessage(message As TransferJobMessageBase)
		message.JobType = JobType
		message.TransferMode = TapiClientName
		message.CorellationID = JobGuid.ToString()
		message.UnitOfMeasure = "Bytes(s)"
		If Not (CaseInfo Is Nothing) Then
			message.WorkspaceID = CaseInfo.ArtifactID
		End If
	End Sub

End Class
