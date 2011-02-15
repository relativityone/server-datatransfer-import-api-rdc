Imports System.Xml
Imports System.Core
Imports System.Xml.Linq


Namespace kCura.EDDS.WinForm
	Public Class ApplicationFileForm
		Inherits System.Windows.Forms.Form
		Private WithEvents OpenFileDialog As OpenFileDialog

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
			Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ApplicationFileForm))
			Me.MainMenu = New System.Windows.Forms.MainMenu(Me.components)
			Me.MenuFile = New System.Windows.Forms.MenuItem()
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
			Me.ApplicationInformationGroupBox.Location = New System.Drawing.Point(16, 139)
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
			Me.ApplicationArtifactsGroupBox.Controls.Add(Me.TreeView1)
			Me.ApplicationArtifactsGroupBox.Location = New System.Drawing.Point(12, 233)
			Me.ApplicationArtifactsGroupBox.Name = "ApplicationArtifactsGroupBox"
			Me.ApplicationArtifactsGroupBox.Size = New System.Drawing.Size(720, 378)
			Me.ApplicationArtifactsGroupBox.TabIndex = 24
			Me.ApplicationArtifactsGroupBox.TabStop = False
			Me.ApplicationArtifactsGroupBox.Text = "Application Artifacts"
			'
			'TreeView1
			'
			Me.TreeView1.Location = New System.Drawing.Point(10, 19)
			Me.TreeView1.Name = "TreeView1"
			Me.TreeView1.Size = New System.Drawing.Size(704, 345)
			Me.TreeView1.TabIndex = 0
			'
			'GroupBox1
			'
			Me.GroupBox1.Controls.Add(Me.BrowseCasesButton)
			Me.GroupBox1.Controls.Add(Me.CaseListTextBox)
			Me.GroupBox1.Location = New System.Drawing.Point(16, 78)
			Me.GroupBox1.Name = "GroupBox1"
			Me.GroupBox1.Size = New System.Drawing.Size(720, 55)
			Me.GroupBox1.TabIndex = 25
			Me.GroupBox1.TabStop = False
			Me.GroupBox1.Text = "Application Case"
			'
			'BrowseCasesButton
			'
			Me.BrowseCasesButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.BrowseCasesButton.Location = New System.Drawing.Point(688, 23)
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
			Me.CaseListTextBox.Size = New System.Drawing.Size(680, 20)
			Me.CaseListTextBox.TabIndex = 5
			Me.CaseListTextBox.Text = "Select a case  ..."
			'
			'ApplicationFileForm
			'
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.ClientSize = New System.Drawing.Size(752, 664)
			Me.Controls.Add(Me.GroupBox1)
			Me.Controls.Add(Me.ApplicationArtifactsGroupBox)
			Me.Controls.Add(Me.ApplicationInformationGroupBox)
			Me.Controls.Add(Me.ApplicationFileGroupBox)
			Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
			Me.Menu = Me.MainMenu
			Me.MinimumSize = New System.Drawing.Size(760, 621)
			Me.Name = "ApplicationFileForm"
			Me.Text = "Relativity Desktop Client | Import Application File"
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

		Private Sub BrowseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BrowseButton.Click
			OpenFileDialog.ShowDialog()
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
			Dim document As Xml.XmlDocument

			document = LoadFileIntoXML(OpenFileDialog.FileName)
			Dim ErrorMsg As Action(Of String) = Sub(msg As String)
																e.Cancel = True
																MsgBox(String.Format(System.Globalization.CultureInfo.CurrentCulture, "Invalid File: {0}", msg))
															End Sub
			If document Is Nothing Then
				ErrorMsg("File is not an application")
			Else
				If Not LoadApplicationNameFromNode(document.SelectSingleNode("/Application/Name")) Then
					ErrorMsg("Application must have a name")
				End If

				Dim version As String = LoadApplicationVersionFromNode(document.SelectSingleNode("/Application/Version"))
				If String.IsNullOrEmpty(version) Then
					ApplicationVersion.Visible = False
					VersionLabel.Visible = False
				Else
					ApplicationVersion.Text = version
					ApplicationVersion.Visible = True
					VersionLabel.Visible = True
				End If

				LoadApplicationTree(document)
			End If

			If e.Cancel Then
				Return
			End If

			FilePath.Text = OpenFileDialog.FileName
			'I am sure that there is a way to make the openFileDialog preserve the old directory, but for whatever reason I can't get it to happen.
			'Consider this a workaround, and if anyone knows how to make it work properly, please remove this line!
			OpenFileDialog.InitialDirectory = IO.Path.GetDirectoryName(OpenFileDialog.FileName)
			_document = document
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
			Get
				Return _application
			End Get
			Set(ByVal Value As kCura.EDDS.WinForm.Application)
				_application = Value
			End Set
		End Property

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
		Private _caseInfos As Generic.List(Of Relativity.CaseInfo)
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

		Private Sub LoadApplicationTree(ByVal document As Xml.XmlDocument)
			TreeView1.Nodes.Clear()
			TreeView1.BeginUpdate()
			Dim newNode As New TreeNode
			Dim doc As XElement = XElement.Load(New System.IO.StringReader(document.OuterXml))
			Dim objsNode = TreeView1.Nodes.Add("Object Types")
			Dim boldfont As New Font(TreeView.DefaultFont, FontStyle.Bold)
			objsNode.NodeFont = boldfont
			For Each objlment As XElement In doc...<Object>
				Dim objNode = objsNode.Nodes.Add(objlment.<Name>.Value)
				PopulateChildren(objNode, "Fields", "Field", objlment)
				PopulateChildren(objNode, "Layouts", "Layout", objlment)
				PopulateChildren(objNode, "Tabs", "Tab", objlment)
				PopulateChildren(objNode, "Views", "View", objlment)
			Next
			Dim externalTabsNode = TreeView1.Nodes.Add("External Tabs")
			externalTabsNode.NodeFont = boldfont
			For Each tablment As XElement In doc...<ExternalTab>
				externalTabsNode.Nodes.Add(tablment.<Name>.Value)
			Next
			Dim scriptsNode = TreeView1.Nodes.Add("Scripts")
			scriptsNode.NodeFont = boldfont
			For Each scriptElment As XElement In doc...<ScriptElement>
				scriptsNode.Nodes.Add(scriptElment.<Name>.Value)
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
					grandChild = childNodes.Nodes.Add(elmnt.<DisplayName>.Value)
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

		Private Function LoadApplicationVersionFromNode(ByVal node As Xml.XmlNode) As String
			If node Is Nothing Then
				Return Nothing
			Else
				Return node.InnerText
			End If
		End Function

		Private Function LoadFileIntoXML(ByVal filePath As String) As Xml.XmlDocument
			Try
				Dim document As New Xml.XmlDocument
				document.Load(OpenFileDialog.FileName)
				Return document
			Catch ex As Exception
				Return Nothing
			End Try
		End Function

#End Region

	End Class
End Namespace