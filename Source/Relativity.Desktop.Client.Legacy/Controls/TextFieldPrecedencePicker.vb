Imports System.Text

Namespace Relativity.Desktop.Client
	Public Class TextFieldPrecedencePicker
		Private WithEvents _textFieldPrecedenceForm As TextPrecedenceForm
		Private _allAvailableLongTextFields As List(Of kCura.WinEDDS.ViewFieldInfo)
		Private _toolTip As New ToolTip()
		Private _selectedFields As New List(Of kCura.WinEDDS.ViewFieldInfo)

		Public ReadOnly Property LabelText As String
			Get
				Return _selectedTextFieldsTextBox.Text
			End Get
		End Property

		Public ReadOnly Property ToolTipText As String
			Get
				Return _toolTip.GetToolTip(_selectedTextFieldsTextBox)
			End Get
		End Property

		Public Property SelectedFields() As List(Of kCura.WinEDDS.ViewFieldInfo)
			Get
				Return _selectedFields
			End Get
			Set(ByVal value As List(Of kCura.WinEDDS.ViewFieldInfo))
				_selectedFields = value
			End Set
		End Property

		Public WriteOnly Property AllAvailableLongTextFields() As List(Of kCura.WinEDDS.ViewFieldInfo)
			Set(ByVal value As List(Of kCura.WinEDDS.ViewFieldInfo))
				If Me.SelectedFields.Count > 0 Then
					Dim tempSelectedFields As New List(Of kCura.WinEDDS.ViewFieldInfo)()
					tempSelectedFields.AddRange(Me.SelectedFields)
					For Each selectedItem As kCura.WinEDDS.ViewFieldInfo In tempSelectedFields
						Dim selectedItemViewlFieldInfo As kCura.WinEDDS.ViewFieldInfo = DirectCast(selectedItem, kCura.WinEDDS.ViewFieldInfo)
						Dim selectedItemExists As Boolean = False
						For Each item As kCura.WinEDDS.ViewFieldInfo In value
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

		Public Sub LoadNewSelectedFields(newFields As IEnumerable(Of kCura.WinEDDS.ViewFieldInfo))
			Dim updatedListOfSelectedFields As New List(Of kCura.WinEDDS.ViewFieldInfo)()
			If newFields IsNot Nothing Then
				For Each incomingSelectedField As kCura.WinEDDS.ViewFieldInfo In newFields
					Dim incomingSelectedFieldExistsInCurrentSelectedFieldsList As Boolean = False
					For Each currentlySelectedField In _allAvailableLongTextFields
						If currentlySelectedField.DisplayName.Equals(incomingSelectedField.DisplayName, StringComparison.InvariantCulture) Then
							incomingSelectedFieldExistsInCurrentSelectedFieldsList = True
							incomingSelectedField = currentlySelectedField
							Exit For
						End If
					Next
					If incomingSelectedFieldExistsInCurrentSelectedFieldsList Then
						updatedListOfSelectedFields.Add(incomingSelectedField)
					End If
				Next
			End If
			Me.SelectedFields.Clear()
			Me.SelectedFields.AddRange(updatedListOfSelectedFields)
			Me.ManageLabel()
		End Sub

		Private Sub _pickTextFieldPrecedenceButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _pickTextFieldPrecedenceButton.Click
			_textFieldPrecedenceForm = New TextPrecedenceForm(_allAvailableLongTextFields, Me.SelectedFields)
			_textFieldPrecedenceForm.ShowDialog()
		End Sub

		Private Sub _textFieldPrecedenceForm_OkClicked() Handles _textFieldPrecedenceForm.OkClicked
			ManageColumns()
		End Sub

		Private Sub ManageLabel()
			Dim toolTipBuilder As New StringBuilder()
			For Each selectedField As kCura.WinEDDS.ViewFieldInfo In Me.SelectedFields
				toolTipBuilder.AppendLine(selectedField.DisplayName)
			Next
			_toolTip.AutoPopDelay = 2500
			_toolTip.InitialDelay = 100
			_toolTip.ReshowDelay = 125
			_toolTip.ShowAlways = True
			_toolTip.SetToolTip(_selectedTextFieldsTextBox, toolTipBuilder.ToString().Trim)
			If SelectedFields.Count > 0 Then
				_selectedTextFieldsTextBox.Text = Me.SelectedFields.First.DisplayName & If(SelectedFields.Count > 1, " (+)", "")
			Else
				_selectedTextFieldsTextBox.Text = ""
			End If
		End Sub

		Private Sub ManageColumns()
			Me.SelectedFields.Clear()
			For Each item As Object In _textFieldPrecedenceForm.SelectedTextFields
				Me.SelectedFields.Add(DirectCast(item, kCura.WinEDDS.ViewFieldInfo))
			Next
			ManageLabel()
		End Sub

		Public Sub SelectDefaultTextField(ByVal firstSelectedField As kCura.WinEDDS.ViewFieldInfo)
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