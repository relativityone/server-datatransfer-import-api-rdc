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
			Me.PotentialTextFields = New System.Windows.Forms.ComboBox()
			Me._pickTextFieldPrecedenceButton = New System.Windows.Forms.Button()
			Me.SuspendLayout()
			'
			'PotentialTextFields
			'
			Me.PotentialTextFields.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
				Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.PotentialTextFields.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me.PotentialTextFields.Location = New System.Drawing.Point(0, 0)
			Me.PotentialTextFields.Name = "PotentialTextFields"
			Me.PotentialTextFields.Size = New System.Drawing.Size(149, 21)
			Me.PotentialTextFields.TabIndex = 1
			'
			'_pickTextFieldPrecedenceButton
			'
			Me._pickTextFieldPrecedenceButton.Anchor = System.Windows.Forms.AnchorStyles.Right
			Me._pickTextFieldPrecedenceButton.Location = New System.Drawing.Point(151, 0)
			Me._pickTextFieldPrecedenceButton.Name = "_pickTextFieldPrecedenceButton"
			Me._pickTextFieldPrecedenceButton.Size = New System.Drawing.Size(24, 20)
			Me._pickTextFieldPrecedenceButton.TabIndex = 2
			Me._pickTextFieldPrecedenceButton.Text = "..."
			'
			'TextFieldPrecedencePicker
			'
			Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
			Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
			Me.Controls.Add(Me._pickTextFieldPrecedenceButton)
			Me.Controls.Add(Me.PotentialTextFields)
			Me.Name = "TextFieldPrecedencePicker"
			Me.Size = New System.Drawing.Size(175, 21)
			Me.ResumeLayout(False)

		End Sub
		Friend WithEvents _pickTextFieldPrecedenceButton As System.Windows.Forms.Button
		Public WithEvents PotentialTextFields As System.Windows.Forms.ComboBox

	End Class
End Namespace
