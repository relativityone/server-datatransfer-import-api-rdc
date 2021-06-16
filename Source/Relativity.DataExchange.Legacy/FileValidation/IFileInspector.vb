Imports OutsideIn

Namespace kCura.WinEDDS

	Public Interface IFileInspector
		Function GetFileFormatByFilePath(ByVal filePath As String) As FileFormat
	End Interface

End Namespace