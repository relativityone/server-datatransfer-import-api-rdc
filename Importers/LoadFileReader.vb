Imports kCura.EDDS.Types.MassImport
Imports kCura.WinEDDS.Api
Namespace kCura.WinEDDS
	Public Class LoadFileReader

		Inherits kCura.Utility.DelimitedFileImporter
		Implements IArtifactReader

#Region "Members"

		Protected _columnsAreInitialized As Boolean
		Protected _columnHeaders As String()
		Protected _documentManager As kCura.WinEDDS.Service.DocumentManager
		Protected _uploadManager As kCura.WinEDDS.Service.FileIO
		Protected _codeManager As kCura.WinEDDS.Service.CodeManager
		Protected _folderManager As kCura.WinEDDS.Service.FolderManager
		Protected _fieldQuery As kCura.WinEDDS.Service.FieldQuery
		Protected _bulkImportManager As kCura.WinEDDS.Service.BulkImportManager
		Protected _fileManager As kCura.WinEDDS.Service.FileManager
		Protected _usermanager As kCura.WinEDDS.Service.UserManager
		Protected _objectManager As kCura.WinEDDS.Service.ObjectManager
		Protected _filePathColumn As String
		Protected _filePathColumnIndex As Int32
		Protected _firstLineContainsColumnNames As Boolean
		Protected _docFields As DocumentField()
		Protected _multiValueSeparator As Char()
		Protected _allCodes As kCura.Data.DataView
		Protected _allCodeTypes As kCura.Data.DataView
		Protected _folderID As Int32
		Protected _caseSystemID As Int32
		Protected _caseArtifactID As Int32
		Protected _timeZoneOffset As Int32
		Protected _autoDetect As Boolean
		Protected _uploadFiles As Boolean
		Protected _fieldMap As LoadFileFieldMap
		Protected _createFolderStructure As Boolean
		Protected _destinationFolder As String
		Protected _extractMd5Hash As Boolean
		Protected _fullTextColumnMapsToFileLocation As Boolean
		Private _users As UserCollection
		Protected _sourceFileEncoding As System.Text.Encoding
		Protected _extractedTextFileEncoding As System.Text.Encoding
		Protected _extractedTextFileEncodingName As String
		Protected _artifactTypeID As Int32
		Protected MulticodeMatrix As System.Collections.Hashtable
		Protected _hierarchicalMultiValueFieldDelmiter As String
		Protected _previewCodeCount As New System.Collections.Specialized.HybridDictionary
		Protected _startLineNumber As Int64
		Protected _keyFieldID As Int32
		Protected _settings As kCura.WinEDDS.LoadFile
		Private _codeValidator As CodeValidator.Base
		Private _codesCreated As Int32 = 0
		Private _errorLogFileName As String = ""
		Private _errorLogWriter As System.IO.StreamWriter
		Private WithEvents _lineCounter As kCura.Utility.File.LineCounter
		Private _recordCount As Int64 = -1
		Private _genericTimestamp As System.DateTime
		Private _trackErrorsInFieldValues As Boolean
#End Region

