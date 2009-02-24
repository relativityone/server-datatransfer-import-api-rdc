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
		Friend WithEvents ToolsImportFileDirectoryMenu As System.Windows.Forms.MenuItem
		Friend WithEvents ToolsImportOutlookMenu As System.Windows.Forms.MenuItem
		Friend WithEvents ToolsImportSQLDatabaseMenu As System.Windows.Forms.MenuItem
		Friend WithEvents _toolsMenu As System.Windows.Forms.MenuItem
		Friend WithEvents _toolsMenuSettingsItem As System.Windows.Forms.MenuItem
		Friend WithEvents ToolsExportProductionMenu As System.Windows.Forms.MenuItem
		Friend WithEvents ToolsExportSearchMenu As System.Windows.Forms.MenuItem
		Friend WithEvents _fileMenuRefresh As System.Windows.Forms.MenuItem
		Friend WithEvents MenuItem4 As System.Windows.Forms.MenuItem
		Friend WithEvents _aboutMenuItem As System.Windows.Forms.MenuItem
		Friend WithEvents _exportFoldersMenuItem As System.Windows.Forms.MenuItem
		Friend WithEvents _exportFoldersAndSubfoldersMenuItem As System.Windows.Forms.MenuItem
		Friend WithEvents ToolsImportProductionFileMenu As System.Windows.Forms.MenuItem
    Friend WithEvents _objectTypeDropDown As System.Windows.Forms.ComboBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
      Me.components = New System.ComponentModel.Container
      Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(MainForm))
      Me.MainMenu = New System.Windows.Forms.MainMenu
      Me.MenuItem1 = New System.Windows.Forms.MenuItem
      Me.OpenRepositoryMenu = New System.Windows.Forms.MenuItem
      Me.MenuItem3 = New System.Windows.Forms.MenuItem
      Me.ExitMenu = New System.Windows.Forms.MenuItem
      Me._fileMenuRefresh = New System.Windows.Forms.MenuItem
      Me.MenuItem2 = New System.Windows.Forms.MenuItem
      Me.ImportMenu = New System.Windows.Forms.MenuItem
      Me.ToolsImportImageFileMenu = New System.Windows.Forms.MenuItem
      Me.ToolsImportLoadFileMenu = New System.Windows.Forms.MenuItem
      Me.ToolsImportProductionFileMenu = New System.Windows.Forms.MenuItem
      Me.ToolsImportFileDirectoryMenu = New System.Windows.Forms.MenuItem
      Me.ToolsImportOutlookMenu = New System.Windows.Forms.MenuItem
      Me.ToolsImportSQLDatabaseMenu = New System.Windows.Forms.MenuItem
      Me.ExportMenu = New System.Windows.Forms.MenuItem
      Me.ToolsExportProductionMenu = New System.Windows.Forms.MenuItem
      Me.ToolsExportSearchMenu = New System.Windows.Forms.MenuItem
      Me._exportFoldersMenuItem = New System.Windows.Forms.MenuItem
      Me._exportFoldersAndSubfoldersMenuItem = New System.Windows.Forms.MenuItem
      Me._toolsMenu = New System.Windows.Forms.MenuItem
      Me._toolsMenuSettingsItem = New System.Windows.Forms.MenuItem
      Me.MenuItem4 = New System.Windows.Forms.MenuItem
      Me._aboutMenuItem = New System.Windows.Forms.MenuItem
      Me.StatusBar = New System.Windows.Forms.StatusBar
      Me.AppStatusPanel = New System.Windows.Forms.StatusBarPanel
      Me.LoggedInUserPanel = New System.Windows.Forms.StatusBarPanel
      Me._caseFolderExplorer = New kCura.EDDS.WinForm.CaseFolderExplorer
      Me.EnhancedMenuProvider = New kCura.Windows.Forms.EnhancedMenuProvider(Me.components)
      Me._objectTypeDropDown = New System.Windows.Forms.ComboBox
      CType(Me.AppStatusPanel, System.ComponentModel.ISupportInitialize).BeginInit()
      CType(Me.LoggedInUserPanel, System.ComponentModel.ISupportInitialize).BeginInit()
      Me.SuspendLayout()
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
      'MenuItem2
      '
      Me.EnhancedMenuProvider.SetImageIndex(Me.MenuItem2, -1)
      Me.MenuItem2.Index = 1
      Me.MenuItem2.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.ImportMenu, Me.ExportMenu})
      Me.MenuItem2.OwnerDraw = True
      Me.MenuItem2.Text = "&Tools"
      '
      'ImportMenu
      '
      Me.ImportMenu.Enabled = False
      Me.EnhancedMenuProvider.SetImageIndex(Me.ImportMenu, -1)
      Me.ImportMenu.Index = 0
      Me.ImportMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.ToolsImportImageFileMenu, Me.ToolsImportLoadFileMenu, Me.ToolsImportProductionFileMenu, Me.ToolsImportFileDirectoryMenu, Me.ToolsImportOutlookMenu, Me.ToolsImportSQLDatabaseMenu})
      Me.ImportMenu.OwnerDraw = True
      Me.ImportMenu.Text = "&Import"
      '
      'ToolsImportImageFileMenu
      '
      Me.EnhancedMenuProvider.SetImageIndex(Me.ToolsImportImageFileMenu, -1)
      Me.ToolsImportImageFileMenu.Index = 0
      Me.ToolsImportImageFileMenu.OwnerDraw = True
      Me.ToolsImportImageFileMenu.Shortcut = System.Windows.Forms.Shortcut.CtrlI
      Me.ToolsImportImageFileMenu.Text = "&Image File..."
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
      Me.ToolsImportProductionFileMenu.Text = "Production File..."
      '
      'ToolsImportFileDirectoryMenu
      '
      Me.EnhancedMenuProvider.SetImageIndex(Me.ToolsImportFileDirectoryMenu, -1)
      Me.ToolsImportFileDirectoryMenu.Index = 3
      Me.ToolsImportFileDirectoryMenu.OwnerDraw = True
      Me.ToolsImportFileDirectoryMenu.Text = "&File Directory..."
      Me.ToolsImportFileDirectoryMenu.Visible = False
      '
      'ToolsImportOutlookMenu
      '
      Me.EnhancedMenuProvider.SetImageIndex(Me.ToolsImportOutlookMenu, -1)
      Me.ToolsImportOutlookMenu.Index = 4
      Me.ToolsImportOutlookMenu.OwnerDraw = True
      Me.ToolsImportOutlookMenu.Text = "&Outlook..."
      Me.ToolsImportOutlookMenu.Visible = False
      '
      'ToolsImportSQLDatabaseMenu
      '
      Me.EnhancedMenuProvider.SetImageIndex(Me.ToolsImportSQLDatabaseMenu, -1)
      Me.ToolsImportSQLDatabaseMenu.Index = 5
      Me.ToolsImportSQLDatabaseMenu.OwnerDraw = True
      Me.ToolsImportSQLDatabaseMenu.Text = "&SQL Database..."
      Me.ToolsImportSQLDatabaseMenu.Visible = False
      '
      'ExportMenu
      '
      Me.ExportMenu.Enabled = False
      Me.EnhancedMenuProvider.SetImageIndex(Me.ExportMenu, -1)
      Me.ExportMenu.Index = 1
      Me.ExportMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.ToolsExportProductionMenu, Me.ToolsExportSearchMenu, Me._exportFoldersMenuItem, Me._exportFoldersAndSubfoldersMenuItem})
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
      '_toolsMenu
      '
      Me.EnhancedMenuProvider.SetImageIndex(Me._toolsMenu, -1)
      Me._toolsMenu.Index = 2
      Me._toolsMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me._toolsMenuSettingsItem})
      Me._toolsMenu.OwnerDraw = True
      Me._toolsMenu.Text = "Options"
      '
      '_toolsMenuSettingsItem
      '
      Me.EnhancedMenuProvider.SetImageIndex(Me._toolsMenuSettingsItem, -1)
      Me._toolsMenuSettingsItem.Index = 0
      Me._toolsMenuSettingsItem.OwnerDraw = True
      Me._toolsMenuSettingsItem.Text = "Settings"
      '
      'MenuItem4
      '
      Me.EnhancedMenuProvider.SetImageIndex(Me.MenuItem4, -1)
      Me.MenuItem4.Index = 3
      Me.MenuItem4.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me._aboutMenuItem})
      Me.MenuItem4.OwnerDraw = True
      Me.MenuItem4.Text = "Help"
      '
      '_aboutMenuItem
      '
      Me.EnhancedMenuProvider.SetImageIndex(Me._aboutMenuItem, -1)
      Me._aboutMenuItem.Index = 0
      Me._aboutMenuItem.OwnerDraw = True
      Me._aboutMenuItem.Text = "About"
      '
      'StatusBar
      '
      Me.StatusBar.Location = New System.Drawing.Point(0, 515)
      Me.StatusBar.Name = "StatusBar"
      Me.StatusBar.Panels.AddRange(New System.Windows.Forms.StatusBarPanel() {Me.AppStatusPanel, Me.LoggedInUserPanel})
      Me.StatusBar.ShowPanels = True
      Me.StatusBar.Size = New System.Drawing.Size(332, 22)
      Me.StatusBar.TabIndex = 5
      '
      'AppStatusPanel
      '
      Me.AppStatusPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring
      Me.AppStatusPanel.Width = 216
      '
      'LoggedInUserPanel
      '
      Me.LoggedInUserPanel.Alignment = System.Windows.Forms.HorizontalAlignment.Right
      '
      '_caseFolderExplorer
      '
      Me._caseFolderExplorer.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                  Or System.Windows.Forms.AnchorStyles.Left) _
                  Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
      Me._caseFolderExplorer.Location = New System.Drawing.Point(0, 25)
      Me._caseFolderExplorer.Name = "_caseFolderExplorer"
      Me._caseFolderExplorer.Size = New System.Drawing.Size(332, 490)
      Me._caseFolderExplorer.TabIndex = 6
      '
      '_objectTypeDropDown
      '
      Me._objectTypeDropDown.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                  Or System.Windows.Forms.AnchorStyles.Left) _
                  Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
      Me._objectTypeDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
      Me._objectTypeDropDown.Location = New System.Drawing.Point(0, 4)
      Me._objectTypeDropDown.Name = "_objectTypeDropDown"
      Me._objectTypeDropDown.Size = New System.Drawing.Size(332, 21)
      Me._objectTypeDropDown.TabIndex = 7
      '
      'MainForm
      '
      Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
      Me.ClientSize = New System.Drawing.Size(332, 537)
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

    Friend WithEvents _application As kCura.EDDS.WinForm.Application

    Private Sub OpenRepositoryMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenRepositoryMenu.Click
      _application.OpenCase()
    End Sub

    Private Sub ExitMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitMenu.Click
      _application.ExitApplication()
    End Sub

    Private Sub _application_OnEvent(ByVal appEvent As AppEvent) Handles _application.OnEvent
      Select Case appEvent.EventType
        Case appEvent.AppEventType.LoadCase
          _fileMenuRefresh.Enabled = True
          UpdateStatus("Case Loaded - File Transfer Mode: " & _application.GetConnectionStatus)
          PopulateObjectTypeDropDown()
        Case appEvent.AppEventType.LogOn
          UpdateUserName(_application.LoggedInUser)
        Case appEvent.AppEventType.ExitApplication
          Me.Close()
        Case appEvent.AppEventType.CaseFolderSelected
          ImportMenu.Enabled = True
          ExportMenu.Enabled = True
          'UpdateStatus("Case Folder Load: " + _application.SelectedCaseInfo.RootFolderID.ToString)
      End Select
    End Sub

    Private Sub UpdateStatus(ByVal text As String)
      AppStatusPanel.Text = text
    End Sub

    Private Sub UpdateUserName(ByVal text As String)
      LoggedInUserPanel.Text = text
    End Sub

    Private Sub MainForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
      Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
      If kCura.WinEDDS.Config.WebServiceURL = String.Empty Then
        _application.SetWebServiceURL()
      End If

      If Not _application.DefaultCredentialsAreGood() Then
        _application.NewLogin()
      Else
        _application.LogOn()
        _application.OpenCase()
        kCura.Windows.Forms.EnhancedMenuProvider.Hook(Me)
      End If
      Me.Cursor = System.Windows.Forms.Cursors.Default
    End Sub

    Private Sub MainForm_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
      _application.UpdateWebServiceURL(False)
      kCura.Windows.Forms.EnhancedMenuProvider.Unhook()
    End Sub

    Private Sub ToolsImportImageFileMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolsImportImageFileMenu.Click
      Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
      _application.NewImageFile(_application.SelectedCaseFolderID, _application.SelectedCaseInfo)
      Me.Cursor = System.Windows.Forms.Cursors.Default
    End Sub

    Private Sub ToolsImportProductionFileMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolsImportProductionFileMenu.Click
      Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
      _application.NewProductionFile(_application.SelectedCaseFolderID, _application.SelectedCaseInfo)
      Me.Cursor = System.Windows.Forms.Cursors.Default
    End Sub

    Private Sub ToolsImportLoadFileMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolsImportLoadFileMenu.Click
      Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
      _application.NewLoadFile(_application.SelectedCaseFolderID, _application.SelectedCaseInfo)
      Me.Cursor = System.Windows.Forms.Cursors.Default
    End Sub

    Private Sub ToolsImportFileDirectoryMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolsImportFileDirectoryMenu.Click
      Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
      _application.NewDirectoryImport(_application.SelectedCaseFolderID, _application.SelectedCaseInfo)
      Me.Cursor = System.Windows.Forms.Cursors.Default
    End Sub

    Private Sub ToolsImportOutlookMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolsImportOutlookMenu.Click
      Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
      _application.NewOutlookImport(_application.SelectedCaseFolderID, _application.SelectedCaseInfo)
      Me.Cursor = System.Windows.Forms.Cursors.Default
    End Sub

    Private Sub ToolsExportProductionMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolsExportProductionMenu.Click
      Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
      _application.NewProductionExport(_application.SelectedCaseInfo)
      Me.Cursor = System.Windows.Forms.Cursors.Default
    End Sub

    Private Sub ToolsExportSearchMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolsExportSearchMenu.Click
      Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
      _application.NewSearchExport(_application.SelectedCaseInfo.RootFolderID, _application.SelectedCaseInfo, kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch)
      Me.Cursor = System.Windows.Forms.Cursors.Default
    End Sub

    Private Sub ToolsImportSQLDatabaseMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolsImportSQLDatabaseMenu.Click
      Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
      _application.NewSQLImport(_application.SelectedCaseFolderID, _application.SelectedCaseInfo)
      Me.Cursor = System.Windows.Forms.Cursors.Default
    End Sub

    Private Sub _toolsMenuSettingsItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _toolsMenuSettingsItem.Click
      Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
      _application.NewOptions()
      Me.Cursor = System.Windows.Forms.Cursors.Default
    End Sub

    Private Sub _fileMenuRefresh_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _fileMenuRefresh.Click
      _application.UpdateWebServiceURL(False)
      Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
      _application.RefreshCaseFolders()
      _application.RefreshSelectedCaseInfo()
      Me.Cursor = System.Windows.Forms.Cursors.Default
    End Sub

    Private Sub _aboutMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _aboutMenuItem.Click
      _application.DoAbout()
    End Sub

    Private Sub MainForm_Closed(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Closed
      System.Environment.Exit(0)
    End Sub

    Private Sub _exportFoldersMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _exportFoldersMenuItem.Click
      _application.NewSearchExport(_application.SelectedCaseFolderID, _application.SelectedCaseInfo, ExportFile.ExportType.ParentSearch)
    End Sub

    Private Sub _exportFoldersAndSubfoldersMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _exportFoldersAndSubfoldersMenuItem.Click
      _application.NewSearchExport(_application.SelectedCaseFolderID, _application.SelectedCaseInfo, ExportFile.ExportType.AncestorSearch)
    End Sub

    Private Sub PopulateObjectTypeDropDown()
      Dim objectTypeManager As New kCura.WinEDDS.Service.ObjectTypeManager(_application.Credential, _application.CookieContainer)
      Dim uploadableObjectTypes As System.Data.DataRowCollection = objectTypeManager.RetrieveAllUploadable(_application.SelectedCaseInfo.ArtifactID).Tables(0).Rows
      Dim selectedObjectTypeID As Int32 = 10
      If _objectTypeDropDown.Items.Count > 0 Then
        selectedObjectTypeID = DirectCast(_objectTypeDropDown.SelectedItem, kCura.WinEDDS.ObjectTypeListItem).Value
      End If
      _objectTypeDropDown.Items.Clear()
      For Each objectType As System.Data.DataRow In uploadableObjectTypes
        Dim currentObjectType As New kCura.WinEDDS.ObjectTypeListItem(CType(objectType("DescriptorArtifactTypeID"), Int32), CType(objectType("Name"), String))
        _objectTypeDropDown.Items.Add(currentObjectType)
        If CType(objectType("DescriptorArtifactTypeID"), Int32) = selectedObjectTypeID Then
          Me._objectTypeDropDown.SelectedItem = currentObjectType
        End If
      Next
    End Sub

    Private Sub _objectTypeDropDown_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _objectTypeDropDown.SelectedIndexChanged
      Dim selectedItemValue As Int32 = DirectCast(_objectTypeDropDown.SelectedItem, kCura.WinEDDS.ObjectTypeListItem).Value
      ToolsImportLoadFileMenu.Text = _objectTypeDropDown.Text & " &Load File..."
      If selectedItemValue = 10 Then
        _caseFolderExplorer.Visible = True
        ToolsImportImageFileMenu.Visible = True
        ToolsImportProductionFileMenu.Visible = True
        ExportMenu.Visible = True
      Else
        'setting ExportMenu.Visible to True then False helps to resize the ToolsImportLoadFileMenu according to the length of its text
        ExportMenu.Visible = True
        ExportMenu.Visible = False
        _caseFolderExplorer.Visible = False
        ToolsImportImageFileMenu.Visible = False
        ToolsImportProductionFileMenu.Visible = False
      End If
      _application.ArtifactTypeID = selectedItemValue
    End Sub

  End Class

End Namespace