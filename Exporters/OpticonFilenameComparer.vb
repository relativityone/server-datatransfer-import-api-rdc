Imports System.Collections.Generic

Namespace kCura.WinEDDS.Exporters
    Public Class OpticonFilenameComparer
        Implements IComparer(Of String)

        Public Function Compare(x As String, y As String) As Integer Implements IComparer(Of String).Compare
            Dim firstPrefix As String = Nothing
            Dim secondPrefix As String = Nothing
            Dim firstNumber As Integer = GetDocumentPageNumber(x, firstPrefix)
            Dim secondNumber As Integer = GetDocumentPageNumber(y, secondPrefix)
            If Not String.Equals(firstPrefix, secondPrefix) Then
                Return String.Compare(firstPrefix, secondPrefix)
            End If
            Return firstNumber - secondNumber
        End Function

        Private Function GetDocumentPageNumber(originalLine As String, ByRef prefix As String) As Integer
            Dim filename As String = GetTrimmedFilename(originalLine)
            Dim filenameLength As Integer = filename.Length

            Dim splitted As String() = filename.Split("_"c)
            Dim splittedLength As Integer = splitted.Length
            If splittedLength = 1 Then
                prefix = filename
                Return 0
            End If

            Dim lastNumberString As String = splitted(splittedLength - 1)
            Dim lastNumber As Integer
            If Not Integer.TryParse(lastNumberString, lastNumber) Then
                prefix = filename
                Return 0
            End If

            Dim lastNumberStringLength As Integer = lastNumberString.Length
            prefix = filename.Remove(filenameLength - lastNumberStringLength)
            Return lastNumber
        End Function

        Private Function GetTrimmedFilename(originalLine As String) As String
            Dim filenameWithExtension As String = originalLine.Split(","c)(2)
            Dim index As Integer = filenameWithExtension.LastIndexOf("."c)
            Dim filename As String = filenameWithExtension.Substring(0, index)
            Return filename
        End Function

    End Class
End Namespace