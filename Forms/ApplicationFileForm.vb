Namespace kCura.EDDS.WinForm
	Public Class ApplicationFileForm
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
		Friend WithEvents MainMenu1 As System.Windows.Forms.MainMenu
		Friend WithEvents GroupBox20 As System.Windows.Forms.GroupBox
		Friend WithEvents _browseButton As System.Windows.Forms.Button
		Friend WithEvents _filePath As System.Windows.Forms.TextBox
		Friend WithEvents MenuFile As System.Windows.Forms.MenuItem
		Friend WithEvents MenuImport As System.Windows.Forms.MenuItem
		Friend WithEvents MenuFile_Close As System.Windows.Forms.MenuItem
		Friend WithEvents MenuImport_ImportApplication As System.Windows.Forms.MenuItem
		Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
		Friend WithEvents Label1 As System.Windows.Forms.Label
		Friend WithEvents Label2 As System.Windows.Forms.Label
		Friend WithEvents ApplicationName As System.Windows.Forms.TextBox
		Friend WithEvents ApplicationVersion As System.Windows.Forms.TextBox
		Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
		Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
		Friend WithEvents Label3 As System.Windows.Forms.Label
		Friend WithEvents Label4 As System.Windows.Forms.Label
		Friend WithEvents TextBox2 As System.Windows.Forms.TextBox
		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
			Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(ApplicationFileForm))
			Me.MainMenu1 = New System.Windows.Forms.MainMenu
			Me.MenuFile = New System.Windows.Forms.MenuItem
			Me.MenuFile_Close = New System.Windows.Forms.MenuItem
			Me.MenuImport = New System.Windows.Forms.MenuItem
			Me.MenuImport_ImportApplication = New System.Windows.Forms.MenuItem
			Me.GroupBox20 = New System.Windows.Forms.GroupBox
			Me._browseButton = New System.Windows.Forms.Button
			Me._filePath = New System.Windows.Forms.TextBox
			Me.GroupBox1 = New System.Windows.Forms.GroupBox
			Me.ApplicationName = New System.Windows.Forms.TextBox
			Me.ApplicationVersion = New System.Windows.Forms.TextBox
			Me.Label2 = New System.Windows.Forms.Label
			Me.Label1 = New System.Windows.Forms.Label
			Me.GroupBox2 = New System.Windows.Forms.GroupBox
			Me.TextBox2 = New System.Windows.Forms.TextBox
			Me.Label4 = New System.Windows.Forms.Label
			Me.Label3 = New System.Windows.Forms.Label
			Me.TextBox1 = New System.Windows.Forms.TextBox
			Me.GroupBox20.SuspendLayout()
			Me.GroupBox1.SuspendLayout()
			Me.GroupBox2.SuspendLayout()
			Me.SuspendLayout()
			'
			'MainMenu1
			'
			Me.MainMenu1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuFile, Me.MenuImport})
			'
			'MenuFile
			'
			Me.MenuFile.Index = 0
			Me.MenuFile.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuFile_Close})
			Me.MenuFile.Text = "&File"
			'
			'MenuFile_Close
			'
			Me.MenuFile_Close.Index = 0
			Me.MenuFile_Close.Text = "&Close"
			'
			'MenuImport
			'
			Me.MenuImport.Index = 1
			Me.MenuImport.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuImport_ImportApplication})
			Me.MenuImport.Text = "&Import"
			'
			'MenuImport_ImportApplication
			'
			Me.MenuImport_ImportApplication.Index = 0
			Me.MenuImport_ImportApplication.Text = "Import &Application"
			'
			'GroupBox20
			'
			Me.GroupBox20.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
									Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.GroupBox20.Controls.Add(Me._browseButton)
			Me.GroupBox20.Controls.Add(Me._filePath)
			Me.GroupBox20.Location = New System.Drawing.Point(16, 24)
			Me.GroupBox20.Name = "GroupBox20"
			Me.GroupBox20.Size = New System.Drawing.Size(720, 48)
			Me.GroupBox20.TabIndex = 22
			Me.GroupBox20.TabStop = False
			Me.GroupBox20.Text = "Application File"
			'
			'_browseButton
			'
			Me._browseButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me._browseButton.Location = New System.Drawing.Point(688, 16)
			Me._browseButton.Name = "_browseButton"
			Me._browseButton.Size = New System.Drawing.Size(24, 20)
			Me._browseButton.TabIndex = 4
			Me._browseButton.Text = "..."
			'
			'_filePath
			'
			Me._filePath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
									Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me._filePath.BackColor = System.Drawing.SystemColors.ControlLightLight
			Me._filePath.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me._filePath.ForeColor = System.Drawing.SystemColors.ControlDarkDark
			Me._filePath.Location = New System.Drawing.Point(8, 16)
			Me._filePath.Name = "_filePath"
			Me._filePath.Size = New System.Drawing.Size(680, 20)
			Me._filePath.TabIndex = 2
			Me._filePath.Text = "Select a file ..."
			'
			'GroupBox1
			'
			Me.GroupBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
									Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.GroupBox1.Controls.Add(Me.ApplicationName)
			Me.GroupBox1.Controls.Add(Me.ApplicationVersion)
			Me.GroupBox1.Controls.Add(Me.Label2)
			Me.GroupBox1.Controls.Add(Me.Label1)
			Me.GroupBox1.Location = New System.Drawing.Point(16, 88)
			Me.GroupBox1.Name = "GroupBox1"
			Me.GroupBox1.Size = New System.Drawing.Size(720, 88)
			Me.GroupBox1.TabIndex = 23
			Me.GroupBox1.TabStop = False
			Me.GroupBox1.Text = "Application Information"
			'
			'ApplicationName
			'
			Me.ApplicationName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
									Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.ApplicationName.Enabled = False
			Me.ApplicationName.Location = New System.Drawing.Point(72, 24)
			Me.ApplicationName.Name = "ApplicationName"
			Me.ApplicationName.Size = New System.Drawing.Size(624, 20)
			Me.ApplicationName.TabIndex = 3
			Me.ApplicationName.Text = ""
			'
			'ApplicationVersion
			'
			Me.ApplicationVersion.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
									Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.ApplicationVersion.Enabled = False
			Me.ApplicationVersion.Location = New System.Drawing.Point(72, 56)
			Me.ApplicationVersion.Name = "ApplicationVersion"
			Me.ApplicationVersion.Size = New System.Drawing.Size(624, 20)
			Me.ApplicationVersion.TabIndex = 2
			Me.ApplicationVersion.Text = ""
			'
			'Label2
			'
			Me.Label2.Location = New System.Drawing.Point(16, 56)
			Me.Label2.Name = "Label2"
			Me.Label2.Size = New System.Drawing.Size(48, 23)
			Me.Label2.TabIndex = 1
			Me.Label2.Text = "Version"
			'
			'Label1
			'
			Me.Label1.Location = New System.Drawing.Point(16, 24)
			Me.Label1.Name = "Label1"
			Me.Label1.Size = New System.Drawing.Size(48, 23)
			Me.Label1.TabIndex = 0
			Me.Label1.Text = "Name:"
			'
			'GroupBox2
			'
			Me.GroupBox2.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
									Or System.Windows.Forms.AnchorStyles.Left) _
									Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.GroupBox2.Controls.Add(Me.TextBox2)
			Me.GroupBox2.Controls.Add(Me.Label4)
			Me.GroupBox2.Controls.Add(Me.Label3)
			Me.GroupBox2.Controls.Add(Me.TextBox1)
			Me.GroupBox2.Location = New System.Drawing.Point(16, 192)
			Me.GroupBox2.Name = "GroupBox2"
			Me.GroupBox2.Size = New System.Drawing.Size(720, 360)
			Me.GroupBox2.TabIndex = 24
			Me.GroupBox2.TabStop = False
			Me.GroupBox2.Text = "Application Artifacts"
			'
			'TextBox2
			'
			Me.TextBox2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
									Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.TextBox2.Enabled = False
			Me.TextBox2.Location = New System.Drawing.Point(72, 232)
			Me.TextBox2.Multiline = True
			Me.TextBox2.Name = "TextBox2"
			Me.TextBox2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
			Me.TextBox2.Size = New System.Drawing.Size(624, 104)
			Me.TextBox2.TabIndex = 3
			Me.TextBox2.Text = ""
			'
			'Label4
			'
			Me.Label4.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
			Me.Label4.Location = New System.Drawing.Point(16, 232)
			Me.Label4.Name = "Label4"
			Me.Label4.Size = New System.Drawing.Size(48, 23)
			Me.Label4.TabIndex = 2
			Me.Label4.Text = "Tabs:"
			'
			'Label3
			'
			Me.Label3.Location = New System.Drawing.Point(16, 32)
			Me.Label3.Name = "Label3"
			Me.Label3.Size = New System.Drawing.Size(48, 23)
			Me.Label3.TabIndex = 1
			Me.Label3.Text = "Objects:"
			'
			'TextBox1
			'
			Me.TextBox1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
									Or System.Windows.Forms.AnchorStyles.Left) _
									Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.TextBox1.Enabled = False
			Me.TextBox1.Location = New System.Drawing.Point(72, 32)
			Me.TextBox1.Multiline = True
			Me.TextBox1.Name = "TextBox1"
			Me.TextBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
			Me.TextBox1.Size = New System.Drawing.Size(624, 176)
			Me.TextBox1.TabIndex = 0
			Me.TextBox1.Text = ""
			'
			'ApplicationFileForm
			'
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.ClientSize = New System.Drawing.Size(752, 566)
			Me.Controls.Add(Me.GroupBox2)
			Me.Controls.Add(Me.GroupBox1)
			Me.Controls.Add(Me.GroupBox20)
			Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
			Me.Menu = Me.MainMenu1
			Me.MinimumSize = New System.Drawing.Size(760, 621)
			Me.Name = "ApplicationFileForm"
			Me.Text = "Relativity Desktop Client | Import Applicatioin File"
			Me.GroupBox20.ResumeLayout(False)
			Me.GroupBox1.ResumeLayout(False)
			Me.GroupBox2.ResumeLayout(False)
			Me.ResumeLayout(False)

		End Sub

