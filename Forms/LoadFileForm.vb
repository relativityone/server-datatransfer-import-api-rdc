Namespace kCura.EDDS.WinForm
  Public Class LoadFileForm
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
		Friend WithEvents OpenFileDialog As System.Windows.Forms.OpenFileDialog
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
		Friend WithEvents _importDestinationText As System.Windows.Forms.TextBox
		Friend WithEvents MainMenu As System.Windows.Forms.MainMenu
    Friend WithEvents MenuItem1 As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItem2 As System.Windows.Forms.MenuItem
    Friend WithEvents PreviewMenuFile As System.Windows.Forms.MenuItem
    Friend WithEvents ImportFileMenu As System.Windows.Forms.MenuItem
		Friend WithEvents _fileSaveFieldMapMenuItem As System.Windows.Forms.MenuItem
		Friend WithEvents _saveFieldMapDialog As System.Windows.Forms.SaveFileDialog
		Friend WithEvents _fileLoadFieldMapMenuItem As System.Windows.Forms.MenuItem
		Friend WithEvents _loadFieldMapDialog As System.Windows.Forms.OpenFileDialog
		Friend WithEvents _importMenuPreviewErrorsItem As System.Windows.Forms.MenuItem
		Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
		Friend WithEvents _loadFileTab As System.Windows.Forms.TabPage
		Friend WithEvents _fieldMapTab As System.Windows.Forms.TabPage
		Friend WithEvents _fieldMap As kCura.Windows.Forms.TwoListBox
		Friend WithEvents Label1 As System.Windows.Forms.Label
		Friend WithEvents Label7 As System.Windows.Forms.Label
		Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
		Friend WithEvents _identifiersDropDown As System.Windows.Forms.ComboBox
		Friend WithEvents _loadNativeFiles As System.Windows.Forms.CheckBox
		Friend WithEvents _extractFullTextFromNativeFile As System.Windows.Forms.CheckBox
		Friend WithEvents Label5 As System.Windows.Forms.Label
		Friend WithEvents _nativeFilePathField As System.Windows.Forms.ComboBox
		Friend WithEvents GroupBox4 As System.Windows.Forms.GroupBox
		Friend WithEvents GroupBox23 As System.Windows.Forms.GroupBox
		Friend WithEvents _multiRecordDelimiter As System.Windows.Forms.ComboBox
		Friend WithEvents Label6 As System.Windows.Forms.Label
		Friend WithEvents Label4 As System.Windows.Forms.Label
		Friend WithEvents _quoteDelimiter As System.Windows.Forms.ComboBox
		Friend WithEvents Label3 As System.Windows.Forms.Label
		Friend WithEvents _newLineDelimiter As System.Windows.Forms.ComboBox
		Friend WithEvents Label2 As System.Windows.Forms.Label
		Friend WithEvents _recordDelimiter As System.Windows.Forms.ComboBox
		Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
		Friend WithEvents _fileColumnHeaders As System.Windows.Forms.ListBox
		Friend WithEvents _firstLineContainsColumnNames As System.Windows.Forms.CheckBox
		Friend WithEvents GroupBox20 As System.Windows.Forms.GroupBox
		Friend WithEvents _browseButton As System.Windows.Forms.Button
		Friend WithEvents _filePath As System.Windows.Forms.TextBox
		Friend WithEvents HelpProvider1 As System.Windows.Forms.HelpProvider
		Friend WithEvents _fileColumns As kCura.Windows.Forms.TwoListBox
		Friend WithEvents MenuItem3 As System.Windows.Forms.MenuItem
		Friend WithEvents _fileMenuCloseItem As System.Windows.Forms.MenuItem
		Friend WithEvents _extractMd5Hash As System.Windows.Forms.CheckBox
		Friend WithEvents _destinationFolderPath As System.Windows.Forms.ComboBox
		Friend WithEvents _buildFolderStructure As System.Windows.Forms.CheckBox
		Friend WithEvents Label8 As System.Windows.Forms.Label
		Friend WithEvents GroupBox5 As System.Windows.Forms.GroupBox
		Friend WithEvents MenuItem4 As System.Windows.Forms.MenuItem
		Friend WithEvents _fileRefreshMenuItem As System.Windows.Forms.MenuItem
		Friend WithEvents Label9 As System.Windows.Forms.Label
		Friend WithEvents _overwriteDropdown As System.Windows.Forms.ComboBox
		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
			Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(LoadFileForm))
			Me.OpenFileDialog = New System.Windows.Forms.OpenFileDialog
			Me.GroupBox1 = New System.Windows.Forms.GroupBox
			Me._importDestinationText = New System.Windows.Forms.TextBox
			Me.MainMenu = New System.Windows.Forms.MainMenu
			Me.MenuItem1 = New System.Windows.Forms.MenuItem
			Me._fileLoadFieldMapMenuItem = New System.Windows.Forms.MenuItem
			Me._fileSaveFieldMapMenuItem = New System.Windows.Forms.MenuItem
			Me.MenuItem3 = New System.Windows.Forms.MenuItem
			Me._fileMenuCloseItem = New System.Windows.Forms.MenuItem
			Me.MenuItem4 = New System.Windows.Forms.MenuItem
			Me._fileRefreshMenuItem = New System.Windows.Forms.MenuItem
			Me.MenuItem2 = New System.Windows.Forms.MenuItem
			Me.PreviewMenuFile = New System.Windows.Forms.MenuItem
			Me._importMenuPreviewErrorsItem = New System.Windows.Forms.MenuItem
			Me.ImportFileMenu = New System.Windows.Forms.MenuItem
			Me._saveFieldMapDialog = New System.Windows.Forms.SaveFileDialog
			Me._loadFieldMapDialog = New System.Windows.Forms.OpenFileDialog
			Me.TabControl1 = New System.Windows.Forms.TabControl
			Me._loadFileTab = New System.Windows.Forms.TabPage
			Me.GroupBox20 = New System.Windows.Forms.GroupBox
			Me._browseButton = New System.Windows.Forms.Button
			Me._filePath = New System.Windows.Forms.TextBox
			Me._firstLineContainsColumnNames = New System.Windows.Forms.CheckBox
			Me.GroupBox2 = New System.Windows.Forms.GroupBox
			Me._fileColumnHeaders = New System.Windows.Forms.ListBox
			Me.GroupBox23 = New System.Windows.Forms.GroupBox
			Me._multiRecordDelimiter = New System.Windows.Forms.ComboBox
			Me.Label6 = New System.Windows.Forms.Label
			Me.Label4 = New System.Windows.Forms.Label
			Me._quoteDelimiter = New System.Windows.Forms.ComboBox
			Me.Label3 = New System.Windows.Forms.Label
			Me._newLineDelimiter = New System.Windows.Forms.ComboBox
			Me.Label2 = New System.Windows.Forms.Label
			Me._recordDelimiter = New System.Windows.Forms.ComboBox
			Me._fieldMapTab = New System.Windows.Forms.TabPage
			Me.GroupBox5 = New System.Windows.Forms.GroupBox
			Me._buildFolderStructure = New System.Windows.Forms.CheckBox
			Me._destinationFolderPath = New System.Windows.Forms.ComboBox
			Me.GroupBox4 = New System.Windows.Forms.GroupBox
			Me._extractMd5Hash = New System.Windows.Forms.CheckBox
			Me._loadNativeFiles = New System.Windows.Forms.CheckBox
			Me._extractFullTextFromNativeFile = New System.Windows.Forms.CheckBox
			Me._nativeFilePathField = New System.Windows.Forms.ComboBox
			Me.Label5 = New System.Windows.Forms.Label
			Me.GroupBox3 = New System.Windows.Forms.GroupBox
			Me._overwriteDropdown = New System.Windows.Forms.ComboBox
			Me.Label9 = New System.Windows.Forms.Label
			Me.Label8 = New System.Windows.Forms.Label
			Me._identifiersDropDown = New System.Windows.Forms.ComboBox
			Me.Label7 = New System.Windows.Forms.Label
			Me.Label1 = New System.Windows.Forms.Label
			Me._fileColumns = New kCura.Windows.Forms.TwoListBox
			Me._fieldMap = New kCura.Windows.Forms.TwoListBox
			Me.HelpProvider1 = New System.Windows.Forms.HelpProvider
			Me.GroupBox1.SuspendLayout()
			Me.TabControl1.SuspendLayout()
			Me._loadFileTab.SuspendLayout()
			Me.GroupBox20.SuspendLayout()
			Me.GroupBox2.SuspendLayout()
			Me.GroupBox23.SuspendLayout()
			Me._fieldMapTab.SuspendLayout()
			Me.GroupBox5.SuspendLayout()
			Me.GroupBox4.SuspendLayout()
			Me.GroupBox3.SuspendLayout()
			Me.SuspendLayout()
			'
			'OpenFileDialog
			'
			Me.OpenFileDialog.Filter = "All files (*.*)|*.*|Text Files (*.txt)|*.txt|Log Files (*.log)|*.log|DAT Files|*." & _
			"dat"
			'
			'GroupBox1
			'
			Me.GroupBox1.Controls.Add(Me._importDestinationText)
			Me.GroupBox1.Location = New System.Drawing.Point(8, 4)
			Me.GroupBox1.Name = "GroupBox1"
			Me.GroupBox1.Size = New System.Drawing.Size(720, 40)
			Me.GroupBox1.TabIndex = 8
			Me.GroupBox1.TabStop = False
			Me.GroupBox1.Text = "Import Destination"
			'
			'_importDestinationText
			'
			Me._importDestinationText.BorderStyle = System.Windows.Forms.BorderStyle.None
			Me._importDestinationText.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me._importDestinationText.Location = New System.Drawing.Point(8, 20)
			Me._importDestinationText.Name = "_importDestinationText"
			Me._importDestinationText.ReadOnly = True
			Me._importDestinationText.Size = New System.Drawing.Size(704, 13)
			Me._importDestinationText.TabIndex = 5
			Me._importDestinationText.Text = ""
			'
			'MainMenu
			'
			Me.MainMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuItem1, Me.MenuItem2})
			'
			'MenuItem1
			'
			Me.MenuItem1.Index = 0
			Me.MenuItem1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me._fileLoadFieldMapMenuItem, Me._fileSaveFieldMapMenuItem, Me.MenuItem3, Me._fileMenuCloseItem, Me.MenuItem4, Me._fileRefreshMenuItem})
			Me.MenuItem1.Text = "&File"
			'
			'_fileLoadFieldMapMenuItem
			'
			Me._fileLoadFieldMapMenuItem.Index = 0
			Me._fileLoadFieldMapMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlO
			Me._fileLoadFieldMapMenuItem.Text = "Load Field Map"
			'
			'_fileSaveFieldMapMenuItem
			'
			Me._fileSaveFieldMapMenuItem.Index = 1
			Me._fileSaveFieldMapMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlS
			Me._fileSaveFieldMapMenuItem.Text = "Save Field Map"
			'
			'MenuItem3
			'
			Me.MenuItem3.Index = 2
			Me.MenuItem3.Text = "-"
			'
			'_fileMenuCloseItem
			'
			Me._fileMenuCloseItem.Index = 3
			Me._fileMenuCloseItem.Shortcut = System.Windows.Forms.Shortcut.CtrlW
			Me._fileMenuCloseItem.Text = "Close"
			'
			'MenuItem4
			'
			Me.MenuItem4.Index = 4
			Me.MenuItem4.Text = "-"
			'
			'_fileRefreshMenuItem
			'
			Me._fileRefreshMenuItem.Index = 5
			Me._fileRefreshMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlR
			Me._fileRefreshMenuItem.Text = "Refresh"
			'
			'MenuItem2
			'
			Me.MenuItem2.Index = 1
			Me.MenuItem2.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.PreviewMenuFile, Me._importMenuPreviewErrorsItem, Me.ImportFileMenu})
			Me.MenuItem2.Text = "&Import"
			'
			'PreviewMenuFile
			'
			Me.PreviewMenuFile.Index = 0
			Me.PreviewMenuFile.Shortcut = System.Windows.Forms.Shortcut.F6
			Me.PreviewMenuFile.Text = "&Preview File..."
			'
			'_importMenuPreviewErrorsItem
			'
			Me._importMenuPreviewErrorsItem.Index = 1
			Me._importMenuPreviewErrorsItem.Text = "Preview Errors..."
			'
			'ImportFileMenu
			'
			Me.ImportFileMenu.Index = 2
			Me.ImportFileMenu.Shortcut = System.Windows.Forms.Shortcut.F5
			Me.ImportFileMenu.Text = "&Import File..."
			'
			'_saveFieldMapDialog
			'
			Me._saveFieldMapDialog.CreatePrompt = True
			Me._saveFieldMapDialog.DefaultExt = "kwe"
			Me._saveFieldMapDialog.Filter = "WinEDDS native load files (*.kwe)|*.kwe|All files (*.*)|*.*"
			Me._saveFieldMapDialog.RestoreDirectory = True
			'
			'_loadFieldMapDialog
			'
			Me._loadFieldMapDialog.DefaultExt = "kwe"
			Me._loadFieldMapDialog.Filter = "WinEDDS native load files (*.kwe)|*.kwe|All files (*.*)|*.*"
			Me._loadFieldMapDialog.Title = "Open Saved Field Map"
			'
			'TabControl1
			'
			Me.TabControl1.Controls.Add(Me._loadFileTab)
			Me.TabControl1.Controls.Add(Me._fieldMapTab)
			Me.TabControl1.Location = New System.Drawing.Point(4, 48)
			Me.TabControl1.Name = "TabControl1"
			Me.TabControl1.SelectedIndex = 0
			Me.TabControl1.Size = New System.Drawing.Size(740, 424)
			Me.TabControl1.TabIndex = 21
			'
			'_loadFileTab
			'
			Me._loadFileTab.Controls.Add(Me.GroupBox20)
			Me._loadFileTab.Controls.Add(Me._firstLineContainsColumnNames)
			Me._loadFileTab.Controls.Add(Me.GroupBox2)
			Me._loadFileTab.Controls.Add(Me.GroupBox23)
			Me._loadFileTab.Location = New System.Drawing.Point(4, 22)
			Me._loadFileTab.Name = "_loadFileTab"
			Me._loadFileTab.Size = New System.Drawing.Size(732, 398)
			Me._loadFileTab.TabIndex = 0
			Me._loadFileTab.Text = "Load File"
			'
			'GroupBox20
			'
			Me.GroupBox20.Controls.Add(Me._browseButton)
			Me.GroupBox20.Controls.Add(Me._filePath)
			Me.GroupBox20.Location = New System.Drawing.Point(8, 4)
			Me.GroupBox20.Name = "GroupBox20"
			Me.GroupBox20.Size = New System.Drawing.Size(596, 48)
			Me.GroupBox20.TabIndex = 21
			Me.GroupBox20.TabStop = False
			Me.GroupBox20.Text = "Load File"
			'
			'_browseButton
			'
			Me._browseButton.Location = New System.Drawing.Point(560, 16)
			Me._browseButton.Name = "_browseButton"
			Me._browseButton.Size = New System.Drawing.Size(24, 20)
			Me._browseButton.TabIndex = 4
			Me._browseButton.Text = "..."
			'
			'_filePath
			'
			Me._filePath.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me._filePath.Location = New System.Drawing.Point(16, 16)
			Me._filePath.Name = "_filePath"
			Me._filePath.Size = New System.Drawing.Size(544, 20)
			Me._filePath.TabIndex = 2
			Me._filePath.Text = "Select a file ..."
			'
			'_firstLineContainsColumnNames
			'
			Me._firstLineContainsColumnNames.CheckAlign = System.Drawing.ContentAlignment.TopLeft
			Me._firstLineContainsColumnNames.Checked = True
			Me._firstLineContainsColumnNames.CheckState = System.Windows.Forms.CheckState.Checked
			Me._firstLineContainsColumnNames.Location = New System.Drawing.Point(12, 60)
			Me._firstLineContainsColumnNames.Name = "_firstLineContainsColumnNames"
			Me._firstLineContainsColumnNames.Size = New System.Drawing.Size(136, 36)
			Me._firstLineContainsColumnNames.TabIndex = 20
			Me._firstLineContainsColumnNames.Text = "First line contains column names"
			'
			'GroupBox2
			'
			Me.GroupBox2.Controls.Add(Me._fileColumnHeaders)
			Me.GroupBox2.Location = New System.Drawing.Point(168, 56)
			Me.GroupBox2.Name = "GroupBox2"
			Me.GroupBox2.Size = New System.Drawing.Size(436, 336)
			Me.GroupBox2.TabIndex = 19
			Me.GroupBox2.TabStop = False
			Me.GroupBox2.Text = "File Column Headers"
			'
			'_fileColumnHeaders
			'
			Me._fileColumnHeaders.Location = New System.Drawing.Point(12, 24)
			Me._fileColumnHeaders.Name = "_fileColumnHeaders"
			Me._fileColumnHeaders.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
			Me._fileColumnHeaders.Size = New System.Drawing.Size(412, 303)
			Me._fileColumnHeaders.TabIndex = 17
			'
			'GroupBox23
			'
			Me.GroupBox23.Controls.Add(Me._multiRecordDelimiter)
			Me.GroupBox23.Controls.Add(Me.Label6)
			Me.GroupBox23.Controls.Add(Me.Label4)
			Me.GroupBox23.Controls.Add(Me._quoteDelimiter)
			Me.GroupBox23.Controls.Add(Me.Label3)
			Me.GroupBox23.Controls.Add(Me._newLineDelimiter)
			Me.GroupBox23.Controls.Add(Me.Label2)
			Me.GroupBox23.Controls.Add(Me._recordDelimiter)
			Me.GroupBox23.Location = New System.Drawing.Point(12, 100)
			Me.GroupBox23.Name = "GroupBox23"
			Me.GroupBox23.Size = New System.Drawing.Size(148, 216)
			Me.GroupBox23.TabIndex = 6
			Me.GroupBox23.TabStop = False
			Me.GroupBox23.Text = "Characters"
			'
			'_multiRecordDelimiter
			'
			Me._multiRecordDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._multiRecordDelimiter.Location = New System.Drawing.Point(16, 180)
			Me._multiRecordDelimiter.Name = "_multiRecordDelimiter"
			Me._multiRecordDelimiter.Size = New System.Drawing.Size(120, 21)
			Me._multiRecordDelimiter.TabIndex = 7
			'
			'Label6
			'
			Me.Label6.Location = New System.Drawing.Point(16, 164)
			Me.Label6.Name = "Label6"
			Me.Label6.Size = New System.Drawing.Size(120, 16)
			Me.Label6.TabIndex = 6
			Me.Label6.Text = "Multi-Value Delimiter"
			'
			'Label4
			'
			Me.Label4.Location = New System.Drawing.Point(16, 68)
			Me.Label4.Name = "Label4"
			Me.Label4.Size = New System.Drawing.Size(100, 16)
			Me.Label4.TabIndex = 5
			Me.Label4.Text = "Quote"
			'
			'_quoteDelimiter
			'
			Me._quoteDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._quoteDelimiter.Location = New System.Drawing.Point(16, 88)
			Me._quoteDelimiter.Name = "_quoteDelimiter"
			Me._quoteDelimiter.Size = New System.Drawing.Size(120, 21)
			Me._quoteDelimiter.TabIndex = 4
			'
			'Label3
			'
			Me.Label3.Location = New System.Drawing.Point(16, 116)
			Me.Label3.Name = "Label3"
			Me.Label3.Size = New System.Drawing.Size(100, 16)
			Me.Label3.TabIndex = 3
			Me.Label3.Text = "Newline"
			'
			'_newLineDelimiter
			'
			Me._newLineDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._newLineDelimiter.Location = New System.Drawing.Point(16, 132)
			Me._newLineDelimiter.Name = "_newLineDelimiter"
			Me._newLineDelimiter.Size = New System.Drawing.Size(120, 21)
			Me._newLineDelimiter.TabIndex = 2
			'
			'Label2
			'
			Me.Label2.Location = New System.Drawing.Point(16, 20)
			Me.Label2.Name = "Label2"
			Me.Label2.Size = New System.Drawing.Size(100, 16)
			Me.Label2.TabIndex = 1
			Me.Label2.Text = "Record Delimiter"
			'
			'_recordDelimiter
			'
			Me._recordDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._recordDelimiter.Location = New System.Drawing.Point(16, 36)
			Me._recordDelimiter.Name = "_recordDelimiter"
			Me._recordDelimiter.Size = New System.Drawing.Size(120, 21)
			Me._recordDelimiter.TabIndex = 0
			'
			'_fieldMapTab
			'
			Me._fieldMapTab.Controls.Add(Me.GroupBox5)
			Me._fieldMapTab.Controls.Add(Me.GroupBox4)
			Me._fieldMapTab.Controls.Add(Me.GroupBox3)
			Me._fieldMapTab.Controls.Add(Me.Label7)
			Me._fieldMapTab.Controls.Add(Me.Label1)
			Me._fieldMapTab.Controls.Add(Me._fileColumns)
			Me._fieldMapTab.Controls.Add(Me._fieldMap)
			Me._fieldMapTab.Location = New System.Drawing.Point(4, 22)
			Me._fieldMapTab.Name = "_fieldMapTab"
			Me._fieldMapTab.Size = New System.Drawing.Size(732, 398)
			Me._fieldMapTab.TabIndex = 1
			Me._fieldMapTab.Text = "Field Map"
			'
			'GroupBox5
			'
			Me.GroupBox5.Controls.Add(Me._buildFolderStructure)
			Me.GroupBox5.Controls.Add(Me._destinationFolderPath)
			Me.GroupBox5.Location = New System.Drawing.Point(492, 4)
			Me.GroupBox5.Name = "GroupBox5"
			Me.GroupBox5.Size = New System.Drawing.Size(236, 72)
			Me.GroupBox5.TabIndex = 30
			Me.GroupBox5.TabStop = False
			Me.GroupBox5.Text = "Folder Info"
			'
			'_buildFolderStructure
			'
			Me._buildFolderStructure.Location = New System.Drawing.Point(8, 20)
			Me._buildFolderStructure.Name = "_buildFolderStructure"
			Me._buildFolderStructure.Size = New System.Drawing.Size(220, 16)
			Me._buildFolderStructure.TabIndex = 29
			Me._buildFolderStructure.Text = "Folder information contained in column:"
			'
			'_destinationFolderPath
			'
			Me._destinationFolderPath.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._destinationFolderPath.Enabled = False
			Me._destinationFolderPath.Location = New System.Drawing.Point(8, 40)
			Me._destinationFolderPath.Name = "_destinationFolderPath"
			Me._destinationFolderPath.Size = New System.Drawing.Size(220, 21)
			Me._destinationFolderPath.TabIndex = 28
			'
			'GroupBox4
			'
			Me.GroupBox4.Controls.Add(Me._extractMd5Hash)
			Me.GroupBox4.Controls.Add(Me._loadNativeFiles)
			Me.GroupBox4.Controls.Add(Me._extractFullTextFromNativeFile)
			Me.GroupBox4.Controls.Add(Me._nativeFilePathField)
			Me.GroupBox4.Controls.Add(Me.Label5)
			Me.GroupBox4.Location = New System.Drawing.Point(176, 4)
			Me.GroupBox4.Name = "GroupBox4"
			Me.GroupBox4.Size = New System.Drawing.Size(312, 112)
			Me.GroupBox4.TabIndex = 26
			Me.GroupBox4.TabStop = False
			Me.GroupBox4.Text = "Native File Behavior"
			'
			'_extractMd5Hash
			'
			Me._extractMd5Hash.Location = New System.Drawing.Point(8, 36)
			Me._extractMd5Hash.Name = "_extractMd5Hash"
			Me._extractMd5Hash.Size = New System.Drawing.Size(116, 20)
			Me._extractMd5Hash.TabIndex = 26
			Me._extractMd5Hash.Text = "Extract MD5 Hash"
			'
			'_loadNativeFiles
			'
			Me._loadNativeFiles.Location = New System.Drawing.Point(8, 16)
			Me._loadNativeFiles.Name = "_loadNativeFiles"
			Me._loadNativeFiles.Size = New System.Drawing.Size(116, 20)
			Me._loadNativeFiles.TabIndex = 22
			Me._loadNativeFiles.Text = "Load Native Files"
			'
			'_extractFullTextFromNativeFile
			'
			Me._extractFullTextFromNativeFile.Enabled = False
			Me._extractFullTextFromNativeFile.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
			Me._extractFullTextFromNativeFile.Location = New System.Drawing.Point(124, 16)
			Me._extractFullTextFromNativeFile.Name = "_extractFullTextFromNativeFile"
			Me._extractFullTextFromNativeFile.Size = New System.Drawing.Size(180, 20)
			Me._extractFullTextFromNativeFile.TabIndex = 23
			Me._extractFullTextFromNativeFile.Text = "Extract full text from native files"
			Me._extractFullTextFromNativeFile.Visible = False
			'
			'_nativeFilePathField
			'
			Me._nativeFilePathField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._nativeFilePathField.Enabled = False
			Me._nativeFilePathField.Location = New System.Drawing.Point(8, 80)
			Me._nativeFilePathField.Name = "_nativeFilePathField"
			Me._nativeFilePathField.Size = New System.Drawing.Size(288, 21)
			Me._nativeFilePathField.TabIndex = 24
			'
			'Label5
			'
			Me.Label5.Location = New System.Drawing.Point(8, 64)
			Me.Label5.Name = "Label5"
			Me.Label5.Size = New System.Drawing.Size(192, 13)
			Me.Label5.TabIndex = 25
			Me.Label5.Text = "Native file paths contained in column:"
			Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
			'
			'GroupBox3
			'
			Me.GroupBox3.Controls.Add(Me._overwriteDropdown)
			Me.GroupBox3.Controls.Add(Me.Label9)
			Me.GroupBox3.Controls.Add(Me.Label8)
			Me.GroupBox3.Controls.Add(Me._identifiersDropDown)
			Me.GroupBox3.Location = New System.Drawing.Point(4, 4)
			Me.GroupBox3.Name = "GroupBox3"
			Me.GroupBox3.Size = New System.Drawing.Size(168, 92)
			Me.GroupBox3.TabIndex = 21
			Me.GroupBox3.TabStop = False
			Me.GroupBox3.Text = "Identity and Update Behavior"
			'
			'_overwriteDropdown
			'
			Me._overwriteDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._overwriteDropdown.Items.AddRange(New Object() {"None", "Strict", "Append"})
			Me._overwriteDropdown.Location = New System.Drawing.Point(60, 64)
			Me._overwriteDropdown.Name = "_overwriteDropdown"
			Me._overwriteDropdown.Size = New System.Drawing.Size(100, 21)
			Me._overwriteDropdown.TabIndex = 28
			'
			'Label9
			'
			Me.Label9.Location = New System.Drawing.Point(7, 68)
			Me.Label9.Name = "Label9"
			Me.Label9.Size = New System.Drawing.Size(56, 13)
			Me.Label9.TabIndex = 27
			Me.Label9.Text = "Overwrite:"
			Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
			'
			'Label8
			'
			Me.Label8.Location = New System.Drawing.Point(6, 18)
			Me.Label8.Name = "Label8"
			Me.Label8.Size = New System.Drawing.Size(82, 13)
			Me.Label8.TabIndex = 26
			Me.Label8.Text = "Group Identifier"
			Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
			'
			'_identifiersDropDown
			'
			Me._identifiersDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._identifiersDropDown.Enabled = False
			Me._identifiersDropDown.Location = New System.Drawing.Point(8, 32)
			Me._identifiersDropDown.Name = "_identifiersDropDown"
			Me._identifiersDropDown.Size = New System.Drawing.Size(152, 21)
			Me._identifiersDropDown.TabIndex = 19
			'
			'Label7
			'
			Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me.Label7.Location = New System.Drawing.Point(652, 112)
			Me.Label7.Name = "Label7"
			Me.Label7.Size = New System.Drawing.Size(76, 16)
			Me.Label7.TabIndex = 4
			Me.Label7.Text = "File Columns"
			'
			'Label1
			'
			Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me.Label1.Location = New System.Drawing.Point(4, 112)
			Me.Label1.Name = "Label1"
			Me.Label1.Size = New System.Drawing.Size(100, 16)
			Me.Label1.TabIndex = 3
			Me.Label1.Text = "Case Fields"
			'
			'_fileColumns
			'
			Me._fileColumns.KeepButtonsCentered = False
			Me._fileColumns.LeftOrderControlsVisible = True
			Me._fileColumns.Location = New System.Drawing.Point(372, 132)
			Me._fileColumns.Name = "_fileColumns"
			Me._fileColumns.RightOrderControlVisible = False
			Me._fileColumns.Size = New System.Drawing.Size(392, 276)
			Me._fileColumns.TabIndex = 2
			'
			'_fieldMap
			'
			Me._fieldMap.KeepButtonsCentered = False
			Me._fieldMap.LeftOrderControlsVisible = False
			Me._fieldMap.Location = New System.Drawing.Point(4, 132)
			Me._fieldMap.Name = "_fieldMap"
			Me._fieldMap.RightOrderControlVisible = True
			Me._fieldMap.Size = New System.Drawing.Size(360, 276)
			Me._fieldMap.TabIndex = 1
			'
			'LoadFileForm
			'
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.ClientSize = New System.Drawing.Size(746, 475)
			Me.Controls.Add(Me.TabControl1)
			Me.Controls.Add(Me.GroupBox1)
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
			Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
			Me.MaximizeBox = False
			Me.Menu = Me.MainMenu
			Me.Name = "LoadFileForm"
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
			Me.Text = "Relativity Desktop Client | Import Load File"
			Me.GroupBox1.ResumeLayout(False)
			Me.TabControl1.ResumeLayout(False)
			Me._loadFileTab.ResumeLayout(False)
			Me.GroupBox20.ResumeLayout(False)
			Me.GroupBox2.ResumeLayout(False)
			Me.GroupBox23.ResumeLayout(False)
			Me._fieldMapTab.ResumeLayout(False)
			Me.GroupBox5.ResumeLayout(False)
			Me.GroupBox4.ResumeLayout(False)
			Me.GroupBox3.ResumeLayout(False)
			Me.ResumeLayout(False)

		End Sub

