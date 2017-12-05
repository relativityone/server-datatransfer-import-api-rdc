Imports System.Collections.Generic
Imports System.Data
Imports System.Runtime.InteropServices
Imports System.Threading
Imports kCura.Windows.Process
Imports kCura.WinEDDS.TApi
Imports Relativity

Namespace kCura.WinEDDS.ImportExtension
	Public Class DataReaderImageImporter
		Inherits kCura.WinEDDS.BulkImageFileImporter

		Private _sourceTable As System.Data.DataTable

		Public Sub New(ByVal folderId As Int32, ByVal imageLoadFile As kCura.WinEDDS.ImageLoadFile, ByVal controller As kCura.Windows.Process.Controller, ByRef ioReporterInstance As IIoReporter, 
					   ByRef logger As Logging.ILog, ByVal processID As System.Guid, ByVal sourceDataReader As System.Data.DataTable, ByVal enforceDocumentLimit As Boolean, ByRef tokenSource As CancellationTokenSource,
					   Optional executionSource As Relativity.ExecutionSource = Relativity.ExecutionSource.Unknown)
			MyBase.New(folderId, imageLoadFile, controller, ioReporterInstance, logger, processID, False, enforceDocumentLimit, tokenSource, executionSource)
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

