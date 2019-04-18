Imports kCura.Vendor.Castle.Core.Internal
Imports FileNaming.CustomFileNaming
Imports kCura.WinEDDS
Imports Relativity.Desktop.Client
Imports Relativity.Desktop.Client.Legacy.Controls
Imports Relativity.Import.Export.Services

Public Class ExportForm
	Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

	Public Sub New()
		MyBase.New()

		'This call is required by the Windows Form Designer.
		InitializeComponent()

		'Add any initialization after the InitializeComponent() call
		Me._textFieldPrecedencePicker.SelectedFields = New List(Of kCura.WinEDDS.ViewFieldInfo)
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
	Public WithEvents _columnSelector As TwoListBox
	Public WithEvents _filtersBox As System.Windows.Forms.GroupBox
	Public WithEvents _metadataGroupBox As System.Windows.Forms.GroupBox
	Public WithEvents LabelMetadataDataFileFormat As System.Windows.Forms.Label
	Public WithEvents _nativeFileFormat As System.Windows.Forms.ComboBox
	Public WithEvents _dataFileEncoding As EncodingPicker
	Public WithEvents LabelDataFileEncoding As System.Windows.Forms.Label
	Public WithEvents LabelTextFileEncoding As System.Windows.Forms.Label
	Public WithEvents _textFileEncoding As EncodingPicker
	Public WithEvents _textFieldPrecedencePicker As TextFieldPrecedencePicker
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
	Friend WithEvents _selectFromListButton As Button
	Public WithEvents _textAndNativeFileNamePicker As TextAndNativeFileNamePicker
	Public WithEvents _loadExportSettingsDialog As System.Windows.Forms.OpenFileDialog

	Private Sub InitializeComponent()
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
		Me.LabelNamedAfter = New System.Windows.Forms.Label()
		Me._overwriteCheckBox = New System.Windows.Forms.CheckBox()
		Me._browseButton = New System.Windows.Forms.Button()
		Me._folderPath = New System.Windows.Forms.TextBox()
		Me._appendOriginalFilenameCheckbox = New System.Windows.Forms.CheckBox()
		Me.GroupBoxExportLocation = New System.Windows.Forms.GroupBox()
		Me.TabControl1 = New System.Windows.Forms.TabControl()
		Me._dataSourceTabPage = New System.Windows.Forms.TabPage()
		Me._filtersBox = New System.Windows.Forms.GroupBox()
		Me._selectFromListButton = New System.Windows.Forms.Button()
		Me._startExportAtDocumentNumber = New System.Windows.Forms.NumericUpDown()
		Me.LabelStartAtRecordNumber = New System.Windows.Forms.Label()
		Me.LabelSelectedColumns = New System.Windows.Forms.Label()
		Me._filters = New System.Windows.Forms.ComboBox()
		Me._columnSelector = New TwoListBox()
		Me._destinationFileTabPage = New System.Windows.Forms.TabPage()
		Me.GroupBoxTextAndNativeFileNames = New System.Windows.Forms.GroupBox()
		Me._textAndNativeFileNamePicker = New TextAndNativeFileNamePicker()
		Me._metadataGroupBox = New System.Windows.Forms.GroupBox()
		Me.LabelTextPrecedence = New System.Windows.Forms.Label()
		Me._textFieldPrecedencePicker = New TextFieldPrecedencePicker()
		Me._textFileEncoding = New EncodingPicker()
		Me.LabelTextFileEncoding = New System.Windows.Forms.Label()
		Me.LabelDataFileEncoding = New System.Windows.Forms.Label()
		Me._dataFileEncoding = New EncodingPicker()
		Me.LabelMetadataDataFileFormat = New System.Windows.Forms.Label()
		Me._nativeFileFormat = New System.Windows.Forms.ComboBox()
		Me._exportMulticodeFieldsAsNested = New System.Windows.Forms.CheckBox()
		Me._exportFullTextAsFile = New System.Windows.Forms.CheckBox()
		Me.GroupBoxNative = New System.Windows.Forms.GroupBox()
		Me._exportNativeFiles = New System.Windows.Forms.CheckBox()
		Me.GroupBoxImage = New System.Windows.Forms.GroupBox()
		Me.LabelFileType = New System.Windows.Forms.Label()
		Me._exportImages = New System.Windows.Forms.CheckBox()
		Me._imageFileFormat = New System.Windows.Forms.ComboBox()
		Me.LabelImageDataFileFormat = New System.Windows.Forms.Label()
		Me._imageTypeDropdown = New System.Windows.Forms.ComboBox()
		Me.GroupBoxPhysicalFileExport = New System.Windows.Forms.GroupBox()
		Me._copyFilesFromRepository = New System.Windows.Forms.CheckBox()
		Me._subDirectoryInformationGroupBox = New System.Windows.Forms.GroupBox()
		Me._subdirectoryDigitPadding = New System.Windows.Forms.NumericUpDown()
		Me.LabelSubdirectoryNumberOfDigits = New System.Windows.Forms.Label()
		Me._subdirectoryTextPrefix = New System.Windows.Forms.TextBox()
		Me.LabelTextPrefix = New System.Windows.Forms.Label()
		Me._subDirectoryNativePrefix = New System.Windows.Forms.TextBox()
		Me.LabelNativePrefix = New System.Windows.Forms.Label()
		Me._subDirectoryMaxSize = New System.Windows.Forms.NumericUpDown()
		Me._subdirectoryStartNumber = New System.Windows.Forms.NumericUpDown()
		Me.LabelMaxFiles = New System.Windows.Forms.Label()
		Me.LabelSubdirectoryStartNumber = New System.Windows.Forms.Label()
		Me.LabelImagePrefix = New System.Windows.Forms.Label()
		Me._subdirectoryImagePrefix = New System.Windows.Forms.TextBox()
		Me._volumeInformationGroupBox = New System.Windows.Forms.GroupBox()
		Me._volumeDigitPadding = New System.Windows.Forms.NumericUpDown()
		Me.LabelVolumeNumberOfDigits = New System.Windows.Forms.Label()
		Me._volumeMaxSize = New System.Windows.Forms.NumericUpDown()
		Me._volumeStartNumber = New System.Windows.Forms.NumericUpDown()
		Me.LabelMaxSizeMB = New System.Windows.Forms.Label()
		Me.LabelVolumeStartNumber = New System.Windows.Forms.Label()
		Me.LabelPrefix = New System.Windows.Forms.Label()
		Me._volumePrefix = New System.Windows.Forms.TextBox()
		Me.GroupBoxFilePath = New System.Windows.Forms.GroupBox()
		Me._prefixText = New System.Windows.Forms.TextBox()
		Me._usePrefix = New System.Windows.Forms.RadioButton()
		Me._useAbsolutePaths = New System.Windows.Forms.RadioButton()
		Me._useRelativePaths = New System.Windows.Forms.RadioButton()
		Me._groupBoxLoadFileCharacterInformation = New System.Windows.Forms.GroupBox()
		Me.LabelNestedValue = New System.Windows.Forms.Label()
		Me._nestedValueDelimiter = New System.Windows.Forms.ComboBox()
		Me._multiRecordDelimiter = New System.Windows.Forms.ComboBox()
		Me.LabelMultiValue = New System.Windows.Forms.Label()
		Me.LabelQuote = New System.Windows.Forms.Label()
		Me._quoteDelimiter = New System.Windows.Forms.ComboBox()
		Me.LabelNewline = New System.Windows.Forms.Label()
		Me._newLineDelimiter = New System.Windows.Forms.ComboBox()
		Me.LabelColumn = New System.Windows.Forms.Label()
		Me._recordDelimiter = New System.Windows.Forms.ComboBox()
		Me._saveExportSettingsDialog = New System.Windows.Forms.SaveFileDialog()
		Me._loadExportSettingsDialog = New System.Windows.Forms.OpenFileDialog()
		Me._textAndNativeFileNamePicker = New TextAndNativeFileNamePicker()
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
		Me._productionPrecedenceBox.Location = New System.Drawing.Point(576, 6)
		Me._productionPrecedenceBox.Name = "_productionPrecedenceBox"
		Me._productionPrecedenceBox.Size = New System.Drawing.Size(185, 415)
		Me._productionPrecedenceBox.TabIndex = 16
		Me._productionPrecedenceBox.TabStop = False
		Me._productionPrecedenceBox.Text = "Production Precedence"
		'
		'_productionPrecedenceList
		'
		Me._productionPrecedenceList.IntegralHeight = False
		Me._productionPrecedenceList.Location = New System.Drawing.Point(8, 17)
		Me._productionPrecedenceList.Name = "_productionPrecedenceList"
		Me._productionPrecedenceList.Size = New System.Drawing.Size(142, 390)
		Me._productionPrecedenceList.TabIndex = 2
		'
		'_pickPrecedenceButton
		'
		Me._pickPrecedenceButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me._pickPrecedenceButton.Location = New System.Drawing.Point(152, 387)
		Me._pickPrecedenceButton.Name = "_pickPrecedenceButton"
		Me._pickPrecedenceButton.Size = New System.Drawing.Size(24, 20)
		Me._pickPrecedenceButton.TabIndex = 2
		Me._pickPrecedenceButton.Text = "..."
		'
		'LabelNamedAfter
		'
		Me.LabelNamedAfter.Location = New System.Drawing.Point(20, 18)
		Me.LabelNamedAfter.Name = "LabelNamedAfter"
		Me.LabelNamedAfter.Size = New System.Drawing.Size(96, 21)
		Me.LabelNamedAfter.TabIndex = 30
		Me.LabelNamedAfter.Text = "Named after:"
		Me.LabelNamedAfter.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_overwriteCheckBox
		'
		Me._overwriteCheckBox.Location = New System.Drawing.Point(8, 48)
		Me._overwriteCheckBox.Name = "_overwriteCheckBox"
		Me._overwriteCheckBox.Size = New System.Drawing.Size(100, 16)
		Me._overwriteCheckBox.TabIndex = 3
		Me._overwriteCheckBox.Text = "Overwrite Files"
		'
		'_browseButton
		'
		Me._browseButton.Location = New System.Drawing.Point(388, 20)
		Me._browseButton.Name = "_browseButton"
		Me._browseButton.Size = New System.Drawing.Size(24, 20)
		Me._browseButton.TabIndex = 2
		Me._browseButton.Text = "..."
		'
		'_folderPath
		'
		Me._folderPath.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me._folderPath.Location = New System.Drawing.Point(8, 20)
		Me._folderPath.Name = "_folderPath"
		Me._folderPath.Size = New System.Drawing.Size(380, 20)
		Me._folderPath.TabIndex = 1
		Me._folderPath.Text = "Select a folder ..."
		'
		'_appendOriginalFilenameCheckbox
		'
		Me._appendOriginalFilenameCheckbox.Location = New System.Drawing.Point(12, 44)
		Me._appendOriginalFilenameCheckbox.Name = "_appendOriginalFilenameCheckbox"
		Me._appendOriginalFilenameCheckbox.Size = New System.Drawing.Size(148, 16)
		Me._appendOriginalFilenameCheckbox.TabIndex = 31
		Me._appendOriginalFilenameCheckbox.Text = "Append original filename"
		'
		'GroupBoxExportLocation
		'
		Me.GroupBoxExportLocation.Controls.Add(Me._overwriteCheckBox)
		Me.GroupBoxExportLocation.Controls.Add(Me._browseButton)
		Me.GroupBoxExportLocation.Controls.Add(Me._folderPath)
		Me.GroupBoxExportLocation.Location = New System.Drawing.Point(7, 4)
		Me.GroupBoxExportLocation.Name = "GroupBoxExportLocation"
		Me.GroupBoxExportLocation.Size = New System.Drawing.Size(424, 72)
		Me.GroupBoxExportLocation.TabIndex = 0
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
		Me._dataSourceTabPage.Location = New System.Drawing.Point(4, 22)
		Me._dataSourceTabPage.Name = "_dataSourceTabPage"
		Me._dataSourceTabPage.Size = New System.Drawing.Size(764, 423)
		Me._dataSourceTabPage.TabIndex = 0
		Me._dataSourceTabPage.Text = "Data Source"
		'
		'_filtersBox
		'
		Me._filtersBox.Controls.Add(Me._selectFromListButton)
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
		'_selectFromListButton
		'
		Me._selectFromListButton.Image = CType(resources.GetObject("_selectFromListButton.Image"), System.Drawing.Image)
		Me._selectFromListButton.Location = New System.Drawing.Point(12, 20)
		Me._selectFromListButton.Name = "_selectFromListButton"
		Me._selectFromListButton.Padding = New System.Windows.Forms.Padding(0, 0, 0, 2)
		Me._selectFromListButton.Size = New System.Drawing.Size(21, 21)
		Me._selectFromListButton.TabIndex = 22
		Me._selectFromListButton.Text = " "
		Me._selectFromListButton.UseVisualStyleBackColor = True
		'
		'_startExportAtDocumentNumber
		'
		Me._startExportAtDocumentNumber.Location = New System.Drawing.Point(408, 64)
		Me._startExportAtDocumentNumber.Maximum = New Decimal(New Integer() {10000000, 0, 0, 0})
		Me._startExportAtDocumentNumber.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
		Me._startExportAtDocumentNumber.Name = "_startExportAtDocumentNumber"
		Me._startExportAtDocumentNumber.Size = New System.Drawing.Size(148, 20)
		Me._startExportAtDocumentNumber.TabIndex = 21
		Me._startExportAtDocumentNumber.Value = New Decimal(New Integer() {1, 0, 0, 0})
		'
		'LabelStartAtRecordNumber
		'
		Me.LabelStartAtRecordNumber.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.LabelStartAtRecordNumber.Location = New System.Drawing.Point(393, 48)
		Me.LabelStartAtRecordNumber.Name = "LabelStartAtRecordNumber"
		Me.LabelStartAtRecordNumber.Size = New System.Drawing.Size(160, 16)
		Me.LabelStartAtRecordNumber.TabIndex = 20
		Me.LabelStartAtRecordNumber.Text = "Start Export at Record #"
		Me.LabelStartAtRecordNumber.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'LabelSelectedColumns
		'
		Me.LabelSelectedColumns.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.LabelSelectedColumns.Location = New System.Drawing.Point(200, 48)
		Me.LabelSelectedColumns.Name = "LabelSelectedColumns"
		Me.LabelSelectedColumns.Size = New System.Drawing.Size(160, 16)
		Me.LabelSelectedColumns.TabIndex = 19
		Me.LabelSelectedColumns.Text = "Selected Columns"
		Me.LabelSelectedColumns.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
		'
		'_filters
		'
		Me._filters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._filters.ItemHeight = 13
		Me._filters.Location = New System.Drawing.Point(35, 20)
		Me._filters.Name = "_filters"
		Me._filters.Size = New System.Drawing.Size(524, 21)
		Me._filters.TabIndex = 1
		'
		'_columnSelector
		'
		Me._columnSelector.AlternateRowColors = True
		Me._columnSelector.KeepButtonsCentered = False
		Me._columnSelector.LeftOrderControlsVisible = False
		Me._columnSelector.Location = New System.Drawing.Point(12, 64)
		Me._columnSelector.Name = "_columnSelector"
		Me._columnSelector.OuterBox = ListBoxLocation.Left
		Me._columnSelector.RightOrderControlVisible = False
		Me._columnSelector.Size = New System.Drawing.Size(366, 343)
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
		Me._destinationFileTabPage.Location = New System.Drawing.Point(4, 22)
		Me._destinationFileTabPage.Name = "_destinationFileTabPage"
		Me._destinationFileTabPage.Size = New System.Drawing.Size(764, 423)
		Me._destinationFileTabPage.TabIndex = 1
		Me._destinationFileTabPage.Text = "Destination Files"
		'
		'GroupBoxTextAndNativeFileNames
		'
		Me.GroupBoxTextAndNativeFileNames.Controls.Add(Me._textAndNativeFileNamePicker)
		Me.GroupBoxTextAndNativeFileNames.Controls.Add(Me._appendOriginalFilenameCheckbox)
		Me.GroupBoxTextAndNativeFileNames.Controls.Add(Me.LabelNamedAfter)
		Me.GroupBoxTextAndNativeFileNames.Location = New System.Drawing.Point(435, 4)
		Me.GroupBoxTextAndNativeFileNames.Name = "GroupBoxTextAndNativeFileNames"
		Me.GroupBoxTextAndNativeFileNames.Size = New System.Drawing.Size(324, 68)
		Me.GroupBoxTextAndNativeFileNames.TabIndex = 29
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
		Me._metadataGroupBox.TabIndex = 39
		Me._metadataGroupBox.TabStop = False
		Me._metadataGroupBox.Text = "Metadata"
		'
		'LabelTextPrecedence
		'
		Me.LabelTextPrecedence.Location = New System.Drawing.Point(8, 125)
		Me.LabelTextPrecedence.Name = "LabelTextPrecedence"
		Me.LabelTextPrecedence.Size = New System.Drawing.Size(107, 21)
		Me.LabelTextPrecedence.TabIndex = 44
		Me.LabelTextPrecedence.Text = "Text Precedence:"
		Me.LabelTextPrecedence.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_textFieldPrecedencePicker
		'
		Me._textFieldPrecedencePicker.Location = New System.Drawing.Point(116, 125)
		Me._textFieldPrecedencePicker.Name = "_textFieldPrecedencePicker"
		Me._textFieldPrecedencePicker.SelectedFields = CType(resources.GetObject("_textFieldPrecedencePicker.SelectedFields"), System.Collections.Generic.List(Of kCura.WinEDDS.ViewFieldInfo))
		Me._textFieldPrecedencePicker.Size = New System.Drawing.Size(175, 21)
		Me._textFieldPrecedencePicker.TabIndex = 44
		'
		'_textFileEncoding
		'
		Me._textFileEncoding.Location = New System.Drawing.Point(116, 100)
		Me._textFileEncoding.Name = "_textFileEncoding"
		Me._textFileEncoding.SelectedEncoding = Nothing
		Me._textFileEncoding.Size = New System.Drawing.Size(200, 21)
		Me._textFileEncoding.TabIndex = 43
		'
		'LabelTextFileEncoding
		'
		Me.LabelTextFileEncoding.Location = New System.Drawing.Point(12, 100)
		Me.LabelTextFileEncoding.Name = "LabelTextFileEncoding"
		Me.LabelTextFileEncoding.Size = New System.Drawing.Size(104, 21)
		Me.LabelTextFileEncoding.TabIndex = 43
		Me.LabelTextFileEncoding.Text = "Text File Encoding:"
		Me.LabelTextFileEncoding.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'LabelDataFileEncoding
		'
		Me.LabelDataFileEncoding.Location = New System.Drawing.Point(12, 48)
		Me.LabelDataFileEncoding.Name = "LabelDataFileEncoding"
		Me.LabelDataFileEncoding.Size = New System.Drawing.Size(104, 21)
		Me.LabelDataFileEncoding.TabIndex = 41
		Me.LabelDataFileEncoding.Text = "Data File Encoding:"
		Me.LabelDataFileEncoding.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_dataFileEncoding
		'
		Me._dataFileEncoding.Location = New System.Drawing.Point(116, 48)
		Me._dataFileEncoding.Name = "_dataFileEncoding"
		Me._dataFileEncoding.SelectedEncoding = Nothing
		Me._dataFileEncoding.Size = New System.Drawing.Size(200, 21)
		Me._dataFileEncoding.TabIndex = 41
		'
		'LabelMetadataDataFileFormat
		'
		Me.LabelMetadataDataFileFormat.Location = New System.Drawing.Point(24, 20)
		Me.LabelMetadataDataFileFormat.Name = "LabelMetadataDataFileFormat"
		Me.LabelMetadataDataFileFormat.Size = New System.Drawing.Size(92, 21)
		Me.LabelMetadataDataFileFormat.TabIndex = 40
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
		Me._nativeFileFormat.TabIndex = 40
		'
		'_exportMulticodeFieldsAsNested
		'
		Me._exportMulticodeFieldsAsNested.Location = New System.Drawing.Point(12, 156)
		Me._exportMulticodeFieldsAsNested.Name = "_exportMulticodeFieldsAsNested"
		Me._exportMulticodeFieldsAsNested.Size = New System.Drawing.Size(228, 20)
		Me._exportMulticodeFieldsAsNested.TabIndex = 45
		Me._exportMulticodeFieldsAsNested.Text = "Export Multiple Choice Fields as Nested"
		'
		'_exportFullTextAsFile
		'
		Me._exportFullTextAsFile.Location = New System.Drawing.Point(12, 76)
		Me._exportFullTextAsFile.Name = "_exportFullTextAsFile"
		Me._exportFullTextAsFile.Size = New System.Drawing.Size(196, 20)
		Me._exportFullTextAsFile.TabIndex = 42
		Me._exportFullTextAsFile.Text = "Export Text Field as Files"
		'
		'GroupBoxNative
		'
		Me.GroupBoxNative.Controls.Add(Me._exportNativeFiles)
		Me.GroupBoxNative.Location = New System.Drawing.Point(435, 184)
		Me.GroupBoxNative.Name = "GroupBoxNative"
		Me.GroupBoxNative.Size = New System.Drawing.Size(324, 48)
		Me.GroupBoxNative.TabIndex = 36
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
		Me._exportNativeFiles.TabIndex = 37
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
		Me.GroupBoxImage.TabIndex = 32
		Me.GroupBoxImage.TabStop = False
		Me.GroupBoxImage.Text = "Image "
		'
		'LabelFileType
		'
		Me.LabelFileType.Location = New System.Drawing.Point(24, 76)
		Me.LabelFileType.Name = "LabelFileType"
		Me.LabelFileType.Size = New System.Drawing.Size(92, 21)
		Me.LabelFileType.TabIndex = 35
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
		Me._exportImages.TabIndex = 33
		Me._exportImages.Text = "Export Images"
		'
		'_imageFileFormat
		'
		Me._imageFileFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._imageFileFormat.DropDownWidth = 150
		Me._imageFileFormat.Location = New System.Drawing.Point(116, 48)
		Me._imageFileFormat.Name = "_imageFileFormat"
		Me._imageFileFormat.Size = New System.Drawing.Size(176, 21)
		Me._imageFileFormat.TabIndex = 34
		'
		'LabelImageDataFileFormat
		'
		Me.LabelImageDataFileFormat.Location = New System.Drawing.Point(24, 48)
		Me.LabelImageDataFileFormat.Name = "LabelImageDataFileFormat"
		Me.LabelImageDataFileFormat.Size = New System.Drawing.Size(92, 21)
		Me.LabelImageDataFileFormat.TabIndex = 34
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
		Me._imageTypeDropdown.TabIndex = 35
		'
		'GroupBoxPhysicalFileExport
		'
		Me.GroupBoxPhysicalFileExport.Controls.Add(Me._copyFilesFromRepository)
		Me.GroupBoxPhysicalFileExport.Location = New System.Drawing.Point(7, 84)
		Me.GroupBoxPhysicalFileExport.Name = "GroupBoxPhysicalFileExport"
		Me.GroupBoxPhysicalFileExport.Size = New System.Drawing.Size(212, 48)
		Me.GroupBoxPhysicalFileExport.TabIndex = 4
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
		Me._copyFilesFromRepository.TabIndex = 5
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
		Me._subDirectoryInformationGroupBox.TabIndex = 11
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
		Me._subdirectoryDigitPadding.TabIndex = 4
		Me._subdirectoryDigitPadding.Value = New Decimal(New Integer() {1, 0, 0, 0})
		'
		'LabelSubdirectoryNumberOfDigits
		'
		Me.LabelSubdirectoryNumberOfDigits.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.LabelSubdirectoryNumberOfDigits.Location = New System.Drawing.Point(104, 104)
		Me.LabelSubdirectoryNumberOfDigits.Name = "LabelSubdirectoryNumberOfDigits"
		Me.LabelSubdirectoryNumberOfDigits.Size = New System.Drawing.Size(56, 20)
		Me.LabelSubdirectoryNumberOfDigits.TabIndex = 9
		Me.LabelSubdirectoryNumberOfDigits.Text = "# of digits:"
		Me.LabelSubdirectoryNumberOfDigits.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_subdirectoryTextPrefix
		'
		Me._subdirectoryTextPrefix.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me._subdirectoryTextPrefix.Location = New System.Drawing.Point(116, 76)
		Me._subdirectoryTextPrefix.Name = "_subdirectoryTextPrefix"
		Me._subdirectoryTextPrefix.Size = New System.Drawing.Size(88, 20)
		Me._subdirectoryTextPrefix.TabIndex = 2
		Me._subdirectoryTextPrefix.Text = "TEXT"
		'
		'LabelTextPrefix
		'
		Me.LabelTextPrefix.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.LabelTextPrefix.Location = New System.Drawing.Point(36, 76)
		Me.LabelTextPrefix.Name = "LabelTextPrefix"
		Me.LabelTextPrefix.Size = New System.Drawing.Size(80, 20)
		Me.LabelTextPrefix.TabIndex = 14
		Me.LabelTextPrefix.Text = "Text Prefix: "
		Me.LabelTextPrefix.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_subDirectoryNativePrefix
		'
		Me._subDirectoryNativePrefix.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me._subDirectoryNativePrefix.Location = New System.Drawing.Point(116, 48)
		Me._subDirectoryNativePrefix.Name = "_subDirectoryNativePrefix"
		Me._subDirectoryNativePrefix.Size = New System.Drawing.Size(88, 20)
		Me._subDirectoryNativePrefix.TabIndex = 1
		Me._subDirectoryNativePrefix.Text = "NATIVE"
		'
		'LabelNativePrefix
		'
		Me.LabelNativePrefix.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.LabelNativePrefix.Location = New System.Drawing.Point(44, 48)
		Me.LabelNativePrefix.Name = "LabelNativePrefix"
		Me.LabelNativePrefix.Size = New System.Drawing.Size(72, 20)
		Me.LabelNativePrefix.TabIndex = 13
		Me.LabelNativePrefix.Text = "Native Prefix: "
		Me.LabelNativePrefix.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_subDirectoryMaxSize
		'
		Me._subDirectoryMaxSize.Location = New System.Drawing.Point(116, 132)
		Me._subDirectoryMaxSize.Maximum = New Decimal(New Integer() {2000000, 0, 0, 0})
		Me._subDirectoryMaxSize.Name = "_subDirectoryMaxSize"
		Me._subDirectoryMaxSize.Size = New System.Drawing.Size(88, 20)
		Me._subDirectoryMaxSize.TabIndex = 5
		Me._subDirectoryMaxSize.Value = New Decimal(New Integer() {500, 0, 0, 0})
		'
		'_subdirectoryStartNumber
		'
		Me._subdirectoryStartNumber.Location = New System.Drawing.Point(56, 104)
		Me._subdirectoryStartNumber.Maximum = New Decimal(New Integer() {2000000, 0, 0, 0})
		Me._subdirectoryStartNumber.Name = "_subdirectoryStartNumber"
		Me._subdirectoryStartNumber.Size = New System.Drawing.Size(44, 20)
		Me._subdirectoryStartNumber.TabIndex = 3
		Me._subdirectoryStartNumber.Value = New Decimal(New Integer() {1, 0, 0, 0})
		'
		'LabelMaxFiles
		'
		Me.LabelMaxFiles.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.LabelMaxFiles.Location = New System.Drawing.Point(56, 132)
		Me.LabelMaxFiles.Name = "LabelMaxFiles"
		Me.LabelMaxFiles.Size = New System.Drawing.Size(60, 20)
		Me.LabelMaxFiles.TabIndex = 17
		Me.LabelMaxFiles.Text = "Max Files:"
		Me.LabelMaxFiles.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'LabelSubdirectoryStartNumber
		'
		Me.LabelSubdirectoryStartNumber.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.LabelSubdirectoryStartNumber.Location = New System.Drawing.Point(8, 104)
		Me.LabelSubdirectoryStartNumber.Name = "LabelSubdirectoryStartNumber"
		Me.LabelSubdirectoryStartNumber.Size = New System.Drawing.Size(44, 20)
		Me.LabelSubdirectoryStartNumber.TabIndex = 15
		Me.LabelSubdirectoryStartNumber.Text = "Start #:"
		Me.LabelSubdirectoryStartNumber.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'LabelImagePrefix
		'
		Me.LabelImagePrefix.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.LabelImagePrefix.Location = New System.Drawing.Point(44, 20)
		Me.LabelImagePrefix.Name = "LabelImagePrefix"
		Me.LabelImagePrefix.Size = New System.Drawing.Size(72, 20)
		Me.LabelImagePrefix.TabIndex = 12
		Me.LabelImagePrefix.Text = "Image Prefix: "
		Me.LabelImagePrefix.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_subdirectoryImagePrefix
		'
		Me._subdirectoryImagePrefix.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me._subdirectoryImagePrefix.Location = New System.Drawing.Point(116, 20)
		Me._subdirectoryImagePrefix.Name = "_subdirectoryImagePrefix"
		Me._subdirectoryImagePrefix.Size = New System.Drawing.Size(88, 20)
		Me._subdirectoryImagePrefix.TabIndex = 0
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
		Me._volumeInformationGroupBox.TabIndex = 6
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
		Me._volumeDigitPadding.TabIndex = 2
		Me._volumeDigitPadding.Value = New Decimal(New Integer() {1, 0, 0, 0})
		'
		'LabelVolumeNumberOfDigits
		'
		Me.LabelVolumeNumberOfDigits.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.LabelVolumeNumberOfDigits.Location = New System.Drawing.Point(104, 48)
		Me.LabelVolumeNumberOfDigits.Name = "LabelVolumeNumberOfDigits"
		Me.LabelVolumeNumberOfDigits.Size = New System.Drawing.Size(56, 20)
		Me.LabelVolumeNumberOfDigits.TabIndex = 16
		Me.LabelVolumeNumberOfDigits.Text = "# of digits:"
		Me.LabelVolumeNumberOfDigits.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'_volumeMaxSize
		'
		Me._volumeMaxSize.Location = New System.Drawing.Point(116, 76)
		Me._volumeMaxSize.Maximum = New Decimal(New Integer() {2000000, 0, 0, 0})
		Me._volumeMaxSize.Name = "_volumeMaxSize"
		Me._volumeMaxSize.Size = New System.Drawing.Size(88, 20)
		Me._volumeMaxSize.TabIndex = 3
		Me._volumeMaxSize.Value = New Decimal(New Integer() {650, 0, 0, 0})
		'
		'_volumeStartNumber
		'
		Me._volumeStartNumber.Location = New System.Drawing.Point(52, 48)
		Me._volumeStartNumber.Maximum = New Decimal(New Integer() {2000000, 0, 0, 0})
		Me._volumeStartNumber.Name = "_volumeStartNumber"
		Me._volumeStartNumber.Size = New System.Drawing.Size(44, 20)
		Me._volumeStartNumber.TabIndex = 1
		Me._volumeStartNumber.Value = New Decimal(New Integer() {1, 0, 0, 0})
		'
		'LabelMaxSizeMB
		'
		Me.LabelMaxSizeMB.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.LabelMaxSizeMB.Location = New System.Drawing.Point(32, 76)
		Me.LabelMaxSizeMB.Name = "LabelMaxSizeMB"
		Me.LabelMaxSizeMB.Size = New System.Drawing.Size(82, 20)
		Me.LabelMaxSizeMB.TabIndex = 10
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
		Me._volumePrefix.TabIndex = 0
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
		Me.GroupBoxFilePath.TabIndex = 18
		Me.GroupBoxFilePath.TabStop = False
		Me.GroupBoxFilePath.Text = "File Path"
		'
		'_prefixText
		'
		Me._prefixText.Enabled = False
		Me._prefixText.Location = New System.Drawing.Point(88, 80)
		Me._prefixText.Name = "_prefixText"
		Me._prefixText.Size = New System.Drawing.Size(112, 20)
		Me._prefixText.TabIndex = 22
		'
		'_usePrefix
		'
		Me._usePrefix.Location = New System.Drawing.Point(12, 84)
		Me._usePrefix.Name = "_usePrefix"
		Me._usePrefix.Size = New System.Drawing.Size(76, 16)
		Me._usePrefix.TabIndex = 21
		Me._usePrefix.Text = "Use Prefix"
		'
		'_useAbsolutePaths
		'
		Me._useAbsolutePaths.Location = New System.Drawing.Point(12, 52)
		Me._useAbsolutePaths.Name = "_useAbsolutePaths"
		Me._useAbsolutePaths.Size = New System.Drawing.Size(124, 16)
		Me._useAbsolutePaths.TabIndex = 20
		Me._useAbsolutePaths.Text = "Use Absolute Paths"
		'
		'_useRelativePaths
		'
		Me._useRelativePaths.Checked = True
		Me._useRelativePaths.Location = New System.Drawing.Point(12, 20)
		Me._useRelativePaths.Name = "_useRelativePaths"
		Me._useRelativePaths.Size = New System.Drawing.Size(124, 16)
		Me._useRelativePaths.TabIndex = 19
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
		Me._groupBoxLoadFileCharacterInformation.TabIndex = 23
		Me._groupBoxLoadFileCharacterInformation.TabStop = False
		Me._groupBoxLoadFileCharacterInformation.Text = "Load File Characters"
		'
		'LabelNestedValue
		'
		Me.LabelNestedValue.Location = New System.Drawing.Point(8, 168)
		Me.LabelNestedValue.Name = "LabelNestedValue"
		Me.LabelNestedValue.Size = New System.Drawing.Size(76, 21)
		Me.LabelNestedValue.TabIndex = 28
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
		Me._nestedValueDelimiter.TabIndex = 28
		'
		'_multiRecordDelimiter
		'
		Me._multiRecordDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._multiRecordDelimiter.Enabled = False
		Me._multiRecordDelimiter.Location = New System.Drawing.Point(84, 132)
		Me._multiRecordDelimiter.Name = "_multiRecordDelimiter"
		Me._multiRecordDelimiter.Size = New System.Drawing.Size(116, 21)
		Me._multiRecordDelimiter.TabIndex = 27
		'
		'LabelMultiValue
		'
		Me.LabelMultiValue.Location = New System.Drawing.Point(8, 132)
		Me.LabelMultiValue.Name = "LabelMultiValue"
		Me.LabelMultiValue.Size = New System.Drawing.Size(76, 21)
		Me.LabelMultiValue.TabIndex = 27
		Me.LabelMultiValue.Text = "Multi-Value:"
		Me.LabelMultiValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'LabelQuote
		'
		Me.LabelQuote.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.LabelQuote.Location = New System.Drawing.Point(8, 60)
		Me.LabelQuote.Name = "LabelQuote"
		Me.LabelQuote.Size = New System.Drawing.Size(76, 21)
		Me.LabelQuote.TabIndex = 25
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
		Me._quoteDelimiter.TabIndex = 25
		'
		'LabelNewline
		'
		Me.LabelNewline.Location = New System.Drawing.Point(8, 96)
		Me.LabelNewline.Name = "LabelNewline"
		Me.LabelNewline.Size = New System.Drawing.Size(76, 21)
		Me.LabelNewline.TabIndex = 26
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
		Me._newLineDelimiter.TabIndex = 26
		'
		'LabelColumn
		'
		Me.LabelColumn.Location = New System.Drawing.Point(8, 24)
		Me.LabelColumn.Name = "LabelColumn"
		Me.LabelColumn.Size = New System.Drawing.Size(76, 21)
		Me.LabelColumn.TabIndex = 24
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
		Me._recordDelimiter.TabIndex = 24
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
		'_textAndNativeFileNamePicker
		'
		Me._textAndNativeFileNamePicker.Location = New System.Drawing.Point(116, 18)
		Me._textAndNativeFileNamePicker.Name = "_textAndNativeFileNamePicker"
		Me._textAndNativeFileNamePicker.Size = New System.Drawing.Size(176, 21)
		Me._textAndNativeFileNamePicker.TabIndex = 32
		'
		'ExportForm
		'
		Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
		Me.BackColor = System.Drawing.SystemColors.Control
		Me.ClientSize = New System.Drawing.Size(775, 452)
		Me.Controls.Add(Me.TabControl1)
		Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
		Me.Menu = Me.MainMenu1
		Me.MinimumSize = New System.Drawing.Size(700, 400)
		Me.Name = "ExportForm"
		Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
		Me.Text = "Relativity Desktop Client | Export "
		Me._productionPrecedenceBox.ResumeLayout(False)
		Me.GroupBoxExportLocation.ResumeLayout(False)
		Me.GroupBoxExportLocation.PerformLayout()
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
		Me._subDirectoryInformationGroupBox.PerformLayout()
		CType(Me._subdirectoryDigitPadding, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me._subDirectoryMaxSize, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me._subdirectoryStartNumber, System.ComponentModel.ISupportInitialize).EndInit()
		Me._volumeInformationGroupBox.ResumeLayout(False)
		Me._volumeInformationGroupBox.PerformLayout()
		CType(Me._volumeDigitPadding, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me._volumeMaxSize, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me._volumeStartNumber, System.ComponentModel.ISupportInitialize).EndInit()
		Me.GroupBoxFilePath.ResumeLayout(False)
		Me.GroupBoxFilePath.PerformLayout()
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
		Dim absoluteListBoxLoc As Point = _columnSelector.RightSearchableList.PointToScreen(New Point(0, 0))
		'Convert to a location relative to the Views group (_filtersBox)
		Dim relativeListBoxLoc As Point = Me.LabelSelectedColumns.Parent.PointToClient(absoluteListBoxLoc)
		'Adjust the location of the label
		Me.LabelSelectedColumns.Left = relativeListBoxLoc.X
	End Sub
