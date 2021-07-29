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

		Public Sub New(ByVal folderId As Int32,
					   ByVal imageLoadFile As kCura.WinEDDS.ImageLoadFile,
					   ByVal context As ProcessContext,
					   ByVal ioReporterInstance As IIoReporter,
					   ByVal logger As ILog,
					   ByVal processID As System.Guid,
					   ByVal tokenSource As CancellationTokenSource,
					   ByVal reader As IDataReader,
					   ByVal imageSettings As ImageSettings,
					   ByVal correlationIdFunc As Func(Of String),
					   Optional executionSource As ExecutionSource = ExecutionSource.Unknown)
			MyBase.New(folderId,
					   imageLoadFile,
					   context,
					   ioReporterInstance,
					   logger,
					   processID,
					   False,
					   tokenSource,
					   correlationIdFunc,
					   executionSource)
			_reader = reader
			_imageSettings = imageSettings
		End Sub

		Public Overrides Function GetImageReader() As kCura.WinEDDS.Api.IImageReader
			Return New ImageDataReader(Reader, _imageSettings)
		End Function

		Friend Overrides ReadOnly Property TotalRecords As Long
			Get
				Return _imageReader.CountRecords.GetValueOrDefault()
			End Get
		End Property

		Public ReadOnly Property Reader() As IDataReader
			Get
				Return _reader
			End Get
		End Property
	End Class
End Namespace