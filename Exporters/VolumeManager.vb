Namespace kCura.WinEDDS
	Public Class VolumeManager

#Region "Members"

		Private _settings As ExportFile
		Private _imageFileWriter As System.IO.StreamWriter
		Private _nativeFileWriter As System.IO.StreamWriter
		Private _errorWriter As System.IO.StreamWriter

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
		Private _encoding As System.Text.Encoding
		Private _errorFileLocation As String = ""
		Private _timekeeper As kCura.Utility.Timekeeper
#End Region

		Private Enum ExportFileType
			Image
			Native
		End Enum




#Region "Accessors"
		Public ReadOnly Property ErrorLogFileName() As String
			Get
				Try
					Return _errorFileLocation
				Catch ex As System.Exception
					Return ""
				End Try
			End Get
		End Property
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
				Return _settings.VolumeInfo.SubdirectoryImagePrefix & _currentSubdirectoryNumber.ToString.PadLeft(_subdirectoryLabelPaddingWidth, "0"c)
			End Get
		End Property

		Private ReadOnly Property CurrentNativeSubdirectoryLabel() As String
			Get
				Return _settings.VolumeInfo.SubdirectoryNativePrefix & _currentSubdirectoryNumber.ToString.PadLeft(_subdirectoryLabelPaddingWidth, "0"c)
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

		Public Sub New(ByVal settings As ExportFile, ByVal rootDirectory As String, ByVal overWriteFiles As Boolean, ByVal totalFiles As Int64, ByVal parent As WinEDDS.Exporter, ByVal downloadHandler As FileDownloader, ByVal t As kCura.Utility.Timekeeper)
			_settings = settings
			If Me.Settings.ExportImages Then
			End If
			_timekeeper = t
			_currentVolumeNumber = _settings.VolumeInfo.VolumeStartNumber
			_currentSubdirectoryNumber = _settings.VolumeInfo.SubdirectoryStartNumber
			Dim volumeNumberPaddingWidth As Int32 = CType(System.Math.Floor(System.Math.Log10(CType(_currentVolumeNumber + 1, Double)) + 1), Int32)
			Dim subdirectoryNumberPaddingWidth As Int32 = CType(System.Math.Floor(System.Math.Log10(CType(_currentSubdirectoryNumber + 1, Double)) + 1), Int32)
			Dim totalFilesNumberPaddingWidth As Int32 = CType(System.Math.Floor(System.Math.Log10(CType(totalFiles + _currentVolumeNumber + 1, Double)) + 1), Int32)
			_volumeLabelPaddingWidth = System.Math.Max(totalFilesNumberPaddingWidth, volumeNumberPaddingWidth)
			totalFilesNumberPaddingWidth = CType(System.Math.Floor(System.Math.Log10(CType(totalFiles + _currentSubdirectoryNumber, Double)) + 1), Int32)
			_subdirectoryLabelPaddingWidth = System.Math.Max(totalFilesNumberPaddingWidth, subdirectoryNumberPaddingWidth)
			_currentVolumeSize = 0
			_currentImageSubdirectorySize = 0
			_currentNativeSubdirectorySize = 0
			_downloadManager = downloadHandler
			_parent = parent
			Dim loadFilePath As String = Me.Settings.FolderPath & "\" & Me.Settings.LoadFilesPrefix & "_export." & Me.Settings.LoadFileExtension
			If Not Me.Settings.Overwrite AndAlso System.IO.File.Exists(loadFilePath) Then
				Throw New System.Exception(String.Format("Overwrite not selected and file '{0}' exists.", loadFilePath))
			End If
			If _parent.ExportAsUnicode Then
				_encoding = System.Text.Encoding.Unicode
			Else
				_encoding = System.Text.Encoding.Default
			End If
			_nativeFileWriter = New System.IO.StreamWriter(loadFilePath, False, _encoding)
			Dim logFileExension As String = ""
			Select Case Me.Settings.LogFileFormat
				Case LoadFileType.FileFormat.Opticon
					logFileExension = ".opt"
				Case LoadFileType.FileFormat.IPRO
					logFileExension = ".lfp"
				Case LoadFileType.FileFormat.IPRO_FullText
					logFileExension = "_FULLTEXT_.lfp"
				Case Else
			End Select
			Dim imageFilePath As String = Me.Settings.FolderPath & "\" & Me.Settings.LoadFilesPrefix & "_export" & logFileExension
			If Me.Settings.ExportImages Then
				If Not Me.Settings.Overwrite AndAlso System.IO.File.Exists(imageFilePath) Then
					Throw New System.Exception(String.Format("Overwrite not selected and file '{0}' exists.", imageFilePath))
				End If
				_imageFileWriter = New System.IO.StreamWriter(imageFilePath, False, _encoding)
			End If
		End Sub

		Private Sub LogFileExportError(ByVal type As ExportFileType, ByVal recordIdentifier As String, ByVal fileLocation As String, ByVal errorText As String)
			If _errorWriter Is Nothing Then
				_errorFileLocation = System.IO.Path.GetTempFileName()
				_errorWriter = New System.IO.StreamWriter(_errorFileLocation, False, _encoding)
				_errorWriter.WriteLine("""File Type"",""Document Identifier"",""File Guid"",""Error Description""")
			End If
			_errorWriter.WriteLine(String.Format("""{0}"",""{1}"",""{2}"",""{3}""", type.ToString, recordIdentifier, fileLocation, kCura.Utility.Strings.ToCsvCellContents(errorText)))
			_parent.WriteError(String.Format("{0} - Document [{1}] - File [{2}] - Error: {3}{4}", type.ToString, recordIdentifier, fileLocation, System.Environment.NewLine, errorText))
		End Sub

		Public Sub Finish()
			If Not _nativeFileWriter Is Nothing Then
				_nativeFileWriter.Flush()
				_nativeFileWriter.Close()
			End If
			If Not _imageFileWriter Is Nothing Then
				_imageFileWriter.Flush()
				_imageFileWriter.Close()
			End If
			If Not _errorWriter Is Nothing Then
				_errorWriter.Flush()
				_errorWriter.Close()
			End If
		End Sub
