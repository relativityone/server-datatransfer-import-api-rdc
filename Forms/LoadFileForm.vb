Imports System.Collections.Generic
Imports kCura.Windows.Forms

Namespace kCura.EDDS.WinForm
	Public Class LoadFileForm
		Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "
		Public Sub New()
			MyBase.New()

			_application = kCura.EDDS.WinForm.Application.Instance

			'This call is required by the Windows Form Designer.
			InitializeComponent()

			'Add any initialization after the InitializeComponent() call
			InitializeDocumentSpecificComponents()

		End Sub
		Friend WithEvents _importMenuForceFolderPreviewItem As System.Windows.Forms.MenuItem

		Private ParentArtifactTypeID As Int32
		Private ReadOnly Property IsChildObject() As Boolean
			Get
				Return ParentArtifactTypeID <> 8
			End Get
		End Property

		Private Sub InitializeDocumentSpecificComponents()
			If Me.LoadFile.ArtifactTypeID = 0 Then Me.LoadFile.ArtifactTypeID = _application.ArtifactTypeID
			If Me.LoadFile.ArtifactTypeID = Relativity.ArtifactType.Document Then
				Me.GroupBoxNativeFileBehavior.Enabled = True
				Me.GroupBoxExtractedText.Enabled = True
				Me.GroupBoxFolderInfo.Text = "Folder Info"
				Me._buildFolderStructure.Text = "Folder Information Column"
				ParentArtifactTypeID = 8
			Else
				Dim parentQuery As New kCura.WinEDDS.Service.ObjectTypeManager(_application.Credential, _application.CookieContainer)
				ParentArtifactTypeID = CType(parentQuery.RetrieveParentArtifactTypeID(_application.SelectedCaseInfo.ArtifactID, _
				Me.LoadFile.ArtifactTypeID).Tables(0).Rows(0)("ParentArtifactTypeID"), Int32)
				Me.GroupBoxFolderInfo.Enabled = False
				If Me.IsChildObject Then
					Me.GroupBoxFolderInfo.Enabled = True
					_buildFolderStructure.Checked = True
				End If
				Me.GroupBoxNativeFileBehavior.Enabled = False
				If _application.HasFileField(Me.LoadFile.ArtifactTypeID, True) Then
					Me.GroupBoxNativeFileBehavior.Enabled = True
				End If
				Me.GroupBoxExtractedText.Enabled = False
			End If
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
		Friend WithEvents GroupBoxImportDestination As System.Windows.Forms.GroupBox
		Friend WithEvents _importDestinationText As System.Windows.Forms.Label
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
		Friend WithEvents _importMenuPreviewFoldersAndCodesItem As System.Windows.Forms.MenuItem
		Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
		Friend WithEvents _loadFileTab As System.Windows.Forms.TabPage
		Friend WithEvents _fieldMapTab As System.Windows.Forms.TabPage
		Friend WithEvents _loadNativeFiles As System.Windows.Forms.CheckBox
		Friend WithEvents Label5 As System.Windows.Forms.Label
		Friend WithEvents _nativeFilePathField As System.Windows.Forms.ComboBox
		Friend WithEvents GroupBoxNativeFileBehavior As System.Windows.Forms.GroupBox
		Friend WithEvents GroupBoxCharacters As System.Windows.Forms.GroupBox
		Friend WithEvents _multiRecordDelimiter As System.Windows.Forms.ComboBox
		Friend WithEvents LabelMultiValue As System.Windows.Forms.Label
		Friend WithEvents LabelQuote As System.Windows.Forms.Label
		Friend WithEvents _quoteDelimiter As System.Windows.Forms.ComboBox
		Friend WithEvents LabelNewline As System.Windows.Forms.Label
		Friend WithEvents _newLineDelimiter As System.Windows.Forms.ComboBox
		Friend WithEvents LabelColumn As System.Windows.Forms.Label
		Friend WithEvents _recordDelimiter As System.Windows.Forms.ComboBox
		Friend WithEvents GroupBoxFileColumnHeaders As System.Windows.Forms.GroupBox
		Friend WithEvents _fileColumnHeaders As System.Windows.Forms.ListBox
		Friend WithEvents _firstLineContainsColumnNames As System.Windows.Forms.CheckBox
		Friend WithEvents GroupBoxLoadFile As System.Windows.Forms.GroupBox
		Friend WithEvents _browseButton As System.Windows.Forms.Button
		Friend WithEvents _filePath As System.Windows.Forms.TextBox
		Friend WithEvents HelpProvider1 As System.Windows.Forms.HelpProvider
		Friend WithEvents MenuItem3 As System.Windows.Forms.MenuItem
		Friend WithEvents _fileMenuCloseItem As System.Windows.Forms.MenuItem
		Friend WithEvents _destinationFolderPath As System.Windows.Forms.ComboBox
		Friend WithEvents _buildFolderStructure As System.Windows.Forms.CheckBox
		Friend WithEvents GroupBoxFolderInfo As System.Windows.Forms.GroupBox
		Friend WithEvents MenuItem4 As System.Windows.Forms.MenuItem
		Friend WithEvents _fileRefreshMenuItem As System.Windows.Forms.MenuItem
		Friend WithEvents _overwriteDropdown As System.Windows.Forms.ComboBox
		Friend WithEvents GroupBoxOverwrite As System.Windows.Forms.GroupBox
		Friend WithEvents GroupBoxExtractedText As System.Windows.Forms.GroupBox
		Friend WithEvents _extractedTextValueContainsFileLocation As System.Windows.Forms.CheckBox
		Friend WithEvents LabelFileEncoding As System.Windows.Forms.Label
		Friend WithEvents _advancedButton As System.Windows.Forms.Button
		Friend WithEvents Label9 As System.Windows.Forms.Label
		Friend WithEvents _loadFileEncodingPicker As kCura.EDDS.WinForm.EncodingPicker
		Friend WithEvents _fullTextFileEncodingPicker As kCura.EDDS.WinForm.EncodingPicker
		Friend WithEvents _hierarchicalValueDelimiter As System.Windows.Forms.ComboBox
		Friend WithEvents LabelNestedValue As System.Windows.Forms.Label
		Friend WithEvents _startLineNumberLabel As System.Windows.Forms.Label
		Friend WithEvents _startLineNumber As System.Windows.Forms.NumericUpDown
		Friend WithEvents _fieldMap As kCura.WinEDDS.UIControls.FieldMap
		Friend WithEvents GroupBoxOverlayIdentifier As System.Windows.Forms.GroupBox
		Friend WithEvents _overlayIdentifier As System.Windows.Forms.ComboBox
		Friend WithEvents MenuItem5 As System.Windows.Forms.MenuItem
		Friend WithEvents _importMenuSendEmailNotificationItem As System.Windows.Forms.MenuItem

		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
			Me.components = New System.ComponentModel.Container
			Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(LoadFileForm))
			Me.OpenFileDialog = New System.Windows.Forms.OpenFileDialog
			Me.GroupBoxImportDestination = New System.Windows.Forms.GroupBox
			Me._importDestinationText = New System.Windows.Forms.Label
			Me.MainMenu = New System.Windows.Forms.MainMenu(Me.components)
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
			Me._importMenuPreviewFoldersAndCodesItem = New System.Windows.Forms.MenuItem
			Me.ImportFileMenu = New System.Windows.Forms.MenuItem
			Me.MenuItem5 = New System.Windows.Forms.MenuItem
			Me._importMenuSendEmailNotificationItem = New System.Windows.Forms.MenuItem
			Me._importMenuForceFolderPreviewItem = New System.Windows.Forms.MenuItem
			Me._saveFieldMapDialog = New System.Windows.Forms.SaveFileDialog
			Me._loadFieldMapDialog = New System.Windows.Forms.OpenFileDialog
			Me.TabControl1 = New System.Windows.Forms.TabControl
			Me._loadFileTab = New System.Windows.Forms.TabPage
			Me._startLineNumber = New System.Windows.Forms.NumericUpDown
			Me._startLineNumberLabel = New System.Windows.Forms.Label
			Me._loadFileEncodingPicker = New kCura.EDDS.WinForm.EncodingPicker
			Me.LabelFileEncoding = New System.Windows.Forms.Label
			Me.GroupBoxLoadFile = New System.Windows.Forms.GroupBox
			Me._browseButton = New System.Windows.Forms.Button
			Me._filePath = New System.Windows.Forms.TextBox
			Me._firstLineContainsColumnNames = New System.Windows.Forms.CheckBox
			Me.GroupBoxFileColumnHeaders = New System.Windows.Forms.GroupBox
			Me._fileColumnHeaders = New System.Windows.Forms.ListBox
			Me.GroupBoxCharacters = New System.Windows.Forms.GroupBox
			Me._hierarchicalValueDelimiter = New System.Windows.Forms.ComboBox
			Me.LabelNestedValue = New System.Windows.Forms.Label
			Me._multiRecordDelimiter = New System.Windows.Forms.ComboBox
			Me.LabelMultiValue = New System.Windows.Forms.Label
			Me.LabelQuote = New System.Windows.Forms.Label
			Me._quoteDelimiter = New System.Windows.Forms.ComboBox
			Me.LabelNewline = New System.Windows.Forms.Label
			Me._newLineDelimiter = New System.Windows.Forms.ComboBox
			Me.LabelColumn = New System.Windows.Forms.Label
			Me._recordDelimiter = New System.Windows.Forms.ComboBox
			Me._fieldMapTab = New System.Windows.Forms.TabPage
			Me.GroupBoxOverlayIdentifier = New System.Windows.Forms.GroupBox
			Me._overlayIdentifier = New System.Windows.Forms.ComboBox
			Me._fieldMap = New kCura.WinEDDS.UIControls.FieldMap
			Me.GroupBoxExtractedText = New System.Windows.Forms.GroupBox
			Me._fullTextFileEncodingPicker = New kCura.EDDS.WinForm.EncodingPicker
			Me.Label9 = New System.Windows.Forms.Label
			Me._extractedTextValueContainsFileLocation = New System.Windows.Forms.CheckBox
			Me.GroupBoxOverwrite = New System.Windows.Forms.GroupBox
			Me._overwriteDropdown = New System.Windows.Forms.ComboBox
			Me.GroupBoxFolderInfo = New System.Windows.Forms.GroupBox
			Me._buildFolderStructure = New System.Windows.Forms.CheckBox
			Me._destinationFolderPath = New System.Windows.Forms.ComboBox
			Me.GroupBoxNativeFileBehavior = New System.Windows.Forms.GroupBox
			Me._advancedButton = New System.Windows.Forms.Button
			Me._loadNativeFiles = New System.Windows.Forms.CheckBox
			Me._nativeFilePathField = New System.Windows.Forms.ComboBox
			Me.Label5 = New System.Windows.Forms.Label
			Me.HelpProvider1 = New System.Windows.Forms.HelpProvider
			Me.GroupBoxImportDestination.SuspendLayout()
			Me.TabControl1.SuspendLayout()
			Me._loadFileTab.SuspendLayout()
			CType(Me._startLineNumber, System.ComponentModel.ISupportInitialize).BeginInit()
			Me.GroupBoxLoadFile.SuspendLayout()
			Me.GroupBoxFileColumnHeaders.SuspendLayout()
			Me.GroupBoxCharacters.SuspendLayout()
			Me._fieldMapTab.SuspendLayout()
			Me.GroupBoxOverlayIdentifier.SuspendLayout()
			Me.GroupBoxExtractedText.SuspendLayout()
			Me.GroupBoxOverwrite.SuspendLayout()
			Me.GroupBoxFolderInfo.SuspendLayout()
			Me.GroupBoxNativeFileBehavior.SuspendLayout()
			Me.SuspendLayout()
			'
			'OpenFileDialog
			'
			Me.OpenFileDialog.Filter = "All files (*.*)|*.*|CSV Files (*.csv)|*.csv|Text Files (*.txt)|*.txt|DAT Files|*." & _
			 "dat"
			'
			'GroupBoxImportDestination
			'
			Me.GroupBoxImportDestination.Controls.Add(Me._importDestinationText)
			Me.GroupBoxImportDestination.Location = New System.Drawing.Point(7, 4)
			Me.GroupBoxImportDestination.Name = "GroupBoxImportDestination"
			Me.GroupBoxImportDestination.Size = New System.Drawing.Size(737, 40)
			Me.GroupBoxImportDestination.TabIndex = 8
			Me.GroupBoxImportDestination.TabStop = False
			Me.GroupBoxImportDestination.Text = "Import Destination"
			Me.GroupBoxImportDestination.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			'
			'_importDestinationText
			'
			Me._importDestinationText.BorderStyle = System.Windows.Forms.BorderStyle.None
			Me._importDestinationText.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me._importDestinationText.Location = New System.Drawing.Point(8, 20)
			Me._importDestinationText.Name = "_importDestinationText"
			Me._importDestinationText.Size = New System.Drawing.Size(717, 18)
			Me._importDestinationText.TabIndex = 5
			Me._importDestinationText.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
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
			Me._fileRefreshMenuItem.Shortcut = System.Windows.Forms.Shortcut.F5
			Me._fileRefreshMenuItem.Text = "Refresh"
			'
			'MenuItem2
			'
			Me.MenuItem2.Index = 1
			Me.MenuItem2.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.PreviewMenuFile, Me._importMenuPreviewErrorsItem, Me._importMenuPreviewFoldersAndCodesItem, Me.ImportFileMenu, Me.MenuItem5, Me._importMenuSendEmailNotificationItem, Me._importMenuForceFolderPreviewItem})
			Me.MenuItem2.Text = "&Import"
			'
			'PreviewMenuFile
			'
			Me.PreviewMenuFile.Index = 0
			Me.PreviewMenuFile.Text = "&Preview File..."
			'
			'_importMenuPreviewErrorsItem
			'
			Me._importMenuPreviewErrorsItem.Index = 1
			Me._importMenuPreviewErrorsItem.Text = "Preview Errors..."
			'
			'_importMenuPreviewFoldersAndCodesItem
			'
			Me._importMenuPreviewFoldersAndCodesItem.Enabled = False
			Me._importMenuPreviewFoldersAndCodesItem.Index = 2
			Me._importMenuPreviewFoldersAndCodesItem.Text = "Preview Choices And Folders..."
			'
			'ImportFileMenu
			'
			Me.ImportFileMenu.Index = 3
			Me.ImportFileMenu.Text = "&Import File..."
			'
			'MenuItem5
			'
			Me.MenuItem5.Index = 4
			Me.MenuItem5.Text = "-"
			'
			'_importMenuSendEmailNotificationItem
			'
			Me._importMenuSendEmailNotificationItem.Index = 5
			Me._importMenuSendEmailNotificationItem.Text = "Send email notification on completion"
			'
			'_importMenuForceFolderPreviewItem
			'
			Me._importMenuForceFolderPreviewItem.Index = 6
			Me._importMenuForceFolderPreviewItem.Text = "Force folder preview"
			'
			'_saveFieldMapDialog
			'
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
			Me.TabControl1.Location = New System.Drawing.Point(7, 48)
			Me.TabControl1.Name = "TabControl1"
			Me.TabControl1.SelectedIndex = 0
			Me.TabControl1.Size = New System.Drawing.Size(738, 488)
			Me.TabControl1.TabIndex = 21
			'
			'_loadFileTab
			'
			Me._loadFileTab.Controls.Add(Me._startLineNumber)
			Me._loadFileTab.Controls.Add(Me._startLineNumberLabel)
			Me._loadFileTab.Controls.Add(Me._loadFileEncodingPicker)
			Me._loadFileTab.Controls.Add(Me.LabelFileEncoding)
			Me._loadFileTab.Controls.Add(Me.GroupBoxLoadFile)
			Me._loadFileTab.Controls.Add(Me._firstLineContainsColumnNames)
			Me._loadFileTab.Controls.Add(Me.GroupBoxFileColumnHeaders)
			Me._loadFileTab.Controls.Add(Me.GroupBoxCharacters)
			Me._loadFileTab.Location = New System.Drawing.Point(4, 22)
			Me._loadFileTab.Name = "_loadFileTab"
			Me._loadFileTab.Size = New System.Drawing.Size(728, 462)
			Me._loadFileTab.TabIndex = 0
			Me._loadFileTab.Text = "Load File"
			'
			'_startLineNumber
			'
			Me._startLineNumber.Location = New System.Drawing.Point(68, 84)
			Me._startLineNumber.Maximum = New Decimal(New Integer() {268435455, 1042612833, 542101086, 0})
			Me._startLineNumber.Name = "_startLineNumber"
			Me._startLineNumber.Size = New System.Drawing.Size(148, 20)
			Me._startLineNumber.TabIndex = 4
			'
			'_startLineNumberLabel
			'
			Me._startLineNumberLabel.Location = New System.Drawing.Point(12, 84)
			Me._startLineNumberLabel.Name = "_startLineNumberLabel"
			Me._startLineNumberLabel.Size = New System.Drawing.Size(56, 20)
			Me._startLineNumberLabel.TabIndex = 26
			Me._startLineNumberLabel.Text = "Start Line"
			Me._startLineNumberLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
			'
			'_loadFileEncodingPicker
			'
			Me._loadFileEncodingPicker.Location = New System.Drawing.Point(12, 127)
			Me._loadFileEncodingPicker.Name = "_loadFileEncodingPicker"
			Me._loadFileEncodingPicker.SelectedEncoding = Nothing
			Me._loadFileEncodingPicker.Size = New System.Drawing.Size(200, 21)
			Me._loadFileEncodingPicker.TabIndex = 5
			'
			'LabelFileEncoding
			'
			Me.LabelFileEncoding.Location = New System.Drawing.Point(12, 111)
			Me.LabelFileEncoding.Name = "LabelFileEncoding"
			Me.LabelFileEncoding.Size = New System.Drawing.Size(200, 16)
			Me.LabelFileEncoding.TabIndex = 23
			Me.LabelFileEncoding.Text = "File Encoding"
			'
			'GroupBoxLoadFile
			'
			Me.GroupBoxLoadFile.Controls.Add(Me._browseButton)
			Me.GroupBoxLoadFile.Controls.Add(Me._filePath)
			Me.GroupBoxLoadFile.Location = New System.Drawing.Point(8, 4)
			Me.GroupBoxLoadFile.Name = "GroupBoxLoadFile"
			Me.GroupBoxLoadFile.Size = New System.Drawing.Size(718, 48)
			Me.GroupBoxLoadFile.TabIndex = 2
			Me.GroupBoxLoadFile.TabStop = False
			Me.GroupBoxLoadFile.Text = "Load File"
			Me.GroupBoxLoadFile.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			'
			'_browseButton
			'
			Me._browseButton.Location = New System.Drawing.Point(684, 16)
			Me._browseButton.Name = "_browseButton"
			Me._browseButton.Size = New System.Drawing.Size(24, 20)
			Me._browseButton.TabIndex = 4
			Me._browseButton.Text = "..."
			Me._browseButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			'
			'_filePath
			'
			Me._filePath.BackColor = System.Drawing.SystemColors.ControlLightLight
			Me._filePath.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me._filePath.ForeColor = System.Drawing.SystemColors.ControlDarkDark
			Me._filePath.Location = New System.Drawing.Point(8, 16)
			Me._filePath.Name = "_filePath"
			Me._filePath.Size = New System.Drawing.Size(676, 20)
			Me._filePath.TabIndex = 2
			Me._filePath.Text = "Select a file ..."
			Me._filePath.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			'
			'_firstLineContainsColumnNames
			'
			Me._firstLineContainsColumnNames.CheckAlign = System.Drawing.ContentAlignment.TopLeft
			Me._firstLineContainsColumnNames.Checked = True
			Me._firstLineContainsColumnNames.CheckState = System.Windows.Forms.CheckState.Checked
			Me._firstLineContainsColumnNames.Location = New System.Drawing.Point(12, 60)
			Me._firstLineContainsColumnNames.Name = "_firstLineContainsColumnNames"
			Me._firstLineContainsColumnNames.Size = New System.Drawing.Size(204, 20)
			Me._firstLineContainsColumnNames.TabIndex = 3
			Me._firstLineContainsColumnNames.Text = "First line contains column names"
			'
			'GroupBoxFileColumnHeaders
			'
			Me.GroupBoxFileColumnHeaders.Controls.Add(Me._fileColumnHeaders)
			Me.GroupBoxFileColumnHeaders.Location = New System.Drawing.Point(228, 56)
			Me.GroupBoxFileColumnHeaders.Name = "GroupBoxFileColumnHeaders"
			Me.GroupBoxFileColumnHeaders.Size = New System.Drawing.Size(500, 403)
			Me.GroupBoxFileColumnHeaders.TabIndex = 7
			Me.GroupBoxFileColumnHeaders.TabStop = False
			Me.GroupBoxFileColumnHeaders.Text = "File Column Headers"
			'
			'_fileColumnHeaders
			'
			Me._fileColumnHeaders.Location = New System.Drawing.Point(10, 18)
			Me._fileColumnHeaders.Name = "_fileColumnHeaders"
			Me._fileColumnHeaders.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
			Me._fileColumnHeaders.Size = New System.Drawing.Size(480, 375)
			Me._fileColumnHeaders.TabIndex = 17
			Me._fileColumnHeaders.IntegralHeight = False
			'
			'GroupBoxCharacters
			'
			Me.GroupBoxCharacters.Controls.Add(Me._hierarchicalValueDelimiter)
			Me.GroupBoxCharacters.Controls.Add(Me.LabelNestedValue)
			Me.GroupBoxCharacters.Controls.Add(Me._multiRecordDelimiter)
			Me.GroupBoxCharacters.Controls.Add(Me.LabelMultiValue)
			Me.GroupBoxCharacters.Controls.Add(Me.LabelQuote)
			Me.GroupBoxCharacters.Controls.Add(Me._quoteDelimiter)
			Me.GroupBoxCharacters.Controls.Add(Me.LabelNewline)
			Me.GroupBoxCharacters.Controls.Add(Me._newLineDelimiter)
			Me.GroupBoxCharacters.Controls.Add(Me.LabelColumn)
			Me.GroupBoxCharacters.Controls.Add(Me._recordDelimiter)
			Me.GroupBoxCharacters.Location = New System.Drawing.Point(12, 156)
			Me.GroupBoxCharacters.Name = "GroupBoxCharacters"
			Me.GroupBoxCharacters.Size = New System.Drawing.Size(200, 303)
			Me.GroupBoxCharacters.TabIndex = 6
			Me.GroupBoxCharacters.TabStop = False
			Me.GroupBoxCharacters.Text = "Characters"
			'
			'_hierarchicalValueDelimiter
			'
			Me._hierarchicalValueDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._hierarchicalValueDelimiter.Location = New System.Drawing.Point(16, 226)
			Me._hierarchicalValueDelimiter.Name = "_hierarchicalValueDelimiter"
			Me._hierarchicalValueDelimiter.Size = New System.Drawing.Size(168, 21)
			Me._hierarchicalValueDelimiter.TabIndex = 9
			'
			'LabelNestedValue
			'
			Me.LabelNestedValue.Location = New System.Drawing.Point(16, 212)
			Me.LabelNestedValue.Name = "LabelNestedValue"
			Me.LabelNestedValue.Size = New System.Drawing.Size(160, 16)
			Me.LabelNestedValue.TabIndex = 8
			Me.LabelNestedValue.Text = "Nested Value "
			'
			'_multiRecordDelimiter
			'
			Me._multiRecordDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._multiRecordDelimiter.Location = New System.Drawing.Point(16, 180)
			Me._multiRecordDelimiter.Name = "_multiRecordDelimiter"
			Me._multiRecordDelimiter.Size = New System.Drawing.Size(168, 21)
			Me._multiRecordDelimiter.TabIndex = 7
			'
			'LabelMultiValue
			'
			Me.LabelMultiValue.Location = New System.Drawing.Point(16, 164)
			Me.LabelMultiValue.Name = "LabelMultiValue"
			Me.LabelMultiValue.Size = New System.Drawing.Size(120, 16)
			Me.LabelMultiValue.TabIndex = 6
			Me.LabelMultiValue.Text = "Multi-Value "
			'
			'LabelQuote
			'
			Me.LabelQuote.Location = New System.Drawing.Point(16, 72)
			Me.LabelQuote.Name = "LabelQuote"
			Me.LabelQuote.Size = New System.Drawing.Size(100, 16)
			Me.LabelQuote.TabIndex = 5
			Me.LabelQuote.Text = "Quote"
			'
			'_quoteDelimiter
			'
			Me._quoteDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._quoteDelimiter.Location = New System.Drawing.Point(16, 88)
			Me._quoteDelimiter.Name = "_quoteDelimiter"
			Me._quoteDelimiter.Size = New System.Drawing.Size(168, 21)
			Me._quoteDelimiter.TabIndex = 2
			'
			'LabelNewline
			'
			Me.LabelNewline.Location = New System.Drawing.Point(16, 116)
			Me.LabelNewline.Name = "LabelNewline"
			Me.LabelNewline.Size = New System.Drawing.Size(100, 16)
			Me.LabelNewline.TabIndex = 3
			Me.LabelNewline.Text = "Newline"
			'
			'_newLineDelimiter
			'
			Me._newLineDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._newLineDelimiter.Location = New System.Drawing.Point(16, 132)
			Me._newLineDelimiter.Name = "_newLineDelimiter"
			Me._newLineDelimiter.Size = New System.Drawing.Size(168, 21)
			Me._newLineDelimiter.TabIndex = 4
			'
			'LabelColumn
			'
			Me.LabelColumn.Location = New System.Drawing.Point(16, 20)
			Me.LabelColumn.Name = "LabelColumn"
			Me.LabelColumn.Size = New System.Drawing.Size(100, 16)
			Me.LabelColumn.TabIndex = 1
			Me.LabelColumn.Text = "Column"
			'
			'_recordDelimiter
			'
			Me._recordDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._recordDelimiter.Location = New System.Drawing.Point(16, 36)
			Me._recordDelimiter.Name = "_recordDelimiter"
			Me._recordDelimiter.Size = New System.Drawing.Size(168, 21)
			Me._recordDelimiter.TabIndex = 0
			'
			'_fieldMapTab
			'
			Me._fieldMapTab.Controls.Add(Me.GroupBoxOverlayIdentifier)
			Me._fieldMapTab.Controls.Add(Me._fieldMap)
			Me._fieldMapTab.Controls.Add(Me.GroupBoxExtractedText)
			Me._fieldMapTab.Controls.Add(Me.GroupBoxOverwrite)
			Me._fieldMapTab.Controls.Add(Me.GroupBoxFolderInfo)
			Me._fieldMapTab.Controls.Add(Me.GroupBoxNativeFileBehavior)
			Me._fieldMapTab.Location = New System.Drawing.Point(4, 22)
			Me._fieldMapTab.Name = "_fieldMapTab"
			Me._fieldMapTab.Size = New System.Drawing.Size(728, 462)
			Me._fieldMapTab.TabIndex = 1
			Me._fieldMapTab.Text = "Field Map"
			'
			'GroupBoxOverlayIdentifier
			'
			Me.GroupBoxOverlayIdentifier.Controls.Add(Me._overlayIdentifier)
			Me.GroupBoxOverlayIdentifier.Location = New System.Drawing.Point(4, 352)
			Me.GroupBoxOverlayIdentifier.Name = "GroupBoxOverlayIdentifier"
			Me.GroupBoxOverlayIdentifier.Size = New System.Drawing.Size(234, 56)
			Me.GroupBoxOverlayIdentifier.TabIndex = 11
			Me.GroupBoxOverlayIdentifier.TabStop = False
			Me.GroupBoxOverlayIdentifier.Text = "Overlay Identifier"
			'
			'_overlayIdentifier
			'
			Me._overlayIdentifier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._overlayIdentifier.Enabled = False
			Me._overlayIdentifier.Location = New System.Drawing.Point(8, 24)
			Me._overlayIdentifier.Name = "_overlayIdentifier"
			Me._overlayIdentifier.Size = New System.Drawing.Size(220, 21)
			Me._overlayIdentifier.TabIndex = 28
			'
			'_fieldMap
			'
			Me._fieldMap.Location = New System.Drawing.Point(4, 0)
			Me._fieldMap.Name = "_fieldMap"
			Me._fieldMap.Size = New System.Drawing.Size(720, 288)
			Me._fieldMap.TabIndex = 0
			'
			'GroupBoxExtractedText
			'
			Me.GroupBoxExtractedText.Controls.Add(Me._fullTextFileEncodingPicker)
			Me.GroupBoxExtractedText.Controls.Add(Me.Label9)
			Me.GroupBoxExtractedText.Controls.Add(Me._extractedTextValueContainsFileLocation)
			Me.GroupBoxExtractedText.Location = New System.Drawing.Point(484, 288)
			Me.GroupBoxExtractedText.Name = "GroupBox7"
			Me.GroupBoxExtractedText.Size = New System.Drawing.Size(234, 104)
			Me.GroupBoxExtractedText.TabIndex = 14
			Me.GroupBoxExtractedText.TabStop = False
			Me.GroupBoxExtractedText.Text = "Extracted Text"
			'
			'_fullTextFileEncodingPicker
			'
			Me._fullTextFileEncodingPicker.Location = New System.Drawing.Point(12, 68)
			Me._fullTextFileEncodingPicker.Name = "_fullTextFileEncodingPicker"
			Me._fullTextFileEncodingPicker.SelectedEncoding = Nothing
			Me._fullTextFileEncodingPicker.Size = New System.Drawing.Size(200, 21)
			Me._fullTextFileEncodingPicker.TabIndex = 26
			'
			'Label9
			'
			Me.Label9.Location = New System.Drawing.Point(8, 48)
			Me.Label9.Name = "Label9"
			Me.Label9.Size = New System.Drawing.Size(204, 16)
			Me.Label9.TabIndex = 1
			Me.Label9.Text = "Encoding for undetectable files"
			'
			'_extractedTextValueContainsFileLocation
			'
			Me._extractedTextValueContainsFileLocation.Location = New System.Drawing.Point(8, 16)
			Me._extractedTextValueContainsFileLocation.Name = "_extractedTextValueContainsFileLocation"
			Me._extractedTextValueContainsFileLocation.Size = New System.Drawing.Size(156, 21)
			Me._extractedTextValueContainsFileLocation.TabIndex = 25
			Me._extractedTextValueContainsFileLocation.Text = "Cell contains file location"
			'
			'GroupBoxOverwrite
			'
			Me.GroupBoxOverwrite.Controls.Add(Me._overwriteDropdown)
			Me.GroupBoxOverwrite.Location = New System.Drawing.Point(4, 288)
			Me.GroupBoxOverwrite.Name = "GroupBoxOverwrite"
			Me.GroupBoxOverwrite.Size = New System.Drawing.Size(234, 56)
			Me.GroupBoxOverwrite.TabIndex = 10
			Me.GroupBoxOverwrite.TabStop = False
			Me.GroupBoxOverwrite.Text = "Overwrite"
			'
			'_overwriteDropdown
			'
			Me._overwriteDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._overwriteDropdown.Items.AddRange(New Object() {"Append Only", "Overlay Only", "Append/Overlay"})
			Me._overwriteDropdown.Location = New System.Drawing.Point(8, 24)
			Me._overwriteDropdown.Name = "_overwriteDropdown"
			Me._overwriteDropdown.Size = New System.Drawing.Size(220, 21)
			Me._overwriteDropdown.TabIndex = 28
			'
			'GroupBoxFolderInfo
			'
			Me.GroupBoxFolderInfo.Controls.Add(Me._buildFolderStructure)
			Me.GroupBoxFolderInfo.Controls.Add(Me._destinationFolderPath)
			Me.GroupBoxFolderInfo.Location = New System.Drawing.Point(244, 288)
			Me.GroupBoxFolderInfo.Name = "GroupBoxFolderInfo"
			Me.GroupBoxFolderInfo.Size = New System.Drawing.Size(234, 72)
			Me.GroupBoxFolderInfo.TabIndex = 12
			Me.GroupBoxFolderInfo.TabStop = False
			Me.GroupBoxFolderInfo.Text = "Parent Info"
			'
			'_buildFolderStructure
			'
			Me._buildFolderStructure.Location = New System.Drawing.Point(8, 20)
			Me._buildFolderStructure.Name = "_buildFolderStructure"
			Me._buildFolderStructure.Size = New System.Drawing.Size(160, 16)
			Me._buildFolderStructure.TabIndex = 20
			Me._buildFolderStructure.Text = "Parent information column:"
			'
			'_destinationFolderPath
			'
			Me._destinationFolderPath.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._destinationFolderPath.Enabled = False
			Me._destinationFolderPath.Location = New System.Drawing.Point(8, 40)
			Me._destinationFolderPath.Name = "_destinationFolderPath"
			Me._destinationFolderPath.Size = New System.Drawing.Size(220, 21)
			Me._destinationFolderPath.TabIndex = 21
			'
			'GroupBoxNativeFileBehavior
			'
			Me.GroupBoxNativeFileBehavior.Controls.Add(Me._advancedButton)
			Me.GroupBoxNativeFileBehavior.Controls.Add(Me._loadNativeFiles)
			Me.GroupBoxNativeFileBehavior.Controls.Add(Me._nativeFilePathField)
			Me.GroupBoxNativeFileBehavior.Controls.Add(Me.Label5)
			Me.GroupBoxNativeFileBehavior.Location = New System.Drawing.Point(244, 364)
			Me.GroupBoxNativeFileBehavior.Name = "GroupBoxNativeFileBehavior"
			Me.GroupBoxNativeFileBehavior.Size = New System.Drawing.Size(234, 92)
			Me.GroupBoxNativeFileBehavior.TabIndex = 13
			Me.GroupBoxNativeFileBehavior.TabStop = False
			Me.GroupBoxNativeFileBehavior.Text = "Native File Behavior"
			'
			'_advancedButton
			'
			Me._advancedButton.Location = New System.Drawing.Point(152, 16)
			Me._advancedButton.Name = "_advancedButton"
			Me._advancedButton.Size = New System.Drawing.Size(75, 23)
			Me._advancedButton.TabIndex = 23
			Me._advancedButton.Text = "Repository"
			'
			'_loadNativeFiles
			'
			Me._loadNativeFiles.Location = New System.Drawing.Point(8, 16)
			Me._loadNativeFiles.Name = "_loadNativeFiles"
			Me._loadNativeFiles.Size = New System.Drawing.Size(116, 20)
			Me._loadNativeFiles.TabIndex = 22
			Me._loadNativeFiles.Text = "Load Native Files"
			'
			'_nativeFilePathField
			'
			Me._nativeFilePathField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._nativeFilePathField.Enabled = False
			Me._nativeFilePathField.Location = New System.Drawing.Point(8, 64)
			Me._nativeFilePathField.Name = "_nativeFilePathField"
			Me._nativeFilePathField.Size = New System.Drawing.Size(220, 21)
			Me._nativeFilePathField.TabIndex = 24
			'
			'Label5
			'
			Me.Label5.Location = New System.Drawing.Point(8, 48)
			Me.Label5.Name = "Label5"
			Me.Label5.Size = New System.Drawing.Size(192, 13)
			Me.Label5.TabIndex = 25
			Me.Label5.Text = "Native file paths contained in column:"
			Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
			'
			'LoadFileForm
			'
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.ClientSize = New System.Drawing.Size(754, 547)
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable
			Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
			Me.Controls.Add(Me.TabControl1)
			Me.Controls.Add(Me.GroupBoxImportDestination)
			Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
			Me.MaximizeBox = True
			Me.Menu = Me.MainMenu
			Me.Name = "LoadFileForm"
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
			Me.Text = "Relativity Desktop Client | Import Load File"

			Me.GroupBoxImportDestination.ResumeLayout(False)
			Me.GroupBoxImportDestination.PerformLayout()
			Me.TabControl1.ResumeLayout(False)
			Me._loadFileTab.ResumeLayout(False)
			CType(Me._startLineNumber, System.ComponentModel.ISupportInitialize).EndInit()
			Me.GroupBoxLoadFile.ResumeLayout(False)
			Me.GroupBoxLoadFile.PerformLayout()
			Me.GroupBoxFileColumnHeaders.ResumeLayout(False)
			Me.GroupBoxCharacters.ResumeLayout(False)
			Me._fieldMapTab.ResumeLayout(False)
			Me.GroupBoxOverlayIdentifier.ResumeLayout(False)
			Me.GroupBoxExtractedText.ResumeLayout(False)
			Me.GroupBoxOverwrite.ResumeLayout(False)
			Me.GroupBoxFolderInfo.ResumeLayout(False)
			Me.GroupBoxNativeFileBehavior.ResumeLayout(False)
			Me.MinimumSize = New System.Drawing.Size(762, 560)
			Me.ResumeLayout(False)

		End Sub
