Imports NullableTypes
Namespace kCura.WinEDDS
	Public MustInherit Class LoadFileBase
		Inherits kCura.Utility.DelimitedFileImporter

#Region "Members"

		Protected _columnsAreInitialized As Boolean
		Protected _columnHeaders As String()

		Protected _documentManager As kCura.WinEDDS.Service.DocumentManager
		Protected _uploadManager As kCura.WinEDDS.Service.FileIO
		Protected _codeManager As kCura.WinEDDS.Service.CodeManager
		Protected _folderManager As kCura.WinEDDS.Service.FolderManager
		Protected _fieldQuery As kCura.WinEDDS.Service.FieldQuery
		Protected _bulkImportManager As kCura.WinEDDS.Service.BulkImportManager
		'Protected _multiCodeManager As kCura.WinEDDS.Service.MultiCodeManager
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
		Protected MustOverride ReadOnly Property UseTimeZoneOffset() As Boolean
		Protected _previewCodeCount As New System.Collections.Specialized.HybridDictionary
		Protected _startLineNumber As Int64
		Protected _keyFieldID As Int32
		Protected _settings As kCura.WinEDDS.LoadFile
		Private _codeValidator As CodeValidator.Base
		Private _codesCreated As Int32 = 0

#End Region

#Region "Accessors"

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

		Public ReadOnly Property SingleCodesCreated() As Int32
			Get
				Return _codeValidator.CreatedCodeCount
			End Get
		End Property

		Public ReadOnly Property Users() As UserCollection
			Get
				If _users Is Nothing Then
					_users = New UserCollection(_usermanager, _caseArtifactID)
				End If
				Return _users
			End Get
		End Property

#End Region

#Region " Virtual Members "

		Protected MustOverride Function GetSingleCodeValidator() As CodeValidator.Base


#End Region

		Public Sub New(ByVal args As LoadFile, ByVal timezoneoffset As Int32, ByVal doRetryLogic As Boolean, ByVal autoDetect As Boolean)
			MyBase.New(args.RecordDelimiter, args.QuoteDelimiter, args.NewlineDelimiter, doRetryLogic)
			_settings = args
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
			'_multiCodeManager = New kCura.WinEDDS.Service.MultiCodeManager(args.Credentials, args.CookieContainer)
			_keyFieldID = args.IdentityFieldId
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
			_extractedTextFileEncoding = args.ExtractedTextFileEncoding
			_extractedTextFileEncodingName = args.ExtractedTextFileEncodingName
			_artifactTypeID = args.ArtifactTypeID
			_hierarchicalMultiValueFieldDelmiter = args.HierarchicalValueDelimiter
			_previewCodeCount = args.PreviewCodeCount
			_startLineNumber = args.StartLineNumber
			_codeValidator = Me.GetSingleCodeValidator
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
		End Sub

