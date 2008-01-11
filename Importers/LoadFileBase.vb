Imports NullableTypes
Namespace kCura.WinEDDS
	Public MustInherit Class LoadFileBase
		Inherits kCura.Utility.DelimitedFileImporter

		Protected _columnsAreInitialized As Boolean
		Protected _columnHeaders As String()

		Protected _documentManager As kCura.WinEDDS.Service.DocumentManager
		Protected _uploadManager As kCura.WinEDDS.Service.FileIO
		Protected _codeManager As kCura.WinEDDS.Service.CodeManager
		Protected _folderManager As kCura.WinEDDS.Service.FolderManager
		Protected _fieldQuery As kCura.WinEDDS.Service.FieldQuery
		'Protected _multiCodeManager As kCura.WinEDDS.Service.MultiCodeManager
		Protected _fileManager As kCura.WinEDDS.Service.FileManager
		Protected _usermanager As kCura.WinEDDS.Service.UserManager
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

		Protected MustOverride ReadOnly Property UseTimeZoneOffset() As Boolean

		Public Property AllCodes() As kCura.Data.DataView
			Get
				InitializeCodeTables()
				Return _allCodes
			End Get
			Set(ByVal value As kCura.Data.DataView)
				_allCodes = value
			End Set
		End Property

		Public Property AllCodeTypes() As kCura.Data.DataView
			Get
				InitializeCodeTables()
				Return _allCodeTypes
			End Get
			Set(ByVal value As kCura.Data.DataView)
				_allCodeTypes = value
			End Set
		End Property

		Public ReadOnly Property Users() As UserCollection
			Get
				If _users Is Nothing Then
					_users = New UserCollection(_usermanager, _caseArtifactID)
				End If
				Return _users
			End Get
		End Property

		Public Sub New(ByVal args As LoadFile, ByVal timezoneoffset As Int32, Optional ByVal autoDetect As Boolean = True)
			MyBase.New(args.RecordDelimiter, args.QuoteDelimiter, args.NewlineDelimiter)

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
			'_multiCodeManager = New kCura.WinEDDS.Service.MultiCodeManager(args.Credentials, args.CookieContainer)

			_multiValueSeparator = args.MultiRecordDelimiter.ToString.ToCharArray
			_folderID = args.DestinationFolderID
			_caseSystemID = args.CaseInfo.RootArtifactID
			_caseArtifactID = args.CaseInfo.ArtifactID
			_timeZoneOffset = timezoneoffset
			_autoDetect = autoDetect
			_uploadFiles = args.LoadNativeFiles
			_createFolderStructure = args.CreateFolderStructure
			_destinationFolder = args.FolderStructureContainedInColumn
			_extractMd5Hash = args.ExtractMD5HashFromNativeFile
			_fullTextColumnMapsToFileLocation = args.FullTextColumnContainsFileLocation
			_sourceFileEncoding = args.SourceFileEncoding
		End Sub

