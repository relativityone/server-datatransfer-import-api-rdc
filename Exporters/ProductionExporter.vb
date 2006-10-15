Imports System.IO
Imports System.Text
Namespace kCura.WinEDDS
	Public Class ProductionExporter

#Region "Members"
		Private _productionManager As kCura.WinEDDS.Service.ProductionManager
		Private _fileManager As kCura.WinEDDS.Service.FileManager
		Private _downloadManager As kCura.WinEDDS.FileDownloader
		Private _fullTextDownloader As kCura.WinEDDS.FullTextManager
		Private _documentManager As kCura.WinEDDS.Service.DocumentManager
		Private _folderManager As kCura.WinEDDS.Service.FolderManager
		Private _downloadHandler As FileDownloader

		Private WithEvents _processController As kCura.Windows.Process.Controller
		Private _webClient As System.Net.WebClient
		Private _folderList As kCura.WinEDDS.FolderList
		Private _exportFile As kCura.WinEDDS.ExportFile

		Public DocumentsExported As Int32
		Public TotalImages As Int32
		Private _continue As Boolean
		Private _sourceDirectory As String
#End Region

#Region "Unused Members"
		'Private _selectedCaseInfo As kCura.EDDS.Types.CaseInfo
		'Private _productionArtifactID As Int32
		'Private _folderPath As String
		'Private _overwrite As Boolean
		'Private _exportNatives As Boolean
		'Private _loadFileFormat As kCura.WinEDDS.LoadFileType.FileFormat
#End Region

