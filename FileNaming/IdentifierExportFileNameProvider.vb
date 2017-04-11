Imports kCura.WinEDDS.Exporters

Namespace kCura.WinEDDS
	Public Class IdentifierExportFileNameProvider
		Implements IFileNameProvider

		Private ReadOnly _exportSettings As ExportFile

		Public Sub New(exportSettings As ExportFile)
			_exportSettings = exportSettings
		End Sub

		Public Function GetName(exportedObjectInfo As ObjectExportInfo) As String Implements IFileNameProvider.GetName
			Return exportedObjectInfo.NativeFileName(_exportSettings.AppendOriginalFileName)
		End Function
	End Class
End Namespace