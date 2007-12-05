Imports System.IO
Namespace kCura.WinEDDS
	Public Class Exporter

#Region "Members"

		Private _searchManager As kCura.WinEDDS.Service.SearchManager
		Private _folderManager As kCura.WinEDDS.Service.FolderManager
		Private _exportFile As kCura.WinEDDS.ExportFile
		Private _columns As System.Collections.ArrayList
		Private _columnFormats As System.Collections.ArrayList
		Private _sourceDirectory As String
		Private _documentManager As kCura.WinEDDS.Service.DocumentManager
		Public DocumentsExported As Int32
		Public TotalDocuments As Int32
		Private _fullTextDownloader As kCura.WinEDDS.FullTextManager
		Private WithEvents _processController As kCura.Windows.Process.Controller
		Private _downloadHandler As FileDownloader
		Private _halt As Boolean
		Private _volumeManager As VolumeManager
		Private _productionManager As kCura.WinEDDS.Service.ProductionManager
		Private _exportNativesToFileNamedFrom As kCura.WinEDDS.ExportNativeWithFilenameFrom
		Private _beginBatesColumn As String = ""

#End Region

#Region "Accessors"

		Public Property ExportFile() As kCura.WinEDDS.ExportFile
			Get
				Return _exportFile
			End Get
			Set(ByVal value As kCura.WinEDDS.ExportFile)
				_exportFile = value
			End Set
		End Property
		Public Property Columns() As System.Collections.ArrayList
			Get
				Return _columns
			End Get
			Set(ByVal value As System.Collections.ArrayList)
				_columns = value
			End Set
		End Property
		Public Property ColumnFormats() As System.Collections.ArrayList
			Get
				Return _columnFormats
			End Get
			Set(ByVal value As System.Collections.ArrayList)
				_columnFormats = value
			End Set
		End Property
		Public Property ExportNativesToFileNamedFrom() As kCura.WinEDDS.ExportNativeWithFilenameFrom
			Get
				Return _exportNativesToFileNamedFrom
			End Get
			Set(ByVal value As kCura.WinEDDS.ExportNativeWithFilenameFrom)
				_exportNativesToFileNamedFrom = value
			End Set
		End Property
		Public ReadOnly Property ErrorLogFileName() As String
			Get
				If Not _volumeManager Is Nothing Then
					Return _volumeManager.ErrorLogFileName
				End If
			End Get
		End Property

#End Region

#Region "Constructors"

		Public Sub New(ByVal exportFile As kCura.WinEDDS.ExportFile, ByVal processController As kCura.Windows.Process.Controller)
			_searchManager = New kCura.WinEDDS.Service.SearchManager(exportFile.Credential, exportFile.CookieContainer)
			_folderManager = New kCura.WinEDDS.Service.FolderManager(exportFile.Credential, exportFile.CookieContainer)
			_documentManager = New kCura.WinEDDS.Service.DocumentManager(exportFile.Credential, exportFile.CookieContainer)
			_downloadHandler = New FileDownloader(exportFile.Credential, exportFile.CaseInfo.DocumentPath & "\EDDS" & exportFile.CaseInfo.ArtifactID, exportFile.CaseInfo.DownloadHandlerURL, exportFile.CookieContainer, kCura.WinEDDS.Service.Settings.AuthenticationToken)
			_halt = False
			_processController = processController
			Me.DocumentsExported = 0
			Me.TotalDocuments = 1
			Me.ExportFile = exportFile
			Me.ExportFile.FolderPath = Me.ExportFile.FolderPath + "\"
			Me.ExportNativesToFileNamedFrom = exportFile.ExportNativesToFileNamedFrom
		End Sub

