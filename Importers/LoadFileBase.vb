Imports System.Threading
Imports kCura.Utility.Extensions.Enumerable
Imports kCura.WinEDDS.Helpers
Imports kCura.WinEDDS.TApi
Imports Relativity
Imports Relativity.Logging

Namespace kCura.WinEDDS
	Public MustInherit Class LoadFileBase
		Inherits ImportExportTapiBase

#Region "Members"

		Protected _columnsAreInitialized As Boolean
		Protected _columnHeaders As String()

		Protected _documentManager As kCura.WinEDDS.Service.DocumentManager
		Protected _codeManager As kCura.WinEDDS.Service.CodeManager
		Protected _folderManager As kCura.WinEDDS.Service.FolderManager
		Protected _fieldQuery As kCura.WinEDDS.Service.FieldQuery
		Protected _bulkImportManager As kCura.WinEDDS.Service.BulkImportManager
		Protected _usermanager As kCura.WinEDDS.Service.UserManager
		Protected _objectManager As kCura.WinEDDS.Service.ObjectManager

		Protected _filePathColumn As String
		Protected _filePathColumnIndex As Int32
		Protected _firstLineContainsColumnNames As Boolean
		Protected _docFields As DocumentField()
		Protected _multiValueSeparator As Char()
		Protected _allCodes As kCura.Data.DataView
		Protected _allCodeTypes As kCura.Data.DataView
		Protected _folderID As Int32 'The destination folder id
		Protected _caseSystemID As Int32
		Protected _caseArtifactID As Int32
		Protected _timeZoneOffset As Int32
		Protected _autoDetect As Boolean
		Protected _uploadFiles As Boolean
		Protected _fieldMap As LoadFileFieldMap
		Protected _createFolderStructure As Boolean
		Protected _destinationFolder As String

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
		Protected WithEvents _artifactReader As Api.IArtifactReader
		Protected _executionSource As Relativity.ExecutionSource
		Public Property SkipExtractedTextEncodingCheck As Boolean
		Public Property LoadImportedFullTextFromServer As Boolean
		Public Property DisableExtractedTextFileLocationValidation As Boolean
		Public Property OIFileIdMapped As Boolean
		Public Property OIFileIdColumnName As String
		Public Property OIFileTypeColumnName As String
		Public Property FileSizeMapped() As Boolean
		Public Property FileSizeColumn() As String
		Public Property FileNameColumn As String
		Public Property SupportedByViewerColumn As String
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

		Protected Overrides ReadOnly Property CurrentLineNumber() As Int32
			Get
				Return _artifactReader.CurrentLineNumber
			End Get
		End Property

		Protected Overridable ReadOnly Property BulkImportManager As kCura.WinEDDS.Service.BulkImportManager
			Get
				If _bulkImportManager Is Nothing Then _bulkImportManager = New kCura.WinEDDS.Service.BulkImportManager(_settings.Credentials, _settings.CookieContainer)

				Return _bulkImportManager
			End Get
		End Property

#End Region

#Region " Virtual Members "

		Protected MustOverride Function GetSingleCodeValidator() As CodeValidator.Base
		Protected MustOverride Function GetArtifactReader() As Api.IArtifactReader

