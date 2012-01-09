Namespace kCura.EDDS.WinForm
	Public Class TextPrecedenceForm
		Friend WithEvents _longTextFields As kCura.Windows.Forms.TwoListBox
		Friend WithEvents _availableLongTextFieldsLabel As System.Windows.Forms.Label
		Friend WithEvents _selectedLongTextFieldsLabel As System.Windows.Forms.Label



		Private Sub TextPrecedenceForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
			'Doing this here so designer works at least for some elements
			Me._longTextFields = New kCura.Windows.Forms.TwoListBox()
			Me._longTextFields.AlternateRowColors = False
			Me._longTextFields.KeepButtonsCentered = True
			Me._longTextFields.LeftOrderControlsVisible = False
			Me._longTextFields.Location = New System.Drawing.Point(8, 104)
			Me._longTextFields.Name = "_longTextFields"
			Me._longTextFields.RightOrderControlVisible = True
			Me._longTextFields.Size = New System.Drawing.Size(360, 280)
			Me._longTextFields.TabIndex = 2

			Me._availableLongTextFieldsLabel = New System.Windows.Forms.Label
			Me._availableLongTextFieldsLabel.Location = New System.Drawing.Point(8, 88)
			Me._availableLongTextFieldsLabel.Name = "_availableLongTextFieldsLabel"
			Me._availableLongTextFieldsLabel.Size = New System.Drawing.Size(144, 16)
			Me._availableLongTextFieldsLabel.TabIndex = 3
			Me._availableLongTextFieldsLabel.Text = "Available Long Text Fields"

			Me._selectedLongTextFieldsLabel = New System.Windows.Forms.Label
			Me._selectedLongTextFieldsLabel.Location = New System.Drawing.Point(196, 88)
			Me._selectedLongTextFieldsLabel.Name = "_selectedLongTextFieldsLabel"
			Me._selectedLongTextFieldsLabel.Size = New System.Drawing.Size(144, 16)
			Me._selectedLongTextFieldsLabel.TabIndex = 4
			Me._selectedLongTextFieldsLabel.Text = "Selected Long Text Fields"

			Me.Controls.Add(Me._longTextFields)
			Me.Controls.Add(Me._availableLongTextFieldsLabel)
			Me.Controls.Add(Me._selectedLongTextFieldsLabel)

		End Sub
	End Class
End Namespace