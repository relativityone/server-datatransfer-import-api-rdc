Namespace kCura.EDDS.WinForm.Controls
	<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
	Partial Class TextAndNativeFileNamePicker
		Inherits System.Windows.Forms.UserControl

		'UserControl overrides dispose to clean up the component list.
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
			Me._comboBox = New System.Windows.Forms.ComboBox()
			Me._customOptionsButton = New System.Windows.Forms.Button()
			Me.SuspendLayout()
			'
			'_comboBox
			'
			Me._comboBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me._comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._comboBox.FormattingEnabled = True
			Me._comboBox.Location = New System.Drawing.Point(0, 0)
			Me._comboBox.Name = "_comboBox"
			Me._comboBox.Size = New System.Drawing.Size(149, 21)
			Me._comboBox.TabIndex = 0
			'
			'_customOptionsButton
			'
			Me._customOptionsButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me._customOptionsButton.Location = New System.Drawing.Point(151, 0)
			Me._customOptionsButton.Name = "_customOptionsButton"
			Me._customOptionsButton.Size = New System.Drawing.Size(24, 20)
			Me._customOptionsButton.TabIndex = 1
			Me._customOptionsButton.Text = "..."
			Me._customOptionsButton.UseVisualStyleBackColor = True
			'
			'TextAndNativeFileNamePicker
			'
			Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
			Me.Controls.Add(Me._customOptionsButton)
			Me.Controls.Add(Me._comboBox)
			Me.Name = "TextAndNativeFileNamePicker"
			Me.Size = New System.Drawing.Size(176, 20)
			Me.ResumeLayout(False)

		End Sub

		Friend WithEvents _comboBox As ComboBox
		Friend WithEvents _customOptionsButton As Button
	End Class
End Namespace