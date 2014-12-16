Imports System.Collections.Generic
Imports kCura.WinEDDS.Api
Imports kCura.WinEDDS
Imports kCura.Utility
Imports System.Xml.Linq
Imports System.Linq
Imports System.Xml.XPath

Namespace kCura.WinEDDS.ImportExtension
	Public Class DataReaderReader

		Implements kCura.WinEDDS.Api.IArtifactReader


		Private Const _KCURAMARKERFILENAME As String = "kcuramarkerfilename"

		Private _reader As System.Data.IDataReader
		Private ReadOnly _FileSettings As FileSettings
		Private _loadFileSettings As kCura.WinEDDS.LoadFile
		Protected _currentLineNumber As Long = 0
		Private _size As Long = -1
		Private _columnNames As String()
		Private _allFields As Api.ArtifactFieldCollection
		Private _tempLocalDirectory As String
		Private _markerNameIsMapped As Nullable(Of Boolean)
		Private _identifierFieldIndex As Integer
		Private _lastSourceIdentifier As String

		Public Sub New(ByVal args As DataReaderReaderInitializationArgs, ByVal fieldMap As kCura.WinEDDS.LoadFile, ByVal reader As System.Data.IDataReader)
			Me.New(args, fieldMap, reader, New FileSettings() With {.OIFileIdMapped = False, .FileSizeMapped = False})
		End Sub

		Public Sub New(ByVal args As DataReaderReaderInitializationArgs, ByVal fieldMap As kCura.WinEDDS.LoadFile, ByVal reader As System.Data.IDataReader, fileSettings As FileSettings)
			_reader = reader
			_FileSettings = FileSettings
			If _reader Is Nothing Then Throw New NullReferenceException("The reader being passed into this IDataReaderReader is null")
			If _reader.IsClosed = True OrElse _reader.FieldCount = 0 Then Throw New ArgumentException("The reader being passed into this IDataReaderReader is empty")
			_loadFileSettings = fieldMap
			_allFields = args.AllFields
			If args.TemporaryLocalDirectory Is Nothing Then
				_tempLocalDirectory = System.IO.Path.GetTempPath + "FlexMigrationFiles\"
			Else
				_tempLocalDirectory = args.TemporaryLocalDirectory
			End If

			If Not fieldMap Is Nothing Then
				For i As Integer = 0 To reader.FieldCount - 1
					If reader.GetName(i).Equals(fieldMap.SelectedIdentifierField.FieldName, StringComparison.CurrentCultureIgnoreCase) Then
						_identifierFieldIndex = i
						Exit For
					End If
				Next
			Else
				_identifierFieldIndex = -1
			End If
		End Sub

		Public Property TemporaryLocalDirectory As String
			Get
				Return _tempLocalDirectory
			End Get
			Set(value As String)
				_tempLocalDirectory = value
			End Set
		End Property

