<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AboutForm
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
		Me.OKBtn = New System.Windows.Forms.Button()
		Me.MainTextLabel = New System.Windows.Forms.Label()
		Me.DividerCtrl = New System.Windows.Forms.GroupBox()
		Me.CopyrightBtn = New System.Windows.Forms.Button()
		Me.CopyrightTextBox = New System.Windows.Forms.TextBox()
		Me.Panel1 = New System.Windows.Forms.Panel()
		Me.SuspendLayout()
		'
		'OKBtn
		'
		Me.OKBtn.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.OKBtn.Location = New System.Drawing.Point(338, 352)
		Me.OKBtn.Name = "OKBtn"
		Me.OKBtn.Size = New System.Drawing.Size(75, 23)
		Me.OKBtn.TabIndex = 0
		Me.OKBtn.Text = "OK"
		Me.OKBtn.UseVisualStyleBackColor = True
		'
		'MainTextLabel
		'
		Me.MainTextLabel.BackColor = System.Drawing.Color.White
		Me.MainTextLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
		Me.MainTextLabel.Location = New System.Drawing.Point(12, 26)
		Me.MainTextLabel.Name = "MainTextLabel"
		Me.MainTextLabel.Size = New System.Drawing.Size(401, 138)
		Me.MainTextLabel.TabIndex = 1
		Me.MainTextLabel.Text = "Relativity Desktop Client"
		'
		'DividerCtrl
		'
		Me.DividerCtrl.Location = New System.Drawing.Point(36, 174)
		Me.DividerCtrl.Name = "DividerCtrl"
		Me.DividerCtrl.Size = New System.Drawing.Size(350, 2)
		Me.DividerCtrl.TabIndex = 2
		Me.DividerCtrl.TabStop = False
		Me.DividerCtrl.Text = "Copyright Notices"
		'
		'CopyrightBtn
		'
		Me.CopyrightBtn.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.CopyrightBtn.Location = New System.Drawing.Point(147, 352)
		Me.CopyrightBtn.Name = "CopyrightBtn"
		Me.CopyrightBtn.Size = New System.Drawing.Size(185, 23)
		Me.CopyrightBtn.TabIndex = 3
		Me.CopyrightBtn.Text = "Copyright and License Notices"
		Me.CopyrightBtn.UseVisualStyleBackColor = True
		'
		'CopyrightTextBox
		'
		Me.CopyrightTextBox.Location = New System.Drawing.Point(12, 186)
		Me.CopyrightTextBox.Multiline = True
		Me.CopyrightTextBox.Name = "CopyrightTextBox"
		Me.CopyrightTextBox.ReadOnly = True
		Me.CopyrightTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
		Me.CopyrightTextBox.Size = New System.Drawing.Size(401, 160)
		Me.CopyrightTextBox.TabIndex = 4
		'
		'Panel1
		'
		Me.Panel1.BackColor = System.Drawing.Color.White
		Me.Panel1.Location = New System.Drawing.Point(0, 0)
		Me.Panel1.Name = "Panel1"
		Me.Panel1.Size = New System.Drawing.Size(444, 164)
		Me.Panel1.TabIndex = 5
		'
		'AboutForm
		'
		Me.AcceptButton = Me.OKBtn
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(425, 385)
		Me.Controls.Add(Me.CopyrightTextBox)
		Me.Controls.Add(Me.CopyrightBtn)
		Me.Controls.Add(Me.DividerCtrl)
		Me.Controls.Add(Me.MainTextLabel)
		Me.Controls.Add(Me.OKBtn)
		Me.Controls.Add(Me.Panel1)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
		Me.MaximizeBox = False
		Me.MinimizeBox = False
		Me.Name = "AboutForm"
		Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
		Me.Text = "Relativity Desktop Client | About"
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Friend WithEvents OKBtn As System.Windows.Forms.Button
	Friend WithEvents MainTextLabel As System.Windows.Forms.Label
	Friend WithEvents DividerCtrl As System.Windows.Forms.GroupBox
	Friend WithEvents CopyrightBtn As System.Windows.Forms.Button
	Friend WithEvents CopyrightTextBox As System.Windows.Forms.TextBox
	Friend WithEvents Panel1 As System.Windows.Forms.Panel
End Class
