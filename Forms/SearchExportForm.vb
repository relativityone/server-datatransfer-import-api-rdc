Public Class SearchExportForm
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
	Friend WithEvents _multiRecordDelimiter As System.Windows.Forms.ComboBox
	Friend WithEvents Label6 As System.Windows.Forms.Label
	Friend WithEvents Label4 As System.Windows.Forms.Label
	Friend WithEvents _quoteDelimiter As System.Windows.Forms.ComboBox
	Friend WithEvents Label3 As System.Windows.Forms.Label
	Friend WithEvents _newLineDelimiter As System.Windows.Forms.ComboBox
	Friend WithEvents Label2 As System.Windows.Forms.Label
	Friend WithEvents _recordDelimiter As System.Windows.Forms.ComboBox
	Friend WithEvents ExportMenu As System.Windows.Forms.MenuItem
	Friend WithEvents RunMenu As System.Windows.Forms.MenuItem
	Friend WithEvents _searchList As System.Windows.Forms.ComboBox
	Friend WithEvents _destinationFolderDialog As System.Windows.Forms.FolderBrowserDialog
	Friend WithEvents _searchesBox As System.Windows.Forms.GroupBox
	Friend WithEvents _exportFullText As System.Windows.Forms.CheckBox
	Friend WithEvents _exportNativeFiles As System.Windows.Forms.CheckBox
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
		Me.MainMenu1 = New System.Windows.Forms.MainMenu
		Me.ExportMenu = New System.Windows.Forms.MenuItem
		Me.RunMenu = New System.Windows.Forms.MenuItem
		Me._searchesBox = New System.Windows.Forms.GroupBox
		Me._exportNativeFiles = New System.Windows.Forms.CheckBox
		Me._exportFullText = New System.Windows.Forms.CheckBox
		Me._searchList = New System.Windows.Forms.ComboBox
		Me.GroupBox3 = New System.Windows.Forms.GroupBox
		Me._overwriteButton = New System.Windows.Forms.CheckBox
		Me._browseButton = New System.Windows.Forms.Button
		Me._folderPath = New System.Windows.Forms.TextBox
		Me.GroupBox23 = New System.Windows.Forms.GroupBox
		Me._multiRecordDelimiter = New System.Windows.Forms.ComboBox
		Me.Label6 = New System.Windows.Forms.Label
		Me.Label4 = New System.Windows.Forms.Label
		Me._quoteDelimiter = New System.Windows.Forms.ComboBox
		Me.Label3 = New System.Windows.Forms.Label
		Me._newLineDelimiter = New System.Windows.Forms.ComboBox
		Me.Label2 = New System.Windows.Forms.Label
		Me._recordDelimiter = New System.Windows.Forms.ComboBox
		Me._destinationFolderDialog = New System.Windows.Forms.FolderBrowserDialog
		Me._searchesBox.SuspendLayout()
		Me.GroupBox3.SuspendLayout()
		Me.GroupBox23.SuspendLayout()
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
		Me.ExportMenu.Text = "Export"
		'
		'RunMenu
		'
		Me.RunMenu.Index = 0
		Me.RunMenu.Shortcut = System.Windows.Forms.Shortcut.F5
		Me.RunMenu.Text = "Run..."
		'
		'_searchesBox
		'
		Me._searchesBox.Controls.Add(Me._exportNativeFiles)
		Me._searchesBox.Controls.Add(Me._exportFullText)
		Me._searchesBox.Controls.Add(Me._searchList)
		Me._searchesBox.Location = New System.Drawing.Point(4, 4)
		Me._searchesBox.Name = "_searchesBox"
		Me._searchesBox.Size = New System.Drawing.Size(568, 80)
		Me._searchesBox.TabIndex = 10
		Me._searchesBox.TabStop = False
		Me._searchesBox.Text = "Searches"
		'
		'_exportNativeFiles
		'
		Me._exportNativeFiles.Checked = True
		Me._exportNativeFiles.CheckState = System.Windows.Forms.CheckState.Checked
		Me._exportNativeFiles.Location = New System.Drawing.Point(128, 48)
		Me._exportNativeFiles.Name = "_exportNativeFiles"
		Me._exportNativeFiles.Size = New System.Drawing.Size(124, 20)
		Me._exportNativeFiles.TabIndex = 3
		Me._exportNativeFiles.Text = "Export Native Files"
		'
		'_exportFullText
		'
		Me._exportFullText.Location = New System.Drawing.Point(8, 48)
		Me._exportFullText.Name = "_exportFullText"
		Me._exportFullText.Size = New System.Drawing.Size(108, 20)
		Me._exportFullText.TabIndex = 2
		Me._exportFullText.Text = "Export Full Text"
		'
		'_searchList
		'
		Me._searchList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._searchList.Location = New System.Drawing.Point(8, 20)
		Me._searchList.Name = "_searchList"
		Me._searchList.Size = New System.Drawing.Size(552, 21)
		Me._searchList.TabIndex = 1
		'
		'GroupBox3
		'
		Me.GroupBox3.Controls.Add(Me._overwriteButton)
		Me.GroupBox3.Controls.Add(Me._browseButton)
		Me.GroupBox3.Controls.Add(Me._folderPath)
		Me.GroupBox3.Location = New System.Drawing.Point(4, 92)
		Me.GroupBox3.Name = "GroupBox3"
		Me.GroupBox3.Size = New System.Drawing.Size(568, 72)
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
		Me._folderPath.TabIndex = 5
		Me._folderPath.Text = "Select a file ..."
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
		Me.GroupBox23.Location = New System.Drawing.Point(4, 172)
		Me.GroupBox23.Name = "GroupBox23"
		Me.GroupBox23.Size = New System.Drawing.Size(568, 68)
		Me.GroupBox23.TabIndex = 12
		Me.GroupBox23.TabStop = False
		Me.GroupBox23.Text = "Characters"
		'
		'_multiRecordDelimiter
		'
		Me._multiRecordDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._multiRecordDelimiter.Location = New System.Drawing.Point(404, 36)
		Me._multiRecordDelimiter.Name = "_multiRecordDelimiter"
		Me._multiRecordDelimiter.Size = New System.Drawing.Size(124, 21)
		Me._multiRecordDelimiter.TabIndex = 7
		'
		'Label6
		'
		Me.Label6.Location = New System.Drawing.Point(404, 20)
		Me.Label6.Name = "Label6"
		Me.Label6.Size = New System.Drawing.Size(120, 16)
		Me.Label6.TabIndex = 6
		Me.Label6.Text = "Multi-Record Delimiter"
		'
		'Label4
		'
		Me.Label4.Location = New System.Drawing.Point(272, 20)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New System.Drawing.Size(100, 16)
		Me.Label4.TabIndex = 5
		Me.Label4.Text = "Quote"
		'
		'_quoteDelimiter
		'
		Me._quoteDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._quoteDelimiter.Location = New System.Drawing.Point(272, 36)
		Me._quoteDelimiter.Name = "_quoteDelimiter"
		Me._quoteDelimiter.Size = New System.Drawing.Size(124, 21)
		Me._quoteDelimiter.TabIndex = 4
		'
		'Label3
		'
		Me.Label3.Location = New System.Drawing.Point(140, 20)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New System.Drawing.Size(100, 16)
		Me.Label3.TabIndex = 3
		Me.Label3.Text = "Newline"
		'
		'_newLineDelimiter
		'
		Me._newLineDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me._newLineDelimiter.Location = New System.Drawing.Point(140, 36)
		Me._newLineDelimiter.Name = "_newLineDelimiter"
		Me._newLineDelimiter.Size = New System.Drawing.Size(124, 21)
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
		Me._recordDelimiter.Size = New System.Drawing.Size(124, 21)
		Me._recordDelimiter.TabIndex = 0
		'
		'SearchExportForm
		'
		Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
		Me.ClientSize = New System.Drawing.Size(576, 245)
		Me.Controls.Add(Me.GroupBox23)
		Me.Controls.Add(Me.GroupBox3)
		Me.Controls.Add(Me._searchesBox)
		Me.Menu = Me.MainMenu1
		Me.Name = "SearchExportForm"
		Me.Text = "SearchExportForm"
		Me._searchesBox.ResumeLayout(False)
		Me.GroupBox3.ResumeLayout(False)
		Me.GroupBox23.ResumeLayout(False)
		Me.ResumeLayout(False)

	End Sub

