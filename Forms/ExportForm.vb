Public Class ExportForm
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
	Friend WithEvents GroupBox23 As System.Windows.Forms.GroupBox
	Friend WithEvents ExportMenu As System.Windows.Forms.MenuItem
	Friend WithEvents RunMenu As System.Windows.Forms.MenuItem
	Friend WithEvents _destinationFolderDialog As System.Windows.Forms.FolderBrowserDialog
	Friend WithEvents _exportFullText As System.Windows.Forms.CheckBox
	Friend WithEvents _exportNativeFiles As System.Windows.Forms.CheckBox
	Friend WithEvents _filtersBox As System.Windows.Forms.GroupBox
	Friend WithEvents _filters As System.Windows.Forms.ComboBox
	Friend WithEvents _exportImages As System.Windows.Forms.CheckBox
	Friend WithEvents Label1 As System.Windows.Forms.Label
	Friend WithEvents Label12 As System.Windows.Forms.Label
	Friend WithEvents _loadFileCharacterInformation As System.Windows.Forms.GroupBox
	Friend WithEvents _multiRecordDelimiter As System.Windows.Forms.ComboBox
	Friend WithEvents Label6 As System.Windows.Forms.Label
	Friend WithEvents Label4 As System.Windows.Forms.Label
	Friend WithEvents _quoteDelimiter As System.Windows.Forms.ComboBox
	Friend WithEvents Label3 As System.Windows.Forms.Label
	Friend WithEvents _newLineDelimiter As System.Windows.Forms.ComboBox
	Friend WithEvents Label2 As System.Windows.Forms.Label
	Friend WithEvents _recordDelimiter As System.Windows.Forms.ComboBox
	Friend WithEvents _nativeFileFormat As System.Windows.Forms.ComboBox
	Friend WithEvents _imageFileFormat As System.Windows.Forms.ComboBox
	Friend WithEvents _pickPrecedenceButton As System.Windows.Forms.Button
	Friend WithEvents _productionPrecedenceBox As System.Windows.Forms.GroupBox
	Friend WithEvents MenuItem1 As System.Windows.Forms.MenuItem
	Friend WithEvents _settingsMenuVolumeInfoItem As System.Windows.Forms.MenuItem
	Friend WithEvents _productionPrecedenceList As System.Windows.Forms.ListBox
	Friend WithEvents _useRelativePaths As System.Windows.Forms.RadioButton
	Friend WithEvents _useAbsolutePaths As System.Windows.Forms.RadioButton
	Friend WithEvents _usePrefix As System.Windows.Forms.RadioButton
	Friend WithEvents _prefixText As System.Windows.Forms.TextBox
	Friend WithEvents Label5 As System.Windows.Forms.Label
	Friend WithEvents _overwriteButton As System.Windows.Forms.CheckBox
	Friend WithEvents _browseButton As System.Windows.Forms.Button
	Friend WithEvents _folderPath As System.Windows.Forms.TextBox
	Friend WithEvents _appendOriginalFilename As System.Windows.Forms.CheckBox
	Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
	Friend WithEvents _nativeFileNameSource As System.Windows.Forms.ComboBox
	Friend WithEvents _exportFullTextAsFile As System.Windows.Forms.CheckBox
	Friend WithEvents _imageTypeDropdown As System.Windows.Forms.ComboBox
	Friend WithEvents Label7 As System.Windows.Forms.Label
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
		Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(ExportForm))
		Me.MainMenu1 = New System.Windows.Forms.MainMenu
		Me.ExportMenu = New System.Windows.Forms.MenuItem
		Me.RunMenu = New System.Windows.Forms.MenuItem
		Me.MenuItem1 = New System.Windows.Forms.MenuItem
		Me._settingsMenuVolumeInfoItem = New System.Windows.Forms.MenuItem
		Me._filtersBox = New System.Windows.Forms.GroupBox
		Me._exportFullTextAsFile = New System.Windows.Forms.CheckBox
		Me._exportImages = New System.Windows.Forms.CheckBox
		Me._exportNativeFiles = New System.Windows.Forms.CheckBox
		Me._exportFullText = New System.Windows.Forms.CheckBox
		Me._filters = New System.Windows.Forms.ComboBox
		Me.GroupBox23 = New System.Windows.Forms.GroupBox
		Me.Label7 = New System.Windows.Forms.Label
		Me._imageTypeDropdown = New System.Windows.Forms.ComboBox
		Me._prefixText = New System.Windows.Forms.TextBox
		Me._usePrefix = New System.Windows.Forms.RadioButton
		Me._useAbsolutePaths = New System.Windows.Forms.RadioButton
		Me._useRelativePaths = New System.Windows.Forms.RadioButton
		Me.Label12 = New System.Windows.Forms.Label
		Me._imageFileFormat = New System.Windows.Forms.ComboBox
		Me.Label1 = New System.Windows.Forms.Label
		Me._nativeFileFormat = New System.Windows.Forms.ComboBox
		Me._destinationFolderDialog = New System.Windows.Forms.FolderBrowserDialog
		Me._loadFileCharacterInformation = New System.Windows.Forms.GroupBox
		Me._multiRecordDelimiter = New System.Windows.Forms.ComboBox
		Me.Label6 = New System.Windows.Forms.Label
		Me.Label4 = New System.Windows.Forms.Label
		Me._quoteDelimiter = New System.Windows.Forms.ComboBox
		Me.Label3 = New System.Windows.Forms.Label
		Me._newLineDelimiter = New System.Windows.Forms.ComboBox
		Me.Label2 = New System.Windows.Forms.Label
		Me._recordDelimiter = New System.Windows.Forms.ComboBox
		Me._productionPrecedenceBox = New System.Windows.Forms.GroupBox
		Me._productionPrecedenceList = New System.Windows.Forms.ListBox
		Me._pickPrecedenceButton = New System.Windows.Forms.Button
		Me.Label5 = New System.Windows.Forms.Label
		Me._overwriteButton = New System.Windows.Forms.CheckBox
		Me._browseButton = New System.Windows.Forms.Button
		Me._folderPath = New System.Windows.Forms.TextBox
		Me._appendOriginalFilename = New System.Windows.Forms.CheckBox
		Me.GroupBox3 = New System.Windows.Forms.GroupBox
		Me._nativeFileNameSource = New System.Windows.Forms.ComboBox
		Me._filtersBox.SuspendLayout()
		Me.GroupBox23.SuspendLayout()
		Me._loadFileCharacterInformation.SuspendLayout()
		Me._productionPrecedenceBox.SuspendLayout()
		Me.GroupBox3.SuspendLayout()
		Me.SuspendLayout()
		'
		'MainMenu1
		'
		Me.MainMenu1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.ExportMenu, Me.MenuItem1})
		'
		'ExportMenu
		'
		Me.ExportMenu.Index = 0
		Me.ExportMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.RunMenu})
		Me.ExportMenu.Text = "File"
		'
		'RunMenu
		'
		Me.RunMenu.Index = 0
		Me.RunMenu.Shortcut = System.Windows.Forms.Shortcut.F5
		Me.RunMenu.Text = "Run..."
		'
		'MenuItem1
		'
		Me.MenuItem1.Index = 1
		Me.MenuItem1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me._settingsMenuVolumeInfoItem})
		Me.MenuItem1.Text = "Settings"
		'
		'_settingsMenuVolumeInfoItem
		'
		Me._settingsMenuVolumeInfoItem.Index = 0
		Me._settingsMenuVolumeInfoItem.Text = "Volume Info"
		'
		'_filtersBox
		'
		Me._filtersBox.Controls.Add(Me._exportFullTextAsFile)
		Me._filtersBox.Controls.Add(Me._exportImages)
		Me._filtersBox.Controls.Add(Me._exportNativeFiles)
		Me._filtersBox.Controls.Add(Me._exportFullText)
		Me._filtersBox.Controls.Add(Me._filters)
		Me._filtersBox.Location = New System.Drawing.Point(4, 4)
		Me._filtersBox.Name = "_filtersBox"
		Me._filtersBox.Size = New System.Drawing.Size(572, 76)
		Me._filtersBox.TabIndex = 10
		Me._filtersBox.TabStop = False
		Me._filtersBox.Text = "Export"
		'
		'_exportFullTextAsFile
		'
		Me._exportFullTextAsFile.Enabled = False
		Me._exportFullTextAsFile.Location = New System.Drawing.Point(108, 48)
		Me._exportFullTextAsFile.Name = "_exportFullTextAsFile"
		Me._exportFullTextAsFile.Size = New System.Drawing.Size(152, 20)
		Me._exportFullTextAsFile.TabIndex = 5
		Me._exportFullTextAsFile.Text = "Export Full Text as Files"
		'
		'_exportImages
		'
		Me._exportImages.Checked = True
		Me._exportImages.CheckState = System.Windows.Forms.CheckState.Checked
		Me._exportImages.Location = New System.Drawing.Point(260, 48)
		Me._exportImages.Name = "_exportImages"
		Me._exportImages.Size = New System.Drawing.Size(96, 20)
		Me._exportImages.TabIndex = 4
		Me._exportImages.Text = "Export Images"
		'
		'_exportNativeFiles
		'
		Me._exportNativeFiles.Checked = True
		Me._exportNativeFiles.CheckState = System.Windows.Forms.CheckState.Checked
		Me._exportNativeFiles.Location = New System.Drawing.Point(364, 48)
		Me._exportNativeFiles.Name = "_exportNativeFiles"
		Me._exportNativeFiles.Size = New System.Drawing.Size(124, 20)
		Me._exportNativeFiles.TabIndex = 3
		Me._exportNativeFiles.Text = "Export Native Files"
		'
		'_exportFullText
		'
		Me._exportFullText.Location = New System.Drawing.Point(9, 48)
		Me._exportFullText.Name = "_exportFullText"
		Me._exportFullText.Size = New System.Drawing.Size(108, 20)
		Me._exportFullText.TabIndex = 2
		Me._exportFullText.Text = "Export Full Text"
		'
		'_filters
		'
		Me._filters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._filters.Location = New System.Drawing.Point(8, 20)
		Me._filters.Name = "_filters"
		Me._filters.Size = New System.Drawing.Size(557, 21)
		Me._filters.TabIndex = 1
		'
		'GroupBox23
		'
		Me.GroupBox23.Controls.Add(Me.Label7)
		Me.GroupBox23.Controls.Add(Me._imageTypeDropdown)
		Me.GroupBox23.Controls.Add(Me._prefixText)
		Me.GroupBox23.Controls.Add(Me._usePrefix)
		Me.GroupBox23.Controls.Add(Me._useAbsolutePaths)
		Me.GroupBox23.Controls.Add(Me._useRelativePaths)
		Me.GroupBox23.Controls.Add(Me.Label12)
		Me.GroupBox23.Controls.Add(Me._imageFileFormat)
		Me.GroupBox23.Controls.Add(Me.Label1)
		Me.GroupBox23.Controls.Add(Me._nativeFileFormat)
		Me.GroupBox23.Location = New System.Drawing.Point(3, 168)
		Me.GroupBox23.Name = "GroupBox23"
		Me.GroupBox23.Size = New System.Drawing.Size(293, 278)
		Me.GroupBox23.TabIndex = 12
		Me.GroupBox23.TabStop = False
		Me.GroupBox23.Text = "Export File Formats"
		'
		'Label7
		'
		Me.Label7.Location = New System.Drawing.Point(8, 120)
		Me.Label7.Name = "Label7"
		Me.Label7.Size = New System.Drawing.Size(144, 16)
		Me.Label7.TabIndex = 18
		Me.Label7.Text = "Image Type"
		Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
		'
		'_imageTypeDropdown
		'
		Me._imageTypeDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._imageTypeDropdown.DropDownWidth = 150
		Me._imageTypeDropdown.Items.AddRange(New Object() {"Select...", "Single-page TIF/JPG", "Multi-page TIF", "PDF"})
		Me._imageTypeDropdown.Location = New System.Drawing.Point(8, 136)
		Me._imageTypeDropdown.Name = "_imageTypeDropdown"
		Me._imageTypeDropdown.Size = New System.Drawing.Size(140, 21)
		Me._imageTypeDropdown.TabIndex = 17
		'
		'_prefixText
		'
		Me._prefixText.Enabled = False
		Me._prefixText.Location = New System.Drawing.Point(176, 92)
		Me._prefixText.Name = "_prefixText"
		Me._prefixText.Size = New System.Drawing.Size(104, 20)
		Me._prefixText.TabIndex = 16
		Me._prefixText.Text = ""
		'
		'_usePrefix
		'
		Me._usePrefix.Location = New System.Drawing.Point(160, 76)
		Me._usePrefix.Name = "_usePrefix"
		Me._usePrefix.Size = New System.Drawing.Size(124, 16)
		Me._usePrefix.TabIndex = 15
		Me._usePrefix.Text = "Use Prefix"
		'
		'_useAbsolutePaths
		'
		Me._useAbsolutePaths.Location = New System.Drawing.Point(160, 52)
		Me._useAbsolutePaths.Name = "_useAbsolutePaths"
		Me._useAbsolutePaths.Size = New System.Drawing.Size(124, 16)
		Me._useAbsolutePaths.TabIndex = 14
		Me._useAbsolutePaths.Text = "Use Absolute Paths"
		'
		'_useRelativePaths
		'
		Me._useRelativePaths.Checked = True
		Me._useRelativePaths.Location = New System.Drawing.Point(160, 28)
		Me._useRelativePaths.Name = "_useRelativePaths"
		Me._useRelativePaths.Size = New System.Drawing.Size(124, 16)
		Me._useRelativePaths.TabIndex = 13
		Me._useRelativePaths.TabStop = True
		Me._useRelativePaths.Text = "Use Relative Paths"
		'
		'Label12
		'
		Me.Label12.Location = New System.Drawing.Point(8, 72)
		Me.Label12.Name = "Label12"
		Me.Label12.Size = New System.Drawing.Size(144, 16)
		Me.Label12.TabIndex = 12
		Me.Label12.Text = "Image Format"
		Me.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
		'
		'_imageFileFormat
		'
		Me._imageFileFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._imageFileFormat.DropDownWidth = 150
		Me._imageFileFormat.Location = New System.Drawing.Point(8, 88)
		Me._imageFileFormat.Name = "_imageFileFormat"
		Me._imageFileFormat.Size = New System.Drawing.Size(140, 21)
		Me._imageFileFormat.TabIndex = 11
		'
		'Label1
		'
		Me.Label1.Location = New System.Drawing.Point(8, 24)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(104, 16)
		Me.Label1.TabIndex = 10
		Me.Label1.Text = "Load File Format"
		Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
		'
		'_nativeFileFormat
		'
		Me._nativeFileFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._nativeFileFormat.Items.AddRange(New Object() {"Select...", "Comma-separated (.csv)", "Tab-delimited (.txt)", "Concordance (.dat)", "Custom (.txt)", "HTML (.html)"})
		Me._nativeFileFormat.Location = New System.Drawing.Point(8, 40)
		Me._nativeFileFormat.Name = "_nativeFileFormat"
		Me._nativeFileFormat.Size = New System.Drawing.Size(140, 21)
		Me._nativeFileFormat.TabIndex = 9
		'
		'_loadFileCharacterInformation
		'
		Me._loadFileCharacterInformation.Controls.Add(Me._multiRecordDelimiter)
		Me._loadFileCharacterInformation.Controls.Add(Me.Label6)
		Me._loadFileCharacterInformation.Controls.Add(Me.Label4)
		Me._loadFileCharacterInformation.Controls.Add(Me._quoteDelimiter)
		Me._loadFileCharacterInformation.Controls.Add(Me.Label3)
		Me._loadFileCharacterInformation.Controls.Add(Me._newLineDelimiter)
		Me._loadFileCharacterInformation.Controls.Add(Me.Label2)
		Me._loadFileCharacterInformation.Controls.Add(Me._recordDelimiter)
		Me._loadFileCharacterInformation.Location = New System.Drawing.Point(300, 168)
		Me._loadFileCharacterInformation.Name = "_loadFileCharacterInformation"
		Me._loadFileCharacterInformation.Size = New System.Drawing.Size(276, 120)
		Me._loadFileCharacterInformation.TabIndex = 15
		Me._loadFileCharacterInformation.TabStop = False
		Me._loadFileCharacterInformation.Text = "Native Load File Custom Characters"
		'
		'_multiRecordDelimiter
		'
		Me._multiRecordDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._multiRecordDelimiter.Enabled = False
		Me._multiRecordDelimiter.Location = New System.Drawing.Point(144, 88)
		Me._multiRecordDelimiter.Name = "_multiRecordDelimiter"
		Me._multiRecordDelimiter.Size = New System.Drawing.Size(124, 21)
		Me._multiRecordDelimiter.TabIndex = 15
		'
		'Label6
		'
		Me.Label6.Location = New System.Drawing.Point(144, 72)
		Me.Label6.Name = "Label6"
		Me.Label6.Size = New System.Drawing.Size(120, 16)
		Me.Label6.TabIndex = 14
		Me.Label6.Text = "Multi-Value Delimiter"
		'
		'Label4
		'
		Me.Label4.Location = New System.Drawing.Point(144, 24)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New System.Drawing.Size(100, 16)
		Me.Label4.TabIndex = 13
		Me.Label4.Text = "Quote"
		'
		'_quoteDelimiter
		'
		Me._quoteDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._quoteDelimiter.Enabled = False
		Me._quoteDelimiter.Location = New System.Drawing.Point(144, 40)
		Me._quoteDelimiter.Name = "_quoteDelimiter"
		Me._quoteDelimiter.Size = New System.Drawing.Size(124, 21)
		Me._quoteDelimiter.TabIndex = 12
		'
		'Label3
		'
		Me.Label3.Location = New System.Drawing.Point(12, 24)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New System.Drawing.Size(100, 16)
		Me.Label3.TabIndex = 11
		Me.Label3.Text = "Newline"
		'
		'_newLineDelimiter
		'
		Me._newLineDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._newLineDelimiter.Enabled = False
		Me._newLineDelimiter.Location = New System.Drawing.Point(12, 40)
		Me._newLineDelimiter.Name = "_newLineDelimiter"
		Me._newLineDelimiter.Size = New System.Drawing.Size(124, 21)
		Me._newLineDelimiter.TabIndex = 10
		'
		'Label2
		'
		Me.Label2.Location = New System.Drawing.Point(12, 72)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(100, 16)
		Me.Label2.TabIndex = 9
		Me.Label2.Text = "Column Delimiter"
		'
		'_recordDelimiter
		'
		Me._recordDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._recordDelimiter.Enabled = False
		Me._recordDelimiter.Location = New System.Drawing.Point(12, 88)
		Me._recordDelimiter.Name = "_recordDelimiter"
		Me._recordDelimiter.Size = New System.Drawing.Size(124, 21)
		Me._recordDelimiter.TabIndex = 8
		'
		'_productionPrecedenceBox
		'
		Me._productionPrecedenceBox.Controls.Add(Me._productionPrecedenceList)
		Me._productionPrecedenceBox.Controls.Add(Me._pickPrecedenceButton)
		Me._productionPrecedenceBox.Location = New System.Drawing.Point(300, 300)
		Me._productionPrecedenceBox.Name = "_productionPrecedenceBox"
		Me._productionPrecedenceBox.Size = New System.Drawing.Size(272, 146)
		Me._productionPrecedenceBox.TabIndex = 16
		Me._productionPrecedenceBox.TabStop = False
		Me._productionPrecedenceBox.Text = "Production Precedence"
		'
		'_productionPrecedenceList
		'
		Me._productionPrecedenceList.Location = New System.Drawing.Point(8, 17)
		Me._productionPrecedenceList.Name = "_productionPrecedenceList"
		Me._productionPrecedenceList.Size = New System.Drawing.Size(224, 121)
		Me._productionPrecedenceList.TabIndex = 2
		'
		'_pickPrecedenceButton
		'
		Me._pickPrecedenceButton.Location = New System.Drawing.Point(236, 118)
		Me._pickPrecedenceButton.Name = "_pickPrecedenceButton"
		Me._pickPrecedenceButton.Size = New System.Drawing.Size(24, 20)
		Me._pickPrecedenceButton.TabIndex = 1
		Me._pickPrecedenceButton.Text = "..."
		'
		'Label5
		'
		Me.Label5.Location = New System.Drawing.Point(104, 48)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New System.Drawing.Size(128, 16)
		Me.Label5.TabIndex = 18
		Me.Label5.Text = "Native files named after:"
		Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_overwriteButton
		'
		Me._overwriteButton.Location = New System.Drawing.Point(8, 48)
		Me._overwriteButton.Name = "_overwriteButton"
		Me._overwriteButton.Size = New System.Drawing.Size(100, 16)
		Me._overwriteButton.TabIndex = 7
		Me._overwriteButton.Text = "Overwrite Files"
		'
		'_browseButton
		'
		Me._browseButton.Location = New System.Drawing.Point(540, 20)
		Me._browseButton.Name = "_browseButton"
		Me._browseButton.Size = New System.Drawing.Size(24, 20)
		Me._browseButton.TabIndex = 6
		Me._browseButton.Text = "..."
		'
		'_folderPath
		'
		Me._folderPath.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me._folderPath.Location = New System.Drawing.Point(8, 20)
		Me._folderPath.Name = "_folderPath"
		Me._folderPath.Size = New System.Drawing.Size(532, 20)
		Me._folderPath.TabIndex = 5
		Me._folderPath.Text = "Select a folder ..."
		'
		'_appendOriginalFilename
		'
		Me._appendOriginalFilename.Location = New System.Drawing.Point(416, 48)
		Me._appendOriginalFilename.Name = "_appendOriginalFilename"
		Me._appendOriginalFilename.Size = New System.Drawing.Size(148, 16)
		Me._appendOriginalFilename.TabIndex = 17
		Me._appendOriginalFilename.Text = "Append original filename"
		'
		'GroupBox3
		'
		Me.GroupBox3.Controls.Add(Me._nativeFileNameSource)
		Me.GroupBox3.Controls.Add(Me.Label5)
		Me.GroupBox3.Controls.Add(Me._overwriteButton)
		Me.GroupBox3.Controls.Add(Me._browseButton)
		Me.GroupBox3.Controls.Add(Me._folderPath)
		Me.GroupBox3.Controls.Add(Me._appendOriginalFilename)
		Me.GroupBox3.Location = New System.Drawing.Point(4, 88)
		Me.GroupBox3.Name = "GroupBox3"
		Me.GroupBox3.Size = New System.Drawing.Size(572, 72)
		Me.GroupBox3.TabIndex = 11
		Me.GroupBox3.TabStop = False
		Me.GroupBox3.Text = "Export Location"
		'
		'_nativeFileNameSource
		'
		Me._nativeFileNameSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._nativeFileNameSource.Items.AddRange(New Object() {"Select...", "Identifier", "Begin bates"})
		Me._nativeFileNameSource.Location = New System.Drawing.Point(244, 44)
		Me._nativeFileNameSource.Name = "_nativeFileNameSource"
		Me._nativeFileNameSource.Size = New System.Drawing.Size(152, 21)
		Me._nativeFileNameSource.TabIndex = 19
		Me._nativeFileNameSource.Visible = False
		'
		'ExportForm
		'
		Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
		Me.BackColor = System.Drawing.SystemColors.Control
		Me.ClientSize = New System.Drawing.Size(580, 468)
		Me.Controls.Add(Me._productionPrecedenceBox)
		Me.Controls.Add(Me._loadFileCharacterInformation)
		Me.Controls.Add(Me.GroupBox23)
		Me.Controls.Add(Me.GroupBox3)
		Me.Controls.Add(Me._filtersBox)
		Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
		Me.MaximumSize = New System.Drawing.Size(588, 495)
		Me.Menu = Me.MainMenu1
		Me.MinimumSize = New System.Drawing.Size(588, 495)
		Me.Name = "ExportForm"
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
		Me.Text = "Relativity Desktop Client | Export "
		Me._filtersBox.ResumeLayout(False)
		Me.GroupBox23.ResumeLayout(False)
		Me._loadFileCharacterInformation.ResumeLayout(False)
		Me._productionPrecedenceBox.ResumeLayout(False)
		Me.GroupBox3.ResumeLayout(False)
		Me.ResumeLayout(False)

	End Sub

