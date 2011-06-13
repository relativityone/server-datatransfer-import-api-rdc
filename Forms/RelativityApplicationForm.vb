Imports System.Xml
Imports System.Xml.Linq
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Linq
Imports Facet.Combinatorics
Imports Relativity.Applications.Serialization.Elements
Imports kCura.EDDS.WinForm.Presentation.Controller
Imports kCura.EDDS.WinForm.Data

Namespace kCura.EDDS.WinForm
	Public Class RelativityApplicationForm
		Inherits System.Windows.Forms.Form

		Const FORM_BOTTOM_BUFFER As Int32 = 50

		Private Enum FormUIState
			General = 1
			FieldMapping = 2
		End Enum

		Private WithEvents OpenFileDialog As OpenFileDialog
		Friend WithEvents MenuFile_Refresh As System.Windows.Forms.MenuItem
		Friend WithEvents MenuFile_Separator As System.Windows.Forms.MenuItem
		Private _caseInfos As Generic.List(Of Relativity.CaseInfo)
		Private _document As Xml.XmlDocument
		Friend WithEvents ImportButton As System.Windows.Forms.Button
		Friend WithEvents MapFieldsButton As System.Windows.Forms.Button
		Friend WithEvents AppArtifactsPanel As System.Windows.Forms.Panel
		Friend WithEvents FieldMapPanel As System.Windows.Forms.Panel
		Friend WithEvents ObjectInfoGroupBox As System.Windows.Forms.GroupBox
		Friend WithEvents ArtifactMappingGroupBox As System.Windows.Forms.GroupBox
		Friend WithEvents ObjectMapComboBox As System.Windows.Forms.ComboBox
		Friend WithEvents Label1 As System.Windows.Forms.Label
		Friend WithEvents TargetFieldList As System.Windows.Forms.ListBox
		Friend WithEvents MappedList_Target As System.Windows.Forms.ListBox
		Friend WithEvents MappedList_App As System.Windows.Forms.ListBox
		Friend WithEvents AppFieldList As System.Windows.Forms.ListBox
		Friend WithEvents MapToAppBtn As System.Windows.Forms.Button
		Friend WithEvents AppToMapBtn As System.Windows.Forms.Button
		Friend WithEvents MapToTargetBtn As System.Windows.Forms.Button
		Friend WithEvents TargetToMapBtn As System.Windows.Forms.Button
		Private _filename As String
		Friend WithEvents AppNotLoadedLabel As System.Windows.Forms.Label
		Private _formState As FormUIState = FormUIState.General
		Private _isAppAndCaseLoaded As Boolean = False
		Dim _app As RelativityApplicationElement
		Dim _appMappingData As AppMappingData
		Friend WithEvents NoTargetFieldsQualifyLabel As System.Windows.Forms.Label
		Dim _mapController As FieldMapFourPickerController


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
		Friend WithEvents ArtifactsTreeView As System.Windows.Forms.TreeView
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
			Me.AppNotLoadedLabel = New System.Windows.Forms.Label()
			Me.ArtifactsTreeView = New System.Windows.Forms.TreeView()
			Me.GroupBox1 = New System.Windows.Forms.GroupBox()
			Me.BrowseCasesButton = New System.Windows.Forms.Button()
			Me.CaseListTextBox = New System.Windows.Forms.TextBox()
			Me.ImportButton = New System.Windows.Forms.Button()
			Me.MapFieldsButton = New System.Windows.Forms.Button()
			Me.AppArtifactsPanel = New System.Windows.Forms.Panel()
			Me.FieldMapPanel = New System.Windows.Forms.Panel()
			Me.ArtifactMappingGroupBox = New System.Windows.Forms.GroupBox()
			Me.MapToAppBtn = New System.Windows.Forms.Button()
			Me.AppToMapBtn = New System.Windows.Forms.Button()
			Me.MapToTargetBtn = New System.Windows.Forms.Button()
			Me.TargetToMapBtn = New System.Windows.Forms.Button()
			Me.TargetFieldList = New System.Windows.Forms.ListBox()
			Me.MappedList_Target = New System.Windows.Forms.ListBox()
			Me.MappedList_App = New System.Windows.Forms.ListBox()
			Me.AppFieldList = New System.Windows.Forms.ListBox()
			Me.ObjectInfoGroupBox = New System.Windows.Forms.GroupBox()
			Me.ObjectMapComboBox = New System.Windows.Forms.ComboBox()
			Me.Label1 = New System.Windows.Forms.Label()
			Me.NoTargetFieldsQualifyLabel = New System.Windows.Forms.Label()
			Me.ApplicationFileGroupBox.SuspendLayout()
			Me.ApplicationInformationGroupBox.SuspendLayout()
			Me.ApplicationArtifactsGroupBox.SuspendLayout()
			Me.GroupBox1.SuspendLayout()
			Me.AppArtifactsPanel.SuspendLayout()
			Me.FieldMapPanel.SuspendLayout()
			Me.ArtifactMappingGroupBox.SuspendLayout()
			Me.ObjectInfoGroupBox.SuspendLayout()
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
			Me.ApplicationFileGroupBox.Controls.Add(Me.BrowseButton)
			Me.ApplicationFileGroupBox.Controls.Add(Me.FilePath)
			Me.ApplicationFileGroupBox.Location = New System.Drawing.Point(15, 12)
			Me.ApplicationFileGroupBox.Name = "ApplicationFileGroupBox"
			Me.ApplicationFileGroupBox.Size = New System.Drawing.Size(868, 48)
			Me.ApplicationFileGroupBox.TabIndex = 1
			Me.ApplicationFileGroupBox.TabStop = False
			Me.ApplicationFileGroupBox.Text = "Application File"
			'
			'BrowseButton
			'
			Me.BrowseButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.BrowseButton.Location = New System.Drawing.Point(834, 16)
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
			Me.FilePath.Size = New System.Drawing.Size(820, 20)
			Me.FilePath.TabIndex = 2
			Me.FilePath.Text = "Select a file ..."
			'
			'ApplicationInformationGroupBox
			'
			Me.ApplicationInformationGroupBox.Controls.Add(Me.ApplicationName)
			Me.ApplicationInformationGroupBox.Controls.Add(Me.ApplicationVersion)
			Me.ApplicationInformationGroupBox.Controls.Add(Me.VersionLabel)
			Me.ApplicationInformationGroupBox.Controls.Add(Me.NameLabel)
			Me.ApplicationInformationGroupBox.Location = New System.Drawing.Point(15, 127)
			Me.ApplicationInformationGroupBox.Name = "ApplicationInformationGroupBox"
			Me.ApplicationInformationGroupBox.Size = New System.Drawing.Size(868, 88)
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
			Me.ApplicationName.Size = New System.Drawing.Size(800, 20)
			Me.ApplicationName.TabIndex = 8
			'
			'ApplicationVersion
			'
			Me.ApplicationVersion.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
									Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.ApplicationVersion.Location = New System.Drawing.Point(60, 56)
			Me.ApplicationVersion.Name = "ApplicationVersion"
			Me.ApplicationVersion.ReadOnly = True
			Me.ApplicationVersion.Size = New System.Drawing.Size(800, 20)
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
			Me.ApplicationArtifactsGroupBox.Controls.Add(Me.AppNotLoadedLabel)
			Me.ApplicationArtifactsGroupBox.Controls.Add(Me.ArtifactsTreeView)
			Me.ApplicationArtifactsGroupBox.Location = New System.Drawing.Point(10, 5)
			Me.ApplicationArtifactsGroupBox.Name = "ApplicationArtifactsGroupBox"
			Me.ApplicationArtifactsGroupBox.Size = New System.Drawing.Size(868, 342)
			Me.ApplicationArtifactsGroupBox.TabIndex = 10
			Me.ApplicationArtifactsGroupBox.TabStop = False
			Me.ApplicationArtifactsGroupBox.Text = "Application Artifacts"
			'
			'AppNotLoadedLabel
			'
			Me.AppNotLoadedLabel.AutoSize = True
			Me.AppNotLoadedLabel.BackColor = System.Drawing.SystemColors.Window
			Me.AppNotLoadedLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me.AppNotLoadedLabel.Location = New System.Drawing.Point(289, 134)
			Me.AppNotLoadedLabel.Name = "AppNotLoadedLabel"
			Me.AppNotLoadedLabel.Size = New System.Drawing.Size(286, 26)
			Me.AppNotLoadedLabel.TabIndex = 9
			Me.AppNotLoadedLabel.Text = "Select an Application File and Target Workspace" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "using the buttons above"
			Me.AppNotLoadedLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter
			'
			'ArtifactsTreeView
			'
			Me.ArtifactsTreeView.Dock = System.Windows.Forms.DockStyle.Fill
			Me.ArtifactsTreeView.Enabled = False
			Me.ArtifactsTreeView.Location = New System.Drawing.Point(3, 16)
			Me.ArtifactsTreeView.Name = "ArtifactsTreeView"
			Me.ArtifactsTreeView.Size = New System.Drawing.Size(862, 323)
			Me.ArtifactsTreeView.TabIndex = 8
			'
			'GroupBox1
			'
			Me.GroupBox1.Controls.Add(Me.BrowseCasesButton)
			Me.GroupBox1.Controls.Add(Me.CaseListTextBox)
			Me.GroupBox1.Location = New System.Drawing.Point(15, 66)
			Me.GroupBox1.Name = "GroupBox1"
			Me.GroupBox1.Size = New System.Drawing.Size(868, 55)
			Me.GroupBox1.TabIndex = 4
			Me.GroupBox1.TabStop = False
			Me.GroupBox1.Text = "Target Workspace"
			'
			'BrowseCasesButton
			'
			Me.BrowseCasesButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.BrowseCasesButton.Location = New System.Drawing.Point(834, 23)
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
			Me.CaseListTextBox.Size = New System.Drawing.Size(820, 20)
			Me.CaseListTextBox.TabIndex = 5
			Me.CaseListTextBox.Text = "Select a workspace  ..."
			'
			'ImportButton
			'
			Me.ImportButton.Enabled = False
			Me.ImportButton.Location = New System.Drawing.Point(724, 585)
			Me.ImportButton.Name = "ImportButton"
			Me.ImportButton.Size = New System.Drawing.Size(75, 23)
			Me.ImportButton.TabIndex = 11
			Me.ImportButton.Text = "Import"
			Me.ImportButton.UseVisualStyleBackColor = True
			'
			'MapFieldsButton
			'
			Me.MapFieldsButton.Enabled = False
			Me.MapFieldsButton.Location = New System.Drawing.Point(805, 585)
			Me.MapFieldsButton.Name = "MapFieldsButton"
			Me.MapFieldsButton.Size = New System.Drawing.Size(87, 23)
			Me.MapFieldsButton.TabIndex = 12
			Me.MapFieldsButton.Text = "Map Fields >>"
			Me.MapFieldsButton.UseVisualStyleBackColor = True
			'
			'AppArtifactsPanel
			'
			Me.AppArtifactsPanel.Controls.Add(Me.ApplicationArtifactsGroupBox)
			Me.AppArtifactsPanel.Location = New System.Drawing.Point(5, 230)
			Me.AppArtifactsPanel.Name = "AppArtifactsPanel"
			Me.AppArtifactsPanel.Size = New System.Drawing.Size(887, 350)
			Me.AppArtifactsPanel.TabIndex = 13
			'
			'FieldMapPanel
			'
			Me.FieldMapPanel.Controls.Add(Me.ArtifactMappingGroupBox)
			Me.FieldMapPanel.Controls.Add(Me.ObjectInfoGroupBox)
			Me.FieldMapPanel.Location = New System.Drawing.Point(5, 632)
			Me.FieldMapPanel.Name = "FieldMapPanel"
			Me.FieldMapPanel.Size = New System.Drawing.Size(887, 350)
			Me.FieldMapPanel.TabIndex = 14
			'
			'ArtifactMappingGroupBox
			'
			Me.ArtifactMappingGroupBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
									Or System.Windows.Forms.AnchorStyles.Left) _
									Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.ArtifactMappingGroupBox.Controls.Add(Me.NoTargetFieldsQualifyLabel)
			Me.ArtifactMappingGroupBox.Controls.Add(Me.MapToAppBtn)
			Me.ArtifactMappingGroupBox.Controls.Add(Me.AppToMapBtn)
			Me.ArtifactMappingGroupBox.Controls.Add(Me.MapToTargetBtn)
			Me.ArtifactMappingGroupBox.Controls.Add(Me.TargetToMapBtn)
			Me.ArtifactMappingGroupBox.Controls.Add(Me.TargetFieldList)
			Me.ArtifactMappingGroupBox.Controls.Add(Me.MappedList_Target)
			Me.ArtifactMappingGroupBox.Controls.Add(Me.MappedList_App)
			Me.ArtifactMappingGroupBox.Controls.Add(Me.AppFieldList)
			Me.ArtifactMappingGroupBox.Location = New System.Drawing.Point(11, 66)
			Me.ArtifactMappingGroupBox.Name = "ArtifactMappingGroupBox"
			Me.ArtifactMappingGroupBox.Size = New System.Drawing.Size(867, 273)
			Me.ArtifactMappingGroupBox.TabIndex = 11
			Me.ArtifactMappingGroupBox.TabStop = False
			Me.ArtifactMappingGroupBox.Text = "Artifact Mapping"
			'
			'MapToAppBtn
			'
			Me.MapToAppBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me.MapToAppBtn.Location = New System.Drawing.Point(204, 108)
			Me.MapToAppBtn.Name = "MapToAppBtn"
			Me.MapToAppBtn.Size = New System.Drawing.Size(45, 23)
			Me.MapToAppBtn.TabIndex = 7
			Me.MapToAppBtn.Text = "<"
			Me.MapToAppBtn.UseVisualStyleBackColor = True
			'
			'AppToMapBtn
			'
			Me.AppToMapBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me.AppToMapBtn.Location = New System.Drawing.Point(204, 79)
			Me.AppToMapBtn.Name = "AppToMapBtn"
			Me.AppToMapBtn.Size = New System.Drawing.Size(45, 23)
			Me.AppToMapBtn.TabIndex = 6
			Me.AppToMapBtn.Text = ">"
			Me.AppToMapBtn.UseVisualStyleBackColor = True
			'
			'MapToTargetBtn
			'
			Me.MapToTargetBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me.MapToTargetBtn.Location = New System.Drawing.Point(620, 108)
			Me.MapToTargetBtn.Name = "MapToTargetBtn"
			Me.MapToTargetBtn.Size = New System.Drawing.Size(45, 23)
			Me.MapToTargetBtn.TabIndex = 5
			Me.MapToTargetBtn.Text = ">"
			Me.MapToTargetBtn.UseVisualStyleBackColor = True
			'
			'TargetToMapBtn
			'
			Me.TargetToMapBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me.TargetToMapBtn.Location = New System.Drawing.Point(620, 79)
			Me.TargetToMapBtn.Name = "TargetToMapBtn"
			Me.TargetToMapBtn.Size = New System.Drawing.Size(45, 23)
			Me.TargetToMapBtn.TabIndex = 4
			Me.TargetToMapBtn.Text = "<"
			Me.TargetToMapBtn.UseVisualStyleBackColor = True
			'
			'TargetFieldList
			'
			Me.TargetFieldList.FormattingEnabled = True
			Me.TargetFieldList.Location = New System.Drawing.Point(672, 28)
			Me.TargetFieldList.Name = "TargetFieldList"
			Me.TargetFieldList.Size = New System.Drawing.Size(177, 225)
			Me.TargetFieldList.TabIndex = 3
			'
			'MappedList_Target
			'
			Me.MappedList_Target.FormattingEnabled = True
			Me.MappedList_Target.Location = New System.Drawing.Point(434, 28)
			Me.MappedList_Target.Name = "MappedList_Target"
			Me.MappedList_Target.Size = New System.Drawing.Size(177, 225)
			Me.MappedList_Target.TabIndex = 2
			'
			'MappedList_App
			'
			Me.MappedList_App.FormattingEnabled = True
			Me.MappedList_App.Location = New System.Drawing.Point(257, 28)
			Me.MappedList_App.Name = "MappedList_App"
			Me.MappedList_App.Size = New System.Drawing.Size(177, 225)
			Me.MappedList_App.TabIndex = 1
			'
			'AppFieldList
			'
			Me.AppFieldList.FormattingEnabled = True
			Me.AppFieldList.Location = New System.Drawing.Point(18, 28)
			Me.AppFieldList.Name = "AppFieldList"
			Me.AppFieldList.Size = New System.Drawing.Size(177, 225)
			Me.AppFieldList.TabIndex = 0
			'
			'ObjectInfoGroupBox
			'
			Me.ObjectInfoGroupBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
									Or System.Windows.Forms.AnchorStyles.Left) _
									Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.ObjectInfoGroupBox.Controls.Add(Me.ObjectMapComboBox)
			Me.ObjectInfoGroupBox.Controls.Add(Me.Label1)
			Me.ObjectInfoGroupBox.Location = New System.Drawing.Point(8, 0)
			Me.ObjectInfoGroupBox.Name = "ObjectInfoGroupBox"
			Me.ObjectInfoGroupBox.Size = New System.Drawing.Size(870, 60)
			Me.ObjectInfoGroupBox.TabIndex = 10
			Me.ObjectInfoGroupBox.TabStop = False
			Me.ObjectInfoGroupBox.Text = "Ojbect Information"
			'
			'ObjectMapComboBox
			'
			Me.ObjectMapComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me.ObjectMapComboBox.FormattingEnabled = True
			Me.ObjectMapComboBox.Location = New System.Drawing.Point(88, 25)
			Me.ObjectMapComboBox.Name = "ObjectMapComboBox"
			Me.ObjectMapComboBox.Size = New System.Drawing.Size(303, 21)
			Me.ObjectMapComboBox.TabIndex = 10
			'
			'Label1
			'
			Me.Label1.Location = New System.Drawing.Point(18, 28)
			Me.Label1.Name = "Label1"
			Me.Label1.Size = New System.Drawing.Size(74, 23)
			Me.Label1.TabIndex = 9
			Me.Label1.Text = "Object Name:"
			'
			'NoTargetFieldsQualifyLabel
			'
			Me.NoTargetFieldsQualifyLabel.BackColor = System.Drawing.SystemColors.Window
			Me.NoTargetFieldsQualifyLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me.NoTargetFieldsQualifyLabel.Location = New System.Drawing.Point(688, 84)
			Me.NoTargetFieldsQualifyLabel.Name = "NoTargetFieldsQualifyLabel"
			Me.NoTargetFieldsQualifyLabel.Size = New System.Drawing.Size(147, 149)
			Me.NoTargetFieldsQualifyLabel.TabIndex = 15
			Me.NoTargetFieldsQualifyLabel.Text = "No fields in the target workspace qualify for mapping to this application field."
			Me.NoTargetFieldsQualifyLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter
			Me.NoTargetFieldsQualifyLabel.Visible = False
			'
			'RelativityApplicationForm
			'
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.ClientSize = New System.Drawing.Size(904, 1011)
			Me.Controls.Add(Me.FieldMapPanel)
			Me.Controls.Add(Me.AppArtifactsPanel)
			Me.Controls.Add(Me.GroupBox1)
			Me.Controls.Add(Me.ApplicationInformationGroupBox)
			Me.Controls.Add(Me.ApplicationFileGroupBox)
			Me.Controls.Add(Me.MapFieldsButton)
			Me.Controls.Add(Me.ImportButton)
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
			Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
			Me.Menu = Me.MainMenu
			Me.MinimumSize = New System.Drawing.Size(380, 395)
			Me.Name = "RelativityApplicationForm"
			Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
			Me.Text = "Relativity Desktop Client | Import Application"
			Me.ApplicationFileGroupBox.ResumeLayout(False)
			Me.ApplicationFileGroupBox.PerformLayout()
			Me.ApplicationInformationGroupBox.ResumeLayout(False)
			Me.ApplicationInformationGroupBox.PerformLayout()
			Me.ApplicationArtifactsGroupBox.ResumeLayout(False)
			Me.ApplicationArtifactsGroupBox.PerformLayout()
			Me.GroupBox1.ResumeLayout(False)
			Me.GroupBox1.PerformLayout()
			Me.AppArtifactsPanel.ResumeLayout(False)
			Me.FieldMapPanel.ResumeLayout(False)
			Me.ArtifactMappingGroupBox.ResumeLayout(False)
			Me.ObjectInfoGroupBox.ResumeLayout(False)
			Me.ResumeLayout(False)

		End Sub

