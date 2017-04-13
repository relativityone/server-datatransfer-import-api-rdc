Imports System.Collections.Concurrent
Imports System.IO
Imports System.Text
Imports kCura.WinEDDS.Exporters
Imports kCura.WinEDDS.LoadFileEntry
Imports kCura.WinEDDS.IO

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
		Private _downloadManager As Service.Export.IExportFileDownloader

		Private _parent As WinEDDS.Exporter
		Private _columnHeaderString As String
		Private _hasWrittenColumnHeaderString As Boolean = False
		Private _encoding As System.Text.Encoding
		Private _errorFileLocation As String = ""
		Private _timekeeper As kCura.Utility.Timekeeper
		Private _statistics As kCura.WinEDDS.Statistics
		Private _totalExtractedTextFileLength As Int64 = 0
		Private _halt As Boolean = False
		Private _ordinalLookup As New System.Collections.Generic.Dictionary(Of String, Int32)
		Private _loadFileFormatter As Exporters.ILoadFileCellFormatter
		
		Private _fileHelper As IFileHelper
		Private _fileStreamFactory As IFileStreamFactory
		Private _directoryHelper As IDirectoryHelper
		Private _fileNameProvider As IFileNameProvider
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

		Public ReadOnly Property OrdinalLookup() As System.Collections.Generic.Dictionary(Of String, Int32)
			Get
				Return _ordinalLookup
			End Get
		End Property

		Protected Overridable ReadOnly Property NumberOfRetries() As Int32
			Get
				Return kCura.Utility.Config.IOErrorNumberOfRetries
			End Get
		End Property

		Protected Overridable ReadOnly Property WaitTimeBetweenRetryAttempts() As Int32
			Get
				Return kCura.Utility.Config.IOErrorWaitTimeInSeconds
			End Get
		End Property

#End Region

#Region "Constructors"
		Private ReadOnly Property InteractionManager As Exporters.IUserNotification
			Get
				Return _parent.InteractionManager
			End Get
		End Property

		Public Sub New(ByVal settings As ExportFile, ByVal totalFiles As Int64, ByVal parent As WinEDDS.Exporter, ByVal downloadHandler As Service.Export.IExportFileDownloader, ByVal t As kCura.Utility.Timekeeper, ByVal columnNamesInOrder As String(), ByVal statistics As kCura.WinEDDS.ExportStatistics, fileHelper As IFileHelper, directoryHelper As IDirectoryHelper, fileNameProvider As IFileNameProvider)
			_settings = settings
			_statistics = statistics
			_parent = parent

			_fileHelper = fileHelper
			_fileStreamFactory = New FileStreamFactory(_fileHelper)
			_directoryHelper = directoryHelper
			_fileNameProvider = fileNameProvider

			_timekeeper = t
			_currentVolumeNumber = _settings.VolumeInfo.VolumeStartNumber
			_currentSubdirectoryNumber = _settings.VolumeInfo.SubdirectoryStartNumber
			Dim volumeNumberPaddingWidth As Int32 = CType(System.Math.Floor(System.Math.Log10(CType(_currentVolumeNumber + 1, Double)) + 1), Int32)
			Dim subdirectoryNumberPaddingWidth As Int32 = CType(System.Math.Floor(System.Math.Log10(CType(_currentSubdirectoryNumber + 1, Double)) + 1), Int32)
			Dim totalFilesNumberPaddingWidth As Int32 = CType(System.Math.Floor(System.Math.Log10(CType(totalFiles + _currentVolumeNumber + 1, Double)) + 1), Int32)
			_volumeLabelPaddingWidth = System.Math.Max(totalFilesNumberPaddingWidth, volumeNumberPaddingWidth)
			totalFilesNumberPaddingWidth = CType(System.Math.Floor(System.Math.Log10(CType(totalFiles + _currentSubdirectoryNumber, Double)) + 1), Int32)
			_subdirectoryLabelPaddingWidth = System.Math.Max(totalFilesNumberPaddingWidth, subdirectoryNumberPaddingWidth)
			Dim validator As New Exporters.Validator.PaddingWarningValidator()
			If Not validator.IsValid(_settings, _volumeLabelPaddingWidth, _subdirectoryLabelPaddingWidth) Then
				If Not InteractionManager.AlertWarningSkippable(validator.ErrorMessages) Then
					parent.Shutdown()
					Return
				End If
			End If
			If Not parent.ExportManager.HasExportPermissions(_settings.CaseArtifactID) Then Throw New Service.ExportManager.InsufficientPermissionsForExportException("Export permissions revoked!  Please contact your system administrator to re-instate export permissions.")

			_subdirectoryLabelPaddingWidth = settings.SubdirectoryDigitPadding
			_volumeLabelPaddingWidth = settings.VolumeDigitPadding
			_currentVolumeSize = 0
			_currentImageSubdirectorySize = 0
			_currentTextSubdirectorySize = 0
			_currentNativeSubdirectorySize = 0
			_downloadManager = downloadHandler
			_downloadManager.FileHelper = _fileHelper

			Dim loadFilePath As String = Me.LoadFileDestinationPath
			If Not Me.Settings.Overwrite AndAlso _fileHelper.Exists(loadFilePath) Then
				InteractionManager.Alert(String.Format("Overwrite not selected and file '{0}' exists.", loadFilePath))
				_parent.Shutdown()
				Return
			End If
			If Me.Settings.ExportImages AndAlso Not Me.Settings.Overwrite AndAlso _fileHelper.Exists(Me.ImageFileDestinationPath) Then
				InteractionManager.Alert(String.Format("Overwrite not selected and file '{0}' exists.", Me.ImageFileDestinationPath))
				_parent.Shutdown()
				Return
			End If
			_encoding = Me.Settings.LoadFileEncoding

			If settings.ExportNative OrElse settings.SelectedViewFields.Length > 0 Then
				Dim nativeFileStream As FileStream = _fileStreamFactory.Create(loadFilePath, False)
				_nativeFileWriter = New System.IO.StreamWriter(nativeFileStream, _encoding)
			End If
			Dim imageFilePath As String = Me.ImageFileDestinationPath
			If Me.Settings.ExportImages Then
				If Not Me.Settings.Overwrite AndAlso _fileHelper.Exists(imageFilePath) Then
					Throw New System.Exception(String.Format("Overwrite not selected and file '{0}' exists.", imageFilePath))
				End If
				Dim imageFileStream As FileStream = _fileStreamFactory.Create(imageFilePath, False)
				_imageFileWriter = New System.IO.StreamWriter(imageFileStream, Me.GetImageFileEncoding)
			End If
			If Me.Settings.LoadFileIsHtml Then
				_loadFileFormatter = New Exporters.HtmlCellFormatter(Me.Settings)
			Else
				_loadFileFormatter = New Exporters.DelimitedCellFormatter(Me.Settings)
			End If
			For i As Int32 = 0 To columnNamesInOrder.Length - 1
				_ordinalLookup.Add(columnNamesInOrder(i), i)
			Next
			If Not Me.Settings.SelectedTextFields Is Nothing AndAlso Me.Settings.SelectedTextFields.Count > 0 Then
				Dim newindex As Int32 = _ordinalLookup.Count
				_ordinalLookup.Add(Relativity.Export.Constants.TEXT_PRECEDENCE_AWARE_ORIGINALSOURCE_AVF_COLUMN_NAME, newindex)
				_ordinalLookup.Add(Relativity.Export.Constants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME, newindex + 1)
			End If

		End Sub

		Private Function CurrentVolumeLabel(ByVal currentVolumeNumber As Integer) As String
			Return _settings.VolumeInfo.VolumePrefix & currentVolumeNumber.ToString.PadLeft(_volumeLabelPaddingWidth, "0"c)
		End Function

		Private Function CurrentImageSubdirectoryLabel(ByVal currentSubDirectoryNumber As Integer) As String
			Return _settings.VolumeInfo.SubdirectoryImagePrefix & currentSubDirectoryNumber.ToString.PadLeft(_subdirectoryLabelPaddingWidth, "0"c)
		End Function

		Private Function CurrentNativeSubdirectoryLabel(ByVal currentSubDirectoryNumber As Integer) As String
			Return _settings.VolumeInfo.SubdirectoryNativePrefix & currentSubDirectoryNumber.ToString.PadLeft(_subdirectoryLabelPaddingWidth, "0"c)
		End Function

		Private Function CurrentFullTextSubdirectoryLabel(ByVal currentSubDirectoryNumber As Integer) As String
			Return _settings.VolumeInfo.SubdirectoryFullTextPrefix & currentSubDirectoryNumber.ToString.PadLeft(_subdirectoryLabelPaddingWidth, "0"c)
		End Function

		Public Sub FinalizeVolumeAndSubDirPredictions(ByRef prediction As VolumePredictions, ByVal artifact As Exporters.ObjectExportInfo)

			'Reset native file count and size if not exporting natives or not copying native files from repo
			If Not Me.Settings.ExportNative OrElse Not Me.Settings.VolumeInfo.CopyNativeFilesFromRepository Then
				prediction.NativeFileCount = 0
				prediction.NativeFilesSize = 0
			End If

			'Predict images
			If Me.Settings.ExportImages AndAlso Me.Settings.VolumeInfo.CopyImageFilesFromRepository Then
				If prediction.ImageFileCount > 0 AndAlso (Me.Settings.TypeOfImage = ExportFile.ImageType.MultiPageTiff OrElse Me.Settings.TypeOfImage = ExportFile.ImageType.Pdf) Then
					prediction.ImageFileCount = 1
				End If
			Else
				prediction.ImageFileCount = 0
				prediction.ImageFilesSize = 0
			End If

			'Predict text
			If Me.Settings.ExportFullText AndAlso Me.Settings.ExportFullTextAsFile Then
				Dim count As Int32
				Dim columnName As String
				For count = 0 To _parent.ExportableColumns.Count - 1
					Dim field As WinEDDS.ViewFieldInfo = DirectCast(_parent.ExportableColumns(count), WinEDDS.ViewFieldInfo)
					columnName = field.AvfColumnName
					Dim fieldValue As Object = artifact.Metadata(_ordinalLookup(columnName))
					If field.FieldType = Relativity.FieldTypeHelper.FieldType.Text OrElse field.FieldType = Relativity.FieldTypeHelper.FieldType.OffTableText Then
						If Me.Settings.SelectedTextFields IsNot Nothing AndAlso TypeOf field Is CoalescedTextViewField Then
							prediction.TextFileCount += 1
							If TypeOf fieldValue Is Byte() Then
								fieldValue = System.Text.Encoding.Unicode.GetString(DirectCast(fieldValue, Byte()))
							End If
							If fieldValue Is Nothing Then fieldValue = String.Empty
							Dim textValue As String = fieldValue.ToString
							If textValue = Relativity.Constants.LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN Then
								prediction.TextFilesSize += 2 * 1048576 'This is the naive approach - assume the final text will be twice as long as the max length limit
							Else
								prediction.TextFilesSize += Me.Settings.TextFileEncoding.GetByteCount(textValue)
							End If
						End If
					End If
				Next
			Else
				prediction.TextFileCount = 0
				prediction.TextFilesSize = 0
			End If
		End Sub

		Public Function GetCurrentVolumeNumber(ByVal predictions As VolumePredictions) As Integer
			If _currentVolumeSize + predictions.TotalFileSize > Me.VolumeMaxSize AndAlso _currentVolumeSize > 0 Then
				Me.UpdateVolume()
			End If
			_currentVolumeSize += predictions.TotalFileSize
			Return _currentVolumeNumber
		End Function

		Public Function GetCurrentSubDirectoryNumber(ByVal predictions As VolumePredictions) As Integer
			If (_currentNativeSubdirectorySize + predictions.NativeFileCount > Me.SubDirectoryMaxSize AndAlso _currentNativeSubdirectorySize > 0) OrElse
				 (_currentImageSubdirectorySize + predictions.ImageFileCount > Me.SubDirectoryMaxSize AndAlso _currentImageSubdirectorySize > 0) OrElse 
				 (_currentTextSubdirectorySize + predictions.TextFileCount > Me.SubDirectoryMaxSize AndAlso _currentTextSubdirectorySize > 0) Then
				Me.UpdateSubdirectory()
			End If
			_currentNativeSubdirectorySize += predictions.NativeFileCount
			_currentImageSubdirectorySize += predictions.ImageFileCount
			_currentTextSubdirectorySize += predictions.TextFileCount
			Return _currentSubdirectoryNumber
		End Function

		Private Function GetImageFileEncoding() As System.Text.Encoding
			Dim retval As System.Text.Encoding
			If Me.Settings.ExportImages Then
				retval = System.Text.Encoding.Default
				If Me.Settings.LogFileFormat <> LoadFileType.FileFormat.Opticon Then
					retval = System.Text.Encoding.UTF8
				End If
			Else
				retval = _encoding
			End If
			Return retval
		End Function


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
				If String.IsNullOrEmpty(_errorFileLocation) Then _errorFileLocation = System.IO.Path.GetTempFileName()
				Return _errorFileLocation
			End Get
		End Property

		Private Sub LogFileExportError(ByVal type As ExportFileType, ByVal recordIdentifier As String, ByVal fileLocation As String, ByVal errorText As String)
			Try
				If _errorWriter Is Nothing Then
					Dim errorFileStream As FileStream = _fileStreamFactory.Create(Me.ErrorDestinationPath, False)
					_errorWriter = New System.IO.StreamWriter(errorFileStream, _encoding)
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

		Public Function ExportArtifact(ByVal artifact As Exporters.ObjectExportInfo, ByVal linesToWriteDat As ConcurrentDictionary(Of Int32, ILoadFileEntry),
																	 ByVal linesToWriteOpt As ConcurrentDictionary(Of String, String), ByVal threadNumber As Integer, ByVal volumeNumber As Integer,ByVal subDirectoryNumber As Integer) As Int64
			Dim tries As Int32 = 0
			Dim maxTries As Int32 = NumberOfRetries + 1
			While tries < maxTries And Not Me.Halt
				tries += 1
				Try
					Return Me.ExportArtifact(artifact, linesToWriteDat, linesToWriteOpt, tries > 1, threadNumber, volumeNumber, subDirectoryNumber)
					Exit While
				Catch ex As kCura.WinEDDS.Exceptions.ExportBaseException
					If tries = maxTries Then Throw
					_parent.WriteWarning(String.Format("Error writing data file(s) for document {0}", artifact.IdentifierValue))
					_parent.WriteWarning(String.Format("Actual error: {0}", ex.ToString))
					If tries > 1 Then
						_parent.WriteWarning(String.Format("Waiting {0} seconds to retry", WaitTimeBetweenRetryAttempts))
						System.Threading.Thread.CurrentThread.Join(WaitTimeBetweenRetryAttempts * 1000)
					Else
						_parent.WriteWarning("Retrying now")
					End If
				End Try
			End While
			Return Nothing
		End Function

		Private Function ReInitializeStream(ByVal brokenStream As System.IO.StreamWriter, ByVal position As Int64, ByVal filepath As String, ByVal encoding As System.Text.Encoding) As System.IO.StreamWriter
			If brokenStream Is Nothing Then Return Nothing
			Try
				brokenStream.Close()
			Catch ex As Exception
			End Try
			Try
				brokenStream = Nothing
			Catch ex As Exception
			End Try
			Try
				Dim newFileStream As FileStream = _fileStreamFactory.Create(filepath, True)
				Dim retval As New System.IO.StreamWriter(newFileStream, encoding)
				retval.BaseStream.Position = position
				Return retval
			Catch ex As System.IO.IOException
				Throw New kCura.WinEDDS.Exceptions.FileWriteException(Exceptions.FileWriteException.DestinationFile.Generic, ex)
			End Try
		End Function

