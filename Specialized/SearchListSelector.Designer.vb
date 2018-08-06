Namespace Specialized
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class SearchListSelector
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
            Me.selectionSearchInput = New System.Windows.Forms.TextBox()
            Me.selectionListBox = New System.Windows.Forms.ListBox()
            Me._selectButton = New System.Windows.Forms.Button()
            Me._cancelButton = New System.Windows.Forms.Button()
            Me.SuspendLayout()
            '
            'selectionSearchInput
            '
            Me.selectionSearchInput.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.selectionSearchInput.Location = New System.Drawing.Point(12, 12)
            Me.selectionSearchInput.Name = "selectionSearchInput"
            Me.selectionSearchInput.Size = New System.Drawing.Size(247, 20)
            Me.selectionSearchInput.TabIndex = 0
            '
            'selectionListBox
            '
            Me.selectionListBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.selectionListBox.FormattingEnabled = True
            Me.selectionListBox.Location = New System.Drawing.Point(12, 38)
            Me.selectionListBox.Name = "selectionListBox"
            Me.selectionListBox.Size = New System.Drawing.Size(247, 212)
            Me.selectionListBox.TabIndex = 1
            '
            '_selectButton
            '
            Me._selectButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me._selectButton.DialogResult = System.Windows.Forms.DialogResult.OK
            Me._selectButton.Location = New System.Drawing.Point(103, 255)
            Me._selectButton.Name = "_selectButton"
            Me._selectButton.Size = New System.Drawing.Size(75, 23)
            Me._selectButton.TabIndex = 2
            Me._selectButton.Text = "Select"
            Me._selectButton.UseVisualStyleBackColor = True
            '
            '_cancelButton
            '
            Me._cancelButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me._cancelButton.Location = New System.Drawing.Point(184, 256)
            Me._cancelButton.Name = "_cancelButton"
            Me._cancelButton.Size = New System.Drawing.Size(75, 23)
            Me._cancelButton.TabIndex = 3
            Me._cancelButton.Text = "Cancel"
            Me._cancelButton.UseVisualStyleBackColor = True
            '
            'SearchListSelector
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(271, 290)
            Me.Controls.Add(Me._cancelButton)
            Me.Controls.Add(Me._selectButton)
            Me.Controls.Add(Me.selectionListBox)
            Me.Controls.Add(Me.selectionSearchInput)
            Me.Name = "SearchListSelector"
            Me.Text = "SearchListSelector"
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Friend WithEvents selectionSearchInput As Windows.Forms.TextBox
        Friend WithEvents selectionListBox As Windows.Forms.ListBox
        Friend WithEvents _selectButton As Windows.Forms.Button
        Friend WithEvents _cancelButton As Windows.Forms.Button
    End Class
End Namespace