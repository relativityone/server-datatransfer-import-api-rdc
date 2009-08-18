Imports System.IO
Namespace kCura.WinEDDS
	Public Class Exporter

#Region "Members"

		Private _searchManager As kCura.WinEDDS.Service.SearchManager
		Private _folderManager As kCura.WinEDDS.Service.FolderManager
		Private _fieldManager As kCura.WinEDDS.Service.FieldManager
		Private _auditManager As kCura.WinEDDS.Service.AuditManager
		Private _exportFile As kCura.WinEDDS.ExportFile
		Private _columns As System.Collections.ArrayList
		Private _sourceDirectory As String
		Private _documentManager As kCura.WinEDDS.Service.DocumentManager
		Public DocumentsExported As Int32
		Public TotalDocuments As Int32
		Private _fullTextDownloader As kCura.WinEDDS.FullTextManager
		Private WithEvents _processController As kCura.Windows.Process.Controller
		Private WithEvents _downloadHandler As FileDownloader
		Private _halt As Boolean
		Private _volumeManager As VolumeManager
		Private _productionManager As kCura.WinEDDS.Service.ProductionManager
		Private _exportNativesToFileNamedFrom As kCura.WinEDDS.ExportNativeWithFilenameFrom
		Private _beginBatesColumn As String = ""
		Private _timekeeper As New kCura.Utility.Timekeeper
		Private _productionArtifactIDs As Int32()
		Private _isEssentialCount As Int32
		Private _lastStatusMessageTs As Long = System.DateTime.Now.Ticks
		Private _lastDocumentsExportedCountReported As Int32 = 0
		Private _fieldCollectionHasExtractedText As Boolean = False
		Private _statistics As New kCura.WinEDDS.ExportStatistics
		Private _lastStatisticsSnapshot As IDictionary
		Private _start As System.DateTime
		Private _warningCount As Int32 = 0
		Private _errorCount As Int32 = 0
		Private _fileCount As Int64 = 0
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

		Public Event ShutdownEvent()
		Public Sub Shutdown()
			RaiseEvent ShutdownEvent()
		End Sub