#Region " Artifact Reader Implementation "

		Public Function SourceIdentifierValue() As String Implements IArtifactReader.SourceIdentifierValue
			Return _lastSourceIdentifier
		End Function

		Public Sub AdvanceRecord() Implements kCura.WinEDDS.Api.IArtifactReader.AdvanceRecord
			If _reader.Read() Then
				_currentLineNumber += 1
			Else
				_currentLineNumber += 1
				_reader.Close()
			End If

		End Sub

		Public ReadOnly Property BytesProcessed() As Long Implements kCura.WinEDDS.Api.IArtifactReader.BytesProcessed
			Get
				'If Me.SizeInBytes < 1 OrElse _reader.Rows.Count = 0 Then
				If Me.SizeInBytes < 1 OrElse _reader.IsClosed = True Then
					Return 0
				Else
					'Return CType((_currentLineNumber / _reader.Rows.Count) * Me.SizeInBytes, Long)
					Return CType((_currentLineNumber / 1) * Me.SizeInBytes, Long)
				End If
			End Get
		End Property

		Public Function CountRecords() As Long Implements kCura.WinEDDS.Api.IArtifactReader.CountRecords
			'Return _reader.Rows.Count
			Return 1
		End Function

		Public ReadOnly Property CurrentLineNumber() As Integer Implements kCura.WinEDDS.Api.IArtifactReader.CurrentLineNumber
			Get
				Return CType(_currentLineNumber, Int32)
			End Get
		End Property

		Public Function GetColumnNames(ByVal args As Object) As String() Implements kCura.WinEDDS.Api.IArtifactReader.GetColumnNames
			If _columnNames Is Nothing Then
				Dim retval As New System.Collections.ArrayList
				For iFieldIndex As Integer = 0 To _reader.FieldCount - 1
					retval.Add(_reader.GetName(iFieldIndex))
				Next
				_columnNames = DirectCast(retval.ToArray(GetType(String)), String())
			End If
			Return _columnNames
		End Function

		Public ReadOnly Property HasMoreRecords() As Boolean Implements kCura.WinEDDS.Api.IArtifactReader.HasMoreRecords
			Get
				Return Not (_reader.IsClosed)
			End Get
		End Property

		Private Function DoesFilenameMarkerFieldExistInIDataReader() As Boolean
			If Not _markerNameIsMapped.HasValue Then
				Try
					Dim isValidItemName As Integer = _reader.GetOrdinal(_KCURAMARKERFILENAME)
					_markerNameIsMapped = True
				Catch ex As IndexOutOfRangeException
					_markerNameIsMapped = False
				Catch ex As ArgumentException
					_markerNameIsMapped = False
				End Try
			End If
			Return _markerNameIsMapped.Value
		End Function

		Private Function nameWithoutIndex(ByVal nameWithIndex As String) As String
			If nameWithIndex Is Nothing Then
				Return Nothing
			End If
			Dim openParenIndex As Int32 = nameWithIndex.LastIndexOf("("c)
			If openParenIndex >= 0 Then
				Return nameWithIndex.Substring(0, openParenIndex)
			Else
				Return nameWithIndex
			End If
		End Function

		Public Function ReadArtifact() As kCura.WinEDDS.Api.ArtifactFieldCollection Implements kCura.WinEDDS.Api.IArtifactReader.ReadArtifact
			Try
				Return ReadArtifactData()
			Catch ex As Exception
				Throw
			Finally
				AdvanceRecord()
			End Try
		End Function

		Private Function ReadArtifactData() As kCura.WinEDDS.Api.ArtifactFieldCollection
			Dim retval As Api.ArtifactFieldCollection
			Dim oiFileType As String = ""
			Dim oiFileId As Int32
			If _FileSettings.OIFileIdMapped Then
				For i As Integer = 0 To _reader.FieldCount - 1
					If (_reader.GetName(i) = _FileSettings.TypeColumnName) Then
						oiFileType = _reader.GetValue(i).ToString()
					ElseIf (_reader.GetName(i) = _FileSettings.IDColumnName) Then
						Dim value As Int32 = -1
						Dim readerValue As String = _reader.GetValue(i).ToString()
						Int32.TryParse(readerValue, value)
						oiFileId = value
					End If
				Next
			End If
			Dim fileSize As Nullable(Of Long) = Nothing
			If _FileSettings.FileSizeMapped Then
				For i As Integer = 0 To _reader.FieldCount - 1
					If (_reader.GetName(i) = _FileSettings.FileSizeColumn) Then
						Dim value As Long = -1
						Dim readerValue As String = _reader.GetValue(i).ToString()
						Long.TryParse(readerValue, value)
						fileSize = value
						Exit For
					End If
				Next
			End If
			'TODO: Factory when I have time
			If (Not String.IsNullOrEmpty(oiFileType)) Then
				If (fileSize.HasValue) Then
					retval = New NativeFilePopulatedArtifactFieldCollection() With {.OixFileID = New FileIDData(oiFileId, oiFileType), .FileSize = fileSize.GetValueOrDefault()}
				Else
					retval = New NativeFileValidatedArtifactFieldCollection() With {.OixFileID = New FileIDData(oiFileId, oiFileType)}
				End If
			Else
				If (fileSize.HasValue) Then
					retval = New FileSizePopulatedArtifactFieldCollection() With {.FileSize = fileSize.GetValueOrDefault()}
				Else
					retval = New Api.ArtifactFieldCollection
				End If
			End If
			Dim folderStructureContainedInColumnWithoutIndex As String = nameWithoutIndex(_loadFileSettings.FolderStructureContainedInColumn)

			' step 1 - save off the identifier for the current record
			If _identifierFieldIndex > -1 Then
				_lastSourceIdentifier = _reader.Item(_identifierFieldIndex).ToString()
			End If
			For i As Integer = 0 To _reader.FieldCount - 1
				Dim field As Api.ArtifactField = _allFields(_reader.GetName(i).ToLower())
				If Not field Is Nothing Then
					If Not field.DisplayName = folderStructureContainedInColumnWithoutIndex Then
						Dim thisCell As Api.ArtifactField = field.Copy
						SetFieldValueInvoker(i, thisCell, field.DisplayName)
						retval.Add(thisCell)
					End If
				End If
			Next

			'NativeFilePathColumn is in the format  displayname(index).  _reader only has name.  I need to get the index, get _reader.name, and add a field with the data.
			If _loadFileSettings.ArtifactTypeID = Relativity.ArtifactType.Document Then
				If _loadFileSettings.LoadNativeFiles AndAlso Not _loadFileSettings.NativeFilePathColumn Is Nothing AndAlso Not _loadFileSettings.NativeFilePathColumn = String.Empty Then
					Dim nativeFileIndex As Int32 = Int32.Parse(_loadFileSettings.NativeFilePathColumn.Substring(_loadFileSettings.NativeFilePathColumn.LastIndexOf("(")).Trim("()".ToCharArray))
					Dim displayName As String = _reader.GetName(nativeFileIndex - 1)
					Dim field As New Api.ArtifactField(New DocumentField(displayName, -1, Relativity.FieldTypeHelper.FieldType.File, Relativity.FieldCategory.FileInfo, New Nullable(Of Int32)(Nothing), New Nullable(Of Int32)(Nothing), New Nullable(Of Int32)(Nothing), True, EDDS.WebAPI.DocumentManagerBase.ImportBehaviorChoice.LeaveBlankValuesUnchanged, False))
					SetFieldValueInvoker(nativeFileIndex - 1, field, displayName)
					retval.Add(field)
				End If
			End If

			If Not _loadFileSettings.DataGridIDColumn Is Nothing AndAlso Not _loadFileSettings.DataGridIDColumn = String.Empty Then
				Dim dataGridIndex As Int32 = Int32.Parse(_loadFileSettings.DataGridIDColumn.Substring(_loadFileSettings.DataGridIDColumn.LastIndexOf("(")).Trim("()".ToCharArray))
				Dim displayName As String = _reader.GetName(dataGridIndex - 1)
				Dim field As New Api.ArtifactField(New kCura.EDDS.WebAPI.DocumentManagerBase.Field() With {.ArtifactID = -3, .DisplayName = "DataGridID"})
				SetFieldValueInvoker(dataGridIndex - 1, field, displayName)
				retval.Add(field)
			End If

			If _loadFileSettings.CopyFilesToDocumentRepository = True Then
				If Not System.IO.Directory.Exists(_tempLocalDirectory) Then System.IO.Directory.CreateDirectory(_tempLocalDirectory)

				If DoesFilenameMarkerFieldExistInIDataReader() Then
					'When importing files, we copy to a local directory using the filename in kCuraMarkerFilename before actual import.
					' we do not do this when we are pointing to files using links.
					' we also don't do this if kCuraMarkerFilename field is not present because we can copy from the current location 
					For Each field As Api.ArtifactField In retval
						If field.Type = Relativity.FieldTypeHelper.FieldType.File Then
							If field.ValueAsString <> String.Empty Then
								If System.IO.File.Exists(field.ValueAsString) Then

									Dim newLocation As String
									Try
										'If _KCURAMARKERFILENAME is a column in the table, use it for the filename.  If not, use original filename.
										Dim tempString As String = _reader.Item(_KCURAMARKERFILENAME).ToString
										newLocation = System.IO.Path.Combine(_tempLocalDirectory, _reader.Item(_KCURAMARKERFILENAME).ToString())
									Catch ex As Exception
										newLocation = System.IO.Path.Combine(_tempLocalDirectory, System.IO.Path.GetFileName(field.ValueAsString))
									End Try
									If System.IO.File.Exists(newLocation) Then
										'Import API file access denied when importing read only files
										Try
											kCura.Utility.File.Instance.Delete(newLocation)
										Catch ex As Exception
											'This is to make sure we don't have buggy data.  Clients that have run the line that sets attributes below will never hit this line.
											'However, clients on old versions of ImportAPI may hit this line and we want to make sure we're accounting for our mistake.
											System.IO.File.SetAttributes(newLocation, System.IO.FileAttributes.Normal)
											kCura.Utility.File.Instance.Delete(newLocation)
										End Try
									End If
									System.IO.File.Copy(field.ValueAsString, newLocation)
									'Set the attributes to Normal so we can delete the file if it's read only
									System.IO.File.SetAttributes(newLocation, System.IO.FileAttributes.Normal)
									field.Value = newLocation
									'Only one file field is allowed
									Exit For
								Else
									' [Salesforce:84392]
									' We used to throw an exception here, but it caused the import to be aborted
									' so right now we silently ignore this error and this error will be caught by
									' BulkLoadFileImporter.ManageDocuments. This is consistent with other
									' implementations of IArtifactReader (see LoadFileReader.ReadArtifact).
								End If
							End If
						End If
					Next
				End If
			End If

			'Added by Nick 12/23 to fix parent/child object problem
			If _loadFileSettings.CreateFolderStructure AndAlso Not _loadFileSettings.FolderStructureContainedInColumn Is Nothing AndAlso Not _loadFileSettings.FolderStructureContainedInColumn = String.Empty Then
				Dim parentIndex As Int32 = Int32.Parse(_loadFileSettings.FolderStructureContainedInColumn.Substring(_loadFileSettings.FolderStructureContainedInColumn.LastIndexOf("(")).Trim("()".ToCharArray))
				Dim displayName As String = _reader.GetName(parentIndex - 1)
				Dim field As New Api.ArtifactField(displayName, -2, Relativity.FieldTypeHelper.FieldType.Object, Relativity.FieldCategory.ParentArtifact, New Nullable(Of Int32)(Nothing), New Nullable(Of Int32)(255), New Nullable(Of Int32)(Nothing), False)
				SetFieldValueInvoker(parentIndex - 1, field, displayName)
				retval.Add(field)
			End If

			If Me.CurrentLineNumber = 0 Then
				DisplayFieldMap(_reader, retval)
			End If


			Return retval
		End Function

		' Extracted from the middle of ReadArtifact() for testing purposes
		Protected Sub SetFieldValueInvoker(ByVal idx As Integer, ByVal field As ArtifactField, ByVal displayName As String)
			Try
				Me.SetFieldValue(field, _reader.Item(idx))
			Catch ex As Exception
				' for ease of message formatting, the line number is a one-based number
				Throw New Exceptions.FieldValueImportException(ex, _currentLineNumber + 1, displayName, ex.Message)
			End Try
		End Sub

		Private Sub DisplayFieldMap(ByVal sourceData As System.Data.IDataReader, ByVal destination As kCura.WinEDDS.Api.ArtifactFieldCollection)
			'TODO: fix thsi display.  something with uppercase/lowercase, I think.
			'Display a list field mappings and unmapped fields

			Const SPECIALFILENAME As String = "*Filename*"
			Const NATIVEFILEPATH As String = "*NativeFilePath*"
			Const NOTMAPPED As String = "NOT MAPPED (Target field not found)"

			Dim mappedFields As String = String.Concat(vbCrLf, "----------", vbCrLf, "Field Mapping:", vbCrLf)
			Dim fieldMapRow As String = String.Concat("Source field [{0}] --> Destination field [{1}]", vbCrLf)
			For i As Integer = 0 To sourceData.FieldCount - 1
				Dim field As Api.ArtifactField = destination(_reader.GetName(i).ToLower)
				If field Is Nothing Then
					If _loadFileSettings.CopyFilesToDocumentRepository = True And _reader.GetName(i).ToLower = _KCURAMARKERFILENAME.ToLower Then
						'This is a special field.
						mappedFields = String.Concat(mappedFields, String.Format(fieldMapRow, _reader.GetName(i).ToLower, SPECIALFILENAME))
						RaiseEvent FieldMapped(_reader.GetName(i), SPECIALFILENAME)
					Else
						If Not (_loadFileSettings.NativeFilePathColumn Is Nothing) AndAlso _reader.GetName(i).ToLower = nameWithoutIndex(_loadFileSettings.NativeFilePathColumn.ToLower) Then
							'This is a special field
							mappedFields = String.Concat(mappedFields, String.Format(fieldMapRow, _reader.GetName(i).ToLower, NATIVEFILEPATH))
							RaiseEvent FieldMapped(_reader.GetName(i), NATIVEFILEPATH)
						Else
							mappedFields = String.Concat(mappedFields, String.Format(fieldMapRow, _reader.GetName(i).ToLower, NOTMAPPED))
							RaiseEvent FieldMapped(_reader.GetName(i), NOTMAPPED)

						End If
					End If
				Else
					mappedFields = String.Concat(mappedFields, String.Format(fieldMapRow, _reader.GetName(i).ToLower, field.DisplayName.ToLower))
					RaiseEvent FieldMapped(_reader.GetName(i), field.DisplayName)
				End If
			Next

			mappedFields = String.Concat(mappedFields, "----------", vbCrLf)
			RaiseEvent StatusMessage(mappedFields)
		End Sub

		Private Function NonTextField(ByVal fieldType As Relativity.FieldTypeHelper.FieldType) As Boolean
			Select Case fieldType
				Case Relativity.FieldTypeHelper.FieldType.Boolean, Relativity.FieldTypeHelper.FieldType.Currency,
				 Relativity.FieldTypeHelper.FieldType.Decimal, Relativity.FieldTypeHelper.FieldType.Date,
				 Relativity.FieldTypeHelper.FieldType.Integer
					Return True
				Case Else
					Return False
			End Select

		End Function


		Protected Sub SetFieldValue(ByVal field As Api.ArtifactField, ByVal value As Object)

			If value Is Nothing Then
				field.Value = Nothing
			Else
				If TypeOf value Is String And NonTextField(field.Type) Then
					' we have a mismatch - string data passed in
					Dim stringValue As String = CType(value, String)
					SetNativeFieldFromTextValue(field, stringValue)
				Else
					SetNativeFieldValue(field, value)

				End If
			End If
		End Sub


		Private Sub SetNativeFieldFromTextValue(ByVal field As ArtifactField, ByVal value As String)
			If value.Trim.Length = 0 Then
				field.Value = Nothing
			Else
				Select Case field.Type
					Case Relativity.FieldTypeHelper.FieldType.Boolean
						field.Value = kCura.Utility.NullableTypesEnhanced.GetNullableBoolean(value)

					Case Relativity.FieldTypeHelper.FieldType.Currency, Relativity.FieldTypeHelper.FieldType.Decimal
						field.Value = kCura.Utility.NullableTypesHelper.ToNullableDecimal(value.Trim)

					Case Relativity.FieldTypeHelper.FieldType.Date
						field.Value = kCura.Utility.NullableTypesEnhanced.GetNullableDateTime(value)

					Case Relativity.FieldTypeHelper.FieldType.Integer
						field.Value = NullableTypesHelper.ToNullableInt32(value.Replace(",", ""))

					Case Else
						Throw New System.ArgumentException("Unsupported field type '" & field.Type.ToString & "'")
				End Select
			End If
		End Sub

		Private Sub SetNativeFieldValue(ByVal field As ArtifactField, ByVal value As Object)

			'RaiseEvent StatusMessage("Field ArtifactID = " & field.ArtifactID)
			Select Case field.Type
				Case Relativity.FieldTypeHelper.FieldType.Boolean
					field.Value = kCura.Utility.NullableTypesHelper.DBNullConvertToNullable(Of Boolean)(value)
				Case Relativity.FieldTypeHelper.FieldType.Code
					field.Value = kCura.Utility.NullableTypesHelper.DBNullString(value)
				Case Relativity.FieldTypeHelper.FieldType.Text, Relativity.FieldTypeHelper.FieldType.OffTableText
					field.Value = kCura.Utility.NullableTypesHelper.DBNullString(value)
				Case Relativity.FieldTypeHelper.FieldType.User
					field.Value = kCura.Utility.NullableTypesHelper.DBNullString(value)
				Case Relativity.FieldTypeHelper.FieldType.Varchar
					field.Value = kCura.Utility.NullableTypesHelper.DBNullString(value)
				Case Relativity.FieldTypeHelper.FieldType.Object
					field.Value = kCura.Utility.NullableTypesHelper.DBNullString(value)
				Case Relativity.FieldTypeHelper.FieldType.Currency, Relativity.FieldTypeHelper.FieldType.Decimal
					field.Value = kCura.Utility.NullableTypesHelper.DBNullConvertToNullable(Of Decimal)(value)
				Case Relativity.FieldTypeHelper.FieldType.Date
					field.Value = kCura.Utility.NullableTypesHelper.DBNullConvertToNullable(Of System.DateTime)(value)
				Case Relativity.FieldTypeHelper.FieldType.File
					field.Value = kCura.Utility.NullableTypesHelper.DBNullString(value)
					'field.Value = value.ToString
				Case Relativity.FieldTypeHelper.FieldType.Integer
					field.Value = kCura.Utility.NullableTypesHelper.DBNullConvertToNullable(Of Int32)(value)
					'field.Value = kCura.Utility.NullableTypesHelper.ToNullableInt32(value)
				Case Relativity.FieldTypeHelper.FieldType.MultiCode, Relativity.FieldTypeHelper.FieldType.Objects
					field.Value = LoadFileReader.GetStringArrayFromDelimitedFieldValue(value, _loadFileSettings.MultiRecordDelimiter)
				Case Else
					Throw New System.ArgumentException("Unsupported field type '" & field.Type.ToString & "'")
			End Select
		End Sub


		Public ReadOnly Property SizeInBytes() As Long Implements kCura.WinEDDS.Api.IArtifactReader.SizeInBytes
			Get
				If _size = -1 Then
					Dim sw As New System.IO.MemoryStream
					Dim serializer As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
					serializer.Serialize(sw, _reader)
					_size = sw.Length
					sw.Close()
				End If
				Return _size
			End Get
		End Property

