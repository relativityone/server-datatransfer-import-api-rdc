Public Class TextDisplayForm
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "
	Private _application As kCura.EDDS.WinForm.Application

	Public Sub New()
		MyBase.New()

		'This call is required by the Windows Form Designer.
		InitializeComponent()
		'Add any initialization after the InitializeComponent() call
		_application = kCura.EDDS.WinForm.Application.Instance
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
	Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
		Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(TextDisplayForm))
		Me.TextBox1 = New System.Windows.Forms.TextBox
		Me.SuspendLayout()
		'
		'TextBox1
		'
		Me.TextBox1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
								Or System.Windows.Forms.AnchorStyles.Left) _
								Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.TextBox1.Location = New System.Drawing.Point(4, 4)
		Me.TextBox1.Multiline = True
		Me.TextBox1.Name = "TextBox1"
		Me.TextBox1.ReadOnly = True
		Me.TextBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both
		Me.TextBox1.Size = New System.Drawing.Size(340, 336)
		Me.TextBox1.TabIndex = 0
		Me.TextBox1.Text = ""
		Me.TextBox1.WordWrap = False
		'
		'TextDisplayForm
		'
		Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
		Me.ClientSize = New System.Drawing.Size(344, 341)
		Me.Controls.Add(Me.TextBox1)
		Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
		Me.Name = "TextDisplayForm"
		Me.Text = "TextDisplayForm"
		Me.ResumeLayout(False)

	End Sub

#End Region

	Public WithEvents ProcessObserver As kCura.Windows.Process.ProcessObserver

	Private Sub ProcessObserver_OnProcessEvent(ByVal evt As kCura.Windows.Process.ProcessEvent) Handles ProcessObserver.OnProcessEvent
		_application.CursorWait()
		Me.AppendText(evt.Message & vbNewLine)
		_application.CursorDefault()
	End Sub

	Delegate Sub AppendTextCallback(ByVal [text] As String)

	Private Sub AppendText(ByVal t As String)
		If TextBox1.InvokeRequired Then
			Dim d As New AppendTextCallback(AddressOf AppendText)
			Me.Invoke(d, New Object() {t})
		Else
			TextBox1.Text &= t
		End If
	End Sub

	Private Sub TextDisplayForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

	End Sub


	Private Sub TextDisplayForm_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TextBox1.KeyDown
		If e.Control AndAlso e.KeyCode = Keys.A Then
			TextBox1.SelectAll()
		End If
	End Sub
End Class