#Region "Code Parsing"

		Private Sub InitializeCodeTables()
			If _autoDetect Then
				If _allCodes Is Nothing Then
					Dim ds As DataSet = _codeManager.RetrieveCodesAndTypesForCase(_caseArtifactID)
					_allCodes = New kCura.Data.DataView(ds.Tables("AllCodes"))					'HACK: Hard-coded
					_allCodeTypes = New kCura.Data.DataView(ds.Tables("AllCodeTypes"))					'HACK: Hard-coded
				End If
			End If
		End Sub

		Public Function GetCode(ByVal value As String, ByVal column As Int32, ByVal field As DocumentField, ByVal forPreview As Boolean) As NullableTypes.NullableInt32
			InitializeCodeTables()
			Dim codeArtifactID As Int32
			Dim newCodeOrderValue As Int32
			If value = String.Empty Then
				Return NullableTypes.NullableInt32.Null
			End If
			Dim codeTableIndex As Int32 = FindCodeByDisplayValue(value, field.CodeTypeID.Value)
			If field.CodeTypeID.IsNull Then
				Throw New MissingCodeTypeException(Me.CurrentLineNumber, column)
			End If
			If codeTableIndex > -1 Then
				Return GetNullableInteger(_allCodes(codeTableIndex)("ArtifactID").ToString, column)				'HACK: Hard-coded
			Else
				If forPreview Then
					Return New NullableTypes.NullableInt32(-1)
				Else
					If _autoDetect Then
						newCodeOrderValue = GetNewCodeOrderValue(field.CodeTypeID.Value)
						Dim code As kCura.EDDS.WebAPI.CodeManagerBase.Code = _codeManager.CreateNewCodeDTOProxy(field.CodeTypeID.Value, value, newCodeOrderValue, _caseSystemID)
						codeArtifactID = _codeManager.Create(_caseArtifactID, code)
						If codeArtifactID = -200 Then
							Throw New CodeCreationException(Me.CurrentLineNumber, column, value)
						End If
						Dim newRow As DataRowView = _allCodes.AddNew
						_allCodes = Nothing
						Return New NullableInt32(codeArtifactID)
					End If
				End If
			End If
		End Function

		Private Function FindCodeByDisplayValue(ByVal value As String, ByVal codeTypeID As Int32) As Int32
			Dim i As Int32
			For i = 0 To _allCodes.Count - 1
				If _allCodes(i)("Name").ToString.ToLower = value.ToLower AndAlso (DirectCast(_allCodes(i)("CodeTypeID"), Int32)) = codeTypeID Then
					Return i
				End If
			Next
			Return -1
		End Function

		Private Function GetNewCodeOrderValue(ByVal codeTypeID As Int32) As Int32
			Return 0
			Dim row As System.Data.DataRowView
			Dim newOrder As Int32 = 0
			Dim oldOrder As Int32
			If _allCodes.Count > 0 Then
				For Each row In _allCodes
					If CType(row("CodeTypeID"), Int32) = codeTypeID Then
						oldOrder = CType(row("Order"), Int32)
						If oldOrder > newOrder Then
							newOrder = oldOrder
						End If
					End If
				Next
				newOrder += 1
			Else
				newOrder = 1
			End If
			Return newOrder
		End Function

		Public Function GetMultiCode(ByVal value As String, ByVal column As Int32, ByVal field As DocumentField, ByVal forPreview As Boolean) As NullableTypes.NullableInt32()
			Dim codeDisplayNames As String() = value.Split(_multiValueSeparator)
			Dim codes(codeDisplayNames.Length - 1) As NullableInt32
			Dim i As Int32
			For i = 0 To codeDisplayNames.Length - 1
				codes(i) = GetCode(codeDisplayNames(i).Trim, column, field, forPreview)
			Next
			Return codes
		End Function

