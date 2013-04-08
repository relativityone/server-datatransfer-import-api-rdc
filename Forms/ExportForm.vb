Imports System.Linq
Imports System.Collections.Generic

Public Class ExportForm
	Inherits System.Windows.Forms.Form
	'Implements kCura.EDDS.WinForm.IExportForm


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
	Public WithEvents MainMenu1 As System.Windows.Forms.MainMenu
	Public WithEvents ExportMenu As System.Windows.Forms.MenuItem
	Public WithEvents RunMenu As System.Windows.Forms.MenuItem
	Public WithEvents _destinationFolderDialog As System.Windows.Forms.FolderBrowserDialog
	Public WithEvents _pickPrecedenceButton As System.Windows.Forms.Button
	Public WithEvents _productionPrecedenceBox As System.Windows.Forms.GroupBox
	Public WithEvents _productionPrecedenceList As System.Windows.Forms.ListBox
	Public WithEvents Label5 As System.Windows.Forms.Label
	Public WithEvents _overwriteCheckBox As System.Windows.Forms.CheckBox
	Public WithEvents _browseButton As System.Windows.Forms.Button
	Public WithEvents _folderPath As System.Windows.Forms.TextBox
	Public WithEvents _appendOriginalFilename As System.Windows.Forms.CheckBox
	Public WithEvents GroupBox3 As System.Windows.Forms.GroupBox
	Public WithEvents _nativeFileNameSource As System.Windows.Forms.ComboBox
	Public WithEvents TabControl1 As System.Windows.Forms.TabControl
	Public WithEvents _dataSourceTabPage As System.Windows.Forms.TabPage
	Public WithEvents _destinationFileTabPage As System.Windows.Forms.TabPage
	Public WithEvents _loadFileCharacterInformation As System.Windows.Forms.GroupBox
	Public WithEvents _multiRecordDelimiter As System.Windows.Forms.ComboBox
	Public WithEvents Label6 As System.Windows.Forms.Label
	Public WithEvents Label4 As System.Windows.Forms.Label
	Public WithEvents _quoteDelimiter As System.Windows.Forms.ComboBox
	Public WithEvents Label3 As System.Windows.Forms.Label
	Public WithEvents _newLineDelimiter As System.Windows.Forms.ComboBox
	Public WithEvents Label2 As System.Windows.Forms.Label
	Public WithEvents _recordDelimiter As System.Windows.Forms.ComboBox
	Public WithEvents GroupBox23 As System.Windows.Forms.GroupBox
	Public WithEvents _imageTypeDropdown As System.Windows.Forms.ComboBox
	Public WithEvents _prefixText As System.Windows.Forms.TextBox
	Public WithEvents _usePrefix As System.Windows.Forms.RadioButton
	Public WithEvents _useAbsolutePaths As System.Windows.Forms.RadioButton
	Public WithEvents _useRelativePaths As System.Windows.Forms.RadioButton
	Public WithEvents _imageFileFormat As System.Windows.Forms.ComboBox
	Public WithEvents GroupBox1 As System.Windows.Forms.GroupBox
	Public WithEvents _copyFilesFromRepository As System.Windows.Forms.CheckBox
	Public WithEvents _subDirectoryInformationGroupBox As System.Windows.Forms.GroupBox
	Public WithEvents _subdirectoryTextPrefix As System.Windows.Forms.TextBox
	Public WithEvents Label8 As System.Windows.Forms.Label
	Public WithEvents _subDirectoryNativePrefix As System.Windows.Forms.TextBox
	Public WithEvents Label13 As System.Windows.Forms.Label
	Public WithEvents _subDirectoryMaxSize As System.Windows.Forms.NumericUpDown
	Public WithEvents _subdirectoryStartNumber As System.Windows.Forms.NumericUpDown
	Public WithEvents Label9 As System.Windows.Forms.Label
	Public WithEvents Label10 As System.Windows.Forms.Label
	Public WithEvents Label11 As System.Windows.Forms.Label
	Public WithEvents _subdirectoryImagePrefix As System.Windows.Forms.TextBox
	Public WithEvents _volumeInformationGroupBox As System.Windows.Forms.GroupBox
	Public WithEvents _volumeMaxSize As System.Windows.Forms.NumericUpDown
	Public WithEvents _volumeStartNumber As System.Windows.Forms.NumericUpDown
	Public WithEvents Label14 As System.Windows.Forms.Label
	Public WithEvents Label15 As System.Windows.Forms.Label
	Public WithEvents Label16 As System.Windows.Forms.Label
	Public WithEvents _volumePrefix As System.Windows.Forms.TextBox
	Public WithEvents Label12 As System.Windows.Forms.Label
	Public WithEvents Label7 As System.Windows.Forms.Label
	Public WithEvents GroupBox2 As System.Windows.Forms.GroupBox
	Public WithEvents _exportImages As System.Windows.Forms.CheckBox
	Public WithEvents Label17 As System.Windows.Forms.Label
	Public WithEvents GroupBox4 As System.Windows.Forms.GroupBox
	Public WithEvents _exportNativeFiles As System.Windows.Forms.CheckBox
	Public WithEvents _exportFullTextAsFile As System.Windows.Forms.CheckBox
	Public WithEvents GroupBox6 As System.Windows.Forms.GroupBox
	Public WithEvents _exportMulticodeFieldsAsNested As System.Windows.Forms.CheckBox
	Public WithEvents _nestedValueDelimiter As System.Windows.Forms.ComboBox
	Public WithEvents Label18 As System.Windows.Forms.Label
	Public WithEvents _filters As System.Windows.Forms.ComboBox
	Public WithEvents _columnSelecter As kCura.Windows.Forms.TwoListBox
	Public WithEvents _filtersBox As System.Windows.Forms.GroupBox
	Public WithEvents _metadataGroup As System.Windows.Forms.GroupBox
	Public WithEvents Label1 As System.Windows.Forms.Label
	Public WithEvents _nativeFileFormat As System.Windows.Forms.ComboBox
	Public WithEvents _dataFileEncoding As kCura.EDDS.WinForm.EncodingPicker
	Public WithEvents Label19 As System.Windows.Forms.Label
	Public WithEvents Label20 As System.Windows.Forms.Label
	Public WithEvents _textFileEncoding As kCura.EDDS.WinForm.EncodingPicker
	Public WithEvents _textFieldPrecedencePicker As kCura.EDDS.WinForm.TextFieldPrecedencePicker
	Public WithEvents Label21 As System.Windows.Forms.Label
	Public WithEvents RefreshMenu As System.Windows.Forms.MenuItem
	Public WithEvents MenuItem3 As System.Windows.Forms.MenuItem
	Public WithEvents SaveExportSettings As System.Windows.Forms.MenuItem
	Public WithEvents LoadExportSettings As System.Windows.Forms.MenuItem
	Public WithEvents _volumeDigitPadding As System.Windows.Forms.NumericUpDown
	Public WithEvents Label22 As System.Windows.Forms.Label
	Public WithEvents _subdirectoryDigitPadding As System.Windows.Forms.NumericUpDown
	Public WithEvents Label23 As System.Windows.Forms.Label
	Public WithEvents Label24 As System.Windows.Forms.Label
	Public WithEvents _startExportAtDocumentNumber As System.Windows.Forms.NumericUpDown
	Public WithEvents _saveExportSettingsDialog As System.Windows.Forms.SaveFileDialog
	Public WithEvents _loadExportSettingsDialog As System.Windows.Forms.OpenFileDialog
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
		Me.components = New System.ComponentModel.Container()
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ExportForm))
		Me.MainMenu1 = New System.Windows.Forms.MainMenu(Me.components)
		Me.ExportMenu = New System.Windows.Forms.MenuItem()
		Me.RunMenu = New System.Windows.Forms.MenuItem()
		Me.SaveExportSettings = New System.Windows.Forms.MenuItem()
		Me.LoadExportSettings = New System.Windows.Forms.MenuItem()
		Me.MenuItem3 = New System.Windows.Forms.MenuItem()
		Me.RefreshMenu = New System.Windows.Forms.MenuItem()
		Me._destinationFolderDialog = New System.Windows.Forms.FolderBrowserDialog()
		Me._productionPrecedenceBox = New System.Windows.Forms.GroupBox()
		Me._productionPrecedenceList = New System.Windows.Forms.ListBox()
		Me._pickPrecedenceButton = New System.Windows.Forms.Button()
		Me.Label5 = New System.Windows.Forms.Label()
		Me._overwriteCheckBox = New System.Windows.Forms.CheckBox()
		Me._browseButton = New System.Windows.Forms.Button()
		Me._folderPath = New System.Windows.Forms.TextBox()
		Me._appendOriginalFilename = New System.Windows.Forms.CheckBox()
		Me.GroupBox3 = New System.Windows.Forms.GroupBox()
		Me._nativeFileNameSource = New System.Windows.Forms.ComboBox()
		Me.TabControl1 = New System.Windows.Forms.TabControl()
		Me._dataSourceTabPage = New System.Windows.Forms.TabPage()
		Me._filtersBox = New System.Windows.Forms.GroupBox()
		Me._startExportAtDocumentNumber = New System.Windows.Forms.NumericUpDown()
		Me.Label24 = New System.Windows.Forms.Label()
		Me.Label18 = New System.Windows.Forms.Label()
		Me._filters = New System.Windows.Forms.ComboBox()
		Me._columnSelecter = New kCura.Windows.Forms.TwoListBox()
		Me._destinationFileTabPage = New System.Windows.Forms.TabPage()
		Me.GroupBox6 = New System.Windows.Forms.GroupBox()
		Me._metadataGroup = New System.Windows.Forms.GroupBox()
		Me.Label21 = New System.Windows.Forms.Label()
		Me._textFieldPrecedencePicker = New kCura.EDDS.WinForm.TextFieldPrecedencePicker()
		Me._textFileEncoding = New kCura.EDDS.WinForm.EncodingPicker()
		Me.Label20 = New System.Windows.Forms.Label()
		Me.Label19 = New System.Windows.Forms.Label()
		Me._dataFileEncoding = New kCura.EDDS.WinForm.EncodingPicker()
		Me.Label1 = New System.Windows.Forms.Label()
		Me._nativeFileFormat = New System.Windows.Forms.ComboBox()
		Me._exportMulticodeFieldsAsNested = New System.Windows.Forms.CheckBox()
		Me._exportFullTextAsFile = New System.Windows.Forms.CheckBox()
		Me.GroupBox4 = New System.Windows.Forms.GroupBox()
		Me._exportNativeFiles = New System.Windows.Forms.CheckBox()
		Me.GroupBox2 = New System.Windows.Forms.GroupBox()
		Me.Label17 = New System.Windows.Forms.Label()
		Me._exportImages = New System.Windows.Forms.CheckBox()
		Me._imageFileFormat = New System.Windows.Forms.ComboBox()
		Me.Label12 = New System.Windows.Forms.Label()
		Me._imageTypeDropdown = New System.Windows.Forms.ComboBox()
		Me.GroupBox1 = New System.Windows.Forms.GroupBox()
		Me._copyFilesFromRepository = New System.Windows.Forms.CheckBox()
		Me._subDirectoryInformationGroupBox = New System.Windows.Forms.GroupBox()
		Me._subdirectoryDigitPadding = New System.Windows.Forms.NumericUpDown()
		Me.Label23 = New System.Windows.Forms.Label()
		Me._subdirectoryTextPrefix = New System.Windows.Forms.TextBox()
		Me.Label8 = New System.Windows.Forms.Label()
		Me._subDirectoryNativePrefix = New System.Windows.Forms.TextBox()
		Me.Label13 = New System.Windows.Forms.Label()
		Me._subDirectoryMaxSize = New System.Windows.Forms.NumericUpDown()
		Me._subdirectoryStartNumber = New System.Windows.Forms.NumericUpDown()
		Me.Label9 = New System.Windows.Forms.Label()
		Me.Label10 = New System.Windows.Forms.Label()
		Me.Label11 = New System.Windows.Forms.Label()
		Me._subdirectoryImagePrefix = New System.Windows.Forms.TextBox()
		Me._volumeInformationGroupBox = New System.Windows.Forms.GroupBox()
		Me._volumeDigitPadding = New System.Windows.Forms.NumericUpDown()
		Me.Label22 = New System.Windows.Forms.Label()
		Me._volumeMaxSize = New System.Windows.Forms.NumericUpDown()
		Me._volumeStartNumber = New System.Windows.Forms.NumericUpDown()
		Me.Label14 = New System.Windows.Forms.Label()
		Me.Label15 = New System.Windows.Forms.Label()
		Me.Label16 = New System.Windows.Forms.Label()
		Me._volumePrefix = New System.Windows.Forms.TextBox()
		Me.GroupBox23 = New System.Windows.Forms.GroupBox()
		Me._prefixText = New System.Windows.Forms.TextBox()
		Me._usePrefix = New System.Windows.Forms.RadioButton()
		Me._useAbsolutePaths = New System.Windows.Forms.RadioButton()
		Me._useRelativePaths = New System.Windows.Forms.RadioButton()
		Me._loadFileCharacterInformation = New System.Windows.Forms.GroupBox()
		Me.Label7 = New System.Windows.Forms.Label()
		Me._nestedValueDelimiter = New System.Windows.Forms.ComboBox()
		Me._multiRecordDelimiter = New System.Windows.Forms.ComboBox()
		Me.Label6 = New System.Windows.Forms.Label()
		Me.Label4 = New System.Windows.Forms.Label()
		Me._quoteDelimiter = New System.Windows.Forms.ComboBox()
		Me.Label3 = New System.Windows.Forms.Label()
		Me._newLineDelimiter = New System.Windows.Forms.ComboBox()
		Me.Label2 = New System.Windows.Forms.Label()
		Me._recordDelimiter = New System.Windows.Forms.ComboBox()
		Me._saveExportSettingsDialog = New System.Windows.Forms.SaveFileDialog()
		Me._loadExportSettingsDialog = New System.Windows.Forms.OpenFileDialog()
		Me._productionPrecedenceBox.SuspendLayout()
		Me.GroupBox3.SuspendLayout()
		Me.TabControl1.SuspendLayout()
		Me._dataSourceTabPage.SuspendLayout()
		Me._filtersBox.SuspendLayout()
		CType(Me._startExportAtDocumentNumber, System.ComponentModel.ISupportInitialize).BeginInit()
		Me._destinationFileTabPage.SuspendLayout()
		Me.GroupBox6.SuspendLayout()
		Me._metadataGroup.SuspendLayout()
		Me.GroupBox4.SuspendLayout()
		Me.GroupBox2.SuspendLayout()
		Me.GroupBox1.SuspendLayout()
		Me._subDirectoryInformationGroupBox.SuspendLayout()
		CType(Me._subdirectoryDigitPadding, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me._subDirectoryMaxSize, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me._subdirectoryStartNumber, System.ComponentModel.ISupportInitialize).BeginInit()
		Me._volumeInformationGroupBox.SuspendLayout()
		CType(Me._volumeDigitPadding, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me._volumeMaxSize, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me._volumeStartNumber, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.GroupBox23.SuspendLayout()
		Me._loadFileCharacterInformation.SuspendLayout()
		Me.SuspendLayout()
		'
		'MainMenu1
		'
		Me.MainMenu1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.ExportMenu})
		'
		'ExportMenu
		'
		Me.ExportMenu.Index = 0
		Me.ExportMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.RunMenu, Me.SaveExportSettings, Me.LoadExportSettings, Me.MenuItem3, Me.RefreshMenu})
		Me.ExportMenu.Text = "File"
		'
		'RunMenu
		'
		Me.RunMenu.Index = 0
		Me.RunMenu.Text = "Run"
		'
		'SaveExportSettings
		'
		Me.SaveExportSettings.Index = 1
		Me.SaveExportSettings.Shortcut = System.Windows.Forms.Shortcut.CtrlS
		Me.SaveExportSettings.Text = "Save Export Settings"
		'
		'LoadExportSettings
		'
		Me.LoadExportSettings.Index = 2
		Me.LoadExportSettings.Shortcut = System.Windows.Forms.Shortcut.CtrlO
		Me.LoadExportSettings.Text = "Load Export Settings"
		'
		'MenuItem3
		'
		Me.MenuItem3.Index = 3
		Me.MenuItem3.Text = "-"
		'
		'RefreshMenu
		'
		Me.RefreshMenu.Index = 4
		Me.RefreshMenu.Shortcut = System.Windows.Forms.Shortcut.F5
		Me.RefreshMenu.Text = "Refresh"
		'
		'_productionPrecedenceBox
		'
		Me._productionPrecedenceBox.Controls.Add(Me._productionPrecedenceList)
		Me._productionPrecedenceBox.Controls.Add(Me._pickPrecedenceButton)
		Me._productionPrecedenceBox.Location = New System.Drawing.Point(568, 12)
		Me._productionPrecedenceBox.Name = "_productionPrecedenceBox"
		Me._productionPrecedenceBox.Size = New System.Drawing.Size(184, 356)
		Me._productionPrecedenceBox.TabIndex = 1
		Me._productionPrecedenceBox.TabStop = False
		Me._productionPrecedenceBox.Text = "Production Precedence"
		'
		'_productionPrecedenceList
		'
		Me._productionPrecedenceList.Location = New System.Drawing.Point(8, 17)
		Me._productionPrecedenceList.Name = "_productionPrecedenceList"
		Me._productionPrecedenceList.Size = New System.Drawing.Size(140, 329)
		Me._productionPrecedenceList.TabIndex = 0
		'
		'_pickPrecedenceButton
		'
		Me._pickPrecedenceButton.Location = New System.Drawing.Point(152, 328)
		Me._pickPrecedenceButton.Name = "_pickPrecedenceButton"
		Me._pickPrecedenceButton.Size = New System.Drawing.Size(24, 20)
		Me._pickPrecedenceButton.TabIndex = 1
		Me._pickPrecedenceButton.Text = "..."
		'
		'Label5
		'
		Me.Label5.Location = New System.Drawing.Point(20, 18)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New System.Drawing.Size(96, 21)
		Me.Label5.TabIndex = 0
		Me.Label5.Text = "Named after:"
		Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_overwriteCheckBox
		'
		Me._overwriteCheckBox.Location = New System.Drawing.Point(8, 48)
		Me._overwriteCheckBox.Name = "_overwriteCheckBox"
		Me._overwriteCheckBox.Size = New System.Drawing.Size(100, 16)
		Me._overwriteCheckBox.TabIndex = 2
		Me._overwriteCheckBox.Text = "Overwrite Files"
		'
		'_browseButton
		'
		Me._browseButton.Location = New System.Drawing.Point(388, 20)
		Me._browseButton.Name = "_browseButton"
		Me._browseButton.Size = New System.Drawing.Size(24, 20)
		Me._browseButton.TabIndex = 1
		Me._browseButton.Text = "..."
		'
		'_folderPath
		'
		Me._folderPath.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me._folderPath.Location = New System.Drawing.Point(8, 20)
		Me._folderPath.Name = "_folderPath"
		Me._folderPath.Size = New System.Drawing.Size(380, 20)
		Me._folderPath.TabIndex = 0
		Me._folderPath.Text = "Select a folder ..."
		'
		'_appendOriginalFilename
		'
		Me._appendOriginalFilename.Location = New System.Drawing.Point(12, 44)
		Me._appendOriginalFilename.Name = "_appendOriginalFilename"
		Me._appendOriginalFilename.Size = New System.Drawing.Size(148, 16)
		Me._appendOriginalFilename.TabIndex = 2
		Me._appendOriginalFilename.Text = "Append original filename"
		'
		'GroupBox3
		'
		Me.GroupBox3.Controls.Add(Me._overwriteCheckBox)
		Me.GroupBox3.Controls.Add(Me._browseButton)
		Me.GroupBox3.Controls.Add(Me._folderPath)
		Me.GroupBox3.Location = New System.Drawing.Point(8, 4)
		Me.GroupBox3.Name = "GroupBox3"
		Me.GroupBox3.Size = New System.Drawing.Size(424, 72)
		Me.GroupBox3.TabIndex = 0
		Me.GroupBox3.TabStop = False
		Me.GroupBox3.Text = "Export Location"
		'
		'_nativeFileNameSource
		'
		Me._nativeFileNameSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._nativeFileNameSource.Items.AddRange(New Object() {"Select...", "Identifier", "Begin production number"})
		Me._nativeFileNameSource.Location = New System.Drawing.Point(116, 18)
		Me._nativeFileNameSource.Name = "_nativeFileNameSource"
		Me._nativeFileNameSource.Size = New System.Drawing.Size(176, 21)
		Me._nativeFileNameSource.TabIndex = 1
		Me._nativeFileNameSource.Visible = False
		'
		'TabControl1
		'
		Me.TabControl1.Controls.Add(Me._dataSourceTabPage)
		Me.TabControl1.Controls.Add(Me._destinationFileTabPage)
		Me.TabControl1.Location = New System.Drawing.Point(0, 0)
		Me.TabControl1.Name = "TabControl1"
		Me.TabControl1.SelectedIndex = 0
		Me.TabControl1.Size = New System.Drawing.Size(776, 452)
		Me.TabControl1.TabIndex = 0
		'
		'_dataSourceTabPage
		'
		Me._dataSourceTabPage.Controls.Add(Me._filtersBox)
		Me._dataSourceTabPage.Controls.Add(Me._productionPrecedenceBox)
		Me._dataSourceTabPage.Location = New System.Drawing.Point(4, 22)
		Me._dataSourceTabPage.Name = "_dataSourceTabPage"
		Me._dataSourceTabPage.Size = New System.Drawing.Size(768, 426)
		Me._dataSourceTabPage.TabIndex = 0
		Me._dataSourceTabPage.Text = "Data Source"
		'
		'_filtersBox
		'
		Me._filtersBox.Controls.Add(Me._startExportAtDocumentNumber)
		Me._filtersBox.Controls.Add(Me.Label24)
		Me._filtersBox.Controls.Add(Me.Label18)
		Me._filtersBox.Controls.Add(Me._filters)
		Me._filtersBox.Controls.Add(Me._columnSelecter)
		Me._filtersBox.Location = New System.Drawing.Point(4, 12)
		Me._filtersBox.Name = "_filtersBox"
		Me._filtersBox.Size = New System.Drawing.Size(560, 356)
		Me._filtersBox.TabIndex = 0
		Me._filtersBox.TabStop = False
		Me._filtersBox.Text = "Export"
		'
		'_startExportAtDocumentNumber
		'
		Me._startExportAtDocumentNumber.Location = New System.Drawing.Point(404, 68)
		Me._startExportAtDocumentNumber.Maximum = New Decimal(New Integer() {10000000, 0, 0, 0})
		Me._startExportAtDocumentNumber.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
		Me._startExportAtDocumentNumber.Name = "_startExportAtDocumentNumber"
		Me._startExportAtDocumentNumber.Size = New System.Drawing.Size(148, 20)
		Me._startExportAtDocumentNumber.TabIndex = 4
		Me._startExportAtDocumentNumber.Value = New Decimal(New Integer() {1, 0, 0, 0})
		'
		'Label24
		'
		Me.Label24.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label24.Location = New System.Drawing.Point(392, 48)
		Me.Label24.Name = "Label24"
		Me.Label24.Size = New System.Drawing.Size(160, 16)
		Me.Label24.TabIndex = 3
		Me.Label24.Text = "Start Export at Record #"
		Me.Label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'Label18
		'
		Me.Label18.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label18.Location = New System.Drawing.Point(180, 48)
		Me.Label18.Name = "Label18"
		Me.Label18.Size = New System.Drawing.Size(160, 16)
		Me.Label18.TabIndex = 1
		Me.Label18.Text = "Selected Columns"
		Me.Label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_filters
		'
		Me._filters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._filters.ItemHeight = 13
		Me._filters.Location = New System.Drawing.Point(8, 20)
		Me._filters.Name = "_filters"
		Me._filters.Size = New System.Drawing.Size(544, 21)
		Me._filters.TabIndex = 0
		'
		'_columnSelecter
		'
		Me._columnSelecter.AlternateRowColors = True
		Me._columnSelecter.KeepButtonsCentered = False
		Me._columnSelecter.LeftOrderControlsVisible = False
		Me._columnSelecter.Location = New System.Drawing.Point(12, 64)
		Me._columnSelecter.Name = "_columnSelecter"
		Me._columnSelecter.OuterBox = kCura.Windows.Forms.ListBoxLocation.Left
		Me._columnSelecter.RightOrderControlVisible = True
		Me._columnSelecter.Size = New System.Drawing.Size(360, 280)
		Me._columnSelecter.TabIndex = 2
		'
		'_destinationFileTabPage
		'
		Me._destinationFileTabPage.Controls.Add(Me.GroupBox6)
		Me._destinationFileTabPage.Controls.Add(Me._metadataGroup)
		Me._destinationFileTabPage.Controls.Add(Me.GroupBox4)
		Me._destinationFileTabPage.Controls.Add(Me.GroupBox2)
		Me._destinationFileTabPage.Controls.Add(Me.GroupBox1)
		Me._destinationFileTabPage.Controls.Add(Me._subDirectoryInformationGroupBox)
		Me._destinationFileTabPage.Controls.Add(Me._volumeInformationGroupBox)
		Me._destinationFileTabPage.Controls.Add(Me.GroupBox23)
		Me._destinationFileTabPage.Controls.Add(Me._loadFileCharacterInformation)
		Me._destinationFileTabPage.Controls.Add(Me.GroupBox3)
		Me._destinationFileTabPage.Location = New System.Drawing.Point(4, 22)
		Me._destinationFileTabPage.Name = "_destinationFileTabPage"
		Me._destinationFileTabPage.Size = New System.Drawing.Size(768, 426)
		Me._destinationFileTabPage.TabIndex = 1
		Me._destinationFileTabPage.Text = "Destination Files"
		'
		'GroupBox6
		'
		Me.GroupBox6.Controls.Add(Me._appendOriginalFilename)
		Me.GroupBox6.Controls.Add(Me.Label5)
		Me.GroupBox6.Controls.Add(Me._nativeFileNameSource)
		Me.GroupBox6.Location = New System.Drawing.Point(436, 4)
		Me.GroupBox6.Name = "GroupBox6"
		Me.GroupBox6.Size = New System.Drawing.Size(324, 68)
		Me.GroupBox6.TabIndex = 6
		Me.GroupBox6.TabStop = False
		Me.GroupBox6.Text = "Text and Native File Names"
		'
		'_metadataGroup
		'
		Me._metadataGroup.Controls.Add(Me.Label21)
		Me._metadataGroup.Controls.Add(Me._textFieldPrecedencePicker)
		Me._metadataGroup.Controls.Add(Me._textFileEncoding)
		Me._metadataGroup.Controls.Add(Me.Label20)
		Me._metadataGroup.Controls.Add(Me.Label19)
		Me._metadataGroup.Controls.Add(Me._dataFileEncoding)
		Me._metadataGroup.Controls.Add(Me.Label1)
		Me._metadataGroup.Controls.Add(Me._nativeFileFormat)
		Me._metadataGroup.Controls.Add(Me._exportMulticodeFieldsAsNested)
		Me._metadataGroup.Controls.Add(Me._exportFullTextAsFile)
		Me._metadataGroup.Location = New System.Drawing.Point(436, 236)
		Me._metadataGroup.Name = "_metadataGroup"
		Me._metadataGroup.Size = New System.Drawing.Size(324, 184)
		Me._metadataGroup.TabIndex = 9
		Me._metadataGroup.TabStop = False
		Me._metadataGroup.Text = "Metadata"
		'
		'Label21
		'
		Me.Label21.Location = New System.Drawing.Point(8, 125)
		Me.Label21.Name = "Label21"
		Me.Label21.Size = New System.Drawing.Size(107, 21)
		Me.Label21.TabIndex = 7
		Me.Label21.Text = "Text Precedence:"
		Me.Label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_textFieldPrecedencePicker
		'
		Me._textFieldPrecedencePicker.Location = New System.Drawing.Point(116, 125)
		Me._textFieldPrecedencePicker.Name = "_textFieldPrecedencePicker"
		Me._textFieldPrecedencePicker.SelectedFields = CType(resources.GetObject("_textFieldPrecedencePicker.SelectedFields"), System.Collections.Generic.List(Of kCura.WinEDDS.ViewFieldInfo))
		Me._textFieldPrecedencePicker.Size = New System.Drawing.Size(175, 21)
		Me._textFieldPrecedencePicker.TabIndex = 8
		'
		'_textFileEncoding
		'
		Me._textFileEncoding.Location = New System.Drawing.Point(116, 100)
		Me._textFileEncoding.Name = "_textFileEncoding"
		Me._textFileEncoding.SelectedEncoding = Nothing
		Me._textFileEncoding.Size = New System.Drawing.Size(200, 21)
		Me._textFileEncoding.TabIndex = 6
		'
		'Label20
		'
		Me.Label20.Location = New System.Drawing.Point(12, 100)
		Me.Label20.Name = "Label20"
		Me.Label20.Size = New System.Drawing.Size(104, 21)
		Me.Label20.TabIndex = 5
		Me.Label20.Text = "Text File Encoding:"
		Me.Label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'Label19
		'
		Me.Label19.Location = New System.Drawing.Point(12, 48)
		Me.Label19.Name = "Label19"
		Me.Label19.Size = New System.Drawing.Size(104, 21)
		Me.Label19.TabIndex = 2
		Me.Label19.Text = "Data File Encoding:"
		Me.Label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_dataFileEncoding
		'
		Me._dataFileEncoding.Location = New System.Drawing.Point(116, 48)
		Me._dataFileEncoding.Name = "_dataFileEncoding"
		Me._dataFileEncoding.SelectedEncoding = Nothing
		Me._dataFileEncoding.Size = New System.Drawing.Size(200, 21)
		Me._dataFileEncoding.TabIndex = 3
		'
		'Label1
		'
		Me.Label1.Location = New System.Drawing.Point(24, 20)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(92, 21)
		Me.Label1.TabIndex = 0
		Me.Label1.Text = "Data File Format:"
		Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_nativeFileFormat
		'
		Me._nativeFileFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._nativeFileFormat.Items.AddRange(New Object() {"Select...", "Comma-separated (.csv)", "Concordance (.dat)", "Custom (.txt)", "HTML (.html)"})
		Me._nativeFileFormat.Location = New System.Drawing.Point(116, 20)
		Me._nativeFileFormat.Name = "_nativeFileFormat"
		Me._nativeFileFormat.Size = New System.Drawing.Size(176, 21)
		Me._nativeFileFormat.TabIndex = 1
		'
		'_exportMulticodeFieldsAsNested
		'
		Me._exportMulticodeFieldsAsNested.Location = New System.Drawing.Point(12, 156)
		Me._exportMulticodeFieldsAsNested.Name = "_exportMulticodeFieldsAsNested"
		Me._exportMulticodeFieldsAsNested.Size = New System.Drawing.Size(228, 20)
		Me._exportMulticodeFieldsAsNested.TabIndex = 9
		Me._exportMulticodeFieldsAsNested.Text = "Export Multiple Choice Fields as Nested"
		'
		'_exportFullTextAsFile
		'
		Me._exportFullTextAsFile.Location = New System.Drawing.Point(12, 76)
		Me._exportFullTextAsFile.Name = "_exportFullTextAsFile"
		Me._exportFullTextAsFile.Size = New System.Drawing.Size(196, 20)
		Me._exportFullTextAsFile.TabIndex = 4
		Me._exportFullTextAsFile.Text = "Export Text Field as Files"
		'
		'GroupBox4
		'
		Me.GroupBox4.Controls.Add(Me._exportNativeFiles)
		Me.GroupBox4.Location = New System.Drawing.Point(436, 184)
		Me.GroupBox4.Name = "GroupBox4"
		Me.GroupBox4.Size = New System.Drawing.Size(324, 48)
		Me.GroupBox4.TabIndex = 8
		Me.GroupBox4.TabStop = False
		Me.GroupBox4.Text = "Native "
		'
		'_exportNativeFiles
		'
		Me._exportNativeFiles.Checked = True
		Me._exportNativeFiles.CheckState = System.Windows.Forms.CheckState.Checked
		Me._exportNativeFiles.Location = New System.Drawing.Point(12, 20)
		Me._exportNativeFiles.Name = "_exportNativeFiles"
		Me._exportNativeFiles.Size = New System.Drawing.Size(300, 20)
		Me._exportNativeFiles.TabIndex = 0
		Me._exportNativeFiles.Text = "Export Native Files"
		'
		'GroupBox2
		'
		Me.GroupBox2.Controls.Add(Me.Label17)
		Me.GroupBox2.Controls.Add(Me._exportImages)
		Me.GroupBox2.Controls.Add(Me._imageFileFormat)
		Me.GroupBox2.Controls.Add(Me.Label12)
		Me.GroupBox2.Controls.Add(Me._imageTypeDropdown)
		Me.GroupBox2.Location = New System.Drawing.Point(436, 76)
		Me.GroupBox2.Name = "GroupBox2"
		Me.GroupBox2.Size = New System.Drawing.Size(324, 104)
		Me.GroupBox2.TabIndex = 7
		Me.GroupBox2.TabStop = False
		Me.GroupBox2.Text = "Image "
		'
		'Label17
		'
		Me.Label17.Location = New System.Drawing.Point(24, 76)
		Me.Label17.Name = "Label17"
		Me.Label17.Size = New System.Drawing.Size(92, 21)
		Me.Label17.TabIndex = 3
		Me.Label17.Text = "File Type:"
		Me.Label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_exportImages
		'
		Me._exportImages.Checked = True
		Me._exportImages.CheckState = System.Windows.Forms.CheckState.Checked
		Me._exportImages.Location = New System.Drawing.Point(12, 20)
		Me._exportImages.Name = "_exportImages"
		Me._exportImages.Size = New System.Drawing.Size(96, 20)
		Me._exportImages.TabIndex = 0
		Me._exportImages.Text = "Export Images"
		'
		'_imageFileFormat
		'
		Me._imageFileFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._imageFileFormat.DropDownWidth = 150
		Me._imageFileFormat.Location = New System.Drawing.Point(116, 48)
		Me._imageFileFormat.Name = "_imageFileFormat"
		Me._imageFileFormat.Size = New System.Drawing.Size(176, 21)
		Me._imageFileFormat.TabIndex = 2
		'
		'Label12
		'
		Me.Label12.Location = New System.Drawing.Point(24, 48)
		Me.Label12.Name = "Label12"
		Me.Label12.Size = New System.Drawing.Size(92, 21)
		Me.Label12.TabIndex = 1
		Me.Label12.Text = "Data File Format:"
		Me.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_imageTypeDropdown
		'
		Me._imageTypeDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._imageTypeDropdown.DropDownWidth = 150
		Me._imageTypeDropdown.Items.AddRange(New Object() {"Select...", "Single-page TIF/JPG", "Multi-page TIF", "PDF"})
		Me._imageTypeDropdown.Location = New System.Drawing.Point(116, 76)
		Me._imageTypeDropdown.Name = "_imageTypeDropdown"
		Me._imageTypeDropdown.Size = New System.Drawing.Size(176, 21)
		Me._imageTypeDropdown.TabIndex = 4
		'
		'GroupBox1
		'
		Me.GroupBox1.Controls.Add(Me._copyFilesFromRepository)
		Me.GroupBox1.Location = New System.Drawing.Point(8, 84)
		Me.GroupBox1.Name = "GroupBox1"
		Me.GroupBox1.Size = New System.Drawing.Size(212, 48)
		Me.GroupBox1.TabIndex = 1
		Me.GroupBox1.TabStop = False
		Me.GroupBox1.Text = "Physical File Export"
		'
		'_copyFilesFromRepository
		'
		Me._copyFilesFromRepository.Checked = True
		Me._copyFilesFromRepository.CheckState = System.Windows.Forms.CheckState.Checked
		Me._copyFilesFromRepository.Location = New System.Drawing.Point(12, 20)
		Me._copyFilesFromRepository.Name = "_copyFilesFromRepository"
		Me._copyFilesFromRepository.Size = New System.Drawing.Size(164, 16)
		Me._copyFilesFromRepository.TabIndex = 0
		Me._copyFilesFromRepository.Text = "Copy Files From Repository"
		'
		'_subDirectoryInformationGroupBox
		'
		Me._subDirectoryInformationGroupBox.Controls.Add(Me._subdirectoryDigitPadding)
		Me._subDirectoryInformationGroupBox.Controls.Add(Me.Label23)
		Me._subDirectoryInformationGroupBox.Controls.Add(Me._subdirectoryTextPrefix)
		Me._subDirectoryInformationGroupBox.Controls.Add(Me.Label8)
		Me._subDirectoryInformationGroupBox.Controls.Add(Me._subDirectoryNativePrefix)
		Me._subDirectoryInformationGroupBox.Controls.Add(Me.Label13)
		Me._subDirectoryInformationGroupBox.Controls.Add(Me._subDirectoryMaxSize)
		Me._subDirectoryInformationGroupBox.Controls.Add(Me._subdirectoryStartNumber)
		Me._subDirectoryInformationGroupBox.Controls.Add(Me.Label9)
		Me._subDirectoryInformationGroupBox.Controls.Add(Me.Label10)
		Me._subDirectoryInformationGroupBox.Controls.Add(Me.Label11)
		Me._subDirectoryInformationGroupBox.Controls.Add(Me._subdirectoryImagePrefix)
		Me._subDirectoryInformationGroupBox.Location = New System.Drawing.Point(8, 256)
		Me._subDirectoryInformationGroupBox.Name = "_subDirectoryInformationGroupBox"
		Me._subDirectoryInformationGroupBox.Size = New System.Drawing.Size(212, 164)
		Me._subDirectoryInformationGroupBox.TabIndex = 3
		Me._subDirectoryInformationGroupBox.TabStop = False
		Me._subDirectoryInformationGroupBox.Text = "Subdirectory Information"
		'
		'_subdirectoryDigitPadding
		'
		Me._subdirectoryDigitPadding.Location = New System.Drawing.Point(160, 104)
		Me._subdirectoryDigitPadding.Maximum = New Decimal(New Integer() {2000000, 0, 0, 0})
		Me._subdirectoryDigitPadding.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
		Me._subdirectoryDigitPadding.Name = "_subdirectoryDigitPadding"
		Me._subdirectoryDigitPadding.Size = New System.Drawing.Size(44, 20)
		Me._subdirectoryDigitPadding.TabIndex = 9
		Me._subdirectoryDigitPadding.Value = New Decimal(New Integer() {1, 0, 0, 0})
		'
		'Label23
		'
		Me.Label23.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.Label23.Location = New System.Drawing.Point(104, 104)
		Me.Label23.Name = "Label23"
		Me.Label23.Size = New System.Drawing.Size(56, 20)
		Me.Label23.TabIndex = 8
		Me.Label23.Text = "# of digits:"
		Me.Label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_subdirectoryTextPrefix
		'
		Me._subdirectoryTextPrefix.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me._subdirectoryTextPrefix.Location = New System.Drawing.Point(116, 76)
		Me._subdirectoryTextPrefix.Name = "_subdirectoryTextPrefix"
		Me._subdirectoryTextPrefix.Size = New System.Drawing.Size(88, 20)
		Me._subdirectoryTextPrefix.TabIndex = 5
		Me._subdirectoryTextPrefix.Text = "TEXT"
		'
		'Label8
		'
		Me.Label8.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.Label8.Location = New System.Drawing.Point(36, 76)
		Me.Label8.Name = "Label8"
		Me.Label8.Size = New System.Drawing.Size(80, 20)
		Me.Label8.TabIndex = 4
		Me.Label8.Text = "Text Prefix: "
		Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_subDirectoryNativePrefix
		'
		Me._subDirectoryNativePrefix.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me._subDirectoryNativePrefix.Location = New System.Drawing.Point(116, 48)
		Me._subDirectoryNativePrefix.Name = "_subDirectoryNativePrefix"
		Me._subDirectoryNativePrefix.Size = New System.Drawing.Size(88, 20)
		Me._subDirectoryNativePrefix.TabIndex = 3
		Me._subDirectoryNativePrefix.Text = "NATIVE"
		'
		'Label13
		'
		Me.Label13.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.Label13.Location = New System.Drawing.Point(44, 48)
		Me.Label13.Name = "Label13"
		Me.Label13.Size = New System.Drawing.Size(72, 20)
		Me.Label13.TabIndex = 2
		Me.Label13.Text = "Native Prefix: "
		Me.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_subDirectoryMaxSize
		'
		Me._subDirectoryMaxSize.Location = New System.Drawing.Point(116, 132)
		Me._subDirectoryMaxSize.Maximum = New Decimal(New Integer() {2000000, 0, 0, 0})
		Me._subDirectoryMaxSize.Name = "_subDirectoryMaxSize"
		Me._subDirectoryMaxSize.Size = New System.Drawing.Size(88, 20)
		Me._subDirectoryMaxSize.TabIndex = 11
		Me._subDirectoryMaxSize.Value = New Decimal(New Integer() {500, 0, 0, 0})
		'
		'_subdirectoryStartNumber
		'
		Me._subdirectoryStartNumber.Location = New System.Drawing.Point(56, 104)
		Me._subdirectoryStartNumber.Maximum = New Decimal(New Integer() {2000000, 0, 0, 0})
		Me._subdirectoryStartNumber.Name = "_subdirectoryStartNumber"
		Me._subdirectoryStartNumber.Size = New System.Drawing.Size(44, 20)
		Me._subdirectoryStartNumber.TabIndex = 7
		Me._subdirectoryStartNumber.Value = New Decimal(New Integer() {1, 0, 0, 0})
		'
		'Label9
		'
		Me.Label9.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.Label9.Location = New System.Drawing.Point(56, 132)
		Me.Label9.Name = "Label9"
		Me.Label9.Size = New System.Drawing.Size(60, 20)
		Me.Label9.TabIndex = 10
		Me.Label9.Text = "Max Files:"
		Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'Label10
		'
		Me.Label10.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.Label10.Location = New System.Drawing.Point(8, 104)
		Me.Label10.Name = "Label10"
		Me.Label10.Size = New System.Drawing.Size(44, 20)
		Me.Label10.TabIndex = 6
		Me.Label10.Text = "Start #:"
		Me.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'Label11
		'
		Me.Label11.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.Label11.Location = New System.Drawing.Point(44, 20)
		Me.Label11.Name = "Label11"
		Me.Label11.Size = New System.Drawing.Size(72, 20)
		Me.Label11.TabIndex = 0
		Me.Label11.Text = "Image Prefix: "
		Me.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_subdirectoryImagePrefix
		'
		Me._subdirectoryImagePrefix.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me._subdirectoryImagePrefix.Location = New System.Drawing.Point(116, 20)
		Me._subdirectoryImagePrefix.Name = "_subdirectoryImagePrefix"
		Me._subdirectoryImagePrefix.Size = New System.Drawing.Size(88, 20)
		Me._subdirectoryImagePrefix.TabIndex = 1
		Me._subdirectoryImagePrefix.Text = "IMG"
		'
		'_volumeInformationGroupBox
		'
		Me._volumeInformationGroupBox.Controls.Add(Me._volumeDigitPadding)
		Me._volumeInformationGroupBox.Controls.Add(Me.Label22)
		Me._volumeInformationGroupBox.Controls.Add(Me._volumeMaxSize)
		Me._volumeInformationGroupBox.Controls.Add(Me._volumeStartNumber)
		Me._volumeInformationGroupBox.Controls.Add(Me.Label14)
		Me._volumeInformationGroupBox.Controls.Add(Me.Label15)
		Me._volumeInformationGroupBox.Controls.Add(Me.Label16)
		Me._volumeInformationGroupBox.Controls.Add(Me._volumePrefix)
		Me._volumeInformationGroupBox.Location = New System.Drawing.Point(8, 140)
		Me._volumeInformationGroupBox.Name = "_volumeInformationGroupBox"
		Me._volumeInformationGroupBox.Size = New System.Drawing.Size(212, 108)
		Me._volumeInformationGroupBox.TabIndex = 2
		Me._volumeInformationGroupBox.TabStop = False
		Me._volumeInformationGroupBox.Text = "Volume Information"
		'
		'_volumeDigitPadding
		'
		Me._volumeDigitPadding.Location = New System.Drawing.Point(160, 48)
		Me._volumeDigitPadding.Maximum = New Decimal(New Integer() {2000000, 0, 0, 0})
		Me._volumeDigitPadding.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
		Me._volumeDigitPadding.Name = "_volumeDigitPadding"
		Me._volumeDigitPadding.Size = New System.Drawing.Size(44, 20)
		Me._volumeDigitPadding.TabIndex = 5
		Me._volumeDigitPadding.Value = New Decimal(New Integer() {1, 0, 0, 0})
		'
		'Label22
		'
		Me.Label22.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.Label22.Location = New System.Drawing.Point(104, 48)
		Me.Label22.Name = "Label22"
		Me.Label22.Size = New System.Drawing.Size(56, 20)
		Me.Label22.TabIndex = 4
		Me.Label22.Text = "# of digits:"
		Me.Label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_volumeMaxSize
		'
		Me._volumeMaxSize.Location = New System.Drawing.Point(116, 76)
		Me._volumeMaxSize.Maximum = New Decimal(New Integer() {2000000, 0, 0, 0})
		Me._volumeMaxSize.Name = "_volumeMaxSize"
		Me._volumeMaxSize.Size = New System.Drawing.Size(88, 20)
		Me._volumeMaxSize.TabIndex = 7
		Me._volumeMaxSize.Value = New Decimal(New Integer() {650, 0, 0, 0})
		'
		'_volumeStartNumber
		'
		Me._volumeStartNumber.Location = New System.Drawing.Point(52, 48)
		Me._volumeStartNumber.Maximum = New Decimal(New Integer() {2000000, 0, 0, 0})
		Me._volumeStartNumber.Name = "_volumeStartNumber"
		Me._volumeStartNumber.Size = New System.Drawing.Size(44, 20)
		Me._volumeStartNumber.TabIndex = 3
		Me._volumeStartNumber.Value = New Decimal(New Integer() {1, 0, 0, 0})
		'
		'Label14
		'
		Me.Label14.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.Label14.Location = New System.Drawing.Point(32, 76)
		Me.Label14.Name = "Label14"
		Me.Label14.Size = New System.Drawing.Size(82, 20)
		Me.Label14.TabIndex = 6
		Me.Label14.Text = "Max Size (MB):"
		Me.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'Label15
		'
		Me.Label15.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.Label15.Location = New System.Drawing.Point(8, 48)
		Me.Label15.Name = "Label15"
		Me.Label15.Size = New System.Drawing.Size(44, 20)
		Me.Label15.TabIndex = 2
		Me.Label15.Text = "Start #:"
		Me.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'Label16
		'
		Me.Label16.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.Label16.Location = New System.Drawing.Point(76, 20)
		Me.Label16.Name = "Label16"
		Me.Label16.Size = New System.Drawing.Size(40, 20)
		Me.Label16.TabIndex = 0
		Me.Label16.Text = "Prefix: "
		Me.Label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_volumePrefix
		'
		Me._volumePrefix.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me._volumePrefix.Location = New System.Drawing.Point(116, 20)
		Me._volumePrefix.Name = "_volumePrefix"
		Me._volumePrefix.Size = New System.Drawing.Size(88, 20)
		Me._volumePrefix.TabIndex = 1
		Me._volumePrefix.Text = "VOL"
		'
		'GroupBox23
		'
		Me.GroupBox23.Controls.Add(Me._prefixText)
		Me.GroupBox23.Controls.Add(Me._usePrefix)
		Me.GroupBox23.Controls.Add(Me._useAbsolutePaths)
		Me.GroupBox23.Controls.Add(Me._useRelativePaths)
		Me.GroupBox23.Location = New System.Drawing.Point(224, 84)
		Me.GroupBox23.Name = "GroupBox23"
		Me.GroupBox23.Size = New System.Drawing.Size(208, 120)
		Me.GroupBox23.TabIndex = 4
		Me.GroupBox23.TabStop = False
		Me.GroupBox23.Text = "File Path"
		'
		'_prefixText
		'
		Me._prefixText.Enabled = False
		Me._prefixText.Location = New System.Drawing.Point(88, 80)
		Me._prefixText.Name = "_prefixText"
		Me._prefixText.Size = New System.Drawing.Size(112, 20)
		Me._prefixText.TabIndex = 3
		'
		'_usePrefix
		'
		Me._usePrefix.Location = New System.Drawing.Point(12, 84)
		Me._usePrefix.Name = "_usePrefix"
		Me._usePrefix.Size = New System.Drawing.Size(76, 16)
		Me._usePrefix.TabIndex = 2
		Me._usePrefix.Text = "Use Prefix"
		'
		'_useAbsolutePaths
		'
		Me._useAbsolutePaths.Location = New System.Drawing.Point(12, 52)
		Me._useAbsolutePaths.Name = "_useAbsolutePaths"
		Me._useAbsolutePaths.Size = New System.Drawing.Size(124, 16)
		Me._useAbsolutePaths.TabIndex = 1
		Me._useAbsolutePaths.Text = "Use Absolute Paths"
		'
		'_useRelativePaths
		'
		Me._useRelativePaths.Checked = True
		Me._useRelativePaths.Location = New System.Drawing.Point(12, 20)
		Me._useRelativePaths.Name = "_useRelativePaths"
		Me._useRelativePaths.Size = New System.Drawing.Size(124, 16)
		Me._useRelativePaths.TabIndex = 0
		Me._useRelativePaths.TabStop = True
		Me._useRelativePaths.Text = "Use Relative Paths"
		'
		'_loadFileCharacterInformation
		'
		Me._loadFileCharacterInformation.Controls.Add(Me.Label7)
		Me._loadFileCharacterInformation.Controls.Add(Me._nestedValueDelimiter)
		Me._loadFileCharacterInformation.Controls.Add(Me._multiRecordDelimiter)
		Me._loadFileCharacterInformation.Controls.Add(Me.Label6)
		Me._loadFileCharacterInformation.Controls.Add(Me.Label4)
		Me._loadFileCharacterInformation.Controls.Add(Me._quoteDelimiter)
		Me._loadFileCharacterInformation.Controls.Add(Me.Label3)
		Me._loadFileCharacterInformation.Controls.Add(Me._newLineDelimiter)
		Me._loadFileCharacterInformation.Controls.Add(Me.Label2)
		Me._loadFileCharacterInformation.Controls.Add(Me._recordDelimiter)
		Me._loadFileCharacterInformation.Location = New System.Drawing.Point(224, 212)
		Me._loadFileCharacterInformation.Name = "_loadFileCharacterInformation"
		Me._loadFileCharacterInformation.Size = New System.Drawing.Size(208, 208)
		Me._loadFileCharacterInformation.TabIndex = 5
		Me._loadFileCharacterInformation.TabStop = False
		Me._loadFileCharacterInformation.Text = "Load File Characters"
		'
		'Label7
		'
		Me.Label7.Location = New System.Drawing.Point(8, 168)
		Me.Label7.Name = "Label7"
		Me.Label7.Size = New System.Drawing.Size(76, 21)
		Me.Label7.TabIndex = 8
		Me.Label7.Text = "Nested Value:"
		Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_nestedValueDelimiter
		'
		Me._nestedValueDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._nestedValueDelimiter.Enabled = False
		Me._nestedValueDelimiter.Location = New System.Drawing.Point(84, 168)
		Me._nestedValueDelimiter.Name = "_nestedValueDelimiter"
		Me._nestedValueDelimiter.Size = New System.Drawing.Size(116, 21)
		Me._nestedValueDelimiter.TabIndex = 9
		'
		'_multiRecordDelimiter
		'
		Me._multiRecordDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._multiRecordDelimiter.Enabled = False
		Me._multiRecordDelimiter.Location = New System.Drawing.Point(84, 132)
		Me._multiRecordDelimiter.Name = "_multiRecordDelimiter"
		Me._multiRecordDelimiter.Size = New System.Drawing.Size(116, 21)
		Me._multiRecordDelimiter.TabIndex = 7
		'
		'Label6
		'
		Me.Label6.Location = New System.Drawing.Point(8, 132)
		Me.Label6.Name = "Label6"
		Me.Label6.Size = New System.Drawing.Size(76, 21)
		Me.Label6.TabIndex = 6
		Me.Label6.Text = "Multi-Value:"
		Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'Label4
		'
		Me.Label4.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.Label4.Location = New System.Drawing.Point(8, 60)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New System.Drawing.Size(76, 21)
		Me.Label4.TabIndex = 2
		Me.Label4.Text = "Quote:"
		Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_quoteDelimiter
		'
		Me._quoteDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._quoteDelimiter.Enabled = False
		Me._quoteDelimiter.Location = New System.Drawing.Point(84, 60)
		Me._quoteDelimiter.Name = "_quoteDelimiter"
		Me._quoteDelimiter.Size = New System.Drawing.Size(116, 21)
		Me._quoteDelimiter.TabIndex = 3
		'
		'Label3
		'
		Me.Label3.Location = New System.Drawing.Point(8, 96)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New System.Drawing.Size(76, 21)
		Me.Label3.TabIndex = 4
		Me.Label3.Text = "Newline:"
		Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_newLineDelimiter
		'
		Me._newLineDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._newLineDelimiter.Enabled = False
		Me._newLineDelimiter.Location = New System.Drawing.Point(84, 96)
		Me._newLineDelimiter.Name = "_newLineDelimiter"
		Me._newLineDelimiter.Size = New System.Drawing.Size(116, 21)
		Me._newLineDelimiter.TabIndex = 5
		'
		'Label2
		'
		Me.Label2.Location = New System.Drawing.Point(8, 24)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(76, 21)
		Me.Label2.TabIndex = 0
		Me.Label2.Text = "Column:"
		Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_recordDelimiter
		'
		Me._recordDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._recordDelimiter.Enabled = False
		Me._recordDelimiter.Location = New System.Drawing.Point(84, 24)
		Me._recordDelimiter.Name = "_recordDelimiter"
		Me._recordDelimiter.Size = New System.Drawing.Size(116, 21)
		Me._recordDelimiter.TabIndex = 1
		'
		'_saveExportSettingsDialog
		'
		Me._saveExportSettingsDialog.DefaultExt = "kwx"
		Me._saveExportSettingsDialog.Filter = "Relativity Desktop Client settings files (*.kwx)|*.kwx|All files (*.*)|*.*"
		Me._saveExportSettingsDialog.RestoreDirectory = True
		'
		'_loadExportSettingsDialog
		'
		Me._loadExportSettingsDialog.DefaultExt = "kwx"
		Me._loadExportSettingsDialog.Filter = "Relativity Desktop Client settings files (*.kwx)|*.kwx|All files (*.*)|*.*"
		Me._loadExportSettingsDialog.RestoreDirectory = True
		'
		'ExportForm
		'
		Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
		Me.BackColor = System.Drawing.SystemColors.Control
		Me.ClientSize = New System.Drawing.Size(776, 453)
		Me.Controls.Add(Me.TabControl1)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
		Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
		Me.MaximizeBox = False
		Me.Menu = Me.MainMenu1
		Me.Name = "ExportForm"
		Me.Text = "Relativity Desktop Client | Export "
		Me._productionPrecedenceBox.ResumeLayout(False)
		Me.GroupBox3.ResumeLayout(False)
		Me.GroupBox3.PerformLayout()
		Me.TabControl1.ResumeLayout(False)
		Me._dataSourceTabPage.ResumeLayout(False)
		Me._filtersBox.ResumeLayout(False)
		CType(Me._startExportAtDocumentNumber, System.ComponentModel.ISupportInitialize).EndInit()
		Me._destinationFileTabPage.ResumeLayout(False)
		Me.GroupBox6.ResumeLayout(False)
		Me._metadataGroup.ResumeLayout(False)
		Me.GroupBox4.ResumeLayout(False)
		Me.GroupBox2.ResumeLayout(False)
		Me.GroupBox1.ResumeLayout(False)
		Me._subDirectoryInformationGroupBox.ResumeLayout(False)
		Me._subDirectoryInformationGroupBox.PerformLayout()
		CType(Me._subdirectoryDigitPadding, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me._subDirectoryMaxSize, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me._subdirectoryStartNumber, System.ComponentModel.ISupportInitialize).EndInit()
		Me._volumeInformationGroupBox.ResumeLayout(False)
		Me._volumeInformationGroupBox.PerformLayout()
		CType(Me._volumeDigitPadding, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me._volumeMaxSize, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me._volumeStartNumber, System.ComponentModel.ISupportInitialize).EndInit()
		Me.GroupBox23.ResumeLayout(False)
		Me.GroupBox23.PerformLayout()
		Me._loadFileCharacterInformation.ResumeLayout(False)
		Me.ResumeLayout(False)

	End Sub

