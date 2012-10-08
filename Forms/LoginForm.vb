Namespace kCura.EDDS.WinForm
	Public Class LoginForm
		Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

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
		Friend WithEvents Label1 As System.Windows.Forms.Label
		Friend WithEvents Label2 As System.Windows.Forms.Label
		Friend WithEvents _okButton As System.Windows.Forms.Button
		Friend WithEvents _cancelButton As System.Windows.Forms.Button
		Friend WithEvents _loginTextBox As System.Windows.Forms.TextBox
		Friend WithEvents _passwordTextBox As System.Windows.Forms.TextBox
		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
			Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(LoginForm))
			Me._loginTextBox = New System.Windows.Forms.TextBox
			Me.Label1 = New System.Windows.Forms.Label
			Me._passwordTextBox = New System.Windows.Forms.TextBox
			Me.Label2 = New System.Windows.Forms.Label
			Me._okButton = New System.Windows.Forms.Button
			Me._cancelButton = New System.Windows.Forms.Button
			Me.SuspendLayout()
			'
			'_loginTextBox
			'
			Me._loginTextBox.Location = New System.Drawing.Point(8, 24)
			Me._loginTextBox.Name = "_loginTextBox"
			Me._loginTextBox.Size = New System.Drawing.Size(240, 20)
			Me._loginTextBox.TabIndex = 0


			'
			'Label1
			'
			Me.Label1.Location = New System.Drawing.Point(8, 8)
			Me.Label1.Name = "Label1"
			Me.Label1.Size = New System.Drawing.Size(100, 16)
			Me.Label1.TabIndex = 1
			Me.Label1.Text = "Login"
			'
			'_passwordTextBox
			'
			Me._passwordTextBox.Location = New System.Drawing.Point(8, 72)
			Me._passwordTextBox.Name = "_passwordTextBox"
			Me._passwordTextBox.PasswordChar = Global.Microsoft.VisualBasic.ChrW(8226)
			Me._passwordTextBox.Size = New System.Drawing.Size(240, 20)
			Me._passwordTextBox.TabIndex = 2

			'
			'Label2
			'
			Me.Label2.Location = New System.Drawing.Point(8, 56)
			Me.Label2.Name = "Label2"
			Me.Label2.Size = New System.Drawing.Size(100, 16)
			Me.Label2.TabIndex = 3
			Me.Label2.Text = "Password"
			'
			'_okButton
			'
			Me._okButton.Location = New System.Drawing.Point(88, 120)
			Me._okButton.Name = "_okButton"
			Me._okButton.Size = New System.Drawing.Size(75, 23)
			Me._okButton.TabIndex = 4
			Me._okButton.Text = "OK"
			'
			'_cancelButton
			'
			Me._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me._cancelButton.Location = New System.Drawing.Point(168, 120)
			Me._cancelButton.Name = "_cancelButton"
			Me._cancelButton.Size = New System.Drawing.Size(75, 23)
			Me._cancelButton.TabIndex = 5
			Me._cancelButton.Text = "Cancel"
			'
			'LoginForm
			'
			Me.AcceptButton = Me._okButton
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.CancelButton = Me._cancelButton
			Me.ClientSize = New System.Drawing.Size(257, 158)
			Me.Controls.Add(Me._cancelButton)
			Me.Controls.Add(Me._okButton)
			Me.Controls.Add(Me.Label2)
			Me.Controls.Add(Me._passwordTextBox)
			Me.Controls.Add(Me._loginTextBox)
			Me.Controls.Add(Me.Label1)
			Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
			Me.MaximizeBox = False
			Me.MaximumSize = New System.Drawing.Size(265, 185)
			Me.MinimizeBox = False
			Me.MinimumSize = New System.Drawing.Size(265, 185)
			Me.Name = "LoginForm"
			Me.ShowInTaskbar = False
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
			Me.Text = "Relativity Desktop Client | Login"
			Me.TopMost = True
			Me.ResumeLayout(False)
			Me.PerformLayout()

		End Sub

#End Region

#If DEBUG Then
		Private _timer As System.Threading.Timer
		Private Delegate Sub AutoLoginDelegateSub()
		Private _autoLoginDelegate As AutoLoginDelegateSub
#End If

		Friend WithEvents _application As kCura.EDDS.WinForm.Application

		Private _credential As System.Net.NetworkCredential
		Private _openCaseSelector As Boolean = True

		Public Property Credential() As System.Net.NetworkCredential
			Get
				Return _credential
			End Get
			Set(ByVal value As System.Net.NetworkCredential)
				_credential = value
			End Set
		End Property

		Public Property OpenCaseSelector() As Boolean
			Get
				Return _openCaseSelector
			End Get
			Set(ByVal value As Boolean)
				_openCaseSelector = value
			End Set
		End Property

		'Private Sub _okButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _okButton.Click
		'	If _loginTextBox.Text.IndexOf("\") = -1 Then
		'		_credential = New Net.NetworkCredential(_loginTextBox.Text, _passwordTextBox.Text)
		'	Else
		'		_credential = New Net.NetworkCredential(_loginTextBox.Text.Split("\".ToCharArray)(1), _passwordTextBox.Text, _loginTextBox.Text.Split("\".ToCharArray)(0))
		'	End If
		'	RaiseEvent OK_Click(_credential)
		'End Sub

		Private Sub _okButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _okButton.Click
			_credential = New Net.NetworkCredential(_loginTextBox.Text, _passwordTextBox.Text)
			RaiseEvent OK_Click(_credential, _openCaseSelector)
		End Sub

		Public Event OK_Click(ByVal cred As Net.NetworkCredential, ByVal openCaseSelector As Boolean)

		Private Sub _cancelButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _cancelButton.Click
			_application.ExitApplication()
		End Sub


		Private Sub LoginForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
			Me.Focus()
			_loginTextBox.Focus()
		End Sub


	End Class
End Namespace