#End Region

		Public Sub SetFieldValue(ByVal field As DocumentField, ByVal values As String(), ByVal column As Int32)
			Dim value As String
			If column = -1 Then
				value = String.Empty
			Else
				Try
					value = values(column)
				Catch ex As System.Exception
					value = ""
				End Try
			End If
			If field.FieldCategoryID = kCura.DynamicFields.Types.FieldCategory.FullText Then
				value = value.Replace(NewlineProxy, Microsoft.VisualBasic.ControlChars.NewLine)
			End If
			SetFieldValue(field, value, column)
		End Sub

		Public Sub SetFieldValue(ByVal field As DocumentField, ByVal value As String, ByVal column As Int32)
			SetFieldValue(field, value, column, False)
		End Sub

		Public Sub SetFieldValue(ByVal field As DocumentField, ByVal value As String, ByVal column As Int32, ByVal forPreview As Boolean)
			Select Case CType(field.FieldTypeID, kCura.DynamicFields.Types.FieldTypeHelper.FieldType)
				Case kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Boolean
					field.Value = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(GetNullableBoolean(value.Trim, column))
				Case kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Integer
					field.Value = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(GetNullableInteger(value.Trim, column))
				Case kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Currency, kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Decimal
					field.Value = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(GetNullableDecimal(value.Trim, column))
				Case kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Date
					field.Value = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(GetNullableDateTime(value.Trim, column))
				Case kCura.DynamicFields.Types.FieldTypeHelper.FieldType.User
					field.Value = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(Me.GetUserArtifactID(value.Trim, column))
					If forPreview Then field.Value = value.Trim
				Case kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Code
					Dim fieldValue As String = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(GetCode(value.Trim, column, field, forPreview))
					If forPreview And fieldValue = "-1" Then
						fieldValue = "[new code]"
					End If
					field.Value = fieldValue
				Case kCura.DynamicFields.Types.FieldTypeHelper.FieldType.MultiCode
					If value = String.Empty Then
						field.Value = String.Empty
					Else
						Dim codeValues As NullableTypes.NullableInt32() = GetMultiCode(value.Trim, column, field, forPreview)
						Dim i As Int32
						Dim newVal As String = String.Empty
						Dim codeName As String
						If codeValues.Length > 0 Then
							newVal &= codeValues(0).ToString
							If forPreview And newVal = "-1" Then
								newVal = "[new code]"
							End If
							If codeValues.Length > 1 Then
								For i = 1 To codeValues.Length - 1
									codeName = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(codeValues(i))
									If forPreview And codeName = "-1" Then
										codeName = "[new code]"
									End If
									newVal &= ";" & codeName
								Next
							End If
						End If
						field.Value = newVal
					End If
				Case kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Varchar
					Select Case field.FieldCategory
						Case DynamicFields.Types.FieldCategory.GroupIdentifier
							field.Value = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(Me.GetGroupIdentifierField(value, column, field.FieldLength.Value))
						Case Else
							field.Value = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(GetNullableFixedString(value, column, field.FieldLength.Value))
					End Select
				Case Else				'FieldTypeHelper.FieldType.Text
					If field.FieldCategory = DynamicFields.Types.FieldCategory.FullText AndAlso _fullTextColumnMapsToFileLocation Then
						If value = "" Then
							field.Value = ""
						ElseIf Not System.IO.File.Exists(value) Then
							Throw New MissingFullTextFileException(Me.CurrentLineNumber, column)
						Else
							If forPreview Then
								Dim sr As New System.IO.StreamReader(value, _sourceFileEncoding, True)
								Dim i As Int32 = 0
								Dim sb As New System.Text.StringBuilder
								While sr.Peek <> -1 AndAlso i < 100
									sb.Append(ChrW(sr.Read))
									i += 1
								End While
								If i = 100 Then sb.Append("...")
								sr.Close()
								sb = sb.Replace(System.Environment.NewLine, Me.NewlineProxy).Replace(ChrW(10), Me.NewlineProxy).Replace(ChrW(13), Me.NewlineProxy)
								field.Value = sb.ToString
							Else
								field.Value = value
							End If
						End If
					Else
						If value.Length > 100 AndAlso forPreview Then
							field.Value = value.Substring(0, 100) & "...."
						Else
							field.Value = value
						End If
					End If
			End Select
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
				End Select
			End Try
			If nullableDateValue.IsNull Then Return nullableDateValue
			Dim datevalue As DateTime
			datevalue = nullableDateValue.Value
			Dim timeZoneOffset As Int32 = 0
			If Me.UseTimeZoneOffset Then
				timeZoneOffset = _timeZoneOffset
			End If
			If datevalue.TimeOfDay.Ticks = 0 Then
				datevalue = datevalue.AddHours(12 - timeZoneOffset)
			Else
				datevalue = datevalue.AddHours(0 - timeZoneOffset)
			End If
			If datevalue < DateTime.Parse("1/1/1753") Then
				Throw New kCura.Utility.DelimitedFileImporter.DateException(Me.CurrentLineNumber, column)
			End If
			Return New NullableDateTime(datevalue)
		End Function

		Public Function GetGroupIdentifierField(ByVal value As String, ByVal column As Int32, ByVal fieldLength As Int32) As NullableString
			Dim nv As NullableString = Me.GetNullableFixedString(value, column, fieldLength)
			'If nv.IsNull Then
			'	Throw New NullGroupIdentifierException(Me.CurrentLineNumber, column)
			'Else
			Return nv
			'End If
		End Function

		Public Function GetUserArtifactID(ByVal value As String, ByVal column As Int32) As NullableInt32
			If value = "" Then Return NullableTypes.NullableInt32.Null
			Dim retval As NullableInt32 = Me.Users(value)
			If retval.IsNull Then
				Throw New MissingUserException(Me.CurrentLineNumber, column, value)
			Else
				Return retval
			End If
		End Function

#Region "Exceptions"

		Public Class IdentifierOverlapException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal identityValue As String, ByVal previousLineNumber As String)
				MyBase.New(String.Format("Document '({0})' has been previously processed in this file on line {1}.", identityValue, previousLineNumber))
			End Sub
		End Class

		Public Class MissingCodeTypeException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal row As Int32, ByVal column As Int32)
				MyBase.New(row, column, String.Format("Document field is marked as a code type, but it's missing a CodeType."))
			End Sub
		End Class

		Public Class NullGroupIdentifierException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal row As Int32, ByVal column As Int32)
				MyBase.New(row, column, String.Format("Group Identifier fields cannot accept null or empty values."))
			End Sub
		End Class

		Public Class MissingFullTextFileException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal row As Int32, ByVal column As Int32)
				MyBase.New(row, column, String.Format("Full text file specified does not exist."))
			End Sub
		End Class

		Public Class MissingUserException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal row As Int32, ByVal column As Int32, ByVal invalidEmailaddress As String)
				MyBase.New(row, column, String.Format("User '{0}' does not exist in the system or is not available for assignment.", invalidEmailaddress))
			End Sub
		End Class

		Public Class CodeCreationException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal row As Int32, ByVal column As Int32, ByVal newCodeValue As String)
				MyBase.New(row, column, String.Format("The maximum number of choices available for this field has been reached.  There is no room to add '{0}' to the list.  Upload halted.", newCodeValue))
			End Sub
		End Class

		Public Class ColumnCountMismatchException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal row As Int32, ByVal expecting As Int32, ByVal actual As Int32)
				MyBase.New(row, -1, String.Format("There are an invalid number of cells in this row - expecting:{0}, actual:{1}.", expecting, actual))
			End Sub
		End Class
#End Region

	End Class
End Namespace