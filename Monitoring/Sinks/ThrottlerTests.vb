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
		Public Async Function Test() As Task
			Dim transferJobApmThroughputMessage As New TransferJobApmThroughputMessage()

			Dim serviceFactory As IServiceFactory = Substitute.For(Of IServiceFactory)
			Dim messageService As MessageService = New MessageService()
			Dim metricsFactory As IMetricsManagerFactory = Substitute.For(Of IMetricsManagerFactory)
			Dim metricsManager As IMetricsManager = Substitute.For(Of IMetricsManager)

			metricsFactory.CreateAPMKeplerManager(Arg.Any(Of IServiceFactory)).Returns(Function (arg) metricsManager)

			Dim metric As IMetricSink = New ThrottledMetricSink(new JobLiveMetricSink(serviceFactory, metricsFactory), Function() TimeSpan.FromSeconds(1))
			metric.Subscribe(messageService)
			Await messageService.Send(transferJobApmThroughputMessage)
			Await messageService.Send(transferJobApmThroughputMessage)

			metricsManager.ReceivedWithAnyArgs(1).LogDouble(Arg.Any(Of string), Arg.Any(Of Double), Arg.Any(Of IMetricMetadata))
		End Function
	End Class
End Namespace
