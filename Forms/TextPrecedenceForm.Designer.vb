Namespace kCura.EDDS.WinForm
	<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
	Partial Class TextPrecedenceForm
		Inherits System.Windows.Forms.Form

		'Form overrides dispose to clean up the component list.
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
			Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TextPrecedenceForm))
			Me._okButton = New System.Windows.Forms.Button()
			Me._cancelButton = New System.Windows.Forms.Button()
			Me.SuspendLayout()
			'
			'_okButton
			'
			Me._okButton.Location = New System.Drawing.Point(369, 327)
			Me._okButton.Name = "_okButton"
			Me._okButton.Size = New System.Drawing.Size(75, 23)
			Me._okButton.TabIndex = 0
			Me._okButton.Text = "OK"
			Me._okButton.UseVisualStyleBackColor = True
			'
			'_cancelButton
			'
			Me._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me._cancelButton.Location = New System.Drawing.Point(369, 358)
			Me._cancelButton.Name = "_cancelButton"
			Me._cancelButton.Size = New System.Drawing.Size(75, 23)
			Me._cancelButton.TabIndex = 1
			Me._cancelButton.Text = "Cancel"
			Me._cancelButton.UseVisualStyleBackColor = True
			'
			'TextPrecedenceForm
			'
			Me.AcceptButton = Me._okButton
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.CancelButton = Me._cancelButton
			Me.ClientSize = New System.Drawing.Size(456, 393)
			Me.Controls.Add(Me._cancelButton)
			Me.Controls.Add(Me._okButton)
			Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
			Me.MaximizeBox = False
			Me.MaximumSize = New System.Drawing.Size(464, 420)
			Me.MinimizeBox = False
			Me.MinimumSize = New System.Drawing.Size(464, 420)
			Me.Name = "TextPrecedenceForm"
			Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
			Me.Text = "Text Precedence Form"
			Me.ResumeLayout(False)

		End Sub
		Friend WithEvents _okButton As System.Windows.Forms.Button
		Friend WithEvents _cancelButton As System.Windows.Forms.Button


	End Class
End Namespace
