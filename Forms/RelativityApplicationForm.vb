Imports System.Xml
Imports System.Xml.Linq

Namespace kCura.EDDS.WinForm
	Public Class RelativityApplicationForm
		Inherits System.Windows.Forms.Form

		Private WithEvents OpenFileDialog As OpenFileDialog
		Friend WithEvents MenuFile_Refresh As System.Windows.Forms.MenuItem
		Friend WithEvents MenuFile_Separator As System.Windows.Forms.MenuItem
		Private _caseInfos As Generic.List(Of Relativity.CaseInfo)
		Private _document As Xml.XmlDocument
		Private _filename As String

#Region " Windows Form Designer generated code "

		Public Sub New()
			MyBase.New()

			'This call is required by the Windows Form Designer.
			InitializeComponent()

			'Add any initialization after the InitializeComponent() call
			OpenFileDialog = New OpenFileDialog
			OpenFileDialog.Filter = "XML Files (*.xml)|*.xml"
			OpenFileDialog.InitialDirectory = IO.Path.Combine(IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), "Applications")
			OpenFileDialog.CheckFileExists = True
			OpenFileDialog.CheckPathExists = True
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
		Friend WithEvents ApplicationFileGroupBox As System.Windows.Forms.GroupBox
		Friend WithEvents ApplicationInformationGroupBox As System.Windows.Forms.GroupBox
		Friend WithEvents ApplicationArtifactsGroupBox As System.Windows.Forms.GroupBox
		Friend WithEvents MainMenu As System.Windows.Forms.MainMenu
		Friend WithEvents NameLabel As System.Windows.Forms.Label
		Friend WithEvents VersionLabel As System.Windows.Forms.Label
		Friend WithEvents FilePath As System.Windows.Forms.TextBox
		Friend WithEvents TreeView1 As System.Windows.Forms.TreeView
		Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
		Friend WithEvents BrowseCasesButton As System.Windows.Forms.Button
		Friend WithEvents CaseListTextBox As System.Windows.Forms.TextBox
		Friend WithEvents BrowseButton As System.Windows.Forms.Button
		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
			Me.components = New System.ComponentModel.Container()
			Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(RelativityApplicationForm))
			Me.MainMenu = New System.Windows.Forms.MainMenu(Me.components)
			Me.MenuFile = New System.Windows.Forms.MenuItem()
			Me.MenuFile_Refresh = New System.Windows.Forms.MenuItem()
			Me.MenuFile_Separator = New System.Windows.Forms.MenuItem()
			Me.MenuFile_Close = New System.Windows.Forms.MenuItem()
			Me.MenuImport = New System.Windows.Forms.MenuItem()
			Me.MenuImport_ImportApplication = New System.Windows.Forms.MenuItem()
			Me.ApplicationFileGroupBox = New System.Windows.Forms.GroupBox()
			Me.BrowseButton = New System.Windows.Forms.Button()
			Me.FilePath = New System.Windows.Forms.TextBox()
			Me.ApplicationInformationGroupBox = New System.Windows.Forms.GroupBox()
			Me.ApplicationName = New System.Windows.Forms.TextBox()
			Me.ApplicationVersion = New System.Windows.Forms.TextBox()
			Me.VersionLabel = New System.Windows.Forms.Label()
			Me.NameLabel = New System.Windows.Forms.Label()
			Me.ApplicationArtifactsGroupBox = New System.Windows.Forms.GroupBox()
			Me.TreeView1 = New System.Windows.Forms.TreeView()
			Me.GroupBox1 = New System.Windows.Forms.GroupBox()
			Me.BrowseCasesButton = New System.Windows.Forms.Button()
			Me.CaseListTextBox = New System.Windows.Forms.TextBox()
			Me.ApplicationFileGroupBox.SuspendLayout()
			Me.ApplicationInformationGroupBox.SuspendLayout()
			Me.ApplicationArtifactsGroupBox.SuspendLayout()
			Me.GroupBox1.SuspendLayout()
			Me.SuspendLayout()
			'
			'MainMenu
			'
			Me.MainMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuFile, Me.MenuImport})
			'
			'MenuFile
			'
			Me.MenuFile.Index = 0
			Me.MenuFile.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuFile_Refresh, Me.MenuFile_Separator, Me.MenuFile_Close})
			Me.MenuFile.Text = "&File"
			'
			'MenuFile_Refresh
			'
			Me.MenuFile_Refresh.Enabled = False
			Me.MenuFile_Refresh.Index = 0
			Me.MenuFile_Refresh.Shortcut = System.Windows.Forms.Shortcut.F5
			Me.MenuFile_Refresh.Text = "&Refresh"
			'
			'MenuFile_Separator
			'
			Me.MenuFile_Separator.Index = 1
			Me.MenuFile_Separator.Text = "-"
			'
			'MenuFile_Close
			'
			Me.MenuFile_Close.Index = 2
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
			Me.MenuImport_ImportApplication.Enabled = False
			Me.MenuImport_ImportApplication.Index = 0
			Me.MenuImport_ImportApplication.Shortcut = System.Windows.Forms.Shortcut.F4
			Me.MenuImport_ImportApplication.Text = "Import &Application"
			'
			'ApplicationFileGroupBox
			'
			Me.ApplicationFileGroupBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
						Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.ApplicationFileGroupBox.Controls.Add(Me.BrowseButton)
			Me.ApplicationFileGroupBox.Controls.Add(Me.FilePath)
			Me.ApplicationFileGroupBox.Location = New System.Drawing.Point(15, 12)
			Me.ApplicationFileGroupBox.Name = "ApplicationFileGroupBox"
			Me.ApplicationFileGroupBox.Size = New System.Drawing.Size(613, 48)
			Me.ApplicationFileGroupBox.TabIndex = 1
			Me.ApplicationFileGroupBox.TabStop = False
			Me.ApplicationFileGroupBox.Text = "Application File"
			'
			'BrowseButton
			'
			Me.BrowseButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.BrowseButton.Location = New System.Drawing.Point(579, 16)
			Me.BrowseButton.Name = "BrowseButton"
			Me.BrowseButton.Size = New System.Drawing.Size(24, 20)
			Me.BrowseButton.TabIndex = 3
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
			Me.FilePath.ReadOnly = True
			Me.FilePath.Size = New System.Drawing.Size(565, 20)
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
			Me.ApplicationInformationGroupBox.Location = New System.Drawing.Point(15, 127)
			Me.ApplicationInformationGroupBox.Name = "ApplicationInformationGroupBox"
			Me.ApplicationInformationGroupBox.Size = New System.Drawing.Size(613, 88)
			Me.ApplicationInformationGroupBox.TabIndex = 7
			Me.ApplicationInformationGroupBox.TabStop = False
			Me.ApplicationInformationGroupBox.Text = "Application Information"
			'
			'ApplicationName
			'
			Me.ApplicationName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
						Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.ApplicationName.Location = New System.Drawing.Point(60, 24)
			Me.ApplicationName.Name = "ApplicationName"
			Me.ApplicationName.ReadOnly = True
			Me.ApplicationName.Size = New System.Drawing.Size(545, 20)
			Me.ApplicationName.TabIndex = 8
			'
			'ApplicationVersion
			'
			Me.ApplicationVersion.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
						Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.ApplicationVersion.Location = New System.Drawing.Point(60, 56)
			Me.ApplicationVersion.Name = "ApplicationVersion"
			Me.ApplicationVersion.ReadOnly = True
			Me.ApplicationVersion.Size = New System.Drawing.Size(545, 20)
			Me.ApplicationVersion.TabIndex = 9
			'
			'VersionLabel
			'
			Me.VersionLabel.Location = New System.Drawing.Point(6, 56)
			Me.VersionLabel.Name = "VersionLabel"
			Me.VersionLabel.Size = New System.Drawing.Size(48, 23)
			Me.VersionLabel.TabIndex = 1
			Me.VersionLabel.Text = "Version:"
			'
			'NameLabel
			'
			Me.NameLabel.Location = New System.Drawing.Point(6, 24)
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
			Me.ApplicationArtifactsGroupBox.Controls.Add(Me.TreeView1)
			Me.ApplicationArtifactsGroupBox.Location = New System.Drawing.Point(13, 221)
			Me.ApplicationArtifactsGroupBox.Name = "ApplicationArtifactsGroupBox"
			Me.ApplicationArtifactsGroupBox.Size = New System.Drawing.Size(615, 247)
			Me.ApplicationArtifactsGroupBox.TabIndex = 10
			Me.ApplicationArtifactsGroupBox.TabStop = False
			Me.ApplicationArtifactsGroupBox.Text = "Application Artifacts"
			'
			'TreeView1
			'
			Me.TreeView1.Dock = System.Windows.Forms.DockStyle.Fill
			Me.TreeView1.Location = New System.Drawing.Point(3, 16)
			Me.TreeView1.Name = "TreeView1"
			Me.TreeView1.Size = New System.Drawing.Size(609, 228)
			Me.TreeView1.TabIndex = 8
			'
			'GroupBox1
			'
			Me.GroupBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
						Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.GroupBox1.Controls.Add(Me.BrowseCasesButton)
			Me.GroupBox1.Controls.Add(Me.CaseListTextBox)
			Me.GroupBox1.Location = New System.Drawing.Point(15, 66)
			Me.GroupBox1.Name = "GroupBox1"
			Me.GroupBox1.Size = New System.Drawing.Size(613, 55)
			Me.GroupBox1.TabIndex = 4
			Me.GroupBox1.TabStop = False
			Me.GroupBox1.Text = "Application Workspaces"
			'
			'BrowseCasesButton
			'
			Me.BrowseCasesButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.BrowseCasesButton.Location = New System.Drawing.Point(579, 23)
			Me.BrowseCasesButton.Name = "BrowseCasesButton"
			Me.BrowseCasesButton.Size = New System.Drawing.Size(24, 20)
			Me.BrowseCasesButton.TabIndex = 6
			Me.BrowseCasesButton.Text = "..."
			'
			'CaseListTextBox
			'
			Me.CaseListTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
						Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.CaseListTextBox.BackColor = System.Drawing.SystemColors.ControlLightLight
			Me.CaseListTextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me.CaseListTextBox.ForeColor = System.Drawing.SystemColors.ControlDarkDark
			Me.CaseListTextBox.Location = New System.Drawing.Point(8, 23)
			Me.CaseListTextBox.Name = "CaseListTextBox"
			Me.CaseListTextBox.ReadOnly = True
			Me.CaseListTextBox.Size = New System.Drawing.Size(565, 20)
			Me.CaseListTextBox.TabIndex = 5
			Me.CaseListTextBox.Text = "Select a workspace  ..."
			'
			'ApplicationFileForm
			'
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.ClientSize = New System.Drawing.Size(640, 480)
			Me.Controls.Add(Me.GroupBox1)
			Me.Controls.Add(Me.ApplicationArtifactsGroupBox)
			Me.Controls.Add(Me.ApplicationInformationGroupBox)
			Me.Controls.Add(Me.ApplicationFileGroupBox)
			Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
			Me.Menu = Me.MainMenu
			Me.MinimumSize = New System.Drawing.Size(380, 395)
			Me.Name = "ApplicationFileForm"
			Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
			Me.Text = "Relativity Desktop Client | Import Application"
			Me.ApplicationFileGroupBox.ResumeLayout(False)
			Me.ApplicationFileGroupBox.PerformLayout()
			Me.ApplicationInformationGroupBox.ResumeLayout(False)
			Me.ApplicationInformationGroupBox.PerformLayout()
			Me.ApplicationArtifactsGroupBox.ResumeLayout(False)
			Me.GroupBox1.ResumeLayout(False)
			Me.GroupBox1.PerformLayout()
			Me.ResumeLayout(False)

		End Sub

