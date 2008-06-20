Imports System.IO
Namespace kCura.WinEDDS
	Public Class BulkImageFileImporter
		Inherits kCura.Utility.DelimitedFileImporter

#Region "Members"

		Private _docManager As kCura.WinEDDS.Service.DocumentManager
		Private _fieldQuery As kCura.WinEDDS.Service.FieldQuery
		Private _folderManager As kCura.WinEDDS.Service.FolderManager
		Private _fileUploader As kCura.WinEDDS.FileUploader
		Private _textUploader As kCura.WinEDDS.FileUploader
		Private _fileManager As kCura.WinEDDS.Service.FileManager
		Private _productionManager As kCura.WinEDDS.Service.ProductionManager
		Private _bulkImportManager As kCura.WinEDDS.Service.BulkImportManager
		Private _folderID As Int32
		Private _productionArtifactID As Int32
		Private _overwrite As String
		Private _filePath As String
		Private _selectedIdentifierField As String
		Private _fileLineCount As Int32
		Private _continue As Boolean
		Private _overwriteOK As Boolean
		Private _replaceFullText As Boolean
		Private _order As Int32
		Private _csvwriter As System.Text.StringBuilder
		Private _nextLine As String()
		Private _errorLogFileName As String
		Private _errorLogWriter As System.IO.StreamWriter
		Private _autoNumberImages As Boolean
		Private _copyFilesToRepository As Boolean
		Private _repositoryPath As String
		Private _textRepositoryPath As String
		Private _caseInfo As kCura.EDDS.Types.CaseInfo

		Private WithEvents _processController As kCura.Windows.Process.Controller
		Private _productionDTO As kCura.EDDS.WebAPI.ProductionManagerBase.Production
		Private _keyFieldDto As kCura.EDDS.WebAPI.FieldManagerBase.Field
		Private _bulkLoadFileWriter As System.IO.StreamWriter
		Private _sourceTextEncoding As System.Text.Encoding = System.Text.Encoding.Default
		Private _uploadKey As String = ""
#End Region

#Region "Accessors"

		Friend WriteOnly Property FilePath() As String
			Set(ByVal value As String)
				_filePath = value
			End Set
		End Property

		Public ReadOnly Property ErrorLogFileName() As String
			Get
				Return _errorLogFileName
			End Get
		End Property

		Public ReadOnly Property HasErrors() As Boolean
			Get
				Return _errorLogFileName <> ""
			End Get
		End Property

		Protected ReadOnly Property Continue() As Boolean
			Get
				Return Not MyBase.HasReachedEOF AndAlso _continue
			End Get
		End Property

#End Region

#Region "Constructors"

		Public Sub New(ByVal folderID As Int32, ByVal args As ImageLoadFile, ByVal controller As kCura.Windows.Process.Controller)
			MyBase.New(New Char() {","c})
			_docManager = New kCura.WinEDDS.Service.DocumentManager(args.Credential, args.CookieContainer)
			_fieldQuery = New kCura.WinEDDS.Service.FieldQuery(args.Credential, args.CookieContainer)
			_folderManager = New kCura.WinEDDS.Service.FolderManager(args.Credential, args.CookieContainer)
			_fileManager = New kCura.WinEDDS.Service.FileManager(args.Credential, args.CookieContainer)
			_productionManager = New kCura.WinEDDS.Service.ProductionManager(args.Credential, args.CookieContainer)
			_bulkImportManager = New kCura.WinEDDS.Service.BulkImportManager(args.Credential, args.CookieContainer)
			_repositoryPath = args.SelectedCasePath & "EDDS" & args.CaseInfo.ArtifactID & "\"
			_textRepositoryPath = args.CaseDefaultPath & "EDDS" & args.CaseInfo.ArtifactID & "\"
			_fileUploader = New kCura.WinEDDS.FileUploader(args.Credential, args.CaseInfo.ArtifactID, _repositoryPath, args.CookieContainer)
			_textUploader = New kCura.WinEDDS.FileUploader(args.Credential, args.CaseInfo.ArtifactID, _textRepositoryPath, args.CookieContainer)
			_folderID = folderID

			_productionArtifactID = args.ProductionArtifactID
			If _productionArtifactID <> 0 Then
				_productionDTO = _productionManager.Read(args.CaseInfo.ArtifactID, _productionArtifactID)
				_keyFieldDto = New kCura.WinEDDS.Service.FieldManager(args.Credential, args.CookieContainer).Read(args.CaseInfo.ArtifactID, args.BeginBatesFieldArtifactID)
			End If
			_overwrite = args.Overwrite
			_replaceFullText = args.ReplaceFullText
			_selectedIdentifierField = args.ControlKeyField
			_processController = controller
			_copyFilesToRepository = args.CopyFilesToDocumentRepository
			_continue = True
			_autoNumberImages = args.AutoNumberImages
			_caseInfo = args.CaseInfo
		End Sub

