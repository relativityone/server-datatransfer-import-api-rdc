Imports kCura.WinEDDS
Imports NullableTypes.HelperFunctions
Namespace kCura.WinEDDS.ImportExtension
	Public Class QueryReader
		Implements kCura.WinEDDS.Api.IArtifactReader

		Private _table As System.Data.DataTable
		Private _loadFileSettings As kCura.WinEDDS.LoadFile
		Private _currentLineNumber As Long = 0
		Private _size As Long = -1
		Private _columnNames As String()
		Private _allFields As Api.ArtifactFieldCollection
		Public Sub New(ByVal args As QueryReaderInitializationArgs, ByVal fieldMap As kCura.WinEDDS.LoadFile, ByVal table As System.Data.DataTable)
			_table = table
			If _table Is Nothing Then Throw New NullReferenceException("The table being passed into this QueryReader is null")
			If _table.Rows.Count = 0 OrElse _table.Columns.Count = 0 Then Throw New ArgumentException("The table being passed into this QueryReader is empty")
			_loadFileSettings = fieldMap
			_allFields = args.AllFields
		End Sub
#Region " Artifact Reader Implementation "

		Public Sub AdvanceRecord() Implements kCura.WinEDDS.Api.IArtifactReader.AdvanceRecord
			_currentLineNumber += 1
		End Sub

		Public ReadOnly Property BytesProcessed() As Long Implements kCura.WinEDDS.Api.IArtifactReader.BytesProcessed
			Get
				If Me.SizeInBytes < 1 OrElse _table.Rows.Count = 0 Then
					Return 0
				Else
					Return CType((_currentLineNumber / _table.Rows.Count) * Me.SizeInBytes, Long)
				End If
			End Get
		End Property

		Public Function CountRecords() As Long Implements kCura.WinEDDS.Api.IArtifactReader.CountRecords
			Return _table.Rows.Count
		End Function

		Public ReadOnly Property CurrentLineNumber() As Integer Implements kCura.WinEDDS.Api.IArtifactReader.CurrentLineNumber
			Get
				Return CType(_currentLineNumber, Int32)
			End Get
		End Property

		Public Function GetColumnNames(ByVal args As Object) As String() Implements kCura.WinEDDS.Api.IArtifactReader.GetColumnNames
			If _columnNames Is Nothing Then
				Dim retval As New System.Collections.ArrayList
				For Each col As System.Data.DataColumn In _table.Columns
					retval.Add(col.ColumnName)
				Next
				_columnNames = DirectCast(retval.ToArray(GetType(String)), String())
			End If
			Return _columnNames
		End Function

		Public ReadOnly Property HasMoreRecords() As Boolean Implements kCura.WinEDDS.Api.IArtifactReader.HasMoreRecords
			Get
				Return Not (_currentLineNumber + 1 >= _table.Rows.Count)
			End Get
		End Property


		Public Function ReadArtifact() As kCura.WinEDDS.Api.ArtifactFieldCollection Implements kCura.WinEDDS.Api.IArtifactReader.ReadArtifact
			Dim retval As New Api.ArtifactFieldCollection
			For Each column As System.Data.DataColumn In _table.Columns
				Dim field As Api.ArtifactField = _allFields(column.ColumnName)
				If field Is Nothing Then
					If Me.CurrentLineNumber = 0 Then RaiseEvent StatusMessage("There is no field corresponding with the name '" & column.ColumnName & "'; data will be unmapped")
				Else
					Dim thisCell As Api.ArtifactField = field.Copy
					Me.SetFieldValue(thisCell, _table.Rows(Me.CurrentLineNumber)(column.ColumnName))
					retval.Add(thisCell)
				End If
			Next
			_currentLineNumber += 1
			Return retval
		End Function

		Private Sub SetFieldValue(ByVal field As Api.ArtifactField, ByVal value As Object)
			Select Case field.Type
				Case kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Boolean
					field.Value = DBNullConvert.ToNullableBoolean(value)
				Case kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Code, kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Object, kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Text, kCura.DynamicFields.Types.FieldTypeHelper.FieldType.User, kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Varchar
					If value Is Nothing Then value = ""
					field.Value = value.ToString
				Case kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Currency, kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Decimal
					field.Value = DBNullConvert.ToNullableDecimal(value)
				Case kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Date
					field.Value = DBNullConvert.ToNullableDateTime(value)
				Case kCura.DynamicFields.Types.FieldTypeHelper.FieldType.File
					field.Value = value.ToString
				Case kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Integer
					field.Value = DBNullConvert.ToNullableInt32(value)
				Case kCura.DynamicFields.Types.FieldTypeHelper.FieldType.MultiCode, kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Objects
					If value Is Nothing Then value = String.Empty
					Dim xml As String = value.ToString
					If xml = String.Empty Then
						field.Value = Nothing
					Else
						Dim nodes As New System.Collections.ArrayList
						Dim doc As New System.Xml.XmlDocument
						doc.LoadXml(xml)
						For Each node As System.Xml.XmlElement In doc.ChildNodes(0).ChildNodes
							nodes.Add(node.InnerText)
						Next
						'for each 
						field.Value = DirectCast(nodes.ToArray(GetType(String)), String())
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
					serializer.Serialize(sw, _table)
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