#End Region

#Region " Event Handlers "

		Private Sub MenuFile_Refresh_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuFile_Refresh.Click
			Dim document As Xml.XmlDocument = LoadFileIntoXML(_filename)

			If Not document Is Nothing AndAlso LoadApplicationNameFromNode(document.SelectSingleNode("/Application/Name")) Then
				LoadApplicationVersionFromNode(document.SelectSingleNode("/Application/Version"))
				LoadApplicationTree(document)
				_document = document
			Else
				MsgBox("Unable to refresh the loaded Relativity Application template file.", MsgBoxStyle.Exclamation, "Relativity Desktop Client")
			End If
		End Sub

		Private Sub MenuFile_Close_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles MenuFile_Close.Click
			Me.Close()
		End Sub

		Private Sub MenuImport_ImportApplication_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles MenuImport_ImportApplication.Click
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			_application.ImportApplicationFile(_caseInfos, _document)
			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Sub

		Private Sub OpenFileDialog_FileOk(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog.FileOk
			Dim document As Xml.XmlDocument = LoadFileIntoXML(OpenFileDialog.FileName)

			Dim ErrorMsg As Action(Of String) = Sub(msg As String)
																						e.Cancel = True
																						MsgBox(msg, MsgBoxStyle.Critical, "Relativity Desktop Client")
																					End Sub

			If document Is Nothing Then
				ErrorMsg("The file is not a valid XML file.")
			ElseIf Not LoadApplicationNameFromNode(document.SelectSingleNode("/Application/Name")) Then
				ErrorMsg("The file is not a valid Relativity Application template file.")
			End If

			If Not e.Cancel Then
				LoadApplicationVersionFromNode(document.SelectSingleNode("/Application/Version"))
				LoadApplicationTree(document)

				FilePath.Text = OpenFileDialog.FileName
				OpenFileDialog.InitialDirectory = IO.Path.GetDirectoryName(OpenFileDialog.FileName)	' Preserve old directory

				_filename = OpenFileDialog.FileName
				_document = document

				MenuImport_ImportApplication.Enabled = True
				MenuFile_Refresh.Enabled = True
			End If
		End Sub

		Private Sub BrowseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BrowseButton.Click
			OpenFileDialog.ShowDialog()
		End Sub

		Private Sub BrowseCasesButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BrowseCasesButton.Click
			Dim frm As New CaseSelectForm
			frm.MultiSelect = True
			If frm.ShowDialog() = DialogResult.OK Then
				Me.CaseInfos = frm.SelectedCaseInfo
			End If
		End Sub

#End Region

#Region " Friend Properties "

		Friend Property Application() As kCura.EDDS.WinForm.Application
			
		Friend Property CookieContainer() As System.Net.CookieContainer

		Friend Property Credentials() As System.Net.NetworkCredential

		Friend Property CaseInfos() As Generic.List(Of Relativity.CaseInfo)
			Get
				Return _caseInfos
			End Get
			Set(ByVal Value As Generic.List(Of Relativity.CaseInfo))
				_caseInfos = Value
				Dim sb As New System.Text.StringBuilder
				For Each info As Relativity.CaseInfo In _caseInfos
					sb.AppendFormat("{0}, ", info.Name)
				Next
				sb.Remove(sb.Length - 2, 2)
				Me.CaseListTextBox.Text = sb.ToString
			End Set
		End Property

#End Region

#Region " Private Methods "

		Private Function LoadApplicationNameFromNode(ByVal node As Xml.XmlNode) As Boolean
			Dim result As Boolean
			If node Is Nothing OrElse String.IsNullOrEmpty(node.InnerText) Then
				result = False
			Else
				ApplicationName.Text = node.InnerText
				result = True
			End If
			Return result
		End Function

		Private Function LoadApplicationVersionFromNode(ByVal node As Xml.XmlNode) As Boolean
			Dim result As Boolean
			If node Is Nothing OrElse String.IsNullOrEmpty(node.InnerText) Then
				ApplicationVersion.Text = "N/A"
				result = False
			Else
				ApplicationVersion.Text = node.InnerText
				result = True
			End If
			Return result
		End Function

		Private Sub LoadApplicationTree(ByVal document As Xml.XmlDocument)
			TreeView1.Nodes.Clear()
			TreeView1.BeginUpdate()
			Dim newNode As New TreeNode
			Dim doc As XElement = XElement.Load(New System.IO.StringReader(document.OuterXml))
			Dim objsNode = TreeView1.Nodes.Add("Object Types")
			Dim boldfont As New Font(TreeView.DefaultFont, FontStyle.Bold)
			objsNode.NodeFont = boldfont
			For Each item As XElement In doc...<Object>
				Dim objNode = objsNode.Nodes.Add(item.<Name>.Value)
				PopulateChildren(objNode, "Fields", "Field", item)
				PopulateChildren(objNode, "Layouts", "Layout", item)
				PopulateChildren(objNode, "Tabs", "Tab", item)
				PopulateChildren(objNode, "Views", "View", item)
				PopulateChildren(objNode, "ObjectRules", "ObjectRule", item)
			Next
			Dim externalTabsNode = TreeView1.Nodes.Add("External Tabs")
			externalTabsNode.NodeFont = boldfont
			For Each item As XElement In doc...<ExternalTab>
				externalTabsNode.Nodes.Add(item.<Name>.Value)
			Next
			Dim scriptsNode = TreeView1.Nodes.Add("Scripts")
			scriptsNode.NodeFont = boldfont
			For Each item As XElement In doc...<ScriptElement>
				scriptsNode.Nodes.Add(item.<Name>.Value)
			Next
			TreeView1.EndUpdate()
		End Sub

		Private Sub PopulateChildren(ByVal rootNode As TreeNode, ByVal rootName As String, ByVal childName As String, ByVal objlment As XElement)
			Dim childNodes = rootNode.Nodes.Add(rootName)
			Dim boldfont As New Font(TreeView.DefaultFont, FontStyle.Bold)
			childNodes.NodeFont = boldfont
			For Each elmnt As XElement In objlment.Descendants(childName)
				Dim grandChild As TreeNode = Nothing
				If String.IsNullOrEmpty(elmnt.<Name>.Value) Then
					If Not String.IsNullOrEmpty(elmnt.<DisplayName>.Value) Then
						grandChild = childNodes.Nodes.Add(elmnt.<DisplayName>.Value)
					End If
				Else
					grandChild = childNodes.Nodes.Add(elmnt.<Name>.Value)
				End If
				If childName = "Tab" Then
					If Not String.IsNullOrEmpty(elmnt.<ParentTab>.Value) Then
						Dim pt = elmnt.<ParentTab>
						childNodes.Nodes.Add(pt.<Name>.Value)
					End If
				End If
			Next
		End Sub

		Private Function LoadFileIntoXML(ByVal filePath As String) As Xml.XmlDocument
			Try
				Dim document As New Xml.XmlDocument
				document.Load(filePath)
				Return document
			Catch ex As Exception
			End Try

			Return Nothing
		End Function

#End Region

	End Class
End Namespace