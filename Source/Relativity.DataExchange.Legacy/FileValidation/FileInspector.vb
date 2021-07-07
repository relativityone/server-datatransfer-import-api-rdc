Imports OutsideIn

Namespace kCura.WinEDDS
	Public Class FileInspector
		Implements IFileInspector

		Private Readonly _exporter As OutsideIn.Exporter

		Sub New()
			_exporter = OutsideIn.OutsideIn.NewLocalExporter()
		End Sub

		Public Function GetFileFormatByFilePath(filePath As String) As FileFormat Implements IFileInspector.GetFileFormatByFilePath
			Dim retVal As FileFormat = _exporter.Identify(New System.IO.FileInfo(filePath))
			Return retVal
		End Function

	End Class

End Namespace