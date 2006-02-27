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
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents _browseButton As System.Windows.Forms.Button
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents OpenFileDialog As System.Windows.Forms.OpenFileDialog
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents _overWrite As System.Windows.Forms.CheckBox
		Friend WithEvents _filePath As System.Windows.Forms.TextBox
    Friend WithEvents _multiRecordDelimiter As System.Windows.Forms.ComboBox
    Friend WithEvents _quoteDelimiter As System.Windows.Forms.ComboBox
    Friend WithEvents _newLineDelimiter As System.Windows.Forms.ComboBox
    Friend WithEvents _recordDelimiter As System.Windows.Forms.ComboBox
    Friend WithEvents _nativeFilePathField As System.Windows.Forms.ComboBox
    Friend WithEvents _loadNativeFiles As System.Windows.Forms.CheckBox
    Friend WithEvents _extractFullTextFromNativeFile As System.Windows.Forms.CheckBox
    Friend WithEvents _importDestinationText As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox32 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox233 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox23 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox20 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox30 As System.Windows.Forms.GroupBox
    Friend WithEvents _fileColumnHeaders As System.Windows.Forms.ListBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents _fieldMap As kCura.Windows.Forms.TwoListBox
    Friend WithEvents MainMenu As System.Windows.Forms.MainMenu
    Friend WithEvents MenuItem1 As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItem2 As System.Windows.Forms.MenuItem
    Friend WithEvents PreviewMenuFile As System.Windows.Forms.MenuItem
    Friend WithEvents ImportFileMenu As System.Windows.Forms.MenuItem
		Friend WithEvents _fileSaveFieldMapMenuItem As System.Windows.Forms.MenuItem
		Friend WithEvents _saveFieldMapDialog As System.Windows.Forms.SaveFileDialog
		Friend WithEvents _fileLoadFieldMapMenuItem As System.Windows.Forms.MenuItem
		Friend WithEvents _loadFieldMapDialog As System.Windows.Forms.OpenFileDialog
		Friend WithEvents _firstLineContainsColumnNames As System.Windows.Forms.CheckBox
		Friend WithEvents _importMenuPreviewErrorsItem As System.Windows.Forms.MenuItem
		Friend WithEvents _identifiersDropDown As System.Windows.Forms.ComboBox
		Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
			Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(LoadFileForm))
			Me.GroupBox32 = New System.Windows.Forms.GroupBox
			Me._fieldMap = New kCura.Windows.Forms.TwoListBox
			Me.GroupBox233 = New System.Windows.Forms.GroupBox
			Me._overWrite = New System.Windows.Forms.CheckBox
			Me._filePath = New System.Windows.Forms.TextBox
			Me._browseButton = New System.Windows.Forms.Button
			Me.GroupBox23 = New System.Windows.Forms.GroupBox
			Me._multiRecordDelimiter = New System.Windows.Forms.ComboBox
			Me.Label6 = New System.Windows.Forms.Label
			Me.Label4 = New System.Windows.Forms.Label
			Me._quoteDelimiter = New System.Windows.Forms.ComboBox
			Me.Label3 = New System.Windows.Forms.Label
			Me._newLineDelimiter = New System.Windows.Forms.ComboBox
			Me.Label2 = New System.Windows.Forms.Label
			Me._recordDelimiter = New System.Windows.Forms.ComboBox
			Me.GroupBox20 = New System.Windows.Forms.GroupBox
			Me.OpenFileDialog = New System.Windows.Forms.OpenFileDialog
			Me._nativeFilePathField = New System.Windows.Forms.ComboBox
			Me.GroupBox30 = New System.Windows.Forms.GroupBox
			Me._firstLineContainsColumnNames = New System.Windows.Forms.CheckBox
			Me._loadNativeFiles = New System.Windows.Forms.CheckBox
			Me._extractFullTextFromNativeFile = New System.Windows.Forms.CheckBox
			Me.Label5 = New System.Windows.Forms.Label
			Me.GroupBox1 = New System.Windows.Forms.GroupBox
			Me._importDestinationText = New System.Windows.Forms.TextBox
			Me._fileColumnHeaders = New System.Windows.Forms.ListBox
			Me.GroupBox2 = New System.Windows.Forms.GroupBox
			Me.MainMenu = New System.Windows.Forms.MainMenu
			Me.MenuItem1 = New System.Windows.Forms.MenuItem
			Me._fileSaveFieldMapMenuItem = New System.Windows.Forms.MenuItem
			Me._fileLoadFieldMapMenuItem = New System.Windows.Forms.MenuItem
			Me.MenuItem2 = New System.Windows.Forms.MenuItem
			Me.PreviewMenuFile = New System.Windows.Forms.MenuItem
			Me._importMenuPreviewErrorsItem = New System.Windows.Forms.MenuItem
			Me.ImportFileMenu = New System.Windows.Forms.MenuItem
			Me._saveFieldMapDialog = New System.Windows.Forms.SaveFileDialog
			Me._loadFieldMapDialog = New System.Windows.Forms.OpenFileDialog
			Me._identifiersDropDown = New System.Windows.Forms.ComboBox
			Me.GroupBox3 = New System.Windows.Forms.GroupBox
			Me.GroupBox32.SuspendLayout()
			Me.GroupBox233.SuspendLayout()
			Me.GroupBox23.SuspendLayout()
			Me.GroupBox20.SuspendLayout()
			Me.GroupBox30.SuspendLayout()
			Me.GroupBox1.SuspendLayout()
			Me.GroupBox2.SuspendLayout()
			Me.GroupBox3.SuspendLayout()
			Me.SuspendLayout()
			'
			'GroupBox32
			'
			Me.GroupBox32.Controls.Add(Me._fieldMap)
			Me.GroupBox32.Location = New System.Drawing.Point(152, 164)
			Me.GroupBox32.Name = "GroupBox32"
			Me.GroupBox32.Size = New System.Drawing.Size(408, 304)
			Me.GroupBox32.TabIndex = 0
			Me.GroupBox32.TabStop = False
			Me.GroupBox32.Text = "Field Map"
			'
			'_fieldMap
			'
			Me._fieldMap.LeftOrderControlsVisible = True
			Me._fieldMap.Location = New System.Drawing.Point(8, 16)
			Me._fieldMap.Name = "_fieldMap"
			Me._fieldMap.RightOrderControlVisible = True
			Me._fieldMap.Size = New System.Drawing.Size(392, 276)
			Me._fieldMap.TabIndex = 0
			'
			'GroupBox233
			'
			Me.GroupBox233.Controls.Add(Me._overWrite)
			Me.GroupBox233.Location = New System.Drawing.Point(8, 380)
			Me.GroupBox233.Name = "GroupBox233"
			Me.GroupBox233.Size = New System.Drawing.Size(136, 40)
			Me.GroupBox233.TabIndex = 1
			Me.GroupBox233.TabStop = False
			Me.GroupBox233.Text = "Update Behavior"
			'
			'_overWrite
			'
			Me._overWrite.Location = New System.Drawing.Point(8, 16)
			Me._overWrite.Name = "_overWrite"
			Me._overWrite.Size = New System.Drawing.Size(96, 20)
			Me._overWrite.TabIndex = 2
			Me._overWrite.Text = "Overwrite"
			'
			'_filePath
			'
			Me._filePath.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me._filePath.Location = New System.Drawing.Point(8, 16)
			Me._filePath.Name = "_filePath"
			Me._filePath.Size = New System.Drawing.Size(680, 20)
			Me._filePath.TabIndex = 2
			Me._filePath.Text = "Select a file ..."
			'
			'_browseButton
			'
			Me._browseButton.Location = New System.Drawing.Point(692, 16)
			Me._browseButton.Name = "_browseButton"
			Me._browseButton.Size = New System.Drawing.Size(24, 20)
			Me._browseButton.TabIndex = 4
			Me._browseButton.Text = "..."
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
			Me.GroupBox23.Location = New System.Drawing.Point(8, 164)
			Me.GroupBox23.Name = "GroupBox23"
			Me.GroupBox23.Size = New System.Drawing.Size(136, 208)
			Me.GroupBox23.TabIndex = 5
			Me.GroupBox23.TabStop = False
			Me.GroupBox23.Text = "Characters"
			'
			'_multiRecordDelimiter
			'
			Me._multiRecordDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._multiRecordDelimiter.Location = New System.Drawing.Point(8, 180)
			Me._multiRecordDelimiter.Name = "_multiRecordDelimiter"
			Me._multiRecordDelimiter.Size = New System.Drawing.Size(120, 21)
			Me._multiRecordDelimiter.TabIndex = 7
			'
			'Label6
			'
			Me.Label6.Location = New System.Drawing.Point(8, 164)
			Me.Label6.Name = "Label6"
			Me.Label6.Size = New System.Drawing.Size(120, 16)
			Me.Label6.TabIndex = 6
			Me.Label6.Text = "Multi-Record Delimiter"
			'
			'Label4
			'
			Me.Label4.Location = New System.Drawing.Point(8, 64)
			Me.Label4.Name = "Label4"
			Me.Label4.Size = New System.Drawing.Size(100, 16)
			Me.Label4.TabIndex = 5
			Me.Label4.Text = "Quote"
			'
			'_quoteDelimiter
			'
			Me._quoteDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._quoteDelimiter.Location = New System.Drawing.Point(8, 84)
			Me._quoteDelimiter.Name = "_quoteDelimiter"
			Me._quoteDelimiter.Size = New System.Drawing.Size(120, 21)
			Me._quoteDelimiter.TabIndex = 4
			'
			'Label3
			'
			Me.Label3.Location = New System.Drawing.Point(8, 116)
			Me.Label3.Name = "Label3"
			Me.Label3.Size = New System.Drawing.Size(100, 16)
			Me.Label3.TabIndex = 3
			Me.Label3.Text = "Newline"
			'
			'_newLineDelimiter
			'
			Me._newLineDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._newLineDelimiter.Location = New System.Drawing.Point(8, 132)
			Me._newLineDelimiter.Name = "_newLineDelimiter"
			Me._newLineDelimiter.Size = New System.Drawing.Size(120, 21)
			Me._newLineDelimiter.TabIndex = 2
			'
			'Label2
			'
			Me.Label2.Location = New System.Drawing.Point(8, 20)
			Me.Label2.Name = "Label2"
			Me.Label2.Size = New System.Drawing.Size(100, 16)
			Me.Label2.TabIndex = 1
			Me.Label2.Text = "Record Delimiter"
			'
			'_recordDelimiter
			'
			Me._recordDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._recordDelimiter.Location = New System.Drawing.Point(8, 36)
			Me._recordDelimiter.Name = "_recordDelimiter"
			Me._recordDelimiter.Size = New System.Drawing.Size(120, 21)
			Me._recordDelimiter.TabIndex = 0
			'
			'GroupBox20
			'
			Me.GroupBox20.Controls.Add(Me._browseButton)
			Me.GroupBox20.Controls.Add(Me._filePath)
			Me.GroupBox20.Location = New System.Drawing.Point(8, 48)
			Me.GroupBox20.Name = "GroupBox20"
			Me.GroupBox20.Size = New System.Drawing.Size(720, 40)
			Me.GroupBox20.TabIndex = 6
			Me.GroupBox20.TabStop = False
			Me.GroupBox20.Text = "Load File"
			'
			'OpenFileDialog
			'
			Me.OpenFileDialog.Filter = "Text Files (*.txt)|*.txt|Log Files (*.log)|*.log|All files (*.*)|*.*"
			'
			'_nativeFilePathField
			'
			Me._nativeFilePathField.Enabled = False
			Me._nativeFilePathField.Location = New System.Drawing.Point(320, 16)
			Me._nativeFilePathField.Name = "_nativeFilePathField"
			Me._nativeFilePathField.Size = New System.Drawing.Size(208, 21)
			Me._nativeFilePathField.TabIndex = 6
			Me._nativeFilePathField.Text = "Select..."
			'
			'GroupBox30
			'
			Me.GroupBox30.Controls.Add(Me._firstLineContainsColumnNames)
			Me.GroupBox30.Controls.Add(Me._loadNativeFiles)
			Me.GroupBox30.Controls.Add(Me._extractFullTextFromNativeFile)
			Me.GroupBox30.Controls.Add(Me.Label5)
			Me.GroupBox30.Controls.Add(Me._nativeFilePathField)
			Me.GroupBox30.Location = New System.Drawing.Point(8, 92)
			Me.GroupBox30.Name = "GroupBox30"
			Me.GroupBox30.Size = New System.Drawing.Size(720, 68)
			Me.GroupBox30.TabIndex = 7
			Me.GroupBox30.TabStop = False
			Me.GroupBox30.Text = "Native Files"
			'
			'_firstLineContainsColumnNames
			'
			Me._firstLineContainsColumnNames.Checked = True
			Me._firstLineContainsColumnNames.CheckState = System.Windows.Forms.CheckState.Checked
			Me._firstLineContainsColumnNames.Location = New System.Drawing.Point(12, 44)
			Me._firstLineContainsColumnNames.Name = "_firstLineContainsColumnNames"
			Me._firstLineContainsColumnNames.Size = New System.Drawing.Size(220, 20)
			Me._firstLineContainsColumnNames.TabIndex = 10
			Me._firstLineContainsColumnNames.Text = "First line contains column names"
			'
			'_loadNativeFiles
			'
			Me._loadNativeFiles.Location = New System.Drawing.Point(12, 16)
			Me._loadNativeFiles.Name = "_loadNativeFiles"
			Me._loadNativeFiles.Size = New System.Drawing.Size(116, 20)
			Me._loadNativeFiles.TabIndex = 9
			Me._loadNativeFiles.Text = "Load Native Files"
			'
			'_extractFullTextFromNativeFile
			'
			Me._extractFullTextFromNativeFile.Enabled = False
			Me._extractFullTextFromNativeFile.Location = New System.Drawing.Point(536, 16)
			Me._extractFullTextFromNativeFile.Name = "_extractFullTextFromNativeFile"
			Me._extractFullTextFromNativeFile.Size = New System.Drawing.Size(180, 20)
			Me._extractFullTextFromNativeFile.TabIndex = 8
			Me._extractFullTextFromNativeFile.Text = "Extract full text from native files"
			'
			'Label5
			'
			Me.Label5.Location = New System.Drawing.Point(124, 16)
			Me.Label5.Name = "Label5"
			Me.Label5.Size = New System.Drawing.Size(192, 20)
			Me.Label5.TabIndex = 7
			Me.Label5.Text = "Native file paths contained in column:"
			Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight
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
			'_fileColumnHeaders
			'
			Me._fileColumnHeaders.Location = New System.Drawing.Point(8, 16)
			Me._fileColumnHeaders.Name = "_fileColumnHeaders"
			Me._fileColumnHeaders.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
			Me._fileColumnHeaders.Size = New System.Drawing.Size(144, 277)
			Me._fileColumnHeaders.TabIndex = 17
			'
			'GroupBox2
			'
			Me.GroupBox2.Controls.Add(Me._fileColumnHeaders)
			Me.GroupBox2.Location = New System.Drawing.Point(568, 164)
			Me.GroupBox2.Name = "GroupBox2"
			Me.GroupBox2.Size = New System.Drawing.Size(160, 304)
			Me.GroupBox2.TabIndex = 18
			Me.GroupBox2.TabStop = False
			Me.GroupBox2.Text = "File Column Headers"
			'
			'MainMenu
			'
			Me.MainMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuItem1, Me.MenuItem2})
			'
			'MenuItem1
			'
			Me.MenuItem1.Index = 0
			Me.MenuItem1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me._fileSaveFieldMapMenuItem, Me._fileLoadFieldMapMenuItem})
			Me.MenuItem1.Text = "&File"
			'
			'_fileSaveFieldMapMenuItem
			'
			Me._fileSaveFieldMapMenuItem.Index = 0
			Me._fileSaveFieldMapMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlS
			Me._fileSaveFieldMapMenuItem.Text = "Save Field Map"
			'
			'_fileLoadFieldMapMenuItem
			'
			Me._fileLoadFieldMapMenuItem.Index = 1
			Me._fileLoadFieldMapMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlO
			Me._fileLoadFieldMapMenuItem.Text = "Load Field Map"
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
			'_identifiersDropDown
			'
			Me._identifiersDropDown.Enabled = False
			Me._identifiersDropDown.Location = New System.Drawing.Point(8, 16)
			Me._identifiersDropDown.Name = "_identifiersDropDown"
			Me._identifiersDropDown.Size = New System.Drawing.Size(121, 21)
			Me._identifiersDropDown.TabIndex = 19
			Me._identifiersDropDown.Text = "Select..."
			'
			'GroupBox3
			'
			Me.GroupBox3.Controls.Add(Me._identifiersDropDown)
			Me.GroupBox3.Location = New System.Drawing.Point(8, 424)
			Me.GroupBox3.Name = "GroupBox3"
			Me.GroupBox3.Size = New System.Drawing.Size(136, 44)
			Me.GroupBox3.TabIndex = 20
			Me.GroupBox3.TabStop = False
			Me.GroupBox3.Text = "Identifier"
			'
			'LoadFileForm
			'
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.ClientSize = New System.Drawing.Size(734, 475)
			Me.Controls.Add(Me.GroupBox3)
			Me.Controls.Add(Me.GroupBox2)
			Me.Controls.Add(Me.GroupBox1)
			Me.Controls.Add(Me.GroupBox30)
			Me.Controls.Add(Me.GroupBox20)
			Me.Controls.Add(Me.GroupBox23)
			Me.Controls.Add(Me.GroupBox233)
			Me.Controls.Add(Me.GroupBox32)
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
			Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
			Me.MaximizeBox = False
			Me.Menu = Me.MainMenu
			Me.Name = "LoadFileForm"
			Me.Text = "Import Load File"
			Me.GroupBox32.ResumeLayout(False)
			Me.GroupBox233.ResumeLayout(False)
			Me.GroupBox23.ResumeLayout(False)
			Me.GroupBox20.ResumeLayout(False)
			Me.GroupBox30.ResumeLayout(False)
			Me.GroupBox1.ResumeLayout(False)
			Me.GroupBox2.ResumeLayout(False)
			Me.GroupBox3.ResumeLayout(False)
			Me.ResumeLayout(False)

		End Sub

