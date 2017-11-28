Imports System.Data
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
		    Dim fileInfoFailedExceptionHelper As IFileInfoFailedExceptionHelper = New FileInfoFailedExceptionHelper
		    Dim logger As Relativity.Logging.ILog = RelativityLogFactory.CreateLog("WinEDDS")
		    Dim ioWarningPublisher As New IoWarningPublisher()
		    Dim ioReporter As IIoReporter = IoReporterFactory.CreateIoReporter(kCura.Utility.Config.IOErrorNumberOfRetries, kCura.Utility.Config.IOErrorWaitTimeInSeconds, 
		                                                                       WinEDDS.Config.DisableNativeLocationValidation, fileInfoFailedExceptionHelper, logger, ioWarningPublisher)

			Return New DataReaderImageImporter(ImageLoadFile.DestinationFolderID, ImageLoadFile, Me.ProcessController, ioReporter, logger, System.Guid.NewGuid, _sourceData, enforceDocumentLimit, ExecutionSource)

		End Function

		Protected Overrides Sub Execute()
			MyBase.Execute()
		End Sub

	End Class
End Namespace

