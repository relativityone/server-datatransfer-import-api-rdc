Imports System.Collections
Namespace kCura.WinEDDS
	Public Class FolderCache
		Private _ht As Hashtable
		Private _folderManager As Service.FolderManager
		Private _rootFolderID As Int32
		Private _caseContextArtifactID As Int32
		Default Public ReadOnly Property FolderID(ByVal folderPath As String) As Int32
			Get
				If _ht.ContainsKey(folderPath) Then
					Return DirectCast(_ht(folderPath), FolderCacheItem).FolderID
				Else
					Dim newFolder As FolderCacheItem = Me.GetNewFolder(folderPath)
					_ht.Add(folderPath, newFolder)
					Return newFolder.FolderID
				End If
			End Get
		End Property

		Private Function GetNewFolder(ByVal folderPath As String) As FolderCacheItem
			Dim pathToDestination As New ArrayList
			Dim s As String = folderPath.Substring(0, folderPath.LastIndexOf("\"))
			If s = "" Then s = "\"
			pathToDestination.Add(folderPath.Substring(folderPath.LastIndexOf("\") + 1))
			Dim parentFolder As FolderCacheItem = Me.FindParentFolder(s, pathToDestination)
			Return CreateFolders(parentFolder, pathToDestination)
		End Function

		Private Function CreateFolders(ByVal parentFolder As FolderCacheItem, ByVal folderNames As ArrayList) As FolderCacheItem
			If folderNames.Count > 0 Then
				Dim newFolderName As String = CType(folderNames(0), String)
				folderNames.RemoveAt(0)
				Dim newFolderID As Int32 = _folderManager.Create(_caseContextArtifactID, parentFolder.FolderID, newFolderName)
				Dim newFolder As New FolderCacheItem(newFolderName, parentFolder.Path & "\" & newFolderName, newFolderID)
				parentFolder.AddChild(newFolder)
				_ht.Add(newFolder.Path, newFolder)
				Return CreateFolders(newFolder, folderNames)
			Else
				Return parentFolder
			End If
		End Function

		Private Function FindParentFolder(ByVal folderPath As String, ByVal pathToDestination As ArrayList) As FolderCacheItem
			If _ht.ContainsKey(folderPath) Then
				Return DirectCast(_ht(folderPath), FolderCacheItem)
			Else
				Dim pathEntry As String = folderPath.Substring(folderPath.LastIndexOf("\") + 1)
				If pathEntry = "" Then
					pathEntry = "\"
					Return DirectCast(_ht("\"), FolderCacheItem)
				End If
				pathToDestination.Insert(0, pathEntry)
				Return FindParentFolder(folderPath.Substring(0, folderPath.LastIndexOf("\")), pathToDestination)
				End If
		End Function

		Public Sub New(ByVal folderManager As Service.FolderManager, ByVal rootFolderID As Int32, ByVal caseContextArtifactID As Int32)
			_ht = New Hashtable
			_folderManager = folderManager
			_caseContextArtifactID = caseContextArtifactID
			_rootFolderID = rootFolderID
			Dim folderRow As System.Data.DataRow
			Dim foldersDataSet As System.Data.DataSet = _folderManager.RetrieveFolderAndDescendants(_caseContextArtifactID, rootFolderID)
			foldersDataSet.Relations.Add("NodeRelation", foldersDataSet.Tables(0).Columns("ArtifactID"), foldersDataSet.Tables(0).Columns("ParentArtifactID"))
			Dim path As String
			For Each folderRow In foldersDataSet.Tables(0).Rows
				If TypeOf folderRow("ParentArtifactID") Is DBNull Then
					Dim f As New FolderCacheItem("", "\", _rootFolderID)
					_ht.Add(f.Path, f)
					RecursivelyPopulate(folderRow, f)
				End If
			Next
		End Sub

		Private Sub RecursivelyPopulate(ByVal dataRow As System.Data.DataRow, ByVal parent As FolderCacheItem)
			Dim childDataRow As System.Data.DataRow
			Dim childNode As System.Windows.Forms.TreeNode
			Dim newPath As String
			For Each childDataRow In dataRow.GetChildRows("NodeRelation")
				If parent.Path = "\" Then
					newPath = "\" & childDataRow("Name").ToString
				Else
					newPath = parent.Path & "\" & childDataRow("Name").ToString
				End If
				Dim childFolder As New FolderCacheItem(childDataRow("Name").ToString, newPath, CType(childDataRow("ArtifactID"), Int32))
				_ht.Add(childFolder.Path, childFolder)
				parent.AddChild(childFolder)
				RecursivelyPopulate(childDataRow, childFolder)
			Next
		End Sub
	End Class

#Region "Folder Cache Item"
	Public Class FolderCacheItem
		Private _name As String
		Private _path As String
		Private _folderID As Int32
		Private _children As System.Collections.ArrayList
		Public Property Name() As String
			Get
				Return _name
			End Get
			Set(ByVal value As String)
				_name = value
			End Set
		End Property

		Public Property Path() As String
			Get
				Return _path
			End Get
			Set(ByVal value As String)
				_path = value
			End Set
		End Property

		Public Property FolderID() As Int32
			Get
				Return _folderID
			End Get
			Set(ByVal value As Int32)
				_folderID = value
			End Set
		End Property

		Public Property Children() As FolderCacheItem()
			Get
				Return DirectCast(_children.ToArray(GetType(FolderCacheItem)), FolderCacheItem())
			End Get
			Set(ByVal value As FolderCacheItem())
				_children = New ArrayList(value)
			End Set
		End Property

		Public Sub AddChild(ByVal folderCacheItem As FolderCacheItem)
			_children.Add(folderCacheItem)
		End Sub

		Public Sub New(ByVal name As String, ByVal path As String, ByVal folderID As Int32)
			_name = name
			_path = path
			_folderID = folderID
			_children = New ArrayList
		End Sub
	End Class
#End Region
End Namespace