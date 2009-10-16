Namespace kCura.EDDS.WinForm
	Public Class ApplicationFileForm
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
		Friend WithEvents MenuFile As System.Windows.Forms.MenuItem
		Friend WithEvents MenuImport As System.Windows.Forms.MenuItem
		Friend WithEvents MenuFile_Close As System.Windows.Forms.MenuItem
		Friend WithEvents MenuImport_ImportApplication As System.Windows.Forms.MenuItem
		Friend WithEvents ApplicationName As System.Windows.Forms.TextBox
		Friend WithEvents ApplicationVersion As System.Windows.Forms.TextBox
		Friend WithEvents OpenFileDialog As System.Windows.Forms.OpenFileDialog
		Friend WithEvents ObjectList As System.Windows.Forms.ListBox
		Friend WithEvents TabList As System.Windows.Forms.ListBox
		Friend WithEvents ApplicationFileGroupBox As System.Windows.Forms.GroupBox
		Friend WithEvents ApplicationInformationGroupBox As System.Windows.Forms.GroupBox
		Friend WithEvents ApplicationArtifactsGroupBox As System.Windows.Forms.GroupBox
		Friend WithEvents MainMenu As System.Windows.Forms.MainMenu
		Friend WithEvents NameLabel As System.Windows.Forms.Label
		Friend WithEvents VersionLabel As System.Windows.Forms.Label
		Friend WithEvents ObjectsLabel As System.Windows.Forms.Label
		Friend WithEvents TabsLabel As System.Windows.Forms.Label
		Friend WithEvents FilePath As System.Windows.Forms.TextBox
		Friend WithEvents BrowseButton As System.Windows.Forms.Button
		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
			Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(ApplicationFileForm))
			Me.MainMenu = New System.Windows.Forms.MainMenu
			Me.MenuFile = New System.Windows.Forms.MenuItem
			Me.MenuFile_Close = New System.Windows.Forms.MenuItem
			Me.MenuImport = New System.Windows.Forms.MenuItem
			Me.MenuImport_ImportApplication = New System.Windows.Forms.MenuItem
			Me.ApplicationFileGroupBox = New System.Windows.Forms.GroupBox
			Me.BrowseButton = New System.Windows.Forms.Button
			Me.FilePath = New System.Windows.Forms.TextBox
			Me.ApplicationInformationGroupBox = New System.Windows.Forms.GroupBox
			Me.ApplicationName = New System.Windows.Forms.TextBox
			Me.ApplicationVersion = New System.Windows.Forms.TextBox
			Me.VersionLabel = New System.Windows.Forms.Label
			Me.NameLabel = New System.Windows.Forms.Label
			Me.ApplicationArtifactsGroupBox = New System.Windows.Forms.GroupBox
			Me.TabList = New System.Windows.Forms.ListBox
			Me.ObjectList = New System.Windows.Forms.ListBox
			Me.TabsLabel = New System.Windows.Forms.Label
			Me.ObjectsLabel = New System.Windows.Forms.Label
			Me.OpenFileDialog = New System.Windows.Forms.OpenFileDialog
			Me.ApplicationFileGroupBox.SuspendLayout()
			Me.ApplicationInformationGroupBox.SuspendLayout()
			Me.ApplicationArtifactsGroupBox.SuspendLayout()
			Me.SuspendLayout()
			'
			'MainMenu
			'
			Me.MainMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuFile, Me.MenuImport})
			'
			'MenuFile
			'
			Me.MenuFile.Index = 0
			Me.MenuFile.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuFile_Close})
			Me.MenuFile.Text = "&File"
			'
			'MenuFile_Close
			'
			Me.MenuFile_Close.Index = 0
			Me.MenuFile_Close.Text = "&Close"
			'
			'MenuImport
			'
			Me.MenuImport.Index = 1
			Me.MenuImport.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuImport_ImportApplication})
			Me.MenuImport.Text = "&Import"
			'
			'MenuImport_ImportApplication
			'
			Me.MenuImport_ImportApplication.Index = 0
			Me.MenuImport_ImportApplication.Text = "Import &Application"
			'
			'ApplicationFileGroupBox
			'
			Me.ApplicationFileGroupBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
									Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.ApplicationFileGroupBox.Controls.Add(Me.BrowseButton)
			Me.ApplicationFileGroupBox.Controls.Add(Me.FilePath)
			Me.ApplicationFileGroupBox.Location = New System.Drawing.Point(16, 24)
			Me.ApplicationFileGroupBox.Name = "ApplicationFileGroupBox"
			Me.ApplicationFileGroupBox.Size = New System.Drawing.Size(720, 48)
			Me.ApplicationFileGroupBox.TabIndex = 22
			Me.ApplicationFileGroupBox.TabStop = False
			Me.ApplicationFileGroupBox.Text = "Application File"
			'
			'BrowseButton
			'
			Me.BrowseButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.BrowseButton.Location = New System.Drawing.Point(688, 16)
			Me.BrowseButton.Name = "BrowseButton"
			Me.BrowseButton.Size = New System.Drawing.Size(24, 20)
			Me.BrowseButton.TabIndex = 4
			Me.BrowseButton.Text = "..."
			'
			'FilePath
			'
			Me.FilePath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
									Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.FilePath.BackColor = System.Drawing.SystemColors.ControlLightLight
			Me.FilePath.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me.FilePath.ForeColor = System.Drawing.SystemColors.ControlDarkDark
			Me.FilePath.Location = New System.Drawing.Point(8, 16)
			Me.FilePath.Name = "FilePath"
			Me.FilePath.Size = New System.Drawing.Size(680, 20)
			Me.FilePath.TabIndex = 2
			Me.FilePath.Text = "Select a file ..."
			'
			'ApplicationInformationGroupBox
			'
			Me.ApplicationInformationGroupBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
									Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.ApplicationInformationGroupBox.Controls.Add(Me.ApplicationName)
			Me.ApplicationInformationGroupBox.Controls.Add(Me.ApplicationVersion)
			Me.ApplicationInformationGroupBox.Controls.Add(Me.VersionLabel)
			Me.ApplicationInformationGroupBox.Controls.Add(Me.NameLabel)
			Me.ApplicationInformationGroupBox.Location = New System.Drawing.Point(16, 88)
			Me.ApplicationInformationGroupBox.Name = "ApplicationInformationGroupBox"
			Me.ApplicationInformationGroupBox.Size = New System.Drawing.Size(720, 88)
			Me.ApplicationInformationGroupBox.TabIndex = 23
			Me.ApplicationInformationGroupBox.TabStop = False
			Me.ApplicationInformationGroupBox.Text = "Application Information"
			'
			'ApplicationName
			'
			Me.ApplicationName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
									Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.ApplicationName.Location = New System.Drawing.Point(72, 24)
			Me.ApplicationName.Name = "ApplicationName"
			Me.ApplicationName.ReadOnly = True
			Me.ApplicationName.Size = New System.Drawing.Size(624, 20)
			Me.ApplicationName.TabIndex = 3
			Me.ApplicationName.Text = ""
			'
			'ApplicationVersion
			'
			Me.ApplicationVersion.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
									Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.ApplicationVersion.Location = New System.Drawing.Point(72, 56)
			Me.ApplicationVersion.Name = "ApplicationVersion"
			Me.ApplicationVersion.ReadOnly = True
			Me.ApplicationVersion.Size = New System.Drawing.Size(624, 20)
			Me.ApplicationVersion.TabIndex = 2
			Me.ApplicationVersion.Text = ""
			'
			'VersionLabel
			'
			Me.VersionLabel.Location = New System.Drawing.Point(16, 56)
			Me.VersionLabel.Name = "VersionLabel"
			Me.VersionLabel.Size = New System.Drawing.Size(48, 23)
			Me.VersionLabel.TabIndex = 1
			Me.VersionLabel.Text = "Version"
			'
			'NameLabel
			'
			Me.NameLabel.Location = New System.Drawing.Point(16, 24)
			Me.NameLabel.Name = "NameLabel"
			Me.NameLabel.Size = New System.Drawing.Size(48, 23)
			Me.NameLabel.TabIndex = 0
			Me.NameLabel.Text = "Name:"
			'
			'ApplicationArtifactsGroupBox
			'
			Me.ApplicationArtifactsGroupBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
									Or System.Windows.Forms.AnchorStyles.Left) _
									Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.ApplicationArtifactsGroupBox.Controls.Add(Me.TabList)
			Me.ApplicationArtifactsGroupBox.Controls.Add(Me.ObjectList)
			Me.ApplicationArtifactsGroupBox.Controls.Add(Me.TabsLabel)
			Me.ApplicationArtifactsGroupBox.Controls.Add(Me.ObjectsLabel)
			Me.ApplicationArtifactsGroupBox.Location = New System.Drawing.Point(16, 192)
			Me.ApplicationArtifactsGroupBox.Name = "ApplicationArtifactsGroupBox"
			Me.ApplicationArtifactsGroupBox.Size = New System.Drawing.Size(720, 360)
			Me.ApplicationArtifactsGroupBox.TabIndex = 24
			Me.ApplicationArtifactsGroupBox.TabStop = False
			Me.ApplicationArtifactsGroupBox.Text = "Application Artifacts"
			'
			'TabList
			'
			Me.TabList.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
									Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.TabList.Location = New System.Drawing.Point(72, 232)
			Me.TabList.Name = "TabList"
			Me.TabList.Size = New System.Drawing.Size(624, 108)
			Me.TabList.TabIndex = 5
			'
			'ObjectList
			'
			Me.ObjectList.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
									Or System.Windows.Forms.AnchorStyles.Left) _
									Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.ObjectList.Location = New System.Drawing.Point(72, 32)
			Me.ObjectList.Name = "ObjectList"
			Me.ObjectList.Size = New System.Drawing.Size(624, 173)
			Me.ObjectList.TabIndex = 4
			'
			'TabsLabel
			'
			Me.TabsLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
			Me.TabsLabel.Location = New System.Drawing.Point(16, 232)
			Me.TabsLabel.Name = "TabsLabel"
			Me.TabsLabel.Size = New System.Drawing.Size(48, 23)
			Me.TabsLabel.TabIndex = 2
			Me.TabsLabel.Text = "Tabs:"
			'
			'ObjectsLabel
			'
			Me.ObjectsLabel.Location = New System.Drawing.Point(16, 32)
			Me.ObjectsLabel.Name = "ObjectsLabel"
			Me.ObjectsLabel.Size = New System.Drawing.Size(48, 23)
			Me.ObjectsLabel.TabIndex = 1
			Me.ObjectsLabel.Text = "Objects:"
			'
			'OpenFileDialog
			'
			Me.OpenFileDialog.Filter = "XML Files (*.xml)|*.xml"
			'
			'ApplicationFileForm
			'
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.ClientSize = New System.Drawing.Size(752, 566)
			Me.Controls.Add(Me.ApplicationArtifactsGroupBox)
			Me.Controls.Add(Me.ApplicationInformationGroupBox)
			Me.Controls.Add(Me.ApplicationFileGroupBox)
			Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
			Me.Menu = Me.MainMenu
			Me.MinimumSize = New System.Drawing.Size(760, 621)
			Me.Name = "ApplicationFileForm"
			Me.Text = "Relativity Desktop Client | Import Applicatioin File"
			Me.ApplicationFileGroupBox.ResumeLayout(False)
			Me.ApplicationInformationGroupBox.ResumeLayout(False)
			Me.ApplicationArtifactsGroupBox.ResumeLayout(False)
			Me.ResumeLayout(False)

		End Sub