#End Region

		Public Function ExportSearch() As Boolean
			Try
				Me.Search()
			Catch ex As System.Exception
				Me.WriteFatalError(String.Format("A fatal error occurred on document #{0}", Me.DocumentsExported), ex)
				If Not _volumeManager Is Nothing Then
					_volumeManager.Close()
				End If
			End Try
			Return Me.ErrorLogFileName = ""
		End Function

		Private Function Search() As Boolean
			Dim fileTable As System.Data.DataTable
			Dim folderTable As System.Data.DataTable
			Dim fullTextFiles As System.Data.DataTable
			Dim fileURI As String
			Dim fileName As String
			Dim fullTextFileGuid As String
			Dim documentSpot As Int32
			Dim count As Int32
			Dim volumeFile As String


			Me.WriteUpdate("Retrieving export data from the server...")
			Select Case Me.ExportFile.TypeOfExport
				Case ExportFile.ExportType.ArtifactSearch
					Me.TotalDocuments = _searchManager.CountSearchByArtifactID(Me.ExportFile.CaseArtifactID, Me.ExportFile.ArtifactID)
				Case ExportFile.ExportType.ParentSearch
					Me.TotalDocuments = _searchManager.CountSearchByParentArtifactID(Me.ExportFile.CaseArtifactID, Me.ExportFile.ArtifactID, False, ExportFile.ViewID)
				Case ExportFile.ExportType.AncestorSearch
					Me.TotalDocuments = _searchManager.CountSearchByParentArtifactID(Me.ExportFile.CaseArtifactID, Me.ExportFile.ArtifactID, True, ExportFile.ViewID)
				Case ExportFile.ExportType.Production
					Me.TotalDocuments = _searchManager.CountSearchByProductionArtifactID(Me.ExportFile.CaseArtifactID, Me.ExportFile.ArtifactID)
			End Select
			RaiseEvent FileTransferModeChangeEvent(_downloadHandler.UploaderType.ToString)
			_volumeManager = New VolumeManager(Me.ExportFile, Me.ExportFile.FolderPath, Me.ExportFile.Overwrite, Me.TotalDocuments, Me, _downloadHandler)
			folderTable = _folderManager.RetrieveAllByCaseID(Me.ExportFile.CaseArtifactID).Tables(0)
			_sourceDirectory = _documentManager.GetDocumentDirectoryByCaseArtifactID(Me.ExportFile.CaseArtifactID) & "\"
			_fullTextDownloader = New kCura.WinEDDS.FullTextManager(Me.ExportFile.Credential, _sourceDirectory, Me.ExportFile.CookieContainer)
			Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, "Created search log file.")
			_volumeManager.ColumnHeaderString = Me.LoadColumns()
			Me.WriteUpdate("Data retrieved. Beginning search export...")

			Dim documentTable As System.Data.DataTable
			Dim start, finish As Int32
			For start = 0 To Me.TotalDocuments - 1 Step Config.SearchExportChunkSize
				finish = Math.Min(Me.TotalDocuments - 1, start + Config.SearchExportChunkSize - 1)
				Select Case Me.ExportFile.TypeOfExport
					Case ExportFile.ExportType.ArtifactSearch
						documentTable = _searchManager.SearchBySearchArtifactID(Me.ExportFile.CaseArtifactID, Me.ExportFile.ArtifactID, start, finish).Tables(0)
					Case ExportFile.ExportType.ParentSearch
						documentTable = _searchManager.SearchByParentArtifactID(Me.ExportFile.CaseArtifactID, Me.ExportFile.ArtifactID, False, start, finish, ExportFile.ViewID).Tables(0)
					Case ExportFile.ExportType.AncestorSearch
						documentTable = _searchManager.SearchByParentArtifactID(Me.ExportFile.CaseArtifactID, Me.ExportFile.ArtifactID, True, start, finish, ExportFile.ViewID).Tables(0)
					Case ExportFile.ExportType.Production
						documentTable = _searchManager.SearchByProductionArtifactID(Me.ExportFile.CaseArtifactID, Me.ExportFile.ArtifactID, start, finish).Tables(0)
				End Select
				Dim docRow As System.Data.DataRow
				Dim artifactIDs As New ArrayList
				Dim docRows As New ArrayList
				For Each docRow In documentTable.Rows
					artifactIDs.Add(CType(docRow("ArtifactID"), Int32))
					docRows.Add(docRow)
				Next
				ExportChunk(DirectCast(artifactIDs.ToArray(GetType(Int32)), Int32()), DirectCast(docRows.ToArray(GetType(System.Data.DataRow)), System.Data.DataRow()))
				artifactIDs.Clear()
				docRows.Clear()
				If _halt Then Exit For
			Next
			_volumeManager.Finish()
		End Function

