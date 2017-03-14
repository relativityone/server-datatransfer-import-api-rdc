Imports System.IO

Namespace kCura.WinEDDS.IO
	Public Class FileHelper
		Implements IFileHelper

		Public Function Create(filePath As String) As FileStream Implements IFileHelper.Create
			return File.Create(filePath)
		End Function

		Public Sub Delete(filePath As String) Implements IFileHelper.Delete
			File.Delete(filePath)
		End Sub

		Public Sub Copy(sourceFilePath As String, destinationFilePath As String) Implements IFileHelper.Copy
			Copy(sourceFilePath, destinationFilePath, false)
		End Sub

		Public Sub Copy(sourceFilePath As String, destinationFilePath As String, overwrite As Boolean) Implements IFileHelper.Copy
			File.Copy(sourceFilePath, destinationFilePath, overwrite)
		End Sub

		Public Function Exists(filePath As String) As Boolean Implements IFileHelper.Exists
			return File.Exists(filePath)
		End Function

		Public Function Create(filePath As String, append As Boolean) As FileStream Implements IFileHelper.Create
			Dim mode As FileMode = If(append, FileMode.Append, FileMode.Create)
			return new FileStream(filePath, mode, FileAccess.ReadWrite, FileShare.None)
		End Function
	End Class
End NameSpace