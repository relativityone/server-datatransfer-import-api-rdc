Imports System.Data

Namespace kCura.WinEDDS.ImportExtension
	Public Class DataReaderImageImporterProcess
		Inherits kCura.WinEDDS.ImportImageFileProcess

		Private _sourceData As System.Data.DataTable

		Public Sub New(ByVal sourceData As System.Data.DataTable)
			MyBase.New()
			_sourceData = sourceData
		End Sub

		Protected Overrides Function GetImageFileImporter() As kCura.WinEDDS.BulkImageFileImporter
			Return New DataReaderImageImporter(ImageLoadFile.DestinationFolderID, ImageLoadFile, Me.ProcessController, System.Guid.NewGuid, _sourceData, CloudInstance, ExecutionSource)

		End Function

		Protected Overrides Sub Execute()
			MyBase.Execute()
		End Sub

	End Class
End Namespace

