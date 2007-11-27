Namespace kCura.EDDS.WinForm
	Public Class AdvancedFileLocation
		Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

		Public Sub New()
			MyBase.New()

			'This call is required by the Windows Form Designer.
			InitializeComponent()

			'Add any initialization after the InitializeComponent() call
			_application = kCura.EDDS.WinForm.Application.Instance
			_repositories.Items.Clear()
			If Not _application.DocumentRepositoryList Is Nothing AndAlso _application.DocumentRepositoryList.Length > 0 Then
				Dim paths(_application.DocumentRepositoryList.Length - 1) As String
				System.Array.Copy(_application.DocumentRepositoryList, paths, _application.DocumentRepositoryList.Length)
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
		Friend WithEvents _copyFilesToRepository As System.Windows.Forms.CheckBox
		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
			Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(AdvancedFileLocation))
			Me._cancelButton = New System.Windows.Forms.Button
			Me.Label1 = New System.Windows.Forms.Label
			Me._copyFilesToRepository = New System.Windows.Forms.CheckBox
			Me._repositories = New System.Windows.Forms.ComboBox
			Me._okButton = New System.Windows.Forms.Button
			Me._defaultButton = New System.Windows.Forms.Button
			Me.SuspendLayout()
			'
			'_cancelButton
			'
			Me._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me._cancelButton.Location = New System.Drawing.Point(540, 112)
			Me._cancelButton.Name = "_cancelButton"
			Me._cancelButton.TabIndex = 3
			Me._cancelButton.Text = "Cancel"
			'
			'Label1
			'
			Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me.Label1.Location = New System.Drawing.Point(40, 40)
			Me.Label1.Name = "Label1"
			Me.Label1.Size = New System.Drawing.Size(132, 16)
			Me.Label1.TabIndex = 4
			Me.Label1.Text = "Document Repository"
			'
			'_copyFilesToRepository
			'
			Me._copyFilesToRepository.Checked = True
			Me._copyFilesToRepository.CheckState = System.Windows.Forms.CheckState.Checked
			Me._copyFilesToRepository.Location = New System.Drawing.Point(8, 8)
			Me._copyFilesToRepository.Name = "_copyFilesToRepository"
			Me._copyFilesToRepository.Size = New System.Drawing.Size(140, 24)
			Me._copyFilesToRepository.TabIndex = 8
			Me._copyFilesToRepository.Text = "Copy files to repository"
			'
			'_repositories
			'
			Me._repositories.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._repositories.Location = New System.Drawing.Point(40, 60)
			Me._repositories.Name = "_repositories"
			Me._repositories.Size = New System.Drawing.Size(504, 21)
			Me._repositories.TabIndex = 9
			'
			'_okButton
			'
			Me._okButton.Location = New System.Drawing.Point(460, 112)
			Me._okButton.Name = "_okButton"
			Me._okButton.TabIndex = 2
			Me._okButton.Text = "OK"
			'
			'_defaultButton
			'
			Me._defaultButton.Location = New System.Drawing.Point(544, 60)
			Me._defaultButton.Name = "_defaultButton"
			Me._defaultButton.Size = New System.Drawing.Size(48, 21)
			Me._defaultButton.TabIndex = 10
			Me._defaultButton.Text = "Default"
			Me._defaultButton.TextAlign = System.Drawing.ContentAlignment.TopLeft
			'
			'AdvancedFileLocation
			'
			Me.AcceptButton = Me._okButton
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.CancelButton = Me._cancelButton
			Me.ClientSize = New System.Drawing.Size(616, 137)
			Me.Controls.Add(Me._defaultButton)
			Me.Controls.Add(Me._repositories)
			Me.Controls.Add(Me._copyFilesToRepository)
			Me.Controls.Add(Me.Label1)
			Me.Controls.Add(Me._cancelButton)
			Me.Controls.Add(Me._okButton)
			Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
			Me.MaximizeBox = False
			Me.MinimizeBox = False
			Me.Name = "AdvancedFileLocation"
			Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
			Me.Text = "File Repository Preferences"
			Me.ResumeLayout(False)

		End Sub

#End Region

		Private WithEvents _application As kCura.EDDS.WinForm.Application
		Friend SelectDefaultPath As Boolean = True
		Private Sub _includeOriginals_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _copyFilesToRepository.CheckedChanged
			_repositories.Enabled = _copyFilesToRepository.Checked
		End Sub

		Private Sub AdvancedFileLocation_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
			If Me.SelectDefaultPath Then Me.SelectPath(_application.SelectedCaseInfo.DocumentPath)
		End Sub

		Public Event FileLocationOK(ByVal copyFiles As Boolean, ByVal selectedRepository As String)

		Private Sub _okButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _okButton.Click
			Dim copyFilesToRepository As Boolean = _copyFilesToRepository.Checked()
			Dim selectedRepository As String = ""
			If copyFilesToRepository Then selectedRepository = _repositories.SelectedItem.ToString
			Me.Close()
			RaiseEvent FileLocationOK(_copyFilesToRepository.Checked, selectedRepository)
		End Sub

		Public Sub SelectPath(ByVal path As String)
			If Not path Is Nothing AndAlso Not path = "" Then
				Dim i As Int32
				Dim foundIt As Boolean = False
				For i = 0 To _repositories.Items.Count - 1
					If _repositories.Items(i).ToString.ToLower = path.ToLower Then
						_repositories.SelectedIndex = i
						foundIt = True
						Exit For
					End If
				Next		'
				If Not foundIt AndAlso path.ToLower <> _application.SelectedCaseInfo.DocumentPath.ToLower Then
					Me.SelectPath(_application.SelectedCaseInfo.DocumentPath)
				End If
			End If
		End Sub

		Private Sub _defaultButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _defaultButton.Click
			Me.SelectPath(_application.SelectedCaseInfo.DocumentPath)
		End Sub

	End Class

End Namespace