#Region " Constructors "

		Public Sub New(ByVal args As LoadFile, ByVal trackErrorsAsFieldValues As Boolean)
			MyBase.New(args.RecordDelimiter, args.QuoteDelimiter, args.NewlineDelimiter, True)
			_settings = args
			_settings = args
			_trackErrorsInFieldValues = trackErrorsAsFieldValues
			_docFields = args.FieldMap.DocumentFields
			_filePathColumn = args.NativeFilePathColumn
			_firstLineContainsColumnNames = args.FirstLineContainsHeaders
			_fieldMap = args.FieldMap
			_documentManager = New kCura.WinEDDS.Service.DocumentManager(args.Credentials, args.CookieContainer)
			_uploadManager = New kCura.WinEDDS.Service.FileIO(args.Credentials, args.CookieContainer)
			_codeManager = New kCura.WinEDDS.Service.CodeManager(args.Credentials, args.CookieContainer)
			_folderManager = New kCura.WinEDDS.Service.FolderManager(args.Credentials, args.CookieContainer)
			_fieldQuery = New kCura.WinEDDS.Service.FieldQuery(args.Credentials, args.CookieContainer)
			_fileManager = New kCura.WinEDDS.Service.FileManager(args.Credentials, args.CookieContainer)
			_usermanager = New kCura.WinEDDS.Service.UserManager(args.Credentials, args.CookieContainer)
			_bulkImportManager = New kCura.WinEDDS.Service.BulkImportManager(args.Credentials, args.CookieContainer)
			_objectManager = New kCura.WinEDDS.Service.ObjectManager(args.Credentials, args.CookieContainer)
			_keyFieldID = args.IdentityFieldId
			_multiValueSeparator = args.MultiRecordDelimiter.ToString.ToCharArray
			_folderID = args.DestinationFolderID
			_caseSystemID = args.CaseInfo.RootArtifactID
			_caseArtifactID = args.CaseInfo.ArtifactID
			_uploadFiles = args.LoadNativeFiles
			_createFolderStructure = args.CreateFolderStructure
			_destinationFolder = args.FolderStructureContainedInColumn
			_extractMd5Hash = args.ExtractMD5HashFromNativeFile
			_fullTextColumnMapsToFileLocation = args.FullTextColumnContainsFileLocation
			_sourceFileEncoding = args.SourceFileEncoding
			_extractedTextFileEncoding = args.ExtractedTextFileEncoding
			_extractedTextFileEncodingName = args.ExtractedTextFileEncodingName
			_artifactTypeID = args.ArtifactTypeID
			_hierarchicalMultiValueFieldDelmiter = args.HierarchicalValueDelimiter
			_previewCodeCount = args.PreviewCodeCount
			_startLineNumber = args.StartLineNumber
			MulticodeMatrix = New System.Collections.Hashtable
			If _keyFieldID > 0 AndAlso args.OverwriteDestination.ToLower <> "strict" Then
				_keyFieldID = -1
			End If
			If _keyFieldID = -1 Then
				For Each field As DocumentField In _docFields
					If field.FieldCategory = DynamicFields.Types.FieldCategory.Identifier Then
						_keyFieldID = field.FieldID
						Exit For
					End If
				Next
			End If

			Me.InitializeLineCounter(args.FilePath)
			'Reader = New System.IO.StreamReader(args.FilePath, args.SourceFileEncoding)
		End Sub

#End Region

#Region " Initialization "

		Private Sub InitializeLineCounter(ByVal path As String)
			_lineCounter = New kCura.Utility.File.LineCounter
			_lineCounter.Path = path
		End Sub

		Private Sub EnsureReader()
			If Me.Reader Is Nothing Then
				If _sourceFileEncoding Is Nothing Then _sourceFileEncoding = System.Text.Encoding.Default
				Me.Reader = New System.IO.StreamReader(_settings.FilePath, _sourceFileEncoding, True)
			End If
		End Sub

#End Region

