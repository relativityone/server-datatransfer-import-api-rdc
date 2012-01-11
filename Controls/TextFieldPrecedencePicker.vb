Imports System.Collections.Generic
Imports System.Text
Imports System.Linq

Namespace kCura.EDDS.WinForm
	Public Class TextFieldPrecedencePicker
		Private WithEvents _textFieldPrecedenceForm As kCura.EDDS.WinForm.TextPrecedenceForm
		Private _allAvailableLongTextFields As List(Of ViewFieldInfo)
		Private _toolTip As New ToolTip()
		Private _selectedFields As New List(Of ViewFieldInfo)

		Public Property SelectedFields() As List(Of ViewFieldInfo)
			Get
				Return _selectedFields
			End Get
			Set(ByVal value As List(Of ViewFieldInfo))
				_selectedFields = value
			End Set
		End Property

		Public WriteOnly Property AllAvailableLongTextFields() As List(Of ViewFieldInfo)
			Set(ByVal value As List(Of ViewFieldInfo))
				If Me.SelectedFields.Count > 0 Then
					Dim tempSelectedFields As New List(Of ViewFieldInfo)()
					tempSelectedFields.AddRange(Me.SelectedFields)
					For Each selectedItem As ViewFieldInfo In tempSelectedFields
						Dim selectedItemViewlFieldInfo As ViewFieldInfo = DirectCast(selectedItem, ViewFieldInfo)
						Dim selectedItemExists As Boolean = False
						For Each item As ViewFieldInfo In value
							If selectedItemViewlFieldInfo.AvfId = item.AvfId Then
								selectedItemExists = True
								Exit For
							End If
						Next
						If Not selectedItemExists Then
							Me.SelectedFields.Remove(selectedItem)
						End If
					Next
				End If
				_allAvailableLongTextFields = value
				ManageLabel()
			End Set
		End Property

	Private Sub _pickTextFieldPrecedenceButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _pickTextFieldPrecedenceButton.Click
			_textFieldPrecedenceForm = New kCura.EDDS.WinForm.TextPrecedenceForm(_allAvailableLongTextFields, Me.SelectedFields)
			_textFieldPrecedenceForm.ShowDialog()
		End Sub

		Private Sub _textFieldPrecedenceForm_OkClicked() Handles _textFieldPrecedenceForm.OkClicked
			ManageColumns()
		End Sub

		Private Sub ManageLabel()
			Dim toolTipBuilder As New StringBuilder()
			For Each selectedField As ViewFieldInfo In Me.SelectedFields
				toolTipBuilder.AppendLine(selectedField.DisplayName)
			Next
			_toolTip.AutoPopDelay = 2500
			_toolTip.InitialDelay = 100
			_toolTip.ReshowDelay = 125
			_toolTip.ShowAlways = True
			_toolTip.SetToolTip(_selectedTextFieldsTextBox, toolTipBuilder.ToString().Trim)
			If SelectedFields.Count > 0 Then
				_selectedTextFieldsTextBox.Text = Me.SelectedFields.First.DisplayName & If(SelectedFields.Count > 1, "...", "")
			Else
				_selectedTextFieldsTextBox.Text = ""
			End If
		End Sub

		Private Sub ManageColumns()
			Me.SelectedFields.Clear()
			For Each item As Object In _textFieldPrecedenceForm.SelectedTextFields
				Me.SelectedFields.Add(DirectCast(item, ViewFieldInfo))
			Next
			ManageLabel()
		End Sub

		Public Sub SelectDefautlTextField(ByVal firstSelectedField As ViewFieldInfo)
			If _allAvailableLongTextFields.Count > 0 Then
				Me.SelectedFields.Clear()
				If firstSelectedField IsNot Nothing Then
					Me.SelectedFields.Add(firstSelectedField)
				End If
				ManageLabel()
			End If
		End Sub
	End Class
End Namespace
