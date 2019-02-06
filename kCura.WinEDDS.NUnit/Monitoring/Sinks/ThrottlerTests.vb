Imports System.Threading.Tasks
Imports kCura.WinEDDS.Monitoring
Imports NUnit.Framework
Imports Relativity.DataTransfer.MessageService
Imports Relativity.Services.ServiceProxy
Imports NSubstitute
Imports Relativity.DataTransfer.MessageService.MetricsManager.APM

Namespace kCura.WinEDDS.NUnit.Monitoring.Sinks

	<TestFixture>
	Public Class ThrottlerTests
		<Test>
		Public Sub WhenToggleIfOff_ThenDontSendMessages
			Dim aSink As IMetricSink(Of MockMessage) =  Substitute.For(Of IMetricSink(Of MockMessage))

			Dim subject As IMetricSink(Of MockMessage) = New ThrottledMetricSink(Of MockMessage)(aSink, Function() TimeSpan.FromMinutes(1))
			subject.OnMessage(New MockMessage())
			subject.OnMessage(New MockMessage())

			aSink.ReceivedWithAnyArgs(1).OnMessage(Arg.Any(Of MockMessage))
		End Sub

		Public Class MockMessage
			Implements IMessage
		End Class
	End Class
End Namespace