#End Region

	Private WithEvents _application As kCura.EDDS.WinForm.Application
	Protected _exportFile As kCura.WinEDDS.ExportFile

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
		Return System.IO.Directory.Exists(_folderPath.Text) AndAlso Not _searchList.SelectedItem Is Nothing
	End Function

	Private Sub RunMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RunMenu.Click
		Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
		_exportFile.FolderPath = _folderPath.Text
		If Me.ExportFile.TypeOfExport = ExportFile.ExportType.ArtifactSearch Then
			_searchesBox.Text = "Searches"
			_exportFile.ArtifactID = CType(_searchList.SelectedValue, Int32)
		Else
			_searchesBox.Text = "Views"
			_exportFile.ViewID = CType(_searchList.SelectedValue, Int32)
		End If
		_exportFile.Overwrite = _overwriteButton.Checked
		_exportFile.ExportFullText = _exportFullText.Checked
		_exportFile.ExportNative = _exportNativeFiles.Checked
		_exportFile.QuoteDelimiter = Chr(CType(_quoteDelimiter.SelectedValue, Int32))
		_exportFile.RecordDelimiter = Chr(CType(_recordDelimiter.SelectedValue, Int32))
		_exportFile.MultiRecordDelimiter = Chr(CType(_multiRecordDelimiter.SelectedValue, Int32))
		_exportFile.NewlineDelimiter = Chr(CType(_newLineDelimiter.SelectedValue, Int32))
		_exportFile.CookieContainer = _application.CookieContainer

		_application.StartSearch(Me.ExportFile)
		Me.Cursor = System.Windows.Forms.Cursors.Default
	End Sub

	Private Sub _browseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _browseButton.Click
		_destinationFolderDialog.ShowDialog()
		_folderPath.Text = _destinationFolderDialog.SelectedPath()
	End Sub

	Private Sub ExportProduction_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
		_searchList.DataSource = ExportFile.DataTable
		_searchList.DisplayMember = "Name"
		_searchList.ValueMember = "ArtifactID"

		kCura.EDDS.WinForm.Utility.InitializeCharacterDropDown(_recordDelimiter, _exportFile.RecordDelimiter)
		kCura.EDDS.WinForm.Utility.InitializeCharacterDropDown(_quoteDelimiter, _exportFile.QuoteDelimiter)
		kCura.EDDS.WinForm.Utility.InitializeCharacterDropDown(_newLineDelimiter, _exportFile.NewlineDelimiter)
		kCura.EDDS.WinForm.Utility.InitializeCharacterDropDown(_multiRecordDelimiter, _exportFile.MultiRecordDelimiter)
		RunMenu.Enabled = ReadyToRun()
	End Sub

	Private Sub _searchList_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _searchList.SelectedIndexChanged
		RunMenu.Enabled = ReadyToRun()
	End Sub

	Private Sub _folderPath_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _folderPath.TextChanged
		RunMenu.Enabled = ReadyToRun()
	End Sub
End Class