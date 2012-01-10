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
			Me.SelectedTextFieldsListBox = New System.Windows.Forms.ListBox()
			Me.SuspendLayout()
			'
			'_pickTextFieldPrecedenceButton
			'
			Me._pickTextFieldPrecedenceButton.Anchor = System.Windows.Forms.AnchorStyles.Top
			Me._pickTextFieldPrecedenceButton.Location = New System.Drawing.Point(151, 0)
			Me._pickTextFieldPrecedenceButton.Name = "_pickTextFieldPrecedenceButton"
			Me._pickTextFieldPrecedenceButton.Size = New System.Drawing.Size(24, 20)
			Me._pickTextFieldPrecedenceButton.TabIndex = 2
			Me._pickTextFieldPrecedenceButton.Text = "..."
			'
			'SelectedTextFieldsListBox
			'
			Me.SelectedTextFieldsListBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me.SelectedTextFieldsListBox.FormattingEnabled = True
			Me.SelectedTextFieldsListBox.ItemHeight = 15
			Me.SelectedTextFieldsListBox.Location = New System.Drawing.Point(0, 0)
			Me.SelectedTextFieldsListBox.Name = "SelectedTextFieldsListBox"
			Me.SelectedTextFieldsListBox.Size = New System.Drawing.Size(149, 19)
			Me.SelectedTextFieldsListBox.TabIndex = 3
			'
			'TextFieldPrecedencePicker
			'
			Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
			Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
			Me.Controls.Add(Me.SelectedTextFieldsListBox)
			Me.Controls.Add(Me._pickTextFieldPrecedenceButton)
			Me.Name = "TextFieldPrecedencePicker"
			Me.Size = New System.Drawing.Size(175, 21)
			Me.ResumeLayout(False)

		End Sub
		Friend WithEvents _pickTextFieldPrecedenceButton As System.Windows.Forms.Button
		Public WithEvents SelectedTextFieldsListBox As System.Windows.Forms.ListBox

	End Class
End Namespace
