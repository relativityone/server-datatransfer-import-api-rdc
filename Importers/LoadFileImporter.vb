Namespace kCura.WinEDDS
	Public Class LoadFileImporter

		Inherits kCura.WinEDDS.LoadFileBase

#Region "Members"

		Private _overwrite As Boolean
		Private WithEvents _uploader As kCura.WinEDDS.FileUploader
		Private _path As String
		Private _pathIsSet As Boolean = False
		Private _selectedIdentifier As DocumentField
		Private _docFieldCollection As DocumentFieldCollection
		Private _parentFolderDTO As kCura.EDDS.WebAPI.FolderManagerBase.Folder
		Private _recordCount As Int32 = -1
		Private _extractFullTextFromNative As Boolean
		Private _allFields As kCura.EDDS.WebAPI.DocumentManagerBase.Field()
		Protected _continue As Boolean
		Protected _processedDocumentIdentifiers As Collections.Specialized.NameValueCollection
		Protected WithEvents _processController As kCura.Windows.Process.Controller
		Protected _offset As Int32 = 0
		Protected _firstTimeThrough As Boolean
		Private _docsToAdd As ArrayList
		Private _docsToUpdate As ArrayList
		Private _filesToAdd As ArrayList
		Private _docsToProcess As ArrayList
		Private _timeKeeper As TimeKeeper
		Private _killWorker As Boolean
		Private _workerRunning As Boolean
		Private WithEvents _lineCounter As kCura.Utility.File.LineCounter
		Private _genericTimestamp As System.DateTime
#End Region

#Region "Accessors"

		Friend WriteOnly Property FilePath() As String
			Set(ByVal value As String)
				If System.IO.File.Exists(value) Then
					_path = value
					_pathIsSet = True
				End If
			End Set
		End Property

		Public ReadOnly Property AllDocumentFields() As kCura.EDDS.WebAPI.DocumentManagerBase.Field()
			Get
				If _allFields Is Nothing Then
					_allFields = _fieldQuery.RetrieveAllAsArray(_folderID)
				End If
				Dim field As kCura.EDDS.WebAPI.DocumentManagerBase.Field
				For Each field In _allFields
					field.Value = Nothing
				Next
				Return _allFields
			End Get
		End Property

#End Region

