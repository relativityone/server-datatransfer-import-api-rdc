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
	Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
	Friend WithEvents _overwriteButton As System.Windows.Forms.CheckBox
	Friend WithEvents _browseButton As System.Windows.Forms.Button
	Friend WithEvents _folderPath As System.Windows.Forms.TextBox
	Friend WithEvents GroupBox23 As System.Windows.Forms.GroupBox
	Friend WithEvents ExportMenu As System.Windows.Forms.MenuItem
	Friend WithEvents RunMenu As System.Windows.Forms.MenuItem
	Friend WithEvents _destinationFolderDialog As System.Windows.Forms.FolderBrowserDialog
	Friend WithEvents _exportFullText As System.Windows.Forms.CheckBox
	Friend WithEvents _exportNativeFiles As System.Windows.Forms.CheckBox
	Friend WithEvents _filtersBox As System.Windows.Forms.GroupBox
	Friend WithEvents _filters As System.Windows.Forms.ComboBox
	Friend WithEvents _useAbsolutePaths As System.Windows.Forms.CheckBox
	Friend WithEvents _exportImages As System.Windows.Forms.CheckBox
	Friend WithEvents Label1 As System.Windows.Forms.Label
	Friend WithEvents _volumePrefix As System.Windows.Forms.TextBox
	Friend WithEvents Label5 As System.Windows.Forms.Label
	Friend WithEvents Label7 As System.Windows.Forms.Label
	Friend WithEvents Label8 As System.Windows.Forms.Label
	Friend WithEvents _volumeStartNumber As System.Windows.Forms.NumericUpDown
	Friend WithEvents _volumeMaxSize As System.Windows.Forms.NumericUpDown
	Friend WithEvents _subDirectoryMaxSize As System.Windows.Forms.NumericUpDown
	Friend WithEvents _subdirectoryStartNumber As System.Windows.Forms.NumericUpDown
	Friend WithEvents Label9 As System.Windows.Forms.Label
	Friend WithEvents Label10 As System.Windows.Forms.Label
	Friend WithEvents Label11 As System.Windows.Forms.Label
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
	Friend WithEvents _volumeInformationGroupBox As System.Windows.Forms.GroupBox
	Friend WithEvents _subDirectoryInformationGroupBox As System.Windows.Forms.GroupBox
	Friend WithEvents _nativeFileFormat As System.Windows.Forms.ComboBox
	Friend WithEvents _imageFileFormat As System.Windows.Forms.ComboBox
	Friend WithEvents Label13 As System.Windows.Forms.Label
	Friend WithEvents _subdirectoryImagePrefix As System.Windows.Forms.TextBox
	Friend WithEvents _subDirectoryNativePrefix As System.Windows.Forms.TextBox
	Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
	Friend WithEvents _productionPrecedenceList As System.Windows.Forms.ComboBox
	Friend WithEvents _pickPrecedenceButton As System.Windows.Forms.Button
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
		Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(ExportForm))
		Me.MainMenu1 = New System.Windows.Forms.MainMenu
		Me.ExportMenu = New System.Windows.Forms.MenuItem
		Me.RunMenu = New System.Windows.Forms.MenuItem
		Me._filtersBox = New System.Windows.Forms.GroupBox
		Me._exportImages = New System.Windows.Forms.CheckBox
		Me._exportNativeFiles = New System.Windows.Forms.CheckBox
		Me._exportFullText = New System.Windows.Forms.CheckBox
		Me._filters = New System.Windows.Forms.ComboBox
		Me.GroupBox3 = New System.Windows.Forms.GroupBox
		Me._overwriteButton = New System.Windows.Forms.CheckBox
		Me._browseButton = New System.Windows.Forms.Button
		Me._folderPath = New System.Windows.Forms.TextBox
		Me.GroupBox23 = New System.Windows.Forms.GroupBox
		Me.Label12 = New System.Windows.Forms.Label
		Me._imageFileFormat = New System.Windows.Forms.ComboBox
		Me.Label1 = New System.Windows.Forms.Label
		Me._nativeFileFormat = New System.Windows.Forms.ComboBox
		Me._useAbsolutePaths = New System.Windows.Forms.CheckBox
		Me._destinationFolderDialog = New System.Windows.Forms.FolderBrowserDialog
		Me._volumeInformationGroupBox = New System.Windows.Forms.GroupBox
		Me._volumeMaxSize = New System.Windows.Forms.NumericUpDown
		Me._volumeStartNumber = New System.Windows.Forms.NumericUpDown
		Me.Label8 = New System.Windows.Forms.Label
		Me.Label7 = New System.Windows.Forms.Label
		Me.Label5 = New System.Windows.Forms.Label
		Me._volumePrefix = New System.Windows.Forms.TextBox
		Me._subDirectoryInformationGroupBox = New System.Windows.Forms.GroupBox
		Me._subDirectoryNativePrefix = New System.Windows.Forms.TextBox
		Me.Label13 = New System.Windows.Forms.Label
		Me._subDirectoryMaxSize = New System.Windows.Forms.NumericUpDown
		Me._subdirectoryStartNumber = New System.Windows.Forms.NumericUpDown
		Me.Label9 = New System.Windows.Forms.Label
		Me.Label10 = New System.Windows.Forms.Label
		Me.Label11 = New System.Windows.Forms.Label
		Me._subdirectoryImagePrefix = New System.Windows.Forms.TextBox
		Me._loadFileCharacterInformation = New System.Windows.Forms.GroupBox
		Me._multiRecordDelimiter = New System.Windows.Forms.ComboBox
		Me.Label6 = New System.Windows.Forms.Label
		Me.Label4 = New System.Windows.Forms.Label
		Me._quoteDelimiter = New System.Windows.Forms.ComboBox
		Me.Label3 = New System.Windows.Forms.Label
		Me._newLineDelimiter = New System.Windows.Forms.ComboBox
		Me.Label2 = New System.Windows.Forms.Label
		Me._recordDelimiter = New System.Windows.Forms.ComboBox
		Me.GroupBox1 = New System.Windows.Forms.GroupBox
		Me._pickPrecedenceButton = New System.Windows.Forms.Button
		Me._productionPrecedenceList = New System.Windows.Forms.ComboBox
		Me._filtersBox.SuspendLayout()
		Me.GroupBox3.SuspendLayout()
		Me.GroupBox23.SuspendLayout()
		Me._volumeInformationGroupBox.SuspendLayout()
		CType(Me._volumeMaxSize, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me._volumeStartNumber, System.ComponentModel.ISupportInitialize).BeginInit()
		Me._subDirectoryInformationGroupBox.SuspendLayout()
		CType(Me._subDirectoryMaxSize, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me._subdirectoryStartNumber, System.ComponentModel.ISupportInitialize).BeginInit()
		Me._loadFileCharacterInformation.SuspendLayout()
		Me.GroupBox1.SuspendLayout()
		Me.SuspendLayout()
		'
		'MainMenu1
		'
		Me.MainMenu1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.ExportMenu})
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
		'_filtersBox
		'
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
		'_exportImages
		'
		Me._exportImages.Checked = True
		Me._exportImages.CheckState = System.Windows.Forms.CheckState.Checked
		Me._exportImages.Location = New System.Drawing.Point(244, 48)
		Me._exportImages.Name = "_exportImages"
		Me._exportImages.Size = New System.Drawing.Size(204, 20)
		Me._exportImages.TabIndex = 4
		Me._exportImages.Text = "Export Images"
		'
		'_exportNativeFiles
		'
		Me._exportNativeFiles.Checked = True
		Me._exportNativeFiles.CheckState = System.Windows.Forms.CheckState.Checked
		Me._exportNativeFiles.Location = New System.Drawing.Point(120, 48)
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
		'GroupBox3
		'
		Me.GroupBox3.Controls.Add(Me._overwriteButton)
		Me.GroupBox3.Controls.Add(Me._browseButton)
		Me.GroupBox3.Controls.Add(Me._folderPath)
		Me.GroupBox3.Location = New System.Drawing.Point(4, 88)
		Me.GroupBox3.Name = "GroupBox3"
		Me.GroupBox3.Size = New System.Drawing.Size(572, 72)
		Me.GroupBox3.TabIndex = 11
		Me.GroupBox3.TabStop = False
		Me.GroupBox3.Text = "Export Location"
		'
		'_overwriteButton
		'
		Me._overwriteButton.Location = New System.Drawing.Point(8, 48)
		Me._overwriteButton.Name = "_overwriteButton"
		Me._overwriteButton.Size = New System.Drawing.Size(548, 16)
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
		Me._folderPath.Text = "Select a file ..."
		'
		'GroupBox23
		'
		Me.GroupBox23.Controls.Add(Me.Label12)
		Me.GroupBox23.Controls.Add(Me._imageFileFormat)
		Me.GroupBox23.Controls.Add(Me.Label1)
		Me.GroupBox23.Controls.Add(Me._nativeFileFormat)
		Me.GroupBox23.Controls.Add(Me._useAbsolutePaths)
		Me.GroupBox23.Location = New System.Drawing.Point(3, 308)
		Me.GroupBox23.Name = "GroupBox23"
		Me.GroupBox23.Size = New System.Drawing.Size(281, 120)
		Me.GroupBox23.TabIndex = 12
		Me.GroupBox23.TabStop = False
		Me.GroupBox23.Text = "Export Load File Format"
		'
		'Label12
		'
		Me.Label12.Location = New System.Drawing.Point(8, 72)
		Me.Label12.Name = "Label12"
		Me.Label12.Size = New System.Drawing.Size(80, 16)
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
		Me.Label1.Size = New System.Drawing.Size(80, 16)
		Me.Label1.TabIndex = 10
		Me.Label1.Text = "Native Format"
		Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
		'
		'_nativeFileFormat
		'
		Me._nativeFileFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._nativeFileFormat.Items.AddRange(New Object() {"Select...", "Comma-separated values (.csv)", "Tab-delimited (.txt)", "Concordance (.dat)", "Custom (.txt)"})
		Me._nativeFileFormat.Location = New System.Drawing.Point(8, 40)
		Me._nativeFileFormat.Name = "_nativeFileFormat"
		Me._nativeFileFormat.Size = New System.Drawing.Size(268, 21)
		Me._nativeFileFormat.TabIndex = 9
		'
		'_useAbsolutePaths
		'
		Me._useAbsolutePaths.Location = New System.Drawing.Point(152, 92)
		Me._useAbsolutePaths.Name = "_useAbsolutePaths"
		Me._useAbsolutePaths.Size = New System.Drawing.Size(124, 16)
		Me._useAbsolutePaths.TabIndex = 8
		Me._useAbsolutePaths.Text = "Use Absolute Paths "
		'
		'_volumeInformationGroupBox
		'
		Me._volumeInformationGroupBox.Controls.Add(Me._volumeMaxSize)
		Me._volumeInformationGroupBox.Controls.Add(Me._volumeStartNumber)
		Me._volumeInformationGroupBox.Controls.Add(Me.Label8)
		Me._volumeInformationGroupBox.Controls.Add(Me.Label7)
		Me._volumeInformationGroupBox.Controls.Add(Me.Label5)
		Me._volumeInformationGroupBox.Controls.Add(Me._volumePrefix)
		Me._volumeInformationGroupBox.Location = New System.Drawing.Point(4, 168)
		Me._volumeInformationGroupBox.Name = "_volumeInformationGroupBox"
		Me._volumeInformationGroupBox.Size = New System.Drawing.Size(280, 132)
		Me._volumeInformationGroupBox.TabIndex = 13
		Me._volumeInformationGroupBox.TabStop = False
		Me._volumeInformationGroupBox.Text = "Volume Information"
		'
		'_volumeMaxSize
		'
		Me._volumeMaxSize.Location = New System.Drawing.Point(88, 76)
		Me._volumeMaxSize.Maximum = New Decimal(New Integer() {2000000, 0, 0, 0})
		Me._volumeMaxSize.Name = "_volumeMaxSize"
		Me._volumeMaxSize.Size = New System.Drawing.Size(180, 20)
		Me._volumeMaxSize.TabIndex = 11
		Me._volumeMaxSize.Value = New Decimal(New Integer() {650, 0, 0, 0})
		'
		'_volumeStartNumber
		'
		Me._volumeStartNumber.Location = New System.Drawing.Point(88, 48)
		Me._volumeStartNumber.Maximum = New Decimal(New Integer() {2000000, 0, 0, 0})
		Me._volumeStartNumber.Name = "_volumeStartNumber"
		Me._volumeStartNumber.Size = New System.Drawing.Size(180, 20)
		Me._volumeStartNumber.TabIndex = 10
		Me._volumeStartNumber.Value = New Decimal(New Integer() {1, 0, 0, 0})
		'
		'Label8
		'
		Me.Label8.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.Label8.Location = New System.Drawing.Point(4, 76)
		Me.Label8.Name = "Label8"
		Me.Label8.Size = New System.Drawing.Size(82, 16)
		Me.Label8.TabIndex = 9
		Me.Label8.Text = "Max Size (MB):"
		Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'Label7
		'
		Me.Label7.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.Label7.Location = New System.Drawing.Point(44, 48)
		Me.Label7.Name = "Label7"
		Me.Label7.Size = New System.Drawing.Size(44, 16)
		Me.Label7.TabIndex = 8
		Me.Label7.Text = "Start #:"
		Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'Label5
		'
		Me.Label5.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.Label5.Location = New System.Drawing.Point(48, 24)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New System.Drawing.Size(40, 16)
		Me.Label5.TabIndex = 7
		Me.Label5.Text = "Prefix: "
		Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_volumePrefix
		'
		Me._volumePrefix.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me._volumePrefix.Location = New System.Drawing.Point(88, 20)
		Me._volumePrefix.Name = "_volumePrefix"
		Me._volumePrefix.Size = New System.Drawing.Size(180, 20)
		Me._volumePrefix.TabIndex = 6
		Me._volumePrefix.Text = "VOL"
		'
		'_subDirectoryInformationGroupBox
		'
		Me._subDirectoryInformationGroupBox.Controls.Add(Me._subDirectoryNativePrefix)
		Me._subDirectoryInformationGroupBox.Controls.Add(Me.Label13)
		Me._subDirectoryInformationGroupBox.Controls.Add(Me._subDirectoryMaxSize)
		Me._subDirectoryInformationGroupBox.Controls.Add(Me._subdirectoryStartNumber)
		Me._subDirectoryInformationGroupBox.Controls.Add(Me.Label9)
		Me._subDirectoryInformationGroupBox.Controls.Add(Me.Label10)
		Me._subDirectoryInformationGroupBox.Controls.Add(Me.Label11)
		Me._subDirectoryInformationGroupBox.Controls.Add(Me._subdirectoryImagePrefix)
		Me._subDirectoryInformationGroupBox.Location = New System.Drawing.Point(296, 168)
		Me._subDirectoryInformationGroupBox.Name = "_subDirectoryInformationGroupBox"
		Me._subDirectoryInformationGroupBox.Size = New System.Drawing.Size(280, 132)
		Me._subDirectoryInformationGroupBox.TabIndex = 14
		Me._subDirectoryInformationGroupBox.TabStop = False
		Me._subDirectoryInformationGroupBox.Text = "Subdirectory Information"
		'
		'_subDirectoryNativePrefix
		'
		Me._subDirectoryNativePrefix.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me._subDirectoryNativePrefix.Location = New System.Drawing.Point(88, 48)
		Me._subDirectoryNativePrefix.Name = "_subDirectoryNativePrefix"
		Me._subDirectoryNativePrefix.Size = New System.Drawing.Size(176, 20)
		Me._subDirectoryNativePrefix.TabIndex = 20
		Me._subDirectoryNativePrefix.Text = "NATIVE"
		'
		'Label13
		'
		Me.Label13.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.Label13.Location = New System.Drawing.Point(16, 52)
		Me.Label13.Name = "Label13"
		Me.Label13.Size = New System.Drawing.Size(72, 16)
		Me.Label13.TabIndex = 19
		Me.Label13.Text = "Native Prefix: "
		Me.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_subDirectoryMaxSize
		'
		Me._subDirectoryMaxSize.Location = New System.Drawing.Point(88, 104)
		Me._subDirectoryMaxSize.Maximum = New Decimal(New Integer() {2000000, 0, 0, 0})
		Me._subDirectoryMaxSize.Name = "_subDirectoryMaxSize"
		Me._subDirectoryMaxSize.Size = New System.Drawing.Size(176, 20)
		Me._subDirectoryMaxSize.TabIndex = 17
		Me._subDirectoryMaxSize.Value = New Decimal(New Integer() {500, 0, 0, 0})
		'
		'_subdirectoryStartNumber
		'
		Me._subdirectoryStartNumber.Location = New System.Drawing.Point(88, 76)
		Me._subdirectoryStartNumber.Maximum = New Decimal(New Integer() {2000000, 0, 0, 0})
		Me._subdirectoryStartNumber.Name = "_subdirectoryStartNumber"
		Me._subdirectoryStartNumber.Size = New System.Drawing.Size(176, 20)
		Me._subdirectoryStartNumber.TabIndex = 16
		Me._subdirectoryStartNumber.Value = New Decimal(New Integer() {1, 0, 0, 0})
		'
		'Label9
		'
		Me.Label9.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.Label9.Location = New System.Drawing.Point(28, 104)
		Me.Label9.Name = "Label9"
		Me.Label9.Size = New System.Drawing.Size(60, 16)
		Me.Label9.TabIndex = 15
		Me.Label9.Text = "Max Files:"
		Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'Label10
		'
		Me.Label10.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.Label10.Location = New System.Drawing.Point(42, 80)
		Me.Label10.Name = "Label10"
		Me.Label10.Size = New System.Drawing.Size(44, 16)
		Me.Label10.TabIndex = 14
		Me.Label10.Text = "Start #:"
		Me.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'Label11
		'
		Me.Label11.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.Label11.Location = New System.Drawing.Point(16, 24)
		Me.Label11.Name = "Label11"
		Me.Label11.Size = New System.Drawing.Size(72, 16)
		Me.Label11.TabIndex = 13
		Me.Label11.Text = "Image Prefix: "
		Me.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_subdirectoryImagePrefix
		'
		Me._subdirectoryImagePrefix.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me._subdirectoryImagePrefix.Location = New System.Drawing.Point(88, 20)
		Me._subdirectoryImagePrefix.Name = "_subdirectoryImagePrefix"
		Me._subdirectoryImagePrefix.Size = New System.Drawing.Size(176, 20)
		Me._subdirectoryImagePrefix.TabIndex = 12
		Me._subdirectoryImagePrefix.Text = "IMG"
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
		Me._loadFileCharacterInformation.Location = New System.Drawing.Point(296, 308)
		Me._loadFileCharacterInformation.Name = "_loadFileCharacterInformation"
		Me._loadFileCharacterInformation.Size = New System.Drawing.Size(280, 120)
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
		Me.Label2.Text = "Record Delimiter"
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
		'GroupBox1
		'
		Me.GroupBox1.Controls.Add(Me._pickPrecedenceButton)
		Me.GroupBox1.Controls.Add(Me._productionPrecedenceList)
		Me.GroupBox1.Location = New System.Drawing.Point(4, 436)
		Me.GroupBox1.Name = "GroupBox1"
		Me.GroupBox1.Size = New System.Drawing.Size(572, 52)
		Me.GroupBox1.TabIndex = 16
		Me.GroupBox1.TabStop = False
		Me.GroupBox1.Text = "Production Precedence"
		'
		'_pickPrecedenceButton
		'
		Me._pickPrecedenceButton.Location = New System.Drawing.Point(540, 20)
		Me._pickPrecedenceButton.Name = "_pickPrecedenceButton"
		Me._pickPrecedenceButton.Size = New System.Drawing.Size(24, 20)
		Me._pickPrecedenceButton.TabIndex = 1
		Me._pickPrecedenceButton.Text = "..."
		'
		'_productionPrecedenceList
		'
		Me._productionPrecedenceList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._productionPrecedenceList.Location = New System.Drawing.Point(8, 20)
		Me._productionPrecedenceList.Name = "_productionPrecedenceList"
		Me._productionPrecedenceList.Size = New System.Drawing.Size(532, 21)
		Me._productionPrecedenceList.TabIndex = 0
		'
		'ExportForm
		'
		Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
		Me.ClientSize = New System.Drawing.Size(580, 493)
		Me.Controls.Add(Me.GroupBox1)
		Me.Controls.Add(Me._loadFileCharacterInformation)
		Me.Controls.Add(Me._subDirectoryInformationGroupBox)
		Me.Controls.Add(Me._volumeInformationGroupBox)
		Me.Controls.Add(Me.GroupBox23)
		Me.Controls.Add(Me.GroupBox3)
		Me.Controls.Add(Me._filtersBox)
		Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
		Me.Menu = Me.MainMenu1
		Me.Name = "ExportForm"
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
		Me.Text = "Relativity Desktop Client | Export "
		Me._filtersBox.ResumeLayout(False)
		Me.GroupBox3.ResumeLayout(False)
		Me.GroupBox23.ResumeLayout(False)
		Me._volumeInformationGroupBox.ResumeLayout(False)
		CType(Me._volumeMaxSize, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me._volumeStartNumber, System.ComponentModel.ISupportInitialize).EndInit()
		Me._subDirectoryInformationGroupBox.ResumeLayout(False)
		CType(Me._subDirectoryMaxSize, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me._subdirectoryStartNumber, System.ComponentModel.ISupportInitialize).EndInit()
		Me._loadFileCharacterInformation.ResumeLayout(False)
		Me.GroupBox1.ResumeLayout(False)
		Me.ResumeLayout(False)

	End Sub

