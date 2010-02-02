Namespace kCura.EDDS.WinForm
	Public Class Utility
		Public Shared Sub InitializeCharacterDropDown(ByVal ddown As System.Windows.Forms.ListControl, ByVal selectedValue As Char)
			ddown.Tag = False
			ddown.DataSource = WinEDDS.Utility.BuildProxyCharacterDatatable()
			ddown.ValueMember = "CharValue"
			ddown.DisplayMember = "Display"
			ddown.SelectedValue = selectedValue
			ddown.Tag = True
		End Sub

		Public Shared Function ExtractValueFromCharacterDropDown(ByVal ddown As System.Windows.Forms.ComboBox, ByRef charvalue As Char) As Char
			Dim x As Char = charvalue
			Try
				charvalue = CType(ddown.SelectedValue, Char)
			Catch ex As System.Exception
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

		Public Shared Function ExtractFieldMap(ByVal caseFields As kCura.Windows.Forms.TwoListBox, ByVal fileColumns As kCura.Windows.Forms.TwoListBox, ByVal docFieldList As DocumentFieldCollection, ByVal artifactTypeID As Int32) As LoadFileFieldMap
			Dim selectedFields As System.Windows.Forms.ListBox.ObjectCollection = caseFields.RightListBoxItems
			Dim selectedColumns As System.Windows.Forms.ListBox.ObjectCollection = fileColumns.LeftListBoxItems
			Dim fieldMap As New kCura.WinEDDS.LoadFileFieldMap
			Dim docfield As DocumentField
			Dim i As Int32
			Dim columnIndex As Int32
			Dim maxfields As Int32 = Math.Max(selectedFields.Count, selectedColumns.Count)
			For i = 0 To maxfields - 1
				If i >= selectedFields.Count Then
					docfield = Nothing
				Else
					docfield = docFieldList.Item(CType(selectedFields(i), String).Replace(" [Identifier]", ""))
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

		Public Shared Sub ThrowExceptionToGUI(ByVal ex As System.Exception)
			Dim frm As New kCura.EDDS.WinForm.ErrorForm(ex)
			frm.Show()
		End Sub

		Public Shared Sub InitializeEncodingDropdown(ByVal ddown As System.Windows.Forms.ComboBox)
			ddown.Items.Clear()
			ddown.Items.Add(New WinEDDS.EncodingListItem(System.Text.Encoding.Default, "Default"))
			ddown.Items.Add(New WinEDDS.EncodingListItem(System.Text.Encoding.Unicode, "Unicode"))
			ddown.Items.Add(New WinEDDS.EncodingListItem(System.Text.Encoding.ASCII, "ASCII"))
			ddown.Items.Add(New WinEDDS.EncodingListItem(System.Text.Encoding.BigEndianUnicode, "Big-Endian Unicode"))
			ddown.Items.Add(New WinEDDS.EncodingListItem(System.Text.Encoding.UTF7, "UTF-7"))
			ddown.Items.Add(New WinEDDS.EncodingListItem(System.Text.Encoding.UTF8, "UTF-8"))
			ddown.SelectedIndex = 0
		End Sub

	End Class
End Namespace