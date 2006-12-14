Namespace kCura.EDDS.WinForm
	Public Class ProductionExportForm
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
		Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
		Friend WithEvents _browseButton As System.Windows.Forms.Button
		Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
		Friend WithEvents MainMenu As System.Windows.Forms.MainMenu
		Friend WithEvents ExportMenu As System.Windows.Forms.MenuItem
		Friend WithEvents RunMenu As System.Windows.Forms.MenuItem
		Friend WithEvents _destinationFolderDialog As System.Windows.Forms.FolderBrowserDialog
		Friend WithEvents _productionList As System.Windows.Forms.ComboBox
		Friend WithEvents _folderPath As System.Windows.Forms.TextBox
		Friend WithEvents _overwriteButton As System.Windows.Forms.CheckBox
		Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
		Friend WithEvents _loadFileFormat As System.Windows.Forms.ComboBox
		Friend WithEvents _exportNatives As System.Windows.Forms.CheckBox
		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
			Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(ProductionExportForm))
			Me.GroupBox3 = New System.Windows.Forms.GroupBox
			Me._overwriteButton = New System.Windows.Forms.CheckBox
			Me._browseButton = New System.Windows.Forms.Button
			Me._folderPath = New System.Windows.Forms.TextBox
			Me.GroupBox1 = New System.Windows.Forms.GroupBox
			Me._exportNatives = New System.Windows.Forms.CheckBox
			Me._productionList = New System.Windows.Forms.ComboBox
			Me.MainMenu = New System.Windows.Forms.MainMenu
			Me.ExportMenu = New System.Windows.Forms.MenuItem
			Me.RunMenu = New System.Windows.Forms.MenuItem
			Me._destinationFolderDialog = New System.Windows.Forms.FolderBrowserDialog
			Me.GroupBox2 = New System.Windows.Forms.GroupBox
			Me._loadFileFormat = New System.Windows.Forms.ComboBox
			Me.GroupBox3.SuspendLayout()
			Me.GroupBox1.SuspendLayout()
			Me.GroupBox2.SuspendLayout()
			Me.SuspendLayout()
			'
			'GroupBox3
			'
			Me.GroupBox3.Controls.Add(Me._overwriteButton)
			Me.GroupBox3.Controls.Add(Me._browseButton)
			Me.GroupBox3.Controls.Add(Me._folderPath)
			Me.GroupBox3.Location = New System.Drawing.Point(4, 88)
			Me.GroupBox3.Name = "GroupBox3"
			Me.GroupBox3.Size = New System.Drawing.Size(568, 72)
			Me.GroupBox3.TabIndex = 8
			Me.GroupBox3.TabStop = False
			Me.GroupBox3.Text = "Select a Location..."
			'
			'_overwriteButton
			'
			Me._overwriteButton.Location = New System.Drawing.Point(8, 48)
			Me._overwriteButton.Name = "_overwriteButton"
			Me._overwriteButton.Size = New System.Drawing.Size(548, 16)
			Me._overwriteButton.TabIndex = 4
			Me._overwriteButton.Text = "Overwrite Files"
			'
			'_browseButton
			'
			Me._browseButton.Location = New System.Drawing.Point(536, 20)
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
			Me._folderPath.Size = New System.Drawing.Size(524, 20)
			Me._folderPath.TabIndex = 3
			Me._folderPath.Text = "Select a file ..."
			'
			'GroupBox1
			'
			Me.GroupBox1.Controls.Add(Me._exportNatives)
			Me.GroupBox1.Controls.Add(Me._productionList)
			Me.GroupBox1.Location = New System.Drawing.Point(4, 4)
			Me.GroupBox1.Name = "GroupBox1"
			Me.GroupBox1.Size = New System.Drawing.Size(568, 76)
			Me.GroupBox1.TabIndex = 9
			Me.GroupBox1.TabStop = False
			Me.GroupBox1.Text = "Production"
			'
			'_exportNatives
			'
			Me._exportNatives.Location = New System.Drawing.Point(8, 52)
			Me._exportNatives.Name = "_exportNatives"
			Me._exportNatives.Size = New System.Drawing.Size(548, 16)
			Me._exportNatives.TabIndex = 2
			Me._exportNatives.Text = "Export Natives"
			'
			'_productionList
			'
			Me._productionList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._productionList.Location = New System.Drawing.Point(8, 20)
			Me._productionList.Name = "_productionList"
			Me._productionList.Size = New System.Drawing.Size(552, 21)
			Me._productionList.TabIndex = 1
			'
			'MainMenu
			'
			Me.MainMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.ExportMenu})
			'
			'ExportMenu
			'
			Me.ExportMenu.Index = 0
			Me.ExportMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.RunMenu})
			Me.ExportMenu.Text = "&Export"
			'
			'RunMenu
			'
			Me.RunMenu.Index = 0
			Me.RunMenu.Shortcut = System.Windows.Forms.Shortcut.F5
			Me.RunMenu.Text = "&Run..."
			'
			'GroupBox2
			'
			Me.GroupBox2.Controls.Add(Me._loadFileFormat)
			Me.GroupBox2.Location = New System.Drawing.Point(4, 164)
			Me.GroupBox2.Name = "GroupBox2"
			Me.GroupBox2.Size = New System.Drawing.Size(568, 52)
			Me.GroupBox2.TabIndex = 10
			Me.GroupBox2.TabStop = False
			Me.GroupBox2.Text = "Export Load File Format"
			'
			'_loadFileFormat
			'
			Me._loadFileFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._loadFileFormat.DropDownWidth = 150
			Me._loadFileFormat.Location = New System.Drawing.Point(8, 20)
			Me._loadFileFormat.Name = "_loadFileFormat"
			Me._loadFileFormat.Size = New System.Drawing.Size(150, 21)
			Me._loadFileFormat.TabIndex = 5
			'
			'ProductionExportForm
			'
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.ClientSize = New System.Drawing.Size(576, 225)
			Me.Controls.Add(Me.GroupBox2)
			Me.Controls.Add(Me.GroupBox1)
			Me.Controls.Add(Me.GroupBox3)
			Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
			Me.Menu = Me.MainMenu
			Me.Name = "ProductionExportForm"
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
			Me.Text = "Relativity Desktop Client | Export Production"
			Me.GroupBox3.ResumeLayout(False)
			Me.GroupBox1.ResumeLayout(False)
			Me.GroupBox2.ResumeLayout(False)
			Me.ResumeLayout(False)

		End Sub

