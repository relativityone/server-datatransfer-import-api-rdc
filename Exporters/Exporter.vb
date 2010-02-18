Imports System.IO
Namespace kCura.WinEDDS
	Public Class Exporter

#Region "Members"

		Private _searchManager As kCura.WinEDDS.Service.SearchManager
		Public ExportManager As kCura.WinEDDS.Service.ExportManager
		Private _folderManager As kCura.WinEDDS.Service.FolderManager
		Private _fieldManager As kCura.WinEDDS.Service.FieldManager
		Private _auditManager As kCura.WinEDDS.Service.AuditManager
		Private _exportFile As kCura.WinEDDS.ExportFile
		Private _columns As System.Collections.ArrayList
		Private _sourceDirectory As String
		Private _documentManager As kCura.WinEDDS.Service.DocumentManager
		Public DocumentsExported As Int32
		Public TotalExportArtifactCount As Int32
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

		Public Property Settings() As kCura.WinEDDS.ExportFile
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
			_downloadHandler.TotalWebTime = 0
			_productionManager = New kCura.WinEDDS.Service.ProductionManager(exportFile.Credential, exportFile.CookieContainer)
			_auditManager = New kCura.WinEDDS.Service.AuditManager(exportFile.Credential, exportFile.CookieContainer)
			_fieldManager = New kCura.WinEDDS.Service.FieldManager(exportFile.Credential, exportFile.CookieContainer)
			Me.ExportManager = New kCura.WinEDDS.Service.ExportManager(exportFile.Credential, exportFile.CookieContainer)
			_halt = False
			_processController = processController
			Me.DocumentsExported = 0
			Me.TotalExportArtifactCount = 1
			Me.Settings = exportFile
			Me.Settings.FolderPath = Me.Settings.FolderPath + "\"
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

		Private Function IsExtractedTextSelected() As Boolean
			For Each vfi As ViewFieldInfo In Me.Settings.SelectedViewFields
				If vfi.Category = DynamicFields.Types.FieldCategory.FullText Then Return True
			Next
			Return False
		End Function
		Private Function ExtractedTextField() As ViewFieldInfo
			For Each v As ViewFieldInfo In Me.Settings.AllExportableFields
				If v.Category = DynamicFields.Types.FieldCategory.FullText Then Return v
			Next
			Throw New System.Exception("Full text field somehow not in all fields")
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
			Dim exportInitializationArgs As kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults
			Dim columnHeaderString As String = Me.LoadColumns
			Dim allAvfIds As New System.Collections.Generic.List(Of Int32)
			For i As Int32 = 0 To _columns.Count - 1
				allAvfIds.Add(Me.Settings.SelectedViewFields(i).AvfId)
			Next
			Dim production As kCura.EDDS.WebAPI.ProductionManagerBase.Production
			If Me.Settings.TypeOfExport = ExportFile.ExportType.Production Then
				production = _productionManager.Read(Me.Settings.CaseArtifactID, Me.Settings.ArtifactID)
				With _fieldManager.Read(Me.Settings.CaseArtifactID, production.BeginBatesFieldArtifactID)
					_beginBatesColumn = kCura.DynamicFields.Types.FieldColumnNameHelper.GetSqlFriendlyName(.DisplayName)
					If Not allAvfIds.Contains(.ArtifactViewFieldID) Then allAvfIds.Add(.ArtifactViewFieldID)
				End With
			End If
			If Me.Settings.ExportImages AndAlso Me.Settings.LogFileFormat = LoadFileType.FileFormat.IPRO_FullText Then
				If Not Me.IsExtractedTextSelected Then
					allAvfIds.Add(Me.ExtractedTextField.AvfId)
				End If
			End If
			Select Case Me.Settings.TypeOfExport
				Case ExportFile.ExportType.ArtifactSearch
					typeOfExportDisplayString = "search"
					exportInitializationArgs = Me.ExportManager.InitializeSearchExport(_exportFile.CaseInfo.ArtifactID, Me.Settings.ArtifactID, allAvfIds.ToArray, Me.Settings.StartAtDocumentNumber + 1)

				Case ExportFile.ExportType.ParentSearch
					typeOfExportDisplayString = "folder"
					exportInitializationArgs = Me.ExportManager.InitializeFolderExport(Me.Settings.CaseArtifactID, Me.Settings.ViewID, Me.Settings.ArtifactID, False, allAvfIds.ToArray, Me.Settings.StartAtDocumentNumber + 1)

				Case ExportFile.ExportType.AncestorSearch
					typeOfExportDisplayString = "folder and subfolder"
					exportInitializationArgs = Me.ExportManager.InitializeFolderExport(Me.Settings.CaseArtifactID, Me.Settings.ViewID, Me.Settings.ArtifactID, True, allAvfIds.ToArray, Me.Settings.StartAtDocumentNumber + 1)

				Case ExportFile.ExportType.Production
					typeOfExportDisplayString = "production"
					exportInitializationArgs = Me.ExportManager.InitializeProductionExport(_exportFile.CaseInfo.ArtifactID, Me.Settings.ArtifactID, allAvfIds.ToArray, Me.Settings.StartAtDocumentNumber + 1)
			End Select
			Me.TotalExportArtifactCount = CType(exportInitializationArgs.RowCount, Int32)
			If Me.TotalExportArtifactCount - 1 < Me.Settings.StartAtDocumentNumber Then
				Dim msg As String = String.Format("The chosen start item number ({0}) exceeds the number of {2} items in the export ({1}).  Export halted.", Me.Settings.StartAtDocumentNumber + 1, Me.TotalExportArtifactCount, vbNewLine)
				MsgBox(msg, MsgBoxStyle.Critical, "Error")
				Me.Shutdown()
				Return False
			Else
				Me.TotalExportArtifactCount -= Me.Settings.StartAtDocumentNumber
			End If
			_statistics.MetadataTime += System.Math.Max(System.DateTime.Now.Ticks - startTicks, 1)
			RaiseEvent FileTransferModeChangeEvent(_downloadHandler.UploaderType.ToString)
			_volumeManager = New VolumeManager(Me.Settings, Me.Settings.FolderPath, Me.Settings.Overwrite, Me.TotalExportArtifactCount, Me, _downloadHandler, _timekeeper, exportInitializationArgs.ColumnNames, _statistics)
			_fullTextDownloader = New kCura.WinEDDS.FullTextManager(Me.Settings.Credential, _sourceDirectory, Me.Settings.CookieContainer)
			Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, "Created search log file.", True)
			_volumeManager.ColumnHeaderString = columnHeaderString
			Me.WriteUpdate("Data retrieved. Beginning " & typeOfExportDisplayString & " export...")

			Dim records As Object()
			Dim start, finish, realStart As Int32
			Dim lastRecordCount As Int32 = -1
			While lastRecordCount <> 0
				realStart = start + Me.Settings.StartAtDocumentNumber
				finish = Math.Min(Me.TotalExportArtifactCount - 1 + Me.Settings.StartAtDocumentNumber, realStart + Config.ExportBatchSize - 1)
				_timekeeper.MarkStart("Exporter_GetDocumentBlock")
				startTicks = System.DateTime.Now.Ticks
				records = Me.ExportManager.RetrieveResultsBlock(Me.Settings.CaseInfo.ArtifactID, exportInitializationArgs.RunId, Me.Settings.ArtifactTypeID, allAvfIds.ToArray, Config.ExportBatchSize)
				If Me.Settings.TypeOfExport = ExportFile.ExportType.Production AndAlso production IsNot Nothing AndAlso production.DocumentsHaveRedactions Then
					WriteStatusLineWithoutDocCount(kCura.Windows.Process.EventType.Warning, "Please Note - Documents in this production were produced with redactions applied. Ensure that you take steps to update the extracted text to suppress this redacted text.", True)
				End If
				If records Is Nothing Then Exit While
				lastRecordCount = records.Length
				_statistics.MetadataTime += System.Math.Max(System.DateTime.Now.Ticks - startTicks, 1)
				_timekeeper.MarkEnd("Exporter_GetDocumentBlock")
				Dim artifactIDs As New ArrayList
				Dim artifactIdOrdinal As Int32 = _volumeManager.OrdinalLookup("ArtifactID")
				If records.Length > 0 Then
					For Each artifactMetadata As Object() In records
						artifactIDs.Add(artifactMetadata(artifactIdOrdinal))
					Next
					ExportChunk(DirectCast(artifactIDs.ToArray(GetType(Int32)), Int32()), records)
					artifactIDs.Clear()
					fileCount = 0
				End If
				If _halt Then Exit While
			End While
			For start = 0 To Me.TotalExportArtifactCount - 1 Step Config.ExportBatchSize

			Next
			Me.WriteStatusLine(Windows.Process.EventType.Status, _downloadHandler.TotalWebTime.ToString, True)
			_timekeeper.GenerateCsvReportItemsAsRows()
			_volumeManager.Finish()
			Me.AuditRun(True)
		End Function

