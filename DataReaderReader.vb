Imports kCura.WinEDDS
Namespace kCura.WinEDDS.ImportExtension
	Public Class DataReaderReader
		Implements kCura.WinEDDS.Api.IArtifactReader

		Private Const _KCURAMARKERFILENAME As String = "kcuramarkerfilename"

		Private _reader As System.Data.IDataReader
		Private _loadFileSettings As kCura.WinEDDS.LoadFile
		Private _currentLineNumber As Long = 0
		Private _size As Long = -1
		Private _columnNames As String()
		Private _allFields As Api.ArtifactFieldCollection
		Private _tempLocalDirectory As String
		Private _markerNameIsMapped As Nullable(Of Boolean)

		Public Sub New(ByVal args As DataReaderReaderInitializationArgs, ByVal fieldMap As kCura.WinEDDS.LoadFile, ByVal reader As System.Data.IDataReader)
			_reader = reader
			If _reader Is Nothing Then Throw New NullReferenceException("The reader being passed into this IDataReaderReader is null")
			If _reader.IsClosed = True OrElse _reader.FieldCount = 0 Then Throw New ArgumentException("The reader being passed into this IDataReaderReader is empty")
			_loadFileSettings = fieldMap
			_allFields = args.AllFields
			_tempLocalDirectory = System.IO.Path.GetTempPath + "FlexMigrationFiles\"
		End Sub

