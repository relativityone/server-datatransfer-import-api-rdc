Imports System.Net
Imports kCura.WinEDDS
Imports Relativity.Desktop.Client.Legacy.Controls
Imports Relativity.Import.Export
Imports Relativity.Import.Export.Services
Imports Relativity.OAuth2Client.Exceptions

Namespace Relativity.Desktop.Client
	Public Class MainForm
		Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

		Public Sub New()
			MyBase.New()

			'This call is required by the Windows Form Designer.
			InitializeComponent()

			'Add any initialization after the InitializeComponent() call
			_application = Global.Relativity.Desktop.Client.Application.Instance
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
		Friend WithEvents ToolsMenu As System.Windows.Forms.MenuItem
		Friend WithEvents ToolsImportImageFileMenu As System.Windows.Forms.MenuItem
		Friend WithEvents StatusBar As System.Windows.Forms.StatusBar
		Friend WithEvents AppStatusPanel As System.Windows.Forms.StatusBarPanel
		Friend WithEvents LoggedInUserPanel As System.Windows.Forms.StatusBarPanel
		Friend WithEvents MainMenu As System.Windows.Forms.MainMenu
		Friend WithEvents _caseFolderExplorer As CaseFolderExplorer
		Friend WithEvents ExitMenu As System.Windows.Forms.MenuItem
		Friend WithEvents EnhancedMenuProvider As EnhancedMenuProvider
		Friend WithEvents ImportMenu As System.Windows.Forms.MenuItem
		Friend WithEvents ToolsImportLoadFileMenu As System.Windows.Forms.MenuItem
		Friend WithEvents ExportMenu As System.Windows.Forms.MenuItem
		Friend WithEvents OptionsMenu As System.Windows.Forms.MenuItem
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
			Me.ToolsMenu = New System.Windows.Forms.MenuItem()
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
			Me.OptionsMenu = New System.Windows.Forms.MenuItem()
			Me._toolsMenuSettingsItem = New System.Windows.Forms.MenuItem()
			Me._optionsMenuCheckConnectivityItem = New System.Windows.Forms.MenuItem()
			Me.MenuItem4 = New System.Windows.Forms.MenuItem()
			Me._aboutMenuItem = New System.Windows.Forms.MenuItem()
			Me._helpMenuItem = New System.Windows.Forms.MenuItem()
			Me.StatusBar = New System.Windows.Forms.StatusBar()
			Me.AppStatusPanel = New System.Windows.Forms.StatusBarPanel()
			Me.LoggedInUserPanel = New System.Windows.Forms.StatusBarPanel()
			Me._objectTypeDropDown = New System.Windows.Forms.ComboBox()
			Me.EnhancedMenuProvider = New EnhancedMenuProvider(Me.components)
			Me._caseFolderExplorer = New CaseFolderExplorer()
			CType(Me.AppStatusPanel, System.ComponentModel.ISupportInitialize).BeginInit()
			CType(Me.LoggedInUserPanel, System.ComponentModel.ISupportInitialize).BeginInit()
			Me.SuspendLayout()
			'
			'MainMenu
			'
			Me.MainMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuItem1, Me.ToolsMenu, Me.OptionsMenu, Me.MenuItem4})
			'
			'MenuItem1
			'
			Me.EnhancedMenuProvider.SetImageIndex(Me.MenuItem1, -1)
			Me.MenuItem1.Index = 0
			Me.MenuItem1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.OpenRepositoryMenu, Me.MenuItem3, Me.ExitMenu, Me._fileMenuRefresh})
			Me.MenuItem1.OwnerDraw = True
			Me.MenuItem1.Text = "&File"
			'
			'OpenRepositoryMenu
			'
			Me.EnhancedMenuProvider.SetImageIndex(Me.OpenRepositoryMenu, -1)
			Me.OpenRepositoryMenu.Index = 0
			Me.OpenRepositoryMenu.OwnerDraw = True
			Me.OpenRepositoryMenu.Text = "&Open..."
			'
			'MenuItem3
			'
			Me.EnhancedMenuProvider.SetImageIndex(Me.MenuItem3, -1)
			Me.MenuItem3.Index = 1
			Me.MenuItem3.OwnerDraw = True
			Me.MenuItem3.Text = "-"
			'
			'ExitMenu
			'
			Me.EnhancedMenuProvider.SetImageIndex(Me.ExitMenu, -1)
			Me.ExitMenu.Index = 2
			Me.ExitMenu.OwnerDraw = True
			Me.ExitMenu.Text = "&Exit"
			'
			'_fileMenuRefresh
			'
			Me._fileMenuRefresh.Enabled = False
			Me.EnhancedMenuProvider.SetImageIndex(Me._fileMenuRefresh, -1)
			Me._fileMenuRefresh.Index = 3
			Me._fileMenuRefresh.OwnerDraw = True
			Me._fileMenuRefresh.Shortcut = System.Windows.Forms.Shortcut.F5
			Me._fileMenuRefresh.Text = "Refresh"
			'
			'ToolsMenu
			'
			Me.EnhancedMenuProvider.SetImageIndex(Me.ToolsMenu, -1)
			Me.ToolsMenu.Index = 1
			Me.ToolsMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.ImportMenu, Me.ExportMenu})
			Me.ToolsMenu.OwnerDraw = True
			Me.ToolsMenu.Text = "&Tools"
			'
			'ImportMenu
			'
			Me.ImportMenu.Enabled = False
			Me.EnhancedMenuProvider.SetImageIndex(Me.ImportMenu, -1)
			Me.ImportMenu.Index = 0
			Me.ImportMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.ToolsImportImageFileMenu, Me.ToolsImportLoadFileMenu, Me.ToolsImportProductionFileMenu})
			Me.ImportMenu.OwnerDraw = True
			Me.ImportMenu.Text = "&Import"
			'
			'ToolsImportImageFileMenu
			'
			Me.EnhancedMenuProvider.SetImageIndex(Me.ToolsImportImageFileMenu, -1)
			Me.ToolsImportImageFileMenu.Index = 0
			Me.ToolsImportImageFileMenu.OwnerDraw = True
			Me.ToolsImportImageFileMenu.Shortcut = System.Windows.Forms.Shortcut.CtrlI
			Me.ToolsImportImageFileMenu.Text = "&Image Load File..."
			'
			'ToolsImportLoadFileMenu
			'
			Me.EnhancedMenuProvider.SetImageIndex(Me.ToolsImportLoadFileMenu, -1)
			Me.ToolsImportLoadFileMenu.Index = 1
			Me.ToolsImportLoadFileMenu.OwnerDraw = True
			Me.ToolsImportLoadFileMenu.Shortcut = System.Windows.Forms.Shortcut.CtrlL
			Me.ToolsImportLoadFileMenu.Text = "&Load File..."
			'
			'ToolsImportProductionFileMenu
			'
			Me.EnhancedMenuProvider.SetImageIndex(Me.ToolsImportProductionFileMenu, -1)
			Me.ToolsImportProductionFileMenu.Index = 2
			Me.ToolsImportProductionFileMenu.OwnerDraw = True
			Me.ToolsImportProductionFileMenu.Shortcut = System.Windows.Forms.Shortcut.CtrlP
			Me.ToolsImportProductionFileMenu.Text = "Production Load File..."
			'
			'ExportMenu
			'
			Me.ExportMenu.Enabled = False
			Me.EnhancedMenuProvider.SetImageIndex(Me.ExportMenu, -1)
			Me.ExportMenu.Index = 1
			Me.ExportMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.ToolsExportProductionMenu, Me.ToolsExportSearchMenu, Me._exportFoldersMenuItem, Me._exportFoldersAndSubfoldersMenuItem, Me._exportObjectsMenuItem})
			Me.ExportMenu.OwnerDraw = True
			Me.ExportMenu.Text = "&Export"
			'
			'ToolsExportProductionMenu
			'
			Me.EnhancedMenuProvider.SetImageIndex(Me.ToolsExportProductionMenu, -1)
			Me.ToolsExportProductionMenu.Index = 0
			Me.ToolsExportProductionMenu.OwnerDraw = True
			Me.ToolsExportProductionMenu.Text = "Production Set..."
			'
			'ToolsExportSearchMenu
			'
			Me.EnhancedMenuProvider.SetImageIndex(Me.ToolsExportSearchMenu, -1)
			Me.ToolsExportSearchMenu.Index = 1
			Me.ToolsExportSearchMenu.OwnerDraw = True
			Me.ToolsExportSearchMenu.Text = "Saved Search..."
			'
			'_exportFoldersMenuItem
			'
			Me.EnhancedMenuProvider.SetImageIndex(Me._exportFoldersMenuItem, -1)
			Me._exportFoldersMenuItem.Index = 2
			Me._exportFoldersMenuItem.OwnerDraw = True
			Me._exportFoldersMenuItem.Text = "Folder..."
			'
			'_exportFoldersAndSubfoldersMenuItem
			'
			Me.EnhancedMenuProvider.SetImageIndex(Me._exportFoldersAndSubfoldersMenuItem, -1)
			Me._exportFoldersAndSubfoldersMenuItem.Index = 3
			Me._exportFoldersAndSubfoldersMenuItem.OwnerDraw = True
			Me._exportFoldersAndSubfoldersMenuItem.Text = "Folder and Subfolders..."
			'
			'_exportObjectsMenuItem
			'
			Me.EnhancedMenuProvider.SetImageIndex(Me._exportObjectsMenuItem, -1)
			Me._exportObjectsMenuItem.Index = 4
			Me._exportObjectsMenuItem.OwnerDraw = True
			Me._exportObjectsMenuItem.Text = "Objects"
			Me._exportObjectsMenuItem.Visible = False
			'
			'OptionsMenu
			'
			Me.EnhancedMenuProvider.SetImageIndex(Me.OptionsMenu, -1)
			Me.OptionsMenu.Index = 2
			Me.OptionsMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me._toolsMenuSettingsItem, Me._optionsMenuCheckConnectivityItem})
			Me.OptionsMenu.OwnerDraw = True
			Me.OptionsMenu.Text = "Options"
			'
			'_toolsMenuSettingsItem
			'
			Me.EnhancedMenuProvider.SetImageIndex(Me._toolsMenuSettingsItem, -1)
			Me._toolsMenuSettingsItem.Index = 0
			Me._toolsMenuSettingsItem.OwnerDraw = True
			Me._toolsMenuSettingsItem.Text = "Settings..."
			'
			'_optionsMenuCheckConnectivityItem
			'
			Me._optionsMenuCheckConnectivityItem.Enabled = False
			Me.EnhancedMenuProvider.SetImageIndex(Me._optionsMenuCheckConnectivityItem, -1)
			Me._optionsMenuCheckConnectivityItem.Index = 1
			Me._optionsMenuCheckConnectivityItem.OwnerDraw = True
			Me._optionsMenuCheckConnectivityItem.Text = "Check Connectivity..."
			'
			'MenuItem4
			'
			Me.EnhancedMenuProvider.SetImageIndex(Me.MenuItem4, -1)
			Me.MenuItem4.Index = 3
			Me.MenuItem4.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me._aboutMenuItem, Me._helpMenuItem})
			Me.MenuItem4.OwnerDraw = True
			Me.MenuItem4.Text = "Help"
			'
			'_aboutMenuItem
			'
			Me.EnhancedMenuProvider.SetImageIndex(Me._aboutMenuItem, -1)
			Me._aboutMenuItem.Index = 0
			Me._aboutMenuItem.OwnerDraw = True
			Me._aboutMenuItem.Text = "About..."
			'
			'_helpMenuItem
			'
			Me._helpMenuItem.Enabled = False
			Me.EnhancedMenuProvider.SetImageIndex(Me._helpMenuItem, -1)
			Me._helpMenuItem.Index = 1
			Me._helpMenuItem.OwnerDraw = True
			Me._helpMenuItem.Text = "Help"
			'
			'StatusBar
			'
			Me.StatusBar.Location = New System.Drawing.Point(0, 368)
			Me.StatusBar.Name = "StatusBar"
			Me.StatusBar.Panels.AddRange(New System.Windows.Forms.StatusBarPanel() {Me.AppStatusPanel, Me.LoggedInUserPanel})
			Me.StatusBar.ShowPanels = True
			Me.StatusBar.Size = New System.Drawing.Size(584, 22)
			Me.StatusBar.TabIndex = 5
			'
			'AppStatusPanel
			'
			Me.AppStatusPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring
			Me.AppStatusPanel.Name = "AppStatusPanel"
			Me.AppStatusPanel.Width = 467
			'
			'LoggedInUserPanel
			'
			Me.LoggedInUserPanel.Alignment = System.Windows.Forms.HorizontalAlignment.Right
			Me.LoggedInUserPanel.Name = "LoggedInUserPanel"
			'
			'_objectTypeDropDown
			'
			Me._objectTypeDropDown.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
				Or System.Windows.Forms.AnchorStyles.Left) _
				Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me._objectTypeDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._objectTypeDropDown.Location = New System.Drawing.Point(0, 4)
			Me._objectTypeDropDown.Name = "_objectTypeDropDown"
			Me._objectTypeDropDown.Size = New System.Drawing.Size(584, 21)
			Me._objectTypeDropDown.TabIndex = 7
			'
			'_caseFolderExplorer
			'
			Me._caseFolderExplorer.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
				Or System.Windows.Forms.AnchorStyles.Left) _
				Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me._caseFolderExplorer.ExportContextMenuEnabled = True
			Me._caseFolderExplorer.ImportContextMenuEnabled = True
			Me._caseFolderExplorer.Location = New System.Drawing.Point(0, 25)
			Me._caseFolderExplorer.Name = "_caseFolderExplorer"
			Me._caseFolderExplorer.Size = New System.Drawing.Size(584, 343)
			Me._caseFolderExplorer.TabIndex = 6
			'
			'MainForm
			'
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.ClientSize = New System.Drawing.Size(584, 390)
			Me.Controls.Add(Me._objectTypeDropDown)
			Me.Controls.Add(Me._caseFolderExplorer)
			Me.Controls.Add(Me.StatusBar)
			Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
			Me.Menu = Me.MainMenu
			Me.Name = "MainForm"
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
			Me.Text = "Relativity Desktop Client"
			CType(Me.AppStatusPanel, System.ComponentModel.ISupportInitialize).EndInit()
			CType(Me.LoggedInUserPanel, System.ComponentModel.ISupportInitialize).EndInit()
			Me.ResumeLayout(False)

		End Sub

