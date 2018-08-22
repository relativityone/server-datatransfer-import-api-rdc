﻿Namespace kCura.EDDS.WinForm.Controls
	Public Class TextAndNativeFileNamePicker

		Public Const SelectOption = "Select..."
		Public Const IdentifierOption = "Identifier"
		Public Const ProductionOption = "Begin production number"
		Public Const CustomOption = "Custom"

		Public Sub Initialize(exportType As ExportFile.ExportType)
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

		Private Sub _nativeFileNameSourceComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles _comboBox.SelectedIndexChanged
			Dim isCustomSelected = (SelectedItem = CustomOption)
			_customOptionsButton.Enabled = isCustomSelected
		End Sub
	End Class
End Namespace