Imports System.IO
Namespace kCura.WinEDDS
	Public Class SearchExporter
		Private _searchManager As kCura.WinEDDS.Service.SearchManager
		Private _folderManager As kCura.WinEDDS.Service.FolderManager
		'Private _webClient As System.Net.WebClient
		Private _exportFile As kCura.WinEDDS.ExportFile
		Private _columns As System.Collections.ArrayList
		Private _sourceDirectory As String
		Private _documentManager As kCura.WinEDDS.Service.DocumentManager
		Public DocumentsExported As Int32
		Public TotalDocuments As Int32
		Public FolderList As kCura.WinEDDS.FolderList
		Private _fullTextDownloader As kCura.WinEDDS.FullTextManager
		Private WithEvents _processController As kCura.Windows.Process.Controller
		Private _downloadHandler As FileDownloader
		Private _halt As Boolean

#Region "Public Properties"
		Public Property ExportFile() As kCura.WinEDDS.ExportFile
			Get
				Return _exportFile
			End Get
			Set(ByVal value As kCura.WinEDDS.ExportFile)
				_exportFile = value
			End Set
		End Property

		'Public Property WebClient() As System.Net.WebClient
		'	Get
		'		Return _webClient
		'	End Get
		'	Set(ByVal value As System.Net.WebClient)
		'		_webClient = value
		'	End Set
		'End Property

		Public Property Columns() As System.Collections.ArrayList
			Get
				Return _columns
			End Get
			Set(ByVal value As System.Collections.ArrayList)
				_columns = value
			End Set
		End Property
#End Region

#Region "Public Events"
		Public Event FatalErrorEvent(ByVal message As String, ByVal ex As Exception)
		Public Event StatusMessage(ByVal exportArgs As ExportEventArgs)
		Public Event DisableCloseButton()
		Public Event EnableCloseButton()
#End Region