#End Region

	Private WithEvents _application As Application
	Protected _exportFile As kCura.WinEDDS.ExportFile
	Protected WithEvents _precedenceForm As ProductionPrecedenceForm
	Protected WithEvents _textFieldPrecedenceForm As TextPrecedenceForm
	Private _allExportableFields As kCura.WinEDDS.ViewFieldInfo
	Private _dataSourceIsSet As Boolean = False
	Private _objectTypeName As String = ""
	Private _isLoadingExport As Boolean = False
	Private _masterDT As DataTable


	Public Property Application() As Application
		Get
			Return _application
		End Get
		Set(ByVal value As Application)
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

	Public Async Function GetObjectTypeName() As Task(Of String)
		If _objectTypeName = "" Then
			For Each row As System.Data.DataRow In New kCura.WinEDDS.Service.ObjectTypeManager(Await _application.GetCredentialsAsync(), _application.CookieContainer).RetrieveAllUploadable(_application.SelectedCaseInfo.ArtifactID).Tables(0).Rows
				If CType(row("DescriptorArtifactTypeID"), Int32) = Me.ExportFile.ArtifactTypeID Then
					_objectTypeName = row("Name").ToString
				End If
			Next
		End If
		Return _objectTypeName
	End Function

	Private Sub AppendErrorMessage(ByVal msg As System.Text.StringBuilder, ByVal errorText As String)
		msg.Append(" - ").Append(errorText).Append(vbNewLine)
	End Sub

	Private Async Sub SaveExportSettings_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SaveExportSettings.Click
		Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
		Try
			If Await PopulateExportFile(Me, False) Then
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
		If _exportNativeFiles.Checked OrElse _columnSelector.RightSearchableListItems.Count > 0 Then
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
		If _textAndNativeFileNamePicker.SelectedItem = TextAndNativeFileNamePicker.SelectOption Then
			AppendErrorMessage(msg, "No file naming method selected")
		End If
		If _textAndNativeFileNamePicker.SelectedItem = TextAndNativeFileNamePicker.CustomOption Then
			ValidateCustomFileNamingOptions(msg)
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

	Private Sub ValidateCustomFileNamingOptions(msg As Text.StringBuilder)
		ValidateCustomFileNamingFieldsAreSelected(msg)
		ValidateCustomFileNamingProductionPrecedenceIsSelected(msg)
	End Sub

	Private Sub ValidateCustomFileNamingFieldsAreSelected(msg As Text.StringBuilder)
		If _textAndNativeFileNamePicker.Selection Is Nothing Then
			AppendErrorMessage(msg, "No custom file naming fields selected")
		End If
	End Sub

	Private Sub ValidateCustomFileNamingProductionPrecedenceIsSelected(msg As Text.StringBuilder)
		Dim isProductionBegBatesSelected As Boolean? = _textAndNativeFileNamePicker.Selection?.FirstOrDefault()?.IsProductionBegBates
		If Not isProductionBegBatesSelected Then
			Return
		End If

		If _exportFile.TypeOfExport = ExportFile.ExportType.Production Then
			Return
		End If

		If _productionPrecedenceList.Items.Count <> 1 Then
			Return
		End If

		Dim precedence = DirectCast(_productionPrecedenceList.Items(0), Pair)
		If precedence.Value.Equals("-1") Then
			AppendErrorMessage(msg, "No production precedence selected for custom file naming")
		End If
	End Sub

	Public Async Function PopulateExportFile(ByVal abstractExportForm As ExportForm, ByVal validateForm As Boolean) As Task(Of Boolean)
		Dim d As DocumentFieldCollection = Await _application.CurrentFields(_exportFile.ArtifactTypeID, True)
		_exportFile.ObjectTypeName = Await _application.GetObjectTypeName(_exportFile.ArtifactTypeID)
		If validateForm AndAlso Not Me.IsValid(abstractExportForm) Then Return False
		If Not Await _application.IsConnected() Then Return False
		_exportFile.FolderPath = _folderPath.Text
		Select Case Me.ExportFile.TypeOfExport
			Case ExportFile.ExportType.AncestorSearch
				_exportFile.ViewID = CType(_filters.SelectedValue, Int32)
				_exportFile.LoadFilesPrefix = DirectCast(_filters.SelectedItem, System.Data.DataRowView)(_filters.DisplayMember).ToString
			Case ExportFile.ExportType.ArtifactSearch
				_exportFile.ArtifactID = CType(_filters.SelectedValue, Int32)
				If Not Await _application.IsAssociatedSearchProviderAccessible(_exportFile.CaseArtifactID, _exportFile.ArtifactID) Then
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
		End Select
		If _textAndNativeFileNamePicker.SelectedItem.ToString = TextAndNativeFileNamePicker.IdentifierOption Then
			_exportFile.ExportNativesToFileNamedFrom = ExportNativeWithFilenameFrom.Identifier
		ElseIf _textAndNativeFileNamePicker.SelectedItem.ToString = TextAndNativeFileNamePicker.ProductionOption Then
			_exportFile.ExportNativesToFileNamedFrom = ExportNativeWithFilenameFrom.Production
		Else
			_exportFile.ExportNativesToFileNamedFrom = ExportNativeWithFilenameFrom.Custom
			PopulateCustomFileNamingField()
		End If
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
		For Each field As kCura.WinEDDS.ViewFieldInfo In _columnSelector.RightSearchableListItems
			selectedViewFields.Add(field)
		Next
		_exportFile.SelectedViewFields = DirectCast(selectedViewFields.ToArray(GetType(kCura.WinEDDS.ViewFieldInfo)), kCura.WinEDDS.ViewFieldInfo())
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

	Private Sub PopulateCustomFileNamingField()
		Dim selection = _textAndNativeFileNamePicker.Selection
		If selection Is Nothing Then
			Return
		End If
		Dim firstField = New FirstFieldDescriptorPart(selection(0).FieldID, selection(0).IsProductionBegBates)
		Dim secondField As ExtendedDescriptorPart = Nothing
		Dim thirdField As ExtendedDescriptorPart = Nothing
		If selection.Count >= 2 Then
			secondField = GetDescriptorPartFromSelectionPart(selection(1))
			If selection.Count >= 3 Then
				thirdField = GetDescriptorPartFromSelectionPart(selection(2))
			End If
		End If
		Dim descriptorModel = New CustomFileNameDescriptorModel(firstField, secondField, thirdField)
		_exportFile.UseCustomFileNaming = True
		_exportFile.CustomFileNaming = descriptorModel
	End Sub

	Private Function GetDescriptorPartFromSelectionPart(selectionPart As CustomFileNameSelectionPart) As ExtendedDescriptorPart
		Dim separatorPart = New SeparatorDescriptorPart(selectionPart.Separator)
		If selectionPart.HasCustomText() Then
			Dim customTextDescriptorPart = New CustomTextDescriptorPart(selectionPart.CustomText)
			Return New ExtendedDescriptorPart(separatorPart, customTextDescriptorPart)
		Else
			Dim fieldDescriptorPart = New FieldDescriptorPart(selectionPart.FieldID)
			Return New ExtendedDescriptorPart(separatorPart, fieldDescriptorPart)
		End If
	End Function

	Private Async Sub LoadExportSettings_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LoadExportSettings.Click
		If _loadExportSettingsDialog.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
			Dim settings As String = Relativity.Import.Export.Io.FileSystem.Instance.File.ReadAllText(_loadExportSettingsDialog.FileName)
			Dim newFile As ExtendedExportFile = New kCura.WinEDDS.ExportFileSerializer().DeserializeExportFile(_exportFile, settings)
			If TypeOf newFile Is kCura.WinEDDS.ErrorExportFile Then
				MsgBox(DirectCast(newFile, kCura.WinEDDS.ErrorExportFile).ErrorMessage, MsgBoxStyle.Exclamation)
			Else
				Dim exportFilterSelectionForm As New ExportFilterSelectForm(newFile.ViewID, ExportTypeStringName, DirectCast(_filters.DataSource, DataTable))
				exportFilterSelectionForm.ShowDialog()
				If exportFilterSelectionForm.DialogResult = DialogResult.OK Then
					If exportFilterSelectionForm.SelectedItemArtifactIDs IsNot Nothing Then
						_filters.SelectedValue = exportFilterSelectionForm.SelectedItemArtifactIDs(0)
					End If
					Await LoadExportFile(newFile)
					_exportFile = newFile
				End If

				_columnSelector.EnsureHorizontalScrollbars()
			End If
		End If
	End Sub

	''' <summary>
	''' Select view field.
	''' Move view field from left listbox to right listbox.
	''' </summary>
	''' <param name="listboxViewField">listbox view field to be selected</param>
	Public Sub SelectField(listboxViewField As Object)
		_columnSelector.RightSearchableList.AddField(listboxViewField)
		_columnSelector.LeftSearchableList.RemoveField(listboxViewField)
	End Sub

	Public Async Function LoadExportFile(ByVal ef As kCura.WinEDDS.ExportFile) As Task
		_isLoadingExport = True
		If _exportNativeFiles.Checked <> ef.ExportNative Then _exportNativeFiles.Checked = ef.ExportNative
		If _exportImages.Checked <> ef.ExportImages Then _exportImages.Checked = ef.ExportImages
		If _overwriteCheckBox.Checked <> ef.Overwrite Then _overwriteCheckBox.Checked = ef.Overwrite
		If _folderPath.Text <> ef.FolderPath Then _folderPath.Text = ef.FolderPath
		If ef.VolumeDigitPadding >= _volumeDigitPadding.Minimum AndAlso ef.VolumeDigitPadding <= _volumeDigitPadding.Maximum Then
			If _volumeDigitPadding.Value <> ef.VolumeDigitPadding Then _volumeDigitPadding.Value = ef.VolumeDigitPadding
		End If
		If ef.VolumeInfo IsNot Nothing Then
			Dim copyFilesFromRepo As Boolean = ef.VolumeInfo.CopyNativeFilesFromRepository OrElse ef.VolumeInfo.CopyImageFilesFromRepository
			If _copyFilesFromRepository.Checked <> copyFilesFromRepo Then _copyFilesFromRepository.Checked = copyFilesFromRepo
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
			Case ExportNativeWithFilenameFrom.Identifier
				_textAndNativeFileNamePicker.SelectedItem = TextAndNativeFileNamePicker.IdentifierOption
			Case ExportNativeWithFilenameFrom.Production
				_textAndNativeFileNamePicker.SelectedItem = TextAndNativeFileNamePicker.ProductionOption
			Case ExportNativeWithFilenameFrom.Select
				_textAndNativeFileNamePicker.SelectedItem = TextAndNativeFileNamePicker.SelectOption
			Case ExportNativeWithFilenameFrom.Custom
				_textAndNativeFileNamePicker.SelectedItem = TextAndNativeFileNamePicker.CustomOption
		End Select

		If ef.UseCustomFileNaming Then
			LoadCustomFileNamingField(ef.CustomFileNaming)
		End If

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
			_columnSelector.ClearSelection(ListBoxLocation.Left)
			_columnSelector.LeftSearchableList.ClearListBox()
			Array.Sort(ef.AllExportableFields)
			_columnSelector.LeftSearchableList.AddFields(ef.AllExportableFields)
		End If

		If ef.SelectedViewFields IsNot Nothing Then
			_columnSelector.ClearSelection(ListBoxLocation.Right)
			_columnSelector.RightSearchableList.ClearListBox()

			ef.SelectedViewFields _
				.SelectMany(Function(x) Relativity.Desktop.Client.Utility.FindCounterpartField(_columnSelector.LeftSearchableListItems, x)) _
				.ForEach(AddressOf SelectField)

			If ef.AllExportableFields IsNot Nothing Then
				Dim defaultSelectedIds As New System.Collections.ArrayList
				If _columnSelector.RightSearchableListItems.Count = 0 Then
					_metadataGroupBox.Enabled = False
					If Not _filters.SelectedItem Is Nothing Then defaultSelectedIds = DirectCast(Me.ExportFile.ArtifactAvfLookup(CType(_filters.SelectedValue, Int32)), ArrayList)
					For Each defaultSelectedId As Int32 In defaultSelectedIds
						For Each field As kCura.WinEDDS.ViewFieldInfo In ef.AllExportableFields
							If field.AvfId = defaultSelectedId Then
								Dim avfNumber = field.AvfId
								Dim found As Boolean = ef.SelectedViewFields.Any(Function(addedItem) avfNumber = addedItem.AvfId)
								If Not found Then
									If Me.ExportFile.ArtifactTypeID = ArtifactType.Document Then
										_columnSelector.RightSearchableList.AddField(New kCura.WinEDDS.ViewFieldInfo(field))
										Exit For
									ElseIf field.FieldType <> FieldType.File Then
										_columnSelector.RightSearchableList.AddField(New kCura.WinEDDS.ViewFieldInfo(field))
										Exit For
									End If
								End If
							End If
						Next
					Next
				End If
			End If
		End If

		ManagePotentialTextFields()

		_textFieldPrecedencePicker.LoadNewSelectedFields(ef.SelectedTextFields)

		Dim trueStartExportAtDocumentNumber As Int32 = ef.StartAtDocumentNumber + 1
		If trueStartExportAtDocumentNumber >= _startExportAtDocumentNumber.Minimum AndAlso trueStartExportAtDocumentNumber <= _startExportAtDocumentNumber.Maximum Then
			_startExportAtDocumentNumber.Value = trueStartExportAtDocumentNumber
		End If

		If ef.ImagePrecedence IsNot Nothing AndAlso ef.ImagePrecedence.Length > 0 Then
			Dim validPrecedenceTable As System.Data.DataTable = Await _application.GetProductionPrecendenceList(ef.CaseInfo)
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
	End Function

	Private Sub LoadCustomFileNamingField(descriptorModel As CustomFileNameDescriptorModel)
		Dim selection = New List(Of CustomFileNameSelectionPart)
		Dim firstFieldSelectionPart = New CustomFileNameSelectionPart(descriptorModel.FirstFieldDescriptorPart().Value, descriptorModel.FirstFieldDescriptorPart().IsProduction)
		selection.Add(firstFieldSelectionPart)
		Dim extendedDescriptorParts = descriptorModel.ExtendedDescriptorParts()
		For i = 0 To (extendedDescriptorParts.Count - 1)
			Dim extendedDescriptorPart = extendedDescriptorParts(i)
			Dim separator = extendedDescriptorPart.Separator.Value
			Dim fieldDescriptorPart = TryCast(extendedDescriptorPart.ValuePart, FieldDescriptorPart)
			If fieldDescriptorPart IsNot Nothing Then
				Dim fieldSelectionPart = New CustomFileNameSelectionPart(separator, fieldDescriptorPart.Value)
				selection.Add(fieldSelectionPart)
				Continue For
			End If
			Dim customTextDescriptorPart = TryCast(extendedDescriptorPart.ValuePart, CustomTextDescriptorPart)
			If customTextDescriptorPart IsNot Nothing Then
				Dim textSelectionPart = New CustomFileNameSelectionPart(separator, customTextDescriptorPart.Value)
				selection.Add(textSelectionPart)
				Continue For
			End If
		Next
		_textAndNativeFileNamePicker.Selection = selection
	End Sub

	Private Async Sub RunMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RunMenu.Click
		Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
		Try
			If Await Me.PopulateExportFile(Me, True) Then Await _application.StartSearch(Me.ExportFile)
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
		retval.CopyNativeFilesFromRepository = _copyFilesFromRepository.Checked
		retval.CopyImageFilesFromRepository = _copyFilesFromRepository.Checked
		Return retval
	End Function

	Private Sub ExportProduction_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
		HandleLoad(sender, e, Relativity.Desktop.Client.Config.ExportVolumeDigitPadding, Relativity.Desktop.Client.Config.ExportSubdirectoryDigitPadding)
		_columnSelector.EnsureHorizontalScrollbars()
		_columnSelector.LeftOrderControlsVisible = False
		_columnSelector.RightOrderControlVisible = True
		InitializeLayout()
	End Sub

	Public Async Sub HandleLoad(ByVal sender As Object, ByVal e As System.EventArgs, ByVal volumeDigitPadding As Int32, ByVal exportSubdirectoryDigitPadding As Int32)
		_dataSourceIsSet = False
		_masterDT = ExportFile.DataTable
		_filters.DataSource = _masterDT
		_filters.DisplayMember = "Name"
		_filters.ValueMember = "ArtifactID"
		_dataSourceIsSet = True

		Relativity.Desktop.Client.Utility.InitializeCharacterDropDown(_recordDelimiter, _exportFile.RecordDelimiter)
		Relativity.Desktop.Client.Utility.InitializeCharacterDropDown(_quoteDelimiter, _exportFile.QuoteDelimiter)
		Relativity.Desktop.Client.Utility.InitializeCharacterDropDown(_newLineDelimiter, _exportFile.NewlineDelimiter)
		Relativity.Desktop.Client.Utility.InitializeCharacterDropDown(_multiRecordDelimiter, _exportFile.MultiRecordDelimiter)
		Relativity.Desktop.Client.Utility.InitializeCharacterDropDown(_nestedValueDelimiter, _exportFile.NestedValueDelimiter)
		_volumeDigitPadding.Value = volumeDigitPadding
		_subdirectoryDigitPadding.Value = exportSubdirectoryDigitPadding
		_imageFileFormat.DataSource = kCura.WinEDDS.LoadFileType.GetLoadFileTypes
		_imageFileFormat.DisplayMember = "DisplayName"
		_imageFileFormat.ValueMember = "Value"
		_imageTypeDropdown.SelectedIndex = 0
		_exportMulticodeFieldsAsNested.Checked = Me.ExportFile.MulticodesAsNested
		Select Case Me.ExportFile.TypeOfExport
			Case ExportFile.ExportType.ArtifactSearch
				_filters.Text = "Searches"
				_filtersBox.Text = "Searches"
				Me.Text = "Relativity Desktop Client | Export Saved Search"
			Case ExportFile.ExportType.ParentSearch, ExportFile.ExportType.AncestorSearch
				_filters.Text = "Views"
				_filtersBox.Text = "Views"
				If Me.ExportFile.ArtifactTypeID = ArtifactType.Document Then
					Me.Text = "Relativity Desktop Client | Export Folder"
					If Me.ExportFile.TypeOfExport = ExportFile.ExportType.AncestorSearch Then
						Me.Text = "Relativity Desktop Client | Export Folder and Subfolders"
					End If
				Else
					Dim objectTypeName As String = Await Me.GetObjectTypeName()
					Me.Text = String.Format("Relativity Desktop Client | Export {0} Objects", objectTypeName)
				End If
			Case ExportFile.ExportType.Production
				_filters.Text = "Productions"
				_filtersBox.Text = "Productions"
				Me.Text = "Relativity Desktop Client | Export Production Set"
				_productionPrecedenceBox.Visible = False
		End Select
		If Not Me.ExportFile.ArtifactTypeID = ArtifactType.Document Then
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
		_textAndNativeFileNamePicker.Initialize(ExportFile.TypeOfExport, ExportFile.AllExportableFields)
		Me.InitializeColumnSelecter()
		Me.InitializeFileControls()

	End Sub

	Private Sub _searchList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _filters.SelectedIndexChanged
		If Not _isLoadingExport AndAlso _dataSourceIsSet AndAlso Not _filters.SelectedItem Is Nothing Then Me.InitializeColumnSelecter()
		If _textFieldPrecedencePicker IsNot Nothing Then
			Dim textFields As List(Of kCura.WinEDDS.ViewFieldInfo) = GetAllLongTextFields()
			textFields.Sort()
			_textFieldPrecedencePicker.AllAvailableLongTextFields = textFields
			_textFieldPrecedencePicker.SelectDefaultTextField(Nothing)
		End If
	End Sub

	Private Sub InitializeFileControls()
		If Me.ExportFile.ArtifactTypeID = ArtifactType.Document Then Exit Sub
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
		If defaultSelectedIds Is Nothing Then
			defaultSelectedIds = New ArrayList()
		End If
		For Each field As kCura.WinEDDS.ViewFieldInfo In Me.ExportFile.AllExportableFields
			If Not defaultSelectedIds.Contains(field.AvfId) Then
				If Me.ExportFile.ArtifactTypeID = ArtifactType.Document Then
					leftListBoxItems.Add(New kCura.WinEDDS.ViewFieldInfo(field))
				ElseIf field.FieldType <> FieldType.File Then
					leftListBoxItems.Add(New kCura.WinEDDS.ViewFieldInfo(field))
				End If
			End If
		Next
		For Each defaultSelectedId As Int32 In defaultSelectedIds
			For Each field As kCura.WinEDDS.ViewFieldInfo In Me.ExportFile.AllExportableFields
				If field.AvfId = defaultSelectedId Then
					If Me.ExportFile.ArtifactTypeID = ArtifactType.Document Then
						_columnSelector.RightSearchableList.AddField(New kCura.WinEDDS.ViewFieldInfo(field))
						Exit For
					ElseIf field.FieldType <> FieldType.File Then
						_columnSelector.RightSearchableList.AddField(New kCura.WinEDDS.ViewFieldInfo(field))
						Exit For
					End If
				End If
			Next
		Next
		leftListBoxItems.Sort()
		_columnSelector.LeftSearchableList.AddFields(leftListBoxItems.ToArray())
		Me.ManagePotentialTextFields()
	End Sub

	Private Sub _exportImages_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _exportImages.CheckedChanged
		_useAbsolutePaths.Enabled = True
		_imageFileFormat.Enabled = _exportImages.Checked
		_imageTypeDropdown.Enabled = _exportImages.Checked And _copyFilesFromRepository.Checked
	End Sub

	Private Sub _exportNativeFiles_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _exportNativeFiles.CheckedChanged
		_useAbsolutePaths.Enabled = True
		_metadataGroupBox.Enabled = _columnSelector.RightSearchableListItems.Count > 0 OrElse _exportNativeFiles.Checked
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

	Private Async Sub _pickPrecedenceButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _pickPrecedenceButton.Click
		Dim dt As System.Data.DataTable = Await _application.GetProductionPrecendenceList(ExportFile.CaseInfo)
		If dt Is Nothing Then Exit Sub
		_precedenceForm = New ProductionPrecedenceForm
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
		_metadataGroupBox.Enabled = _columnSelector.RightSearchableListItems.Count > 0 OrElse _exportNativeFiles.Checked
		Me.ManagePotentialTextFields()
	End Sub

	Private Sub ManagePotentialTextFields()
		Dim textFields As List(Of kCura.WinEDDS.ViewFieldInfo) = GetAllLongTextFields()
		textFields.Sort()
		_textFieldPrecedencePicker.AllAvailableLongTextFields = textFields
	End Sub

	Private Function GetAllLongTextFields() As List(Of kCura.WinEDDS.ViewFieldInfo)
		Dim textFields As New List(Of kCura.WinEDDS.ViewFieldInfo)
		textFields.AddRange(Me.GetRightColumnTextFields())
		textFields.AddRange(Me.GetLeftColumnTextFields())
		Return textFields
	End Function

	Private Function GetRightColumnTextFields() As List(Of kCura.WinEDDS.ViewFieldInfo)
		Return GetTextFields(_columnSelector.RightSearchableListItems.Cast(Of kCura.WinEDDS.ViewFieldInfo).ToList())
	End Function

	Private Function GetLeftColumnTextFields() As List(Of kCura.WinEDDS.ViewFieldInfo)
		Return GetTextFields(_columnSelector.LeftSearchableListItems.Cast(Of kCura.WinEDDS.ViewFieldInfo)().ToList())
	End Function

	Private Function GetTextFields(ByVal unfilteredList As List(Of kCura.WinEDDS.ViewFieldInfo)) As List(Of kCura.WinEDDS.ViewFieldInfo)
		Return (From field In unfilteredList Where field.FieldType = FieldType.Text OrElse field.FieldType = FieldType.OffTableText Select field).ToList()
	End Function

	Private Async Sub RefreshMenu_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles RefreshMenu.Click
		Await Me.RefreshRelativityInformation()
	End Sub

	Private Async Function RefreshRelativityInformation() As Task
		Dim selectedColumns As New System.Collections.ArrayList
		For Each field As kCura.WinEDDS.ViewFieldInfo In _columnSelector.RightSearchableListItems
			selectedColumns.Add(New kCura.WinEDDS.ViewFieldInfo(field))
		Next
		Dim selectedDataSource As Int32 = CInt(_filters.SelectedValue)
		_dataSourceIsSet = False
		Dim newExportFile As kCura.WinEDDS.ExportFile = Await _application.GetNewExportFileSettingsObject(_exportFile.ArtifactID, _exportFile.CaseInfo, _exportFile.TypeOfExport, _exportFile.ArtifactTypeID)
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
			Return
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
		Dim allLongTextFields As List(Of kCura.WinEDDS.ViewFieldInfo) = GetAllLongTextFields()
		allLongTextFields.Sort()
		If allLongTextFields.Count > 0 Then
			_textFieldPrecedencePicker.AllAvailableLongTextFields = allLongTextFields
		End If
		'TODO: this will send -1 index to OnDraw during refresh on exports. Known defect. In backlog
		_columnSelector.LeftSearchableList.ClearListBox()
		_columnSelector.RightSearchableList.ClearListBox()
		Dim al As New System.Collections.ArrayList(_exportFile.AllExportableFields)
		al.Sort()
		_columnSelector.LeftSearchableList.AddFields(al.ToArray())
		For Each field As kCura.WinEDDS.ViewFieldInfo In selectedColumns
			Dim itemToShiftIndex As Int32 = -1
			For i As Int32 = 0 To _columnSelector.LeftSearchableListItems.Count - 1
				Dim item As kCura.WinEDDS.ViewFieldInfo = DirectCast(_columnSelector.LeftSearchableListItems(i), kCura.WinEDDS.ViewFieldInfo)
				If item.AvfId = field.AvfId Then
					itemToShiftIndex = i
					Exit For
				End If
			Next
			If itemToShiftIndex >= 0 Then
				Dim item As kCura.WinEDDS.ViewFieldInfo = DirectCast(_columnSelector.LeftSearchableListItems(itemToShiftIndex), kCura.WinEDDS.ViewFieldInfo)
				_columnSelector.LeftSearchableList.RemoveField(item)
				_columnSelector.RightSearchableList.AddField(item)
			End If
		Next
	End Function

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

	Private Sub _selectFromListButton_Click(sender As Object, e As EventArgs) Handles _selectFromListButton.Click
		Cursor = Cursors.WaitCursor
		Dim selectorFormName As String = "Select " & ExportTypeStringName
		Dim searchListSelector As New SearchListSelector(_masterDT, selectorFormName)
		If searchListSelector.ShowDialog() = Windows.Forms.DialogResult.OK Then
			_filters.SelectedValue = searchListSelector.SelectedValue
		End If
		Cursor = Cursors.Default
	End Sub
End Class