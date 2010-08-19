Imports System.Collections.Generic

Namespace kCura.WinEDDS.ImportExtension
	Public Class DataReaderImageImporter
		Inherits kCura.WinEDDS.BulkImageFileImporter

		Private _sourceReader As System.Data.IDataReader

		Public Sub New(ByVal folderId As Int32, ByVal imageLoadFile As kCura.WinEDDS.ImageLoadFile, ByVal controller As kCura.Windows.Process.Controller, ByVal processID As System.Guid, ByVal sourceDataReader As System.Data.IDataReader)
			MyBase.New(folderId, imageLoadFile, controller, processID, False)
			_sourceReader = sourceDataReader
		End Sub

		Public Overrides Function GetImageReader() As kCura.WinEDDS.Api.IImageReader
			'Return New Datasource
			Return New ImageDataTableReader(DirectCast(SourceData, System.Data.IDataReader))
		End Function

		Public ReadOnly Property SourceData() As System.Data.IDataReader
			Get
				Return _sourceReader
			End Get
		End Property
	End Class
End Namespace

