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
		Public TotalFiles As Int32
		Private _continue As Boolean
		Private _sourceDirectory As String
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
		End Property

		Public ReadOnly Property FolderPath() As String
			Get
				Return Me.ExportFile.FolderPath + "\"
			End Get
		End Property

		Public ReadOnly Property Overwrite() As Boolean
			Get
				Return Me.ExportFile.Overwrite
			End Get
		End Property

		Public ReadOnly Property LoadFileFormat() As kCura.WinEDDS.LoadFileType.FileFormat
			Get
				Return Me.ExportFile.LogFileFormat
			End Get
		End Property

		Public ReadOnly Property SelectedCaseInfo() As kCura.EDDS.Types.CaseInfo
			Get
				Return Me.ExportFile.CaseInfo
			End Get
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
			RaiseEvent StatusMessage(New ExportEventArgs(Me.DocumentsExported, Me.TotalFiles, line, e))
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

			Me.ExportFile = exportFile
			Me.WebClient = New System.Net.WebClient
			Me.WebClient.Credentials = cred
			_processController = processController
			_continue = True

			_downloadHandler = New FileDownloader(exportFile.Credential, exportFile.CaseInfo.DocumentPath & "\EDDS" & exportFile.CaseInfo.ArtifactID, exportFile.CaseInfo.DownloadHandlerURL, exportFile.CookieContainer, Not kCura.WinEDDS.Service.Settings.WindowsAuthentication)
			_productionManager = New kCura.WinEDDS.Service.ProductionManager(cred, cookieContainer)
			_folderManager = New kCura.WinEDDS.Service.FolderManager(cred, cookieContainer)
			_documentManager = New kCura.WinEDDS.Service.DocumentManager(cred, cookieContainer)
			_downloadManager = New FileDownloader(cred, destinationFolderPath, exportFile.CaseInfo.DownloadHandlerURL, cookieContainer, Not kCura.WinEDDS.Service.Settings.WindowsAuthentication)
			_fileManager = New kCura.WinEDDS.Service.FileManager(cred, cookieContainer)
			_folderManager = New kCura.WinEDDS.Service.FolderManager(cred, cookieContainer)
			_fileManager = New kCura.WinEDDS.Service.FileManager(cred, cookieContainer)
			_fullTextDownloader = New kCura.WinEDDS.FullTextManager(cred, _sourceDirectory, cookieContainer)
			_processController = processController
			_sourceDirectory = _documentManager.GetDocumentDirectoryByCaseArtifactID(Me.SelectedCaseInfo.ArtifactID)
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
			Dim documentImagesTable As System.Data.DataTable
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
			Dim currentDocumentHasImage As Boolean
			Dim currentPageFirstByteNumber As Int32
			'Reads my loadfile format and determines the extension for the load file.
			If Me.LoadFileFormat = kCura.WinEDDS.LoadFileType.FileFormat.Concordance Then
				fileFormat = "log"
			Else
				fileFormat = "lfp"
			End If
			Me.WriteUpdate("Retrieving export data from the server...")

			documentImagesTable = _fileManager.RetrieveByProductionArtifactIDForProduction(Me.SelectedCaseInfo.ArtifactID, Me.ProductionArtifactID).Tables(0)

			production = _productionManager.Read(Me.SelectedCaseInfo.ArtifactID, Me.ProductionArtifactID)
			currentVolumeNumber = production.VolumeStartNumber
			Me.BeginVolume(currentVolumeName, production.VolumePrefix, currentVolumeNumber, production.SubdirectoryStartNumber, imagesSubdirectoryCount, nativesSubDirectoryCount, volumeLog, fullTextVolumeLog, nativesVolumeLog)
			currentImagesDirectoryName = Me.CreateSubdirectory(production.SubdirectoryPrefix, production.SubdirectoryStartNumber, currentVolumeName)
			currentNativeDirectoryName = Me.CreateSubdirectory("NATIVES", production.SubdirectoryStartNumber, currentVolumeName)

			Me.TotalFiles = GetTotalFiles(documentImagesTable)
			Me.WriteUpdate("Data retrieved. Beginning production export...")

			For iterator = 0 To documentImagesTable.Rows.Count - 1
				isFirstDocumentImage = (iterator = 0 OrElse CType(documentImagesTable.Rows(iterator - 1)("DocumentArtifactID"), Int32) <> CType(documentImagesTable.Rows(iterator)("DocumentArtifactID"), Int32))
				currentDocumentHasImage = Me.HasImage(documentImagesTable.Rows(iterator))
				If isFirstDocumentImage OrElse Not currentDocumentHasImage Then
					currentDocumentFilesSize = Me.CurrentDocumentFilesSize(documentImagesTable, iterator, Me.ProductionArtifactID)
					currentDocumentImagesCount = Me.CurrentDocumentImageCount(documentImagesTable, iterator)
					currentDocumentHasNative = Me.HasNative(documentImagesTable.Rows(iterator))
					If volumeSize + currentDocumentFilesSize > CType(production.VolumeMaxSize, Int64) * 1024 * 1024 Then
						If iterator > 0 Then
							Me.CompleteVolume(currentVolumeName, production.Name, volumeLog, fullTextVolumeLog, nativesVolumeLog, fileFormat)
						End If
						volumeSize = 0
						currentVolumeNumber = currentVolumeNumber + 1
						Me.BeginVolume(currentVolumeName, production.VolumePrefix, currentVolumeNumber, production.SubdirectoryStartNumber - 1, imagesSubdirectoryCount, nativesSubDirectoryCount, volumeLog, fullTextVolumeLog, nativesVolumeLog)
					End If
					If iterator > 0 Then
						Dim createNewNativeSubdirectory As Boolean = Me.ExportFile.ExportNative AndAlso currentDocumentHasNative AndAlso currentNativesSubdirectoryFileCount + 1 > production.SubdirectoryMaxFiles OrElse volumeSize = 0
						Dim createNewImagesSubdirectory As Boolean = (currentImagesSubdirectoryFileCount + currentDocumentImagesCount) > production.SubdirectoryMaxFiles AndAlso currentDocumentHasImage OrElse volumeSize = 0
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
					If currentDocumentHasImage Then
						fileName = String.Format("{0}{1}\{2}\{3}.tif", Me.FolderPath, currentVolumeName, currentImagesDirectoryName, CType(documentImagesTable.Rows(iterator)("BatesNumber"), String))
						ExportDocumentImage(fileName, CType(documentImagesTable.Rows(iterator)("ImageGuid"), String), CType(documentImagesTable.Rows(iterator)("DocumentArtifactID"), Int32), CType(documentImagesTable.Rows(iterator)("BatesNumber"), String))
						currentImagesSubdirectoryFileCount += 1
						Me.CreateLoadFileEntries(volumeLog, fullTextVolumeLog, documentImagesTable, iterator, currentVolumeName, fileName, isFirstDocumentImage, currentPageFirstByteNumber, currentImagesDirectoryName)
					End If
					If Me.ExportFile.ExportNative AndAlso currentDocumentHasNative Then
						nativesVolumeLog.Append(Me.ExportNativeFile(currentNativeDirectoryName, currentVolumeName, documentImagesTable.Rows(iterator)))
						currentNativesSubdirectoryFileCount += 1
					End If
					'Me.CreateLoadFileEntries(volumeLog, fullTextVolumeLog, documentImagesTable, iterator, currentVolumeName, fileName, isFirstDocumentImage, currentPageFirstByteNumber, currentNativeDirectoryName)
				Catch ex As System.Exception
					Me.WriteError(String.Format("Error occurred on document #{0} with message: {1}", iterator + 1, ex.Message))
				End Try
				If Not _continue Then
					Exit For
				End If
				If isFirstDocumentImage OrElse Not currentDocumentHasImage Then
					volumeSize = volumeSize + currentDocumentFilesSize
				End If
			Next
			Me.CompleteVolume(currentVolumeName, production.Name, volumeLog, fullTextVolumeLog, nativesVolumeLog, fileFormat)
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
				Me.CreateVolumeLogFile(currentVolumeName, productionName + "_FullText_", fullTextVolumeLog.ToString, loadFileExtension)
			End If
			If Me.ExportFile.ExportNative AndAlso nativesVolumeLog.ToString.Length > 0 Then
				Dim fields As String() = {"BEGBATES", "ENDBATES", "FILE_PATH"}
				nativesVolumeLog.Insert(0, Me.BuildNativesLog(fields))
				Me.CreateVolumeLogFile(currentVolumeName, productionName & "_NATIVES", nativesVolumeLog.ToString, "log")
			End If
			Me.DeleteEmptyFolders(currentVolumeName)
		End Sub

		Private Sub CreateLoadFileEntries(ByVal volumeLog As System.Text.StringBuilder, ByVal fullTextVolumeLog As System.Text.StringBuilder, ByVal documentImagesTable As System.Data.DataTable, ByVal count As Int32, ByVal currentVolume As String, ByVal filename As String, ByVal isFirstDocumentImage As Boolean, ByRef currentPageFirstByteNumber As Int32, ByVal currentDirectory As String)
			Dim fullTextGuid As String
			Dim fullText As String
			Dim pageText As String
			'Dim currentPage As Int32 = count
			Select Case Me.LoadFileFormat
				Case LoadFileType.FileFormat.Concordance
					volumeLog.Append(BuildVolumeLog(CType(documentImagesTable.Rows(count)("BatesNumber"), String), currentVolume, filename, isFirstDocumentImage))
				Case LoadFileType.FileFormat.IPRO, LoadFileType.FileFormat.IPRO_FullText
					If Me.LoadFileFormat = kCura.WinEDDS.LoadFileType.FileFormat.IPRO_FullText Then
						If isFirstDocumentImage Then
							currentPageFirstByteNumber = 0
						End If
						Try
							fullTextGuid = _fileManager.GetFullTextGuidsByDocumentArtifactIdAndType(Me.SelectedCaseInfo.ArtifactID, CType(documentImagesTable.Rows(count)("DocumentArtifactID"), Int32), 2)
							If fullTextGuid = "" Then
								fullText = ""
								Me.WriteWarning(String.Format("Could not retrieve full text for document #{0}", count + 1))
							Else
								fullText = _fullTextDownloader.ReadFullTextFile(_sourceDirectory & "\" & fullTextGuid)
							End If
							pageText = fullText.Substring(currentPageFirstByteNumber, CInt(documentImagesTable.Rows(count)("ByteRange")))
							pageText = pageText.Replace(ChrW(10), " ")
							pageText = pageText.Replace(",", "")
							pageText = pageText.Replace(" ", "|0|0|0|0^")
							fullTextVolumeLog.AppendFormat("FT,{0},1,1,{1}", CType(documentImagesTable.Rows(count)("BatesNumber"), String), pageText)
							fullTextVolumeLog.AppendFormat("{0}", Microsoft.VisualBasic.ControlChars.NewLine)
							currentPageFirstByteNumber += CInt(documentImagesTable.Rows(count)("ByteRange"))
						Catch ex As System.InvalidCastException
							Me.WriteWarning(String.Format("Could not retrieve full text for document #{0}", count + 1))
						Catch ex As System.IO.FileNotFoundException
							WriteWarning(ex.Message)
						End Try
					End If
					volumeLog.Append(BuildIproLog(CType(documentImagesTable.Rows(count)("BatesNumber"), String), currentVolume, currentDirectory, isFirstDocumentImage))
			End Select
		End Sub

#Region "Private Helper Functions"
		Private Function ExportArgs(ByVal line As String, ByVal eventType As kCura.Windows.Process.EventType) As kCura.WinEDDS.ExportEventArgs
			Return New kCura.WinEDDS.ExportEventArgs(Me.DocumentsExported, Me.TotalFiles, line, eventType)
		End Function

		Private Sub ExportDocumentImage(ByVal fileName As String, ByVal fileGuid As String, ByVal artifactID As Int32, ByVal batesNumber As String)
			If System.IO.File.Exists(fileName) Then
				If Me.Overwrite Then
					System.IO.File.Delete(fileName)
					Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Overwriting document {0}.tif.", batesNumber))
					_downloadManager.DownloadFile(fileName, fileGuid, artifactID, Me.ExportFile.CaseArtifactID.ToString)
				Else
					Me.WriteWarning(String.Format("{0}.tif already exists. Skipping file export.", batesNumber))
				End If
			Else
				Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Now exporting document {0}.tif.", batesNumber))
				_downloadManager.DownloadFile(fileName, fileGuid, artifactID, Me.ExportFile.CaseArtifactID.ToString)
			End If
			Me.DocumentsExported += 1
			Me.WriteUpdate(String.Format("Finished exporting document {0}.tif.", batesNumber))
		End Sub

		Private Function ExportNativeFile(ByVal nativesDirectory As String, ByVal currentVolume As String, ByVal row As System.Data.DataRow) As String
			Dim exportFilePath, bates, fileName, fileGuid As String
			Dim values As String()
			Dim documentWasExported As Boolean = False
			Dim documentArtifactID As Int32 = CType(row("DocumentArtifactID"), Int32)
			bates = CType(row("NativeIdentifier"), String)
			fileName = bates & "_" & CType(row("NativeFileName"), String)
			fileGuid = CType(row("NativeGuid"), String)

			exportFilePath = String.Format("{1}{0}{2}{0}{3}{0}{4}", "\", Me.ExportFile.FolderPath, currentVolume, nativesDirectory, fileName)

			If System.IO.File.Exists(exportFilePath) Then
				If Me.ExportFile.Overwrite Then
					System.IO.File.Delete(exportFilePath)
					Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Overwriting file {0}.", exportFilePath))
					_downloadHandler.DownloadFile(exportFilePath, fileGuid, documentArtifactID, Me.ExportFile.CaseArtifactID.ToString)
					documentWasExported = True
				Else
					Me.WriteWarning(String.Format("{0} already exists. Skipping file export.", exportFilePath))
				End If
			Else
				Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Now exporting file {0}.", exportFilePath))
				_downloadHandler.DownloadFile(exportFilePath, fileGuid, documentArtifactID, Me.ExportFile.CaseArtifactID.ToString)
				documentWasExported = True
			End If
			Me.WriteUpdate(String.Format("Finished exporting file {0}.", exportFilePath))
			If documentWasExported Then
				If Not HasImage(row) Then
					Me.DocumentsExported += 1
				End If
				values = New String() {bates, bates, exportFilePath}
				Return BuildNativesLog(values)
			Else
				Return ""
			End If
		End Function

		Private Sub DeleteEmptyFolders(ByVal currentVolume As String)
			Dim volumePath As String
			Dim subdirectoryPath As String
			Dim directoryIO As System.IO.Directory
			Dim i As Int32
			Dim subdirectories As String()

			volumePath = String.Format("{0}\{1}", Me.ExportFile.FolderPath, currentVolume)
			subdirectories = directoryIO.GetDirectories(volumePath)
			For i = 0 To subdirectories.Length - 1
				If directoryIO.GetFiles(subdirectories(i)).Length = 0 Then
					directoryIO.Delete(subdirectories(i))
				End If
			Next
		End Sub

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

		Private Function CurrentDocumentFilesSize(ByVal dataTable As System.Data.DataTable, ByVal initialPosition As Int32, ByVal productionArtifactID As Int32) As Long
			Dim documentArtifactID As Int32
			Dim count As Int32 = initialPosition
			Dim imagesSize As Long = 0
			Dim nativeSize As Long = 0

			documentArtifactID = CType(dataTable.Rows(count)("DocumentArtifactID"), Int32)

			While count < dataTable.Rows.Count AndAlso CType(dataTable.Rows(count)("documentArtifactID"), Int32) = documentArtifactID
				If Me.ExportFile.ExportNative AndAlso HasNative(dataTable.Rows(count)) AndAlso nativeSize = 0 Then
					nativeSize += _fileManager.RetrieveNativeFileSize(_sourceDirectory, CType(dataTable.Rows(count)("NativeGuid"), String))
				End If
				If HasImage(dataTable.Rows(count)) Then
					imagesSize = imagesSize + CType(dataTable.Rows(count)("ImageSize"), Long)
				End If
				count = count + 1
				If count = dataTable.Rows.Count Then
					Exit While
				End If
			End While
			Return imagesSize + nativeSize
		End Function

		Private Function CurrentDocumentImageCount(ByVal dataTable As System.Data.DataTable, ByVal initialPosition As Int32) As Int32
			Dim documentArtifactID As Int32
			Dim count As Int32 = initialPosition

			documentArtifactID = CType(dataTable.Rows(count)("DocumentArtifactID"), Int32)
			While count < dataTable.Rows.Count AndAlso CType(dataTable.Rows(count)("documentArtifactID"), Int32) = documentArtifactID AndAlso Me.HasImage(dataTable.Rows(count))
				count = count + 1
			End While
			Return count - initialPosition
		End Function

		Private Function HasNative(ByVal row As System.Data.DataRow) As Boolean
			If row("NativeGuid") Is System.DBNull.Value Then
				Return False
			Else
				Return True
			End If
		End Function

		Private Function HasImage(ByVal row As System.data.DataRow) As Boolean
			If row("ImageGuid") Is System.DBNull.Value Then
				Return False
			Else
				Return True
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
			Dim volumeFilePath As String
			Dim volumeFileName As String

			volumeFileName = String.Format("{0}_{1}.{2}", productionName, currentVolume, format)
			volumeFileName = System.Text.RegularExpressions.Regex.Replace(volumeFileName, "[\\\/:|*<>\?""]+", "_")
			volumeFilePath = String.Format("{0}{1}\{2}", Me.FolderPath, currentVolume, volumeFileName)
			If volumeLog.Length > 0 Then
				If System.IO.File.Exists(volumeFilePath) Then
					Me.WriteWarning(String.Format("Log file '{0}' already exists, overwriting file.", volumeFilePath))
					System.IO.File.Delete(volumeFilePath)
				End If
				writer = System.IO.File.CreateText(volumeFilePath)
				writer.Write(volumeLog)
				writer.Close()
				Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Created log file {0} for volume {1}.", volumeFileName, currentVolume))
			Else
				If System.IO.File.Exists(volumeFilePath) Then
					System.IO.File.Delete(volumeFilePath)
				End If
			End If
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

		Private Function GetTotalFiles(ByVal documentsTable As System.Data.DataTable) As Int32
			Dim i As Int32
			Dim fileCount As Int32 = 0
			For i = 0 To documentsTable.Rows.Count - 1
				If Me.HasImage(documentsTable.Rows(i)) Then
					fileCount += 1
				ElseIf Me.HasNative(documentsTable.Rows(i)) Then
					fileCount += 1
				End If
			Next
			Return fileCount
		End Function

#End Region

		Private Sub _processController_HaltProcessEvent(ByVal processID As System.Guid) Handles _processController.HaltProcessEvent
			_continue = False
		End Sub
	End Class
End Namespace