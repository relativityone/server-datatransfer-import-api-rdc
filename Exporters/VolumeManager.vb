Namespace kCura.WinEDDS
	Public Class VolumeManager

#Region "Members"

		Private _settings As ExportFile
		Private _imageFileWriter As System.IO.StreamWriter
		Private _nativeFileWriter As System.IO.StreamWriter
		Private _errorWriter As System.IO.StreamWriter

		Private _nativeFileWriterPosition As Int64 = 0
		Private _imageFileWriterPosition As Int64 = 0
		Private _errorWriterPosition As Int64 = 0

		Private _currentVolumeNumber As Int32
		Private _currentSubdirectoryNumber As Int32

		Private _currentVolumeSize As Int64
		Private _currentImageSubdirectorySize As Int64
		Private _currentNativeSubdirectorySize As Int64
		Private _currentTextSubdirectorySize As Int64
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
		Private _statistics As kCura.WinEDDS.Statistics
		Private _totalExtractedTextFileLength As Int64 = 0
		Private _halt As Boolean = False
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

		Public Property Halt() As Boolean
			Get
				Return _halt
			End Get
			Set(ByVal value As Boolean)
				_halt = value
			End Set
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

		Private ReadOnly Property CurrentFullTextSubdirectoryLabel() As String
			Get
				Return _settings.VolumeInfo.SubdirectoryFullTextPrefix & _currentSubdirectoryNumber.ToString.PadLeft(_subdirectoryLabelPaddingWidth, "0"c)
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

		Public Sub New(ByVal settings As ExportFile, ByVal rootDirectory As String, ByVal overWriteFiles As Boolean, ByVal totalFiles As Int64, ByVal parent As WinEDDS.Exporter, ByVal downloadHandler As FileDownloader, ByVal t As kCura.Utility.Timekeeper, ByVal statistics As kCura.WinEDDS.ExportStatistics)
			_settings = settings
			_statistics = statistics
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
			If Not (_volumeLabelPaddingWidth <= settings.VolumeDigitPadding AndAlso _subdirectoryLabelPaddingWidth <= settings.SubdirectoryDigitPadding) Then
				Dim message As New System.Text.StringBuilder
				If _volumeLabelPaddingWidth > settings.VolumeDigitPadding Then message.AppendFormat("The selected volume padding of {0} is less than the recommended volume padding {1} for this export" & vbNewLine, settings.VolumeDigitPadding, _volumeLabelPaddingWidth)
				If _subdirectoryLabelPaddingWidth > settings.SubdirectoryDigitPadding Then message.AppendFormat("The selected subdirectory padding of {0} is less than the recommended subdirectory padding {1} for this export" & vbNewLine, settings.SubdirectoryDigitPadding, _subdirectoryLabelPaddingWidth)
				message.Append("Continue with this selection?")
				Select Case MsgBox(message.ToString, MsgBoxStyle.OKCancel)
					Case MsgBoxResult.Cancel
						parent.Shutdown()
						Exit Sub
				End Select
			End If
			_subdirectoryLabelPaddingWidth = settings.SubdirectoryDigitPadding
			_volumeLabelPaddingWidth = settings.VolumeDigitPadding
			_currentVolumeSize = 0
			_currentImageSubdirectorySize = 0
			_currentTextSubdirectorySize = 0
			_currentNativeSubdirectorySize = 0
			_downloadManager = downloadHandler
			_parent = parent
			Dim loadFilePath As String = Me.LoadFileDestinationPath
			If Not Me.Settings.Overwrite AndAlso System.IO.File.Exists(loadFilePath) Then
				MsgBox(String.Format("Overwrite not selected and file '{0}' exists.", loadFilePath))
				_parent.Shutdown()
				Exit Sub
			End If
			If Me.Settings.ExportImages AndAlso Not Me.Settings.Overwrite AndAlso System.IO.File.Exists(Me.ImageFileDestinationPath) Then
				MsgBox(String.Format("Overwrite not selected and file '{0}' exists.", Me.ImageFileDestinationPath))
				_parent.Shutdown()
				Exit Sub
			End If
			_encoding = Me.Settings.LoadFileEncoding

			If settings.ExportNative OrElse settings.SelectedViewFields.Length > 0 Then _nativeFileWriter = New System.IO.StreamWriter(loadFilePath, False, _encoding)
			Dim imageFilePath As String = Me.ImageFileDestinationPath
			If Me.Settings.ExportImages Then
				If Not Me.Settings.Overwrite AndAlso System.IO.File.Exists(imageFilePath) Then
					Throw New System.Exception(String.Format("Overwrite not selected and file '{0}' exists.", imageFilePath))
				End If
				_imageFileWriter = New System.IO.StreamWriter(imageFilePath, False, _encoding)
			End If
		End Sub




		Public ReadOnly Property LoadFileDestinationPath() As String
			Get
				Return Me.Settings.FolderPath.TrimEnd("\"c) & "\" & Me.Settings.LoadFilesPrefix & "_export." & Me.Settings.LoadFileExtension
			End Get
		End Property

		Public ReadOnly Property ImageFileDestinationPath() As String
			Get
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
				Return Me.Settings.FolderPath.TrimEnd("\"c) & "\" & Me.Settings.LoadFilesPrefix & "_export" & logFileExension
			End Get
		End Property

		Public ReadOnly Property ErrorDestinationPath() As String
			Get
				If _errorFileLocation Is Nothing OrElse _errorFileLocation = "" Then
					_errorFileLocation = System.IO.Path.GetTempFileName()
				End If
				Return _errorFileLocation
			End Get
		End Property

		Private Sub LogFileExportError(ByVal type As ExportFileType, ByVal recordIdentifier As String, ByVal fileLocation As String, ByVal errorText As String)
			Try
				If _errorWriter Is Nothing Then
					_errorWriter = New System.IO.StreamWriter(Me.ErrorDestinationPath, False, _encoding)
					_errorWriter.WriteLine("""File Type"",""Document Identifier"",""File Guid"",""Error Description""")
				End If
				_errorWriter.WriteLine(String.Format("""{0}"",""{1}"",""{2}"",""{3}""", type.ToString, recordIdentifier, fileLocation, kCura.Utility.Strings.ToCsvCellContents(errorText)))
			Catch ex As System.IO.IOException
				Throw New kCura.WinEDDS.Exceptions.FileWriteException(Exceptions.FileWriteException.DestinationFile.Errors, ex)
			End Try
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

		Public Function ExportArtifact(ByVal artifact As Exporters.ObjectExportInfo) As Int64
			Dim tries As Int32 = kCura.Utility.Config.Settings.IoErrorNumberOfRetries
			While tries > 0 And Not Me.Halt
				tries -= 1
				Try
					Return Me.ExportArtifact(artifact, tries < (kCura.Utility.Config.Settings.IoErrorNumberOfRetries - 1))
					Exit While
				Catch ex As kCura.WinEDDS.Exceptions.ExportBaseException
					If tries = 0 Then Throw
					_parent.WriteWarning(String.Format("Error writing data file(s) for document {0}", artifact.IdentifierValue))
					_parent.WriteWarning(String.Format("Actual error: {0}", ex.ToString))
					If tries <> kCura.Utility.Config.Settings.IoErrorNumberOfRetries - 1 Then
						_parent.WriteWarning(String.Format("Waiting {0} seconds to retry", kCura.Utility.Config.Settings.IoErrorWaitTimeInSeconds))
						System.Threading.Thread.CurrentThread.Join(kCura.Utility.Config.Settings.IoErrorWaitTimeInSeconds * 1000)
					Else
						_parent.WriteWarning("Retrying now")
					End If
				End Try
			End While

		End Function

		Private Sub ReInitializeAllStreams()
			If Not _nativeFileWriter Is Nothing Then _nativeFileWriter = Me.ReInitializeStream(_nativeFileWriter, _nativeFileWriterPosition, Me.LoadFileDestinationPath, _encoding)
			If Not _imageFileWriter Is Nothing Then _imageFileWriter = Me.ReInitializeStream(_imageFileWriter, _imageFileWriterPosition, Me.ImageFileDestinationPath, _encoding)
			If Not _errorWriter Is Nothing Then _errorWriter = Me.ReInitializeStream(_errorWriter, _errorWriterPosition, Me.ErrorDestinationPath, System.Text.Encoding.Default)
		End Sub

		Private Function ReInitializeStream(ByVal brokenStream As System.IO.StreamWriter, ByVal position As Int64, ByVal filepath As String, ByVal encoding As System.Text.Encoding) As System.IO.StreamWriter
			If brokenStream Is Nothing Then Exit Function
			Try
				brokenStream.Close()
			Catch ex As Exception
			End Try
			Try
				brokenStream = Nothing
			Catch ex As Exception
			End Try
			Try
				Dim retval As New System.IO.StreamWriter(filepath, True, encoding)
				retval.BaseStream.Position = position
				Return retval
			Catch ex As System.IO.IOException
				Throw New kCura.WinEDDS.Exceptions.FileWriteException(Exceptions.FileWriteException.DestinationFile.Generic, ex)
			End Try
		End Function

		Private Function ExportArtifact(ByVal artifact As Exporters.ObjectExportInfo, ByVal isRetryAttempt As Boolean) As Int64
			If isRetryAttempt Then Me.ReInitializeAllStreams()
			Dim totalFileSize As Int64 = 0
			Dim loadFileBytes As Int64 = 0
			Dim extracteTextFileSizeForVolume As Int64 = 0
			Dim image As Exporters.ImageExportInfo
			Dim imageSuccess As Boolean = True
			Dim nativeSuccess As Boolean = True
			Dim updateVolumeAfterExport As Boolean = False
			Dim updateSubDirectoryAfterExport As Boolean = False
			If Me.Settings.ExportImages Then
				For Each image In artifact.Images
					_timekeeper.MarkStart("VolumeManager_DownloadImage")
					Try
						If Me.Settings.VolumeInfo.CopyFilesFromRepository Then
							totalFileSize += Me.DownloadImage(image)
						End If
						image.HasBeenCounted = True
					Catch ex As System.Exception
						image.TempLocation = ""
						Me.LogFileExportError(ExportFileType.Image, artifact.IdentifierValue, image.FileGuid, ex.ToString)
						imageSuccess = False
					End Try
					_timekeeper.MarkEnd("VolumeManager_DownloadImage")
				Next
			End If
			Dim imageCount As Long = artifact.Images.Count
			Dim successfulRollup As Boolean = True
			If artifact.Images.Count > 0 AndAlso (Me.Settings.TypeOfImage = ExportFile.ImageType.MultiPageTiff OrElse Me.Settings.TypeOfImage = ExportFile.ImageType.Pdf) Then
				Dim imageList(artifact.Images.Count - 1) As String
				For i As Int32 = 0 To imageList.Length - 1
					imageList(i) = DirectCast(artifact.Images(i), Exporters.ImageExportInfo).TempLocation
				Next
				Dim tempLocation As String = Me.Settings.FolderPath.TrimEnd("\"c) & "\" & System.Guid.NewGuid.ToString & ".tmp"
				Dim converter As New kCura.Utility.Image
				Try
					Select Case Me.Settings.TypeOfImage
						Case ExportFile.ImageType.MultiPageTiff
							converter.ConvertTIFFsToMultiPage(imageList, tempLocation)
						Case ExportFile.ImageType.Pdf
							If Not tempLocation Is Nothing AndAlso Not tempLocation = "" Then converter.ConvertImagesToMultiPagePdf(imageList, tempLocation)
					End Select
					imageCount = 1
					For Each imageLocation As String In imageList
						kCura.Utility.File.Delete(imageLocation)
					Next
					Dim ext As String = ""
					Select Case Me.Settings.TypeOfImage
						Case ExportFile.ImageType.Pdf
							ext = ".pdf"
						Case ExportFile.ImageType.MultiPageTiff
							ext = ".tif"
					End Select
					Dim currentTempLocation As String = Me.GetImageExportLocation(image)
					currentTempLocation = currentTempLocation.Substring(0, currentTempLocation.LastIndexOf("."))
					currentTempLocation &= ext
					DirectCast(artifact.Images(0), Exporters.ImageExportInfo).TempLocation = currentTempLocation
					currentTempLocation = DirectCast(artifact.Images(0), Exporters.ImageExportInfo).FileName
					currentTempLocation = currentTempLocation.Substring(0, currentTempLocation.LastIndexOf("."))
					currentTempLocation &= ext
					DirectCast(artifact.Images(0), Exporters.ImageExportInfo).FileName = currentTempLocation
					Dim location As String = DirectCast(artifact.Images(0), Exporters.ImageExportInfo).TempLocation
					If System.IO.File.Exists(DirectCast(artifact.Images(0), Exporters.ImageExportInfo).TempLocation) Then
						If Me.Settings.Overwrite Then
							kCura.Utility.File.Delete(DirectCast(artifact.Images(0), Exporters.ImageExportInfo).TempLocation)
							kCura.Utility.File.Move(tempLocation, DirectCast(artifact.Images(0), Exporters.ImageExportInfo).TempLocation)
						Else
							_parent.WriteWarning("File exists - file copy skipped: " & DirectCast(artifact.Images(0), Exporters.ImageExportInfo).TempLocation)
						End If
					Else
						kCura.Utility.File.Move(tempLocation, DirectCast(artifact.Images(0), Exporters.ImageExportInfo).TempLocation)
					End If
				Catch ex As kCura.Utility.Image.ImageRollupException
					successfulRollup = False
					Try
						If Not tempLocation Is Nothing AndAlso Not tempLocation = "" Then kCura.Utility.File.Delete(tempLocation)
						_parent.WriteImgProgressError(artifact, ex.ImageIndex, ex, "Document exported in single-page image mode.")
					Catch ioex As System.IO.IOException
						Throw New kCura.WinEDDS.Exceptions.FileWriteException(Exceptions.FileWriteException.DestinationFile.Errors, ioex)
					End Try
				End Try
			End If

			If Me.Settings.ExportNative Then
				_timekeeper.MarkStart("VolumeManager_DownloadNative")
				Try
					If Me.Settings.VolumeInfo.CopyFilesFromRepository Then
						Dim downloadSize As Int64 = Me.DownloadNative(artifact)
						If Not artifact.HasCountedNative Then totalFileSize += downloadSize
					End If
					artifact.HasCountedNative = True
				Catch ex As System.Exception
					Me.LogFileExportError(ExportFileType.Native, artifact.IdentifierValue, artifact.NativeFileGuid, ex.ToString)
				End Try
				_timekeeper.MarkEnd("VolumeManager_DownloadNative")
			End If
			Dim tempLocalFullTextFilePath As String = ""
			Dim tempLocalIproFullTextFilePath As String = ""

			Dim extractedTextFileLength As Long = 0
			If Me.Settings.ExportFullText Then
				tempLocalFullTextFilePath = Me.DownloadTextFieldAsFile(artifact, Me.Settings.SelectedTextField)
				Dim len As Int64 = kCura.Utility.File.Length(tempLocalFullTextFilePath)
				If Me.Settings.ExportFullTextAsFile Then
					If Not artifact.HasCountedTextFile Then
						totalFileSize += len
						extracteTextFileSizeForVolume += len
					End If
					artifact.HasCountedTextFile = True
				End If
				artifact.HasFullText = (len > 0)
			End If

			If Me.Settings.LogFileFormat = LoadFileType.FileFormat.IPRO_FullText Then
				If Me.Settings.SelectedTextField Is Nothing OrElse Me.Settings.SelectedTextField.Category <> DynamicFields.Types.FieldCategory.FullText Then
					tempLocalIproFullTextFilePath = System.IO.Path.GetTempFileName
					Dim tries As Int32 = 20
					Dim start As Int64 = System.DateTime.Now.Ticks
					While tries > 0 AndAlso Not Me.Halt
						tries -= 1
						Try
							_downloadManager.DownloadFullTextFile(tempLocalIproFullTextFilePath, artifact.ArtifactID, _settings.CaseInfo.ArtifactID.ToString)
							Exit While
						Catch ex As System.Exception
							If tries = 19 Then
								_parent.WriteStatusLine(Windows.Process.EventType.Warning, "Second attempt to download full text for document " & artifact.IdentifierValue, True)
							ElseIf tries > 0 Then
								_parent.WriteStatusLine(Windows.Process.EventType.Warning, "Additional attempt to download full text for document " & artifact.IdentifierValue & " failed - retrying in 30 seconds", True)
								System.Threading.Thread.CurrentThread.Join(30000)
							Else
								Throw
							End If
						End Try
					End While
					_statistics.MetadataTime += System.Math.Max(System.DateTime.Now.Ticks - start, 1)
				ElseIf Me.Settings.SelectedTextField.Category = DynamicFields.Types.FieldCategory.FullText Then
					tempLocalIproFullTextFilePath = String.Copy(tempLocalFullTextFilePath)
				End If
			End If

			Dim textCount As Int32 = 0
			If Me.Settings.ExportFullTextAsFile AndAlso artifact.HasFullText Then textCount += 1
			If totalFileSize + _currentVolumeSize > Me.VolumeMaxSize Then
				If _currentVolumeSize = 0 Then
					updateVolumeAfterExport = True
				Else
					Me.UpdateVolume()
				End If
			ElseIf imageCount + _currentImageSubdirectorySize > Me.SubDirectoryMaxSize Then
				Me.UpdateSubdirectory()
			ElseIf artifact.NativeCount + _currentNativeSubdirectorySize > Me.SubDirectoryMaxSize Then
				Me.UpdateSubdirectory()
			ElseIf textCount + _currentTextSubdirectorySize > Me.SubDirectoryMaxSize Then
				Me.UpdateSubdirectory()
			End If
			If Me.Settings.ExportImages Then
				_timekeeper.MarkStart("VolumeManager_ExportImages")
				Me.ExportImages(artifact.Images, tempLocalIproFullTextFilePath, successfulRollup)
				_timekeeper.MarkEnd("VolumeManager_ExportImages")
			End If
			Dim nativeCount As Int32 = 0
			Dim nativeLocation As String = ""
			If Me.Settings.ExportNative AndAlso Me.Settings.VolumeInfo.CopyFilesFromRepository Then
				Dim nativeFileName As String = Me.GetNativeFileName(artifact)
				Dim localFilePath As String = Me.GetLocalNativeFilePath(artifact, nativeFileName)
				_timekeeper.MarkStart("VolumeManager_ExportNative")
				Me.ExportNative(localFilePath, artifact.NativeFileGuid, artifact.ArtifactID, nativeFileName, artifact.NativeTempLocation)
				_timekeeper.MarkEnd("VolumeManager_ExportNative")
				If artifact.NativeTempLocation = "" Then
					nativeLocation = ""
				Else
					nativeCount = 1
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
			Try
				If Not _hasWrittenColumnHeaderString AndAlso Not _nativeFileWriter Is Nothing Then
					_nativeFileWriter.Write(_columnHeaderString)
					_hasWrittenColumnHeaderString = True
				End If
				Me.UpdateLoadFile(artifact.DataRow, artifact.HasFullText, artifact.ArtifactID, nativeLocation, tempLocalFullTextFilePath, artifact, extractedTextFileLength)
			Catch ex As System.IO.IOException
				Throw New kCura.WinEDDS.Exceptions.FileWriteException(Exceptions.FileWriteException.DestinationFile.Load, ex)
			End Try

			_parent.DocumentsExported += artifact.DocCount
			_currentVolumeSize += totalFileSize
			If Me.Settings.VolumeInfo.CopyFilesFromRepository Then
				_currentNativeSubdirectorySize += artifact.NativeCount
				If Me.Settings.ExportFullTextAsFile AndAlso artifact.HasFullText Then _currentTextSubdirectorySize += 1
				_currentImageSubdirectorySize += imageCount
			End If
			If updateSubDirectoryAfterExport Then Me.UpdateSubdirectory()
			If updateVolumeAfterExport Then Me.UpdateVolume()
			_parent.WriteUpdate("Document " & artifact.IdentifierValue & " exported.", False)

			Try
				If Not _nativeFileWriter Is Nothing Then _nativeFileWriter.Flush()
			Catch ex As Exception
				Throw New Exceptions.FileWriteException(Exceptions.FileWriteException.DestinationFile.Load, ex)
			End Try
			Try
				If Not _imageFileWriter Is Nothing Then _imageFileWriter.Flush()
			Catch ex As Exception
				Throw New Exceptions.FileWriteException(Exceptions.FileWriteException.DestinationFile.Image, ex)
			End Try
			Try
				If Not _errorWriter Is Nothing Then _errorWriter.Flush()
			Catch ex As Exception
				Throw New Exceptions.FileWriteException(Exceptions.FileWriteException.DestinationFile.Errors, ex)
			End Try
			If Not _nativeFileWriter Is Nothing Then
				_nativeFileWriterPosition = _nativeFileWriter.BaseStream.Position
				loadFileBytes += kCura.Utility.File.GetFileSize(DirectCast(_nativeFileWriter.BaseStream, System.IO.FileStream).Name)
			End If
			If Not _imageFileWriter Is Nothing Then
				_imageFileWriterPosition = _imageFileWriter.BaseStream.Position
				loadFileBytes += kCura.Utility.File.GetFileSize(DirectCast(_imageFileWriter.BaseStream, System.IO.FileStream).Name)
			End If
			_totalExtractedTextFileLength += extractedTextFileLength
			_statistics.MetadataBytes = loadFileBytes + _totalExtractedTextFileLength
			_statistics.FileBytes += totalFileSize - extracteTextFileSizeForVolume
			If Not _errorWriter Is Nothing Then _errorWriterPosition = _errorWriter.BaseStream.Position
			If Not Me.Settings.VolumeInfo.CopyFilesFromRepository Then
				Return 0
			Else
				Return imageCount + nativeCount
			End If
		End Function

		Private Function DownloadTextFieldAsFile(ByVal artifact As WinEDDS.Exporters.ObjectExportInfo, ByVal field As WinEDDS.ViewFieldInfo) As String
			Dim tempLocalFullTextFilePath As String = System.IO.Path.GetTempFileName
			Dim tries As Int32 = 20
			Dim start As Int64 = System.DateTime.Now.Ticks
			While tries > 0 AndAlso Not Me.Halt
				tries -= 1
				Try
					If Me.Settings.ArtifactTypeID = kCura.EDDS.Types.ArtifactType.Document AndAlso field.Category = DynamicFields.Types.FieldCategory.FullText Then
						_downloadManager.DownloadFullTextFile(tempLocalFullTextFilePath, artifact.ArtifactID, _settings.CaseInfo.ArtifactID.ToString)
					Else
						_downloadManager.DownloadLongTextFile(tempLocalFullTextFilePath, artifact.ArtifactID, field, _settings.CaseInfo.ArtifactID.ToString)
					End If
					Exit While
				Catch ex As System.Exception
					If tries = 19 Then
						_parent.WriteStatusLine(Windows.Process.EventType.Warning, "Second attempt to download full text for document " & artifact.IdentifierValue, True)
					ElseIf tries > 0 Then
						_parent.WriteStatusLine(Windows.Process.EventType.Warning, "Additional attempt to download full text for document " & artifact.IdentifierValue & " failed - retrying in 30 seconds", True)
						System.Threading.Thread.CurrentThread.Join(30000)
					Else
						Throw
					End If
				End Try
			End While
			_statistics.MetadataTime += System.Math.Max(System.DateTime.Now.Ticks - start, 1)
			Return tempLocalFullTextFilePath
		End Function

		Private Function GetLocalNativeFilePath(ByVal doc As Exporters.ObjectExportInfo, ByVal nativeFileName As String) As String
			Dim localFilePath As String = Me.Settings.FolderPath
			If localFilePath.Chars(localFilePath.Length - 1) <> "\"c Then localFilePath &= "\"
			localFilePath &= Me.CurrentVolumeLabel & "\" & Me.CurrentNativeSubdirectoryLabel & "\"
			If Not System.IO.Directory.Exists(localFilePath) Then System.IO.Directory.CreateDirectory(localFilePath)
			Return localFilePath & nativeFileName
		End Function

		Private Function GetLocalTextFilePath(ByVal doc As Exporters.ObjectExportInfo) As String
			Dim localFilePath As String = Me.Settings.FolderPath
			If localFilePath.Chars(localFilePath.Length - 1) <> "\"c Then localFilePath &= "\"
			localFilePath &= Me.CurrentVolumeLabel & "\" & Me.CurrentFullTextSubdirectoryLabel & "\"
			If Not System.IO.Directory.Exists(localFilePath) Then System.IO.Directory.CreateDirectory(localFilePath)
			Return localFilePath & doc.FullTextFileName(Me.NameTextFilesAfterIdentifier)
		End Function

		Private Function GetNativeFileName(ByVal doc As Exporters.ObjectExportInfo) As String
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


		Public Sub ExportImages(ByVal images As System.Collections.ArrayList, ByVal localFullTextPath As String, ByVal successfulRollup As Boolean)
			Dim image As WinEDDS.Exporters.ImageExportInfo
			Dim i As Int32 = 0
			Dim fullTextReader As System.IO.StreamReader = Nothing
			Dim localFilePath As String = Me.Settings.FolderPath
			Dim subfolderPath As String = Me.CurrentVolumeLabel & "\" & Me.CurrentImageSubdirectoryLabel & "\"
			Dim pageOffset As Long
			If localFilePath.Chars(localFilePath.Length - 1) <> "\"c Then localFilePath &= "\"
			localFilePath &= subfolderPath
			If Not System.IO.Directory.Exists(localFilePath) AndAlso Me.Settings.VolumeInfo.CopyFilesFromRepository Then System.IO.Directory.CreateDirectory(localFilePath)
			Try
				If Me.Settings.LogFileFormat = LoadFileType.FileFormat.IPRO_FullText Then
					If System.IO.File.Exists(localFullTextPath) Then
						fullTextReader = New System.IO.StreamReader(localFullTextPath, _encoding, True)
					End If
				End If
				If images.Count > 0 AndAlso (Me.Settings.TypeOfImage = ExportFile.ImageType.MultiPageTiff OrElse Me.Settings.TypeOfImage = ExportFile.ImageType.Pdf) AndAlso successfulRollup Then
					Dim marker As Exporters.ImageExportInfo = DirectCast(images(0), Exporters.ImageExportInfo)
					Me.ExportDocumentImage(localFilePath & marker.FileName, marker.FileGuid, marker.ArtifactID, marker.BatesNumber, marker.TempLocation)
					Dim copyfile As String
					Select Case Me.Settings.TypeOfExportedFilePath
						Case ExportFile.ExportedFilePathType.Absolute
							copyfile = localFilePath & marker.FileName
						Case ExportFile.ExportedFilePathType.Relative
							copyfile = ".\" & subfolderPath & marker.FileName
						Case ExportFile.ExportedFilePathType.Prefix
							copyfile = Me.Settings.FilePrefix.TrimEnd("\"c) & "\" & subfolderPath & marker.FileName
					End Select
					If Me.Settings.LogFileFormat = LoadFileType.FileFormat.Opticon Then
						Me.CreateImageLogEntry(marker.BatesNumber, copyfile, localFilePath, 1, fullTextReader, localFullTextPath <> "", Int64.MinValue, images.Count)
					Else
						For j As Int32 = 0 To images.Count - 1
							If (j = 0 AndAlso DirectCast(images(j), Exporters.ImageExportInfo).PageOffset.IsNull) OrElse j = images.Count - 1 Then
								pageOffset = Int64.MinValue
							Else
								Dim nextImage As Exporters.ImageExportInfo = DirectCast(images(j + 1), Exporters.ImageExportInfo)
								If nextImage.PageOffset.IsNull Then
									pageOffset = Int64.MinValue
								Else
									pageOffset = nextImage.PageOffset.Value
								End If
							End If
							image = DirectCast(images(j), WinEDDS.Exporters.ImageExportInfo)
							Me.CreateImageLogEntry(image.BatesNumber, copyfile, localFilePath, j + 1, fullTextReader, localFullTextPath <> "", pageOffset, images.Count)
						Next
					End If
					marker.TempLocation = copyfile
				Else
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
						If Me.Settings.VolumeInfo.CopyFilesFromRepository Then
							Me.ExportDocumentImage(localFilePath & image.FileName, image.FileGuid, image.ArtifactID, image.BatesNumber, image.TempLocation)
							Dim copyfile As String
							Select Case Me.Settings.TypeOfExportedFilePath
								Case ExportFile.ExportedFilePathType.Absolute
									copyfile = localFilePath & image.FileName
								Case ExportFile.ExportedFilePathType.Relative
									copyfile = ".\" & subfolderPath & image.FileName
								Case ExportFile.ExportedFilePathType.Prefix
									copyfile = Me.Settings.FilePrefix.TrimEnd("\"c) & "\" & subfolderPath & image.FileName
							End Select
							Me.CreateImageLogEntry(image.BatesNumber, copyfile, localFilePath, i + 1, fullTextReader, localFullTextPath <> "", pageOffset, images.Count)
							image.TempLocation = copyfile
						Else
							Me.CreateImageLogEntry(image.BatesNumber, image.SourceLocation, image.SourceLocation, i + 1, fullTextReader, localFullTextPath <> "", pageOffset, images.Count)
						End If
						i += 1
					Next
				End If

			Catch ex As System.Exception
				If Not fullTextReader Is Nothing Then fullTextReader.Close()
				Throw
			End Try
			If Not fullTextReader Is Nothing Then fullTextReader.Close()
		End Sub

		Private Function DownloadImage(ByVal image As Exporters.ImageExportInfo) As Int64
			If image.FileGuid = "" Then Return 0
			Dim start As Int64 = System.DateTime.Now.Ticks
			Dim tempFile As String = Me.GetImageExportLocation(image)
			'If Me.Settings.TypeOfImage = ExportFile.ImageType.Pdf Then
			'	tempFile = System.IO.Path.GetTempFileName
			'	kCura.Utility.File.Delete(tempFile)
			'End If
			If System.IO.File.Exists(tempFile) Then
				If _settings.Overwrite Then
					kCura.Utility.File.Delete(tempFile)
					_parent.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Overwriting image for {0}.", image.BatesNumber), False)
				Else
					_parent.WriteWarning(String.Format("{0} already exists. Skipping file export.", tempFile))
					Return 0
				End If
			End If
			Dim tries As Int32 = 20
			While tries > 0 AndAlso Not Me.Halt
				tries -= 1
				Try
					_downloadManager.DownloadFileForDocument(tempFile, image.FileGuid, image.SourceLocation, image.ArtifactID, _settings.CaseArtifactID.ToString)
					image.TempLocation = tempFile
					Exit While
				Catch ex As System.Exception
					If tries = 19 Then
						_parent.WriteStatusLine(Windows.Process.EventType.Warning, "Second attempt to download image " & image.BatesNumber & " - exact error: " & ex.ToString, True)
					ElseIf tries > 0 Then
						_parent.WriteStatusLine(Windows.Process.EventType.Warning, "Additional attempt to download image " & image.BatesNumber & " failed - retrying in 30 seconds - exact error: " & ex.ToString, True)
						System.Threading.Thread.CurrentThread.Join(30000)
					Else
						Throw
					End If
				End Try
			End While
			_statistics.FileTime += System.Math.Max(System.DateTime.Now.Ticks - start, 1)
			Return kCura.Utility.File.Length(tempFile)
		End Function

		Private Sub ExportDocumentImage(ByVal fileName As String, ByVal fileGuid As String, ByVal artifactID As Int32, ByVal batesNumber As String, ByVal tempFileLocation As String)
			If Not tempFileLocation = "" AndAlso Not tempFileLocation.ToLower = fileName.ToLower Then
				If System.IO.File.Exists(fileName) Then
					If _settings.Overwrite Then
						kCura.Utility.File.Delete(fileName)
						_parent.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Overwriting document image {0}.", batesNumber), False)
						kCura.Utility.File.Move(tempFileLocation, fileName)
					Else
						_parent.WriteWarning(String.Format("{0}.tif already exists. Skipping file export.", batesNumber))
					End If
				Else
					_timekeeper.MarkStart("VolumeManager_ExportDocumentImage_WriteStatus")
					_parent.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Now exporting document image {0}.", batesNumber), False)
					_timekeeper.MarkEnd("VolumeManager_ExportDocumentImage_WriteStatus")
					_timekeeper.MarkStart("VolumeManager_ExportDocumentImage_MoveFile")
					kCura.Utility.File.Move(tempFileLocation, fileName)
					_timekeeper.MarkEnd("VolumeManager_ExportDocumentImage_MoveFile")
				End If
				_timekeeper.MarkStart("VolumeManager_ExportDocumentImage_WriteStatus")
				_parent.WriteStatusLine(Windows.Process.EventType.Status, String.Format("Finished exporting document image {0}.", batesNumber), False)
				_timekeeper.MarkEnd("VolumeManager_ExportDocumentImage_WriteStatus")
			End If
			'_parent.DocumentsExported += 1
		End Sub

		Private Function GetLfpFullTextTransform(ByVal c As Char) As String
			Select Case c
				Case ChrW(10), " "c
					Return "|0|0|0|0^"
				Case ","c
					Return ""
				Case Else
					Return c.ToString
			End Select
		End Function
		Private Sub CreateImageLogEntry(ByVal batesNumber As String, ByVal copyFile As String, ByVal pathToImage As String, ByVal pageNumber As Int32, ByVal fullTextReader As System.IO.StreamReader, ByVal expectingTextForPage As Boolean, ByVal pageOffset As Long, ByVal numberOfImages As Int32)
			Dim fullTextGuid As String
			Dim fullText As String
			Try
				Select Case _settings.LogFileFormat
					Case LoadFileType.FileFormat.Opticon
						Me.WriteOpticonLine(batesNumber, pageNumber = 1, copyFile, numberOfImages)
					Case LoadFileType.FileFormat.IPRO
						Me.WriteIproImageLine(batesNumber, pageNumber, copyFile)
					Case LoadFileType.FileFormat.IPRO_FullText
						Dim currentPageFirstByteNumber As Long
						If fullTextReader Is Nothing Then
							If pageNumber = 1 AndAlso expectingTextForPage Then _parent.WriteWarning(String.Format("Could not retrieve full text for document '{0}'", batesNumber))
						Else
							Dim pageText As New System.Text.StringBuilder
							If pageNumber = 1 Then
								currentPageFirstByteNumber = 0
							Else
								currentPageFirstByteNumber = fullTextReader.BaseStream.Position
							End If
							_imageFileWriter.Write("FT,")
							_imageFileWriter.Write(batesNumber)
							_imageFileWriter.Write(",1,1,")
							Select Case pageOffset
								Case Int64.MinValue
									Dim c As Int32 = fullTextReader.Read
									While c <> -1
										_imageFileWriter.Write(Me.GetLfpFullTextTransform(ChrW(c)))
										c = fullTextReader.Read
									End While
								Case Else
									Dim i As Int32 = 0
									Dim c As Int32 = fullTextReader.Read
									While i < pageOffset AndAlso c <> -1
										_imageFileWriter.Write(Me.GetLfpFullTextTransform(ChrW(c)))
										c = fullTextReader.Read
										i += 1
									End While
							End Select
							_imageFileWriter.Write(vbNewLine)
						End If
						_imageFileWriter.Flush()
						Me.WriteIproImageLine(batesNumber, pageNumber, copyFile)
				End Select
			Catch ex As System.IO.IOException
				Throw New kCura.WinEDDS.Exceptions.FileWriteException(Exceptions.FileWriteException.DestinationFile.Image, ex)
			End Try
		End Sub

		Private Sub WriteIproImageLine(ByVal batesNumber As String, ByVal pageNumber As Int32, ByVal fullFilePath As String)
			Dim linefactory As New Exporters.LineFactory.SimpleIproImageLineFactory(batesNumber, pageNumber, fullFilePath, Me.CurrentVolumeLabel, Me.Settings.TypeOfImage)
			linefactory.WriteLine(_imageFileWriter)
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
			If Not tempLocation = "" AndAlso Not tempLocation.ToLower = exportFileName.ToLower AndAlso Me.Settings.VolumeInfo.CopyFilesFromRepository Then
				If System.IO.File.Exists(exportFileName) Then
					If _settings.Overwrite Then
						kCura.Utility.File.Delete(exportFileName)
						_parent.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Overwriting document {0}.", systemFileName), False)
						kCura.Utility.File.Move(tempLocation, exportFileName)
					Else
						_parent.WriteWarning(String.Format("{0} already exists. Skipping file export.", systemFileName))
					End If
				Else
					_timekeeper.MarkStart("VolumeManager_ExportNative_WriteStatus")
					_parent.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Now exporting document {0}.", systemFileName), False)
					_timekeeper.MarkEnd("VolumeManager_ExportNative_WriteStatus")
					_timekeeper.MarkStart("VolumeManager_ExportNative_MoveFile")
					kCura.Utility.File.Move(tempLocation, exportFileName)
					_timekeeper.MarkEnd("VolumeManager_ExportNative_MoveFile")
				End If
			End If
			_timekeeper.MarkStart("VolumeManager_ExportNative_WriteStatus")
			_timekeeper.MarkEnd("VolumeManager_ExportNative_WriteStatus")
		End Function

		Private Function DownloadNative(ByVal artifact As Exporters.ObjectExportInfo) As Int64
			If Me.Settings.ArtifactTypeID = kCura.EDDS.Types.ArtifactType.Document AndAlso artifact.NativeFileGuid = "" Then Return 0
			If Not artifact.FileID > 0 Then Return 0
			Dim nativeFileName As String = Me.GetNativeFileName(artifact)
			Dim tempFile As String = Me.GetLocalNativeFilePath(artifact, nativeFileName)
			Dim start As Int64 = System.DateTime.Now.Ticks
			If System.IO.File.Exists(tempFile) Then
				If Settings.Overwrite Then
					kCura.Utility.File.Delete(tempFile)
					_parent.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Overwriting document {0}.", nativeFileName), False)
				Else
					_parent.WriteWarning(String.Format("{0} already exists. Skipping file export.", tempFile))
					artifact.NativeTempLocation = tempFile
					Return kCura.Utility.File.Length(tempFile)
				End If
			End If
			Dim tries As Int32 = 20
			While tries > 0 AndAlso Not Me.Halt
				tries -= 1
				Try
					If Me.Settings.ArtifactTypeID = kCura.EDDS.Types.ArtifactType.Document Then
						_downloadManager.DownloadFileForDocument(tempFile, artifact.NativeFileGuid, artifact.NativeSourceLocation, artifact.ArtifactID, _settings.CaseArtifactID.ToString)
					Else
						_downloadManager.DownloadFileForDynamicObject(tempFile, artifact.NativeSourceLocation, artifact.ArtifactID, _settings.CaseArtifactID.ToString, artifact.FileID, Me.Settings.FileField.FieldID)
					End If
					Exit While
				Catch ex As System.Exception
					If tries = 19 Then
						_parent.WriteStatusLine(Windows.Process.EventType.Warning, "Second attempt to download native for document " & artifact.IdentifierValue, True)
					ElseIf tries > 0 Then
						_parent.WriteStatusLine(Windows.Process.EventType.Warning, "Additional attempt to download native for document " & artifact.IdentifierValue & " failed - retrying in 30 seconds", True)
						System.Threading.Thread.CurrentThread.Join(30000)
					Else
						Throw
					End If
				End Try
			End While
			artifact.NativeTempLocation = tempFile
			_statistics.FileTime += System.Math.Max(System.DateTime.Now.Ticks - start, 1)
			Return kCura.Utility.File.Length(tempFile)
		End Function

		Public Sub UpdateLoadFile(ByVal row As System.Data.DataRow, ByVal hasFullText As Boolean, ByVal documentArtifactID As Int32, ByVal nativeLocation As String, ByVal fullTextTempFile As String, ByVal doc As Exporters.ObjectExportInfo, ByRef extractedTextByteCount As Int64)
			If _nativeFileWriter Is Nothing Then Exit Sub
			If Me.Settings.LoadFileIsHtml Then
				Me.UpdateHtmlLoadFile(row, hasFullText, documentArtifactID, nativeLocation, fullTextTempFile, doc, extractedTextByteCount)
				Exit Sub
			End If
			Dim count As Int32
			Dim fieldValue As String
			Dim columnName As String
			Dim location As String = nativeLocation
			For count = 0 To _parent.Columns.Count - 1
				Dim field As WinEDDS.ViewFieldInfo = DirectCast(_parent.Columns(count), WinEDDS.ViewFieldInfo)
				columnName = field.AvfColumnName
				If field.FieldType = DynamicFields.Types.FieldTypeHelper.FieldType.Text Then
					Dim sourceFieldEncoding As System.Text.Encoding
					If field.IsUnicodeEnabled Then
						sourceFieldEncoding = System.Text.Encoding.Unicode
					Else
						sourceFieldEncoding = System.Text.Encoding.Default
					End If
					If Not Me.Settings.SelectedTextField Is Nothing AndAlso Me.Settings.SelectedTextField.AvfId = field.AvfId Then
						Dim bodyText As New System.Text.StringBuilder
						If Not hasFullText Then
							bodyText = New System.Text.StringBuilder("")
							_nativeFileWriter.Write(String.Format("{0}{1}{0}", _settings.QuoteDelimiter, bodyText.ToString))
						Else
							Select Case Me.Settings.ExportFullTextAsFile
								Case True
									Dim localTextPath As String = Me.GetLocalTextFilePath(doc)
									If System.IO.File.Exists(localTextPath) Then
										If _settings.Overwrite Then
											kCura.Utility.File.Delete(localTextPath)
											Dim sw As New System.IO.StreamWriter(localTextPath, False, Me.Settings.TextFileEncoding)
											Dim sr As New System.IO.StreamReader(fullTextTempFile, sourceFieldEncoding, True)
											Dim c As Int32 = sr.Read
											While c <> -1
												sw.Write(ChrW(c))
												c = sr.Read
											End While
											sw.Close()
											sr.Close()
											kCura.Utility.File.Delete(fullTextTempFile)
											_parent.WriteStatusLine(kCura.Windows.Process.EventType.Status, localTextPath & " overwritten", False)
										Else
											_parent.WriteWarning(localTextPath & " already exists. Skipping file export.")
										End If
									Else
										Dim sw As New System.IO.StreamWriter(localTextPath, False, Me.Settings.TextFileEncoding)
										Dim sr As New System.IO.StreamReader(fullTextTempFile, sourceFieldEncoding, True)
										Dim c As Int32 = sr.Read
										While c <> -1
											sw.Write(ChrW(c))
											c = sr.Read
										End While
										sw.Close()
										sr.Close()
										kCura.Utility.File.Delete(fullTextTempFile)
									End If
									Dim textLocation As String
									extractedTextByteCount += kCura.Utility.File.GetFileSize(localTextPath)
									Select Case Me.Settings.TypeOfExportedFilePath
										Case ExportFile.ExportedFilePathType.Absolute
											textLocation = localTextPath
										Case ExportFile.ExportedFilePathType.Relative
											textLocation = ".\" & Me.CurrentVolumeLabel & "\" & Me.CurrentFullTextSubdirectoryLabel & "\" & doc.FullTextFileName(Me.NameTextFilesAfterIdentifier)
										Case ExportFile.ExportedFilePathType.Prefix
											textLocation = Me.Settings.FilePrefix.TrimEnd("\"c) & "\" & Me.CurrentVolumeLabel & "\" & Me.CurrentFullTextSubdirectoryLabel & "\" & doc.FullTextFileName(Me.NameTextFilesAfterIdentifier)
									End Select
									_nativeFileWriter.Write(String.Format("{0}{1}{0}", _settings.QuoteDelimiter, textLocation))
								Case False
									Dim sr As New System.IO.StreamReader(fullTextTempFile, sourceFieldEncoding, True)
									Dim c As Int32 = sr.Read
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
							End Select
							kCura.Utility.File.Delete(fullTextTempFile)
						End If
					Else
						Dim textLocation As String = Me.DownloadTextFieldAsFile(doc, field)
						Dim sr As New System.IO.StreamReader(textLocation, sourceFieldEncoding, True)
						Dim c As Int32 = sr.Read
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
						kCura.Utility.File.Delete(textLocation)
					End If
				Else				 'Handle not full text issue
					Dim val As Object = row(columnName)
					If TypeOf val Is Byte() Then
						val = System.Text.Encoding.Unicode.GetString(DirectCast(val, Byte()))
					End If
					If field.FieldType = DynamicFields.Types.FieldTypeHelper.FieldType.Date Then
						Dim datetime As NullableString = NullableTypes.HelperFunctions.DBNullConvert.ToNullableString(val)
						If datetime.IsNull OrElse datetime.Value = "" Then
							val = ""
						Else
							val = System.DateTime.Parse(datetime.Value, System.Globalization.CultureInfo.InvariantCulture).ToString(field.FormatString)
						End If
					End If
					'System.Web.HttpUtility.HtmlEncode()
					fieldValue = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(NullableTypes.HelperFunctions.DBNullConvert.ToNullableString(val))
					If field.IsMultiValueField Then
						fieldValue = Me.GetMultivalueString(fieldValue)
					ElseIf field.IsCodeOrMulticodeField Then
						fieldValue = System.Web.HttpUtility.HtmlDecode(fieldValue)
						fieldValue = fieldValue.Trim(New Char() {ChrW(11)}).Replace(ChrW(11), _settings.MultiRecordDelimiter)
					End If
					fieldValue = fieldValue.Replace(System.Environment.NewLine, ChrW(10).ToString)
					fieldValue = fieldValue.Replace(ChrW(13), ChrW(10))
					fieldValue = fieldValue.Replace(ChrW(10), _settings.NewlineDelimiter)
					fieldValue = fieldValue.Replace(_settings.QuoteDelimiter, _settings.QuoteDelimiter & _settings.QuoteDelimiter)
					_nativeFileWriter.Write(String.Format("{0}{1}{0}", _settings.QuoteDelimiter, fieldValue))
				End If
				If Not count = _parent.Columns.Count - 1 Then
					_nativeFileWriter.Write(_settings.RecordDelimiter)
				End If
			Next
			If _settings.ExportNative Then
				If Me.Settings.VolumeInfo.CopyFilesFromRepository Then
					_nativeFileWriter.Write(String.Format("{2}{0}{1}{0}", _settings.QuoteDelimiter, location, _settings.RecordDelimiter))
				Else
					_nativeFileWriter.Write(String.Format("{2}{0}{1}{0}", _settings.QuoteDelimiter, doc.NativeSourceLocation, _settings.RecordDelimiter))
				End If
			End If
			_nativeFileWriter.Write(vbNewLine)
		End Sub

		Public Sub UpdateHtmlLoadFile(ByVal row As System.Data.DataRow, ByVal hasFullText As Boolean, ByVal documentArtifactID As Int32, ByVal nativeLocation As String, ByVal fullTextTempFile As String, ByVal doc As Exporters.ObjectExportInfo, ByRef extractedTextByteCount As Int64)
			Dim count As Int32
			Dim fieldValue As String
			Dim columnName As String
			Dim location As String = nativeLocation
			_nativeFileWriter.Write("<tr>")
			For count = 0 To _parent.Columns.Count - 1
				Dim field As WinEDDS.ViewFieldInfo = DirectCast(_parent.Columns(count), WinEDDS.ViewFieldInfo)
				columnName = field.AvfColumnName
				Dim encoding As System.Text.Encoding = System.Text.Encoding.Unicode
				If Not field.IsUnicodeEnabled Then
					encoding = System.Text.Encoding.Default
				End If
				If field.FieldType = DynamicFields.Types.FieldTypeHelper.FieldType.Text Then
					If Not Me.Settings.SelectedTextField Is Nothing AndAlso Me.Settings.SelectedTextField.AvfId = field.AvfId Then
						Dim bodyText As New System.Text.StringBuilder
						If Not hasFullText Then
							bodyText = New System.Text.StringBuilder("")
							_nativeFileWriter.Write(String.Format("<td></td>"))
						Else
							Select Case Me.Settings.ExportFullTextAsFile
								Case True
									Dim localTextPath As String = Me.GetLocalTextFilePath(doc)
									If System.IO.File.Exists(localTextPath) Then
										If _settings.Overwrite Then
											kCura.Utility.File.Delete(localTextPath)
											Dim sw As New System.IO.StreamWriter(localTextPath, False, Me.Settings.TextFileEncoding)
											Dim sr As New System.IO.StreamReader(fullTextTempFile, encoding, True)
											Dim c As Int32 = sr.Read
											While c <> -1
												sw.Write(ChrW(c))
												c = sr.Read
											End While
											sw.Close()
											sr.Close()
											kCura.Utility.File.Delete(fullTextTempFile)
											_parent.WriteStatusLine(kCura.Windows.Process.EventType.Status, localTextPath & " overwritten", False)
										Else
											_parent.WriteWarning(localTextPath & " already exists. Skipping file export.")
										End If
									Else
										Dim sw As New System.IO.StreamWriter(localTextPath, False, Me.Settings.TextFileEncoding)
										Dim sr As New System.IO.StreamReader(fullTextTempFile, encoding, True)
										Dim c As Int32 = sr.Read
										While c <> -1
											sw.Write(ChrW(c))
											c = sr.Read
										End While
										sw.Close()
										sr.Close()
										kCura.Utility.File.Delete(fullTextTempFile)
									End If
									Dim textLocation As String
									extractedTextByteCount += kCura.Utility.File.GetFileSize(localTextPath)
									Select Case Me.Settings.TypeOfExportedFilePath
										Case ExportFile.ExportedFilePathType.Absolute
											textLocation = localTextPath
										Case ExportFile.ExportedFilePathType.Relative
											textLocation = ".\" & Me.CurrentVolumeLabel & "\" & Me.CurrentFullTextSubdirectoryLabel & "\" & doc.FullTextFileName(Me.NameTextFilesAfterIdentifier)
										Case ExportFile.ExportedFilePathType.Prefix
											textLocation = Me.Settings.FilePrefix.TrimEnd("\"c) & "\" & Me.CurrentVolumeLabel & "\" & Me.CurrentFullTextSubdirectoryLabel & "\" & doc.FullTextFileName(Me.NameTextFilesAfterIdentifier)
									End Select
									_nativeFileWriter.Write(String.Format("<td><a style='display:block' href='{0}'>{1}</a></td>", textLocation, "TextFile"))
								Case False
									Dim sr As New System.IO.StreamReader(fullTextTempFile, encoding, True)
									Dim c As Int32 = sr.Read
									_nativeFileWriter.Write("<td>")
									While Not c = -1
										_nativeFileWriter.Write(System.Web.HttpUtility.HtmlEncode(ChrW(c)))
										c = sr.Read
									End While
									_nativeFileWriter.Write("</td>")
									sr.Close()
							End Select
							kCura.Utility.File.Delete(fullTextTempFile)
						End If
					Else
						Dim textLocation As String = Me.DownloadTextFieldAsFile(doc, field)
						Dim sr As New System.IO.StreamReader(textLocation, encoding, True)
						Dim c As Int32 = sr.Read
						_nativeFileWriter.Write("<td>")
						While Not c = -1
							_nativeFileWriter.Write(System.Web.HttpUtility.HtmlEncode(ChrW(c)))
							c = sr.Read
						End While
						_nativeFileWriter.Write("</td>")
						sr.Close()
						kCura.Utility.File.Delete(textLocation)
					End If
				Else
					Dim val As Object = row(columnName)
					If TypeOf val Is Byte() Then
						val = System.Text.Encoding.Unicode.GetString(DirectCast(val, Byte()))
					End If
					If field.FieldType = DynamicFields.Types.FieldTypeHelper.FieldType.Date Then
						Dim datetime As NullableString = NullableTypes.HelperFunctions.DBNullConvert.ToNullableString(val)
						If datetime.IsNull OrElse datetime.Value = "" Then
							val = ""
						Else
							val = System.DateTime.Parse(datetime.Value, System.Globalization.CultureInfo.InvariantCulture).ToString(field.FormatString)
						End If
					End If
					fieldValue = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(NullableTypes.HelperFunctions.DBNullConvert.ToNullableString(val))

					If field.IsMultiValueField Then
						fieldValue = Me.GetMultivalueString(fieldValue)
					ElseIf field.IsCodeOrMulticodeField Then
						fieldValue = System.Web.HttpUtility.HtmlDecode(fieldValue)
						fieldValue = fieldValue.Trim(New Char() {ChrW(11)}).Replace(ChrW(11), _settings.MultiRecordDelimiter)
					End If
					fieldValue = System.Web.HttpUtility.HtmlEncode(fieldValue)
					_nativeFileWriter.Write(String.Format("{0}{1}{2}", "<td>", fieldValue, "</td>"))
				End If
			Next
			If _settings.ExportImages Then
				_nativeFileWriter.Write(String.Format("<td>{0}</td>", Me.GetImagesHtmlString(doc)))
			End If
			If _settings.ExportNative Then
				If Me.Settings.VolumeInfo.CopyFilesFromRepository Then
					_nativeFileWriter.Write(String.Format("<td>{0}</td>", Me.GetNativeHtmlString(doc, location)))
				Else
					_nativeFileWriter.Write(String.Format("<td>{0}</td>", Me.GetNativeHtmlString(doc, doc.NativeSourceLocation)))
				End If
			End If
			_nativeFileWriter.Write("</tr>")
			_nativeFileWriter.Write(vbNewLine)
		End Sub

		Private Function NameTextFilesAfterIdentifier() As Boolean
			If Me.Settings.TypeOfExport = ExportFile.ExportType.Production Then
				Return _parent.ExportNativesToFileNamedFrom = ExportNativeWithFilenameFrom.Identifier
			Else
				Return True
			End If
		End Function
		Private Function GetImagesHtmlString(ByVal doc As Exporters.ObjectExportInfo) As String
			If doc.Images.Count = 0 Then Return ""
			Dim retval As New System.Text.StringBuilder
			For Each image As Exporters.ImageExportInfo In doc.Images
				Dim loc As String = image.TempLocation
				If Not Me.Settings.VolumeInfo.CopyFilesFromRepository Then
					loc = image.SourceLocation
				End If
				retval.AppendFormat("<a style='display:block' href='{0}'>{1}</a>", loc, image.FileName)
				If Me.Settings.TypeOfImage = ExportFile.ImageType.MultiPageTiff OrElse Me.Settings.TypeOfImage = ExportFile.ImageType.Pdf Then Exit For
			Next
			Return retval.ToString
		End Function

		Private Function GetNativeHtmlString(ByVal doc As Exporters.ObjectExportInfo, ByVal location As String) As String
			If doc.NativeCount = 0 Then Return ""
			Dim retval As New System.Text.StringBuilder
			retval.AppendFormat("<a style='display:block' href='{0}'>{1}</a>", location, doc.NativeFileName(Me.Settings.AppendOriginalFileName))
			Return retval.ToString
		End Function

		Private Function GetMultivalueString(ByVal input As String) As String
			Dim xr As New System.Xml.XmlTextReader(New System.IO.StringReader("<objects>" & input & "</objects>"))
			Dim firstTimeThrough As Boolean = True
			Dim sb As New System.Text.StringBuilder
			While xr.Read
				If xr.Name = "object" And xr.IsStartElement Then
					xr.Read()
					If firstTimeThrough Then
						firstTimeThrough = False
					Else
						sb.Append(Me.Settings.MultiRecordDelimiter)
					End If
					Dim cleanval As String = xr.Value.Trim
					sb.Append(cleanval)
				End If
			End While
			xr.Close()
			Return sb.ToString
		End Function

		Public Sub UpdateVolume()
			_currentVolumeSize = 0
			_currentImageSubdirectorySize = 0
			_currentNativeSubdirectorySize = 0
			_currentTextSubdirectorySize = 0
			_currentSubdirectoryNumber = _settings.VolumeInfo.SubdirectoryStartNumber
			_currentVolumeNumber += 1
		End Sub

		Public Sub UpdateSubdirectory()
			_currentImageSubdirectorySize = 0
			_currentNativeSubdirectorySize = 0
			_currentTextSubdirectorySize = 0
			_currentSubdirectoryNumber += 1
		End Sub

	End Class
End Namespace