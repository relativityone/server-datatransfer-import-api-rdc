Namespace Specialized
	<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
	Partial Class SearchableList
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
			Me._textBox = New System.Windows.Forms.TextBox()
			Me._listBox = New kCura.Windows.Forms.ListBox()
			Me.SuspendLayout()
			'
			'_textBox
			'
			Me._textBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me._textBox.Location = New System.Drawing.Point(0, 0)
			Me._textBox.Name = "_textBox"
			Me._textBox.Size = New System.Drawing.Size(144, 20)
			Me._textBox.TabIndex = 0
			'
			'_listBox
			'
			Me._listBox.AlternateColors = False
			Me._listBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
			Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me._listBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
			Me._listBox.FormattingEnabled = True
			Me._listBox.HighlightIndex = -1
			Me._listBox.HorizontalScrollbar = True
			Me._listBox.HorizontalScrollOffset = 0
			Me._listBox.Location = New System.Drawing.Point(0, 26)
			Me._listBox.Name = "_listBox"
			Me._listBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
			Me._listBox.Size = New System.Drawing.Size(144, 251)
			Me._listBox.TabIndex = 17
			Me._listBox.VerticalScrollOffset = 0
			'
			'SearchableList
			'
			Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
			Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
			Me.Controls.Add(Me._listBox)
			Me.Controls.Add(Me._textBox)
			Me.Name = "SearchableList"
			Me.Size = New System.Drawing.Size(144, 280)
			Me.ResumeLayout(False)
			Me.PerformLayout()

		End Sub

		Friend WithEvents _textBox As Windows.Forms.TextBox
		Friend WithEvents _listBox As kCura.Windows.Forms.ListBox
	End Class
End Namespace