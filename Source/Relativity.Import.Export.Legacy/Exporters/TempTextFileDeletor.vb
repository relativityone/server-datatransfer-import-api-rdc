﻿Namespace kCura.WinEDDS
	Public Class TempTextFileDeletor
		Private _filepaths As System.Collections.Generic.IEnumerable(Of String)
		Public Sub New(ByVal filePaths As System.Collections.Generic.IEnumerable(Of String))
			_filepaths = filePaths
			If _filepaths Is Nothing Then _filepaths = New String() {}
		End Sub
		Public Sub DeleteFiles()
			For Each path As String In _filepaths
				If Not String.IsNullOrEmpty(path) Then Relativity.Import.Export.Io.FileSystem.Instance.File.Delete(path)
			Next
		End Sub
	End Class
End Namespace