#End Region

		Friend WithEvents _application As kCura.EDDS.WinForm.Application
		Private WithEvents _advancedFileForm As AdvancedFileLocation
		Private _loadFile As kCura.WinEDDS.LoadFile

		Friend ReadOnly Property ReadyToRun() As Boolean
			Get
				Return True
			End Get
		End Property

		Private Function GetOverwrite() As String
			If _overwriteDropdown.SelectedItem Is Nothing Then Return "None"
			Select Case _overwriteDropdown.SelectedItem.ToString.ToLower
				Case "append only"
					Return "None"
				Case "overlay only"
					Return "Strict"
				Case "append/overlay"
					Return "Append"
				Case Else
					Throw New IndexOutOfRangeException("'" & _overwriteDropdown.SelectedItem.ToString.ToLower & "' isn't a valid option.")
			End Select
		End Function

		Private Function GetOverwriteDropdownItem(ByVal input As String) As String
			Select Case input.ToLower
				Case "none"
					Return "Append Only"
				Case "strict"
					Return "Overlay Only"
				Case "append"
					Return "Append/Overlay"
				Case Else
					Throw New IndexOutOfRangeException("'" & input.ToLower & "' isn't a valid option.")
			End Select
		End Function

		Private Function GetSuitableKeyFields() As DocumentField()
			Dim retval As New System.Collections.ArrayList
			For Each field As DocumentField In _application.CurrentFields(Me.LoadFile.ArtifactTypeID, True)
				If (field.FieldCategory = Relativity.FieldCategory.Generic OrElse field.FieldCategory = Relativity.FieldCategory.Identifier) AndAlso field.FieldTypeID = Relativity.FieldTypeHelper.FieldType.Varchar Then
					If field.FieldCategory = Relativity.FieldCategory.Identifier Then field.FieldName &= " [Identifier]"
					retval.Add(field)
				End If
			Next
			retval.Sort(New DocumentFieldCollection.FieldNameComparer)
			Return DirectCast(retval.ToArray(GetType(DocumentField)), DocumentField())
		End Function

		Private Sub AppendErrorMessage(ByVal msg As System.Text.StringBuilder, ByVal errorText As String)
			msg.Append(" - ").Append(errorText).Append(vbNewLine)
		End Sub

		Private Function PopulateLoadFileObject(ByVal doFormValidation As Boolean) As Boolean
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			If doFormValidation Then
				Dim msg As New System.Text.StringBuilder

				If Not Me.EnsureConnection() Then Return False
				If _loadNativeFiles.Checked AndAlso _nativeFilePathField.SelectedIndex = -1 Then Me.AppendErrorMessage(msg, "Native file field unselected")
				If Me.LoadFile.ArtifactTypeID = Relativity.ArtifactType.Document Then
					If _buildFolderStructure.Checked AndAlso _destinationFolderPath.SelectedIndex = -1 Then Me.AppendErrorMessage(msg, "Folder information unselected")
				Else
					If Me.IsChildObject Then
						If Not _overwriteDropdown.SelectedItem Is Nothing AndAlso _overwriteDropdown.SelectedItem.ToString.ToLower() = "append only" Then
							If _buildFolderStructure.Checked AndAlso _destinationFolderPath.SelectedIndex = -1 Then Me.AppendErrorMessage(msg, "Parent information unselected")
						ElseIf _buildFolderStructure.Checked AndAlso _destinationFolderPath.SelectedIndex = -1 Then
							Me.AppendErrorMessage(msg, "Parent information unselected")
						End If
					End If
				End If
				If _fieldMap.FieldColumns.RightListBoxItems.Count = 0 Then
					Me.AppendErrorMessage(msg, "No file columns mapped")
				End If
				If _fieldMap.LoadFileColumns.LeftListBoxItems.Count = 0 Then
					Me.AppendErrorMessage(msg, "No fields mapped")
				End If
				Try
					If _filePath.Text.Trim = "" OrElse _filePath.Text.Trim.ToLower = "select file to load..." Then
						Me.AppendErrorMessage(msg, "No load file selected")
					ElseIf Not System.IO.File.Exists(_filePath.Text) Then
						Me.AppendErrorMessage(msg, "Selected load file does not exist")
					End If
				Catch
					Me.AppendErrorMessage(msg, "Access is restricted to selected load file")
				End Try
				If _loadFileEncodingPicker.SelectedEncoding Is Nothing Then
					Me.AppendErrorMessage(msg, "No file encoding selected")
				End If
				If _extractedTextValueContainsFileLocation.Checked AndAlso _fullTextFileEncodingPicker.SelectedEncoding Is Nothing Then
					Me.AppendErrorMessage(msg, "No text file encoding selected for extracted text")
				End If
				If msg.ToString.Trim <> String.Empty Then
					msg.Insert(0, "The following issues need to be addressed before continuing:" & vbNewLine & vbNewLine)
					MsgBox(msg.ToString, MsgBoxStyle.Exclamation, "Warning")
					Me.Cursor = System.Windows.Forms.Cursors.Default
					Return False
				End If
			End If
			Me.PopulateLoadFileDelimiters()
			If Not Me.EnsureConnection() Then Return Nothing
			Dim currentFields As WinEDDS.DocumentFieldCollection = _application.CurrentFields(Me.LoadFile.ArtifactTypeID, True)
			If currentFields Is Nothing Then
				Me.Cursor = System.Windows.Forms.Cursors.Default
				Return False
			End If
			Me.LoadFile.FieldMap = kCura.EDDS.WinForm.Utility.ExtractFieldMap(_fieldMap.FieldColumns, _fieldMap.LoadFileColumns, currentFields, Me.LoadFile.ArtifactTypeID, Me.LoadFile.ObjectFieldIdListContainsArtifactId)
			'Dim groupIdentifier As DocumentField = _application.CurrentGroupIdentifierField
			'If _identifiersDropDown.SelectedIndex > 0 Then
			'	Dim columnname As String = CType(_identifiersDropDown.SelectedItem, String)
			'	Dim openParenIndex As Int32 = columnname.LastIndexOf("("c) + 1
			'	Dim closeParenIndex As Int32 = columnname.LastIndexOf(")"c)
			'	Dim fieldColumnIndex As Int32 = Int32.Parse(columnname.Substring(openParenIndex, closeParenIndex - openParenIndex)) - 1
			'	If Not groupIdentifier Is Nothing Then
			'		Me.LoadFile.FieldMap.Add(New kCura.WinEDDS.LoadFileFieldMap.LoadFileFieldMapItem(groupIdentifier, fieldColumnIndex))
			'	End If
			'End If
			If LoadFile.ArtifactTypeID = 0 Then
				LoadFile.ArtifactTypeID = _application.ArtifactTypeID
			End If

			LoadFile.SourceFileEncoding = _loadFileEncodingPicker.SelectedEncoding
			LoadFile.FullTextColumnContainsFileLocation = _extractedTextValueContainsFileLocation.Checked

			LoadFile.ExtractedTextFileEncoding = _fullTextFileEncodingPicker.SelectedEncoding
			If _extractedTextValueContainsFileLocation.Checked AndAlso _fullTextFileEncodingPicker.SelectedEncoding IsNot Nothing Then
				LoadFile.ExtractedTextFileEncodingName = Relativity.SqlNameHelper.GetSqlFriendlyName(_fullTextFileEncodingPicker.SelectedEncoding.EncodingName).ToLower
			End If
			LoadFile.LoadNativeFiles = _loadNativeFiles.Checked
			If _overwriteDropdown.SelectedItem Is Nothing Then
				LoadFile.OverwriteDestination = "None"
			Else
				LoadFile.OverwriteDestination = Me.GetOverwrite
			End If
			If LoadFile.OverwriteDestination = "Strict" Then
				LoadFile.IdentityFieldId = DirectCast(_overlayIdentifier.SelectedItem, DocumentField).FieldID
			Else
				LoadFile.IdentityFieldId = -1
			End If
			LoadFile.FirstLineContainsHeaders = _firstLineContainsColumnNames.Checked
			If System.IO.File.Exists(_filePath.Text) Then
				LoadFile.FilePath = _filePath.Text
			End If
			LoadFile.SelectedIdentifierField = _application.GetDocumentFieldFromName(_application.GetCaseIdentifierFields(Me.LoadFile.ArtifactTypeID)(0))
			'If Not _identifiersDropDown.SelectedItem Is Nothing Then
			'	LoadFile.GroupIdentifierColumn = _identifiersDropDown.SelectedItem.ToString
			'Else
			'	LoadFile.GroupIdentifierColumn = Nothing
			'End If

			If _loadNativeFiles.Checked Then
				If Not _nativeFilePathField.SelectedItem Is Nothing Then
					LoadFile.NativeFilePathColumn = _nativeFilePathField.SelectedItem.ToString
				Else
					LoadFile.NativeFilePathColumn = Nothing
				End If
				'Add the file field as a mapped field for non document object types
				If Me.LoadFile.ArtifactTypeID <> Relativity.ArtifactType.Document Then
					For Each field As DocumentField In currentFields.AllFields
						If field.FieldTypeID = Relativity.FieldTypeHelper.FieldType.File Then
							Dim openParenIndex As Int32 = LoadFile.NativeFilePathColumn.LastIndexOf("("c) + 1
							Dim closeParenIndex As Int32 = LoadFile.NativeFilePathColumn.LastIndexOf(")"c)
							Dim nativePathColumn As Int32 = Int32.Parse(LoadFile.NativeFilePathColumn.Substring(openParenIndex, closeParenIndex - openParenIndex)) - 1
							LoadFile.FieldMap.Add(New kCura.WinEDDS.LoadFileFieldMap.LoadFileFieldMapItem(field, nativePathColumn))
						End If
					Next
				End If
			End If
			LoadFile.CreateFolderStructure = _buildFolderStructure.Checked
			If LoadFile.OverwriteDestination.ToLower <> "strict" AndAlso LoadFile.OverwriteDestination.ToLower <> "append" Then
				If LoadFile.CreateFolderStructure Then
					If Not _destinationFolderPath.SelectedItem Is Nothing Then
						LoadFile.FolderStructureContainedInColumn = _destinationFolderPath.SelectedItem.ToString
					Else
						LoadFile.FolderStructureContainedInColumn = Nothing
					End If
				End If
			Else
				If Me.IsChildObject Then
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
			End If
			Me.LoadFile.CaseDefaultPath = _application.SelectedCaseInfo.DocumentPath
			If _startLineNumber.Text = "" Then
				Me.LoadFile.StartLineNumber = 0
			Else
				Me.LoadFile.StartLineNumber = CType(_startLineNumber.Text, Int64)
			End If
			If Me.LoadFile.IdentityFieldId = -1 Then Me.LoadFile.IdentityFieldId = _application.CurrentFields(Me.LoadFile.ArtifactTypeID).IdentifierFields(0).FieldID
			Me.LoadFile.SendEmailOnLoadCompletion = _importMenuSendEmailNotificationItem.Checked
			Me.LoadFile.ForceFolderPreview = _importMenuForceFolderPreviewItem.Checked
			Me.Cursor = System.Windows.Forms.Cursors.Default
			Return True
		End Function

		Private Sub MarkIdentifierField(ByVal fieldNames As String())
			Dim identifierFields As String() = _application.GetCaseIdentifierFields(Me.LoadFile.ArtifactTypeID)
			Dim i As Int32
			For i = 0 To fieldNames.Length - 1
				If System.Array.IndexOf(identifierFields, fieldNames(i)) <> -1 Then
					fieldNames(i) = fieldNames(i) & " [Identifier]"
				End If
			Next
		End Sub

		Public Sub LoadFormControls(ByVal loadFileObjectUpdatedFromFile As Boolean)
			If Me.LoadFile.ArtifactTypeID = 0 Then Me.LoadFile.ArtifactTypeID = _application.ArtifactTypeID
			Me.Text = String.Format("Relativity Desktop Client | Import {0} Load File", _application.GetObjectTypeName(Me.LoadFile.ArtifactTypeID))
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			kCura.EDDS.WinForm.Utility.InitializeCharacterDropDown(_recordDelimiter, _loadFile.RecordDelimiter)
			kCura.EDDS.WinForm.Utility.InitializeCharacterDropDown(_quoteDelimiter, _loadFile.QuoteDelimiter)
			kCura.EDDS.WinForm.Utility.InitializeCharacterDropDown(_newLineDelimiter, _loadFile.NewlineDelimiter)
			kCura.EDDS.WinForm.Utility.InitializeCharacterDropDown(_multiRecordDelimiter, _loadFile.MultiRecordDelimiter)
			kCura.EDDS.WinForm.Utility.InitializeCharacterDropDown(_hierarchicalValueDelimiter, _loadFile.HierarchicalValueDelimiter)
			_importMenuSendEmailNotificationItem.Visible = _application.SendLoadNotificationEmailEnabled
			If Not String.IsNullOrEmpty(LoadFile.FilePath) Then
				_filePath.Text = LoadFile.FilePath
			End If
			_importDestinationText.Text = _application.GetCaseFolderPath(LoadFile.DestinationFolderID)
			_fieldMap.ClearAll()
			_fileColumnHeaders.Items.Clear()
			_nativeFilePathField.Items.Clear()
			_destinationFolderPath.Items.Clear()
			_loadNativeFiles.Checked = LoadFile.LoadNativeFiles
			_firstLineContainsColumnNames.Checked = LoadFile.FirstLineContainsHeaders
			_extractedTextValueContainsFileLocation.Checked = LoadFile.FullTextColumnContainsFileLocation
			_fullTextFileEncodingPicker.Enabled = _extractedTextValueContainsFileLocation.Checked
			_overwriteDropdown.SelectedItem = Me.GetOverwriteDropdownItem(LoadFile.OverwriteDestination)
			If loadFileObjectUpdatedFromFile Then			' has to get called before the loadfileobjectupdatedfromfile block
				_loadFileEncodingPicker.SelectedEncoding = Me.LoadFile.SourceFileEncoding
				_fullTextFileEncodingPicker.SelectedEncoding = Me.LoadFile.ExtractedTextFileEncoding
			End If
			RefreshNativeFilePathFieldAndFileColumnHeaders()
			If Not Me.EnsureConnection() Then Exit Sub
			Dim caseFields As String() = _application.GetNonFileCaseFields(LoadFile.CaseInfo.ArtifactID, _application.ArtifactTypeID, True)
			If loadFileObjectUpdatedFromFile Then
				Dim columnHeaders As String()
				If System.IO.File.Exists(Me.LoadFile.FilePath) Then
					columnHeaders = _application.GetColumnHeadersFromLoadFile(Me.LoadFile, _firstLineContainsColumnNames.Checked)
				Else
					MsgBox("The load file specified does not exist.", MsgBoxStyle.Exclamation, "Relativity Desktop Client Warning")
					columnHeaders = New String() {}
				End If
				BuildMappingFromLoadFile(caseFields, columnHeaders)
				If LoadFile.LoadNativeFiles Then
					_loadNativeFiles.Checked = True
					If LoadFile.NativeFilePathColumn <> String.Empty Then
						_nativeFilePathField.SelectedItem = LoadFile.NativeFilePathColumn
					End If
				End If
				_buildFolderStructure.Checked = LoadFile.CreateFolderStructure
				ActionMenuEnabled = ReadyToRun
			Else
				Me.MarkIdentifierField(caseFields)
				_fieldMap.FieldColumns.LeftListBoxItems.AddRange(caseFields)
			End If
			'_identifiersDropDown.Items.AddRange(_application.IdentiferFieldDropdownPopulator)
			_overwriteDropdown.SelectedItem = Me.GetOverwriteDropdownItem(LoadFile.OverwriteDestination)
			_overlayIdentifier.Items.Clear()
			_overlayIdentifier.Items.AddRange(Me.GetSuitableKeyFields)
			_importMenuSendEmailNotificationItem.Checked = Me.LoadFile.SendEmailOnLoadCompletion
			_importMenuForceFolderPreviewItem.Checked = Me.LoadFile.ForceFolderPreview
			If Not loadFileObjectUpdatedFromFile Then
				For Each item As DocumentField In _overlayIdentifier.Items
					If item.FieldCategory = Relativity.FieldCategory.Identifier Then
						_overlayIdentifier.SelectedItem = item
						Exit For
					End If
				Next
			Else
				For Each item As DocumentField In _overlayIdentifier.Items
					If item.FieldID = LoadFile.IdentityFieldId Then
						_overlayIdentifier.SelectedItem = item
						Exit For
					End If
				Next
			End If

			If Me.LoadFile.ArtifactTypeID = Relativity.ArtifactType.Document Then
				If _overwriteDropdown.SelectedItem Is Nothing Then
					_destinationFolderPath.Enabled = _buildFolderStructure.Checked
				Else
					_destinationFolderPath.Enabled = Not (_overwriteDropdown.SelectedItem.ToString.ToLower = "overlay only" OrElse _overwriteDropdown.SelectedItem.ToString.ToLower = "append/overlay") AndAlso _buildFolderStructure.Checked
				End If
			Else
				If _overwriteDropdown.SelectedItem Is Nothing Then
					_destinationFolderPath.Enabled = _buildFolderStructure.Checked
				Else
					If Me.IsChildObject Then
						Select Case _overwriteDropdown.SelectedItem.ToString.ToLower
							Case "append/overlay"
								_buildFolderStructure.Checked = True
								_buildFolderStructure.Enabled = False
								_destinationFolderPath.Enabled = True
							Case "overlay only"
								_buildFolderStructure.Enabled = True
								_destinationFolderPath.Enabled = _buildFolderStructure.Checked
							Case Else							 'append only
								_buildFolderStructure.Checked = True
								_buildFolderStructure.Enabled = False
								_destinationFolderPath.Enabled = True
						End Select
					End If
				End If
			End If
			If Not Me.LoadFile.FolderStructureContainedInColumn Is Nothing Then
				If loadFileObjectUpdatedFromFile AndAlso _destinationFolderPath.Enabled And Not Me.LoadFile.FolderStructureContainedInColumn.ToLower Is Nothing Then
					Dim i As Int32
					For i = 0 To _destinationFolderPath.Items.Count - 1
						If DirectCast(_destinationFolderPath.Items(i), String).ToLower = Me.LoadFile.FolderStructureContainedInColumn.ToLower Then
							_destinationFolderPath.SelectedIndex = i
							Exit For
						End If
					Next
				End If
			End If
			'If Not Me.LoadFile.GroupIdentifierColumn Is Nothing AndAlso Me.LoadFile.GroupIdentifierColumn <> "" AndAlso _
			''_identifiersDropDown.Items.Contains(LoadFile.GroupIdentifierColumn) Then
			'	'_identifiersDropDown.SelectedItem = LoadFile.GroupIdentifierColumn
			'End If

			If Me.LoadFile.ArtifactTypeID = Relativity.ArtifactType.Document Then
				_extractedTextValueContainsFileLocation.Enabled = Me.FullTextColumnIsMapped
			End If
			_fullTextFileEncodingPicker.Enabled = _extractedTextValueContainsFileLocation.Enabled And _extractedTextValueContainsFileLocation.Checked

			'If LoadFile.OverwriteDestination AndAlso Not LoadFile.SelectedIdentifierField Is Nothing Then
			'	_overWrite.Checked = True
			'	caseFieldName = _application.GetSelectedIdentifier(LoadFile.SelectedIdentifierField)
			'	If caseFieldName <> String.Empty Then
			'		_identifiersDropDown.SelectedItem = caseFieldName
			'	End If
			'End If
			_fieldMap.FieldColumns.EnsureHorizontalScrollbars()
			_fieldMap.LoadFileColumns.EnsureHorizontalScrollbars()
			_startLineNumber.Value = CType(LoadFile.StartLineNumber, Decimal)
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

