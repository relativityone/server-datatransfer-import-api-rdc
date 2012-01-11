Namespace kCura.EDDS.WinForm
	<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
	Partial Class TextFieldPrecedencePicker
		Inherits System.Windows.Forms.UserControl

		'UserControl overrides dispose to clean up the component list.
		<System.Diagnostics.DebuggerNonUserCode()> _
		Protected Overrides Sub Dispose(ByVal disposing As Boolean)
			Try
				If disposing AndAlso components IsNot Nothing Then
					components.Dispose()
				End If
			Finally
				MyBase.Dispose(disposing)
			End Try
		End Sub

		'Required by the Windows Form Designer
		Private components As System.ComponentModel.IContainer

		'NOTE: The following procedure is required by the Windows Form Designer
		'It can be modified using the Windows Form Designer.  
		'Do not modify it using the code editor.
		<System.Diagnostics.DebuggerStepThrough()> _
		Private Sub InitializeComponent()
			Me._pickTextFieldPrecedenceButton = New System.Windows.Forms.Button()
			Me._selectedTextFieldsTextBox = New System.Windows.Forms.TextBox()
			Me.SuspendLayout()
			'
			'_pickTextFieldPrecedenceButton
			'
			Me._pickTextFieldPrecedenceButton.Anchor = System.Windows.Forms.AnchorStyles.Top
			Me._pickTextFieldPrecedenceButton.Location = New System.Drawing.Point(150, 0)
			Me._pickTextFieldPrecedenceButton.Name = "_pickTextFieldPrecedenceButton"
			Me._pickTextFieldPrecedenceButton.Size = New System.Drawing.Size(24, 20)
			Me._pickTextFieldPrecedenceButton.TabIndex = 2
			Me._pickTextFieldPrecedenceButton.Text = "..."
			'
			'_selectedTextFieldsTextBox
			'
			Me._selectedTextFieldsTextBox.Location = New System.Drawing.Point(0, 0)
			Me._selectedTextFieldsTextBox.Name = "_selectedTextFieldsTextBox"
			Me._selectedTextFieldsTextBox.ReadOnly = True
			Me._selectedTextFieldsTextBox.Size = New System.Drawing.Size(148, 20)
			Me._selectedTextFieldsTextBox.TabIndex = 4
			'
			'TextFieldPrecedencePicker
			'
			Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
			Me.Controls.Add(Me._selectedTextFieldsTextBox)
			Me.Controls.Add(Me._pickTextFieldPrecedenceButton)
			Me.Name = "TextFieldPrecedencePicker"
			Me.Size = New System.Drawing.Size(175, 20)
			Me.ResumeLayout(False)
			Me.PerformLayout()

		End Sub
		Private WithEvents _selectedTextFieldsTextBox As System.Windows.Forms.TextBox
		Private WithEvents _pickTextFieldPrecedenceButton As System.Windows.Forms.Button

	End Class
End Namespace
