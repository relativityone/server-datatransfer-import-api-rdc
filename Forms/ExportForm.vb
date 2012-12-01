Imports System.Linq
Imports System.Collections.Generic
Imports kCura.Windows.Forms

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
	Public WithEvents LabelNamedAfter As System.Windows.Forms.Label
	Public WithEvents _overwriteCheckBox As System.Windows.Forms.CheckBox
	Public WithEvents _browseButton As System.Windows.Forms.Button
	Public WithEvents _folderPath As System.Windows.Forms.TextBox
	Public WithEvents _appendOriginalFilenameCheckbox As System.Windows.Forms.CheckBox
	Public WithEvents GroupBoxExportLocation As System.Windows.Forms.GroupBox
	Public WithEvents _nativeFileNameSourceCombo As System.Windows.Forms.ComboBox
	Public WithEvents TabControl1 As System.Windows.Forms.TabControl
	Public WithEvents _dataSourceTabPage As System.Windows.Forms.TabPage
	Public WithEvents _destinationFileTabPage As System.Windows.Forms.TabPage
	Public WithEvents _groupBoxLoadFileCharacterInformation As System.Windows.Forms.GroupBox
	Public WithEvents _multiRecordDelimiter As System.Windows.Forms.ComboBox
	Public WithEvents LabelMultiValue As System.Windows.Forms.Label
	Public WithEvents LabelQuote As System.Windows.Forms.Label
	Public WithEvents _quoteDelimiter As System.Windows.Forms.ComboBox
	Public WithEvents LabelNewline As System.Windows.Forms.Label
	Public WithEvents _newLineDelimiter As System.Windows.Forms.ComboBox
	Public WithEvents LabelColumn As System.Windows.Forms.Label
	Public WithEvents _recordDelimiter As System.Windows.Forms.ComboBox
	Public WithEvents GroupBoxFilePath As System.Windows.Forms.GroupBox
	Public WithEvents _imageTypeDropdown As System.Windows.Forms.ComboBox
	Public WithEvents _prefixText As System.Windows.Forms.TextBox
	Public WithEvents _usePrefix As System.Windows.Forms.RadioButton
	Public WithEvents _useAbsolutePaths As System.Windows.Forms.RadioButton
	Public WithEvents _useRelativePaths As System.Windows.Forms.RadioButton
	Public WithEvents _imageFileFormat As System.Windows.Forms.ComboBox
	Public WithEvents GroupBoxPhysicalFileExport As System.Windows.Forms.GroupBox
	Public WithEvents _copyFilesFromRepository As System.Windows.Forms.CheckBox
	Public WithEvents _subDirectoryInformationGroupBox As System.Windows.Forms.GroupBox
	Public WithEvents _subdirectoryTextPrefix As System.Windows.Forms.TextBox
	Public WithEvents LabelTextPrefix As System.Windows.Forms.Label
	Public WithEvents _subDirectoryNativePrefix As System.Windows.Forms.TextBox
	Public WithEvents LabelNativePrefix As System.Windows.Forms.Label
	Public WithEvents _subDirectoryMaxSize As System.Windows.Forms.NumericUpDown
	Public WithEvents _subdirectoryStartNumber As System.Windows.Forms.NumericUpDown
	Public WithEvents LabelMaxFiles As System.Windows.Forms.Label
	Public WithEvents LabelSubdirectoryStartNumber As System.Windows.Forms.Label
	Public WithEvents LabelImagePrefix As System.Windows.Forms.Label
	Public WithEvents _subdirectoryImagePrefix As System.Windows.Forms.TextBox
	Public WithEvents _volumeInformationGroupBox As System.Windows.Forms.GroupBox
	Public WithEvents _volumeMaxSize As System.Windows.Forms.NumericUpDown
	Public WithEvents _volumeStartNumber As System.Windows.Forms.NumericUpDown
	Public WithEvents LabelMaxSizeMB As System.Windows.Forms.Label
	Public WithEvents LabelVolumeStartNumber As System.Windows.Forms.Label
	Public WithEvents LabelPrefix As System.Windows.Forms.Label
	Public WithEvents _volumePrefix As System.Windows.Forms.TextBox
	Public WithEvents LabelImageDataFileFormat As System.Windows.Forms.Label
	Public WithEvents LabelNestedValue As System.Windows.Forms.Label
	Public WithEvents GroupBoxImage As System.Windows.Forms.GroupBox
	Public WithEvents _exportImages As System.Windows.Forms.CheckBox
	Public WithEvents LabelFileType As System.Windows.Forms.Label
	Public WithEvents GroupBoxNative As System.Windows.Forms.GroupBox
	Public WithEvents _exportNativeFiles As System.Windows.Forms.CheckBox
	Public WithEvents _exportFullTextAsFile As System.Windows.Forms.CheckBox
	Public WithEvents GroupBoxTextAndNativeFileNames As System.Windows.Forms.GroupBox
	Public WithEvents _exportMulticodeFieldsAsNested As System.Windows.Forms.CheckBox
	Public WithEvents _nestedValueDelimiter As System.Windows.Forms.ComboBox
	Public WithEvents LabelSelectedColumns As System.Windows.Forms.Label
	Public WithEvents _filters As System.Windows.Forms.ComboBox
	Public WithEvents _columnSelector As kCura.Windows.Forms.TwoListBox
	Public WithEvents _filtersBox As System.Windows.Forms.GroupBox
	Public WithEvents _metadataGroupBox As System.Windows.Forms.GroupBox
	Public WithEvents LabelMetadataDataFileFormat As System.Windows.Forms.Label
	Public WithEvents _nativeFileFormat As System.Windows.Forms.ComboBox
	Public WithEvents _dataFileEncoding As kCura.EDDS.WinForm.EncodingPicker
	Public WithEvents LabelDataFileEncoding As System.Windows.Forms.Label
	Public WithEvents LabelTextFileEncoding As System.Windows.Forms.Label
	Public WithEvents _textFileEncoding As kCura.EDDS.WinForm.EncodingPicker
	Public WithEvents _textFieldPrecedencePicker As kCura.EDDS.WinForm.TextFieldPrecedencePicker
	Public WithEvents LabelTextPrecedence As System.Windows.Forms.Label
	Public WithEvents RefreshMenu As System.Windows.Forms.MenuItem
	Public WithEvents MenuItem3 As System.Windows.Forms.MenuItem
	Public WithEvents SaveExportSettings As System.Windows.Forms.MenuItem
	Public WithEvents LoadExportSettings As System.Windows.Forms.MenuItem
	Public WithEvents _volumeDigitPadding As System.Windows.Forms.NumericUpDown
	Public WithEvents LabelVolumeNumberOfDigits As System.Windows.Forms.Label
	Public WithEvents _subdirectoryDigitPadding As System.Windows.Forms.NumericUpDown
	Public WithEvents LabelSubdirectoryNumberOfDigits As System.Windows.Forms.Label
	Public WithEvents LabelStartAtRecordNumber As System.Windows.Forms.Label
	Public WithEvents _startExportAtDocumentNumber As System.Windows.Forms.NumericUpDown
	Public WithEvents _saveExportSettingsDialog As System.Windows.Forms.SaveFileDialog
	Public WithEvents _loadExportSettingsDialog As System.Windows.Forms.OpenFileDialog

	Private Sub InitializeComponent()
		Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(ExportForm))
		Me.MainMenu1 = New System.Windows.Forms.MainMenu
		Me.ExportMenu = New System.Windows.Forms.MenuItem
		Me.RunMenu = New System.Windows.Forms.MenuItem
		Me.SaveExportSettings = New System.Windows.Forms.MenuItem
		Me.LoadExportSettings = New System.Windows.Forms.MenuItem
		Me.MenuItem3 = New System.Windows.Forms.MenuItem
		Me.RefreshMenu = New System.Windows.Forms.MenuItem
		Me._destinationFolderDialog = New System.Windows.Forms.FolderBrowserDialog
		Me._productionPrecedenceBox = New System.Windows.Forms.GroupBox
		Me._productionPrecedenceList = New System.Windows.Forms.ListBox
		Me._pickPrecedenceButton = New System.Windows.Forms.Button
		Me.LabelNamedAfter = New System.Windows.Forms.Label
		Me._overwriteCheckBox = New System.Windows.Forms.CheckBox
		Me._browseButton = New System.Windows.Forms.Button
		Me._folderPath = New System.Windows.Forms.TextBox
		Me._appendOriginalFilenameCheckbox = New System.Windows.Forms.CheckBox
		Me.GroupBoxExportLocation = New System.Windows.Forms.GroupBox
		Me._nativeFileNameSourceCombo = New System.Windows.Forms.ComboBox
		Me.TabControl1 = New System.Windows.Forms.TabControl
		Me._dataSourceTabPage = New System.Windows.Forms.TabPage
		Me._filtersBox = New System.Windows.Forms.GroupBox
		Me._startExportAtDocumentNumber = New System.Windows.Forms.NumericUpDown
		Me.LabelStartAtRecordNumber = New System.Windows.Forms.Label
		Me.LabelSelectedColumns = New System.Windows.Forms.Label
		Me._filters = New System.Windows.Forms.ComboBox
		Me._columnSelector = New kCura.Windows.Forms.TwoListBox
		Me._destinationFileTabPage = New System.Windows.Forms.TabPage
		Me.GroupBoxTextAndNativeFileNames = New System.Windows.Forms.GroupBox
		Me._metadataGroupBox = New System.Windows.Forms.GroupBox
		Me.LabelTextPrecedence = New System.Windows.Forms.Label
		Me._textFileEncoding = New kCura.EDDS.WinForm.EncodingPicker
		Me._textFieldPrecedencePicker = New kCura.EDDS.WinForm.TextFieldPrecedencePicker
		Me.LabelTextFileEncoding = New System.Windows.Forms.Label
		Me.LabelDataFileEncoding = New System.Windows.Forms.Label
		Me._dataFileEncoding = New kCura.EDDS.WinForm.EncodingPicker
		Me.LabelMetadataDataFileFormat = New System.Windows.Forms.Label
		Me._nativeFileFormat = New System.Windows.Forms.ComboBox
		Me._exportMulticodeFieldsAsNested = New System.Windows.Forms.CheckBox
		Me._exportFullTextAsFile = New System.Windows.Forms.CheckBox
		Me.GroupBoxNative = New System.Windows.Forms.GroupBox
		Me._exportNativeFiles = New System.Windows.Forms.CheckBox
		Me.GroupBoxImage = New System.Windows.Forms.GroupBox
		Me.LabelFileType = New System.Windows.Forms.Label
		Me._exportImages = New System.Windows.Forms.CheckBox
		Me._imageFileFormat = New System.Windows.Forms.ComboBox
		Me.LabelImageDataFileFormat = New System.Windows.Forms.Label
		Me._imageTypeDropdown = New System.Windows.Forms.ComboBox
		Me.GroupBoxPhysicalFileExport = New System.Windows.Forms.GroupBox
		Me._copyFilesFromRepository = New System.Windows.Forms.CheckBox
		Me._subDirectoryInformationGroupBox = New System.Windows.Forms.GroupBox
		Me._subdirectoryDigitPadding = New System.Windows.Forms.NumericUpDown
		Me.LabelSubdirectoryNumberOfDigits = New System.Windows.Forms.Label
		Me._subdirectoryTextPrefix = New System.Windows.Forms.TextBox
		Me.LabelTextPrefix = New System.Windows.Forms.Label
		Me._subDirectoryNativePrefix = New System.Windows.Forms.TextBox
		Me.LabelNativePrefix = New System.Windows.Forms.Label
		Me._subDirectoryMaxSize = New System.Windows.Forms.NumericUpDown
		Me._subdirectoryStartNumber = New System.Windows.Forms.NumericUpDown
		Me.LabelMaxFiles = New System.Windows.Forms.Label
		Me.LabelSubdirectoryStartNumber = New System.Windows.Forms.Label
		Me.LabelImagePrefix = New System.Windows.Forms.Label
		Me._subdirectoryImagePrefix = New System.Windows.Forms.TextBox
		Me._volumeInformationGroupBox = New System.Windows.Forms.GroupBox
		Me._volumeDigitPadding = New System.Windows.Forms.NumericUpDown
		Me.LabelVolumeNumberOfDigits = New System.Windows.Forms.Label
		Me._volumeMaxSize = New System.Windows.Forms.NumericUpDown
		Me._volumeStartNumber = New System.Windows.Forms.NumericUpDown
		Me.LabelMaxSizeMB = New System.Windows.Forms.Label
		Me.LabelVolumeStartNumber = New System.Windows.Forms.Label
		Me.LabelPrefix = New System.Windows.Forms.Label
		Me._volumePrefix = New System.Windows.Forms.TextBox
		Me.GroupBoxFilePath = New System.Windows.Forms.GroupBox
		Me._prefixText = New System.Windows.Forms.TextBox
		Me._usePrefix = New System.Windows.Forms.RadioButton
		Me._useAbsolutePaths = New System.Windows.Forms.RadioButton
		Me._useRelativePaths = New System.Windows.Forms.RadioButton
		Me._groupBoxLoadFileCharacterInformation = New System.Windows.Forms.GroupBox
		Me.LabelNestedValue = New System.Windows.Forms.Label
		Me._nestedValueDelimiter = New System.Windows.Forms.ComboBox
		Me._multiRecordDelimiter = New System.Windows.Forms.ComboBox
		Me.LabelMultiValue = New System.Windows.Forms.Label
		Me.LabelQuote = New System.Windows.Forms.Label
		Me._quoteDelimiter = New System.Windows.Forms.ComboBox
		Me.LabelNewline = New System.Windows.Forms.Label
		Me._newLineDelimiter = New System.Windows.Forms.ComboBox
		Me.LabelColumn = New System.Windows.Forms.Label
		Me._recordDelimiter = New System.Windows.Forms.ComboBox
		Me._saveExportSettingsDialog = New System.Windows.Forms.SaveFileDialog
		Me._loadExportSettingsDialog = New System.Windows.Forms.OpenFileDialog
		Me._productionPrecedenceBox.SuspendLayout()
		Me.GroupBoxExportLocation.SuspendLayout()
		Me.TabControl1.SuspendLayout()
		Me._dataSourceTabPage.SuspendLayout()
		Me._filtersBox.SuspendLayout()
		CType(Me._startExportAtDocumentNumber, System.ComponentModel.ISupportInitialize).BeginInit()
		Me._destinationFileTabPage.SuspendLayout()
		Me.GroupBoxTextAndNativeFileNames.SuspendLayout()
		Me._metadataGroupBox.SuspendLayout()
		Me.GroupBoxNative.SuspendLayout()
		Me.GroupBoxImage.SuspendLayout()
		Me.GroupBoxPhysicalFileExport.SuspendLayout()
		Me._subDirectoryInformationGroupBox.SuspendLayout()
		CType(Me._subdirectoryDigitPadding, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me._subDirectoryMaxSize, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me._subdirectoryStartNumber, System.ComponentModel.ISupportInitialize).BeginInit()
		Me._volumeInformationGroupBox.SuspendLayout()
		CType(Me._volumeDigitPadding, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me._volumeMaxSize, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me._volumeStartNumber, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.GroupBoxFilePath.SuspendLayout()
		Me._groupBoxLoadFileCharacterInformation.SuspendLayout()
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
		'LoadExportSettings
		'
		Me.LoadExportSettings.Index = 0
		Me.LoadExportSettings.Shortcut = System.Windows.Forms.Shortcut.CtrlO
		Me.LoadExportSettings.Text = "Load Export Settings"
		'
		'SaveExportSettings
		'
		Me.SaveExportSettings.Index = 1
		Me.SaveExportSettings.Shortcut = System.Windows.Forms.Shortcut.CtrlS
		Me.SaveExportSettings.Text = "Save Export Settings"
		'
		'RunMenu
		'
		Me.RunMenu.Index = 2
		Me.RunMenu.Text = "Run"
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
		Me._productionPrecedenceBox.Name = "_productionPrecedenceBox"
		Me._productionPrecedenceBox.Text = "Production Precedence"
		Me._productionPrecedenceBox.Controls.Add(Me._productionPrecedenceList)
		Me._productionPrecedenceBox.Controls.Add(Me._pickPrecedenceButton)
		Me._productionPrecedenceBox.Location = New System.Drawing.Point(576, 6)
		Me._productionPrecedenceBox.Size = New System.Drawing.Size(185, 415)
		Me._productionPrecedenceBox.TabIndex = 16
		Me._productionPrecedenceBox.TabStop = False
		'
		'_productionPrecedenceList
		'
		Me._productionPrecedenceList.Location = New System.Drawing.Point(8, 17)
		Me._productionPrecedenceList.Name = "_productionPrecedenceList"
		Me._productionPrecedenceList.Size = New System.Drawing.Size(142, 390)
		Me._productionPrecedenceList.TabIndex = 2
		Me._productionPrecedenceList.IntegralHeight = False
		'
		'_pickPrecedenceButton
		'
		Me._pickPrecedenceButton.Location = New System.Drawing.Point(152, 387)
		Me._pickPrecedenceButton.Name = "_pickPrecedenceButton"
		Me._pickPrecedenceButton.Size = New System.Drawing.Size(24, 20)
		Me._pickPrecedenceButton.TabIndex = 2
		Me._pickPrecedenceButton.Text = "..."
		Me._pickPrecedenceButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		'
		'LabelNamedAfter
		'
		Me.LabelNamedAfter.Location = New System.Drawing.Point(20, 18)
		Me.LabelNamedAfter.Name = "LabelNamedAfter"
		Me.LabelNamedAfter.Size = New System.Drawing.Size(96, 21)
		Me.LabelNamedAfter.TabIndex = 18
		Me.LabelNamedAfter.Text = "Named after:"
		Me.LabelNamedAfter.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_nativeFileNameSourceCombo
		'
		Me._nativeFileNameSourceCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._nativeFileNameSourceCombo.Items.AddRange(New Object() {"Select...", "Identifier", "Begin production number"})
		Me._nativeFileNameSourceCombo.Location = New System.Drawing.Point(116, 18)
		Me._nativeFileNameSourceCombo.Name = "_nativeFileNameSourceCombo"
		Me._nativeFileNameSourceCombo.Size = New System.Drawing.Size(176, 21)
		Me._nativeFileNameSourceCombo.TabIndex = 19
		Me._nativeFileNameSourceCombo.Visible = False
		'
		'_appendOriginalFilenameCheckbox
		'
		Me._appendOriginalFilenameCheckbox.Location = New System.Drawing.Point(12, 44)
		Me._appendOriginalFilenameCheckbox.Name = "_appendOriginalFilenameCheckbox"
		Me._appendOriginalFilenameCheckbox.Size = New System.Drawing.Size(148, 16)
		Me._appendOriginalFilenameCheckbox.TabIndex = 17
		Me._appendOriginalFilenameCheckbox.Text = "Append original filename"
		'
		'_overwriteButton
		'
		Me._overwriteCheckBox.Location = New System.Drawing.Point(8, 48)
		Me._overwriteCheckBox.Name = "_overwriteButton"
		Me._overwriteCheckBox.Size = New System.Drawing.Size(100, 16)
		Me._overwriteCheckBox.TabIndex = 7
		Me._overwriteCheckBox.Text = "Overwrite Files"
		'
		'_browseButton
		'
		Me._browseButton.Location = New System.Drawing.Point(388, 20)
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
		Me._folderPath.Size = New System.Drawing.Size(380, 20)
		Me._folderPath.TabIndex = 5
		Me._folderPath.Text = "Select a folder ..."
		'
		'GroupBoxExportLocation
		'
		Me.GroupBoxExportLocation.Controls.Add(Me._overwriteCheckBox)
		Me.GroupBoxExportLocation.Controls.Add(Me._browseButton)
		Me.GroupBoxExportLocation.Controls.Add(Me._folderPath)
		Me.GroupBoxExportLocation.Location = New System.Drawing.Point(7, 4)
		Me.GroupBoxExportLocation.Name = "GroupBoxExportLocation"
		Me.GroupBoxExportLocation.Size = New System.Drawing.Size(424, 72)
		Me.GroupBoxExportLocation.TabIndex = 11
		Me.GroupBoxExportLocation.TabStop = False
		Me.GroupBoxExportLocation.Text = "Export Location"
		'
		'TabControl1
		'
		Me.TabControl1.Controls.Add(Me._dataSourceTabPage)
		Me.TabControl1.Controls.Add(Me._destinationFileTabPage)
		Me.TabControl1.Location = New System.Drawing.Point(0, 0)
		Me.TabControl1.Name = "TabControl1"
		Me.TabControl1.SelectedIndex = 0
		Me.TabControl1.Size = New System.Drawing.Size(772, 449)
		Me.TabControl1.TabIndex = 17
		'
		'_dataSourceTabPage
		'
		Me._dataSourceTabPage.Controls.Add(Me._filtersBox)
		Me._dataSourceTabPage.Controls.Add(Me._productionPrecedenceBox)
		Me._dataSourceTabPage.Location = New System.Drawing.Point(3, 21)
		Me._dataSourceTabPage.Name = "_dataSourceTabPage"
		Me._dataSourceTabPage.Size = New System.Drawing.Size(766, 425)
		Me._dataSourceTabPage.TabIndex = 0
		Me._dataSourceTabPage.Text = "Data Source"
		'
		'_filtersBox
		'
		Me._filtersBox.Controls.Add(Me._startExportAtDocumentNumber)
		Me._filtersBox.Controls.Add(Me.LabelStartAtRecordNumber)
		Me._filtersBox.Controls.Add(Me.LabelSelectedColumns)
		Me._filtersBox.Controls.Add(Me._filters)
		Me._filtersBox.Controls.Add(Me._columnSelector)
		Me._filtersBox.Location = New System.Drawing.Point(4, 6)
		Me._filtersBox.Name = "_filtersBox"
		Me._filtersBox.Size = New System.Drawing.Size(566, 415)
		Me._filtersBox.TabIndex = 10
		Me._filtersBox.TabStop = False
		Me._filtersBox.Text = "Export"
		'
		'_startExportAtDocumentNumber
		'
		Me._startExportAtDocumentNumber.Name = "_startExportAtDocumentNumber"
		Me._startExportAtDocumentNumber.Location = New System.Drawing.Point(408, 64)
		Me._startExportAtDocumentNumber.Size = New System.Drawing.Size(148, 20)
		Me._startExportAtDocumentNumber.Maximum = New Decimal(New Integer() {10000000, 0, 0, 0})
		Me._startExportAtDocumentNumber.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
		Me._startExportAtDocumentNumber.TabIndex = 21
		Me._startExportAtDocumentNumber.Value = New Decimal(New Integer() {1, 0, 0, 0})
		'
		'LabelStartAtRecordNumber
		'
		Me.LabelStartAtRecordNumber.Name = "LabelStartAtRecordNumber"
		Me.LabelStartAtRecordNumber.Text = "Start Export at Record #"
		Me.LabelStartAtRecordNumber.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.LabelStartAtRecordNumber.Location = New System.Drawing.Point(393, 48)
		Me.LabelStartAtRecordNumber.Size = New System.Drawing.Size(160, 16)
		Me.LabelStartAtRecordNumber.TabIndex = 20
		Me.LabelStartAtRecordNumber.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'LabelSelectedColumns
		'
		Me.LabelSelectedColumns.Name = "LabelSelectedColumns"
		Me.LabelSelectedColumns.Text = "Selected Columns"
		Me.LabelSelectedColumns.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.LabelSelectedColumns.Location = New System.Drawing.Point(200, 48)
		Me.LabelSelectedColumns.Size = New System.Drawing.Size(160, 16)
		Me.LabelSelectedColumns.TabIndex = 19
		Me.LabelSelectedColumns.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
		'
		'_filters
		'
		Me._filters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._filters.ItemHeight = 13
		Me._filters.Location = New System.Drawing.Point(8, 20)
		Me._filters.Name = "_filters"
		Me._filters.Size = New System.Drawing.Size(548, 21)
		Me._filters.TabIndex = 1
		'
		'_columnSelector
		'
		Me._columnSelector.Name = "_columnSelector"
		Me._columnSelector.Location = New System.Drawing.Point(12, 64)
		Me._columnSelector.Size = New System.Drawing.Size(366, 343)
		Me._columnSelector.AlternateRowColors = True
		Me._columnSelector.KeepButtonsCentered = False
		Me._columnSelector.LeftOrderControlsVisible = False
		Me._columnSelector.RightOrderControlVisible = True
		Me._columnSelector.TabIndex = 17
		'
		'_destinationFileTabPage
		'
		Me._destinationFileTabPage.Controls.Add(Me.GroupBoxTextAndNativeFileNames)
		Me._destinationFileTabPage.Controls.Add(Me._metadataGroupBox)
		Me._destinationFileTabPage.Controls.Add(Me.GroupBoxNative)
		Me._destinationFileTabPage.Controls.Add(Me.GroupBoxImage)
		Me._destinationFileTabPage.Controls.Add(Me.GroupBoxPhysicalFileExport)
		Me._destinationFileTabPage.Controls.Add(Me._subDirectoryInformationGroupBox)
		Me._destinationFileTabPage.Controls.Add(Me._volumeInformationGroupBox)
		Me._destinationFileTabPage.Controls.Add(Me.GroupBoxFilePath)
		Me._destinationFileTabPage.Controls.Add(Me._groupBoxLoadFileCharacterInformation)
		Me._destinationFileTabPage.Controls.Add(Me.GroupBoxExportLocation)
		Me._destinationFileTabPage.Location = New System.Drawing.Point(3, 21)
		Me._destinationFileTabPage.Name = "_destinationFileTabPage"
		Me._destinationFileTabPage.Size = New System.Drawing.Size(766, 425)
		Me._destinationFileTabPage.TabIndex = 1
		Me._destinationFileTabPage.Text = "Destination Files"
		'
		'GroupBoxTextAndNativeFileNames
		'
		Me.GroupBoxTextAndNativeFileNames.Controls.Add(Me._appendOriginalFilenameCheckbox)
		Me.GroupBoxTextAndNativeFileNames.Controls.Add(Me.LabelNamedAfter)
		Me.GroupBoxTextAndNativeFileNames.Controls.Add(Me._nativeFileNameSourceCombo)
		Me.GroupBoxTextAndNativeFileNames.Location = New System.Drawing.Point(435, 4)
		Me.GroupBoxTextAndNativeFileNames.Name = "GroupBoxTextAndNativeFileNames"
		Me.GroupBoxTextAndNativeFileNames.Size = New System.Drawing.Size(324, 68)
		Me.GroupBoxTextAndNativeFileNames.TabIndex = 26
		Me.GroupBoxTextAndNativeFileNames.TabStop = False
		Me.GroupBoxTextAndNativeFileNames.Text = "Text and Native File Names"
		'
		'_metadataGroupBox
		'
		Me._metadataGroupBox.Controls.Add(Me.LabelTextPrecedence)
		Me._metadataGroupBox.Controls.Add(Me._textFieldPrecedencePicker)
		Me._metadataGroupBox.Controls.Add(Me._textFileEncoding)
		Me._metadataGroupBox.Controls.Add(Me.LabelTextFileEncoding)
		Me._metadataGroupBox.Controls.Add(Me.LabelDataFileEncoding)
		Me._metadataGroupBox.Controls.Add(Me._dataFileEncoding)
		Me._metadataGroupBox.Controls.Add(Me.LabelMetadataDataFileFormat)
		Me._metadataGroupBox.Controls.Add(Me._nativeFileFormat)
		Me._metadataGroupBox.Controls.Add(Me._exportMulticodeFieldsAsNested)
		Me._metadataGroupBox.Controls.Add(Me._exportFullTextAsFile)
		Me._metadataGroupBox.Location = New System.Drawing.Point(435, 236)
		Me._metadataGroupBox.Name = "_metadataGroupBox"
		Me._metadataGroupBox.Size = New System.Drawing.Size(324, 184)
		Me._metadataGroupBox.TabIndex = 25
		Me._metadataGroupBox.TabStop = False
		Me._metadataGroupBox.Text = "Metadata"
		'
		'LabelTextPrecedence
		'
		Me.LabelTextPrecedence.Location = New System.Drawing.Point(8, 125)
		Me.LabelTextPrecedence.Name = "LabelTextPrecedence"
		Me.LabelTextPrecedence.Size = New System.Drawing.Size(107, 21)
		Me.LabelTextPrecedence.TabIndex = 21
		Me.LabelTextPrecedence.Text = "Text Precedence:"
		Me.LabelTextPrecedence.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'_textFieldPrecedencePicker
		Me._textFieldPrecedencePicker.Location = New System.Drawing.Point(116, 125)
		Me._textFieldPrecedencePicker.Name = "_textFieldPrecedencePicker"
		Me._textFieldPrecedencePicker.Size = New System.Drawing.Size(175, 21)
		Me._textFieldPrecedencePicker.TabIndex = 20
		'
		'_textFileEncoding
		'
		Me._textFileEncoding.Location = New System.Drawing.Point(116, 100)
		Me._textFileEncoding.Name = "_textFileEncoding"
		Me._textFileEncoding.Size = New System.Drawing.Size(200, 21)
		Me._textFileEncoding.TabIndex = 19
		'
		'LabelTextFileEncoding
		'
		Me.LabelTextFileEncoding.Location = New System.Drawing.Point(12, 100)
		Me.LabelTextFileEncoding.Name = "LabelTextFileEncoding"
		Me.LabelTextFileEncoding.Size = New System.Drawing.Size(104, 21)
		Me.LabelTextFileEncoding.TabIndex = 18
		Me.LabelTextFileEncoding.Text = "Text File Encoding:"
		Me.LabelTextFileEncoding.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'LabelDataFileEncoding
		'
		Me.LabelDataFileEncoding.Location = New System.Drawing.Point(12, 48)
		Me.LabelDataFileEncoding.Name = "LabelDataFileEncoding"
		Me.LabelDataFileEncoding.Size = New System.Drawing.Size(104, 21)
		Me.LabelDataFileEncoding.TabIndex = 17
		Me.LabelDataFileEncoding.Text = "Data File Encoding:"
		Me.LabelDataFileEncoding.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_dataFileEncoding
		'
		Me._dataFileEncoding.Location = New System.Drawing.Point(116, 48)
		Me._dataFileEncoding.Name = "_dataFileEncoding"
		Me._dataFileEncoding.Size = New System.Drawing.Size(200, 21)
		Me._dataFileEncoding.TabIndex = 16
		'
		'LabelMetadataDataFileFormat
		'
		Me.LabelMetadataDataFileFormat.Location = New System.Drawing.Point(24, 20)
		Me.LabelMetadataDataFileFormat.Name = "LabelMetadataDataFileFormat"
		Me.LabelMetadataDataFileFormat.Size = New System.Drawing.Size(92, 21)
		Me.LabelMetadataDataFileFormat.TabIndex = 15
		Me.LabelMetadataDataFileFormat.Text = "Data File Format:"
		Me.LabelMetadataDataFileFormat.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_nativeFileFormat
		'
		Me._nativeFileFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._nativeFileFormat.Items.AddRange(New Object() {"Select...", "Comma-separated (.csv)", "Concordance (.dat)", "Custom (.txt)", "HTML (.html)"})
		Me._nativeFileFormat.Location = New System.Drawing.Point(116, 20)
		Me._nativeFileFormat.Name = "_nativeFileFormat"
		Me._nativeFileFormat.Size = New System.Drawing.Size(176, 21)
		Me._nativeFileFormat.TabIndex = 14
		'
		'_exportMulticodeFieldsAsNested
		'
		Me._exportMulticodeFieldsAsNested.Location = New System.Drawing.Point(12, 156)
		Me._exportMulticodeFieldsAsNested.Name = "_exportMulticodeFieldsAsNested"
		Me._exportMulticodeFieldsAsNested.Size = New System.Drawing.Size(228, 20)
		Me._exportMulticodeFieldsAsNested.TabIndex = 8
		Me._exportMulticodeFieldsAsNested.Text = "Export Multiple Choice Fields as Nested"
		'
		'_exportFullTextAsFile
		'
		Me._exportFullTextAsFile.Location = New System.Drawing.Point(12, 76)
		Me._exportFullTextAsFile.Name = "_exportFullTextAsFile"
		Me._exportFullTextAsFile.Size = New System.Drawing.Size(196, 20)
		Me._exportFullTextAsFile.TabIndex = 7
		Me._exportFullTextAsFile.Text = "Export Text Field as Files"
		'
		'GroupBoxNative
		'
		Me.GroupBoxNative.Controls.Add(Me._exportNativeFiles)
		Me.GroupBoxNative.Location = New System.Drawing.Point(435, 184)
		Me.GroupBoxNative.Name = "GroupBoxNative"
		Me.GroupBoxNative.Size = New System.Drawing.Size(324, 48)
		Me.GroupBoxNative.TabIndex = 24
		Me.GroupBoxNative.TabStop = False
		Me.GroupBoxNative.Text = "Native "
		'
		'_exportNativeFiles
		'
		Me._exportNativeFiles.Checked = True
		Me._exportNativeFiles.CheckState = System.Windows.Forms.CheckState.Checked
		Me._exportNativeFiles.Location = New System.Drawing.Point(12, 20)
		Me._exportNativeFiles.Name = "_exportNativeFiles"
		Me._exportNativeFiles.Size = New System.Drawing.Size(300, 20)
		Me._exportNativeFiles.TabIndex = 11
		Me._exportNativeFiles.Text = "Export Native Files"
		'
		'GroupBoxImage
		'
		Me.GroupBoxImage.Controls.Add(Me.LabelFileType)
		Me.GroupBoxImage.Controls.Add(Me._exportImages)
		Me.GroupBoxImage.Controls.Add(Me._imageFileFormat)
		Me.GroupBoxImage.Controls.Add(Me.LabelImageDataFileFormat)
		Me.GroupBoxImage.Controls.Add(Me._imageTypeDropdown)
		Me.GroupBoxImage.Location = New System.Drawing.Point(435, 76)
		Me.GroupBoxImage.Name = "GroupBoxImage"
		Me.GroupBoxImage.Size = New System.Drawing.Size(324, 104)
		Me.GroupBoxImage.TabIndex = 23
		Me.GroupBoxImage.TabStop = False
		Me.GroupBoxImage.Text = "Image "
		'
		'LabelFileType
		'
		Me.LabelFileType.Location = New System.Drawing.Point(24, 76)
		Me.LabelFileType.Name = "LabelFileType"
		Me.LabelFileType.Size = New System.Drawing.Size(92, 21)
		Me.LabelFileType.TabIndex = 19
		Me.LabelFileType.Text = "File Type:"
		Me.LabelFileType.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_exportImages
		'
		Me._exportImages.Checked = True
		Me._exportImages.CheckState = System.Windows.Forms.CheckState.Checked
		Me._exportImages.Location = New System.Drawing.Point(12, 20)
		Me._exportImages.Name = "_exportImages"
		Me._exportImages.Size = New System.Drawing.Size(96, 20)
		Me._exportImages.TabIndex = 18
		Me._exportImages.Text = "Export Images"
		'
		'_imageFileFormat
		'
		Me._imageFileFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._imageFileFormat.DropDownWidth = 150
		Me._imageFileFormat.Location = New System.Drawing.Point(116, 48)
		Me._imageFileFormat.Name = "_imageFileFormat"
		Me._imageFileFormat.Size = New System.Drawing.Size(176, 21)
		Me._imageFileFormat.TabIndex = 11
		'
		'LabelImageDataFileFormat
		'
		Me.LabelImageDataFileFormat.Location = New System.Drawing.Point(24, 48)
		Me.LabelImageDataFileFormat.Name = "LabelImageDataFileFormat"
		Me.LabelImageDataFileFormat.Size = New System.Drawing.Size(92, 21)
		Me.LabelImageDataFileFormat.TabIndex = 12
		Me.LabelImageDataFileFormat.Text = "Data File Format:"
		Me.LabelImageDataFileFormat.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_imageTypeDropdown
		'
		Me._imageTypeDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._imageTypeDropdown.DropDownWidth = 150
		Me._imageTypeDropdown.Items.AddRange(New Object() {"Select...", "Single-page TIF/JPG", "Multi-page TIF", "PDF"})
		Me._imageTypeDropdown.Location = New System.Drawing.Point(116, 76)
		Me._imageTypeDropdown.Name = "_imageTypeDropdown"
		Me._imageTypeDropdown.Size = New System.Drawing.Size(176, 21)
		Me._imageTypeDropdown.TabIndex = 17
		'
		'GroupBoxPhysicalFileExport
		'
		Me.GroupBoxPhysicalFileExport.Controls.Add(Me._copyFilesFromRepository)
		Me.GroupBoxPhysicalFileExport.Location = New System.Drawing.Point(7, 84)
		Me.GroupBoxPhysicalFileExport.Name = "GroupBoxPhysicalFileExport"
		Me.GroupBoxPhysicalFileExport.Size = New System.Drawing.Size(212, 48)
		Me.GroupBoxPhysicalFileExport.TabIndex = 22
		Me.GroupBoxPhysicalFileExport.TabStop = False
		Me.GroupBoxPhysicalFileExport.Text = "Physical File Export"
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
		Me._subDirectoryInformationGroupBox.Controls.Add(Me.LabelSubdirectoryNumberOfDigits)
		Me._subDirectoryInformationGroupBox.Controls.Add(Me._subdirectoryTextPrefix)
		Me._subDirectoryInformationGroupBox.Controls.Add(Me.LabelTextPrefix)
		Me._subDirectoryInformationGroupBox.Controls.Add(Me._subDirectoryNativePrefix)
		Me._subDirectoryInformationGroupBox.Controls.Add(Me.LabelNativePrefix)
		Me._subDirectoryInformationGroupBox.Controls.Add(Me._subDirectoryMaxSize)
		Me._subDirectoryInformationGroupBox.Controls.Add(Me._subdirectoryStartNumber)
		Me._subDirectoryInformationGroupBox.Controls.Add(Me.LabelMaxFiles)
		Me._subDirectoryInformationGroupBox.Controls.Add(Me.LabelSubdirectoryStartNumber)
		Me._subDirectoryInformationGroupBox.Controls.Add(Me.LabelImagePrefix)
		Me._subDirectoryInformationGroupBox.Controls.Add(Me._subdirectoryImagePrefix)
		Me._subDirectoryInformationGroupBox.Location = New System.Drawing.Point(7, 256)
		Me._subDirectoryInformationGroupBox.Name = "_subDirectoryInformationGroupBox"
		Me._subDirectoryInformationGroupBox.Size = New System.Drawing.Size(212, 164)
		Me._subDirectoryInformationGroupBox.TabIndex = 21
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
		Me._subdirectoryDigitPadding.TabIndex = 24
		Me._subdirectoryDigitPadding.Value = New Decimal(New Integer() {1, 0, 0, 0})
		'
		'LabelSubdirectoryNumberOfDigits
		'
		Me.LabelSubdirectoryNumberOfDigits.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.LabelSubdirectoryNumberOfDigits.Location = New System.Drawing.Point(104, 104)
		Me.LabelSubdirectoryNumberOfDigits.Name = "LabelSubdirectoryNumberOfDigits"
		Me.LabelSubdirectoryNumberOfDigits.Size = New System.Drawing.Size(56, 20)
		Me.LabelSubdirectoryNumberOfDigits.TabIndex = 23
		Me.LabelSubdirectoryNumberOfDigits.Text = "# of digits:"
		Me.LabelSubdirectoryNumberOfDigits.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_subdirectoryTextPrefix
		'
		Me._subdirectoryTextPrefix.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me._subdirectoryTextPrefix.Location = New System.Drawing.Point(116, 76)
		Me._subdirectoryTextPrefix.Name = "_subdirectoryTextPrefix"
		Me._subdirectoryTextPrefix.Size = New System.Drawing.Size(88, 20)
		Me._subdirectoryTextPrefix.TabIndex = 22
		Me._subdirectoryTextPrefix.Text = "TEXT"
		'
		'LabelTextPrefix
		'
		Me.LabelTextPrefix.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.LabelTextPrefix.Location = New System.Drawing.Point(36, 76)
		Me.LabelTextPrefix.Name = "LabelTextPrefix"
		Me.LabelTextPrefix.Size = New System.Drawing.Size(80, 20)
		Me.LabelTextPrefix.TabIndex = 21
		Me.LabelTextPrefix.Text = "Text Prefix: "
		Me.LabelTextPrefix.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_subDirectoryNativePrefix
		'
		Me._subDirectoryNativePrefix.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me._subDirectoryNativePrefix.Location = New System.Drawing.Point(116, 48)
		Me._subDirectoryNativePrefix.Name = "_subDirectoryNativePrefix"
		Me._subDirectoryNativePrefix.Size = New System.Drawing.Size(88, 20)
		Me._subDirectoryNativePrefix.TabIndex = 20
		Me._subDirectoryNativePrefix.Text = "NATIVE"
		'
		'LabelNativePrefix
		'
		Me.LabelNativePrefix.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.LabelNativePrefix.Location = New System.Drawing.Point(44, 48)
		Me.LabelNativePrefix.Name = "LabelNativePrefix"
		Me.LabelNativePrefix.Size = New System.Drawing.Size(72, 20)
		Me.LabelNativePrefix.TabIndex = 19
		Me.LabelNativePrefix.Text = "Native Prefix: "
		Me.LabelNativePrefix.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_subDirectoryMaxSize
		'
		Me._subDirectoryMaxSize.Location = New System.Drawing.Point(116, 132)
		Me._subDirectoryMaxSize.Maximum = New Decimal(New Integer() {2000000, 0, 0, 0})
		Me._subDirectoryMaxSize.Name = "_subDirectoryMaxSize"
		Me._subDirectoryMaxSize.Size = New System.Drawing.Size(88, 20)
		Me._subDirectoryMaxSize.TabIndex = 17
		Me._subDirectoryMaxSize.Value = New Decimal(New Integer() {500, 0, 0, 0})
		'
		'_subdirectoryStartNumber
		'
		Me._subdirectoryStartNumber.Location = New System.Drawing.Point(56, 104)
		Me._subdirectoryStartNumber.Maximum = New Decimal(New Integer() {2000000, 0, 0, 0})
		Me._subdirectoryStartNumber.Name = "_subdirectoryStartNumber"
		Me._subdirectoryStartNumber.Size = New System.Drawing.Size(44, 20)
		Me._subdirectoryStartNumber.TabIndex = 16
		Me._subdirectoryStartNumber.Value = New Decimal(New Integer() {1, 0, 0, 0})
		'
		'LabelMaxFiles
		'
		Me.LabelMaxFiles.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.LabelMaxFiles.Location = New System.Drawing.Point(56, 132)
		Me.LabelMaxFiles.Name = "LabelMaxFiles"
		Me.LabelMaxFiles.Size = New System.Drawing.Size(60, 20)
		Me.LabelMaxFiles.TabIndex = 15
		Me.LabelMaxFiles.Text = "Max Files:"
		Me.LabelMaxFiles.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'LabelSubdirectoryStartNumber
		'
		Me.LabelSubdirectoryStartNumber.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.LabelSubdirectoryStartNumber.Location = New System.Drawing.Point(8, 104)
		Me.LabelSubdirectoryStartNumber.Name = "LabelSubdirectoryStartNumber"
		Me.LabelSubdirectoryStartNumber.Size = New System.Drawing.Size(44, 20)
		Me.LabelSubdirectoryStartNumber.TabIndex = 14
		Me.LabelSubdirectoryStartNumber.Text = "Start #:"
		Me.LabelSubdirectoryStartNumber.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'LabelImagePrefix
		'
		Me.LabelImagePrefix.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.LabelImagePrefix.Location = New System.Drawing.Point(44, 20)
		Me.LabelImagePrefix.Name = "LabelImagePrefix"
		Me.LabelImagePrefix.Size = New System.Drawing.Size(72, 20)
		Me.LabelImagePrefix.TabIndex = 13
		Me.LabelImagePrefix.Text = "Image Prefix: "
		Me.LabelImagePrefix.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_subdirectoryImagePrefix
		'
		Me._subdirectoryImagePrefix.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me._subdirectoryImagePrefix.Location = New System.Drawing.Point(116, 20)
		Me._subdirectoryImagePrefix.Name = "_subdirectoryImagePrefix"
		Me._subdirectoryImagePrefix.Size = New System.Drawing.Size(88, 20)
		Me._subdirectoryImagePrefix.TabIndex = 12
		Me._subdirectoryImagePrefix.Text = "IMG"
		'
		'_volumeInformationGroupBox
		'
		Me._volumeInformationGroupBox.Controls.Add(Me._volumeDigitPadding)
		Me._volumeInformationGroupBox.Controls.Add(Me.LabelVolumeNumberOfDigits)
		Me._volumeInformationGroupBox.Controls.Add(Me._volumeMaxSize)
		Me._volumeInformationGroupBox.Controls.Add(Me._volumeStartNumber)
		Me._volumeInformationGroupBox.Controls.Add(Me.LabelMaxSizeMB)
		Me._volumeInformationGroupBox.Controls.Add(Me.LabelVolumeStartNumber)
		Me._volumeInformationGroupBox.Controls.Add(Me.LabelPrefix)
		Me._volumeInformationGroupBox.Controls.Add(Me._volumePrefix)
		Me._volumeInformationGroupBox.Location = New System.Drawing.Point(7, 140)
		Me._volumeInformationGroupBox.Name = "_volumeInformationGroupBox"
		Me._volumeInformationGroupBox.Size = New System.Drawing.Size(212, 108)
		Me._volumeInformationGroupBox.TabIndex = 20
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
		Me._volumeDigitPadding.TabIndex = 13
		Me._volumeDigitPadding.Value = New Decimal(New Integer() {1, 0, 0, 0})
		'
		'LabelVolumeNumberOfDigits
		'
		Me.LabelVolumeNumberOfDigits.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.LabelVolumeNumberOfDigits.Location = New System.Drawing.Point(104, 48)
		Me.LabelVolumeNumberOfDigits.Name = "LabelVolumeNumberOfDigits"
		Me.LabelVolumeNumberOfDigits.Size = New System.Drawing.Size(56, 20)
		Me.LabelVolumeNumberOfDigits.TabIndex = 12
		Me.LabelVolumeNumberOfDigits.Text = "# of digits:"
		Me.LabelVolumeNumberOfDigits.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_volumeMaxSize
		'
		Me._volumeMaxSize.Location = New System.Drawing.Point(116, 76)
		Me._volumeMaxSize.Maximum = New Decimal(New Integer() {2000000, 0, 0, 0})
		Me._volumeMaxSize.Name = "_volumeMaxSize"
		Me._volumeMaxSize.Size = New System.Drawing.Size(88, 20)
		Me._volumeMaxSize.TabIndex = 11
		Me._volumeMaxSize.Value = New Decimal(New Integer() {650, 0, 0, 0})
		'
		'_volumeStartNumber
		'
		Me._volumeStartNumber.Location = New System.Drawing.Point(52, 48)
		Me._volumeStartNumber.Maximum = New Decimal(New Integer() {2000000, 0, 0, 0})
		Me._volumeStartNumber.Name = "_volumeStartNumber"
		Me._volumeStartNumber.Size = New System.Drawing.Size(44, 20)
		Me._volumeStartNumber.TabIndex = 10
		Me._volumeStartNumber.Value = New Decimal(New Integer() {1, 0, 0, 0})
		'
		'LabelMaxSizeMB
		'
		Me.LabelMaxSizeMB.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.LabelMaxSizeMB.Location = New System.Drawing.Point(32, 76)
		Me.LabelMaxSizeMB.Name = "LabelMaxSizeMB"
		Me.LabelMaxSizeMB.Size = New System.Drawing.Size(82, 20)
		Me.LabelMaxSizeMB.TabIndex = 9
		Me.LabelMaxSizeMB.Text = "Max Size (MB):"
		Me.LabelMaxSizeMB.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'LabelVolumeStartNumber
		'
		Me.LabelVolumeStartNumber.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.LabelVolumeStartNumber.Location = New System.Drawing.Point(8, 48)
		Me.LabelVolumeStartNumber.Name = "LabelVolumeStartNumber"
		Me.LabelVolumeStartNumber.Size = New System.Drawing.Size(44, 20)
		Me.LabelVolumeStartNumber.TabIndex = 8
		Me.LabelVolumeStartNumber.Text = "Start #:"
		Me.LabelVolumeStartNumber.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'LabelPrefix
		'
		Me.LabelPrefix.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.LabelPrefix.Location = New System.Drawing.Point(76, 20)
		Me.LabelPrefix.Name = "LabelPrefix"
		Me.LabelPrefix.Size = New System.Drawing.Size(40, 20)
		Me.LabelPrefix.TabIndex = 7
		Me.LabelPrefix.Text = "Prefix: "
		Me.LabelPrefix.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_volumePrefix
		'
		Me._volumePrefix.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me._volumePrefix.Location = New System.Drawing.Point(116, 20)
		Me._volumePrefix.Name = "_volumePrefix"
		Me._volumePrefix.Size = New System.Drawing.Size(88, 20)
		Me._volumePrefix.TabIndex = 6
		Me._volumePrefix.Text = "VOL"
		'
		'GroupBoxFilePath
		'
		Me.GroupBoxFilePath.Controls.Add(Me._prefixText)
		Me.GroupBoxFilePath.Controls.Add(Me._usePrefix)
		Me.GroupBoxFilePath.Controls.Add(Me._useAbsolutePaths)
		Me.GroupBoxFilePath.Controls.Add(Me._useRelativePaths)
		Me.GroupBoxFilePath.Location = New System.Drawing.Point(223, 84)
		Me.GroupBoxFilePath.Name = "GroupBoxFilePath"
		Me.GroupBoxFilePath.Size = New System.Drawing.Size(208, 120)
		Me.GroupBoxFilePath.TabIndex = 12
		Me.GroupBoxFilePath.TabStop = False
		Me.GroupBoxFilePath.Text = "File Path"
		'
		'_prefixText
		'
		Me._prefixText.Enabled = False
		Me._prefixText.Location = New System.Drawing.Point(88, 80)
		Me._prefixText.Name = "_prefixText"
		Me._prefixText.Size = New System.Drawing.Size(112, 20)
		Me._prefixText.TabIndex = 16
		Me._prefixText.Text = ""
		'
		'_usePrefix
		'
		Me._usePrefix.Location = New System.Drawing.Point(12, 84)
		Me._usePrefix.Name = "_usePrefix"
		Me._usePrefix.Size = New System.Drawing.Size(76, 16)
		Me._usePrefix.TabIndex = 15
		Me._usePrefix.Text = "Use Prefix"
		'
		'_useAbsolutePaths
		'
		Me._useAbsolutePaths.Location = New System.Drawing.Point(12, 52)
		Me._useAbsolutePaths.Name = "_useAbsolutePaths"
		Me._useAbsolutePaths.Size = New System.Drawing.Size(124, 16)
		Me._useAbsolutePaths.TabIndex = 14
		Me._useAbsolutePaths.Text = "Use Absolute Paths"
		'
		'_useRelativePaths
		'
		Me._useRelativePaths.Checked = True
		Me._useRelativePaths.Location = New System.Drawing.Point(12, 20)
		Me._useRelativePaths.Name = "_useRelativePaths"
		Me._useRelativePaths.Size = New System.Drawing.Size(124, 16)
		Me._useRelativePaths.TabIndex = 13
		Me._useRelativePaths.TabStop = True
		Me._useRelativePaths.Text = "Use Relative Paths"
		'
		'_groupBoxLoadFileCharacterInformation
		'
		Me._groupBoxLoadFileCharacterInformation.Controls.Add(Me.LabelNestedValue)
		Me._groupBoxLoadFileCharacterInformation.Controls.Add(Me._nestedValueDelimiter)
		Me._groupBoxLoadFileCharacterInformation.Controls.Add(Me._multiRecordDelimiter)
		Me._groupBoxLoadFileCharacterInformation.Controls.Add(Me.LabelMultiValue)
		Me._groupBoxLoadFileCharacterInformation.Controls.Add(Me.LabelQuote)
		Me._groupBoxLoadFileCharacterInformation.Controls.Add(Me._quoteDelimiter)
		Me._groupBoxLoadFileCharacterInformation.Controls.Add(Me.LabelNewline)
		Me._groupBoxLoadFileCharacterInformation.Controls.Add(Me._newLineDelimiter)
		Me._groupBoxLoadFileCharacterInformation.Controls.Add(Me.LabelColumn)
		Me._groupBoxLoadFileCharacterInformation.Controls.Add(Me._recordDelimiter)
		Me._groupBoxLoadFileCharacterInformation.Location = New System.Drawing.Point(223, 212)
		Me._groupBoxLoadFileCharacterInformation.Name = "_groupBoxLoadFileCharacterInformation"
		Me._groupBoxLoadFileCharacterInformation.Size = New System.Drawing.Size(208, 208)
		Me._groupBoxLoadFileCharacterInformation.TabIndex = 15
		Me._groupBoxLoadFileCharacterInformation.TabStop = False
		Me._groupBoxLoadFileCharacterInformation.Text = "Load File Characters"
		'
		'LabelNestedValue
		'
		Me.LabelNestedValue.Location = New System.Drawing.Point(8, 168)
		Me.LabelNestedValue.Name = "LabelNestedValue"
		Me.LabelNestedValue.Size = New System.Drawing.Size(76, 21)
		Me.LabelNestedValue.TabIndex = 17
		Me.LabelNestedValue.Text = "Nested Value:"
		Me.LabelNestedValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_nestedValueDelimiter
		'
		Me._nestedValueDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._nestedValueDelimiter.Enabled = False
		Me._nestedValueDelimiter.Location = New System.Drawing.Point(84, 168)
		Me._nestedValueDelimiter.Name = "_nestedValueDelimiter"
		Me._nestedValueDelimiter.Size = New System.Drawing.Size(116, 21)
		Me._nestedValueDelimiter.TabIndex = 16
		'
		'_multiRecordDelimiter
		'
		Me._multiRecordDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._multiRecordDelimiter.Enabled = False
		Me._multiRecordDelimiter.Location = New System.Drawing.Point(84, 132)
		Me._multiRecordDelimiter.Name = "_multiRecordDelimiter"
		Me._multiRecordDelimiter.Size = New System.Drawing.Size(116, 21)
		Me._multiRecordDelimiter.TabIndex = 15
		'
		'LabelMultiValue
		'
		Me.LabelMultiValue.Location = New System.Drawing.Point(8, 132)
		Me.LabelMultiValue.Name = "LabelMultiValue"
		Me.LabelMultiValue.Size = New System.Drawing.Size(76, 21)
		Me.LabelMultiValue.TabIndex = 14
		Me.LabelMultiValue.Text = "Multi-Value:"
		Me.LabelMultiValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'LabelQuote
		'
		Me.LabelQuote.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.LabelQuote.Location = New System.Drawing.Point(8, 60)
		Me.LabelQuote.Name = "LabelQuote"
		Me.LabelQuote.Size = New System.Drawing.Size(76, 21)
		Me.LabelQuote.TabIndex = 13
		Me.LabelQuote.Text = "Quote:"
		Me.LabelQuote.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_quoteDelimiter
		'
		Me._quoteDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._quoteDelimiter.Enabled = False
		Me._quoteDelimiter.Location = New System.Drawing.Point(84, 60)
		Me._quoteDelimiter.Name = "_quoteDelimiter"
		Me._quoteDelimiter.Size = New System.Drawing.Size(116, 21)
		Me._quoteDelimiter.TabIndex = 12
		'
		'LabelNewline
		'
		Me.LabelNewline.Location = New System.Drawing.Point(8, 96)
		Me.LabelNewline.Name = "LabelNewline"
		Me.LabelNewline.Size = New System.Drawing.Size(76, 21)
		Me.LabelNewline.TabIndex = 11
		Me.LabelNewline.Text = "Newline:"
		Me.LabelNewline.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_newLineDelimiter
		'
		Me._newLineDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._newLineDelimiter.Enabled = False
		Me._newLineDelimiter.Location = New System.Drawing.Point(84, 96)
		Me._newLineDelimiter.Name = "_newLineDelimiter"
		Me._newLineDelimiter.Size = New System.Drawing.Size(116, 21)
		Me._newLineDelimiter.TabIndex = 10
		'
		'LabelColumn
		'
		Me.LabelColumn.Location = New System.Drawing.Point(8, 24)
		Me.LabelColumn.Name = "LabelColumn"
		Me.LabelColumn.Size = New System.Drawing.Size(76, 21)
		Me.LabelColumn.TabIndex = 9
		Me.LabelColumn.Text = "Column:"
		Me.LabelColumn.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_recordDelimiter
		'
		Me._recordDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._recordDelimiter.Enabled = False
		Me._recordDelimiter.Location = New System.Drawing.Point(84, 24)
		Me._recordDelimiter.Name = "_recordDelimiter"
		Me._recordDelimiter.Size = New System.Drawing.Size(116, 21)
		Me._recordDelimiter.TabIndex = 8
		'
		'_saveExportSettingsDialog
		'
		Me._saveExportSettingsDialog.DefaultExt = "kwx"
		Me._saveExportSettingsDialog.Filter = "Relativity Desktop Client settings files (*.kwx)|*.kwx|All files (*.*)|*.*"
		Me._saveExportSettingsDialog.RestoreDirectory = True

		Me._loadExportSettingsDialog.DefaultExt = "kwx"
		Me._loadExportSettingsDialog.Filter = "Relativity Desktop Client settings files (*.kwx)|*.kwx|All files (*.*)|*.*"
		Me._loadExportSettingsDialog.RestoreDirectory = True

		'
		'ExportForm
		'
		Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
		Me.FormBorderStyle = Windows.Forms.FormBorderStyle.Sizable
		Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
		Me.MaximizeBox = True
		Me.MinimumSize = New System.Drawing.Size(700, 400)
		Me.BackColor = System.Drawing.SystemColors.Control
		Me.ClientSize = New System.Drawing.Size(775, 452)
		Me.Controls.Add(Me.TabControl1)
		Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
		Me.Menu = Me.MainMenu1
		Me.Name = "ExportForm"
		Me.Text = "Relativity Desktop Client | Export "

		Me._productionPrecedenceBox.ResumeLayout(False)
		Me.GroupBoxExportLocation.ResumeLayout(False)
		Me.TabControl1.ResumeLayout(False)
		Me._dataSourceTabPage.ResumeLayout(False)
		Me._filtersBox.ResumeLayout(False)
		CType(Me._startExportAtDocumentNumber, System.ComponentModel.ISupportInitialize).EndInit()
		Me._destinationFileTabPage.ResumeLayout(False)
		Me.GroupBoxTextAndNativeFileNames.ResumeLayout(False)
		Me._metadataGroupBox.ResumeLayout(False)
		Me.GroupBoxNative.ResumeLayout(False)
		Me.GroupBoxImage.ResumeLayout(False)
		Me.GroupBoxPhysicalFileExport.ResumeLayout(False)
		Me._subDirectoryInformationGroupBox.ResumeLayout(False)
		CType(Me._subdirectoryDigitPadding, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me._subDirectoryMaxSize, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me._subdirectoryStartNumber, System.ComponentModel.ISupportInitialize).EndInit()
		Me._volumeInformationGroupBox.ResumeLayout(False)
		CType(Me._volumeDigitPadding, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me._volumeMaxSize, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me._volumeStartNumber, System.ComponentModel.ISupportInitialize).EndInit()
		Me.GroupBoxFilePath.ResumeLayout(False)
		Me._groupBoxLoadFileCharacterInformation.ResumeLayout(False)
		Me.ResumeLayout(False)

	End Sub

#End Region

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

	Private _layoutRatioList As List(Of RelativeLayoutData)
	Private _layoutDifferenceList As List(Of RelativeLayoutData)

	Private Function CalcReferenceDistance() As Int32
		Return _startExportAtDocumentNumber.Width
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

		'Layout properties which are based on a ratio to another layout property. 
		If _layoutRatioList Is Nothing Then
			_layoutRatioList = New List(Of RelativeLayoutData)

			'When the width of the dialog increases by 3 pixels, the column selector increases by 2 pixels.  The ratio is 2/3 = .666667
			_layoutRatioList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForRatio.Width, _columnSelector, LayoutRelativePropertyTypeForRatio.Width, 0.666667))
			'When the width of the dialog increases by 2 pixels, the production listbox increases by 1 pixel.  The ratio is 1/3 = .333333
			_layoutRatioList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForRatio.Width, _productionPrecedenceList, LayoutRelativePropertyTypeForRatio.Width, 0.333333))
			'The height of the column selector increases 1-for-1 with the height of the dialog (as an alternative, this could have been set as a difference)
			_layoutRatioList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForRatio.Height, _columnSelector, LayoutRelativePropertyTypeForRatio.Height, 1.0))
			'The height of the column selector increases 1-for-1 with the height of the dialog (as an alternative, this could have been set as a difference)
			_layoutRatioList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForRatio.Height, _productionPrecedenceList, LayoutRelativePropertyTypeForRatio.Height, 1.0))
		End If

		_layoutRatioList.ForEach(Sub(x)
															 x.InitalizeRatioValues()
														 End Sub)

		'Layout properties which are directly based on another layout property.  These are all properties with a 1-1 ration
		If _layoutDifferenceList Is Nothing Then
			_layoutDifferenceList = New List(Of RelativeLayoutData)

			_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Height, _columnSelector, LayoutRelativePropertyTypeForDifference.Height))
			_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Height, _productionPrecedenceList, LayoutRelativePropertyTypeForDifference.Height))

			_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Width, TabControl1, LayoutRelativePropertyTypeForDifference.Width))
			_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Width, _dataSourceTabPage, LayoutRelativePropertyTypeForDifference.Width))
			_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Height, TabControl1, LayoutRelativePropertyTypeForDifference.Height))
			_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Height, _dataSourceTabPage, LayoutRelativePropertyTypeForDifference.Height))

			_layoutDifferenceList.Add(New RelativeLayoutData(_columnSelector, LayoutBasePropertyTypeForDifference.Width, _filtersBox, LayoutRelativePropertyTypeForDifference.Width))
			_layoutDifferenceList.Add(New RelativeLayoutData(_columnSelector, LayoutBasePropertyTypeForDifference.Width, _filters, LayoutRelativePropertyTypeForDifference.Width))
			_layoutDifferenceList.Add(New RelativeLayoutData(_columnSelector, LayoutBasePropertyTypeForDifference.Height, _filtersBox, LayoutRelativePropertyTypeForDifference.Height))
			_layoutDifferenceList.Add(New RelativeLayoutData(_columnSelector, LayoutBasePropertyTypeForDifference.Right, Me.LabelSelectedColumns, LayoutRelativePropertyTypeForDifference.Left))
			_layoutDifferenceList.Add(New RelativeLayoutData(_columnSelector, LayoutBasePropertyTypeForDifference.Right, Me.LabelStartAtRecordNumber, LayoutRelativePropertyTypeForDifference.Left))
			_layoutDifferenceList.Add(New RelativeLayoutData(_columnSelector, LayoutBasePropertyTypeForDifference.Right, Me._startExportAtDocumentNumber, LayoutRelativePropertyTypeForDifference.Left))

			_layoutDifferenceList.Add(New RelativeLayoutData(_filtersBox, LayoutBasePropertyTypeForDifference.Right, Me._productionPrecedenceBox, LayoutRelativePropertyTypeForDifference.Left))

			_layoutDifferenceList.Add(New RelativeLayoutData(_productionPrecedenceList, LayoutBasePropertyTypeForDifference.Width, Me._productionPrecedenceBox, LayoutRelativePropertyTypeForDifference.Width))
			_layoutDifferenceList.Add(New RelativeLayoutData(_productionPrecedenceList, LayoutBasePropertyTypeForDifference.Height, Me._productionPrecedenceBox, LayoutRelativePropertyTypeForDifference.Height))
		End If

		_layoutDifferenceList.ForEach(Sub(x)
																		x.InitializeDifference()
																	End Sub)

		_layoutReferenceDistance = CalcReferenceDistance()

		AdjustColumnLabel()
	End Sub

	Public Sub AdjustLayout()
		If Not _layoutLastFormSize.Equals(Me.Size) Then
			For Each x As RelativeLayoutData In _layoutRatioList
				x.AdjustRelativeControlBasedOnRatio()
			Next

			For Each x As RelativeLayoutData In _layoutDifferenceList
				x.AdjustRelativeControlBasedOnDifference()
			Next

			_layoutLastFormSize = Me.Size

			AdjustColumnLabel()
		End If
	End Sub

	Private Sub AdjustColumnLabel()
		'Adjust the location of the label to be aligned with the left side of the Right ListBox

		'Get the absolute position of the Right ListBox of the TwoListBox in screen coordinates
		Dim absoluteListBoxLoc As Point = _columnSelector.RightListBox.PointToScreen(New Point(0, 0))
		'Convert to a location relative to the Views group (_filtersBox)
		Dim relativeListBoxLoc As Point = Me.LabelSelectedColumns.Parent.PointToClient(absoluteListBoxLoc)
		'Adjust the location of the label
		Me.LabelSelectedColumns.Left = relativeListBoxLoc.X
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
		If _exportNativeFiles.Checked OrElse _columnSelector.RightListBoxItems.Count > 0 Then
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
		If Me.ExportFile.TypeOfExport = ExportFile.ExportType.Production AndAlso _exportNativeFiles.Checked Then
			If CType(_nativeFileNameSourceCombo.SelectedItem, String) = "Select..." Then
				AppendErrorMessage(msg, "No file name source selected")
			End If
		End If
		If _dataFileEncoding.SelectedEncoding Is Nothing Then
			AppendErrorMessage(msg, "No encoding selected for metadata file.")
		End If
		If _exportFullTextAsFile.Checked Then
			If _textFileEncoding.SelectedEncoding Is Nothing Then
				AppendErrorMessage(msg, "No encoding selected for text field files.")
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
				If _nativeFileNameSourceCombo.SelectedItem.ToString.ToLower = "identifier" Then
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
		_exportFile.AppendOriginalFileName = _appendOriginalFilenameCheckbox.Checked

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
		For Each field As ViewFieldInfo In _columnSelector.RightListBoxItems
			selectedViewFields.Add(field)
		Next
		_exportFile.SelectedViewFields = DirectCast(selectedViewFields.ToArray(GetType(ViewFieldInfo)), ViewFieldInfo())
		If _textFieldPrecedencePicker.SelectedFields.Count > 0 Then
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

				_columnSelector.EnsureHorizontalScrollbars()
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
		If _appendOriginalFilenameCheckbox.Checked <> ef.AppendOriginalFileName Then _appendOriginalFilenameCheckbox.Checked = ef.AppendOriginalFileName

		Select Case ef.ExportNativesToFileNamedFrom
			Case kCura.WinEDDS.ExportNativeWithFilenameFrom.Identifier
				_nativeFileNameSourceCombo.SelectedItem = "Identifier"
			Case kCura.WinEDDS.ExportNativeWithFilenameFrom.Production
				_nativeFileNameSourceCombo.SelectedItem = "Begin production number"
			Case kCura.WinEDDS.ExportNativeWithFilenameFrom.Select
				_nativeFileNameSourceCombo.SelectedItem = "Select..."
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
			_columnSelector.ClearSelection(kCura.Windows.Forms.ListBoxLocation.Left)
			_columnSelector.LeftListBoxItems.Clear()
			Array.Sort(ef.AllExportableFields)
			_columnSelector.LeftListBoxItems.AddRange(ef.AllExportableFields)
		End If

		If ef.SelectedViewFields IsNot Nothing Then



			Dim itemsToRemoveFromLeftListBox As New System.Collections.Generic.List(Of kCura.WinEDDS.ViewFieldInfo)()
			_columnSelector.ClearSelection(kCura.Windows.Forms.ListBoxLocation.Right)
			_columnSelector.RightListBoxItems.Clear()
			For Each viewFieldFromKwx As kCura.WinEDDS.ViewFieldInfo In ef.SelectedViewFields
				For Each leftListBoxViewField As kCura.WinEDDS.ViewFieldInfo In _columnSelector.LeftListBoxItems
					If leftListBoxViewField.DisplayName.Equals(viewFieldFromKwx.DisplayName, StringComparison.InvariantCulture) Then
						itemsToRemoveFromLeftListBox.Add(leftListBoxViewField)
						_columnSelector.RightListBoxItems.Add(leftListBoxViewField)
					End If
				Next
			Next

			If _columnSelector.RightListBoxItems.Count = 0 Then
				_metadataGroupBox.Enabled = False
			End If

			For Each vfi As kCura.WinEDDS.ViewFieldInfo In itemsToRemoveFromLeftListBox
				_columnSelector.LeftListBoxItems.Remove(vfi)
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
		If Not imageType.HasValue Then
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
		_columnSelector.EnsureHorizontalScrollbars()
		_columnSelector.LeftOrderControlsVisible = False
		_columnSelector.RightOrderControlVisible = True
		InitializeLayout()
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
		LabelNamedAfter.Visible = False
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
				LabelNamedAfter.Visible = True
				_filters.Text = "Productions"
				_filtersBox.Text = "Productions"
				_nativeFileNameSourceCombo.Visible = True
				_nativeFileNameSourceCombo.SelectedIndex = 0
				Me.Text = "Relativity Desktop Client | Export Production Set"
				_productionPrecedenceBox.Visible = False
		End Select
		If Not Me.ExportFile.ArtifactTypeID = Relativity.ArtifactType.Document Then
			_productionPrecedenceBox.Visible = False
			LabelNativePrefix.Text = "File Prefix"
			GroupBoxTextAndNativeFileNames.Text = "Text and File Names"
			LabelImagePrefix.Visible = False
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

		Dim selectedTextFields As List(Of ViewFieldInfo) = Me.GetRightColumnTextFields
		_textFieldPrecedencePicker.SelectDefaultTextField(If(selectedTextFields.Count > 0, selectedTextFields.First(), Nothing))
	End Sub

	Private Sub _searchList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _filters.SelectedIndexChanged
		If Not _isLoadingExport AndAlso _dataSourceIsSet AndAlso Not _filters.SelectedItem Is Nothing Then Me.InitializeColumnSelecter()
		If _textFieldPrecedencePicker IsNot Nothing Then
			Dim textFields As List(Of ViewFieldInfo) = GetAllLongTextFields()
			textFields.Sort()
			_textFieldPrecedencePicker.AllAvailableLongTextFields = textFields
			Dim selectedTextFields As List(Of ViewFieldInfo) = Me.GetRightColumnTextFields
			_textFieldPrecedencePicker.SelectDefaultTextField(If(selectedTextFields.Count > 0, selectedTextFields.First(), Nothing))
		End If
	End Sub

	Private Sub InitializeFileControls()
		If Me.ExportFile.ArtifactTypeID = Relativity.ArtifactType.Document Then Exit Sub
		_exportImages.Checked = False
		_exportImages.Enabled = False
		GroupBoxImage.Visible = False
		If Me.ExportFile.HasFileField Then
			GroupBoxNative.Text = Me.ExportFile.FileField.FieldName
			_exportNativeFiles.Text = String.Format("Export {0} Files", Me.ExportFile.FileField.FieldName)
		Else
			_exportNativeFiles.Checked = False
			_exportNativeFiles.Enabled = False
			GroupBoxNative.Visible = False
		End If
	End Sub

	Private Sub InitializeColumnSelecter()
		_columnSelector.ClearAll()
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
						_columnSelector.RightListBoxItems.Add(New ViewFieldInfo(field))
						Exit For
					ElseIf field.FieldType <> Relativity.FieldTypeHelper.FieldType.File Then
						_columnSelector.RightListBoxItems.Add(New ViewFieldInfo(field))
						Exit For
					End If
				End If
			Next
		Next
		leftListBoxItems.Sort()
		_columnSelector.LeftListBoxItems.AddRange(leftListBoxItems.ToArray())
		Me.ManagePotentialTextFields()
	End Sub

	Private Sub _exportImages_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _exportImages.CheckedChanged
		_useAbsolutePaths.Enabled = True
		_imageFileFormat.Enabled = _exportImages.Checked
		_imageTypeDropdown.Enabled = _exportImages.Checked And _copyFilesFromRepository.Checked
	End Sub

	Private Sub _exportNativeFiles_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _exportNativeFiles.CheckedChanged
		_useAbsolutePaths.Enabled = True
		_metadataGroupBox.Enabled = _columnSelector.RightListBoxItems.Count > 0 OrElse _exportNativeFiles.Checked
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


	Private Sub _columnSelecter_ItemsShifted() Handles _columnSelector.ItemsShifted
		_metadataGroupBox.Enabled = _columnSelector.RightListBoxItems.Count > 0 OrElse _exportNativeFiles.Checked
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
		Return GetTextFields(_columnSelector.RightListBoxItems.Cast(Of ViewFieldInfo)().ToList())
	End Function

	Private Function GetLeftColumnTextFields() As List(Of ViewFieldInfo)
		Return GetTextFields(_columnSelector.LeftListBoxItems.Cast(Of ViewFieldInfo)().ToList())
	End Function

	Private Function GetTextFields(ByVal unfilteredList As List(Of ViewFieldInfo)) As List(Of ViewFieldInfo)
		Return (From field In unfilteredList Where field.FieldType = Relativity.FieldTypeHelper.FieldType.Text Select field).ToList()
	End Function

	Private Sub RefreshMenu_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles RefreshMenu.Click
		Me.RefreshRelativityInformation()
	End Sub

	Private Sub RefreshRelativityInformation()
		Dim selectedColumns As New System.Collections.ArrayList
		For Each field As kCura.WinEDDS.ViewFieldInfo In _columnSelector.RightListBoxItems
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
		_columnSelector.LeftListBoxItems.Clear()
		_columnSelector.RightListBoxItems.Clear()
		Dim al As New System.Collections.ArrayList(_exportFile.AllExportableFields)
		al.Sort()
		_columnSelector.LeftListBoxItems.AddRange(al.ToArray())
		For Each field As ViewFieldInfo In selectedColumns
			Dim itemToShiftIndex As Int32 = -1
			For i As Int32 = 0 To _columnSelector.LeftListBoxItems.Count - 1
				Dim item As ViewFieldInfo = DirectCast(_columnSelector.LeftListBoxItems(i), ViewFieldInfo)
				If item.AvfId = field.AvfId Then
					itemToShiftIndex = i
					Exit For
				End If
			Next
			If itemToShiftIndex >= 0 Then
				Dim item As ViewFieldInfo = DirectCast(_columnSelector.LeftListBoxItems(itemToShiftIndex), ViewFieldInfo)
				_columnSelector.LeftListBoxItems.Remove(item)
				_columnSelector.RightListBoxItems.Add(item)
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