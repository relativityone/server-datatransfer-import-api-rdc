Public Class SetWebServiceURL
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
	Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
	Friend WithEvents Label1 As System.Windows.Forms.Label
	Friend WithEvents _WebServiceUrl As System.Windows.Forms.TextBox
	Friend WithEvents _okButton As System.Windows.Forms.Button
	Friend WithEvents _cancelButton As System.Windows.Forms.Button
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
		Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(SetWebServiceURL))
		Me.GroupBox1 = New System.Windows.Forms.GroupBox
		Me._WebServiceUrl = New System.Windows.Forms.TextBox
		Me.Label1 = New System.Windows.Forms.Label
		Me._okButton = New System.Windows.Forms.Button
		Me._cancelButton = New System.Windows.Forms.Button
		Me.GroupBox1.SuspendLayout()
		Me.SuspendLayout()
		'
		'GroupBox1
		'
		Me.GroupBox1.Controls.Add(Me._WebServiceUrl)
		Me.GroupBox1.Location = New System.Drawing.Point(8, 40)
		Me.GroupBox1.Name = "GroupBox1"
		Me.GroupBox1.Size = New System.Drawing.Size(424, 48)
		Me.GroupBox1.TabIndex = 0
		Me.GroupBox1.TabStop = False
		Me.GroupBox1.Text = "Web Service URL"
		'
		'_WebServiceUrl
		'
		Me._WebServiceUrl.Location = New System.Drawing.Point(8, 16)
		Me._WebServiceUrl.Name = "_WebServiceUrl"
		Me._WebServiceUrl.Size = New System.Drawing.Size(408, 20)
		Me._WebServiceUrl.TabIndex = 1
		Me._WebServiceUrl.Text = ""
		'
		'Label1
		'
		Me.Label1.Location = New System.Drawing.Point(8, 8)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(424, 16)
		Me.Label1.TabIndex = 0
		Me.Label1.Text = "The current WebServices URL could not be found. Please enter another"
		Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
		'
		'_okButton
		'
		Me._okButton.Location = New System.Drawing.Point(136, 96)
		Me._okButton.Name = "_okButton"
		Me._okButton.TabIndex = 2
		Me._okButton.Text = "OK"
		'
		'_cancelButton
		'
		Me._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
		Me._cancelButton.Location = New System.Drawing.Point(224, 96)
		Me._cancelButton.Name = "_cancelButton"
		Me._cancelButton.TabIndex = 3
		Me._cancelButton.Text = "Cancel"
		'
		'SetWebServiceURL
		'
		Me.AcceptButton = Me._okButton
		Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
		Me.CancelButton = Me._cancelButton
		Me.ClientSize = New System.Drawing.Size(440, 125)
		Me.Controls.Add(Me._cancelButton)
		Me.Controls.Add(Me._okButton)
		Me.Controls.Add(Me.Label1)
		Me.Controls.Add(Me.GroupBox1)
		Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
		Me.MaximizeBox = False
		Me.MinimizeBox = False
		Me.Name = "SetWebServiceURL"
		Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
		Me.Text = "Relativity Desktop Client | WebServices URL Not Found"
		Me.TopMost = True
		Me.GroupBox1.ResumeLayout(False)
		Me.ResumeLayout(False)

	End Sub

#End Region

	Private _required As Boolean
	Private _validInput As Boolean
	Friend WithEvents _application As kCura.EDDS.WinForm.Application

	Public Event ExitApplication()

	Public Property Required() As Boolean
		Get
			Return _required
		End Get
		Set(ByVal value As Boolean)
			_required = True
		End Set
	End Property

	Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
		_WebServiceUrl.Text = Config.WebServiceURL
	End Sub

	Private Sub _okButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _okButton.Click
		If _WebServiceUrl.Text <> String.Empty Then
			If Not _WebServiceUrl.Text.Chars(_WebServiceUrl.Text.Length - 1) = "/" Then
				_WebServiceUrl.Text &= "/"
			End If
			Config.WebServiceURL = _WebServiceUrl.Text
			_validInput = True
			Me.Close()
		End If
	End Sub

	Private Sub _cancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _cancelButton.Click
		Me.Close()
	End Sub

	Private Sub SetWebServiceURL_Closed(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Closed
		If Me.Required And Not _validInput Then
			RaiseEvent ExitApplication()
		End If
	End Sub
End Class