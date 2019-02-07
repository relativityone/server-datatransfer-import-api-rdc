Namespace kCura.WinEDDS.Helpers
	Public Class CaseInsensitiveFilePathHelper
		Implements IFilePathHelper

		Private ReadOnly _fileSystem As kCura.WinEDDS.TApi.IFileSystem

		Public Sub New(fileSystem As kCura.WinEDDS.TApi.IFileSystem)
			_fileSystem = fileSystem
		End Sub

		Public Function GetExistingFilePath(path As String) As String Implements IFilePathHelper.GetExistingFilePath
			If String.IsNullOrEmpty(path) Then
				Return Nothing
			End If

			Try
				Return If(_fileSystem.File.Exists(path), path, Nothing)
			Catch ex As Exception
				Return Nothing
			End Try
		End Function
	End Class
End NameSpace