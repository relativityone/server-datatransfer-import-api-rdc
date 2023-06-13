Imports System.Collections.Generic
Imports kCura.WinEDDS.Api
Imports Relativity.DataExchange
Imports Relativity.DataExchange.Io
Imports Relativity.DataExchange.Process
Imports Relativity.DataExchange.Service

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

		Private _fileSettingsFileNameColumnIndex As Integer
		Private _fileSettingsFileSizeColumnIndex As Integer
		Private _fileSettingsTypeColumnNameIndex As Integer
		Private _fileSettingsIDColumnNameIndex As Integer
		Private _fileSupportedByViewerColumnNameIndex As Integer

		Public Sub New(ByVal args As DataReaderReaderInitializationArgs, ByVal fieldMap As kCura.WinEDDS.LoadFile, ByVal reader As System.Data.IDataReader)
			Me.New(args, fieldMap, reader, New FileSettings() With {.OIFileIdMapped = False, .FileSizeMapped = False, .FileNameColumn = Nothing})
		End Sub

		Public Sub New(ByVal args As DataReaderReaderInitializationArgs, ByVal fieldMap As kCura.WinEDDS.LoadFile, ByVal reader As System.Data.IDataReader, fileSettings As FileSettings)
			_reader = reader

			If (fileSettings Is Nothing) Then
				fileSettings = New FileSettings() With {.FileNameColumn = Nothing, .FileSizeColumn = Nothing, .FileSizeMapped = False, .IDColumnName = Nothing, .OIFileIdMapped = False, .TypeColumnName = Nothing}
			End If
			_FileSettings = fileSettings
			
			_reader.ThrowIfNull(nameof(_reader))
			If _reader.IsClosed = True OrElse _reader.FieldCount = 0 Then Throw New ArgumentException("The reader is closed or empty")
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

			_fileSettingsFileNameColumnIndex = -1
			_fileSettingsFileSizeColumnIndex = -1
			_fileSettingsTypeColumnNameIndex = -1
			_fileSettingsIDColumnNameIndex = -1
			_fileSupportedByViewerColumnNameIndex = -1
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
            Try
                If Not _reader.Read() Then
                    _reader.Close()
                End If
            Finally
                _currentLineNumber += 1
            End Try
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

		''' <summary>
		''' Returns total records count, but only when <see cref="_reader"/> already read all the records, otherwise returns null.
		''' This is because <see cref="_reader"/> is a forward-only record viewer and does not contain information about total number of rows.
		''' To calculate total number of records we need to iterate through all the rows and increment <see cref="_currentLineNumber"/>
		''' </summary>
		''' <returns>Total records count if <see cref="_reader"/> is closed, null otherwise</returns>
		Public Function CountRecords() As Long? Implements kCura.WinEDDS.Api.IArtifactReader.CountRecords
			If _reader.IsClosed Then
				Return CurrentLineNumber
			End If
			Return Nothing
		End Function

		Public ReadOnly Property CurrentLineNumber() As Integer Implements kCura.WinEDDS.Api.IArtifactReader.CurrentLineNumber
			Get
				Return CType(_currentLineNumber, Int32)
			End Get
		End Property

		Public Function GetColumnNames(ByVal args As Object) As String() Implements kCura.WinEDDS.Api.IArtifactReader.GetColumnNames
			If _columnNames Is Nothing Then
				_columnNames = Enumerable _
					.Range(0, _reader.FieldCount) _
					.Select(Function(fieldIndex) _reader.GetName(fieldIndex)) _
					.ToArray()
			End If

			Return _columnNames
		End Function

		Public Sub ValidateColumnNames(invalidNameAction As Action(Of String)) Implements IArtifactReader.ValidateColumnNames
			For Each invalidName As String In GetColumnNames(Nothing).Where(Function(columnName) _allFields(columnName.ToLower()) Is Nothing)
				invalidNameAction(invalidName)
			Next
		End Sub

		Public ReadOnly Property HasMoreRecords() As Boolean Implements kCura.WinEDDS.Api.IArtifactReader.HasMoreRecords
			Get
				Return Not (_reader.IsClosed)
			End Get
		End Property

		Private Function DoesFilenameMarkerFieldExistInIDataReader() As Boolean
			If Not _markerNameIsMapped.HasValue Then
				Try
					_reader.GetOrdinal(_KCURAMARKERFILENAME)
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
			Dim artifactFieldCollection As ArtifactFieldCollection = GetArtifactFieldCollectionWithBasicFields()

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
						artifactFieldCollection.Add(thisCell)
					End If
				End If
			Next

			'NativeFilePathColumn is in the format  displayname(index).  _reader only has name.  I need to get the index, get _reader.name, and add a field with the data.
			If _loadFileSettings.ArtifactTypeID = ArtifactType.Document Then
				If _loadFileSettings.LoadNativeFiles AndAlso Not _loadFileSettings.NativeFilePathColumn Is Nothing AndAlso Not _loadFileSettings.NativeFilePathColumn = String.Empty Then
					Dim nativeFileIndex As Int32 = Int32.Parse(_loadFileSettings.NativeFilePathColumn.Substring(_loadFileSettings.NativeFilePathColumn.LastIndexOf("(")).Trim("()".ToCharArray))
					Dim displayName As String = _reader.GetName(nativeFileIndex - 1)
					Dim field As New Api.ArtifactField(New DocumentField(displayName, -1, FieldType.File, FieldCategory.FileInfo, New Nullable(Of Int32)(Nothing), New Nullable(Of Int32)(Nothing), New Nullable(Of Int32)(Nothing), True, EDDS.WebAPI.DocumentManagerBase.ImportBehaviorChoice.LeaveBlankValuesUnchanged, False))
					SetFieldValueInvoker(nativeFileIndex - 1, field, displayName)
					artifactFieldCollection.Add(field)
				End If
			End If

			If Not _loadFileSettings.DataGridIDColumn Is Nothing AndAlso Not _loadFileSettings.DataGridIDColumn = String.Empty Then
				Dim dataGridIndex As Int32 = Int32.Parse(_loadFileSettings.DataGridIDColumn.Substring(_loadFileSettings.DataGridIDColumn.LastIndexOf("(")).Trim("()".ToCharArray))
				Dim displayName As String = _reader.GetName(dataGridIndex - 1)
				Dim field As New Api.ArtifactField(New kCura.EDDS.WebAPI.DocumentManagerBase.Field() With {.ArtifactID = -3, .DisplayName = BulkLoadFileImporter.DATA_GRID_ID_FIELD_NAME})
				SetFieldValueInvoker(dataGridIndex - 1, field, displayName)
				artifactFieldCollection.Add(field)
			End If

			If _loadFileSettings.CopyFilesToDocumentRepository = True Then
				If Not System.IO.Directory.Exists(_tempLocalDirectory) Then System.IO.Directory.CreateDirectory(_tempLocalDirectory)

				If DoesFilenameMarkerFieldExistInIDataReader() Then
					'When importing files, we copy to a local directory using the filename in kCuraMarkerFilename before actual import.
					' we do not do this when we are pointing to files using links.
					' we also don't do this if kCuraMarkerFilename field is not present because we can copy from the current location 
					For Each field As Api.ArtifactField In artifactFieldCollection
						If field.Type = FieldType.File Then
							If field.ValueAsString <> String.Empty Then
								If System.IO.File.Exists(field.ValueAsString) Then

									Dim newLocation As String
									Try
										'If _KCURAMARKERFILENAME is a column in the table, use it for the filename.  If not, use original filename.
										Dim kcuraMarkerFilename As String = _reader.Item(_KCURAMARKERFILENAME).ToString
										newLocation = System.IO.Path.Combine(_tempLocalDirectory, kcuraMarkerFilename)
									Catch ex As Exception
										newLocation = System.IO.Path.Combine(_tempLocalDirectory, System.IO.Path.GetFileName(field.ValueAsString))
									End Try
									If System.IO.File.Exists(newLocation) Then
										'Import API file access denied when importing read only files
										Try
											Global.Relativity.DataExchange.Io.FileSystem.Instance.File.Delete(newLocation)
										Catch ex As Exception
											'This is to make sure we don't have buggy data.  Clients that have run the line that sets attributes below will never hit this line.
											'However, clients on old versions of ImportAPI may hit this line and we want to make sure we're accounting for our mistake.
											System.IO.File.SetAttributes(newLocation, System.IO.FileAttributes.Normal)
											Global.Relativity.DataExchange.Io.FileSystem.Instance.File.Delete(newLocation)
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
				Dim field As New Api.ArtifactField(displayName, -2, FieldType.Object, FieldCategory.ParentArtifact, New Nullable(Of Int32)(Nothing), New Nullable(Of Int32)(255), New Nullable(Of Int32)(Nothing), False)
				SetFieldValueInvoker(parentIndex - 1, field, displayName)
				artifactFieldCollection.Add(field)
			End If

			If Me.CurrentLineNumber = 0 Then
				DisplayFieldMap(_reader, artifactFieldCollection)
			End If


			Return artifactFieldCollection
		End Function

		Private Function GetArtifactFieldCollectionWithBasicFields() As ArtifactFieldCollection
			Dim fileTypeInfo As FileTypeInfo = GetFileTypeIdInfo()
			Dim fileSizeSet As Long? = GetFileSizeData()
			Dim fileNameSet As String = GetFileNameData()

			Return InjectableArtifactFieldCollection.CreateFieldCollection(fileNameSet, fileTypeInfo, fileSizeSet)
		End Function

		Private Function GetFileNameData() As String

			Dim fileName As String = Nothing
			If (Not String.IsNullOrEmpty(_FileSettings.FileNameColumn)) Then
				If _fileSettingsFileNameColumnIndex = -1 Then
					For i As Integer = 0 To _reader.FieldCount - 1
						If (_reader.GetName(i) = _FileSettings.FileNameColumn) Then
							Dim fileNameValue As Object = _reader.GetValue(i)
							If (fileNameValue IsNot Nothing) Then
								fileName = fileNameValue.ToString()
							End If
							_fileSettingsFileNameColumnIndex = i
							Exit For
						End If
					Next
				Else
					If (_reader.GetName(_fileSettingsFileNameColumnIndex) = _FileSettings.FileNameColumn) Then
						Dim fileNameValue As Object = _reader.GetValue(_fileSettingsFileNameColumnIndex)
						If (fileNameValue IsNot Nothing) Then
							fileName = fileNameValue.ToString()
						End If
					End If
				End If
			End If
			
			If (Not String.IsNullOrEmpty(fileName)) Then
				Return fileName
			End If

			Return Nothing
		End Function

		Private Function GetFileSizeData() As Long?

			If Not _FileSettings.FileSizeMapped Then
				Return Nothing
			End If

			Dim fileSize As Nullable(Of Long) = Nothing

			If _fileSettingsFileSizeColumnIndex = -1 Then
				For i As Integer = 0 To _reader.FieldCount - 1
					If (_reader.GetName(i) = _FileSettings.FileSizeColumn) Then
						Dim value As Long = -1
						Dim readerValue As String = Convert.ToString(_reader.GetValue(i))
						Long.TryParse(readerValue, value)
						fileSize = value
						_fileSettingsFileSizeColumnIndex = i
						Exit For
					End If
				Next
			Else
				If (_reader.GetName(_fileSettingsFileSizeColumnIndex) = _FileSettings.FileSizeColumn) Then
					Dim value As Long = -1
					Dim readerValue As String = Convert.ToString(_reader.GetValue(_fileSettingsFileSizeColumnIndex))
					Long.TryParse(readerValue, value)
					fileSize = value
				End If
			End If

			If (fileSize.HasValue) Then
				Return fileSize
			End If

			Return Nothing
		End Function

		Private Function GetSupportedByViewerData() As Boolean?

			If String.IsNullOrEmpty(_FileSettings.SupportedByViewerColumn)
				Return Nothing
			End If

			Dim supportedByViewer As Nullable(Of Boolean) = Nothing

			If _fileSupportedByViewerColumnNameIndex = -1 Then
				For i As Integer = 0 To _reader.FieldCount - 1
					If (_reader.GetName(i) = _FileSettings.SupportedByViewerColumn) Then
						Dim value As Boolean = False
						Dim readerValue As String = Convert.ToString(_reader.GetValue(i))
						Boolean.TryParse(readerValue, value)
						supportedByViewer = value
						_fileSupportedByViewerColumnNameIndex = i
						Exit For
					End If
				Next
			Else
				If (_reader.GetName(_fileSupportedByViewerColumnNameIndex) = _FileSettings.SupportedByViewerColumn) Then
					Dim value As Boolean = False
					Dim readerValue As String = _reader.GetValue(_fileSupportedByViewerColumnNameIndex).ToString()
					Boolean.TryParse(readerValue, value)
					supportedByViewer = value
				End If
			End If

			If (supportedByViewer.HasValue) Then
				Return supportedByViewer
			End If

			Return Nothing
		End Function

		Private Function GetFileTypeIdInfo() As FileTypeInfo

			If Not _FileSettings.OIFileIdMapped
				Return Nothing
			End If

			Dim oiFileType As String = ""
			Dim oiFileId As Int32
			
			If (_fileSettingsTypeColumnNameIndex = -1 Or _fileSettingsIDColumnNameIndex = -1) Then
				For i As Integer = 0 To _reader.FieldCount - 1
					If (_reader.GetName(i) = _FileSettings.TypeColumnName) Then
						oiFileType = Convert.ToString(_reader.GetValue(i))
						_fileSettingsTypeColumnNameIndex = i
					ElseIf (_reader.GetName(i) = _FileSettings.IDColumnName) Then
						Dim value As Int32 = -1
						Dim readerValue As String = _reader.GetValue(i).ToString()
						Int32.TryParse(readerValue, value)
						oiFileId = value
						_fileSettingsIDColumnNameIndex = i
					End If
				Next
			Else
				If (_reader.GetName(_fileSettingsTypeColumnNameIndex) = _FileSettings.TypeColumnName) Then
					oiFileType = _reader.GetValue(_fileSettingsTypeColumnNameIndex).ToString()
				End If
				If (_reader.GetName(_fileSettingsIDColumnNameIndex) = _FileSettings.IDColumnName) Then
					Dim value As Int32 = -1
					Dim readerValue As String = _reader.GetValue(_fileSettingsIDColumnNameIndex).ToString()
					Int32.TryParse(readerValue, value)
					oiFileId = value
				End If
			End If

			If String.IsNullOrEmpty(oiFileType) Then
				Return Nothing
			End If

			Dim isSupportedByViewer As Boolean? = Nothing
			'Use IsSupportedByViewer mapping only when IDColumnName is not mapped, type id has precedence over this
			If String.IsNullOrEmpty(_FileSettings.IDColumnName)
				isSupportedByViewer = GetSupportedByViewerData()
			End If

			If isSupportedByViewer Is Nothing
				Return New FileTypeInfo(oiFileId, oiFileType)
			Else 
				Return New ExtendedFileTypeInfo(oiFileType, isSupportedByViewer.Value)
			End If
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

		Private Function NonTextField(ByVal fieldType As FieldType) As Boolean
			Select Case fieldType
				Case FieldType.Boolean, FieldType.Currency,
				 FieldType.Decimal, FieldType.Date,
				 FieldType.Integer
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
					Case FieldType.Boolean
						field.Value = NullableTypesHelper.GetNullableBoolean(value)

					Case FieldType.Currency, FieldType.Decimal
						field.Value = NullableTypesHelper.ToNullableDecimal(value.Trim)

					Case FieldType.Date
						field.Value = NullableTypesHelper.GetNullableDateTime(value)

					Case FieldType.Integer
						field.Value = NullableTypesHelper.ToNullableInt32(value.Replace(",", ""))

					Case Else
						Throw New System.ArgumentException("Unsupported field type '" & field.Type.ToString & "'")
				End Select
			End If
		End Sub

		Private Sub SetNativeFieldValue(ByVal field As ArtifactField, ByVal value As Object)

			'RaiseEvent StatusMessage("Field ArtifactID = " & field.ArtifactID)
			Select Case field.Type
				Case FieldType.Boolean
					field.Value = NullableTypesHelper.DBNullConvertToNullable(Of Boolean)(value)
				Case FieldType.Code
					field.Value = NullableTypesHelper.DBNullString(value)
				Case FieldType.Text, FieldType.OffTableText
					If TypeOf value Is System.IO.Stream
						field.Value = value
					Else
						field.Value = NullableTypesHelper.DBNullString(value)
					End If
				Case FieldType.User
					field.Value = NullableTypesHelper.DBNullString(value)
				Case FieldType.Varchar
					field.Value = NullableTypesHelper.DBNullString(value)
				Case FieldType.Object
					field.Value = NullableTypesHelper.DBNullString(value)
				Case FieldType.Currency, FieldType.Decimal
					field.Value = NullableTypesHelper.DBNullConvertToNullable(Of Decimal)(value)
				Case FieldType.Date
					field.Value = NullableTypesHelper.DBNullConvertToNullable(Of System.DateTime)(value)
				Case FieldType.File
					field.Value = NullableTypesHelper.DBNullString(value)
					'field.Value = value.ToString
				Case FieldType.Integer
					field.Value = NullableTypesHelper.DBNullConvertToNullable(Of Int32)(value)
					'field.Value = NullableTypesHelper.ToNullableInt32(value)
				Case FieldType.MultiCode, FieldType.Objects
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
		Public Event OnIoWarning(ByVal e As IoWarningEventArgs) Implements kCura.WinEDDS.Api.IArtifactReader.OnIoWarning

#End Region

#Region " Implemented but empty "

		Public Sub Halt() Implements kCura.WinEDDS.Api.IArtifactReader.Halt
		End Sub

        ''' <summary>
        ''' Reads all remaining rows and then closes <see cref="IDataReader"/>. This is because the only way to get the total number of records is to read all the rows.
        ''' </summary>
		Public Sub Close() Implements kCura.WinEDDS.Api.IArtifactReader.Close
            While Not _reader.IsClosed
                AdvanceRecord
            End While
		End Sub

		Public Function ManageErrorRecords(ByVal errorMessageFileLocation As String, ByVal prePushErrorLineNumbersFileName As String) As String Implements kCura.WinEDDS.Api.IArtifactReader.ManageErrorRecords
			Dim artifactReader As IArtifactReader = TryCast(_reader, IArtifactReader)
			Return artifactReader?.ManageErrorRecords(errorMessageFileLocation, prePushErrorLineNumbersFileName)
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
		Public Property FileNameColumn As String
		Public Property SupportedByViewerColumn As String
	End Class
End Namespace

