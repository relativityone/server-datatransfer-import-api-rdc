Imports kCura.WinEDDS

Namespace Relativity.Desktop.Client
	Public Class TextAndNativeFileNamePicker

		Public Const SelectOption = "Select..."
		Public Const IdentifierOption = "Identifier"
		Public Const ProductionOption = "Begin production number"
		Public Const CustomOption = "Custom"

		Private WithEvents _form As TextAndNativeFileNameForm
		Private _fields As kCura.WinEDDS.ViewFieldInfo()

		Public Sub Initialize(exportType As ExportFile.ExportType, fields As kCura.WinEDDS.ViewFieldInfo())
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

		Public Property Selection As IList(Of CustomFileNameSelectionPart)

		Private Sub _nativeFileNameSourceComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles _comboBox.SelectedIndexChanged
			Dim isCustomSelected = (SelectedItem = CustomOption)
			_customOptionsButton.Enabled = isCustomSelected
		End Sub

		Private Sub _customOptionsButton_Click(sender As Object, e As EventArgs) Handles _customOptionsButton.Click
			_form = New TextAndNativeFileNameForm()
			_form.Initialize(_fields, Selection)
			_form.ShowDialog()
		End Sub

		Private Sub _form_ApplyClicked() Handles _form.ApplyClicked
			Selection = _form.GetSelection()
		End Sub

	End Class
End Namespace