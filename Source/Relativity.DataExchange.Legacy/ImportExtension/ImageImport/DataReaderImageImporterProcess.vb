Imports kCura.Relativity.DataReaderClient
Imports Monitoring.Sinks
Imports Relativity.DataExchange
Imports Relativity.DataExchange.Io

Namespace kCura.WinEDDS.ImportExtension
	Public Class DataReaderImageImporterProcess
		Inherits kCura.WinEDDS.ImportImageFileProcess

		Private _reader As IDataReader
		Private _imageSettings As ImageSettings

		Public Sub New(ByVal sourceData As ImageSourceIDataReader, ByVal imageSettings As ImageSettings, ByVal metricService As IMetricService, ByVal runningContext As IRunningContext)
			MyBase.New(metricService, runningContext)
			_reader = sourceData.Reader
			_imageSettings = imageSettings
		End Sub

		Protected Overrides Function GetImageFileImporter() As kCura.WinEDDS.BulkImageFileImporter
			_ioReporterContext = New IoReporterContext(Me.FileSystem, Me.AppSettings, New WaitAndRetryPolicy(Me.AppSettings))
			Dim reporter As IIoReporter = Me.CreateIoReporter(_ioReporterContext)
			Return New DataReaderImageImporter(
				ImageLoadFile.DestinationFolderID,
				ImageLoadFile,
				Me.Context,
				reporter,
				Logger,
				System.Guid.NewGuid,
				EnforceDocumentLimit,
				Me.CancellationTokenSource,
				_reader,
				_imageSettings,
				Me.RunningContext.ExecutionSource)
		End Function

		Protected Overrides Sub OnExecute()
			MyBase.OnExecute()
		End Sub

	End Class
End Namespace