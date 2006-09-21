Imports System.IO
Namespace kCura.WinEDDS
	Public Class ProductionExporter
		Private _productionManager As kCura.WinEDDS.Service.ProductionManager
		Private _fileManager As kCura.WinEDDS.Service.FileManager
		Private _productionArtifactID As Int32
		Private _folderPath As String
		Private _overwrite As Boolean
		Private _webClient As System.Net.WebClient
		Private _selectedCaseInfo As kCura.EDDS.Types.CaseInfo
		Public DocumentsExported As Int32
		Public TotalDocuments As Int32
		Private _downloadManager As kCura.WinEDDS.FileDownloader
		Private _fullTextDownloader As kCura.WinEDDS.FullTextManager
		Private _loadFileFormat As kCura.WinEDDS.LoadFileType.FileFormat
		Private _documentManager As kCura.WinEDDS.Service.DocumentManager
		Private _sourceDirectory As String
		Private WithEvents _processController As kCura.Windows.Process.Controller
		Private _continue As Boolean

#Region "Public Properties"
		Public Property ProductionArtifactID() As Int32
			Get
				Return _productionArtifactID
			End Get
			Set(ByVal value As Int32)
				_productionArtifactID = value
			End Set
		End Property

		Public Property FolderPath() As String
			Get
				Return _folderPath
			End Get
			Set(ByVal value As String)
				_folderPath = value
			End Set
		End Property

		Public Property Overwrite() As Boolean
			Get
				Return _overwrite
			End Get
			Set(ByVal value As Boolean)
				_overwrite = value
			End Set
		End Property

		Public Property WebClient() As System.Net.WebClient
			Get
				Return _webClient
			End Get
			Set(ByVal value As System.Net.WebClient)
				_webClient = value
			End Set
		End Property

		Public Property SelectedCaseInfo() As kCura.EDDS.Types.CaseInfo
			Get
				Return _selectedCaseInfo
			End Get
			Set(ByVal value As kCura.EDDS.Types.CaseInfo)
				_selectedCaseInfo = value
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

		Public Sub New(ByVal cred As Net.NetworkCredential, ByVal productionArtifactID As Int32, ByVal folderPath As String, _
		ByVal selectedCaseInfo As kCura.EDDS.Types.CaseInfo, ByVal overwrite As Boolean, ByVal controller As kCura.Windows.Process.Controller, ByVal fileformat As kCura.WinEDDS.LoadFileType.FileFormat, _
		ByVal cookieContainer As System.Net.CookieContainer, ByVal identity As kCura.EDDS.EDDSIdentity)
			_productionManager = New kCura.WinEDDS.Service.ProductionManager(cred, cookieContainer, identity)
			_fileManager = New kCura.WinEDDS.Service.FileManager(cred, cookieContainer, identity)
			_productionArtifactID = productionArtifactID
			_folderPath = folderPath + "\"
			_overwrite = overwrite
			_webClient = New System.Net.WebClient
			_webClient.Credentials = cred
			_processController = controller
			_loadFileFormat = fileformat
			Me.SelectedCaseInfo = selectedCaseInfo
			Me.DocumentsExported = 0
			Me.TotalDocuments = 1
			_continue = True
			_downloadManager = New FileDownloader(cred, selectedCaseInfo.DocumentPath & "\EDDS" & selectedCaseInfo.ArtifactID, selectedCaseInfo.DownloadHandlerURL, cookieContainer)
			_documentManager = New kCura.WinEDDS.Service.DocumentManager(cred, cookieContainer, identity)
			_sourceDirectory = _documentManager.GetDocumentDirectoryByContextArtifactID(Me.SelectedCaseInfo.RootArtifactID)
			_fullTextDownloader = New kCura.WinEDDS.FullTextManager(cred, _sourceDirectory, cookieContainer)
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
			Dim dataTable As System.Data.DataTable
			Dim guidTable As System.Data.DataTable

			Dim fileURI As String
			Dim fileName As String
			Dim currentVolume As String
			Dim currentDirectory As String
			Dim volumeLog As New System.Text.StringBuilder
			Dim fullTextVolumeLog As New System.Text.StringBuilder
			Dim fileFormat As String

			Dim nextVolume As Int32
			Dim nextDirectory As Int32
			Dim volumeSize As Long = 0
			Dim subdirectoryCount As Int32 = 0
			Dim documentCount As Int32
			Dim documentSize As Long
			Dim count As Int32

			Dim currentBates As String = ""
			Dim fullTextLog As New System.Text.StringBuilder
			Dim fulltextguid As String
			Dim fullText, pageText As String

			Dim isFirstDocument As Boolean
			Dim startindex As Int32

			If _loadFileFormat = kCura.WinEDDS.LoadFileType.FileFormat.Concordance Then
				fileFormat = "log"
			Else
				fileFormat = "lfp"
			End If

			Me.WriteUpdate("Retrieving export data from the server...")
			dataTable = _fileManager.RetrieveByProductionArtifactIDForProduction(Me.ProductionArtifactID).Tables(0)

			Me.TotalDocuments = dataTable.Rows.Count()
			Me.WriteUpdate("Data retrieved. Beginning production export...")

			production = _productionManager.Read(Me.ProductionArtifactID)
			currentVolume = Me.CreateVolumeDirectory(production.VolumePrefix, production.VolumeStartNumber)
			currentDirectory = Me.CreateSubdirectory(production.SubdirectoryPrefix, production.SubdirectoryStartNumber, currentVolume)
			nextVolume = production.VolumeStartNumber + 1
			nextDirectory = production.SubdirectoryStartNumber + 1

			documentSize = Me.CurrentDocumentSize(dataTable, 0, ProductionArtifactID)
			documentCount = Me.CurrentDocumentCount(dataTable, 0)

			For count = 0 To dataTable.Rows.Count - 1
				isFirstDocument = (count = 0 OrElse CType(dataTable.Rows(count - 1)("DocumentArtifactID"), Int32) <> CType(dataTable.Rows(count)("DocumentArtifactID"), Int32))
				Try
					'If count > 0 AndAlso CType(dataTable.Rows(count - 1)("DocumentArtifactID"), Int32) <> CType(dataTable.Rows(count)("DocumentArtifactID"), Int32) Then
					If Not isFirstDocument Then
						volumeSize = volumeSize + documentSize
						documentSize = Me.CurrentDocumentSize(dataTable, count, ProductionArtifactID)
						If volumeSize + documentSize > production.VolumeMaxSize * 1024 * 1024 Then
							Me.CreateVolumeLogFile(currentVolume, production.Name, volumeLog.ToString, fileFormat)
							If _loadFileFormat = kCura.WinEDDS.LoadFileType.FileFormat.IPRO_FullText Then
								Me.CreateVolumeLogFile(currentVolume, production.Name + "_FT", fullTextVolumeLog.ToString, fileFormat)
								fullTextVolumeLog = New System.Text.StringBuilder
							End If
							volumeLog = New System.Text.StringBuilder
							currentVolume = Me.CreateVolumeDirectory(production.VolumePrefix, nextVolume)
							nextVolume = nextVolume + 1
							volumeSize = 0
						End If
						subdirectoryCount = subdirectoryCount + documentCount
						documentCount = Me.CurrentDocumentCount(dataTable, count)
						If subdirectoryCount + documentCount > production.SubdirectoryMaxFiles OrElse volumeSize = 0 Then
							currentDirectory = Me.CreateSubdirectory(production.SubdirectoryPrefix, nextDirectory, currentVolume)
							nextDirectory = nextDirectory + 1
							subdirectoryCount = 0
						End If
					End If

					'fileURI = String.Format("{0}Download.aspx?ArtifactID={1}&GUID={2}", Me.SelectedCaseInfo.DownloadHandlerURL, CType(dataTable.Rows(count)("ArtifactID"), Int32), CType(dataTable.Rows(count)("ImageGuid"), String))
					fileName = String.Format("{0}{1}\{2}\{3}.tif", Me.FolderPath, currentVolume, currentDirectory, CType(dataTable.Rows(count)("BatesNumber"), String))
					Me.ExportFile(fileName, CType(dataTable.Rows(count)("ImageGuid"), String), CType(dataTable.Rows(count)("DocumentArtifactID"), Int32), CType(dataTable.Rows(count)("BatesNumber"), String))

					If _loadFileFormat = kCura.WinEDDS.LoadFileType.FileFormat.Concordance Then
						volumeLog.Append(BuildVolumeLog(CType(dataTable.Rows(count)("BatesNumber"), String), currentVolume, fileName, isFirstDocument))
					ElseIf _loadFileFormat = kCura.WinEDDS.LoadFileType.FileFormat.IPRO_FullText Then
						If isFirstDocument Then
							startindex = 0
							fulltextguid = _fileManager.GetFullTextGuidsByDocumentArtifactIdAndType(CType(dataTable.Rows(count)("documentArtifactID"), Int32), 2)
							Try
								fullText = _fullTextDownloader.ReadFullTextFile(_sourceDirectory & fulltextguid)
							Catch ex As System.IO.FileNotFoundException
								WriteWarning(ex.Message)
							End Try
						End If
						pageText = fullText.Substring(startindex, CInt(dataTable.Rows(count)("ByteRange")) - 1)						'Dim iproTextEntry As String = ChrW(sr.Read())
						pageText = pageText.Replace(ChrW(10), " ")
						pageText = pageText.Replace(",", "")
						pageText = pageText.Replace(" ", "|0|0|0|0^")
						fullTextVolumeLog.AppendFormat("FT,{0},1,1,{1}", CType(dataTable.Rows(count)("BatesNumber"), String), pageText)
						fullTextVolumeLog.AppendFormat("{0}", Microsoft.VisualBasic.ControlChars.NewLine)
						startindex += CInt(dataTable.Rows(count)("ByteRange")) - 1
					End If
					If Not _loadFileFormat = kCura.WinEDDS.LoadFileType.FileFormat.Concordance Then
						volumeLog.Append(BuildIproLog(CType(dataTable.Rows(count)("BatesNumber"), String), currentVolume, currentDirectory, isFirstDocument))
					End If

				Catch ex As System.Exception
					Me.WriteError(String.Format("Error occurred on document #{0} with message: {1}", count + 1, ex.Message))
				End Try
				If Not _continue Then
					Exit For
				End If
			Next
			Me.CreateVolumeLogFile(currentVolume, production.Name, volumeLog.ToString, fileFormat)
			If _loadFileFormat = kCura.WinEDDS.LoadFileType.FileFormat.IPRO_FullText Then
				Me.CreateVolumeLogFile(currentVolume, production.Name & "_FT", fullTextVolumeLog.ToString, fileFormat)
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

		Private Sub ExportFile(ByVal fileName As String, ByVal fileGuid As String, ByVal artifactID As Int32, ByVal batesNumber As String)		'ByVal fileURI As String, ByVal batesNumber As String)
			If System.IO.File.Exists(fileName) Then
				If Me.Overwrite Then
					System.IO.File.Delete(fileName)
					Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Overwriting document {0}.tif.", batesNumber))
					_downloadManager.DownloadFile(fileName, fileGuid, artifactID)
					'Me.WebClient.DownloadFile(fileURI, fileName)
				Else
					Me.WriteWarning(String.Format("{0}.tif already exists. Skipping file export.", batesNumber))
				End If
			Else
				Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Now exporting document {0}.tif.", batesNumber))
				_downloadManager.DownloadFile(fileName, fileGuid, artifactID)
				'Me.WebClient.DownloadFile(fileURI, fileName)
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

		Private Function BuildFullTextIproLog(ByVal fullText As String, ByVal pageNumber As Int32, ByVal byteRange As Int32) As String

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