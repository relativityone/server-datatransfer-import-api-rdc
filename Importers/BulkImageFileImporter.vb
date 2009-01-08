Imports System.IO
Namespace kCura.WinEDDS
	Public Class BulkImageFileImporter
		Inherits kCura.Utility.DelimitedFileImporter

#Region "Members"

		Private _docManager As kCura.WinEDDS.Service.DocumentManager
		Private _fieldQuery As kCura.WinEDDS.Service.FieldQuery
		Private _folderManager As kCura.WinEDDS.Service.FolderManager
		Private WithEvents _fileUploader As kCura.WinEDDS.FileUploader
		Private WithEvents _bcpuploader As kCura.WinEDDS.FileUploader
		Private WithEvents _textUploader As kCura.WinEDDS.FileUploader
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
		Private _runId As String = ""
		Private _settings As ImageLoadFile
		Private _batchCount As Int32 = 0
		Private _errorCount As Int32 = 0
		Private _errorMessageFileLocation As String = ""
		Private _errorRowsFileLocation As String = ""
		Private _fileIdentifierLookup As System.Collections.Hashtable

		Private _processID As Guid

		Public Const MaxNumberOfErrorsInGrid As Int32 = 1000
#End Region

#Region "Accessors"

		Friend WriteOnly Property FilePath() As String
			Set(ByVal value As String)
				_filePath = value
			End Set
		End Property

		Public ReadOnly Property HasErrors() As Boolean
			Get
				Return _errorCount > 0
			End Get
		End Property

		Protected ReadOnly Property Continue() As Boolean
			Get
				Return Not MyBase.HasReachedEOF AndAlso _continue
			End Get
		End Property

#End Region

