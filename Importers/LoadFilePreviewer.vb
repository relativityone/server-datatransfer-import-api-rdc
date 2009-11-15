'Imports kCura.EDDS.DynamicFields
Namespace kCura.WinEDDS
	Public Class LoadFilePreviewer
		Inherits kCura.WinEDDS.LoadFileBase



#Region "Members"
		Private _errorsOnly As Boolean
		Private WithEvents _processController As kCura.Windows.Process.Controller
		Private _continue As Boolean = True
		Private _columnCount As Int32 = 0
		Private _nativeFileCheckColumnName As String = ""
		Private _relationalDocumentFields As DocumentField()
		Private _selectedCaseArtifactID As Int32
#End Region

#Region "Constructors"

		Public Sub New(ByVal args As LoadFile, ByVal timeZoneOffset As Int32, ByVal errorsOnly As Boolean, ByVal doRetryLogic As Boolean, Optional ByVal processController As kCura.Windows.Process.Controller = Nothing)
			MyBase.New(args, timeZoneOffset, doRetryLogic, True)
			_selectedCaseArtifactID = args.CaseInfo.ArtifactID
			_errorsOnly = errorsOnly
			_processController = processController
		End Sub

#End Region

#Region "Event"

		Public Enum EventType
			Begin
			Complete
			Progress
		End Enum

		Public Class EventArgs
			Private _bytesRead As Int64
			Private _totalBytes As Int64
			Private _stepSize As Int64
			Private _type As EventType

			Public ReadOnly Property BytesRead() As Int64
				Get
					Return _bytesRead
				End Get
			End Property

			Public ReadOnly Property TotalBytes() As Int64
				Get
					Return _totalBytes
				End Get
			End Property

			Public ReadOnly Property StepSize() As Int64
				Get
					Return _stepSize
				End Get
			End Property
			Public ReadOnly Property Type() As EventType
				Get
					Return _type
				End Get
			End Property

			Public Sub New(ByVal type As EventType, ByVal bytes As Int64, ByVal total As Int64, ByVal [step] As Int64)
				_bytesRead = bytes
				_totalBytes = total
				_stepSize = [step]
				_type = type
			End Sub
		End Class
		Public Event OnEvent(ByVal e As EventArgs)

		Private Sub ProcessStart(ByVal bytes As Int64, ByVal total As Int64, ByVal [step] As Int64)
			RaiseOnEvent(EventType.Begin, bytes, total, [step])
		End Sub

		Private Sub ProcessProgress(ByVal bytes As Int64, ByVal total As Int64, ByVal [step] As Int64)
			RaiseOnEvent(EventType.Progress, bytes, total, [step])
		End Sub

		Private Sub ProcessComplete(ByVal bytes As Int64, ByVal total As Int64, ByVal [step] As Int64)
			RaiseOnEvent(EventType.Complete, bytes, total, [step])
		End Sub

		Private Sub RaiseOnEvent(ByVal type As EventType, ByVal bytes As Int64, ByVal total As Int64, ByVal [step] As Int64)
			RaiseEvent OnEvent(New EventArgs(type, bytes, total, [step]))
		End Sub
