Imports Relativity.DataTransfer.MessageService.Tools

Namespace Monitoring.Sinks
    Public MustInherit Class MetricSinkBase
        Implements IMessageSink(Of TransferJobCompletedMessage)
        Implements IMessageSink(Of TransferJobCompletedRecordsCountMessage)
        Implements IMessageSink(Of TransferJobFailedMessage)
        Implements IMessageSink(Of TransferJobProgressMessage)
        Implements IMessageSink(Of TransferJobStartedMessage)
        Implements IMessageSink(Of TransferJobStatisticsMessage)
        Implements IMessageSink(Of TransferJobThroughputMessage)
        Implements IMessageSink(Of TransferJobTotalRecordsCountMessage)

        Public Sub OnMessage(message As TransferJobCompletedMessage) Implements IMessageSink(Of TransferJobCompletedMessage).OnMessage
            Log(message)
        End Sub

        Public Sub OnMessage(message As TransferJobCompletedRecordsCountMessage) Implements IMessageSink(Of TransferJobCompletedRecordsCountMessage).OnMessage
            Log(message)
        End Sub

        Public Sub OnMessage(message As TransferJobFailedMessage) Implements IMessageSink(Of TransferJobFailedMessage).OnMessage
            Log(message)
        End Sub

        Public Sub OnMessage(message As TransferJobProgressMessage) Implements IMessageSink(Of TransferJobProgressMessage).OnMessage
            Log(message)
        End Sub

        Public Sub OnMessage(message As TransferJobStartedMessage) Implements IMessageSink(Of TransferJobStartedMessage).OnMessage
            Log(message)
        End Sub

        Public Sub OnMessage(message As TransferJobStatisticsMessage) Implements IMessageSink(Of TransferJobStatisticsMessage).OnMessage
            Log(message)
        End Sub

        Public Sub OnMessage(message As TransferJobThroughputMessage) Implements IMessageSink(Of TransferJobThroughputMessage).OnMessage
            Log(message)
        End Sub

        Public Sub OnMessage(message As TransferJobTotalRecordsCountMessage) Implements IMessageSink(Of TransferJobTotalRecordsCountMessage).OnMessage
            Log(message)
        End Sub

        Protected MustOverride Sub Log(message As TransferJobMessageBase)
    End Class
End NameSpace