#Region "Constructors"

		Public Sub New(ByVal args As LoadFile, ByVal processController As kCura.Windows.Process.Controller, ByVal timeZoneOffset As Int32)
			Me.New(args, processController, timeZoneOffset, True)
		End Sub

		Public Sub New(ByVal args As LoadFile, ByVal processController As kCura.Windows.Process.Controller, ByVal timeZoneOffset As Int32, ByVal autoDetect As Boolean)
			MyBase.New(args, timeZoneOffset, autoDetect)
			_overwrite = args.OverwriteDestination
			_uploader = New kCura.WinEDDS.FileUploader(args.Credentials, _documentManager.GetDocumentDirectoryByCaseArtifactID(args.CaseInfo.ArtifactID) & "\", args.CookieContainer)
			_extractFullTextFromNative = args.ExtractFullTextFromNativeFile
			_selectedIdentifier = args.SelectedIdentifierField
			_docFieldCollection = New DocumentFieldCollection(args.FieldMap.DocumentFields)
			If autoDetect Then _parentFolderDTO = _foldermanager.Read(_folderID)
			_processController = processController
			_continue = True
			_firstTimeThrough = True
		End Sub

#End Region

#Region "Utility"

		Public Function GetColumnNames(ByVal path As String) As String()
			reader = New System.IO.StreamReader(path, System.Text.Encoding.Default)
			Dim columnNames As String() = GetLine
			If Not _firstLineContainsColumnNames Then
				Dim i As Int32
				For i = 0 To columnNames.Length - 1
					columnNames(i) = "Column (" & (i + 1).ToString & ")"
				Next
			Else
				Dim i As Int32
				For i = 0 To columnNames.Length - 1
					columnNames(i) &= String.Format(" ({0})", i + 1)
				Next
			End If
			reader.Close()
			Me.ResetLineCounter()
			Return columnNames
		End Function

#End Region

#Region "Main"

		Public Overloads Sub ReadFile()
			ReadFile(_path)
		End Sub

		Public Overloads Overrides Function ReadFile(ByVal path As String) As Object
			Try
				RaiseEvent StartFileImport()
				Dim markStart As DateTime = DateTime.Now
				InitializeMembers(path)
				StartMassProcessor()
				While _continue AndAlso Not HasReachedEOF
					Try
						_processedDocumentIdentifiers.Add(ManageDocument(GetLine), CurrentLineNumber.ToString)
					Catch ex As kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
						WriteError(ex.Message)
					Catch ex As System.Exception
						WriteFatalError(Me.CurrentLineNumber, ex)
					End Try
				End While
				StopMassProcessor()
				While _workerRunning
					System.Threading.Thread.CurrentThread.Join(1000)
				End While
				RaiseEvent EndFileImport()
				WriteEndImport("Finish")
				Me.Close()
				_timeKeeper.Add("Total", DateTime.Now.Subtract(markStart).TotalMilliseconds)
				Return True
			Catch ex As Exception
				WriteFatalError(Me.CurrentLineNumber, ex)
			End Try
		End Function

		Private Sub InitializeMembers(ByVal path As String)
			_lineCounter = New kCura.Utility.File.LineCounter
			_lineCounter.Path = path
			_lineCounter.CountLines()
		End Sub

		Private Function ManageDocument(ByVal values As String()) As String
			If _docsToProcess.Count > 1000 Then
				While _docsToProcess.Count > 500
					System.Threading.Thread.CurrentThread.Join(1000)
				End While
			End If
			Dim markStart As DateTime = DateTime.Now
			Dim filename As String
			Dim fileGuid As String = String.Empty
			Dim uploadFile As Boolean = (_filePathColumnIndex <> -1) AndAlso _uploadFiles
			Dim fileExists As Boolean
			Dim fieldCollection As New DocumentFieldCollection
			Dim identityValue As String = String.Empty
			Dim documentArtifactID As Int32
			Dim markUploadStart As DateTime = DateTime.Now
			If uploadFile Then
				filename = values(_filePathColumnIndex)
				fileExists = System.IO.File.Exists(filename)
				If filename <> String.Empty AndAlso Not fileExists Then Throw New InvalidFilenameException(filename)
				If fileExists Then
					Dim now As DateTime = DateTime.Now
					fileGuid = _uploader.UploadFile(filename, _folderID)
					filename = filename.Substring(filename.LastIndexOf("\") + 1)
					WriteStatusLine(Windows.Process.EventType.Status, String.Format("End upload file. ({0}ms)", DateTime.op_Subtraction(DateTime.Now, now).Milliseconds))
				End If
			End If
			_timeKeeper.Add("UploadFile", DateTime.Now.Subtract(markUploadStart).TotalMilliseconds)
			Dim markPrepareFields As DateTime = DateTime.Now
			identityValue = PrepareFieldCollectionAndExtractIdentityValue(fieldCollection, values)
			_timeKeeper.Add("PrepareFields", DateTime.Now.Subtract(markPrepareFields).TotalMilliseconds)
			If identityValue = String.Empty Then
				Throw New IdentityValueNotSetException
			ElseIf Not _processedDocumentIdentifiers(identityValue) Is Nothing Then
				Throw New IdentifierOverlapException(identityValue, _processedDocumentIdentifiers(identityValue))
			End If
			Dim metadoc As New MetaDocument(fileGuid, identityValue, fieldCollection, fileExists AndAlso uploadFile AndAlso fileGuid <> String.Empty, filename, uploadFile, CurrentLineNumber)
			_docsToProcess.Add(metadoc)
			Return identityValue
		End Function

#End Region

#Region "Thread Management"

		Private Sub MassProcessWorker()
			_workerRunning = True
			While (Not _killWorker OrElse _docsToProcess.Count > 0) AndAlso _continue
				If _docsToProcess.Count > 0 Then
					Dim metaDoc As MetaDocument = DirectCast(_docsToProcess(0), MetaDocument)
					_docsToProcess.RemoveAt(0)
					Try
						ManageDocumentMetaData(metaDoc)
					Catch ex As System.Exception
						WriteFatalError(CurrentLineNumber, ex)
					End Try
				Else
					System.Threading.Thread.CurrentThread.Join(1000)
				End If
			End While
			_workerRunning = False
		End Sub

		Private Sub StartMassProcessor()
			_killWorker = False
			Dim thread As New System.Threading.Thread(AddressOf MassProcessWorker)
			thread.Start()
		End Sub

		Private Sub StopMassProcessor()
			_killWorker = True
		End Sub

#End Region

#Region "WebService Calls"

		Private Sub ManageDocumentMetaData(ByVal metaDoc As MetaDocument)
			Try
				Dim doc As kCura.EDDS.WebAPI.DocumentManagerBase.Document
				Dim documentArtifactID As Int32
				Dim markReadDoc As DateTime = DateTime.Now
				Try
					doc = _documentManager.ReadFromIdentifier(_folderID, _selectedIdentifier.FieldName, metaDoc.IdentityValue)
				Catch ex As Exception
					Throw New AmbiguousIdentifierValueException(ex)
				End Try
				_timeKeeper.Add("ReadUpload", DateTime.Now.Subtract(markReadDoc).TotalMilliseconds)
				markReadDoc = DateTime.Now
				If doc Is Nothing Then
					documentArtifactID = CreateDocument(metaDoc, _extractFullTextFromNative)
				Else
					documentArtifactID = UpdateDocument(doc, metaDoc, _extractFullTextFromNative)
				End If
				Dim o As New Object
				_timeKeeper.Add("Manage", DateTime.Now.Subtract(markReadDoc).TotalMilliseconds)
			Catch ex As kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
				WriteError(ex.Message)
			Catch ex As System.Exception
				WriteFatalError(metaDoc.LineNumber, ex)
			End Try
			WriteStatusLine(Windows.Process.EventType.Progress, String.Format("Document '{0}' processed.", metaDoc.IdentityValue), metaDoc.LineNumber)
		End Sub

		Private Function CreateDocument(ByVal mdoc As MetaDocument, ByVal extractText As Boolean) As Int32
			Return CreateDocument(mdoc.FieldCollection, mdoc.IdentityValue, mdoc.FileGuid <> String.Empty AndAlso extractText, mdoc.Filename, mdoc.FileGuid, mdoc)
		End Function

		Private Function CreateDocument(ByVal fieldCollection As DocumentFieldCollection, ByVal identityValue As String, ByVal extractText As Boolean, ByVal filename As String, ByVal fileguid As String, ByVal mdoc As MetaDocument) As Int32
			Dim documentDTO As New kCura.EDDS.WebAPI.DocumentManagerBase.Document
			documentDTO.Fields = AllDocumentFields
			documentDTO.ParentArtifactID = New NullableTypes.NullableInt32(_folderID)
			documentDTO.ContainerID = _parentFolderDTO.ContainerID
			documentDTO.AccessControlListIsInherited = True
			documentDTO.AccessControlListID = _parentFolderDTO.AccessControlListID
			documentDTO.DocumentAgentFlags = New kCura.EDDS.WebAPI.DocumentManagerBase.DocumentAgentFlags
			documentDTO.DocumentAgentFlags.UpdateFullText = extractText
			documentDTO.DocumentAgentFlags.IndexStatus = kCura.EDDS.Types.IndexStatus.IndexLowPriority
			Dim now As System.DateTime = System.DateTime.Now
			SetFieldValues(documentDTO, fieldCollection)

			If mdoc.UploadFile And mdoc.IndexFileInDB Then
				Dim fileDTO As kCura.EDDS.WebAPI.DocumentManagerBase.File = CreateFileDTO(filename, fileguid)
				documentDTO.Files = New kCura.EDDS.WebApi.DocumentManagerBase.File() {fileDTO}
			End If

			Try
				WriteStatusLine(Windows.Process.EventType.Status, String.Format("Creating document '{0}' in database.", identityValue))
				Return _documentManager.Create(documentDTO)
			Catch ex As Exception
				Throw New DocumentDomainException(ex)
			End Try
		End Function

		Private Function UpdateDocument(ByVal docDTO As kCura.EDDS.WebAPI.DocumentManagerBase.Document, ByVal mdoc As MetaDocument, ByVal extractText As Boolean) As Int32
			Return UpdateDocument(docDTO, mdoc.FieldCollection, mdoc.IdentityValue, mdoc.UploadFile AndAlso mdoc.FileGuid <> String.Empty, mdoc.FileGuid <> String.Empty AndAlso extractText, mdoc.Filename, mdoc.FileGuid)
		End Function

		Private Function UpdateDocument(ByVal docDTO As kCura.EDDS.WebAPI.DocumentManagerBase.Document, ByVal fieldCollection As DocumentFieldCollection, ByVal identityValue As String, ByVal uploadFile As Boolean, ByVal extractText As Boolean, ByVal fileName As String, ByVal fileGuid As String) As Int32
			If _overwrite Then
				WriteStatusLine(Windows.Process.EventType.Status, String.Format("Updating document '{0}' in database.", identityValue))
				docDTO.DocumentAgentFlags.UpdateFullText = extractText
				docDTO.DocumentAgentFlags.IndexStatus = kCura.EDDS.Types.IndexStatus.IndexLowPriority
				SetFieldValues(docDTO, fieldCollection)
				Dim fileList As New ArrayList
				If uploadFile Then
					Dim oldFile As kCura.EDDS.WebAPI.DocumentManagerBase.File
					Dim hasOldFile As Boolean = False
					If Not docDTO.Files Is Nothing Then
						For Each oldFile In docDTO.Files
							If oldFile.Type = kCura.EDDS.Types.FileType.Native Then
								hasOldFile = True
								Exit For
							End If
						Next
					End If
					Dim fileDTO As kCura.EDDS.WebAPI.DocumentManagerBase.File = CreateFileDTO(fileName, fileGuid)
					If Not hasOldFile Then
						fileList.Add(fileDTO)
					Else
						fileList.Add(fileDTO)
						fileList.Add(oldFile)
					End If
				End If
				Dim fullTextFileDTO As kCura.EDDS.WebAPI.DocumentManagerBase.File
				For Each fullTextFileDTO In docDTO.Files
					If fullTextFileDTO.Type = 2 Then
						Exit For
					End If
				Next
				If Not fullTextFileDTO Is Nothing AndAlso fullTextFileDTO.Type = 2 AndAlso Not extractText Then
					fileList.Add(fullTextFileDTO)
				End If
				If fileList.Count = 0 Then
					docDTO.Files = Nothing
				Else
					docDTO.Files = DirectCast(fileList.ToArray(GetType(kCura.EDDS.WebAPI.DocumentManagerBase.File)), kCura.EDDS.WebAPI.DocumentManagerBase.File())
				End If
				Try
					_documentManager.Update(docDTO)
				Catch ex As Exception
					Throw New DocumentDomainException(ex)
				End Try
				Return docDTO.ArtifactID
			Else
					Throw New DocumentOverwriteException
				End If
		End Function

		Private Function CreateFileDTO(ByVal filename As String, ByVal fileguid As String) As kCura.EDDS.WebAPI.DocumentManagerBase.File
			Dim fileDTO As New kCura.EDDS.WebAPI.DocumentManagerBase.File
			fileDTO.DocumentArtifactID = 0
			fileDTO.Filename = filename
			fileDTO.Guid = fileguid
			fileDTO.Order = 0
			fileDTO.Type = kCura.EDDS.Types.FileType.Native
			Return fileDTO
		End Function

#End Region

#Region "Field Preparation"

		Private Function PrepareFieldCollectionAndExtractIdentityValue(ByVal fieldCollection As DocumentFieldCollection, ByVal values As String()) As String
			Dim item As LoadFileFieldMap.LoadFileFieldMapItem
			Dim identityValue As String = String.Empty
			Dim docfield As DocumentField
			For Each item In _fieldmap
				If _firstTimeThrough Then
					If item.DocumentField Is Nothing Then
						WriteStatusLine(Windows.Process.EventType.Warning, String.Format("File column '{0}' will be unmapped", item.NativeFileColumnIndex + 1), 0)
					End If
					If item.NativeFileColumnIndex = -1 Then
						WriteStatusLine(Windows.Process.EventType.Warning, String.Format("Field '{0}' will be unmapped", item.DocumentField.FieldName), 0)
					End If
				End If
				If Not item.DocumentField Is Nothing Then
					docfield = New DocumentField(item.DocumentField)
					MyBase.SetFieldValue(docfield, values, item.NativeFileColumnIndex)
					If docfield.FieldName = _selectedIdentifier.FieldName Then
						identityValue = docfield.Value
					End If
					fieldCollection.Add(docfield)
				End If
			Next
			_firstTimeThrough = False
			Return identityValue
		End Function

#End Region

#Region "Field Value Manipulation"

		Private Sub SetFieldValues(ByVal documentDTO As kCura.EDDS.WebAPI.DocumentManagerBase.Document, ByVal selectedFields As DocumentFieldCollection)
			Dim fieldDTO As kCura.EDDS.WebAPI.DocumentManagerBase.Field
			Dim docField As DocumentField
			Dim encoder As New System.Text.ASCIIEncoding
			Dim value As String
			For Each fieldDTO In documentDTO.Fields
				docField = selectedFields.Item(fieldDTO.ArtifactID)
				If docField Is Nothing Then
					If fieldDTO.FieldCategoryID = kCura.EDDS.Types.FieldCategory.FullText Then
						fieldDTO.Value = String.Empty
					Else
						If fieldDTO.Value Is Nothing Then
							fieldDTO.Value = String.Empty
						End If
					End If
				Else
					Select Case fieldDTO.FieldType
						Case kCura.EDDS.WebAPI.DocumentManagerBase.FieldType.MultiCode
							SetMultiCode(fieldDTO, docField)
						Case Else
							If docField.FieldCategoryID = kCura.EDDS.Types.FieldCategory.FullText Then
								fieldDTO.Value = _uploader.UploadTextAsFile(docField.Value, _folderid, System.Guid.NewGuid.ToString)
								'fieldDTO.Value = encoder.GetBytes(docField.Value)
							Else
								fieldDTO.Value = docField.Value
							End If
					End Select
				End If
			Next
			fieldDTO = Nothing
			encoder = Nothing
			value = Nothing
		End Sub

		Public Sub SetMultiCode(ByVal fieldDTO As kCura.EDDS.WebAPI.DocumentManagerBase.Field, ByVal docField As DocumentField)
			Dim existingValue As String
			If fieldDTO.Value Is Nothing Then
				fieldDTO.Value = String.Empty
			End If
			existingValue = CType(fieldDTO.Value, String)
			Dim multiCodeID As Int32
			Dim j As Int32
			Dim valueArray As String() = docField.Value.Split(";".ToCharArray)
			If valueArray.Length = 1 AndAlso valueArray(0) = String.Empty Then
				If existingValue = String.Empty Then
					fieldDTO.Value = _multicodeManager.CreateNewMultiCodeID(_folderID).ToString
				Else
					_multiCodeManager.DeleteFromMultiCodeArtifactByMultiCodeID(_folderID, Int32.Parse(existingValue))
				End If
				'fieldDTO.Value = String.Empty
			Else
				Dim codeArtifactIDs(valueArray.Length - 1) As Int32
				For j = 0 To codeArtifactIDs.Length - 1
					Try
						codeArtifactIDs(j) = Int32.Parse(valueArray(j))
					Catch ex As Exception
					End Try
				Next
				If (existingValue = String.Empty OrElse existingValue = "0") AndAlso valueArray.Length > 0 Then
					multiCodeID = _multiCodeManager.CreateNewMultiCodeID(_folderID)
				Else
					multiCodeID = Int32.Parse(existingValue)
				End If
				If multiCodeID > 0 Then
					_multiCodeManager.DeleteFromMultiCodeArtifactByMultiCodeID(_folderID, multiCodeID)
					Dim codeFieldValues As New System.Collections.ArrayList
					codeFieldValues.AddRange(codeArtifactIDs)
					_multiCodeManager.SetMultiCodeValues(multiCodeID, codeFieldValues.ToArray)
					fieldDTO.Value = multiCodeID.ToString
				Else
					fieldDTO.Value = String.Empty
				End If
			End If
		End Sub

#End Region

#Region "Status Window"

		Private Sub WriteStatusLine(ByVal et As kCura.Windows.Process.EventType, ByVal line As String, ByVal lineNumber As Int32)
			line = line & String.Format(" [line {0}]", lineNumber)
			'If Not _status Is Nothing AndAlso Not _status.IsDisposed Then
			'	_status.UpdateStatusWindow(line)
			'End If
			RaiseEvent StatusMessage(New kCura.Windows.Process.StatusEventArgs(et, lineNumber + _offset, _recordCount, line))
		End Sub

		Private Sub WriteStatusLine(ByVal et As kCura.Windows.Process.EventType, ByVal line As String)
			WriteStatusLine(et, line, Me.CurrentLineNumber)
		End Sub

		Private Sub WriteFatalError(ByVal lineNumber As Int32, ByVal ex As Exception)
			RaiseEvent FatalErrorEvent("Error processing line: " + lineNumber.ToString, ex)
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

		Private Sub WriteEndImport(ByVal line As String)
			WriteStatusLine(kCura.Windows.Process.EventType.End, line)
		End Sub

		Private Sub _uploader_UploadStatusEvent(ByVal s As String) Handles _uploader.UploadStatusEvent
			WriteStatusLine(kCura.Windows.Process.EventType.Status, s)
		End Sub

#End Region

#Region "Public Events"

		Public Event FatalErrorEvent(ByVal message As String, ByVal ex As Exception)
		Public Event StatusMessage(ByVal args As kCura.Windows.Process.StatusEventArgs)
		Public Event EndFileImport()
		Public Event StartFileImport()
		Public Event UploadModeChangeEvent(ByVal mode As String)

#Region "File Prep Event"
		Public Event FilePrepEvent(ByVal e As FilePrepEventArgs)
		Public Class FilePrepEventArgs
			Public Enum FilePrepEventType
				OpenFile
				CloseFile
				ReadEvent
			End Enum
			Private _type As FilePrepEventType
			Private _newlinesRead As Int32
			Private _bytesRead As Int32
			Private _totalBytes As Int32
			Private _stepSize As Int32
			Private _startTime As System.DateTime
			Private _endTime As System.DateTime

#Region "Accessors"

			Public ReadOnly Property Type() As FilePrepEventType
				Get
					Return _type
				End Get
			End Property

			Public ReadOnly Property NewlinesRead() As Int32
				Get
					Return _newlinesRead
				End Get
			End Property

			Public ReadOnly Property BytesRead() As Int32
				Get
					Return _bytesRead
				End Get
			End Property

			Public ReadOnly Property TotalBytes() As Int32
				Get
					Return _totalBytes
				End Get
			End Property

			Public ReadOnly Property StepSize() As Int32
				Get
					Return _stepSize
				End Get
			End Property

			Public ReadOnly Property StartTime() As System.DateTime
				Get
					Return _startTime
				End Get
			End Property

			Public ReadOnly Property EndTime() As System.DateTime
				Get
					Return _endTime
				End Get
			End Property

#End Region

			Public Sub New(ByVal eventType As FilePrepEventType, ByVal newlines As Int32, ByVal bytes As Int32, ByVal total As Int32, ByVal [step] As Int32, ByVal start As System.DateTime, ByVal [end] As System.DateTime)
				_type = eventType
				_newlinesRead = newlines
				_bytesRead = bytes
				_totalBytes = total
				_stepSize = [step]
				_startTime = start
				_endTime = [end]
			End Sub
		End Class
#End Region

#End Region

#Region "Event Handlers"

		Private Sub _uploader_UploadModeChangeEvent(ByVal mode As String) Handles _uploader.UploadModeChangeEvent
			RaiseEvent UploadModeChangeEvent(mode)
		End Sub

		Private Sub _processController_HaltProcessEvent(ByVal processID As System.Guid) Handles _processController.HaltProcessEvent
			_continue = False
			_lineCounter.StopCounting()
		End Sub

#End Region

#Region "Exceptions"

		Public Class IdentityValueNotSetException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New()
				MyBase.New("Identity value not set")
			End Sub
		End Class

		Public Class DocumentDomainException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal ex As System.Exception)
				MyBase.New("Error accessing document information in domain layer", ex)
			End Sub
		End Class

		Public Class AmbiguousIdentifierValueException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal parentException As System.Exception)
				MyBase.New("Identifier has more than one row associated with it", parentException)
			End Sub
		End Class

		Public Class DocumentOverwriteException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New()
				MyBase.New("Identifier points to document that would be overwritten")
			End Sub
		End Class

		Public Class InvalidFilenameException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal filename As String)
				MyBase.New(String.Format("File '{0}' not found.", filename))
			End Sub
		End Class

		Public Class FileUploadFailedException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New()
				MyBase.New(String.Format("File upload failed - file not added."))
			End Sub
		End Class

		Public Class IdentifierOverlapException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal identityValue As String, ByVal previousLineNumber As String)
				MyBase.New(String.Format("Document '({0})' has been previously processed in this file on line {1}.", identityValue, previousLineNumber))
			End Sub
		End Class
