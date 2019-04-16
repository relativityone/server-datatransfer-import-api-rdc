Namespace kCura.EDDS.WinForm
	Public Class ProductionSelectForm
		Inherits kCura.EDDS.WinForm.SelectFormBase

#Region " Declarations & Properties "
		Private _selectedProductionArtifactID As Generic.List(Of Int32)

		Public ReadOnly Property SelectedProductionArtifactID() As Generic.List(Of Int32)
			Get
				Return _selectedProductionArtifactID
			End Get
		End Property
		Protected Property PossibleProductionNameToSelect() As String

#End Region
		Private _productions As System.Data.DataTable

		Private Property CaseInfo As Relativity.CaseInfo

		Private Sub ItemSelectForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
			_productions = Application.GetListOfProductionsForCase(Me.CaseInfo)
			Me.Text = "Relativity Desktop Client | Select Production"
			Me.LoadItems(String.Empty)
			Me.Focus()
			SelecteAnItem()
			SearchQuery.Focus()
		End Sub

		Protected Overrides Sub ItemListView_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ItemListView.SelectedIndexChanged
			Dim list As New Generic.List(Of Int32)
			If ItemListView.SelectedItems.Count <> 0 Then
				For Each si As ListViewItem In ItemListView.SelectedItems
					list.Add(DirectCast(si.Tag, Int32))
				Next
				_selectedProductionArtifactID = list
				ConfirmButton.Enabled = True
			Else
				_selectedProductionArtifactID = list
			End If
		End Sub

		Private Sub CancelBtn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CancelBtn.Click
			_selectedProductionArtifactID = Nothing
			Me.DialogResult = DialogResult.Cancel
			Me.Close()
		End Sub

		Protected Overrides Sub LoadItems(ByVal searchText As String)
			Me.Cursor = Cursors.WaitCursor
			ItemListView.Items.Clear()
			If _productions Is Nothing Then
				Me.Cursor = Cursors.Default
				Me.Close()
				Exit Sub
			End If
			Dim dt As DataTable = _productions
			Dim row As DataRow
			For Each row In dt.Rows
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

		Private Sub SelecteAnItem()
			For Each item As ListViewItem In ItemListView.Items
				If item.Text.Equals(PossibleProductionNameToSelect, StringComparison.InvariantCulture) Then
					item.Selected = True
					Exit For
				End If
			Next
		End Sub

		Public Sub New(ByVal caseInfo As Relativity.CaseInfo, ByVal possibleProductionNameToSelect As String)
			MyBase.New()
			Me.CaseInfo = caseInfo
			Me.MultiSelect = False
			Me.PossibleProductionNameToSelect = possibleProductionNameToSelect
		End Sub


	End Class
End Namespace