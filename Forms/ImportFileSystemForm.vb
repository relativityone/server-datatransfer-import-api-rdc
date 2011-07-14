Namespace kCura.EDDS.WinForm
	Public Class ImportFileSystemForm
		Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

		Public Sub New()
			MyBase.New()

			'This call is required by the Windows Form Designer.
			InitializeComponent()

			'Add any initialization after the InitializeComponent() call
			_application = kCura.EDDS.WinForm.Application.Instance()

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
		Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
		Friend WithEvents _importDestinationText As System.Windows.Forms.TextBox
		Friend WithEvents GroupBox20 As System.Windows.Forms.GroupBox
		Friend WithEvents _browseButton As System.Windows.Forms.Button
		Friend WithEvents _filePath As System.Windows.Forms.TextBox
		Friend WithEvents MainMenu As System.Windows.Forms.MainMenu
		Friend WithEvents MenuItem1 As System.Windows.Forms.MenuItem
		Friend WithEvents _importDirectoryMenu As System.Windows.Forms.MenuItem
		Friend WithEvents FolderBrowserDialog As System.Windows.Forms.FolderBrowserDialog
		Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
		Friend WithEvents Label1 As System.Windows.Forms.Label
		Friend WithEvents Label2 As System.Windows.Forms.Label
		Friend WithEvents _caseFields As kCura.Windows.Forms.TwoListBox
		Friend WithEvents _fileFields As kCura.Windows.Forms.TwoListBox
		Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
		Friend WithEvents Label3 As System.Windows.Forms.Label
		Friend WithEvents Label4 As System.Windows.Forms.Label
		Friend WithEvents _batesNumberPrefix As System.Windows.Forms.TextBox
		Friend WithEvents _batesNumberSeed As System.Windows.Forms.TextBox
		Friend WithEvents _attachFiles As System.Windows.Forms.CheckBox
		Friend WithEvents _extractFullTextFromFile As System.Windows.Forms.CheckBox
		Friend WithEvents Label5 As System.Windows.Forms.Label
		Friend WithEvents FileExtentionTextBox As System.Windows.Forms.TextBox
		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
			Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(ImportFileSystemForm))
			Me.GroupBox1 = New System.Windows.Forms.GroupBox
			Me._importDestinationText = New System.Windows.Forms.TextBox
			Me.GroupBox20 = New System.Windows.Forms.GroupBox
			Me.Label5 = New System.Windows.Forms.Label
			Me.FileExtentionTextBox = New System.Windows.Forms.TextBox
			Me._browseButton = New System.Windows.Forms.Button
			Me._filePath = New System.Windows.Forms.TextBox
			Me.MainMenu = New System.Windows.Forms.MainMenu
			Me.MenuItem1 = New System.Windows.Forms.MenuItem
			Me._importDirectoryMenu = New System.Windows.Forms.MenuItem
			Me.FolderBrowserDialog = New System.Windows.Forms.FolderBrowserDialog
			Me.GroupBox2 = New System.Windows.Forms.GroupBox
			Me._fileFields = New kCura.Windows.Forms.TwoListBox
			Me.Label2 = New System.Windows.Forms.Label
			Me.Label1 = New System.Windows.Forms.Label
			Me._caseFields = New kCura.Windows.Forms.TwoListBox
			Me.GroupBox3 = New System.Windows.Forms.GroupBox
			Me._extractFullTextFromFile = New System.Windows.Forms.CheckBox
			Me._attachFiles = New System.Windows.Forms.CheckBox
			Me.Label4 = New System.Windows.Forms.Label
			Me.Label3 = New System.Windows.Forms.Label
			Me._batesNumberSeed = New System.Windows.Forms.TextBox
			Me._batesNumberPrefix = New System.Windows.Forms.TextBox
			Me.GroupBox1.SuspendLayout()
			Me.GroupBox20.SuspendLayout()
			Me.GroupBox2.SuspendLayout()
			Me.GroupBox3.SuspendLayout()
			Me.SuspendLayout()
			'
			'GroupBox1
			'
			Me.GroupBox1.Controls.Add(Me._importDestinationText)
			Me.GroupBox1.Location = New System.Drawing.Point(4, 0)
			Me.GroupBox1.Name = "GroupBox1"
			Me.GroupBox1.Size = New System.Drawing.Size(736, 40)
			Me.GroupBox1.TabIndex = 10
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
			Me._importDestinationText.Size = New System.Drawing.Size(724, 13)
			Me._importDestinationText.TabIndex = 5
			Me._importDestinationText.Text = ""
			'
			'GroupBox20
			'
			Me.GroupBox20.Controls.Add(Me.Label5)
			Me.GroupBox20.Controls.Add(Me.FileExtentionTextBox)
			Me.GroupBox20.Controls.Add(Me._browseButton)
			Me.GroupBox20.Controls.Add(Me._filePath)
			Me.GroupBox20.Location = New System.Drawing.Point(4, 44)
			Me.GroupBox20.Name = "GroupBox20"
			Me.GroupBox20.Size = New System.Drawing.Size(736, 72)
			Me.GroupBox20.TabIndex = 9
			Me.GroupBox20.TabStop = False
			Me.GroupBox20.Text = "File Directory"
			'
			'Label5
			'
			Me.Label5.Location = New System.Drawing.Point(8, 44)
			Me.Label5.Name = "Label5"
			Me.Label5.Size = New System.Drawing.Size(136, 20)
			Me.Label5.TabIndex = 6
			Me.Label5.Text = "File Extentions To Import:"
			Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight
			'
			'FileExtentionTextBox
			'
			Me.FileExtentionTextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me.FileExtentionTextBox.Location = New System.Drawing.Point(148, 44)
			Me.FileExtentionTextBox.Name = "FileExtentionTextBox"
			Me.FileExtentionTextBox.Size = New System.Drawing.Size(584, 20)
			Me.FileExtentionTextBox.TabIndex = 5
			Me.FileExtentionTextBox.Text = "pdf;txt;doc;rtf;xls;ppt;doc;vsd"
			'
			'_browseButton
			'
			Me._browseButton.Location = New System.Drawing.Point(708, 16)
			Me._browseButton.Name = "_browseButton"
			Me._browseButton.Size = New System.Drawing.Size(24, 20)
			Me._browseButton.TabIndex = 4
			Me._browseButton.Text = "..."
			'
			'_filePath
			'
			Me._filePath.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me._filePath.Location = New System.Drawing.Point(8, 16)
			Me._filePath.Name = "_filePath"
			Me._filePath.Size = New System.Drawing.Size(696, 20)
			Me._filePath.TabIndex = 2
			Me._filePath.Text = "Select a directory ..."
			'
			'MainMenu
			'
			Me.MainMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuItem1})
			'
			'MenuItem1
			'
			Me.MenuItem1.Index = 0
			Me.MenuItem1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me._importDirectoryMenu})
			Me.MenuItem1.Text = "&Import"
			'
			'_importDirectoryMenu
			'
			Me._importDirectoryMenu.Index = 0
			Me._importDirectoryMenu.Text = "&Import Directory..."
			'
			'GroupBox2
			'
			Me.GroupBox2.Controls.Add(Me._fileFields)
			Me.GroupBox2.Controls.Add(Me.Label2)
			Me.GroupBox2.Controls.Add(Me.Label1)
			Me.GroupBox2.Controls.Add(Me._caseFields)
			Me.GroupBox2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me.GroupBox2.Location = New System.Drawing.Point(4, 168)
			Me.GroupBox2.Name = "GroupBox2"
			Me.GroupBox2.Size = New System.Drawing.Size(736, 320)
			Me.GroupBox2.TabIndex = 12
			Me.GroupBox2.TabStop = False
			Me.GroupBox2.Text = "Field Map"
			'
			'_fileFields
			'
			Me._fileFields.AlternateRowColors = False
			Me._fileFields.KeepButtonsCentered = False
			Me._fileFields.LeftOrderControlsVisible = True
			Me._fileFields.Location = New System.Drawing.Point(372, 36)
			Me._fileFields.Name = "_fileFields"
			Me._fileFields.RightOrderControlVisible = False
			Me._fileFields.Size = New System.Drawing.Size(356, 280)
			Me._fileFields.TabIndex = 4
			Me._fileFields.OuterBox = kCura.Windows.Forms.TwoListBox.ListBoxLocation.Right
			'
			'Label2
			'
			Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me.Label2.Location = New System.Drawing.Point(396, 20)
			Me.Label2.Name = "Label2"
			Me.Label2.Size = New System.Drawing.Size(332, 16)
			Me.Label2.TabIndex = 3
			Me.Label2.Text = "File Fields"
			Me.Label2.TextAlign = System.Drawing.ContentAlignment.TopRight
			'
			'Label1
			'
			Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me.Label1.Location = New System.Drawing.Point(8, 20)
			Me.Label1.Name = "Label1"
			Me.Label1.Size = New System.Drawing.Size(332, 16)
			Me.Label1.TabIndex = 2
			Me.Label1.Text = "Case Fields"
			'
			'_caseFields
			'
			Me._caseFields.AlternateRowColors = False
			Me._caseFields.KeepButtonsCentered = False
			Me._caseFields.LeftOrderControlsVisible = False
			Me._caseFields.Location = New System.Drawing.Point(8, 36)
			Me._caseFields.Name = "_caseFields"
			Me._caseFields.RightOrderControlVisible = True
			Me._caseFields.Size = New System.Drawing.Size(360, 280)
			Me._caseFields.TabIndex = 0
			Me._caseFields.OuterBox = kCura.Windows.Forms.TwoListBox.ListBoxLocation.Left
			'
			'GroupBox3
			'
			Me.GroupBox3.Controls.Add(Me._extractFullTextFromFile)
			Me.GroupBox3.Controls.Add(Me._attachFiles)
			Me.GroupBox3.Controls.Add(Me.Label4)
			Me.GroupBox3.Controls.Add(Me.Label3)
			Me.GroupBox3.Controls.Add(Me._batesNumberSeed)
			Me.GroupBox3.Controls.Add(Me._batesNumberPrefix)
			Me.GroupBox3.Location = New System.Drawing.Point(4, 120)
			Me.GroupBox3.Name = "GroupBox3"
			Me.GroupBox3.Size = New System.Drawing.Size(736, 40)
			Me.GroupBox3.TabIndex = 13
			Me.GroupBox3.TabStop = False
			Me.GroupBox3.Text = "Settings"
			'
			'_extractFullTextFromFile
			'
			Me._extractFullTextFromFile.Checked = True
			Me._extractFullTextFromFile.CheckState = System.Windows.Forms.CheckState.Checked
			Me._extractFullTextFromFile.Location = New System.Drawing.Point(504, 20)
			Me._extractFullTextFromFile.Name = "_extractFullTextFromFile"
			Me._extractFullTextFromFile.Size = New System.Drawing.Size(164, 16)
			Me._extractFullTextFromFile.TabIndex = 7
			Me._extractFullTextFromFile.Text = "Extract Full Text From File"
			'
			'_attachFiles
			'
			Me._attachFiles.Checked = True
			Me._attachFiles.CheckState = System.Windows.Forms.CheckState.Checked
			Me._attachFiles.Location = New System.Drawing.Point(412, 20)
			Me._attachFiles.Name = "_attachFiles"
			Me._attachFiles.Size = New System.Drawing.Size(84, 16)
			Me._attachFiles.TabIndex = 6
			Me._attachFiles.Text = "Attach Files"
			'
			'Label4
			'
			Me.Label4.Location = New System.Drawing.Point(212, 16)
			Me.Label4.Name = "Label4"
			Me.Label4.Size = New System.Drawing.Size(112, 20)
			Me.Label4.TabIndex = 5
			Me.Label4.Text = "Bates Number Seed:"
			Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight
			'
			'Label3
			'
			Me.Label3.Location = New System.Drawing.Point(8, 16)
			Me.Label3.Name = "Label3"
			Me.Label3.Size = New System.Drawing.Size(112, 20)
			Me.Label3.TabIndex = 4
			Me.Label3.Text = "Bates Number Prefix:"
			Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
			'
			'_batesNumberSeed
			'
			Me._batesNumberSeed.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me._batesNumberSeed.Location = New System.Drawing.Point(328, 16)
			Me._batesNumberSeed.Name = "_batesNumberSeed"
			Me._batesNumberSeed.Size = New System.Drawing.Size(68, 20)
			Me._batesNumberSeed.TabIndex = 3
			Me._batesNumberSeed.Text = "1"
			'
			'_batesNumberPrefix
			'
			Me._batesNumberPrefix.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me._batesNumberPrefix.Location = New System.Drawing.Point(120, 16)
			Me._batesNumberPrefix.Name = "_batesNumberPrefix"
			Me._batesNumberPrefix.Size = New System.Drawing.Size(68, 20)
			Me._batesNumberPrefix.TabIndex = 2
			Me._batesNumberPrefix.Text = "FS"
			'
			'ImportFileSystemForm
			'
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.ClientSize = New System.Drawing.Size(744, 493)
			Me.Controls.Add(Me.GroupBox3)
			Me.Controls.Add(Me.GroupBox2)
			Me.Controls.Add(Me.GroupBox1)
			Me.Controls.Add(Me.GroupBox20)
			Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
			Me.Menu = Me.MainMenu
			Me.Name = "ImportFileSystemForm"
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
			Me.Text = "Relativity Desktop Client | Import File Directory"
			Me.GroupBox1.ResumeLayout(False)
			Me.GroupBox20.ResumeLayout(False)
			Me.GroupBox2.ResumeLayout(False)
			Me.GroupBox3.ResumeLayout(False)
			Me.ResumeLayout(False)

		End Sub