#End Region

		Friend WithEvents _application As kCura.EDDS.WinForm.Application

		Private _loadFile As kCura.WinEDDS.LoadFile

		Friend ReadOnly Property ReadyToRun() As Boolean
			Get
				If Not Me.EnsureConnection() Then Return False
				Dim rtr As Boolean
				If _loadNativeFiles.Checked Then
					rtr = _nativeFilePathField.SelectedIndex <> -1
				Else
					rtr = True
				End If
				If rtr AndAlso _buildFolderStructure.Checked Then
					rtr = _destinationFolderPath.SelectedIndex <> -1 AndAlso rtr
				End If
				Return _
				 _fieldMap.RightListBoxItems.Count > 0 AndAlso _
				 _fileColumns.LeftListBoxItems.Count > 0 AndAlso _
				 rtr AndAlso _
				 System.IO.File.Exists(_filePath.Text) AndAlso _
				_identifiersDropDown.SelectedIndex <> -1
			End Get
		End Property

		Private Sub PopulateLoadFileObject()
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			Me.PopulateLoadFileDelimiters()
			If Not Me.EnsureConnection() Then Exit Sub
			Me.LoadFile.FieldMap = kCura.EDDS.WinForm.Utility.ExtractFieldMap(_fieldMap, _fileColumns, _application.CurrentFields)
			If _identifiersDropDown.SelectedIndex > -1 Then
				Dim columnname As String = CType(_identifiersDropDown.SelectedItem, String)
				Dim openParenIndex As Int32 = columnname.LastIndexOf("("c) + 1
				Dim closeParenIndex As Int32 = columnname.LastIndexOf(")"c)
				Dim fieldColumnIndex As Int32 = Int32.Parse(columnname.Substring(openParenIndex, closeParenIndex - openParenIndex)) - 1
				Dim groupIdentifier As DocumentField = _application.CurrentGroupIdentifierField
				If Not groupIdentifier Is Nothing Then
					Me.LoadFile.FieldMap.Add(New kCura.WinEDDS.LoadFileFieldMap.LoadFileFieldMapItem(groupIdentifier, fieldColumnIndex))
				End If
			End If
			LoadFile.ExtractFullTextFromNativeFile = _extractFullTextFromNativeFile.Checked
			LoadFile.LoadNativeFiles = _loadNativeFiles.Checked
			If _overwriteDropdown.SelectedItem Is Nothing Then
				LoadFile.OverwriteDestination = "None"
			Else
				LoadFile.OverwriteDestination = _overwriteDropdown.SelectedItem.ToString
			End If
			LoadFile.ExtractMD5HashFromNativeFile = _extractMd5Hash.Enabled AndAlso _extractMd5Hash.Checked
			LoadFile.FirstLineContainsHeaders = _firstLineContainsColumnNames.Checked
			If System.IO.File.Exists(_filePath.Text) Then
				LoadFile.FilePath = _filePath.Text
			End If
			LoadFile.SelectedIdentifierField = _application.GetDocumentFieldFromName(_application.GetCaseIdentifierFields(0))
			If Not _identifiersDropDown.SelectedItem Is Nothing Then LoadFile.GroupIdentifierColumn = _identifiersDropDown.SelectedItem.ToString
			If _loadNativeFiles.Checked Then
				If Not _nativeFilePathField.SelectedItem Is Nothing Then
					LoadFile.NativeFilePathColumn = _nativeFilePathField.SelectedItem.ToString
				Else
					LoadFile.NativeFilePathColumn = Nothing
				End If
			End If
			If LoadFile.OverwriteDestination.ToLower <> "strict" AndAlso LoadFile.OverwriteDestination.ToLower <> "append" Then
				LoadFile.CreateFolderStructure = _buildFolderStructure.Checked
				If LoadFile.CreateFolderStructure Then
					If Not _destinationFolderPath.SelectedItem Is Nothing Then
						LoadFile.FolderStructureContainedInColumn = _destinationFolderPath.SelectedItem.ToString
					Else
						LoadFile.FolderStructureContainedInColumn = Nothing
					End If
				End If
			Else
				LoadFile.CreateFolderStructure = False
			End If
			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Sub

		Public Sub LoadFormControls(ByVal loadFileObjectUpdatedFromFile As Boolean)
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			kCura.EDDS.WinForm.Utility.InitializeCharacterDropDown(_recordDelimiter, _loadFile.RecordDelimiter)
			kCura.EDDS.WinForm.Utility.InitializeCharacterDropDown(_quoteDelimiter, _loadFile.QuoteDelimiter)
			kCura.EDDS.WinForm.Utility.InitializeCharacterDropDown(_newLineDelimiter, _loadFile.NewlineDelimiter)
			kCura.EDDS.WinForm.Utility.InitializeCharacterDropDown(_multiRecordDelimiter, _loadFile.MultiRecordDelimiter)
			_filePath.Text = LoadFile.FilePath
			_importDestinationText.Text = _application.GetCaseFolderPath(LoadFile.DestinationFolderID)
			_fieldMap.ClearAll()
			_fileColumns.ClearAll()
			_fileColumnHeaders.Items.Clear()
			_nativeFilePathField.Items.Clear()
			_destinationFolderPath.Items.Clear()
			_identifiersDropDown.Items.Clear()
			_loadNativeFiles.Checked = LoadFile.LoadNativeFiles
			RefreshNativeFilePathFieldAndFileColumnHeaders()
			If Not Me.EnsureConnection() Then Exit Sub
			Dim caseFields As String() = _application.GetCaseFields(LoadFile.CaseInfo.ArtifactID)
			Dim caseFieldName As String
			If loadFileObjectUpdatedFromFile Then
				Dim columnHeaders As String()
				If System.IO.File.Exists(Me.LoadFile.FilePath) Then
					columnHeaders = _application.GetColumnHeadersFromLoadFile(Me.LoadFile, _firstLineContainsColumnNames.Checked)
				Else
					MsgBox("The load file specified does not exist.", MsgBoxStyle.Exclamation, "WinRelativity Warning")
					columnHeaders = New String() {}
				End If

				BuildMappingFromLoadFile(caseFields, columnHeaders)
				If LoadFile.LoadNativeFiles Then
					_loadNativeFiles.Checked = True
					If LoadFile.NativeFilePathColumn <> String.Empty Then
						_nativeFilePathField.SelectedItem = LoadFile.NativeFilePathColumn
					End If
					_extractFullTextFromNativeFile.Checked = LoadFile.ExtractFullTextFromNativeFile
				End If
				_buildFolderStructure.Checked = LoadFile.CreateFolderStructure
				ActionMenuEnabled = ReadyToRun
			Else
				_fieldMap.LeftListBoxItems.AddRange(caseFields)
			End If
			'_identifiersDropDown.Items.AddRange(_application.IdentiferFieldDropdownPopulator)
			_overwriteDropdown.SelectedItem = LoadFile.OverwriteDestination
			_identifiersDropDown.Enabled = True			'LoadFile.OverwriteDestination
			If _overwriteDropdown.SelectedItem Is Nothing Then
				_destinationFolderPath.Enabled = _buildFolderStructure.Checked
			Else
				_destinationFolderPath.Enabled = Not (_overwriteDropdown.SelectedItem.ToString.ToLower = "strict" OrElse _overwriteDropdown.SelectedItem.ToString.ToLower = "append") AndAlso _buildFolderStructure.Checked
			End If
			If loadFileObjectUpdatedFromFile AndAlso _destinationFolderPath.Enabled Then
				Dim i As Int32
				For i = 0 To _destinationFolderPath.Items.Count - 1
					If DirectCast(_destinationFolderPath.Items(i), String).ToLower = Me.LoadFile.FolderStructureContainedInColumn.ToLower Then
						_destinationFolderPath.SelectedIndex = i
						Exit For
					End If
				Next
			End If
			If Not Me.LoadFile.GroupIdentifierColumn Is Nothing AndAlso Me.LoadFile.GroupIdentifierColumn <> "" AndAlso _
			_identifiersDropDown.Items.Contains(LoadFile.GroupIdentifierColumn) Then
				_identifiersDropDown.SelectedItem = LoadFile.GroupIdentifierColumn
			End If

			'If LoadFile.OverwriteDestination AndAlso Not LoadFile.SelectedIdentifierField Is Nothing Then
			'	_overWrite.Checked = True
			'	caseFieldName = _application.GetSelectedIdentifier(LoadFile.SelectedIdentifierField)
			'	If caseFieldName <> String.Empty Then
			'		_identifiersDropDown.SelectedItem = caseFieldName
			'	End If
			'End If
			_extractMd5Hash.Enabled = EnableMd5Hash
			ActionMenuEnabled = ReadyToRun
			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Sub

		Public Property LoadFile() As kCura.WinEDDS.LoadFile
			Get
				If _loadFile Is Nothing Then
					_loadFile = New kCura.WinEDDS.LoadFile
				End If
				If _loadFile.CookieContainer Is Nothing Then
					_loadFile.CookieContainer = kCura.EDDS.WinForm.Application.Instance.CookieContainer
				End If
				'If _loadFile.Identity Is Nothing Then
				'	_loadFile.Identity = kCura.EDDS.WinForm.Application.Instance.Identity
				'End If
				Return _loadFile
			End Get
			Set(ByVal value As kCura.WinEDDS.LoadFile)
				_loadFile = value
				Me.LoadFormControls(False)
			End Set
		End Property

		Private Sub _browseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _browseButton.Click
			OpenFileDialog.ShowDialog()
		End Sub

		Private Function RefreshNativeFilePathFieldAndFileColumnHeaders() As String()
			Dim columnHeaders As String()
			_fileColumnHeaders.Items.Clear()
			_nativeFilePathField.Items.Clear()
			_destinationFolderPath.Items.Clear()
			_identifiersDropDown.Items.Clear()
			_fileColumns.ClearAll()
			If System.IO.File.Exists(LoadFile.FilePath) Then
				PopulateLoadFileDelimiters()
				columnHeaders = _application.GetColumnHeadersFromLoadFile(LoadFile, _firstLineContainsColumnNames.Checked)
				'_filePath.Text = LoadFile.FilePath\
				_fileColumns.RightListBoxItems.AddRange(columnHeaders)
				_fileColumnHeaders.Items.AddRange(columnHeaders)
				_nativeFilePathField.Items.AddRange(columnHeaders)
				_destinationFolderPath.Items.AddRange(columnHeaders)
				_identifiersDropDown.Items.AddRange(columnHeaders)
				If LoadFile.LoadNativeFiles AndAlso System.IO.File.Exists(LoadFile.FilePath) Then
					_nativeFilePathField.SelectedItem = LoadFile.NativeFilePathColumn
				End If
			Else
			End If
			_nativeFilePathField.SelectedItem = Nothing
			_nativeFilePathField.Text = "Select ..."
			_destinationFolderPath.SelectedItem = Nothing
			_destinationFolderPath.Text = "Select ..."
			ActionMenuEnabled = ReadyToRun
			Return columnHeaders
		End Function

		Private Sub OpenFileDialog_FileOk(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog.FileOk
			Dim oldfilepath As String
			Try
				If Not Me.EnsureConnection Then Exit Sub
				oldfilepath = _filePath.Text
				_filePath.Text = OpenFileDialog.FileName
				PopulateLoadFileObject()
				RefreshNativeFilePathFieldAndFileColumnHeaders()
			Catch ex As System.IO.IOException
				MsgBox(ex.Message & Environment.NewLine & "Please close any application that might have a hold on the file before proceeding.", MsgBoxStyle.Exclamation)
				_filePath.Text = oldfilepath
			End Try
		End Sub

		Private Sub _filePath_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _filePath.TextChanged
			ActionMenuEnabled = ReadyToRun
			LoadFile.FilePath = _filePath.Text
			RefreshNativeFilePathFieldAndFileColumnHeaders()
		End Sub

		Private Sub ImportFileMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ImportFileMenu.Click
			PopulateLoadFileObject()
			If _application.ReadyToLoad(Utility.ExtractFieldNames(_fieldMap.LeftListBoxItems)) Then
				_application.ImportLoadFile(Me.LoadFile)
			End If
		End Sub

		Private Sub LoadFileForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
			'kCura.Windows.Forms.EnhancedMenuProvider.Hook(Me)
		End Sub

		Private Sub LoadFileForm_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
			'kCura.Windows.Forms.EnhancedMenuProvider.Unhook()
		End Sub

		Private Sub _loadNativeFiles_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _loadNativeFiles.CheckedChanged
			_nativeFilePathField.Enabled = _loadNativeFiles.Checked
			_extractFullTextFromNativeFile.Enabled = _loadNativeFiles.Checked
			'If Not _nativeFilePathField.Items.Count = 0 Then
			'	_nativeFilePathField.SelectedItem = _nativeFilePathField.Items(0)
			'End If
			_nativeFilePathField.SelectedItem = Nothing
			_nativeFilePathField.Text = "Select ..."
			_extractMd5Hash.Enabled = EnableMd5Hash
			ActionMenuEnabled = ReadyToRun
		End Sub

		Private Sub _overwriteDestination_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _overwriteDropdown.SelectedIndexChanged
			LoadFile.OverwriteDestination = _overwriteDropdown.SelectedItem.ToString
			Select Case LoadFile.OverwriteDestination.ToLower
				Case "none"
					_buildFolderStructure.Enabled = True
					_destinationFolderPath.Enabled = _buildFolderStructure.Checked
				Case Else
					_destinationFolderPath.Enabled = False
					_buildFolderStructure.Checked = False
					_buildFolderStructure.Enabled = False
					_destinationFolderPath.SelectedItem = Nothing
					_destinationFolderPath.Text = "Select ..."
			End Select
			ActionMenuEnabled = ReadyToRun
			'_identifiersDropDown.Enabled = _overWrite.Checked
		End Sub

		Private Sub PreviewMenuFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PreviewMenuFile.Click
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			PopulateLoadFileObject()
			_application.PreviewLoadFile(_loadFile, False)
			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Sub

		Private Sub _fileSaveFieldMapMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _fileSaveFieldMapMenuItem.Click
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			PopulateLoadFileObject()
			_saveFieldMapDialog.ShowDialog()
			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Sub

		Private Sub _saveFieldMapDialog_FileOk(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles _saveFieldMapDialog.FileOk
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			If Not System.IO.File.Exists(_saveFieldMapDialog.FileName) Then
				System.IO.File.Create(_saveFieldMapDialog.FileName).Close()
			End If
			'PopulateLoadFileObject()
			_application.SaveLoadFile(Me.LoadFile, _saveFieldMapDialog.FileName)
			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Sub

		Private Sub _fileLoadFieldMapMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _fileLoadFieldMapMenuItem.Click
			_loadFieldMapDialog.ShowDialog()
		End Sub

		Private Sub _loadFieldMapDialog_FileOk(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles _loadFieldMapDialog.FileOk
			Dim newLoadFile As LoadFile = _application.ReadLoadFile(Me.LoadFile, _loadFieldMapDialog.FileName)
			If Not newLoadFile Is Nothing Then
				_loadFile = newLoadFile
				Me.LoadFormControls(True)
			End If
		End Sub

		Private Sub _fieldMap_ItemsShifted() Handles _fieldMap.ItemsShifted
			ActionMenuEnabled = ReadyToRun
			_extractMd5Hash.Enabled = EnableMd5Hash
		End Sub

		Private ReadOnly Property EnableMd5Hash() As Boolean
			Get
				If Not _nativeFilePathField.Enabled Then Return False
				Dim item As String
				For Each item In _fieldMap.RightListBoxItems
					Try
						If _application.CurrentFields.Item(item).FieldCategoryID = kCura.DynamicFields.Types.FieldCategory.DuplicateHash Then
							Return False
						End If
					Catch
					End Try
				Next
				Return True
			End Get
		End Property

		Private Sub _fileColumns_ItemsShifted() Handles _fileColumns.ItemsShifted
			ActionMenuEnabled = ReadyToRun
		End Sub

		Private Property ActionMenuEnabled() As Boolean
			Get
				Return ImportFileMenu.Enabled AndAlso PreviewMenuFile.Enabled AndAlso _importMenuPreviewErrorsItem.Enabled
			End Get
			Set(ByVal value As Boolean)
				ImportFileMenu.Enabled = value
				PreviewMenuFile.Enabled = value
				_importMenuPreviewErrorsItem.Enabled = value
			End Set
		End Property

		Private Sub _nativeFilePathField_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _nativeFilePathField.SelectedIndexChanged
			ActionMenuEnabled = ReadyToRun
		End Sub

		Private Sub _firstLineContainsColumnNames_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _firstLineContainsColumnNames.CheckedChanged
			LoadFile.FirstLineContainsHeaders = _firstLineContainsColumnNames.Checked
			RefreshNativeFilePathFieldAndFileColumnHeaders()
		End Sub

		Private Sub _importMenuPreviewErrorsItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _importMenuPreviewErrorsItem.Click
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			PopulateLoadFileObject()
			_application.PreviewLoadFile(_loadFile, True)
			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Sub

		Private Sub _identifiersDropDown_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _identifiersDropDown.SelectedIndexChanged
			ActionMenuEnabled = ReadyToRun
		End Sub

		Private Sub _characterDropdown_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _recordDelimiter.SelectedIndexChanged, _quoteDelimiter.SelectedIndexChanged, _newLineDelimiter.SelectedIndexChanged, _multiRecordDelimiter.SelectedIndexChanged
			'PopulateLoadFileObject()
			RefreshNativeFilePathFieldAndFileColumnHeaders()
		End Sub

		Private Sub PopulateLoadFileDelimiters()
			LoadFile.QuoteDelimiter = Chr(CType(_quoteDelimiter.SelectedValue, Int32))
			LoadFile.RecordDelimiter = Chr(CType(_recordDelimiter.SelectedValue, Int32))
			LoadFile.MultiRecordDelimiter = Chr(CType(_multiRecordDelimiter.SelectedValue, Int32))
			LoadFile.NewlineDelimiter = Chr(CType(_newLineDelimiter.SelectedValue, Int32))
		End Sub

		Private Sub BuildMappingFromLoadFile(ByVal casefields As String(), ByVal columnHeaders As String())
			Dim caseFieldName As String
			Dim selectedFieldNameList As New ArrayList
			Dim selectedColumnNameList As New ArrayList
			Dim item As LoadFileFieldMap.LoadFileFieldMapItem
			_fieldMap.ClearAll()
			_fileColumns.ClearAll()
			For Each item In _loadFile.FieldMap
				If _
				 Not item.DocumentField Is Nothing AndAlso _
				 item.NativeFileColumnIndex > -1 AndAlso _
				 Array.IndexOf(casefields, item.DocumentField.FieldName) > -1 AndAlso _
				 item.NativeFileColumnIndex < columnHeaders.Length _
				 Then
					selectedFieldNameList.Add(item.DocumentField.FieldName)
					selectedColumnNameList.Add(columnHeaders(item.NativeFileColumnIndex))
				End If
			Next
			For Each item In _loadFile.FieldMap
				If Not item.DocumentField Is Nothing AndAlso columnHeaders.Length = 0 AndAlso Array.IndexOf(casefields, item.DocumentField.FieldName) > -1 Then
					selectedFieldNameList.Add(item.DocumentField.FieldName)
				End If
			Next
			Dim selectedFieldNames As String() = DirectCast(selectedFieldNameList.ToArray(GetType(String)), String())
			Dim selectedColumnNames As String() = DirectCast(selectedColumnNameList.ToArray(GetType(String)), String())
			_fieldMap.RightListBoxItems.AddRange(selectedFieldNames)
			_fileColumns.LeftListBoxItems.AddRange(selectedColumnNames)
			Dim name As String
			For Each name In casefields
				If Array.IndexOf(selectedFieldNames, name) = -1 Then
					_fieldMap.LeftListBoxItems.Add(name)
				End If
			Next
			For Each name In columnHeaders
				If Array.IndexOf(selectedColumnNames, name) = -1 Then
					_fileColumns.RightListBoxItems.Add(name)
				End If
			Next
		End Sub

		Private Sub _fileMenuCloseItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _fileMenuCloseItem.Click
			Me.Close()
		End Sub

		Private Sub _buildFolderStructure_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _buildFolderStructure.CheckedChanged
			If _buildFolderStructure.Checked Then
				_destinationFolderPath.Enabled = True
			Else
				_destinationFolderPath.Enabled = False
				_destinationFolderPath.SelectedItem = Nothing
				_destinationFolderPath.Text = "Select ..."
			End If
			ActionMenuEnabled = ReadyToRun
		End Sub

		Private Sub _destinationFolderPath_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _destinationFolderPath.SelectedIndexChanged
			ActionMenuEnabled = ReadyToRun
		End Sub

		Private Sub _fileRefreshMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _fileRefreshMenuItem.Click
			Dim caseFields As String() = _application.GetCaseFields(LoadFile.CaseInfo.ArtifactID, True)
			If caseFields Is Nothing Then Exit Sub
			Dim fieldName As String
			For Each fieldName In caseFields
				If Not _fieldMap.RightListBoxItems.Contains(fieldName) AndAlso Not _fieldMap.LeftListBoxItems.Contains(fieldName) Then
					_fieldMap.LeftListBoxItems.Add(fieldName)
				End If
			Next
			Dim itemsToRemove As New System.Collections.ArrayList
			For Each fieldName In _fieldMap.LeftListBoxItems
				If Array.IndexOf(caseFields, (fieldName)) = -1 Then
					itemsToRemove.Add(fieldName)
				End If
			Next
			For Each fieldName In itemsToRemove
				_fieldMap.LeftListBoxItems.Remove(fieldName)
			Next

			itemsToRemove = New Collections.ArrayList
			For Each fieldName In _fieldMap.RightListBoxItems
				If Array.IndexOf(caseFields, (fieldName)) = -1 Then
					itemsToRemove.Add(fieldName)
				End If
			Next
			For Each fieldName In itemsToRemove
				_fieldMap.RightListBoxItems.Remove(fieldName)
			Next
		End Sub

		Private Function EnsureConnection() As Boolean
			If Not _loadFile Is Nothing AndAlso Not _loadFile.CaseInfo Is Nothing Then
				Dim casefields As String() = Nothing
				Dim continue As Boolean = True
				Try
					casefields = _application.GetCaseFields(_loadFile.CaseInfo.ArtifactID, True)
					Return Not casefields Is Nothing
				Catch ex As System.Exception
					If ex.Message.IndexOf("Need To Re Login") <> -1 Then
						Return False
					Else
						Throw
					End If
				End Try
			Else
				Return True
			End If
		End Function
	End Class
End Namespace