#Region " Rollup Images "

		Private Sub RollupImages(ByRef imageCount As Long, ByRef successfulRollup As Boolean, ByVal artifact As kCura.WinEDDS.Exporters.ObjectExportInfo,
														 ByVal image As kCura.WinEDDS.Exporters.ImageExportInfo, ByVal currentVolumeNumber As Integer, ByVal currentSubDirectoryNumber As Integer)
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
					_fileHelper.Delete(imageLocation)
				Next
				Dim ext As String = ""
				Select Case Me.Settings.TypeOfImage
					Case ExportFile.ImageType.Pdf
						ext = ".pdf"
					Case ExportFile.ImageType.MultiPageTiff
						ext = ".tif"
				End Select
				Dim currentTempLocation As String = Me.GetImageExportLocation(image, currentVolumeNumber, currentSubDirectoryNumber)
				If currentTempLocation.IndexOf("."c) <> -1 Then currentTempLocation = currentTempLocation.Substring(0, currentTempLocation.LastIndexOf("."))
				currentTempLocation &= ext
				DirectCast(artifact.Images(0), Exporters.ImageExportInfo).TempLocation = currentTempLocation
				currentTempLocation = DirectCast(artifact.Images(0), Exporters.ImageExportInfo).FileName
				If currentTempLocation.IndexOf("."c) <> -1 Then currentTempLocation = currentTempLocation.Substring(0, currentTempLocation.LastIndexOf("."))
				currentTempLocation &= ext
				DirectCast(artifact.Images(0), Exporters.ImageExportInfo).FileName = currentTempLocation
				Dim location As String = DirectCast(artifact.Images(0), Exporters.ImageExportInfo).TempLocation
				If _fileHelper.Exists(DirectCast(artifact.Images(0), Exporters.ImageExportInfo).TempLocation) Then
					If Me.Settings.Overwrite Then
						_fileHelper.Delete(DirectCast(artifact.Images(0), Exporters.ImageExportInfo).TempLocation)
						_fileHelper.Move(tempLocation, DirectCast(artifact.Images(0), Exporters.ImageExportInfo).TempLocation)
					Else
						_parent.WriteWarning("File exists - file copy skipped: " & DirectCast(artifact.Images(0), Exporters.ImageExportInfo).TempLocation)
					End If
				Else
					_fileHelper.Move(tempLocation, DirectCast(artifact.Images(0), Exporters.ImageExportInfo).TempLocation)
				End If
			Catch ex As kCura.Utility.Image.ImageRollupException
				successfulRollup = False
				Try
					If Not tempLocation Is Nothing AndAlso Not tempLocation = "" Then
						_fileHelper.Delete(tempLocation)
					End If
					_parent.WriteImgProgressError(artifact, ex.ImageIndex, ex, "Document exported in single-page image mode.")
				Catch ioex As System.IO.IOException
					Throw New kCura.WinEDDS.Exceptions.FileWriteException(Exceptions.FileWriteException.DestinationFile.Errors, ioex)
				End Try
			End Try
		End Sub

