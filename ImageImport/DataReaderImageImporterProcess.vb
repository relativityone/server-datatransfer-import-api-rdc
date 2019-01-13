Imports System.Data
Imports System.Threading
Imports kCura.WinEDDS.Helpers
Imports kCura.WinEDDS.TApi

Namespace kCura.WinEDDS.ImportExtension
	Public Class DataReaderImageImporterProcess
		Inherits kCura.WinEDDS.ImportImageFileProcess

		Private _sourceData As System.Data.DataTable

		Public Sub New(ByVal sourceData As System.Data.DataTable)
			MyBase.New()
			_sourceData = sourceData
		End Sub

		Protected Overrides Function GetImageFileImporter() As kCura.WinEDDS.BulkImageFileImporter
			Dim tokenSource As CancellationTokenSource = New CancellationTokenSource()
		    Dim logger As Relativity.Logging.ILog = RelativityLogFactory.CreateLog(RelativityLogFactory.WinEDDSSubSystem)
		    Dim ioWarningPublisher As New IoWarningPublisher()
		    Dim ioReporter As IIoReporter = IoReporterFactory.CreateIoReporter(
			    kCura.Utility.Config.IOErrorNumberOfRetries, _
			    kCura.Utility.Config.IOErrorWaitTimeInSeconds, _
			    WinEDDS.Config.DisableNativeLocationValidation, _
			    WinEDDS.Config.RetryOptions, _
			    logger, _
			    ioWarningPublisher, _ 
			    tokenSource.Token)
			Return New DataReaderImageImporter(ImageLoadFile.DestinationFolderID, ImageLoadFile, Me.ProcessController, ioReporter, logger, System.Guid.NewGuid, _sourceData, enforceDocumentLimit, tokenSource, ExecutionSource)
		End Function

		Protected Overrides Sub Execute()
			MyBase.Execute()
		End Sub

	End Class
End Namespace

