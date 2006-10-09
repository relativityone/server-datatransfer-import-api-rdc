Imports System.IO
Namespace kCura.WinEDDS
	Public Class NativeFileExporter
		Private _fileManager As kCura.WinEDDS.Service.FileManager
		Private _folderManager As kCura.WinEDDS.Service.FolderManager
		Private _fullTextDownloader As kCura.WinEDDS.FullTextManager
		Private _documentManager As kCura.WinEDDS.Service.DocumentManager
		Private _exportFile As kCura.WinEDDS.ExportFile
		Private _columns As System.Collections.ArrayList
		Private _sourceDirectory As String
		Private _downloadHandler As FileDownloader
		Private _halt As Boolean
		Private _overwrite As Boolean
		Private _nativesDirectory As String
		Private _name As String
		Public DocumentsExported As Int32
		Public TotalDocuments As Int32
		Public FolderList As kCura.WinEDDS.FolderList
		Private WithEvents _processController As kCura.Windows.Process.Controller
		
#Region "Public Properties"
		Public Property ExportFile() As kCura.WinEDDS.ExportFile
			Get
				Return _exportFile
			End Get
			Set(ByVal value As kCura.WinEDDS.ExportFile)
				_exportFile = value
			End Set
		End Property

		Public Property NativesDirectory() As String
			Get
				Return _nativesDirectory
			End Get
			Set(ByVal value As String)
				_nativesDirectory = value
			End Set
		End Property
#End Region

#Region "Public Events"
		Public Event FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception)
		Public Event StatusMessage(ByVal exportArgs As ExportEventArgs)
		Public Event DisableCloseButton()
		Public Event EnableCloseButton()
#End Region

#Region "Messaging"
		Private Sub WriteFatalError(ByVal line As String, ByVal ex As System.Exception)
			RaiseEvent FatalErrorEvent(line, ex)
		End Sub

		Private Sub WriteStatusLine(ByVal e As kCura.Windows.Process.EventType, ByVal line As String)
			RaiseEvent StatusMessage(New ExportEventArgs(Me.DocumentsExported, Me.TotalDocuments, line, e))
		End Sub

		Private Sub WriteError(ByVal line As String)
			WriteStatusLine(kCura.Windows.Process.EventType.Error, line)
		End Sub

		Private Sub WriteWarning(ByVal line As String)
			WriteStatusLine(kCura.Windows.Process.EventType.Warning, line)
		End Sub

		Private Sub WriteUpdate(ByVal line As String)
			WriteStatusLine(kCura.Windows.Process.EventType.Progress, line)
		End Sub
