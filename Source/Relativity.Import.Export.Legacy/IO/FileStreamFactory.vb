Imports System.IO

Namespace kCura.WinEDDS.IO
	Public Class FileStreamFactory
		Implements IFileStreamFactory

		Private Readonly _fileHelper As Global.Relativity.Import.Export.Io.IFile

		Public Sub New(fileHelper As Global.Relativity.Import.Export.Io.IFile)
			_fileHelper = fileHelper
		End Sub

		Public Function Create(filePath As String) As FileStream Implements IFileStreamFactory.Create
			Return _fileHelper.Create(filePath)
		End Function

		Public Function Create(filePath As String, append As Boolean) As FileStream Implements IFileStreamFactory.Create
			Return _fileHelper.Create(filePath, append)
		End Function

	End Class
End NameSpace