#End Region

#Region " Event Handlers "

		Private Sub MenuFile_Refresh_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuFile_Refresh.Click
			RefreshAllData()
		End Sub

		Private Sub RefreshAllData()
			Dim document As Xml.XmlDocument = LoadFileIntoXML(_filename)

			If Not document Is Nothing AndAlso LoadApplicationNameFromNode(document.SelectSingleNode("/Application/Name")) Then
				LoadApplicationVersionFromNode(document.SelectSingleNode("/Application/Version"))
				LoadApplicationTree(document)
				LoadMappingControls(document)
				_document = document
			Else
				MsgBox("Unable to refresh the loaded Relativity Application template file.", MsgBoxStyle.Exclamation, "Relativity Desktop Client")
			End If
		End Sub

		Private Sub MenuFile_Close_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles MenuFile_Close.Click
			Me.Close()
		End Sub

		Private Sub MenuImport_ImportApplication_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles MenuImport_ImportApplication.Click, ImportButton.Click
			If _mapController.AreMappingsValid Then
				MessageBox.Show("IMPORT!")
				Return
			Else
				MessageBox.Show("You have unfinished field mappings that must be completed before importing.")
				Return
			End If
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			_Application.ImportApplicationFile(_caseInfos, _document)
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

				'Dim factory As New AppMappingDataFactory()
				LoadMappingControls(document)

				FilePath.Text = OpenFileDialog.FileName
				OpenFileDialog.InitialDirectory = IO.Path.GetDirectoryName(OpenFileDialog.FileName)	' Preserve old directory

				_filename = OpenFileDialog.FileName
				_document = document
				_isAppAndCaseLoaded = True
				SetFormState(FormUIState.General)
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
				RefreshAllData()
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
			'Dim svc As New kCura.EDDS.WebAPI.TemplateManagerBase.TemplateManager
			'svc.GetAvailableFieldsForMapping(

			ArtifactsTreeView.Nodes.Clear()
			ArtifactsTreeView.BeginUpdate()
			Dim newNode As New TreeNode
			Dim doc As XElement = XElement.Load(New System.IO.StringReader(document.OuterXml))
			Dim objsNode = ArtifactsTreeView.Nodes.Add("Object Types")
			Dim boldfont As New Font(TreeView.DefaultFont, FontStyle.Bold)
			objsNode.NodeFont = boldfont
			For Each item As XElement In doc...<Object>
				Dim objNode = objsNode.Nodes.Add(item.<Name>.Value)
				PopulateChildren(objNode, "Fields", "Field", item)
				PopulateChildren(objNode, "Layouts", "Layout", item)
				PopulateChildren(objNode, "Tabs", "Tab", item)
				PopulateChildren(objNode, "Views", "View", item)
				PopulateChildren(objNode, "ObjectRules", "ObjectRule", item)
				PopulateChildren(objNode, "EventHandlers", "ActiveSync", item)
			Next
			Dim externalTabsNode = ArtifactsTreeView.Nodes.Add("External Tabs")
			externalTabsNode.NodeFont = boldfont
			For Each item As XElement In doc...<ExternalTab>
				externalTabsNode.Nodes.Add(item.<Name>.Value)
			Next
			Dim scriptsNode = ArtifactsTreeView.Nodes.Add("Scripts")
			scriptsNode.NodeFont = boldfont
			For Each item As XElement In doc...<ScriptElement>
				scriptsNode.Nodes.Add(item.<Name>.Value)
			Next
			ArtifactsTreeView.EndUpdate()
		End Sub

		Private Sub PopulateChildren(ByVal rootNode As TreeNode, ByVal rootName As String, ByVal childName As String, ByVal objlment As XElement)
			Dim childNodes = rootNode.Nodes.Add(rootName)
			Dim boldfont As New Font(TreeView.DefaultFont, FontStyle.Bold)
			childNodes.NodeFont = boldfont
			For Each elmnt As XElement In objlment.Descendants(childName)
				Dim grandChild As TreeNode = Nothing

				If String.IsNullOrEmpty(elmnt.<Name>.Value) Then
					Select Case childName
						Case "ActiveSync"
							If Not String.IsNullOrEmpty(elmnt.<ClassName>.Value) Then
								grandChild = childNodes.Nodes.Add(elmnt.<ClassName>.Value)
							End If
						Case Else
							If Not String.IsNullOrEmpty(elmnt.<DisplayName>.Value) Then
								grandChild = childNodes.Nodes.Add(elmnt.<DisplayName>.Value)
							End If
					End Select
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

		Private Sub MapFieldsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MapFieldsButton.Click
			If _formState = FormUIState.General Then
				SetFormState(FormUIState.FieldMapping)
			Else
				SetFormState(FormUIState.General)
			End If
		End Sub

		Private Sub RelativityApplicationForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
			'Set up form size and bottom panels
			Dim newHeight As Int32 = ImportButton.Location.Y + ImportButton.Height + FORM_BOTTOM_BUFFER
			Me.Size = New Size(Me.Width, newHeight)
			FieldMapPanel.Location = AppArtifactsPanel.Location
			SetFormState(FormUIState.General)
		End Sub

		Private Sub SetFormState(ByVal formState As FormUIState)
			_formState = formState
			Dim toggleVal As Boolean = _formState = FormUIState.General
			MenuImport_ImportApplication.Enabled = _isAppAndCaseLoaded
			ImportButton.Enabled = _isAppAndCaseLoaded
			MenuFile_Refresh.Enabled = _isAppAndCaseLoaded
			ArtifactsTreeView.Visible = _isAppAndCaseLoaded
			ArtifactsTreeView.Enabled = _isAppAndCaseLoaded
			AppNotLoadedLabel.Visible = Not _isAppAndCaseLoaded
			AppArtifactsPanel.Visible = toggleVal
			MapFieldsButton.Enabled = _isAppAndCaseLoaded
			BrowseButton.Enabled = toggleVal
			BrowseCasesButton.Enabled = toggleVal
			FilePath.Enabled = toggleVal
			CaseListTextBox.Enabled = toggleVal
			ArtifactsTreeView.Visible = toggleVal
			If toggleVal Then
				MapFieldsButton.Text = "Map Fields >>"
			Else
				MapFieldsButton.Text = "<< Back"
			End If
			FieldMapPanel.Visible = Not toggleVal
		End Sub


