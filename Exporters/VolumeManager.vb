Namespace kCura.WinEDDS
	Public Class VolumeManager

#Region "Members"

		Private _settings As ExportFile
		Private _imageFileWriter As System.IO.StreamWriter
		Private _nativeFileWriter As System.IO.StreamWriter

		Private _currentVolumeNumber As Int32
		Private _currentSubdirectoryNumber As Int32

		Private _currentVolumeSize As Int64
		Private _currentImageSubdirectorySize As Int64
		Private _currentNativeSubdirectorySize As Int64
		Private _documentManager As kCura.WinEDDS.Service.DocumentManager

		Private _volumeLabelPaddingWidth As Int32
		Private _subdirectoryLabelPaddingWidth As Int32
		Private _downloadManager As FileDownloader

		Private _parent As WinEDDS.Exporter
		Private _columnHeaderString As String
		Private _hasWrittenColumnHeaderString As Boolean = False
#End Region

#Region "Accessors"

		Public ReadOnly Property Settings() As ExportFile
			Get
				Return _settings
			End Get
		End Property

		Private ReadOnly Property CurrentVolumeLabel() As String
			Get
				Return _settings.VolumeInfo.VolumePrefix & _currentVolumeNumber.ToString.PadLeft(_volumeLabelPaddingWidth, "0"c)
			End Get
		End Property

		Private ReadOnly Property CurrentImageSubdirectoryLabel() As String
			Get
				Return _settings.VolumeInfo.SubdirectoryImagePrefix & _currentVolumeNumber.ToString.PadLeft(_subdirectoryLabelPaddingWidth, "0"c)
			End Get
		End Property

		Private ReadOnly Property CurrentNativeSubdirectoryLabel() As String
			Get
				Return _settings.VolumeInfo.SubdirectoryNativePrefix & _currentVolumeNumber.ToString.PadLeft(_subdirectoryLabelPaddingWidth, "0"c)
			End Get
		End Property

		Public ReadOnly Property SubDirectoryMaxSize() As Int64
			Get
				Return Me.Settings.VolumeInfo.SubdirectoryMaxSize
			End Get
		End Property

		Public ReadOnly Property VolumeMaxSize() As Long
			Get
				Return Me.Settings.VolumeInfo.VolumeMaxSize * 1024 * 1024
			End Get
		End Property

		Public Property ColumnHeaderString() As String
			Get
				Return _columnHeaderString
			End Get
			Set(ByVal value As String)
				_columnHeaderString = value
			End Set
		End Property

#End Region

