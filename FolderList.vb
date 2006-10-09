Namespace kCura.WinEDDS
	Public Class FolderList
		Private _folders As System.Collections.ArrayList
		Private _maxOrder As Int32

		Private Function Item(ByVal index As Int32) As kCura.WinEDDS.FolderList.FolderItem
			Return CType(_folders.Item(index), kCura.WinEDDS.FolderList.FolderItem)
		End Function

		Public Function ItemByArtifactID(ByVal artifactID As Int32) As kCura.WinEDDS.FolderList.FolderItem
			Dim folderItem As kCura.WinEDDS.FolderList.FolderItem

			For Each folderItem In _folders
				If folderItem.ArtifactID = artifactID Then
					Return folderItem
				End If
			Next
		End Function

		Public Function BaseFolder() As kCura.WinEDDS.FolderList.FolderItem
			Dim folderItem As kCura.WinEDDS.FolderList.FolderItem

			For Each folderItem In _folders
				If folderItem.Order = 0 Then
					Return folderItem
				End If
			Next
		End Function

		Public Sub CreateFolders(ByVal path As String)
			Dim order As Int32
			Dim folderItem As kCura.WinEDDS.FolderList.FolderItem
			'path = Utility.GetFilesystemSafeName(path)
			For order = 0 To _maxOrder
				For Each folderItem In _folders
					If folderItem.Order = order AndAlso Not System.IO.Directory.Exists(path + folderItem.Path) Then
						System.IO.Directory.CreateDirectory(path + folderItem.Path)
					End If
				Next
			Next
		End Sub

		Public Sub DeleteEmptyFolders(ByVal path As String)
			Dim order As Int32 = _maxOrder
			Dim folderItem As kCura.WinEDDS.FolderList.FolderItem
			While order > -1
				For Each folderItem In _folders
					If folderItem.Order = order AndAlso System.IO.Directory.Exists(path + folderItem.Path) Then
						If System.IO.Directory.GetFiles(path + folderItem.Path).Length = 0 AndAlso System.IO.Directory.GetDirectories(path + folderItem.Path).Length = 0 Then
							System.IO.Directory.Delete(path + folderItem.Path)
						End If
					End If
				Next
				order = order - 1
			End While
		End Sub

		Public Sub New(ByVal folderTable As System.Data.DataTable)
			Dim row As System.Data.DataRow
			Dim folderItem As kCura.WinEDDS.FolderList.FolderItem

			_maxOrder = 0
			_folders = New System.Collections.ArrayList
			For Each row In folderTable.Rows
				If TypeOf (row("ParentArtifactID")) Is System.DBNull Then
					_folders.Add(New kCura.WinEDDS.FolderList.FolderItem(CType(row("Name"), String), CType(row("ArtifactID"), Int32), 0))
				Else
					_folders.Add(New kCura.WinEDDS.FolderList.FolderItem(CType(row("Name"), String), CType(row("ArtifactID"), Int32), CType(row("ParentArtifactID"), Int32)))
				End If
			Next
			For Each folderItem In _folders
				If folderItem.ParentArtifactID = 0 Then
					folderItem.Path = Utility.GetFilesystemSafeName(folderItem.Name) + "\"
					folderItem.Order = 0
					PathFolderItems(folderItem)
				End If
			Next
		End Sub

		Private Sub PathFolderItems(ByVal parentFolderItem As kCura.WinEDDS.FolderList.FolderItem)
			Dim folderItem As kCura.WinEDDS.FolderList.FolderItem

			For Each folderItem In _folders
				If folderItem.ParentArtifactID = parentFolderItem.ArtifactID Then
					folderItem.Path = parentFolderItem.Path + Utility.GetFilesystemSafeName(folderItem.Name) + "\"
					folderItem.Order = parentFolderItem.Order + 1
					If folderItem.Order > _maxOrder Then
						_maxOrder = folderItem.Order
					End If
					PathFolderItems(folderItem)
				End If
			Next
		End Sub

		Public Class FolderItem
			Public ArtifactID As Int32
			Public ParentArtifactID As Int32
			Public Path As String
			Public Order As Int32
			Public Name As String

			Public Sub New(ByVal name As String, ByVal artifactID As Int32, ByVal parentArtifactID As Int32)
				Me.Name = name
				Me.ArtifactID = artifactID
				Me.ParentArtifactID = parentArtifactID
			End Sub

			'Public ReadOnly Property SafePath() As String
			'	Get
			'		Dim retval As String = Utility.GetFilesystemSafeName(Me.Path)
			'		retval = retval.TrimEnd(" "c)
			'		Return retval & "\"
			'	End Get
			'End Property
		End Class
	End Class
End Namespace