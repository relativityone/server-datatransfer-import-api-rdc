Imports kCura.WinEDDS
Imports Relativity.Desktop.Client.Legacy.Controls
Imports Relativity.Import.Export.Services

Namespace Relativity.Desktop.Client
	Public Class Utility
		Public Shared Sub InitializeCharacterDropDown(ByVal ddown As System.Windows.Forms.ListControl, ByVal selectedValue As Char)
			ddown.Tag = False
			ddown.DataSource = kCura.WinEDDS.Utility.BuildProxyCharacterDatatable()
			ddown.ValueMember = "CharValue"
			ddown.DisplayMember = "Display"
			ddown.SelectedValue = selectedValue
			ddown.Tag = True
		End Sub

		Public Shared Sub ExtractValueFromCharacterDropDown(ByVal ddown As System.Windows.Forms.ComboBox, ByRef charvalue As Char)
			Dim x As Char = charvalue
			Try
				charvalue = CType(ddown.SelectedValue, Char)
			Catch ex As Exception
				charvalue = x
			End Try
		End Sub

		Public Shared Function ExtractFieldMap(ByVal tLSelect As TwoListBox, ByVal docFieldList As DocumentFieldCollection) As DocumentField()
			Dim i As Int32
			Dim docfields(tLSelect.RightSearchableListItems.Count - 1) As DocumentField
			Dim docfield As DocumentField
			For i = 0 To docfields.Length - 1
				docfield = docFieldList.Item(CType(tLSelect.RightSearchableListItems.Item(i), String))
				docfields(i) = docfield
			Next
			Return docfields
		End Function

		Public Shared Function ExtractFieldMap(ByVal caseFields As Global.Relativity.Desktop.Client.Legacy.Controls.TwoListBox, ByVal fileColumns As TwoListBox, ByVal docFieldList As DocumentFieldCollection, ByVal artifactTypeID As Int32, ObjectFieldIdList As IList(Of Int32)) As LoadFileFieldMap
			Dim selectedFields As List(Of Object) = caseFields.RightSearchableListItems
			Dim selectedColumns As List(Of Object) = fileColumns.LeftSearchableListItems
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

				If (Not ObjectFieldIdList Is Nothing AndAlso ObjectFieldIdList.Contains(docfield.FieldID)) Then
					docfield.ImportBehavior = kCura.EDDS.WebAPI.DocumentManagerBase.ImportBehaviorChoice.ObjectFieldContainsArtifactId
				End If
				fieldMap.Add(New LoadFileFieldMap.LoadFileFieldMapItem(docfield, columnIndex))
			Next
			Return fieldMap
		End Function

		Public Shared Function ExtractFieldNames(ByVal list As List(Of Object)) As String()
			Dim i As Int32
			Dim names(list.Count - 1) As String
			For i = 0 To list.Count - 1
				names(i) = CType(list(i), String)
			Next
			Return names
		End Function

		Public Shared Sub ThrowExceptionToGUI(ByVal ex As System.Exception)
			Dim frm As New ErrorForm(ex)
			frm.Show()
		End Sub

		Public Shared Sub InitializeEncodingDropdown(ByVal ddown As System.Windows.Forms.ComboBox)
			ddown.Items.Clear()
			ddown.Items.Add(New kCura.WinEDDS.EncodingListItem(System.Text.Encoding.Default, "Default"))
			ddown.Items.Add(New kCura.WinEDDS.EncodingListItem(System.Text.Encoding.Unicode, "Unicode"))
			ddown.Items.Add(New kCura.WinEDDS.EncodingListItem(System.Text.Encoding.ASCII, "ASCII"))
			ddown.Items.Add(New kCura.WinEDDS.EncodingListItem(System.Text.Encoding.BigEndianUnicode, "Big-Endian Unicode"))
			ddown.Items.Add(New kCura.WinEDDS.EncodingListItem(System.Text.Encoding.UTF7, "UTF-7"))
			ddown.Items.Add(New kCura.WinEDDS.EncodingListItem(System.Text.Encoding.UTF8, "UTF-8"))
			ddown.SelectedIndex = 0
		End Sub

		Public Shared Function ConvertOverwriteDestinationToLegacyValues(ByVal loadfile As LoadFile) As LoadFile
			Dim overwriteDestination = loadfile.OverwriteDestination
			Select Case overwriteDestination.ToLower()
				Case ImportOverwriteType.Overlay.ToString.ToLower
					loadfile.OverwriteDestination = "Strict"
				Case ImportOverwriteType.AppendOverlay.ToString.ToLower
					loadfile.OverwriteDestination = "Append"
				Case ImportOverwriteType.Append.ToString.ToLower
					loadfile.OverwriteDestination = "None"
			End Select
			Return loadfile
		End Function

		Public Shared Function ConvertLegacyOverwriteDestinationValueToEnum(ByVal overWriteDestination As String) As String
			Dim retval As String = overWriteDestination
			If Not String.IsNullOrEmpty(overWriteDestination) Then

				Select Case overWriteDestination.ToLower
					Case "strict"
						retval = ImportOverwriteType.Overlay.ToString
					Case "append"
						retval = ImportOverwriteType.AppendOverlay.ToString
					Case "none"
						retval = ImportOverwriteType.Append.ToString
				End Select
			End If
			Return retval
		End Function

		Public Shared Function FindFieldByName(listboxItems As ICollection(Of Object), field As Relativity.Import.Export.Services.ViewFieldInfo) As Relativity.Import.Export.Services.ViewFieldInfo
			Return FindFieldBy(Function(x As Relativity.Import.Export.Services.ViewFieldInfo) x.DisplayName.Equals(field.DisplayName, StringComparison.InvariantCulture), listboxItems)
		End Function

		Public Shared Function FindFieldByArtifactId(listboxItems As ICollection(Of Object), field As Relativity.Import.Export.Services.ViewFieldInfo) As Relativity.Import.Export.Services.ViewFieldInfo
			Return FindFieldBy(Function(x As Relativity.Import.Export.Services.ViewFieldInfo) x.FieldArtifactId = field.FieldArtifactId, listboxItems)
		End Function

		Public Shared Function FindFieldBy(predicate As Func(Of Relativity.Import.Export.Services.ViewFieldInfo, Boolean), listboxItems As ICollection(Of Object)) As Relativity.Import.Export.Services.ViewFieldInfo
			For Each item As Relativity.Import.Export.Services.ViewFieldInfo In listboxItems
				If predicate(item) Then
					Return item
				End If
			Next
			Return Nothing
		End Function

		''' <summary>
		''' Find field in listboxItems collection which is a counterpart for fields found in kwx settings file.
		''' First compare by name, then by artifactId
		''' Wrap the result in a List or return empty List if a field is not found
		''' </summary>
		''' <param name="listboxItems">collections of all view fields from the workspace</param>
		''' <param name="field">field from mappings from kwx file</param>
		''' <returns></returns>
		Public Shared Function FindCounterpartField(ByRef listboxItems As ICollection(Of Object), ByVal field As Relativity.Import.Export.Services.ViewFieldInfo) As ICollection(Of Object)
			Dim fieldByName As Relativity.Import.Export.Services.ViewFieldInfo = FindFieldByName(listboxItems, field)
			If fieldByName IsNot Nothing Then
				Return New List(Of Object) From {fieldByName}
			End If
			Dim fieldByArtifactId As Relativity.Import.Export.Services.ViewFieldInfo = FindFieldByArtifactId(listboxItems, field)
			If fieldByArtifactId IsNot Nothing Then
				Return New List(Of Object) From {fieldByArtifactId}
			End If
			Return New List(Of Object)
		End Function


	End Class
End Namespace