#Region " Artifact Reader Implementation "

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

		Private Function FormatMultiCodeValueToXml(ByVal multicodeValue As String) As String
			'Convert x\y\z;a\b\c; to <root><item>x\y\z</item><item>a\b\c</item></root>
			'; is based on _loadFileSettings.MultiRecordDelimiter
			Dim retValue As String = String.Empty
			Dim multiValue() As String = multicodeValue.Split(_loadFileSettings.MultiRecordDelimiter)
			If multiValue.Length > 0 Then
				For Each multiChoiceValue As String In multiValue
					If Not multiChoiceValue = String.Empty Then
						retValue &= String.Format("<item>{0}</item>", multiChoiceValue)
					End If
				Next
				retValue = String.Format("<root>{0}</root>", retValue)
			End If
			Return retValue
		End Function

		Private Function DoesFilenameMarkerFieldExistInIDataReader() As Boolean
			If Not _markerNameIsMapped.HasValue Then
				Try
					Dim isValidItemName As Integer = _reader.GetOrdinal(_KCURAMARKERFILENAME)
					_markerNameIsMapped = True
				Catch ex As IndexOutOfRangeException
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
			Dim retval As New Api.ArtifactFieldCollection


			Dim folderStructureContainedInColumnWithoutIndex As String = nameWithoutIndex(_loadFileSettings.FolderStructureContainedInColumn)
			Dim nativeFilePathColumnWithoutIndex As String = nameWithoutIndex(_loadFileSettings.NativeFilePathColumn)

			For i As Integer = 0 To _reader.FieldCount - 1
				Dim field As Api.ArtifactField = _allFields(_reader.GetName(i).ToLower)
				If Not field Is Nothing Then
					If Not field.DisplayName = folderStructureContainedInColumnWithoutIndex Then
						Dim thisCell As Api.ArtifactField = field.Copy
						Me.SetFieldValue(thisCell, _reader.Item(i))
						retval.Add(thisCell)
					End If
				End If
			Next

			'NativeFilePathColumn is in the format  displayname(index).  _reader only has name.  I need to get the index, get _reader.name, and add a field with the data.
			If _loadFileSettings.ArtifactTypeID = Relativity.ArtifactType.Document Then
				If _loadFileSettings.LoadNativeFiles AndAlso Not _loadFileSettings.NativeFilePathColumn Is Nothing AndAlso Not _loadFileSettings.NativeFilePathColumn = String.Empty Then
					Dim nativeFileIndex As Int32 = Int32.Parse(_loadFileSettings.NativeFilePathColumn.Substring(_loadFileSettings.NativeFilePathColumn.LastIndexOf("(")).Trim("()".ToCharArray))
					Dim displayName As String = _reader.GetName(nativeFileIndex - 1)
					Dim field As New Api.ArtifactField(New DocumentField(displayName, -1, Relativity.FieldTypeHelper.FieldType.File, Relativity.FieldCategory.FileInfo, New Nullable(Of Int32)(Nothing), New Nullable(Of Int32)(Nothing), New Nullable(Of Int32)(Nothing), True, EDDS.WebAPI.DocumentManagerBase.ImportBehaviorChoice.LeaveBlankValuesUnchanged))
					Me.SetFieldValue(field, _reader.Item(nativeFileIndex - 1))
					retval.Add(field)
				End If
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
										newLocation = _tempLocalDirectory & _reader.Item(_KCURAMARKERFILENAME).ToString
									Catch ex As Exception
										newLocation = _tempLocalDirectory & System.IO.Path.GetFileName(field.ValueAsString)
									End Try
									If System.IO.File.Exists(newLocation) Then
										kCura.Utility.File.Delete(newLocation)
										Try
											kCura.Utility.File.Delete(newLocation)
										Catch ex As Exception
											'This is to make sure we don't have buggy data.  Clients that have run the line that sets attributes below will never hit this line.
											'However, clients on old versions of ImportAPI may hit this line and we want to make sure we're accounting for our mistake.
											System.IO.File.SetAttributes(newLocation, System.IO.FileAttributes.Normal)
											kCura.Utility.File.Delete(newLocation)
										End Try
									End If
									System.IO.File.Copy(field.ValueAsString, newLocation)
									'Set the attributes to Normal so we can delete the file if it's read only
									System.IO.File.SetAttributes(newLocation, System.IO.FileAttributes.Normal)
									field.Value = newLocation
									'Only one file field is allowed
									Exit For
								Else
									Throw New Exception("File" + field.ValueAsString + " does not exist or you don't have sufficient privilidges to access it")
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
				Dim field As New Api.ArtifactField(displayName, -2, Relativity.FieldTypeHelper.FieldType.Object, Relativity.FieldCategory.ParentArtifact, New Nullable(Of Int32)(Nothing), New Nullable(Of Int32)(255), New Nullable(Of Int32)(Nothing))
				Me.SetFieldValue(field, _reader.Item(parentIndex - 1))
				retval.Add(field)
			End If

			If Me.CurrentLineNumber = 0 Then
				DisplayFieldMap(_reader, retval)
			End If

			AdvanceRecord()

			Return retval
		End Function

		Private Sub DisplayFieldMap(ByVal sourceData As System.Data.IDataReader, ByVal destination As kCura.WinEDDS.Api.ArtifactFieldCollection)
			'TODO: fix thsi display.  something with uppercase/lowercase, I think.
			'Display a list field mappings and unmapped fields
			Dim mappedFields As String = String.Concat(vbCrLf, "----------", vbCrLf, "Field Mapping:", vbCrLf)
			Dim fieldMapRow As String = String.Concat("Source field [{0}] --> Destination field [{1}]", vbCrLf)
			For i As Integer = 0 To sourceData.FieldCount - 1
				Dim field As Api.ArtifactField = destination(_reader.GetName(i).ToLower)
				If field Is Nothing Then
					If _loadFileSettings.CopyFilesToDocumentRepository = True And _reader.GetName(i).ToLower = _KCURAMARKERFILENAME.ToLower Then
						'This is a special field.
						mappedFields = String.Concat(mappedFields, String.Format(fieldMapRow, _reader.GetName(i).ToLower, "*Filename*"))
					Else
						If Not (_loadFileSettings.NativeFilePathColumn Is Nothing) AndAlso _reader.GetName(i).ToLower = nameWithoutIndex(_loadFileSettings.NativeFilePathColumn.ToLower) Then
							'This is a special field
							mappedFields = String.Concat(mappedFields, String.Format(fieldMapRow, _reader.GetName(i).ToLower, "*NativeFilePath*"))
						Else
							mappedFields = String.Concat(mappedFields, String.Format(fieldMapRow, _reader.GetName(i).ToLower, "NOT MAPPED"))
						End If
					End If
				Else
					mappedFields = String.Concat(mappedFields, String.Format(fieldMapRow, _reader.GetName(i).ToLower, field.DisplayName.ToLower))
				End If
			Next

			mappedFields = String.Concat(mappedFields, "----------", vbCrLf)
			RaiseEvent StatusMessage(mappedFields)
		End Sub

		Private Sub SetFieldValue(ByVal field As Api.ArtifactField, ByVal value As Object)
			'RaiseEvent StatusMessage("Field ArtifactID = " & field.ArtifactID)
			Select Case field.Type
				Case Relativity.FieldTypeHelper.FieldType.Boolean
					field.Value = kCura.Utility.NullableTypesHelper.DBNullConvertToNullable(Of Boolean)(value)
				Case Relativity.FieldTypeHelper.FieldType.Code
					field.Value = kCura.Utility.NullableTypesHelper.DBNullString(value)
				Case Relativity.FieldTypeHelper.FieldType.Text
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
					field.Value = kCura.Utility.NullableTypesHelper.DBNullString(value)
					Dim xml As String = value.ToString
					If Not xml = String.Empty Then
						xml = FormatMultiCodeValueToXml(xml)
						Dim nodes As New System.Collections.ArrayList
						Dim doc As New System.Xml.XmlDocument
						doc.LoadXml(xml)
						For Each node As System.Xml.XmlElement In doc.ChildNodes(0).ChildNodes
							nodes.Add(node.InnerText)
						Next
						field.Value = DirectCast(nodes.ToArray(GetType(String)), String())
					Else
						field.Value = New String() {}
					End If
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
		Public Event DataSourcePrep(ByVal e As kCura.WinEDDS.Api.DataSourcePrepEventArgs) Implements kCura.WinEDDS.Api.IArtifactReader.DataSourcePrep
		Public Event OnIoWarning(ByVal e As kCura.WinEDDS.Api.IoWarningEventArgs) Implements kCura.WinEDDS.Api.IArtifactReader.OnIoWarning

#End Region

#Region " Implemented but empty "

		Public Sub Halt() Implements kCura.WinEDDS.Api.IArtifactReader.Halt
		End Sub

		Public Sub Close() Implements kCura.WinEDDS.Api.IArtifactReader.Close
		End Sub

		Public Function ManageErrorRecords(ByVal errorMessageFileLocation As String, ByVal prePushErrorLineNumbersFileName As String) As String Implements kCura.WinEDDS.Api.IArtifactReader.ManageErrorRecords
		End Function

		Public Sub OnFatalErrorState() Implements kCura.WinEDDS.Api.IArtifactReader.OnFatalErrorState
		End Sub

#End Region

#End Region

	End Class
End Namespace

