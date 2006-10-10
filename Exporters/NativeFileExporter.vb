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
				Throw
			End Try
		End Sub

		Private Sub Export()
			Dim nativesTable As System.Data.DataTable
			Dim folderTable As System.Data.DataTable
			Dim writer As System.IO.StreamWriter
			Dim volumeFile As String

			Dim i As Int32 = 0
			Dim docRow As System.Data.DataRow
			Dim artifactIDs As New ArrayList
			Dim docRows As New ArrayList
			Dim fileName As String = String.Empty
			Dim valuesArray As String()
			
			'Me.WriteUpdate("Retrieving export data from the server...")

			nativesTable = _fileManager.RetrieveNativesForProductionExport(Me.ExportFile.ArtifactID).Tables(0)
			folderTable = _folderManager.RetrieveAllByCaseID(Me.ExportFile.ArtifactID).Tables(0)

			Me.FolderList = New kCura.WinEDDS.FolderList(folderTable)
			Me.FolderList.CreateFolders(Me.NativesDirectory)

			If Not System.IO.Directory.Exists(Me.NativesDirectory & Me.FolderList.BaseFolder.Path) Then
				System.IO.Directory.CreateDirectory(Me.NativesDirectory & Me.FolderList.BaseFolder.Path)
			End If

			volumeFile = String.Format("{0}{1}_NATIVES.log", Me.ExportFile.FolderPath, _name)

			If System.IO.File.Exists(volumeFile) Then
				'Me.WriteWarning(String.Format("Search log file '{0}' already exists, overwriting file.", volumeFile))
				System.IO.File.Delete(volumeFile)
			End If
			writer = System.IO.File.CreateText(volumeFile)
			'Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, "Created search log file.")
			Me.TotalDocuments = nativesTable.Rows.Count()
			'Me.WriteUpdate("Data retrieved. Beginning search export...")

			valuesArray = New String() {"BEGBATES", "ENDBATES", "FILE_PATH"}
			writer.Write(LogFileEntry(valuesArray))

			For Each docRow In nativesTable.Rows
				Dim bates As String = CType(docRow("TextIdentifier"), String)
				fileName = String.Format("{0}{1}{2}", Me.NativesDirectory, Me.FolderList.ItemByArtifactID(CType(docRow("ParentArtifactID"), Int32)).Path, CType(docRow("Filename"), String))
				Me.ExportNativeFile(fileName, CType(docRow("Guid"), String), CType(docRow("DocumentArtifactID"), Int32), fileName)
				valuesArray = New String() {bates, bates, fileName}
				writer.Write(Me.LogFileEntry(valuesArray))
				Me.DocumentsExported += 1
				'Me.WriteUpdate("Exported document " & i + 1)
				If _halt Then Exit Sub
			Next
			writer.Close()
			Me.FolderList.DeleteEmptyFolders(Me.NativesDirectory)
		End Sub

		Private Function LogFileEntry(ByVal values As String()) As String
			Dim retString As New System.Text.StringBuilder
			Dim i As Int32

			For i = 0 To values.Length - 1
				retString.AppendFormat("{0}{1}{0}", Me.ExportFile.QuoteDelimiter, values(i))
				If i = values.Length - 1 Then
					retString.Append(System.Environment.NewLine)
				Else
					retString.Append(Me.ExportFile.RecordDelimiter)
				End If
			Next
			Return retString.ToString
		End Function

		Private Sub ExportNativeFile(ByVal exportFileName As String, ByVal fileGuid As String, ByVal artifactID As Int32, ByVal systemFileName As String)		'ByVal fileURI As String, ByVal systemFileName As String)
			If System.IO.File.Exists(exportFileName) Then
				If Me.ExportFile.Overwrite Then
					System.IO.File.Delete(exportFileName)
					'Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Overwriting document {0}.", systemFileName))
					_downloadHandler.DownloadFile(exportFileName, fileGuid, artifactID)
				Else
					'Me.WriteWarning(String.Format("{0} already exists. Skipping file export.", systemFileName))
				End If
			Else
				'Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Now exporting document {0}.", systemFileName))
				_downloadHandler.DownloadFile(exportFileName, fileGuid, artifactID)
			End If
			'Me.WriteUpdate(String.Format("Finished exporting document {0}.", systemFileName))
		End Sub

		Private Function CreateSubdirectory(ByVal subdirectoryPrefix As String, ByVal subdirectoryNumber As Integer, ByVal parentVolume As String) As String
			Dim subDirectory As String
			subDirectory = String.Format("{0}{1}", subdirectoryPrefix, ZeroFillNumber(subdirectoryNumber, 3))
			If Not System.IO.Directory.Exists(String.Format("{0}{1}", parentVolume, subDirectory)) Then
				System.IO.Directory.CreateDirectory(String.Format("{0}{1}", parentVolume, subDirectory))
				'Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Created subdirectory {0}.", subDirectory))
			End If
			Return subDirectory
		End Function

		Private Function ZeroFillNumber(ByVal value As Int32, ByVal length As Int32) As String
			Dim lengthOfValue As Int32 = CType(System.Math.Floor(System.Math.Log10(CType(value, Double))), Int32) + 1
			Dim retVal As New System.Text.StringBuilder
			retVal.Append("0"c, length - lengthOfValue)
			Return retVal.ToString & value.ToString
		End Function
	End Class
End Namespace