#End Region

	Private WithEvents _application As kCura.EDDS.WinForm.Application
	Protected _exportFile As kCura.WinEDDS.ExportFile
	Protected WithEvents _precedenceForm As kCura.EDDS.WinForm.ProductionPrecedenceForm
	Protected WithEvents _textFieldPrecedenceForm As kCura.EDDS.WinForm.TextPrecedenceForm
	Private _allExportableFields As Relativity.ViewFieldInfo
	Private _dataSourceIsSet As Boolean = False
	Private _objectTypeName As String = ""
	Private _isLoadingExport As Boolean = False
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

	Public ReadOnly Property ObjectTypeName() As String
		Get
			If _objectTypeName = "" Then
				For Each row As System.Data.DataRow In New kCura.WinEDDS.Service.ObjectTypeManager(_application.Credential, _application.CookieContainer).RetrieveAllUploadable(_application.SelectedCaseInfo.ArtifactID).Tables(0).Rows
					If CType(row("DescriptorArtifactTypeID"), Int32) = Me.ExportFile.ArtifactTypeID Then
						_objectTypeName = row("Name").ToString
					End If
				Next
			End If
			Return _objectTypeName
		End Get
	End Property

	Private Sub AppendErrorMessage(ByVal msg As System.Text.StringBuilder, ByVal errorText As String)
		msg.Append(" - ").Append(errorText).Append(vbNewLine)
	End Sub

	Private Sub SaveExportSettings_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SaveExportSettings.Click
		Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
		Try
			If PopulateExportFile(Me, False) Then
				Select Case _saveExportSettingsDialog.ShowDialog()
					Case DialogResult.OK
						_application.SaveExportFile(_exportFile, _saveExportSettingsDialog.FileName)
				End Select
			End If
		Catch
			Throw
		Finally
			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Try
	End Sub

	Private Function IsValid(ByVal abstractExportForm As ExportForm) As Boolean
		Dim msg As New System.Text.StringBuilder
		Dim retval As Boolean = True
		If Not System.IO.Directory.Exists(_folderPath.Text) OrElse _folderPath.Text.Trim = String.Empty Then
			If _folderPath.Text.Trim = String.Empty Then
				AppendErrorMessage(msg, "Export destination folder empty")
			Else
				AppendErrorMessage(msg, "Export destination folder does not exist")
			End If
		End If
		If _filters.SelectedItem Is Nothing Then
			msg.AppendFormat("No {0} selected", Me.ExportTypeStringName.ToLower)
		End If
		If _exportNativeFiles.Checked OrElse _columnSelecter.RightListBoxItems.Count > 0 Then
			If CType(_nativeFileFormat.SelectedItem, String) = "Select..." Then
				AppendErrorMessage(msg, "No metadata data file format selected")
			End If
		End If
		If _exportImages.Checked Then
			If CType(_imageFileFormat.SelectedValue, Int32) = -1 Then
				AppendErrorMessage(msg, "No image data file format selected")
			End If
			If _imageTypeDropdown.SelectedIndex = 0 Then
				AppendErrorMessage(msg, "No image file type selected")
			End If
		End If
		If Me.ExportFile.TypeOfExport = ExportFile.ExportType.Production Then
			If _exportNativeFiles.Checked Then
				If CType(_nativeFileNameSource.SelectedItem, String) = "Select..." Then
					AppendErrorMessage(msg, "No file name source selected")
				End If
			End If
			Dim batesFound As Boolean = False
			Dim defaultSelectedIds As New System.Collections.ArrayList
			If _filters.SelectedItem IsNot Nothing Then defaultSelectedIds = DirectCast(Me.ExportFile.ArtifactAvfLookup(CType(_filters.SelectedValue, Int32)), ArrayList)

			For Each id As Integer In defaultSelectedIds
				batesFound = False
				For Each item As kCura.WinEDDS.ViewFieldInfo In _columnSelecter.RightListBoxItems
					If id = item.AvfId Then
						batesFound = True
						Exit For
					End If
				Next
				If Not batesFound Then Exit For
			Next

			If Not batesFound Then
				AppendErrorMessage(msg, "Bates fields are not selected columns")
			End If
		End If
		If _dataFileEncoding.SelectedEncoding Is Nothing Then
			AppendErrorMessage(msg, "No encoding selected for metadata file.")
		End If
		If _exportFullTextAsFile.Checked Then
			If _textFileEncoding.SelectedEncoding Is Nothing Then
				AppendErrorMessage(msg, "No encoding selected for text field files.")
			End If
			If _textFieldPrecedencePicker.SelectedFields IsNot Nothing Then
				If _textFieldPrecedencePicker.SelectedFields.Count = 0 Then
					AppendErrorMessage(msg, "When exporting text field as files is selected, you must set text precedence.")
				End If
			End If
		End If
		If Me.CreateVolume Then
			If Not _subdirectoryImagePrefix.Text.Trim <> "" Then AppendErrorMessage(msg, "Subdirectory Image Prefix cannot be blank.")
			If Not _subdirectoryTextPrefix.Text.Trim <> "" Then AppendErrorMessage(msg, "Subdirectory Text Prefix cannot be blank.")
			If CType(_subDirectoryMaxSize.Value, Int64) < 1 OrElse _subDirectoryMaxSize.Text.Trim = "" Then AppendErrorMessage(msg, "Subdirectory Max Size must be greater than zero.")
			If Not _subDirectoryNativePrefix.Text.Trim <> "" Then AppendErrorMessage(msg, "Subdirectory Native Prefix cannot be blank.")
			If CType(_subdirectoryStartNumber.Value, Int32) < 1 OrElse _subdirectoryStartNumber.Text.Trim = "" Then AppendErrorMessage(msg, "Subdirectory Start Number must be greater than zero.")
			If CType(_volumeMaxSize.Value, Int64) < 1 OrElse _volumeMaxSize.Text.Trim = "" Then AppendErrorMessage(msg, "Volume Max Size must be greater than zero.")
			If Not _volumePrefix.Text.Trim <> "" Then AppendErrorMessage(msg, "Volume Prefix cannot be blank.")
			If CType(_volumeStartNumber.Value, Int32) < 1 Then AppendErrorMessage(msg, "Volume Start Number must be greater than zero.")
		End If
		If msg.ToString.Trim <> String.Empty Then
			msg.Insert(0, "The following issues need to be addressed before continuing:" & vbNewLine & vbNewLine)
			MsgBox(msg.ToString, MsgBoxStyle.Exclamation, "Warning")
			retval = False
		End If
		Return retval
	End Function

	Public Function PopulateExportFile(ByVal abstractExportForm As ExportForm, ByVal validateForm As Boolean) As Boolean
		Dim d As DocumentFieldCollection = _application.CurrentFields(_exportFile.ArtifactTypeID, True)
		Dim retval As Boolean = True
		_exportFile.ObjectTypeName = _application.GetObjectTypeName(_exportFile.ArtifactTypeID)
		If validateForm AndAlso Not Me.IsValid(abstractExportForm) Then Return False
		If Not _application.IsConnected(_exportFile.CaseArtifactID, 10) Then Return False
		_exportFile.FolderPath = _folderPath.Text
		Select Case Me.ExportFile.TypeOfExport
			Case ExportFile.ExportType.AncestorSearch
				_exportFile.ViewID = CType(_filters.SelectedValue, Int32)
				_exportFile.LoadFilesPrefix = DirectCast(_filters.SelectedItem, System.Data.DataRowView)(_filters.DisplayMember).ToString
			Case ExportFile.ExportType.ArtifactSearch
				_exportFile.ArtifactID = CType(_filters.SelectedValue, Int32)
				If Not _application.IsAssociatedSearchProviderAccessible(_exportFile.CaseArtifactID, _exportFile.ArtifactID) Then
					Me.Cursor = System.Windows.Forms.Cursors.Default
					Return Nothing
				End If
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
		_exportFile.MulticodesAsNested = _exportMulticodeFieldsAsNested.Checked
		_exportFile.Overwrite = _overwriteCheckBox.Checked
		'_exportFile.ExportFullText = _exportFullText.Checked
		_exportFile.ExportFullTextAsFile = _exportFullTextAsFile.Checked
		_exportFile.ExportNative = _exportNativeFiles.Checked
		_exportFile.QuoteDelimiter = ChrW(CType(_quoteDelimiter.SelectedValue, Int32))
		_exportFile.RecordDelimiter = ChrW(CType(_recordDelimiter.SelectedValue, Int32))
		_exportFile.MultiRecordDelimiter = ChrW(CType(_multiRecordDelimiter.SelectedValue, Int32))
		_exportFile.NewlineDelimiter = ChrW(CType(_newLineDelimiter.SelectedValue, Int32))
		_exportFile.NestedValueDelimiter = ChrW(CType(_nestedValueDelimiter.SelectedValue, Int32))
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
		'TODO: WINFLEX - ArtifactTypeID
		_exportFile.IdentifierColumnName = d.IdentifierFieldNames(0)
		_exportFile.RenameFilesToIdentifier = True
		_exportFile.VolumeInfo = Me.BuildVolumeInfo
		_exportFile.ExportImages = _exportImages.Checked
		_exportFile.LogFileFormat = CType(_imageFileFormat.SelectedValue, kCura.WinEDDS.LoadFileType.FileFormat)
		_exportFile.LoadFileIsHtml = _nativeFileFormat.SelectedIndex = 4
		If _exportFile.LoadFileIsHtml Then
			_exportFile.LoadFileExtension = "html"
		Else
			_exportFile.LoadFileExtension = Me.GetNativeFileFormatExtension()
		End If
		_exportFile.ImagePrecedence = Me.GetImagePrecedence
		_exportFile.TypeOfImage = Me.GetSelectedImageType
		Dim selectedViewFields As New System.Collections.ArrayList
		For Each field As ViewFieldInfo In _columnSelecter.RightListBoxItems
			selectedViewFields.Add(field)
		Next
		_exportFile.SelectedViewFields = DirectCast(selectedViewFields.ToArray(GetType(ViewFieldInfo)), ViewFieldInfo())
		If Me.GetRightColumnTextFields.Count > 0 AndAlso _textFieldPrecedencePicker.SelectedFields.Count > 0 Then
			_exportFile.SelectedTextFields = _textFieldPrecedencePicker.SelectedFields.ToArray()
			_exportFile.ExportFullText = True
		Else
			_exportFile.SelectedTextFields = Nothing
			_exportFile.ExportFullText = False
		End If
		_exportFile.LoadFileEncoding = _dataFileEncoding.SelectedEncoding
		_exportFile.TextFileEncoding = _textFileEncoding.SelectedEncoding
		_exportFile.VolumeDigitPadding = CType(_volumeDigitPadding.Value, Int32)
		_exportFile.SubdirectoryDigitPadding = CType(_subdirectoryDigitPadding.Value, Int32)
		_exportFile.StartAtDocumentNumber = CType(_startExportAtDocumentNumber.Value, Int32) - 1
		Return True
	End Function


	Private Sub LoadExportSettings_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LoadExportSettings.Click
		If _loadExportSettingsDialog.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
			Dim settings As String = kCura.Utility.File.Instance.ReadFileAsString(_loadExportSettingsDialog.FileName)
			Dim newFile As ExportFile = New kCura.WinEDDS.ExportFileSerializer().DeserializeExportFile(_exportFile, settings)
			If TypeOf newFile Is kCura.WinEDDS.ErrorExportFile Then
				MsgBox(DirectCast(newFile, kCura.WinEDDS.ErrorExportFile).ErrorMessage, MsgBoxStyle.Exclamation)
			Else
				Dim exportFilterSelectionForm As New kCura.EDDS.WinForm.ExportFilterSelectForm(newFile.LoadFilesPrefix, ExportTypeStringName, DirectCast(_filters.DataSource, DataTable))
				exportFilterSelectionForm.ShowDialog()
				If exportFilterSelectionForm.DialogResult = DialogResult.OK Then
					If exportFilterSelectionForm.SelectedItemArtifactIDs IsNot Nothing Then
						_filters.SelectedValue = exportFilterSelectionForm.SelectedItemArtifactIDs(0)
					End If
					LoadExportFile(newFile)
					_exportFile = newFile
				End If

				_columnSelecter.EnsureHorizontalScrollbars()
			End If
		End If
	End Sub

	Public Sub LoadExportFile(ByVal ef As kCura.WinEDDS.ExportFile)
		_isLoadingExport = True
		If _exportNativeFiles.Checked <> ef.ExportNative Then _exportNativeFiles.Checked = ef.ExportNative
		If _exportImages.Checked <> ef.ExportImages Then _exportImages.Checked = ef.ExportImages
		If _overwriteCheckBox.Checked <> ef.Overwrite Then _overwriteCheckBox.Checked = ef.Overwrite
		If _folderPath.Text <> ef.FolderPath Then _folderPath.Text = ef.FolderPath
		If ef.VolumeDigitPadding >= _volumeDigitPadding.Minimum AndAlso ef.VolumeDigitPadding <= _volumeDigitPadding.Maximum Then
			If _volumeDigitPadding.Value <> ef.VolumeDigitPadding Then _volumeDigitPadding.Value = ef.VolumeDigitPadding
		End If
		If ef.VolumeInfo IsNot Nothing Then
			If _copyFilesFromRepository.Checked <> ef.VolumeInfo.CopyFilesFromRepository Then _copyFilesFromRepository.Checked = ef.VolumeInfo.CopyFilesFromRepository
			If Not _volumePrefix.Text.Equals(ef.VolumeInfo.VolumePrefix, StringComparison.InvariantCultureIgnoreCase) Then _volumePrefix.Text = ef.VolumeInfo.VolumePrefix
			If _volumeStartNumber.Value <> ef.VolumeInfo.VolumeStartNumber Then _volumeStartNumber.Value = ef.VolumeInfo.VolumeStartNumber
			If _volumeMaxSize.Value <> ef.VolumeInfo.VolumeMaxSize Then _volumeMaxSize.Value = ef.VolumeInfo.VolumeMaxSize
			If Not _subdirectoryImagePrefix.Text.Equals(ef.VolumeInfo.SubdirectoryImagePrefix) Then _subdirectoryImagePrefix.Text = ef.VolumeInfo.SubdirectoryImagePrefix(False)
			If Not _subDirectoryNativePrefix.Text.Equals(ef.VolumeInfo.SubdirectoryNativePrefix) Then _subDirectoryNativePrefix.Text = ef.VolumeInfo.SubdirectoryNativePrefix(False)
			If Not _subdirectoryTextPrefix.Text.Equals(ef.VolumeInfo.SubdirectoryFullTextPrefix) Then _subdirectoryTextPrefix.Text = ef.VolumeInfo.SubdirectoryFullTextPrefix(False)
			If _subdirectoryStartNumber.Value <> ef.VolumeInfo.SubdirectoryStartNumber Then _subdirectoryStartNumber.Value = ef.VolumeInfo.SubdirectoryStartNumber
			If _subDirectoryMaxSize.Value <> ef.VolumeInfo.SubdirectoryMaxSize Then _subDirectoryMaxSize.Value = ef.VolumeInfo.SubdirectoryMaxSize
		End If
		If ef.SubdirectoryDigitPadding >= _subdirectoryDigitPadding.Minimum AndAlso ef.SubdirectoryDigitPadding <= _subdirectoryDigitPadding.Maximum Then
			If _subdirectoryDigitPadding.Value <> ef.SubdirectoryDigitPadding Then _subdirectoryDigitPadding.Value = ef.SubdirectoryDigitPadding
		End If
		Select Case ef.TypeOfExportedFilePath
			Case kCura.WinEDDS.ExportFile.ExportedFilePathType.Absolute
				_useAbsolutePaths.Checked = True
			Case kCura.WinEDDS.ExportFile.ExportedFilePathType.Prefix
				_usePrefix.Checked = True
				_prefixText.Text = ef.FilePrefix
			Case kCura.WinEDDS.ExportFile.ExportedFilePathType.Relative
				_useRelativePaths.Checked = True
		End Select
		_recordDelimiter.SelectedValue = ef.RecordDelimiter
		If _appendOriginalFilename.Checked <> ef.AppendOriginalFileName Then _appendOriginalFilename.Checked = ef.AppendOriginalFileName

		Select Case ef.ExportNativesToFileNamedFrom
			Case kCura.WinEDDS.ExportNativeWithFilenameFrom.Identifier
				_nativeFileNameSource.SelectedItem = "Identifier"
			Case kCura.WinEDDS.ExportNativeWithFilenameFrom.Production
				_nativeFileNameSource.SelectedItem = "Begin production number"
			Case kCura.WinEDDS.ExportNativeWithFilenameFrom.Select
				_nativeFileNameSource.SelectedItem = "Select..."
		End Select

		If ef.LoadFileIsHtml Then
			_nativeFileFormat.SelectedItem = "HTML (.html)"
		Else
			Select Case ef.LoadFileExtension
				Case "csv"
					_nativeFileFormat.SelectedItem = "Comma-separated (.csv)"
				Case "dat"
					_nativeFileFormat.SelectedItem = "Concordance (.dat)"
				Case "txt"
					_nativeFileFormat.SelectedItem = "Custom (.txt)"
				Case Else
					_nativeFileFormat.SelectedIndex = 0
			End Select
		End If

		If ef.ExportImages Then
			If ef.LogFileFormat.HasValue Then
				_imageFileFormat.SelectedValue = ef.LogFileFormat.Value
			Else
				_imageFileFormat.SelectedIndex = 0
			End If
			Me.SetSelectedImageType(ef.TypeOfImage)
		End If
		_dataFileEncoding.InitializeDropdown()
		_dataFileEncoding.SelectedEncoding = ef.LoadFileEncoding
		_textFileEncoding.InitializeDropdown()
		_textFileEncoding.SelectedEncoding = ef.TextFileEncoding
		_recordDelimiter.SelectedValue = ef.RecordDelimiter
		_quoteDelimiter.SelectedValue = ef.QuoteDelimiter
		_newLineDelimiter.SelectedValue = ef.NewlineDelimiter
		_nestedValueDelimiter.SelectedValue = ef.NestedValueDelimiter
		_multiRecordDelimiter.SelectedValue = ef.MultiRecordDelimiter
		_exportFullTextAsFile.Checked = ef.ExportFullTextAsFile



		_exportMulticodeFieldsAsNested.Checked = ef.MulticodesAsNested

		If ef.AllExportableFields IsNot Nothing Then
			_columnSelecter.ClearSelection(kCura.Windows.Forms.ListBoxLocation.Left)
			_columnSelecter.LeftListBoxItems.Clear()
			Array.Sort(ef.AllExportableFields)
			_columnSelecter.LeftListBoxItems.AddRange(ef.AllExportableFields)
		End If

		If ef.SelectedViewFields IsNot Nothing Then



			Dim itemsToRemoveFromLeftListBox As New System.Collections.Generic.List(Of kCura.WinEDDS.ViewFieldInfo)()
			_columnSelecter.ClearSelection(kCura.Windows.Forms.ListBoxLocation.Right)
			_columnSelecter.RightListBoxItems.Clear()
			For Each viewFieldFromKwx As kCura.WinEDDS.ViewFieldInfo In ef.SelectedViewFields
				For Each leftListBoxViewField As kCura.WinEDDS.ViewFieldInfo In _columnSelecter.LeftListBoxItems
					If leftListBoxViewField.DisplayName.Equals(viewFieldFromKwx.DisplayName, StringComparison.InvariantCulture) Then
						itemsToRemoveFromLeftListBox.Add(leftListBoxViewField)
						_columnSelecter.RightListBoxItems.Add(leftListBoxViewField)
					End If
				Next
			Next
			
			If ef.AllExportableFields IsNot Nothing Then
				Dim defaultSelectedIds As New System.Collections.ArrayList
				If Not _filters.SelectedItem Is Nothing Then defaultSelectedIds = DirectCast(Me.ExportFile.ArtifactAvfLookup(CType(_filters.SelectedValue, Int32)), ArrayList)

				For Each defaultSelectedId As Int32 In defaultSelectedIds
					For Each field As ViewFieldInfo In ef.AllExportableFields
						If field.AvfId = defaultSelectedId Then
							Dim avfNumber = field.AvfId
							Dim found As Boolean = ef.SelectedViewFields.Any(Function(addedItem) avfNumber = addedItem.AvfId)
							If Not found Then
								If Me.ExportFile.ArtifactTypeID = Relativity.ArtifactType.Document Then
									_columnSelecter.RightListBoxItems.Add(New ViewFieldInfo(field))
									Exit For
								ElseIf field.FieldType <> Relativity.FieldTypeHelper.FieldType.File Then
									_columnSelecter.RightListBoxItems.Add(New ViewFieldInfo(field))
									Exit For
								End If
							End If
						End If
					Next
				Next
			End If

			If _columnSelecter.RightListBoxItems.Count = 0 Then
				_metadataGroup.Enabled = False
			End If

			For Each vfi As kCura.WinEDDS.ViewFieldInfo In itemsToRemoveFromLeftListBox
				_columnSelecter.LeftListBoxItems.Remove(vfi)
			Next
		End If

		ManagePotentialTextFields()

		_textFieldPrecedencePicker.LoadNewSelectedFields(ef.SelectedTextFields)

		Dim trueStartExportAtDocumentNumber As Int32 = ef.StartAtDocumentNumber + 1
		If trueStartExportAtDocumentNumber >= _startExportAtDocumentNumber.Minimum AndAlso trueStartExportAtDocumentNumber <= _startExportAtDocumentNumber.Maximum Then
			_startExportAtDocumentNumber.Value = trueStartExportAtDocumentNumber
		End If

		If ef.ImagePrecedence IsNot Nothing AndAlso ef.ImagePrecedence.Length > 0 Then
			Dim validPrecedenceTable As System.Data.DataTable = _application.GetProductionPrecendenceList(ef.CaseInfo)
			Dim validPrecedencePairs As New System.Collections.Generic.List(Of kCura.WinEDDS.Pair)()
			For i As Int32 = 0 To validPrecedenceTable.Rows.Count - 1
				validPrecedencePairs.Add(New kCura.WinEDDS.Pair(validPrecedenceTable.Rows.Item(i)("Value").ToString, validPrecedenceTable.Rows.Item(i)("Display").ToString))
			Next
			_productionPrecedenceList.Items.Clear()

			For Each precedencePair As kCura.WinEDDS.Pair In ef.ImagePrecedence
				For Each item As kCura.WinEDDS.Pair In validPrecedencePairs
					If precedencePair.Display.Equals(item.Display, StringComparison.InvariantCulture) Then
						_productionPrecedenceList.Items.Add(item)
						Exit For
					End If
				Next
				If precedencePair.Display = "Original" AndAlso precedencePair.Value = "-1" Then _productionPrecedenceList.Items.Add(precedencePair)
			Next
			If _productionPrecedenceList.Items.Count = 0 Then _productionPrecedenceList.Items.Add(New Pair("-1", "Original"))
		Else
			'original is already there
		End If

		_isLoadingExport = False


	End Sub

	Private Function FindArtifactIDByName(ByVal dropDown As ComboBox, ByVal name As String) As Int32?
		Dim retVal As Int32?
		For i As Int32 = 0 To dropDown.Items.Count - 1
			Dim dropDownRow As DataRow = DirectCast(dropDown.Items(i), System.Data.DataRowView).Row
			If CStr(dropDownRow("Name")).Equals(name, StringComparison.InvariantCulture) Then
				retVal = CInt(dropDownRow("ArtifactID"))
				Exit For
			End If
		Next
		Return retVal
	End Function

	Private Sub RunMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RunMenu.Click
		Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
		Try
			If Me.PopulateExportFile(Me, True) Then _application.StartSearch(Me.ExportFile)
		Catch
			Throw
		Finally
			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Try
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
			Return String.Empty
		End If
	End Function

	Private Function GetSelectedImageType() As ExportFile.ImageType
		'Select...
		'Single-page TIF/JPG
		'Multi-page TIF
		'PDF
		If Not _copyFilesFromRepository.Checked Then Return ExportFile.ImageType.SinglePage
		Select Case _imageTypeDropdown.SelectedIndex
			Case 0
				Return kCura.WinEDDS.ExportFile.ImageType.Select
			Case 1
				Return ExportFile.ImageType.SinglePage
			Case 2
				Return ExportFile.ImageType.MultiPageTiff
			Case 3
				Return ExportFile.ImageType.Pdf
		End Select
		Return Nothing
	End Function

	Private Sub SetSelectedImageType(ByVal imageType As ExportFile.ImageType?)
		If Not imageType.HasValue OrElse imageType.Value = kCura.WinEDDS.ExportFile.ImageType.Select Then
			_imageTypeDropdown.SelectedIndex = 0
		ElseIf imageType.Value = kCura.WinEDDS.ExportFile.ImageType.SinglePage Then
			_imageTypeDropdown.SelectedIndex = 1
		ElseIf imageType.Value = kCura.WinEDDS.ExportFile.ImageType.MultiPageTiff Then
			_imageTypeDropdown.SelectedIndex = 2
		ElseIf imageType.Value = kCura.WinEDDS.ExportFile.ImageType.Pdf Then
			_imageTypeDropdown.SelectedIndex = 3
		Else
			Throw New ArgumentException("Unsupported image type: " & imageType.Value.ToString)
		End If
	End Sub

	Private Function GetDatasourceToolTip() As String
		Select Case ExportFile.TypeOfExport
			Case ExportFile.ExportType.ArtifactSearch
				Return "This is a list of all accessible saved searches in the system"
			Case ExportFile.ExportType.AncestorSearch, ExportFile.ExportType.ParentSearch
				Return "This is a list of all accessible views in the system"
			Case ExportFile.ExportType.Production
				Return "This is a list of all accessible productions marked as ""Produced"", which are not currently in the process of being populated or processed"
		End Select
		Return Nothing
	End Function

	Private Sub _browseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _browseButton.Click
		_destinationFolderDialog.ShowDialog()
		_folderPath.Text = _destinationFolderDialog.SelectedPath()
	End Sub

	Private Function BuildVolumeInfo() As Exporters.VolumeInfo
		Dim retval As New Exporters.VolumeInfo
		retval.SubdirectoryMaxSize = Int32.Parse(_subDirectoryMaxSize.Text)
		retval.SubdirectoryImagePrefix = _subdirectoryImagePrefix.Text
		retval.SubdirectoryNativePrefix = _subDirectoryNativePrefix.Text
		retval.SubdirectoryFullTextPrefix = _subdirectoryTextPrefix.Text
		retval.SubdirectoryStartNumber = Int32.Parse(_subdirectoryStartNumber.Text)
		retval.VolumeMaxSize = Int32.Parse(_volumeMaxSize.Text)
		retval.VolumePrefix = _volumePrefix.Text
		retval.VolumeStartNumber = Int32.Parse(_volumeStartNumber.Text)
		retval.CopyFilesFromRepository = _copyFilesFromRepository.Checked
		Return retval
	End Function

	Private Sub ExportProduction_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
		HandleLoad(sender, e, kCura.EDDS.WinForm.Config.ExportVolumeDigitPadding, kCura.EDDS.WinForm.Config.ExportSubdirectoryDigitPadding)
		_columnSelecter.EnsureHorizontalScrollbars()
	End Sub

	Public Sub HandleLoad(ByVal sender As Object, ByVal e As System.EventArgs, ByVal volumeDigitPadding As Int32, ByVal exportSubdirectoryDigitPadding As Int32)
		_dataSourceIsSet = False
		_filters.DataSource = ExportFile.DataTable
		_filters.DisplayMember = "Name"
		_filters.ValueMember = "ArtifactID"
		_dataSourceIsSet = True

		kCura.EDDS.WinForm.Utility.InitializeCharacterDropDown(_recordDelimiter, _exportFile.RecordDelimiter)
		kCura.EDDS.WinForm.Utility.InitializeCharacterDropDown(_quoteDelimiter, _exportFile.QuoteDelimiter)
		kCura.EDDS.WinForm.Utility.InitializeCharacterDropDown(_newLineDelimiter, _exportFile.NewlineDelimiter)
		kCura.EDDS.WinForm.Utility.InitializeCharacterDropDown(_multiRecordDelimiter, _exportFile.MultiRecordDelimiter)
		kCura.EDDS.WinForm.Utility.InitializeCharacterDropDown(_nestedValueDelimiter, _exportFile.NestedValueDelimiter)
		_volumeDigitPadding.Value = volumeDigitPadding
		_subdirectoryDigitPadding.Value = exportSubdirectoryDigitPadding
		_imageFileFormat.DataSource = kCura.WinEDDS.LoadFileType.GetLoadFileTypes
		_imageFileFormat.DisplayMember = "DisplayName"
		_imageFileFormat.ValueMember = "Value"
		_imageTypeDropdown.SelectedIndex = 0
		_exportMulticodeFieldsAsNested.Checked = Me.ExportFile.MulticodesAsNested
		Label5.Visible = False
		Select Case Me.ExportFile.TypeOfExport
			Case ExportFile.ExportType.ArtifactSearch
				_filters.Text = "Searches"
				_filtersBox.Text = "Searches"
				Me.Text = "Relativity Desktop Client | Export Saved Search"
			Case ExportFile.ExportType.ParentSearch, ExportFile.ExportType.AncestorSearch
				_filters.Text = "Views"
				_filtersBox.Text = "Views"
				If Me.ExportFile.ArtifactTypeID = Relativity.ArtifactType.Document Then
					Me.Text = "Relativity Desktop Client | Export Folder"
					If Me.ExportFile.TypeOfExport = ExportFile.ExportType.AncestorSearch Then
						Me.Text = "Relativity Desktop Client | Export Folder and Subfolders"
					End If
				Else
					Me.Text = String.Format("Relativity Desktop Client | Export {0} Objects", Me.ObjectTypeName)
				End If
			Case ExportFile.ExportType.Production
				Label5.Visible = True
				_filters.Text = "Productions"
				_filtersBox.Text = "Productions"
				_nativeFileNameSource.Visible = True
				_nativeFileNameSource.SelectedIndex = 0
				Me.Text = "Relativity Desktop Client | Export Production Set"
				_productionPrecedenceBox.Visible = False
		End Select
		If Not Me.ExportFile.ArtifactTypeID = Relativity.ArtifactType.Document Then
			_productionPrecedenceBox.Visible = False
			Label13.Text = "File Prefix"
			GroupBox6.Text = "Text and File Names"
			Label11.Visible = False
			_subdirectoryImagePrefix.Visible = False
		End If

		Dim filtersToolTip As New ToolTip
		filtersToolTip.AutoPopDelay = 5000
		filtersToolTip.InitialDelay = 1000
		filtersToolTip.ReshowDelay = 500
		filtersToolTip.ShowAlways = True
		filtersToolTip.SetToolTip(_filters, Me.GetDatasourceToolTip)
		_nativeFileFormat.SelectedIndex = 0
		_productionPrecedenceList.Items.Add(New Pair("-1", "Original"))
		Me.InitializeColumnSelecter()
		Me.InitializeFileControls()

	End Sub

	Private Sub _searchList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _filters.SelectedIndexChanged
		If Not _isLoadingExport AndAlso _dataSourceIsSet AndAlso Not _filters.SelectedItem Is Nothing Then Me.InitializeColumnSelecter()
		If _textFieldPrecedencePicker IsNot Nothing Then
			Dim textFields As List(Of ViewFieldInfo) = GetAllLongTextFields()
			textFields.Sort()
			_textFieldPrecedencePicker.AllAvailableLongTextFields = textFields
			_textFieldPrecedencePicker.SelectDefaultTextField(Nothing)
		End If
	End Sub

	Private Sub InitializeFileControls()
		If Me.ExportFile.ArtifactTypeID = Relativity.ArtifactType.Document Then Exit Sub
		_exportImages.Checked = False
		_exportImages.Enabled = False
		GroupBox2.Visible = False
		If Me.ExportFile.HasFileField Then
			GroupBox4.Text = Me.ExportFile.FileField.FieldName
			_exportNativeFiles.Text = String.Format("Export {0} Files", Me.ExportFile.FileField.FieldName)
		Else
			_exportNativeFiles.Checked = False
			_exportNativeFiles.Enabled = False
			GroupBox4.Visible = False
		End If
	End Sub

	Private Sub InitializeColumnSelecter()
		_columnSelecter.ClearAll()
		Dim defaultSelectedIds As New System.Collections.ArrayList
		If Not _filters.SelectedItem Is Nothing Then defaultSelectedIds = DirectCast(Me.ExportFile.ArtifactAvfLookup(CType(_filters.SelectedValue, Int32)), ArrayList)
		Dim leftListBoxItems As New System.Collections.ArrayList
		For Each field As ViewFieldInfo In Me.ExportFile.AllExportableFields
			If Not defaultSelectedIds.Contains(field.AvfId) Then
				If Me.ExportFile.ArtifactTypeID = Relativity.ArtifactType.Document Then
					leftListBoxItems.Add(New ViewFieldInfo(field))
				ElseIf field.FieldType <> Relativity.FieldTypeHelper.FieldType.File Then
					leftListBoxItems.Add(New ViewFieldInfo(field))
				End If
			End If
		Next
		For Each defaultSelectedId As Int32 In defaultSelectedIds
			For Each field As ViewFieldInfo In Me.ExportFile.AllExportableFields
				If field.AvfId = defaultSelectedId Then
					If Me.ExportFile.ArtifactTypeID = Relativity.ArtifactType.Document Then
						_columnSelecter.RightListBoxItems.Add(New ViewFieldInfo(field))
						Exit For
					ElseIf field.FieldType <> Relativity.FieldTypeHelper.FieldType.File Then
						_columnSelecter.RightListBoxItems.Add(New ViewFieldInfo(field))
						Exit For
					End If
				End If
			Next
		Next
		leftListBoxItems.Sort()
		_columnSelecter.LeftListBoxItems.AddRange(leftListBoxItems.ToArray())
		Me.ManagePotentialTextFields()
	End Sub

	Private Sub _exportImages_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _exportImages.CheckedChanged
		_useAbsolutePaths.Enabled = True
		_imageFileFormat.Enabled = _exportImages.Checked
		_imageTypeDropdown.Enabled = _exportImages.Checked And _copyFilesFromRepository.Checked
	End Sub

	Private Sub _exportNativeFiles_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _exportNativeFiles.CheckedChanged
		_useAbsolutePaths.Enabled = True
		_metadataGroup.Enabled = _columnSelecter.RightListBoxItems.Count > 0 OrElse _exportNativeFiles.Checked
		_nativeFileFormat.Enabled = True

	End Sub

	Private Sub ToggleLoadFileCharacterInformation(ByVal enabled As Boolean)
		_newLineDelimiter.Enabled = enabled
		_recordDelimiter.Enabled = enabled
		_quoteDelimiter.Enabled = enabled
		_multiRecordDelimiter.Enabled = enabled
		_nestedValueDelimiter.Enabled = enabled
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
				_nestedValueDelimiter.SelectedValue = "\"c
				'Case "Tab-delimited (.txt)"
				'	Me.ToggleLoadFileCharacterInformation(False)
				'	_recordDelimiter.SelectedValue = ChrW(9)
				'	_quoteDelimiter.SelectedValue = ChrW(34)
				'	_newLineDelimiter.SelectedValue = ChrW(10)
				'	_multiRecordDelimiter.SelectedValue = ChrW(59)
				'	_nestedValueDelimiter.SelectedValue = "\"c
			Case "Concordance (.dat)"
				Me.ToggleLoadFileCharacterInformation(False)
				_recordDelimiter.SelectedValue = ChrW(20)
				_quoteDelimiter.SelectedValue = ChrW(254)
				_newLineDelimiter.SelectedValue = ChrW(174)
				_multiRecordDelimiter.SelectedValue = ChrW(59)
				_nestedValueDelimiter.SelectedValue = "\"c
			Case "Custom (.txt)"
				Me.ToggleLoadFileCharacterInformation(True)
			Case Else
				Me.ToggleLoadFileCharacterInformation(False)
		End Select
	End Sub

	Private Sub _pickPrecedenceButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _pickPrecedenceButton.Click
		Dim dt As System.Data.DataTable = _application.GetProductionPrecendenceList(ExportFile.CaseInfo)
		If dt Is Nothing Then Exit Sub
		_precedenceForm = New kCura.EDDS.WinForm.ProductionPrecedenceForm
		_precedenceForm.ExportFile = Me.ExportFile
		_precedenceForm.PrecedenceTable = dt
		If _productionPrecedenceList.Items.Count > 0 Then
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

	Private Sub _usePrefix_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _usePrefix.CheckedChanged
		If _usePrefix.Checked Then
			_prefixText.Enabled = True
		Else
			_prefixText.Enabled = False
		End If
	End Sub

	Private Sub _exportFullText_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
		'If _exportFullText.Checked Then
		_exportFullTextAsFile.Enabled = True
		'Else
		'	_exportFullTextAsFile.Enabled = False
		'End If
	End Sub

	Private Sub _folderPath_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles _folderPath.Enter
		_destinationFolderDialog.ShowDialog()
		_folderPath.Text = _destinationFolderDialog.SelectedPath()
	End Sub

	Private Sub _volumeMaxSize_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles _volumeMaxSize.LostFocus
		If _volumeMaxSize.Text.Trim = "" Then _volumeMaxSize.Text = "0"
	End Sub

	Private Sub _volumeStartNumber_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles _volumeStartNumber.LostFocus
		If _volumeStartNumber.Text.Trim = "" Then _volumeStartNumber.Text = "0"
	End Sub

	Private Sub _subdirectoryMaxSize_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles _subDirectoryMaxSize.LostFocus
		If _subDirectoryMaxSize.Text.Trim = "" Then _subDirectoryMaxSize.Text = "0"
	End Sub

	Private Sub _subdirectoryStartNumber_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles _subdirectoryStartNumber.LostFocus
		If _subdirectoryStartNumber.Text.Trim = "" Then _subdirectoryStartNumber.Text = "0"
	End Sub

	Private Sub _volumePrefix_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _volumePrefix.TextChanged
		_volumePrefix.Text = Me.CleanPath(_volumePrefix.Text)
		_volumePrefix.SelectionStart = _volumePrefix.Text.Length
	End Sub

	Private Sub _subdirectoryImagePrefix_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _subdirectoryImagePrefix.TextChanged
		_subdirectoryImagePrefix.Text = Me.CleanPath(_subdirectoryImagePrefix.Text)
		_subdirectoryImagePrefix.SelectionStart = _subdirectoryImagePrefix.Text.Length
	End Sub

	Private Sub _subDirectoryNativePrefix_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _subDirectoryNativePrefix.TextChanged
		_subDirectoryNativePrefix.Text = Me.CleanPath(_subDirectoryNativePrefix.Text)
		_subDirectoryNativePrefix.SelectionStart = _subDirectoryNativePrefix.Text.Length
	End Sub

	Private Sub _subDirectoryTextPrefix_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _subdirectoryTextPrefix.TextChanged
		_subdirectoryTextPrefix.Text = Me.CleanPath(_subdirectoryTextPrefix.Text)
		_subdirectoryTextPrefix.SelectionStart = _subdirectoryTextPrefix.Text.Length
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


	Private Sub _columnSelecter_ItemsShifted() Handles _columnSelecter.ItemsShifted
		_metadataGroup.Enabled = _columnSelecter.RightListBoxItems.Count > 0 OrElse _exportNativeFiles.Checked
		Me.ManagePotentialTextFields()
	End Sub

	Private Sub ManagePotentialTextFields()
		Dim textFields As List(Of ViewFieldInfo) = GetAllLongTextFields()
		textFields.Sort()
		_textFieldPrecedencePicker.AllAvailableLongTextFields = textFields
	End Sub

	Private Function GetAllLongTextFields() As List(Of ViewFieldInfo)
		Dim textFields As New List(Of ViewFieldInfo)
		textFields.AddRange(Me.GetRightColumnTextFields())
		textFields.AddRange(Me.GetLeftColumnTextFields())
		Return textFields
	End Function

	Private Function GetRightColumnTextFields() As List(Of ViewFieldInfo)
		Return GetTextFields(_columnSelecter.RightListBoxItems.Cast(Of ViewFieldInfo)().ToList())
	End Function

	Private Function GetLeftColumnTextFields() As List(Of ViewFieldInfo)
		Return GetTextFields(_columnSelecter.LeftListBoxItems.Cast(Of ViewFieldInfo)().ToList())
	End Function

	Private Function GetTextFields(ByVal unfilteredList As List(Of ViewFieldInfo)) As List(Of ViewFieldInfo)
		Return (From field In unfilteredList Where field.FieldType = Relativity.FieldTypeHelper.FieldType.Text Select field).ToList()
	End Function

	Private Sub RefreshMenu_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles RefreshMenu.Click
		Me.RefreshRelativityInformation()
	End Sub

	Private Sub RefreshRelativityInformation()
		Dim selectedColumns As New System.Collections.ArrayList
		For Each field As kCura.WinEDDS.ViewFieldInfo In _columnSelecter.RightListBoxItems
			selectedColumns.Add(New kCura.WinEDDS.ViewFieldInfo(field))
		Next
		Dim selectedDataSource As Int32 = CInt(_filters.SelectedValue)
		_dataSourceIsSet = False
		Dim newExportFile As kCura.WinEDDS.ExportFile = _application.GetNewExportFileSettingsObject(_exportFile.ArtifactID, _exportFile.CaseInfo, _exportFile.TypeOfExport, _exportFile.ArtifactTypeID)
		If newExportFile.DataTable.Rows.Count = 0 Then
			Dim s As New System.Text.StringBuilder
			s.Append("There are no exportable ")
			Select Case newExportFile.TypeOfExport
				Case ExportFile.ExportType.Production
					s.Append("productions ")
				Case ExportFile.ExportType.ArtifactSearch
					s.Append("saved searches ")
				Case Else
					s.Append("views ")
			End Select
			s.Append("in this case")
			MsgBox(s.ToString, MsgBoxStyle.Critical, "Relativity Desktop Client")
			Me.Close()
			Exit Sub
		End If
		_exportFile.DataTable = newExportFile.DataTable
		_exportFile.AllExportableFields = newExportFile.AllExportableFields
		_exportFile.ArtifactAvfLookup = newExportFile.ArtifactAvfLookup
		_filters.DataSource = ExportFile.DataTable
		_filters.DisplayMember = "Name"
		_filters.ValueMember = "ArtifactID"
		Dim selectedindex As Int32 = -1
		For i As Int32 = 0 To _exportFile.DataTable.Rows.Count - 1
			If ExportFile.DataTable.Rows(i)("ArtifactID").ToString = selectedDataSource.ToString Then
				selectedindex = i
				Exit For
			End If
		Next
		If selectedindex = -1 Then
			selectedindex = 0
			Dim msg As String = "Selected "
			msg &= ExportTypeStringName().ToLower
			msg &= " is no longer available."
			MsgBox(msg, MsgBoxStyle.Exclamation, "Relativity Desktop Client")
		End If
		_dataSourceIsSet = True
		_filters.SelectedIndex = selectedindex
		Dim allLongTextFields As List(Of ViewFieldInfo) = GetAllLongTextFields()
		allLongTextFields.Sort()
		If allLongTextFields.Count > 0 Then
			_textFieldPrecedencePicker.AllAvailableLongTextFields = allLongTextFields
		End If
		'TODO: this will send -1 index to OnDraw during refresh on exports. Known defect. In backlog
		_columnSelecter.LeftListBoxItems.Clear()
		_columnSelecter.RightListBoxItems.Clear()
		Dim al As New System.Collections.ArrayList(_exportFile.AllExportableFields)
		al.Sort()
		_columnSelecter.LeftListBoxItems.AddRange(al.ToArray())
		For Each field As ViewFieldInfo In selectedColumns
			Dim itemToShiftIndex As Int32 = -1
			For i As Int32 = 0 To _columnSelecter.LeftListBoxItems.Count - 1
				Dim item As ViewFieldInfo = DirectCast(_columnSelecter.LeftListBoxItems(i), ViewFieldInfo)
				If item.AvfId = field.AvfId Then
					itemToShiftIndex = i
					Exit For
				End If
			Next
			If itemToShiftIndex >= 0 Then
				Dim item As ViewFieldInfo = DirectCast(_columnSelecter.LeftListBoxItems(itemToShiftIndex), ViewFieldInfo)
				_columnSelecter.LeftListBoxItems.Remove(item)
				_columnSelecter.RightListBoxItems.Add(item)
			End If
		Next
	End Sub

	Private ReadOnly Property ExportTypeStringName() As String
		Get
			Select Case Me.ExportFile.TypeOfExport
				Case ExportFile.ExportType.AncestorSearch, ExportFile.ExportType.ParentSearch
					Return "View"
				Case ExportFile.ExportType.ArtifactSearch
					Return "Saved Search"
				Case ExportFile.ExportType.Production
					Return "Production"
			End Select
			Return ""
		End Get
	End Property

	Private Sub _copyFilesFromRepository_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _copyFilesFromRepository.CheckedChanged
		_imageTypeDropdown.Enabled = _exportImages.Checked And _copyFilesFromRepository.Checked
		If Not _copyFilesFromRepository.Checked Then
			_imageTypeDropdown.SelectedIndex = 1
		End If
	End Sub



End Class