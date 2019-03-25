Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports kCura.WinEDDS.Importers
Imports kCura.WinEDDS.Service

Namespace kCura.WinEDDS

	''' <summary>
	''' Represents a class object to manage client-side folder operations through a simple cache.
	''' </summary>
	''' <remarks>
	''' This type is thread safe.
	''' </remarks>
	Public Class FolderCache
		Implements IFolderCache

		Public Const PathSeparatorChar As Char = "\"C
		Public Const PathSeparator As String = "\"
		Public Const FolderNotFoundId As Integer = -1
		Private ReadOnly _dictionary As ConcurrentDictionary(Of String, FolderCacheItem)
		Private ReadOnly _rootFolder As FolderCacheItem
		Private ReadOnly _hierarchicArtifactManager As IHierarchicArtifactManager
		Private ReadOnly _rootFolderId As Int32
		Private ReadOnly _workspaceId As Int32
		Private ReadOnly _logger As Relativity.Logging.ILog

		''' <summary>
		''' Initializes a new instance of the <see cref="FolderCache"/> class.
		''' </summary>
		''' <param name="logger">
		''' The Relativity logger instance.
		''' </param>
		''' <param name="hierarchicArtifactManager">
		''' The interface used to manage folder hierarchical artifacts.
		''' </param>
		''' <param name="rootFolderId">
		''' The root folder artifact identifier.
		''' </param>
		''' <param name="workspaceId">
		''' The workspace artifact identifier.
		''' </param>
		''' <exception cref="kCura.WinEDDS.Exceptions.WebApiException">
		''' Thrown when a failure occurs retrieving folders.
		''' </exception>
		Public Sub New(logger As Relativity.Logging.ILog, hierarchicArtifactManager As IHierarchicArtifactManager, rootFolderId As Int32, workspaceId As Int32)
			If (logger Is Nothing) Then
				Throw New ArgumentNullException(NameOf(logger))
			End If

			If (hierarchicArtifactManager Is Nothing) Then
				Throw New ArgumentNullException(NameOf(hierarchicArtifactManager))
			End If

			If (rootFolderId < 1) Then
				Throw New ArgumentOutOfRangeException(NameOf(rootFolderId), "The root folder artifact identifier must be greater than zero.")
			End If

			If (workspaceId < 1) Then
				Throw New ArgumentOutOfRangeException(NameOf(workspaceId), "The workspace artifact identifier must be greater than zero.")
			End If

			_logger = logger
			_hierarchicArtifactManager = hierarchicArtifactManager
			_workspaceId = workspaceId
			_rootFolderId = rootFolderID

			' Ensure the root folder always exists within the cache.
			_dictionary = New ConcurrentDictionary(Of String, FolderCacheItem)
			_rootFolder = New FolderCacheItem(PathSeparator, _rootFolderId)
			_dictionary(_rootFolder.Path) = _rootFolder

			Try
				Dim folderRow As System.Data.DataRow
				Dim foldersDataSet As System.Data.DataSet = _hierarchicArtifactManager.RetrieveArtifacts(_workspaceId, rootFolderID)
				foldersDataSet.Relations.Add("NodeRelation", foldersDataSet.Tables(0).Columns("ArtifactID"), foldersDataSet.Tables(0).Columns("ParentArtifactID"))
				For Each folderRow In foldersDataSet.Tables(0).Rows
					If TypeOf folderRow("ParentArtifactID") Is DBNull Then
						RecursivelyPopulate(folderRow, _rootFolder)
					End If
				Next
			Catch ex As Exception
				If kCura.WinEDDS.Helpers.ExceptionHelper.IsFatalException(ex) Then
					Throw
				End If

				_logger.LogFatal(ex, "Failed to retrieve all sub-folders within the {RootFolderId} root folder for workspace {WorkspaceId}.", rootFolderID, _workspaceId)
				Dim message As String = $"A fatal error occurred retrieving all sub-folders within the {rootFolderID} root folder for workspace {_workspaceId}."				
				message = kCura.WinEDDS.Helpers.ExceptionHelper.AppendTryAgainAdminFatalMessage(message)
				Throw New kCura.WinEDDS.Exceptions.WebApiException(message, ex)
			End Try
		End Sub

		''' <summary>
		''' Gets the total number of folders in the cache.
		''' </summary>
		''' <value>
		''' The total count.
		''' </value>
		Public ReadOnly Property Count As Int32 Implements IFolderCache.Count
			Get
				Return _dictionary.Count
			End Get
		End Property

		''' <summary>
		''' Retrieves the folder artifact identifier for the specified folder path and automatically create all sub-folder paths that don't already exist.
		''' </summary>
		''' <param name="folderPath">
		''' The path used to lookup the folder artifact identifier.
		''' </param>
		''' <returns>
		''' The folder artifact identifier.
		''' </returns>
		''' <exception cref="kCura.WinEDDS.Exceptions.WebApiException">
		''' Thrown when a failure occurs retrieving or creating folders.
		''' </exception>
		Public Function GetFolderId(ByVal folderPath As String) As Int32 Implements IFolderCache.GetFolderId
		    Dim newFolderPath As New System.Text.StringBuilder
		    For Each folder As String In folderPath.Split(PathSeparatorChar)
			    If Not folder.Trim = "" Then
				    newFolderPath.Append(PathSeparator & folder.Trim)
			    End If
		    Next
		    folderPath = newFolderPath.ToString
		    If folderPath = "" Then
			    folderPath = PathSeparator
		    End If

		    If _dictionary.ContainsKey(folderPath) Then
			    Return DirectCast(_dictionary(folderPath), FolderCacheItem).FolderID
		    Else
				Try
				    Dim newFolder As FolderCacheItem = Me.GetNewFolder(folderPath)
				    If Not _dictionary.ContainsKey(folderPath) Then
					    _dictionary(folderPath) = newFolder
				    End If
				    Return newFolder.FolderID
				Catch ex As Exception
					If kCura.WinEDDS.Helpers.ExceptionHelper.IsFatalException(ex) Then
						Throw
					End If

					_logger.LogFatal(ex, "Failed to create the {FolderPath} folder path for workspace {WorkspaceId}.", folderPath, _workspaceId)
					Dim message As String = $"A fatal error occurred creating the '{folderPath}' folder path for workspace {_workspaceId}."
					message = kCura.WinEDDS.Helpers.ExceptionHelper.AppendTryAgainAdminFatalMessage(message)
					Throw New kCura.WinEDDS.Exceptions.WebApiException(message, ex)
				End Try
		    End If
		End Function

		Private Function GetNewFolder(ByVal folderPath As String) As FolderCacheItem
			Dim pathToDestination As New List(Of String)
			Dim s As String
			If folderPath = "" OrElse folderPath = PathSeparator Then
				s = PathSeparator
			Else
				s = folderPath.Substring(0, folderPath.LastIndexOf(PathSeparator))
			End If
			pathToDestination.Add(folderPath.Substring(folderPath.LastIndexOf(PathSeparator) + 1))
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
				Dim newFolderID As Int32 = _hierarchicArtifactManager.Read(_workspaceId, parentFolder.FolderID, newFolderName)
				If FolderNotFoundId = newFolderID Then
					newFolderID = _hierarchicArtifactManager.Create(_workspaceId, parentFolder.FolderID, newFolderName)
				End If

				Dim parentFolderPath As String
				If parentFolder.Path = PathSeparator Then
					parentFolderPath = ""
				Else
					parentFolderPath = parentFolder.Path
				End If
				Dim newFolder As New FolderCacheItem(parentFolderPath & PathSeparator & newFolderName, newFolderID)
				_dictionary(newFolder.Path) = newFolder
				Return CreateFolders(newFolder, folderNames)
			Else
				Return parentFolder
			End If
		End Function

		Private Function FindParentFolder(ByVal folderPath As String, ByVal pathToDestination As List(Of String)) As FolderCacheItem
			If _dictionary.ContainsKey(folderPath) Then
				Return DirectCast(_dictionary(folderPath), FolderCacheItem)
			Else
				Dim pathEntry As String = folderPath.Substring(folderPath.LastIndexOf(PathSeparator) + 1)
				If pathEntry = "" Then
					Return _rootFolder
				End If
				pathToDestination.Insert(0, pathEntry)
				Return FindParentFolder(folderPath.Substring(0, folderPath.LastIndexOf(PathSeparator)), pathToDestination)
			End If
		End Function

		Private Sub RecursivelyPopulate(ByVal dataRow As System.Data.DataRow, ByVal parent As FolderCacheItem)
			Dim childDataRow As System.Data.DataRow
			Dim newPath As String
			For Each childDataRow In dataRow.GetChildRows("NodeRelation")
				If parent.Path = PathSeparator Then
					newPath = PathSeparator & childDataRow("Name").ToString.Trim
				Else
					newPath = parent.Path & PathSeparator & childDataRow("Name").ToString.Trim
				End If
				Dim childFolder As New FolderCacheItem(newPath.Trim, CType(childDataRow("ArtifactID"), Int32))
				If Not _dictionary.ContainsKey(childFolder.Path.Trim) Then
					_dictionary(childFolder.Path.Trim) = childFolder
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