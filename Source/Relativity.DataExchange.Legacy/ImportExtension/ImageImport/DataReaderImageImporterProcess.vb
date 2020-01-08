Imports kCura.Relativity.DataReaderClient
Imports Monitoring.Sinks
Imports Relativity.DataExchange
Imports Relativity.DataExchange.Io

Namespace kCura.WinEDDS.ImportExtension
	Public Class DataReaderImageImporterProcess
		Inherits kCura.WinEDDS.ImportImageFileProcess

		Private _reader As IDataReader
		Private _imageSettings As ImageSettings

		Public Sub New(ByVal sourceData As ImageSourceIDataReader, ByVal imageSettings As ImageSettings, ByVal metricService As IMetricService)
			MyBase.New(metricService)
			_reader = sourceData.Reader
			_imageSettings = imageSettings
		End Sub

		''' <summary>
		''' We need to subtracts 1 because in <see cref="BulkImageFileImporter"/> we increment
		''' <see cref="BulkImageFileImporter.FileTapiProgressCount"/> to take into account
		''' the start line but this is necessary only in RDC. In Import API we don't need to do this.
		''' </summary>
		''' <returns>Number of completed records</returns>
		Protected Overrides Function GetCompletedRecordsCount() As Long
			Dim completedRecords As Long = MyBase.GetCompletedRecordsCount()
			Return CLng(IIf(completedRecords = 0, completedRecords, completedRecords - 1))
		End Function

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
				ExecutionSource)
		End Function

		Protected Overrides Sub OnExecute()
			MyBase.OnExecute()
		End Sub

	End Class
End Namespace