#End Region
		Private _processedIdentifiers As New Collections.Specialized.NameValueCollection

		Private Function GetPosition() As Int32
			Return CType(Me.Reader.BaseStream.Position, Int32)
		End Function

		Public Overrides Function ReadFile(ByVal path As String) As Object
			Dim earlyexit As Boolean = False
			_relationalDocumentFields = _fieldQuery.RetrieveAllAsDocumentFieldCollection(_selectedCaseArtifactID, _artifactTypeID).GetFieldsByCategory(DynamicFields.Types.FieldCategory.Relational)
			Reader = New System.IO.StreamReader(path, _sourceFileEncoding)
			Dim filesize As Int64 = Reader.BaseStream.Length
			Dim stepsize As Int64 = CType(filesize / 100, Int64)
			ProcessStart(0, filesize, stepsize)
			Dim fieldArrays As New System.Collections.ArrayList
			If _firstLineContainsColumnNames Then
				_columnHeaders = GetLine
				_columnCount = _columnHeaders.Length
				If _uploadFiles Then
					Dim openParenIndex As Int32 = _filePathColumn.LastIndexOf("("c) + 1
					Dim closeParenIndex As Int32 = _filePathColumn.LastIndexOf(")"c)
					_filePathColumnIndex = Int32.Parse(_filePathColumn.Substring(openParenIndex, closeParenIndex - openParenIndex)) - 1
				End If
				'_filePathColumnIndex = Array.IndexOf(_columnHeaders, _filePathColumn)
			Else
				If _uploadFiles Then
					_filePathColumnIndex = Int32.Parse(_filePathcolumn.Replace("Column", "").Replace("(", "").Replace(")", "").Trim) - 1
				End If
			End If
			Dim i As Int32 = 0
			i = 0
			While Not HasReachedEOF AndAlso _continue
				If fieldArrays.Count < 1000 Then
					Dim line As String() = Me.GetLine
					If Not _firstLineContainsColumnNames AndAlso fieldArrays.Count = 0 Then
						_columnCount = line.Length
					End If
					Dim x As DocumentField() = CheckLine(line)
					If Not x Is Nothing Then fieldArrays.Add(x)
					i += 1
					If i Mod 100 = 0 Then ProcessProgress(GetPosition, filesize, stepsize)
				Else
					earlyexit = True
					Exit While
				End If
			End While
			If earlyexit Then
				ProcessComplete(-1, filesize, -1)
			Else
				ProcessComplete(filesize, filesize, stepsize)
			End If
			Me.Close()
			Return fieldArrays
		End Function


		Private Function CheckLine(ByVal values As String()) As DocumentField()
			Dim mapItem As LoadFileFieldMap.LoadFileFieldMapItem
			Dim lineContainsErrors As Boolean = False
			Dim retval As New ArrayList
			Dim valToParse As String = ""
			Dim identifierField As DocumentField
			Dim groupIdentifierField As DocumentField
			Dim duplicateHashField As DocumentField
			Dim unmappedFields As New System.Collections.Specialized.HybridDictionary
			Dim mappedFields As New System.Collections.Specialized.HybridDictionary
			Dim idFieldColumnIndex As Int32
			Dim idFieldOriginalValue As String
			For Each relationalField As DocumentField In _relationalDocumentFields
				unmappedFields.Add(relationalField.FieldID, relationalField)
			Next
			For Each mapItem In _fieldMap
				If mapItem.NativeFileColumnIndex > -1 AndAlso Not mapItem.DocumentField Is Nothing Then
					Try
						valToParse = values(mapItem.NativeFileColumnIndex)
					Catch ex As System.Exception
						valToParse = ""
					End Try
					Dim docfield As New DocumentField(mapItem.DocumentField)
					Select Case docfield.FieldCategoryID
						Case kCura.DynamicFields.Types.FieldCategory.Relational
							If unmappedFields.Contains(docfield.FieldID) Then
								unmappedFields.Remove(docField.FieldID)
							End If
							If Not mappedFields.Contains(docField.FieldID) Then
								mappedFields.Add(docField.FieldID, docfield)
							End If
							'If docfield.FieldName.ToLower = "group identifier" Then
							'	groupIdentifierField = docfield
							'End If
							'If docfield.FieldName.ToLower = "md5 hash" Then
							'	duplicateHashField = docfield
							'End If
						Case kCura.DynamicFields.Types.FieldCategory.Identifier
							If Not _keyFieldID > 0 Then identifierField = docfield
					End Select
					If _keyFieldID > 0 AndAlso _keyFieldID = docField.FieldID Then
						identifierField = docfield
						idFieldColumnIndex = mapItem.NativeFileColumnIndex
						idFieldOriginalValue = valToParse
					End If

					lineContainsErrors = lineContainsErrors Or SetFieldValueOrErrorMessage(docfield, valToParse, mapItem.NativeFileColumnIndex)
					'dont add field if object type is not a document and the field is a file field
					If Not (_artifactTypeID <> kCura.EDDS.Types.ArtifactType.Document And docfield.FieldTypeID = kCura.DynamicFields.Types.FieldTypeHelper.FieldType.File) Then
						retval.Add(docfield)
					End If
				End If
			Next
			If _columnCount <> values.Length Then
				lineContainsErrors = True
				Dim df As DocumentField
				For Each df In retval
					df.Value = New ColumnCountMismatchException(Me.CurrentLineNumber, _columnCount, values.Length).Message
				Next
			End If

			If Not identifierField Is Nothing Then
				If _processedIdentifiers(identifierField.Value) Is Nothing Then
					_processedIdentifiers(identifierField.Value) = Me.CurrentLineNumber.ToString
				Else
					'Throw New IdentifierOverlapException(identifierField.Value, _processedIdentifiers(identifierField.Value))
					identifierField.Value = String.Format("Error: The identifier '{0}' has been previously proccessed on line {1}.", identifierField.Value, _processedIdentifiers(identifierField.Value))
					lineContainsErrors = True
				End If
			End If

			If Not identifierField Is Nothing And _artifactTypeID = kCura.EDDS.Types.ArtifactType.Document Then
				For Each field As DocumentField In unmappedFields.Values
					Dim f As New DocumentField(field)
					If _columnCount <> values.Length Then
						f.Value = New ColumnCountMismatchException(Me.CurrentLineNumber, _columnCount, values.Length).Message
					Else
						lineContainsErrors = lineContainsErrors Or SetFieldValueOrErrorMessage(f, idFieldOriginalValue, idFieldColumnIndex)
						'f.Value = identifierField.Value & " (if not set)"
					End If
					retval.Add(f)
				Next
				For Each field As DocumentField In mappedFields.Values
					If field.Value = "" Then field.Value = identifierField.Value
				Next
			End If

			'If Not identifierField Is Nothing AndAlso _
			' Not groupIdentifierField Is Nothing AndAlso _
			' groupIdentifierField.Value = "" Then
			'	groupIdentifierField.Value = identifierField.Value
			'End If

			'If Not identifierField Is Nothing Then
			'	If Not duplicateHashField Is Nothing Then
			'		If duplicateHashField.Value = "" Then duplicateHashField.Value = identifierField.Value
			'	Else
			'		Dim docfield As New DocumentField("MD5 Hash", -1, -1, -1, NullableInt32.Null, NullableInt32.Null, False)
			'		If _extractMd5Hash Then
			'			docfield.Value = "[Extracted from native]"
			'		Else
			'			docField.Value = identifierField.Value & " (if not set)"
			'		End If
			'		retval.Add(docfield)
			'	End If
			'End If

			If _uploadFiles Then
				If _nativeFileCheckColumnName = "" Then Me.SetNativeFileCheckColumnName(retval)
				Dim filePath As String = values(_filePathColumnIndex)
				Dim existsFilePath As String
				If filePath.Length > 1 Then
					If filePath.Chars(0) = "\"c AndAlso Not filePath.Chars(1) = "\" Then
						existsFilePath = "." & filePath
					Else
						existsFilePath = filePath
					End If
				End If
				Dim docfield As New DocumentField(_nativeFileCheckColumnName, -1, -1, -1, NullableInt32.Null, NullableInt32.Null, NullableInt32.Null, False)
				If filePath = "" Then
					docfield.Value = "No File Specified."
				ElseIf Not System.IO.File.Exists(existsFilePath) Then
					docfield.Value = String.Format("Error: file '{0}' does not exist", filePath)
					lineContainsErrors = True
				Else
					docfield.Value = filePath
				End If
				retval.Add(docfield)
			End If

			If _createFolderStructure AndAlso _artifactTypeID <> kCura.EDDS.Types.ArtifactType.Document Then
				Dim openParenIndex As Int32 = _destinationFolder.LastIndexOf("("c) + 1
				Dim closeParenIndex As Int32 = _destinationFolder.LastIndexOf(")"c)
				Dim parentObjectIdentifierIndex As Int32 = Int32.Parse(_destinationFolder.Substring(openParenIndex, closeParenIndex - openParenIndex)) - 1
				Dim docfield As New DocumentField("Parent Object Identifier", -1, -1, -1, NullableInt32.Null, NullableInt32.Null, NullableInt32.Null, False)
				docField.Value = values(parentObjectIdentifierIndex)
				Dim textIdentifier As String = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(kCura.Utility.NullableTypesHelper.ToNullableString(values(parentObjectIdentifierIndex)))
				If textIdentifier = "" Then
					docField.Value = New ParentObjectReferenceRequiredException(Me.CurrentLineNumber, parentObjectIdentifierIndex).Message
					lineContainsErrors = True
				Else
					Dim parentObjectTable As System.Data.DataTable = _objectManager.RetrieveArtifactIdOfMappedParentObject(_caseArtifactID, _
					textIdentifier, _artifactTypeID).Tables(0)
					If parentObjectTable.Rows.Count > 1 Then
						docField.Value = New DuplicateObjectReferenceException(Me.CurrentLineNumber, parentObjectIdentifierIndex, "Parent Info").Message
						lineContainsErrors = True
					End If
				End If
				retval.Add(docfield)
			End If
			If _createFolderStructure AndAlso _artifactTypeID = kCura.EDDS.Types.ArtifactType.Document Then
				Dim openParenIndex As Int32 = _destinationFolder.LastIndexOf("("c) + 1
				Dim closeParenIndex As Int32 = _destinationFolder.LastIndexOf(")"c)
				Dim parentObjectIdentifierIndex As Int32 = Int32.Parse(_destinationFolder.Substring(openParenIndex, closeParenIndex - openParenIndex)) - 1
				Dim docfield As New DocumentField("Parent_Folder_Identifier", -1, -1, -1, NullableInt32.Null, NullableInt32.Null, NullableInt32.Null, False)
				docField.Value = values(parentObjectIdentifierIndex)
				retval.Add(docfield)
			End If

			If _errorsOnly Then
				If lineContainsErrors Then
					Return DirectCast(retval.ToArray(GetType(DocumentField)), DocumentField())
				Else
					Return Nothing
				End If
			Else
				Return DirectCast(retval.ToArray(GetType(DocumentField)), DocumentField())
			End If
		End Function

		Private Sub SetNativeFileCheckColumnName(ByVal fields As System.Collections.ArrayList)
			Dim val As String = "Native File to Upload"
			Dim field As DocumentField
			Dim i As Int32
			For i = 0 To 100
				Dim isNameInUse As Boolean = False
				For Each field In fields
					If val = field.FieldName Then
						isNameInUse = True
						Exit For
					End If
				Next
				If Not isNameInUse Then
					_nativeFileCheckColumnName = val
					Exit Sub
				Else
					val &= " "
				End If
			Next
			_nativeFileCheckColumnName = val
		End Sub

		'Private Function CheckLine(ByVal values As String()) As DocumentField()
		'	Dim docFields(_docFields.Length - 1) As DocumentField
		'	Dim docField As DocumentField
		'	Dim i As Int32 = 0
		'	Dim lineContainsErrors As Boolean = False
		'	'Dim fieldIDs(_docFields.Length - 1) As Int32
		'	'Dim fieldValues(_docFields.Length - 1) As String
		'	For Each docField In _docfields
		'		docField.Value = ""
		'	Next
		'	For Each docField In _docFields
		'		lineContainsErrors = lineContainsErrors Or SetFieldValueOrErrorMessage(docField, values(i), i)
		'		i += 1
		'		If values.Length - 1 < i Then
		'			Exit For
		'		End If
		'	Next
		'	For i = 0 To _docFields.Length - 1
		'		docFields(i) = New DocumentField(_docFields(i))
		'		docFields(i).Value = _docFields(i).Value
		'	Next
		'	If _errorsOnly Then
		'		If lineContainsErrors Then
		'			Return docFields
		'		Else
		'			Return Nothing
		'		End If
		'	Else
		'		Return docFields
		'	End If
		'End Function

		Private Function SetFieldValueOrErrorMessage(ByVal field As DocumentField, ByVal value As String, ByVal column As Int32) As Boolean
			Try
				SetFieldValue(field, value, column, True, "")
				Return False
			Catch ex As kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
				field.Value = ex.Message
				Return True
			End Try
		End Function

		Private Sub _processController_HaltProcessEvent(ByVal processID As System.Guid) Handles _processController.HaltProcessEvent
			_continue = False
		End Sub

		Protected Overrides ReadOnly Property UseTimeZoneOffset() As Boolean
			Get
				Return False
			End Get
		End Property

		Protected Overrides Function GetSingleCodeValidator() As CodeValidator.Base
			Return New CodeValidator.SinglePreviewer(_settings.CaseInfo, _codeManager)
		End Function
	End Class
End Namespace

