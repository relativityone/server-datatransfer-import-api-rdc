Namespace kCura.Windows.Process
	Public Class MessageBoxForm
		Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

		Public Sub New()
			MyBase.New()

			'This call is required by the Windows Form Designer.
			InitializeComponent()

			'Add any initialization after the InitializeComponent() call

		End Sub

		'Form overrides dispose to clean up the component list.
		Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
			If disposing Then
				If Not (components Is Nothing) Then
					components.Dispose()
				End If
			End If
			MyBase.Dispose(disposing)
		End Sub

		'Required by the Windows Form Designer
		Private components As System.ComponentModel.IContainer

		'NOTE: The following procedure is required by the Windows Form Designer
		'It can be modified using the Windows Form Designer.  
		'Do not modify it using the code editor.
		Friend WithEvents _messageBox As System.Windows.Forms.TextBox
		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
			Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(MessageBoxForm))
			Me._messageBox = New System.Windows.Forms.TextBox
			Me.SuspendLayout()
			'
			'_messageBox
			'
			Me._messageBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
									Or System.Windows.Forms.AnchorStyles.Left) _
									Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me._messageBox.Location = New System.Drawing.Point(0, 0)
			Me._messageBox.Multiline = True
			Me._messageBox.Name = "_messageBox"
			Me._messageBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
			Me._messageBox.Size = New System.Drawing.Size(292, 272)
			Me._messageBox.TabIndex = 0
			Me._messageBox.Text = ""
			'
			'MessageBoxForm
			'
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.ClientSize = New System.Drawing.Size(292, 273)
			Me.Controls.Add(Me._messageBox)
			Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
			Me.Name = "MessageBoxForm"
			Me.Text = "Status Bar Help"
			Me.ResumeLayout(False)

		End Sub

#End Region

	End Class
End Namespace