#End Region

		Friend WithEvents _application As kCura.EDDS.WinForm.Application

		Protected _exportFile As kCura.WinEDDS.ExportFile

		Private Function ReadyToRun() As Boolean
			Return (Not _productionList.SelectedItem Is Nothing) AndAlso (System.IO.Directory.Exists(_folderPath.Text))
		End Function

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

		Private Sub RunMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RunMenu.Click
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			_exportFile.FolderPath = _folderPath.Text
			_exportFile.ArtifactID = CType(_productionList.SelectedValue, Int32)
			_exportFile.Overwrite = _overwriteButton.Checked
			_exportFile.ExportNative = _exportNatives.Checked
			_exportFile.LogFileFormat = CType(_loadFileFormat.SelectedValue, kCura.WinEDDS.LoadFileType.FileFormat)
			_exportFile.CookieContainer = _application.CookieContainer
			_application.StartProduction(Me.ExportFile)
			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Sub

		Private Sub _browseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _browseButton.Click
			_destinationFolderDialog.ShowDialog()
			_folderPath.Text = _destinationFolderDialog.SelectedPath()
		End Sub

		Private Sub ExportProduction_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
			_productionList.DataSource = ExportFile.DataTable
			_productionList.DisplayMember = "Name"
			_productionList.ValueMember = "ArtifactID"
			_loadFileFormat.DataSource = kCura.WinEDDS.LoadFileType.GetLoadFileTypes
			_loadFileFormat.DisplayMember = "DisplayName"
			_loadFileFormat.ValueMember = "Value"
			RunMenu.Enabled = ReadyToRun()
		End Sub

		Private Sub _folderPath_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _folderPath.TextChanged
			RunMenu.Enabled = ReadyToRun()
		End Sub

		Private Sub _productionList_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _productionList.SelectedIndexChanged
			RunMenu.Enabled = ReadyToRun()
		End Sub
	End Class
End Namespace