#End Region

	Private WithEvents _application As kCura.EDDS.WinForm.Application
	Protected _exportFile As kCura.WinEDDS.ExportFile
	Protected WithEvents _precedenceForm As kCura.EDDS.WinForm.ProductionPrecedenceForm

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
			End If
		Catch ex As System.Exception
			retval = False
		End Try
		Return retval
	End Function

	Private Sub RunMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RunMenu.Click
		Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
		_exportFile.FolderPath = _folderPath.Text
		Select Case Me.ExportFile.TypeOfExport
			Case ExportFile.ExportType.AncestorSearch
				_exportFile.ViewID = CType(_filters.SelectedValue, Int32)
			Case ExportFile.ExportType.ArtifactSearch
				_exportFile.ArtifactID = CType(_filters.SelectedValue, Int32)
			Case ExportFile.ExportType.ParentSearch
				_exportFile.ViewID = CType(_filters.SelectedValue, Int32)
			Case ExportFile.ExportType.Production
				_exportFile.ArtifactID = CType(_filters.SelectedValue, Int32)
		End Select
		_exportFile.Overwrite = _overwriteButton.Checked
		_exportFile.ExportFullText = _exportFullText.Checked
		_exportFile.ExportNative = _exportNativeFiles.Checked
		_exportFile.QuoteDelimiter = Chr(CType(_quoteDelimiter.SelectedValue, Int32))
		_exportFile.RecordDelimiter = Chr(CType(_recordDelimiter.SelectedValue, Int32))
		_exportFile.MultiRecordDelimiter = Chr(CType(_multiRecordDelimiter.SelectedValue, Int32))
		_exportFile.NewlineDelimiter = Chr(CType(_newLineDelimiter.SelectedValue, Int32))

		_exportFile.CookieContainer = _application.CookieContainer
		_exportFile.UseAbsolutePaths = _useAbsolutePaths.Checked
		_exportFile.IdentifierColumnName = _application.GetCaseIdentifierFields(0)
		_exportFile.RenameFilesToIdentifier = True
		_exportFile.VolumeInfo = Me.BuildVolumeInfo
		_exportFile.ExportImages = _exportImages.Checked
		_exportFile.LogFileFormat = CType(_imageFileFormat.SelectedValue, kCura.WinEDDS.LoadFileType.FileFormat)
		_exportFile.LoadFileExtension = Me.GetNativeFileFormatExtension()
		_exportFile.ImagePrecedence = Me.GetImagePrecedence
		_application.StartSearch(Me.ExportFile)
		Me.Cursor = System.Windows.Forms.Cursors.Default
	End Sub


	Private Function GetImagePrecedence() As Pair()
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
	Private Sub _browseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _browseButton.Click
		_destinationFolderDialog.ShowDialog()
		_folderPath.Text = _destinationFolderDialog.SelectedPath()
	End Sub

	Private Function BuildVolumeInfo() As Exporters.VolumeInfo
		Dim retval As New Exporters.VolumeInfo
		retval.SubdirectoryMaxSize = CType(_subDirectoryMaxSize.Value, Int64)
		retval.SubdirectoryImagePrefix = _subdirectoryImagePrefix.Text
		retval.SubdirectoryNativePrefix = _subDirectoryNativePrefix.Text
		retval.SubdirectoryStartNumber = CType(_subdirectoryStartNumber.Value, Int32)
		retval.VolumeMaxSize = CType(_volumeMaxSize.Value, Int64)
		retval.VolumePrefix = _volumePrefix.Text
		retval.VolumeStartNumber = CType(_volumeStartNumber.Value, Int32)
		If retval.SubdirectoryMaxSize = 0 OrElse retval.VolumeMaxSize = 0 Then
			Return Nothing
		Else
			Return retval
		End If
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

		Select Case Me.ExportFile.TypeOfExport
			Case ExportFile.ExportType.ArtifactSearch
				_filters.Text = "Searches"
				_filtersBox.Text = "Searches"
				Me.Text = "Search Export Form"
			Case ExportFile.ExportType.ParentSearch, ExportFile.ExportType.AncestorSearch
				_filters.Text = "Views"
				_filtersBox.Text = "Views"
				Me.Text = "Native Export Form"
			Case ExportFile.ExportType.Production
				_filters.Text = "Productions"
				_filtersBox.Text = "Productions"
				_exportImages.Text = "Export Produced Images"
				Me.Text = "Production Export Form"
		End Select
		_nativeFileFormat.SelectedIndex = 0
		_productionPrecedenceList.Items.Add(New Pair("-1", "Original"))
		RunMenu.Enabled = ReadyToRun()
	End Sub

	Private Sub _searchList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _filters.SelectedIndexChanged
		RunMenu.Enabled = ReadyToRun()
	End Sub

	Private Sub _folderPath_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _folderPath.TextChanged
		RunMenu.Enabled = ReadyToRun()
	End Sub

	Private Sub _exportImages_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _exportImages.CheckedChanged
		Me.ToggleVolumeEnabled(Me.CreateVolume)
		_useAbsolutePaths.Enabled = Me.CreateVolume
		_imageFileFormat.Enabled = _exportImages.Checked
	End Sub

	Private Sub _exportNativeFiles_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _exportNativeFiles.CheckedChanged
		Me.ToggleVolumeEnabled(Me.CreateVolume)
		_useAbsolutePaths.Enabled = Me.CreateVolume
		_nativeFileFormat.Enabled = _exportNativeFiles.Checked
	End Sub

	Private Sub ToggleVolumeEnabled(ByVal enabled As Boolean)
		_volumeMaxSize.Enabled = enabled
		_volumePrefix.Enabled = enabled
		_volumeStartNumber.Enabled = enabled
		_subDirectoryMaxSize.Enabled = enabled
		_subdirectoryImagePrefix.Enabled = enabled
		_subDirectoryNativePrefix.Enabled = enabled
		_subdirectoryStartNumber.Enabled = enabled
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
			Case "Comma-separated values (.csv)"
				Me.ToggleLoadFileCharacterInformation(False)
				_recordDelimiter.SelectedValue = Chr(44)
				_quoteDelimiter.SelectedValue = Chr(34)
				_newLineDelimiter.SelectedValue = Chr(174)
				_multiRecordDelimiter.SelectedValue = Chr(59)
			Case "Tab-delimited (.txt)"
				Me.ToggleLoadFileCharacterInformation(False)
				_recordDelimiter.SelectedValue = Chr(9)
				_quoteDelimiter.SelectedValue = Chr(34)
				_newLineDelimiter.SelectedValue = Chr(174)
				_multiRecordDelimiter.SelectedValue = Chr(59)
			Case "Concordance (.dat)"
				Me.ToggleLoadFileCharacterInformation(False)
				_recordDelimiter.SelectedValue = Chr(20)
				_quoteDelimiter.SelectedValue = Chr(254)
				_newLineDelimiter.SelectedValue = Chr(174)
				_multiRecordDelimiter.SelectedValue = Chr(59)
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
		_precedenceForm = New kCura.EDDS.WinForm.ProductionPrecedenceForm
		_precedenceForm.ExportFile = Me.ExportFile
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
End Class