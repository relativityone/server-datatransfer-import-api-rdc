Imports kCura.WinEDDS
Imports Relativity.Desktop.Client.Legacy.Controls

Namespace Relativity.Desktop.Client
	Public Class LoadFileForm
		Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "
		Public Sub New()
			MyBase.New()

			_application = Global.Relativity.Desktop.Client.Application.Instance

			'This call is required by the Windows Form Designer.
			InitializeComponent()

		End Sub
		Friend WithEvents _importMenuForceFolderPreviewItem As System.Windows.Forms.MenuItem

		Private ParentArtifactTypeID As Int32
		Friend WithEvents MenuItem7 As System.Windows.Forms.MenuItem
		Friend WithEvents ViewFieldMapMenuItem As System.Windows.Forms.MenuItem
		Public WithEvents FieldMapMenuBar As System.Windows.Forms.MenuItem
		Public WithEvents MenuItem4 As System.Windows.Forms.MenuItem
		Public WithEvents _fileRefreshMenuItem As System.Windows.Forms.MenuItem
		Public WithEvents ViewFieldMapButton As System.Windows.Forms.Button
		Public WithEvents AutoFieldMapButton As System.Windows.Forms.Button
		Private ReadOnly Property IsChildObject() As Boolean
			Get
				Return ParentArtifactTypeID <> 8
			End Get
		End Property

		Private Async Function GetMultiObjectMultiChoiceCache() As Task(Of DocumentFieldCollection)
			If _multiObjectMultiChoiceCache Is Nothing Then
				Dim container As DocumentFieldCollection = Await _application.CurrentNonFileFields(_application.ArtifactTypeID, False)
				_multiObjectMultiChoiceCache = New DocumentFieldCollection()
				For Each docInfo As DocumentField In container
					If docInfo.FieldTypeID = Global.Relativity.FieldTypeHelper.FieldType.MultiCode OrElse docInfo.FieldTypeID = Global.Relativity.FieldTypeHelper.FieldType.Objects Then
						_multiObjectMultiChoiceCache.Add(docInfo)
					End If
				Next
			End If
			Return _multiObjectMultiChoiceCache
		End Function

		Private _multiObjectMultiChoiceCache As DocumentFieldCollection = Nothing

		Private Async Sub InitializeDocumentSpecificComponentsOnLoad() Handles Me.Load
			Await InitializeDocumentSpecificComponents()
		End Sub

		Private Async Function InitializeDocumentSpecificComponents() As Task
			If Me.LoadFile.ArtifactTypeID = 0 Then Me.LoadFile.ArtifactTypeID = _application.ArtifactTypeID
			If Me.LoadFile.ArtifactTypeID = Global.Relativity.ArtifactType.Document Then
				Me.GroupBoxNativeFileBehavior.Enabled = True
				Me.GroupBoxExtractedText.Enabled = True
				Me.GroupBoxFolderInfo.Text = "Folder Info"
				Me._buildFolderStructure.Text = "Folder Information Column"
				ParentArtifactTypeID = 8
			Else
				Dim parentQuery As New kCura.WinEDDS.Service.ObjectTypeManager(Await _application.GetCredentialsAsync(), _application.CookieContainer)
				ParentArtifactTypeID = CType(parentQuery.RetrieveParentArtifactTypeID(_application.SelectedCaseInfo.ArtifactID,
				Me.LoadFile.ArtifactTypeID).Tables(0).Rows(0)("ParentArtifactTypeID"), Int32)
				Me.GroupBoxFolderInfo.Enabled = False
				If Me.IsChildObject Then
					Me.GroupBoxFolderInfo.Enabled = True
					_buildFolderStructure.Checked = True
				Else
					Me.GroupBoxFolderInfo.Enabled = False
					_buildFolderStructure.Checked = False
				End If
				Me.GroupBoxNativeFileBehavior.Enabled = False
				If Await _application.HasFileField(Me.LoadFile.ArtifactTypeID, True) Then
					Me.GroupBoxNativeFileBehavior.Enabled = True
				End If
				Me.GroupBoxExtractedText.Enabled = False
			End If
		End Function

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
		Public WithEvents OpenFileDialog As System.Windows.Forms.OpenFileDialog
		Public WithEvents GroupBoxImportDestination As System.Windows.Forms.GroupBox
		Public WithEvents _importDestinationText As System.Windows.Forms.Label
		Public WithEvents MainMenu As System.Windows.Forms.MainMenu
		Public WithEvents MenuItem1 As System.Windows.Forms.MenuItem
		Public WithEvents MenuItem2 As System.Windows.Forms.MenuItem
		Public WithEvents PreviewMenuFile As System.Windows.Forms.MenuItem
		Public WithEvents ImportFileMenu As System.Windows.Forms.MenuItem
		Public WithEvents _fileSaveFieldMapMenuItem As System.Windows.Forms.MenuItem
		Public WithEvents _saveFieldMapDialog As System.Windows.Forms.SaveFileDialog
		Public WithEvents _fileLoadFieldMapMenuItem As System.Windows.Forms.MenuItem
		Public WithEvents _loadFieldMapDialog As System.Windows.Forms.OpenFileDialog
		Public WithEvents _importMenuPreviewErrorsItem As System.Windows.Forms.MenuItem
		Public WithEvents _importMenuPreviewFoldersAndCodesItem As System.Windows.Forms.MenuItem
		Public WithEvents TabControl1 As System.Windows.Forms.TabControl
		Public WithEvents _loadFileTab As System.Windows.Forms.TabPage
		Public WithEvents _fieldMapTab As System.Windows.Forms.TabPage
		Public WithEvents _loadNativeFiles As System.Windows.Forms.CheckBox
		Public WithEvents Label5 As System.Windows.Forms.Label
		Public WithEvents _nativeFilePathField As System.Windows.Forms.ComboBox
		Public WithEvents GroupBoxNativeFileBehavior As System.Windows.Forms.GroupBox
		Public WithEvents GroupBoxCharacters As System.Windows.Forms.GroupBox
		Public WithEvents _multiRecordDelimiter As System.Windows.Forms.ComboBox
		Public WithEvents LabelMultiValue As System.Windows.Forms.Label
		Public WithEvents LabelQuote As System.Windows.Forms.Label
		Public WithEvents _quoteDelimiter As System.Windows.Forms.ComboBox
		Public WithEvents LabelNewline As System.Windows.Forms.Label
		Public WithEvents _newLineDelimiter As System.Windows.Forms.ComboBox
		Public WithEvents LabelColumn As System.Windows.Forms.Label
		Public WithEvents _recordDelimiter As System.Windows.Forms.ComboBox
		Public WithEvents GroupBoxFileColumnHeaders As System.Windows.Forms.GroupBox
		Public WithEvents _fileColumnHeaders As System.Windows.Forms.ListBox
		Public WithEvents _firstLineContainsColumnNames As System.Windows.Forms.CheckBox
		Public WithEvents GroupBoxLoadFile As System.Windows.Forms.GroupBox
		Public WithEvents _browseButton As System.Windows.Forms.Button
		Public WithEvents _filePath As System.Windows.Forms.TextBox
		Public WithEvents HelpProvider1 As System.Windows.Forms.HelpProvider
		Public WithEvents _fileMenuCloseItem As System.Windows.Forms.MenuItem
		Public WithEvents _destinationFolderPath As System.Windows.Forms.ComboBox
		Public WithEvents _buildFolderStructure As System.Windows.Forms.CheckBox
		Public WithEvents GroupBoxFolderInfo As System.Windows.Forms.GroupBox
		Public WithEvents _overwriteDropdown As System.Windows.Forms.ComboBox
		Public WithEvents GroupBoxOverwrite As System.Windows.Forms.GroupBox
		Public WithEvents GroupBoxExtractedText As System.Windows.Forms.GroupBox
		Public WithEvents _overlayExtractedText As System.Windows.Forms.ComboBox
		Public WithEvents _extractedTextValueContainsFileLocation As System.Windows.Forms.CheckBox
		Public WithEvents LabelFileEncoding As System.Windows.Forms.Label
		Public WithEvents _advancedButton As System.Windows.Forms.Button
		Public WithEvents Label9 As System.Windows.Forms.Label
		Public WithEvents _loadFileEncodingPicker As EncodingPicker
		Public WithEvents _fullTextFileEncodingPicker As EncodingPicker
		Public WithEvents _hierarchicalValueDelimiter As System.Windows.Forms.ComboBox
		Public WithEvents LabelNestedValue As System.Windows.Forms.Label
		Public WithEvents _startLineNumberLabel As System.Windows.Forms.Label
		Public WithEvents _startLineNumber As System.Windows.Forms.NumericUpDown
		Public WithEvents _fieldMap As FieldMap
		Public WithEvents GroupBoxOverlayIdentifier As System.Windows.Forms.GroupBox
		Public WithEvents _overlayIdentifier As System.Windows.Forms.ComboBox
		Public WithEvents MenuItem5 As System.Windows.Forms.MenuItem
		Public WithEvents _importMenuSendEmailNotificationItem As System.Windows.Forms.MenuItem
		Public WithEvents GroupBoxOverlayBehavior As System.Windows.Forms.GroupBox
		Public WithEvents _overlayBehavior As System.Windows.Forms.ComboBox

		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
			Me.components = New System.ComponentModel.Container()
			Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(LoadFileForm))
			Me.OpenFileDialog = New System.Windows.Forms.OpenFileDialog()
			Me.GroupBoxImportDestination = New System.Windows.Forms.GroupBox()
			Me._importDestinationText = New System.Windows.Forms.Label()
			Me.MainMenu = New System.Windows.Forms.MainMenu(Me.components)
			Me.MenuItem1 = New System.Windows.Forms.MenuItem()
			Me._fileLoadFieldMapMenuItem = New System.Windows.Forms.MenuItem()
			Me._fileSaveFieldMapMenuItem = New System.Windows.Forms.MenuItem()
			Me.MenuItem7 = New System.Windows.Forms.MenuItem()
			Me.ViewFieldMapMenuItem = New System.Windows.Forms.MenuItem()
			Me.FieldMapMenuBar = New System.Windows.Forms.MenuItem()
			Me._fileMenuCloseItem = New System.Windows.Forms.MenuItem()
			Me.MenuItem4 = New System.Windows.Forms.MenuItem()
			Me._fileRefreshMenuItem = New System.Windows.Forms.MenuItem()
			Me.MenuItem2 = New System.Windows.Forms.MenuItem()
			Me.PreviewMenuFile = New System.Windows.Forms.MenuItem()
			Me._importMenuPreviewErrorsItem = New System.Windows.Forms.MenuItem()
			Me._importMenuPreviewFoldersAndCodesItem = New System.Windows.Forms.MenuItem()
			Me.ImportFileMenu = New System.Windows.Forms.MenuItem()
			Me.MenuItem5 = New System.Windows.Forms.MenuItem()
			Me._importMenuSendEmailNotificationItem = New System.Windows.Forms.MenuItem()
			Me._importMenuForceFolderPreviewItem = New System.Windows.Forms.MenuItem()
			Me._saveFieldMapDialog = New System.Windows.Forms.SaveFileDialog()
			Me._loadFieldMapDialog = New System.Windows.Forms.OpenFileDialog()
			Me.TabControl1 = New System.Windows.Forms.TabControl()
			Me._loadFileTab = New System.Windows.Forms.TabPage()
			Me._startLineNumber = New System.Windows.Forms.NumericUpDown()
			Me._startLineNumberLabel = New System.Windows.Forms.Label()
			Me.LabelFileEncoding = New System.Windows.Forms.Label()
			Me.GroupBoxLoadFile = New System.Windows.Forms.GroupBox()
			Me._browseButton = New System.Windows.Forms.Button()
			Me._filePath = New System.Windows.Forms.TextBox()
			Me._firstLineContainsColumnNames = New System.Windows.Forms.CheckBox()
			Me.GroupBoxFileColumnHeaders = New System.Windows.Forms.GroupBox()
			Me._fileColumnHeaders = New System.Windows.Forms.ListBox()
			Me.GroupBoxCharacters = New System.Windows.Forms.GroupBox()
			Me._hierarchicalValueDelimiter = New System.Windows.Forms.ComboBox()
			Me.LabelNestedValue = New System.Windows.Forms.Label()
			Me._multiRecordDelimiter = New System.Windows.Forms.ComboBox()
			Me.LabelMultiValue = New System.Windows.Forms.Label()
			Me.LabelQuote = New System.Windows.Forms.Label()
			Me._quoteDelimiter = New System.Windows.Forms.ComboBox()
			Me.LabelNewline = New System.Windows.Forms.Label()
			Me._newLineDelimiter = New System.Windows.Forms.ComboBox()
			Me.LabelColumn = New System.Windows.Forms.Label()
			Me._recordDelimiter = New System.Windows.Forms.ComboBox()
			Me._fieldMapTab = New System.Windows.Forms.TabPage()
			Me.GroupBoxOverlayIdentifier = New System.Windows.Forms.GroupBox()
			Me._overlayIdentifier = New System.Windows.Forms.ComboBox()
			Me.GroupBoxOverlayBehavior = New System.Windows.Forms.GroupBox()
			Me._overlayBehavior = New System.Windows.Forms.ComboBox()
			Me._fieldMap = New FieldMap()
			Me.GroupBoxExtractedText = New System.Windows.Forms.GroupBox()
			Me._overlayExtractedText = New System.Windows.Forms.ComboBox()
			Me.Label9 = New System.Windows.Forms.Label()
			Me._extractedTextValueContainsFileLocation = New System.Windows.Forms.CheckBox()
			Me.GroupBoxOverwrite = New System.Windows.Forms.GroupBox()
			Me._overwriteDropdown = New System.Windows.Forms.ComboBox()
			Me.GroupBoxFolderInfo = New System.Windows.Forms.GroupBox()
			Me._buildFolderStructure = New System.Windows.Forms.CheckBox()
			Me._destinationFolderPath = New System.Windows.Forms.ComboBox()
			Me.GroupBoxNativeFileBehavior = New System.Windows.Forms.GroupBox()
			Me._advancedButton = New System.Windows.Forms.Button()
			Me._loadNativeFiles = New System.Windows.Forms.CheckBox()
			Me._nativeFilePathField = New System.Windows.Forms.ComboBox()
			Me.Label5 = New System.Windows.Forms.Label()
			Me.HelpProvider1 = New System.Windows.Forms.HelpProvider()
			Me.ViewFieldMapButton = New System.Windows.Forms.Button()
			Me.AutoFieldMapButton = New System.Windows.Forms.Button()
			Me._loadFileEncodingPicker = New EncodingPicker()
			Me._fullTextFileEncodingPicker = New EncodingPicker()
			Me.GroupBoxImportDestination.SuspendLayout()
			Me.TabControl1.SuspendLayout()
			Me._loadFileTab.SuspendLayout()
			CType(Me._startLineNumber, System.ComponentModel.ISupportInitialize).BeginInit()
			Me.GroupBoxLoadFile.SuspendLayout()
			Me.GroupBoxFileColumnHeaders.SuspendLayout()
			Me.GroupBoxCharacters.SuspendLayout()
			Me._fieldMapTab.SuspendLayout()
			Me.GroupBoxOverlayIdentifier.SuspendLayout()
			Me.GroupBoxOverlayBehavior.SuspendLayout()
			Me.GroupBoxExtractedText.SuspendLayout()
			Me.GroupBoxOverwrite.SuspendLayout()
			Me.GroupBoxFolderInfo.SuspendLayout()
			Me.GroupBoxNativeFileBehavior.SuspendLayout()
			Me.SuspendLayout()
			'
			'OpenFileDialog
			'
			Me.OpenFileDialog.Filter = "All files (*.*)|*.*|CSV Files (*.csv)|*.csv|Text Files (*.txt)|*.txt|DAT Files|*." &
			"dat"
			'
			'GroupBoxImportDestination
			'
			Me.GroupBoxImportDestination.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
							Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.GroupBoxImportDestination.Controls.Add(Me._importDestinationText)
			Me.GroupBoxImportDestination.Location = New System.Drawing.Point(7, 4)
			Me.GroupBoxImportDestination.Name = "GroupBoxImportDestination"
			Me.GroupBoxImportDestination.Size = New System.Drawing.Size(737, 40)
			Me.GroupBoxImportDestination.TabIndex = 8
			Me.GroupBoxImportDestination.TabStop = False
			Me.GroupBoxImportDestination.Text = "Import Destination"
			'
			'_importDestinationText
			'
			Me._importDestinationText.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
							Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me._importDestinationText.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me._importDestinationText.Location = New System.Drawing.Point(8, 20)
			Me._importDestinationText.Name = "_importDestinationText"
			Me._importDestinationText.Size = New System.Drawing.Size(717, 18)
			Me._importDestinationText.TabIndex = 5
			'
			'MainMenu
			'
			Me.MainMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuItem1, Me.MenuItem2})
			'
			'MenuItem1
			'
			Me.MenuItem1.Index = 0
			Me.MenuItem1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me._fileLoadFieldMapMenuItem, Me._fileSaveFieldMapMenuItem, Me.MenuItem7, Me.ViewFieldMapMenuItem, Me.FieldMapMenuBar, Me._fileMenuCloseItem, Me.MenuItem4, Me._fileRefreshMenuItem})
			Me.MenuItem1.Text = "&File"
			'
			'_fileLoadFieldMapMenuItem
			'
			Me._fileLoadFieldMapMenuItem.Index = 0
			Me._fileLoadFieldMapMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlO
			Me._fileLoadFieldMapMenuItem.Text = "Load Import Settings (kwe)"
			'
			'_fileSaveFieldMapMenuItem
			'
			Me._fileSaveFieldMapMenuItem.Index = 1
			Me._fileSaveFieldMapMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlS
			Me._fileSaveFieldMapMenuItem.Text = "Save Import Settings (kwe)"
			'
			'MenuItem7
			'
			Me.MenuItem7.Index = 2
			Me.MenuItem7.Text = "-"
			'
			'ViewFieldMapMenuItem
			'
			Me.ViewFieldMapMenuItem.Index = 3
			Me.ViewFieldMapMenuItem.Text = "View/Save Field Map"
			'
			'FieldMapMenuBar
			'
			Me.FieldMapMenuBar.Index = 4
			Me.FieldMapMenuBar.Text = "-"
			'
			'_fileMenuCloseItem
			'
			Me._fileMenuCloseItem.Index = 5
			Me._fileMenuCloseItem.Shortcut = System.Windows.Forms.Shortcut.CtrlW
			Me._fileMenuCloseItem.Text = "Close"
			'
			'MenuItem4
			'
			Me.MenuItem4.Index = 6
			Me.MenuItem4.Text = "-"
			'
			'_fileRefreshMenuItem
			'
			Me._fileRefreshMenuItem.Index = 7
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
			Me.TabControl1.Size = New System.Drawing.Size(738, 546)
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
			Me._loadFileTab.Size = New System.Drawing.Size(730, 520)
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
			Me.GroupBoxLoadFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
							Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.GroupBoxLoadFile.Controls.Add(Me._browseButton)
			Me.GroupBoxLoadFile.Controls.Add(Me._filePath)
			Me.GroupBoxLoadFile.Location = New System.Drawing.Point(8, 4)
			Me.GroupBoxLoadFile.Name = "GroupBoxLoadFile"
			Me.GroupBoxLoadFile.Size = New System.Drawing.Size(720, 48)
			Me.GroupBoxLoadFile.TabIndex = 2
			Me.GroupBoxLoadFile.TabStop = False
			Me.GroupBoxLoadFile.Text = "Load File"
			'
			'_browseButton
			'
			Me._browseButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me._browseButton.Location = New System.Drawing.Point(686, 16)
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
			Me._filePath.Size = New System.Drawing.Size(678, 20)
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
			Me._fileColumnHeaders.IntegralHeight = False
			Me._fileColumnHeaders.Location = New System.Drawing.Point(10, 18)
			Me._fileColumnHeaders.Name = "_fileColumnHeaders"
			Me._fileColumnHeaders.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
			Me._fileColumnHeaders.Size = New System.Drawing.Size(480, 375)
			Me._fileColumnHeaders.TabIndex = 17
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
			Me._fieldMapTab.Controls.Add(Me.ViewFieldMapButton)
			Me._fieldMapTab.Controls.Add(Me.AutoFieldMapButton)
			Me._fieldMapTab.Controls.Add(Me.GroupBoxOverlayIdentifier)
			Me._fieldMapTab.Controls.Add(Me.GroupBoxOverlayBehavior)
			Me._fieldMapTab.Controls.Add(Me._fieldMap)
			Me._fieldMapTab.Controls.Add(Me.GroupBoxExtractedText)
			Me._fieldMapTab.Controls.Add(Me.GroupBoxOverwrite)
			Me._fieldMapTab.Controls.Add(Me.GroupBoxFolderInfo)
			Me._fieldMapTab.Controls.Add(Me.GroupBoxNativeFileBehavior)
			Me._fieldMapTab.Location = New System.Drawing.Point(4, 22)
			Me._fieldMapTab.Name = "_fieldMapTab"
			Me._fieldMapTab.Size = New System.Drawing.Size(730, 520)
			Me._fieldMapTab.TabIndex = 1
			Me._fieldMapTab.Text = "Field Map"
			'
			'GroupBoxOverlayIdentifier
			'
			Me.GroupBoxOverlayIdentifier.Controls.Add(Me._overlayIdentifier)
			Me.GroupBoxOverlayIdentifier.Location = New System.Drawing.Point(4, 385)
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
			'GroupBoxOverlayBehavior
			'
			Me.GroupBoxOverlayBehavior.Controls.Add(Me._overlayBehavior)
			Me.GroupBoxOverlayBehavior.Location = New System.Drawing.Point(4, 449)
			Me.GroupBoxOverlayBehavior.Name = "GroupBoxOverlayBehavior"
			Me.GroupBoxOverlayBehavior.Size = New System.Drawing.Size(234, 56)
			Me.GroupBoxOverlayBehavior.TabIndex = 12
			Me.GroupBoxOverlayBehavior.TabStop = False
			Me.GroupBoxOverlayBehavior.Text = "Multi-Select Field Overlay Behavior"
			'
			'_overlayBehavior
			'
			Me._overlayBehavior.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._overlayBehavior.Enabled = False
			Me._overlayBehavior.Items.AddRange(New Object() {"Select...", "Merge Values", "Replace Values", "Use Field Settings"})
			Me._overlayBehavior.Location = New System.Drawing.Point(8, 24)
			Me._overlayBehavior.Name = "_overlayBehavior"
			Me._overlayBehavior.Size = New System.Drawing.Size(220, 21)
			Me._overlayBehavior.TabIndex = 29
			'
			'_fieldMap
			'
			Me._fieldMap.Location = New System.Drawing.Point(4, 0)
			Me._fieldMap.Name = "_fieldMap"
			Me.HelpProvider1.SetShowHelp(Me._fieldMap, True)
			Me._fieldMap.Size = New System.Drawing.Size(720, 288)
			Me._fieldMap.TabIndex = 0
			'
			'GroupBoxExtractedText
			'
			Me.GroupBoxExtractedText.Controls.Add(Me._fullTextFileEncodingPicker)
			Me.GroupBoxExtractedText.Controls.Add(Me.Label9)
			Me.GroupBoxExtractedText.Controls.Add(Me._overlayExtractedText)
			Me.GroupBoxExtractedText.Controls.Add(Me._extractedTextValueContainsFileLocation)
			Me.GroupBoxExtractedText.Location = New System.Drawing.Point(484, 321)
			Me.GroupBoxExtractedText.Name = "GroupBoxExtractedText"
			Me.GroupBoxExtractedText.Size = New System.Drawing.Size(234, 130)
			Me.GroupBoxExtractedText.TabIndex = 14
			Me.GroupBoxExtractedText.TabStop = False
			Me.GroupBoxExtractedText.Text = "Extracted Text"
			'
			'_overlayIdentifier
			'
			Me._overlayExtractedText.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._overlayExtractedText.Enabled = False
			Me._overlayExtractedText.Location = New System.Drawing.Point(8, 40)
			Me._overlayExtractedText.Name = "_overlayExtractedText"
			Me._overlayExtractedText.Size = New System.Drawing.Size(220, 21)
			Me._overlayExtractedText.TabIndex = 28
			'
			'Label9
			'
			Me.Label9.Location = New System.Drawing.Point(8, 70)
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
			Me.GroupBoxOverwrite.Location = New System.Drawing.Point(4, 321)
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
			Me.GroupBoxFolderInfo.Location = New System.Drawing.Point(244, 321)
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
			Me.GroupBoxNativeFileBehavior.Location = New System.Drawing.Point(244, 397)
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
			'Button1
			'
			Me.ViewFieldMapButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
			Me.ViewFieldMapButton.Location = New System.Drawing.Point(4, 287)
			Me.ViewFieldMapButton.Name = "Button1"
			Me.ViewFieldMapButton.Size = New System.Drawing.Size(118, 21)
			Me.ViewFieldMapButton.TabIndex = 24
			Me.ViewFieldMapButton.Text = "View/Save Field Map"
			'
			'Button2
			'
			Me.AutoFieldMapButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
			Me.AutoFieldMapButton.Location = New System.Drawing.Point(130, 287)
			Me.AutoFieldMapButton.Name = "Button2"
			Me.AutoFieldMapButton.Size = New System.Drawing.Size(118, 21)
			Me.AutoFieldMapButton.TabIndex = 25
			Me.AutoFieldMapButton.Text = "Auto Map Fields"
			'
			'_loadFileEncodingPicker
			'
			Me._loadFileEncodingPicker.Location = New System.Drawing.Point(12, 127)
			Me._loadFileEncodingPicker.Name = "_loadFileEncodingPicker"
			Me._loadFileEncodingPicker.SelectedEncoding = Nothing
			Me._loadFileEncodingPicker.Size = New System.Drawing.Size(200, 21)
			Me._loadFileEncodingPicker.TabIndex = 5
			'
			'_fullTextFileEncodingPicker
			'
			Me._fullTextFileEncodingPicker.Location = New System.Drawing.Point(8, 90)
			Me._fullTextFileEncodingPicker.Name = "_fullTextFileEncodingPicker"
			Me._fullTextFileEncodingPicker.SelectedEncoding = Nothing
			Me._fullTextFileEncodingPicker.Size = New System.Drawing.Size(200, 21)
			Me._fullTextFileEncodingPicker.TabIndex = 26
			'
			'LoadFileForm
			'
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.ClientSize = New System.Drawing.Size(754, 619)
			Me.Controls.Add(Me.TabControl1)
			Me.Controls.Add(Me.GroupBoxImportDestination)
			Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
			Me.Menu = Me.MainMenu
			Me.MinimumSize = New System.Drawing.Size(762, 646)
			Me.Name = "LoadFileForm"
			Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
			Me.Text = "Relativity Desktop Client | Import Load File"
			Me.GroupBoxImportDestination.ResumeLayout(False)
			Me.TabControl1.ResumeLayout(False)
			Me._loadFileTab.ResumeLayout(False)
			CType(Me._startLineNumber, System.ComponentModel.ISupportInitialize).EndInit()
			Me.GroupBoxLoadFile.ResumeLayout(False)
			Me.GroupBoxLoadFile.PerformLayout()
			Me.GroupBoxFileColumnHeaders.ResumeLayout(False)
			Me.GroupBoxCharacters.ResumeLayout(False)
			Me._fieldMapTab.ResumeLayout(False)
			Me.GroupBoxOverlayIdentifier.ResumeLayout(False)
			Me.GroupBoxOverlayBehavior.ResumeLayout(False)
			Me.GroupBoxExtractedText.ResumeLayout(False)
			Me.GroupBoxOverwrite.ResumeLayout(False)
			Me.GroupBoxFolderInfo.ResumeLayout(False)
			Me.GroupBoxNativeFileBehavior.ResumeLayout(False)
			Me.ResumeLayout(False)

		End Sub
