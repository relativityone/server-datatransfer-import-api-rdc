Imports System.IO

Public Class FileNameHelper
	Public Shared Function AppendExtensionToFileWhenMissing(fileName As String, extension As String) As String
		If String.IsNullOrWhiteSpace(extension) Then
			Return fileName
		End If

		If fileName Is Nothing Then
			Return Nothing
		End If

		Dim currentExtension As String = Path.GetExtension(fileName)
		If extension.Equals(currentExtension, StringComparison.InvariantCultureIgnoreCase) Then
			Return fileName
		Else
			Return fileName + extension
		End If
	End Function
End Class
