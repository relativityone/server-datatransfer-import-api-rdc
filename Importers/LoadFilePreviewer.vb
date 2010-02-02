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

		Public Function ReadFile(ByVal path As String) As Object
			Dim earlyexit As Boolean = False
			_relationalDocumentFields = _fieldQuery.RetrieveAllAsDocumentFieldCollection(_selectedCaseArtifactID, _artifactTypeID).GetFieldsByCategory(DynamicFields.Types.FieldCategory.Relational)
			Dim filesize As Int64 = _artifactReader.SizeInBytes
			Dim stepsize As Int64 = CType(filesize / 100, Int64)
			ProcessStart(0, filesize, stepsize)
			Dim fieldArrays As New System.Collections.ArrayList
			_columnHeaders = _artifactReader.GetColumnNames(_settings)
			_columnCount = _columnHeaders.Length
			If _firstLineContainsColumnNames Then
				If _uploadFiles Then
					Dim openParenIndex As Int32 = _filePathColumn.LastIndexOf("("c) + 1
					Dim closeParenIndex As Int32 = _filePathColumn.LastIndexOf(")"c)
					_filePathColumnIndex = Int32.Parse(_filePathColumn.Substring(openParenIndex, closeParenIndex - openParenIndex)) - 1
				End If
				'_filePathColumnIndex = Array.IndexOf(_columnHeaders, _filePathColumn)
				If _artifactReader.HasMoreRecords Then _artifactReader.AdvanceRecord()
			Else
				If _uploadFiles Then
					_filePathColumnIndex = Int32.Parse(_filePathColumn.Replace("Column", "").Replace("(", "").Replace(")", "").Trim) - 1
				End If
			End If
			Dim i As Int32 = 0
			i = 0
			While _artifactReader.HasMoreRecords AndAlso _continue
				If fieldArrays.Count < 1000 Then
					Try
						Dim record As Api.ArtifactFieldCollection = _artifactReader.ReadArtifact
						If Not _firstLineContainsColumnNames AndAlso fieldArrays.Count = 0 Then
							_columnCount = record.Count
						End If
						Dim x As Api.ArtifactField() = CheckLine(record)
						'If Not x Is Nothing AndAlso Not (_firstLineContainsColumnNames AndAlso i = 0) Then fieldArrays.Add(x)
						If Not x Is Nothing Then fieldArrays.Add(x)
					Catch ex As kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
						fieldArrays.Add(ex)
					End Try
					i += 1
					If i Mod 100 = 0 Then ProcessProgress(_artifactReader.BytesProcessed, filesize, stepsize)
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
			_artifactReader.Close()
			Return fieldArrays
		End Function


		Private Function CheckLine(ByVal record As Api.ArtifactFieldCollection) As Api.ArtifactField()
			Dim mapItem As LoadFileFieldMap.LoadFileFieldMapItem
			Dim lineContainsErrors As Boolean = False
			Dim retval As New ArrayList
			Dim artifactfield As Api.ArtifactField
			Dim identifierField As Api.ArtifactField
			Dim unmappedFields As New System.Collections.Specialized.HybridDictionary
			Dim mappedFields As New System.Collections.Specialized.HybridDictionary
			Dim idFieldColumnIndex As Int32
			Dim idFieldOriginalValue As String
			For Each relationalField As DocumentField In _relationalDocumentFields
				unmappedFields.Add(relationalField.FieldID, New Api.ArtifactField(relationalField))
			Next
			If _keyFieldID > 0 Then
				identifierField = record(_keyFieldID)
			Else
				identifierField = record.IdentifierField
			End If
			For Each mapItem In _fieldMap
				If mapItem.NativeFileColumnIndex > -1 AndAlso Not mapItem.DocumentField Is Nothing Then
					Dim field As Api.ArtifactField = record(mapItem.DocumentField.FieldID)
					If Not (_artifactTypeID <> kCura.EDDS.Types.ArtifactType.Document And field.Type = kCura.DynamicFields.Types.FieldTypeHelper.FieldType.File) Then
						Select Case field.Category
							Case kCura.DynamicFields.Types.FieldCategory.Relational
								If unmappedFields.Contains(field.ArtifactID) Then
									unmappedFields.Remove(field.ArtifactID)
								End If
								If Not mappedFields.Contains(field.ArtifactID) Then
									mappedFields.Add(field.ArtifactID, field)
								End If
							Case kCura.DynamicFields.Types.FieldCategory.Identifier
								If Not _keyFieldID > 0 Then identifierField = field
						End Select
						lineContainsErrors = lineContainsErrors Or SetFieldValueOrErrorMessage(field, mapItem.NativeFileColumnIndex, identifierField.ValueAsString)
						'dont add field if object type is not a document and the field is a file field
						retval.Add(field)
					End If
				End If
			Next

			If Not identifierField Is Nothing AndAlso Not identifierField.Value Is Nothing Then
				If _processedIdentifiers(identifierField.Value.ToString) Is Nothing Then
					_processedIdentifiers(identifierField.Value.ToString) = Me.CurrentLineNumber.ToString
				Else
					'Throw New IdentifierOverlapException(identifierField.Value, _processedIdentifiers(identifierField.Value))
					identifierField.Value = String.Format("Error: The identifier '{0}' has been previously proccessed on line {1}.", identifierField.Value.ToString, _processedIdentifiers(identifierField.Value.ToString))
					lineContainsErrors = True
				End If
			End If

			If Not identifierField Is Nothing And _artifactTypeID = kCura.EDDS.Types.ArtifactType.Document Then
				For Each field As Api.ArtifactField In unmappedFields.Values
					field.Value = identifierField.ValueAsString
					'field.Value = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(Me.GetNullableFixedString(record.IdentifierField.ValueAsString, -1, field.TextLength))
					lineContainsErrors = lineContainsErrors Or SetFieldValueOrErrorMessage(field, -1, identifierField.ValueAsString)
					retval.Add(field)
				Next
				For Each field As Api.ArtifactField In mappedFields.Values
					If field.ValueAsString = "" Then field.Value = identifierField.Value
				Next
			End If

			If _uploadFiles Then
				If _nativeFileCheckColumnName = "" Then Me.SetNativeFileCheckColumnName(retval)
				Dim filePath As String = record.FileField.ValueAsString
				Dim existsFilePath As String
				If filePath.Length > 1 Then
					If filePath.Chars(0) = "\"c AndAlso Not filePath.Chars(1) = "\" Then
						existsFilePath = "." & filePath
					Else
						existsFilePath = filePath
					End If
				End If
				If filePath = "" Then
					record.FileField.Value = "No File Specified."
				ElseIf Not System.IO.File.Exists(existsFilePath) Then
					record.FileField.Value = String.Format("Error: file '{0}' does not exist", filePath)
					lineContainsErrors = True
				Else
					record.FileField.Value = filePath
				End If
				retval.Add(record.FileField)
			End If

			If _createFolderStructure Then
				Dim field As Api.ArtifactField = record.FieldList(DynamicFields.Types.FieldCategory.ParentArtifact)(0)
				If _artifactTypeID <> kCura.EDDS.Types.ArtifactType.Document Then
					If field.ValueAsString = String.Empty Then
						field.Value = New ParentObjectReferenceRequiredException(Me.CurrentLineNumber, -1).Message
						lineContainsErrors = True
					Else
						Dim parentObjectTable As System.Data.DataTable = _objectManager.RetrieveArtifactIdOfMappedParentObject(_caseArtifactID, field.ValueAsString, _artifactTypeID).Tables(0)
						If parentObjectTable.Rows.Count > 1 Then
							field.Value = New DuplicateObjectReferenceException(Me.CurrentLineNumber, -1, "Parent Info").Message
							lineContainsErrors = True
						End If
					End If
					field.DisplayName = "Parent Object Identifier"
				Else
					field.DisplayName = "Parent Folder Identifier"
				End If
				retval.Add(field)
			End If

			'If _createFolderStructure AndAlso _artifactTypeID <> kCura.EDDS.Types.ArtifactType.Document Then
			'	Dim openParenIndex As Int32 = _destinationFolder.LastIndexOf("("c) + 1
			'	Dim closeParenIndex As Int32 = _destinationFolder.LastIndexOf(")"c)
			'	Dim parentObjectIdentifierIndex As Int32 = Int32.Parse(_destinationFolder.Substring(openParenIndex, closeParenIndex - openParenIndex)) - 1
			'	Dim docfield As New DocumentField("Parent Object Identifier", -1, -1, -1, NullableInt32.Null, NullableInt32.Null, False)
			'	docField.Value = values(parentObjectIdentifierIndex)
			'	Dim textIdentifier As String = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(kCura.Utility.NullableTypesHelper.ToNullableString(values(parentObjectIdentifierIndex)))
			'	If textIdentifier = "" Then
			'		docField.Value = New ParentObjectReferenceRequiredException(Me.CurrentLineNumber, parentObjectIdentifierIndex).Message
			'		lineContainsErrors = True
			'	Else
			'		Dim parentObjectTable As System.Data.DataTable = _objectManager.RetrieveArtifactIdOfMappedParentObject(_caseArtifactID, _
			'		textIdentifier, _artifactTypeID).Tables(0)
			'		If parentObjectTable.Rows.Count > 1 Then
			'			docField.Value = New DuplicateObjectReferenceException(Me.CurrentLineNumber, parentObjectIdentifierIndex, "Parent Info").Message
			'			lineContainsErrors = True
			'		End If
			'	End If
			'	retval.Add(docfield)
			'ElseIf _createFolderStructure AndAlso _artifactTypeID = kCura.EDDS.Types.ArtifactType.Document Then
			'	Dim openParenIndex As Int32 = _destinationFolder.LastIndexOf("("c) + 1
			'	Dim closeParenIndex As Int32 = _destinationFolder.LastIndexOf(")"c)
			'	Dim parentObjectIdentifierIndex As Int32 = Int32.Parse(_destinationFolder.Substring(openParenIndex, closeParenIndex - openParenIndex)) - 1
			'	Dim docfield As New DocumentField("Parent_Folder_Identifier", -1, -1, -1, NullableInt32.Null, NullableInt32.Null, False)
			'	docField.Value = values(parentObjectIdentifierIndex)
			'	retval.Add(docfield)
			'End If

			If _errorsOnly Then
				If lineContainsErrors Then
					Return DirectCast(retval.ToArray(GetType(Api.ArtifactField)), Api.ArtifactField())
				Else
					Return Nothing
				End If
			Else
				Return DirectCast(retval.ToArray(GetType(Api.ArtifactField)), Api.ArtifactField())
			End If
		End Function

		Private Sub SetNativeFileCheckColumnName(ByVal fields As System.Collections.ArrayList)
			Dim val As String = "Native File to Upload"
			Dim field As Api.ArtifactField
			Dim i As Int32
			For i = 0 To 100
				Dim isNameInUse As Boolean = False
				For Each field In fields
					If val = field.DisplayName Then
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

		Private Function SetFieldValueOrErrorMessage(ByVal field As Api.ArtifactField, ByVal column As Int32, ByVal identityValue As String) As Boolean
			Try
				SetFieldValue(field, column, True, identityValue)
				Return TypeOf field.Value Is System.Exception
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

		Protected Overrides Function GetArtifactReader() As Api.IArtifactReader
			Return New LoadFileReader(_settings, True)
		End Function
	End Class
End Namespace

