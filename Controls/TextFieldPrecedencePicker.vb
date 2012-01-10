Imports System.Collections.Generic
Imports System.Text

Namespace kCura.EDDS.WinForm
	Public Class TextFieldPrecedencePicker
		Private WithEvents _textFieldPrecedenceForm As kCura.EDDS.WinForm.TextPrecedenceForm
		Private _allAvailableLongTextFields As List(Of ViewFieldInfo)

		Public WriteOnly Property AllAvailableLongTextFields() As List(Of ViewFieldInfo)
			Set(ByVal value As List(Of ViewFieldInfo))
				Dim toolTipStringBuilder As New StringBuilder()
				If SelectedTextFieldsListBox.Items.Count > 0 Then
					Dim listOfPotentialTextFields As New List(Of ViewFieldInfo)()
					For Each pObject As ViewFieldInfo In PotentialTextFieldsList
						Dim pObjectVfi As ViewFieldInfo = DirectCast(pObject, ViewFieldInfo)
						listOfPotentialTextFields.Add(pObjectVfi)
					Next

					For Each selectedItem As ViewFieldInfo In listOfPotentialTextFields
						Dim selectedItemViewlFieldInfo As ViewFieldInfo = DirectCast(selectedItem, ViewFieldInfo)
						Dim selectedItemExists As Boolean = False
						For Each item As ViewFieldInfo In value
							If selectedItemViewlFieldInfo.AvfId = item.AvfId Then
								selectedItemExists = True
								Exit For
							End If
						Next
						If Not selectedItemExists Then
							SelectedTextFieldsListBox.Items.Remove(selectedItem)
						Else
							toolTipStringBuilder.AppendLine(selectedItem.DisplayName)
						End If
					Next
				End If
				_allAvailableLongTextFields = value
				SetToolTip(toolTipStringBuilder.ToString().Trim())
			End Set
		End Property

		Private _toolTip As New ToolTip()

		Public ReadOnly Property PotentialTextFieldsList() As List(Of ViewFieldInfo)
			Get
				Dim retVal As New List(Of ViewFieldInfo)()
				For i As Int32 = 0 To SelectedTextFieldsListBox.Items.Count - 1
					retVal.Add(DirectCast(SelectedTextFieldsListBox.Items(i), ViewFieldInfo))
				Next
				Return retVal
			End Get
		End Property

		Private Sub _pickTextFieldPrecedenceButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _pickTextFieldPrecedenceButton.Click
			_textFieldPrecedenceForm = New kCura.EDDS.WinForm.TextPrecedenceForm(_allAvailableLongTextFields, SelectedTextFieldsListBox)
			_textFieldPrecedenceForm.ShowDialog()
		End Sub

		Private Sub _textFieldPrecedenceForm_OkClicked() Handles _textFieldPrecedenceForm.OkClicked
			ManageColumns()
		End Sub

		Private Sub TextFieldPrecedencePicker_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
			'ManageColumns()
		End Sub

		Private Sub SetToolTip(ByVal text As String)
			_toolTip.AutoPopDelay = 5000
			_toolTip.InitialDelay = 1000
			_toolTip.ReshowDelay = 500
			_toolTip.ShowAlways = True
			_toolTip.SetToolTip(SelectedTextFieldsListBox, text)
		End Sub

		Private Sub ManageColumns()
			SelectedTextFieldsListBox.Items.Clear()
			Dim toolTipStringBuilder As New StringBuilder()
			For Each item As Object In _textFieldPrecedenceForm.SelectedTextFields
				SelectedTextFieldsListBox.Items.Add(item)
				toolTipStringBuilder.AppendLine(CType(item, ViewFieldInfo).DisplayName)
			Next
			If SelectedTextFieldsListBox.Items.Count > 0 Then
				SelectedTextFieldsListBox.SelectedIndex = 0
			End If

			SetToolTip(toolTipStringBuilder.ToString().Trim())
		End Sub
	End Class
End Namespace