#Region " Events "

		Public Event StatusMessage(ByVal message As String) Implements kCura.WinEDDS.Api.IArtifactReader.StatusMessage
		Public Event FieldMapped(ByVal sourceField As String, ByVal workspaceField As String) Implements IArtifactReader.FieldMapped
		Public Event DataSourcePrep(ByVal e As kCura.WinEDDS.Api.DataSourcePrepEventArgs) Implements kCura.WinEDDS.Api.IArtifactReader.DataSourcePrep
		Public Event OnIoWarning(ByVal e As kCura.WinEDDS.Api.IoWarningEventArgs) Implements kCura.WinEDDS.Api.IArtifactReader.OnIoWarning

#End Region

#Region " Implemented but empty "

		Public Sub Halt() Implements kCura.WinEDDS.Api.IArtifactReader.Halt
		End Sub

		Public Sub Close() Implements kCura.WinEDDS.Api.IArtifactReader.Close
		End Sub

		Public Function ManageErrorRecords(ByVal errorMessageFileLocation As String, ByVal prePushErrorLineNumbersFileName As String) As String Implements kCura.WinEDDS.Api.IArtifactReader.ManageErrorRecords
			Return Nothing
		End Function

		Public Sub OnFatalErrorState() Implements kCura.WinEDDS.Api.IArtifactReader.OnFatalErrorState
		End Sub

#End Region

#End Region

	End Class

	Public Class FileSettings
		Public Property OIFileIdMapped() As Boolean
		Public Property IDColumnName() As String
		Public Property TypeColumnName() As String
		Public Property FileSizeMapped() As Boolean
		Public Property FileSizeColumn() As String
	End Class
End Namespace

