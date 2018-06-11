Imports System.ComponentModel
Imports System.Net
Imports System.Threading.Tasks
Imports Relativity.Transfer

Namespace kCura.EDDS.WinForm
	Public Class AdvancedFileLocation
		Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

		Public Sub New()
			MyBase.New()

			'This call is required by the Windows Form Designer.
			InitializeComponent()

			'Add any initialization after the InitializeComponent() call
			_repositories.Items.Clear()
			If Not kCura.EDDS.WinForm.Application.Instance.DocumentRepositoryList Is Nothing AndAlso kCura.EDDS.WinForm.Application.Instance.DocumentRepositoryList.Length > 0 Then
				Dim paths(kCura.EDDS.WinForm.Application.Instance.DocumentRepositoryList.Length - 1) As String
				System.Array.Copy(kCura.EDDS.WinForm.Application.Instance.DocumentRepositoryList, paths, kCura.EDDS.WinForm.Application.Instance.DocumentRepositoryList.Length)
				System.Array.Sort(paths)
				_repositories.Items.AddRange(paths)
			End If

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
		Friend WithEvents _cancelButton As System.Windows.Forms.Button
		Friend WithEvents Label1 As System.Windows.Forms.Label
		Friend WithEvents _okButton As System.Windows.Forms.Button
		Friend WithEvents _defaultButton As System.Windows.Forms.Button
		Friend WithEvents _repositories As System.Windows.Forms.ComboBox
		Friend WithEvents _copyNativeFiles As System.Windows.Forms.RadioButton
        Friend WithEvents asperaModeWarningLinkLabel As LinkLabel
        Friend WithEvents _keepNativeFiles As System.Windows.Forms.RadioButton
		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(AdvancedFileLocation))
        Me._cancelButton = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me._repositories = New System.Windows.Forms.ComboBox()
        Me._okButton = New System.Windows.Forms.Button()
        Me._defaultButton = New System.Windows.Forms.Button()
        Me._copyNativeFiles = New System.Windows.Forms.RadioButton()
        Me._keepNativeFiles = New System.Windows.Forms.RadioButton()
        Me.asperaModeWarningLinkLabel = New System.Windows.Forms.LinkLabel()
        Me.SuspendLayout
        '
        '_cancelButton
        '
        Me._cancelButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me._cancelButton.Location = New System.Drawing.Point(548, 127)
        Me._cancelButton.Name = "_cancelButton"
        Me._cancelButton.Size = New System.Drawing.Size(75, 23)
        Me._cancelButton.TabIndex = 3
        Me._cancelButton.Text = "Cancel"
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.Label1.Location = New System.Drawing.Point(12, 52)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(132, 16)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Document Repository"
        '
        '_repositories
        '
        Me._repositories.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me._repositories.Location = New System.Drawing.Point(12, 72)
        Me._repositories.Name = "_repositories"
        Me._repositories.Size = New System.Drawing.Size(560, 21)
        Me._repositories.TabIndex = 9
        '
        '_okButton
        '
        Me._okButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me._okButton.Location = New System.Drawing.Point(468, 127)
        Me._okButton.Name = "_okButton"
        Me._okButton.Size = New System.Drawing.Size(75, 23)
        Me._okButton.TabIndex = 2
        Me._okButton.Text = "OK"
        '
        '_defaultButton
        '
        Me._defaultButton.Location = New System.Drawing.Point(572, 72)
        Me._defaultButton.Name = "_defaultButton"
        Me._defaultButton.Size = New System.Drawing.Size(50, 21)
        Me._defaultButton.TabIndex = 10
        Me._defaultButton.Text = "Default"
        Me._defaultButton.TextAlign = System.Drawing.ContentAlignment.TopLeft
        '
        '_copyNativeFiles
        '
        Me._copyNativeFiles.Checked = true
        Me._copyNativeFiles.Location = New System.Drawing.Point(12, 4)
        Me._copyNativeFiles.Name = "_copyNativeFiles"
        Me._copyNativeFiles.Size = New System.Drawing.Size(384, 24)
        Me._copyNativeFiles.TabIndex = 11
        Me._copyNativeFiles.TabStop = true
        Me._copyNativeFiles.Text = "Copy files from current location to selected document repository"
        '
        '_keepNativeFiles
        '
        Me._keepNativeFiles.CheckAlign = System.Drawing.ContentAlignment.TopLeft
        Me._keepNativeFiles.Location = New System.Drawing.Point(12, 28)
        Me._keepNativeFiles.Name = "_keepNativeFiles"
        Me._keepNativeFiles.Size = New System.Drawing.Size(612, 24)
        Me._keepNativeFiles.TabIndex = 12
        Me._keepNativeFiles.Text = "Do not copy files to a Relativity Document Repository. Files already reside in a "& _ 
    "valid and Relativity-accessible location"
        Me._keepNativeFiles.TextAlign = System.Drawing.ContentAlignment.TopLeft
        '
        'asperaModeWarningLinkLabel
        '
        Me.asperaModeWarningLinkLabel.AutoSize = true
        Me.asperaModeWarningLinkLabel.LinkColor = System.Drawing.Color.Blue
        Me.asperaModeWarningLinkLabel.Location = New System.Drawing.Point(12, 96)
        Me.asperaModeWarningLinkLabel.Name = "asperaModeWarningLinkLabel"
        Me.asperaModeWarningLinkLabel.Size = New System.Drawing.Size(440, 13)
        Me.asperaModeWarningLinkLabel.TabIndex = 13
        Me.asperaModeWarningLinkLabel.TabStop = true
        Me.asperaModeWarningLinkLabel.Text = "Repository settings cannot be changed when RDC runs in Aspera mode. Click to read"& _ 
    " more."
        '
        'AdvancedFileLocation
        '
        Me.AcceptButton = Me._okButton
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.CancelButton = Me._cancelButton
        Me.ClientSize = New System.Drawing.Size(628, 156)
        Me.Controls.Add(Me.asperaModeWarningLinkLabel)
        Me.Controls.Add(Me._keepNativeFiles)
        Me.Controls.Add(Me._copyNativeFiles)
        Me.Controls.Add(Me._defaultButton)
        Me.Controls.Add(Me._repositories)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me._cancelButton)
        Me.Controls.Add(Me._okButton)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"),System.Drawing.Icon)
        Me.MaximizeBox = false
        Me.MinimizeBox = false
        Me.Name = "AdvancedFileLocation"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.Text = "File Repository Preferences"
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub

