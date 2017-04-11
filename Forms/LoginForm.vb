Imports System.Security.AccessControl
Imports System.Threading.Tasks
Imports kCura.WinEDDS.Credentials
Imports Relativity.OAuth2Client.Implicit.RelativityWebBrowser

Namespace kCura.EDDS.WinForm
	Public Class LoginForm
		Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

		Public Sub New()
			MyBase.New()

			'This call is required by the Windows Form Designer.
			InitializeComponent()

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
		Private _browser As RelativityWebBrowser
		'NOTE: The following procedure is required by the Windows Form Designer
		'It can be modified using the Windows Form Designer.  
		'Do not modify it using the code editor.
		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
			Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(LoginForm))
			Me.SuspendLayout()
			
			Me._browser = New RelativityWebBrowser()
			Me._browser.AllowNavigation = True
			Me._browser.Dock = DockStyle.Fill
			Me.Controls.Add(_browser)

			'
			'LoginForm
			'
			Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
			Me.Size = new System.Drawing.Size(600, 600)
			Me.Name = "LoginForm"
			Me.ShowInTaskbar = False
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
			Me.Text = "Relativity Desktop Client | Login"
			Me.TopMost = True
			Me.ResumeLayout(False)
			Me.PerformLayout()
		End Sub

#End Region

		Public ReadOnly Property Browser() As RelativityWebBrowser
			Get
				return _browser
			End Get
		End Property

		Private Sub LoginForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
			Me.Focus()
			_browser.Show()
			_browser.AllowNavigation = True
		End Sub

		Private Async Sub LoginForm_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Shown
			Try
				await RelativityWebApiCredentialsProvider.Instance().GetCredentialsAsync()
			Catch ex As TaskCanceledException
				'Login form was closed, ignore
			End Try
			
		End Sub

		 Private Sub LoginForm_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
			RelativityWebApiCredentialsProvider.Instance().Cancel()
		 End Sub

	End Class
End Namespace
