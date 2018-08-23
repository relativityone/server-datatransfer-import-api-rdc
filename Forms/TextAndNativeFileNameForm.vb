Imports System.Collections.Generic
Imports System.Linq
Imports Relativity

Namespace kCura.EDDS.WinForm.Forms
	Public Class TextAndNativeFileNameForm

		Private Const CustomTextOption As String = "Custom Text..."

		Private ReadOnly AllowedFieldTypes As FieldTypeHelper.FieldType() = New FieldTypeHelper.FieldType() {
			FieldTypeHelper.FieldType.Varchar,
			FieldTypeHelper.FieldType.Date,
			FieldTypeHelper.FieldType.Integer,
			FieldTypeHelper.FieldType.Decimal,
			FieldTypeHelper.FieldType.Boolean,
			FieldTypeHelper.FieldType.Code
		}

		Private ReadOnly Separators As SeparatorSelection() = New SeparatorSelection() {
			New SeparatorSelection("- (hyphen)", "-"),
			New SeparatorSelection("_ (underscore)", "_")
		}

		Private _numberOfFields As Integer
		Private _availableFields As List(Of FieldSelection)

		Public Sub Initialize(fields As ViewFieldInfo())
			ChangeNumberOfFields(1)
			InitializeAvailableFields(fields)
			InitializeBindings()
		End Sub

		Private Sub InitializeBindings()
			_secondFieldComboBox.DataSource = _availableFields
			_secondFieldComboBox.DisplayMember = "DisplayName"
			_secondFieldComboBox.ValueMember = "ID"
			InitializeFieldBindings(_secondFieldComboBox)
			InitializeFieldBindings(_thirdFieldComboBox)
			InitializeSeparatorBindings(_firstSeparatorComboBox)
			InitializeSeparatorBindings(_secondSeparatorComboBox)
		End Sub

		Private Sub InitializeFieldBindings(comboBox As ComboBox)
			comboBox.DataSource = _availableFields.ToList()
			comboBox.DisplayMember = "DisplayName"
		End Sub

		Private Sub InitializeSeparatorBindings(comboBox As ComboBox)
			comboBox.DataSource = Separators.ToList()
			comboBox.DisplayMember = "DisplayName"
		End Sub

		Private Sub ChangeNumberOfFields(numberOfFields As Integer)
			_numberOfFields = numberOfFields
			ToggleControls()
		End Sub

		Private Sub ToggleControls()
			_addSecondFieldButton.Visible = (_numberOfFields = 1)
			_firstSeparatorComboBox.Visible = (_numberOfFields > 1)
			_secondFieldComboBox.Visible = (_numberOfFields > 1)
			ToggleSecondFieldCustomTextBox()
			_addThirdFieldButton.Visible = (_numberOfFields = 2)
			_removeSecondFieldButton.Visible = (_numberOfFields = 2)
			_secondSeparatorComboBox.Visible = (_numberOfFields > 2)
			_thirdFieldComboBox.Visible = (_numberOfFields > 2)
			ToggleThirdFieldCustomTextBox()
			_removeThirdFieldButton.Visible = (_numberOfFields = 3)
		End Sub

		Private Sub ToggleSecondFieldCustomTextBox()
			ToggleFieldCustomTextBox(_secondFieldCustomTextBox, _secondFieldComboBox, 1)
		End Sub

		Private Sub ToggleThirdFieldCustomTextBox()
			ToggleFieldCustomTextBox(_thirdFieldCustomTextBox, _thirdFieldComboBox, 2)
		End Sub

		Private Sub ToggleFieldCustomTextBox(textBox As TextBox, comboBox As ComboBox, fieldNumber As Integer)
			Dim textBoxVisible = False
			If _numberOfFields > fieldNumber Then
				Dim selectedItem = TryCast(comboBox.SelectedItem, FieldSelection)
				If selectedItem IsNot Nothing And selectedItem.DisplayName = CustomTextOption Then
					textBoxVisible = True
				End If
			End If
			textBox.Visible = textBoxVisible
		End Sub

		Private Sub InitializeAvailableFields(fields As IEnumerable(Of ViewFieldInfo))
			_availableFields = New List(Of FieldSelection) From {New FieldSelection(CustomTextOption, -1)}
			Dim databaseFields = fields.
				Where(Function(f) AllowedFieldTypes.Contains(f.FieldType)).
				Select(Function(f) New FieldSelection(f.DisplayName, f.FieldArtifactId))
			_availableFields.AddRange(databaseFields)
		End Sub

		Private Sub _addSecondFieldButton_Click(sender As Object, e As EventArgs) Handles _addSecondFieldButton.Click
			ChangeNumberOfFields(2)
		End Sub

		Private Sub _addThirdFieldButton_Click(sender As Object, e As EventArgs) Handles _addThirdFieldButton.Click
			ChangeNumberOfFields(3)
		End Sub

		Private Sub _removeSecondFieldButton_Click(sender As Object, e As EventArgs) Handles _removeSecondFieldButton.Click
			ChangeNumberOfFields(1)
		End Sub

		Private Sub _removeThirdFieldButton_Click(sender As Object, e As EventArgs) Handles _removeThirdFieldButton.Click
			ChangeNumberOfFields(2)
		End Sub

		Private Sub _secondFieldComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles _secondFieldComboBox.SelectedIndexChanged
			ToggleSecondFieldCustomTextBox()
		End Sub

		Private Sub _thirdFieldComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles _thirdFieldComboBox.SelectedIndexChanged
			ToggleThirdFieldCustomTextBox()
		End Sub

		Private Class FieldSelection

			Public Sub New(displayName As String, id As Integer)
				Me.DisplayName = displayName
				Me.ID = id
			End Sub

			Public Property DisplayName As String
			Public Property ID As Integer

		End Class

		Private Class SeparatorSelection

			Public Sub New(displayName As String, value As String)
				Me.DisplayName = displayName
				Me.Value = value
			End Sub

			Public Property DisplayName As String
			Public Property Value As String

		End Class

	End Class
End Namespace