#Region " Field Value Management "

		Private Sub SetFieldValue(ByVal field As ArtifactField, ByVal value As String, ByVal column As Int32)
			Try
				Select Case field.Type
					Case kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Boolean
						field.Value = Me.GetNullableBoolean(value.Trim, column)
					Case kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Integer
						field.Value = Me.GetNullableInteger(value.Trim, column)
					Case kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Currency, kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Decimal
						field.Value = Me.GetNullableDecimal(value.Trim, column)
					Case kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Date
						field.Value = Me.GetNullableDateTime(value.Trim, column)
					Case kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Code, DynamicFields.Types.FieldTypeHelper.FieldType.Object, DynamicFields.Types.FieldTypeHelper.FieldType.User, DynamicFields.Types.FieldTypeHelper.FieldType.File
						field.Value = value.Trim
						If field.Value.ToString = String.Empty Then field.Value = Nothing
					Case kCura.DynamicFields.Types.FieldTypeHelper.FieldType.MultiCode, DynamicFields.Types.FieldTypeHelper.FieldType.Objects
						Dim al As New System.Collections.ArrayList
						For Each item As String In value.Split(_settings.MultiRecordDelimiter)
							If Not item.Trim = String.Empty Then al.Add(item.Trim)
						Next
						field.Value = DirectCast(al.ToArray(GetType(String)), String())
					Case kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Varchar
						field.Value = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(Me.GetNullableFixedString(value, column, field.TextLength))
					Case kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Text
						If _settings.FullTextColumnContainsFileLocation Then
							field.Value = value
						Else
							field.Value = value.Replace(Me.NewlineProxy, vbNewLine)
						End If
					Case Else
						Throw New System.Exception("Unsupported field type '" & field.Type.ToString & "'")
				End Select
			Catch ex As ImporterExceptionBase
				If _trackErrorsInFieldValues Then
					field.Value = ex
				Else
					Throw
				End If

			End Try
		End Sub

		Public Overloads Function GetNullableDateTime(ByVal value As String, ByVal column As Int32) As NullableDateTime
			Dim nullableDateValue As NullableDateTime
			Try
				nullableDateValue = MyBase.GetNullableDateTime(value, column)
			Catch ex As System.Exception
				Select Case value.Trim
					Case "00/00/0000", "0/0/0000", "0/0/00", "00/00/00", "0/00", "0/0000", "00/00", "00/0000", "0"
						nullableDateValue = NullableDateTime.Null
					Case Else
						Try

							If System.Text.RegularExpressions.Regex.IsMatch(value.Trim, "\d\d\d\d\d\d\d\d") Then
								If value.Trim = "00000000" Then
									nullableDateValue = NullableDateTime.Null
								Else
									Dim v As String = value.Trim
									Dim year As Int32 = Int32.Parse(v.Substring(0, 4))
									Dim month As Int32 = Int32.Parse(v.Substring(4, 2))
									Dim day As Int32 = Int32.Parse(v.Substring(6, 2))
									Try
										nullableDateValue = New NullableDateTime(New System.DateTime(year, month, day))
									Catch dx As System.Exception
										Throw New kCura.Utility.DelimitedFileImporter.DateException(Me.CurrentLineNumber, column)
									End Try
								End If
							Else
								Throw New kCura.Utility.DelimitedFileImporter.DateException(Me.CurrentLineNumber, column)
							End If
						Catch
							Throw New kCura.Utility.DelimitedFileImporter.DateException(Me.CurrentLineNumber, column)
						End Try
				End Select
			End Try
			Try
				If nullableDateValue.IsNull Then Return nullableDateValue
				Dim datevalue As DateTime
				datevalue = nullableDateValue.Value
				Dim timeZoneOffset As Int32 = 0
				If datevalue < DateTime.Parse("1/1/1753") Then
					Throw New kCura.Utility.DelimitedFileImporter.DateException(Me.CurrentLineNumber, column)
				End If
				Return New NullableDateTime(datevalue)
			Catch ex As Exception
				Throw New kCura.Utility.DelimitedFileImporter.DateException(Me.CurrentLineNumber, column)
			End Try
		End Function

#End Region

#Region " Error File Management "

		Private Function ToDelimetedLine(ByVal values As String()) As String
			Dim sb As New System.Text.StringBuilder
			Dim value As String
			For Each value In values
				sb.Append(Me.Bound & value & Me.Bound & Me.Delimiter)
			Next
			sb.Remove(sb.Length - 1, 1)
			Return sb.ToString
		End Function

		Private Sub LogErrorLine(ByVal values As String())
			If values Is Nothing Then Exit Sub
			If _errorLogFileName = "" Then
				_errorLogFileName = System.IO.Path.GetTempFileName()
				_errorLogWriter = New System.IO.StreamWriter(_errorLogFileName, False, _sourceFileEncoding)
				_errorLogWriter.WriteLine(Me.ToDelimetedLine(_columnHeaders))
			End If
			_errorLogWriter.WriteLine(Me.ToDelimetedLine(values))
		End Sub

		Public ReadOnly Property ErrorLogFileName() As String
			Get
				Return _errorLogFileName
			End Get
		End Property

