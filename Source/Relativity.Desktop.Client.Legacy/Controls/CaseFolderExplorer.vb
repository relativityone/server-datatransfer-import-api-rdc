Imports System.Threading.Tasks
Imports kCura.Windows.Forms

Namespace kCura.EDDS.WinForm
	Public Class CaseFolderExplorer
		Inherits System.Windows.Forms.UserControl

#Region " Windows Form Designer generated code "

		Public Sub New()
			MyBase.New()

			'This call is required by the Windows Form Designer.
			InitializeComponent()

			'Add any initialization after the InitializeComponent() call
			_application = kCura.EDDS.WinForm.Application.Instance
		End Sub

		'UserControl overrides dispose to clean up the component list.
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
		Friend WithEvents _treeView As System.Windows.Forms.TreeView
		Friend WithEvents ImageList As System.Windows.Forms.ImageList
		Friend WithEvents Import As System.Windows.Forms.MenuItem
		Friend WithEvents ImportImageFile As System.Windows.Forms.MenuItem
		Friend WithEvents ImportLoadFIle As System.Windows.Forms.MenuItem
		Friend WithEvents Export As System.Windows.Forms.MenuItem
		Friend WithEvents ExportFolder As System.Windows.Forms.MenuItem
		Friend WithEvents ExportFolderAndSubfolders As System.Windows.Forms.MenuItem
		Friend WithEvents _folderContextMenu As System.Windows.Forms.ContextMenu
		Friend WithEvents NewFolderMenu As System.Windows.Forms.MenuItem
		Friend WithEvents MenuItem2 As System.Windows.Forms.MenuItem
		Friend WithEvents ImportProduction As System.Windows.Forms.MenuItem
		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
			Me.components = New System.ComponentModel.Container
			Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(CaseFolderExplorer))
			Me._treeView = New System.Windows.Forms.TreeView
			Me.ImageList = New System.Windows.Forms.ImageList(Me.components)
			Me._folderContextMenu = New System.Windows.Forms.ContextMenu
			Me.NewFolderMenu = New System.Windows.Forms.MenuItem
			Me.MenuItem2 = New System.Windows.Forms.MenuItem
			Me.Import = New System.Windows.Forms.MenuItem
			Me.ImportImageFile = New System.Windows.Forms.MenuItem
			Me.ImportLoadFIle = New System.Windows.Forms.MenuItem
			Me.ImportProduction = New System.Windows.Forms.MenuItem
			Me.Export = New System.Windows.Forms.MenuItem
			Me.ExportFolder = New System.Windows.Forms.MenuItem
			Me.ExportFolderAndSubfolders = New System.Windows.Forms.MenuItem
			Me.SuspendLayout()
			'
			'_treeView
			'
			Me._treeView.Dock = System.Windows.Forms.DockStyle.Fill
			Me._treeView.HideSelection = False
			Me._treeView.ImageList = Me.ImageList
			Me._treeView.Location = New System.Drawing.Point(0, 0)
			Me._treeView.Name = "_treeView"
			Me._treeView.Size = New System.Drawing.Size(150, 0)
			Me._treeView.Sorted = True
			Me._treeView.TabIndex = 0
			'
			'ImageList
			'
			Me.ImageList.ImageSize = New System.Drawing.Size(16, 16)
			Me.ImageList.ImageStream = CType(resources.GetObject("ImageList.ImageStream"), System.Windows.Forms.ImageListStreamer)
			Me.ImageList.TransparentColor = System.Drawing.Color.Transparent
			'
			'_folderContextMenu
			'
			Me._folderContextMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.NewFolderMenu, Me.MenuItem2, Me.Import, Me.Export})
			'
			'NewFolderMenu
			'
			Me.NewFolderMenu.Index = 0
			Me.NewFolderMenu.Text = "&New Folder..."
			'
			'ToolsMenu
			'
			Me.MenuItem2.Index = 1
			Me.MenuItem2.Text = "-"
			'
			'Import
			'
			Me.Import.Index = 2
			Me.Import.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.ImportImageFile, Me.ImportLoadFIle, Me.ImportProduction})
			Me.Import.Text = "&Import"
			'
			'ImportImageFile
			'
			Me.ImportImageFile.Index = 0
			Me.ImportImageFile.Text = "&Image File..."
			'
			'ImportLoadFIle
			'
			Me.ImportLoadFIle.Index = 1
			Me.ImportLoadFIle.Text = "&Load File..."
			'
			'ImportProduction
			'
			Me.ImportProduction.Index = 2
			Me.ImportProduction.Text = "Production File..."

			'
			'Export
			'
			Me.Export.Index = 3
			Me.Export.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.ExportFolder, Me.ExportFolderAndSubfolders})
			Me.Export.Text = "&Export"
			'
			'ExportFolder
			'
			Me.ExportFolder.Index = 0
			Me.ExportFolder.Text = "&Folder..."
			'
			'ExportFolderAndSubfolders
			'
			Me.ExportFolderAndSubfolders.Index = 1
			Me.ExportFolderAndSubfolders.Text = "&Folder And Subfolders..."
			'
			'CaseFolderExplorer
			'
			Me.Controls.Add(Me._treeView)
			Me.Name = "CaseFolderExplorer"
			Me.Size = New System.Drawing.Size(150, 0)
			Me.ResumeLayout(False)

		End Sub

