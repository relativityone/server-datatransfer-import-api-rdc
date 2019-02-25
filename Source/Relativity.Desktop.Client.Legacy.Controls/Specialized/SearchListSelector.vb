Imports System.Collections.Generic
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Windows.Forms


Namespace kCura.Windows.Forms.Specialized
	Public Class SearchListSelector
		Inherits Form
		Private ReadOnly _dataSource As DataTable
		Private _timer As Timer
		Private _isTextBoxPlaceholderUsed As Boolean = True
		Private _isUserTyping As Boolean = False

		Private Const _DELAYED_TEXT_CHANGED_TIMEOUT_IN_MILLISECONDS As Integer = 600
		Private Const _DISPLAY_MEMBER_FIELD_NAME As String = "Name"
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
			selectionListBox.DisplayMember = _DISPLAY_MEMBER_FIELD_NAME
			selectionListBox.ValueMember = "ArtifactID"
		End Sub

		Private Sub selectionSearchInput_TextChanged(sender As Object, e As EventArgs) _
			Handles selectionSearchInput.TextChanged
			If _isTextBoxPlaceholderUsed AndAlso Not _isUserTyping Then Return

			If IsNothing(_timer) Then
				_timer = New Timer()
				AddHandler _timer.Tick, AddressOf Timer_Tick
				_timer.Interval = _DELAYED_TEXT_CHANGED_TIMEOUT_IN_MILLISECONDS
			Else
				_timer.Stop()

			End If

			_timer.Start()
		End Sub

		Private Sub Timer_Tick(sender As Object, e As EventArgs)
			If Not IsNothing(_timer) Then
				_timer.Stop()
			End If
			Cursor = Cursors.WaitCursor
			selectionListBox.DataSource = FilterRowsFromDataTable(_dataSource, selectionSearchInput.Text)
			UpdateAcceptButtonStatus()
			Cursor = Cursors.Default
		End Sub

		Private Sub UpdateAcceptButtonStatus()
			If selectionListBox.Items.Count = 0 Then
				_selectButton.Enabled = False
			Else
				_selectButton.Enabled = True
			End If
		End Sub

		Protected Function FilterRowsFromDataTable(sourceDt As DataTable, substring As String) As DataTable
			If Not String.IsNullOrEmpty(substring) Then
				Dim currentDt As DataTable = sourceDt.Clone()
				Dim substringLowercase As String = substring.ToLower()
				Dim items As IEnumerable(Of DataRow) =
						sourceDt.Select.Where(
							Function(x As DataRow) x.Item(_DISPLAY_MEMBER_FIELD_NAME).ToString().ToLower().Contains(substringLowercase))
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

		Private Sub selectionSearchInput_Enter(sender As Object, e As EventArgs) Handles selectionSearchInput.Enter
			_isUserTyping = True
			If _isTextBoxPlaceholderUsed Then
				selectionSearchInput.Text = ""
				selectionSearchInput.ForeColor = Color.Black
				selectionSearchInput.Font = New Font(selectionSearchInput.Font, FontStyle.Regular)
			End If
		End Sub

		Private Sub selectionSearchInput_Leave(sender As Object, e As EventArgs) Handles selectionSearchInput.Leave
			_isUserTyping = False
			If selectionSearchInput.Text = "" Then
				_isTextBoxPlaceholderUsed = True
				selectionSearchInput.Text = "Filter"
				selectionSearchInput.ForeColor = Color.Gray
				selectionSearchInput.Font = New Font(selectionSearchInput.Font, FontStyle.Italic)
			Else
				_isTextBoxPlaceholderUsed = False
			End If
		End Sub


		Public ReadOnly Property SelectedValue() As Object
			Get
				Return selectionListBox.SelectedValue
			End Get
		End Property
	End Class
End Namespace