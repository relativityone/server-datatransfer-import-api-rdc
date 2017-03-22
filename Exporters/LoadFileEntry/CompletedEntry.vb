Imports System.Text

Namespace kCura.WinEDDS.LoadFileEntry
	Public Class CompletedLoadFileEntry
		Implements ILoadFileEntry

		Public LineBuilder As StringBuilder

		Public Sub New(ByVal entryString As String)
			LineBuilder = New StringBuilder(entryString)
		End Sub

		Public Sub New()
			LineBuilder = New StringBuilder()
		End Sub

		Public Sub Write(ByRef fileWriter As System.IO.StreamWriter) Implements ILoadFileEntry.Write
			fileWriter.Write(LineBuilder.ToString())
		End Sub
	End Class
End Namespace
