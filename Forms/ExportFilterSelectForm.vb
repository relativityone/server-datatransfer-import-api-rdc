Namespace kCura.EDDS.WinForm
	Public Class ExportFilterSelectForm
		Inherits kCura.EDDS.WinForm.SelectFormBase

#Region " Declarations & Properties "
		Private _selectedItemArtifactIDs As Generic.List(Of Int32)

		Public ReadOnly Property SelectedItemArtifactIDs() As Generic.List(Of Int32)
			Get
				Return _selectedItemArtifactIDs
			End Get
		End Property

		Private Property PossibleItemNameToSelect() As String
		Private _itemListTable As System.Data.DataTable
#End Region


		Protected Sub ItemSelectForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
			Me.LoadItems(String.Empty)
			Me.Focus()
			SelectItemByName(PossibleItemNameToSelect)
		End Sub

		Protected Overrides Sub ItemListView_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ItemListView.SelectedIndexChanged
			Dim list As New Generic.List(Of Int32)
			If ItemListView.SelectedItems.Count <> 0 Then
				For Each si As ListViewItem In ItemListView.SelectedItems
					list.Add(DirectCast(si.Tag, Int32))
				Next
				_selectedItemArtifactIDs = list
				ConfirmButton.Enabled = True
			Else
				_selectedItemArtifactIDs = list
			End If
		End Sub

		Private Sub CancelBtn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CancelBtn.Click
			_selectedItemArtifactIDs = Nothing
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
			For Each row As DataRow In _itemListTable.Rows
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
			For i As Int32 = 0 To ItemListView.Items.Count - 1
				If ItemListView.Items(i).Text.Equals(itemName, StringComparison.InvariantCulture) Then
					ItemListView.Items(i).Focused = True
					ItemListView.Items(i).Selected = True
					ItemListView.EnsureVisible(i)
					ItemListView.HideSelection = False
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

		Private Sub ExportFilterSelectForm_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
			If ItemListView.SelectedItems.Count = 0 Then
				SearchQuery.Focus()
			Else
				ItemListView.Focus()
			End If
		End Sub
	End Class
End Namespace