#Region "Constructors"

		Public Sub New(ByVal exportFile As kCura.WinEDDS.ExportFile, ByVal processController As kCura.Windows.Process.Controller)
			_searchManager = New kCura.WinEDDS.Service.SearchManager(exportFile.Credential, exportFile.CookieContainer)
			_folderManager = New kCura.WinEDDS.Service.FolderManager(exportFile.Credential, exportFile.CookieContainer)
			_documentManager = New kCura.WinEDDS.Service.DocumentManager(exportFile.Credential, exportFile.CookieContainer)
			_downloadHandler = New FileDownloader(exportFile.Credential, exportFile.CaseInfo.DocumentPath & "\EDDS" & exportFile.CaseInfo.ArtifactID, exportFile.CaseInfo.DownloadHandlerURL, exportFile.CookieContainer, kCura.WinEDDS.Service.Settings.AuthenticationToken)
			_productionManager = New kCura.WinEDDS.Service.ProductionManager(exportFile.Credential, exportFile.CookieContainer)
			_auditManager = New kCura.WinEDDS.Service.AuditManager(exportFile.Credential, exportFile.CookieContainer)
			_fieldManager = New kCura.WinEDDS.Service.FieldManager(exportFile.Credential, exportFile.CookieContainer)
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
				_start = System.DateTime.Now
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
			Dim fullTextFiles As System.Data.DataTable
			Dim fileURI As String
			Dim fileName As String
			Dim fullTextFileGuid As String
			Dim documentSpot As Int32
			Dim count As Int32
			Dim volumeFile As String
			Dim typeOfExportDisplayString As String = ""
			Dim fileCount As Int32 = 0
			Dim errorOutputFilePath As String = _exportFile.FolderPath & "\" & _exportFile.LoadFilesPrefix & "_img_errors.txt"
			If System.IO.File.Exists(errorOutputFilePath) AndAlso _exportFile.Overwrite Then kCura.Utility.File.Delete(errorOutputFilePath)
			Me.WriteUpdate("Retrieving export data from the server...")
			Dim startTicks As Int64 = System.DateTime.Now.Ticks
			Select Case Me.ExportFile.TypeOfExport
				Case ExportFile.ExportType.ArtifactSearch
					typeOfExportDisplayString = "search"
					Me.TotalDocuments = _searchManager.CountSearchByArtifactID(Me.ExportFile.CaseArtifactID, Me.ExportFile.ArtifactID)
				Case ExportFile.ExportType.ParentSearch
					typeOfExportDisplayString = "folder"
					Me.TotalDocuments = _searchManager.CountSearchByParentArtifactID(Me.ExportFile.CaseArtifactID, Me.ExportFile.ArtifactID, False, ExportFile.ViewID)
				Case ExportFile.ExportType.AncestorSearch
					typeOfExportDisplayString = "folder and subfolder"
					Me.TotalDocuments = _searchManager.CountSearchByParentArtifactID(Me.ExportFile.CaseArtifactID, Me.ExportFile.ArtifactID, True, ExportFile.ViewID)
				Case ExportFile.ExportType.Production
					typeOfExportDisplayString = "production"
					Me.TotalDocuments = _searchManager.CountSearchByProductionArtifactID(Me.ExportFile.CaseArtifactID, Me.ExportFile.ArtifactID)
			End Select
			If Me.TotalDocuments - 1 < Me.ExportFile.StartAtDocumentNumber Then
				Dim msg As String = String.Format("The chosen start document number ({0}) exceeds the number of {2}documents in the export ({1}).  Export halted.", Me.ExportFile.StartAtDocumentNumber + 1, Me.TotalDocuments, vbNewLine)
				MsgBox(msg, MsgBoxStyle.Critical, "Error")
				Me.Shutdown()
				Return False
			Else
				Me.TotalDocuments -= Me.ExportFile.StartAtDocumentNumber
			End If
			_statistics.MetadataTime += System.Math.Max(System.DateTime.Now.Ticks - startTicks, 1)
			RaiseEvent FileTransferModeChangeEvent(_downloadHandler.UploaderType.ToString)
			Dim columnHeaderString As String = Me.LoadColumns
			_volumeManager = New VolumeManager(Me.ExportFile, Me.ExportFile.FolderPath, Me.ExportFile.Overwrite, Me.TotalDocuments, Me, _downloadHandler, _timekeeper, _statistics)
			'folderTable = _folderManager.RetrieveAllByCaseID(Me.ExportFile.CaseArtifactID).Tables(0)
			_fullTextDownloader = New kCura.WinEDDS.FullTextManager(Me.ExportFile.Credential, _sourceDirectory, Me.ExportFile.CookieContainer)
			Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, "Created search log file.", True)
			_volumeManager.ColumnHeaderString = columnHeaderString
			Me.WriteUpdate("Data retrieved. Beginning " & typeOfExportDisplayString & " export...")
			Dim allAvfIds(_columns.Count - 1) As Int32
			For i As Int32 = 0 To allAvfIds.Length - 1
				allAvfIds(i) = Me.ExportFile.SelectedViewFields(i).AvfId
			Next
			Dim documentTable As System.Data.DataTable
			Dim start, finish, realStart As Int32
			For start = 0 To Me.TotalDocuments - 1 Step Config.ExportBatchSize
				realStart = start + Me.ExportFile.StartAtDocumentNumber
				finish = Math.Min(Me.TotalDocuments - 1 + Me.ExportFile.StartAtDocumentNumber, realStart + Config.ExportBatchSize - 1)
				_timekeeper.MarkStart("Exporter_GetDocumentBlock")
				startTicks = System.DateTime.Now.Ticks
				Select Case Me.ExportFile.TypeOfExport
					Case ExportFile.ExportType.ArtifactSearch
						documentTable = _searchManager.SearchBySearchArtifactID(Me.ExportFile.CaseArtifactID, Me.ExportFile.ArtifactID, realStart, finish, allAvfIds, Me.ExportFile.MulticodesAsNested, Me.ExportFile.NestedValueDelimiter).Tables(0)
					Case ExportFile.ExportType.ParentSearch
						documentTable = _searchManager.SearchByParentArtifactID(Me.ExportFile.CaseArtifactID, Me.ExportFile.ArtifactID, False, realStart, finish, ExportFile.ViewID, allAvfIds, Me.ExportFile.MulticodesAsNested, Me.ExportFile.NestedValueDelimiter).Tables(0)
					Case ExportFile.ExportType.AncestorSearch
						documentTable = _searchManager.SearchByParentArtifactID(Me.ExportFile.CaseArtifactID, Me.ExportFile.ArtifactID, True, realStart, finish, ExportFile.ViewID, allAvfIds, Me.ExportFile.MulticodesAsNested, Me.ExportFile.NestedValueDelimiter).Tables(0)
					Case ExportFile.ExportType.Production
						Dim production As kCura.EDDS.WebAPI.ProductionManagerBase.Production = _productionManager.Read(Me.ExportFile.CaseArtifactID, Me.ExportFile.ArtifactID)
						Dim avfIdsToAdd As New System.Collections.ArrayList
						With _fieldManager.Read(Me.ExportFile.CaseArtifactID, production.BeginBatesFieldArtifactID)
							_beginBatesColumn = kCura.DynamicFields.Types.FieldColumnNameHelper.GetSqlFriendlyName(.DisplayName)
							If System.Array.IndexOf(allAvfIds, .ArtifactViewFieldID) = -1 Then avfIdsToAdd.Add(.ArtifactViewFieldID)
						End With
						If avfIdsToAdd.Count > 0 Then
							avfIdsToAdd.AddRange(allAvfIds)
							allAvfIds = DirectCast(avfIdsToAdd.ToArray(GetType(Int32)), Int32())
						End If
						documentTable = _searchManager.SearchByProductionArtifactID(Me.ExportFile.CaseArtifactID, Me.ExportFile.ArtifactID, realStart, finish, allAvfIds, Me.ExportFile.MulticodesAsNested, Me.ExportFile.NestedValueDelimiter).Tables(0)
						If production.DocumentsHaveRedactions Then
							WriteStatusLineWithoutDocCount(kCura.Windows.Process.EventType.Warning, "Please Note - Documents in this production were produced with redactions applied. Ensure that you take steps to update the extracted text to suppress this redacted text.", True)
						End If
				End Select
				_statistics.MetadataTime += System.Math.Max(System.DateTime.Now.Ticks - startTicks, 1)
				_timekeeper.MarkEnd("Exporter_GetDocumentBlock")
				Dim docRow As System.Data.DataRow
				Dim artifactIDs As New ArrayList
				Dim docRows As New ArrayList
				For Each docRow In documentTable.Rows
					artifactIDs.Add(CType(docRow("ArtifactID"), Int32))
					docRows.Add(docRow)
					fileCount += CType(docRow("kCura_FileCount_Computed"), Int32)
					If fileCount > Config.ExportBatchSize Then
						ExportChunk(DirectCast(artifactIDs.ToArray(GetType(Int32)), Int32()), DirectCast(docRows.ToArray(GetType(System.Data.DataRow)), System.Data.DataRow()))
						artifactIDs.Clear()
						docRows.Clear()
						fileCount = 0
					End If
				Next
				If docRows.Count > 0 Then
					ExportChunk(DirectCast(artifactIDs.ToArray(GetType(Int32)), Int32()), DirectCast(docRows.ToArray(GetType(System.Data.DataRow)), System.Data.DataRow()))
					artifactIDs.Clear()
					docRows.Clear()
					fileCount = 0
				End If
				If _halt Then Exit For
			Next
			_timekeeper.GenerateCsvReportItemsAsRows()
			_volumeManager.Finish()
			Me.AuditRun(True)
		End Function

