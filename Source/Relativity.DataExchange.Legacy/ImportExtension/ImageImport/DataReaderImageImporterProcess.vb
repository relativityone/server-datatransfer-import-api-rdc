Imports Monitoring.Sinks
Imports Relativity.DataExchange
Imports Relativity.DataExchange.Io

Namespace kCura.WinEDDS.ImportExtension
	Public Class DataReaderImageImporterProcess
		Inherits kCura.WinEDDS.ImportImageFileProcess

		Private _sourceData As System.Data.DataTable

		Public Sub New(ByVal sourceData As System.Data.DataTable, ByVal metricService As IMetricService)
			MyBase.New(metricService)
			_sourceData = sourceData
		End Sub

		Protected Overrides Function GetImageFileImporter() As kCura.WinEDDS.BulkImageFileImporter
			_ioReporterContext = New IoReporterContext(Me.FileSystem, Me.AppSettings, New WaitAndRetryPolicy(Me.AppSettings))
			Dim reporter As IIoReporter = Me.CreateIoReporter(_ioReporterContext)
			Return New DataReaderImageImporter(
				ImageLoadFile.DestinationFolderID, _
				ImageLoadFile, _
				Me.Context, _
				reporter, _
				logger, _
				System.Guid.NewGuid, _
				_sourceData, _
				enforceDocumentLimit, _
				Me.CancellationTokenSource, _
				ExecutionSource)
		End Function

		Protected Overrides Sub OnExecute()
			MyBase.OnExecute()
		End Sub

	End Class
End Namespace