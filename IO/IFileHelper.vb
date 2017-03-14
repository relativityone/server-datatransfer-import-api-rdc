Imports System.IO

Namespace kCura.WinEDDS.IO

	Public Interface IFileHelper
		Function Create(filePath As String) As FileStream
		Sub Delete(filePath As String)
		Sub Copy(sourceFilePath As String, destinationFilePath As String)
		Sub Copy(sourceFilePath As String, destinationFilePath As String, overwrite As Boolean)
		Function Exists(filePath As String) As Boolean
		Function Create(filePath As String, append As Boolean) As FileStream
	End Interface
End NameSpace