#End Region

#Region " Event Handlers "

		Private Sub BrowseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BrowseButton.Click
			OpenFileDialog.ShowDialog()
		End Sub

		Private Sub MenuFile_Close_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles MenuFile_Close.Click
			Me.Close()
		End Sub

		Private Sub MenuImport_ImportApplication_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles MenuImport_ImportApplication.Click
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

			Dim installationParameters As New kCura.EDDS.WebAPI.TemplateManagerBase.ApplicationInstallationParameters
			installationParameters.CaseId = Me.CaseInfo.ArtifactID

			Dim applicationDeploymentSystem As New WinEDDS.Service.TemplateManager(Me.Credentials, Me.CookieContainer)
			Dim installationResult As kCura.EDDS.WebAPI.TemplateManagerBase.ApplicationInstallationResult = applicationDeploymentSystem.InstallTemplate(_document, installationParameters)

			If installationResult.Success Then
				Dim installedArtifacts As New System.Text.StringBuilder
				For Each applicationArtifact As kCura.EDDS.WebAPI.TemplateManagerBase.ApplicationArtifact In installationResult.ApplicationArtifacts
					installedArtifacts.AppendFormat(System.Globalization.CultureInfo.CurrentCulture, "Created {0}: {1} (ID = {2}){3}", applicationArtifact.Type, applicationArtifact.Name, applicationArtifact.ArtifactId, System.Environment.NewLine)
				Next
				MsgBox(String.Format(System.Globalization.CultureInfo.CurrentCulture, "Installation successful.{0}{0}{1}", System.Environment.NewLine, installedArtifacts))
			Else
				MsgBox(String.Format(System.Globalization.CultureInfo.CurrentCulture, "Error installing Application: {0}", installationResult.ExceptionMessage))
			End If

			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Sub

		Private Sub OpenFileDialog_FileOk(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog.FileOk
			Dim document As New Xml.XmlDocument
			Dim fileIsValid As Boolean
			Dim errorMessage As String = String.Empty

			fileIsValid = LoadFileIntoXML(document, OpenFileDialog.FileName)

			If fileIsValid Then
				'check the name
				fileIsValid = LoadApplicationNameFromNode(document.SelectSingleNode("/Application/Name"))
			Else
				errorMessage = "Invalid file type.  Files must be xml files."
			End If

			If fileIsValid Then
				'check the version
				fileIsValid = LoadApplicationVersionFromNode(document.SelectSingleNode("/Application/Version"))
			ElseIf errorMessage = String.Empty Then
				errorMessage = "Invalid application file.  'Name' property is missing."
			End If

			If fileIsValid Then
				'check the objects
				fileIsValid = LoadApplicationObjectsFromNodes(document.SelectNodes("/Application/Objects/Object/Name"))
			ElseIf errorMessage = String.Empty Then
				errorMessage = "Invalid application file.  'Version' property is missing."
			End If

			If fileIsValid Then
				fileIsValid = LoadApplicationTabsFromNodes(document.SelectNodes("/Application/ExternalTabs/ExternalTab/Name"))
			ElseIf errorMessage = String.Empty Then
				errorMessage = "Invalid applicagtion file.  There are no Object Types to install."
			End If

			If fileIsValid Then
				FilePath.Text = OpenFileDialog.FileName
				_document = document
			Else
				MsgBox(String.Format(System.Globalization.CultureInfo.CurrentCulture, "Invalid File: {0}", errorMessage))
			End If
		End Sub

#End Region

#Region " Friend Properties "

		Friend Property Application() As kCura.EDDS.WinForm.Application
			Get
				Return _application
			End Get
			Set(ByVal Value As kCura.EDDS.WinForm.Application)
				_application = Value
			End Set
		End Property

		Friend Property CaseInfo() As kCura.EDDS.Types.CaseInfo
			Get
				Return _caseInfo
			End Get
			Set(ByVal Value As kCura.EDDS.Types.CaseInfo)
				_caseInfo = Value
			End Set
		End Property

		Friend Property CookieContainer() As System.Net.CookieContainer
			Get
				Return _cookieContainer
			End Get
			Set(ByVal Value As System.Net.CookieContainer)
				_cookieContainer = Value
			End Set
		End Property

		Friend Property Credentials() As System.Net.NetworkCredential
			Get
				Return _credentials
			End Get
			Set(ByVal Value As System.Net.NetworkCredential)
				_credentials = Value
			End Set
		End Property

#End Region

#Region " Private Fields "

		Private WithEvents _application As kCura.EDDS.WinForm.Application
		Private _caseInfo As kCura.EDDS.Types.CaseInfo
		Private _cookieContainer As System.Net.CookieContainer
		Private _credentials As System.Net.NetworkCredential
		Private _document As Xml.XmlDocument

#End Region

#Region " Private Methods "

		Private Function LoadApplicationNameFromNode(ByVal node As Xml.XmlNode) As Boolean
			Dim result As Boolean
			If node Is Nothing Then
				result = False
			Else
				ApplicationName.Text = node.InnerText
				result = True
			End If
			Return result
		End Function

		Private Function LoadApplicationObjectsFromNodes(ByVal nodes As Xml.XmlNodeList) As Boolean
			Dim result As Boolean
			If nodes.Count > 0 Then
				ObjectList.Items.Clear()
				For Each node As Xml.XmlNode In nodes
					ObjectList.Items.Add(node.InnerText)
				Next
				result = True
			Else
				result = False
			End If
			Return result
		End Function

		Private Function LoadApplicationTabsFromNodes(ByVal nodes As Xml.XmlNodeList) As Boolean
			Dim result As Boolean
			TabList.Items.Clear()
			For Each node As Xml.XmlNode In nodes
				TabList.Items.Add(node.InnerText)
			Next
			result = True
			Return result
		End Function

		Private Function LoadApplicationVersionFromNode(ByVal node As Xml.XmlNode) As Boolean
			Dim result As Boolean
			If node Is Nothing Then
				result = False
			Else
				ApplicationVersion.Text = node.InnerText
				result = True
			End If
			Return result
		End Function

		Private Function LoadFileIntoXML(ByVal document As Xml.XmlDocument, ByVal filePath As String) As Boolean
			Dim result As Boolean
			If Not OpenFileDialog.FileName.EndsWith(".xml") Then
				result = False
			Else
				Try
					document.Load(OpenFileDialog.FileName)
					result = True
				Catch ex As Exception
					result = False
				End Try
			End If
			Return result
		End Function

#End Region

	End Class
End Namespace