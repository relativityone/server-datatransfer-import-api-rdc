Namespace kCura.EDDS.WinForm
	Public Class ExportFilterSelectForm
		Inherits kCura.EDDS.WinForm.SelectFormBase

#Region " Declarations & Properties "
		Private _selectedItemArtifactID As Generic.List(Of Int32)

		Public ReadOnly Property SelectedItemArtifactID() As Generic.List(Of Int32)
			Get
				Return _selectedItemArtifactID
			End Get
		End Property

		Private Property PossibleItemNameToSelect() As String
		Private _itemListTable As System.Data.DataTable
#End Region


		Private Sub ItemSelectForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
			Me.LoadItems(String.Empty)
			Me.Focus()
			SelectItemByName(PossibleItemNameToSelect)
			SearchQuery.Focus()
		End Sub

		Protected Overrides Sub ItemListView_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ItemListView.SelectedIndexChanged
			Dim list As New Generic.List(Of Int32)
			If ItemListView.SelectedItems.Count <> 0 Then
				For Each si As ListViewItem In ItemListView.SelectedItems
					list.Add(DirectCast(si.Tag, Int32))
				Next
				_selectedItemArtifactID = list
				ConfirmButton.Enabled = True
			Else
				_selectedItemArtifactID = list
			End If
		End Sub

		Private Sub CancelBtn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CancelBtn.Click
			_selectedItemArtifactID = Nothing
			Me.DialogResult = DialogResult.Cancel
			Me.Close()
		End Sub

		Protected Overrides Sub LoadItems(ByVal searchText As String)
			Me.Cursor = Cursors.WaitCursor
			ItemListView.Items.Clear()
			If _itemListTable Is Nothing Then
				Me.Cursor = Cursors.Default
				Me.Close()
				Exit Sub
			End If
			For Each row As DataRow In _itemListTable
				If String.IsNullOrEmpty(searchText.Trim) OrElse CType(row.Item("Name"), String).ToLower.IndexOf(searchText.Trim.ToLower) <> -1 Then
					Dim listItem As New System.Windows.Forms.ListViewItem
					listItem.Text = CType(row.Item("Name"), String)
					listItem.Tag = CType(row.Item("ArtifactID"), Int32)
					ItemListView.Items.Add(listItem)
				End If
			Next
			NameColumnHeader.Width = ItemListView.Width - 6
			Me.Cursor = Cursors.Default

		End Sub

		Private Sub SelectItemByName(ByVal itemName As String)
			For Each item As ListViewItem In ItemListView.Items
				If item.Text.Equals(itemName, StringComparison.InvariantCulture) Then
					item.Selected = True
					Exit For
				End If
			Next
		End Sub

		Public Sub New(ByVal savedItemNameToSelect As String, ByVal objectTypeName As String, ByVal listViewDataSource As DataTable)
			MyBase.New()
			Me.MultiSelect = False
			Me.PossibleItemNameToSelect = savedItemNameToSelect
			_itemListTable = listViewDataSource
			Me.Text = String.Format("Relativity Desktop Client | Select {0}", objectTypeName)
		End Sub

	End Class
End Namespace