#End Region

		Protected Sub AdvanceLine()
			_artifactReader.AdvanceRecord()
		End Sub

		Protected Sub New(args As LoadFile, _
		                  reporter As IIoReporter, _
		                  logger As ILog, _
		                  timezoneoffset As Int32, _
		                  doRetryLogic As Boolean, _
		                  autoDetect As Boolean, _
		                  cancellationToken As CancellationTokenSource, _
		                  ByVal Optional executionSource As ExecutionSource = ExecutionSource.Unknown)
			Me.New(args, _
			       reporter, _
			       logger, _
			       timezoneoffset, _
			       doRetryLogic, _
			       autoDetect, _
			       cancellationToken, _
			       initializeArtifactReader:=True, _
			       executionSource := executionSource)
		End Sub

		Protected Sub New(args As LoadFile, _
		                  reporter As IIoReporter, _
		                  logger As ILog, _
		                  timezoneoffset As Int32, _
		                  doRetryLogic As Boolean, _
		                  autoDetect As Boolean, _
		                  cancellationToken As CancellationTokenSource, _
		                  initializeArtifactReader As Boolean, _
		                  ByVal Optional executionSource As ExecutionSource = ExecutionSource.Unknown)
            MyBase.New(reporter, logger, cancellationToken)

			_settings = args
			OIFileIdColumnName = args.OIFileIdColumnName
			OIFileIdMapped = args.OIFileIdMapped
			OIFileTypeColumnName = args.OIFileTypeColumnName
			FileSizeMapped = args.FileSizeMapped
			FileSizeColumn = args.FileSizeColumn
			FileNameColumn = args.FileNameColumn
			SupportedByViewerColumn = args.SupportedByViewerColumn
			_timeZoneOffset = timezoneoffset
			_autoDetect = autoDetect
			_executionSource = executionSource
			InitializeManagers(args)

			If initializeArtifactReader Then
				Me.InitializeArtifactReader()
			End If
		End Sub

		Protected Sub InitializeArtifactReader()
			Dim args As LoadFile = _settings
			_caseArtifactID = args.CaseInfo.ArtifactID
			_artifactReader = Me.GetArtifactReader()
			_docFields = args.FieldMap.DocumentFields
			_filePathColumn = args.NativeFilePathColumn
			_firstLineContainsColumnNames = args.FirstLineContainsHeaders
			_fieldMap = args.FieldMap

			_keyFieldID = args.IdentityFieldId
			_multiValueSeparator = args.MultiRecordDelimiter.ToString.ToCharArray
			_folderID = args.DestinationFolderID
			_caseSystemID = args.CaseInfo.RootArtifactID
			_uploadFiles = args.LoadNativeFiles
			_createFolderStructure = args.CreateFolderStructure
			_destinationFolder = args.FolderStructureContainedInColumn
			_fullTextColumnMapsToFileLocation = args.FullTextColumnContainsFileLocation
			_sourceFileEncoding = args.SourceFileEncoding
			_extractedTextFileEncoding = args.ExtractedTextFileEncoding
			_extractedTextFileEncodingName = args.ExtractedTextFileEncodingName
			_artifactTypeID = args.ArtifactTypeID
			_hierarchicalMultiValueFieldDelmiter = args.HierarchicalValueDelimiter
			_previewCodeCount = args.PreviewCodeCount
			_startLineNumber = args.StartLineNumber
			_codeValidator = Me.GetSingleCodeValidator()

			MulticodeMatrix = New System.Collections.Hashtable
			If _keyFieldID > 0 AndAlso args.OverwriteDestination.ToLower <> Relativity.ImportOverwriteType.Overlay.ToString.ToLower AndAlso args.ArtifactTypeID = Relativity.ArtifactType.Document Then
				_keyFieldID = -1
			End If
			If _keyFieldID = -1 Then
				For Each field As DocumentField In _docFields
					If field.FieldCategory = Relativity.FieldCategory.Identifier Then
						_keyFieldID = field.FieldID
						Exit For
					End If
				Next
			End If
		End Sub

		Protected Overridable Sub InitializeManagers(ByVal args As LoadFile)
			_documentManager = New kCura.WinEDDS.Service.DocumentManager(args.Credentials, args.CookieContainer)
			_codeManager = New kCura.WinEDDS.Service.CodeManager(args.Credentials, args.CookieContainer)
			_folderManager = New kCura.WinEDDS.Service.FolderManager(args.Credentials, args.CookieContainer)
			_fieldQuery = New kCura.WinEDDS.Service.FieldQuery(args.Credentials, args.CookieContainer)
			_usermanager = New kCura.WinEDDS.Service.UserManager(args.Credentials, args.CookieContainer)
			'_bulkImportManager = New kCura.WinEDDS.Service.BulkImportManager(args.Credentials, args.CookieContainer)
			_objectManager = New kCura.WinEDDS.Service.ObjectManager(args.Credentials, args.CookieContainer)
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

		Public Function GetCode(ByVal value As String, ByVal column As Int32, ByVal field As Api.ArtifactField, ByVal forPreview As Boolean) As Nullable(Of Int32)
			Try
				Return _codeValidator.ValidateSingleCode(field, value)
			Catch ex As CodeValidator.CodeCreationException
				Throw New CodeCreationException(Me.CurrentLineNumber, column, ex.IsFatal, ex.Message)
			End Try
		End Function

		Private Function CheckNestedChoicesNameLength(ByVal codeStr As String) As Boolean
			Dim choiceNames As String() = codeStr.Split(New Char() {CChar(_hierarchicalMultiValueFieldDelmiter)})
			For Each choiceString As String In choiceNames
				If choiceString.Length > 200 Then Return True
			Next
			Return False
		End Function

		Public Overridable Function GetMultiCode(ByVal value As String(), ByVal column As Int32, ByVal field As Api.ArtifactField, ByVal forPreview As Boolean) As Nullable(Of Int32)()
			Try
				Dim al As New System.Collections.ArrayList(value)
				Dim goodCodes As New System.Collections.ArrayList
				For Each codeString As String In al
					codeString = codeString.Trim
					If codeString <> "" Then
						If goodCodes.Contains(codeString) Then Throw New DuplicateMulticodeValueException(Me.CurrentLineNumber, column, codeString)
						If CheckNestedChoicesNameLength(codeString) Then Throw New CodeCreationException(Me.CurrentLineNumber, column, False, "Proposed choice name '" & codeString & "' exceeds 200 characters, which is the maximum allowable.")
						goodCodes.Add(codeString)
					End If
				Next
				Dim codeDisplayNames As String() = DirectCast(goodCodes.ToArray(GetType(String)), String())
				Dim i As Int32
				Dim hierarchicCodeManager As Service.IHierarchicArtifactManager
				If forPreview Then
					hierarchicCodeManager = New Service.FieldSpecificCodePreviewer(_codeManager, field.CodeTypeID)
				Else
					hierarchicCodeManager = New Service.FieldSpecificCodeManager(_codeManager, field.CodeTypeID)
				End If
				If Not Me.MulticodeMatrix.Contains(field.CodeTypeID) Then
					Me.MulticodeMatrix.Add(field.CodeTypeID, New NestedArtifactCache(hierarchicCodeManager, _caseSystemID, _caseArtifactID, _hierarchicalMultiValueFieldDelmiter))
				End If
				Dim artifactCache As NestedArtifactCache = DirectCast(Me.MulticodeMatrix(field.CodeTypeID), NestedArtifactCache)
				Dim c As New System.Collections.ArrayList
				For Each codeString As String In codeDisplayNames
					For Each id As Object() In artifactCache.SelectedIds(_hierarchicalMultiValueFieldDelmiter & codeString.Trim(_hierarchicalMultiValueFieldDelmiter.ToCharArray))
						If CType(id(0), Int32) = -200 Then Throw New System.Exception("This choice or multi-choice field is not enabled as unicode.  Upload halted")
						If forPreview Then
							c.Add(CType(id(0), Int32))
							AddToCodeCountPreviewHashTable(field.ArtifactID, field.DisplayName, CType(id(1), String))
						Else
							If Not c.Contains(CType(id(0), Int32)) Then c.Add(CType(id(0), Int32))
						End If
					Next
				Next
				If c.Count > 0 Then
					Dim codes(c.Count - 1) As Nullable(Of Int32)
					For i = 0 To codes.Length - 1
						codes(i) = New Nullable(Of Int32)(CType(c(i), Int32))
					Next
					Return codes
				Else
					Return New Nullable(Of Int32)() {}
				End If
			Catch ex As Exceptions.CodeCreationFailedException
				Throw New CodeCreationException(Me.CurrentLineNumber, column, True, ex.ToString)
			End Try
		End Function

		'returning identifier/artifactID pairs for objects. -1 for new artifactIDs
		Public Overridable Function GetObjects(ByVal value As String(), ByVal column As Int32, ByVal field As Api.ArtifactField, ByVal associatedObjectTypeID As Int32) As System.Collections.Hashtable
			Dim al As New System.Collections.ArrayList(value)
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
				If objectName.Length > field.TextLength Then Throw New kCura.Utility.DelimitedFileImporter.InputStringExceedsFixedLengthException(Me.CurrentLineNumber, column, field.TextLength, field.DisplayName)
				nameIDPairs(objectName) = Me.LookupArtifactIDForName(objectName, associatedObjectTypeID)
			Next
			Return nameIDPairs
		End Function

		Public Overridable Function LookupNameForArtifactID(objectArtifactID As Int32, associatedObjectTypeID As Int32) As String
			Dim artifactTextIdentifier As System.Data.DataSet = _objectManager.RetrieveTextIdentifierOfMappedObject(_caseArtifactID, CInt(objectArtifactID), associatedObjectTypeID)
			Dim retval As String = String.Empty
			If artifactTextIdentifier.Tables(0).Rows.Count > 0 Then retval = DirectCast(artifactTextIdentifier.Tables(0).Rows(0)(0), String)
			Return retval
		End Function

		Public Overridable Function LookupArtifactIDForName(objectName As String, associatedObjectTypeID As Int32) As Int32
			Dim artifactID As System.Data.DataSet = _objectManager.RetrieveArtifactIdOfMappedObject(_caseArtifactID, objectName, associatedObjectTypeID)
			Dim retval As Int32 = -1

			If artifactID.Tables(0).Rows.Count > 0 Then retval = CInt(artifactID.Tables(0).Rows(0)(0))
			Return retval
		End Function

		Public Overridable Function GetObjectsByArtifactID(ByVal value As String(), ByVal column As Int32, ByVal field As Api.ArtifactField, ByVal associatedObjectTypeID As Int32) As System.Collections.Hashtable
			Dim al As New System.Collections.ArrayList(value)
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

			Dim objectArtifactIds As String() = DirectCast(goodObjects.ToArray(GetType(String)), String())
			Dim nameIDPairs As New System.Collections.Hashtable

			For Each objectArtifactId As String In objectArtifactIds
				If objectArtifactId.Length > field.TextLength Then Throw New kCura.Utility.DelimitedFileImporter.InputStringExceedsFixedLengthException(Me.CurrentLineNumber, column, field.TextLength, field.DisplayName)
				nameIDPairs(objectArtifactId) = Me.LookupNameForArtifactID(CInt(objectArtifactId), associatedObjectTypeID)
			Next
			Return nameIDPairs
		End Function

