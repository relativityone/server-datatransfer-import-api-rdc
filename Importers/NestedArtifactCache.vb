Imports System.Collections
Namespace kCura.WinEDDS
	Public Class NestedArtifactCache

		Private _ht As New Hashtable
		Private _manager As Service.IHierarchicArtifactManager
		Private _rootArtifactID As Int32
		Private _caseContextArtifactID As Int32
		Private _nestedItemDelimiter As String

		Public ReadOnly Property SelectedIds(ByVal artifactPath As String) As Int32()
			Get
				Dim artifactVal As Int32 = -1
				If _ht.ContainsKey(artifactPath) Then
					artifactVal = DirectCast(_ht(artifactPath), ArtifactCacheItem).ArtifactID
				Else
					Dim newArtifact As ArtifactCacheItem = Me.GetNewArtifact(artifactPath)
					If Not _ht.ContainsKey(artifactPath) Then _ht.Add(artifactPath, newArtifact)
					artifactVal = newArtifact.ArtifactID
				End If
				Dim strings As String() = artifactPath.Trim(_nestedItemDelimiter.ToCharArray).Split(_nestedItemDelimiter.ToCharArray)
				Dim retval As New System.Collections.ArrayList
				If strings.Length = 1 Then Return New Int32() {artifactVal}
				Dim keyLookup As String = ""
				For i As Int32 = 0 To strings.Length - 1
					keyLookup &= _nestedItemDelimiter
					keyLookup &= strings(i)
					retval.Add(DirectCast(_ht(keyLookup), ArtifactCacheItem).ArtifactID)
				Next
				Return DirectCast(retval.ToArray(GetType(Int32)), Int32())
			End Get
		End Property

		Private Function GetNewArtifact(ByVal fullPath As String) As ArtifactCacheItem
			Dim pathToDestination As New ArrayList
			Dim s As String
			If fullPath = "" OrElse fullPath = _nestedItemDelimiter Then
				s = _nestedItemDelimiter
			Else
				s = fullPath.Substring(0, fullPath.LastIndexOf(_nestedItemDelimiter))
			End If
			pathToDestination.Add(fullPath.Substring(fullPath.LastIndexOf(_nestedItemDelimiter) + 1))
			Dim parentArtifact As ArtifactCacheItem = Me.FindParentArtifact(s, pathToDestination)
			Return CreateArtifacts(parentArtifact, pathToDestination)
		End Function

		Private Function CreateArtifacts(ByVal parentArtifact As ArtifactCacheItem, ByVal artifactNames As ArrayList) As ArtifactCacheItem
			If artifactNames.Count > 0 Then
				Dim newArtifactName As String = CType(artifactNames(0), String)
				artifactNames.RemoveAt(0)
				Dim newArtifactID As Int32 = _manager.Create(_caseContextArtifactID, parentArtifact.ArtifactID, newArtifactName)
				Dim parentArtifactPath As String
				If parentArtifact.Path = _nestedItemDelimiter Then
					parentArtifactPath = ""
				Else
					parentArtifactPath = parentArtifact.Path
				End If
				Dim newArtifact As New ArtifactCacheItem(newArtifactName, parentArtifactPath & _nestedItemDelimiter & newArtifactName, newArtifactID)
				parentArtifact.AddChild(newArtifact)
				_ht.Add(newArtifact.Path, newArtifact)
				Return CreateArtifacts(newArtifact, artifactNames)
			Else
				Return parentArtifact
			End If
		End Function

		Private Function FindParentArtifact(ByVal artifactPath As String, ByVal pathToDestination As ArrayList) As ArtifactCacheItem
			If _ht.ContainsKey(artifactPath) Then
				Return DirectCast(_ht(artifactPath), ArtifactCacheItem)
			Else
				Dim pathEntry As String = artifactPath.Substring(artifactPath.LastIndexOf(_nestedItemDelimiter) + 1)
				If pathEntry = "" Then
					pathEntry = _nestedItemDelimiter
					Return DirectCast(_ht(_nestedItemDelimiter), ArtifactCacheItem)
				End If
				pathToDestination.Insert(0, pathEntry)
				Return FindParentArtifact(artifactPath.Substring(0, artifactPath.LastIndexOf(_nestedItemDelimiter)), pathToDestination)
			End If
		End Function

		Public Sub New(ByVal artifactManager As Service.IHierarchicArtifactManager, ByVal rootArtifactID As Int32, ByVal caseContextArtifactID As Int32, ByVal nestedItemDelimiter As String)
			_ht = New Hashtable
			_manager = artifactManager
			_caseContextArtifactID = caseContextArtifactID
			_rootArtifactID = rootArtifactID
			_nestedItemDelimiter = nestedItemDelimiter
			Dim artifactRow As System.Data.DataRow
			Dim artifactsDataSet As System.Data.DataSet = _manager.RetrieveArtifacts(_caseContextArtifactID, rootArtifactID)
			artifactsDataSet.Relations.Add("NodeRelation", artifactsDataSet.Tables(0).Columns("ArtifactID"), artifactsDataSet.Tables(0).Columns("ParentArtifactID"))
			Dim path As String
			For Each artifactRow In artifactsDataSet.Tables(0).Rows
				If TypeOf artifactRow("ParentArtifactID") Is DBNull Then
					Dim f As New ArtifactCacheItem("", _nestedItemDelimiter, _rootArtifactID)
					If Not _ht.ContainsKey(f.Path) Then _ht.Add(f.Path, f)
					RecursivelyPopulate(artifactRow, f)
				End If
			Next
		End Sub

		Private Sub RecursivelyPopulate(ByVal dataRow As System.Data.DataRow, ByVal parent As ArtifactCacheItem)
			Dim childDataRow As System.Data.DataRow
			Dim childNode As System.Windows.Forms.TreeNode
			Dim newPath As String
			For Each childDataRow In dataRow.GetChildRows("NodeRelation")
				If parent.Path = _nestedItemDelimiter Then
					newPath = _nestedItemDelimiter & childDataRow("Name").ToString
				Else
					newPath = parent.Path & _nestedItemDelimiter & childDataRow("Name").ToString
				End If
				Dim childArtifact As New ArtifactCacheItem(childDataRow("Name").ToString, newPath, CType(childDataRow("ArtifactID"), Int32))
				If Not _ht.ContainsKey(childArtifact.Path) Then
					_ht.Add(childArtifact.Path, childArtifact)
					parent.AddChild(childArtifact)
				End If
				RecursivelyPopulate(childDataRow, childArtifact)
			Next
		End Sub
	End Class

#Region "Artifact Cache Item"
	Public Class ArtifactCacheItem
		Private _name As String
		Private _path As String
		Private _artifactID As Int32
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

		Public Property ArtifactID() As Int32
			Get
				Return _artifactID
			End Get
			Set(ByVal value As Int32)
				_artifactID = value
			End Set
		End Property

		Public Property Children() As ArtifactCacheItem()
			Get
				Return DirectCast(_children.ToArray(GetType(ArtifactCacheItem)), ArtifactCacheItem())
			End Get
			Set(ByVal value As ArtifactCacheItem())
				_children = New ArrayList(value)
			End Set
		End Property

		Public Sub AddChild(ByVal item As ArtifactCacheItem)
			_children.Add(item)
		End Sub

		Public Sub New(ByVal name As String, ByVal path As String, ByVal artifactID As Int32)
			_name = name
			_path = path
			_artifactID = artifactID
			_children = New ArrayList
		End Sub
	End Class
#End Region

End Namespace