#Region "Private Helper Functions"

		Private Sub ExportChunk(ByVal documentArtifactIDs As Int32(), ByVal docRows As System.Data.DataRow())
			Dim natives As New System.Data.DataView
			'Dim fullTexts As New System.Data.DataView
			Dim images As New System.Data.DataView
			Dim productionImages As New System.Data.DataView
			Dim i As Int32 = 0
			Dim productionArtifactID As Int32 = 0
			Dim start As Int64
			If Me.ExportFile.TypeOfExport = ExportFile.ExportType.Production Then productionArtifactID = ExportFile.ArtifactID
			If Me.ExportFile.ExportNative Then
				start = System.DateTime.Now.Ticks
				If Me.ExportFile.TypeOfExport = ExportFile.ExportType.Production Then
					natives.Table = _searchManager.RetrieveNativesForProduction(Me.ExportFile.CaseArtifactID, productionArtifactID, kCura.Utility.Array.IntArrayToCSV(documentArtifactIDs)).Tables(0)
				Else
					natives.Table = _searchManager.RetrieveNativesForSearch(Me.ExportFile.CaseArtifactID, kCura.Utility.Array.IntArrayToCSV(documentArtifactIDs)).Tables(0)
				End If
				_statistics.MetadataTime += System.Math.Max(System.DateTime.Now.Ticks - start, 1)
			End If

			'If Me.ExportFile.ExportFullText OrElse Me.ExportFile.LogFileFormat = LoadFileType.FileFormat.IPRO_FullText Then fullTexts.Table = _searchManager.RetrieveFullTextExistenceForSearch(Me.ExportFile.CaseArtifactID, documentArtifactIDs).Tables(0)
			If Me.ExportFile.ExportImages Then
				_timekeeper.MarkStart("Exporter_GetImagesForDocumentBlock")
				start = System.DateTime.Now.Ticks
				images.Table = Me.RetrieveImagesForDocuments(documentArtifactIDs, Me.ExportFile.ImagePrecedence)
				productionImages.Table = Me.RetrieveProductionImagesForDocuments(documentArtifactIDs, Me.ExportFile.ImagePrecedence)
				_statistics.MetadataTime += System.Math.Max(System.DateTime.Now.Ticks - start, 1)
				_timekeeper.MarkEnd("Exporter_GetImagesForDocumentBlock")
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
				'documentInfo.HasFullText = Me.DocumentHasExtractedText(fullTexts, documentArtifactIDs(i))
				If nativeRow Is Nothing Then
					documentInfo.NativeFileGuid = ""
					documentInfo.OriginalFileName = ""
					documentInfo.NativeSourceLocation = ""
				Else
					documentInfo.NativeFileGuid = nativeRow("Guid").ToString
					documentInfo.OriginalFileName = nativeRow("Filename").ToString
					documentInfo.NativeSourceLocation = nativeRow("Location").ToString
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
				_fileCount += _volumeManager.ExportDocument(documentInfo)
				_lastStatisticsSnapshot = _statistics.ToDictionary
				Me.WriteUpdate("Exported document " & i + 1, i = documentArtifactIDs.Length - 1)
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
					image.SourceLocation = drv("Location").ToString
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
								image.SourceLocation = drv("Location").ToString
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
					If i = 0 Then
						image.BatesNumber = documentInfo.IdentifierValue
					Else
						image.BatesNumber = drv("Identifier").ToString
						If image.BatesNumber.IndexOf(image.FileGuid) <> -1 Then
							image.BatesNumber = documentInfo.IdentifierValue & "_" & i.ToString.PadLeft(imagesView.Count.ToString.Length + 1, "0"c)
						End If
					End If
					'image.BatesNumber = drv("Identifier").ToString
					Dim filenameExtension As String = ""
					If image.FileName.IndexOf(".") <> -1 Then
						filenameExtension = "." & image.FileName.Substring(image.FileName.LastIndexOf(".") + 1)
					End If
					image.FileName = kCura.Utility.File.ConvertIllegalCharactersInFilename(image.BatesNumber.ToString & filenameExtension)
					image.SourceLocation = drv("Location").ToString
					retval.Add(image)
					i += 1
				Next
			End If
			Return retval
		End Function

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
			'Dim table As System.Data.DataTable
			Dim retString As New System.Text.StringBuilder
			If _exportFile.LoadFileIsHtml Then
				retString.Append("<html><head><title>" & System.Web.HttpUtility.HtmlEncode(_exportFile.CaseInfo.Name) & "</title>")
				retString.Append("<style type='text/css'>" & vbNewLine)
				retString.Append("td {vertical-align: top;background-color:#EEEEEE;}" & vbNewLine)
				retString.Append("th {color:#DDDDDD;text-align:left;}" & vbNewLine)
				retString.Append("table {background-color:#000000;}" & vbNewLine)
				retString.Append("</style>" & vbNewLine)
				retString.Append("</head><body>" & vbNewLine)
				retString.Append("<table width='100%'><tr>" & vbNewLine)
			End If
			For Each field As WinEDDS.ViewFieldInfo In Me.ExportFile.SelectedViewFields
				Me.ExportFile.ExportFullText = Me.ExportFile.ExportFullText OrElse field.Category = DynamicFields.Types.FieldCategory.FullText
			Next
			_columns = New System.Collections.ArrayList(Me.ExportFile.SelectedViewFields)
			For i As Int32 = 0 To _columns.Count - 1
				Dim field As ViewFieldInfo = DirectCast(_columns(i), ViewFieldInfo)
				If _exportFile.LoadFileIsHtml Then
					retString.AppendFormat("{0}{1}{2}", "<th>", System.Web.HttpUtility.HtmlEncode(field.DisplayName), "</th>")
				Else
					retString.AppendFormat("{0}{1}{0}", Me.ExportFile.QuoteDelimiter, field.DisplayName)
					If i < _columns.Count - 1 Then retString.Append(Me.ExportFile.RecordDelimiter)
				End If
			Next
			If _fieldCollectionHasExtractedText AndAlso Not Me.ExportFile.ExportFullText Then
				Me.ExportFile.ExportFullText = True
				Me.ExportFile.ExportFullTextAsFile = False
			End If

			If Not Me.ExportFile.LoadFileIsHtml Then retString = New System.Text.StringBuilder(retString.ToString.TrimEnd(Me.ExportFile.RecordDelimiter))
			If _exportFile.LoadFileIsHtml Then
				If Me.ExportFile.ExportImages Then retString.Append("<th>Image Files</th>")
				If Me.ExportFile.ExportNative Then retString.Append("<th>Native Files</th>")
				'If Me.ExportFile.ExportFullText Then retString.Append("<th>Extracted Text</th>")
				retString.Append(vbNewLine & "</tr>" & vbNewLine)
			Else
				If Me.ExportFile.ExportNative Then retString.AppendFormat("{2}{0}{1}{0}", Me.ExportFile.QuoteDelimiter, "FILE_PATH", Me.ExportFile.RecordDelimiter)
				'If Me.ExportFile.ExportFullText Then retString.AppendFormat("{2}{0}{1}{0}", Me.ExportFile.QuoteDelimiter, "Extracted Text", Me.ExportFile.RecordDelimiter)
			End If
			retString.Append(System.Environment.NewLine)
			Return retString.ToString
		End Function

		Private Function ShowField(ByVal fieldName As String) As Boolean
			Select Case fieldName
				Case "ExtractedText"
					_fieldCollectionHasExtractedText = True
					Return False
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
					If productionIDs.Length > 0 Then Return _searchManager.RetrieveImagesByProductionIDsAndDocumentIDsForExport(Me.ExportFile.CaseArtifactID, productionIDs, documentArtifactIDs).Tables(0)
			End Select
		End Function

		Private Function GetProductionArtifactIDs(ByVal productionOrderList As Pair()) As Int32()
			If _productionArtifactIDs Is Nothing Then
				Dim retval As New System.Collections.ArrayList
				Dim item As Pair
				For Each item In productionOrderList
					If item.Value <> "-1" Then
						retval.Add(Int32.Parse(item.Value))
					End If
				Next
				_productionArtifactIDs = DirectCast(retval.ToArray(GetType(Int32)), Int32())
			End If
			Return _productionArtifactIDs
		End Function