#End Region

		Public Sub ExportDocument(ByVal documentInfo As Exporters.DocumentExportInfo)
			Dim totalFileSize As Int64 = 0
			Dim image As Exporters.ImageExportInfo
			Dim imageSuccess As Boolean = True
			Dim nativeSuccess As Boolean = True
			Dim updateVolumeAfterExport As Boolean = False
			Dim updateSubDirectoryAfterExport As Boolean = False
			If Me.Settings.ExportImages Then
				For Each image In documentInfo.Images
					_timekeeper.MarkStart("VolumeManager_DownloadImage")
					Try
						totalFileSize += Me.DownloadImage(image)
					Catch ex As System.Exception
						image.TempLocation = ""
						Me.LogFileExportError(ExportFileType.Image, documentInfo.IdentifierValue, image.FileGuid, ex.ToString)
						imageSuccess = False
					End Try
					_timekeeper.MarkEnd("VolumeManager_DownloadImage")
				Next
			End If
			If Me.Settings.ExportNative Then
				_timekeeper.MarkStart("VolumeManager_DownloadNative")
				Try
					totalFileSize += Me.DownloadNative(documentInfo)
				Catch ex As System.Exception
					Me.LogFileExportError(ExportFileType.Native, documentInfo.IdentifierValue, documentInfo.NativeFileGuid, ex.ToString)
				End Try
				_timekeeper.MarkEnd("VolumeManager_DownloadNative")
			End If
			If totalFileSize + _currentVolumeSize > Me.VolumeMaxSize Then
				If _currentVolumeSize = 0 Then
					updateVolumeAfterExport = True
				Else
					Me.UpdateVolume()
				End If
			ElseIf documentInfo.ImageCount + _currentImageSubdirectorySize >= Me.SubDirectoryMaxSize OrElse _
			 documentInfo.NativeCount + _currentNativeSubdirectorySize >= Me.SubDirectoryMaxSize Then
				If _currentImageSubdirectorySize = 0 OrElse _currentNativeSubdirectorySize = 0 Then
					updateSubDirectoryAfterExport = True
				Else
					Me.UpdateSubdirectory()
				End If
			End If
			Dim tempLocalFullTextFilePath As String = ""
			If Me.Settings.ExportFullText OrElse Me.Settings.LogFileFormat = LoadFileType.FileFormat.IPRO_FullText Then
				If documentInfo.HasFullText Then
					tempLocalFullTextFilePath = System.IO.Path.GetTempFileName
					Try
						_downloadManager.DownloadFullTextFile(tempLocalFullTextFilePath, documentInfo.DocumentArtifactID, _settings.CaseInfo.ArtifactID.ToString)
					Catch ex As System.Exception
						_downloadManager.DownloadFullTextFile(tempLocalFullTextFilePath, documentInfo.DocumentArtifactID, _settings.CaseInfo.ArtifactID.ToString)
					End Try
				End If
			End If
			If Me.Settings.ExportImages Then
				_timekeeper.MarkStart("VolumeManager_ExportImages")
				Me.ExportImages(documentInfo.Images, tempLocalFullTextFilePath)
				_timekeeper.MarkEnd("VolumeManager_ExportImages")
			End If
			Dim nativeLocation As String = ""
			If Me.Settings.ExportNative Then
				Dim nativeFileName As String = Me.GetNativeFileName(documentInfo)
				Dim localFilePath As String = Me.GetLocalNativeFilePath(documentInfo, nativeFileName)
				_timekeeper.MarkStart("VolumeManager_ExportNative")
				Me.ExportNative(localFilePath, documentInfo.NativeFileGuid, documentInfo.DocumentArtifactID, nativeFileName, documentInfo.NativeTempLocation)
				_timekeeper.MarkEnd("VolumeManager_ExportNative")
				If documentInfo.NativeTempLocation = "" Then
					nativeLocation = ""
				Else
					Select Case Me.Settings.TypeOfExportedFilePath
						Case ExportFile.ExportedFilePathType.Absolute
							nativeLocation = localFilePath
						Case ExportFile.ExportedFilePathType.Relative
							nativeLocation = ".\" & Me.CurrentVolumeLabel & "\" & Me.CurrentNativeSubdirectoryLabel & "\" & nativeFileName
						Case ExportFile.ExportedFilePathType.Prefix
							nativeLocation = Me.Settings.FilePrefix.TrimEnd("\"c) & "\" & Me.CurrentVolumeLabel & "\" & Me.CurrentNativeSubdirectoryLabel & "\" & nativeFileName
					End Select
				End If
			End If
			If Not _hasWrittenColumnHeaderString Then
				_nativeFileWriter.Write(_columnHeaderString)
				_hasWrittenColumnHeaderString = True
			End If
			Me.UpdateLoadFile(documentInfo.DataRow, documentInfo.HasFullText, documentInfo.DocumentArtifactID, nativeLocation, tempLocalFullTextFilePath)
			_parent.DocumentsExported += 1
			_currentVolumeSize += totalFileSize
			_currentNativeSubdirectorySize += documentInfo.NativeCount
			_currentImageSubdirectorySize += documentInfo.ImageCount
			If updateSubDirectoryAfterExport Then Me.UpdateSubdirectory()
			If updateVolumeAfterExport Then Me.UpdateVolume()

		End Sub

		Private Function GetLocalNativeFilePath(ByVal doc As Exporters.DocumentExportInfo, ByVal nativeFileName As String) As String
			Dim localFilePath As String = Me.Settings.FolderPath
			If localFilePath.Chars(localFilePath.Length - 1) <> "\"c Then localFilePath &= "\"
			localFilePath &= Me.CurrentVolumeLabel & "\" & Me.CurrentNativeSubdirectoryLabel & "\"
			If Not System.IO.Directory.Exists(localFilePath) Then System.IO.Directory.CreateDirectory(localFilePath)
			Return localFilePath & nativeFileName
		End Function

		Private Function GetNativeFileName(ByVal doc As Exporters.DocumentExportInfo) As String
			Select Case _parent.ExportNativesToFileNamedFrom
				Case ExportNativeWithFilenameFrom.Identifier
					Return doc.NativeFileName(Me.Settings.AppendOriginalFileName)
				Case ExportNativeWithFilenameFrom.Production
					Return doc.ProductionBeginBatesFileName(Me.Settings.AppendOriginalFileName)
			End Select
		End Function

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

		Private Function GetImageExportLocation(ByVal image As Exporters.ImageExportInfo) As String
			Dim localFilePath As String = Me.Settings.FolderPath
			Dim subfolderPath As String = Me.CurrentVolumeLabel & "\" & Me.CurrentImageSubdirectoryLabel & "\"
			If localFilePath.Chars(localFilePath.Length - 1) <> "\"c Then localFilePath &= "\"
			localFilePath &= subfolderPath
			If Not System.IO.Directory.Exists(localFilePath) Then System.IO.Directory.CreateDirectory(localFilePath)
			Return localFilePath & image.FileName
		End Function


		Public Sub ExportImages(ByVal images As System.Collections.ArrayList, ByVal localFullTextPath As String)
			Dim image As WinEDDS.Exporters.ImageExportInfo
			Dim i As Int32 = 0
			Dim fullTextReader As System.IO.StreamReader = Nothing
			Dim localFilePath As String = Me.Settings.FolderPath
			Dim subfolderPath As String = Me.CurrentVolumeLabel & "\" & Me.CurrentImageSubdirectoryLabel & "\"
			Dim pageOffset As Long
			If localFilePath.Chars(localFilePath.Length - 1) <> "\"c Then localFilePath &= "\"
			localFilePath &= subfolderPath
			If Not System.IO.Directory.Exists(localFilePath) Then System.IO.Directory.CreateDirectory(localFilePath)
			Try
				If Me.Settings.LogFileFormat = LoadFileType.FileFormat.IPRO_FullText Then
					If System.IO.File.Exists(localFullTextPath) Then
						fullTextReader = New System.IO.StreamReader(localFullTextPath, _encoding, True)
					End If
				End If
				For Each image In images
					If (i = 0 AndAlso image.PageOffset.IsNull) OrElse i = images.Count - 1 Then
						pageOffset = Int64.MinValue
					Else
						Dim nextImage As Exporters.ImageExportInfo = DirectCast(images(i + 1), Exporters.ImageExportInfo)
						If nextImage.PageOffset.IsNull Then
							pageOffset = Int64.MinValue
						Else
							pageOffset = nextImage.PageOffset.Value
						End If
					End If
					Me.ExportDocumentImage(localFilePath & image.FileName, image.FileGuid, image.ArtifactID, image.BatesNumber, image.TempLocation)
					Select Case Me.Settings.TypeOfExportedFilePath
						Case ExportFile.ExportedFilePathType.Absolute
							Me.CreateImageLogEntry(image.BatesNumber, localFilePath & image.FileName, localFilePath, i = 0, fullTextReader, localFullTextPath <> "", pageOffset, images.Count)
						Case ExportFile.ExportedFilePathType.Relative
							Me.CreateImageLogEntry(image.BatesNumber, ".\" & subfolderPath & image.FileName, localFilePath, i = 0, fullTextReader, localFullTextPath <> "", pageOffset, images.Count)
						Case ExportFile.ExportedFilePathType.Prefix
							Me.CreateImageLogEntry(image.BatesNumber, Me.Settings.FilePrefix.TrimEnd("\"c) & "\" & subfolderPath & image.FileName, localFilePath, i = 0, fullTextReader, localFullTextPath <> "", pageOffset, images.Count)
					End Select
					i += 1
				Next
			Catch ex As System.Exception
				If Not fullTextReader Is Nothing Then fullTextReader.Close()
				Throw
			End Try
			If Not fullTextReader Is Nothing Then fullTextReader.Close()
		End Sub

		Private Function DownloadImage(ByVal image As Exporters.ImageExportInfo) As Int64
			If image.FileGuid = "" Then Return 0
			Dim tempFile As String = Me.GetImageExportLocation(image)
			If System.IO.File.Exists(tempFile) Then
				If _settings.Overwrite Then
					System.IO.File.Delete(tempFile)
					_parent.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Overwriting image for {0}.", image.BatesNumber), False)
				Else
					_parent.WriteWarning(String.Format("{0} already exists. Skipping file export.", tempFile))
					Return 0
				End If
			End If
			Try
				_downloadManager.DownloadFile(tempFile, image.FileGuid, image.SourceLocation, image.ArtifactID, _settings.CaseArtifactID.ToString)
			Catch ex As System.Exception
				_downloadManager.DownloadFile(tempFile, image.FileGuid, image.SourceLocation, image.ArtifactID, _settings.CaseArtifactID.ToString)
			End Try
			image.TempLocation = tempFile
			Return New System.IO.FileInfo(tempFile).Length
		End Function

		Private Sub ExportDocumentImage(ByVal fileName As String, ByVal fileGuid As String, ByVal artifactID As Int32, ByVal batesNumber As String, ByVal tempFileLocation As String)
			If Not tempFileLocation = "" AndAlso Not tempFileLocation.ToLower = fileName.ToLower Then
				If System.IO.File.Exists(fileName) Then
					If _settings.Overwrite Then
						System.IO.File.Delete(fileName)
						_parent.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Overwriting document {0}.tif.", batesNumber), False)
						System.IO.File.Move(tempFileLocation, fileName)
					Else
						_parent.WriteWarning(String.Format("{0}.tif already exists. Skipping file export.", batesNumber))
					End If
				Else
					_timekeeper.MarkStart("VolumeManager_ExportDocumentImage_WriteStatus")
					_parent.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Now exporting document {0}.tif.", batesNumber), False)
					_timekeeper.MarkEnd("VolumeManager_ExportDocumentImage_WriteStatus")
					_timekeeper.MarkStart("VolumeManager_ExportDocumentImage_MoveFile")
					System.IO.File.Move(tempFileLocation, fileName)
					_timekeeper.MarkEnd("VolumeManager_ExportDocumentImage_MoveFile")
				End If
				_timekeeper.MarkStart("VolumeManager_ExportDocumentImage_WriteStatus")
				_parent.WriteStatusLine(Windows.Process.EventType.Status, String.Format("Finished exporting document {0}.tif.", batesNumber), False)
				_timekeeper.MarkEnd("VolumeManager_ExportDocumentImage_WriteStatus")
			End If
			'_parent.DocumentsExported += 1
		End Sub

		Private Sub CreateImageLogEntry(ByVal batesNumber As String, ByVal copyFile As String, ByVal pathToImage As String, ByVal firstDocument As Boolean, ByVal fullTextReader As System.IO.StreamReader, ByVal expectingTextForPage As Boolean, ByVal pageOffset As Long, ByVal numberOfImages As Int32)
			Dim fullTextGuid As String
			Dim fullText As String
			'Dim currentPage As Int32 = count
			Select Case _settings.LogFileFormat
				Case LoadFileType.FileFormat.Opticon
					Me.WriteOpticonLine(batesNumber, firstDocument, copyFile, numberOfImages)
				Case LoadFileType.FileFormat.IPRO
					Me.WriteIproImageLine(batesNumber, firstDocument, copyFile)
				Case LoadFileType.FileFormat.IPRO_FullText
					Dim currentPageFirstByteNumber As Long
					If fullTextReader Is Nothing Then
						If firstDocument AndAlso expectingTextForPage Then _parent.WriteWarning(String.Format("Could not retrieve full text for document '{0}'", batesNumber))
					Else
						Dim pageText As New System.Text.StringBuilder
						If firstDocument Then
							currentPageFirstByteNumber = 0
						Else
							currentPageFirstByteNumber = fullTextReader.BaseStream.Position
						End If
						Select Case pageOffset
							Case Int64.MinValue
								pageText.Append(fullTextReader.ReadToEnd)
							Case Else
								Dim i As Int32 = 0
								While i < pageOffset
									pageText.Append(ChrW(fullTextReader.Read))
									i += 1
								End While
						End Select
						pageText = pageText.Replace(ChrW(10), " ")
						pageText = pageText.Replace(",", "")
						pageText = pageText.Replace(" ", "|0|0|0|0^")
						_imageFileWriter.WriteLine(String.Format("FT,{0},1,1,{1}", batesNumber, pageText.ToString))
					End If
					Me.WriteIproImageLine(batesNumber, firstDocument, copyFile)
			End Select

		End Sub

		Private Sub WriteIproImageLine(ByVal batesNumber As String, ByVal firstDocument As Boolean, ByVal copyFile As String)
			Dim log As New System.Text.StringBuilder

			log.AppendFormat("IM,{0},", batesNumber)
			If firstDocument Then
				log.Append("D,")
			Else
				log.Append(" ,")
			End If
			Dim pti As String = ""
			Dim filename As String = ""
			If copyFile.LastIndexOf("\"c) = -1 Then
				pti = ""
				filename = copyFile
			Else
				pti = copyFile.Substring(0, copyFile.LastIndexOf("\"c))
				filename = copyFile.Substring(copyFile.LastIndexOf("\") + 1)
			End If
			log.AppendFormat("0,{0};{1};{2};2", Me.CurrentVolumeLabel, pti, filename)
			_imageFileWriter.WriteLine(log.ToString)
		End Sub

		Private Sub WriteOpticonLine(ByVal batesNumber As String, ByVal firstDocument As Boolean, ByVal copyFile As String, ByVal imageCount As Int32)
			Dim log As New System.Text.StringBuilder
			log.AppendFormat("{0},{1},{2},", batesNumber, Me.CurrentVolumeLabel, copyFile)
			If firstDocument Then log.Append("Y")
			log.Append(",,,")
			If firstDocument Then log.Append(imageCount)
			_imageFileWriter.WriteLine(log.ToString)
		End Sub
#End Region

		Private Function ExportNative(ByVal exportFileName As String, ByVal fileGuid As String, ByVal artifactID As Int32, ByVal systemFileName As String, ByVal tempLocation As String) As String
			If Not tempLocation = "" AndAlso Not tempLocation.ToLower = exportFileName.ToLower Then
				If System.IO.File.Exists(exportFileName) Then
					If _settings.Overwrite Then
						System.IO.File.Delete(exportFileName)
						_parent.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Overwriting document {0}.", systemFileName), False)
						System.IO.File.Move(tempLocation, exportFileName)
					Else
						_parent.WriteWarning(String.Format("{0} already exists. Skipping file export.", systemFileName))
					End If
				Else
					_timekeeper.MarkStart("VolumeManager_ExportNative_WriteStatus")
					_parent.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Now exporting document {0}.", systemFileName), False)
					_timekeeper.MarkEnd("VolumeManager_ExportNative_WriteStatus")
					_timekeeper.MarkStart("VolumeManager_ExportNative_MoveFile")
					System.IO.File.Move(tempLocation, exportFileName)
					_timekeeper.MarkEnd("VolumeManager_ExportNative_MoveFile")
				End If
			End If
			_timekeeper.MarkStart("VolumeManager_ExportNative_WriteStatus")
			_parent.WriteUpdate(String.Format("Finished exporting document {0}.", systemFileName), False)
			_timekeeper.MarkEnd("VolumeManager_ExportNative_WriteStatus")
		End Function

		Private Function DownloadNative(ByVal docinfo As Exporters.DocumentExportInfo) As Int64
			If docinfo.NativeFileGuid = "" Then Return 0
			Dim nativeFileName As String = Me.GetNativeFileName(docinfo)
			Dim tempFile As String = Me.GetLocalNativeFilePath(docinfo, nativeFileName)
			If System.IO.File.Exists(tempFile) Then
				If Settings.Overwrite Then
					System.IO.File.Delete(tempFile)
					_parent.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Overwriting document {0}.", nativeFileName), False)
				Else
					_parent.WriteWarning(String.Format("{0} already exists. Skipping file export.", tempFile))
					Return 0
				End If
			End If
			Try
				_downloadManager.DownloadFile(tempFile, docinfo.NativeFileGuid, docinfo.NativeSourceLocation, docinfo.DocumentArtifactID, _settings.CaseArtifactID.ToString)
			Catch ex As System.Exception
				_downloadManager.DownloadFile(tempFile, docinfo.NativeFileGuid, docinfo.NativeSourceLocation, docinfo.DocumentArtifactID, _settings.CaseArtifactID.ToString)
			End Try
			docinfo.NativeTempLocation = tempFile
			Return New System.IO.FileInfo(tempFile).Length
		End Function

		Public Sub UpdateLoadFile(ByVal row As System.Data.DataRow, ByVal hasFullText As Boolean, ByVal documentArtifactID As Int32, ByVal nativeLocation As String, ByVal fullTextTempFile As String)
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
				If _parent.ColumnFormats(count).ToString <> "" Then
					Dim datetime As NullableString = NullableTypes.HelperFunctions.DBNullConvert.ToNullableString(val)
					If datetime.IsNull OrElse datetime.Value = "" Then
						val = ""
					Else
						val = System.DateTime.Parse(datetime.Value, System.Globalization.CultureInfo.InvariantCulture).ToString(_parent.ColumnFormats(count).ToString)
					End If
				End If
				fieldValue = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(NullableTypes.HelperFunctions.DBNullConvert.ToNullableString(val))
				fieldValue = fieldValue.Replace(System.Environment.NewLine, ChrW(10).ToString)
				fieldValue = fieldValue.Replace(ChrW(13), ChrW(10))
				fieldValue = fieldValue.Replace(ChrW(10), _settings.NewlineDelimiter)
				fieldValue = fieldValue.Replace(_settings.QuoteDelimiter, _settings.QuoteDelimiter & _settings.QuoteDelimiter)
				If fieldValue.Length > 1 AndAlso fieldValue.Chars(0) = ChrW(11) AndAlso fieldValue.Chars(fieldValue.Length - 1) = ChrW(11) Then
					fieldValue = fieldValue.Trim(New Char() {ChrW(11)}).Replace(ChrW(11), _settings.MultiRecordDelimiter)
				End If
				retString.AppendFormat("{0}{1}{0}", _settings.QuoteDelimiter, fieldValue)
				If Not count = _parent.Columns.Count - 1 Then
					retString.Append(_settings.RecordDelimiter)
				End If
			Next
			If _settings.ExportNative Then retString.AppendFormat("{2}{0}{1}{0}", _settings.QuoteDelimiter, location, _settings.RecordDelimiter)
			_nativeFileWriter.Write(retString.ToString)
			If _settings.ExportFullText Then
				Dim bodyText As New System.Text.StringBuilder
				If Not hasFullText Then
					bodyText = New System.Text.StringBuilder("")
					_nativeFileWriter.Write(String.Format("{2}{0}{1}{0}" & vbNewLine, _settings.QuoteDelimiter, bodyText.ToString, _settings.RecordDelimiter))
				Else
					Dim sr As New System.IO.StreamReader(fullTextTempFile, System.Text.Encoding.Unicode)
					Dim c As Int32 = sr.Read
					_nativeFileWriter.Write(_settings.RecordDelimiter)
					_nativeFileWriter.Write(_settings.QuoteDelimiter)
					While Not c = -1
						Select Case c
							Case AscW(_settings.QuoteDelimiter)
								_nativeFileWriter.Write(_settings.QuoteDelimiter & _settings.QuoteDelimiter)
							Case 13, 10
								_nativeFileWriter.Write(_settings.NewlineDelimiter)
								If sr.Peek = 10 Then
									sr.Read()
								End If
							Case Else
								_nativeFileWriter.Write(ChrW(c))
						End Select
						c = sr.Read
					End While
					_nativeFileWriter.Write(_settings.QuoteDelimiter)
					sr.Close()
					Try
						System.IO.File.Delete(fullTextTempFile)
					Catch
						Try
							System.IO.File.Delete(fullTextTempFile)
						Catch
						End Try
					End Try
				End If
			End If
			_nativeFileWriter.Write(vbNewLine)
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