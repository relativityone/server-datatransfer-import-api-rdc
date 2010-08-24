Imports System.IO

Namespace kCura.Relativity.DataReaderClient.NUnit.Helpers
	Public Class FileHelper
		''' <summary>
		''' Writes information to log file
		''' </summary>
		''' <param name="strData">String data to write to text file</param>
		''' <param name="ErrInfo">optional field which holds error information</param>
		''' <remarks></remarks>
		Public Shared Sub SaveTextToFile(ByVal strData As String, ByVal logDirName As String, Optional ByVal ErrInfo As String = "")
			Dim fullPath As String = Path.Combine(logDirName, "DBSetupLog_" + _
				DateTime.Now.ToFileTime().ToString() + _
				".txt")
			If Not Directory.Exists(logDirName) Then
				Directory.CreateDirectory(logDirName)
			End If

			File.WriteAllText(fullPath, strData + ErrInfo)

		End Sub
	End Class
End Namespace
