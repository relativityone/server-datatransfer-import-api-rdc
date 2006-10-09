Imports System.IO
Namespace kCura.WinEDDS
	Public Class ProductionExporter

#Region "Members"
		Private _productionManager As kCura.WinEDDS.Service.ProductionManager
		Private _fileManager As kCura.WinEDDS.Service.FileManager
		Private _downloadManager As kCura.WinEDDS.FileDownloader
		Private _fullTextDownloader As kCura.WinEDDS.FullTextManager
		Private _documentManager As kCura.WinEDDS.Service.DocumentManager
		Private _folderManager As kCura.WinEDDS.Service.FolderManager

		Private WithEvents _processController As kCura.Windows.Process.Controller
		Private _webClient As System.Net.WebClient
		Private _folderList As kCura.WinEDDS.FolderList
		Private _exportFile As kCura.WinEDDS.ExportFile

		Public DocumentsExported As Int32
		Public TotalDocuments As Int32
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
			Dim cred As Net.NetworkCredential = exportFile.Credential
			Dim cookieContainer As System.Net.CookieContainer = exportFile.CookieContainer
			Dim identity As kCura.EDDS.EDDSIdentity = exportFile.Identity
			Dim destinationFolderPath As String = exportFile.CaseInfo.DocumentPath & "\EDDS" & exportFile.CaseInfo.ArtifactID

			Me.DocumentsExported = 0
			Me.TotalDocuments = 1

			Me.ExportFile = exportFile
			Me.WebClient = New System.Net.WebClient
			Me.WebClient.Credentials = cred
			_processController = processController
			_continue = True

			_productionManager = New kCura.WinEDDS.Service.ProductionManager(cred, cookieContainer, identity)
			_folderManager = New kCura.WinEDDS.Service.FolderManager(cred, cookieContainer, identity)
			_documentManager = New kCura.WinEDDS.Service.DocumentManager(cred, cookieContainer, identity)
			_downloadManager = New FileDownloader(cred, destinationFolderPath, exportFile.CaseInfo.DownloadHandlerURL, cookieContainer)
			_fileManager = New kCura.WinEDDS.Service.FileManager(cred, cookieContainer, identity)
			_folderManager = New kCura.WinEDDS.Service.FolderManager(cred, cookieContainer, identity)
			_fileManager = New kCura.WinEDDS.Service.FileManager(cred, cookieContainer, identity)
			_fullTextDownloader = New kCura.WinEDDS.FullTextManager(cred, _sourceDirectory, cookieContainer)

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
			Dim documentTable, guidTable As System.Data.DataTable
			Dim fileName, currentVolume, currentDirectory, fileFormat, fulltextguid, fullText, pageText As String
			Dim volumeLog As New System.Text.StringBuilder
			Dim fullTextVolumeLog As New System.Text.StringBuilder
			Dim fullTextLog As New System.Text.StringBuilder
			Dim nextVolume, nextDirectory, documentCount, count, startindex As Int32
			Dim volumeSize As Long = 0
			Dim documentSize As Long
			Dim subdirectoryCount As Int32 = 0
			Dim currentBates As String = ""
			Dim isFirstDocument As Boolean

			If Me.LoadFileFormat = kCura.WinEDDS.LoadFileType.FileFormat.Concordance Then
				fileFormat = "log"
			Else
				fileFormat = "lfp"
			End If

			Me.WriteUpdate("Retrieving export data from the server...")
			documentTable = _fileManager.RetrieveByProductionArtifactIDForProduction(Me.ProductionArtifactID).Tables(0)

			Me.TotalDocuments = documentTable.Rows.Count()
			Me.WriteUpdate("Data retrieved. Beginning production export...")

			production = _productionManager.Read(Me.ProductionArtifactID)

			currentVolume = Me.CreateVolumeDirectory(production.VolumePrefix, production.VolumeStartNumber)
			currentDirectory = Me.CreateSubdirectory(production.SubdirectoryPrefix, production.SubdirectoryStartNumber, currentVolume)
			nextVolume = production.VolumeStartNumber + 1
			nextDirectory = production.SubdirectoryStartNumber + 1

			documentSize = Me.CurrentDocumentSize(documentTable, 0, Me.ProductionArtifactID)
			documentCount = Me.CurrentDocumentCount(documentTable, 0)

			For count = 0 To documentTable.Rows.Count - 1
				isFirstDocument = (count = 0 OrElse CType(documentTable.Rows(count - 1)("DocumentArtifactID"), Int32) <> CType(documentTable.Rows(count)("DocumentArtifactID"), Int32))
				Try
					If Not isFirstDocument Then
						volumeSize = volumeSize + documentSize
						documentSize = Me.CurrentDocumentSize(documentTable, count, Me.ProductionArtifactID)
						If volumeSize + documentSize > production.VolumeMaxSize * 1024 * 1024 Then
							Me.CreateVolumeLogFile(currentVolume, production.Name, volumeLog.ToString, fileFormat)
							If Me.LoadFileFormat = kCura.WinEDDS.LoadFileType.FileFormat.IPRO_FullText Then
								Me.CreateVolumeLogFile(currentVolume, production.Name + "_FT_", fullTextVolumeLog.ToString, fileFormat)
								fullTextVolumeLog = New System.Text.StringBuilder
							End If
							volumeLog = New System.Text.StringBuilder
							currentVolume = Me.CreateVolumeDirectory(production.VolumePrefix, nextVolume)
							nextVolume = nextVolume + 1
							volumeSize = 0
						End If
						subdirectoryCount = subdirectoryCount + documentCount
						documentCount = Me.CurrentDocumentCount(documentTable, count)
						If subdirectoryCount + documentCount > production.SubdirectoryMaxFiles OrElse volumeSize = 0 Then
							currentDirectory = Me.CreateSubdirectory(production.SubdirectoryPrefix, nextDirectory, currentVolume)
							nextDirectory = nextDirectory + 1
							subdirectoryCount = 0
						End If
					End If

					fileName = String.Format("{0}{1}\{2}\{3}.tif", Me.FolderPath, currentVolume, currentDirectory, CType(documentTable.Rows(count)("BatesNumber"), String))
					ExportDocument(fileName, CType(documentTable.Rows(count)("ImageGuid"), String), CType(documentTable.Rows(count)("DocumentArtifactID"), Int32), CType(documentTable.Rows(count)("BatesNumber"), String))

					If Me.LoadFileFormat = kCura.WinEDDS.LoadFileType.FileFormat.Concordance Then
						volumeLog.Append(BuildVolumeLog(CType(documentTable.Rows(count)("BatesNumber"), String), currentVolume, fileName, isFirstDocument))
					ElseIf Me.LoadFileFormat = kCura.WinEDDS.LoadFileType.FileFormat.IPRO_FullText Then
						If isFirstDocument Then
							startindex = 0
							fulltextguid = _fileManager.GetFullTextGuidsByDocumentArtifactIdAndType(CType(documentTable.Rows(count)("documentArtifactID"), Int32), 2)
							Try
								fullText = _fullTextDownloader.ReadFullTextFile(_sourceDirectory & fulltextguid)
								pageText = fullText.Substring(startindex, CInt(documentTable.Rows(count)("ByteRange")) - 1)
								pageText = pageText.Replace(ChrW(10), " ")
								pageText = pageText.Replace(",", "")
								pageText = pageText.Replace(" ", "|0|0|0|0^")
								fullTextVolumeLog.AppendFormat("FT,{0},1,1,{1}", CType(documentTable.Rows(count)("BatesNumber"), String), pageText)
								fullTextVolumeLog.AppendFormat("{0}", Microsoft.VisualBasic.ControlChars.NewLine)
								startindex += CInt(documentTable.Rows(count)("ByteRange")) - 1
							Catch ex As System.IO.FileNotFoundException
								WriteWarning(ex.Message)
							Catch ex As System.InvalidCastException
								Me.WriteWarning(String.Format("Could not retrieve full text for document #{0}", count + 1))
							End Try
						End If
					End If
					If Not Me.LoadFileFormat = kCura.WinEDDS.LoadFileType.FileFormat.Concordance Then
						volumeLog.Append(BuildIproLog(CType(documentTable.Rows(count)("BatesNumber"), String), currentVolume, currentDirectory, isFirstDocument))
					End If

				Catch ex As System.Exception
					Me.WriteError(String.Format("Error occurred on document #{0} with message: {1}", count + 1, ex.Message))
				End Try
				If Not _continue Then
					Exit For
				End If
			Next

			Me.CreateVolumeLogFile(currentVolume, production.Name, volumeLog.ToString, fileFormat)
			If Me.LoadFileFormat = kCura.WinEDDS.LoadFileType.FileFormat.IPRO_FullText Then
				Me.CreateVolumeLogFile(currentVolume, production.Name & "_FT", fullTextVolumeLog.ToString, fileFormat)
			End If

			If Me.ExportFile.ExportNative Then
				Me.ExportFile.FolderPath = String.Format("{0}{1}", Me.FolderPath, currentVolume)
				Try
					Dim nativesExporter As New kCura.WinEDDS.NativeFileExporter(Me.ExportFile, _processController, production.VolumeStartNumber, production.Name)
					nativesExporter.ExportNatives()
				Catch ex As System.Exception
					WriteError(String.Format("Could not export Natives: {0}", ex.Message.ToString))
				End Try
			End If
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
			Return New kCura.WinEDDS.ExportEventArgs(Me.DocumentsExported, Me.TotalDocuments, line, eventType)
		End Function

		Private Sub ExportDocument(ByVal fileName As String, ByVal fileGuid As String, ByVal artifactID As Int32, ByVal batesNumber As String)		'ByVal fileURI As String, ByVal batesNumber As String)
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

		Private Function ZeroFillNumber(ByVal value As Int32, ByVal length As Int32) As String
			Dim lengthOfValue As Int32 = CType(System.Math.Floor(System.Math.Log10(CType(value, Double))), Int32) + 1
			Dim retVal As New System.Text.StringBuilder
			retVal.Append("0"c, length - lengthOfValue)
			Return retVal.ToString & value.ToString
		End Function

		Private Function CurrentDocumentSize(ByVal dataTable As System.Data.DataTable, ByVal initialPosition As Int32, ByVal productionArtifactID As Int32) As Long
			Dim documentArtifactID As Int32
			Dim count As Int32 = initialPosition
			Dim size As Long = 0

			documentArtifactID = CType(dataTable.Rows(count)("DocumentArtifactID"), Int32)
			While count < dataTable.Rows.Count AndAlso CType(dataTable.Rows(count)("documentArtifactID"), Int32) = documentArtifactID
				size = size + CType(dataTable.Rows(count)("ImageSize"), Long)
				count = count + 1
			End While
			Return size
		End Function

		Private Function CurrentDocumentCount(ByVal dataTable As System.Data.DataTable, ByVal initialPosition As Int32) As Int32
			Dim documentArtifactID As Int32
			Dim count As Int32 = initialPosition

			documentArtifactID = CType(dataTable.Rows(count)("DocumentArtifactID"), Int32)
			While count < dataTable.Rows.Count AndAlso CType(dataTable.Rows(count)("documentArtifactID"), Int32) = documentArtifactID
				count = count + 1
			End While
			Return count - initialPosition
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