#End Region

		Friend SelectDefaultPath As Boolean = True

		Private Sub AdvancedFileLocation_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
			SetRepositoriesComboBoxAccessibility()
			_asperaModeWarningLinkLabel.Visible = IsUsingAsperaConnectionMode()
			If Me.SelectDefaultPath Then
		        Me.SelectPath(Application.Instance.SelectedCaseInfo.DocumentPath)
		    End If

		    Config.FlushEddsConfigSettings()
		    _keepNativeFiles.Visible = Not Config.CloudInstance
        End Sub

		Public Event FileLocationOK(ByVal copyFiles As Boolean, ByVal selectedRepository As String)

		Private Sub _okButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _okButton.Click
			Dim copyFilesToRepository As Boolean = _copyNativeFiles.Checked()
			Dim selectedRepository As String = ""
			If copyFilesToRepository Then selectedRepository = _repositories.SelectedItem.ToString
			Me.Close()
			RaiseEvent FileLocationOK(_copyNativeFiles.Checked, selectedRepository)
		End Sub

		Public Sub SelectPath(ByVal path As String)
			If Not path Is Nothing AndAlso Not path = "" Then
				Dim i As Int32
				Dim foundIt As Boolean = False
				For i = 0 To _repositories.Items.Count - 1
					Dim repoStr As String = _repositories.Items(i).ToString
					Dim pathStr As String = path
					If Not repoStr.EndsWith("\") Then repoStr &= "\"
					If Not pathStr.EndsWith("\") Then pathStr &= "\"
					If repoStr.ToLower = pathStr.ToLower Then
						_repositories.SelectedIndex = i
						foundIt = True
						Exit For
					End If
				Next			'
				If Not foundIt AndAlso path.ToLower <> kCura.EDDS.WinForm.Application.Instance.SelectedCaseInfo.DocumentPath.ToLower Then
					Me.SelectPath(kCura.EDDS.WinForm.Application.Instance.SelectedCaseInfo.DocumentPath)
				End If
			End If
		End Sub

        Public Sub SetRepositoriesComboBoxAccessibility()
            If IsUsingAsperaConnectionMode() Then
                _repositories.Enabled = False
            Else
                _repositories.Enabled = _copyNativeFiles.Checked
            End If
        End Sub

		Private Function IsUsingAsperaConnectionMode() As Boolean
			Return Application.Instance.SelectedTransferClientId = Guid.Parse(TransferClientConstants.AsperaClientId)
		End Function

		Private Sub _defaultButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _defaultButton.Click
			Me.SelectPath(kCura.EDDS.WinForm.Application.Instance.SelectedCaseInfo.DocumentPath)
		End Sub

		Private Sub _keepNativeFiles_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _keepNativeFiles.CheckedChanged, _copyNativeFiles.CheckedChanged
		    SetRepositoriesComboBoxAccessibility()
		End Sub

        Private Sub AsperaModeWarningLinkLabel_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles asperaModeWarningLinkLabel.LinkClicked
            MessageBox.Show(Me, "Repository settings cannot be changed when RDC runs in Aspera transfer mode. Aspera Server is " &
                "configured to upload native files to the default workspace fileshare location. " &
                "If you want to switch Repository in RDC you need to first change RDC settings in application configuration file to force Web transfer mode. " &
                "Please note that Web transfer is considerably slower than Aspera", "Relativity Desktop Client", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Sub
    End Class

End Namespace
