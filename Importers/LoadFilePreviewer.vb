'Imports kCura.EDDS.DynamicFields
Imports System.IO
Imports kCura.WinEDDS.Api
Imports kCura.Utility

Namespace kCura.WinEDDS
	Public Class LoadFilePreviewer
		Inherits kCura.WinEDDS.LoadFileBase


#Region "Members"
		Private _errorsOnly As Boolean
		Private WithEvents _processController As kCura.Windows.Process.Controller
		Private _continue As Boolean = True
		Private _columnCount As Int32 = 0
		Private _nativeFileCheckColumnName As String = ""
		Private _selectedCaseArtifactID As Int32
		Public Shared extractedTextEncodingFieldName As String = "Extracted Text Encoding"
		Private _relationalDocumentFields As DocumentField()
		Private _processedIdentifiers As New Collections.Specialized.NameValueCollection
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

		Public Function ReadFile(ByVal path As String, ByVal formType As Int32) As Object
			Dim earlyexit As Boolean = False

			_relationalDocumentFields = _fieldQuery.RetrieveAllAsDocumentFieldCollection(_selectedCaseArtifactID, _artifactTypeID).GetFieldsByCategory(Relativity.FieldCategory.Relational)
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
				If fieldArrays.Count < kCura.WinEDDS.Config.PREVIEW_THRESHOLD Then
					Try
						Dim record As Api.ArtifactFieldCollection = _artifactReader.ReadArtifact
						If Not _firstLineContainsColumnNames AndAlso fieldArrays.Count = 0 Then
							_columnCount = record.Count
						End If
						Dim x As Api.ArtifactField() = CheckLine(record, formType)
						'If Not x Is Nothing AndAlso Not (_firstLineContainsColumnNames AndAlso i = 0) Then fieldArrays.Add(x)
						If Not x Is Nothing Then fieldArrays.Add(x)
					Catch ex As ImporterExceptionBase
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


		Private Function CheckLine(ByVal record As Api.ArtifactFieldCollection, ByVal formType As Int32) As Api.ArtifactField()
			Dim mapItem As LoadFileFieldMap.LoadFileFieldMapItem
			Dim lineContainsErrors As Boolean = False
			Dim retval As New Collections.ArrayList
			Dim identifierField As Api.ArtifactField
			Dim unmappedRelationalNoBlankFields As New System.Collections.Specialized.HybridDictionary
			Dim mappedRelationalNoBlankFields As New System.Collections.Specialized.HybridDictionary
			For Each relationalField As DocumentField In _relationalDocumentFields
				If relationalField.ImportBehavior.HasValue Then
					If relationalField.ImportBehavior = EDDS.WebAPI.DocumentManagerBase.ImportBehaviorChoice.ReplaceBlankValuesWithIdentifier Then
						unmappedRelationalNoBlankFields.Add(relationalField.FieldID, New Api.ArtifactField(relationalField))
					End If
				End If
			Next
			If _keyFieldID > 0 Then
				identifierField = record(_keyFieldID)
			Else
				identifierField = record.IdentifierField
			End If
			Dim codePageId As Int32 = -1
			For Each mapItem In _fieldMap
				If mapItem.NativeFileColumnIndex > -1 AndAlso Not mapItem.DocumentField Is Nothing Then
					Dim field As Api.ArtifactField = record(mapItem.DocumentField.FieldID)
					If Not (_artifactTypeID <> Relativity.ArtifactType.Document And field.Type = Relativity.FieldTypeHelper.FieldType.File) Then
						Select Case field.Category
							Case Relativity.FieldCategory.Relational
								If unmappedRelationalNoBlankFields.Contains(field.ArtifactID) Then
									unmappedRelationalNoBlankFields.Remove(field.ArtifactID)
								End If
								If Not mappedRelationalNoBlankFields.Contains(field.ArtifactID) Then
									mappedRelationalNoBlankFields.Add(field.ArtifactID, field)
								End If
							Case Relativity.FieldCategory.Identifier
								If Not _keyFieldID > 0 Then
									identifierField = field
								End If
						End Select

						lineContainsErrors = lineContainsErrors Or SetFieldValueOrErrorMessage(field, mapItem.NativeFileColumnIndex, identifierField.ValueAsString, codePageId, mapItem.DocumentField.ImportBehavior)
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
					identifierField.Value = New Exceptions.ErrorMessage(String.Format("Error: The identifier '{0}' has been previously proccessed on line {1}.", identifierField.Value.ToString, _processedIdentifiers(identifierField.Value.ToString)))
					lineContainsErrors = True
				End If
			End If

			' only do this if we are NOT in Overlay mode
			If _settings.OverwriteDestination.ToLower <> ImportOverwriteModeEnum.Overlay.ToString.ToLower Then
				If Not identifierField Is Nothing And _artifactTypeID = Relativity.ArtifactType.Document Then
					For Each field As Api.ArtifactField In unmappedRelationalNoBlankFields.Values
						If record.IdentifierField IsNot Nothing Then
							field.Value = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(Me.GetNullableFixedString(record.IdentifierField.ValueAsString, -1, field.TextLength, field.DisplayName))
						End If
						lineContainsErrors = lineContainsErrors Or SetFieldValueOrErrorMessage(field, -1, identifierField.ValueAsString, -1, Nothing)
						retval.Add(field)
					Next
				End If
			End If

			If _uploadFiles Then
				If _nativeFileCheckColumnName = "" Then Me.SetNativeFileCheckColumnName(retval)
				Dim filePath As String = record.FileField.ValueAsString
				Dim existsFilePath As String = Nothing
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
					record.FileField.Value = New Exceptions.ErrorMessage(String.Format("Error: file '{0}' does not exist", filePath))
					lineContainsErrors = True
				Else
					record.FileField.Value = filePath
				End If
				retval.Add(record.FileField)
			End If

			If _createFolderStructure Then
				Dim field As Api.ArtifactField = record.FieldList(Relativity.FieldCategory.ParentArtifact)(0)
				If _artifactTypeID <> Relativity.ArtifactType.Document Then
					If field.ValueAsString = String.Empty Then
						field.Value = New Exceptions.ErrorMessage(New ParentObjectReferenceRequiredException(Me.CurrentLineNumber, -1).Message)
						lineContainsErrors = True
					Else
						Dim parentObjectTable As System.Data.DataTable = _objectManager.RetrieveArtifactIdOfMappedParentObject(_caseArtifactID, field.ValueAsString, _artifactTypeID).Tables(0)
						If parentObjectTable.Rows.Count > 1 Then
							field.Value = New Exceptions.ErrorMessage(New DuplicateObjectReferenceException(Me.CurrentLineNumber, -1, "Parent Info").Message)
							lineContainsErrors = True
						End If
					End If
					field.DisplayName = "Parent Object Identifier"
				Else
					field.DisplayName = "Parent Folder Identifier"
				End If
				retval.Add(field)
			End If
			If _settings.FullTextColumnContainsFileLocation AndAlso formType = 1 Then
				Dim field As New Api.ArtifactField(extractedTextEncodingFieldName, -500, 0, 0, Nothing, Nothing, Nothing, False)
				If codePageId > 0 Then
					field.Value = System.Text.Encoding.GetEncoding(codePageId).EncodingName
				Else
					field.Value = String.Empty
				End If
				retval.Add(field)
			End If

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

		Private Function SetFieldValueOrErrorMessage(ByVal field As Api.ArtifactField, ByVal column As Int32, ByVal identityValue As String, ByRef extractedTextCodePageId As Int32, ByVal importBehavior As EDDS.WebAPI.DocumentManagerBase.ImportBehaviorChoice?) As Boolean
			Try
				SetFieldValue(field, column, True, identityValue, extractedTextCodePageId, importBehavior)
				Return TypeOf field.Value Is System.Exception
			Catch ex As ImporterExceptionBase
				field.Value = New Exceptions.ErrorMessage(ex.Message)
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