#End Region

		Protected Function FieldValueContainsTextFileLocation(field As Api.ArtifactField) As Boolean
			Dim containsFileLocation As Boolean = (_fullTextColumnMapsToFileLocation AndAlso field.Category = Relativity.FieldCategory.FullText)
			If Not containsFileLocation Then
				containsFileLocation = field.DisplayName.Equals(_settings.LongTextColumnThatContainsPathToFullText, StringComparison.InvariantCultureIgnoreCase)
			End If
			Return containsFileLocation
		End Function


		Public Sub SetFieldValue(ByVal field As Api.ArtifactField, ByVal columnIndex As Int32, ByVal forPreview As Boolean, ByVal identityValue As String, ByRef extractedTextFileCodePageId As Int32, ByVal importBehavior As EDDS.WebAPI.DocumentManagerBase.ImportBehaviorChoice?)
			If TypeOf field.Value Is System.Exception Then
				Throw DirectCast(field.Value, System.Exception)
			End If

			Select Case field.Type
				Case Relativity.FieldTypeHelper.FieldType.Boolean
					field.Value = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(CType(field.Value, Nullable(Of Boolean)))

				Case Relativity.FieldTypeHelper.FieldType.Integer
					field.Value = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(CType(field.Value, Nullable(Of Int32)))

				Case Relativity.FieldTypeHelper.FieldType.Currency, Relativity.FieldTypeHelper.FieldType.Decimal
					field.Value = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(CType(field.Value, Nullable(Of Decimal)))

				Case Relativity.FieldTypeHelper.FieldType.Date
					field.Value = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(CType(field.Value, Nullable(Of DateTime)), True)

				Case Relativity.FieldTypeHelper.FieldType.User
					Dim previewValue As String = String.Empty
					If field.Value Is Nothing Then
						field.Value = String.Empty
					ElseIf field.Value.ToString <> String.Empty Then
						previewValue = field.ValueAsString
						field.Value = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(Me.GetUserArtifactID(field.Value.ToString, columnIndex))
					End If
					If forPreview Then field.Value = previewValue

				Case Relativity.FieldTypeHelper.FieldType.Code
					Dim fieldValue As String
					If field.Value Is Nothing Then
						fieldValue = String.Empty
					Else
						fieldValue = field.Value.ToString.Trim
					End If
					Dim fieldDisplayValue As String = String.Copy(fieldValue)
					Dim code As Nullable(Of Int32) = GetCode(fieldValue, columnIndex, field, forPreview)
					fieldValue = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(code)
					If forPreview Then
						If fieldValue = "-1" Then
							fieldValue = "[new code]"
						End If
						If fieldValue <> String.Empty Then
							AddToCodeCountPreviewHashTable(field.ArtifactID, field.DisplayName, fieldDisplayValue)
						End If
					End If
					field.Value = fieldValue
					If TypeOf Me Is BulkLoadFileImporter Then
						fieldValue = ChrW(11) & fieldDisplayValue & ChrW(11)
						field.Value = fieldValue
						If Not fieldDisplayValue = String.Empty Then
							DirectCast(Me, BulkLoadFileImporter).WriteCodeLineToTempFile(identityValue, Int32.Parse(kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(code)), field.CodeTypeID)
							field.Value = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(code)
						Else
							field.Value = String.Empty
						End If
					End If

				Case Relativity.FieldTypeHelper.FieldType.MultiCode
					Dim value As String() = Nothing
					If Not field.Value Is Nothing Then value = DirectCast(field.Value, String())
					If field.Value Is Nothing Then value = New System.String() {}
					If value.Length = 0 Then
						field.Value = String.Empty
						If TypeOf Me Is BulkLoadFileImporter Then
							field.Value = String.Empty
						End If
					Else
						Dim codeValues As Nullable(Of Int32)() = GetMultiCode(value, columnIndex, field, forPreview)
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
								field.Value = String.Empty
							Else
								'field.Value = ChrW(11) & oldval.Trim(_multiValueSeparator).Replace(_multiValueSeparator, ChrW(11)) & ChrW(11)
								field.Value = ChrW(11) & value.ToDelimitedString(ChrW(11)) & ChrW(11)
								For Each codeValue As Nullable(Of Int32) In codeValues
									If Not codeValue Is Nothing Then
										DirectCast(Me, BulkLoadFileImporter).WriteCodeLineToTempFile(identityValue, codeValue.Value, field.CodeTypeID)
									End If
								Next
								Dim sb As New System.Text.StringBuilder
								For Each codeValue As Nullable(Of Int32) In codeValues
									If Not codeValue Is Nothing Then
										sb.Append(codeValue.Value)
										sb.Append(",")
									End If
								Next
								field.Value = sb.ToString.TrimEnd(","c)
							End If
						End If
					End If

				Case Relativity.FieldTypeHelper.FieldType.Varchar
					If field.Value Is Nothing Then field.Value = String.Empty
					field.Value = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(Me.GetNullableFixedString(field.ValueAsString, columnIndex, field.TextLength, field.DisplayName))
					If field.Category = Relativity.FieldCategory.Relational Then
						If field.Value.ToString = String.Empty AndAlso importBehavior.HasValue AndAlso importBehavior.Value = EDDS.WebAPI.DocumentManagerBase.ImportBehaviorChoice.ReplaceBlankValuesWithIdentifier Then
							field.Value = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(Me.GetNullableFixedString(identityValue, columnIndex, field.TextLength, field.DisplayName))
						End If
					End If

				Case Relativity.FieldTypeHelper.FieldType.Object
					If field.Value Is Nothing Then field.Value = String.Empty
					field.Value = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(GetNullableAssociatedObjectName(field.Value.ToString, columnIndex, field.TextLength, field.DisplayName))
					If forPreview Then field.Value = field.Value.ToString.Trim

				Case Relativity.FieldTypeHelper.FieldType.Objects
					If (Not Me._settings.ObjectFieldIdListContainsArtifactId Is Nothing) AndAlso Me._settings.ObjectFieldIdListContainsArtifactId.Contains(field.ArtifactID) Then
						SetFieldValueObjectsByArtifactID(field, columnIndex, forPreview, identityValue)
					Else
						SetFieldValueObjectsByName(field, columnIndex, forPreview, identityValue)
					End If

				Case Relativity.FieldTypeHelper.FieldType.Text, Relativity.FieldTypeHelper.FieldType.OffTableText
					If FieldValueContainsTextFileLocation(field) Then
						Dim value As String = field.ValueAsString
						Dim performExtractedTextFileLocationValidation As Boolean = Not DisableExtractedTextFileLocationValidation
						If value = String.Empty Then
							field.Value = String.Empty
							Exit Select
						End If

						Try
							' Prevent nested retry operations.
							Const retry As Boolean = False
							Dim maxRetryAttempts As Integer = kCura.Utility.Config.IOErrorNumberOfRetries
							Dim currentRetryAttempt As Integer = 0

							' REL-272765: Added I/O resiliency and support document level errors.
							Dim policy As IWaitAndRetryPolicy = New WaitAndRetryPolicy(
								kCura.Utility.Config.IOErrorNumberOfRetries, _
								kCura.Utility.Config.IOErrorWaitTimeInSeconds)

							' Note: a lambda can't modify a ref param; therefore, a policy block return value is used.
							Dim returnCodePage As Int32? = policy.WaitAndRetry(Of Int32?)(
								RetryPolicies.IoStandardPolicy,
								Function(count)
									currentRetryAttempt = count
									Return TimeSpan.FromSeconds(kCura.Utility.Config.IOErrorWaitTimeInSeconds)
								End Function,
								Sub(exception, span)
									Me.PublishIoRetryMessage(exception, span, currentRetryAttempt, maxRetryAttempts)
								End Sub,
								Function(token) As Int32?
									Dim codePage As Int32? = Nothing
									If (performExtractedTextFileLocationValidation) Then
										Dim foundFileName As String = Me.GetExistingFilePath(value, retry)
										Dim fileExists As Boolean = Not String.IsNullOrEmpty(foundFileName)
										
										If Not fileExists
											Throw New MissingFullTextFileException(Me.CurrentLineNumber, columnIndex)
										End If

										If Not String.Equals(value, foundFileName)
											Dim message As String = $"File {value} defined in column {columnIndex} in line {Me.CurrentLineNumber} does not exist. File {foundFileName} will be used instead."
											Me.PublishIoWarningEvent(new IoWarningEventArgs(message, currentLineNumber))
											value = foundFileName
										End If
									End If
									
									Dim detectedEncoding As System.Text.Encoding = _extractedTextFileEncoding
									Dim determinedEncodingStream As DeterminedEncodingStream

									'This logic exists as an attempt to improve import speeds.  The DetectEncoding call first checks if the file
									' exists, followed by a read of the first few bytes. The File.Exists check can be very expensive when going
									' across the network for the file, so this override allows that check to be skipped.
									' -Phil S. 07/27/2012
									If Not SkipExtractedTextEncodingCheck Then
										determinedEncodingStream = kCura.WinEDDS.Utility.DetectEncoding(value, False, performExtractedTextFileLocationValidation)
										detectedEncoding = determinedEncodingStream.DeterminedEncoding
									End If
															
									If (performExtractedTextFileLocationValidation AndAlso (Me.GetFileLength(value, retry) > GetMaxExtractedTextLength(detectedEncoding))) Then
										Throw New ExtractedTextTooLargeException
									Else
										If forPreview Then
											' Determine Encoding Here
											determinedEncodingStream = kCura.WinEDDS.Utility.DetectEncoding(value, False)
											detectedEncoding = determinedEncodingStream.DeterminedEncoding
											Dim chosenEncoding As System.Text.Encoding
											If detectedEncoding IsNot Nothing Then
												chosenEncoding = detectedEncoding
											Else
												chosenEncoding = _extractedTextFileEncoding
											End If
											Dim sr As New System.IO.StreamReader(determinedEncodingStream.UnderlyingStream, chosenEncoding)
											Dim i As Int32 = 0
											Dim sb As New System.Text.StringBuilder
											While sr.Peek <> -1 AndAlso i < 100
												sb.Append(ChrW(sr.Read))
												i += 1
											End While
											If i = 100 Then
												sb.Append("...")
											End If
											codePage = chosenEncoding.CodePage
											sr.Close()
											determinedEncodingStream.Close()
											field.Value = sb.ToString
										Else
											field.Value = value
										End If
									End If
									Return codePage
								End Function,
								Me.CancellationToken)
							' Only update the ref parameter when a non-null value is returned.
							If returnCodePage.HasValue Then
								extractedTextFileCodePageId = returnCodePage.Value
							End If
						Catch ex As System.IO.FileNotFoundException
							Throw
						Catch ex As System.IO.IOException
							Dim message As String = $"An I/O error occurred reading the file associated with the '{field.DisplayName}' full text field."
							Throw New kCura.WinEDDS.Exceptions.ImportIOException(message, ex)
						End Try
					End If
				Case Else
					Throw New System.Exception("Unsupported Field Type '" & field.Type.ToString & "'")
			End Select
		End Sub

		Private Sub SetFieldValueObjectsByName(ByVal field As Api.ArtifactField, ByVal columnIndex As Int32, ByVal forPreview As Boolean, ByVal identityValue As String)
			Dim value As String() = Nothing
			If Not field.Value Is Nothing Then value = DirectCast(field.Value, String())
			If field.Value Is Nothing Then value = New System.String() {}

			If value.Length = 0 Then
				field.Value = String.Empty
			Else
				Dim objectValues As System.Collections.Hashtable = GetObjects(value, columnIndex, field, field.AssociatedObjectTypeID)
				Dim newVal As String = String.Empty
				Dim objName As String
				If objectValues.Count > 0 Then
					For Each objName In objectValues.Keys
						'If forPreview And DirectCast(objectValues(objName), Int32) = -1 Then
						'	objectValues(objName) = "[new object]"
						'End If
						'newVal &= ";" & objName
						If forPreview And DirectCast(objectValues(objName), Int32) = -1 Then
							newVal &= ";" & "[new object]"
						Else
							newVal &= ";" & objName
						End If
					Next
				End If
				newVal = newVal.TrimStart(";"c)
				field.Value = newVal
				If TypeOf Me Is BulkLoadFileImporter Then
					If objectValues.Count = 0 Then
						field.Value = ""
					Else
						field.Value = ChrW(11) & value.ToDelimitedString(ChrW(11)) & ChrW(11)
						Dim sb As New System.Text.StringBuilder
						For Each objectValue As String In objectValues.Keys
							DirectCast(Me, BulkLoadFileImporter).WriteObjectLineToTempFile(identityValue, objectValue, CType(objectValues(objectValue), Int32), field.AssociatedObjectTypeID, field.ArtifactID)
							sb.Append("'" & objectValue & "'")
							sb.Append(",")
						Next
						field.Value = sb.ToString.TrimEnd(","c)
					End If
				End If
			End If
		End Sub

		Private Sub SetFieldValueObjectsByArtifactID(ByVal field As Api.ArtifactField, ByVal columnIndex As Int32, ByVal forPreview As Boolean, ByVal identityValue As String)
			Dim value As String() = Nothing
			If Not field.Value Is Nothing Then value = DirectCast(field.Value, String())
			If field.Value Is Nothing Then value = New System.String() {}

			If value.Length = 0 Then
				field.Value = String.Empty
			Else
				Dim objectValues As System.Collections.Hashtable = GetObjectsByArtifactID(value, columnIndex, field, field.AssociatedObjectTypeID)

				Dim newVal As String = String.Empty
				Dim objArtifactID As String
				If objectValues.Count > 0 Then
					For Each objArtifactID In objectValues.Keys
						If forPreview And String.IsNullOrEmpty(objectValues(objArtifactID).ToString()) Then
							newVal &= ";" & "[new object]"
						Else
							newVal &= ";" & objArtifactID
						End If
					Next
				End If
				newVal = newVal.TrimStart(";"c)
				field.Value = newVal
				If TypeOf Me Is BulkLoadFileImporter Then
					If objectValues.Count = 0 Then
						field.Value = ""
					Else
						field.Value = ChrW(11) & value.ToDelimitedString(ChrW(11)) & ChrW(11)
						Dim sb As New System.Text.StringBuilder
						For Each objectValue As String In objectValues.Keys
							DirectCast(Me, BulkLoadFileImporter).WriteObjectLineToTempFile(identityValue, objectValues(objectValue).ToString(), CInt(objectValue), field.AssociatedObjectTypeID, field.ArtifactID)
							sb.Append(objectValue)
							sb.Append(",")
						Next
						field.Value = sb.ToString.TrimEnd(","c)
					End If
				End If
			End If
		End Sub

		Public Function GetMaxExtractedTextLength(ByVal encoding As System.Text.Encoding) As Int32
			Dim oneGigabyte As Int32 = Convert.ToInt32(Int32.MaxValue / 2)

			If encoding Is Nothing Then
				' Unknown encoding, assume it's ASCII or some other one-byte encoding scheme. When writing to the BCPPath
				' this will be converted to UTF-16 which will double the size of the data.
				Return oneGigabyte
			ElseIf encoding Is System.Text.Encoding.UTF8 Then
				Return oneGigabyte
			Else
				' Encoding is already UTF-16 (or comparable)
				Return Int32.MaxValue
			End If
		End Function

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

		Public Function GetUserArtifactID(ByVal value As String, ByVal column As Int32) As Nullable(Of Int32)
			If value = String.Empty Then Return Nothing
			Dim retval As Nullable(Of Int32) = Me.Users(value)
			If retval Is Nothing Then
				Throw New MissingUserException(Me.CurrentLineNumber, column, value)
			Else
				Return retval
			End If
		End Function

		Public Function GetNullableFixedString(ByVal value As String, ByVal column As Int32, ByVal fieldLength As Int32, ByVal displayName As String) As String
			If value.Length > fieldLength Then
				Throw New kCura.Utility.DelimitedFileImporter.InputStringExceedsFixedLengthException(CurrentLineNumber, column, fieldLength, displayName)
			Else
				Return kCura.Utility.NullableTypesHelper.DBNullString(value)
			End If
		End Function

		Public Function GetNullableAssociatedObjectName(ByVal value As String, ByVal column As Int32, ByVal fieldLength As Int32, ByVal fieldName As String) As String
			If value.Length > fieldLength Then
				Throw New kCura.Utility.DelimitedFileImporter.InputObjectNameExceedsFixedLengthException(CurrentLineNumber, column, fieldLength, fieldName)
			Else
				Return kCura.Utility.NullableTypesHelper.DBNullString(value)
			End If
		End Function