#End Region

#Region "Enumerations"

		Private Enum Columns
			BatesNumber = 0
			FileLocation = 2
			MultiPageIndicator = 3
		End Enum

#End Region

#Region "Main"

		Public Overloads Sub ReadFile()
			Me.ReadFile(_filePath)
		End Sub

		Private Sub ProcessList(ByVal al As System.Collections.ArrayList, ByVal status As Int32)
			Try
				If al.Count = 0 Then Exit Sub
				Me.ProcessDocument(al, status)
				al.Clear()
			Catch ex As Exception
				Throw
			End Try
		End Sub

		Public Overloads Overrides Function ReadFile(ByVal path As String) As Object
			Dim bulkLoadFilePath As String = System.IO.Path.GetTempFileName
			_bulkLoadFileWriter = New System.IO.StreamWriter(bulkLoadFilePath, False, System.Text.Encoding.Unicode)
			Try
				Dim documentIdentifier As String = String.Empty
				_fileLineCount = kCura.Utility.File.CountLinesInFile(path)
				Reader = New StreamReader(path)
				RaiseStatusEvent(kCura.Windows.Process.EventType.Progress, "Begin Image Upload")
				Dim al As New System.collections.ArrayList
				Dim line As String()
				Dim status As Int32 = 0
				While Me.Continue
					Dim lineList As New System.Collections.ArrayList(Me.GetLine)
					lineList.Add(Me.CurrentLineNumber.ToString)
					line = DirectCast(lineList.ToArray(GetType(String)), String())
					If (line(Columns.MultiPageIndicator).ToUpper = "Y") Then
						Me.ProcessList(al, status)
					End If
					status = status Or Me.ProcessImageLine(line)
					al.Add(line)
					If Not Me.Continue Then
						Me.ProcessList(al, status)
						Exit While
					End If
				End While
				_bulkLoadFileWriter.Close()

				'_uploadKey = _fileUploader.UploadBcpFile(_caseInfo.ArtifactID, bulkLoadFilePath)
				_uploadKey = ""
				Dim overwrite As kCura.EDDS.WebAPI.BulkImportManagerBase.OverwriteType
				Select Case _overwrite.ToLower
					Case "none"
						overwrite = EDDS.WebAPI.BulkImportManagerBase.OverwriteType.Append
					Case "strict"
						overwrite = EDDS.WebAPI.BulkImportManagerBase.OverwriteType.Overlay
					Case Else
						overwrite = EDDS.WebAPI.BulkImportManagerBase.OverwriteType.Both
				End Select
				If _uploadKey <> "" Then
					If _productionArtifactID = 0 Then
						_bulkImportManager.BulkImportImage(_caseInfo.ArtifactID, _uploadKey, _replaceFullText, overwrite, _folderID, _repositoryPath, True)
					Else
						_bulkImportManager.BulkImportProductionImage(_caseInfo.ArtifactID, _uploadKey, _replaceFullText, overwrite, _folderID, _repositoryPath, _productionArtifactID, True)
					End If
				Else
					_fileUploader.DestinationFolderPath = _caseInfo.DocumentPath
					_uploadKey = _fileUploader.UploadFile(bulkLoadFilePath, _caseInfo.ArtifactID)
					If _productionArtifactID = 0 Then
						_bulkImportManager.BulkImportImage(_caseInfo.ArtifactID, _uploadKey, _replaceFullText, overwrite, _folderID, _caseInfo.DocumentPath, False)
					Else
						_bulkImportManager.BulkImportProductionImage(_caseInfo.ArtifactID, _uploadKey, _replaceFullText, overwrite, _folderID, _caseInfo.DocumentPath, _productionArtifactID, False)
					End If
				End If

				'push file across
				'run bulk copy
				'run sql
				'clean up temp table

				Me.CompleteSuccess()
			Catch ex As System.Exception
				Me.CompleteError(ex)
			End Try
		End Function

		Private Sub CompleteSuccess()
			If Not _errorLogWriter Is Nothing Then
				_errorLogWriter.Close()
			End If
			Me.Reader.Close()
			If _productionArtifactID <> 0 Then _productionManager.DoPostImportProcessing(_fileUploader.CaseArtifactID, _productionArtifactID)
			RaiseStatusEvent(kCura.Windows.Process.EventType.Progress, "End Image Upload")
		End Sub

		Private Sub CompleteError(ByVal ex As System.Exception)
			Try
				_bulkLoadFileWriter.Close()
			Catch x As System.Exception
			End Try
			Try
				_errorLogWriter.Close()
			Catch x As System.Exception
			End Try
			Try
				Me.Reader.Close()
			Catch x As System.Exception
			End Try
			RaiseFatalError(ex)
		End Sub

		Public Sub ProcessDocument(ByVal al As System.Collections.ArrayList, ByVal status As Int32)
			Try
				GetImagesForDocument(al, status)
			Catch ex As System.Exception
				'Me.LogErrorInFile(al)
				Throw
			End Try
		End Sub