#Region "Constructors"

		Public Sub New(ByVal folderID As Int32, ByVal args As ImageLoadFile, ByVal controller As kCura.Windows.Process.Controller, ByVal processID As Guid)
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
			_bcpuploader = New kCura.WinEDDS.FileUploader(args.Credential, args.CaseInfo.ArtifactID, _repositoryPath, args.CookieContainer)
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
			_settings = args
			_processID = processID
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

		Private Sub ProcessList(ByVal al As System.Collections.ArrayList, ByRef status As Int32, ByVal bulkLoadFilePath As String)
			Try
				If al.Count = 0 Then Exit Sub
				Me.ProcessDocument(al, status)
				al.Clear()
				status = 0
				If _bulkLoadFileWriter.BaseStream.Length > Config.ImportBatchMaxVolume OrElse _batchCount > Config.ImportBatchSize - 1 Then
					Me.PushImageBatch(bulkLoadFilePath, False)
				End If
			Catch ex As Exception
				Throw
			End Try
		End Sub

		Public Function PushImageBatch(ByVal bulkLoadFilePath As String, ByVal isFinal As Boolean) As Object
			_bulkLoadFileWriter.Close()
			_fileIdentifierLookup.Clear()
			If _batchCount = 0 Then Exit Function

			_batchCount = 0

			_uploadKey = _bcpuploader.UploadBcpFile(_caseInfo.ArtifactID, bulkLoadFilePath)
			'_uploadKey = ""
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
					_runId = _bulkImportManager.BulkImportImage(_caseInfo.ArtifactID, _uploadKey, _replaceFullText, overwrite, _folderID, _repositoryPath, True, _runId).ToString
				Else
					_runId = _bulkImportManager.BulkImportProductionImage(_caseInfo.ArtifactID, _uploadKey, _replaceFullText, overwrite, _folderID, _repositoryPath, _productionArtifactID, True, _runId, _keyFieldDto.ArtifactID).ToString()
				End If
			Else
				Dim oldDestinationFolderPath As String = System.String.Copy(_bcpuploader.DestinationFolderPath)
				_bcpuploader.DestinationFolderPath = _caseInfo.DocumentPath
				_uploadKey = _bcpuploader.UploadFile(bulkLoadFilePath, _caseInfo.ArtifactID)
				_bcpuploader.DestinationFolderPath = oldDestinationFolderPath
				If _productionArtifactID = 0 Then
					_runId = _bulkImportManager.BulkImportImage(_caseInfo.ArtifactID, _uploadKey, _replaceFullText, overwrite, _folderID, _caseInfo.DocumentPath, False, _runId).ToString
				Else
					_runId = _bulkImportManager.BulkImportProductionImage(_caseInfo.ArtifactID, _uploadKey, _replaceFullText, overwrite, _folderID, _caseInfo.DocumentPath, _productionArtifactID, False, _runId, _keyFieldDto.ArtifactID).ToString
				End If
			End If
			If Not isFinal Then
				Try
					_bulkLoadFileWriter = New System.IO.StreamWriter(bulkLoadFilePath, False, System.Text.Encoding.Unicode)
				Catch
					_bulkLoadFileWriter = New System.IO.StreamWriter(bulkLoadFilePath, False, System.Text.Encoding.Unicode)
				End Try
			End If
			ManageErrors()
		End Function

		Public Overloads Overrides Function ReadFile(ByVal path As String) As Object
			Dim bulkLoadFilePath As String = System.IO.Path.GetTempFileName
			_fileIdentifierLookup = New System.Collections.Hashtable
			_bulkLoadFileWriter = New System.IO.StreamWriter(bulkLoadFilePath, False, System.Text.Encoding.Unicode)
			Try
				Dim documentIdentifier As String = String.Empty
				_fileLineCount = kCura.Utility.File.CountLinesInFile(path)
				Reader = New StreamReader(path)
				_filePath = path
				RaiseStatusEvent(kCura.Windows.Process.EventType.Progress, "Begin Image Upload")
				Dim al As New System.collections.ArrayList
				Dim line As String()
				Dim status As Int32 = 0
				While Me.Continue
					Dim lineList As New System.Collections.ArrayList(Me.GetLine)
					If lineList.Count < 4 Then Throw New InvalidLineFormatException(Me.CurrentLineNumber, lineList.Count)
					lineList.Add(Me.CurrentLineNumber.ToString)
					line = DirectCast(lineList.ToArray(GetType(String)), String())
					If (line(Columns.MultiPageIndicator).ToUpper = "Y") Then
						Me.ProcessList(al, status, bulkLoadFilePath)
					End If
					status = status Or Me.ProcessImageLine(line)
					al.Add(line)
					If Not Me.Continue Then
						Me.ProcessList(al, status, bulkLoadFilePath)
						Exit While
					End If
				End While
				Me.PushImageBatch(bulkLoadFilePath, True)
				Me.CompleteSuccess()
				CleanupTempTables()
			Catch ex As System.Exception
				Me.CompleteError(ex)
			End Try
		End Function



		Private Sub CompleteSuccess()
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

		Public Function ProcessImageLine(ByVal values As String()) As kCura.EDDS.Types.MassImport.ImportStatus
			Try
				Dim retval As kCura.EDDS.Types.MassImport.ImportStatus = EDDS.Types.MassImport.ImportStatus.Pending
				'check for existence
				If values(Columns.BatesNumber).Trim = "" Then
					Me.RaiseStatusEvent(Windows.Process.EventType.Error, String.Format("No image file specified on line."))
					retval = EDDS.Types.MassImport.ImportStatus.NoImageSpecifiedOnLine
				ElseIf Not System.IO.File.Exists(Me.GetFileLocation(values)) Then
					Me.RaiseStatusEvent(Windows.Process.EventType.Error, String.Format("Image file specified ( {0} ) does not exist.", values(Columns.FileLocation)))
					retval = EDDS.Types.MassImport.ImportStatus.FileSpecifiedDne
				Else
					Dim validator As New kCura.ImageValidator.ImageValidator
					Dim path As String = Me.GetFileLocation(values)
					Try
						validator.ValidateImage(path)
						Me.RaiseStatusEvent(Windows.Process.EventType.Status, String.Format("Image file ( {0} ) validated.", values(Columns.FileLocation)))
					Catch ex As System.Exception
						'Me.RaiseStatusEvent(Windows.Process.EventType.Error, String.Format("Error in '{0}': {1}", path, ex.Message))
						retval = EDDS.Types.MassImport.ImportStatus.InvalidImageFormat
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
				Dim hasFileIdentifierProblem As Boolean = False
				For Each line As String() In lines
					If _fileIdentifierLookup.ContainsKey(line(Columns.BatesNumber).Trim) Then
						hasFileIdentifierProblem = True
					Else
						_fileIdentifierLookup.Add(line(Columns.BatesNumber).Trim, line(Columns.BatesNumber).Trim)
					End If
				Next
				If hasFileIdentifierProblem Then status += kCura.EDDS.Types.MassImport.ImportStatus.IdentifierOverlap

				Dim valueArray As String() = DirectCast(lines(0), String())
				Dim textFileList As New System.Collections.ArrayList
				Dim documentId As String = valueArray(Columns.BatesNumber)
				Dim offset As Int64 = 0
				For i As Int32 = 0 To lines.Count - 1
					valueArray = DirectCast(lines(i), String())
					Me.GetImageForDocument(Me.GetFileLocation(valueArray), valueArray(Columns.BatesNumber), documentId, i, offset, textFileList, i < lines.Count - 1, valueArray(valueArray.Length - 1), status)
				Next
				For Each filename As String In textFileList
					With New System.IO.StreamReader(filename, _settings.FullTextEncoding, True)
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
			If Not _autoNumberImages OrElse lines.Count <= 1 Then Exit Sub
			Dim allsame As Boolean = True
			Dim batesnumber As String = DirectCast(lines(0), String())(Columns.BatesNumber)
			For i As Int32 = 0 To lines.Count - 1
				allsame = allsame AndAlso batesnumber = DirectCast(lines(i), String())(Columns.BatesNumber)
				If Not allsame Then Exit Sub
			Next
			For i As Int32 = 1 To lines.Count - 1
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
				_batchCount += 1
				If status = 0 Then
					If _copyFilesToRepository Then
						RaiseStatusEvent(kCura.Windows.Process.EventType.Progress, String.Format("Uploading File '{0}'.", filename))
						fileGuid = _fileUploader.UploadFile(imageFileName, _folderID)
						fileLocation = _fileUploader.DestinationFolderPath.TrimEnd("\"c) & "\" & fileGuid
					Else
						RaiseStatusEvent(kCura.Windows.Process.EventType.Progress, String.Format("Processing image '{0}'.", batesNumber))
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
				If _replaceFullText AndAlso System.IO.File.Exists(extractedTextFileName) AndAlso Not fullTextFiles Is Nothing Then
					offset += New System.IO.FileInfo(extractedTextFileName).Length
				End If
				_bulkLoadFileWriter.Write(status & ",")
				_bulkLoadFileWriter.Write("0,")
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
				If writeLineTermination Then
					_bulkLoadFileWriter.Write(kCura.EDDS.Types.Constants.ENDLINETERMSTRING)
				End If
			Catch ex As Exception
				Throw
			End Try
		End Sub

#End Region

#Region "Events and Event Handling"

		Public Event FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception)
		Public Event StatusMessage(ByVal args As kCura.Windows.Process.StatusEventArgs)
		Public Event ReportErrorEvent(ByVal row As System.Collections.IDictionary)
		Public Event UploadModeChangeEvent(ByVal mode As String, ByVal isBulkEnabled As Boolean)

		Private Sub RaiseFatalError(ByVal ex As System.Exception)
			RaiseEvent FatalErrorEvent("Error processing line: " + CurrentLineNumber.ToString, ex)
		End Sub

		Private Sub RaiseStatusEvent(ByVal et As kCura.Windows.Process.EventType, ByVal line As String, Optional ByVal lineOffset As Int32 = 0)
			RaiseEvent StatusMessage(New kCura.Windows.Process.StatusEventArgs(et, Me.CurrentLineNumber - lineOffset, _fileLineCount, line))
		End Sub

		Private Sub _processObserver_CancelImport(ByVal processID As System.Guid) Handles _processController.HaltProcessEvent
			If processID.ToString = _processID.ToString Then _continue = False
		End Sub

		Private Sub RaiseReportError(ByVal row As System.Collections.Hashtable, ByVal lineNumber As Int32, ByVal identifier As String, ByVal type As String)
			_errorCount += 1
			If _errorMessageFileLocation = "" Then _errorMessageFileLocation = System.IO.Path.GetTempFileName
			Dim errorMessageFileWriter As New System.IO.StreamWriter(_errorMessageFileLocation, True, System.Text.Encoding.Default)
			If _errorCount < Me.MaxNumberOfErrorsInGrid Then
				RaiseEvent ReportErrorEvent(row)
			ElseIf _errorCount = Me.MaxNumberOfErrorsInGrid Then
				Dim moretobefoundMessage As New System.Collections.Hashtable
				moretobefoundMessage.Add("Message", "Maximum number of errors for display reached.  Export errors to view full list.")
				RaiseEvent ReportErrorEvent(moretobefoundMessage)
			End If
			errorMessageFileWriter.WriteLine(String.Format("""{1}{0}{2}{0}{3}{0}{4}""", """,""", row("Line Number").ToString, row("DocumentID").ToString, row("FileID").ToString, row("Messages").ToString))
			errorMessageFileWriter.Close()
		End Sub

		Private Sub _uploader_UploadModeChangeEvent(ByVal mode As String, ByVal isBulkEnabled As Boolean) Handles _fileUploader.UploadModeChangeEvent, _bcpuploader.UploadModeChangeEvent
			RaiseEvent UploadModeChangeEvent(mode, isBulkEnabled)
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

		Public Class InvalidLineFormatException
			Inherits System.Exception
			Public Sub New(ByVal lineNumber As Int32, ByVal numberOfColumns As Int32)
				MyBase.New(String.Format("Invalid opticon file line {0}.  There must be at least 4 columns per line in an opticon file, there are {1} in the current line", lineNumber, numberOfColumns))
			End Sub
		End Class