#Region "Constructors"

		Public Sub New(ByVal settings As ExportFile, ByVal rootDirectory As String, ByVal overWriteFiles As Boolean, ByVal totalFiles As Int64, ByVal parent As WinEDDS.Exporter, ByVal downloadHandler As FileDownloader)
			_settings = settings
			If Me.Settings.ExportImages Then
			End If
			_currentVolumeNumber = _settings.VolumeInfo.VolumeStartNumber
			_currentSubdirectoryNumber = _settings.VolumeInfo.SubdirectoryStartNumber
			Dim volumeNumberPaddingWidth As Int32 = CType(System.Math.Floor(System.Math.Log10(CType(_currentVolumeNumber, Double)) + 1), Int32)
			Dim subdirectoryNumberPaddingWidth As Int32 = CType(System.Math.Floor(System.Math.Log10(CType(_currentSubdirectoryNumber, Double)) + 1), Int32)
			Dim totalFilesNumberPaddingWidth As Int32 = CType(System.Math.Floor(System.Math.Log10(CType(totalFiles + _currentVolumeNumber, Double)) + 1), Int32)
			_volumeLabelPaddingWidth = System.Math.Max(totalFilesNumberPaddingWidth, volumeNumberPaddingWidth)
			totalFilesNumberPaddingWidth = CType(System.Math.Floor(System.Math.Log10(CType(totalFiles + _currentSubdirectoryNumber, Double)) + 1), Int32)
			_subdirectoryLabelPaddingWidth = System.Math.Max(totalFilesNumberPaddingWidth, subdirectoryNumberPaddingWidth)
			_currentVolumeSize = 0
			_currentImageSubdirectorySize = 0
			_currentNativeSubdirectorySize = 0
			_downloadManager = downloadHandler
			_parent = parent
			_nativeFileWriter = New System.IO.StreamWriter(Me.Settings.FolderPath & "\" & "export." & Me.Settings.LoadFileExtension, False, System.Text.Encoding.Default)
			Dim logFileExension As String = ""
			Select Case Me.Settings.LogFileFormat
				Case LoadFileType.FileFormat.Concordance
					logFileExension = ".opt"
				Case Else
					logFileExension = ".ldf"
			End Select
			_imageFileWriter = New System.IO.StreamWriter(Me.Settings.FolderPath & "\" & "export" & logFileExension, False)
		End Sub

		Public Sub Finish()
			_nativeFileWriter.Flush()
			_nativeFileWriter.Close()
			_imageFileWriter.Flush()
			_imageFileWriter.Close()
		End Sub
#End Region

		Public Sub ExportDocument(ByVal documentInfo As Exporters.DocumentExportInfo)
			Dim totalFileSize As Int64 = 0
			Dim image As Exporters.ImageExportInfo
			If Me.Settings.ExportImages Then
				For Each image In documentInfo.Images
					totalFileSize += Me.DownloadImage(image)
				Next
			End If
			If Me.Settings.ExportNative Then
				totalFileSize += Me.DownloadNative(documentInfo)
			End If
			If totalFileSize + _currentVolumeSize > Me.VolumeMaxSize Then
				Me.UpdateVolume()
			ElseIf documentInfo.ImageCount + _currentImageSubdirectorySize > Me.SubDirectoryMaxSize OrElse _
			 documentInfo.NativeCount + _currentNativeSubdirectorySize > Me.SubDirectoryMaxSize Then
				Me.UpdateSubdirectory()
			End If

			If Me.Settings.ExportImages Then
				Me.ExportImages(documentInfo.Images)
			End If
			Dim nativeLocation As String = ""
			If Me.Settings.ExportNative Then
				Dim localFilePath As String = Me.Settings.FolderPath
				If localFilePath.Chars(localFilePath.Length - 1) <> "\"c Then localFilePath &= "\"
				localFilePath &= Me.CurrentVolumeLabel & "\" & Me.CurrentNativeSubdirectoryLabel & "\"
				If Not System.IO.Directory.Exists(localFilePath) Then System.IO.Directory.CreateDirectory(localFilePath)
				localFilePath &= documentInfo.NativeFileName
				Me.ExportNative(localFilePath, documentInfo.NativeFileGuid, documentInfo.DocumentArtifactID, documentInfo.NativeFileName, documentInfo.NativeTempLocation)
				If documentInfo.NativeTempLocation = "" Then
					nativeLocation = ""
				Else
					nativeLocation = localFilePath
				End If
			End If
			If Me.Settings.ExportFullText OrElse Me.Settings.ExportNative Then
				If Not _hasWrittenColumnHeaderString Then
					_nativeFileWriter.Write(_columnHeaderString)
					_hasWrittenColumnHeaderString = True
				End If
				Me.UpdateLoadFile(documentInfo.DataRow, documentInfo.FullTextFileGuid, documentInfo.DocumentArtifactID, nativeLocation)
			End If
			_parent.DocumentsExported += 1

			_currentVolumeSize += totalFileSize
			_currentNativeSubdirectorySize += documentInfo.NativeCount
			_currentImageSubdirectorySize += documentInfo.ImageCount
		End Sub

		Public Sub Close()
			If Not _imageFileWriter Is Nothing Then
				_imageFileWriter.Flush()
				_imageFileWriter.Close()
			End If
			If Not _nativeFileWriter Is Nothing Then
				_nativeFileWriter.Flush()
				_nativeFileWriter.Close()
			End If
		End Sub

#Region "Image Export"

		Public Sub ExportImages(ByVal images As System.Collections.ArrayList)
			Dim image As WinEDDS.Exporters.ImageExportInfo
			Dim i As Int32 = 0
			Dim localFilePath As String = Me.Settings.FolderPath

			If localFilePath.Chars(localFilePath.Length - 1) <> "\"c Then localFilePath &= "\"
			localFilePath &= Me.CurrentVolumeLabel & "\" & Me.CurrentImageSubdirectoryLabel & "\"
			If Not System.IO.Directory.Exists(localFilePath) Then System.IO.Directory.CreateDirectory(localFilePath)
			For Each image In images
				Me.ExportDocumentImage(localFilePath & image.FileName, image.FileGuid, image.ArtifactID, image.BatesNumber, image.TempLocation)
				Me.CreateImageLogEntry(image.BatesNumber, localFilePath & image.FileName, localFilePath, i = 0)
				i += 1
			Next
		End Sub

		Private Function DownloadImage(ByVal image As Exporters.ImageExportInfo) As Int64
			Dim tempFile As String = System.IO.Path.GetTempFileName
			_downloadManager.DownloadFile(tempFile, image.FileGuid, image.ArtifactID, _settings.CaseArtifactID.ToString)
			image.TempLocation = tempFile
			Return New System.IO.FileInfo(tempFile).Length
		End Function

		Private Sub ExportDocumentImage(ByVal fileName As String, ByVal fileGuid As String, ByVal artifactID As Int32, ByVal batesNumber As String, ByVal tempFileLocation As String)
			If System.IO.File.Exists(fileName) Then
				If _settings.Overwrite Then
					System.IO.File.Delete(fileName)
					_parent.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Overwriting document {0}.tif.", batesNumber))
					System.IO.File.Move(tempFileLocation, fileName)
				Else
					_parent.WriteWarning(String.Format("{0}.tif already exists. Skipping file export.", batesNumber))
				End If
			Else
				_parent.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Now exporting document {0}.tif.", batesNumber))
				System.IO.File.Move(tempFileLocation, fileName)
			End If
			_parent.WriteStatusLine(Windows.Process.EventType.Status, String.Format("Finished exporting document {0}.tif.", batesNumber))
			'_parent.DocumentsExported += 1
		End Sub

		Private Sub CreateImageLogEntry(ByVal batesNumber As String, ByVal copyFile As String, ByVal pathToImage As String, ByVal firstDocument As Boolean)
			Dim fullTextGuid As String
			Dim fullText As String
			Dim pageText As String
			'Dim currentPage As Int32 = count
			Select Case _settings.LogFileFormat
				Case LoadFileType.FileFormat.Concordance
					Dim log As New System.Text.StringBuilder
					log.AppendFormat("{0},{1},{2},", batesNumber, Me.CurrentVolumeLabel, copyFile)
					If firstDocument Then
						log.Append("Y")
					End If
					log.Append(",,,")
					_imageFileWriter.WriteLine(log.ToString)
				Case LoadFileType.FileFormat.IPRO
					Dim log As New System.Text.StringBuilder

					log.AppendFormat("IM,{0},", batesNumber)
					If firstDocument Then
						log.Append("D,")
					Else
						log.Append(" ,")
					End If
					log.AppendFormat("0,{0};{1};{2}.tif;2", Me.CurrentVolumeLabel, pathToImage, batesNumber)
					_imageFileWriter.WriteLine(log.ToString)
				Case LoadFileType.FileFormat.IPRO_FullText
					'TODO: Support This
					'If Me.LoadFileFormat = kCura.WinEDDS.LoadFileType.FileFormat.IPRO_FullText Then
					'	If isFirstDocumentImage Then
					'		currentPageFirstByteNumber = 0
					'	End If
					'	Try
					'		fullTextGuid = _fileManager.GetFullTextGuidsByDocumentArtifactIdAndType(Me.SelectedCaseInfo.ArtifactID, CType(documentImagesTable.Rows(count)("DocumentArtifactID"), Int32), 2)
					'		If fullTextGuid = "" Then
					'			fullText = ""
					'			Me.WriteWarning(String.Format("Could not retrieve full text for document #{0}", count + 1))
					'		Else
					'			Dim tempFile As String = System.IO.Path.GetTempFileName
					'			_downloadManager.DownloadFile(tempFile, fullTextGuid, CType(documentImagesTable.Rows(count)("DocumentArtifactID"), Int32), Me.SelectedCaseInfo.ArtifactID.ToString)
					'			Dim sr As New System.IO.StreamReader(tempFile)
					'			fullText = sr.ReadToEnd
					'			sr.Close()
					'			System.IO.File.Delete(tempFile)
					'		End If
					'		pageText = fullText.Substring(currentPageFirstByteNumber, CInt(documentImagesTable.Rows(count)("ByteRange")))
					'		pageText = pageText.Replace(ChrW(10), " ")
					'		pageText = pageText.Replace(",", "")
					'		pageText = pageText.Replace(" ", "|0|0|0|0^")
					'		fullTextVolumeLog.AppendFormat("FT,{0},1,1,{1}", CType(documentImagesTable.Rows(count)("BatesNumber"), String), pageText)
					'		fullTextVolumeLog.AppendFormat("{0}", Microsoft.VisualBasic.ControlChars.NewLine)
					'		currentPageFirstByteNumber += CInt(documentImagesTable.Rows(count)("ByteRange"))
					'	Catch ex As System.InvalidCastException
					'		Me.WriteWarning(String.Format("Could not retrieve full text for document #{0}", count + 1))
					'	Catch ex As System.IO.FileNotFoundException
					'		WriteWarning(ex.Message)
					'	End Try
					'End If
					'volumeLog.Append(BuildIproLog(CType(documentImagesTable.Rows(count)("BatesNumber"), String), currentVolume, currentDirectory, isFirstDocumentImage))
			End Select

		End Sub

#End Region

		Private Function ExportNative(ByVal exportFileName As String, ByVal fileGuid As String, ByVal artifactID As Int32, ByVal systemFileName As String, ByVal tempLocation As String) As String
			If Not tempLocation = "" Then
				If System.IO.File.Exists(exportFileName) Then
					If _settings.Overwrite Then
						System.IO.File.Delete(exportFileName)
						_parent.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Overwriting document {0}.", systemFileName))
						System.IO.File.Move(tempLocation, exportFileName)
					Else
						_parent.WriteWarning(String.Format("{0} already exists. Skipping file export.", systemFileName))
					End If
				Else
					_parent.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Now exporting document {0}.", systemFileName))
					System.IO.File.Move(tempLocation, exportFileName)
				End If
			End If
			_parent.WriteUpdate(String.Format("Finished exporting document {0}.", systemFileName))
		End Function

		Private Function DownloadNative(ByVal docinfo As Exporters.DocumentExportInfo) As Int64
			If docinfo.NativeFileGuid = "" Then Return 0
			Dim tempFile As String = System.IO.Path.GetTempFileName
			_downloadManager.DownloadFile(tempFile, docinfo.NativeFileGuid, docinfo.DocumentArtifactID, _settings.CaseArtifactID.ToString)
			docinfo.NativeTempLocation = tempFile
			Return New System.IO.FileInfo(tempFile).Length
		End Function

		Public Sub UpdateLoadFile(ByVal row As System.Data.DataRow, ByVal fullTextFileGuid As String, ByVal documentArtifactID As Int32, ByVal nativeLocation As String)
			Dim count As Int32
			Dim fieldValue As String
			Dim retString As New System.Text.StringBuilder
			Dim columnName As String
			Dim location As String = nativeLocation
			For count = 0 To _parent.Columns.Count - 1
				'If TypeOf Me.Columns(count) Is DBNull Then
				'	fieldValue = String.Empty
				'Else
				'	fieldValue = CType(row(CType(Me.Columns(count), String)), String)
				'End If
				columnName = CType(_parent.Columns(count), String)
				Dim val As Object = row(columnName)
				If TypeOf val Is Byte() Then
					val = System.Text.Encoding.Unicode.GetString(DirectCast(val, Byte()))
				End If
				fieldValue = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(NullableTypes.HelperFunctions.DBNullConvert.ToNullableString(val))
				fieldValue = fieldValue.Replace(System.Environment.NewLine, ChrW(10).ToString)
				fieldValue = fieldValue.Replace(ChrW(13), ChrW(10))
				fieldValue = fieldValue.Replace(ChrW(10), _settings.NewlineDelimiter)
				If fieldValue.Length > 1 AndAlso fieldValue.Chars(0) = ChrW(11) AndAlso fieldValue.Chars(fieldValue.Length - 1) = ChrW(11) Then
					fieldValue = fieldValue.Trim(New Char() {ChrW(11)}).Replace(ChrW(11), _settings.MultiRecordDelimiter)
				End If
				retString.AppendFormat("{0}{1}{0}", _settings.QuoteDelimiter, fieldValue)
				If Not count = _parent.Columns.Count - 1 Then
					retString.Append(_settings.RecordDelimiter)
				End If
			Next
			If _settings.ExportNative Then retString.AppendFormat("{2}{0}{1}{0}", _settings.QuoteDelimiter, location, _settings.RecordDelimiter)
			If _settings.ExportFullText Then
				Dim bodyText As String
				If fullTextFileGuid Is Nothing Then
					bodyText = String.Empty
				Else
					Dim tempFile As String = System.IO.Path.GetTempFileName
					_downloadManager.DownloadFile(tempFile, fullTextFileGuid, documentArtifactID, _settings.CaseInfo.ArtifactID.ToString)
					Dim sr As New System.IO.StreamReader(tempFile)
					bodyText = sr.ReadToEnd.Replace(System.Environment.NewLine, _settings.NewlineDelimiter).Replace(ChrW(13), _settings.NewlineDelimiter).Replace(ChrW(10), _settings.NewlineDelimiter)
					sr.Close()
					System.IO.File.Delete(tempFile)
				End If
				retString.AppendFormat("{2}{0}{1}{0}", _settings.QuoteDelimiter, bodyText, _settings.RecordDelimiter)
			End If
			_nativeFileWriter.WriteLine(retString.ToString)
		End Sub

		Public Sub UpdateVolume()
			_currentVolumeSize = 0
			_currentImageSubdirectorySize = 0
			_currentNativeSubdirectorySize = 0
			_currentSubdirectoryNumber = _settings.VolumeInfo.SubdirectoryStartNumber
			_currentVolumeNumber += 1
		End Sub

		Public Sub UpdateSubdirectory()
			_currentImageSubdirectorySize = 0
			_currentNativeSubdirectorySize = 0
			_currentSubdirectoryNumber += 1
		End Sub

	End Class
End Namespace