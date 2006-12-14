Namespace kCura.EDDS.WinForm
  Public Class ImageLoad
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New()
      MyBase.New()

      'This call is required by the Windows Form Designer.
      InitializeComponent()

      'Add any initialization after the InitializeComponent() call
      _application = kCura.EDDS.WinForm.Application.Instance
			'_imageLoadFile = New kCura.WinEDDS.ImageLoadFile(kCura.EDDS.WinForm.Application.Instance.Identity)
			_imageLoadFile = New kCura.WinEDDS.ImageLoadFile
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
		Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
		Friend WithEvents _browseButton As System.Windows.Forms.Button
		Friend WithEvents _filePath As System.Windows.Forms.TextBox
		Friend WithEvents GroupBox233 As System.Windows.Forms.GroupBox
		Friend WithEvents _overWrite As System.Windows.Forms.CheckBox
		Friend WithEvents MainMenu As System.Windows.Forms.MainMenu
		Friend WithEvents MenuItem1 As System.Windows.Forms.MenuItem
		Friend WithEvents ImportFileMenu As System.Windows.Forms.MenuItem
		Friend WithEvents _openFileDialog As System.Windows.Forms.OpenFileDialog
		Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
		Friend WithEvents _controlKeyField As System.Windows.Forms.ComboBox
		Friend WithEvents _importMenuSaveSettingsItem As System.Windows.Forms.MenuItem
		Friend WithEvents MenuItem4 As System.Windows.Forms.MenuItem
		Friend WithEvents _saveImageLoadFileDialog As System.Windows.Forms.SaveFileDialog
		Friend WithEvents _importMenuLoadSettingsItem As System.Windows.Forms.MenuItem
		Friend WithEvents _loadImageLoadFileDialog As System.Windows.Forms.OpenFileDialog
		Friend WithEvents _replaceFullText As System.Windows.Forms.CheckBox
		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
			Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(ImageLoad))
			Me.GroupBox3 = New System.Windows.Forms.GroupBox
			Me._browseButton = New System.Windows.Forms.Button
			Me._filePath = New System.Windows.Forms.TextBox
			Me._openFileDialog = New System.Windows.Forms.OpenFileDialog
			Me.GroupBox233 = New System.Windows.Forms.GroupBox
			Me._replaceFullText = New System.Windows.Forms.CheckBox
			Me._overWrite = New System.Windows.Forms.CheckBox
			Me.MainMenu = New System.Windows.Forms.MainMenu
			Me.MenuItem1 = New System.Windows.Forms.MenuItem
			Me.ImportFileMenu = New System.Windows.Forms.MenuItem
			Me.MenuItem4 = New System.Windows.Forms.MenuItem
			Me._importMenuSaveSettingsItem = New System.Windows.Forms.MenuItem
			Me._importMenuLoadSettingsItem = New System.Windows.Forms.MenuItem
			Me.GroupBox1 = New System.Windows.Forms.GroupBox
			Me._controlKeyField = New System.Windows.Forms.ComboBox
			Me._saveImageLoadFileDialog = New System.Windows.Forms.SaveFileDialog
			Me._loadImageLoadFileDialog = New System.Windows.Forms.OpenFileDialog
			Me.GroupBox3.SuspendLayout()
			Me.GroupBox233.SuspendLayout()
			Me.GroupBox1.SuspendLayout()
			Me.SuspendLayout()
			'
			'GroupBox3
			'
			Me.GroupBox3.Controls.Add(Me._browseButton)
			Me.GroupBox3.Controls.Add(Me._filePath)
			Me.GroupBox3.Location = New System.Drawing.Point(4, 4)
			Me.GroupBox3.Name = "GroupBox3"
			Me.GroupBox3.Size = New System.Drawing.Size(568, 48)
			Me.GroupBox3.TabIndex = 7
			Me.GroupBox3.TabStop = False
			Me.GroupBox3.Text = "Load File"
			'
			'_browseButton
			'
			Me._browseButton.Location = New System.Drawing.Point(536, 20)
			Me._browseButton.Name = "_browseButton"
			Me._browseButton.Size = New System.Drawing.Size(24, 20)
			Me._browseButton.TabIndex = 6
			Me._browseButton.Text = "..."
			'
			'_filePath
			'
			Me._filePath.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me._filePath.Location = New System.Drawing.Point(8, 20)
			Me._filePath.Name = "_filePath"
			Me._filePath.Size = New System.Drawing.Size(524, 20)
			Me._filePath.TabIndex = 5
			Me._filePath.Text = "Select a file ..."
			'
			'_openFileDialog
			'
			Me._openFileDialog.Filter = "Log Files (*.log)|*.log|Text Files (*.txt)|*.txt|All files (*.*)|*.*"
			'
			'GroupBox233
			'
			Me.GroupBox233.Controls.Add(Me._replaceFullText)
			Me.GroupBox233.Controls.Add(Me._overWrite)
			Me.GroupBox233.Location = New System.Drawing.Point(292, 60)
			Me.GroupBox233.Name = "GroupBox233"
			Me.GroupBox233.Size = New System.Drawing.Size(280, 52)
			Me.GroupBox233.TabIndex = 8
			Me.GroupBox233.TabStop = False
			Me.GroupBox233.Text = "Update Behavior"
			'
			'_replaceFullText
			'
			Me._replaceFullText.Checked = True
			Me._replaceFullText.CheckState = System.Windows.Forms.CheckState.Checked
			Me._replaceFullText.Location = New System.Drawing.Point(152, 16)
			Me._replaceFullText.Name = "_replaceFullText"
			Me._replaceFullText.Size = New System.Drawing.Size(120, 24)
			Me._replaceFullText.TabIndex = 3
			Me._replaceFullText.Text = "Replace Full Text"
			'
			'_overWrite
			'
			Me._overWrite.Location = New System.Drawing.Point(8, 20)
			Me._overWrite.Name = "_overWrite"
			Me._overWrite.Size = New System.Drawing.Size(96, 16)
			Me._overWrite.TabIndex = 2
			Me._overWrite.Text = "Overwrite"
			'
			'MainMenu
			'
			Me.MainMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuItem1})
			'
			'MenuItem1
			'
			Me.MenuItem1.Index = 0
			Me.MenuItem1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.ImportFileMenu, Me.MenuItem4, Me._importMenuSaveSettingsItem, Me._importMenuLoadSettingsItem})
			Me.MenuItem1.Text = "&Import"
			'
			'ImportFileMenu
			'
			Me.ImportFileMenu.Index = 0
			Me.ImportFileMenu.Shortcut = System.Windows.Forms.Shortcut.F5
			Me.ImportFileMenu.Text = "&Import File..."
			'
			'MenuItem4
			'
			Me.MenuItem4.Index = 1
			Me.MenuItem4.Text = "-"
			'
			'_importMenuSaveSettingsItem
			'
			Me._importMenuSaveSettingsItem.Index = 2
			Me._importMenuSaveSettingsItem.Shortcut = System.Windows.Forms.Shortcut.CtrlS
			Me._importMenuSaveSettingsItem.Text = "Save Settings"
			'
			'_importMenuLoadSettingsItem
			'
			Me._importMenuLoadSettingsItem.Index = 3
			Me._importMenuLoadSettingsItem.Shortcut = System.Windows.Forms.Shortcut.CtrlO
			Me._importMenuLoadSettingsItem.Text = "Load Settings"
			'
			'GroupBox1
			'
			Me.GroupBox1.Controls.Add(Me._controlKeyField)
			Me.GroupBox1.Location = New System.Drawing.Point(4, 60)
			Me.GroupBox1.Name = "GroupBox1"
			Me.GroupBox1.Size = New System.Drawing.Size(280, 52)
			Me.GroupBox1.TabIndex = 9
			Me.GroupBox1.TabStop = False
			Me.GroupBox1.Text = "Identifier Field"
			'
			'_controlKeyField
			'
			Me._controlKeyField.Location = New System.Drawing.Point(8, 16)
			Me._controlKeyField.Name = "_controlKeyField"
			Me._controlKeyField.Size = New System.Drawing.Size(176, 21)
			Me._controlKeyField.TabIndex = 1
			Me._controlKeyField.Text = "select ..."
			'
			'_saveImageLoadFileDialog
			'
			Me._saveImageLoadFileDialog.DefaultExt = "kwe"
			Me._saveImageLoadFileDialog.Filter = "WinEDDS image load files (*.kwi)|*.kwi|All Files (*.*)|*.*"
			'
			'_loadImageLoadFileDialog
			'
			Me._loadImageLoadFileDialog.Filter = "WinEDDS image load files (*.kwi)|*.kwi|All Files (*.*)|*.*"
			'
			'ImageLoad
			'
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.ClientSize = New System.Drawing.Size(580, 145)
			Me.Controls.Add(Me.GroupBox1)
			Me.Controls.Add(Me.GroupBox233)
			Me.Controls.Add(Me.GroupBox3)
			Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
			Me.MaximizeBox = False
			Me.Menu = Me.MainMenu
			Me.Name = "ImageLoad"
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
			Me.Text = "Relativity Desktop Client | Import Image Load File"
			Me.GroupBox3.ResumeLayout(False)
			Me.GroupBox233.ResumeLayout(False)
			Me.GroupBox1.ResumeLayout(False)
			Me.ResumeLayout(False)

		End Sub