#End Region

#Region "Reporting"

		Public Class TimeKeeper
			Private _hashtable As Hashtable
			Public Sub New()
				_hashtable = New Hashtable
			End Sub

			Public Sub Add(ByVal key As String, ByVal value As Double)
				If _hashtable(key) Is Nothing Then
					_hashtable.Add(key, value)
				Else
					_hashtable(key) = value + DirectCast(_hashtable(key), Double)
				End If
			End Sub

			Public Function ToCollectionString() As String
				Dim sb As New System.Text.StringBuilder
				Dim nl As String = System.Environment.NewLine
				Dim tab As String = Microsoft.VisualBasic.ControlChars.Tab
				Dim key As String
				Dim i As Int32
				For Each key In _hashtable.Keys
					sb.Append(key & tab)
				Next
				sb.Append(nl)
				For Each key In _hashtable.Keys
					sb.Append((DirectCast(_hashtable(key), Double) / 1000).ToString & tab)
				Next
				sb.Append(nl)
				Return sb.ToString
			End Function
		End Class

#End Region

#Region "Preprocessing"

		Private Sub _lineCounter_OnEvent(ByVal e As kCura.Utility.File.LineCounter.EventArgs) Handles _lineCounter.OnEvent
			Select Case e.Type
				Case kCura.Utility.File.LineCounter.EventType.Begin
					_genericTimestamp = System.DateTime.Now
					RaiseEvent FilePrepEvent(New FilePrepEventArgs(FilePrepEventArgs.FilePrepEventType.OpenFile, 0, 0, e.TotalBytes, e.StepSize, _genericTimestamp, System.DateTime.Now))
				Case kCura.Utility.File.LineCounter.EventType.Progress
					RaiseEvent FilePrepEvent(New FilePrepEventArgs(FilePrepEventArgs.FilePrepEventType.ReadEvent, e.NewlinesRead, e.BytesRead, e.TotalBytes, e.StepSize, _genericTimestamp, System.DateTime.Now))
				Case kCura.Utility.File.LineCounter.EventType.Complete
					_recordCount = e.NewlinesRead
					RaiseEvent FilePrepEvent(New FilePrepEventArgs(FilePrepEventArgs.FilePrepEventType.ReadEvent, e.NewlinesRead, e.TotalBytes, e.TotalBytes, e.StepSize, _genericTimestamp, System.DateTime.Now))
					Dim path As String = _lineCounter.Path
					_columnHeaders = GetColumnNames(path)
					_processedDocumentIdentifiers = New Collections.Specialized.NameValueCollection
					Reader = New System.IO.StreamReader(path, System.Text.Encoding.Default)
					_docsToAdd = New ArrayList
					_docsToUpdate = New ArrayList
					_docsToProcess = New ArrayList
					If _firstLineContainsColumnNames Then
						_columnHeaders = GetLine
						_recordCount -= 1
						_offset = -1
					End If
					If Not _filePathColumn Is Nothing Then
						Dim openParenIndex As Int32 = _filePathColumn.LastIndexOf("("c) + 1
						Dim closeParenIndex As Int32 = _filePathColumn.LastIndexOf(")"c)
						_filePathColumnIndex = Int32.Parse(_filePathColumn.Substring(openParenIndex, closeParenIndex - openParenIndex)) - 1
					Else
						_filePathColumnIndex = -1
					End If
					_timeKeeper = New TimeKeeper
			End Select
		End Sub

#End Region

	End Class
End Namespace