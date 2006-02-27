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
				rowDisplay = String.Format("{0} (ASCII:{1})", Chr(i), i.ToString.PadLeft(3, "0"c))
				row.Add(rowDisplay)
				rowValue = Chr(i)
				row.Add(rowValue)
				dt.Rows.Add(row.ToArray)
			Next
			Return dt
		End Function

		Public Shared Function BuildSelectedFieldNameList(ByVal allFields As DocumentFieldCollection, ByVal selectedFields As DocumentField()) As String()
			Dim al As New ArrayList
			Dim field As DocumentField
			For Each field In selectedFields
				If allFields.Exists(field.FieldName) Then
					al.Add(allFields.Item(field.FieldName).FieldName)
				End If
			Next
			Return DirectCast(al.ToArray(GetType(String)), String())
		End Function

		Public Shared Function GetFieldNamesFromFieldArray(ByVal documentFields As DocumentField()) As String()
			Dim i As Int32
			Dim retval(documentFields.Length - 1) As String
			For i = 0 To retval.Length - 1
				retval(i) = documentFields(i).FieldName
			Next
			Return retval
		End Function
	End Class
End Namespace