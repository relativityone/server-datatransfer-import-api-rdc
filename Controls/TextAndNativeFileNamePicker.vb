Imports System.Collections.Generic
Imports kCura.EDDS.WinForm.Forms

Namespace kCura.EDDS.WinForm.Controls
	Public Class TextAndNativeFileNamePicker

		Public Const SelectOption = "Select..."
		Public Const IdentifierOption = "Identifier"
		Public Const ProductionOption = "Begin production number"
		Public Const CustomOption = "Custom"

		Private WithEvents _form As TextAndNativeFileNameForm
		Private _fields As ViewFieldInfo()
		Private _selection As IList(Of CustomFileNameSelectionPart)

		Public Sub Initialize(exportType As ExportFile.ExportType, fields As ViewFieldInfo())
			_fields = fields
			_comboBox.Items.Clear()
			AddDropdownItems(exportType)
		End Sub

		Private Sub AddDropdownItems(exportType As ExportFile.ExportType)
			_comboBox.Items.Add(SelectOption)
			_comboBox.Items.Add(IdentifierOption)
			If exportType = ExportFile.ExportType.Production Then
				_comboBox.Items.Add(ProductionOption)
			End If
			_comboBox.Items.Add(CustomOption)
			SelectedItem = SelectOption
		End Sub

		Public Property SelectedItem As String
			Get
				Return _comboBox.SelectedItem.ToString()
			End Get
			Set
				_comboBox.SelectedItem = Value
			End Set
		End Property

		Public ReadOnly Property Selection As IList(Of CustomFileNameSelectionPart)
			Get
				Return _selection
			End Get
		End Property

		Private Sub _nativeFileNameSourceComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles _comboBox.SelectedIndexChanged
			Dim isCustomSelected = (SelectedItem = CustomOption)
			_customOptionsButton.Enabled = isCustomSelected
		End Sub

		Private Sub _customOptionsButton_Click(sender As Object, e As EventArgs) Handles _customOptionsButton.Click
			_form = New TextAndNativeFileNameForm()
			_form.Initialize(_fields)
			_form.ShowDialog()
		End Sub

		Private Sub _form_ApplyClicked() Handles _form.ApplyClicked
			_selection = _form.GetSelection()
		End Sub

	End Class
End Namespace