#End Region

		'' Private WithEvents _optionsForm As OptionsForm = Nothing
		Friend WithEvents _application As Global.Relativity.Desktop.Client.Application
		Public Const MAX_LENGTH_OF_OBJECT_NAME_BEFORE_TRUNCATION As Int32 = 25

		Private _isConnecting As Boolean = False

		Private Async Sub OpenRepositoryMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenRepositoryMenu.Click
			Try
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
						Await _application.OpenCaseAsync().ConfigureAwait(False)
						EnhancedMenuProvider.Hook(Me)
					End If
				Else
					_application.OpenCaseSelector = False
					Await _application.GetCredentialsAsync()
					Await _application.OpenCaseAsync().ConfigureAwait(False)
				End If
			Catch ex As LoginCanceledException
				'user close the login window, do nothing
			End Try

		End Sub

		Private Sub ExitMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitMenu.Click
			_application.ExitApplication()
		End Sub

		Private Sub _application_OnEvent(ByVal appEvent As AppEvent) Handles _application.OnEvent
			Me.Invoke(Async Sub() Await HandleEventOnUiThreadAsync(appEvent))
		End Sub

		Private Async Function HandleEventOnUiThreadAsync(appEvent As AppEvent) As Task

			Select Case appEvent.EventType
				Case AppEvent.AppEventType.LoadCase
					_fileMenuRefresh.Enabled = True
					UpdateStatus("Workspace Loaded - File Transfer Mode: Connecting...")
					_isConnecting = True
					ToggleImportExportMenu()
					Dim modeTask = Task.Run(Async Function() Await _application.GetConnectionStatus().ConfigureAwait(False)).ConfigureAwait(True)
					Await PopulateObjectTypeDropDown()
					Dim mode As String = Await modeTask
					UpdateStatus($"Workspace Loaded - File Transfer Mode: {mode}")
					_isConnecting = False
					_optionsMenuCheckConnectivityItem.Enabled = True
					ToggleImportExportMenu()
					_application.OpenCaseSelector = False
				Case AppEvent.AppEventType.LogOnForm
					'Enable help once logged into Relativity via RDC login form
					Me._helpMenuItem.Enabled = True
				Case AppEvent.AppEventType.LogOn
					UpdateUserName(_application.LoggedInUser)
					'Enable help once logged into Relativity via Windows Authentication
					Me._helpMenuItem.Enabled = True
				Case AppEvent.AppEventType.ExitApplication
					Me.Close()
				Case AppEvent.AppEventType.WorkspaceFolderSelected
					'disable import and export menus if no permission
					ToggleImportExportMenu()
					'UpdateStatus("Case Folder Load: " + _application.SelectedCaseInfo.RootFolderID.ToString)
					'disable staging explorer menu if no permission
				Case AppEvent.AppEventType.LogOnRequested
					'' please note that url input and connection loop retry takes place on the stack
					'' if in doubt what it means please try to input several times invalid web api url from main form settings and check call stack while having breakpoint on the following line
					'' TODO: this shloud be rewritten to use simple while-like loop
					Await CheckCertificateAsync()
			End Select
		End Function

		Private Sub ToggleImportExportMenu()
			Dim hasImportPermission = _application.UserHasImportPermission
			Dim hasExportPermission = _application.UserHasExportPermission
			Dim isImportEnabled = hasImportPermission And Not _isConnecting
			Dim isExportEnabled = hasExportPermission And Not _isConnecting

			ImportMenu.Enabled = isImportEnabled
			_caseFolderExplorer.ImportContextMenuEnabled = isImportEnabled

			ExportMenu.Enabled = isExportEnabled
			_caseFolderExplorer.ExportContextMenuEnabled = isExportEnabled

			ImportMenu.Visible = hasImportPermission
			ExportMenu.Visible = hasExportPermission
		End Sub

		Private Sub UpdateStatus(ByVal text As String)
			AppStatusPanel.Text = text
		End Sub

		Private Sub UpdateUserName(ByVal text As String)
			LoggedInUserPanel.Text = text
		End Sub

		Private Async Sub MainForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

			ServicePointManager.DefaultConnectionLimit = Environment.ProcessorCount * 12
			LoadWindowSize()
			Me.CenterToScreen()
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			_application.TemporaryForceFolderPreview = AppSettings.Instance.ForceFolderPreview
			If AppSettings.Instance.WebApiServiceUrl = String.Empty Then
				_application.SetWebServiceURL()
			End If

			'' Can't do this in Application.vb without refactoring AttemptLogin (which needs this form as a parameter)
			Await CheckCertificateAsync()

			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Sub

		Private Async Function CheckCertificateAsync() As Task
			Try
				If (_application.CertificateTrusted()) Then
					Await _application.AttemptLogin(Me)
				Else
					_application.CertificateCheckPrompt()
				End If
			Catch ex As WebException
				_application.HandleWebException(ex)
			Catch ex As RelativityVersionMismatchException
				_application.ChangeWebServiceUrl(ex.Message + " Try a new URL?")
			End Try

		End Function

		Private Async Sub WebServiceURLChangedAsync() Handles _application.ReCheckCertificate
			'Disable help since user will be asked to login again
			Me._helpMenuItem.Enabled = False
			Await CheckCertificateAsync()
		End Sub

		Private Async Sub MainForm_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
			SaveWindowSize()
			_application.UpdateForceFolderPreview()
			_application.UpdateWebServiceURL(False)
			Await _application.Logout()
			EnhancedMenuProvider.Unhook()
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
			Await _application.RefreshCaseFolders().ConfigureAwait(False)
			Await _application.RefreshSelectedCaseInfoAsync().ConfigureAwait(False)
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
			Dim objectTypeManager As New kCura.WinEDDS.Service.ObjectTypeManager(Await _application.GetCredentialsAsync().ConfigureAwait(True), _application.CookieContainer)
			Dim uploadableObjectTypes As System.Data.DataRowCollection = objectTypeManager.RetrieveAllUploadable(_application.SelectedCaseInfo.ArtifactID).Tables(0).Rows
			Dim selectedObjectTypeID As Int32 = ArtifactType.Document
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
			If selectedItemValue = ArtifactType.Document Then
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

		Private Sub LoadWindowSize()
			If kCura.WinEDDS.Config.MainFormWindowWidth <> Nothing AndAlso kCura.WinEDDS.Config.MainFormWindowHeight <> Nothing Then
				Me.Size = New Size(kCura.WinEDDS.Config.MainFormWindowWidth, kCura.WinEDDS.Config.MainFormWindowHeight)
			End If
		End Sub

		Private Sub SaveWindowSize()
			If Me.WindowState = FormWindowState.Normal Then
				kCura.WinEDDS.Config.MainFormWindowWidth = Me.Size.Width
				kCura.WinEDDS.Config.MainFormWindowHeight = Me.Size.Height
			Else
				kCura.WinEDDS.Config.MainFormWindowWidth = Me.RestoreBounds.Size.Width
				kCura.WinEDDS.Config.MainFormWindowHeight = Me.RestoreBounds.Size.Height
			End If
		End Sub

	End Class

End Namespace