#End Region

#Region " Status Communication "

		Private Sub _lineCounter_OnEvent(ByVal e As kCura.Utility.File.LineCounter.EventArgs) Handles _lineCounter.OnEvent
			Select Case e.Type
				Case kCura.Utility.File.LineCounter.EventType.Begin
					_genericTimestamp = System.DateTime.Now
					RaiseEvent DataSourcePrep(New DataSourcePrepEventArgs(DataSourcePrepEventArgs.EventType.Open, 0, 0, e.TotalBytes, _genericTimestamp, System.DateTime.Now))
				Case kCura.Utility.File.LineCounter.EventType.Progress
					RaiseEvent DataSourcePrep(New DataSourcePrepEventArgs(DataSourcePrepEventArgs.EventType.ReadEvent, e.BytesRead, e.TotalBytes, e.StepSize, _genericTimestamp, System.DateTime.Now))
				Case kCura.Utility.File.LineCounter.EventType.Complete
					_recordCount = e.NewlinesRead
					RaiseEvent DataSourcePrep(New DataSourcePrepEventArgs(DataSourcePrepEventArgs.EventType.ReadEvent, e.TotalBytes, e.TotalBytes, e.StepSize, _genericTimestamp, System.DateTime.Now))
					Dim path As String = _lineCounter.Path
					_columnHeaders = GetColumnNames(_settings)
					Reader = New System.IO.StreamReader(path, _sourceFileEncoding)
					If _firstLineContainsColumnNames Then
						_columnHeaders = GetLine
						_recordCount -= 1
					End If
					If Not _filePathColumn Is Nothing Then
						Dim openParenIndex As Int32 = _filePathColumn.LastIndexOf("("c) + 1
						Dim closeParenIndex As Int32 = _filePathColumn.LastIndexOf(")"c)
						_filePathColumnIndex = Int32.Parse(_filePathColumn.Substring(openParenIndex, closeParenIndex - openParenIndex)) - 1
					Else
						_filePathColumnIndex = -1
					End If
			End Select
		End Sub

		Protected Overrides Sub RaiseIoWarning(ByVal e As kCura.Utility.RobustIoReporter.IoWarningEventArgs)
			If e.Exception Is Nothing Then
				RaiseEvent OnIoWarning(New kCura.WinEDDS.Api.IoWarningEventArgs(e.Message, e.CurrentLineNumber))
			Else
				RaiseEvent OnIoWarning(New kCura.WinEDDS.Api.IoWarningEventArgs(e.WaitTime, e.Exception, e.CurrentLineNumber))
			End If
		End Sub

#End Region

