Namespace kCura.WinEDDS
	Public Class Utility
		Public Shared Function BuildProxyCharacterDatatable() As DataTable
			Dim i As Int32
			Dim row As ArrayList
			Dim dt As DataTable
			dt = New DataTable
			dt.Columns.Add("Display", GetType(String))
			dt.Columns.Add("CharValue", GetType(Int32))
			Dim rowValue As Char
			Dim rowDisplay As String
			For i = 1 To 255
				row = New ArrayList
				rowDisplay = String.Format("{0} (ASCII:{1})", ChrW(i), i.ToString.PadLeft(3, "0"c))
				row.Add(rowDisplay)
				rowValue = ChrW(i)
				row.Add(rowValue)
				dt.Rows.Add(row.ToArray)
			Next
			Return dt
		End Function

		Public Shared Function GetFieldNamesFromFieldArray(ByVal documentFields As DocumentField()) As String()
			Dim i As Int32
			Dim retval(documentFields.Length - 1) As String
			For i = 0 To retval.Length - 1
				retval(i) = documentFields(i).FieldName
			Next
			Return retval
		End Function

		Public Shared Function GetFilesystemSafeName(ByVal input As String) As String
			Dim output As String = String.Copy(input)
			output = output.Replace("/", " ")
			output = output.Replace(":", " ")
			output = output.Replace("?", " ")
			output = output.Replace("*", " ")
			output = output.Replace("<", " ")
			output = output.Replace(">", " ")
			output = output.Replace("|", " ")
			output = output.Replace("\", " ")
			output = output.Replace("""", " ")
			Return output
		End Function
	End Class
End Namespace