#Region "Private Helper Functions"

		Private Sub ExportChunk(ByVal documentArtifactIDs As Int32(), ByVal records As Object())
			Dim natives As New System.Data.DataView
			Dim images As New System.Data.DataView
			Dim productionImages As New System.Data.DataView
			Dim i As Int32 = 0
			Dim productionArtifactID As Int32 = 0
			Dim start As Int64
			If Me.Settings.TypeOfExport = ExportFile.ExportType.Production Then productionArtifactID = Settings.ArtifactID
			If Me.Settings.ExportNative Then
				start = System.DateTime.Now.Ticks
				If Me.Settings.TypeOfExport = ExportFile.ExportType.Production Then
					natives.Table = _searchManager.RetrieveNativesForProduction(Me.Settings.CaseArtifactID, productionArtifactID, kCura.Utility.Array.IntArrayToCSV(documentArtifactIDs)).Tables(0)
				ElseIf Me.Settings.ArtifactTypeID = kCura.EDDS.Types.ArtifactType.Document Then
					natives.Table = _searchManager.RetrieveNativesForSearch(Me.Settings.CaseArtifactID, kCura.Utility.Array.IntArrayToCSV(documentArtifactIDs)).Tables(0)
				Else
					Dim dt As System.Data.DataTable = _searchManager.RetrieveFilesForDynamicObjects(Me.Settings.CaseArtifactID, Me.Settings.FileField.FieldID, documentArtifactIDs).Tables(0)
					If dt Is Nothing Then
						natives = Nothing
					Else
						natives.Table = dt
					End If
				End If
				_statistics.MetadataTime += System.Math.Max(System.DateTime.Now.Ticks - start, 1)
			End If
			If Me.Settings.ExportImages Then
				_timekeeper.MarkStart("Exporter_GetImagesForDocumentBlock")
				start = System.DateTime.Now.Ticks
				images.Table = Me.RetrieveImagesForDocuments(documentArtifactIDs, Me.Settings.ImagePrecedence)
				productionImages.Table = Me.RetrieveProductionImagesForDocuments(documentArtifactIDs, Me.Settings.ImagePrecedence)
				_statistics.MetadataTime += System.Math.Max(System.DateTime.Now.Ticks - start, 1)
				_timekeeper.MarkEnd("Exporter_GetImagesForDocumentBlock")
			End If
			Dim beginBatesColumnIndex As Int32 = -1
			If Me.ExportNativesToFileNamedFrom = ExportNativeWithFilenameFrom.Production Then
				beginBatesColumnIndex = _volumeManager.OrdinalLookup(_beginBatesColumn)
			End If
			Dim identifierColumnName As String = kCura.DynamicFields.Types.FieldColumnNameHelper.GetSqlFriendlyName(Me.Settings.IdentifierColumnName)
			Dim identifierColumnIndex As Int32 = _volumeManager.OrdinalLookup(identifierColumnName)
			For i = 0 To documentArtifactIDs.Length - 1
				Dim artifact As New Exporters.ObjectExportInfo
				Dim record As Object() = DirectCast(records(i), Object())
				Dim nativeRow As System.Data.DataRowView = GetNativeRow(natives, documentArtifactIDs(i))
				If Me.ExportNativesToFileNamedFrom = ExportNativeWithFilenameFrom.Production Then
					artifact.ProductionBeginBates = record(beginBatesColumnIndex).ToString
				End If
				artifact.IdentifierValue = record(identifierColumnIndex).ToString
				artifact.Images = Me.PrepareImages(images, productionImages, documentArtifactIDs(i), artifact.IdentifierValue, artifact, Me.Settings.ImagePrecedence)
				If nativeRow Is Nothing Then
					artifact.NativeFileGuid = ""
					artifact.OriginalFileName = ""
					artifact.NativeSourceLocation = ""
				Else
					artifact.OriginalFileName = nativeRow("Filename").ToString
					artifact.NativeSourceLocation = nativeRow("Location").ToString
					If Me.Settings.ArtifactTypeID = kCura.EDDS.Types.ArtifactType.Document Then
						artifact.NativeFileGuid = nativeRow("Guid").ToString
					Else
						artifact.FileID = CType(nativeRow("FileID"), Int32)
					End If
				End If
				If nativeRow Is Nothing Then
					artifact.NativeExtension = ""
				ElseIf nativeRow("Filename").ToString.IndexOf(".") <> -1 Then
					artifact.NativeExtension = nativeRow("Filename").ToString.Substring(nativeRow("Filename").ToString.LastIndexOf(".") + 1)
				Else
					artifact.NativeExtension = ""
				End If
				artifact.ArtifactID = documentArtifactIDs(i)
				artifact.Metadata = DirectCast(records(i), Object())
				_fileCount += _volumeManager.ExportArtifact(artifact)
				_lastStatisticsSnapshot = _statistics.ToDictionary
				Me.WriteUpdate("Exported document " & i + 1, i = documentArtifactIDs.Length - 1)
				If _halt Then Exit Sub
			Next
		End Sub

		Private Function PrepareImagesForProduction(ByVal imagesView As System.Data.DataView, ByVal documentArtifactID As Int32, ByVal batesBase As String, ByVal artifact As Exporters.ObjectExportInfo) As System.Collections.ArrayList
			Dim retval As New System.Collections.ArrayList
			If Not Me.Settings.ExportImages Then Return retval
			imagesView.RowFilter = "DocumentArtifactID = " & documentArtifactID.ToString
			Dim i As Int32 = 0
			If imagesView.Count > 0 Then
				Dim drv As System.Data.DataRowView
				For Each drv In imagesView
					Dim image As New Exporters.ImageExportInfo
					image.FileName = drv("ImageFileName").ToString
					image.FileGuid = drv("ImageGuid").ToString
					image.ArtifactID = documentArtifactID
					image.PageOffset = kCura.Utility.NullableTypesHelper.DBNullConvertToNullable(Of Int32)(drv("ByteRange"))
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

		Private Function PrepareImages(ByVal imagesView As System.Data.DataView, ByVal productionImagesView As System.Data.DataView, ByVal documentArtifactID As Int32, ByVal batesBase As String, ByVal artifact As Exporters.ObjectExportInfo, ByVal productionOrderList As Pair()) As System.Collections.ArrayList
			Dim retval As New System.Collections.ArrayList
			If Not Me.Settings.ExportImages Then Return retval
			If Me.Settings.TypeOfExport = ExportFile.ExportType.Production Then Return Me.PrepareImagesForProduction(productionImagesView, documentArtifactID, batesBase, artifact)
			Dim item As Pair
			For Each item In productionOrderList
				If item.Value = "-1" Then
					Return Me.PrepareOriginalImages(imagesView, documentArtifactID, batesBase, artifact)
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
								image.PageOffset = kCura.Utility.NullableTypesHelper.DBNullConvertToNullable(Of Int32)(drv("ByteRange"))
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

		Private Function PrepareOriginalImages(ByVal imagesView As System.Data.DataView, ByVal documentArtifactID As Int32, ByVal batesBase As String, ByVal artifact As Exporters.ObjectExportInfo) As System.Collections.ArrayList
			Dim retval As New System.Collections.ArrayList
			If Not Me.Settings.ExportImages Then Return retval
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
					image.PageOffset = kCura.Utility.NullableTypesHelper.DBNullConvertToNullable(Of Int32)(drv("ByteRange"))
					If i = 0 Then
						image.BatesNumber = artifact.IdentifierValue
					Else
						image.BatesNumber = drv("Identifier").ToString
						If image.BatesNumber.IndexOf(image.FileGuid) <> -1 Then
							image.BatesNumber = artifact.IdentifierValue & "_" & i.ToString.PadLeft(imagesView.Count.ToString.Length + 1, "0"c)
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
			If Not Me.Settings.ExportNative Then Return Nothing
			If Me.Settings.ArtifactTypeID = 10 Then
				dv.RowFilter = "DocumentArtifactID = " & artifactID.ToString
			Else
				dv.RowFilter = "ObjectArtifactID = " & artifactID.ToString
			End If
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
			For Each field As WinEDDS.ViewFieldInfo In Me.Settings.SelectedViewFields
				Me.Settings.ExportFullText = Me.Settings.ExportFullText OrElse field.Category = DynamicFields.Types.FieldCategory.FullText
			Next
			_columns = New System.Collections.ArrayList(Me.Settings.SelectedViewFields)
			For i As Int32 = 0 To _columns.Count - 1
				Dim field As ViewFieldInfo = DirectCast(_columns(i), ViewFieldInfo)
				If _exportFile.LoadFileIsHtml Then
					retString.AppendFormat("{0}{1}{2}", "<th>", System.Web.HttpUtility.HtmlEncode(field.DisplayName), "</th>")
				Else
					retString.AppendFormat("{0}{1}{0}", Me.Settings.QuoteDelimiter, field.DisplayName)
					If i < _columns.Count - 1 Then retString.Append(Me.Settings.RecordDelimiter)
				End If
			Next
			If _fieldCollectionHasExtractedText AndAlso Not Me.Settings.ExportFullText Then
				Me.Settings.ExportFullText = True
				Me.Settings.ExportFullTextAsFile = False
			End If

			If Not Me.Settings.LoadFileIsHtml Then retString = New System.Text.StringBuilder(retString.ToString.TrimEnd(Me.Settings.RecordDelimiter))
			If _exportFile.LoadFileIsHtml Then
				If Me.Settings.ExportImages Then retString.Append("<th>Image Files</th>")
				If Me.Settings.ExportNative Then retString.Append("<th>Native Files</th>")
				'If Me.Settings.ExportFullText Then retString.Append("<th>Extracted Text</th>")
				retString.Append(vbNewLine & "</tr>" & vbNewLine)
			Else
				If Me.Settings.ExportNative Then retString.AppendFormat("{2}{0}{1}{0}", Me.Settings.QuoteDelimiter, "FILE_PATH", Me.Settings.RecordDelimiter)
				'If Me.Settings.ExportFullText Then retString.AppendFormat("{2}{0}{1}{0}", Me.Settings.QuoteDelimiter, "Extracted Text", Me.Settings.RecordDelimiter)
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
			Select Case Me.Settings.TypeOfExport
				Case ExportFile.ExportType.Production
					Return Nothing
				Case Else
					Return _searchManager.RetrieveImagesForDocuments(Me.Settings.CaseArtifactID, documentArtifactIDs).Tables(0)
			End Select
		End Function

		Private Function RetrieveProductionImagesForDocuments(ByVal documentArtifactIDs As Int32(), ByVal productionOrderList As Pair()) As System.Data.DataTable
			Select Case Me.Settings.TypeOfExport
				Case ExportFile.ExportType.Production
					Return _searchManager.RetrieveImagesForProductionDocuments(Me.Settings.CaseArtifactID, documentArtifactIDs, Int32.Parse(productionOrderList(0).Value)).Tables(0)
				Case Else
					Dim productionIDs As Int32() = Me.GetProductionArtifactIDs(productionOrderList)
					If productionIDs.Length > 0 Then Return _searchManager.RetrieveImagesByProductionIDsAndDocumentIDsForExport(Me.Settings.CaseArtifactID, productionIDs, documentArtifactIDs).Tables(0)
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
			args.AppendOriginalFilenames = Me.Settings.AppendOriginalFileName
			args.Bound = Me.Settings.QuoteDelimiter
			args.ArtifactTypeID = Me.Settings.ArtifactTypeID
			Select Case Me.Settings.TypeOfExport
				Case ExportFile.ExportType.AncestorSearch
					args.DataSourceArtifactID = Me.Settings.ViewID
				Case ExportFile.ExportType.ArtifactSearch
					args.DataSourceArtifactID = Me.Settings.ArtifactID
				Case ExportFile.ExportType.ParentSearch
					args.DataSourceArtifactID = Me.Settings.ViewID
				Case ExportFile.ExportType.Production
					args.DataSourceArtifactID = Me.Settings.ArtifactID
			End Select
			args.Delimiter = Me.Settings.RecordDelimiter
			args.DestinationFilesystemFolder = Me.Settings.FolderPath
			args.DocumentExportCount = Me.DocumentsExported
			args.ErrorCount = _errorCount
			If Not Me.Settings.SelectedTextField Is Nothing Then args.ExportedTextFieldID = Me.Settings.SelectedTextField.FieldArtifactId
			If Me.Settings.ExportFullTextAsFile Then
				args.ExportedTextFileEncodingCodePage = Me.Settings.TextFileEncoding.CodePage
				args.ExportTextFieldAsFiles = True
			Else
				args.ExportTextFieldAsFiles = False
			End If
			Dim fields As New System.Collections.ArrayList
			For Each field As ViewFieldInfo In Me.Settings.SelectedViewFields
				If Not fields.Contains(field.FieldArtifactId) Then fields.Add(field.FieldArtifactId)
			Next
			args.Fields = DirectCast(fields.ToArray(GetType(Int32)), Int32())
			args.ExportNativeFiles = Me.Settings.ExportNative
			If args.Fields.Length > 0 OrElse Me.Settings.ExportNative Then
				args.MetadataLoadFileEncodingCodePage = Me.Settings.LoadFileEncoding.CodePage
				Select Case Me.Settings.LoadFileExtension.ToLower
					Case "txt"
						args.MetadataLoadFileFormat = EDDS.WebAPI.AuditManagerBase.LoadFileFormat.Custom
					Case "csv"
						args.MetadataLoadFileFormat = EDDS.WebAPI.AuditManagerBase.LoadFileFormat.Csv
					Case "dat"
						args.MetadataLoadFileFormat = EDDS.WebAPI.AuditManagerBase.LoadFileFormat.Dat
					Case "html"
						args.MetadataLoadFileFormat = EDDS.WebAPI.AuditManagerBase.LoadFileFormat.Html
				End Select
				args.MultiValueDelimiter = Me.Settings.MultiRecordDelimiter
				args.ExportMultipleChoiceFieldsAsNested = Me.Settings.MulticodesAsNested
				args.NestedValueDelimiter = Me.Settings.NestedValueDelimiter
				args.NewlineProxy = Me.Settings.NewlineDelimiter
			End If
			Try
				args.FileExportCount = CType(_fileCount, Int32)
			Catch
			End Try
			Select Case Me.Settings.TypeOfExportedFilePath
				Case ExportFile.ExportedFilePathType.Absolute
					args.FilePathSettings = "Use Absolute Paths"
				Case ExportFile.ExportedFilePathType.Prefix
					args.FilePathSettings = "Use Prefix: " & Me.Settings.FilePrefix
				Case ExportFile.ExportedFilePathType.Relative
					args.FilePathSettings = "Use Relative Paths"
			End Select
			If Me.Settings.ExportImages Then
				args.ExportImages = True
				Select Case Me.Settings.TypeOfImage
					Case ExportFile.ImageType.MultiPageTiff
						args.ImageFileType = EDDS.WebAPI.AuditManagerBase.ImageFileExportType.MultiPageTiff
					Case ExportFile.ImageType.Pdf
						args.ImageFileType = EDDS.WebAPI.AuditManagerBase.ImageFileExportType.PDF
					Case ExportFile.ImageType.SinglePage
						args.ImageFileType = EDDS.WebAPI.AuditManagerBase.ImageFileExportType.SinglePage
				End Select
				Select Case Me.Settings.LogFileFormat
					Case LoadFileType.FileFormat.IPRO
						args.ImageLoadFileFormat = EDDS.WebAPI.AuditManagerBase.ImageLoadFileFormatType.Ipro
					Case LoadFileType.FileFormat.IPRO_FullText
						args.ImageLoadFileFormat = EDDS.WebAPI.AuditManagerBase.ImageLoadFileFormatType.IproFullText
					Case LoadFileType.FileFormat.Opticon
						args.ImageLoadFileFormat = EDDS.WebAPI.AuditManagerBase.ImageLoadFileFormatType.Opticon
				End Select
				Dim hasOriginal As Boolean = False
				Dim hasProduction As Boolean = False
				For Each pair As WinEDDS.Pair In Me.Settings.ImagePrecedence
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
			args.OverwriteFiles = Me.Settings.Overwrite
			Dim preclist As New System.Collections.ArrayList
			For Each pair As WinEDDS.Pair In Me.Settings.ImagePrecedence
				preclist.Add(Int32.Parse(pair.Value))
			Next
			args.ProductionPrecedence = DirectCast(preclist.ToArray(GetType(Int32)), Int32())
			args.RunTimeInMilliseconds = CType(System.Math.Min(System.DateTime.Now.Subtract(_start).TotalMilliseconds, Int32.MaxValue), Int32)
			If Me.Settings.TypeOfExport = ExportFile.ExportType.AncestorSearch OrElse Me.Settings.TypeOfExport = ExportFile.ExportType.ParentSearch Then
				args.SourceRootFolderID = Me.Settings.ArtifactID
			End If
			args.SubdirectoryImagePrefix = Me.Settings.VolumeInfo.SubdirectoryImagePrefix
			args.SubdirectoryMaxFileCount = Me.Settings.VolumeInfo.SubdirectoryMaxSize
			args.SubdirectoryNativePrefix = Me.Settings.VolumeInfo.SubdirectoryNativePrefix
			args.SubdirectoryStartNumber = Me.Settings.VolumeInfo.SubdirectoryStartNumber
			args.SubdirectoryTextPrefix = Me.Settings.VolumeInfo.SubdirectoryFullTextPrefix
			'args.TextAndNativeFilesNamedAfterFieldID = Me.ExportNativesToFileNamedFrom
			If Me.ExportNativesToFileNamedFrom = ExportNativeWithFilenameFrom.Identifier Then
				For Each field As ViewFieldInfo In Me.Settings.AllExportableFields
					If field.Category = DynamicFields.Types.FieldCategory.Identifier Then
						args.TextAndNativeFilesNamedAfterFieldID = field.FieldArtifactId
						Exit For
					End If
				Next
			Else
				For Each field As ViewFieldInfo In Me.Settings.AllExportableFields
					If field.AvfColumnName.ToLower = _beginBatesColumn.ToLower Then
						args.TextAndNativeFilesNamedAfterFieldID = field.FieldArtifactId
						Exit For
					End If
				Next
			End If
			args.TotalFileBytesExported = _statistics.FileBytes
			args.TotalMetadataBytesExported = _statistics.MetadataBytes
			Select Case Me.Settings.TypeOfExport
				Case ExportFile.ExportType.AncestorSearch
					args.Type = "Folder and Subfolders"
				Case ExportFile.ExportType.ArtifactSearch
					args.Type = "Saved Search"
				Case ExportFile.ExportType.ParentSearch
					args.Type = "Folder"
				Case ExportFile.ExportType.Production
					args.Type = "Production Set"
			End Select
			args.VolumeMaxSize = Me.Settings.VolumeInfo.VolumeMaxSize
			args.VolumePrefix = Me.Settings.VolumeInfo.VolumePrefix
			args.VolumeStartNumber = Me.Settings.VolumeInfo.VolumeStartNumber
			args.StartExportAtDocumentNumber = Me.Settings.StartAtDocumentNumber + 1
			args.CopyFilesFromRepository = Me.Settings.VolumeInfo.CopyFilesFromRepository
			args.WarningCount = _warningCount
			Try
				_auditManager.AuditExport(Me.Settings.CaseInfo.ArtifactID, Not success, args)
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
				RaiseEvent StatusMessage(New ExportEventArgs(Me.DocumentsExported, Me.TotalExportArtifactCount, line & appendString, e, _lastStatisticsSnapshot))
			End If
		End Sub

		Friend Sub WriteStatusLineWithoutDocCount(ByVal e As kCura.Windows.Process.EventType, ByVal line As String, ByVal isEssential As Boolean)
			Dim now As Long = System.DateTime.Now.Ticks
			If now - _lastStatusMessageTs > 10000000 OrElse isEssential Then
				_lastStatusMessageTs = now
				_lastDocumentsExportedCountReported = Me.DocumentsExported
				RaiseEvent StatusMessage(New ExportEventArgs(Me.DocumentsExported, Me.TotalExportArtifactCount, line, e, _lastStatisticsSnapshot))
			End If
		End Sub

		Friend Sub WriteError(ByVal line As String)
			_errorCount += 1
			WriteStatusLine(kCura.Windows.Process.EventType.Error, line, True)
		End Sub

		Friend Sub WriteImgProgressError(ByVal artifact As Exporters.ObjectExportInfo, ByVal imageIndex As Int32, ByVal ex As System.Exception, Optional ByVal notes As String = "")
			Dim sw As New System.IO.StreamWriter(_exportFile.FolderPath & "\" & _exportFile.LoadFilesPrefix & "_img_errors.txt", True, _exportFile.LoadFileEncoding)
			sw.WriteLine(System.DateTime.Now.ToString("s"))
			sw.WriteLine(String.Format("DOCUMENT: {0}", artifact.IdentifierValue))
			If imageIndex > -1 AndAlso artifact.Images.Count > 0 Then
				sw.WriteLine(String.Format("IMAGE: {0} ({1} of {2})", artifact.Images(imageIndex), imageIndex + 1, artifact.Images.Count))
			End If
			If Not notes = "" Then sw.WriteLine("NOTES: " & notes)
			sw.WriteLine("ERROR: " & ex.ToString)
			sw.WriteLine("")
			sw.Flush()
			sw.Close()
			Dim errorLine As String = String.Format("Error processing images for document {0}: {1}. Check {2}_img_errors.txt for details", artifact.IdentifierValue, ex.Message.TrimEnd("."c), _exportFile.LoadFilesPrefix)
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