#Region " IArtifactReader Implementation "

		Public Event OnIoWarning(ByVal e As kCura.WinEDDS.Api.IoWarningEventArgs) Implements Api.IArtifactReader.OnIoWarning
		Public Event StatusMessage(ByVal message As String) Implements Api.IArtifactReader.StatusMessage
		Public Event DataSourcePrep(ByVal e As Api.DataSourcePrepEventArgs) Implements Api.IArtifactReader.DataSourcePrep

		Public ReadOnly Property HasMoreRecords() As Boolean Implements Api.IArtifactReader.HasMoreRecords
			Get
				Me.EnsureReader()
				Return Not Me.HasReachedEOF
			End Get
		End Property

		Public Function GetColumnNames(ByVal args As Object) As String() Implements Api.IArtifactReader.GetColumnNames
			If _columnHeaders Is Nothing Then
				Dim path As String = DirectCast(args, kCura.WinEDDS.LoadFile).FilePath
				Me.EnsureReader()
				Dim columnNames As String() = GetLine
				Me.Reader.BaseStream.Seek(0, IO.SeekOrigin.Begin)
				Me.ResetLineCounter()
				Me.Reader.Close()
				Me.Reader = Nothing
				_columnHeaders = columnNames
			End If
			Return _columnHeaders
		End Function

		Public Sub AdvanceRecord() Implements Api.IArtifactReader.AdvanceRecord
			Me.AdvanceLine()
		End Sub

		Public Sub Close1() Implements Api.IArtifactReader.Close
			Me.Close()
			Try
				_errorLogWriter.Close()
			Catch ex As System.Exception
			End Try
		End Sub

		Public ReadOnly Property CurrentLineNumber1() As Integer Implements Api.IArtifactReader.CurrentLineNumber
			Get
				Return Me.CurrentLineNumber
			End Get
		End Property

		Public Sub Halt() Implements Api.IArtifactReader.Halt
			_lineCounter.StopCounting()
		End Sub

		Public ReadOnly Property SizeInBytes() As Long Implements Api.IArtifactReader.SizeInBytes
			Get
				Me.EnsureReader()
				Return Me.Reader.BaseStream.Length
			End Get
		End Property

		Public ReadOnly Property BytesProcessed() As Long Implements Api.IArtifactReader.BytesProcessed
			Get
				Return Me.Reader.BaseStream.Position
			End Get
		End Property

		Public Sub OnFatalErrorState() Implements Api.IArtifactReader.OnFatalErrorState
			Me.DoRetryLogic = False
		End Sub

		Public Function CountRecords() As Int64 Implements Api.IArtifactReader.CountRecords
			Me.InitializeLineCounter(_settings.FilePath)
			_lineCounter.CountLines(_sourceFileEncoding, New kCura.Utility.File.LineCounter.LineCounterArgs(Me.Bound, Me.Delimiter))
			Return _recordCount
		End Function

		Public Function ManageErrorRecords(ByVal errorMessageFileLocation As String, ByVal prePushErrorLineNumbersFileName As String) As String Implements IArtifactReader.ManageErrorRecords
			RaiseEvent StatusMessage("Generating error line file.")
			Dim allErrors As New kCura.Utility.GenericCsvReader(errorMessageFileLocation, System.Text.Encoding.Default, True)
			Dim clientErrors As System.IO.StreamReader
			'Me.Reader.BaseStream.Seek(0, IO.SeekOrigin.Begin)
			Me.Reader = New System.IO.StreamReader(_settings.FilePath, _sourceFileEncoding, True)
			Me.ResetLineCounter()
			If prePushErrorLineNumbersFileName = "" Then
				clientErrors = New System.IO.StreamReader(System.IO.Path.GetTempFileName, System.Text.Encoding.Default)
			Else
				clientErrors = New System.IO.StreamReader(prePushErrorLineNumbersFileName, System.Text.Encoding.Default)
			End If
			Dim advanceClient As Boolean = True
			Dim advanceAll As Boolean = True
			Dim allErrorsLine As Int32
			Dim clientErrorsLine As Int32
			Dim errorLinesFileLocation As String = System.IO.Path.GetTempFileName
			Dim sw As New System.IO.StreamWriter(errorLinesFileLocation, False, _sourceFileEncoding)
			If _settings.FirstLineContainsHeaders Then
				sw.WriteLine(Me.ToDelimetedLine(Me.GetLine))
			End If
			If prePushErrorLineNumbersFileName = "" Then
				clientErrorsLine = Int32.MaxValue
			Else
				clientErrorsLine = Int32.Parse(clientErrors.ReadLine)
			End If
			If Not allErrors.Eof Then
				Dim e As String() = allErrors.ReadLine
				If e(3) <> "client" Then
					allErrorsLine = Int32.Parse(e(0))
				Else
					While Not e Is Nothing AndAlso e(3) = "client"
						e = allErrors.ReadLine
					End While
					If e Is Nothing Then
						allErrorsLine = Int32.MaxValue
					Else
						allErrorsLine = Int32.Parse(e(0))
					End If
				End If
			Else
				allErrorsLine = Int32.MaxValue
			End If
			Dim line As String()
			Dim currentLine As String()
			Dim [continue] As Boolean = True And Not Me.Reader.Peek = -1
			While [continue]
				If Me.CurrentLineNumber < System.Math.Min(clientErrorsLine, allErrorsLine) Then
					If Me.Reader.Peek = -1 Then
						[continue] = False
					Else
						line = Me.GetLine()
					End If
				Else
					sw.WriteLine(Me.ToDelimetedLine(line))
					If Me.CurrentLineNumber = clientErrorsLine Then
						If clientErrors.Peek = -1 Then
							clientErrorsLine = Int32.MaxValue
						Else
							clientErrorsLine = Int32.Parse(clientErrors.ReadLine)
						End If
					End If
					If Me.CurrentLineNumber = allErrorsLine Then
						If allErrors.Eof Then
							allErrorsLine = Int32.MaxValue
						Else
							allErrorsLine = Int32.Parse(allErrors.ReadLine(0))
						End If
					End If
				End If
				[continue] = ((Not allErrors.Eof Or clientErrors.Peek <> -1 Or Me.CurrentLineNumber <= System.Math.Min(clientErrorsLine, allErrorsLine)) And [continue])
			End While
			sw.Close()
			allErrors.Close()
			clientErrors.Close()
			RaiseEvent StatusMessage("Error line file generation complete.")
			Return errorLinesFileLocation
		End Function

		Public Function ReadArtifact() As Api.ArtifactFieldCollection Implements Api.IArtifactReader.ReadArtifact
			Dim collection As New Api.ArtifactFieldCollection
			Dim line As String() = Me.GetLine
			If line.Length <> _columnHeaders.Length Then Throw New BulkLoadFileImporter.ColumnCountMismatchException(Me.CurrentLineNumber, _columnHeaders.Length, line.Length)
			For Each mapItem As kCura.WinEDDS.LoadFileFieldMap.LoadFileFieldMapItem In _settings.FieldMap
				If Not mapItem.DocumentField Is Nothing Then
					Dim field As New Api.ArtifactField(mapItem.DocumentField)
					If mapItem.NativeFileColumnIndex < 0 OrElse mapItem.NativeFileColumnIndex > line.Length - 1 Then
						Me.SetFieldValue(field, String.Empty, mapItem.NativeFileColumnIndex)
					Else
						Me.SetFieldValue(field, line(mapItem.NativeFileColumnIndex), mapItem.NativeFileColumnIndex)
					End If
					collection.Add(field)
				End If
			Next
			If _settings.LoadNativeFiles AndAlso Not _settings.NativeFilePathColumn Is Nothing AndAlso Not _settings.NativeFilePathColumn = String.Empty AndAlso collection.FileField Is Nothing Then
				Dim nativeFileIndex As Int32 = Int32.Parse(_settings.NativeFilePathColumn.Substring(_settings.NativeFilePathColumn.LastIndexOf("(")).Trim("()".ToCharArray))
				Dim field As New Api.ArtifactField(New DocumentField("File", -1, kCura.DynamicFields.Types.FieldTypeHelper.FieldType.File, kCura.DynamicFields.Types.FieldCategory.FileInfo, NullableInt32.Null, NullableInt32.Null, NullableInt32.Null, True))
				field.Value = line(nativeFileIndex - 1)
				collection.Add(field)
			End If
			If _settings.CreateFolderStructure AndAlso Not _settings.FolderStructureContainedInColumn Is Nothing AndAlso Not _settings.FolderStructureContainedInColumn = String.Empty Then
				Dim parentIndex As Int32 = Int32.Parse(_settings.FolderStructureContainedInColumn.Substring(_settings.FolderStructureContainedInColumn.LastIndexOf("(")).Trim("()".ToCharArray))
				Dim field As New Api.ArtifactField(New DocumentField("Parent", -2, kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Object, kCura.DynamicFields.Types.FieldCategory.ParentArtifact, NullableInt32.Null, NullableInt32.Null, NullableInt32.Null, True))
				field.Value = line(parentIndex - 1)
				collection.Add(field)
			End If
			Return collection
		End Function

#End Region

#Region " Unimplemented Virtual Methods "

		Public Overrides Function ReadFile(ByVal path As String) As Object
			Throw New System.Exception("Not Implemented")
		End Function

#End Region

	End Class

End Namespace