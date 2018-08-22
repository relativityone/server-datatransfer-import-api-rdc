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
			Me._nativeFileNameSourceComboBox = New System.Windows.Forms.ComboBox()
			Me._nativeFileNameCustomOptions = New System.Windows.Forms.Button()
			Me.SuspendLayout()
			'
			'_nativeFileNameSourceComboBox
			'
			Me._nativeFileNameSourceComboBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me._nativeFileNameSourceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._nativeFileNameSourceComboBox.FormattingEnabled = True
			Me._nativeFileNameSourceComboBox.Location = New System.Drawing.Point(0, 0)
			Me._nativeFileNameSourceComboBox.Name = "_nativeFileNameSourceComboBox"
			Me._nativeFileNameSourceComboBox.Size = New System.Drawing.Size(149, 21)
			Me._nativeFileNameSourceComboBox.TabIndex = 0
			'
			'_nativeFileNameCustomOptions
			'
			Me._nativeFileNameCustomOptions.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me._nativeFileNameCustomOptions.Location = New System.Drawing.Point(151, 0)
			Me._nativeFileNameCustomOptions.Name = "_nativeFileNameCustomOptions"
			Me._nativeFileNameCustomOptions.Size = New System.Drawing.Size(24, 20)
			Me._nativeFileNameCustomOptions.TabIndex = 1
			Me._nativeFileNameCustomOptions.Text = "..."
			Me._nativeFileNameCustomOptions.UseVisualStyleBackColor = True
			'
			'TextAndNativeFileNamePicker
			'
			Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
			Me.Controls.Add(Me._nativeFileNameCustomOptions)
			Me.Controls.Add(Me._nativeFileNameSourceComboBox)
			Me.Name = "TextAndNativeFileNamePicker"
			Me.Size = New System.Drawing.Size(176, 20)
			Me.ResumeLayout(False)

		End Sub

		Friend WithEvents _nativeFileNameSourceComboBox As ComboBox
		Friend WithEvents _nativeFileNameCustomOptions As Button
	End Class
End Namespace