#Region "Resizing"
		'These member variables are populated with data needed to resize the controls

		'Avoid adjusting the layout if the size hasn't changed
		Private _layoutLastFormSize As Size

		' Used to keep track of whether we need to calculate the layout values.  In addition to
		' initial population, they may need to be populated later due to autoscaling.  Autoscaling
		' will change the distance between concrols which we would not expect to change.  If this
		' happens, the _layout info which contains the relative location of controls needs to be 
		' updated.
		Private _layoutReferenceDistance As Int32 = 0

		Private _layoutDifferenceList As List(Of RelativeLayoutData)

		Private Function CalcReferenceDistance() As Int32
			Return GroupBoxNativeFileBehavior.Top - GroupBoxOverwrite.Top
		End Function

		Private Sub OnForm_Layout(ByVal sender As Object, ByVal e As System.Windows.Forms.LayoutEventArgs) Handles MyBase.Layout
			'The reference distance should remain constant even if the dialog box is resized
			If _layoutReferenceDistance <> CalcReferenceDistance() Then
				InitializeLayout()
			Else
				AdjustLayout()
			End If
		End Sub

		Private Sub InitializeLayout()
			_layoutLastFormSize = Me.Size

			'Layout properties which are directly based on another layout property
			If _layoutDifferenceList Is Nothing Then
				_layoutDifferenceList = New List(Of RelativeLayoutData)

				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Width, _fieldMapTab, LayoutRelativePropertyTypeForDifference.Width))
				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Height, _fieldMapTab, LayoutRelativePropertyTypeForDifference.Height))
				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Width, _fieldMap, LayoutRelativePropertyTypeForDifference.Width))
				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Height, _fieldMap, LayoutRelativePropertyTypeForDifference.Height))

				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Width, TabControl1, LayoutRelativePropertyTypeForDifference.Width))
				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Width, _fieldMapTab, LayoutRelativePropertyTypeForDifference.Width))
				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Height, TabControl1, LayoutRelativePropertyTypeForDifference.Height))
				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Height, _fieldMapTab, LayoutRelativePropertyTypeForDifference.Height))

				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Height, GroupBoxOverwrite, LayoutRelativePropertyTypeForDifference.Top))
				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Height, GroupBoxOverlayIdentifier, LayoutRelativePropertyTypeForDifference.Top))
				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Height, GroupBoxFolderInfo, LayoutRelativePropertyTypeForDifference.Top))
				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Height, GroupBoxNativeFileBehavior, LayoutRelativePropertyTypeForDifference.Top))
				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Height, GroupBoxExtractedText, LayoutRelativePropertyTypeForDifference.Top))

				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Height, GroupBoxCharacters, LayoutRelativePropertyTypeForDifference.Height))
				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Height, GroupBoxFileColumnHeaders, LayoutRelativePropertyTypeForDifference.Height))
				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Width, GroupBoxFileColumnHeaders, LayoutRelativePropertyTypeForDifference.Width))
				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Height, _fileColumnHeaders, LayoutRelativePropertyTypeForDifference.Height))
				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Width, _fileColumnHeaders, LayoutRelativePropertyTypeForDifference.Width))

				_layoutDifferenceList.Add(New RelativeLayoutData(GroupBoxOverwrite, LayoutBasePropertyTypeForDifference.Right, GroupBoxFolderInfo, LayoutRelativePropertyTypeForDifference.Left))
				_layoutDifferenceList.Add(New RelativeLayoutData(GroupBoxOverwrite, LayoutBasePropertyTypeForDifference.Right, GroupBoxNativeFileBehavior, LayoutRelativePropertyTypeForDifference.Left))
				_layoutDifferenceList.Add(New RelativeLayoutData(GroupBoxFolderInfo, LayoutBasePropertyTypeForDifference.Right, GroupBoxExtractedText, LayoutRelativePropertyTypeForDifference.Left))
			End If

			_layoutDifferenceList.ForEach(Sub(x)
																			x.InitializeDifference()
																		End Sub)

			_layoutReferenceDistance = CalcReferenceDistance()
		End Sub

		Public Sub AdjustLayout()
			If Not _layoutLastFormSize.Equals(Me.Size) Then
				For Each x As RelativeLayoutData In _layoutDifferenceList
					x.AdjustRelativeControlBasedOnDifference()
				Next

				_layoutLastFormSize = Me.Size
			End If
		End Sub