#Region "Messaging"
		Private Sub WriteFatalError(ByVal line As String, ByVal ex As Exception)
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

		Public Sub New(ByVal exportFile As kCura.WinEDDS.ExportFile, ByVal processController As kCura.Windows.Process.Controller)
			_searchManager = New kCura.WinEDDS.Service.SearchManager(exportFile.Credential)
			_folderManager = New kCura.WinEDDS.Service.FolderManager(exportFile.Credential)
			_documentManager = New kCura.WinEDDS.Service.DocumentManager(exportFile.Credential)
			'_webClient = New System.Net.WebClient
			'_webClient.Credentials = exportFile.Credential
			_downloadHandler = New FileDownloader(exportFile.Credential, exportFile.CaseInfo.DocumentPath & "\EDDS" & exportFile.CaseInfo.ArtifactID, exportFile.CaseInfo.DownloadHandlerURL)
			_halt = False
			_processController = processController
			Me.DocumentsExported = 0
			Me.TotalDocuments = 1
			Me.ExportFile = exportFile
			Me.ExportFile.FolderPath = Me.ExportFile.FolderPath + "\"
		End Sub

		Public Sub ExportSearch()
			Try
				Me.Search()
			Catch ex As Exception
				Me.WriteFatalError(String.Format("A fatal error occurred on document #{0}", Me.DocumentsExported), ex)
			End Try
		End Sub

		Private Sub Search()
			Dim documentTable As System.Data.DataTable
			Dim fileTable As System.Data.DataTable
			Dim folderTable As System.Data.DataTable
			Dim fullTextFiles As System.Data.DataTable
			Dim fileURI As String
			Dim fileName As String
			Dim fullTextFileGuid As String
			Dim documentSpot, currentDocID As Int32
			Dim count As Int32
			Dim writer As System.IO.StreamWriter
			Dim volumeFile As String

			


			Me.WriteUpdate("Retrieving export data from the server...")
			Select Case Me.ExportFile.TypeOfExport
				Case ExportFile.ExportType.ArtifactSearch
					documentTable = _searchManager.SearchBySearchArtifactID(Me.ExportFile.ArtifactID).Tables(0)
				Case ExportFile.ExportType.ParentSearch
					documentTable = _searchManager.SearchByParentArtifactID(Me.ExportFile.ArtifactID, False).Tables(0)
				Case ExportFile.ExportType.AncestorSearch
					documentTable = _searchManager.SearchByParentArtifactID(Me.ExportFile.ArtifactID, True).Tables(0)
			End Select
			folderTable = _folderManager.RetrieveAllByCaseID(Me.ExportFile.ArtifactID).Tables(0)
			Me.FolderList = New kCura.WinEDDS.FolderList(folderTable)
			Me.FolderList.CreateFolders(Me.ExportFile.FolderPath)
			'If Me.ExportFile.ExportFullText Then fullTextFiles = _searchManager.RetrieveFullTextFilesForSearch(Me.ExportFile.ArtifactID, GetDocumentsString(documentTable)).Tables(0)
			_sourceDirectory = _documentManager.GetDocumentDirectoryByContextArtifactID(Me.ExportFile.ArtifactID)
			_fullTextDownloader = New kCura.WinEDDS.FullTextManager(Me.ExportFile.Credential, _sourceDirectory)
			If Not System.IO.Directory.Exists(Me.ExportFile.FolderPath & Me.FolderList.BaseFolder.Path) Then
				System.IO.Directory.CreateDirectory(Me.ExportFile.FolderPath & Me.FolderList.BaseFolder.Path)
			End If
			volumeFile = String.Format("{0}{1}export.log", Me.ExportFile.FolderPath, Me.FolderList.BaseFolder.Path)
			If System.IO.File.Exists(volumeFile) Then
				Me.WriteWarning(String.Format("Search log file '{0}' already exists, overwriting file.", volumeFile))
				System.IO.File.Delete(volumeFile)
			End If
			writer = System.IO.File.CreateText(volumeFile)
			Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, "Created search log file.")
			Me.TotalDocuments = documentTable.Rows.Count()
			writer.Write(Me.LoadAndWriteColumns())
			Me.WriteUpdate("Data retrieved. Beginning search export...")

			Dim i As Int32 = 0
			Dim docRow As System.Data.DataRow
			Dim docIDs As New ArrayList
			Dim docRows As New ArrayList
			For Each docRow In documentTable.Rows
				If Not _halt Then
					i += 1
					docIDs.Add(CType(docRow("DocumentID"), Int32))
					docRows.Add(docRow)
					If i Mod Config.SearchExportChunkSize = 0 Then
						ExportChunk(DirectCast(docIDs.ToArray(GetType(Int32)), Int32()), DirectCast(docRows.ToArray(GetType(System.Data.DataRow)), System.Data.DataRow()), writer)
						docIDs.Clear()
						docRows.Clear()
					End If
				End If
			Next
			If docIDs.Count > 0 Then ExportChunk(DirectCast(docIDs.ToArray(GetType(Int32)), Int32()), DirectCast(docRows.ToArray(GetType(System.Data.DataRow)), System.Data.DataRow()), writer)
			writer.Close()
			'Me.TotalDocuments = fileTable.Rows.Count()
			'logFile.Append(Me.LoadAndWriteColumns())
			'Me.WriteUpdate("Data retrieved. Beginning search export...")

			'Me.FolderList = New kCura.WinEDDS.FolderList(folderTable)
			'Me.FolderList.CreateFolders(Me.ExportFile.FolderPath)
			'For count = 0 To fileTable.Rows.Count - 1
			'	currentDocID = CType(fileTable.Rows(count)("DocumentID"), Int32)
			'	For documentSpot = 0 To documentTable.Rows.Count - 1
			'		If CType(documentTable.Rows(documentSpot)("DocumentID"), Int32) = currentDocID Then
			'			Exit For
			'		End If
			'	Next
			'	fullTextFileGuid = GetFullTextFileGuid(fullTextFiles, currentDocID)
			'	fileName = String.Format("{0}{1}{2}", Me.ExportFile.FolderPath, Me.FolderList.ItemByArtifactID(CType(documentTable.Rows(documentSpot)("ParentArtifactID"), Int32)).Path, CType(fileTable.Rows(count)("Filename"), String))
			'	fileURI = String.Format("{0}Download.aspx?ArtifactID={1}&GUID={2}", Me.ExportFile.CaseInfo.DownloadHandlerURL, CType(documentTable.Rows(documentSpot)("ArtifactID"), Int32), CType(fileTable.Rows(count)("Guid"), String))
			'	Me.ExportNative(fileName, fileURI, fileName)
			'	logFile.Append(Me.LogFileEntry(documentTable.Rows(documentSpot), fileName, fullTextFileGuid))
			'Next
			'CreateVolumeLogFile(logFile.ToString)
		End Sub

