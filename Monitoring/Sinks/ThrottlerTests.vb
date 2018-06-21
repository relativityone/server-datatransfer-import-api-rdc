

Imports System.Threading
Imports kCura.WinEDDS.Monitoring
Imports NSubstitute
Imports NUnit.Framework
Imports Relativity.DataTransfer.MessageService

Namespace Monitoring.Sinks

	<TestFixture>
	Public Class ThrottlerTests
		<Test>
		Public Sub WhenThrottlingIsOn_ThenSendOnlyNotThrottledMessage
			Dim aSink As IMetricSink(Of MockMessage) =  Substitute.For(Of IMetricSink(Of MockMessage))

			Dim subject As IMetricSink(Of MockMessage) = New ThrottledMetricSink(Of MockMessage)(aSink, Function() TimeSpan.FromMinutes(1))
			subject.OnMessage(New MockMessage())
			subject.OnMessage(New MockMessage())
			subject.OnMessage(New MockMessage())
			subject.OnMessage(New MockMessage())

			aSink.ReceivedWithAnyArgs(1).OnMessage(Arg.Any(Of MockMessage))
		End Sub

		<Test>
		Public Sub WhenThrottlingIsSetToZero_ThenSendOnlyAllMessages
			Dim aSink As IMetricSink(Of MockMessage) =  Substitute.For(Of IMetricSink(Of MockMessage))

			Dim subject As ThrottledMetricSink(Of MockMessage) = New ThrottledMetricSink(Of MockMessage)(aSink, Function() TimeSpan.Zero)
			subject.OnMessage(New MockMessage())
			subject.OnMessage(New MockMessage())
			subject.OnMessage(New MockMessage())
			subject.OnMessage(New MockMessage())

			aSink.ReceivedWithAnyArgs(4).OnMessage(Arg.Any(Of MockMessage))
		End Sub

		<Test>
		Public Sub WhenThrottlingIsSet_AndTimeouts_ThePassRemainingMessages
			Dim aSink As IMetricSink(Of MockMessage) =  Substitute.For(Of IMetricSink(Of MockMessage))

			Dim subject As IMetricSink(Of MockMessage) = New ThrottledMetricSink(Of MockMessage)(aSink, Function() TimeSpan.FromMilliseconds(200))
			subject.OnMessage(New MockMessage())
			subject.OnMessage(New MockMessage())
			subject.OnMessage(New MockMessage())
			subject.OnMessage(New MockMessage())
			Thread.Sleep(300)
			subject.OnMessage(New MockMessage())
			subject.OnMessage(New MockMessage())
			subject.OnMessage(New MockMessage())
			subject.OnMessage(New MockMessage())

			aSink.ReceivedWithAnyArgs(2).OnMessage(Arg.Any(Of MockMessage))
		End Sub

		Public Class MockMessage
			Implements IMessage
		End Class
	End Class
End Namespace
