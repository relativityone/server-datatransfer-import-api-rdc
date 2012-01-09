Namespace kCura.EDDS.WinForm
	Public Class TextFieldPrecedencePicker
		Public Event Picker_Click As EventHandler
		Private Sub PickerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles _pickTextFieldPrecedenceButton.Click
			RaiseEvent Picker_Click(Me, e)
		End Sub
	End Class
End Namespace