#Region "Private Helper Functions"

		Private Sub ExportChunk(ByVal documentArtifactIDs As Int32(), ByVal docRows As System.Data.DataRow())
			Dim natives As New System.Data.DataView
			Dim fullTexts As New System.Data.DataView
			Dim images As New System.Data.DataView
			Dim productionImages As New System.Data.DataView
			Dim i As Int32 = 0
			Dim productionArtifactID As Int32 = 0
			If Me.ExportFile.TypeOfExport = ExportFile.ExportType.Production Then productionArtifactID = ExportFile.ArtifactID
			If Me.ExportFile.ExportNative Then natives.Table = _searchManager.RetrieveNativesForSearch(Me.ExportFile.CaseArtifactID, kCura.Utility.Array.IntArrayToCSV(documentArtifactIDs)).Tables(0)
			If Me.ExportFile.ExportFullText Then fullTexts.Table = _searchManager.RetrieveFullTextExistenceForSearch(Me.ExportFile.CaseArtifactID, documentArtifactIDs).Tables(0)
			If Me.ExportFile.ExportImages Then
				images.Table = Me.RetrieveImagesForDocuments(documentArtifactIDs, Me.ExportFile.ImagePrecedence)
				productionImages.Table = Me.RetrieveProductionImagesForDocuments(documentArtifactIDs, Me.ExportFile.ImagePrecedence)
			End If

			For i = 0 To documentArtifactIDs.Length - 1
				Dim documentInfo As New Exporters.DocumentExportInfo
				Dim nativeRow As System.Data.DataRowView = GetNativeRow(natives, documentArtifactIDs(i))
				If Me.ExportNativesToFileNamedFrom = ExportNativeWithFilenameFrom.Production Then
					documentInfo.ProductionBeginBates = docRows(i)(_beginBatesColumn).ToString
				End If
				Dim identifierColumnName As String = kCura.DynamicFields.Types.FieldColumnNameHelper.GetSqlFriendlyName(Me.ExportFile.IdentifierColumnName)
				documentInfo.IdentifierValue = docRows(i)(identifierColumnName).ToString
				documentInfo.Images = Me.PrepareImages(images, productionImages, documentArtifactIDs(i), documentInfo.IdentifierValue, documentInfo, Me.ExportFile.ImagePrecedence)
				documentInfo.HasFullText = Me.DocumentHasExtractedText(fullTexts, documentArtifactIDs(i))
				If nativeRow Is Nothing Then
					documentInfo.NativeFileGuid = ""
					documentInfo.OriginalFileName = ""
				Else
					documentInfo.NativeFileGuid = nativeRow("Guid").ToString
					documentInfo.OriginalFileName = nativeRow("Filename").ToString
				End If
				If nativeRow Is Nothing Then
					documentInfo.NativeExtension = ""
				ElseIf nativeRow("Filename").ToString.IndexOf(".") <> -1 Then
					documentInfo.NativeExtension = nativeRow("Filename").ToString.Substring(nativeRow("Filename").ToString.LastIndexOf(".") + 1)
				Else
					documentInfo.NativeExtension = ""
				End If
				documentInfo.DocumentArtifactID = documentArtifactIDs(i)
				documentInfo.DataRow = docRows(i)
				_volumeManager.ExportDocument(documentInfo)
				Me.WriteUpdate("Exported document " & i + 1)
				If _halt Then Exit Sub
			Next
		End Sub

		Private Function PrepareImagesForProduction(ByVal imagesView As System.Data.DataView, ByVal documentArtifactID As Int32, ByVal batesBase As String, ByVal documentInfo As Exporters.DocumentExportInfo) As System.Collections.ArrayList
			Dim retval As New System.Collections.ArrayList
			If Not Me.ExportFile.ExportImages Then Return retval
			imagesView.RowFilter = "DocumentArtifactID = " & documentArtifactID.ToString
			Dim i As Int32 = 0
			If imagesView.Count > 0 Then
				Dim drv As System.Data.DataRowView
				For Each drv In imagesView
					Dim image As New Exporters.ImageExportInfo
					image.FileName = drv("ImageFileName").ToString
					image.FileGuid = drv("ImageGuid").ToString
					image.ArtifactID = documentArtifactID
					image.PageOffset = NullableTypes.HelperFunctions.DBNullConvert.ToNullableInt32(drv("ByteRange"))
					image.BatesNumber = drv("BatesNumber").ToString
					Dim filenameExtension As String = ""
					If image.FileName.IndexOf(".") <> -1 Then
						filenameExtension = "." & image.FileName.Substring(image.FileName.LastIndexOf(".") + 1)
					End If
					image.FileName = image.BatesNumber & filenameExtension
					If Not image.FileGuid = "" Then
						retval.Add(image)
					End If
					i += 1
				Next
			End If
			Return retval
		End Function

		Private Function PrepareImages(ByVal imagesView As System.Data.DataView, ByVal productionImagesView As System.Data.DataView, ByVal documentArtifactID As Int32, ByVal batesBase As String, ByVal documentInfo As Exporters.DocumentExportInfo, ByVal productionOrderList As Pair()) As System.Collections.ArrayList
			Dim retval As New System.Collections.ArrayList
			If Not Me.ExportFile.ExportImages Then Return retval
			If Me.ExportFile.TypeOfExport = ExportFile.ExportType.Production Then Return Me.PrepareImagesForProduction(productionImagesView, documentArtifactID, batesBase, documentInfo)
			Dim item As Pair
			For Each item In productionOrderList
				If item.Value = "-1" Then
					Return Me.PrepareOriginalImages(imagesView, documentArtifactID, batesBase, documentInfo)
				Else
					productionImagesView.RowFilter = String.Format("DocumentArtifactID = {0} AND ProductionArtifactID = {1}", documentArtifactID, item.Value)
					If productionImagesView.Count > 0 Then
						Dim drv As System.Data.DataRowView
						Dim i As Int32 = 0
						For Each drv In productionImagesView
							Dim image As New Exporters.ImageExportInfo
							image.FileName = drv("ImageFileName").ToString
							image.FileGuid = drv("ImageGuid").ToString
							If image.FileGuid <> "" Then
								image.ArtifactID = documentArtifactID
								image.BatesNumber = drv("BatesNumber").ToString
								image.PageOffset = NullableTypes.HelperFunctions.DBNullConvert.ToNullableInt32(drv("ByteRange"))
								Dim filenameExtension As String = ""
								If image.FileName.IndexOf(".") <> -1 Then
									filenameExtension = "." & image.FileName.Substring(image.FileName.LastIndexOf(".") + 1)
								End If
								image.FileName = image.BatesNumber & filenameExtension
								retval.Add(image)
								i += 1
							End If
						Next
						Return retval
					End If
				End If
			Next
			Return retval
		End Function

		Private Function PrepareOriginalImages(ByVal imagesView As System.Data.DataView, ByVal documentArtifactID As Int32, ByVal batesBase As String, ByVal documentInfo As Exporters.DocumentExportInfo) As System.Collections.ArrayList
			Dim retval As New System.Collections.ArrayList
			If Not Me.ExportFile.ExportImages Then Return retval
			Dim item As Pair
			imagesView.RowFilter = "DocumentArtifactID = " & documentArtifactID.ToString
			Dim i As Int32 = 0
			If imagesView.Count > 0 Then
				Dim drv As System.Data.DataRowView
				For Each drv In imagesView
					Dim image As New Exporters.ImageExportInfo
					image.FileName = drv("Filename").ToString
					image.FileGuid = drv("Guid").ToString
					image.ArtifactID = documentArtifactID
					image.PageOffset = NullableTypes.HelperFunctions.DBNullConvert.ToNullableInt32(drv("ByteRange"))
					'If i = 0 Then
					'	image.BatesNumber = batesBase
					'Else
					'	image.BatesNumber = batesBase & "_" & i.ToString.PadLeft(imagesView.Count.ToString.Length, "0"c)
					'End If
					image.BatesNumber = drv("Identifier").ToString
					Dim filenameExtension As String = ""
					If image.FileName.IndexOf(".") <> -1 Then
						filenameExtension = "." & image.FileName.Substring(image.FileName.LastIndexOf(".") + 1)
					End If
					image.FileName = image.BatesNumber & filenameExtension
					retval.Add(image)
					i += 1
				Next
			End If
			Return retval
		End Function

		'Private Function PrepareImages(ByVal imagesView As System.Data.DataView, ByVal productionImagesView As System.Data.DataView, ByVal documentArtifactID As Int32, ByVal batesBase As String, ByVal documentInfo As Exporters.DocumentExportInfo) As System.Collections.ArrayList
		'	If Me.ExportFile.TypeOfExport = ExportFile.ExportType.Production Then Return Me.PrepareImagesForProduction(productionImagesView, documentArtifactID, batesBase, documentInfo)
		'	Dim retval As New System.Collections.ArrayList
		'	If Not Me.ExportFile.ExportImages Then Return retval
		'	imagesView.RowFilter = "DocumentArtifactID = " & documentArtifactID.ToString
		'	Dim i As Int32 = 0
		'	If imagesView.Count > 0 Then
		'		Dim drv As System.Data.DataRowView
		'		For Each drv In imagesView
		'			Dim image As New Exporters.ImageExportInfo
		'			image.FileName = drv("Filename").ToString
		'			image.FileGuid = drv("Guid").ToString
		'			image.ArtifactID = documentArtifactID
		'			If i = 0 Then
		'				image.BatesNumber = batesBase
		'			Else
		'				image.BatesNumber = batesBase & "_" & i.ToString.PadLeft(imagesView.Count.ToString.Length, "0"c)
		'			End If
		'			Dim filenameExtension As String = ""
		'			If image.FileName.IndexOf(".") <> -1 Then
		'				filenameExtension = "." & image.FileName.Substring(image.FileName.LastIndexOf(".") + 1)
		'			End If
		'			image.FileName = image.BatesNumber & filenameExtension
		'			retval.Add(image)
		'			i += 1
		'		Next
		'	End If
		'	Return retval
		'End Function
		'Private Function GetFullTextFileGuid(ByVal dt As System.Data.DataTable, ByVal documentArtifactID As Int32) As String
		'	Dim row As System.Data.DataRow
		'	If Me.ExportFile.ExportFullText Then
		'		For Each row In dt.Rows
		'			If CType(row("DocumentArtifactID"), Int32) = documentArtifactID Then
		'				Return CType(row("Guid"), String)
		'			End If
		'		Next
		'	Else
		'		Return String.Empty
		'	End If
		'End Function

		Private Function DocumentHasExtractedText(ByVal dv As System.Data.DataView, ByVal documentArtifactID As Int32) As Boolean
			dv.RowFilter = "ArtifactID = " & documentArtifactID
			Try
				Return CType(dv(0)("HasExtractedText"), Boolean)
			Catch ex As System.Exception
				Return False
			End Try
		End Function

		Private Function GetNativeRow(ByVal dv As System.Data.DataView, ByVal artifactID As Int32) As System.Data.DataRowView
			If Not Me.ExportFile.ExportNative Then Return Nothing
			dv.RowFilter = "DocumentArtifactID = " & artifactID.ToString
			If dv.Count > 0 Then
				Return dv(0)
			Else
				Return Nothing
			End If
		End Function

		Private Function LoadColumns() As String
			Dim count As Int32
			Dim columnName As String
			Dim table As System.Data.DataTable
			Dim retString As New System.Text.StringBuilder

			_columns = New System.Collections.ArrayList
			_columnFormats = New System.Collections.ArrayList
			Select Case Me.ExportFile.TypeOfExport
				Case ExportFile.ExportType.AncestorSearch, ExportFile.ExportType.ParentSearch
					table = _searchManager.RetrieveSearchFields(Me.ExportFile.CaseArtifactID, Me.ExportFile.ViewID).Tables(0)
				Case ExportFile.ExportType.ArtifactSearch
					table = _searchManager.RetrieveSearchFields(Me.ExportFile.CaseArtifactID, Me.ExportFile.ArtifactID).Tables(0)
				Case ExportFile.ExportType.Production
					table = _searchManager.RetrieveSearchFieldsForProduction(Me.ExportFile.CaseArtifactID, Me.ExportFile.ArtifactID).Tables(0)
					Dim row As System.Data.DataRow
					For Each row In table.Rows
						If row("ColumnType").ToString.ToLower = "beginbates" Then _beginBatesColumn = row("ColumnName").ToString
					Next
			End Select

			For count = 0 To table.Rows.Count - 1
				columnName = CType(table.Rows(count)("ColumnName"), String)
				If ShowField(columnName) Then
					_columns.Add(columnName)
					retString.AppendFormat("{0}{1}{0}", Me.ExportFile.QuoteDelimiter, columnName)
					If count <> table.Rows.Count - 1 Then
						retString.Append(Me.ExportFile.RecordDelimiter)
					End If
					If table.Rows(count)("ItemListType").ToString.ToLower = "datetime" Then
						_columnFormats.Add(table.Rows(count)("FormatString"))
					Else
						_columnFormats.Add("")
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
			Dim artifactIDs As New System.Text.StringBuilder
			Dim row As System.Data.DataRow

			For Each row In documentTable.Rows
				artifactIDs.AppendFormat(",{0}", CType(row("ArtifactID"), Int32))
			Next
			artifactIDs.Remove(0, 1)
			Return artifactIDs.ToString
		End Function

		Private Sub ExportNative(ByVal exportFileName As String, ByVal fileGuid As String, ByVal artifactID As Int32, ByVal systemFileName As String)		'ByVal fileURI As String, ByVal systemFileName As String)
			If Not Me.ExportFile.ExportNative Then Exit Sub
			If System.IO.File.Exists(exportFileName) Then
				If Me.ExportFile.Overwrite Then
					System.IO.File.Delete(exportFileName)
					Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Overwriting document {0}.", systemFileName))
					_downloadHandler.DownloadFile(exportFileName, fileGuid, artifactID, Me.ExportFile.CaseArtifactID.ToString)
				Else
					Me.WriteWarning(String.Format("{0} already exists. Skipping file export.", systemFileName))
				End If
			Else
				Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Now exporting document {0}.", systemFileName))
				_downloadHandler.DownloadFile(exportFileName, fileGuid, artifactID, Me.ExportFile.CaseArtifactID.ToString)
			End If
			Me.WriteUpdate(String.Format("Finished exporting document {0}.", systemFileName))
		End Sub

		Private Function LogFileEntry(ByVal row As System.Data.DataRow, ByVal location As String, ByVal fullTextFileGuid As String) As String
			Dim count As Int32
			Dim fieldValue As String
			Dim retString As New System.Text.StringBuilder
			Dim columnName As String
			For count = 0 To Me.Columns.Count - 1
				'If TypeOf Me.Columns(count) Is DBNull Then
				'	fieldValue = String.Empty
				'Else
				'	fieldValue = CType(row(CType(Me.Columns(count), String)), String)
				'End If
				columnName = CType(Me.Columns(count), String)
				Dim val As Object = row(columnName)
				If TypeOf val Is Byte() Then
					val = System.Text.Encoding.Unicode.GetString(DirectCast(val, Byte()))
				End If
				fieldValue = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(NullableTypes.HelperFunctions.DBNullConvert.ToNullableString(val))
				fieldValue = fieldValue.Replace(System.Environment.NewLine, ChrW(10).ToString)
				fieldValue = fieldValue.Replace(ChrW(13), ChrW(10))
				fieldValue = fieldValue.Replace(ChrW(10), Me.ExportFile.NewlineDelimiter)
				If fieldValue.Length > 1 AndAlso fieldValue.Chars(0) = ChrW(11) AndAlso fieldValue.Chars(fieldValue.Length - 1) = ChrW(11) Then
					fieldValue = fieldValue.Trim(New Char() {ChrW(11)}).Replace(ChrW(11), Me.ExportFile.MultiRecordDelimiter)
				End If
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

		'Private Sub CreateVolumeLogFile(ByVal volumeLog As String)
		'	Dim writer As System.IO.StreamWriter
		'	Dim volumeFile As String

		'	volumeFile = String.Format("{0}{1}\export.log", Me.ExportFile.FolderPath, Me.FolderList.BaseFolder.Path)
		'	If System.IO.File.Exists(volumeFile) Then
		'		Me.WriteWarning(String.Format("Search log file '{0}' already exists, overwriting file.", volumeFile))
		'		System.IO.File.Delete(volumeFile)
		'	End If
		'	writer = System.IO.File.CreateText(volumeFile)
		'	writer.Write(volumeLog)
		'	writer.Close()
		'	Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, "Created search log file.")
		'End Sub

		Private Function RetrieveImagesForDocuments(ByVal documentArtifactIDs As Int32(), ByVal productionOrderList As Pair()) As System.Data.DataTable
			Select Case Me.ExportFile.TypeOfExport
				Case ExportFile.ExportType.Production
					Return Nothing
				Case Else
					Return _searchManager.RetrieveImagesForDocuments(Me.ExportFile.CaseArtifactID, documentArtifactIDs).Tables(0)
			End Select
		End Function

		Private Function RetrieveProductionImagesForDocuments(ByVal documentArtifactIDs As Int32(), ByVal productionOrderList As Pair()) As System.Data.DataTable
			Select Case Me.ExportFile.TypeOfExport
				Case ExportFile.ExportType.Production
					Return _searchManager.RetrieveImagesForProductionDocuments(Me.ExportFile.CaseArtifactID, documentArtifactIDs, Int32.Parse(productionOrderList(0).Value)).Tables(0)
				Case Else
					Dim productionIDs As Int32() = Me.GetProductionArtifactIDs(productionOrderList)
					If productionIDs.Length > 0 Then Return _searchManager.RetrieveByProductionIDsAndDocumentIDs(Me.ExportFile.CaseArtifactID, productionIDs, documentArtifactIDs).Tables(0)
			End Select
		End Function

		Private Function GetProductionArtifactIDs(ByVal productionOrderList As Pair()) As Int32()
			Dim retval As New System.Collections.ArrayList
			Dim item As Pair
			For Each item In productionOrderList
				If item.Value <> "-1" Then
					retval.Add(Int32.Parse(item.Value))
				End If
			Next
			Return DirectCast(retval.ToArray(GetType(Int32)), Int32())
		End Function
