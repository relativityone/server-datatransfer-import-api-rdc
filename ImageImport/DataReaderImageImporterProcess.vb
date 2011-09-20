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

			Return New DataReaderImageImporter(1003697, ImageLoadFile, New kCura.Windows.Process.Controller, System.Guid.NewGuid, _sourceData)

		End Function

		Protected Overrides Sub Execute()
			MyBase.Execute()
			Dim tempdir As String = System.IO.Path.GetTempPath & "FlexMigrationFiles\"
			If System.IO.Directory.Exists(tempdir) Then System.IO.Directory.Delete(tempdir, True)
		End Sub

	End Class
End Namespace