#End Region

		Friend WithEvents _application As Global.Relativity.Desktop.Client.Application
		Private WithEvents _advancedFileForm As AdvancedFileLocation
		Private _loadFile As kCura.WinEDDS.LoadFile

		Friend ReadOnly Property ReadyToRun() As Boolean
			Get
				Return True
			End Get
		End Property

		Private Function GetOverwrite() As Global.Relativity.ImportOverwriteType
			If _overwriteDropdown.SelectedItem Is Nothing Then Return Global.Relativity.ImportOverwriteType.Append
			Select Case _overwriteDropdown.SelectedItem.ToString.ToLower
				Case "append only"
					Return Global.Relativity.ImportOverwriteType.Append
				Case "overlay only"
					Return Global.Relativity.ImportOverwriteType.Overlay
				Case "append/overlay"
					Return Global.Relativity.ImportOverwriteType.AppendOverlay
				Case Else
					Throw New IndexOutOfRangeException("'" & _overwriteDropdown.SelectedItem.ToString.ToLower & "' isn't a valid option.")
			End Select
		End Function

		Private Function GetOverwriteDropdownItem(ByVal input As String) As String
			Select Case CType([Enum].Parse(GetType(Global.Relativity.ImportOverwriteType), input, True), Global.Relativity.ImportOverwriteType)
				Case Global.Relativity.ImportOverwriteType.Append
					Return "Append Only"
				Case Global.Relativity.ImportOverwriteType.Overlay
					Return "Overlay Only"
				Case Global.Relativity.ImportOverwriteType.AppendOverlay
					Return "Append/Overlay"
				Case Else
					Throw New IndexOutOfRangeException("'" & input.ToLower & "' isn't a valid option.")
			End Select
		End Function

		Private Function GetOverlayBehavior() As LoadFile.FieldOverlayBehavior?
			If _overlayBehavior.SelectedItem Is Nothing Then Return Nothing
			Select Case _overlayBehavior.SelectedItem.ToString.ToLower
				Case "select..."
					Return Nothing
				Case "use field settings"
					Return LoadFile.FieldOverlayBehavior.UseRelativityDefaults
				Case "merge values"
					Return LoadFile.FieldOverlayBehavior.MergeAll
				Case "replace values"
					Return LoadFile.FieldOverlayBehavior.ReplaceAll
				Case Else
					Throw New IndexOutOfRangeException("'" & _overlayBehavior.SelectedItem.ToString.ToLower & "' isn't a valid option.")
			End Select
		End Function

		Private Function GetOverlayBehaviorDropdownItem(ByVal behavior As LoadFile.FieldOverlayBehavior?) As String
			If behavior Is Nothing Then Return "Select..."
			Select Case behavior
				Case LoadFile.FieldOverlayBehavior.UseRelativityDefaults
					Return "Use Field Settings"
				Case LoadFile.FieldOverlayBehavior.MergeAll
					Return "Merge Values"
				Case LoadFile.FieldOverlayBehavior.ReplaceAll
					Return "Replace Values"
				Case Else
					Throw New IndexOutOfRangeException("'" & behavior.ToString() & "' isn't a valid option.")
			End Select
		End Function

		Private Async Function GetSuitableKeyFields() As Task(Of kCura.WinEDDS.DocumentField())
			Dim retval As New System.Collections.ArrayList
			For Each field As kCura.WinEDDS.DocumentField In Await _application.CurrentFields(Me.LoadFile.ArtifactTypeID, True)
				If (field.FieldCategory = Global.Relativity.FieldCategory.Generic OrElse field.FieldCategory = Global.Relativity.FieldCategory.Identifier) AndAlso field.FieldTypeID = Global.Relativity.FieldTypeHelper.FieldType.Varchar Then
					If field.FieldCategory = Global.Relativity.FieldCategory.Identifier Then field.FieldName &= " [Identifier]"
					retval.Add(field)
				End If
			Next
			retval.Sort(New DocumentFieldCollection.FieldNameComparer)
			Return DirectCast(retval.ToArray(GetType(DocumentField)), DocumentField())
		End Function

		Private Async Function GetLongTextFields() As Task(Of DocumentFieldCollection)
			Dim container As DocumentFieldCollection = Await _application.CurrentNonFileFields(_application.ArtifactTypeID, False)
			Dim longTextFields = New DocumentFieldCollection()
			For Each docInfo As DocumentField In container
				If docInfo.FieldTypeID = Global.Relativity.FieldTypeHelper.FieldType.Text Then
					longTextFields.Add(docInfo)
				End If
			Next
			Return longTextFields

		End Function

		Private Async Function GetMappedLongTextFields() As Task(Of DocumentField())
			Dim mappedLongTextFields As New System.Collections.ArrayList
			For Each field As DocumentField In Await Me.GetLongTextFields()
				If Me._fieldMap.FieldColumns.RightSearchableListItems.Contains(field.FieldName) Then
					mappedLongTextFields.Add(field)
				End If
			Next
			mappedLongTextFields.Sort(New DocumentFieldCollection.FieldNameComparer)
			Return DirectCast(mappedLongTextFields.ToArray(GetType(DocumentField)), DocumentField())
		End Function

		Private Async Function AnyLongTextIsMapped() As Task(Of Boolean)
			If (Await Me.GetMappedLongTextFields).Length > 0 Then
				Return True
			End If
			Return False
		End Function

		Private Sub AppendErrorMessage(ByVal msg As System.Text.StringBuilder, ByVal errorText As String)
			msg.Append(" - ").Append(errorText).Append(vbNewLine)
		End Sub

		Private Async Function IsOverlayBehaviorEnabled() As Task(Of Boolean)
			If GetOverwrite() = Global.Relativity.ImportOverwriteType.Append Then
				Return False
			End If
			For Each fieldName As String In Me._fieldMap.FieldColumns.RightSearchableListItems
				If (Await GetMultiObjectMultiChoiceCache()).Exists(fieldName) Then
					Return True
				End If
			Next
			Return False
		End Function

		Private Async Function PopulateLoadFileObject(ByVal doFormValidation As Boolean) As Task(Of Boolean)
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			If doFormValidation Then
				Dim msg As New System.Text.StringBuilder

				If Not Await Me.EnsureConnection() Then Return False
				If _loadNativeFiles.Checked AndAlso _nativeFilePathField.SelectedIndex = -1 Then Me.AppendErrorMessage(msg, "Native file field unselected")
				If Me.LoadFile.ArtifactTypeID = Global.Relativity.ArtifactType.Document Then
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
				If _fieldMap.FieldColumns.RightSearchableListItems.Count = 0 Then
					Me.AppendErrorMessage(msg, "No file columns mapped")
				End If
				If _fieldMap.LoadFileColumns.LeftSearchableListItems.Count = 0 Then
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
				If CheckIfExtractedTextValueContainsFileLocationFieldIsEnabledAndChecked() AndAlso _fullTextFileEncodingPicker.SelectedEncoding Is Nothing Then
					Me.AppendErrorMessage(msg, "No text file encoding selected for extracted text")
				End If

				If CheckIfExtractedTextValueContainsFileLocationFieldIsEnabledAndChecked() Then
					If _overlayExtractedText.SelectedItem Is Nothing Then
						Me.AppendErrorMessage(msg, "No field selected for extracted text")
					End If
				End If

				If _overlayBehavior.Enabled AndAlso Not GetOverlayBehavior.HasValue Then
					Me.AppendErrorMessage(msg, "No multi-select field overlay behavior has been selected")
				End If

				If msg.ToString.Trim <> String.Empty Then
					msg.Insert(0, "The following issues need to be addressed before continuing:" & vbNewLine & vbNewLine)
					MsgBox(msg.ToString, MsgBoxStyle.Exclamation, "Warning")
					Me.Cursor = System.Windows.Forms.Cursors.Default
					Return False
				End If
			End If
			Me.PopulateLoadFileDelimiters()
			If Not Await Me.EnsureConnection() Then Return Nothing
			Dim currentFields As kCura.WinEDDS.DocumentFieldCollection = Await _application.CurrentFields(Me.LoadFile.ArtifactTypeID, True)
			If currentFields Is Nothing Then
				Me.Cursor = System.Windows.Forms.Cursors.Default
				Return False
			End If
			Me.LoadFile.FieldMap = Utility.ExtractFieldMap(_fieldMap.FieldColumns, _fieldMap.LoadFileColumns, currentFields, Me.LoadFile.ArtifactTypeID, Me.LoadFile.ObjectFieldIdListContainsArtifactId)
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
			LoadFile.FullTextColumnContainsFileLocation = CheckIfExtractedTextValueContainsFileLocationFieldIsEnabledAndChecked()

			If CheckIfExtractedTextValueContainsFileLocationFieldIsEnabledAndChecked() Then
				If _overlayExtractedText.SelectedItem IsNot Nothing Then
					LoadFile.LongTextColumnThatContainsPathToFullText = _overlayExtractedText.SelectedItem.ToString
				End If
			End If

			LoadFile.ExtractedTextFileEncoding = _fullTextFileEncodingPicker.SelectedEncoding
			If CheckIfExtractedTextValueContainsFileLocationFieldIsEnabledAndChecked() AndAlso _fullTextFileEncodingPicker.SelectedEncoding IsNot Nothing Then
				LoadFile.ExtractedTextFileEncodingName = Global.Relativity.SqlNameHelper.GetSqlFriendlyName(_fullTextFileEncodingPicker.SelectedEncoding.EncodingName).ToLower
			End If
			LoadFile.LoadNativeFiles = _loadNativeFiles.Checked
			If _overwriteDropdown.SelectedItem Is Nothing Then
				LoadFile.OverwriteDestination = Global.Relativity.ImportOverwriteType.Append.ToString
			Else
				LoadFile.OverwriteDestination = Me.GetOverwrite.ToString
			End If
			'This value comes from kCura.Relativity.DataReaderClient.OverwriteModeEnum, but is not referenced to prevent circular dependencies.
			If LoadFile.OverwriteDestination = Global.Relativity.ImportOverwriteType.Overlay.ToString Then
				LoadFile.IdentityFieldId = DirectCast(_overlayIdentifier.SelectedItem, DocumentField).FieldID
			Else
				LoadFile.IdentityFieldId = -1
			End If
			LoadFile.FirstLineContainsHeaders = _firstLineContainsColumnNames.Checked
			If System.IO.File.Exists(_filePath.Text) Then
				LoadFile.FilePath = _filePath.Text
			End If
			LoadFile.SelectedIdentifierField = Await _application.GetDocumentFieldFromName((Await _application.GetCaseIdentifierFields(Me.LoadFile.ArtifactTypeID))(0))

			If _overlayBehavior.Enabled Then
				LoadFile.OverlayBehavior = Me.GetOverlayBehavior
			Else
				LoadFile.OverlayBehavior = Nothing
			End If

			If _loadNativeFiles.Checked Then
				If Not _nativeFilePathField.SelectedItem Is Nothing Then
					LoadFile.NativeFilePathColumn = _nativeFilePathField.SelectedItem.ToString
				Else
					LoadFile.NativeFilePathColumn = Nothing
				End If
				'Add the file field as a mapped field for non document object types
				If Me.LoadFile.ArtifactTypeID <> Global.Relativity.ArtifactType.Document Then
					For Each field As DocumentField In currentFields.AllFields
						If field.FieldTypeID = Global.Relativity.FieldTypeHelper.FieldType.File Then
							Dim openParenIndex As Int32 = LoadFile.NativeFilePathColumn.LastIndexOf("("c) + 1
							Dim closeParenIndex As Int32 = LoadFile.NativeFilePathColumn.LastIndexOf(")"c)
							Dim nativePathColumn As Int32 = Int32.Parse(LoadFile.NativeFilePathColumn.Substring(openParenIndex, closeParenIndex - openParenIndex)) - 1
							LoadFile.FieldMap.Add(New kCura.WinEDDS.LoadFileFieldMap.LoadFileFieldMapItem(field, nativePathColumn))
						End If
					Next
				End If
			End If
			LoadFile.CreateFolderStructure = _buildFolderStructure.Checked
			'This value comes from kCura.Relativity.DataReaderClient.OverwriteModeEnum, but is not referenced to prevent circular dependencies.
			If LoadFile.OverwriteDestination.ToLower <> Global.Relativity.ImportOverwriteType.Overlay.ToString.ToLower Then
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
			If Me.LoadFile.IdentityFieldId = -1 Then Me.LoadFile.IdentityFieldId = (Await _application.CurrentFields(Me.LoadFile.ArtifactTypeID)).IdentifierFields(0).FieldID
			Me.LoadFile.SendEmailOnLoadCompletion = _importMenuSendEmailNotificationItem.Checked
			Me.LoadFile.ForceFolderPreview = _importMenuForceFolderPreviewItem.Checked

			Me.LoadFile.MoveDocumentsInAppendOverlayMode = String.Equals(Me.LoadFile.OverwriteDestination, Global.Relativity.ImportOverwriteType.AppendOverlay.ToString()) AndAlso Not String.IsNullOrEmpty(Me.LoadFile.FolderStructureContainedInColumn)

			Me.Cursor = System.Windows.Forms.Cursors.Default
			Return True
		End Function

		Private Async Function MarkIdentifierField(ByVal fieldNames As String()) As Task
			Dim identifierFields As String() = Await _application.GetCaseIdentifierFields(Me.LoadFile.ArtifactTypeID)
			Dim i As Int32
			For i = 0 To fieldNames.Length - 1
				If System.Array.IndexOf(identifierFields, fieldNames(i)) <> -1 Then
					fieldNames(i) = fieldNames(i) & " [Identifier]"
				End If
			Next
		End Function

		Public Async Function LoadFormControls(ByVal loadFileObjectUpdatedFromFile As Boolean) As Task
			_multiObjectMultiChoiceCache = Nothing
			If Me.LoadFile.ArtifactTypeID = 0 Then Me.LoadFile.ArtifactTypeID = _application.ArtifactTypeID
			Me.Text = String.Format("Relativity Desktop Client | Import {0} Load File", Await _application.GetObjectTypeName(Me.LoadFile.ArtifactTypeID))
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			Utility.InitializeCharacterDropDown(_recordDelimiter, _loadFile.RecordDelimiter)
			Utility.InitializeCharacterDropDown(_quoteDelimiter, _loadFile.QuoteDelimiter)
			Utility.InitializeCharacterDropDown(_newLineDelimiter, _loadFile.NewlineDelimiter)
			Utility.InitializeCharacterDropDown(_multiRecordDelimiter, _loadFile.MultiRecordDelimiter)
			Utility.InitializeCharacterDropDown(_hierarchicalValueDelimiter, _loadFile.HierarchicalValueDelimiter)
			_importMenuSendEmailNotificationItem.Visible = Await _application.GetSendLoadNotificationEmailEnabledAsync
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
			If loadFileObjectUpdatedFromFile Then           ' has to get called before the loadfileobjectupdatedfromfile block
				_loadFileEncodingPicker.SelectedEncoding = Me.LoadFile.SourceFileEncoding
				_fullTextFileEncodingPicker.SelectedEncoding = Me.LoadFile.ExtractedTextFileEncoding
			End If
			RefreshNativeFilePathFieldAndFileColumnHeaders()
			If Not Await Me.EnsureConnection() Then Return
			Dim caseFieldsCollection As DocumentFieldCollection = Await _application.CurrentNonFileFields(_application.ArtifactTypeID, refresh:=True)
			Dim caseFields As String() = caseFieldsCollection.Names()

			If loadFileObjectUpdatedFromFile Then
				Dim columnHeaders As String()
				If System.IO.File.Exists(Me.LoadFile.FilePath) Then
					columnHeaders = _application.GetColumnHeadersFromLoadFile(Me.LoadFile, _firstLineContainsColumnNames.Checked)
				Else
					MsgBox("The load file specified does not exist.", MsgBoxStyle.Exclamation, "Relativity Desktop Client Warning")
					columnHeaders = New String() {}
				End If
				Await BuildMappingFromLoadFile(caseFieldsCollection, columnHeaders)
				If LoadFile.LoadNativeFiles Then
					_loadNativeFiles.Checked = True
					If LoadFile.NativeFilePathColumn <> String.Empty Then
						_nativeFilePathField.SelectedItem = LoadFile.NativeFilePathColumn
					End If
				End If
				_buildFolderStructure.Checked = LoadFile.CreateFolderStructure
				ActionMenuEnabled = ReadyToRun
			Else
				Await Me.MarkIdentifierField(caseFields)
				_fieldMap.FieldColumns.LeftSearchableList.AddFields(caseFields)
			End If
			'_identifiersDropDown.Items.AddRange(_application.IdentiferFieldDropdownPopulator)
			_overwriteDropdown.SelectedItem = Me.GetOverwriteDropdownItem(LoadFile.OverwriteDestination)
			_overlayBehavior.SelectedItem = Me.GetOverlayBehaviorDropdownItem(LoadFile.OverlayBehavior)
			_overlayBehavior.Enabled = Await IsOverlayBehaviorEnabled()

			_overlayIdentifier.Items.Clear()
			_overlayIdentifier.Items.AddRange(Await Me.GetSuitableKeyFields)
			_importMenuSendEmailNotificationItem.Checked = Me.LoadFile.SendEmailOnLoadCompletion
			_importMenuForceFolderPreviewItem.Checked = Me.LoadFile.ForceFolderPreview
			If Not loadFileObjectUpdatedFromFile Then
				For Each item As DocumentField In _overlayIdentifier.Items
					If item.FieldCategory = Global.Relativity.FieldCategory.Identifier Then
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

			If Me.LoadFile.ArtifactTypeID = Global.Relativity.ArtifactType.Document Then
				If _overwriteDropdown.SelectedItem Is Nothing Then
					_destinationFolderPath.Enabled = _buildFolderStructure.Checked
				Else
					_destinationFolderPath.Enabled = Not (_overwriteDropdown.SelectedItem.ToString.ToLower = "overlay only") AndAlso _buildFolderStructure.Checked
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
							Case Else                            'append only
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

			If Me.LoadFile.ArtifactTypeID = Global.Relativity.ArtifactType.Document Then
				_extractedTextValueContainsFileLocation.Enabled = Await Me.AnyLongTextIsMapped
			End If

			'Loading from KWE
			If Me.LoadFile.LongTextColumnThatContainsPathToFullText IsNot Nothing Then
				_overlayExtractedText.Items.Clear()
				_overlayExtractedText.Items.AddRange(Await Me.GetMappedLongTextFields)
				_overlayExtractedText.SelectedItem = Me.GetExtractedTextFieldAsDocField(Me.LoadFile.LongTextColumnThatContainsPathToFullText)
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
		End Function

		Public Property LoadFile() As kCura.WinEDDS.LoadFile
			Get
				If _loadFile Is Nothing Then
					_loadFile = New kCura.WinEDDS.LoadFile
				End If
				If _loadFile.CookieContainer Is Nothing Then
					_loadFile.CookieContainer = Global.Relativity.Desktop.Client.Application.Instance.CookieContainer
				End If
				'If _loadFile.Identity Is Nothing Then
				'	_loadFile.Identity = Global.Relativity.Desktop.Client.Application.Instance.Identity
				'End If
				Return _loadFile
			End Get
			Set(ByVal value As kCura.WinEDDS.LoadFile)
				_loadFile = value
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
				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Height, GroupBoxOverlayBehavior, LayoutRelativePropertyTypeForDifference.Top))
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

			_layoutDifferenceList.ForEach(Sub(x) x.InitializeDifference())

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
				For Each item As Object In _fieldMap.LoadFileColumns.LeftSearchableListItems
					If Not currentHeaderList.Contains(item.ToString) Then currentHeaderList.Add(item.ToString)
				Next
				For Each item As Object In _fieldMap.LoadFileColumns.RightSearchableListItems
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
				_fieldMap.LoadFileColumns.ClearAll()
				PopulateLoadFileDelimiters()
				System.Array.Sort(columnHeaders)
				_fieldMap.LoadFileColumns.RightSearchableList.AddFields(columnHeaders)
				_fileColumnHeaders.Items.AddRange(columnHeaders)
				_nativeFilePathField.Items.AddRange(columnHeaders)
				_destinationFolderPath.Items.AddRange(columnHeaders)
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

		Private Async Sub AutoFieldMap_Click(sender As Object, e As EventArgs) Handles AutoFieldMapButton.Click
			ClearFieldMapping()
			Dim columnHeaders As String() = (_fieldMap.LoadFileColumns.RightSearchableListItems.Cast(Of String).ToArray())
			System.Array.Sort(columnHeaders)
			Await MatchAndAddLoadFileColumns(columnHeaders)
			_fieldMap.ForceRefresh()
			Me._FieldColumns_ItemsShifted()

		End Sub

		Private Sub ClearFieldMapping()
			MoveListBoxFields(_fieldMap.FieldColumns.RightSearchableListItems, _fieldMap.FieldColumns.LeftSearchableList)
			MoveListBoxFields(_fieldMap.LoadFileColumns.LeftSearchableListItems, _fieldMap.LoadFileColumns.RightSearchableList)
		End Sub

		Private Sub MoveListBoxFields(ByRef source As List(Of Object), ByRef destination As SearchableList)
			destination.AddFields(source.ToArray())
			source.Clear()
		End Sub

		Private Async Function MatchAndAddLoadFileColumns(ByVal columnHeaders As IEnumerable(Of String)) As Task
			Dim matchedColumnHeaders = New ArrayList
			Dim matchedFields = New ArrayList
			Dim currentFields As String() = Await _application.GetCaseFields(_application.SelectedCaseFolderID, _application.ArtifactTypeID, False)
			For Each header In columnHeaders
				Dim parsedHeader = ParseHeader(header)
				For Each field In currentFields
					If field.Equals(parsedHeader, StringComparison.InvariantCultureIgnoreCase) Then
						matchedColumnHeaders.Add(header)
						matchedFields.Add(field)
						Exit For
					End If
				Next
			Next
			Dim updatedMatchedFields = AddIdentifierToStrings(Await _application.GetCaseIdentifierFields(_application.ArtifactTypeID), CType(matchedFields.ToArray(GetType(String)), String()))
			Dim updatedMatchedHeaders = CType(matchedColumnHeaders.ToArray(GetType(String)), String())
			CheckAndRemoveStringRange(_fieldMap.FieldColumns.LeftSearchableList, updatedMatchedFields)
			CheckAndAddStringRange(_fieldMap.FieldColumns.RightSearchableList, updatedMatchedFields)
			CheckAndRemoveStringRange(_fieldMap.LoadFileColumns.RightSearchableList, updatedMatchedHeaders)
			CheckAndAddStringRange(_fieldMap.LoadFileColumns.LeftSearchableList, updatedMatchedHeaders)
		End Function

		Private Sub CheckAndRemoveStringRange(ByRef listBox As SearchableList, ByVal rangeToRemove As String())
			For Each item In rangeToRemove
				If listBox.DataSource.Contains(item) Then
					listBox.RemoveField(item)
				End If
			Next
		End Sub

		Private Sub CheckAndAddStringRange(ByRef listBox As SearchableList, ByVal rangeToAdd As String())
			For Each item In rangeToAdd
				If Not listBox.DataSource.Contains(item) Then
					listBox.AddField(item)
				End If
			Next
		End Sub

		Private Function AddIdentifierToStrings(ByVal identifiers As String(), ByVal range As String()) As String()
			For Each item In range
				If identifiers.Contains(item) Then
					range(Array.IndexOf(range, item)) = item & " [Identifier]"
				End If
			Next
			Return range
		End Function

		Private Function ParseHeader(ByVal header As String) As String
			Dim parsedheader = header
			If (header.EndsWith(")")) Then
				parsedheader = header.Substring(0, header.LastIndexOf("(")).Trim()
			End If
			Return parsedheader
		End Function

		Private Async Sub OpenFileDialog_FileOk(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog.FileOk
			Dim oldfilepath As String = Nothing
			Try
				If Not Await Me.EnsureConnection Then Exit Sub
				oldfilepath = _filePath.Text
				_filePath.Text = OpenFileDialog.FileName
				Await PopulateLoadFileObject(False)
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

		Private Async Sub ImportFileMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ImportFileMenu.Click
			If (Await PopulateLoadFileObject(True)) AndAlso (Await _application.ReadyToLoad(Utility.ExtractFieldNames(_fieldMap.LoadFileColumns.LeftSearchableListItems))) AndAlso (Await _application.ReadyToLoad(Me.LoadFile, False)) Then
				Await _application.ImportLoadFile(Me.LoadFile)
			End If
		End Sub

		Private Sub LoadFileForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
			'Relativity.Desktop.Client.Legacy.Controls.EnhancedMenuProvider.Hook(Me)
			_loadFileEncodingPicker.InitializeDropdown()
			_fullTextFileEncodingPicker.InitializeDropdown()
			_importMenuForceFolderPreviewItem.Checked = _application.TemporaryForceFolderPreview
			If LoadFile.ArtifactTypeID <> Global.Relativity.ArtifactType.Document Then
				_importMenuForceFolderPreviewItem.Checked = False
				_importMenuForceFolderPreviewItem.Enabled = False
			End If
		End Sub

		Private Sub LoadFileForm_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
			'Relativity.Desktop.Client.Legacy.Controls.EnhancedMenuProvider.Unhook()
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

		Private Async Sub _overwriteDestination_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _overwriteDropdown.SelectedIndexChanged
			LoadFile.OverwriteDestination = Me.GetOverwrite.ToString
			If LoadFile.OverwriteDestination.ToLower <> Global.Relativity.ImportOverwriteType.Overlay.ToString.ToLower Then
				For Each field As DocumentField In _overlayIdentifier.Items
					If field.FieldCategory = Global.Relativity.FieldCategory.Identifier Then
						_overlayIdentifier.SelectedItem = field
						Exit For
					End If
				Next
			End If
			Dim overwriteDestination As Global.Relativity.ImportOverwriteType = CType([Enum].Parse(GetType(Global.Relativity.ImportOverwriteType), LoadFile.OverwriteDestination, True), Global.Relativity.ImportOverwriteType)
			If Me.LoadFile.ArtifactTypeID = Global.Relativity.ArtifactType.Document Then
				Select Case overwriteDestination
					Case Global.Relativity.ImportOverwriteType.Append
						_buildFolderStructure.Enabled = True
						_destinationFolderPath.Enabled = _buildFolderStructure.Checked
						_overlayIdentifier.Enabled = False
					Case Global.Relativity.ImportOverwriteType.Overlay
						_destinationFolderPath.Enabled = False
						_buildFolderStructure.Checked = False
						_buildFolderStructure.Enabled = False
						_destinationFolderPath.SelectedItem = Nothing
						_destinationFolderPath.Text = "Select ..."
						_overlayIdentifier.Enabled = True
					Case Else
						_destinationFolderPath.Enabled = True
						_buildFolderStructure.Checked = False
						_buildFolderStructure.Enabled = True
						_destinationFolderPath.SelectedItem = Nothing
						_destinationFolderPath.Text = "Select ..."
						_overlayIdentifier.Enabled = False
				End Select
			ElseIf Me.IsChildObject Then
				Select Case overwriteDestination
					Case Global.Relativity.ImportOverwriteType.Append
						_destinationFolderPath.Enabled = True
						_buildFolderStructure.Checked = True
						_buildFolderStructure.Enabled = False
						_overlayIdentifier.Enabled = False
					Case Global.Relativity.ImportOverwriteType.Overlay
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
				Select Case overwriteDestination
					Case Global.Relativity.ImportOverwriteType.Overlay
						_overlayIdentifier.Enabled = True
					Case Else
						_overlayIdentifier.Enabled = False
				End Select
			End If
			ActionMenuEnabled = ReadyToRun

			_overlayBehavior.Enabled = Await IsOverlayBehaviorEnabled()

		End Sub

		Private Async Sub PreviewMenuFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PreviewMenuFile.Click
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			If (Await PopulateLoadFileObject(True)) AndAlso Await _application.ReadyToLoad(Me.LoadFile, True) Then Await _application.PreviewLoadFile(_loadFile, False, LoadFilePreviewForm.FormType.LoadFile)
			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Sub

		Private Async Sub _fileSaveFieldMapMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _fileSaveFieldMapMenuItem.Click
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			Await PopulateLoadFileObject(False)
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

		Private Async Sub _loadFieldMapDialog_FileOk(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles _loadFieldMapDialog.FileOk
			Dim newLoadFile As LoadFile = Await _application.ReadLoadFile(Me.LoadFile, _loadFieldMapDialog.FileName, False)
			If Not newLoadFile Is Nothing Then
				If newLoadFile.ArtifactTypeID <> Global.Relativity.ArtifactType.Document Then
					newLoadFile.ForceFolderPreview = False
				End If
				_loadFile = newLoadFile
				Await Me.LoadFormControls(True)
			End If
		End Sub

		Private Async Sub _FieldColumns_ItemsShifted() Handles _fieldMap.FieldColumnsItemsShifted
			ActionMenuEnabled = ReadyToRun

			'Cell contains file location is enabled if any long text field is mapped
			_extractedTextValueContainsFileLocation.Enabled = Await Me.AnyLongTextIsMapped()

			'If contains file location checkbox is disabled it should not be checked!
			If Not _extractedTextValueContainsFileLocation.Enabled Then
				_extractedTextValueContainsFileLocation.Checked = False
			End If

			'Extracted Text dropdown is enabled if Cell contains file location is checked and enabled
			_overlayExtractedText.Enabled = CheckIfExtractedTextValueContainsFileLocationFieldIsEnabledAndChecked()

			'If Extracted Text dropdown is disabled it should be cleared!
			If Not _extractedTextValueContainsFileLocation.Enabled Then
				_overlayExtractedText.SelectedItem = Nothing
			End If

			'Get selected item before clearing Extracted Text dropdown items
			Dim selectedItem = Nothing
			If _overlayExtractedText.SelectedItem IsNot Nothing Then
				selectedItem = _overlayExtractedText.SelectedItem
			End If

			'Clear Extracted Text dropdown and then add mapped fields to it
			_overlayExtractedText.Items.Clear()
			_overlayExtractedText.Items.AddRange(Await Me.GetMappedLongTextFields)

			'Set Extracted Text dropdown to previously selected item
			_overlayExtractedText.SelectedItem = selectedItem

			'Set Extracted Text to default if it is mapped and dropdown does not have any selected item
			If _overlayExtractedText.Enabled AndAlso _overlayExtractedText.SelectedItem Is Nothing AndAlso _extractedTextValueContainsFileLocation.Enabled AndAlso _extractedTextValueContainsFileLocation.Checked Then
				Me.SetExtractedTextAsDefault()
			End If

			_fullTextFileEncodingPicker.Enabled = CheckIfExtractedTextValueContainsFileLocationFieldIsEnabledAndChecked()

			'If encoding dropdown is disabled it should be cleared!
			If Not _fullTextFileEncodingPicker.Enabled Then
				_fullTextFileEncodingPicker.SelectedEncoding = Nothing
			End If

			_overlayBehavior.Enabled = Await IsOverlayBehaviorEnabled()

		End Sub

		Private Function GetExtractedTextFieldAsDocField(fieldName As String) As DocumentField
			For Each field As DocumentField In _overlayExtractedText.Items
				If field.FieldName = fieldName Then
					Return field
				End If
			Next
			Return Nothing
		End Function

		Private Sub _LoadFileColumns_ItemsShifted() Handles _fieldMap.LoadFileColumnsItemsShifted
			ActionMenuEnabled = ReadyToRun
		End Sub

		Private Property ActionMenuEnabled() As Boolean
			Get
				Return ImportFileMenu.Enabled AndAlso
				PreviewMenuFile.Enabled AndAlso
				_importMenuPreviewErrorsItem.Enabled AndAlso
				((_importMenuPreviewFoldersAndCodesItem.Enabled AndAlso Me.LoadFile.ArtifactTypeID = Global.Relativity.ArtifactType.Document) OrElse Me.LoadFile.ArtifactTypeID <> Global.Relativity.ArtifactType.Document)
			End Get
			Set(ByVal value As Boolean)
				ImportFileMenu.Enabled = value
				PreviewMenuFile.Enabled = value
				_importMenuPreviewErrorsItem.Enabled = value
				If Me.LoadFile.ArtifactTypeID = Global.Relativity.ArtifactType.Document Then
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

		Private Async Sub _importMenuPreviewErrorsItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _importMenuPreviewErrorsItem.Click
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			If (Await PopulateLoadFileObject(True)) AndAlso Await _application.ReadyToLoad(Me.LoadFile, True) Then Await _application.PreviewLoadFile(_loadFile, True, LoadFilePreviewForm.FormType.LoadFile)
			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Sub

		Private Async Sub _importMenuPreviewFoldersAndCodesItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _importMenuPreviewFoldersAndCodesItem.Click
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			If (Await PopulateLoadFileObject(True)) AndAlso Await _application.ReadyToLoad(Me.LoadFile, True) Then Await _application.PreviewLoadFile(_loadFile, False, LoadFilePreviewForm.FormType.Codes)
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

		Private Async Function BuildMappingFromLoadFile(ByVal caseFieldsCollection As DocumentFieldCollection, ByVal columnHeaders As String()) As Task
			Dim selectedFieldNameList As New ArrayList
			Dim selectedColumnNameList As New ArrayList
			Dim item As LoadFileFieldMap.LoadFileFieldMapItem
			Dim caseFields As String() = caseFieldsCollection.Names()
			_fieldMap.ClearAll()
			For Each item In _loadFile.FieldMap
				If _
				 Not item.DocumentField Is Nothing AndAlso
				 item.NativeFileColumnIndex > -1 AndAlso
				 item.NativeFileColumnIndex < columnHeaders.Length Then
					Dim documentField As DocumentField = caseFieldsCollection.FirstOrDefault(Function(x) x.FieldID = item.DocumentField.FieldID)
					If Not documentField Is Nothing Then
						If documentField.FieldCategoryID = Global.Relativity.FieldCategory.Identifier Then
							selectedFieldNameList.Add(documentField.FieldName & " [Identifier]")
						Else
							selectedFieldNameList.Add(documentField.FieldName)
						End If
						selectedColumnNameList.Add(columnHeaders(item.NativeFileColumnIndex))
					End If
				End If
			Next
			For Each item In _loadFile.FieldMap
				If _
					Not item.DocumentField Is Nothing AndAlso
					columnHeaders.Length = 0 Then
					Dim documentField As DocumentField = caseFieldsCollection.FirstOrDefault(Function(x) x.FieldID = item.DocumentField.FieldID)
					If Not documentField Is Nothing Then
						If documentField.FieldCategoryID = Global.Relativity.FieldCategory.Identifier Then
							selectedFieldNameList.Add(documentField.FieldName & " [Identifier]")
						Else
							selectedFieldNameList.Add(documentField.FieldName)
						End If
					End If
				End If
			Next
			Await Me.MarkIdentifierField(caseFields)
			_fieldMap.MapCaseFieldsToLoadFileFields(caseFields, columnHeaders, selectedFieldNameList, selectedColumnNameList)
		End Function

		Private Sub _fileMenuCloseItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _fileMenuCloseItem.Click
			Me.Close()
		End Sub

		Private Sub _buildFolderStructure_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _buildFolderStructure.CheckedChanged
			If Me.LoadFile.ArtifactTypeID = Global.Relativity.ArtifactType.Document Then
				If _buildFolderStructure.Checked Then
					_destinationFolderPath.Enabled = True
				Else
					_destinationFolderPath.Enabled = False
					_destinationFolderPath.SelectedItem = Nothing
					_destinationFolderPath.Text = "Select ..."
				End If
			ElseIf Me.IsChildObject Then
				Select Case Me.GetOverwrite
					Case Global.Relativity.ImportOverwriteType.Append, Global.Relativity.ImportOverwriteType.AppendOverlay
						_destinationFolderPath.Enabled = True
						_destinationFolderPath.SelectedItem = Nothing
						_destinationFolderPath.Text = "Select ..."
					Case Global.Relativity.ImportOverwriteType.Overlay
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

		Private Async Sub _fileRefreshMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _fileRefreshMenuItem.Click
			_multiObjectMultiChoiceCache = Nothing
			Dim caseFieldsCollection As DocumentFieldCollection = Await _application.CurrentNonFileFields(Me.LoadFile.ArtifactTypeID, refresh:=True)
			Dim caseFields As String() = caseFieldsCollection.Names()
			If caseFields Is Nothing Then Exit Sub
			Await Me.MarkIdentifierField(caseFields)
			Dim fieldName As String
			For Each fieldName In caseFields
				If Not _fieldMap.FieldColumns.RightSearchableListItems.Contains(fieldName) AndAlso Not _fieldMap.FieldColumns.LeftSearchableListItems.Contains(fieldName) Then
					_fieldMap.FieldColumns.LeftSearchableList.AddField(fieldName)
				End If
			Next
			Dim itemsToRemove As New System.Collections.ArrayList
			For Each fieldName In _fieldMap.FieldColumns.LeftSearchableListItems
				If Array.IndexOf(caseFields, (fieldName)) = -1 Then
					itemsToRemove.Add(fieldName)
				End If
			Next
			For Each fieldName In itemsToRemove
				_fieldMap.FieldColumns.LeftSearchableList.RemoveField(fieldName)
			Next

			itemsToRemove = New Collections.ArrayList
			For Each fieldName In _fieldMap.FieldColumns.RightSearchableListItems
				If Array.IndexOf(caseFields, (fieldName)) = -1 Then
					itemsToRemove.Add(fieldName)
				End If
			Next
			For Each fieldName In itemsToRemove
				_fieldMap.FieldColumns.RightSearchableList.RemoveField(fieldName)
			Next
			Await _application.RefreshSelectedCaseInfoAsync().ConfigureAwait(False)
			Me.LoadFile.CaseInfo = _application.SelectedCaseInfo
			Dim id As Int32 = DirectCast(_overlayIdentifier.SelectedItem, DocumentField).FieldID
			_overlayIdentifier.Items.Clear()
			_overlayIdentifier.Items.AddRange(Await Me.GetSuitableKeyFields)
			For Each field As DocumentField In _overlayIdentifier.Items
				If field.FieldID = id Then
					_overlayIdentifier.SelectedItem = field
					Exit For
				End If
			Next
			If _overlayIdentifier.SelectedItem Is Nothing Then
				For Each field As DocumentField In _overlayIdentifier.Items
					If field.FieldCategory = Global.Relativity.FieldCategory.Identifier Then
						_overlayIdentifier.SelectedItem = field
						Exit For
					End If
				Next
			End If

			Await InitializeDocumentSpecificComponents()
		End Sub

		Private Async Function EnsureConnection() As Task(Of Boolean)
			Dim retval As Boolean = False
			If Not _loadFile Is Nothing AndAlso Not _loadFile.CaseInfo Is Nothing Then
				retval = Await _application.EnsureConnection()
			Else
				retval = True
			End If
			Return retval
		End Function

		Private Async Sub _advancedButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _advancedButton.Click
			Dim isUsingAsperaConnectionMode As Boolean = Await Task.Run(Async Function() Await Application.Instance.IsUsingAsperaConnectionMode().ConfigureAwait(False)).ConfigureAwait(True)
			_advancedFileForm = New AdvancedFileLocation
			_advancedFileForm.IsUsingAsperaConnectionMode = isUsingAsperaConnectionMode
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
			'Enable Extracted Text dropdown if Cell contains file location is checked
			_overlayExtractedText.Enabled = _extractedTextValueContainsFileLocation.Checked

			'Set Extracted Text as default if Extracted Text dropdown is enabled and set to Nothing and Cell contains file location is enabled and checked
			If _overlayExtractedText.Enabled AndAlso _overlayExtractedText.SelectedItem Is Nothing AndAlso _extractedTextValueContainsFileLocation.Enabled AndAlso _extractedTextValueContainsFileLocation.Checked Then
				Me.SetExtractedTextAsDefault()
			End If

			' Clear out Extracted Text dropdown if Cell contains file location is unchecked
			If Not _extractedTextValueContainsFileLocation.Checked Then
				_overlayExtractedText.SelectedItem = Nothing
			End If

			_fullTextFileEncodingPicker.Enabled = CheckIfExtractedTextValueContainsFileLocationFieldIsEnabledAndChecked()
		End Sub

		Private Sub SetExtractedTextAsDefault()
			For Each field As DocumentField In _overlayExtractedText.Items
				'Is Extracted Text field
				If field.FieldCategory = Global.Relativity.FieldCategory.FullText Then
					_overlayExtractedText.SelectedItem = field
				End If
			Next
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

		Private Sub ViewFieldMapMenuItem_Click(sender As Object, e As EventArgs) Handles ViewFieldMapMenuItem.Click, ViewFieldMapButton.Click
			Dim count As Int32 = Math.Min(_fieldMap.FieldColumns.RightSearchableListItems.Count, _fieldMap.LoadFileColumns.LeftSearchableListItems.Count)
			Dim drv As New DataTable
			If count > 0 Then
				drv.Columns.Add("Destination Field")
				drv.Columns.Add("Source File Column")
				Dim thisSelectedColumn, thisSelectedField As String
				For i As Int32 = 0 To count - 1
					thisSelectedColumn = _fieldMap.LoadFileColumns.LeftSearchableListItems(i).ToString()
					thisSelectedField = _fieldMap.FieldColumns.RightSearchableListItems(i).ToString()
					drv.Rows.Add({thisSelectedField, thisSelectedColumn})
				Next
			Else
				drv.Columns.Add(" ")
				drv.Rows.Add({"No fields successfully mapped"})
			End If
			Dim x As New FieldMapForm
			Dim dgv As DataGridView = x.FieldMapGrid
			dgv.DataSource = drv
			x.ShowDialog()
		End Sub

		Private Function CheckIfExtractedTextValueContainsFileLocationFieldIsEnabledAndChecked() As Boolean
			Return _extractedTextValueContainsFileLocation.Enabled AndAlso _extractedTextValueContainsFileLocation.Checked
		End Function

	End Class
End Namespace