#End Region

		Private Function CopySelectedLongTextToFile(ByVal artifact As Exporters.ObjectExportInfo, ByRef len As Int64) As String
			Dim field As ViewFieldInfo = Me.GetFieldForLongTextPrecedenceDownload(Nothing, artifact)
			If Not Me.OrdinalLookup.ContainsKey(Relativity.Export.Constants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME) Then
				Return String.Empty
			End If
			Dim text As Object = artifact.Metadata(Me.OrdinalLookup(Relativity.Export.Constants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME))
			If text Is Nothing Then text = String.Empty
			Dim longText As String = text.ToString
			If longText = Relativity.Constants.LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN Then
				Dim filePath As String = Me.DownloadTextFieldAsFile(artifact, field)
				len += _fileHelper.GetFileSize(filePath)
				Return filePath
			Else
				len += longText.Length
				Return String.Empty
			End If
		End Function

		Private Function TextPrecedenceIsSet() As Boolean
			If Me.Settings.SelectedTextFields Is Nothing Then Return False
			If Me.Settings.SelectedTextFields.Count = 0 Then Return False
			Return (From f In Me.Settings.SelectedTextFields Where f IsNot Nothing).Any
		End Function

		Private Function ExportArtifact(ByVal artifact As Exporters.ObjectExportInfo, ByVal linesToWrite As ConcurrentDictionary(Of Int32, ILoadFileEntry),
																		ByVal linesToWriteOpt As ConcurrentDictionary(Of String, String), ByVal isRetryAttempt As Boolean, ByVal threadNumber As Integer, 
																		ByVal volumeNumber As Integer, ByVal subDirectoryNumber As Integer) As Int64
			Dim totalFileSize As Int64 = 0
			Dim extractedTextFileSizeForVolume As Int64 = 0
			Dim image As Exporters.ImageExportInfo = Nothing
			Dim imageSuccess As Boolean = True
			If Me.Settings.ExportImages Then
				For Each image In artifact.Images
					_timekeeper.MarkStart("VolumeManager_DownloadImage", threadNumber)
					Try
						If Me.Settings.VolumeInfo.CopyImageFilesFromRepository Then
							totalFileSize += Me.DownloadImage(image, volumeNumber, subDirectoryNumber)
						End If
						image.HasBeenCounted = True
					Catch ex As System.Exception
						If TypeOf ex Is System.IO.FileNotFoundException AndAlso Me.Halt Then
							Return -1 'Halt was set to true during DownloadImage call before file could be downloaded
						Else 
							image.TempLocation = ""
							Me.LogFileExportError(ExportFileType.Image, artifact.IdentifierValue, image.FileGuid, ex.ToString)
							imageSuccess = False
						End If
					End Try
					_timekeeper.MarkEnd("VolumeManager_DownloadImage", threadNumber)
				Next
			End If
			Dim imageCount As Long = artifact.Images.Count
			Dim successfulRollup As Boolean = True
			If artifact.Images.Count > 0 AndAlso (Me.Settings.TypeOfImage = ExportFile.ImageType.MultiPageTiff OrElse Me.Settings.TypeOfImage = ExportFile.ImageType.Pdf) Then
				Me.RollupImages(imageCount, successfulRollup, artifact, image, volumeNumber, subDirectoryNumber)
			End If

			If Me.Settings.ExportNative Then
				_timekeeper.MarkStart("VolumeManager_DownloadNative", threadNumber)
				Try
					If Me.Settings.VolumeInfo.CopyNativeFilesFromRepository Then
						Dim downloadSize As Int64 = Me.DownloadNative(artifact, volumeNumber, subDirectoryNumber)
						If Not artifact.HasCountedNative Then totalFileSize += downloadSize
					End If
					artifact.HasCountedNative = True
				Catch ex As System.Exception
					If TypeOf ex Is System.IO.FileNotFoundException AndAlso Me.Halt Then
						Return -1 'Halt was set to true during DownloadNative call before file could be downloaded
					Else 
						Me.LogFileExportError(ExportFileType.Native, artifact.IdentifierValue, artifact.NativeFileGuid, ex.ToString)
					End If
				End Try
				_timekeeper.MarkEnd("VolumeManager_DownloadNative", threadNumber)
			End If
			Dim tempLocalFullTextFilePath As String = ""
			Dim tempLocalIproFullTextFilePath As String = ""

			Dim extractedTextFileLength As Long = 0
			If Me.Settings.ExportFullText AndAlso Me.Settings.ExportFullTextAsFile Then
				Dim len As Int64 = 0
				Try
					tempLocalFullTextFilePath = Me.CopySelectedLongTextToFile(artifact, len)
				Catch ex As Exception
					If TypeOf ex Is System.IO.FileNotFoundException AndAlso Me.Halt Then
						Return -1 'Halt was set to true during DownloadFullTextFile call before file could be downloaded
					Else
						Throw
					End If
				End Try
				If Me.Settings.ExportFullTextAsFile Then
					artifact.HasCountedTextFile = True
				End If
				artifact.HasFullText = True
			End If

			If Me.Settings.LogFileFormat = LoadFileType.FileFormat.IPRO_FullText AndAlso Me.Settings.ExportImages Then
				If Not Me.TextPrecedenceIsSet Then
					tempLocalIproFullTextFilePath = System.IO.Path.GetTempFileName
					Dim tries As Int32 = 0
					Dim maxTries As Int32 = NumberOfRetries + 1
					Dim start As Int64 = System.DateTime.Now.Ticks
					'BigData_ET_1037768
					Dim val As String = artifact.Metadata(Me.OrdinalLookup("ExtractedText")).ToString
					If val <> Relativity.Constants.LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN Then
						Dim tempLocalIproFileStream As FileStream = _fileStreamFactory.Create(tempLocalIproFullTextFilePath, False)
						Dim sw As New System.IO.StreamWriter(tempLocalIproFileStream, System.Text.Encoding.Unicode)
						sw.Write(val)
						sw.Close()
					Else
						While tries < maxTries AndAlso Not Me.Halt
							tries += 1
							Try
								_downloadManager.DownloadFullTextFile(tempLocalIproFullTextFilePath, artifact.ArtifactID, _settings.CaseInfo.ArtifactID.ToString)
								Exit While
							Catch ex As System.Exception
								If tries = 1 Then
									_parent.WriteStatusLine(Windows.Process.EventType.Warning, "Second attempt to download full text for document " & artifact.IdentifierValue, True)
								ElseIf tries < maxTries Then
									Dim waitTime As Int32 = WaitTimeBetweenRetryAttempts
									_parent.WriteStatusLine(Windows.Process.EventType.Warning, "Additional attempt to download full text for document " & artifact.IdentifierValue & " failed - retrying in " & waitTime.ToString() & " seconds", True)
									System.Threading.Thread.CurrentThread.Join(waitTime * 1000)
								Else
									Throw
								End If
							End Try
						End While
					End If
					_statistics.MetadataTime += System.Math.Max(System.DateTime.Now.Ticks - start, 1)
				Else
					If tempLocalFullTextFilePath <> String.Empty Then
						tempLocalIproFullTextFilePath = String.Copy(tempLocalFullTextFilePath)
					Else
						tempLocalIproFullTextFilePath = System.IO.Path.GetTempFileName
						Dim tempLocalIproFileStream As FileStream = _fileStreamFactory.Create(tempLocalIproFullTextFilePath, False)
						Dim sw As New System.IO.StreamWriter(tempLocalIproFileStream, System.Text.Encoding.Unicode)
						Dim val As String = artifact.Metadata(Me.OrdinalLookup(Relativity.Export.Constants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME)).ToString
						sw.Write(val)
						sw.Close()
					End If
				End If
			End If
			If Me.Settings.ExportImages Then
				_timekeeper.MarkStart("VolumeManager_ExportImages", threadNumber)
				Me.ExportImages(artifact.Images, tempLocalIproFullTextFilePath, successfulRollup, linesToWriteOpt, threadNumber, volumeNumber, subDirectoryNumber)
				_timekeeper.MarkEnd("VolumeManager_ExportImages", threadNumber)
			End If
			Dim nativeCount As Int32 = 0
			Dim nativeLocation As String = ""
			If Me.Settings.ExportNative AndAlso Me.Settings.VolumeInfo.CopyNativeFilesFromRepository Then
				Dim nativeFileName As String = Me.GetNativeFileName(artifact)
				Dim localFilePath As String = Me.GetLocalNativeFilePath(artifact, nativeFileName, volumeNumber, subDirectoryNumber)
				_timekeeper.MarkStart("VolumeManager_ExportNative", threadNumber)
				Me.ExportNative(localFilePath, artifact.NativeFileGuid, artifact.ArtifactID, nativeFileName, artifact.NativeTempLocation, threadNumber)
				_timekeeper.MarkEnd("VolumeManager_ExportNative", threadNumber)
				If artifact.NativeTempLocation = "" Then
					nativeLocation = ""
				Else
					nativeCount = 1
					Select Case Me.Settings.TypeOfExportedFilePath
						Case ExportFile.ExportedFilePathType.Absolute
							nativeLocation = localFilePath
						Case ExportFile.ExportedFilePathType.Relative
							nativeLocation = ".\" & Me.CurrentVolumeLabel(volumeNumber) & "\" & Me.CurrentNativeSubdirectoryLabel(subDirectoryNumber) & "\" & nativeFileName
						Case ExportFile.ExportedFilePathType.Prefix
							nativeLocation = Me.Settings.FilePrefix.TrimEnd("\"c) & "\" & Me.CurrentVolumeLabel(volumeNumber) & "\" & Me.CurrentNativeSubdirectoryLabel(subDirectoryNumber) & "\" & nativeFileName
					End Select
				End If
			End If
			Try
				If Not _hasWrittenColumnHeaderString AndAlso Not _nativeFileWriter Is Nothing Then
					Dim columnHeaderEntry As ILoadFileEntry = New CompletedLoadFileEntry(_columnHeaderString)
					linesToWrite.AddOrUpdate(-1, columnHeaderEntry, Function() columnHeaderEntry)
					_hasWrittenColumnHeaderString = True
				End If
				Dim loadFileEntry As ILoadFileEntry = Me.UpdateLoadFile(artifact.Metadata, artifact.HasFullText, artifact.ArtifactID, nativeLocation, tempLocalFullTextFilePath, artifact, extractedTextFileLength, volumeNumber, subDirectoryNumber)
				linesToWrite.AddOrUpdate(artifact.ArtifactID, loadFileEntry, Function() loadFileEntry)
			Catch ex As System.IO.IOException
				Throw New kCura.WinEDDS.Exceptions.FileWriteException(Exceptions.FileWriteException.DestinationFile.Load, ex)
			End Try
			_parent.WriteUpdate("Document " & artifact.IdentifierValue & " exported.", False)
			Try
				If Not _errorWriter Is Nothing Then _errorWriter.Flush()
			Catch ex As Exception
				Throw New Exceptions.FileWriteException(Exceptions.FileWriteException.DestinationFile.Errors, ex)
			End Try
			_totalExtractedTextFileLength += extractedTextFileLength
			_statistics.MetadataBytes = _totalExtractedTextFileLength
			_statistics.FileBytes += totalFileSize - extractedTextFileSizeForVolume
			If Not _errorWriter Is Nothing Then _errorWriterPosition = _errorWriter.BaseStream.Position
			Dim deletor As New TempTextFileDeletor(New String() {tempLocalIproFullTextFilePath, tempLocalFullTextFilePath})
			Dim t As New System.Threading.Thread(AddressOf deletor.DeleteFiles)
			t.Start()
			Dim retval As Int64 = 0
			If Me.Settings.VolumeInfo.CopyImageFilesFromRepository Then
				retval += imageCount
			End If
			If Not Me.Settings.VolumeInfo.CopyNativeFilesFromRepository Then
				retval += nativeCount
			End If
			Return retval
		End Function

		Private Function GetFieldForLongTextPrecedenceDownload(ByVal input As ViewFieldInfo, ByVal artifact As WinEDDS.Exporters.ObjectExportInfo) As ViewFieldInfo
			Dim retval As ViewFieldInfo = input
			If input Is Nothing OrElse input.AvfColumnName = Relativity.Export.Constants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME Then
				If Me.Settings.SelectedTextFields IsNot Nothing Then
					retval = (From f In Me.Settings.SelectedTextFields Where f.FieldArtifactId = CInt(artifact.Metadata(_ordinalLookup(Relativity.Export.Constants.TEXT_PRECEDENCE_AWARE_ORIGINALSOURCE_AVF_COLUMN_NAME)))).First
				End If
			End If
			Return retval
		End Function
		Private Function DownloadTextFieldAsFile(ByVal artifact As WinEDDS.Exporters.ObjectExportInfo, ByVal field As WinEDDS.ViewFieldInfo) As String
			Dim tempLocalFullTextFilePath As String = System.IO.Path.GetTempFileName
			Dim tries As Int32 = 0
			Dim maxTries As Int32 = NumberOfRetries + 1
			Dim start As Int64 = System.DateTime.Now.Ticks
			While tries < maxTries AndAlso Not Me.Halt
				tries += 1
				Try
					If Me.Settings.ArtifactTypeID = Relativity.ArtifactType.Document AndAlso field.Category = Relativity.FieldCategory.FullText AndAlso Not TypeOf field Is CoalescedTextViewField Then
						_downloadManager.DownloadFullTextFile(tempLocalFullTextFilePath, artifact.ArtifactID, _settings.CaseInfo.ArtifactID.ToString)
					Else
						Dim fieldToActuallyExportFrom As ViewFieldInfo = Me.GetFieldForLongTextPrecedenceDownload(field, artifact)
						_downloadManager.DownloadLongTextFile(tempLocalFullTextFilePath, artifact.ArtifactID, fieldToActuallyExportFrom, _settings.CaseInfo.ArtifactID.ToString)
					End If
					Exit While
				Catch ex As System.Exception
					If tries = 1 Then
						_parent.WriteStatusLine(Windows.Process.EventType.Warning, "Second attempt to download full text for document " & artifact.IdentifierValue, True)
					ElseIf tries < maxTries Then
						Dim waitTime As Int32 = WaitTimeBetweenRetryAttempts
						_parent.WriteStatusLine(Windows.Process.EventType.Warning, "Additional attempt to download full text for document " & artifact.IdentifierValue & " failed - retrying in " & waitTime.ToString() & " seconds", True)
						System.Threading.Thread.CurrentThread.Join(waitTime * 1000)
					Else
						Throw
					End If
				End Try
			End While
			_statistics.MetadataTime += System.Math.Max(System.DateTime.Now.Ticks - start, 1)
			Return tempLocalFullTextFilePath
		End Function

		Private Function GetLocalNativeFilePath(ByVal doc As Exporters.ObjectExportInfo, ByVal nativeFileName As String, ByVal currentVolumeNumber As Integer, ByVal currentSubDirectoryNumber As Integer) As String
			Dim localFilePath As String = Me.Settings.FolderPath
			If localFilePath.Chars(localFilePath.Length - 1) <> "\"c Then localFilePath &= "\"
			localFilePath &= Me.CurrentVolumeLabel(currentVolumeNumber) & "\" & Me.CurrentNativeSubdirectoryLabel(currentSubDirectoryNumber) & "\"
			If Not _directoryHelper.Exists(localFilePath) Then _directoryHelper.CreateDirectory(localFilePath)
			Return localFilePath & nativeFileName
		End Function

		Private Function GetLocalTextFilePath(ByVal doc As Exporters.ObjectExportInfo, ByVal currentVolumeNumber As Integer, ByVal currentSubDirectoryNumber As Integer) As String
			Dim localFilePath As String = Me.Settings.FolderPath
			If localFilePath.Chars(localFilePath.Length - 1) <> "\"c Then localFilePath &= "\"
			localFilePath &= Me.CurrentVolumeLabel(currentVolumeNumber) & "\" & Me.CurrentFullTextSubdirectoryLabel(currentSubDirectoryNumber) & "\"
			If Not _directoryHelper.Exists(localFilePath) Then _directoryHelper.CreateDirectory(localFilePath)
			Return localFilePath & doc.FullTextFileName(Me.NameTextFilesAfterIdentifier, _parent.NameTextAndNativesAfterBegBates)
		End Function

		Private Function GetNativeFileName(ByVal doc As Exporters.ObjectExportInfo) As String
			'Select Case _parent.ExportNativesToFileNamedFrom
			'	Case ExportNativeWithFilenameFrom.Identifier
			'		Return doc.NativeFileName(Me.Settings.AppendOriginalFileName)
			'	Case ExportNativeWithFilenameFrom.Production
			'		Return doc.ProductionBeginBatesFileName(Me.Settings.AppendOriginalFileName, _parent.NameTextAndNativesAfterBegBates)
			'End Select
			'Return Nothing

			Return _fileNameProvider.GetName(doc)
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

		Private Function GetImageExportLocation(ByVal image As Exporters.ImageExportInfo, ByVal currentVolumeNumber As Integer, ByVal currentSubDirectoryNumber As Integer) As String
			Dim localFilePath As String = Me.Settings.FolderPath
			Dim subfolderPath As String = Me.CurrentVolumeLabel(currentVolumeNumber) & "\" & Me.CurrentImageSubdirectoryLabel(currentSubDirectoryNumber) & "\"
			If localFilePath.Chars(localFilePath.Length - 1) <> "\"c Then localFilePath &= "\"
			localFilePath &= subfolderPath
			If Not _directoryHelper.Exists(localFilePath) Then _directoryHelper.CreateDirectory(localFilePath)
			Return localFilePath & image.FileName
		End Function


		Public Sub ExportImages(ByVal images As System.Collections.ArrayList, ByVal localFullTextPath As String, ByVal successfulRollup As Boolean, ByVal linesToWriteOpt As ConcurrentDictionary(Of String, String),
														ByVal threadNumber As Integer, ByVal currentVolumeNumber As Integer, ByVal currentSubDirectoryNumber As Integer)
			Dim image As WinEDDS.Exporters.ImageExportInfo
			Dim i As Int32 = 0
			Dim fullTextReader As System.IO.StreamReader = Nothing
			Dim localFilePath As String = Me.Settings.FolderPath
			Dim subfolderPath As String = Me.CurrentVolumeLabel(currentVolumeNumber) & "\" & Me.CurrentImageSubdirectoryLabel(currentSubDirectoryNumber) & "\"
			Dim pageOffset As Long
			If localFilePath.Chars(localFilePath.Length - 1) <> "\"c Then localFilePath &= "\"
			localFilePath &= subfolderPath
			If Not _directoryHelper.Exists(localFilePath) AndAlso Me.Settings.VolumeInfo.CopyImageFilesFromRepository Then _directoryHelper.CreateDirectory(localFilePath)
			Try
				If Me.Settings.LogFileFormat = LoadFileType.FileFormat.IPRO_FullText Then
					If _fileHelper.Exists(localFullTextPath) Then
						fullTextReader = New System.IO.StreamReader(localFullTextPath, _encoding, True)
					End If
				End If
				If images.Count > 0 AndAlso (Me.Settings.TypeOfImage = ExportFile.ImageType.MultiPageTiff OrElse Me.Settings.TypeOfImage = ExportFile.ImageType.Pdf) AndAlso successfulRollup Then
					Dim marker As Exporters.ImageExportInfo = DirectCast(images(0), Exporters.ImageExportInfo)
					Me.ExportDocumentImage(localFilePath & marker.FileName, marker.FileGuid, marker.ArtifactID, marker.BatesNumber, marker.TempLocation, threadNumber)
					Dim copyfile As String = Nothing
					Select Case Me.Settings.TypeOfExportedFilePath
						Case ExportFile.ExportedFilePathType.Absolute
							copyfile = localFilePath & marker.FileName
						Case ExportFile.ExportedFilePathType.Relative
							copyfile = ".\" & subfolderPath & marker.FileName
						Case ExportFile.ExportedFilePathType.Prefix
							copyfile = Me.Settings.FilePrefix.TrimEnd("\"c) & "\" & subfolderPath & marker.FileName
					End Select
					If Me.Settings.LogFileFormat = LoadFileType.FileFormat.Opticon Then
						Me.CreateImageLogEntry(marker.BatesNumber, copyfile, localFilePath, 1, fullTextReader, localFullTextPath <> "", Int64.MinValue, images.Count, linesToWriteOpt, currentVolumeNumber)
					Else
						For j As Int32 = 0 To images.Count - 1
							If (j = 0 AndAlso DirectCast(images(j), Exporters.ImageExportInfo).PageOffset Is Nothing) OrElse j = images.Count - 1 Then
								pageOffset = Int64.MinValue
							Else
								Dim nextImage As Exporters.ImageExportInfo = DirectCast(images(j + 1), Exporters.ImageExportInfo)
								If nextImage.PageOffset Is Nothing Then
									pageOffset = Int64.MinValue
								Else
									pageOffset = nextImage.PageOffset.Value
								End If
							End If
							image = DirectCast(images(j), WinEDDS.Exporters.ImageExportInfo)
							Me.CreateImageLogEntry(image.BatesNumber, copyfile, localFilePath, j + 1, fullTextReader, localFullTextPath <> "", pageOffset, images.Count, linesToWriteOpt, currentVolumeNumber)
						Next
					End If
					marker.TempLocation = copyfile
				Else
					For Each image In images
						If (i = 0 AndAlso image.PageOffset Is Nothing) OrElse i = images.Count - 1 Then
							pageOffset = Int64.MinValue
						Else
							Dim nextImage As Exporters.ImageExportInfo = DirectCast(images(i + 1), Exporters.ImageExportInfo)
							If nextImage.PageOffset Is Nothing Then
								pageOffset = Int64.MinValue
							Else
								pageOffset = nextImage.PageOffset.Value
							End If
						End If
						If Me.Settings.VolumeInfo.CopyImageFilesFromRepository Then
							Me.ExportDocumentImage(localFilePath & image.FileName, image.FileGuid, image.ArtifactID, image.BatesNumber, image.TempLocation, threadNumber)
							Dim copyfile As String = Nothing
							Select Case Me.Settings.TypeOfExportedFilePath
								Case ExportFile.ExportedFilePathType.Absolute
									copyfile = localFilePath & image.FileName
								Case ExportFile.ExportedFilePathType.Relative
									copyfile = ".\" & subfolderPath & image.FileName
								Case ExportFile.ExportedFilePathType.Prefix
									copyfile = Me.Settings.FilePrefix.TrimEnd("\"c) & "\" & subfolderPath & image.FileName
							End Select
							Me.CreateImageLogEntry(image.BatesNumber, copyfile, localFilePath, i + 1, fullTextReader, localFullTextPath <> "", pageOffset, images.Count, linesToWriteOpt, currentVolumeNumber)
							image.TempLocation = copyfile
						Else
							Me.CreateImageLogEntry(image.BatesNumber, image.SourceLocation, image.SourceLocation, i + 1, fullTextReader, localFullTextPath <> "", pageOffset, images.Count, linesToWriteOpt, currentVolumeNumber)
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

		Private Function DownloadImage(ByVal image As Exporters.ImageExportInfo, ByVal currentVolumeNumber As Integer, ByVal currentSubDirectoryNumber As Integer) As Int64
			If image.FileGuid = "" Then Return 0
			Dim start As Int64 = System.DateTime.Now.Ticks
			Dim tempFile As String = Me.GetImageExportLocation(image, currentVolumeNumber, currentSubDirectoryNumber)
			'If Me.Settings.TypeOfImage = ExportFile.ImageType.Pdf Then
			'	tempFile = System.IO.Path.GetTempFileName
			'	kCura.Utility.File.Instance.Delete(tempFile)
			'End If
			If _fileHelper.Exists(tempFile) Then
				If _settings.Overwrite Then
					_fileHelper.Delete(tempFile)
					_parent.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Overwriting image for {0}.", image.BatesNumber), False)
				Else
					_parent.WriteWarning(String.Format("{0} already exists. Skipping file export.", tempFile))
					Return 0
				End If
			End If
			Dim tries As Int32 = 0
			Dim maxTries As Int32 = NumberOfRetries + 1
			While tries < maxTries AndAlso Not Me.Halt
				tries += 1
				Try
					image.TempLocation = tempFile
					_downloadManager.DownloadFileForDocument(tempFile, image.FileGuid, image.SourceLocation, image.ArtifactID, _settings.CaseArtifactID.ToString)
					Exit While
				Catch ex As System.Exception
					If tries = 1 Then
						_parent.WriteStatusLine(Windows.Process.EventType.Warning, "Second attempt to download image " & image.BatesNumber & " - exact error: " & ex.ToString, True)
					ElseIf tries < maxTries Then
						Dim waitTime As Int32 = WaitTimeBetweenRetryAttempts
						_parent.WriteStatusLine(Windows.Process.EventType.Warning, "Additional attempt to download image " & image.BatesNumber & " failed - retrying in " & waitTime.ToString() & " seconds - exact error: " & ex.ToString, True)
						System.Threading.Thread.CurrentThread.Join(waitTime * 1000)
					Else
						Throw
					End If
				End Try
			End While
			_statistics.FileTime += System.Math.Max(System.DateTime.Now.Ticks - start, 1)
			Return _fileHelper.GetFileSize(tempFile)
		End Function

		Private Sub ExportDocumentImage(ByVal fileName As String, ByVal fileGuid As String, ByVal artifactID As Int32, ByVal batesNumber As String, ByVal tempFileLocation As String, ByVal threadNumber As Integer)
			If Not tempFileLocation = "" AndAlso Not tempFileLocation.ToLower = fileName.ToLower Then
				If _fileHelper.Exists(fileName) Then
					If _settings.Overwrite Then
						_fileHelper.Delete(fileName)
						_parent.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Overwriting document image {0}.", batesNumber), False)
						_fileHelper.Move(tempFileLocation, fileName)
					Else
						_parent.WriteWarning(String.Format("{0}.tif already exists. Skipping file export.", batesNumber))
					End If
				Else
					_timekeeper.MarkStart("VolumeManager_ExportDocumentImage_WriteStatus", threadNumber)
					_parent.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Now exporting document image {0}.", batesNumber), False)
					_timekeeper.MarkEnd("VolumeManager_ExportDocumentImage_WriteStatus", threadNumber)
					_timekeeper.MarkStart("VolumeManager_ExportDocumentImage_MoveFile", threadNumber)
					_fileHelper.Move(tempFileLocation, fileName)
					_timekeeper.MarkEnd("VolumeManager_ExportDocumentImage_MoveFile", threadNumber)
				End If
				_timekeeper.MarkStart("VolumeManager_ExportDocumentImage_WriteStatus", threadNumber)
				_parent.WriteStatusLine(Windows.Process.EventType.Status, String.Format("Finished exporting document image {0}.", batesNumber), False)
				_timekeeper.MarkEnd("VolumeManager_ExportDocumentImage_WriteStatus", threadNumber)
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

		Private Sub CreateImageLogEntry(ByVal batesNumber As String, ByVal copyFile As String, ByVal pathToImage As String, ByVal pageNumber As Int32, ByVal fullTextReader As System.IO.StreamReader,
																		ByVal expectingTextForPage As Boolean, ByVal pageOffset As Long, ByVal numberOfImages As Int32, ByVal linesToWriteOpt As ConcurrentDictionary(Of String, String),
																		ByVal currentVolumeNumber As Integer)
			Dim lineToWrite As New System.Text.StringBuilder
			Try
				Select Case _settings.LogFileFormat
					Case LoadFileType.FileFormat.Opticon
						Me.WriteOpticonLine(batesNumber, pageNumber = 1, copyFile, numberOfImages, linesToWriteOpt, currentVolumeNumber)
					Case LoadFileType.FileFormat.IPRO
						Me.WriteIproImageLine(batesNumber, pageNumber, copyFile, linesToWriteOpt, currentVolumeNumber)
					Case LoadFileType.FileFormat.IPRO_FullText
						Dim currentPageFirstByteNumber As Long
						If fullTextReader Is Nothing Then
							If pageNumber = 1 AndAlso expectingTextForPage Then _parent.WriteWarning(String.Format("Could not retrieve full text for document '{0}'", batesNumber))
						Else
							If pageNumber = 1 Then
								currentPageFirstByteNumber = 0
							Else
								currentPageFirstByteNumber = fullTextReader.BaseStream.Position
							End If
							
							lineToWrite.Append("FT,")
							lineToWrite.Append(batesNumber)
							lineToWrite.Append(",1,1,")
							Select Case pageOffset
								Case Int64.MinValue
									Dim c As Int32 = fullTextReader.Read
									While c <> -1
										lineToWrite.Append(Me.GetLfpFullTextTransform(ChrW(c)))
										c = fullTextReader.Read
									End While
								Case Else
									Dim i As Int32 = 0
									Dim c As Int32 = fullTextReader.Read
									While i < pageOffset AndAlso c <> -1
										lineToWrite.Append(Me.GetLfpFullTextTransform(ChrW(c)))
										c = fullTextReader.Read
										i += 1
									End While
							End Select
							lineToWrite.Append(vbNewLine)
						End If
						linesToWriteOpt.AddOrUpdate("FT" + batesNumber, lineToWrite.ToString, Function() lineToWrite.ToString)
						Me.WriteIproImageLine(batesNumber, pageNumber, copyFile, linesToWriteOpt, currentVolumeNumber)
				End Select
			Catch ex As System.IO.IOException
				Throw New kCura.WinEDDS.Exceptions.FileWriteException(Exceptions.FileWriteException.DestinationFile.Image, ex)
			End Try
		End Sub

		Private Sub WriteIproImageLine(ByVal batesNumber As String, ByVal pageNumber As Int32, ByVal fullFilePath As String, ByVal linesToWriteOpt As ConcurrentDictionary(Of String, String), ByVal currentVolumeNumber As Integer)
			Dim linefactory As New Exporters.LineFactory.SimpleIproImageLineFactory(batesNumber, pageNumber, fullFilePath, Me.CurrentVolumeLabel(currentVolumeNumber), Me.Settings.TypeOfImage.Value)
			linefactory.WriteLine(_imageFileWriter, linesToWriteOpt)
		End Sub

		Private Sub WriteOpticonLine(ByVal batesNumber As String, ByVal firstDocument As Boolean, ByVal copyFile As String, ByVal imageCount As Int32, ByVal linesToWriteOpt As ConcurrentDictionary(Of String, String),
																 ByVal currentVolumeNumber As Integer)
			Dim log As New System.Text.StringBuilder
			log.AppendFormat("{0},{1},{2},", batesNumber, Me.CurrentVolumeLabel(currentVolumeNumber), copyFile)
			If firstDocument Then log.Append("Y")
			log.Append(",,,")
			If firstDocument Then log.Append(imageCount)
			log.Append(vbNewLine)  
			linesToWriteOpt.AddOrUpdate(batesNumber, log.ToString, Function() log.ToString)
		End Sub
#End Region

		Private Function ExportNative(ByVal exportFileName As String, ByVal fileGuid As String, ByVal artifactID As Int32, ByVal systemFileName As String, ByVal tempLocation As String, ByVal threadNumber As Integer) As String
			If Not tempLocation = "" AndAlso Not tempLocation.ToLower = exportFileName.ToLower AndAlso Me.Settings.VolumeInfo.CopyNativeFilesFromRepository Then
				If _fileHelper.Exists(exportFileName) Then
					If _settings.Overwrite Then
						_fileHelper.Delete(exportFileName)
						_parent.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Overwriting document {0}.", systemFileName), False)
						_fileHelper.Move(tempLocation, exportFileName)
					Else
						_parent.WriteWarning(String.Format("{0} already exists. Skipping file export.", systemFileName))
					End If
				Else
					_timekeeper.MarkStart("VolumeManager_ExportNative_WriteStatus", threadNumber)
					_parent.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Now exporting document {0}.", systemFileName), False)
					_timekeeper.MarkEnd("VolumeManager_ExportNative_WriteStatus", threadNumber)
					_timekeeper.MarkStart("VolumeManager_ExportNative_MoveFile", threadNumber)
					_fileHelper.Move(tempLocation, exportFileName)
					_timekeeper.MarkEnd("VolumeManager_ExportNative_MoveFile", threadNumber)
				End If
			End If
			Return Nothing
		End Function

		Private Function DownloadNative(ByVal artifact As Exporters.ObjectExportInfo, ByVal currentVolumeNumber As Integer, ByVal currentSubDirectoryNumber As Integer) As Int64
			If Me.Settings.ArtifactTypeID = Relativity.ArtifactType.Document AndAlso artifact.NativeFileGuid = "" Then Return 0
			If Not Me.Settings.ArtifactTypeID = Relativity.ArtifactType.Document AndAlso (Not artifact.FileID > 0 OrElse artifact.NativeSourceLocation.Trim = String.Empty) Then Return 0
			Dim nativeFileName As String = Me.GetNativeFileName(artifact)
			Dim tempFile As String = Me.GetLocalNativeFilePath(artifact, nativeFileName, currentVolumeNumber, currentSubDirectoryNumber)
			
			Dim start As Int64 = System.DateTime.Now.Ticks
			If _fileHelper.Exists(tempFile) Then
				If Settings.Overwrite Then
					_fileHelper.Delete(tempFile)
					_parent.WriteStatusLine(kCura.Windows.Process.EventType.Status, String.Format("Overwriting document {0}.", nativeFileName), False)
				Else
					_parent.WriteWarning(String.Format("{0} already exists. Skipping file export.", tempFile))
					artifact.NativeTempLocation = tempFile
					Return _fileHelper.GetFileSize(tempFile)
				End If
			End If
			Dim tries As Int32 = 0
			Dim maxTries As Int32 = NumberOfRetries + 1
			While tries < maxTries AndAlso Not Me.Halt
				tries += 1
				Try
					artifact.NativeTempLocation = tempFile
					If Me.Settings.ArtifactTypeID = Relativity.ArtifactType.Document Then
						_downloadManager.DownloadFileForDocument(tempFile, artifact.NativeFileGuid, artifact.NativeSourceLocation, artifact.ArtifactID, _settings.CaseArtifactID.ToString)
					Else
						_downloadManager.DownloadFileForDynamicObject(tempFile, artifact.NativeSourceLocation, artifact.ArtifactID, _settings.CaseArtifactID.ToString, artifact.FileID, Me.Settings.FileField.FieldID)
					End If
					Exit While
				Catch ex As System.Exception
					If tries = 1 Then
						_parent.WriteStatusLine(Windows.Process.EventType.Warning, "Second attempt to download native for document " & artifact.IdentifierValue, True)
					ElseIf tries < maxTries Then
						Dim waitTime As Int32 = WaitTimeBetweenRetryAttempts
						_parent.WriteStatusLine(Windows.Process.EventType.Warning, "Additional attempt to download native for document " & artifact.IdentifierValue & " failed - retrying in " & waitTime.ToString() & " seconds", True)
						System.Threading.Thread.CurrentThread.Join(waitTime * 1000)
					Else
						Throw
					End If
				End Try
			End While
			
			_statistics.FileTime += System.Math.Max(System.DateTime.Now.Ticks - start, 1)
			Return _fileHelper.GetFileSize(tempFile)
		End Function

		Private Sub WriteLongText(ByVal source As System.IO.TextReader, ByVal output As System.IO.TextWriter, ByVal formatter As Exporters.ILongTextStreamFormatter)
			Dim c As Int32 = source.Read
			Dim doTransform As Boolean = (output Is _nativeFileWriter)
			Try
				While c <> -1
					formatter.TransformAndWriteCharacter(c, output)
					c = source.Read
				End While
			Finally
				If Not source Is Nothing Then
					Try
						source.Close()
					Catch
					End Try
				End If
				If Not output Is Nothing AndAlso Not doTransform Then
					Try
						output.Close()
					Catch
					End Try
				End If
			End Try
		End Sub

		Private Function GetLongTextStreamFormatter(ByRef source As System.IO.TextReader) As Exporters.ILongTextStreamFormatter
			If Me.Settings.LoadFileIsHtml Then
					Return New kCura.WinEDDS.Exporters.HtmlFileLongTextStreamFormatter(_settings, source)
				Else
					Return New kCura.WinEDDS.Exporters.DelimitedFileLongTextStreamFormatter(_settings, source)
				End If
		End Function

		Private Function ManageLongText(ByRef loadFileEntry As DeferredEntry, ByVal sourceValue As Object, ByVal textField As ViewFieldInfo, ByRef downloadedTextFilePath As String,
																		ByVal artifact As Exporters.ObjectExportInfo, ByVal startBound As String, ByVal endBound As String,
																		ByVal currentVolumeNumber As Integer, ByVal currentSubDirectoryNumber As Integer) As Long
			loadFileEntry.AddStringEntry(startBound)

			If TypeOf sourceValue Is Byte() Then
				sourceValue = System.Text.Encoding.Unicode.GetString(DirectCast(sourceValue, Byte()))
			End If
			If sourceValue Is Nothing Then sourceValue = String.Empty
			Dim textValue As String = sourceValue.ToString
			Dim source As System.IO.TextReader
			Dim destination As System.IO.TextWriter = Nothing
			Dim downloadedFileExists As Boolean = Not String.IsNullOrEmpty(downloadedTextFilePath) AndAlso _fileHelper.Exists(downloadedTextFilePath)
			If textValue = Relativity.Constants.LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN Then 'Text was too long and needs to be downloaded to a file instead
				If Me.Settings.SelectedTextFields IsNot Nothing AndAlso TypeOf textField Is CoalescedTextViewField AndAlso downloadedFileExists Then
					If Settings.SelectedTextFields.Count = 1 Then
						source = Me.GetLongTextStream(downloadedTextFilePath, textField)
					Else
						Dim precedenceField As ViewFieldInfo = GetFieldForLongTextPrecedenceDownload(textField, artifact)
						source = Me.GetLongTextStream(downloadedTextFilePath, precedenceField)
					End If
				Else
					downloadedTextFilePath = Me.DownloadTextFieldAsFile(artifact, textField) 'Download long text to temp file
					source = Me.GetLongTextStream(downloadedTextFilePath, textField)
				End If
			Else 'Text was not too long
				source = New System.IO.StringReader(textValue)
			End If
			Dim destinationPathExists As Boolean
			Dim destinationFilePath As String = String.Empty
			Dim formatter As Exporters.ILongTextStreamFormatter
			If Me.Settings.ExportFullTextAsFile AndAlso TypeOf textField Is CoalescedTextViewField Then 'Exporting text to separate file
				destinationFilePath = Me.GetLocalTextFilePath(artifact, currentVolumeNumber, currentSubDirectoryNumber)
				destinationPathExists = _fileHelper.Exists(destinationFilePath)
				If destinationPathExists AndAlso Not _settings.Overwrite Then 'Skip export instead of overwriting file
					_parent.WriteWarning(destinationFilePath & " already exists. Skipping file export.")
				Else 'Overwrite existing text file
					If destinationPathExists Then _parent.WriteStatusLine(kCura.Windows.Process.EventType.Status, "Overwriting: " & destinationFilePath, False)
					Dim destinationFileStream As FileStream = _fileStreamFactory.Create(destinationFilePath, False)
					destination = New System.IO.StreamWriter(destinationFileStream, Me.Settings.TextFileEncoding)
				End If
				formatter = New kCura.WinEDDS.Exporters.NonTransformFormatter
			Else 'Defer writing text to .dat file
				If String.IsNullOrEmpty(downloadedTextFilePath) Then				
					Dim stringBuilder As StringBuilder = new StringBuilder()
					Dim formatterWriter As System.IO.StringWriter = new StringWriter(stringBuilder)
					Dim longTextFormatter As Exporters.ILongTextStreamFormatter = GetLongTextStreamFormatter(source)
					Me.WriteLongText(source, formatterWriter, longTextFormatter)
					loadFileEntry.AddStringEntry(formatterWriter.ToString())
					loadFileEntry.AddStringEntry(endBound)
					Return 0
				Else					
					loadFileEntry.AddPartialEntry(New LongTextWriteDeferredEntry(downloadedTextFilePath, Me.Settings.TextFileEncoding, Me)) 'Defer writing text to .dat file
					source.Close()
					loadFileEntry.AddStringEntry(endBound)
					downloadedTextFilePath = "" 'This prevents the tmp text file from being deleted before we can write it to the .dat file. The deferred logic will delete the tmp file
					Return 0
				end if
			End If 
			If String.IsNullOrEmpty(downloadedTextFilePath) AndAlso Not source Is Nothing AndAlso TypeOf source Is System.IO.StreamReader AndAlso TypeOf DirectCast(source, System.IO.StreamReader).BaseStream Is System.IO.FileStream Then
				downloadedTextFilePath = DirectCast(DirectCast(source, System.IO.StreamReader).BaseStream, System.IO.FileStream).Name
			End If
			If Not destination Is Nothing Then
				Me.WriteLongText(source, destination, formatter)
			End If
			Dim retval As Long = 0
			'Write text file location to .dat file
			If destinationFilePath <> String.Empty Then
				retval = _fileHelper.GetFileSize(destinationFilePath)
				Dim textLocation As String = String.Empty
				Select Case Me.Settings.TypeOfExportedFilePath
					Case ExportFile.ExportedFilePathType.Absolute
						textLocation = destinationFilePath
					Case ExportFile.ExportedFilePathType.Relative
						textLocation = ".\" & Me.CurrentVolumeLabel(currentVolumeNumber) & "\" & Me.CurrentFullTextSubdirectoryLabel(currentSubDirectoryNumber) & "\" & artifact.FullTextFileName(Me.NameTextFilesAfterIdentifier, _parent.NameTextAndNativesAfterBegBates)
					Case ExportFile.ExportedFilePathType.Prefix
						textLocation = Me.Settings.FilePrefix.TrimEnd("\"c) & "\" & Me.CurrentVolumeLabel(currentVolumeNumber) & "\" & Me.CurrentFullTextSubdirectoryLabel(currentSubDirectoryNumber) & "\" & artifact.FullTextFileName(Me.NameTextFilesAfterIdentifier, _parent.NameTextAndNativesAfterBegBates)
				End Select
				If Settings.LoadFileIsHtml Then
					loadFileEntry.AddStringEntry("<a href='" & textLocation & "' target='_textwindow'>" & textLocation & "</a>")
				Else
					loadFileEntry.AddStringEntry(textLocation)
				End If
			End If

			loadFileEntry.AddStringEntry(endBound)
			Return retval
		End Function

		Private Function GetLongTextStream(ByVal artifact As Exporters.ObjectExportInfo, ByVal field As WinEDDS.ViewFieldInfo) As System.IO.TextReader
			Return New System.IO.StreamReader(Me.DownloadTextFieldAsFile(artifact, field), Me.GetLongTextFieldFileEncoding(field))
		End Function

		Private Function GetLongTextStream(ByVal filename As String, ByVal field As WinEDDS.ViewFieldInfo) As System.IO.TextReader
			Return New System.IO.StreamReader(filename, Me.GetLongTextFieldFileEncoding(field))
		End Function

		Private Function GetLongTextFieldFileEncoding(ByVal field As WinEDDS.ViewFieldInfo) As System.Text.Encoding
			If field.IsUnicodeEnabled Then Return System.Text.Encoding.Unicode
			Return System.Text.Encoding.Default
		End Function


		Public Function UpdateLoadFile(ByVal record As Object(), ByVal hasFullText As Boolean, ByVal documentArtifactID As Int32, ByVal nativeLocation As String,
																	 ByRef fullTextTempFile As String, ByVal doc As Exporters.ObjectExportInfo, ByRef extractedTextByteCount As Int64,
																	 ByVal currentVolumeNumber As Integer, ByVal currentSubDirectoryNumber As Integer) As ILoadFileEntry
			If _nativeFileWriter Is Nothing Then Return New CompletedLoadFileEntry("")
			Dim loadFileEntry As DeferredEntry = New DeferredEntry()
			Dim count As Int32
			Dim fieldValue As String
			Dim columnName As String
			Dim location As String = nativeLocation
			Dim rowPrefix As String = _loadFileFormatter.RowPrefix
			If Not String.IsNullOrEmpty(rowPrefix) Then loadFileEntry.AddStringEntry(rowPrefix)
			For count = 0 To _parent.ExportableColumns.Count - 1
				Dim field As WinEDDS.ViewFieldInfo = DirectCast(_parent.ExportableColumns(count), WinEDDS.ViewFieldInfo)
				columnName = field.AvfColumnName
				Dim val As Object = record(_ordinalLookup(columnName))
				If field.FieldType = Relativity.FieldTypeHelper.FieldType.Text OrElse field.FieldType = Relativity.FieldTypeHelper.FieldType.OffTableText Then
					If Me.Settings.LoadFileIsHtml Then
						extractedTextByteCount += Me.ManageLongText(loadFileEntry, val, field, fullTextTempFile, doc, "<td>", "</td>", currentVolumeNumber, currentSubDirectoryNumber)
					Else
						extractedTextByteCount += Me.ManageLongText(loadFileEntry, val, field, fullTextTempFile, doc, _settings.QuoteDelimiter, _settings.QuoteDelimiter, currentVolumeNumber, currentSubDirectoryNumber)
					End If
				Else
					If TypeOf val Is Byte() Then val = System.Text.Encoding.Unicode.GetString(DirectCast(val, Byte()))
					If field.FieldType = Relativity.FieldTypeHelper.FieldType.Date AndAlso field.Category <> Relativity.FieldCategory.MultiReflected Then
						If val Is System.DBNull.Value Then
							val = String.Empty
						ElseIf TypeOf val Is System.DateTime Then
							val = DirectCast(val, System.DateTime).ToString(field.FormatString)
						End If
						'If TypeOf val Is System.datete Then

						'End If
						'If Me.Settings.LoadFileIsHtml Then
						'	Dim datetime As String = kCura.Utility.NullableTypesHelper.DBNullString(val)
						'	If datetime Is Nothing OrElse datetime = "" Then
						'		val = ""
						'	Else
						'		val = System.DateTime.Parse(datetime, System.Globalization.CultureInfo.InvariantCulture).ToString(field.FormatString)
						'	End If
						'Else
						'	val = Me.ToExportableDateString(val, field.FormatString)
						'End If
					End If
					fieldValue = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(kCura.Utility.NullableTypesHelper.DBNullString(val))
					If field.IsMultiValueField Then
						fieldValue = Me.GetMultivalueString(fieldValue, field)
					ElseIf field.IsCodeOrMulticodeField Then
						fieldValue = Me.GetCodeValueString(fieldValue)
					End If
					loadFileEntry.AddStringEntry(_loadFileFormatter.TransformToCell(fieldValue))
				End If

				If Not count = _parent.ExportableColumns.Count - 1 AndAlso Not Me.Settings.LoadFileIsHtml Then
					loadFileEntry.AddStringEntry(_settings.RecordDelimiter)
				End If
			Next

			Dim imagesCell As String = _loadFileFormatter.CreateImageCell(doc)
			If Not String.IsNullOrEmpty(imagesCell) Then loadFileEntry.AddStringEntry(imagesCell)
			If _settings.ExportNative Then
				If Me.Settings.VolumeInfo.CopyNativeFilesFromRepository Then
					loadFileEntry.AddStringEntry(_loadFileFormatter.CreateNativeCell(location, doc))
				Else
					loadFileEntry.AddStringEntry(_loadFileFormatter.CreateNativeCell(doc.NativeSourceLocation, doc))
				End If
			End If
			If Not String.IsNullOrEmpty(_loadFileFormatter.RowSuffix) Then loadFileEntry.AddStringEntry(_loadFileFormatter.RowSuffix)
			loadFileEntry.AddStringEntry(vbNewLine)
			Return loadFileEntry
		End Function

		Private Function ToExportableDateString(ByVal val As Object, ByVal formatString As String) As String
			Dim datetime As String = kCura.Utility.NullableTypesHelper.DBNullString(val)
			Dim retval As String
			If datetime Is Nothing OrElse datetime.Trim = "" Then
				retval = ""
			Else
				retval = System.DateTime.Parse(datetime, System.Globalization.CultureInfo.InvariantCulture).ToString(formatString)
			End If
			Return retval
		End Function

		Private Function GetCodeValueString(ByVal input As String) As String
			input = System.Web.HttpUtility.HtmlDecode(input)
			input = input.Trim(New Char() {ChrW(11)}).Replace(ChrW(11), _settings.MultiRecordDelimiter)
			Return input
		End Function

		Private Function NameTextFilesAfterIdentifier() As Boolean
			If Me.Settings.TypeOfExport = ExportFile.ExportType.Production Then
				Return _parent.ExportNativesToFileNamedFrom = ExportNativeWithFilenameFrom.Identifier
			Else
				Return True
			End If
		End Function

		Public Sub WriteLongTextFileToDatFile(ByRef fileWriter As System.IO.StreamWriter, ByVal longTextPath As String, ByVal encoding As System.Text.Encoding)
			Dim source As System.IO.TextReader = New System.IO.StreamReader(longTextPath, encoding)
			If String.IsNullOrEmpty(longTextPath) AndAlso Not source Is Nothing AndAlso TypeOf source Is System.IO.StreamReader AndAlso TypeOf DirectCast(source, System.IO.StreamReader).BaseStream Is System.IO.FileStream Then
				longTextPath = DirectCast(DirectCast(source, System.IO.StreamReader).BaseStream, System.IO.FileStream).Name
			End If
			Dim formatter As Exporters.ILongTextStreamFormatter = GetLongTextStreamFormatter(source)
			If Not fileWriter Is Nothing Then Me.WriteLongText(source, fileWriter, formatter)
			If Not String.IsNullOrEmpty(longTextPath) Then kCura.Utility.File.Instance.Delete(longTextPath)
		End Sub

		Private Function GetMultivalueString(ByVal input As String, ByVal field As ViewFieldInfo) As String
			Dim retVal As String = input
			If input.Contains("<object>") Then
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
						Select Case field.FieldType
							Case Relativity.FieldTypeHelper.FieldType.Code, Relativity.FieldTypeHelper.FieldType.MultiCode
								cleanval = Me.GetCodeValueString(cleanval)
							Case Relativity.FieldTypeHelper.FieldType.Date
								cleanval = Me.ToExportableDateString(cleanval, field.FormatString)
						End Select
						'If isCodeOrMulticodeField Then cleanval = Me.GetCodeValueString(cleanval)
						sb.Append(cleanval)
					End If
				End While
				xr.Close()
				retVal = sb.ToString
			End If
			Return retVal

		End Function

		Public Sub WriteDatFile(ByVal linesToWriteDat As ConcurrentDictionary(Of Int32, ILoadFileEntry), ByVal artifacts As Exporters.ObjectExportInfo())
			Dim tries As Int32 = 0
			Dim maxTries As Int32 = NumberOfRetries + 1
			Dim lastArtifactId As Int32 = -1
			Dim loadFileBytes As Int64 = 0
			While tries < maxTries And Not Me.Halt
				tries += 1

				'Rollback stream position if retrying write
				If tries > 1 Then
					Me.ReInitializeStream(_nativeFileWriter, _nativeFileWriterPosition, Me.LoadFileDestinationPath, _encoding)
				End If

				Try
					'Write column headers
					Dim loadFileEntry As ILoadFileEntry = Nothing
					linesToWriteDat.TryGetValue(-1, loadFileEntry)
					If Not loadFileEntry Is Nothing Then loadFileEntry.Write(_nativeFileWriter)

					'Write artifact entries
					For Each artifact As Exporters.ObjectExportInfo in artifacts
						lastArtifactId = artifact.ArtifactID
						If linesToWriteDat.TryGetValue(artifact.ArtifactID, loadFileEntry)
							If Not loadFileEntry Is Nothing Then loadFileEntry.Write(_nativeFileWriter)
						End If
					Next

					'Flush writer
					Try
						If Not _nativeFileWriter Is Nothing Then _nativeFileWriter.Flush()
					Catch ex As Exception
						Throw New Exceptions.FileWriteException(Exceptions.FileWriteException.DestinationFile.Load, ex)
					End Try

					'Save file writer stream position in case we need to rollback on retry attempts
					If Not _nativeFileWriter Is Nothing Then
						_nativeFileWriterPosition = _nativeFileWriter.BaseStream.Position
						loadFileBytes += kCura.Utility.File.Instance.GetFileSize(DirectCast(_nativeFileWriter.BaseStream, System.IO.FileStream).Name)
					End If

					'Store statistics
					_statistics.MetadataBytes = loadFileBytes + _totalExtractedTextFileLength
					
					Exit While
				Catch ex As kCura.WinEDDS.Exceptions.ExportBaseException
					If tries = maxTries Then Throw
					_parent.WriteWarning(String.Format("Error writing metadata for artifact {0}", lastArtifactId))
					_parent.WriteWarning(String.Format("Actual error: {0}", ex.ToString))
					If tries > 1 Then
						_parent.WriteWarning(String.Format("Waiting {0} seconds to retry", WaitTimeBetweenRetryAttempts))
						System.Threading.Thread.CurrentThread.Join(WaitTimeBetweenRetryAttempts * 1000)
					Else
						_parent.WriteWarning("Retrying now")
					End If
				End Try
			End While
		End Sub

		Public Sub WriteOptFile(ByVal linesToWriteOpt As ConcurrentDictionary(Of String, String), ByVal artifacts As Exporters.ObjectExportInfo())
			Dim tries As Int32 = 0
			Dim maxTries As Int32 = NumberOfRetries + 1
			Dim lastArtifactId As Int32 = -1
			Dim loadFileBytes As Int64 = 0
			While tries < maxTries And Not Me.Halt
				tries += 1

				'Rollback stream position if retrying write
				If tries > 1 Then
					Me.ReInitializeStream(_imageFileWriter, _imageFileWriterPosition, Me.ImageFileDestinationPath, Me.GetImageFileEncoding())
				End If

				Try	
					Dim lineToWrite As String = ""

					'Write artifact entries
					For Each artifact As Exporters.ObjectExportInfo in artifacts
						For Each image As WinEDDS.Exporters.ImageExportInfo in artifact.Images

							'If IPRO Full Text append FT Lines
							If linesToWriteOpt.TryGetValue("FT" + image.BatesNumber, lineToWrite)
								_imageFileWriter.Write(lineToWrite)
							End If

							'Otherwise go and grab the Image line
							If linesToWriteOpt.TryGetValue(image.BatesNumber, lineToWrite)
								_imageFileWriter.Write(lineToWrite)
							End If
						Next
					Next

					'Flush writer
					Try
						If Not _imageFileWriter Is Nothing Then _imageFileWriter.Flush()
					Catch ex As Exception
						Throw New Exceptions.FileWriteException(Exceptions.FileWriteException.DestinationFile.Image, ex)
					End Try

					'Save file writer stream position in case we need to rollback on retry attempts
					If Not _imageFileWriter Is Nothing Then
						_imageFileWriterPosition = _imageFileWriter.BaseStream.Position
						loadFileBytes += kCura.Utility.File.Instance.GetFileSize(DirectCast(_imageFileWriter.BaseStream, System.IO.FileStream).Name)
					End If

					'Store statistics
					_statistics.MetadataBytes = loadFileBytes + _totalExtractedTextFileLength
					
					Exit While
				Catch ex As kCura.WinEDDS.Exceptions.ExportBaseException
					If tries = maxTries Then Throw
					_parent.WriteWarning(String.Format("Error writing .opt file entry for artifact {0}", lastArtifactId))
					_parent.WriteWarning(String.Format("Actual error: {0}", ex.ToString))
					If tries > 1 Then
						_parent.WriteWarning(String.Format("Waiting {0} seconds to retry", WaitTimeBetweenRetryAttempts))
						System.Threading.Thread.CurrentThread.Join(WaitTimeBetweenRetryAttempts * 1000)
					Else
						_parent.WriteWarning("Retrying now")
					End If
				End Try
			End While
		End Sub

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