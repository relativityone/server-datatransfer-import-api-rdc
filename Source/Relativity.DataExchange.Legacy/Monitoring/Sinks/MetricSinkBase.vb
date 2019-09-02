Imports Relativity.DataTransfer.MessageService.Tools

Namespace Monitoring.Sinks
    Public MustInherit Class MetricSinkBase
        Implements IMessageSink(Of MetricBase)

        Public Sub OnMessage(message As MetricBase) Implements IMessageSink(Of MetricBase).OnMessage
            Log(message)
        End Sub

        ''' <summary>
        ''' Send a given metric to the sink
        ''' </summary>
        ''' <param name="metric">Metric to send</param>
        Protected MustOverride Sub Log(metric As MetricBase)
    End Class
End NameSpace