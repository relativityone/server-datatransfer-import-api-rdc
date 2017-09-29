Imports System.Net
Imports System.Threading.Tasks
Imports Relativity.OAuth2Client.Exceptions

Namespace kCura.EDDS.WinForm
	Public Class MainForm
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
		Friend WithEvents MenuItem1 As System.Windows.Forms.MenuItem
		Friend WithEvents MenuItem3 As System.Windows.Forms.MenuItem
		Friend WithEvents OpenRepositoryMenu As System.Windows.Forms.MenuItem
		Friend WithEvents MenuItem2 As System.Windows.Forms.MenuItem
		Friend WithEvents ToolsImportImageFileMenu As System.Windows.Forms.MenuItem
		Friend WithEvents StatusBar As System.Windows.Forms.StatusBar
		Friend WithEvents AppStatusPanel As System.Windows.Forms.StatusBarPanel
		Friend WithEvents LoggedInUserPanel As System.Windows.Forms.StatusBarPanel
		Friend WithEvents MainMenu As System.Windows.Forms.MainMenu
		Friend WithEvents _caseFolderExplorer As kCura.EDDS.WinForm.CaseFolderExplorer
		Friend WithEvents ExitMenu As System.Windows.Forms.MenuItem
		Friend WithEvents EnhancedMenuProvider As kCura.Windows.Forms.EnhancedMenuProvider
		Friend WithEvents ImportMenu As System.Windows.Forms.MenuItem
		Friend WithEvents ToolsImportLoadFileMenu As System.Windows.Forms.MenuItem
		Friend WithEvents ExportMenu As System.Windows.Forms.MenuItem
		Friend WithEvents _toolsMenu As System.Windows.Forms.MenuItem
		Friend WithEvents _toolsMenuSettingsItem As System.Windows.Forms.MenuItem
		Friend WithEvents ToolsExportProductionMenu As System.Windows.Forms.MenuItem
		Friend WithEvents ToolsExportSearchMenu As System.Windows.Forms.MenuItem
		Friend WithEvents _fileMenuRefresh As System.Windows.Forms.MenuItem
		Friend WithEvents MenuItem4 As System.Windows.Forms.MenuItem
		Friend WithEvents _aboutMenuItem As System.Windows.Forms.MenuItem
		Friend WithEvents _helpMenuItem As System.Windows.Forms.MenuItem
		Friend WithEvents _exportFoldersMenuItem As System.Windows.Forms.MenuItem
		Friend WithEvents _exportFoldersAndSubfoldersMenuItem As System.Windows.Forms.MenuItem
		Friend WithEvents ToolsImportProductionFileMenu As System.Windows.Forms.MenuItem
		Friend WithEvents _objectTypeDropDown As System.Windows.Forms.ComboBox
		Friend WithEvents _optionsMenuCheckConnectivityItem As System.Windows.Forms.MenuItem
		Friend WithEvents TransferMenu As MenuItem
		Friend WithEvents _exportObjectsMenuItem As System.Windows.Forms.MenuItem
		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
		Me.components = New System.ComponentModel.Container()
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
		Me.MainMenu = New System.Windows.Forms.MainMenu(Me.components)
		Me.MenuItem1 = New System.Windows.Forms.MenuItem()
		Me.OpenRepositoryMenu = New System.Windows.Forms.MenuItem()
		Me.MenuItem3 = New System.Windows.Forms.MenuItem()
		Me.ExitMenu = New System.Windows.Forms.MenuItem()
		Me._fileMenuRefresh = New System.Windows.Forms.MenuItem()
		Me.MenuItem2 = New System.Windows.Forms.MenuItem()
		Me.ImportMenu = New System.Windows.Forms.MenuItem()
		Me.ToolsImportImageFileMenu = New System.Windows.Forms.MenuItem()
		Me.ToolsImportLoadFileMenu = New System.Windows.Forms.MenuItem()
		Me.ToolsImportProductionFileMenu = New System.Windows.Forms.MenuItem()
		Me.ExportMenu = New System.Windows.Forms.MenuItem()
		Me.ToolsExportProductionMenu = New System.Windows.Forms.MenuItem()
		Me.ToolsExportSearchMenu = New System.Windows.Forms.MenuItem()
		Me._exportFoldersMenuItem = New System.Windows.Forms.MenuItem()
		Me._exportFoldersAndSubfoldersMenuItem = New System.Windows.Forms.MenuItem()
		Me._exportObjectsMenuItem = New System.Windows.Forms.MenuItem()
		Me.TransferMenu = New System.Windows.Forms.MenuItem()
		Me._toolsMenu = New System.Windows.Forms.MenuItem()
		Me._toolsMenuSettingsItem = New System.Windows.Forms.MenuItem()
		Me._optionsMenuCheckConnectivityItem = New System.Windows.Forms.MenuItem()
		Me.MenuItem4 = New System.Windows.Forms.MenuItem()
		Me._aboutMenuItem = New System.Windows.Forms.MenuItem()
		Me._helpMenuItem = New System.Windows.Forms.MenuItem()
		Me.StatusBar = New System.Windows.Forms.StatusBar()
		Me.AppStatusPanel = New System.Windows.Forms.StatusBarPanel()
		Me.LoggedInUserPanel = New System.Windows.Forms.StatusBarPanel()
		Me._objectTypeDropDown = New System.Windows.Forms.ComboBox()
		Me._caseFolderExplorer = New kCura.EDDS.WinForm.CaseFolderExplorer()
		Me.EnhancedMenuProvider = New kCura.Windows.Forms.EnhancedMenuProvider(Me.components)
		CType(Me.AppStatusPanel,System.ComponentModel.ISupportInitialize).BeginInit
		CType(Me.LoggedInUserPanel,System.ComponentModel.ISupportInitialize).BeginInit
		Me.SuspendLayout
		'
		'MainMenu
		'
		Me.MainMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuItem1, Me.MenuItem2, Me._toolsMenu, Me.MenuItem4})
		'
		'MenuItem1
		'
		Me.EnhancedMenuProvider.SetImageIndex(Me.MenuItem1, -1)
		Me.MenuItem1.Index = 0
		Me.MenuItem1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.OpenRepositoryMenu, Me.MenuItem3, Me.ExitMenu, Me._fileMenuRefresh})
		Me.MenuItem1.OwnerDraw = true
		Me.MenuItem1.Text = "&File"
		'
		'OpenRepositoryMenu
		'
		Me.EnhancedMenuProvider.SetImageIndex(Me.OpenRepositoryMenu, -1)
		Me.OpenRepositoryMenu.Index = 0
		Me.OpenRepositoryMenu.OwnerDraw = true
		Me.OpenRepositoryMenu.Text = "&Open..."
		'
		'MenuItem3
		'
		Me.EnhancedMenuProvider.SetImageIndex(Me.MenuItem3, -1)
		Me.MenuItem3.Index = 1
		Me.MenuItem3.OwnerDraw = true
		Me.MenuItem3.Text = "-"
		'
		'ExitMenu
		'
		Me.EnhancedMenuProvider.SetImageIndex(Me.ExitMenu, -1)
		Me.ExitMenu.Index = 2
		Me.ExitMenu.OwnerDraw = true
		Me.ExitMenu.Text = "&Exit"
		'
		'_fileMenuRefresh
		'
		Me._fileMenuRefresh.Enabled = false
		Me.EnhancedMenuProvider.SetImageIndex(Me._fileMenuRefresh, -1)
		Me._fileMenuRefresh.Index = 3
		Me._fileMenuRefresh.OwnerDraw = true
		Me._fileMenuRefresh.Shortcut = System.Windows.Forms.Shortcut.F5
		Me._fileMenuRefresh.Text = "Refresh"
		'
		'MenuItem2
		'
		Me.EnhancedMenuProvider.SetImageIndex(Me.MenuItem2, -1)
		Me.MenuItem2.Index = 1
		Me.MenuItem2.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.ImportMenu, Me.ExportMenu, Me.TransferMenu})
		Me.MenuItem2.OwnerDraw = true
		Me.MenuItem2.Text = "&Tools"
		'
		'ImportMenu
		'
		Me.ImportMenu.Enabled = false
		Me.EnhancedMenuProvider.SetImageIndex(Me.ImportMenu, -1)
		Me.ImportMenu.Index = 0
		Me.ImportMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.ToolsImportImageFileMenu, Me.ToolsImportLoadFileMenu, Me.ToolsImportProductionFileMenu})
		Me.ImportMenu.OwnerDraw = true
		Me.ImportMenu.Text = "&Import"
		'
		'ToolsImportImageFileMenu
		'
		Me.EnhancedMenuProvider.SetImageIndex(Me.ToolsImportImageFileMenu, -1)
		Me.ToolsImportImageFileMenu.Index = 0
		Me.ToolsImportImageFileMenu.OwnerDraw = true
		Me.ToolsImportImageFileMenu.Shortcut = System.Windows.Forms.Shortcut.CtrlI
		Me.ToolsImportImageFileMenu.Text = "&Image Load File..."
		'
		'ToolsImportLoadFileMenu
		'
		Me.EnhancedMenuProvider.SetImageIndex(Me.ToolsImportLoadFileMenu, -1)
		Me.ToolsImportLoadFileMenu.Index = 1
		Me.ToolsImportLoadFileMenu.OwnerDraw = true
		Me.ToolsImportLoadFileMenu.Shortcut = System.Windows.Forms.Shortcut.CtrlL
		Me.ToolsImportLoadFileMenu.Text = "&Load File..."
		'
		'ToolsImportProductionFileMenu
		'
		Me.EnhancedMenuProvider.SetImageIndex(Me.ToolsImportProductionFileMenu, -1)
		Me.ToolsImportProductionFileMenu.Index = 2
		Me.ToolsImportProductionFileMenu.OwnerDraw = true
		Me.ToolsImportProductionFileMenu.Shortcut = System.Windows.Forms.Shortcut.CtrlP
		Me.ToolsImportProductionFileMenu.Text = "Production Load File..."
		'
		'ExportMenu
		'
		Me.ExportMenu.Enabled = false
		Me.EnhancedMenuProvider.SetImageIndex(Me.ExportMenu, -1)
		Me.ExportMenu.Index = 1
		Me.ExportMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.ToolsExportProductionMenu, Me.ToolsExportSearchMenu, Me._exportFoldersMenuItem, Me._exportFoldersAndSubfoldersMenuItem, Me._exportObjectsMenuItem})
		Me.ExportMenu.OwnerDraw = true
		Me.ExportMenu.Text = "&Export"
		'
		'ToolsExportProductionMenu
		'
		Me.EnhancedMenuProvider.SetImageIndex(Me.ToolsExportProductionMenu, -1)
		Me.ToolsExportProductionMenu.Index = 0
		Me.ToolsExportProductionMenu.OwnerDraw = true
		Me.ToolsExportProductionMenu.Text = "Production Set..."
		'
		'ToolsExportSearchMenu
		'
		Me.EnhancedMenuProvider.SetImageIndex(Me.ToolsExportSearchMenu, -1)
		Me.ToolsExportSearchMenu.Index = 1
		Me.ToolsExportSearchMenu.OwnerDraw = true
		Me.ToolsExportSearchMenu.Text = "Saved Search..."
		'
		'_exportFoldersMenuItem
		'
		Me.EnhancedMenuProvider.SetImageIndex(Me._exportFoldersMenuItem, -1)
		Me._exportFoldersMenuItem.Index = 2
		Me._exportFoldersMenuItem.OwnerDraw = true
		Me._exportFoldersMenuItem.Text = "Folder..."
		'
		'_exportFoldersAndSubfoldersMenuItem
		'
		Me.EnhancedMenuProvider.SetImageIndex(Me._exportFoldersAndSubfoldersMenuItem, -1)
		Me._exportFoldersAndSubfoldersMenuItem.Index = 3
		Me._exportFoldersAndSubfoldersMenuItem.OwnerDraw = true
		Me._exportFoldersAndSubfoldersMenuItem.Text = "Folder and Subfolders..."
		'
		'_exportObjectsMenuItem
		'
		Me.EnhancedMenuProvider.SetImageIndex(Me._exportObjectsMenuItem, -1)
		Me._exportObjectsMenuItem.Index = 4
		Me._exportObjectsMenuItem.OwnerDraw = true
		Me._exportObjectsMenuItem.Text = "Objects"
		Me._exportObjectsMenuItem.Visible = false
		'
		'TransferMenu
		'
		Me.TransferMenu.Enabled = false
		Me.EnhancedMenuProvider.SetImageIndex(Me.TransferMenu, -1)
		Me.TransferMenu.Index = 2
		Me.TransferMenu.OwnerDraw = true
		Me.TransferMenu.Text = "Staging Explorer..."
		Me.TransferMenu.Visible = false
		'
		'_toolsMenu
		'
		Me.EnhancedMenuProvider.SetImageIndex(Me._toolsMenu, -1)
		Me._toolsMenu.Index = 2
		Me._toolsMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me._toolsMenuSettingsItem, Me._optionsMenuCheckConnectivityItem})
		Me._toolsMenu.OwnerDraw = true
		Me._toolsMenu.Text = "Options"
		'
		'_toolsMenuSettingsItem
		'
		Me.EnhancedMenuProvider.SetImageIndex(Me._toolsMenuSettingsItem, -1)
		Me._toolsMenuSettingsItem.Index = 0
		Me._toolsMenuSettingsItem.OwnerDraw = true
		Me._toolsMenuSettingsItem.Text = "Settings..."
		'
		'_optionsMenuCheckConnectivityItem
		'
		Me._optionsMenuCheckConnectivityItem.Enabled = false
		Me.EnhancedMenuProvider.SetImageIndex(Me._optionsMenuCheckConnectivityItem, -1)
		Me._optionsMenuCheckConnectivityItem.Index = 1
		Me._optionsMenuCheckConnectivityItem.OwnerDraw = true
		Me._optionsMenuCheckConnectivityItem.Text = "Check Connectivity..."
		'
		'MenuItem4
		'
		Me.EnhancedMenuProvider.SetImageIndex(Me.MenuItem4, -1)
		Me.MenuItem4.Index = 3
		Me.MenuItem4.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me._aboutMenuItem, Me._helpMenuItem})
		Me.MenuItem4.OwnerDraw = true
		Me.MenuItem4.Text = "Help"
		'
		'_aboutMenuItem
		'
		Me.EnhancedMenuProvider.SetImageIndex(Me._aboutMenuItem, -1)
		Me._aboutMenuItem.Index = 0
		Me._aboutMenuItem.OwnerDraw = true
		Me._aboutMenuItem.Text = "About..."
		'
		'_helpMenuItem
		'
		Me._helpMenuItem.Enabled = false
		Me.EnhancedMenuProvider.SetImageIndex(Me._helpMenuItem, -1)
		Me._helpMenuItem.Index = 1
		Me._helpMenuItem.OwnerDraw = true
		Me._helpMenuItem.Text = "Help"
		'
		'StatusBar
		'
		Me.StatusBar.Location = New System.Drawing.Point(0, 284)
		Me.StatusBar.Name = "StatusBar"
		Me.StatusBar.Panels.AddRange(New System.Windows.Forms.StatusBarPanel() {Me.AppStatusPanel, Me.LoggedInUserPanel})
		Me.StatusBar.ShowPanels = true
		Me.StatusBar.Size = New System.Drawing.Size(332, 22)
		Me.StatusBar.TabIndex = 5
		'
		'AppStatusPanel
		'
		Me.AppStatusPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring
		Me.AppStatusPanel.Name = "AppStatusPanel"
		Me.AppStatusPanel.Width = 215
		'
		'LoggedInUserPanel
		'
		Me.LoggedInUserPanel.Alignment = System.Windows.Forms.HorizontalAlignment.Right
		Me.LoggedInUserPanel.Name = "LoggedInUserPanel"
		'
		'_objectTypeDropDown
		'
		Me._objectTypeDropDown.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
			Or System.Windows.Forms.AnchorStyles.Left)  _
			Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me._objectTypeDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._objectTypeDropDown.Location = New System.Drawing.Point(0, 4)
		Me._objectTypeDropDown.Name = "_objectTypeDropDown"
		Me._objectTypeDropDown.Size = New System.Drawing.Size(332, 21)
		Me._objectTypeDropDown.TabIndex = 7
		'
		'_caseFolderExplorer
		'
		Me._caseFolderExplorer.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
			Or System.Windows.Forms.AnchorStyles.Left)  _
			Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me._caseFolderExplorer.Location = New System.Drawing.Point(0, 25)
		Me._caseFolderExplorer.Name = "_caseFolderExplorer"
		Me._caseFolderExplorer.Size = New System.Drawing.Size(332, 259)
		Me._caseFolderExplorer.TabIndex = 6
		'
		'MainForm
		'
		Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
		Me.ClientSize = New System.Drawing.Size(332, 306)
		Me.Controls.Add(Me._objectTypeDropDown)
		Me.Controls.Add(Me._caseFolderExplorer)
		Me.Controls.Add(Me.StatusBar)
		Me.Icon = CType(resources.GetObject("$this.Icon"),System.Drawing.Icon)
		Me.Menu = Me.MainMenu
		Me.Name = "MainForm"
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
		Me.Text = "Relativity Desktop Client"
		CType(Me.AppStatusPanel,System.ComponentModel.ISupportInitialize).EndInit
		CType(Me.LoggedInUserPanel,System.ComponentModel.ISupportInitialize).EndInit
		Me.ResumeLayout(false)