#End Region

#Region "Worker Methods"

		Public Function ProcessImageLine(ByVal values As String()) As kCura.EDDS.Types.MassImport.ImageStatus
			Try
				Dim retval As kCura.EDDS.Types.MassImport.ImageStatus = EDDS.Types.MassImport.ImageStatus.Pending
				'check for existence
				If values(Columns.BatesNumber).Trim = "" Then
					Me.RaiseStatusEvent(Windows.Process.EventType.Error, String.Format("No image file specified on line."))
					retval = EDDS.Types.MassImport.ImageStatus.NoImageSpecifiedOnLine
				ElseIf Not System.IO.File.Exists(Me.GetFileLocation(values)) Then
					Me.RaiseStatusEvent(Windows.Process.EventType.Error, String.Format("Image file specified ( {0} ) does not exist.", values(Columns.FileLocation)))
					retval = EDDS.Types.MassImport.ImageStatus.ImageSpecifiedDne
				Else
					Dim validator As New kCura.ImageValidator.ImageValidator
					Dim path As String = Me.GetFileLocation(values)
					Try
						validator.ValidateImage(path)
					Catch ex As System.Exception
						Me.RaiseStatusEvent(Windows.Process.EventType.Error, String.Format("Error in '{0}': {1}", path, ex.Message))
						retval = EDDS.Types.MassImport.ImageStatus.InvalidImageFormat
					End Try
				End If
				Return retval
			Catch ex As Exception
				Throw
			End Try
			'check to make sure image is good
		End Function

		Private Function IsNumber(ByVal value As String) As Boolean
			Try
				Dim x As Int32 = CType(value, Int32)
			Catch ex As Exception
				Return False
			End Try
			Return True
		End Function

		'Private Sub LogErrorInFile(ByVal lines As System.Collections.ArrayList)
		'	If lines Is Nothing Then Exit Sub
		'	If _errorLogFileName = "" Then
		'		_errorLogFileName = System.IO.Path.GetTempFileName()
		'		_errorLogWriter = New System.IO.StreamWriter(_errorLogFileName, False, System.Text.Encoding.Default)
		'	End If
		'	Dim line As String()
		'	For Each line In lines
		'		_errorLogWriter.WriteLine(kCura.Utility.Array.StringArrayToCsv(line))
		'	Next
		'End Sub

		Public Shared Function GetFileLocation(ByVal line As String()) As String
			Dim fileLocation As String = line(Columns.FileLocation)
			If fileLocation <> "" AndAlso fileLocation.Chars(0) = "\" AndAlso fileLocation.Chars(1) <> "\" Then
				fileLocation = "." & fileLocation
			End If
			Return fileLocation
		End Function


		Private Function GetImagesForDocument(ByVal lines As ArrayList, ByVal status As Int32) As String()
			Try
				Me.AutoNumberImages(lines)
				Dim valueArray As String() = DirectCast(lines(0), String())
				Dim textFileList As New System.Collections.ArrayList
				Dim documentId As String = valueArray(Columns.BatesNumber)
				Dim offset As Int64 = 0
				For i As Int32 = 0 To lines.Count - 1
					valueArray = DirectCast(lines(i), String())
					Me.GetImageForDocument(Me.GetFileLocation(valueArray), valueArray(Columns.BatesNumber), documentId, i, offset, textFileList, i < lines.Count - 1, valueArray(valueArray.Length - 1), status)
				Next
				For Each filename As String In textFileList
					With New System.IO.StreamReader(filename, _sourceTextEncoding, True)
						_bulkLoadFileWriter.Write(.ReadToEnd)
						.Close()
					End With
				Next
				_bulkLoadFileWriter.Write(kCura.EDDS.Types.Constants.ENDLINETERMSTRING)
			Catch ex As Exception
				Throw
			End Try
		End Function

		Private Sub AutoNumberImages(ByVal lines As ArrayList)
			If Not _autoNumberImages OrElse lines.Count >= 1 Then Exit Sub
			Dim allsame As Boolean = True
			Dim batesnumber As String = DirectCast(lines(0), String())(Columns.BatesNumber)
			For i As Int32 = 0 To lines.Count - 1
				allsame = allsame AndAlso batesnumber = DirectCast(lines(i), String())(Columns.BatesNumber)
				If Not allsame Then Exit Sub
			Next
			For i As Int32 = 1 To lines.Count
				'TODO: configure separator "_"
				DirectCast(lines(i), String())(Columns.BatesNumber) = batesnumber & "_" & i.ToString.PadLeft(lines.Count.ToString.Length, "0"c)
			Next
		End Sub

		Private Sub GetImageForDocument(ByVal imageFileName As String, ByVal batesNumber As String, ByVal documentIdentifier As String, ByVal order As Int32, ByRef offset As Int64, ByVal fullTextFiles As System.Collections.ArrayList, ByVal writeLineTermination As Boolean, ByVal originalLineNumber As String, ByVal status As Int32)
			Try
				Dim filename As String = imageFileName.Substring(imageFileName.LastIndexOf("\") + 1)
				Dim extractedTextFileName As String = imageFileName.Substring(0, imageFileName.LastIndexOf("."c) + 1) & "txt"
				Dim fileGuid As String = ""
				Dim fileLocation As String = imageFileName
				Dim fileSize As Int64 = 0
				If status = 0 Then
					If _copyFilesToRepository Then
						RaiseStatusEvent(kCura.Windows.Process.EventType.Progress, String.Format("Uploading File '{0}'.", filename))
						fileGuid = _fileUploader.UploadFile(imageFileName, _folderID)
						fileLocation = _fileUploader.DestinationFolderPath.TrimEnd("\"c) & "\" & fileGuid
					Else
						fileGuid = System.Guid.NewGuid.ToString
					End If
					If System.IO.File.Exists(imageFileName) Then fileSize = New System.IO.FileInfo(imageFileName).Length
					If _replaceFullText AndAlso System.IO.File.Exists(extractedTextFileName) AndAlso Not fullTextFiles Is Nothing Then
						fullTextFiles.Add(extractedTextFileName)
					Else
						If _replaceFullText AndAlso Not System.IO.File.Exists(extractedTextFileName) Then
							RaiseStatusEvent(kCura.Windows.Process.EventType.Warning, String.Format("File '{0}' not found.  No text updated.", extractedTextFileName))
						End If
					End If
				End If
				_bulkLoadFileWriter.Write(status & ",")
				_bulkLoadFileWriter.Write("0,")
				_bulkLoadFileWriter.Write(originalLineNumber & ",")
				_bulkLoadFileWriter.Write(documentIdentifier & ",")
				_bulkLoadFileWriter.Write(batesNumber & ",")
				_bulkLoadFileWriter.Write(fileGuid & ",")
				_bulkLoadFileWriter.Write(filename & ",")
				_bulkLoadFileWriter.Write(order & ",")
				_bulkLoadFileWriter.Write(offset & ",")
				_bulkLoadFileWriter.Write(fileSize & ",")
				_bulkLoadFileWriter.Write(fileLocation & ",")
				_bulkLoadFileWriter.Write(imageFileName & ",")
				offset += New System.IO.FileInfo(extractedTextFileName).Length
				If writeLineTermination Then
					_bulkLoadFileWriter.Write(kCura.EDDS.Types.Constants.ENDLINETERMSTRING)
				End If
				If _replaceFullText AndAlso System.IO.File.Exists(extractedTextFileName) AndAlso Not fullTextFiles Is Nothing Then
					offset += New System.IO.FileInfo(extractedTextFileName).Length
				End If
			Catch ex As Exception
				Throw
			End Try
		End Sub

#End Region

#Region "Events and Event Handling"

		Public Event FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception)
		Public Event StatusMessage(ByVal args As kCura.Windows.Process.StatusEventArgs)

		Private Sub RaiseFatalError(ByVal ex As System.Exception)
			RaiseEvent FatalErrorEvent("Error processing line: " + CurrentLineNumber.ToString, ex)
		End Sub

		Private Sub RaiseStatusEvent(ByVal et As kCura.Windows.Process.EventType, ByVal line As String, Optional ByVal lineOffset As Int32 = 0)
			RaiseEvent StatusMessage(New kCura.Windows.Process.StatusEventArgs(et, Me.CurrentLineNumber - lineOffset, _fileLineCount, line))
		End Sub

		Private Sub _processObserver_CancelImport(ByVal processID As System.Guid) Handles _processController.HaltProcessEvent
			_continue = False
		End Sub

#End Region

#Region "Exceptions - Errors"
		Public Class FileLoadException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New()
				MyBase.New("Error uploading file.  Skipping line.")
			End Sub
		End Class

		Public Class CreateDocumentException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal parentException As System.Exception)
				MyBase.New("Error creating new document.  Skipping line: " & parentException.Message, parentException)
			End Sub
		End Class

		Public Class OverwriteNoneException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal docIdentifier As String)
				MyBase.New(String.Format("Document '{0}' exists - upload aborted.", docIdentifier))
			End Sub
		End Class

		Public Class OverwriteStrictException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal docIdentifier As String)
				MyBase.New(String.Format("Document '{0}' does not exist - upload aborted.", docIdentifier))
			End Sub
		End Class

		Public Class ImageCountMismatchException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New()
				MyBase.New("Production and Document image counts don't match - upload aborted.")
			End Sub
		End Class

		Public Class DocumentInProductionException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New()
				MyBase.New("Document is already in specified production - upload aborted.")
			End Sub
		End Class

		Public Class ProductionOverwriteException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal identifier As String)
				MyBase.New(String.Format("Document '{0}' belongs to one or more productions.  Document skipped.", identifier))
			End Sub
		End Class

		Public Class RedactionOverwriteException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal identifier As String)
				MyBase.New(String.Format("The one or more images for document '{0}' have redactions.  Document skipped.", identifier))
			End Sub
		End Class

		Public Class InvalidIdentifierKeyException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal identifier As String, ByVal fieldName As String)
				MyBase.New(String.Format("More than one document contains '{0}' as its '{1}' value.  Document skipped.", identifier, fieldName))
			End Sub
		End Class


