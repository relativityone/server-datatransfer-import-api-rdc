﻿Imports System.Collections.Generic
Imports System.Linq
Imports Relativity

Namespace kCura.EDDS.WinForm.Forms
	Public Class TextAndNativeFileNameForm

		Private Const CustomTextOption As String = "Custom Text..."
		Private Const FieldLimit = 3

		Private ReadOnly AllowedFieldTypes As FieldTypeHelper.FieldType() = New FieldTypeHelper.FieldType() {
			FieldTypeHelper.FieldType.Varchar,
			FieldTypeHelper.FieldType.Date,
			FieldTypeHelper.FieldType.Integer,
			FieldTypeHelper.FieldType.Decimal,
			FieldTypeHelper.FieldType.Boolean,
			FieldTypeHelper.FieldType.Code
		}

		Private ReadOnly Separators As SeparatorSelection() = New SeparatorSelection() {
			New SeparatorSelection("_ (underscore)", "_"),
			New SeparatorSelection("- (hyphen)", "-"),
			New SeparatorSelection(". (period)", "."),
			New SeparatorSelection("  (space)", " "),
			New SeparatorSelection(" (none)", "")
		}

		Private _firstField As FieldSelection
		Private _availableFields As List(Of FieldSelection)
		Private _fieldControls As List(Of SingleFieldControls)

		Private ReadOnly Property NumberOfFields As Integer
			Get
				Return _fieldControls.Count
			End Get
		End Property

		Public Sub Initialize(fields As ViewFieldInfo(), selection As IList(Of CustomFileNameSelectionPart))
			ClientSize = New Size(252 * FieldLimit - 75, 102)
			InitializeAvailableFields(fields)
			InitializeFieldControls()
			PopulateControls(selection)
		End Sub

		Private Sub InitializeAvailableFields(fields As IReadOnlyCollection(Of ViewFieldInfo))
			_availableFields = New List(Of FieldSelection) From {New FieldSelection(CustomTextOption, -1)}
			Dim databaseFields = fields.
				Where(Function(f) AllowedFieldTypes.Contains(f.FieldType)).
				Select(Function(f) New FieldSelection(f.DisplayName, f.FieldArtifactId))
			_availableFields.AddRange(databaseFields)
			_firstField = fields.
				Where(Function(f) f.Category = FieldCategory.Identifier).
				Select(Function(f) New FieldSelection(f.DisplayName, f.FieldArtifactId)).
				First()
		End Sub

		Private Sub InitializeFieldControls()
			_fieldControls = New List(Of SingleFieldControls) From {Nothing}
		End Sub

		Private Sub PopulateControls(selection As IList(Of CustomFileNameSelectionPart))
			_removeFieldButton.Visible = False
			_firstFieldTextBox.Text = _firstField.DisplayName
			If selection Is Nothing Then
				Return
			End If
			Dim index = 1
			While index < selection.Count And index < FieldLimit
				AddField()
				Dim selectionPart = selection(index)
				Dim fieldControls = _fieldControls(index)
				fieldControls.SeparatorComboBox.SelectedValue = selectionPart.Separator
				If selectionPart.HasCustomText() Then
					fieldControls.CustomTextBox.Text = selectionPart.CustomText
				Else
					fieldControls.FieldComboBox.SelectedValue = selectionPart.FieldID
				End If
				index += 1
			End While
		End Sub

		Public Function GetSelection() As IList(Of CustomFileNameSelectionPart)
			Dim selection = New List(Of CustomFileNameSelectionPart)
			selection.Add(New CustomFileNameSelectionPart(_firstField.ID))
			For i = 1 To (NumberOfFields - 1)
				Dim fieldControls = _fieldControls(i)
				Dim selectedField = TryCast(fieldControls.FieldComboBox.SelectedItem, FieldSelection)
				Dim selectedSeparator = TryCast(fieldControls.SeparatorComboBox.SelectedItem, SeparatorSelection)
				Dim selectionPart As CustomFileNameSelectionPart
				If selectedField.DisplayName = CustomTextOption Then
					selectionPart = New CustomFileNameSelectionPart(selectedSeparator.Value, fieldControls.CustomTextBox.Text)
				Else
					selectionPart = New CustomFileNameSelectionPart(selectedSeparator.Value, selectedField.ID)
				End If
				selection.Add(selectionPart)
			Next
			Return selection
		End Function

		Private Sub AddField()
			Dim fieldComboBox As ComboBox = AddFieldComboBox(NumberOfFields)
			Dim separatorComboBox As ComboBox = AddSeparatorComboBox()
			Dim customTextBox As TextBox = AddCustomTextBox()
			Dim singleFieldControls = New SingleFieldControls(fieldComboBox, separatorComboBox, customTextBox)
			_fieldControls.Add(singleFieldControls)
			SetupAddAndRemoveButtons()
		End Sub

		Private Function AddFieldComboBox(fieldNumber As Integer) As ComboBox
			Dim fieldComboBox = New ComboBox()
			fieldComboBox.DropDownStyle = ComboBoxStyle.DropDown
			fieldComboBox.FormattingEnabled = True
			fieldComboBox.Location = New Point(252 * NumberOfFields + 14, 13)
			fieldComboBox.Size = New Size(120, 21)
			fieldComboBox.TabIndex = 2 + 3 * NumberOfFields
			fieldComboBox.Tag = fieldNumber
			Controls.Add(fieldComboBox)
			fieldComboBox.DataSource = _availableFields.ToList()
			fieldComboBox.DisplayMember = "DisplayName"
			fieldComboBox.ValueMember = "ID"
			fieldComboBox.AutoCompleteSource = AutoCompleteSource.ListItems
			fieldComboBox.AutoCompleteMode = AutoCompleteMode.None
			AddHandler fieldComboBox.KeyDown, AddressOf CatchEnterKey
			AddHandler fieldComboBox.Leave, AddressOf SelectBestFieldIfNoneChosen
			AddHandler fieldComboBox.SelectedIndexChanged, AddressOf FieldComboBoxSelectionChanged
			Return fieldComboBox
		End Function

		Private Function AddSeparatorComboBox() As ComboBox
			Dim separatorComboBox = New ComboBox()
			separatorComboBox.DropDownStyle = ComboBoxStyle.DropDown
			separatorComboBox.FormattingEnabled = True
			separatorComboBox.Location = New Point(252 * NumberOfFields - 112, 13)
			separatorComboBox.Size = New Size(120, 21)
			separatorComboBox.TabIndex = 1 + 3 * NumberOfFields
			Controls.Add(separatorComboBox)
			separatorComboBox.DataSource = Separators.ToList()
			separatorComboBox.DisplayMember = "DisplayName"
			separatorComboBox.ValueMember = "Value"
			separatorComboBox.AutoCompleteSource = AutoCompleteSource.ListItems
			separatorComboBox.AutoCompleteMode = AutoCompleteMode.None
			AddHandler separatorComboBox.KeyDown, AddressOf CatchEnterKey
			AddHandler separatorComboBox.Leave, AddressOf SelectBestFieldIfNoneChosen
			Return separatorComboBox
		End Function

		Private Function AddCustomTextBox() As TextBox
			Dim customTextBox = New TextBox()
			customTextBox.Location = New Point(252 * NumberOfFields + 14, 41)
			customTextBox.Size = New Size(120, 20)
			customTextBox.TabIndex = 3 + 3 * NumberOfFields
			Controls.Add(customTextBox)
			Return customTextBox
		End Function

		Private Sub RemoveField()
			Dim singleFieldControls = _fieldControls(_fieldControls.Count - 1)
			singleFieldControls.FieldComboBox.DataSource = Nothing
			RemoveHandler singleFieldControls.FieldComboBox.SelectedIndexChanged, AddressOf FieldComboBoxSelectionChanged
			singleFieldControls.SeparatorComboBox.DataSource = Nothing
			Controls.Remove(singleFieldControls.FieldComboBox)
			Controls.Remove(singleFieldControls.SeparatorComboBox)
			Controls.Remove(singleFieldControls.CustomTextBox)
			_fieldControls.Remove(singleFieldControls)
			SetupAddAndRemoveButtons()
		End Sub

		Private Sub SetupAddAndRemoveButtons()
			If NumberOfFields = 1 Then
				_addFieldButton.Visible = True
				_addFieldButton.Location = New Point(252 * NumberOfFields - 112, 12)
				_removeFieldButton.Visible = False
			ElseIf NumberOfFields = FieldLimit Then
				_addFieldButton.Visible = False
				_removeFieldButton.Visible = True
				_removeFieldButton.Location = New Point(252 * NumberOfFields - 112, 12)
			Else
				_addFieldButton.Visible = True
				_addFieldButton.Location = New Point(252 * NumberOfFields - 112, 12)
				_removeFieldButton.Visible = True
				_removeFieldButton.Location = New Point(252 * NumberOfFields - 81, 12)
			End If
		End Sub

		Private Sub ToggleFieldCustomTextBox(comboBox As ComboBox, textBox As TextBox)
			Dim textBoxVisible = False
			Dim selectedItem = TryCast(comboBox.SelectedItem, FieldSelection)
			If selectedItem IsNot Nothing AndAlso selectedItem.DisplayName = CustomTextOption Then
				textBoxVisible = True
			End If
			textBox.Visible = textBoxVisible
		End Sub

		Private Sub CatchEnterKey(sender As Object, e As KeyEventArgs)
			If e.KeyCode = Keys.Enter Then
				e.Handled = True
				SendKeys.Send("{TAB}")
			End If
		End Sub

		Private Sub SelectBestFieldIfNoneChosen(sender As Object, e As EventArgs)
			Dim comboBox = TryCast(sender, ComboBox)
			If comboBox Is Nothing Then
				Return
			End If
			comboBox.DroppedDown = False
			System.Windows.Forms.Application.DoEvents()
			comboBox.SelectedIndex = GetIndexOfBestMatchFromList(comboBox.Items, comboBox.Text)

		End Sub

		Protected Function GetIndexOfBestMatchFromList(items As ComboBox.ObjectCollection, text As String) As Integer
			Dim bestItems = items.Cast(Of ISelection).Where(Function(x) x.DisplayName.ToLower.StartsWith(text.ToLower))
			If bestItems.Count = 0 Then
				Return 0
			Else
				Return items.IndexOf(bestItems.First)
			End If
		End Function

		Public Event ApplyClicked()

		Private Sub FieldComboBoxSelectionChanged(sender As Object, e As EventArgs)
			Dim comboBox = TryCast(sender, ComboBox)
			If comboBox Is Nothing Then
				Return
			End If
			Dim fieldNumber = DirectCast(comboBox.Tag, Integer)
			Dim textBox As TextBox = _fieldControls(fieldNumber).CustomTextBox
			ToggleFieldCustomTextBox(comboBox, textBox)
		End Sub

		Private Sub _addFieldButton_Click(sender As Object, e As EventArgs) Handles _addFieldButton.Click
			AddField()
		End Sub

		Private Sub _removeFieldButton_Click(sender As Object, e As EventArgs) Handles _removeFieldButton.Click
			RemoveField()
		End Sub

		Private Sub _applyButton_Click(sender As Object, e As EventArgs) Handles _applyButton.Click
			RaiseEvent ApplyClicked()
			Close()
		End Sub

		Private Sub _cancelButton_Click(sender As Object, e As EventArgs) Handles _cancelButton.Click
			Close()
		End Sub

		Protected Interface ISelection
			Property DisplayName() As String
		End Interface
		Protected Class FieldSelection
			Implements ISelection
			Public Sub New(displayName As String, id As Integer)
				Me.DisplayName = displayName
				Me.ID = id
			End Sub

			Public Property DisplayName As String Implements ISelection.DisplayName
			Public Property ID As Integer

		End Class

		Protected Class SeparatorSelection
			Implements ISelection
			Public Sub New(displayName As String, value As String)
				Me.DisplayName = displayName
				Me.Value = value
			End Sub

			Public Property DisplayName As String Implements ISelection.DisplayName
			Public Property Value As String

		End Class

		Private Class SingleFieldControls

			Public Sub New(fieldComboBox As ComboBox, separatorComboBox As ComboBox, customTextBox As TextBox)
				Me.FieldComboBox = fieldComboBox
				Me.SeparatorComboBox = separatorComboBox
				Me.CustomTextBox = customTextBox
			End Sub

			Public Property FieldComboBox As ComboBox
			Public Property SeparatorComboBox As ComboBox
			Public Property CustomTextBox As TextBox

		End Class

	End Class
End Namespace