#End Region


#Region "Messaging"

		Private Sub AuditRun(ByVal success As Boolean)
			Dim args As New kCura.EDDS.WebAPI.AuditManagerBase.ExportStatistics
			args.AppendOriginalFilenames = Me.ExportFile.AppendOriginalFileName
			args.Bound = Me.ExportFile.QuoteDelimiter
			Select Case Me.ExportFile.TypeOfExport
				Case ExportFile.ExportType.AncestorSearch
					args.DataSourceArtifactID = Me.ExportFile.ViewID
				Case ExportFile.ExportType.ArtifactSearch
					args.DataSourceArtifactID = Me.ExportFile.ArtifactID
				Case ExportFile.ExportType.ParentSearch
					args.DataSourceArtifactID = Me.ExportFile.ViewID
				Case ExportFile.ExportType.Production
					args.DataSourceArtifactID = Me.ExportFile.ArtifactID
			End Select
			args.Delimiter = Me.ExportFile.RecordDelimiter
			args.DestinationFilesystemFolder = Me.ExportFile.FolderPath
			args.DocumentExportCount = Me.DocumentsExported
			args.ErrorCount = _errorCount
			If Not Me.ExportFile.SelectedTextField Is Nothing Then args.ExportedTextFieldID = Me.ExportFile.SelectedTextField.FieldArtifactId
			If Me.ExportFile.ExportFullTextAsFile Then
				args.ExportedTextFileEncodingCodePage = Me.ExportFile.TextFileEncoding.CodePage
				args.ExportTextFieldAsFiles = True
			Else
				args.ExportTextFieldAsFiles = False
			End If
			Dim fields As New System.Collections.ArrayList
			For Each field As ViewFieldInfo In Me.ExportFile.SelectedViewFields
				If Not fields.Contains(field.FieldArtifactId) Then fields.Add(field.FieldArtifactId)
			Next
			args.Fields = DirectCast(fields.ToArray(GetType(Int32)), Int32())
			args.ExportNativeFiles = Me.ExportFile.ExportNative
			If args.Fields.Length > 0 OrElse Me.ExportFile.ExportNative Then
				args.MetadataLoadFileEncodingCodePage = Me.ExportFile.LoadFileEncoding.CodePage
				Select Case Me.ExportFile.LoadFileExtension.ToLower
					Case "txt"
						args.MetadataLoadFileFormat = EDDS.WebAPI.AuditManagerBase.LoadFileFormat.Custom
					Case "csv"
						args.MetadataLoadFileFormat = EDDS.WebAPI.AuditManagerBase.LoadFileFormat.Csv
					Case "dat"
						args.MetadataLoadFileFormat = EDDS.WebAPI.AuditManagerBase.LoadFileFormat.Dat
					Case "html"
						args.MetadataLoadFileFormat = EDDS.WebAPI.AuditManagerBase.LoadFileFormat.Html
				End Select
				args.MultiValueDelimiter = Me.ExportFile.MultiRecordDelimiter
				args.ExportMultipleChoiceFieldsAsNested = Me.ExportFile.MulticodesAsNested
				args.NestedValueDelimiter = Me.ExportFile.NestedValueDelimiter
				args.NewlineProxy = Me.ExportFile.NewlineDelimiter
			End If
			Try
				args.FileExportCount = CType(_fileCount, Int32)
			Catch
			End Try
			Select Case Me.ExportFile.TypeOfExportedFilePath
				Case ExportFile.ExportedFilePathType.Absolute
					args.FilePathSettings = "Use Absolute Paths"
				Case ExportFile.ExportedFilePathType.Prefix
					args.FilePathSettings = "Use Prefix: " & Me.ExportFile.FilePrefix
				Case ExportFile.ExportedFilePathType.Relative
					args.FilePathSettings = "Use Relative Paths"
			End Select
			If Me.ExportFile.ExportImages Then
				args.ExportImages = True
				Select Case Me.ExportFile.TypeOfImage
					Case ExportFile.ImageType.MultiPageTiff
						args.ImageFileType = EDDS.WebAPI.AuditManagerBase.ImageFileExportType.MultiPageTiff
					Case ExportFile.ImageType.Pdf
						args.ImageFileType = EDDS.WebAPI.AuditManagerBase.ImageFileExportType.PDF
					Case ExportFile.ImageType.SinglePage
						args.ImageFileType = EDDS.WebAPI.AuditManagerBase.ImageFileExportType.SinglePage
				End Select
				Select Case Me.ExportFile.LogFileFormat
					Case LoadFileType.FileFormat.IPRO
						args.ImageLoadFileFormat = EDDS.WebAPI.AuditManagerBase.ImageLoadFileFormatType.Ipro
					Case LoadFileType.FileFormat.IPRO_FullText
						args.ImageLoadFileFormat = EDDS.WebAPI.AuditManagerBase.ImageLoadFileFormatType.IproFullText
					Case LoadFileType.FileFormat.Opticon
						args.ImageLoadFileFormat = EDDS.WebAPI.AuditManagerBase.ImageLoadFileFormatType.Opticon
				End Select
				Dim hasOriginal As Boolean = False
				Dim hasProduction As Boolean = True
				For Each pair As WinEDDS.Pair In Me.ExportFile.ImagePrecedence
					If pair.Value <> "-1" Then
						hasProduction = True
					Else
						hasOriginal = True
					End If
				Next
				If hasProduction AndAlso hasOriginal Then
					args.ImagesToExport = EDDS.WebAPI.AuditManagerBase.ImagesToExportType.Both
				ElseIf hasProduction Then
					args.ImagesToExport = EDDS.WebAPI.AuditManagerBase.ImagesToExportType.Produced
				Else
					args.ImagesToExport = EDDS.WebAPI.AuditManagerBase.ImagesToExportType.Original
				End If
			Else
				args.ExportImages = False
			End If
			args.OverwriteFiles = Me.ExportFile.Overwrite
			Dim preclist As New System.Collections.ArrayList
			For Each pair As WinEDDS.Pair In Me.ExportFile.ImagePrecedence
				preclist.Add(Int32.Parse(pair.Value))
			Next
			args.ProductionPrecedence = DirectCast(preclist.ToArray(GetType(Int32)), Int32())
			args.RunTimeInMilliseconds = CType(System.Math.Min(System.DateTime.Now.Subtract(_start).TotalMilliseconds, Int32.MaxValue), Int32)
			If Me.ExportFile.TypeOfExport = ExportFile.ExportType.AncestorSearch OrElse Me.ExportFile.TypeOfExport = ExportFile.ExportType.ParentSearch Then
				args.SourceRootFolderID = Me.ExportFile.ArtifactID
			End If
			args.SubdirectoryImagePrefix = Me.ExportFile.VolumeInfo.SubdirectoryImagePrefix
			args.SubdirectoryMaxFileCount = Me.ExportFile.VolumeInfo.SubdirectoryMaxSize
			args.SubdirectoryNativePrefix = Me.ExportFile.VolumeInfo.SubdirectoryNativePrefix
			args.SubdirectoryStartNumber = Me.ExportFile.VolumeInfo.SubdirectoryStartNumber
			args.SubdirectoryTextPrefix = Me.ExportFile.VolumeInfo.SubdirectoryFullTextPrefix
			'args.TextAndNativeFilesNamedAfterFieldID = Me.ExportNativesToFileNamedFrom
			If Me.ExportNativesToFileNamedFrom = ExportNativeWithFilenameFrom.Identifier Then
				For Each field As ViewFieldInfo In Me.ExportFile.AllExportableFields
					If field.Category = DynamicFields.Types.FieldCategory.Identifier Then
						args.TextAndNativeFilesNamedAfterFieldID = field.FieldArtifactId
						Exit For
					End If
				Next
			Else
				For Each field As ViewFieldInfo In Me.ExportFile.AllExportableFields
					If field.AvfColumnName.ToLower = _beginBatesColumn.ToLower Then
						args.TextAndNativeFilesNamedAfterFieldID = field.FieldArtifactId
						Exit For
					End If
				Next
			End If
			args.TotalFileBytesExported = _statistics.FileBytes
			args.TotalMetadataBytesExported = _statistics.MetadataBytes
			Select Case Me.ExportFile.TypeOfExport
				Case ExportFile.ExportType.AncestorSearch
					args.Type = "Folder and Subfolders"
				Case ExportFile.ExportType.ArtifactSearch
					args.Type = "Saved Search"
				Case ExportFile.ExportType.ParentSearch
					args.Type = "Folder"
				Case ExportFile.ExportType.Production
					args.Type = "Production Set"
			End Select
			args.VolumeMaxSize = Me.ExportFile.VolumeInfo.VolumeMaxSize
			args.VolumePrefix = Me.ExportFile.VolumeInfo.VolumePrefix
			args.WarningCount = _warningCount
			Try
				_auditManager.AuditExport(Me.ExportFile.CaseInfo.ArtifactID, Not success, args)
			Catch
			End Try
		End Sub

		Friend Sub WriteFatalError(ByVal line As String, ByVal ex As System.Exception)
			Me.AuditRun(False)
			RaiseEvent FatalErrorEvent(line, ex)
		End Sub

		Friend Sub WriteStatusLine(ByVal e As kCura.Windows.Process.EventType, ByVal line As String, ByVal isEssential As Boolean)
			Dim now As Long = System.DateTime.Now.Ticks
			If now - _lastStatusMessageTs > 10000000 OrElse isEssential Then
				_lastStatusMessageTs = now
				Dim appendString As String = " ... " & Me.DocumentsExported - _lastDocumentsExportedCountReported & " document(s) exported."
				_lastDocumentsExportedCountReported = Me.DocumentsExported
				RaiseEvent StatusMessage(New ExportEventArgs(Me.DocumentsExported, Me.TotalDocuments, line & appendString, e, _lastStatisticsSnapshot))
			End If
		End Sub

		Friend Sub WriteStatusLineWithoutDocCount(ByVal e As kCura.Windows.Process.EventType, ByVal line As String, ByVal isEssential As Boolean)
			Dim now As Long = System.DateTime.Now.Ticks
			If now - _lastStatusMessageTs > 10000000 OrElse isEssential Then
				_lastStatusMessageTs = now
				_lastDocumentsExportedCountReported = Me.DocumentsExported
				RaiseEvent StatusMessage(New ExportEventArgs(Me.DocumentsExported, Me.TotalDocuments, line, e, _lastStatisticsSnapshot))
			End If
		End Sub

		Friend Sub WriteError(ByVal line As String)
			_errorCount += 1
			WriteStatusLine(kCura.Windows.Process.EventType.Error, line, True)
		End Sub

		Friend Sub WriteImgProgressError(ByVal documentInfo As Exporters.DocumentExportInfo, ByVal imageIndex As Int32, ByVal ex As System.Exception, Optional ByVal notes As String = "")
			Dim sw As New System.IO.StreamWriter(_exportFile.FolderPath & "\" & _exportFile.LoadFilesPrefix & "_img_errors.txt", True, _exportFile.LoadFileEncoding)
			sw.WriteLine(System.DateTime.Now.ToString("s"))
			sw.WriteLine(String.Format("DOCUMENT: {0}", documentInfo.IdentifierValue))
			If imageIndex > -1 AndAlso documentInfo.Images.Count > 0 Then
				sw.WriteLine(String.Format("IMAGE: {0} ({1} of {2})", documentInfo.Images(imageIndex), imageIndex + 1, documentInfo.Images.Count))
			End If
			If Not notes = "" Then sw.WriteLine("NOTES: " & notes)
			sw.WriteLine("ERROR: " & ex.ToString)
			sw.WriteLine("")
			sw.Flush()
			sw.Close()
			Dim errorLine As String = String.Format("Error processing images for document {0}: {1}. Check {2}_img_errors.txt for details", documentInfo.IdentifierValue, ex.Message.TrimEnd("."c), _exportFile.LoadFilesPrefix)
			Me.WriteError(errorLine)
		End Sub

		Friend Sub WriteWarning(ByVal line As String)
			_warningCount += 1
			WriteStatusLine(kCura.Windows.Process.EventType.Warning, line, True)
		End Sub

		Friend Sub WriteUpdate(ByVal line As String, Optional ByVal isEssential As Boolean = True)
			WriteStatusLine(kCura.Windows.Process.EventType.Progress, line, isEssential)
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
			If Not _volumeManager Is Nothing Then _volumeManager.Halt = True
		End Sub

		Public Event UploadModeChangeEvent(ByVal mode As String)

		Private Sub _downloadHandler_UploadModeChangeEvent(ByVal mode As String) Handles _downloadHandler.UploadModeChangeEvent
			RaiseEvent FileTransferModeChangeEvent(_downloadHandler.UploaderType.ToString)
		End Sub
	End Class
End Namespace