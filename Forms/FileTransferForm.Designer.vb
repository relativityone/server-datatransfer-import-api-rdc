Imports kCura.WinEDDS.FileTransfer.Extension

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FileTransferForm
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
        Me.ElementHost1 = New System.Windows.Forms.Integration.ElementHost()
        Me.FileTransfer1 = New kCura.WinEDDS.FileTransfer.Extension.FileTransfer()
        Me.SuspendLayout
        '
        'ElementHost1
        '
        Me.ElementHost1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ElementHost1.Location = New System.Drawing.Point(0, 0)
        Me.ElementHost1.Name = "ElementHost1"
        Me.ElementHost1.Size = New System.Drawing.Size(684, 461)
        Me.ElementHost1.TabIndex = 0
        Me.ElementHost1.Text = "ElementHost1"
        Me.ElementHost1.Child = Me.FileTransfer1
        '
        'FileTransferForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(684, 461)
        Me.Controls.Add(Me.ElementHost1)
        Me.MinimumSize = New System.Drawing.Size(700, 500)
        Me.Name = "FileTransferForm"
        Me.Text = "File Transfer"
        Me.ResumeLayout(false)

End Sub

    Friend WithEvents ElementHost1 As Integration.ElementHost
    Friend FileTransfer1 As FileTransfer
End Class