#End Region


#Region "Messaging"

		Friend Sub WriteFatalError(ByVal line As String, ByVal ex As System.Exception)
			RaiseEvent FatalErrorEvent(line, ex)
		End Sub

		Friend Sub WriteStatusLine(ByVal e As kCura.Windows.Process.EventType, ByVal line As String)
			RaiseEvent StatusMessage(New ExportEventArgs(Me.DocumentsExported, Me.TotalDocuments, line, e))
		End Sub

		Friend Sub WriteError(ByVal line As String)
			WriteStatusLine(kCura.Windows.Process.EventType.Error, line)
		End Sub

		Friend Sub WriteWarning(ByVal line As String)
			WriteStatusLine(kCura.Windows.Process.EventType.Warning, line)
		End Sub

		Friend Sub WriteUpdate(ByVal line As String)
			WriteStatusLine(kCura.Windows.Process.EventType.Progress, line)
		End Sub

#End Region

#Region "Public Events"

		Public Event FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception)
		Public Event StatusMessage(ByVal exportArgs As ExportEventArgs)
		Public Event FileTransferModeChangeEvent(ByVal mode As String)
		Public Event DisableCloseButton()
		Public Event EnableCloseButton()

#End Region


		Private Sub _processController_HaltProcessEvent(ByVal processID As System.Guid) Handles _processController.HaltProcessEvent
			_halt = True
		End Sub
	End Class
End Namespace