#Region "Code Parsing"

		Private Sub InitializeCodeTables()
			If _autoDetect Then
				If _allCodes Is Nothing Then
					Dim ds As DataSet = _codeManager.RetrieveCodesAndTypesForCase(_caseArtifactID)
					_allCodes = New kCura.Data.DataView(ds.Tables("AllCodes"))
					_allCodeTypes = New kCura.Data.DataView(ds.Tables("AllCodeTypes"))
				End If
			End If
		End Sub

		Public Function GetCode(ByVal value As String, ByVal column As Int32, ByVal field As DocumentField, ByVal forPreview As Boolean) As NullableTypes.NullableInt32
			Try
				Return _codeValidator.ValidateSingleCode(field, value)
			Catch ex As CodeValidator.CodeCreationException
				Throw New CodeCreationException(Me.CurrentLineNumber, column, ex.IsFatal, ex.Message)
			End Try
		End Function
		'Public Function GetCode(ByVal value As String, ByVal column As Int32, ByVal field As DocumentField, ByVal forPreview As Boolean) As NullableTypes.NullableInt32
		'	InitializeCodeTables()
		'	Dim codeArtifactID As Int32
		'	Dim newCodeOrderValue As Int32
		'	If value = String.Empty Then
		'		Return NullableTypes.NullableInt32.Null
		'	End If
		'	Dim codeTableIndex As Int32 = FindCodeByDisplayValue(value, field.CodeTypeID.Value)
		'	If field.CodeTypeID.IsNull Then
		'		Throw New MissingCodeTypeException(Me.CurrentLineNumber, column)
		'	End If
		'	If codeTableIndex > -1 Then
		'		Return GetNullableInteger(_allCodes(codeTableIndex)("ArtifactID").ToString, column)
		'	Else
		'		If forPreview Then
		'			Return New NullableTypes.NullableInt32(-1)
		'		Else
		'			If _autoDetect Then
		'				newCodeOrderValue = GetNewCodeOrderValue(field.CodeTypeID.Value)
		'				Dim code As kCura.EDDS.WebAPI.CodeManagerBase.Code = _codeManager.CreateNewCodeDTOProxy(field.CodeTypeID.Value, value, newCodeOrderValue, _caseSystemID)
		'				If code.Name.Length > 200 Then Throw New CodeCreationException(Me.CurrentLineNumber, column, False, "Proposed choice name '" & code.Name & "' exceeds 200 characters, which is the maximum allowable.")
		'				Dim o As Object = _codeManager.Create(_caseArtifactID, code)
		'				If TypeOf o Is Int32 Then
		'					codeArtifactID = CType(o, Int32)
		'				Else
		'					Throw New CodeCreationException(Me.CurrentLineNumber, column, True, o.ToString)
		'				End If
		'				Select Case codeArtifactID
		'					Case -1
		'						Throw New CodeCreationException(Me.CurrentLineNumber, column, True, value)
		'					Case -200
		'						Throw New System.Exception("This choice or multi-choice field is not enabled as unicode.  Upload halted")
		'				End Select
		'				Dim newRow As DataRowView = _allCodes.AddNew
		'				_allCodes = Nothing
		'				_codesCreated += 1
		'				Return New NullableInt32(codeArtifactID)
		'			End If
		'		End If
		'	End If
		'End Function

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

		Public Overridable Function GetMultiCode(ByVal value As String, ByVal column As Int32, ByVal field As DocumentField, ByVal forPreview As Boolean) As NullableTypes.NullableInt32()
			Try
				Dim al As New System.Collections.ArrayList(value.Split(_multiValueSeparator))
				Dim goodCodes As New System.Collections.ArrayList
				For Each codeString As String In al
					codeString = codeString.Trim
					If codeString <> "" Then
						If goodCodes.Contains(codeString) Then Throw New DuplicateMulticodeValueException(Me.CurrentLineNumber, column, codeString)
						If codeString.Length > 200 Then Throw New CodeCreationException(Me.CurrentLineNumber, column, False, "Proposed choice name '" & codeString & "' exceeds 200 characters, which is the maximum allowable.")
						goodCodes.Add(codeString)
					End If
				Next
				Dim codeDisplayNames As String() = DirectCast(goodCodes.ToArray(GetType(String)), String())
				Dim i As Int32
				Dim hierarchicCodeManager As Service.IHierarchicArtifactManager
				If forPreview Then
					hierarchicCodeManager = New Service.FieldSpecificCodePreviewer(_codeManager, field.CodeTypeID.Value)
				Else
					hierarchicCodeManager = New Service.FieldSpecificCodeManager(_codeManager, field.CodeTypeID.Value)
				End If
				If Not Me.MulticodeMatrix.Contains(field.CodeTypeID.Value) Then
					Me.MulticodeMatrix.Add(field.CodeTypeID.Value, New NestedArtifactCache(hierarchicCodeManager, _caseSystemID, _caseArtifactID, _hierarchicalMultiValueFieldDelmiter))
				End If
				Dim artifactCache As NestedArtifactCache = DirectCast(Me.MulticodeMatrix(field.CodeTypeID.Value), NestedArtifactCache)
				Dim c As New System.Collections.ArrayList
				For Each codeString As String In codeDisplayNames
					For Each id As Object() In artifactCache.SelectedIds(_hierarchicalMultiValueFieldDelmiter & codeString.Trim(_hierarchicalMultiValueFieldDelmiter.ToCharArray))
						If CType(id(0), Int32) = -200 Then Throw New System.Exception("This choice or multi-choice field is not enabled as unicode.  Upload halted")
						If forPreview Then
							c.Add(CType(id(0), Int32))
							AddToCodeCountPreviewHashTable(field.FieldID, field.FieldName, CType(id(1), String))
						Else
							If Not c.Contains(CType(id(0), Int32)) Then c.Add(CType(id(0), Int32))
						End If
					Next
				Next
				If c.Count > 0 Then
					Dim codes(c.Count - 1) As NullableInt32
					For i = 0 To codes.Length - 1
						codes(i) = New NullableTypes.NullableInt32(CType(c(i), Int32))
					Next
					Return codes
				Else
					Return New NullableTypes.NullableInt32() {}
				End If
			Catch ex As Exceptions.CodeCreationFailedException
				Throw New CodeCreationException(Me.CurrentLineNumber, column, True, ex.ToString)
			End Try

			'For i = 0 To codeDisplayNames.Length - 1
			'	codes(i) = GetCode(codeDisplayNames(i).Trim, column, field, forPreview)
			'Next
			'Return codes
		End Function

		'returning identifier/artifactID pairs for objects. -1 for new artifactIDs
		Public Overridable Function GetObjects(ByVal value As String, ByVal column As Int32, ByVal field As DocumentField, ByVal associatedObjectTypeID As Int32) As System.Collections.Hashtable
			Dim al As New System.Collections.ArrayList(value.Split(_multiValueSeparator))
			Dim goodObjects As New System.Collections.ArrayList
			For Each objectString As String In al
				objectString = objectString.Trim
				If objectString <> String.Empty Then
					'################################################ EXCEPTION TYPE #####################################
					If goodObjects.Contains(objectString) Then Throw New DuplicateObjectReferenceException(Me.CurrentLineNumber, column, objectString)
					'Not checking the length of the object because object names have no limit.
					goodObjects.Add(objectString)
				End If
			Next

			Dim objectDisplayNames As String() = DirectCast(goodObjects.ToArray(GetType(String)), String())
			Dim nameIDPairs As New System.Collections.Hashtable
			For Each objectName As String In objectDisplayNames
				Dim artifactID As System.Data.DataSet = _objectManager.RetrieveArtifactIdOfMappedObject(_caseArtifactID, objectName, associatedObjectTypeID)
				If artifactID.Tables(0).Rows.Count > 0 Then
					nameIDPairs(objectName) = CType(artifactID.Tables(0).Rows(0)(0), Int32)
					' .... the heck
				Else
					nameIDPairs(objectName) = -1
				End If
			Next
			Return nameIDPairs
		End Function

