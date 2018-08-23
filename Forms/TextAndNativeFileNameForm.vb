Namespace kCura.WinEDDS.Forms
	Public Class TextAndNativeFileNameForm

		Private _numberOfFields As Integer

		Public Sub Initialize() 'TODO (initialization with parameters)
			ChangeNumberOfFields(1)
		End Sub

		Private Sub ChangeNumberOfFields(numberOfFields As Integer)
			_numberOfFields = numberOfFields
			ToggleControls()
		End Sub

		Private Sub ToggleControls()
			_addSecondFieldButton.Visible = (_numberOfFields = 1)
			_firstSeparatorComboBox.Visible = (_numberOfFields > 1)
			_secondFieldComboBox.Visible = (_numberOfFields > 1)
			_secondFieldCustomTextBox.Visible = (_numberOfFields > 1) 'TODO
			_addThirdFieldButton.Visible = (_numberOfFields = 2)
			_removeSecondFieldButton.Visible = (_numberOfFields = 2)
			_secondSeparatorComboBox.Visible = (_numberOfFields > 2)
			_thirdFieldComboBox.Visible = (_numberOfFields > 2)
			_thirdFieldCustomTextBox.Visible = (_numberOfFields > 2) 'TODO
			_removeThirdFieldButton.Visible = (_numberOfFields = 3)
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

	End Class
End Namespace