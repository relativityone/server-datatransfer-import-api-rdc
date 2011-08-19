Namespace kCura.EDDS.WinForm
	Public Class CaseSelectForm
		Inherits kCura.EDDS.WinForm.SelectFormBase

#Region " Declarations & Properties "
		Private _selectedCaseInfo As Generic.List(Of Relativity.CaseInfo)

		Public ReadOnly Property SelectedCaseInfo() As Generic.List(Of Relativity.CaseInfo)
			Get
				Return _selectedCaseInfo
			End Get
		End Property

#End Region
		Private _cases As System.Data.DataSet
		Private Sub ItemSelectForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
			_cases = Application.GetCases
			Me.Text = "Relativity Desktop Client | Select Workspace"
			Me.LoadItems(String.Empty)
			Me.Focus()
			SearchQuery.Focus()
		End Sub

		Protected Overrides Sub ItemListView_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ItemListView.SelectedIndexChanged
			Dim list As New Generic.List(Of Relativity.CaseInfo)
			If ItemListView.SelectedItems.Count <> 0 Then
				For Each si As ListViewItem In ItemListView.SelectedItems
					list.Add(DirectCast(si.Tag, Relativity.CaseInfo))
				Next
				_selectedCaseInfo = list
				ConfirmButton.Enabled = True
			Else
				_selectedCaseInfo = list
			End If
		End Sub

		Private Sub CancelBtn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CancelBtn.Click
			_selectedCaseInfo = Nothing
			Me.DialogResult = DialogResult.Cancel
			Me.Close()
		End Sub

		Protected Overrides Sub LoadItems(ByVal searchText As String)
			Me.Cursor = Cursors.WaitCursor
			ItemListView.Items.Clear()
			If _cases Is Nothing Then
				Me.Cursor = Cursors.Default
				Me.Close()
				Exit Sub
			End If
			Dim dt As DataTable = _cases.Tables(0)
			Dim row As DataRow
			For Each row In dt.Rows
				If String.IsNullOrEmpty(searchText.Trim) OrElse CType(row.Item("Name"), String).ToLower.IndexOf(searchText.Trim.ToLower) <> -1 Then
					Dim listItem As New System.Windows.Forms.ListViewItem
					listItem.Text = CType(row.Item("Name"), String)
					listItem.Tag = New Relativity.CaseInfo(row)
					ItemListView.Items.Add(listItem)
				End If
			Next
			NameColumnHeader.Width = ItemListView.Width - 6
			Me.Cursor = Cursors.Default
		End Sub

	End Class
End Namespace