#Region "Public Properties"

		Public Property ExportFile() As kCura.WinEDDS.ExportFile
			Get
				Return _exportFile
			End Get
			Set(ByVal value As kCura.WinEDDS.ExportFile)
				_exportFile = value
			End Set
		End Property

		Public ReadOnly Property ProductionArtifactID() As Int32
			Get
				Return Me.ExportFile.ArtifactID
			End Get
			'Set(ByVal value As Int32)
			'	_productionArtifactID = value
			'End Set
		End Property

		Public ReadOnly Property FolderPath() As String
			Get
				Return Me.ExportFile.FolderPath + "\"
			End Get
			'Set(ByVal value As String)
			'	Me.ExportFile.FolderPath = value
			'End Set
		End Property

		Public ReadOnly Property Overwrite() As Boolean
			Get
				Return Me.ExportFile.Overwrite
			End Get
			'Set(ByVal value As Boolean)
			'	_overwrite = value
			'End Set
		End Property

		Public ReadOnly Property LoadFileFormat() As kCura.WinEDDS.LoadFileType.FileFormat
			Get
				Return Me.ExportFile.LogFileFormat
			End Get
			'Set(ByVal value As Int32)
			'	_productionArtifactID = value
			'End Set
		End Property

		Public ReadOnly Property SelectedCaseInfo() As kCura.EDDS.Types.CaseInfo
			Get
				Return Me.ExportFile.CaseInfo
			End Get
			'Set(ByVal value As kCura.EDDS.Types.CaseInfo)
			'	_selectedCaseInfo = value
			'End Set
		End Property

		Public Property WebClient() As System.Net.WebClient
			Get
				Return _webClient
			End Get
			Set(ByVal value As System.Net.WebClient)
				_webClient = value
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
			RaiseEvent StatusMessage(New ExportEventArgs(Me.DocumentsExported, Me.TotalImages, line, e))
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
			Dim cred As Net.NetworkCredential = exportFile.Credential
			Dim cookieContainer As System.Net.CookieContainer = exportFile.CookieContainer
			Dim destinationFolderPath As String = exportFile.CaseInfo.DocumentPath & "\EDDS" & exportFile.CaseInfo.ArtifactID

			Me.DocumentsExported = 0
			Me.TotalImages = 1

			Me.ExportFile = exportFile
			Me.WebClient = New System.Net.WebClient
			Me.WebClient.Credentials = cred
			_processController = processController
			_continue = True

			_productionManager = New kCura.WinEDDS.Service.ProductionManager(cred, cookieContainer)
			_folderManager = New kCura.WinEDDS.Service.FolderManager(cred, cookieContainer)
			_documentManager = New kCura.WinEDDS.Service.DocumentManager(cred, cookieContainer)
			_downloadManager = New FileDownloader(cred, destinationFolderPath, exportFile.CaseInfo.DownloadHandlerURL, cookieContainer)
			_fileManager = New kCura.WinEDDS.Service.FileManager(cred, cookieContainer)
			_folderManager = New kCura.WinEDDS.Service.FolderManager(cred, cookieContainer)
			_fileManager = New kCura.WinEDDS.Service.FileManager(cred, cookieContainer)
			_fullTextDownloader = New kCura.WinEDDS.FullTextManager(cred, _sourceDirectory, cookieContainer)
			_downloadHandler = New FileDownloader(cred, exportFile.CaseInfo.DocumentPath & "\EDDS" & exportFile.CaseInfo.ArtifactID, exportFile.CaseInfo.DownloadHandlerURL, exportFile.CookieContainer)

			_processController = processController
			_sourceDirectory = _documentManager.GetDocumentDirectoryByContextArtifactID(Me.SelectedCaseInfo.RootArtifactID)
		End Sub

		Public Sub CreateVolumes()
			Try
				Me.ExportProduction()
			Catch ex As System.Exception
				Me.WriteFatalError(String.Format("A fatal error occurred on document #{0}", Me.DocumentsExported), ex)
			End Try
		End Sub

		Private Sub ExportProduction()
			Dim production As kCura.EDDS.WebAPI.ProductionManagerBase.Production
			Dim documentImagesTable, guidTable, nativesTable As System.Data.DataTable
			Dim fileName, currentVolumeName, currentNativeDirectoryName, currentImagesDirectoryName, fileFormat As String
			Dim volumeLog As New System.Text.StringBuilder
			Dim nativesVolumeLog As System.Text.StringBuilder
			Dim fullTextVolumeLog As System.Text.StringBuilder
			Dim fullTextLog As System.Text.StringBuilder
			Dim currentDocumentImagesCount, iterator As Int32
			Dim currentVolumeNumber As Int32
			Dim volumeSize As Long = 0
			Dim currentDocumentFilesSize As Long
			Dim imagesSubdirectoryCount As Int32
			Dim nativesSubDirectoryCount As Int32
			Dim currentImagesSubdirectoryFileCount As Int32 = 0
			Dim currentNativesSubdirectoryFileCount As Int32 = 0
			Dim currentBates As String = ""
			Dim isFirstDocumentImage As Boolean
			Dim currentDocumentHasNative As Boolean
			Dim currentPageFirstByteNumber As Int32
			'Reads my loadfile format and determines the extension for the load file.
			If Me.LoadFileFormat = kCura.WinEDDS.LoadFileType.FileFormat.Concordance Then
				fileFormat = "log"
			Else
				fileFormat = "lfp"
			End If
			Me.WriteUpdate("Retrieving export data from the server...")
			documentImagesTable = _fileManager.RetrieveByProductionArtifactIDForProduction(Me.ProductionArtifactID).Tables(0)
			production = _productionManager.Read(Me.ProductionArtifactID)
			currentVolumeNumber = production.VolumeStartNumber
			Me.BeginVolume(currentVolumeName, production.VolumePrefix, currentVolumeNumber, production.SubdirectoryStartNumber, imagesSubdirectoryCount, nativesSubDirectoryCount, volumeLog, fullTextVolumeLog, nativesVolumeLog)
			currentImagesDirectoryName = Me.CreateSubdirectory(production.SubdirectoryPrefix, production.SubdirectoryStartNumber, currentVolumeName)

			If Me.ExportFile.ExportNative Then
				nativesTable = _fileManager.RetrieveNativesForProductionExport(Me.ExportFile.ArtifactID).Tables(0)
				currentNativeDirectoryName = Me.CreateSubdirectory("NATIVES", production.SubdirectoryStartNumber, currentVolumeName)
				Dim fields As String() = {"BEGBATES", "ENDBATES", "FILE_PATH"}
				nativesVolumeLog.Append(Me.BuildNativesLog(fields))
			End If
			'Sets the total number of images in the record set.
			Me.TotalImages = documentImagesTable.Rows.Count
			Me.WriteUpdate("Data retrieved. Beginning production export...")

			For iterator = 0 To documentImagesTable.Rows.Count - 1
				isFirstDocumentImage = (iterator = 0 OrElse CType(documentImagesTable.Rows(iterator - 1)("DocumentArtifactID"), Int32) <> CType(documentImagesTable.Rows(iterator)("DocumentArtifactID"), Int32))
				If isFirstDocumentImage Then
					currentDocumentFilesSize = Me.CurrentDocumentFilesSize(documentImagesTable, nativesTable, iterator, Me.ProductionArtifactID)
					currentDocumentImagesCount = Me.CurrentDocumentImageCount(documentImagesTable, iterator)
					currentDocumentHasNative = Me.HasNative(CType(documentImagesTable.Rows(iterator)("DocumentArtifactID"), Int32), nativesTable)
					If volumeSize + currentDocumentFilesSize > production.VolumeMaxSize * 1024 Then
						If iterator > 0 Then
							Me.CompleteVolume(currentVolumeName, production.Name, volumeLog, fullTextVolumeLog, nativesVolumeLog, fileFormat)
						End If
						volumeSize = 0
						currentVolumeNumber = currentVolumeNumber + 1
						Me.BeginVolume(currentVolumeName, production.VolumePrefix, currentVolumeNumber, production.SubdirectoryStartNumber, imagesSubdirectoryCount, nativesSubDirectoryCount, volumeLog, fullTextVolumeLog, nativesVolumeLog)
					End If
					If iterator > 0 Then
						Dim createNewNativeSubdirectory As Boolean = Me.ExportFile.ExportNative AndAlso currentDocumentHasNative AndAlso currentNativesSubdirectoryFileCount + 1 > production.SubdirectoryMaxFiles OrElse volumeSize = 0
						Dim createNewImagesSubdirectory As Boolean = (currentImagesSubdirectoryFileCount + currentDocumentImagesCount) > production.SubdirectoryMaxFiles OrElse volumeSize = 0
						If createNewNativeSubdirectory OrElse createNewImagesSubdirectory Then
							imagesSubdirectoryCount += 1
							currentImagesDirectoryName = Me.CreateSubdirectory(production.SubdirectoryPrefix, imagesSubdirectoryCount, currentVolumeName)
							currentImagesSubdirectoryFileCount = 0
							If Me.ExportFile.ExportNative Then
								nativesSubDirectoryCount += 1
								currentNativeDirectoryName = Me.CreateSubdirectory("NATIVES", nativesSubDirectoryCount, currentVolumeName)
								currentNativesSubdirectoryFileCount = 0
							End If
						End If
					End If
				End If
				Try
					fileName = String.Format("{0}{1}\{2}\{3}.tif", Me.FolderPath, currentVolumeName, currentImagesDirectoryName, CType(documentImagesTable.Rows(iterator)("BatesNumber"), String))
					ExportDocumentImage(fileName, CType(documentImagesTable.Rows(iterator)("ImageGuid"), String), CType(documentImagesTable.Rows(iterator)("DocumentArtifactID"), Int32), CType(documentImagesTable.Rows(iterator)("BatesNumber"), String))
					currentImagesSubdirectoryFileCount += 1
					If Me.ExportFile.ExportNative AndAlso currentDocumentHasNative Then
						nativesVolumeLog.Append(Me.ExportNativeFile(currentNativeDirectoryName, currentVolumeName, CType(documentImagesTable.Rows(iterator)("DocumentArtifactID"), Int32), nativesTable))
						currentNativesSubdirectoryFileCount += 1
					End If
					Me.CreateLoadFileEntries(volumeLog, fullTextVolumeLog, documentImagesTable, iterator, currentVolumeName, fileName, isFirstDocumentImage, currentPageFirstByteNumber, currentNativeDirectoryName)
				Catch ex As System.Exception
					Me.WriteError(String.Format("Error occurred on document #{0} with message: {1}", iterator + 1, ex.Message))
				End Try
				If Not _continue Then
					Exit For
				End If
				volumeSize = volumeSize + currentDocumentFilesSize
			Next
			Me.CompleteVolume(currentVolumeName, production.Name, volumeLog, fullTextVolumeLog, nativesVolumeLog, fileFormat)

			If Me.ExportFile.ExportNative And documentImagesTable.Rows.Count = 0 Then
				Dim currentNativeFileSize As Long
				Dim nativesRow As System.Data.DataRow

				For Each nativesRow In nativesTable.Rows
					currentNativeFileSize = Me.CurrentNativesSize(nativesTable, CType(nativesRow("documentArtifactID"), Int32))
					If volumeSize + currentNativeFileSize > production.VolumeMaxSize * 1024 Then
						Me.DeleteVolumeLogFile(currentVolumeName, production.Name & "_NATIVES", nativesVolumeLog.ToString, "log")
						Me.DeleteVolumeLogFile(currentVolumeName, production.Name, volumeLog.ToString, fileFormat)
						Me.DeleteSubdirectory(production.SubdirectoryPrefix, production.SubdirectoryStartNumber, currentVolumeName)
						volumeSize = 0
						currentVolumeNumber += 1
						currentVolumeName = Me.CreateVolumeDirectory(production.VolumePrefix, currentVolumeNumber)
						nativesSubDirectoryCount = production.SubdirectoryStartNumber
						currentNativesSubdirectoryFileCount = 0
						currentNativeDirectoryName = Me.CreateSubdirectory("NATIVES", nativesSubDirectoryCount, currentVolumeName)
					ElseIf currentNativesSubdirectoryFileCount + 1 > production.SubdirectoryMaxFiles Then
						nativesSubDirectoryCount += 1
						currentNativesSubdirectoryFileCount = 0
						currentNativeDirectoryName = Me.CreateSubdirectory("NATIVES", nativesSubDirectoryCount, currentVolumeName)
					End If
					Me.ExportNativeFile(currentNativeDirectoryName, currentVolumeName, CType(nativesRow("documentArtifactID"), Int32), nativesTable)
					currentNativesSubdirectoryFileCount += 1
					volumeSize += currentNativeFileSize
				Next
				Me.DeleteVolumeLogFile(currentVolumeName, production.Name & "_NATIVES", nativesVolumeLog.ToString, "log")
				Me.DeleteVolumeLogFile(currentVolumeName, production.Name, volumeLog.ToString, fileFormat)
				Me.DeleteSubdirectory(production.SubdirectoryPrefix, production.SubdirectoryStartNumber, currentVolumeName)
			End If
		End Sub

		Private Sub BeginVolume(ByRef currentVolumeName As String, ByVal productionVolumePrefix As String, ByVal currentVolumeNumber As Int32, ByVal productionSubdirectoryStartNumber As Int32, ByRef imagesSubdirectoryCount As Int32, ByRef nativesSubDirectoryCount As Int32, ByRef volumeLog As StringBuilder, ByRef fullTextVolumeLog As StringBuilder, ByRef nativesVolumeLog As StringBuilder)
			currentVolumeName = Me.CreateVolumeDirectory(productionVolumePrefix, currentVolumeNumber)
			imagesSubdirectoryCount = productionSubdirectoryStartNumber
			nativesSubDirectoryCount = productionSubdirectoryStartNumber
			volumeLog = New System.Text.StringBuilder
			fullTextVolumeLog = New System.Text.StringBuilder
			nativesVolumeLog = New System.Text.StringBuilder
		End Sub

		Private Sub CompleteVolume(ByVal currentVolumeName As String, ByVal productionName As String, ByVal volumeLog As System.Text.StringBuilder, ByVal fullTextVolumeLog As System.Text.StringBuilder, ByVal nativesVolumeLog As System.Text.StringBuilder, ByVal loadFileExtension As String)
			Me.CreateVolumeLogFile(currentVolumeName, productionName, volumeLog.ToString, loadFileExtension)
			If Me.LoadFileFormat = kCura.WinEDDS.LoadFileType.FileFormat.IPRO_FullText Then
				Me.CreateVolumeLogFile(currentVolumeName, productionName + "_FT_", fullTextVolumeLog.ToString, loadFileExtension)
			End If
			If Me.ExportFile.ExportNative Then
				Me.CreateVolumeLogFile(currentVolumeName, productionName & "_NATIVES", nativesVolumeLog.ToString, "log")
			End If
		End Sub

		Private Sub CreateLoadFileEntries(ByVal volumeLog As System.Text.StringBuilder, ByVal fullTextVolumeLog As System.Text.StringBuilder, ByVal documentImagesTable As System.Data.DataTable, ByVal count As Int32, ByVal currentVolume As String, ByVal filename As String, ByVal isFirstDocumentImage As Boolean, ByVal currentPageFirstByteNumber As Int32, ByVal currentDirectory As String)
			Dim fullTextGuid As String
			Dim fullText As String
			Dim pageText As String
			Select Case Me.LoadFileFormat
				Case LoadFileType.FileFormat.Concordance
					volumeLog.Append(BuildVolumeLog(CType(documentImagesTable.Rows(count)("BatesNumber"), String), currentVolume, filename, isFirstDocumentImage))
				Case LoadFileType.FileFormat.IPRO, LoadFileType.FileFormat.IPRO_FullText
					If Me.LoadFileFormat = kCura.WinEDDS.LoadFileType.FileFormat.IPRO_FullText Then
						If isFirstDocumentImage Then
							currentPageFirstByteNumber = 0
							fullTextGuid = _fileManager.GetFullTextGuidsByDocumentArtifactIdAndType(CType(documentImagesTable.Rows(count)("documentArtifactID"), Int32), 2)
							Try
								fullText = _fullTextDownloader.ReadFullTextFile(_sourceDirectory & fullTextGuid)
								pageText = fullText.Substring(currentPageFirstByteNumber, CInt(documentImagesTable.Rows(count)("ByteRange")) - 1)
								pageText = pageText.Replace(ChrW(10), " ")
								pageText = pageText.Replace(",", "")
								pageText = pageText.Replace(" ", "|0|0|0|0^")
								fullTextVolumeLog.AppendFormat("FT,{0},1,1,{1}", CType(documentImagesTable.Rows(count)("BatesNumber"), String), pageText)
								fullTextVolumeLog.AppendFormat("{0}", Microsoft.VisualBasic.ControlChars.NewLine)
								currentPageFirstByteNumber += CInt(documentImagesTable.Rows(count)("ByteRange")) - 1
							Catch ex As System.IO.FileNotFoundException
								WriteWarning(ex.Message)
							Catch ex As System.InvalidCastException
								Me.WriteWarning(String.Format("Could not retrieve full text for document #{0}", count + 1))
							End Try
						End If
					End If
					volumeLog.Append(BuildIproLog(CType(documentImagesTable.Rows(count)("BatesNumber"), String), currentVolume, currentDirectory, isFirstDocumentImage))
			End Select
		End Sub