#End Region

#Region "Exceptions - Fatal"
		Public Class InvalidBatesFormatException
			Inherits System.Exception
			Public Sub New(ByVal batesNumber As String, ByVal productionName As String, ByVal batesPrefix As String, ByVal batesSuffix As String, ByVal batesFormat As String)
				MyBase.New(String.Format("The image with bates number {0} cannot be imported into production '{1}' because the prefix and/or suffix do not match the values specified in the production. Expected prefix: '{2}'. Expected suffix: '{3}'. Expected format: '{4}'.", batesNumber, productionName, batesPrefix, batesSuffix, batesFormat))
			End Sub
		End Class
#End Region

		Private Sub _processController_ExportServerErrors(ByVal exportLocation As String) Handles _processController.ExportServerErrorsEvent
			With _bulkImportManager.GenerateErrorFiles(_caseInfo.ArtifactID, _uploadKey, True)
				Dim downloader As New FileDownloader(DirectCast(_bulkImportManager.Credentials, System.Net.NetworkCredential), _caseInfo.DocumentPath, _caseInfo.DownloadHandlerURL, _bulkImportManager.CookieContainer, kCura.WinEDDS.Service.Settings.AuthenticationToken)
				Dim rowsLocation As String = System.IO.Path.GetTempFileName
				Dim errorsLocation As String = System.IO.Path.GetTempFileName
				downloader.DownloadFile(rowsLocation, .OpticonKey, _caseInfo.ArtifactID.ToString)
				downloader.DownloadFile(errorsLocation, .LogKey, _caseInfo.ArtifactID.ToString)
				Dim exportErrorFilesTo As New System.Windows.Forms.FolderBrowserDialog

				exportErrorFilesTo.SelectedPath = System.IO.Directory.GetCurrentDirectory
				exportErrorFilesTo.ShowDialog()
				Dim folderPath As String = exportErrorFilesTo.SelectedPath
				If Not folderPath = "" Then
					folderPath = folderPath.TrimEnd("\"c) & "\"
				End If
				Dim rootFileName As String = _filePath
				Dim defaultExtension As String
				If Not rootFileName.IndexOf(".") = -1 Then
					defaultExtension = rootFileName.Substring(rootFileName.LastIndexOf("."))
					rootFileName = rootFileName.Substring(0, rootFileName.LastIndexOf("."))
				Else
					defaultExtension = ".opt"
				End If
				rootFileName.Trim("\"c)
				If rootFileName.IndexOf("\") <> -1 Then
					rootFileName = rootFileName.Substring(rootFileName.LastIndexOf("\") + 1)
				End If

				Dim rootFilePath As String = folderPath & rootFileName
				Dim datetimeNow As System.DateTime = System.DateTime.Now
				Dim errorFilePath As String = rootFilePath & "_ErrorLines_" & datetimeNow.Ticks & defaultExtension
				Dim errorReportPath As String = rootFilePath & "_ErrorReport_" & datetimeNow.Ticks & ".csv"
				System.IO.File.Move(rowsLocation, errorFilePath)
				System.IO.File.Move(errorsLocation, errorReportPath)
			End With

		End Sub

	End Class
End Namespace