#Region "Exceptions"

		''' <summary>
		''' The exception thrown when the extracted text file length exceeds the max extracted text length.
		''' When the encoding is not specified or is <see cref="System.Text.Encoding.UTF8"/>, the max length is 1GB;
		''' otherwise, the max length is <see cref="System.Int32.MaxValue"/>.
		''' </summary>
		<Serializable>
		Public Class ExtractedTextTooLargeException
			Inherits kCura.Utility.ImporterExceptionBase

			''' <summary>
			''' Initializes a new instance of the <see cref="ExtractedTextTooLargeException"/> class.
			''' </summary>
			Public Sub New()
				MyBase.New(String.Format("Extracted text is too large."))
			End Sub

			''' <inheritdoc />
			<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
			Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
				MyBase.New(info, context)
			End Sub
		End Class

		''' <summary>
		''' The exception thrown when the identity value has already been processed.
		''' </summary>
		<Serializable>
		Public Class IdentifierOverlapException
			Inherits kCura.Utility.ImporterExceptionBase

			''' <summary>
			''' Initializes a new instance of the <see cref="IdentifierOverlapException"/> class.
			''' </summary>
			''' <param name="identityValue">
			''' The document identity value.
			''' </param>
			''' <param name="previousLineNumber">
			''' The line number containing the existing identity value.
			''' </param>
			Public Sub New(ByVal identityValue As String, ByVal previousLineNumber As String)
				MyBase.New(String.Format("Document '({0})' has been previously processed in this file on line {1}.", identityValue, previousLineNumber))
			End Sub

			''' <inheritdoc />
			<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
			Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
				MyBase.New(info, context)
			End Sub
		End Class

		''' <summary>
		''' The exception thrown when extracted text file validation is enabled and the file does not exist.
		''' </summary>
		<Serializable>
		Public Class MissingFullTextFileException
			Inherits kCura.Utility.ImporterExceptionBase

			''' <summary>
			''' Initializes a new instance of the <see cref="MissingFullTextFileException"/> class.
			''' </summary>
			''' <param name="row">
			''' The row where the error occurred.
			''' </param>
			''' <param name="column">
			''' The column where the error occurred.
			''' </param>
			Public Sub New(ByVal row As Int32, ByVal column As Int32)
				MyBase.New(row, column, String.Format("Error: full text file specified does not exist."))
			End Sub

			''' <inheritdoc />
			<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
			Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
				MyBase.New(info, context)
			End Sub
		End Class

		''' <summary>
		''' The exception thrown when the user artifact does not exist within the Relativity instance.
		''' </summary>
		<Serializable>
		Public Class MissingUserException
			Inherits kCura.Utility.ImporterExceptionBase

			''' <summary>
			''' Initializes a new instance of the <see cref="MissingUserException"/> class.
			''' </summary>
			''' <param name="row">
			''' The row where the error occurred.
			''' </param>
			''' <param name="column">
			''' The column where the error occurred.
			''' </param>
			''' <param name="invalidEmailaddress">
			''' The invalid email or user account.
			''' </param>
			Public Sub New(ByVal row As Int32, ByVal column As Int32, ByVal invalidEmailaddress As String)
				MyBase.New(row, column, String.Format("User '{0}' does not exist in the system or is not available for assignment.", invalidEmailaddress))
			End Sub

			''' <inheritdoc />
			<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
			Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
				MyBase.New(info, context)
			End Sub
		End Class

		''' <summary>
		''' The exception thrown when a failure occurs attempting to create a new code.
		''' </summary>
		<Serializable>
		Public Class CodeCreationException
			Inherits kCura.Utility.ImporterExceptionBase

			''' <summary>
			''' Initializes a new instance of the <see cref="CodeCreationException"/> class.
			''' </summary>
			''' <param name="row">
			''' The row where the error occurred.
			''' </param>
			''' <param name="column">
			''' The column where the error occurred.
			''' </param>
			''' <param name="isFatal">
			''' Specify whether the error is fatal.
			''' </param>
			''' <param name="errorText">
			''' The error text.
			''' </param>
			Public Sub New(ByVal row As Int32, ByVal column As Int32, ByVal isFatal As Boolean, ByVal errorText As String)
				MyBase.New(row, column, errorText)
				Me.IsFatal = isFatal
			End Sub

			''' <inheritdoc />
			<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
			Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
				MyBase.New(info, context)
				Me.IsFatal = info.GetBoolean("IsFatal")
			End Sub

			''' <summary>
			''' Gets a value indicating whether the error is fatal.
			''' </summary>
			''' <value>
			''' <see langword="true"/> if the error is considered fatal; otherwise, <see langword="false"/>.
			''' </value>
			Public ReadOnly Property IsFatal As Boolean

			''' <inheritdoc />
			<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
			Public Overrides Sub GetObjectData(info As System.Runtime.Serialization.SerializationInfo, context As System.Runtime.Serialization.StreamingContext)
				info.AddValue("IsFatal", Me.IsFatal)
				MyBase.GetObjectData(info, context)
			End Sub
		End Class

		''' <summary>
		''' The exception thrown when the load file line includes more columns than defined by the headers.
		''' </summary>
		<Serializable>
		Public Class ColumnCountMismatchException
			Inherits kCura.Utility.ImporterExceptionBase

			''' <summary>
			''' Initializes a new instance of the <see cref="ColumnCountMismatchException"/> class.
			''' </summary>
			''' <param name="row">
			''' The row where the error occurred.
			''' </param>
			''' <param name="expecting">
			''' The expected number of cells in this row.
			''' </param>
			''' <param name="actual">
			''' The actual number of cells in this row.
			''' </param>
			Public Sub New(ByVal row As Int32, ByVal expecting As Int32, ByVal actual As Int32)
				MyBase.New(row, -1, String.Format("There are an invalid number of cells in this row - expecting:{0}, actual:{1}.", expecting, actual))
			End Sub

			''' <inheritdoc />
			<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
			Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
				MyBase.New(info, context)
			End Sub
		End Class

		''' <summary>
		''' The exception thrown when an object reference already exists and would result in a duplicate if allowed to be imported.
		''' </summary>
		<Serializable>
		Public Class DuplicateObjectReferenceException
			Inherits kCura.Utility.ImporterExceptionBase

			''' <summary>
			''' Initializes a new instance of the <see cref="DuplicateObjectReferenceException"/> class.
			''' </summary>
			''' <param name="row">
			''' The row where the error occurred.
			''' </param>
			''' <param name="column">
			''' The column where the error occurred.
			''' </param>
			''' <param name="fieldName">
			''' The field name for the existing reference.
			''' </param>
			Public Sub New(ByVal row As Int32, ByVal column As Int32, ByVal fieldName As String)
				MyBase.New(row, column, String.Format("Object identifier for field {0} references an identifier that is not unique.", fieldName))
			End Sub

			''' <inheritdoc />
			<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
			Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
				MyBase.New(info, context)
			End Sub
		End Class

		''' <summary>
		''' The exception thrown when a parent reference does not exist.
		''' </summary>
		<Serializable>
		Public Class NonExistentParentException
			Inherits kCura.Utility.ImporterExceptionBase

			''' <summary>
			''' Initializes a new instance of the <see cref="NonExistentParentException"/> class.
			''' </summary>
			''' <param name="row">
			''' The row where the error occurred.
			''' </param>
			''' <param name="column">
			''' The column where the error occurred.
			''' </param>
			''' <param name="fieldName">
			''' The field name for the non-existent parent reference.
			''' </param>
			Public Sub New(ByVal row As Int32, ByVal column As Int32, ByVal fieldName As String)
				MyBase.New(row, column, String.Format("Object references a parent object that does not exist.", fieldName))
			End Sub

			''' <inheritdoc />
			<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
			Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
				MyBase.New(info, context)
			End Sub
		End Class

		''' <summary>
		''' The exception thrown when a parent object reference does not exist.
		''' </summary>
		<Serializable>
		Public Class ParentObjectReferenceRequiredException
			Inherits kCura.Utility.ImporterExceptionBase

			''' <summary>
			''' Initializes a new instance of the <see cref="ParentObjectReferenceRequiredException"/> class.
			''' </summary>
			''' <param name="row">
			''' The row where the error occurred.
			''' </param>
			''' <param name="column">
			''' The column where the error occurred.
			''' </param>
			Public Sub New(ByVal row As Int32, ByVal column As Int32)
				MyBase.New(row, column, String.Format("Null parent object identifier found, this is required for the Parent Info field."))
			End Sub

			''' <inheritdoc />
			<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
			Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
				MyBase.New(info, context)
			End Sub
		End Class

		''' <summary>
		''' The exception thrown when a failure occurs accessing the BCP share.
		''' </summary>
		<Serializable>
		Public Class BcpPathAccessException
			Inherits kCura.Utility.ImporterExceptionBase

			''' <summary>
			''' Initializes a new instance of the <see cref="BcpPathAccessException"/> class.
			''' </summary>
			''' <param name="details">
			''' The error details.
			''' </param>
			Public Sub New(ByVal details As String)
				MyBase.New("Error accessing the bcp share. Please contact your system administrator with the following details: " & System.Environment.NewLine & details)
			End Sub

			''' <inheritdoc />
			<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
			Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
				MyBase.New(info, context)
			End Sub
		End Class

		''' <summary>
		''' The exception thrown when a multi-choice value already exists and would result in a duplicate if allowed to be imported.
		''' </summary>
		<Serializable>
		Public Class DuplicateMulticodeValueException
			Inherits kCura.Utility.ImporterExceptionBase

			''' <summary>
			''' Initializes a new instance of the <see cref="DuplicateMulticodeValueException"/> class.
			''' </summary>
			''' <param name="row">
			''' The row where the error occurred.
			''' </param>
			''' <param name="column">
			''' The column where the error occurred.
			''' </param>
			''' <param name="codeName">
			''' The choice name that was duplicated.
			''' </param>
			Public Sub New(ByVal row As Int32, ByVal column As Int32, ByVal codeName As String)
				MyBase.New(row, column, String.Format("Code value '{0}' specified twice for this record", codeName))
			End Sub

			''' <inheritdoc />
			<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
			Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
				MyBase.New(info, context)
			End Sub
		End Class

#End Region

		Private Sub _artifactReader_OnIoWarning(ByVal e As Api.IoWarningEventArgs) Handles _artifactReader.OnIoWarning
			If e.Exception Is Nothing Then
				Me.PublishIoWarningEvent(new IoWarningEventArgs(e.Message, e.CurrentLineNumber))
			Else
				Dim message As String = IoReporter.BuildIoReporterWarningMessage(e.Exception, e.WaitTime)
				Me.PublishIoWarningEvent(new IoWarningEventArgs(message, e.CurrentLineNumber))
			End If
		End Sub
	End Class
End Namespace