#End Region

		Private Sub _browseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _browseButton.Click
			OpenFileDialog.ShowDialog()
		End Sub

		Private Function RefreshNativeFilePathFieldAndFileColumnHeaders(Optional ByVal showWarning As Boolean = False, Optional ByVal comingFromEmptyEncoding As Boolean = True) As String()
			Dim columnHeaders As String() = Nothing
			Dim listsAreSame As Boolean = True
			Dim currentHeaders As String() = Nothing
			Dim determinedEncoding As System.Text.Encoding = Nothing
			If System.IO.File.Exists(LoadFile.FilePath) Then
				_loadFileEncodingPicker.Enabled = True
				LabelFileEncoding.Text = "File Encoding"
				determinedEncoding = kCura.WinEDDS.Utility.DetectEncoding(LoadFile.FilePath, True).DeterminedEncoding
				columnHeaders = _application.GetColumnHeadersFromLoadFile(LoadFile, _firstLineContainsColumnNames.Checked)
				If determinedEncoding IsNot Nothing Then
					'Check for what user selected
					If _loadFileEncodingPicker.SelectedEncoding IsNot Nothing AndAlso Not _loadFileEncodingPicker.SelectedEncoding.Equals(determinedEncoding) Then
						MsgBox(String.Format("The encoding selected in the Relativity Desktop Client is different than the encoding detected in your load file. The encoding selection has been updated to match the detected encoding of your load file. It will be changed from {0} to {1}", _loadFileEncodingPicker.SelectedEncoding.EncodingName, determinedEncoding.EncodingName))
					End If

					_loadFileEncodingPicker.SelectedEncoding = determinedEncoding
					_loadFileEncodingPicker.Enabled = False
					LabelFileEncoding.Text = "File Encoding - Auto Detected"
				ElseIf _loadFileEncodingPicker.SelectedEncoding Is Nothing Then
					_fileColumnHeaders.Items.Clear()
					_fileColumnHeaders.Items.Add("The encoding of the selected load file could not be detected.  Please select the load file's encoding.")
					Return Nothing
				End If


				System.Array.Sort(columnHeaders)
				Dim currentHeaderList As New System.Collections.ArrayList
				For Each item As Object In _fieldMap.LoadFileColumns.LeftListBoxItems
					If Not currentHeaderList.Contains(item.ToString) Then currentHeaderList.Add(item.ToString)
				Next
				For Each item As Object In _fieldMap.LoadFileColumns.RightListBoxItems
					If Not currentHeaderList.Contains(item.ToString) Then currentHeaderList.Add(item.ToString)
				Next
				currentHeaders = DirectCast(currentHeaderList.ToArray(GetType(String)), String())
				System.Array.Sort(currentHeaders)
				If currentHeaders.Length <> columnHeaders.Length Then listsAreSame = False
				If listsAreSame Then
					For i As Int32 = 0 To currentHeaders.Length - 1
						listsAreSame = listsAreSame And (currentHeaders(i) = columnHeaders(i))
						If Not listsAreSame Then Exit For
					Next
				End If
			End If
			If System.IO.File.Exists(LoadFile.FilePath) AndAlso (Not listsAreSame Or comingFromEmptyEncoding) Then
				If currentHeaders.Length > 0 AndAlso Not listsAreSame AndAlso showWarning Then
					MsgBox("Column schema changed with load file." & System.Environment.NewLine & "Column information reset.", MsgBoxStyle.Information, "Relwin Message")
				End If
				_fileColumnHeaders.Items.Clear()
				_nativeFilePathField.Items.Clear()
				_destinationFolderPath.Items.Clear()
				'_identifiersDropDown.Items.Clear()
				_fieldMap.LoadFileColumns.ClearAll()
				PopulateLoadFileDelimiters()
				'columnHeaders = _application.GetColumnHeadersFromLoadFile(LoadFile, _firstLineContainsColumnNames.Checked)
				System.Array.Sort(columnHeaders)
				'_filePath.Text = LoadFile.FilePath\
				_fieldMap.LoadFileColumns.RightListBoxItems.AddRange(columnHeaders)
				_fileColumnHeaders.Items.AddRange(columnHeaders)
				_nativeFilePathField.Items.AddRange(columnHeaders)
				_destinationFolderPath.Items.AddRange(columnHeaders)
				Dim identifiersDropdownRange As New System.Collections.ArrayList
				'_identifiersDropDown.Items.Add("< none >")
				'_identifiersDropDown.Items.AddRange(columnHeaders)
				'_identifiersDropDown.SelectedIndex = 0
				If LoadFile.LoadNativeFiles AndAlso System.IO.File.Exists(LoadFile.FilePath) Then
					_nativeFilePathField.SelectedItem = LoadFile.NativeFilePathColumn
				End If
				_nativeFilePathField.SelectedItem = Nothing
				_nativeFilePathField.Text = "Select ..."
				_destinationFolderPath.SelectedItem = Nothing
				_destinationFolderPath.Text = "Select ..."
			End If
			ActionMenuEnabled = ReadyToRun
			_fieldMap.LoadFileColumns.EnsureHorizontalScrollbars()
			Return columnHeaders
		End Function



		Private Sub OpenFileDialog_FileOk(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog.FileOk
			Dim oldfilepath As String = Nothing
			Try
				If Not Me.EnsureConnection Then Exit Sub
				oldfilepath = _filePath.Text
				_filePath.Text = OpenFileDialog.FileName
				PopulateLoadFileObject(False)
				Dim extension As String = _filePath.Text
				If extension.IndexOf("\") <> -1 Then
					extension = extension.Substring(extension.LastIndexOf("\") + 1)
				End If
				If extension.IndexOf(".") = -1 Then
					extension = ""
				Else
					extension = extension.Substring(extension.LastIndexOf(".") + 1).ToLower
				End If
				Select Case extension
					Case "csv"
						_recordDelimiter.SelectedValue = AscW(",")
						_quoteDelimiter.SelectedValue = AscW("""")
						Me.LoadFile.QuoteDelimiter = """"c
						Me.LoadFile.RecordDelimiter = ","c
					Case "dat"
						_recordDelimiter.SelectedValue = 20
						_quoteDelimiter.SelectedValue = 254
						Me.LoadFile.RecordDelimiter = ChrW(20)
						Me.LoadFile.QuoteDelimiter = ChrW(254)
				End Select
				RefreshNativeFilePathFieldAndFileColumnHeaders(oldfilepath.ToLower <> "select file to load...", Me.LoadFile.SourceFileEncoding Is Nothing)
			Catch ex As System.IO.IOException
				MsgBox(ex.Message & Environment.NewLine & "Please close any application that might have a hold on the file before proceeding.", MsgBoxStyle.Exclamation, "Relativity Desktop Client")
				_filePath.Text = oldfilepath
			End Try
		End Sub

		Private Sub _filePath_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _filePath.TextChanged
			ActionMenuEnabled = ReadyToRun
			LoadFile.FilePath = _filePath.Text
			'RefreshNativeFilePathFieldAndFileColumnHeaders()
		End Sub

		Private Sub ImportFileMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ImportFileMenu.Click
			If PopulateLoadFileObject(True) AndAlso _application.ReadyToLoad(Utility.ExtractFieldNames(_fieldMap.LoadFileColumns.LeftListBoxItems)) AndAlso _application.ReadyToLoad(Me.LoadFile, False) Then
				_application.ImportLoadFile(Me.LoadFile)
			End If
		End Sub

		Private Sub LoadFileForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
			'kCura.Windows.Forms.EnhancedMenuProvider.Hook(Me)
			_loadFileEncodingPicker.InitializeDropdown()
			_fullTextFileEncodingPicker.InitializeDropdown()
			_importMenuForceFolderPreviewItem.Checked = _application.TemporaryForceFolderPreview
			If LoadFile.ArtifactTypeID <> Relativity.ArtifactType.Document Then
				_importMenuForceFolderPreviewItem.Checked = False
				_importMenuForceFolderPreviewItem.Enabled = False
			End If
		End Sub

		Private Sub LoadFileForm_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
			'kCura.Windows.Forms.EnhancedMenuProvider.Unhook()
		End Sub

		Private Sub _loadNativeFiles_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _loadNativeFiles.CheckedChanged
			_nativeFilePathField.Enabled = _loadNativeFiles.Checked
			_advancedButton.Enabled = _loadNativeFiles.Checked
			'If Not _nativeFilePathField.Items.Count = 0 Then
			'	_nativeFilePathField.SelectedItem = _nativeFilePathField.Items(0)
			'End If
			_nativeFilePathField.SelectedItem = Nothing
			_nativeFilePathField.Text = "Select ..."
			ActionMenuEnabled = ReadyToRun
		End Sub

		Private Sub _overwriteDestination_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _overwriteDropdown.SelectedIndexChanged
			LoadFile.OverwriteDestination = Me.GetOverwrite
			If LoadFile.OverwriteDestination.ToLower <> "strict" Then
				For Each field As DocumentField In _overlayIdentifier.Items
					If field.FieldCategory = Relativity.FieldCategory.Identifier Then
						_overlayIdentifier.SelectedItem = field
						Exit For
					End If
				Next
			End If
			If Me.LoadFile.ArtifactTypeID = Relativity.ArtifactType.Document Then
				Select Case LoadFile.OverwriteDestination.ToLower
					Case "none"
						_buildFolderStructure.Enabled = True
						_destinationFolderPath.Enabled = _buildFolderStructure.Checked
						_overlayIdentifier.Enabled = False
					Case "strict"
						_destinationFolderPath.Enabled = False
						_buildFolderStructure.Checked = False
						_buildFolderStructure.Enabled = False
						_destinationFolderPath.SelectedItem = Nothing
						_destinationFolderPath.Text = "Select ..."
						_overlayIdentifier.Enabled = True
					Case Else
						_destinationFolderPath.Enabled = False
						_buildFolderStructure.Checked = False
						_buildFolderStructure.Enabled = False
						_destinationFolderPath.SelectedItem = Nothing
						_destinationFolderPath.Text = "Select ..."
						_overlayIdentifier.Enabled = False
				End Select
			ElseIf Me.IsChildObject Then
				Select Case LoadFile.OverwriteDestination.ToLower
					Case "none"
						_destinationFolderPath.Enabled = True
						_buildFolderStructure.Checked = True
						_buildFolderStructure.Enabled = False
						_overlayIdentifier.Enabled = False
					Case "strict"
						_destinationFolderPath.Enabled = False
						_buildFolderStructure.Checked = False
						_buildFolderStructure.Enabled = True
						_overlayIdentifier.Enabled = True
					Case Else
						_destinationFolderPath.Enabled = True
						_buildFolderStructure.Checked = True
						_buildFolderStructure.Enabled = False
						_overlayIdentifier.Enabled = False
				End Select
			Else
				_destinationFolderPath.Enabled = False
				_buildFolderStructure.Enabled = False
				_buildFolderStructure.Checked = False
				_destinationFolderPath.SelectedItem = Nothing
				_destinationFolderPath.Text = "Select ..."
				Select Case LoadFile.OverwriteDestination.ToLower
					Case "strict"
						_overlayIdentifier.Enabled = True
					Case Else
						_overlayIdentifier.Enabled = False
				End Select
			End If
			ActionMenuEnabled = ReadyToRun
			'_identifiersDropDown.Enabled = _overWrite.Checked
		End Sub

		Private Sub PreviewMenuFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PreviewMenuFile.Click
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			If PopulateLoadFileObject(True) AndAlso _application.ReadyToLoad(Me.LoadFile, True) Then _application.PreviewLoadFile(_loadFile, False, kCura.EDDS.WinForm.LoadFilePreviewForm.FormType.LoadFile)
			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Sub

		Private Sub _fileSaveFieldMapMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _fileSaveFieldMapMenuItem.Click
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			PopulateLoadFileObject(False)
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
			Dim newLoadFile As LoadFile = _application.ReadLoadFile(Me.LoadFile, _loadFieldMapDialog.FileName, False)
			If Not newLoadFile Is Nothing Then
				If newLoadFile.ArtifactTypeID <> Relativity.ArtifactType.Document Then
					newLoadFile.ForceFolderPreview = False
				End If
				_loadFile = newLoadFile
				Me.LoadFormControls(True)
			End If
		End Sub

		Private Sub _FieldColumns_ItemsShifted() Handles _fieldMap.FieldColumnsItemsShifted
			ActionMenuEnabled = ReadyToRun
			_extractedTextValueContainsFileLocation.Enabled = Me.FullTextColumnIsMapped
			_fullTextFileEncodingPicker.Enabled = _extractedTextValueContainsFileLocation.Enabled And _extractedTextValueContainsFileLocation.Checked
		End Sub



		Private Sub _LoadFileColumns_ItemsShifted() Handles _fieldMap.LoadFileColumnsItemsShifted
			ActionMenuEnabled = ReadyToRun
		End Sub

		Private Property ActionMenuEnabled() As Boolean
			Get
				Return ImportFileMenu.Enabled AndAlso _
				PreviewMenuFile.Enabled AndAlso _
				_importMenuPreviewErrorsItem.Enabled AndAlso _
				((_importMenuPreviewFoldersAndCodesItem.Enabled AndAlso Me.LoadFile.ArtifactTypeID = Relativity.ArtifactType.Document) OrElse Me.LoadFile.ArtifactTypeID <> Relativity.ArtifactType.Document)
			End Get
			Set(ByVal value As Boolean)
				ImportFileMenu.Enabled = value
				PreviewMenuFile.Enabled = value
				_importMenuPreviewErrorsItem.Enabled = value
				If Me.LoadFile.ArtifactTypeID = Relativity.ArtifactType.Document Then
					_importMenuPreviewFoldersAndCodesItem.Enabled = value
				Else
					_importMenuPreviewFoldersAndCodesItem.Enabled = False
				End If
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
			If PopulateLoadFileObject(True) AndAlso _application.ReadyToLoad(Me.LoadFile, True) Then _application.PreviewLoadFile(_loadFile, True, kCura.EDDS.WinForm.LoadFilePreviewForm.FormType.LoadFile)
			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Sub

		Private Sub _importMenuPreviewFoldersAndCodesItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _importMenuPreviewFoldersAndCodesItem.Click
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			If PopulateLoadFileObject(True) AndAlso _application.ReadyToLoad(Me.LoadFile, True) Then _application.PreviewLoadFile(_loadFile, False, kCura.EDDS.WinForm.LoadFilePreviewForm.FormType.Codes)
			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Sub

		Private Sub _identifiersDropDown_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
			ActionMenuEnabled = ReadyToRun
		End Sub

		Private Sub _characterDropdown_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _recordDelimiter.SelectedIndexChanged, _quoteDelimiter.SelectedIndexChanged, _newLineDelimiter.SelectedIndexChanged, _multiRecordDelimiter.SelectedIndexChanged, _hierarchicalValueDelimiter.SelectedIndexChanged
			'PopulateLoadFileObject()
			Dim tag As Object = DirectCast(sender, System.Windows.Forms.ComboBox).Tag
			If TypeOf tag Is Boolean AndAlso CType(tag, Boolean) = False Then
				'do nothing
			Else
				PopulateLoadFileDelimiters()
				RefreshNativeFilePathFieldAndFileColumnHeaders()
			End If
		End Sub

		Private Sub PopulateLoadFileDelimiters()
			LoadFile.QuoteDelimiter = ChrW(CType(_quoteDelimiter.SelectedValue, Int32))
			LoadFile.RecordDelimiter = ChrW(CType(_recordDelimiter.SelectedValue, Int32))
			LoadFile.MultiRecordDelimiter = ChrW(CType(_multiRecordDelimiter.SelectedValue, Int32))
			LoadFile.NewlineDelimiter = ChrW(CType(_newLineDelimiter.SelectedValue, Int32))
			LoadFile.SourceFileEncoding = _loadFileEncodingPicker.SelectedEncoding
			LoadFile.HierarchicalValueDelimiter = ChrW(CType(_hierarchicalValueDelimiter.SelectedValue, Int32))
		End Sub

		Private Sub BuildMappingFromLoadFile(ByVal casefields As String(), ByVal columnHeaders As String())
			Dim selectedFieldNameList As New ArrayList
			Dim selectedColumnNameList As New ArrayList
			Dim item As LoadFileFieldMap.LoadFileFieldMapItem
			_fieldMap.ClearAll()
			For Each item In _loadFile.FieldMap
				If _
				 Not item.DocumentField Is Nothing AndAlso _
				 item.NativeFileColumnIndex > -1 AndAlso _
				 Array.IndexOf(casefields, item.DocumentField.FieldName) > -1 AndAlso _
				 item.NativeFileColumnIndex < columnHeaders.Length _
				 Then
					If item.DocumentField.FieldCategoryID = Relativity.FieldCategory.Identifier Then
						selectedFieldNameList.Add(item.DocumentField.FieldName & " [Identifier]")
					Else
						selectedFieldNameList.Add(item.DocumentField.FieldName)
					End If
					selectedColumnNameList.Add(columnHeaders(item.NativeFileColumnIndex))
				End If
			Next
			For Each item In _loadFile.FieldMap
				If Not item.DocumentField Is Nothing AndAlso columnHeaders.Length = 0 AndAlso Array.IndexOf(casefields, item.DocumentField.FieldName) > -1 Then
					If item.DocumentField.FieldCategoryID = Relativity.FieldCategory.Identifier Then
						selectedFieldNameList.Add(item.DocumentField.FieldName & " [Identifier]")
					Else
						selectedFieldNameList.Add(item.DocumentField.FieldName)
					End If
				End If
			Next
			Me.MarkIdentifierField(casefields)
			_fieldMap.MapCaseFieldsToLoadFileFields(casefields, columnHeaders, selectedFieldNameList, selectedColumnNameList)
		End Sub

		Private Sub _fileMenuCloseItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _fileMenuCloseItem.Click
			Me.Close()
		End Sub

		Private Sub _buildFolderStructure_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _buildFolderStructure.CheckedChanged
			If Me.LoadFile.ArtifactTypeID = Relativity.ArtifactType.Document Then
				If _buildFolderStructure.Checked Then
					_destinationFolderPath.Enabled = True
				Else
					_destinationFolderPath.Enabled = False
					_destinationFolderPath.SelectedItem = Nothing
					_destinationFolderPath.Text = "Select ..."
				End If
			ElseIf Me.IsChildObject Then
				Select Case Me.GetOverwrite.ToLower
					Case "none", "append"
						_destinationFolderPath.Enabled = True
						_destinationFolderPath.SelectedItem = Nothing
						_destinationFolderPath.Text = "Select ..."
					Case "strict"
						If _buildFolderStructure.Checked Then
							_destinationFolderPath.Enabled = True
						Else
							_destinationFolderPath.Enabled = False
							_destinationFolderPath.SelectedItem = Nothing
							_destinationFolderPath.Text = "Select ..."
						End If
				End Select
			Else
			End If
			ActionMenuEnabled = ReadyToRun
		End Sub

		Private Sub _destinationFolderPath_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _destinationFolderPath.SelectedIndexChanged
			ActionMenuEnabled = ReadyToRun
		End Sub

		Private Function FullTextColumnIsMapped() As Boolean
			Try
				Dim docFieldObj As DocumentField = _application.CurrentFields(10).FullText
				'If the LoadFile form is being used to load an object instead of a document, then FullText will be Nothing.
				If docFieldObj IsNot Nothing Then
					Dim ftfname As String = docFieldObj.FieldName
					Dim field As String
					For Each field In _fieldMap.FieldColumns.RightListBoxItems
						If field.ToLower = ftfname.ToLower Then
							Return True
						End If
					Next
				End If
				Return False
			Catch
				Return False
			End Try
		End Function

		Private Sub _fileRefreshMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _fileRefreshMenuItem.Click
			Dim caseFields As String() = _application.GetNonFileCaseFields(LoadFile.CaseInfo.ArtifactID, Me.LoadFile.ArtifactTypeID, True)			'_application.GetCaseFields(LoadFile.CaseInfo.ArtifactID, _application.ArtifactTypeID, True)
			If caseFields Is Nothing Then Exit Sub
			Me.MarkIdentifierField(caseFields)
			Dim fieldName As String
			For Each fieldName In caseFields
				If Not _fieldMap.FieldColumns.RightListBoxItems.Contains(fieldName) AndAlso Not _fieldMap.FieldColumns.LeftListBoxItems.Contains(fieldName) Then
					_fieldMap.FieldColumns.LeftListBoxItems.Add(fieldName)
				End If
			Next
			Dim itemsToRemove As New System.Collections.ArrayList
			For Each fieldName In _fieldMap.FieldColumns.LeftListBoxItems
				If Array.IndexOf(caseFields, (fieldName)) = -1 Then
					itemsToRemove.Add(fieldName)
				End If
			Next
			For Each fieldName In itemsToRemove
				_fieldMap.FieldColumns.LeftListBoxItems.Remove(fieldName)
			Next

			itemsToRemove = New Collections.ArrayList
			For Each fieldName In _fieldMap.FieldColumns.RightListBoxItems
				If Array.IndexOf(caseFields, (fieldName)) = -1 Then
					itemsToRemove.Add(fieldName)
				End If
			Next
			For Each fieldName In itemsToRemove
				_fieldMap.FieldColumns.RightListBoxItems.Remove(fieldName)
			Next
			_application.RefreshSelectedCaseInfo()
			Me.LoadFile.CaseInfo = _application.SelectedCaseInfo
			Dim id As Int32 = DirectCast(_overlayIdentifier.SelectedItem, DocumentField).FieldID
			_overlayIdentifier.Items.Clear()
			_overlayIdentifier.Items.AddRange(Me.GetSuitableKeyFields)
			For Each field As DocumentField In _overlayIdentifier.Items
				If field.FieldID = id Then
					_overlayIdentifier.SelectedItem = field
					Exit For
				End If
			Next
			If _overlayIdentifier.SelectedItem Is Nothing Then
				For Each field As DocumentField In _overlayIdentifier.Items
					If field.FieldCategory = Relativity.FieldCategory.Identifier Then
						_overlayIdentifier.SelectedItem = field
						Exit For
					End If
				Next
			End If
			InitializeDocumentSpecificComponents()
		End Sub

		Private Function EnsureConnection() As Boolean
			If Not _loadFile Is Nothing AndAlso Not _loadFile.CaseInfo Is Nothing Then
				Dim casefields As String() = Nothing
				Dim [continue] As Boolean = True
				Try
					casefields = _application.GetCaseFields(_loadFile.CaseInfo.ArtifactID, 10, True)
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

		Private Sub _advancedButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _advancedButton.Click
			_advancedFileForm = New AdvancedFileLocation
			_advancedFileForm._copyNativeFiles.Checked = Me.LoadFile.CopyFilesToDocumentRepository
			_advancedFileForm._keepNativeFiles.Checked = Not Me.LoadFile.CopyFilesToDocumentRepository
			If Not Me.LoadFile.SelectedCasePath Is Nothing AndAlso Not Me.LoadFile.SelectedCasePath = "" Then
				_advancedFileForm.SelectPath(Me.LoadFile.SelectedCasePath)
				_advancedFileForm.SelectDefaultPath = False
			End If
			_advancedFileForm.ShowDialog()
		End Sub

		Private Sub _advancedFileForm_FileLocationOK(ByVal copyFiles As Boolean, ByVal selectedRepository As String) Handles _advancedFileForm.FileLocationOK
			Me.LoadFile.CopyFilesToDocumentRepository = copyFiles
			Me.LoadFile.SelectedCasePath = selectedRepository
		End Sub

		Private Sub _extractedTextValueContainsFileLocation_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _extractedTextValueContainsFileLocation.CheckedChanged
			_fullTextFileEncodingPicker.Enabled = _extractedTextValueContainsFileLocation.Checked
		End Sub

		Private Sub _loadFileEncodingPicker_SelectedEncodingChanged() Handles _loadFileEncodingPicker.SelectedEncodingChanged
			Me.LoadFile.SourceFileEncoding = _loadFileEncodingPicker.SelectedEncoding
			Me.RefreshNativeFilePathFieldAndFileColumnHeaders(_loadFileEncodingPicker.SelectedEncoding Is Nothing)
		End Sub

		Private Sub _importMenuSendEmailNotificationItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _importMenuSendEmailNotificationItem.Click
			_importMenuSendEmailNotificationItem.Checked = Not _importMenuSendEmailNotificationItem.Checked
		End Sub

		Private Sub _importMenuForceFolderPreviewItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _importMenuForceFolderPreviewItem.Click
			_importMenuForceFolderPreviewItem.Checked = Not _importMenuForceFolderPreviewItem.Checked
		End Sub
	End Class
End Namespace
