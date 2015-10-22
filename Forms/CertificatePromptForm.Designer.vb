<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CertificatePromptForm
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
		Me.CloseButton = New System.Windows.Forms.Button()
		Me.BadCertLabel = New System.Windows.Forms.Label()
		Me.AllowButton = New System.Windows.Forms.Button()
		Me.SuspendLayout()
		'
		'CloseButton
		'
		Me.CloseButton.Location = New System.Drawing.Point(203, 51)
		Me.CloseButton.Name = "CloseButton"
		Me.CloseButton.Size = New System.Drawing.Size(75, 23)
		Me.CloseButton.TabIndex = 0
		Me.CloseButton.Text = "Close RDC"
		Me.CloseButton.UseVisualStyleBackColor = True
		'
		'BadCertLabel
		'
		Me.BadCertLabel.AutoSize = True
		Me.BadCertLabel.Location = New System.Drawing.Point(12, 9)
		Me.BadCertLabel.Name = "BadCertLabel"
		Me.BadCertLabel.Size = New System.Drawing.Size(266, 26)
		Me.BadCertLabel.TabIndex = 2
		Me.BadCertLabel.Text = "The certificates being used by Relativity are untrusted. " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Contact a System Admin" & _
	"istrator for more information."
		'
		'AllowButton
		'
		Me.AllowButton.Cursor = System.Windows.Forms.Cursors.Default
		Me.AllowButton.Location = New System.Drawing.Point(15, 51)
		Me.AllowButton.Name = "AllowButton"
		Me.AllowButton.Size = New System.Drawing.Size(162, 23)
		Me.AllowButton.TabIndex = 3
		Me.AllowButton.Text = "Allow un-trusted certificates"
		Me.AllowButton.UseVisualStyleBackColor = True
		'
		'CertificatePromptForm
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(298, 91)
		Me.Controls.Add(Me.AllowButton)
		Me.Controls.Add(Me.BadCertLabel)
		Me.Controls.Add(Me.CloseButton)
		Me.Name = "CertificatePromptForm"
		Me.Text = "CertificatePromptForm"
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Friend WithEvents CloseButton As System.Windows.Forms.Button
	Friend WithEvents BadCertLabel As System.Windows.Forms.Label
	Friend WithEvents AllowButton As System.Windows.Forms.Button
End Class
