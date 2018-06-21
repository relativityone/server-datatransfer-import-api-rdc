Imports System.Threading.Tasks
Imports kCura.WinEDDS
Imports kCura.WinEDDS.Monitoring
Imports NSubstitute
Imports NUnit.Framework
Imports Relativity.DataTransfer.MessageService
Imports Relativity.DataTransfer.MessageService.MetricsManager.APM
Imports Relativity.Services.ServiceProxy

Namespace Monitoring.Sinks
	<TestFixture>
	Public Class ToggleSinkTests
		<Test>
		Public Sub WhenToggleIfOn_ThenPassThroughMessages
			Dim aSink As IMetricSink(Of MockMessage) =  Substitute.For(Of IMetricSink(Of MockMessage))

			Dim subject As IMetricSink(Of MockMessage) = New ToggledMetricSink(Of MockMessage)(aSink, Function() True)
			subject.OnMessage(New MockMessage())
			subject.OnMessage(New MockMessage())
			subject.OnMessage(New MockMessage())
			subject.OnMessage(New MockMessage())

			aSink.ReceivedWithAnyArgs(4).OnMessage(Arg.Any(Of MockMessage))
		End Sub

		<Test>
		Public Sub WhenToggleIfOff_ThenDontSendMessages
			Dim aSink As IMetricSink(Of MockMessage) =  Substitute.For(Of IMetricSink(Of MockMessage))

			Dim subject As IMetricSink(Of MockMessage) = New ToggledMetricSink(Of MockMessage)(aSink, Function() False)
			subject.OnMessage(New MockMessage())
			subject.OnMessage(New MockMessage())
			subject.OnMessage(New MockMessage())
			subject.OnMessage(New MockMessage())

			aSink.DidNotReceiveWithAnyArgs().OnMessage(Arg.Any(Of MockMessage))
		End Sub

		<Test>
		Public Sub WhenToggleIfOff_AndChanged_ThenChangesAreReflectedLive
			Dim aSink As IMetricSink(Of MockMessage) =  Substitute.For(Of IMetricSink(Of MockMessage))
			Dim switch As Boolean
			Dim subject As IMetricSink(Of MockMessage) = New ToggledMetricSink(Of MockMessage)(aSink, Function() switch)
			switch = True
			subject.OnMessage(New MockMessage())
			switch = False
			subject.OnMessage(New MockMessage())
			switch = True
			subject.OnMessage(New MockMessage())
			switch = False
			subject.OnMessage(New MockMessage())

			aSink.ReceivedWithAnyArgs(2).OnMessage(Arg.Any(Of MockMessage))
		End Sub

		Public Class MockMessage
			Implements IMessage
		End Class
	End Class
End Namespace