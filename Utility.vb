Namespace kCura.EDDS.WinForm
	Public Class Utility
		Public Shared Sub InitializeCharacterDropDown(ByVal ddown As System.Windows.Forms.ListControl, ByVal selectedValue As Char)
			ddown.DataSource = WinEDDS.Utility.BuildProxyCharacterDatatable()
			ddown.ValueMember = "CharValue"
			ddown.DisplayMember = "Display"
			ddown.SelectedValue = selectedValue
		End Sub
		Public Shared Function ExtractValueFromCharacterDropDown(ByVal ddown As System.Windows.Forms.ComboBox, ByRef charvalue As Char) As Char
			Dim x As Char = charvalue
			Try
				charvalue = CType(ddown.SelectedValue, Char)
			Catch ex As Exception
				charvalue = x
			End Try
		End Function

		Public Shared Function ExtractFieldMap(ByVal tLSelect As kCura.Windows.Forms.TwoListBox, ByVal docFieldList As DocumentFieldCollection) As DocumentField()
			Dim i As Int32
			Dim docfieldname As String
			Dim docfields(tLSelect.RightListBoxItems.Count - 1) As DocumentField
			Dim docfield As DocumentField
			For i = 0 To docfields.Length - 1
				docfield = docFieldList.Item(CType(tLSelect.RightListBoxItems.Item(i), String))
				docfields(i) = docfield
			Next
			Return docfields
		End Function

		Public Shared Function ExtractFieldMap(ByVal caseFields As kCura.Windows.Forms.TwoListBox, ByVal fileColumns As kCura.Windows.Forms.TwoListBox, ByVal docFieldList As DocumentFieldCollection) As LoadFileFieldMap
			Dim selectedFields As System.Windows.Forms.ListBox.ObjectCollection = caseFields.RightListBoxItems
			Dim selectedColumns As System.Windows.Forms.ListBox.ObjectCollection = fileColumns.LeftListBoxItems
			Dim fieldMap As New kCura.winedds.LoadFileFieldMap
			Dim docfield As DocumentField
			Dim i As Int32
			Dim columnIndex As Int32
			Dim maxfields As Int32 = Math.Max(selectedFields.Count, selectedColumns.Count)
			For i = 0 To maxfields - 1
				If i >= selectedFields.Count Then
					docfield = Nothing
				Else
					docfield = docFieldList.Item(CType(selectedFields(i), String))
				End If
				If i >= selectedColumns.Count Then
					columnIndex = -1
				Else
					Dim columnname As String = CType(selectedColumns(i), String)
					Dim openParenIndex As Int32 = columnname.LastIndexOf("("c) + 1
					Dim closeParenIndex As Int32 = columnname.LastIndexOf(")"c)
					columnIndex = Int32.Parse(columnname.Substring(openParenIndex, closeParenIndex - openParenIndex)) - 1
				End If
				fieldMap.Add(New LoadFileFieldMap.LoadFileFieldMapItem(docfield, columnIndex))
			Next
			Return fieldMap
		End Function

		Public Shared Function ExtractFieldNames(ByVal list As System.Windows.Forms.ListBox.ObjectCollection) As String()
			Dim i As Int32 = 0
			Dim names(list.Count - 1) As String
			For i = 0 To list.Count - 1
				names(i) = CType(list(i), String)
			Next
			Return names
		End Function


		Public Shared Sub ThrowExceptionToGUI(ByVal ex As Exception)
			Dim frm As New kCura.EDDS.WinForm.ErrorForm(ex)
			frm.Show()
		End Sub

	End Class
End Namespace