#End Region

		Private Sub ManageErrors()
			If Not _bulkImportManager.ImageRunHasErrors(_caseInfo.ArtifactID, _runId) Then Exit Sub
			If _errorMessageFileLocation = "" Then _errorMessageFileLocation = System.IO.Path.GetTempFileName
			If _errorRowsFileLocation = "" Then _errorRowsFileLocation = System.IO.Path.GetTempFileName
			Dim w As System.IO.StreamWriter
			Dim r As System.IO.StreamReader

			Dim sr As kCura.Utility.GenericCsvReader
			Try
				With _bulkImportManager.GenerateImageErrorFiles(_caseInfo.ArtifactID, _runId, True)
					Me.RaiseStatusEvent(Windows.Process.EventType.Status, "Retrieving errors from server")
					Dim downloader As New FileDownloader(DirectCast(_bulkImportManager.Credentials, System.Net.NetworkCredential), _caseInfo.DocumentPath, _caseInfo.DownloadHandlerURL, _bulkImportManager.CookieContainer, kCura.WinEDDS.Service.Settings.AuthenticationToken)
					Dim errorsLocation As String = System.IO.Path.GetTempFileName
					downloader.MoveTempFileToLocal(errorsLocation, .LogKey, _caseInfo)
					sr = New kCura.Utility.GenericCsvReader(errorsLocation)
					Dim line As String() = sr.ReadLine
					While Not line Is Nothing
						_errorCount += 1
						Dim ht As New System.Collections.Hashtable
						ht.Add("Line Number", Int32.Parse(line(0)))
						ht.Add("DocumentID", line(1))
						ht.Add("FileID", line(2))
						ht.Add("Messages", line(3))
						RaiseReportError(ht, Int32.Parse(line(0)), line(2), "server")
						RaiseEvent StatusMessage(New kCura.Windows.Process.StatusEventArgs(Windows.Process.EventType.Error, Int32.Parse(line(0)) - 1, _fileLineCount, "[Line " & line(0) & "]" & line(3)))
						line = sr.ReadLine
					End While
					sr.Close()
					Dim tmp As String = System.IO.Path.GetTempFileName
					downloader.MoveTempFileToLocal(tmp, .OpticonKey, _caseInfo)
					w = New System.IO.StreamWriter(_errorRowsFileLocation, True, System.Text.Encoding.Default)
					r = New System.IO.StreamReader(tmp, System.Text.Encoding.Default)
					Dim c As Int32 = r.Read
					While Not c = -1
						w.Write(ChrW(c))
						c = r.Read
					End While
					w.Close()
					r.Close()
					kCura.Utility.File.Delete(tmp)
				End With
			Catch ex As Exception
				Try
					sr.Close()
				Catch
				End Try
				Try
					w.Close()
				Catch
				End Try
				Try
					r.Close()
				Catch
				End Try
				Throw
			End Try
		End Sub


		Private Sub _processController_ExportServerErrorsEvent(ByVal exportLocation As String) Handles _processController.ExportServerErrorsEvent
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

			Dim rootFilePath As String = exportLocation & rootFileName
			Dim datetimeNow As System.DateTime = System.DateTime.Now
			Dim errorFilePath As String = rootFilePath & "_ErrorLines_" & datetimeNow.Ticks & defaultExtension
			Dim errorReportPath As String = rootFilePath & "_ErrorReport_" & datetimeNow.Ticks & ".csv"
			System.IO.File.Copy(_errorRowsFileLocation, errorFilePath)
			System.IO.File.Copy(_errorMessageFileLocation, errorReportPath)
		End Sub

		Private Sub _processController_ExportErrorFileEvent(ByVal exportLocation As String) Handles _processController.ExportErrorFileEvent
			If _errorRowsFileLocation Is Nothing Then Exit Sub
			Try
				If System.IO.File.Exists(_errorRowsFileLocation) Then System.IO.File.Copy(_errorRowsFileLocation, exportLocation, True)
			Catch ex As Exception
				If System.IO.File.Exists(_errorRowsFileLocation) Then System.IO.File.Copy(_errorRowsFileLocation, exportLocation, True)
			End Try
		End Sub

		Private Sub _processController_ExportErrorReportEvent(ByVal exportLocation As String) Handles _processController.ExportErrorReportEvent
			If _errorMessageFileLocation Is Nothing Then Exit Sub
			Try
				If System.IO.File.Exists(_errorMessageFileLocation) Then System.IO.File.Copy(_errorMessageFileLocation, exportLocation, True)
			Catch ex As Exception
				If System.IO.File.Exists(_errorMessageFileLocation) Then System.IO.File.Copy(_errorMessageFileLocation, exportLocation, True)
			End Try
		End Sub

		Private Sub _fileUploader_UploadStatusEvent(ByVal s As String) Handles _fileUploader.UploadStatusEvent
			RaiseStatusEvent(kCura.Windows.Process.EventType.Status, s)
		End Sub

		Private Sub _fileUploader_UploadWarningEvent(ByVal s As String) Handles _fileUploader.UploadWarningEvent
			RaiseStatusEvent(kCura.Windows.Process.EventType.Warning, s)
		End Sub

		Private Sub _textUploader_UploadStatusEvent(ByVal s As String) Handles _textUploader.UploadStatusEvent
			RaiseStatusEvent(kCura.Windows.Process.EventType.Status, s)
		End Sub

		Private Sub _textUploader_UploadWarningEvent(ByVal s As String) Handles _textUploader.UploadWarningEvent
			RaiseStatusEvent(kCura.Windows.Process.EventType.Warning, s)
		End Sub

		Private Sub _processController_ParentFormClosingEvent(ByVal processID As Guid) Handles _processController.ParentFormClosingEvent
			If processID.ToString = _processID.ToString Then Me.CleanupTempTables()
		End Sub

		Private Sub CleanupTempTables()
			If Not _runId Is Nothing AndAlso _runId <> "" Then
				Try
					_bulkImportManager.DisposeTempTables(_caseInfo.ArtifactID, _runId)
				Catch
				End Try
			End If
		End Sub

	End Class
End Namespace