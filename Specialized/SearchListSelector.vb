Imports System.Collections.Generic
Imports System.Data
Imports System.Linq
Imports System.Windows.Forms


Namespace Specialized
	Public Class SearchListSelector
		Inherits Form
		ReadOnly _dataSource As DataTable
		Private _timer As Timer

		Private Const DELAYED_TEXT_CHANGED_TIMEOUT_IN_MILLISECONDS As Integer = 600
		Private Const DISPLAY_MEMBER_FIELD_NAME As String = "Name"
		Protected Sub New()
			InitializeComponent()
		End Sub

		Public Sub New(dataTable As DataTable, formName As String)
			' This call is required by the designer.
			Me.New()
			' Add any initialization after the InitializeComponent() call.
			Text = formName
			_dataSource = dataTable
			SetupListBox()
		End Sub

		Private Sub SetupListBox()
			selectionListBox.DataSource = _dataSource
			selectionListBox.DisplayMember = DISPLAY_MEMBER_FIELD_NAME
			selectionListBox.ValueMember = "ArtifactID"
		End Sub

		Private Sub selectionSearchInput_TextChanged(sender As Object, e As EventArgs) _
			Handles selectionSearchInput.TextChanged
			If Not IsNothing(_timer) Then
				_timer.Stop()
			Else
				_timer = New Timer()
				AddHandler _timer.Tick, AddressOf _timer_Tick
				_timer.Interval = DELAYED_TEXT_CHANGED_TIMEOUT_IN_MILLISECONDS
			End If

			_timer.Start()
		End Sub

		Private Sub _timer_Tick(sender As Object, e As EventArgs)
			If Not IsNothing(_timer) Then
				_timer.Stop()
			End If
			Cursor = Cursors.WaitCursor
			selectionListBox.DataSource = FilterRowsFromDataTable(_dataSource, selectionSearchInput.Text)
			Cursor = Cursors.Default
		End Sub

		Protected Function FilterRowsFromDataTable(sourceDt As DataTable, substring As String) As DataTable
			If Not String.IsNullOrEmpty(substring) Then
				Dim currentDt As DataTable = sourceDt.Clone()
				substring = substring.ToLower()
				Dim items As IEnumerable(Of DataRow) =
						sourceDt.Select.Where(
							Function(x As DataRow) x.Item(DISPLAY_MEMBER_FIELD_NAME).ToString().ToLower().Contains(substring))
				For Each dataRow As DataRow In items
					currentDt.ImportRow(dataRow)
				Next
				Return currentDt
			End If
			Return sourceDt
		End Function

		Private Sub selectionListBox_DoubleClick(sender As Object, e As EventArgs) _
			Handles selectionListBox.MouseDoubleClick
			If selectionListBox.SelectedIndex > -1 Then
				DialogResult = DialogResult.OK
				Close()
			End If
		End Sub

		Public ReadOnly Property SelectedValue() As Object
			Get
				Return selectionListBox.SelectedValue
			End Get
		End Property
	End Class
End Namespace