#End Region

		Friend WithEvents _application As kCura.EDDS.WinForm.Application

		Private _loadFile As kCura.WinEDDS.LoadFile

		Friend ReadOnly Property ReadyToRun() As Boolean
			Get
				Dim rtr As Boolean
				If _loadNativeFiles.Checked Then
					rtr = _nativeFilePathField.SelectedIndex <> -1
				Else
					rtr = True
				End If
				Return _
				 _fieldMap.RightListBoxItems.Count > 0 AndAlso _
				 rtr AndAlso _
				 System.IO.File.Exists(_filePath.Text) AndAlso _
				_identifiersDropDown.SelectedIndex <> -1
			End Get
		End Property

		Private Sub PopulateLoadFileObject()
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			LoadFile.QuoteDelimiter = Chr(CType(_quoteDelimiter.SelectedValue, Int32))
			LoadFile.RecordDelimiter = Chr(CType(_recordDelimiter.SelectedValue, Int32))
			LoadFile.MultiRecordDelimiter = Chr(CType(_multiRecordDelimiter.SelectedValue, Int32))
			LoadFile.NewlineDelimiter = Chr(CType(_newLineDelimiter.SelectedValue, Int32))
			Me.LoadFile.SelectedFields = kCura.EDDS.WinForm.Utility.ExtractFieldMap(_fieldMap, _application.CurrentFields)
			LoadFile.ExtractFullTextFromNativeFile = _extractFullTextFromNativeFile.Checked
			LoadFile.LoadNativeFiles = _loadNativeFiles.Checked
			LoadFile.OverwriteDestination = _overWrite.Checked
			LoadFile.FirstLineContainsHeaders = _firstLineContainsColumnNames.Checked
			If Not _identifiersDropDown.SelectedItem Is Nothing Then LoadFile.SelectedIdentifierField = _application.GetDocumentFieldFromName(_identifiersDropDown.SelectedItem.ToString)
			If _loadNativeFiles.Checked Then
				If Not _nativeFilePathField.SelectedItem Is Nothing Then
					LoadFile.NativeFilePathColumn = _nativeFilePathField.SelectedItem.ToString
				Else
					LoadFile.NativeFilePathColumn = Nothing
				End If
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
			_fieldMap.LeftListBoxItems.Clear()
			_fieldMap.RightListBoxItems.Clear()
			_fileColumnHeaders.Items.Clear()
			_nativeFilePathField.Items.Clear()
			_identifiersDropDown.Items.Clear()
			_loadNativeFiles.Checked = LoadFile.LoadNativeFiles
			RefreshNativeFilePathFieldAndFileColumnHeaders()
			Dim caseFields As String() = _application.GetCaseFields(LoadFile.CaseInfo.RootArtifactID)
			Dim caseFieldName As String
			If loadFileObjectUpdatedFromFile Then
				Dim selectedFieldNames As String() = kCura.WinEDDS.Utility.BuildSelectedFieldNameList(_application.CurrentFields, _loadFile.SelectedFields)
				_fieldMap.RightListBoxItems.AddRange(selectedFieldNames)
				For Each caseFieldName In caseFields
					If Array.IndexOf(selectedFieldNames, caseFieldName) = -1 Then
						_fieldMap.LeftListBoxItems.Add(caseFieldName)
					End If
				Next
				If LoadFile.LoadNativeFiles Then
					_loadNativeFiles.Checked = True
					If LoadFile.NativeFilePathColumn <> String.Empty Then
						_nativeFilePathField.SelectedItem = LoadFile.NativeFilePathColumn
					End If
					_extractFullTextFromNativeFile.Checked = LoadFile.ExtractFullTextFromNativeFile
				End If
				ActionMenuEnabled = ReadyToRun
			Else
				_fieldMap.LeftListBoxItems.AddRange(caseFields)
			End If
			_identifiersDropDown.Items.AddRange(_application.GetCaseIdentifierFields)
			_overWrite.Checked = LoadFile.OverwriteDestination
			_identifiersDropDown.Enabled = True			'LoadFile.OverwriteDestination
			_overWrite.Checked = LoadFile.OverwriteDestination
			If Not LoadFile.SelectedIdentifierField Is Nothing Then
				caseFieldName = _application.GetSelectedIdentifier(LoadFile.SelectedIdentifierField)
				If caseFieldName <> String.Empty Then
					_identifiersDropDown.SelectedItem = caseFieldName
				End If
			End If

			'If LoadFile.OverwriteDestination AndAlso Not LoadFile.SelectedIdentifierField Is Nothing Then
			'	_overWrite.Checked = True
			'	caseFieldName = _application.GetSelectedIdentifier(LoadFile.SelectedIdentifierField)
			'	If caseFieldName <> String.Empty Then
			'		_identifiersDropDown.SelectedItem = caseFieldName
			'	End If
			'End If
			ActionMenuEnabled = ReadyToRun
			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Sub

		Public Property LoadFile() As kCura.WinEDDS.LoadFile
			Get
				If _loadFile Is Nothing Then
					_loadFile = New kCura.WinEDDS.LoadFile
				End If
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

		Private Sub RefreshNativeFilePathFieldAndFileColumnHeaders()
			_fileColumnHeaders.Items.Clear()
			_nativeFilePathField.Items.Clear()
			If System.IO.File.Exists(LoadFile.FilePath) Then
				Dim columnHeaders As String() = _application.GetColumnHeadersFromLoadFile(LoadFile, _firstLineContainsColumnNames.Checked)
				'_filePath.Text = LoadFile.FilePath
				_fileColumnHeaders.Items.AddRange(columnHeaders)
				_nativeFilePathField.Items.AddRange(columnHeaders)
				If LoadFile.LoadNativeFiles AndAlso System.IO.File.Exists(LoadFile.FilePath) Then
					_nativeFilePathField.SelectedItem = LoadFile.NativeFilePathColumn
				End If
			Else
			End If
			_nativeFilePathField.SelectedItem = Nothing
			_nativeFilePathField.Text = "Select ..."
			ActionMenuEnabled = ReadyToRun
		End Sub

		Private Sub OpenFileDialog_FileOk(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog.FileOk
			_filePath.Text = OpenFileDialog.FileName
			PopulateLoadFileObject()
		End Sub

		Private Sub _filePath_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _filePath.TextChanged
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
			ActionMenuEnabled = ReadyToRun
		End Sub

		Private Sub _overWrite_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _overWrite.CheckedChanged
			LoadFile.OverwriteDestination = _overWrite.Checked
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

		Private Sub _nativeFilePathField_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _nativeFilePathField.SelectedIndexChanged
			ActionMenuEnabled = ReadyToRun
		End Sub

		Private Sub _firstLineContainsColumnNames_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _firstLineContainsColumnNames.CheckedChanged
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

		Private Sub _quoteDelimiter_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _quoteDelimiter.SelectedIndexChanged

		End Sub

	End Class
End Namespace
