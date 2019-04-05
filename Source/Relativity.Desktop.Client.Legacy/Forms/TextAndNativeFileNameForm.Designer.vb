Namespace kCura.EDDS.WinForm.Forms
	<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
	Partial Class TextAndNativeFileNameForm
		Inherits System.Windows.Forms.Form

		'Form overrides dispose to clean up the component list.
		<System.Diagnostics.DebuggerNonUserCode()>
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
		<System.Diagnostics.DebuggerStepThrough()>
		Private Sub InitializeComponent()
		Me._cancelButton = New System.Windows.Forms.Button()
		Me._applyButton = New System.Windows.Forms.Button()
		Me._addFieldButton = New System.Windows.Forms.Button()
		Me._removeFieldButton = New System.Windows.Forms.Button()
		Me._firstFieldComboBox = New System.Windows.Forms.ComboBox()
		Me.SuspendLayout()
		'
		'_cancelButton
		'
		Me._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
		Me._cancelButton.Location = New System.Drawing.Point(594, 67)
		Me._cancelButton.Name = "_cancelButton"
		Me._cancelButton.Size = New System.Drawing.Size(75, 23)
		Me._cancelButton.TabIndex = 99
		Me._cancelButton.Text = "Cancel"
		Me._cancelButton.UseVisualStyleBackColor = true
		'
		'_applyButton
		'
		Me._applyButton.Location = New System.Drawing.Point(513, 67)
		Me._applyButton.Name = "_applyButton"
		Me._applyButton.Size = New System.Drawing.Size(75, 23)
		Me._applyButton.TabIndex = 98
		Me._applyButton.Text = "Apply"
		Me._applyButton.UseVisualStyleBackColor = true
		'
		'_addFieldButton
		'
		Me._addFieldButton.Location = New System.Drawing.Point(140, 12)
		Me._addFieldButton.Name = "_addFieldButton"
		Me._addFieldButton.Size = New System.Drawing.Size(25, 23)
		Me._addFieldButton.TabIndex = 96
		Me._addFieldButton.Text = "+"
		Me._addFieldButton.UseVisualStyleBackColor = true
		'
		'_removeFieldButton
		'
		Me._removeFieldButton.Location = New System.Drawing.Point(171, 12)
		Me._removeFieldButton.Name = "_removeFieldButton"
		Me._removeFieldButton.Size = New System.Drawing.Size(25, 23)
		Me._removeFieldButton.TabIndex = 97
		Me._removeFieldButton.Text = "-"
		Me._removeFieldButton.UseVisualStyleBackColor = true
		'
		'_firstFieldComboBox
		'
		Me._firstFieldComboBox.FormattingEnabled = true
		Me._firstFieldComboBox.Location = New System.Drawing.Point(14, 12)
		Me._firstFieldComboBox.Name = "_firstFieldComboBox"
		Me._firstFieldComboBox.Size = New System.Drawing.Size(121, 21)
		Me._firstFieldComboBox.TabIndex = 100
		'
		'TextAndNativeFileNameForm
		'
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
		Me.CancelButton = Me._cancelButton
		Me.ClientSize = New System.Drawing.Size(681, 102)
		Me.Controls.Add(Me._firstFieldComboBox)
		Me.Controls.Add(Me._removeFieldButton)
		Me.Controls.Add(Me._addFieldButton)
		Me.Controls.Add(Me._applyButton)
		Me.Controls.Add(Me._cancelButton)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
		Me.Name = "TextAndNativeFileNameForm"
		Me.ShowIcon = false
		Me.ShowInTaskbar = false
		Me.Text = "Custom File Naming Options"
		Me.ResumeLayout(false)

End Sub
		Friend WithEvents _cancelButton As Button
		Friend WithEvents _applyButton As Button
		Friend WithEvents _addFieldButton As Button
		Friend WithEvents _removeFieldButton As Button
		Friend WithEvents _firstFieldComboBox As ComboBox
	End Class
End Namespace