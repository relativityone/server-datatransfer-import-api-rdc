Imports kCura.WinEDDS
Imports Monitoring.Sinks
Imports Relativity.DataExchange
Imports Relativity.DataExchange.Service

Namespace Processes

    Public Class RdcImportImageFileProcess
        Inherits kCura.WinEDDS.ImportImageFileProcess

        Public Sub New(metricService As IMetricService, runningContext As IRunningContext, logger As Global.Relativity.Logging.ILog)
            MyBase.New(metricService, runningContext, logger)
        End Sub

        Protected Overrides Sub OnSuccess()
            MyBase.OnSuccess()
            Me.SendAutomatedWorkflowTrigger(False)
        End Sub

        Protected Overrides Sub OnHasErrors()
            MyBase.OnHasErrors()
            Me.SendAutomatedWorkflowTrigger(True)
        End Sub

        Private Sub SendAutomatedWorkflowTrigger(hasErrors As Boolean)
            Dim triggerManager As TriggerManager = New TriggerManager(Logger, ServiceFactoryFactory.Create(ImageLoadFile.Credential))
            triggerManager.AttemptSendingTriggerAsync(ImageLoadFile.CaseInfo.ArtifactID, hasErrors, Me.RunningContext.RelativityVersion).GetAwaiter().GetResult()
        End Sub

    End Class

End Namespace
