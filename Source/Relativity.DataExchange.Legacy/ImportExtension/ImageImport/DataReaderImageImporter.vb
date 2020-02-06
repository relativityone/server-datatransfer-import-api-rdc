Imports System.Net.Configuration
Imports System.Threading
Imports kCura.Relativity.DataReaderClient
Imports Relativity.DataExchange.Io
Imports Relativity.DataExchange.Process
Imports Relativity.DataExchange.Service
Imports Relativity.Logging

Namespace kCura.WinEDDS.ImportExtension
	Public Class DataReaderImageImporter
		Inherits kCura.WinEDDS.BulkImageFileImporter

		Private _reader As IDataReader
		Private _imageSettings As ImageSettings

		Public Sub New(ByVal folderId As Int32, _
		               ByVal imageLoadFile As kCura.WinEDDS.ImageLoadFile, _
		               ByVal context As ProcessContext, _
		               ByVal ioReporterInstance As IIoReporter, 
		               ByVal logger As ILog, _
		               ByVal processID As System.Guid, _
		               ByVal enforceDocumentLimit As Boolean, _
		               ByVal tokenSource As CancellationTokenSource,
                       ByVal reader As IDataReader,
					   ByVal imageSettings As ImageSettings, _
		               Optional executionSource As ExecutionSource = ExecutionSource.Unknown)
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
			_reader = reader
			_imageSettings = imageSettings
		End Sub

		Public Overrides Function GetImageReader() As kCura.WinEDDS.Api.IImageReader
			Return New ImageDataReader(Reader, _imageSettings)
		End Function

		Friend Overrides ReadOnly Property TotalRecords As Long
			Get
				' This is temporary workaround for REL-394161
				Return _imageReader.CountRecords.GetValueOrDefault() - 1
			End Get
		End Property

		Public ReadOnly Property Reader() As IDataReader
            Get
                Return _reader
            End Get
        End Property
	End Class
End Namespace