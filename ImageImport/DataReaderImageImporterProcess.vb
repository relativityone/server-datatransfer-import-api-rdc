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
			Select Case ImageLoadFile.Overwrite.ToLower
				Case "append"
					ImageLoadFile.Overwrite = "none"
				Case "overlay"
					ImageLoadFile.Overwrite = "strict"
				Case Else
					ImageLoadFile.Overwrite = "both"
			End Select

			Return New DataReaderImageImporter(ImageLoadFile.DestinationFolderID, ImageLoadFile, New kCura.Windows.Process.Controller, System.Guid.NewGuid, _sourceData)

		End Function

		Protected Overrides Sub Execute()
			MyBase.Execute()
		End Sub

	End Class
End Namespace