#End Region

		Protected WithEvents _application As kCura.EDDS.WinForm.Application

		Public Property ImportContextMenuEnabled() As Boolean
			Get
				Return Import.Enabled
			End Get
			Set
				Import.Enabled = Value
			End Set
		End Property

		Public Property ExportContextMenuEnabled() As Boolean
			Get
				Return Export.Enabled
			End Get
			Set
				Export.Enabled = Value
			End Set
		End Property

		Private _contextMenuTreeNode As System.Windows.Forms.TreeNode

		Private Async Function LoadCase(ByVal caseInfo As Relativity.CaseInfo) As Task
			'check import/export permissions for the case
			Await _application.LoadWorkspacePermissions()

			' sets the availability of import/export menu based on the permissions set for logged in user
			Import.Enabled = _application.UserHasImportPermission
			Export.Enabled = _application.UserHasExportPermission

			_treeView.Nodes.Clear()

			Dim foldersDataSet As System.Data.DataSet = Nothing
			Try
				foldersDataSet = Await _application.GetCaseFolders(caseInfo.ArtifactID)
			Catch ex As System.Exception
				Dim frm As New ErrorDialog
				frm.Initialize(ex, "Error retrieving folder information from server.  Continue?")
				frm.ShowDialog()
				Select Case frm.DialogResult
					Case DialogResult.OK
						foldersDataSet = _application.GetCaseRootFolderForErrorState(caseInfo.ArtifactID)
					Case DialogResult.Cancel
						_application.ExitApplication()
				End Select
			End Try
			If foldersDataSet Is Nothing Then Return
			'Dim cleanser As New DataSetCleanser
			'cleanser.CleanseDataset(foldersDataSet)
			foldersDataSet.Relations.Add("NodeRelation", foldersDataSet.Tables(0).Columns("ArtifactID"), foldersDataSet.Tables(0).Columns("ParentArtifactID"))
			Dim folderRow As System.Data.DataRow
			Dim rootFolderNode As New System.Windows.Forms.TreeNode

			For Each folderRow In foldersDataSet.Tables(0).Rows
				If folderRow("ParentArtifactID") Is System.DBNull.Value Then
					Dim tag As New FolderInfo(CType(folderRow("ArtifactID"), Int32), "Folder")
					tag.Path = "\"
					rootFolderNode.Tag = tag
					rootFolderNode.Text = CType(folderRow("Name"), String)
					rootFolderNode.ImageIndex = 0
					rootFolderNode.SelectedImageIndex = 0
					_treeView.Nodes.Add(rootFolderNode)
					RecursivelyPopulate(folderRow, rootFolderNode, tag.Path)
				End If
			Next
			'rootFolderNode.ExpandAll()
			_treeView.SelectedNode = rootFolderNode

		End Function

		Private Sub RecursivelyPopulate(ByVal dataRow As System.Data.DataRow, ByVal node As System.Windows.Forms.TreeNode, ByVal currentPath As String)
			Dim childDataRow As System.Data.DataRow
			Dim childNode As System.Windows.Forms.TreeNode
			Dim childRows As System.Data.DataRow() = dataRow.GetChildRows("NodeRelation")
			System.Array.Sort(childRows, New FolderRowComparer)
			For Each childDataRow In childRows
				Dim tag As New FolderInfo(CType(childDataRow("ArtifactID"), Int32), "Folder")
				childNode = New System.Windows.Forms.TreeNode
				tag.Path = currentPath & CType(childDataRow("Name"), String) & "\"
				childNode.Tag = tag
				childNode.Text = CType(childDataRow("Name"), String)
				childNode.ImageIndex = 1
				childNode.SelectedImageIndex = 1
				node.Nodes.Add(childNode)
				RecursivelyPopulate(childDataRow, childNode, tag.Path)
			Next
		End Sub

		Private Sub _application_OnEvent(ByVal appEvent As AppEvent) Handles _application.OnEvent
			Select Case appEvent.EventType
				Case appEvent.AppEventType.LoadCase
					_treeView.Invoke(Async Function() As Task
										 Await Me.LoadCase(CType(appEvent, LoadCaseEvent).Case)
									 End Function)
				Case appEvent.AppEventType.NewFolder
					_treeView.Invoke(Sub() Me.AddNewFolder(CType(appEvent, NewFolderEvent)))
			End Select
		End Sub

		Private Sub AddNewFolder(ByVal folderDetails As NewFolderEvent)

			Dim parentNode As System.Windows.Forms.TreeNode
			parentNode = FindNode(folderDetails.ParentFolderID)
			Dim newFolderNode As New System.Windows.Forms.TreeNode
			Dim tag As New FolderInfo(folderDetails.FolderID, "Folder")
			tag.Path = DirectCast(parentNode.Tag, FolderInfo).Path & folderDetails.ExportFriendlyName & "\"
			newFolderNode.Tag = tag
			newFolderNode.Text = folderDetails.ExportFriendlyName
			newFolderNode.ImageIndex = 1
			newFolderNode.SelectedImageIndex = 1
			parentNode.Nodes.Add(newFolderNode)
			parentNode.Expand()
			_treeView.SelectedNode = newFolderNode

		End Sub

		Private Function FindNode(ByVal nodeArtifactID As Int32) As System.Windows.Forms.TreeNode

			Return FindNode(_treeView.Nodes, nodeArtifactID)

		End Function
		Private Function FindNode(ByVal nodes As System.Windows.Forms.TreeNodeCollection, ByVal nodeArtifactID As Int32) As System.Windows.Forms.TreeNode

			Dim node As System.Windows.Forms.TreeNode
			For Each node In nodes
				If CType(node.Tag, FolderInfo).ArtifactID = nodeArtifactID Then
					Return node
				End If
				Dim subnode As System.Windows.Forms.TreeNode = FindNode(node.Nodes, nodeArtifactID)
				If Not (subnode Is Nothing) Then
					Return subnode
				End If
			Next

			Return Nothing

		End Function

		Private Sub _treeView_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles _treeView.MouseDown
			_contextMenuTreeNode = _treeView.GetNodeAt(New System.Drawing.Point(e.X, e.Y))
			_treeView.SelectedNode = _contextMenuTreeNode
			If e.Button = MouseButtons.Right AndAlso Not _contextMenuTreeNode Is Nothing AndAlso CType(_contextMenuTreeNode.Tag, FolderInfo).Type = "Folder" Then
				_folderContextMenu.Show(_treeView, New Point(e.X, e.Y))
			End If
		End Sub

		Private Async Sub ImportImageFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ImportImageFile.Click
			Await _application.NewImageFile(CType(_contextMenuTreeNode.Tag, FolderInfo).ArtifactID, _application.SelectedCaseInfo)
		End Sub

		Private Async Sub ImportLoadFIle_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ImportLoadFIle.Click
			Await _application.NewLoadFile(CType(_contextMenuTreeNode.Tag, FolderInfo).ArtifactID, _application.SelectedCaseInfo)
		End Sub

		Private Sub _treeView_AfterSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles _treeView.AfterSelect
			_application.SelectCaseFolder(CType(_treeView.SelectedNode.Tag, FolderInfo))
		End Sub

		Private Async Sub NewFolderMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewFolderMenu.Click
			Await _application.CreateNewFolder(CType(_treeView.SelectedNode.Tag, FolderInfo).ArtifactID)
		End Sub

		Private Async Sub ExportFolder_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ExportFolder.Click
			Await _application.NewSearchExport(CType(_contextMenuTreeNode.Tag, FolderInfo).ArtifactID, _application.SelectedCaseInfo, ExportFile.ExportType.ParentSearch)
		End Sub

		Private Async Sub ExportFolderAndSubfolders_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ExportFolderAndSubfolders.Click
			Await _application.NewSearchExport(CType(_contextMenuTreeNode.Tag, FolderInfo).ArtifactID, _application.SelectedCaseInfo, ExportFile.ExportType.AncestorSearch)
		End Sub

		Private Async Sub ImportProduction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ImportProduction.Click
			Await _application.NewProductionFile(CType(_contextMenuTreeNode.Tag, FolderInfo).ArtifactID, _application.SelectedCaseInfo)
		End Sub

		Private Class FolderRowComparer
			Implements IComparer
			Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements System.Collections.IComparer.Compare
				Return String.Compare(DirectCast(x, System.Data.DataRow)("Name").ToString.ToLower.Trim, DirectCast(y, System.Data.DataRow)("Name").ToString.ToLower.Trim)
			End Function
		End Class
	End Class
End Namespace