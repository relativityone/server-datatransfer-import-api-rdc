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
		    Dim logger As Relativity.Logging.ILog = RelativityLogFactory.CreateLog("WinEDDS")
		    Dim ioWarningPublisher As New IoWarningPublisher()
		    Dim ioReporter As IIoReporter = IoReporterFactory.CreateIoReporter(kCura.Utility.Config.IOErrorNumberOfRetries, kCura.Utility.Config.IOErrorWaitTimeInSeconds, 
		                                                                       WinEDDS.Config.DisableNativeLocationValidation,  logger, ioWarningPublisher, tokenSource.Token)

			Return New DataReaderImageImporter(ImageLoadFile.DestinationFolderID, ImageLoadFile, Me.ProcessController, ioReporter, logger, System.Guid.NewGuid, _sourceData, enforceDocumentLimit, tokenSource, ExecutionSource)

		End Function

		Protected Overrides Sub Execute()
			MyBase.Execute()
		End Sub

	End Class
End Namespace

