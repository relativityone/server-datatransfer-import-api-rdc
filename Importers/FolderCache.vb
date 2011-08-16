Imports System.Collections
Imports System.Collections.Generic

Namespace kCura.WinEDDS
	Public Class FolderCache
		Private _ht As Hashtable
		Private _folderManager As Service.FolderManager
		Private _rootFolderID As Int32
		Private _caseContextArtifactID As Int32
		Private _serviceURL As String

		Public Property ServiceURL As String
			Get
				Return _serviceURL
			End Get
			Set(value As String)
				_serviceURL = value
				_folderManager.ServiceURL = value
			End Set
		End Property

		Default Public ReadOnly Property FolderID(ByVal folderPath As String) As Int32
			Get
				Dim newFolderPath As New System.Text.StringBuilder
				For Each folder As String In folderPath.Split("\"c)
					If Not folder.Trim = "" Then
						newFolderPath.Append("\" & folder.Trim)
					End If
				Next
				folderPath = newFolderPath.ToString
				If folderPath = "" Then folderPath = "\"
				If _ht.ContainsKey(folderPath) Then
					Return DirectCast(_ht(folderPath), FolderCacheItem).FolderID
				Else
					Dim newFolder As FolderCacheItem = Me.GetNewFolder(folderPath)
					If Not _ht.ContainsKey(folderPath) Then _ht.Add(folderPath, newFolder)
					Return newFolder.FolderID
				End If
			End Get
		End Property

		Private Function GetNewFolder(ByVal folderPath As String) As FolderCacheItem
			Dim pathToDestination As New List(Of String)
			Dim s As String
			If folderPath = "" OrElse folderPath = "\" Then
				s = "\"
			Else
				s = folderPath.Substring(0, folderPath.LastIndexOf("\"))
			End If
			pathToDestination.Add(folderPath.Substring(folderPath.LastIndexOf("\") + 1))
			Dim parentFolder As FolderCacheItem = Me.FindParentFolder(s, pathToDestination)
			Return CreateFolders(parentFolder, pathToDestination)
		End Function

		Private Function CreateFolders(ByVal parentFolder As FolderCacheItem, ByVal folderNames As List(Of String)) As FolderCacheItem
			If folderNames.Count > 0 Then
				Dim newFolderName As String = CType(folderNames(0), String)
				folderNames.RemoveAt(0)

				'We've gotten this far, so the hashtable mapping of folder paths to artifact ids doesn't contain the folder of interest.
				'But the hashtable isn't shared across simultaneous imports (via multiple application instances),
				'so check the database to see if the folder was already created by somebody else.  If not, go ahead and create it.
				Dim newFolderID As Int32 = _folderManager.ReadID(_caseContextArtifactID, parentFolder.FolderID, newFolderName)
				If -1 = newFolderID Then
					newFolderID = _folderManager.Create(_caseContextArtifactID, parentFolder.FolderID, newFolderName)
				End If

				Dim parentFolderPath As String
				If parentFolder.Path = "\" Then
					parentFolderPath = ""
				Else
					parentFolderPath = parentFolder.Path
				End If
				Dim newFolder As New FolderCacheItem(parentFolderPath & "\" & newFolderName, newFolderID)
				_ht.Add(newFolder.Path, newFolder)
				Return CreateFolders(newFolder, folderNames)
			Else
				Return parentFolder
			End If
		End Function

		Private Function FindParentFolder(ByVal folderPath As String, ByVal pathToDestination As List(Of String)) As FolderCacheItem
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
			Me.New(folderManager, rootFolderID, caseContextArtifactID, kCura.WinEDDS.Config.WebServiceURL)
		End Sub

		Public Sub New(ByVal folderManager As Service.FolderManager, ByVal rootFolderID As Int32, ByVal caseContextArtifactID As Int32, ByVal webURL As String)
			_ht = New Hashtable
			'Update ServiceURL property after setting _folderManager to update _folderManager.ServiceURL as well
			_folderManager = folderManager
			ServiceURL = webURL
			_caseContextArtifactID = caseContextArtifactID
			_rootFolderID = rootFolderID
			Dim folderRow As System.Data.DataRow
			Dim foldersDataSet As System.Data.DataSet = _folderManager.RetrieveFolderAndDescendants(_caseContextArtifactID, rootFolderID)
			foldersDataSet.Relations.Add("NodeRelation", foldersDataSet.Tables(0).Columns("ArtifactID"), foldersDataSet.Tables(0).Columns("ParentArtifactID"))

			For Each folderRow In foldersDataSet.Tables(0).Rows
				If TypeOf folderRow("ParentArtifactID") Is DBNull Then
					Dim f As New FolderCacheItem("\", _rootFolderID)
					If Not _ht.ContainsKey(f.Path) Then _ht.Add(f.Path, f)
					RecursivelyPopulate(folderRow, f)
				End If
			Next
		End Sub

		Private Sub RecursivelyPopulate(ByVal dataRow As System.Data.DataRow, ByVal parent As FolderCacheItem)
			Dim childDataRow As System.Data.DataRow
			Dim newPath As String
			For Each childDataRow In dataRow.GetChildRows("NodeRelation")
				If parent.Path = "\" Then
					newPath = "\" & childDataRow("Name").ToString.Trim
				Else
					newPath = parent.Path & "\" & childDataRow("Name").ToString.Trim
				End If
				Dim childFolder As New FolderCacheItem(newPath.Trim, CType(childDataRow("ArtifactID"), Int32))
				If Not _ht.ContainsKey(childFolder.Path.Trim) Then
					_ht.Add(childFolder.Path.Trim, childFolder)
				End If
				RecursivelyPopulate(childDataRow, childFolder)
			Next
		End Sub
	End Class

#Region "Folder Cache Item"
	Public Class FolderCacheItem
		Private _path As String
		Private _folderID As Int32

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

		Public Sub New(ByVal path As String, ByVal folderID As Int32)
			_path = path
			_folderID = folderID
		End Sub
	End Class
#End Region
End Namespace