#End Region

		Public Sub SetFieldValue(ByVal field As DocumentField, ByVal values As String(), ByVal column As Int32, ByVal identityValue As String)
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
			If (field.FieldCategoryID = kCura.DynamicFields.Types.FieldCategory.FullText AndAlso Not _fullTextColumnMapsToFileLocation) OrElse (field.FieldTypeID = kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Text AndAlso Not field.FieldCategoryID = kCura.DynamicFields.Types.FieldCategory.FullText) Then
				value = value.Replace(NewlineProxy, Microsoft.VisualBasic.ControlChars.NewLine)
			End If
			SetFieldValue(field, value, column, identityValue)
		End Sub

		Public Sub SetFieldValue(ByVal field As DocumentField, ByVal value As String, ByVal column As Int32, ByVal identityValue As String)
			SetFieldValue(field, value, column, False, identityValue)
		End Sub

		Public Sub SetFieldValue(ByVal field As DocumentField, ByVal value As String, ByVal column As Int32, ByVal forPreview As Boolean, ByVal identityValue As String)
			Select Case CType(field.FieldTypeID, kCura.DynamicFields.Types.FieldTypeHelper.FieldType)
				Case kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Boolean
					field.Value = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(GetNullableBoolean(value.Trim, column))
				Case kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Integer
					field.Value = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(GetNullableInteger(value.Trim, column))
				Case kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Currency, kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Decimal
					field.Value = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(GetNullableDecimal(value.Trim, column))
				Case kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Date
					field.Value = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(GetNullableDateTime(value.Trim, column), True)
				Case kCura.DynamicFields.Types.FieldTypeHelper.FieldType.User
					field.Value = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(Me.GetUserArtifactID(value.Trim, column))
					If forPreview Then field.Value = value.Trim
				Case kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Code
					Dim fieldValue As String
					Dim code As NullableInt32 = GetCode(value.Trim, column, field, forPreview)
					fieldValue = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(code)
					If forPreview Then
						If fieldValue = "-1" Then
							fieldValue = "[new code]"
						End If
						If fieldValue <> "" Then
							AddToCodeCountPreviewHashTable(field.FieldID, field.FieldName, value.Trim)
						End If
					End If
					field.Value = fieldValue
					If TypeOf Me Is BulkLoadFileImporter Then
						fieldValue = ChrW(11) & value.Trim & ChrW(11)
						field.Value = fieldValue
						If Not value.Trim = "" Then
							DirectCast(Me, BulkLoadFileImporter).WriteCodeLineToTempFile(identityValue, Int32.Parse(kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(code)), field.CodeTypeID.Value)
							Dim sb As New System.Text.StringBuilder
							field.Value = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(code)
						Else
							field.Value = ""
						End If
					End If
				Case kCura.DynamicFields.Types.FieldTypeHelper.FieldType.MultiCode
					If value = String.Empty Then
						field.Value = String.Empty
						If TypeOf Me Is BulkLoadFileImporter Then
							field.Value = ""
						End If
					Else
						Dim oldval As String = value.Trim
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
						If TypeOf Me Is BulkLoadFileImporter Then
							If codeValues.Length = 0 Then
								field.Value = ""
							Else
								field.Value = ChrW(11) & oldval.Trim(_multiValueSeparator).Replace(_multiValueSeparator, ChrW(11)) & ChrW(11)
								For Each codeValue As NullableTypes.NullableInt32 In codeValues
									If Not codeValue.IsNull Then
										DirectCast(Me, BulkLoadFileImporter).WriteCodeLineToTempFile(identityValue, codeValue.Value, field.CodeTypeID.Value)
									End If
								Next
								Dim sb As New System.Text.StringBuilder
								For Each codeValue As NullableTypes.NullableInt32 In codeValues
									If Not codeValue.IsNull Then
										sb.Append(codeValue.Value)
										sb.Append(",")
									End If
								Next
								field.Value = sb.ToString.TrimEnd(","c)
							End If
						End If
					End If
				Case kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Varchar
					Select Case field.FieldCategory
						Case DynamicFields.Types.FieldCategory.Relational
							If field.FieldName.ToLower = "group identifier" Then
								field.Value = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(Me.GetGroupIdentifierField(value, column, field.FieldLength.Value))
							Else
								field.Value = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(GetNullableFixedString(value, column, field.FieldLength.Value))
							End If
							If field.Value = String.Empty Then field.Value = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(GetNullableFixedString(identityValue, column, field.FieldLength.Value))
						Case Else
							field.Value = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(GetNullableFixedString(value, column, field.FieldLength.Value))
					End Select
				Case kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Object
					field.Value = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(GetNullableAssociatedObjectName(value, column, 255, field.FieldName))
					If forPreview Then field.Value = value.Trim
				Case DynamicFields.Types.FieldTypeHelper.FieldType.Objects
					If value = String.Empty Then
						field.Value = String.Empty
					Else
						Dim oldval As String = value.Trim

						Dim fieldManager As New kCura.EDDS.WebAPI.FieldManagerBase.FieldManager
						fieldManager.Credentials = _fileManager.Credentials
						fieldManager.CookieContainer = _fileManager.CookieContainer
						Dim fieldDTO As kCura.EDDS.WebAPI.FieldManagerBase.Field = fieldManager.Read(_caseArtifactID, field.FieldID)
						field.AssociatedObjectTypeID = fieldDTO.AssociativeArtifactTypeID
						'TODO: Move this to wherever "field" is created.

						Dim objectValues As System.Collections.Hashtable = GetObjects(value.Trim, column, field, field.AssociatedObjectTypeID.Value)
						Dim newVal As String = String.Empty
						Dim objName As String
						If objectValues.Count > 0 Then
							For Each objName In objectValues.Keys
								If forPreview And DirectCast(objectValues(objName), Int32) = -1 Then
									objectValues(objName) = "[new object]"
								End If
								newVal &= ";" & objName
							Next
						End If
						newval = newval.TrimStart(";"c)
						field.Value = newVal
						If TypeOf Me Is BulkLoadFileImporter Then
							If objectValues.Count = 0 Then
								field.Value = ""
							Else
								field.Value = ChrW(11) & oldval.Trim(_multiValueSeparator).Replace(_multiValueSeparator, ChrW(11)) & ChrW(11)
								Dim sb As New System.Text.StringBuilder
								For Each objectValue As String In objectValues.Keys
									DirectCast(Me, BulkLoadFileImporter).WriteObjectLineToTempFile(identityValue, objectValue, CType(objectValues(objectValue), Int32), field.AssociatedObjectTypeID.Value, field.FieldID)
									sb.Append("'" & objectValue & "'")
									sb.Append(",")
								Next
								field.Value = sb.ToString.TrimEnd(","c)
							End If
						End If
					End If
				Case Else				'FieldTypeHelper.FieldType.Text
						If field.FieldCategory = DynamicFields.Types.FieldCategory.FullText AndAlso _fullTextColumnMapsToFileLocation Then
							If value = "" Then
								field.Value = ""
							ElseIf Not System.IO.File.Exists(value) Then
								Throw New MissingFullTextFileException(Me.CurrentLineNumber, column)
							Else
								If forPreview Then
									Dim sr As New System.IO.StreamReader(value, _extractedTextFileEncoding)
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

		Public Sub AddToCodeCountPreviewHashTable(ByVal fieldID As Int32, ByVal fieldName As String, ByVal fieldValue As String)
			Dim fieldKeyID As String = String.Format("{0}_{1}", fieldID, fieldName)
			If _previewCodeCount.Contains(fieldKeyID) Then
				Dim codesForField As System.Collections.Specialized.HybridDictionary = DirectCast(_previewCodeCount(fieldKeyID), System.Collections.Specialized.HybridDictionary)
				If Not codesForField.Contains(fieldValue) Then
					codesForField.Add(fieldValue, "")
					_previewCodeCount(fieldKeyID) = codesForField
				End If
			Else
				Dim newHashTable As New System.Collections.Specialized.HybridDictionary
				newHashTable.Add(fieldValue.Trim, "")
				_previewCodeCount.Add(fieldKeyID, newHashTable)
			End If
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
				'If Me.UseTimeZoneOffset Then
				'	timeZoneOffset = _timeZoneOffset
				'End If
				'If datevalue.TimeOfDay.Ticks = 0 Then datevalue = datevalue.AddHours(12)
				'If datevalue.TimeOfDay.Ticks = 0 Then
				'	datevalue = datevalue.AddHours(12 - timeZoneOffset)
				'Else
				'	datevalue = datevalue.AddHours(0 - timeZoneOffset)
				'End If
				If datevalue < DateTime.Parse("1/1/1753") Then
					Throw New kCura.Utility.DelimitedFileImporter.DateException(Me.CurrentLineNumber, column)
				End If
				Return New NullableDateTime(datevalue)
			Catch ex As Exception
				Throw New kCura.Utility.DelimitedFileImporter.DateException(Me.CurrentLineNumber, column)
			End Try
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
				MyBase.New(row, column, String.Format("Error: full text file specified does not exist."))
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
			Private _isFatal As Boolean
			Public ReadOnly Property IsFatal() As Boolean
				Get
					Return _isFatal
				End Get
			End Property
			Public Sub New(ByVal row As Int32, ByVal column As Int32, ByVal isFatal As Boolean, ByVal errorText As String)
				MyBase.New(row, column, errorText)
				_isFatal = isFatal
			End Sub
		End Class

		Public Class ColumnCountMismatchException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal row As Int32, ByVal expecting As Int32, ByVal actual As Int32)
				MyBase.New(row, -1, String.Format("There are an invalid number of cells in this row - expecting:{0}, actual:{1}.", expecting, actual))
			End Sub
		End Class

		Public Class DuplicateObjectReferenceException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal row As Int32, ByVal column As Int32, ByVal fieldName As String)
				MyBase.New(row, column, String.Format("Object identifier for field {0} references an identifier that is not unique.", fieldName))
			End Sub
		End Class

		Public Class NonExistentParentException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal row As Int32, ByVal column As Int32, ByVal fieldName As String)
				MyBase.New(row, column, String.Format("Object references a parent object that does not exist.", fieldName))
			End Sub
		End Class

		Public Class ParentObjectReferenceRequiredException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal row As Int32, ByVal column As Int32)
				MyBase.New(row, column, String.Format("Null parent object identifier found, this is required for the Parent Info field."))
			End Sub
		End Class

		Public Class BcpPathAccessException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal details As String)
				MyBase.New("Error accessing the bcp share. Please contact your system administrator with the following details: " & System.Environment.NewLine & details)
			End Sub
		End Class

		Public Class DuplicateMulticodeValueException
			Inherits kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
			Public Sub New(ByVal row As Int32, ByVal column As Int32, ByVal codeName As String)
				MyBase.New(row, column, String.Format("Code value '{0}' specified twice for this record", codeName))
			End Sub
		End Class

#End Region

	End Class
End Namespace