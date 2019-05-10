Imports System.IO

Namespace kCura.WinEDDS.IO

	Public Interface IFileStreamFactory
		Function Create(filePath As String) As FileStream
		Function Create(filePath As String, append As Boolean) As FileStream
	End Interface
End Namespace
