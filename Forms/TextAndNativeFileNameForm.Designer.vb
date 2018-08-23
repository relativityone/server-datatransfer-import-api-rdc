Namespace kCura.WinEDDS.Forms
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
			Me._firstFieldComboBox = New System.Windows.Forms.TextBox()
			Me._firstSeparatorComboBox = New System.Windows.Forms.ComboBox()
			Me._secondFieldComboBox = New System.Windows.Forms.ComboBox()
			Me._secondSeparatorComboBox = New System.Windows.Forms.ComboBox()
			Me._thirdFieldComboBox = New System.Windows.Forms.ComboBox()
			Me._secondFieldCustomTextBox = New System.Windows.Forms.TextBox()
			Me._thirdFieldCustomTextBox = New System.Windows.Forms.TextBox()
			Me._cancelButton = New System.Windows.Forms.Button()
			Me._applyButton = New System.Windows.Forms.Button()
			Me._removeThirdFieldButton = New System.Windows.Forms.Button()
			Me._addSecondFieldButton = New System.Windows.Forms.Button()
			Me._addThirdFieldButton = New System.Windows.Forms.Button()
			Me._removeSecondFieldButton = New System.Windows.Forms.Button()
			Me.SuspendLayout()
			'
			'_firstFieldComboBox
			'
			Me._firstFieldComboBox.Location = New System.Drawing.Point(13, 13)
			Me._firstFieldComboBox.Name = "_firstFieldComboBox"
			Me._firstFieldComboBox.ReadOnly = True
			Me._firstFieldComboBox.Size = New System.Drawing.Size(120, 20)
			Me._firstFieldComboBox.TabIndex = 0
			Me._firstFieldComboBox.Text = "Control Number"
			'
			'_firstSeparatorComboBox
			'
			Me._firstSeparatorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._firstSeparatorComboBox.FormattingEnabled = True
			Me._firstSeparatorComboBox.Location = New System.Drawing.Point(140, 13)
			Me._firstSeparatorComboBox.Name = "_firstSeparatorComboBox"
			Me._firstSeparatorComboBox.Size = New System.Drawing.Size(40, 21)
			Me._firstSeparatorComboBox.TabIndex = 1
			'
			'_secondFieldComboBox
			'
			Me._secondFieldComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._secondFieldComboBox.FormattingEnabled = True
			Me._secondFieldComboBox.Location = New System.Drawing.Point(186, 13)
			Me._secondFieldComboBox.Name = "_secondFieldComboBox"
			Me._secondFieldComboBox.Size = New System.Drawing.Size(120, 21)
			Me._secondFieldComboBox.TabIndex = 2
			'
			'_secondSeparatorComboBox
			'
			Me._secondSeparatorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._secondSeparatorComboBox.FormattingEnabled = True
			Me._secondSeparatorComboBox.Location = New System.Drawing.Point(312, 13)
			Me._secondSeparatorComboBox.Name = "_secondSeparatorComboBox"
			Me._secondSeparatorComboBox.Size = New System.Drawing.Size(40, 21)
			Me._secondSeparatorComboBox.TabIndex = 3
			'
			'_thirdFieldComboBox
			'
			Me._thirdFieldComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._thirdFieldComboBox.FormattingEnabled = True
			Me._thirdFieldComboBox.Location = New System.Drawing.Point(358, 13)
			Me._thirdFieldComboBox.Name = "_thirdFieldComboBox"
			Me._thirdFieldComboBox.Size = New System.Drawing.Size(120, 21)
			Me._thirdFieldComboBox.TabIndex = 4
			'
			'_secondFieldCustomTextBox
			'
			Me._secondFieldCustomTextBox.Location = New System.Drawing.Point(186, 41)
			Me._secondFieldCustomTextBox.Name = "_secondFieldCustomTextBox"
			Me._secondFieldCustomTextBox.Size = New System.Drawing.Size(120, 20)
			Me._secondFieldCustomTextBox.TabIndex = 5
			'
			'_thirdFieldCustomTextBox
			'
			Me._thirdFieldCustomTextBox.Location = New System.Drawing.Point(358, 41)
			Me._thirdFieldCustomTextBox.Name = "_thirdFieldCustomTextBox"
			Me._thirdFieldCustomTextBox.Size = New System.Drawing.Size(120, 20)
			Me._thirdFieldCustomTextBox.TabIndex = 6
			'
			'_cancelButton
			'
			Me._cancelButton.Location = New System.Drawing.Point(434, 67)
			Me._cancelButton.Name = "_cancelButton"
			Me._cancelButton.Size = New System.Drawing.Size(75, 23)
			Me._cancelButton.TabIndex = 8
			Me._cancelButton.Text = "Cancel"
			Me._cancelButton.UseVisualStyleBackColor = True
			'
			'_applyButton
			'
			Me._applyButton.Location = New System.Drawing.Point(353, 67)
			Me._applyButton.Name = "_applyButton"
			Me._applyButton.Size = New System.Drawing.Size(75, 23)
			Me._applyButton.TabIndex = 9
			Me._applyButton.Text = "Apply"
			Me._applyButton.UseVisualStyleBackColor = True
			'
			'_removeThirdFieldButton
			'
			Me._removeThirdFieldButton.Location = New System.Drawing.Point(484, 12)
			Me._removeThirdFieldButton.Name = "_removeThirdFieldButton"
			Me._removeThirdFieldButton.Size = New System.Drawing.Size(25, 23)
			Me._removeThirdFieldButton.TabIndex = 10
			Me._removeThirdFieldButton.Text = "-"
			Me._removeThirdFieldButton.UseVisualStyleBackColor = True
			'
			'_addSecondFieldButton
			'
			Me._addSecondFieldButton.Location = New System.Drawing.Point(140, 12)
			Me._addSecondFieldButton.Name = "_addSecondFieldButton"
			Me._addSecondFieldButton.Size = New System.Drawing.Size(25, 23)
			Me._addSecondFieldButton.TabIndex = 11
			Me._addSecondFieldButton.Text = "+"
			Me._addSecondFieldButton.UseVisualStyleBackColor = True
			'
			'_addThirdFieldButton
			'
			Me._addThirdFieldButton.Location = New System.Drawing.Point(312, 12)
			Me._addThirdFieldButton.Name = "_addThirdFieldButton"
			Me._addThirdFieldButton.Size = New System.Drawing.Size(25, 23)
			Me._addThirdFieldButton.TabIndex = 12
			Me._addThirdFieldButton.Text = "+"
			Me._addThirdFieldButton.UseVisualStyleBackColor = True
			'
			'_removeSecondFieldButton
			'
			Me._removeSecondFieldButton.Location = New System.Drawing.Point(344, 12)
			Me._removeSecondFieldButton.Name = "_removeSecondFieldButton"
			Me._removeSecondFieldButton.Size = New System.Drawing.Size(25, 23)
			Me._removeSecondFieldButton.TabIndex = 13
			Me._removeSecondFieldButton.Text = "-"
			Me._removeSecondFieldButton.UseVisualStyleBackColor = True
			'
			'TextAndNativeFileNameForm
			'
			Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
			Me.ClientSize = New System.Drawing.Size(521, 102)
			Me.Controls.Add(Me._removeSecondFieldButton)
			Me.Controls.Add(Me._addThirdFieldButton)
			Me.Controls.Add(Me._addSecondFieldButton)
			Me.Controls.Add(Me._removeThirdFieldButton)
			Me.Controls.Add(Me._applyButton)
			Me.Controls.Add(Me._cancelButton)
			Me.Controls.Add(Me._thirdFieldCustomTextBox)
			Me.Controls.Add(Me._secondFieldCustomTextBox)
			Me.Controls.Add(Me._thirdFieldComboBox)
			Me.Controls.Add(Me._secondSeparatorComboBox)
			Me.Controls.Add(Me._secondFieldComboBox)
			Me.Controls.Add(Me._firstSeparatorComboBox)
			Me.Controls.Add(Me._firstFieldComboBox)
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
			Me.Name = "TextAndNativeFileNameForm"
			Me.ShowIcon = False
			Me.ShowInTaskbar = False
			Me.Text = "Custom File Naming Options"
			Me.ResumeLayout(False)
			Me.PerformLayout()

		End Sub

		Friend WithEvents _firstFieldComboBox As TextBox
		Friend WithEvents _firstSeparatorComboBox As ComboBox
		Friend WithEvents _secondFieldComboBox As ComboBox
		Friend WithEvents _secondSeparatorComboBox As ComboBox
		Friend WithEvents _thirdFieldComboBox As ComboBox
		Friend WithEvents _secondFieldCustomTextBox As TextBox
		Friend WithEvents _thirdFieldCustomTextBox As TextBox
		Friend WithEvents _cancelButton As Button
		Friend WithEvents _applyButton As Button
		Friend WithEvents _removeThirdFieldButton As Button
		Friend WithEvents _addSecondFieldButton As Button
		Friend WithEvents _addThirdFieldButton As Button
		Friend WithEvents _removeSecondFieldButton As Button
	End Class
End Namespace