#End Region

		Private WithEvents _application As kCura.EDDS.WinForm.Application

		Private _importFileDirectorySettings As kCura.WinEDDS.ImportFileDirectorySettings

		Private Sub _importDirectoryMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _importDirectoryMenu.Click
			Me.Cursor = Cursors.WaitCursor

			Dim fieldMappings() As ImportFileDirectorySettings.FieldMap

			Dim i As Int32

			ReDim fieldMappings(_caseFields.RightListBoxItems.Count - 1)

			For i = 0 To _caseFields.RightListBoxItems.Count - 1
				Dim fieldMap As New ImportFileDirectorySettings.FieldMap
				fieldMap.CaseField = CType(_caseFields.RightListBoxItems.Item(i), String)
				fieldMap.FileField = CType(_fileFields.LeftListBoxItems.Item(i), String)
				fieldMappings(i) = fieldMap
			Next

			ImportFileDirectorySettings.BatesNumberPrefix = _batesNumberPrefix.Text
			ImportFileDirectorySettings.BatesNumberSeed = CType(_batesNumberSeed.Text, Int32)
			ImportFileDirectorySettings.FieldMappings = fieldMappings
			ImportFileDirectorySettings.AttachFiles = _attachFiles.Checked
			ImportFileDirectorySettings.ExtractFullTextFromFile = _extractFullTextFromFile.Checked
			ImportFileDirectorySettings.FileExtentionsToImport = FileExtentionTextBox.Text
			_application.ImportDirectory(Me.ImportFileDirectorySettings)
			Me.Cursor = Cursors.Default

		End Sub

		Public Sub LoadFormControls()
			Me.Cursor = Cursors.WaitCursor
			_filePath.Text = ImportFileDirectorySettings.FilePath
			_importDestinationText.Text = _application.GetCaseFolderPath(ImportFileDirectorySettings.DestinationFolderID)

			_caseFields.LeftListBoxItems.Clear()
			Dim caseField As String
			'TODO: WINFLEX - ArtifactTypeID
			For Each caseField In _application.GetCaseFields(_importFileDirectorySettings.CaseInfo.ArtifactID, 10)
				_caseFields.LeftListBoxItems.Add(caseField)
			Next

			If _importFileDirectorySettings.EnronImport Then
				_fileFields.RightListBoxItems.Clear()
				_fileFields.RightListBoxItems.Add("AutoGenerated Bates Number")
				_fileFields.RightListBoxItems.Add("File Type")
				_fileFields.RightListBoxItems.Add("Message-ID")
				_fileFields.RightListBoxItems.Add("Date")
				_fileFields.RightListBoxItems.Add("From")
				_fileFields.RightListBoxItems.Add("To")
				_fileFields.RightListBoxItems.Add("Subject")
				_fileFields.RightListBoxItems.Add("Body")
				_fileFields.RightListBoxItems.Add("Mime-Version")
				_fileFields.RightListBoxItems.Add("Content-Type")
				_fileFields.RightListBoxItems.Add("Content-Transfer-Encoding")
				_fileFields.RightListBoxItems.Add("X-From")
				_fileFields.RightListBoxItems.Add("X-To")
				_fileFields.RightListBoxItems.Add("X-CC")
				_fileFields.RightListBoxItems.Add("X-BCC")
				_fileFields.RightListBoxItems.Add("X-Folder")
				_fileFields.RightListBoxItems.Add("X-Origin")
				_fileFields.RightListBoxItems.Add("X-FileName")

			Else
				_fileFields.RightListBoxItems.Add("AutoGenerated Bates Number")
				_fileFields.RightListBoxItems.Add("CreationTime")
				_fileFields.RightListBoxItems.Add("LastModifiedTime")
				_fileFields.RightListBoxItems.Add("DirectoryName")
				_fileFields.RightListBoxItems.Add("Extention")
				_fileFields.RightListBoxItems.Add("FullName")
				_fileFields.RightListBoxItems.Add("Size")
				_fileFields.RightListBoxItems.Add("Name")
			End If

			Me.Cursor = Cursors.Default
		End Sub

		Public Property ImportFileDirectorySettings() As kCura.WinEDDS.ImportFileDirectorySettings
			Get
				If _importFileDirectorySettings Is Nothing Then
					_importFileDirectorySettings = New kCura.WinEDDS.ImportFileDirectorySettings
				End If
				Return _importFileDirectorySettings
			End Get
			Set(ByVal value As kCura.WinEDDS.ImportFileDirectorySettings)
				_importFileDirectorySettings = value
				Me.LoadFormControls()
			End Set
		End Property

		Private Sub _browseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _browseButton.Click
			FolderBrowserDialog.ShowDialog()
			_importFileDirectorySettings.FilePath = FolderBrowserDialog.SelectedPath
			_filePath.Text = _importFileDirectorySettings.FilePath
		End Sub

		Private Sub ImportFileSystemForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
			If _importFileDirectorySettings.EnronImport = True Then
				Me.Text = "Enron Data Importer..."
				_extractFullTextFromFile.Checked = False
				_extractFullTextFromFile.Visible = False
			End If
		End Sub

	End Class
End Namespace