#Region "Private Helper Functions"

		Private Function ExtractFullTextFromGuid(ByVal fullTextGuid As String) As String
			Dim bodyText As String
			If fullTextGuid Is Nothing Then
				bodyText = String.Empty
			Else
				bodyText = _fullTextDownloader.ReadFullTextFile(_sourceDirectory & fullTextGuid)
				'bodyText = bodyText.Replace(System.Environment.NewLine, ChrW(10).ToString)
				'bodyText = bodyText.Replace(ChrW(13), ChrW(10))
				bodyText = bodyText.Replace(ChrW(10), "®"c)
			End If
			Return bodyText
		End Function

		Private Function ExportArgs(ByVal line As String, ByVal eventType As kCura.Windows.Process.EventType) As kCura.WinEDDS.ExportEventArgs
			Return New kCura.WinEDDS.ExportEventArgs(Me.DocumentsExported, Me.TotalImages, line, eventType)
		End Function

		Private Sub ExportDocumentImage(ByVal fileName As String, ByVal fileGuid As String, ByVal artifactID As Int32, ByVal batesNumber As String)		'ByVal fileURI As String, ByVal batesNumber As String)
			If System.IO.File.Exists(fileName) Then
				If Me.Overwrite Then
					System.IO.File.Delete(fileName)
					Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Overwriting document {0}.tif.", batesNumber))
					_downloadManager.DownloadFile(fileName, fileGuid, artifactID)
				Else
					Me.WriteWarning(String.Format("{0}.tif already exists. Skipping file export.", batesNumber))
				End If
			Else
				Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Now exporting document {0}.tif.", batesNumber))
				_downloadManager.DownloadFile(fileName, fileGuid, artifactID)
			End If
			Me.DocumentsExported += 1
			Me.WriteUpdate(String.Format("Finished exporting document {0}.tif.", batesNumber))
		End Sub

		Private Function ExportNativeFile(ByVal nativesDirectory As String, ByVal currentVolume As String, ByVal documentArtifactID As Int32, ByVal nativetable As System.Data.DataTable) As String
			Dim nativeView As New kCura.Data.DataView(nativetable)
			Dim exportFilePath, bates, fileName, fileGuid As String
			Dim values As String()
			Dim flag As Boolean = False

			nativeView.RowFilter = String.Format("documentArtifactID = {0}", documentArtifactID)
			bates = CType(nativeView(0)("TextIdentifier"), String)
			fileName = bates & "_" & CType(nativeView(0)("Filename"), String)
			fileGuid = CType(nativeView(0)("Guid"), String)

			exportFilePath = String.Format("{1}{0}{2}{0}{3}{0}{4}", "\", Me.ExportFile.FolderPath, currentVolume, nativesDirectory, fileName)

			If System.IO.File.Exists(exportFilePath) Then
				If Me.ExportFile.Overwrite Then
					System.IO.File.Delete(exportFilePath)
					Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Overwriting document {0}.", exportFilePath))
					_downloadHandler.DownloadFile(exportFilePath, fileGuid, documentArtifactID)
					flag = True
				Else
					Me.WriteWarning(String.Format("{0} already exists. Skipping file export.", exportFilePath))
				End If
			Else
				Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Now exporting document {0}.", exportFilePath))
				_downloadHandler.DownloadFile(exportFilePath, fileGuid, documentArtifactID)
				flag = True
			End If
			Me.WriteUpdate(String.Format("Finished exporting document {0}.", exportFilePath))
			If flag Then
				values = New String() {bates, bates, exportFilePath}
				Return BuildNativesLog(values)
			Else
				Return ""
			End If
		End Function

		Private Function CreateVolumeDirectory(ByVal volumePrefix As String, ByVal volumeNumber As Integer) As String
			Dim currentVolume As String

			currentVolume = String.Format("{0}{1}", volumePrefix, ZeroFillNumber(volumeNumber, 3))
			If Not System.IO.Directory.Exists(String.Format("{0}{1}", Me.FolderPath, currentVolume)) Then
				System.IO.Directory.CreateDirectory(String.Format("{0}{1}", Me.FolderPath, currentVolume))
				Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Created volume {0}.", currentVolume))
			End If
			Return currentVolume
		End Function

		Private Function CreateSubdirectory(ByVal subdirectoryPrefix As String, ByVal subdirectoryNumber As Integer, ByVal currentVolume As String) As String
			Dim currentDirectory As String

			currentDirectory = String.Format("{0}{1}", subdirectoryPrefix, ZeroFillNumber(subdirectoryNumber, 3))
			If Not System.IO.Directory.Exists(String.Format("{0}{1}\{2}", Me.FolderPath, currentVolume, currentDirectory)) Then
				System.IO.Directory.CreateDirectory(String.Format("{0}{1}\{2}", Me.FolderPath, currentVolume, currentDirectory))
				Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Created subdirectory {0}.", currentDirectory))
			End If
			Return currentDirectory
		End Function

		Private Function DeleteSubdirectory(ByVal subdirectoryPrefix As String, ByVal subdirectoryNumber As Integer, ByVal currentVolume As String) As String
			Dim currentDirectory As String

			currentDirectory = String.Format("{0}{1}", subdirectoryPrefix, ZeroFillNumber(subdirectoryNumber, 3))
			If System.IO.Directory.Exists(String.Format("{0}{1}\{2}", Me.FolderPath, currentVolume, currentDirectory)) Then
				System.IO.Directory.Delete(String.Format("{0}{1}\{2}", Me.FolderPath, currentVolume, currentDirectory))
			End If
			Return currentDirectory
		End Function

		Private Function ZeroFillNumber(ByVal value As Int32, ByVal length As Int32) As String
			Dim lengthOfValue As Int32 = CType(System.Math.Floor(System.Math.Log10(CType(value, Double))), Int32) + 1
			Dim retVal As New System.Text.StringBuilder
			retVal.Append("0"c, length - lengthOfValue)
			Return retVal.ToString & value.ToString
		End Function

		Private Function CurrentDocumentFilesSize(ByVal dataTable As System.Data.DataTable, ByVal nativesTable As System.Data.DataTable, ByVal initialPosition As Int32, ByVal productionArtifactID As Int32) As Long
			Dim documentArtifactID As Int32
			Dim count As Int32 = initialPosition
			Dim size As Long = 0

			documentArtifactID = CType(dataTable.Rows(count)("DocumentArtifactID"), Int32)
			If Me.ExportFile.ExportNative And nativesTable.Rows.Count > 0 Then
				size = size + Me.CurrentNativesSize(nativesTable, CType(dataTable.Rows(count)("documentArtifactID"), Int32))
			End If
			While count < dataTable.Rows.Count AndAlso CType(dataTable.Rows(count)("documentArtifactID"), Int32) = documentArtifactID
				size = size + CType(dataTable.Rows(count)("ImageSize"), Long)
				count = count + 1
				If count = dataTable.Rows.Count Then
					Exit While
				End If
			End While
			Return size
		End Function

		Private Function CurrentNativesSize(ByVal nativesTable As System.Data.DataTable, ByVal documentArtifactID As Int32) As Long
			Dim nativesView As New kCura.Data.DataView(nativesTable)
			nativesView.RowFilter = String.Format("DocumentArtifactID = {0}", documentArtifactID)
			Return _fileManager.RetrieveNativeFileSize(_sourceDirectory, CType(nativesView.Item(0)("Guid"), String))
		End Function

		Private Function CurrentDocumentImageCount(ByVal dataTable As System.Data.DataTable, ByVal initialPosition As Int32) As Int32
			Dim documentArtifactID As Int32
			Dim count As Int32 = initialPosition

			documentArtifactID = CType(dataTable.Rows(count)("DocumentArtifactID"), Int32)
			While count < dataTable.Rows.Count AndAlso CType(dataTable.Rows(count)("documentArtifactID"), Int32) = documentArtifactID
				count = count + 1
			End While
			Return count - initialPosition
		End Function

		Private Function HasNative(ByVal documentArtifactID As Int32, ByVal nativestable As System.Data.DataTable) As Boolean
			Dim view As New kCura.Data.DataView(nativestable)
			view.RowFilter = String.Format("documentArtifactID = {0}", documentArtifactID)
			If view.Count > 0 Then
				Return True
			Else
				Return False
			End If
		End Function

		Private Function BuildVolumeLog(ByVal batesNumber As String, ByVal currentVolume As String, ByVal copyFile As String, ByVal firstDocument As Boolean) As String
			Dim log As New System.Text.StringBuilder

			log.AppendFormat("{0},{1},{2},", batesNumber, currentVolume, copyFile)
			If firstDocument Then
				log.Append("Y")
			End If
			log.AppendFormat(",,,{0}", Microsoft.VisualBasic.ControlChars.NewLine)
			Return log.ToString
		End Function

		Private Function BuildIproLog(ByVal batesNumber As String, ByVal currentVolume As String, ByVal pathToImage As String, ByVal firstDocument As Boolean) As String
			Dim log As New System.Text.StringBuilder

			log.AppendFormat("IM,{0},", batesNumber)
			If firstDocument Then
				log.Append("D,")
			Else
				log.Append(" ,")
			End If
			log.AppendFormat("0,{0};{1};{2}.tif;2", currentVolume, pathToImage, batesNumber)
			log.AppendFormat("{0}", Microsoft.VisualBasic.ControlChars.NewLine)
			Return log.ToString
		End Function

		Private Function BuildNativesLog(ByVal values As String()) As String
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

		Private Sub CreateVolumeLogFile(ByVal currentVolume As String, ByVal productionName As String, ByVal volumeLog As String, ByVal format As String)
			Dim writer As System.IO.StreamWriter
			Dim volumeFile As String

			volumeFile = String.Format("{0}{1}\{2}_{3}.{4}", Me.FolderPath, currentVolume, productionName, currentVolume, format)
			If System.IO.File.Exists(volumeFile) Then
				Me.WriteWarning(String.Format("Image log file '{0}' already exists, overwriting file.", volumeFile))
				System.IO.File.Delete(volumeFile)
			End If
			writer = System.IO.File.CreateText(volumeFile)
			writer.Write(volumeLog)
			writer.Close()
			Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Created image log file for volume {0}.", currentVolume))
		End Sub

		Private Sub DeleteVolumeLogFile(ByVal currentVolume As String, ByVal productionName As String, ByVal volumeLog As String, ByVal format As String)
			Dim writer As System.IO.StreamWriter
			Dim volumeFile As String

			volumeFile = String.Format("{0}{1}\{2}_{3}.{4}", Me.FolderPath, currentVolume, productionName, currentVolume, format)
			If System.IO.File.Exists(volumeFile) Then
				System.IO.File.Delete(volumeFile)
			End If
		End Sub

		Private Sub CreateFullTextFile(ByVal currentBates As String, ByVal productionName As String, ByVal volumeLog As String)
			Dim writer As System.IO.StreamWriter
			Dim fullTextFile As String

			fullTextFile = String.Format("{0}.txt", currentBates)
			If System.IO.File.Exists(fullTextFile) Then
				Me.WriteWarning(String.Format("Full text file '{0}'.txt already exists, overwriting file.", currentBates))
				System.IO.File.Delete(fullTextFile)
			End If
			writer = System.IO.File.CreateText(fullTextFile)
			writer.Write(volumeLog)
			writer.Close()
		End Sub
#End Region

		Private Sub _processController_HaltProcessEvent(ByVal processID As System.Guid) Handles _processController.HaltProcessEvent
			_continue = False
		End Sub
	End Class
End Namespace