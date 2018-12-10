Imports System.Collections.Generic

Namespace kCura.WinEDDS.Exporters
    Public Class OpticonFilenameComparer
        Implements IComparer(Of String)

		Public Function Compare(x As String, y As String) As Integer Implements IComparer(Of String).Compare
			Dim firstFilename As OpticonPageFilename = GetOpticonPageFilename(x)
			Dim secondFilename As OpticonPageFilename = GetOpticonPageFilename(y)
			If Not String.Equals(firstFilename.Prefix, secondFilename.Prefix) Then
				Return String.Compare(firstFilename.Prefix, secondFilename.Prefix)
			End If
			Return firstFilename.PageNumber - secondFilename.PageNumber
		End Function

		Private Function GetOpticonPageFilename(originalLine As String) As OpticonPageFilename
			Dim filename As String = GetTrimmedFilename(originalLine)
			Dim filenameLength As Integer = filename.Length

			Dim splitted As String() = filename.Split("_"c)
			Dim splittedLength As Integer = splitted.Length
			If splittedLength = 1 Then
				Return New OpticonPageFilename(0, filename)
			End If

			Dim lastNumberString As String = splitted(splittedLength - 1)
			Dim lastNumber As Integer
			If Not Integer.TryParse(lastNumberString, lastNumber) Then
				Return New OpticonPageFilename(0, filename)
			End If

			Dim lastNumberStringLength As Integer = lastNumberString.Length
			Dim prefix As String = filename.Remove(filenameLength - lastNumberStringLength)
			Return New OpticonPageFilename(lastNumber, prefix)
		End Function

		Private Function GetTrimmedFilename(originalLine As String) As String
			Dim splittedOriginalLine As String() = originalLine.Split(","c)
			If splittedOriginalLine.Length <> 7 Then
				Return originalLine
			End If
            Dim filenameWithExtension As String = splittedOriginalLine(2)
            Dim index As Integer = filenameWithExtension.LastIndexOf("."c)
			If index < 0
				Return filenameWithExtension
			End If
			Dim filename As String = filenameWithExtension.Substring(0, index)
            Return filename
        End Function

		Private Class OpticonPageFilename

			Public Sub New(pageNumber As Integer, prefix As String)
				Me.PageNumber = pageNumber
				Me.Prefix = prefix
			End Sub

			Public ReadOnly Property PageNumber As Integer
			Public ReadOnly Property Prefix As String

		End Class

	End Class
End Namespace