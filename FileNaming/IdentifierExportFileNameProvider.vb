Imports kCura.WinEDDS.Exporters

Namespace kCura.WinEDDS
	Public Class IdentifierExportFileNameProvider
		Implements IFileNameProvider

		Private ReadOnly _exportedObjectInfo As ObjectExportInfo
		Private ReadOnly _exportSettings As ExportFile

		Public Sub New(exportedObjectInfo As ObjectExportInfo, exportSettings As ExportFile)
			_exportedObjectInfo = exportedObjectInfo
			_exportSettings = exportSettings
		End Sub

		Public Function GetName() As String Implements IFileNameProvider.GetName
			Return _exportedObjectInfo.NativeFileName(_exportSettings.AppendOriginalFileName)
		End Function
	End Class
End Namespace