Imports System.Threading
Imports Relativity
Imports Relativity.Import.Export.Io
Imports Relativity.Import.Export.Process

Namespace kCura.WinEDDS.ImportExtension
	Public Class DataReaderImageImporter
		Inherits kCura.WinEDDS.BulkImageFileImporter

		Private _sourceTable As System.Data.DataTable

		Public Sub New(ByVal folderId As Int32, _
		               ByVal imageLoadFile As kCura.WinEDDS.ImageLoadFile, _
		               ByVal context As ProcessContext, _
		               ByVal ioReporterInstance As IIoReporter, 
		               ByVal logger As Logging.ILog, _
		               ByVal processID As System.Guid, _
		               ByVal sourceDataReader As System.Data.DataTable, _
		               ByVal enforceDocumentLimit As Boolean, _
		               ByVal tokenSource As CancellationTokenSource,
		               Optional executionSource As Global.Relativity.ExecutionSource = Global.Relativity.ExecutionSource.Unknown)
			MyBase.New(folderId, _
			           imageLoadFile, _
			           context, _
			           ioReporterInstance, _
			           logger, _
			           processID, _
			           False, _
			           enforceDocumentLimit, _
			           tokenSource, _
			           executionSource)
			_sourceTable = sourceDataReader
		End Sub

		Public Overrides Function GetImageReader() As kCura.WinEDDS.Api.IImageReader
			'Return New Datasource
			Return New ImageDataTableReader(SourceData)
		End Function

		Public ReadOnly Property SourceData() As System.Data.DataTable
			Get
				Return _sourceTable
			End Get
		End Property
	End Class
End Namespace