#Region "Private Helper Functions"

		Private Sub ExportChunk(ByVal docIDs As Int32(), ByVal docRows As System.Data.DataRow(), ByVal writer As System.io.StreamWriter)
			Dim natives As New System.Data.DataView
			Dim fullTexts As New System.Data.DataView
			Dim i As Int32 = 0
			If Me.ExportFile.ExportNative Then natives.Table = _searchManager.RetrieveNativesForSearch(Me.ExportFile.ArtifactID, kCura.Utility.Array.IntArrayToCSV(docIDs)).Tables(0)
			If Me.ExportFile.ExportFullText Then fullTexts.Table = _searchManager.RetrieveFullTextFilesForSearch(Me.ExportFile.ArtifactID, kCura.Utility.Array.IntArrayToCSV(docIDs)).Tables(0)
			For i = 0 To docIDs.Length - 1
				Dim fullTextFileGuid As String = GetFullTextFileGuid(fullTexts.Table, docIDs(i))
				Dim fileName As String = String.Empty
				Dim nativeRow As System.Data.DataRowView = GetNativeRow(natives, docIDs(i))
				If Me.ExportFile.ExportNative AndAlso Not nativeRow Is Nothing Then
					fileName = String.Format("{0}{1}{2}", Me.ExportFile.FolderPath, Me.FolderList.ItemByArtifactID(CType(docRows(i)("ParentArtifactID"), Int32)).Path, CType(nativeRow("Filename"), String))
					'Dim fileURI As String = String.Format("{0}Download.aspx?ArtifactID={1}&GUID={2}", Me.ExportFile.CaseInfo.DownloadHandlerURL, CType(docRows(i)("ArtifactID"), Int32), CType(nativeRow("Guid"), String))
					Me.ExportNative(fileName, CType(nativeRow("Guid"), String), CType(docRows(i)("ArtifactID"), Int32), fileName)
					'Me.ExportNative(fileName, fileURI, fileName)
				End If
				writer.Write(Me.LogFileEntry(docRows(i), fileName, fullTextFileGuid))
				Me.DocumentsExported += 1
				Me.WriteUpdate("Exported document " & i + 1)
				If _halt Then Exit Sub
			Next
		End Sub

		Private Function GetFullTextFileGuid(ByVal dt As System.Data.DataTable, ByVal docID As Int32) As String
			Dim row As System.Data.DataRow
			If Me.ExportFile.ExportFullText Then
				For Each row In dt.Rows
					If CType(row("DocumentID"), Int32) = docID Then
						Return CType(row("Guid"), String)
					End If
				Next
			Else
				Return String.Empty
			End If
		End Function

		Private Function GetNativeRow(ByVal dv As System.Data.DataView, ByVal docID As Int32) As System.Data.DataRowView
			If Not Me.ExportFile.ExportNative Then Return Nothing
			dv.RowFilter = "DocumentID = " & docID.ToString
			If dv.Count > 0 Then
				Return dv(0)
			Else
				Return Nothing
			End If
		End Function

		Private Function LoadAndWriteColumns() As String
			Dim count As Int32
			Dim columnName As String
			Dim table As System.Data.DataTable
			Dim retString As New System.Text.StringBuilder

			_columns = New System.Collections.ArrayList
			If Me.ExportFile.TypeOfExport = ExportFile.ExportType.ArtifactSearch Then
				table = _searchManager.RetrieveSearchFields(Me.ExportFile.ArtifactID).Tables(0)
			Else
				table = _searchManager.RetrieveSearchFields(Me.ExportFile.ViewID).Tables(0)
			End If
			For count = 0 To table.Rows.Count - 1
				columnName = CType(table.Rows(count)("ColumnName"), String)
				If ShowField(columnName) Then
					_columns.Add(columnName)
					retString.AppendFormat("{0}{1}{0}", Me.ExportFile.QuoteDelimiter, columnName)
					If count <> table.Rows.Count - 1 Then
						retString.Append(Me.ExportFile.RecordDelimiter)
					End If
				End If
			Next
			If Me.ExportFile.ExportNative Then retString.AppendFormat("{2}{0}{1}{0}", Me.ExportFile.QuoteDelimiter, "FILE_PATH", Me.ExportFile.RecordDelimiter)
			If Me.ExportFile.ExportFullText Then retString.AppendFormat("{2}{0}{1}{0}", Me.ExportFile.QuoteDelimiter, "FULL_TEXT", Me.ExportFile.RecordDelimiter)
			retString.Append(System.Environment.NewLine)
			Return retString.ToString
		End Function

		Private Function ShowField(ByVal fieldName As String) As Boolean
			Select Case fieldName
				Case "Edit", "FileIcon", "AccessControlListIsInherited"
					Return False
				Case Else
					Return True
			End Select
		End Function

		Private Function GetDocumentsString(ByVal documentTable As System.Data.DataTable) As String
			Dim documentIDs As New System.Text.StringBuilder
			Dim row As System.Data.DataRow

			For Each row In documentTable.Rows
				documentIDs.AppendFormat(",{0}", CType(row("DocumentID"), Int32))
			Next
			documentIDs.Remove(0, 1)
			Return documentIDs.ToString
		End Function

		Private Sub ExportNative(ByVal exportFileName As String, ByVal fileGuid As String, ByVal artifactID As Int32, ByVal systemFileName As String) 'ByVal fileURI As String, ByVal systemFileName As String)
			If Not Me.ExportFile.ExportNative Then Exit Sub
			If System.IO.File.Exists(exportFileName) Then
				If Me.ExportFile.Overwrite Then
					System.IO.File.Delete(exportFileName)
					Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Overwriting document {0}.", systemFileName))
					_downloadHandler.DownloadFile(exportFileName, fileGuid, artifactID)
					'Me.WebClient.DownloadFile(fileURI, exportFileName)
				Else
					Me.WriteWarning(String.Format("{0} already exists. Skipping file export.", systemFileName))
				End If
			Else
				Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Now exporting document {0}.", systemFileName))
				_downloadHandler.DownloadFile(exportFileName, fileGuid, artifactID)
				'Me.WebClient.DownloadFile(fileURI, exportFileName)
			End If
			Me.WriteUpdate(String.Format("Finished exporting document {0}.", systemFileName))
		End Sub

		Private Function LogFileEntry(ByVal row As System.Data.DataRow, ByVal location As String, ByVal fullTextFileGuid As String) As String
			Dim count As Int32
			Dim fieldValue As String
			Dim retString As New System.Text.StringBuilder

			For count = 0 To Me.Columns.Count - 1
				'If TypeOf Me.Columns(count) Is DBNull Then
				'	fieldValue = String.Empty
				'Else
				'	fieldValue = CType(row(CType(Me.Columns(count), String)), String)
				'End If
				fieldValue = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(NullableTypes.HelperFunctions.DBNullConvert.ToNullableString(row(CType(Me.Columns(count), String))))
				fieldValue.Replace(ChrW(13), Me.ExportFile.NewlineDelimiter)
				retString.AppendFormat("{0}{1}{0}", Me.ExportFile.QuoteDelimiter, fieldValue)
				If Not count = Me.Columns.Count - 1 Then
					retString.Append(Me.ExportFile.RecordDelimiter)
				End If
			Next
			If Me.ExportFile.ExportNative Then retString.AppendFormat("{2}{0}{1}{0}", Me.ExportFile.QuoteDelimiter, location, Me.ExportFile.RecordDelimiter)
			If Me.ExportFile.ExportFullText Then
				Dim bodyText As String
				If fullTextFileGuid Is Nothing Then
					bodyText = String.Empty
				Else
					bodyText = _fullTextDownloader.ReadFullTextFile(_sourceDirectory & fullTextFileGuid)
					bodyText = bodyText.Replace(System.Environment.NewLine, ChrW(10).ToString)
					bodyText = bodyText.Replace(ChrW(13), ChrW(10))
					bodyText = bodyText.Replace(ChrW(10), Me.ExportFile.NewlineDelimiter)
				End If
				retString.AppendFormat("{2}{0}{1}{0}", Me.ExportFile.QuoteDelimiter, bodyText, Me.ExportFile.RecordDelimiter)
			End If
			retString.Append(System.Environment.NewLine)
			Return retString.ToString
		End Function

		Private Sub CreateVolumeLogFile(ByVal volumeLog As String)
			Dim writer As System.IO.StreamWriter
			Dim volumeFile As String

			volumeFile = String.Format("{0}{1}\export.log", Me.ExportFile.FolderPath, Me.FolderList.BaseFolder.Path)
			If System.IO.File.Exists(volumeFile) Then
				Me.WriteWarning(String.Format("Search log file '{0}' already exists, overwriting file.", volumeFile))
				System.IO.File.Delete(volumeFile)
			End If
			writer = System.IO.File.CreateText(volumeFile)
			writer.Write(volumeLog)
			writer.Close()
			Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, "Created search log file.")
		End Sub
#End Region

		Private Sub _processController_HaltProcessEvent(ByVal processID As System.Guid) Handles _processController.HaltProcessEvent
			_halt = True
		End Sub
	End Class
End Namespace