#End Region

#Region " Event Handlers "

		Private Sub MenuFile_Close_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles MenuFile_Close.Click
			Me.Close()
		End Sub

		Private Sub MenuImport_ImportApplication_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles MenuImport_ImportApplication.Click
			MsgBox("Write this.")
		End Sub

#End Region

#Region " Friend Properties "

		Friend Property Application() As kCura.EDDS.WinForm.Application
			Get
				Return _application
			End Get
			Set(ByVal Value As kCura.EDDS.WinForm.Application)
				_application = Value
			End Set
		End Property

		Friend Property CaseInfo() As kCura.EDDS.Types.CaseInfo
			Get
				Return _caseInfo
			End Get
			Set(ByVal Value As kCura.EDDS.Types.CaseInfo)
				_caseInfo = Value
			End Set
		End Property

		Friend Property CookieContainer() As System.Net.CookieContainer
			Get
				Return _cookieContainer
			End Get
			Set(ByVal Value As System.Net.CookieContainer)
				_cookieContainer = Value
			End Set
		End Property

		Friend Property Credentials() As System.Net.NetworkCredential
			Get
				Return _credentials
			End Get
			Set(ByVal Value As System.Net.NetworkCredential)
				_credentials = Value
			End Set
		End Property

#End Region

#Region " Private Fields "

		Private WithEvents _application As kCura.EDDS.WinForm.Application
		Private _caseInfo As kCura.EDDS.Types.CaseInfo
		Private _cookieContainer As System.Net.CookieContainer
		Private _credentials As System.Net.NetworkCredential

#End Region

	End Class
End Namespace