End Sub

#End Region

		'' Private WithEvents _optionsForm As OptionsForm = Nothing
		Private firstTime As Boolean = True
		Friend WithEvents _application As kCura.EDDS.WinForm.Application
		Public Const MAX_LENGTH_OF_OBJECT_NAME_BEFORE_TRUNCATION As Int32 = 25

		Private Async Sub OpenRepositoryMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenRepositoryMenu.Click
			try
				If _application.LastCredentialCheckResult = Application.CredentialCheckResult.AccessDisabled Then
					'The user could have changed the server, so we need to check default credentials again.
					Dim defaultCredentialResult As Application.CredentialCheckResult = _application.AttemptWindowsAuthentication()
					If defaultCredentialResult = Application.CredentialCheckResult.AccessDisabled Then
						MessageBox.Show(Application.ACCESS_DISABLED_MESSAGE, Application.RDC_ERROR_TITLE)
					ElseIf Not defaultCredentialResult = Application.CredentialCheckResult.Success Then
					
						_application.NewLogin()
					Else
						_application.OpenCaseSelector = False
						Await _application.GetCredentialsAsync()
						_application.LogOn()
						Await _application.OpenCase()
						kCura.Windows.Forms.EnhancedMenuProvider.Hook(Me)
					End If
				Else
					_application.OpenCaseSelector = False
					Await _application.GetCredentialsAsync()
					Await _application.OpenCase()
				End If
			Catch ex As LoginCanceledException
				'user close the login window, do nothing
			End Try
			
		End Sub

		Private Sub ExitMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitMenu.Click
			_application.ExitApplication()
		End Sub

		Private Sub _application_OnEvent(ByVal appEvent As AppEvent) Handles _application.OnEvent
			ME.Invoke(Async Sub() Await HandleEventOnUiThread(appEvent))
		End Sub

		Private Async Function HandleEventOnUiThread(appEvent As AppEvent) As Task

			Select Case appEvent.EventType
				Case appEvent.AppEventType.LoadCase
					_fileMenuRefresh.Enabled = True
					UpdateStatus("Workspace Loaded - File Transfer Mode: Connecting...")
					Dim mode = Await _application.GetConnectionStatus
					UpdateStatus($"Workspace Loaded - File Transfer Mode: {mode}")
					Await PopulateObjectTypeDropDown()
					_optionsMenuCheckConnectivityItem.Enabled = True
					ImportMenu.Enabled = _application.UserHasImportPermission
					ExportMenu.Enabled = _application.UserHasExportPermission
					ImportMenu.Visible = _application.UserHasImportPermission
					ExportMenu.Visible = _application.UserHasExportPermission
					_application.OpenCaseSelector = false
				Case AppEvent.AppEventType.LogOnForm
					'Enable help once logged into Relativity via RDC login form
					Me._helpMenuItem.Enabled = True
				Case appEvent.AppEventType.LogOn
					UpdateUserName(_application.LoggedInUser)
					'Enable help once logged into Relativity via Windows Authentication
					Me._helpMenuItem.Enabled = True
				Case appEvent.AppEventType.ExitApplication
					Me.Close()
				Case appEvent.AppEventType.WorkspaceFolderSelected
					'disable import and export menus if no permission
					TransferMenu.Enabled = True
					ImportMenu.Enabled = _application.UserHasImportPermission
					ExportMenu.Enabled = _application.UserHasExportPermission
					ImportMenu.Visible = _application.UserHasImportPermission
					ExportMenu.Visible = _application.UserHasExportPermission
					'UpdateStatus("Case Folder Load: " + _application.SelectedCaseInfo.RootFolderID.ToString)
				Case appEvent.AppEventType.LogOnRequested
					Await _application.AttemptLogin(me)
			End Select
		End Function

		Private Sub UpdateStatus(ByVal text As String)
			AppStatusPanel.Text = text
		End Sub

		Private Sub UpdateUserName(ByVal text As String)
			LoggedInUserPanel.Text = text
		End Sub

		Private Async Sub MainForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

			ServicePointManager.DefaultConnectionLimit = Environment.ProcessorCount * 12

			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			_application.TemporaryForceFolderPreview = kCura.WinEDDS.Config.ForceFolderPreview
			If kCura.WinEDDS.Config.WebServiceURL = String.Empty Then
				_application.SetWebServiceURL()
			End If

			'' Can't do this in Application.vb without refactoring AttemptLogin (which needs this form as a parameter)
			Await CheckCertificate()

			Dim isCloudInstance  = Await _application.GetIsCloudInstance()
			Dim isStagingExplorerEnabled = Await _application.GetIsStagingExplorerEnabled()

			Me.TransferMenu.Visible = isCloudInstance And isStagingExplorerEnabled
			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Sub

		Public Async Function CheckCertificate() As Task
			try
				If (_application.CertificateTrusted()) Then
					Await _application.AttemptLogin(Me)
				Else
					_application.CertificateCheckPrompt()
				End If
			Catch ex As WebException
				_application.HandleWebException(ex)
			End Try
			
		End Function

		Private Async Sub WebServiceURLChanged() Handles _application.ReCheckCertificate
			'Disable help since user will be asked to login again
			Me._helpMenuItem.Enabled = False
			Await CheckCertificate()
			Me.TransferMenu.Visible  = Await _application.GetIsCloudInstance()
		End Sub

		Private Async Sub MainForm_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
			_application.UpdateForceFolderPreview()
			_application.UpdateWebServiceURL(False)
			Await _application.Logout()
			kCura.Windows.Forms.EnhancedMenuProvider.Unhook()
		End Sub

		Private Async Sub ToolsImportImageFileMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolsImportImageFileMenu.Click
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			Await _application.NewImageFile(_application.SelectedCaseFolderID, _application.SelectedCaseInfo)
			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Sub

		Private Async Sub ToolsImportProductionFileMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolsImportProductionFileMenu.Click
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			Await _application.NewProductionFile(_application.SelectedCaseFolderID, _application.SelectedCaseInfo)
			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Sub

		Private Async Sub ToolsImportLoadFileMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolsImportLoadFileMenu.Click
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			Await _application.NewLoadFile(_application.SelectedCaseFolderID, _application.SelectedCaseInfo)
			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Sub

		Private Async Sub ToolsExportProductionMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolsExportProductionMenu.Click
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			Await _application.NewProductionExport(_application.SelectedCaseInfo)
			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Sub

		Private Async Sub ToolsExportSearchMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolsExportSearchMenu.Click
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			Await _application.NewSearchExport(_application.SelectedCaseInfo.RootFolderID, _application.SelectedCaseInfo, kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch)
			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Sub

		Private Sub ToolsTransferFileMenu_Click(sender As Object, e As EventArgs) Handles TransferMenu.Click
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			_application.NewFileTransfer()
			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Sub

		Private Async Sub _exportObjectsMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _exportObjectsMenuItem.Click
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			Await _application.NewSearchExport(_application.SelectedCaseInfo.RootFolderID, _application.SelectedCaseInfo, kCura.WinEDDS.ExportFile.ExportType.AncestorSearch)
			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Sub

		Private Sub _toolsMenuSettingsItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _toolsMenuSettingsItem.Click
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			_application.NewOptions()
			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Sub

		Private Async Sub _fileMenuRefresh_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _fileMenuRefresh.Click
			_application.UpdateWebServiceURL(False)
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			Await _application.RefreshCaseFolders()
			Await _application.RefreshSelectedCaseInfo()
			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Sub

		Private Sub _aboutMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _aboutMenuItem.Click
			_application.DoAbout()
		End Sub

		Private Async Sub _helpMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _helpMenuItem.Click
			Await _application.DoHelp()
		End Sub

		Private Sub MainForm_Closed(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Closed
			System.Environment.Exit(0)
		End Sub

		Private Async Sub _exportFoldersMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _exportFoldersMenuItem.Click
			Await _application.NewSearchExport(_application.SelectedCaseFolderID, _application.SelectedCaseInfo, ExportFile.ExportType.ParentSearch)
		End Sub

		Private Async Sub _exportFoldersAndSubfoldersMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _exportFoldersAndSubfoldersMenuItem.Click
			Await _application.NewSearchExport(_application.SelectedCaseFolderID, _application.SelectedCaseInfo, ExportFile.ExportType.AncestorSearch)
		End Sub

		Private Async Function PopulateObjectTypeDropDown() As Task
			Dim objectTypeManager As New kCura.WinEDDS.Service.ObjectTypeManager(Await _application.GetCredentialsAsync(), _application.CookieContainer)
			Dim uploadableObjectTypes As System.Data.DataRowCollection = objectTypeManager.RetrieveAllUploadable(_application.SelectedCaseInfo.ArtifactID).Tables(0).Rows
			Dim selectedObjectTypeID As Int32 = Relativity.ArtifactType.Document
			If _objectTypeDropDown.Items.Count > 0 Then
				selectedObjectTypeID = DirectCast(_objectTypeDropDown.SelectedItem, kCura.WinEDDS.ObjectTypeListItem).Value
			End If
			_objectTypeDropDown.Items.Clear()
			For Each objectType As System.Data.DataRow In uploadableObjectTypes
				Dim currentObjectType As New kCura.WinEDDS.ObjectTypeListItem(CType(objectType("DescriptorArtifactTypeID"), Int32), CType(objectType("Name"), String), CType(objectType("HasAddPermission"), Boolean))
				_objectTypeDropDown.Items.Add(currentObjectType)
				If CType(objectType("DescriptorArtifactTypeID"), Int32) = selectedObjectTypeID Then
					Me._objectTypeDropDown.SelectedItem = currentObjectType
				End If
			Next
		End Function

		Private Function TruncateTextWithEllipses(ByVal text As String, ByVal maxLength As Integer) As String
			If (text.Length > maxLength) Then
				text = text.Substring(0, maxLength - 3).TrimEnd()
				text += "..."
			End If
			Return text
		End Function

		Private Sub _objectTypeDropDown_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _objectTypeDropDown.SelectedIndexChanged
			Dim selectedObjectType As kCura.WinEDDS.ObjectTypeListItem = DirectCast(_objectTypeDropDown.SelectedItem, kCura.WinEDDS.ObjectTypeListItem)
			Dim selectedItemValue As Int32 = selectedObjectType.Value
			ToolsImportLoadFileMenu.Text = TruncateTextWithEllipses(_objectTypeDropDown.Text, MAX_LENGTH_OF_OBJECT_NAME_BEFORE_TRUNCATION) & " &Load File..."
			ImportMenu.Visible = selectedObjectType.UserCanAdd
			ToolsImportLoadFileMenu.Visible = selectedObjectType.UserCanAdd
			ExportMenu.Visible = True
			If selectedItemValue = Relativity.ArtifactType.Document Then
				_caseFolderExplorer.Visible = True
				ToolsImportImageFileMenu.Visible = selectedObjectType.UserCanAdd
				ToolsImportProductionFileMenu.Visible = selectedObjectType.UserCanAdd
				ToolsExportProductionMenu.Visible = True
				ToolsExportSearchMenu.Visible = True
				_exportFoldersMenuItem.Visible = True
				_exportFoldersAndSubfoldersMenuItem.Visible = True
				_exportObjectsMenuItem.Visible = False
			Else
				'setting ExportMenu.Visible to True then False helps to resize the ToolsImportLoadFileMenu according to the length of its text
				'ExportMenu.Visible = True
				'ExportMenu.Visible = False
				ToolsExportProductionMenu.Visible = False
				ToolsExportSearchMenu.Visible = False
				_exportFoldersMenuItem.Visible = False
				_exportFoldersAndSubfoldersMenuItem.Visible = False
				_exportObjectsMenuItem.Visible = True
				_caseFolderExplorer.Visible = False
				ToolsImportImageFileMenu.Visible = False
				ToolsImportProductionFileMenu.Visible = False
			End If
			_application.ArtifactTypeID = selectedItemValue
		End Sub

		Private Async Sub _optionsMenuCheckConnectivityItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _optionsMenuCheckConnectivityItem.Click
			Await _application.QueryConnectivity()
		End Sub

		
	End Class

End Namespace