#Region "Mapping Control Methods"

		Private Sub AppToMapBtn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AppToMapBtn.Click
			Try
				supressMappedListSelectedIndexChangedEvents = True
				Dim appField As AppField = CType(AppFieldList.SelectedItem, AppField)
				If appField IsNot Nothing Then
					'Signal controller
					_mapController.MapAppField(appField)
					_mapController.SelectAppFieldForMapping(appField)

					'Update listbox selections
					AppFieldList.SelectedItem = Nothing
					MappedList_App.SelectedItem = appField
					MappedList_Target.SelectedIndex = MappedList_App.SelectedIndex
				End If
			Finally
				supressMappedListSelectedIndexChangedEvents = False
				UpdateMappingButtonsAndUI()
			End Try
		End Sub

		Private Sub MapToAppBtn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MapToAppBtn.Click
			Try
				supressMappedListSelectedIndexChangedEvents = True
				Dim appField As AppField = CType(MappedList_App.SelectedItem, AppField)
				If appField IsNot Nothing Then
					'Signal controller
					_mapController.SelectAppFieldForMapping(appField)
					_mapController.UnmapAppField(appField)
					_mapController.DeselectAppFieldForMapping()

					'Update listbox selections
					AppFieldList.SelectedItem = appField
					MappedList_App.SelectedItem = Nothing
					MappedList_Target.SelectedItem = Nothing
				End If
			Finally
				supressMappedListSelectedIndexChangedEvents = False
				UpdateMappingButtonsAndUI()
			End Try
		End Sub

		Private Sub TargetToMapBtn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TargetToMapBtn.Click
			Try
				supressMappedListSelectedIndexChangedEvents = True
				Dim targetField As TargetField = CType(TargetFieldList.SelectedItem, TargetField)
				If targetField IsNot Nothing Then
					Dim appFieldToMapTo As AppField = CType(MappedList_App.SelectedItem, AppField)
					If appFieldToMapTo IsNot Nothing Then
						_mapController.MapTargetField(targetField, appFieldToMapTo)
						TargetFieldList.SelectedItem = Nothing
						MappedList_Target.SelectedItem = targetField
					End If
				End If
			Finally
				supressMappedListSelectedIndexChangedEvents = False
				UpdateMappingButtonsAndUI()
			End Try
		End Sub

		Private Sub MapToTargetBtn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MapToTargetBtn.Click
			Try
				supressMappedListSelectedIndexChangedEvents = True
				Dim targetField As TargetField = CType(MappedList_Target.SelectedItem, TargetField)
				If targetField IsNot Nothing Then
					_mapController.UnmapTargetField(targetField)
					MappedList_Target.SelectedItem = Nothing
					MappedList_App.SelectedItem = Nothing
					TargetFieldList.SelectedItem = targetField
				End If
			Finally
				supressMappedListSelectedIndexChangedEvents = False
				UpdateMappingButtonsAndUI()
			End Try
		End Sub

		Private Sub WireUpMappingEventHandlers()
			AddHandler ObjectMapComboBox.SelectedIndexChanged, AddressOf ObjectMapComboBox_SelectedIndexChanged
			AddHandler MappedList_App.SelectedIndexChanged, AddressOf MappedList_App_SelectedIndexChanged
			AddHandler MappedList_Target.SelectedIndexChanged, AddressOf MappedList_Target_SelectedIndexChanged
			AddHandler AppFieldList.SelectedIndexChanged, AddressOf AppFieldList_SelectedIndexChanged
			AddHandler TargetFieldList.SelectedIndexChanged, AddressOf TargetFieldList_SelectedIndexChanged
		End Sub

		Private Sub LoadMappingControls(ByVal xml As Xml.XmlDocument)
			_app = ApplicationElement.Deserialize(Of RelativityApplicationElement)(xml)
			Dim factory As New AppMappingDataFactory()
			_appMappingData = factory.CreateMappingData(_app)
			Dim mapCandidateFinder As IMappingCandidateFinder = New MappingCandidateFinder(Me.Credentials, Me.CookieContainer, Me.CaseInfos(0).ArtifactID)
			mapCandidateFinder.PopulateMappingCandidates(xml, _appMappingData)
			_mapController = New FieldMapFourPickerController(_appMappingData)

			'Databinding
			ObjectMapComboBox.DataSource = _mapController.AppObjectsList
			ObjectMapComboBox.DisplayMember = "ObjectName"

			MappedList_App.DataSource = _mapController.AppFieldsList_Mapped
			MappedList_App.DisplayMember = "MyName"

			MappedList_Target.DataSource = _mapController.TargetFieldsList_Mapped
			MappedList_Target.DisplayMember = "MyName"

			AppFieldList.DataSource = _mapController.AppFieldsList_NotMapped
			AppFieldList.DisplayMember = "MyName"

			TargetFieldList.DataSource = _mapController.TargetFieldsList_NotMapped
			TargetFieldList.DisplayMember = "MyName"

			NoTargetFieldsQualifyLabel.Visible = False

			WireUpMappingEventHandlers()
			HandleSelectedObjectChanged()
		End Sub

		Dim supressMappedListSelectedIndexChangedEvents As Boolean = False

		Private Sub ObjectMapComboBox_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
			HandleSelectedObjectChanged()
		End Sub

		Private Sub MappedList_App_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
			If supressMappedListSelectedIndexChangedEvents = True Then Return

			If MappedList_App IsNot Nothing Then
				Try
					supressMappedListSelectedIndexChangedEvents = True
					AppFieldList.SelectedItem = Nothing
				Finally
					supressMappedListSelectedIndexChangedEvents = False
				End Try
			End If

			Dim selectedAppField = CType(MappedList_App.SelectedItem, AppField)
			If selectedAppField Is Nothing Then Return
			_mapController.SelectAppFieldForMapping(selectedAppField)
			If MappedList_App.SelectedIndex <> MappedList_Target.SelectedIndex Then
				MappedList_Target.SelectedIndex = MappedList_App.SelectedIndex
			End If

			UpdateMappingButtonsAndUI()
		End Sub

		Private Sub MappedList_Target_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
			If supressMappedListSelectedIndexChangedEvents = True Then Return

			If MappedList_Target IsNot Nothing Then
				Try
					supressMappedListSelectedIndexChangedEvents = True
					AppFieldList.SelectedItem = Nothing
				Finally
					supressMappedListSelectedIndexChangedEvents = False
				End Try
			End If

			Dim listIdx = MappedList_Target.SelectedIndex
			If listIdx >= 0 Then
				MappedList_App.SelectedIndex = listIdx
			End If

			UpdateMappingButtonsAndUI()
		End Sub

		Private Sub AppFieldList_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
			If supressMappedListSelectedIndexChangedEvents = True Then Return

			Try
				supressMappedListSelectedIndexChangedEvents = True
				MappedList_App.SelectedItem = Nothing
				MappedList_Target.SelectedItem = Nothing
			Finally
				supressMappedListSelectedIndexChangedEvents = False
			End Try

			UpdateMappingButtonsAndUI()
		End Sub

		Private Sub TargetFieldList_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
			If supressMappedListSelectedIndexChangedEvents = True Then Return

			Try
				supressMappedListSelectedIndexChangedEvents = True
				AppFieldList.SelectedItem = Nothing
			Finally
				supressMappedListSelectedIndexChangedEvents = False
			End Try

			UpdateMappingButtonsAndUI()
		End Sub

		Private Sub HandleSelectedObjectChanged()
			If supressMappedListSelectedIndexChangedEvents = True Then Return

			'Populate app field list combo box
			Dim selectedAppObject = CType(ObjectMapComboBox.SelectedItem, AppObject)
			If selectedAppObject Is Nothing Then Return
			_mapController.SelectObject(selectedAppObject)

			UpdateMappingButtonsAndUI()
		End Sub

		Private Sub UpdateMappingButtonsAndUI()
			Dim selectedMappedAppField = CType(MappedList_App.SelectedItem, AppField)
			Dim selectedtargetField = CType(TargetFieldList.SelectedItem, TargetField)
			AppToMapBtn.Enabled = Not AppFieldList.SelectedItem Is Nothing
			MapToAppBtn.Enabled = Not MappedList_App.SelectedItem Is Nothing
			TargetToMapBtn.Enabled = selectedtargetField IsNot Nothing AndAlso selectedMappedAppField IsNot Nothing AndAlso selectedMappedAppField.MappedTargetField.IsEmpty
			MapToTargetBtn.Enabled = selectedtargetField IsNot Nothing AndAlso selectedtargetField.IsEmpty = False
			NoTargetFieldsQualifyLabel.Visible = MappedList_App.SelectedItem IsNot Nothing AndAlso (_mapController.TargetFieldsList_NotMapped.Count = 0)
		End Sub

#End Region

	End Class
End Namespace