Imports System.Linq
Imports System.Collections.Generic

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
		Private _debounce As New System.Windows.Forms.Timer With {.Interval = 200}

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

		Private _searchText As String = String.Empty

		Private Function ConvertToListItem(caseTableRow As System.Data.DataRow) As System.Windows.Forms.ListViewItem
			Dim listItem As New System.Windows.Forms.ListViewItem With
			{
				.Text = CType(caseTableRow.Item("Name"), String),
				.Tag = New Relativity.CaseInfo(caseTableRow)
			}
			Return listItem
		End Function

		Private Sub DoLoadItems(myObject As Object, ByVal myEventArgs As EventArgs)
			Me.Cursor = Cursors.WaitCursor
			Try
				_debounce.Stop()
				Dim searchText As String = _searchText
				ItemListView.Items.Clear()
				Dim rows As IEnumerable(Of DataRow) = _cases.Tables(0).Select()
				If Not String.IsNullOrWhiteSpace(searchText) Then
					rows = rows.Where(Function(r) CType(r.Item("Name"), String).ToLower.IndexOf(searchText.Trim.ToLower) <> -1)
				End If
				Dim items As IEnumerable(Of System.Windows.Forms.ListViewItem) = rows.Select(AddressOf ConvertToListItem)
				Dim itemArray As System.Windows.Forms.ListViewItem() = items.ToArray()
				ItemListView.Items.AddRange(itemArray)
				NameColumnHeader.Width = ItemListView.Width - 6
			Catch
				Throw
			Finally
				Me.Cursor = Cursors.Default
			End Try
		End Sub

		Protected Overrides Sub LoadItems(ByVal searchText As String)
			_debounce.Stop()
			If _cases Is Nothing Then
				Me.Close()
				Return
			End If
			RemoveHandler _debounce.Tick, AddressOf DoLoadItems
			_searchText = searchText
			AddHandler _debounce.Tick, AddressOf DoLoadItems
			_debounce.Start()
		End Sub

	End Class
End Namespace