#End Region

	Private WithEvents _application As kCura.EDDS.WinForm.Application
	Protected _exportFile As kCura.WinEDDS.ExportFile
	Protected WithEvents _precedenceForm As kCura.EDDS.WinForm.ProductionPrecedenceForm
	Protected WithEvents _volumeInfoForm As kCura.EDDS.WinForm.VolumeInfoForm
	Private _volumeInfo As Exporters.VolumeInfo

	Public Property Application() As kCura.EDDS.WinForm.Application
		Get
			Return _application
		End Get
		Set(ByVal value As kCura.EDDS.WinForm.Application)
			_application = value
		End Set
	End Property

	Public Property ExportFile() As kCura.WinEDDS.ExportFile
		Get
			Return _exportFile
		End Get
		Set(ByVal value As kCura.WinEDDS.ExportFile)
			_exportFile = value
		End Set
	End Property

	Private Function ReadyToRun() As Boolean
		Dim retval As Boolean = True
		retval = retval AndAlso System.IO.Directory.Exists(_folderPath.Text)
		retval = retval AndAlso Not _filters.SelectedItem Is Nothing
		If Me.CreateVolume Then
			retval = retval AndAlso Not Me.BuildVolumeInfo Is Nothing
		End If
		Try
			If _exportNativeFiles.Checked Then
				If CType(_nativeFileFormat.SelectedItem, String) = "Select..." Then
					retval = False
				End If
			End If
			If _exportImages.Checked Then
				If CType(_imageFileFormat.SelectedValue, Int32) = -1 Then
					retval = False
				End If
				If _imageTypeDropdown.SelectedIndex = 0 Then retval = False
			End If
			If Me.ExportFile.TypeOfExport = ExportFile.ExportType.Production AndAlso _exportNativeFiles.Checked Then
				If CType(_nativeFileNameSource.SelectedItem, String) = "Select..." Then
					retval = False
				End If
			End If
		Catch ex As System.Exception
			retval = False
		End Try
		Return retval
	End Function

	Private Sub RunMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RunMenu.Click
		Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
		If Not _application.IsConnected(_exportFile.CaseArtifactID) Then
			Me.Cursor = System.Windows.Forms.Cursors.Default
			Exit Sub
		End If
		_exportFile.FolderPath = _folderPath.Text
		Select Case Me.ExportFile.TypeOfExport
			Case ExportFile.ExportType.AncestorSearch
				_exportFile.ViewID = CType(_filters.SelectedValue, Int32)
				_exportFile.LoadFilesPrefix = DirectCast(_filters.SelectedItem, System.Data.DataRowView)(_filters.DisplayMember).ToString
			Case ExportFile.ExportType.ArtifactSearch
				_exportFile.ArtifactID = CType(_filters.SelectedValue, Int32)
				_exportFile.LoadFilesPrefix = DirectCast(_filters.SelectedItem, System.Data.DataRowView)(_filters.DisplayMember).ToString
			Case ExportFile.ExportType.ParentSearch
				_exportFile.ViewID = CType(_filters.SelectedValue, Int32)
				_exportFile.LoadFilesPrefix = DirectCast(_filters.SelectedItem, System.Data.DataRowView)(_filters.DisplayMember).ToString
			Case ExportFile.ExportType.Production
				_exportFile.ArtifactID = CType(_filters.SelectedValue, Int32)
				_exportFile.LoadFilesPrefix = DirectCast(_filters.SelectedItem, System.Data.DataRowView)(_filters.DisplayMember).ToString
				If _nativeFileNameSource.SelectedItem.ToString.ToLower = "identifier" Then
					_exportFile.ExportNativesToFileNamedFrom = ExportNativeWithFilenameFrom.Identifier
				Else
					_exportFile.ExportNativesToFileNamedFrom = ExportNativeWithFilenameFrom.Production
				End If
		End Select
		_exportFile.Overwrite = _overwriteButton.Checked
		_exportFile.ExportFullText = _exportFullText.Checked
		_exportFile.ExportFullTextAsFile = _exportFullTextAsFile.Checked
		_exportFile.ExportNative = _exportNativeFiles.Checked
		_exportFile.QuoteDelimiter = ChrW(CType(_quoteDelimiter.SelectedValue, Int32))
		_exportFile.RecordDelimiter = ChrW(CType(_recordDelimiter.SelectedValue, Int32))
		_exportFile.MultiRecordDelimiter = ChrW(CType(_multiRecordDelimiter.SelectedValue, Int32))
		_exportFile.NewlineDelimiter = ChrW(CType(_newLineDelimiter.SelectedValue, Int32))
		_exportFile.AppendOriginalFileName = _appendOriginalFilename.Checked

		_exportFile.CookieContainer = _application.CookieContainer
		_exportFile.FilePrefix = ""
		If _useAbsolutePaths.Checked Then
			_exportFile.TypeOfExportedFilePath = ExportFile.ExportedFilePathType.Absolute
		ElseIf _useRelativePaths.Checked Then
			_exportFile.TypeOfExportedFilePath = ExportFile.ExportedFilePathType.Relative
		Else
			_exportFile.TypeOfExportedFilePath = ExportFile.ExportedFilePathType.Prefix
			_exportFile.FilePrefix = _prefixText.Text
		End If
		_exportFile.IdentifierColumnName = _application.GetCaseIdentifierFields(0)
		_exportFile.RenameFilesToIdentifier = True
		_exportFile.VolumeInfo = Me.BuildVolumeInfo
		_exportFile.ExportImages = _exportImages.Checked
		_exportFile.LogFileFormat = CType(_imageFileFormat.SelectedValue, kCura.WinEDDS.LoadFileType.FileFormat)
		_exportFile.LoadFileIsHtml = _nativeFileFormat.SelectedIndex = 5
		If _exportFile.LoadFileIsHtml Then
			_exportFile.LoadFileExtension = "html"
		Else
			_exportFile.LoadFileExtension = Me.GetNativeFileFormatExtension()
		End If
		_exportFile.ImagePrecedence = Me.GetImagePrecedence
		_exportFile.TypeOfImage = Me.GetSelectedImageType
		_application.StartSearch(Me.ExportFile)
		Me.Cursor = System.Windows.Forms.Cursors.Default
	End Sub


	Private Function GetImagePrecedence() As Pair()
		If Me.ExportFile.TypeOfExport = ExportFile.ExportType.Production Then
			Return New Pair() {New Pair(Me.ExportFile.ArtifactID.ToString, _filters.SelectedText)}
		End If
		Dim retval(_productionPrecedenceList.Items.Count - 1) As Pair
		Dim i As Int32
		For i = 0 To _productionPrecedenceList.Items.Count - 1
			retval(i) = DirectCast(_productionPrecedenceList.Items(i), Pair)
		Next
		Return retval
	End Function
	Private Function GetNativeFileFormatExtension() As String
		Dim selected As String = CType(_nativeFileFormat.SelectedItem, String)
		If selected.IndexOf("(.txt)") <> -1 Then
			Return "txt"
		ElseIf selected.IndexOf("(.csv)") <> -1 Then
			Return "csv"
		ElseIf selected.IndexOf("(.dat)") <> -1 Then
			Return "dat"
		Else
			Return "txt"
		End If
	End Function

	Private Function GetSelectedImageType() As ExportFile.ImageType
		'Select...
		'Single-page TIF/JPG
		'Multi-page TIF
		'PDF
		Select Case _imageTypeDropdown.SelectedIndex
			Case 1
				Return ExportFile.ImageType.SinglePage
			Case 2
				Return ExportFile.ImageType.MultiPageTiff
			Case 3
				Return ExportFile.ImageType.Pdf
		End Select
	End Function
	Private Sub _browseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _browseButton.Click
		_destinationFolderDialog.ShowDialog()
		_folderPath.Text = _destinationFolderDialog.SelectedPath()
	End Sub

	Private Function BuildVolumeInfo() As Exporters.VolumeInfo
		If _volumeInfo Is Nothing Then
			_volumeInfo = New Exporters.VolumeInfo
			_volumeInfo.SubdirectoryMaxSize = 500
			_volumeInfo.SubdirectoryImagePrefix = "IMG"
			_volumeInfo.SubdirectoryNativePrefix = "NATIVE"
			_volumeInfo.SubdirectoryFullTextPrefix = "TEXT"
			_volumeInfo.SubdirectoryStartNumber = 1
			_volumeInfo.VolumeMaxSize = 650
			_volumeInfo.VolumePrefix = "VOL"
			_volumeInfo.VolumeStartNumber = 1
		End If
		Return _volumeInfo
	End Function

	Private Sub ExportProduction_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
		_filters.DataSource = ExportFile.DataTable
		_filters.DisplayMember = "Name"
		_filters.ValueMember = "ArtifactID"


		kCura.EDDS.WinForm.Utility.InitializeCharacterDropDown(_recordDelimiter, _exportFile.RecordDelimiter)
		kCura.EDDS.WinForm.Utility.InitializeCharacterDropDown(_quoteDelimiter, _exportFile.QuoteDelimiter)
		kCura.EDDS.WinForm.Utility.InitializeCharacterDropDown(_newLineDelimiter, _exportFile.NewlineDelimiter)
		kCura.EDDS.WinForm.Utility.InitializeCharacterDropDown(_multiRecordDelimiter, _exportFile.MultiRecordDelimiter)
		_imageFileFormat.DataSource = kCura.WinEDDS.LoadFileType.GetLoadFileTypes
		_imageFileFormat.DisplayMember = "DisplayName"
		_imageFileFormat.ValueMember = "Value"
		_imageTypeDropdown.SelectedIndex = 0
		Label5.Visible = False
		Select Case Me.ExportFile.TypeOfExport
			Case ExportFile.ExportType.ArtifactSearch
				_filters.Text = "Searches"
				_filtersBox.Text = "Searches"
				Me.Text = "Relativity Desktop Client: Export Saved Search"
				_appendOriginalFilename.Location = New System.Drawing.Point(110, 48)
			Case ExportFile.ExportType.ParentSearch, ExportFile.ExportType.AncestorSearch
				_filters.Text = "Views"
				_filtersBox.Text = "Views"
				Me.Text = "Relativity Desktop Client: Export Folder"
				If Me.ExportFile.TypeOfExport = ExportFile.ExportType.AncestorSearch Then
					Me.Text = "Relativity Desktop Client: Export Folder and Subfolders"
				End If
				_appendOriginalFilename.Location = New System.Drawing.Point(110, 48)
			Case ExportFile.ExportType.Production
				Label5.Visible = True
				_filters.Text = "Productions"
				_filtersBox.Text = "Productions"
				'_exportImages.Text = "Export Produced Images"
				_nativeFileNameSource.Visible = True
				_nativeFileNameSource.SelectedIndex = 0
				Me.Text = "Relativity Desktop Client: Export Production Set"
				'Me.Size = New System.Drawing.Size(588, 350)
				'Me.MaximumSize = New System.Drawing.Size(588, 350)
				'Me.MinimumSize = New System.Drawing.Size(588, 350)
				_productionPrecedenceBox.Visible = False
		End Select
		_nativeFileFormat.SelectedIndex = 0
		_productionPrecedenceList.Items.Add(New Pair("-1", "Original"))
		RunMenu.Enabled = ReadyToRun()
	End Sub

	Private Sub _searchList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _filters.SelectedIndexChanged
		RunMenu.Enabled = ReadyToRun()
	End Sub

	Private Sub _folderPath_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _folderPath.TextChanged
		RunMenu.Enabled = ReadyToRun()
	End Sub

	Private Sub _exportImages_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _exportImages.CheckedChanged
		_useAbsolutePaths.Enabled = True
		_imageFileFormat.Enabled = _exportImages.Checked
		If Not _volumeInfo Is Nothing Then
			_imageTypeDropdown.Enabled = _exportImages.Checked And _volumeInfo.CopyFilesFromRepository
		Else
			_imageTypeDropdown.Enabled = _exportImages.Checked
		End If
		RunMenu.Enabled = ReadyToRun()
	End Sub

	Private Sub _exportNativeFiles_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _exportNativeFiles.CheckedChanged
		_useAbsolutePaths.Enabled = True
		_nativeFileFormat.Enabled = True
		RunMenu.Enabled = ReadyToRun()
	End Sub

	Private Sub ToggleLoadFileCharacterInformation(ByVal enabled As Boolean)
		_newLineDelimiter.Enabled = enabled
		_recordDelimiter.Enabled = enabled
		_quoteDelimiter.Enabled = enabled
		_multiRecordDelimiter.Enabled = enabled
	End Sub
	Private ReadOnly Property CreateVolume() As Boolean
		Get
			Return _exportImages.Checked OrElse _exportNativeFiles.Checked
		End Get
	End Property

	Private Sub _nativeFileFormat_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _nativeFileFormat.SelectedIndexChanged
		Select Case _nativeFileFormat.SelectedItem.ToString
			Case "Comma-separated (.csv)"
				Me.ToggleLoadFileCharacterInformation(False)
				_recordDelimiter.SelectedValue = ChrW(44)
				_quoteDelimiter.SelectedValue = ChrW(34)
				_newLineDelimiter.SelectedValue = ChrW(10)
				_multiRecordDelimiter.SelectedValue = ChrW(59)
			Case "Tab-delimited (.txt)"
				Me.ToggleLoadFileCharacterInformation(False)
				_recordDelimiter.SelectedValue = ChrW(9)
				_quoteDelimiter.SelectedValue = ChrW(34)
				_newLineDelimiter.SelectedValue = ChrW(10)
				_multiRecordDelimiter.SelectedValue = ChrW(59)
			Case "Concordance (.dat)"
				Me.ToggleLoadFileCharacterInformation(False)
				_recordDelimiter.SelectedValue = ChrW(20)
				_quoteDelimiter.SelectedValue = ChrW(254)
				_newLineDelimiter.SelectedValue = ChrW(174)
				_multiRecordDelimiter.SelectedValue = ChrW(59)
			Case "Custom (.txt)"
				Me.ToggleLoadFileCharacterInformation(True)
			Case Else
				Me.ToggleLoadFileCharacterInformation(False)
		End Select
		RunMenu.Enabled = ReadyToRun()
	End Sub

	Private Sub _imageFileFormat_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _imageFileFormat.SelectedIndexChanged
		RunMenu.Enabled = ReadyToRun()
	End Sub

	Private Sub _pickPrecedenceButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _pickPrecedenceButton.Click
		Dim dt As System.Data.DataTable = _application.GetProductionPrecendenceList(ExportFile.CaseInfo)
		If dt Is Nothing Then Exit Sub
		_precedenceForm = New kCura.EDDS.WinForm.ProductionPrecedenceForm
		_precedenceForm.ExportFile = Me.ExportFile
		_precedenceForm.PrecedenceTable = dt
		If _productionPrecedenceList.Items.Count > 0 Then
			Dim item As Pair
			Dim precedenceList(_productionPrecedenceList.Items.Count - 1) As Pair
			Dim i As Int32 = 0
			For i = 0 To _productionPrecedenceList.Items.Count - 1
				precedenceList(i) = DirectCast(_productionPrecedenceList.Items(i), Pair)
			Next
			_precedenceForm.PrecedenceList = precedenceList
		Else
			Dim precedenceList(0) As Pair
			precedenceList(0) = New Pair("-1", "Original")
			_precedenceForm.PrecedenceList = Nothing
		End If
		_precedenceForm.ShowDialog()
	End Sub

	Private Sub _precedenceForm_PrecedenceOK(ByVal precedenceList() As kCura.WinEDDS.Pair) Handles _precedenceForm.PrecedenceOK
		_productionPrecedenceList.Items.Clear()
		_productionPrecedenceList.Items.AddRange(precedenceList)
	End Sub

	Private Function CleanPath(ByRef path As String) As String
		Dim retval As String = path
		retval = retval.Replace("\", "")
		retval = retval.Replace("/", "")
		retval = retval.Replace("*", "")
		retval = retval.Replace(":", "")
		retval = retval.Replace("?", "")
		retval = retval.Replace("""", "")
		retval = retval.Replace("<", "")
		retval = retval.Replace(">", "")
		retval = retval.Replace("|", "")
		Return retval
	End Function

	Private Sub _subDirectoryMaxSize_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
		RunMenu.Enabled = ReadyToRun()
	End Sub

	Private Sub _volumeMaxSize_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
		RunMenu.Enabled = ReadyToRun()
	End Sub

	Private Sub _settingsMenuVolumeInfoItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _settingsMenuVolumeInfoItem.Click
		_volumeInfoForm = New kCura.EDDS.WinForm.VolumeInfoForm(Me.BuildVolumeInfo)
		_volumeInfoForm.ShowDialog()
	End Sub

	Private Sub _volumeInfoForm_VolumeOK(ByVal e As kCura.WinEDDS.Exporters.VolumeInfo) Handles _volumeInfoForm.VolumeOK
		_volumeInfo = e
		If _volumeInfo.CopyFilesFromRepository Then
			_imageTypeDropdown.Enabled = _exportImages.Checked
		Else
			_imageTypeDropdown.SelectedIndex = 1
			_imageTypeDropdown.Enabled = False
		End If
	End Sub

	Private Sub _usePrefix_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _usePrefix.CheckedChanged
		If _usePrefix.Checked Then
			_prefixText.Enabled = True
		Else
			_prefixText.Enabled = False
		End If
	End Sub

	Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _nativeFileNameSource.SelectedIndexChanged
		RunMenu.Enabled = ReadyToRun()
	End Sub

	Private Sub _exportFullText_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _exportFullText.CheckedChanged
		If _exportFullText.Checked Then
			_exportFullTextAsFile.Enabled = True
		Else
			_exportFullTextAsFile.Enabled = False
		End If
	End Sub

	Private Sub _imageTypeDropdown_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _imageTypeDropdown.SelectedIndexChanged
		RunMenu.Enabled = ReadyToRun()
	End Sub
End Class