Namespace kCura.EDDS.WinForm.Controls
	Public Class TextAndNativeFileNamePicker

		Public Sub InitializeDropdown()
			'TODO'
		End Sub

		Public Property SelectedItem As String
			Get
				Return _nativeFileNameSourceComboBox.SelectedItem.ToString()
			End Get
			Set
				_nativeFileNameSourceComboBox.SelectedItem = Value
			End Set
		End Property

	End Class
End Namespace