#End Region

		Public Sub New(ByVal exportFile As kCura.WinEDDS.ExportFile, ByVal processController As kCura.Windows.Process.Controller, ByVal volumeStartNumber As Int32, Optional ByVal name As String = "")
			_folderManager = New kCura.WinEDDS.Service.FolderManager(exportFile.Credential, exportFile.CookieContainer, exportFile.Identity)
			_documentManager = New kCura.WinEDDS.Service.DocumentManager(exportFile.Credential, exportFile.CookieContainer, exportFile.Identity)
			_downloadHandler = New FileDownloader(exportFile.Credential, exportFile.CaseInfo.DocumentPath & "\EDDS" & exportFile.CaseInfo.ArtifactID, exportFile.CaseInfo.DownloadHandlerURL, exportFile.CookieContainer)
			_fileManager = New kCura.WinEDDS.Service.FileManager(exportFile.Credential, exportFile.CookieContainer, exportFile.Identity)
			_sourceDirectory = _documentManager.GetDocumentDirectoryByContextArtifactID(exportFile.ArtifactID)
			_fullTextDownloader = New kCura.WinEDDS.FullTextManager(exportFile.Credential, _sourceDirectory, exportFile.CookieContainer)
			_halt = False
			_processController = processController
			Me.DocumentsExported = 0
			Me.TotalDocuments = 1
			Me.ExportFile = exportFile
			Me.ExportFile.FolderPath = Me.ExportFile.FolderPath + "\"
			Me.NativesDirectory = Me.ExportFile.FolderPath & Me.CreateSubdirectory("NATIVES", volumeStartNumber, Me.ExportFile.FolderPath) & "\"
			_name = name
		End Sub

		Public Sub ExportNatives()
			Try
				Me.Export()
			Catch ex As System.Exception
				Me.WriteFatalError(String.Format("A fatal error occurred on document #{0}", Me.DocumentsExported), ex)
			End Try
		End Sub

		Private Sub Export()
			Dim nativesTable As System.Data.DataTable
			Dim folderTable As System.Data.DataTable
			Dim writer As System.IO.StreamWriter
			Dim volumeFile As String

			Me.WriteUpdate("Retrieving export data from the server...")

			nativesTable = _fileManager.RetrieveNativesForProductionExport(Me.ExportFile.ArtifactID).Tables(0)
			folderTable = _folderManager.RetrieveAllByCaseID(Me.ExportFile.ArtifactID).Tables(0)

			Me.FolderList = New kCura.WinEDDS.FolderList(folderTable)
			Me.FolderList.CreateFolders(Me.NativesDirectory)

			If Not System.IO.Directory.Exists(Me.NativesDirectory & Me.FolderList.BaseFolder.Path) Then
				System.IO.Directory.CreateDirectory(Me.NativesDirectory & Me.FolderList.BaseFolder.Path)
			End If

			volumeFile = String.Format("{0}{1}_NATIVES.log", Me.ExportFile.FolderPath, _name)

			If System.IO.File.Exists(volumeFile) Then
				Me.WriteWarning(String.Format("Search log file '{0}' already exists, overwriting file.", volumeFile))
				System.IO.File.Delete(volumeFile)
			End If
			writer = System.IO.File.CreateText(volumeFile)
			Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, "Created search log file.")
			Me.TotalDocuments = nativesTable.Rows.Count()
			Me.WriteUpdate("Data retrieved. Beginning search export...")

			Dim i As Int32 = 0
			Dim docRow As System.Data.DataRow
			Dim artifactIDs As New ArrayList
			Dim docRows As New ArrayList
			Dim fileName As String = String.Empty
			For Each docRow In nativesTable.Rows

				'BEGIN SINGLE EXPORT
				fileName = String.Format("{0}{1}{2}", Me.NativesDirectory, Me.FolderList.ItemByArtifactID(CType(docRow("ParentArtifactID"), Int32)).Path, CType(docRow("Filename"), String))
				Me.ExportNativeFile(fileName, CType(docRow("Guid"), String), CType(docRow("DocumentArtifactID"), Int32), fileName)
				'writer.Write(Me.LogFileEntry(docRows(i), fileName, fullTextFileGuid))
				Me.DocumentsExported += 1
				Me.WriteUpdate("Exported document " & i + 1)
				If _halt Then Exit Sub
				'END SINGLE EXPORT

				'ORIGINAL
				'If Not _halt Then
				'	i += 1
				'	artifactIDs.Add(CType(docRow("DocumentArtifactID"), Int32))
				'	docRows.Add(docRow)
				'	If i Mod Config.SearchExportChunkSize = 0 Then
				'		ExportChunk(DirectCast(artifactIDs.ToArray(GetType(Int32)), Int32()), DirectCast(docRows.ToArray(GetType(System.Data.DataRow)), System.Data.DataRow()), writer, nativesTable)
				'		artifactIDs.Clear()
				'		docRows.Clear()
				'	End If
				'End If
			Next
			'ORIGINAL
			'If artifactIDs.Count > 0 Then
			'	ExportChunk(DirectCast(artifactIDs.ToArray(GetType(Int32)), Int32()), DirectCast(docRows.ToArray(GetType(System.Data.DataRow)), System.Data.DataRow()), writer, nativesTable)
			'End If
			writer.Close()
			Me.FolderList.DeleteEmptyFolders(Me.NativesDirectory)
		End Sub

		Private Sub ExportChunk(ByVal documentArtifactIDs As Int32(), ByVal docRows As System.Data.DataRow(), ByVal writer As System.io.StreamWriter, ByVal nativesTable As System.Data.DataTable)
			Dim natives As New System.Data.DataView
			Dim i As Int32 = 0
			natives.Table = nativesTable
			For i = 0 To documentArtifactIDs.Length - 1
				Dim fileName As String = String.Empty
				Dim nativeRow As System.Data.DataRowView = GetNativeRow(natives, documentArtifactIDs(i))
				If Not nativeRow Is Nothing Then
					fileName = String.Format("{0}{1}{2}", Me.NativesDirectory, Me.FolderList.ItemByArtifactID(CType(docRows(i)("ParentArtifactID"), Int32)).Path, CType(nativeRow("Filename"), String))
					Me.ExportNativeFile(fileName, CType(nativeRow("Guid"), String), CType(docRows(i)("DocumentArtifactID"), Int32), fileName)
				End If
				'writer.Write(Me.LogFileEntry(docRows(i), fileName, fullTextFileGuid))
				Me.DocumentsExported += 1
				Me.WriteUpdate("Exported document " & i + 1)
				If _halt Then Exit Sub
			Next
		End Sub

		Private Sub ExportSingleNative()
			
		End Sub

		Private Function GetNativeRow(ByVal dv As System.Data.DataView, ByVal artifactID As Int32) As System.Data.DataRowView
			dv.RowFilter = "DocumentArtifactID = " & artifactID.ToString
			If dv.Count > 0 Then
				Return dv(0)
			Else
				Return Nothing
			End If
		End Function

		Private Sub ExportNativeFile(ByVal exportFileName As String, ByVal fileGuid As String, ByVal artifactID As Int32, ByVal systemFileName As String)		'ByVal fileURI As String, ByVal systemFileName As String)
			If System.IO.File.Exists(exportFileName) Then
				If Me.ExportFile.Overwrite Then
					System.IO.File.Delete(exportFileName)
					Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Overwriting document {0}.", systemFileName))
					_downloadHandler.DownloadFile(exportFileName, fileGuid, artifactID)
				Else
					Me.WriteWarning(String.Format("{0} already exists. Skipping file export.", systemFileName))
				End If
			Else
				Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Now exporting document {0}.", systemFileName))
				_downloadHandler.DownloadFile(exportFileName, fileGuid, artifactID)
			End If
			Me.WriteUpdate(String.Format("Finished exporting document {0}.", systemFileName))
		End Sub

		Private Function CreateSubdirectory(ByVal subdirectoryPrefix As String, ByVal subdirectoryNumber As Integer, ByVal parentVolume As String) As String
			Dim subDirectory As String
			subDirectory = String.Format("{0}{1}", subdirectoryPrefix, ZeroFillNumber(subdirectoryNumber, 3))
			If Not System.IO.Directory.Exists(String.Format("{0}{1}", parentVolume, subDirectory)) Then
				System.IO.Directory.CreateDirectory(String.Format("{0}{1}", parentVolume, subDirectory))
				Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Created subdirectory {0}.", subDirectory))
			End If
			Return subDirectory
		End Function

		Private Function ZeroFillNumber(ByVal value As Int32, ByVal length As Int32) As String
			Dim lengthOfValue As Int32 = CType(System.Math.Floor(System.Math.Log10(CType(value, Double))), Int32) + 1
			Dim retVal As New System.Text.StringBuilder
			retVal.Append("0"c, length - lengthOfValue)
			Return retVal.ToString & value.ToString
		End Function

		'Private Sub ExportNatives(ByVal production As kCura.EDDS.WebAPI.ProductionManagerBase.Production, ByVal currentVolume As String)
		'			Dim nativesTable As System.Data.DataTable
		'			Dim fileTable As System.Data.DataTable
		'			Dim folderTable As System.Data.DataTable
		'			Dim fullTextFiles As System.Data.DataTable
		'			Dim fileURI As String
		'			Dim fileName As String
		'			Dim documentSpot As Int32
		'			Dim count As Int32
		'			Dim writer As System.IO.StreamWriter
		'			Dim volumeFile As String
		'			Dim nativesDirectory, nativesDirectoryPath As String
		'			Dim folderList As kCura.WinEDDS.FolderList
		'			Dim volumeFileName As String

		'			Me.WriteUpdate("Exporting native files...")
		'			nativesTable = _fileManager.RetrieveNativesForProductionExport(Me.ProductionArtifactID).Tables(0)
		'			folderTable = _folderManager.RetrieveAllByCaseID(production.ContainerID.Value).Tables(0)
		'			_folderList = New kCura.WinEDDS.FolderList(folderTable)

		'			nativesDirectory = Me.CreateSubdirectory("NATIVES", production.VolumeStartNumber, currentVolume)
		'			nativesDirectoryPath = String.Format("{0}{1}\{2}\", Me.FolderPath, currentVolume, nativesDirectory)

		'			_folderList.CreateFolders(nativesDirectoryPath)

		'			_sourceDirectory = _documentManager.GetDocumentDirectoryByContextArtifactID(production.ArtifactID)

		'			volumeFileName = production.Name & "_" & currentVolume & "_NATIVES"
		'			volumeFile = String.Format("{0}{1}\{2}.log", Me.FolderPath, currentVolume, volumeFileName)
		'			If System.IO.File.Exists(volumeFile) Then
		'				Me.WriteWarning(String.Format("Natives log file '{0}' already exists, overwriting file.", volumeFile))
		'				System.IO.File.Delete(volumeFile)
		'			End If
		'			writer = System.IO.File.CreateText(volumeFile)
		'			Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, "Created Natives log file.")
		'			Me.TotalDocuments = nativesTable.Rows.Count()
		'			writer.Write(Me.LoadAndWriteColumns())
		'			Me.WriteUpdate("Data retrieved. Beginning search export...")

		'			Dim i As Int32 = 0
		'			Dim docRow As System.Data.DataRow
		'			Dim artifactIDs As New ArrayList
		'			Dim docRows As New ArrayList
		'			For Each docRow In nativesTable.Rows
		'				i += 1
		'				artifactIDs.Add(CType(docRow("ArtifactID"), Int32))
		'				docRows.Add(docRow)
		'				If i Mod Config.SearchExportChunkSize = 0 Then
		'					ExportChunk(DirectCast(artifactIDs.ToArray(GetType(Int32)), Int32()), DirectCast(docRows.ToArray(GetType(System.Data.DataRow)), System.Data.DataRow()), writer, nativesTable)
		'					artifactIDs.Clear()
		'					docRows.Clear()
		'				End If
		'			Next
		'			If artifactIDs.Count > 0 Then ExportChunk(DirectCast(artifactIDs.ToArray(GetType(Int32)), Int32()), DirectCast(docRows.ToArray(GetType(System.Data.DataRow)), System.Data.DataRow()), writer, nativesTable)
		'			writer.Close()
		'			_folderList.DeleteEmptyFolders(nativesDirectoryPath)

		'		End Sub

		'		Private Sub ExportNative(ByVal exportFileName As String, ByVal fileGuid As String, ByVal artifactID As Int32, ByVal systemFileName As String)		'ByVal fileURI As String, ByVal systemFileName As String)
		'			If System.IO.File.Exists(exportFileName) Then
		'				If _overwrite Then
		'					System.IO.File.Delete(exportFileName)
		'					Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Overwriting document {0}.", systemFileName))
		'					_downloadHandler.DownloadFile(exportFileName, fileGuid, artifactID)
		'					Me.WebClient.DownloadFile(fileURI, exportFileName)
		'				Else
		'					Me.WriteWarning(String.Format("{0} already exists. Skipping file export.", systemFileName))
		'				End If
		'			Else
		'				Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Now exporting document {0}.", systemFileName))
		'				_downloadHandler.DownloadFile(exportFileName, fileGuid, artifactID)
		'				Me.WebClient.DownloadFile(fileURI, exportFileName)
		'			End If
		'			Me.WriteUpdate(String.Format("Finished exporting document {0}.", systemFileName))
		'		End Sub

		'		Private Sub ExportChunk(ByVal documentArtifactIDs As Int32(), ByVal docRows As System.Data.DataRow(), ByVal writer As System.io.StreamWriter, ByVal nativesTable As System.Data.DataTable)
		'			Dim natives As New System.Data.DataView(nativesTable)
		'			Dim fullTexts As New System.Data.DataView
		'			Dim i As Int32 = 0
		'			natives.Table = _searchManager.RetrieveNativesForSearch(Me.ExportFile.ArtifactID, kCura.Utility.Array.IntArrayToCSV(documentArtifactIDs)).Tables(0)
		'			For i = 0 To documentArtifactIDs.Length - 1
		'				Dim fullTextFileGuid As String = GetFullTextFileGuid(fullTexts.Table, documentArtifactIDs(i))
		'				Dim fileName As String = String.Empty
		'				Dim nativeRow As System.Data.DataRowView = GetNativeRow(natives, documentArtifactIDs(i))
		'				If Not nativeRow Is Nothing Then
		'					fileName = String.Format("{0}{1}{2}", Me.FolderPath, _folderList.ItemByArtifactID(CType(docRows(i)("ParentArtifactID"), Int32)).Path, CType(nativeRow("Filename"), String))
		'					Dim fileURI As String = String.Format("{0}Download.aspx?ArtifactID={1}&GUID={2}", "http://localhost/EDDS.Distributed", CType(docRows(i)("ArtifactID"), Int32), CType(nativeRow("Guid"), String))
		'					Dim fileURI As String = String.Format("{0}Download.aspx?ArtifactID={1}&GUID={2}", Me.ExportFile.CaseInfo.DownloadHandlerURL, CType(docRows(i)("ArtifactID"), Int32), CType(nativeRow("Guid"), String))
		'					Me.ExportNative(fileName, CType(nativeRow("Guid"), String), CType(docRows(i)("ArtifactID"), Int32), fileName)
		'					'Me.ExportNative(fileName, fileURI, fileName)
		'				End If
		'				writer.Write(Me.LogFileEntry(docRows(i), fileName, fullTextFileGuid))
		'				Me.DocumentsExported += 1
		'				Me.WriteUpdate("Exported document " & i + 1)
		'				If _halt Then Exit Sub
		'			Next
		'		End Sub

		'		Private Sub ExportFile(ByVal fileName As String, ByVal fileGuid As String, ByVal artifactID As Int32, ByVal batesNumber As String)		'ByVal fileURI As String, ByVal batesNumber As String)
		'			If System.IO.File.Exists(fileName) Then
		'				If Me.Overwrite Then
		'					System.IO.File.Delete(fileName)
		'					Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Overwriting document {0}.tif.", batesNumber))
		'					_downloadManager.DownloadFile(fileName, fileGuid, artifactID)
		'					Me.WebClient.DownloadFile(fileURI, fileName)
		'				Else
		'					Me.WriteWarning(String.Format("{0}.tif already exists. Skipping file export.", batesNumber))
		'				End If
		'			Else
		'				Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Now exporting document {0}.tif.", batesNumber))
		'				_downloadManager.DownloadFile(fileName, fileGuid, artifactID)
		'				Me.WebClient.DownloadFile(fileURI, fileName)
		'			End If
		'			Me.DocumentsExported += 1
		'			Me.WriteUpdate(String.Format("Finished exporting document {0}.tif.", batesNumber))
		'		End Sub
	End Class
End Namespace