#End Region

		Private WithEvents _application As kCura.EDDS.WinForm.Application

		Private _imageLoadFile As kCura.WinEDDS.ImageLoadFile

		Friend Property ImageLoadFile() As kCura.WinEDDS.ImageLoadFile
			Get
				Return _imageLoadFile
			End Get
			Set(ByVal value As kCura.WinEDDS.ImageLoadFile)
				_imageLoadFile = value
			End Set
		End Property

		Private Sub PopulateImageLoadFile()
			Me.Cursor = Cursors.WaitCursor
			If Not Me.EnsureConnection() Then
				Me.Cursor = Cursors.Default
				Exit Sub
			End If
			ImageLoadFile.Overwrite = _overWrite.Checked
			ImageLoadFile.DestinationFolderID = _imageLoadFile.DestinationFolderID
			Me.ImageLoadFile.ReplaceFullText = _replaceFullText.Checked
			If _controlKeyField.SelectedItem Is Nothing Then
				ImageLoadFile.ControlKeyField = Nothing
			Else
				ImageLoadFile.ControlKeyField = _controlKeyField.SelectedItem.ToString
			End If
			Me.Cursor = Cursors.Default
		End Sub

		Private Sub ImageLoad_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
			Me.Cursor = Cursors.WaitCursor
			_controlKeyField.Items.Clear()
			If Not Me.EnsureConnection() Then
				Me.Cursor = Cursors.Default
				Exit Sub
			End If
			_controlKeyField.Items.AddRange(_application.IdentiferFieldDropdownPopulator)
			_overWrite.Checked = ImageLoadFile.Overwrite
			ReadyToRun()
			Me.Cursor = Cursors.Default
		End Sub

		Private Sub ImportFileMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ImportFileMenu.Click
			Me.Cursor = Cursors.WaitCursor
			PopulateImageLoadFile()
			_application.ImportImageFile(_imageLoadFile)
			Me.Cursor = Cursors.Default
		End Sub

		Private Sub _browseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _browseButton.Click
			Me.Cursor = Cursors.WaitCursor
			_openFileDialog.ShowDialog()
			Me.Cursor = Cursors.Default
		End Sub

		Private Sub _openFileDialog_FileOk(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles _openFileDialog.FileOk
			Me.Cursor = Cursors.WaitCursor
			If Not Me.EnsureConnection() Then
				Me.Cursor = Cursors.Default
				Exit Sub
			End If
			_imageLoadFile.FileName = _openFileDialog.FileName
			_filePath.Text = _openFileDialog.FileName
			ReadyToRun()
			Me.Cursor = Cursors.Default
		End Sub

		Private Sub _overWrite_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _overWrite.CheckedChanged
			_imageLoadFile.Overwrite = _overWrite.Checked
		End Sub

		Private Sub _controlKeyField_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _controlKeyField.SelectedIndexChanged
			_imageLoadFile.ControlKeyField = _controlKeyField.SelectedItem.ToString
			If Not Me.EnsureConnection() Then
				Me.Cursor = Cursors.Default
				Exit Sub
			End If
			ReadyToRun()
		End Sub

		Private Sub ReadyToRun()
			ImportFileMenu.Enabled = (Not _controlKeyField.SelectedItem Is Nothing AndAlso System.IO.File.Exists(_imageLoadFile.FileName))
		End Sub

		Private Sub _importMenuSaveSettingsItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _importMenuSaveSettingsItem.Click
			If Not Me.EnsureConnection() Then
				Me.Cursor = Cursors.Default
				Exit Sub
			End If
			PopulateImageLoadFile()
			_saveImageLoadFileDialog.ShowDialog()
		End Sub

		Private Sub _saveImageLoadFileDialog_FileOk(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles _saveImageLoadFileDialog.FileOk
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			If Not Me.EnsureConnection() Then
				Me.Cursor = Cursors.Default
				Exit Sub
			End If
			If Not System.IO.File.Exists(_saveImageLoadFileDialog.FileName) Then
				System.IO.File.Create(_saveImageLoadFileDialog.FileName).Close()
			End If
			_application.SaveImageLoadFile(Me.ImageLoadFile, _saveImageLoadFileDialog.FileName)
			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Sub

		Private Sub _importMenuLoadSettingsItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _importMenuLoadSettingsItem.Click
			_loadImageLoadFileDialog.ShowDialog()
		End Sub

		Private Sub _loadImageLoadFileDialog_FileOk(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles _loadImageLoadFileDialog.FileOk
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			If Not Me.EnsureConnection() Then
				Me.Cursor = Cursors.Default
				Exit Sub
			End If
			If System.IO.File.Exists(_loadImageLoadFileDialog.FileName) Then
				Me.ImageLoadFile = _application.ReadImageLoadFile(_loadImageLoadFileDialog.FileName)
				_overWrite.Checked = ImageLoadFile.Overwrite
				_controlKeyField.SelectedItem = ImageLoadFile.ControlKeyField
				_filePath.Text = ImageLoadFile.FileName
			End If
			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Sub

		Private Function EnsureConnection() As Boolean
			If Not _imageLoadFile Is Nothing AndAlso Not _imageLoadFile.CaseInfo Is Nothing Then
				Dim casefields As String() = Nothing
				Dim continue As Boolean = True
				Try
					casefields = _application.GetCaseFields(_imageLoadFile.CaseInfo.ArtifactID, True)
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