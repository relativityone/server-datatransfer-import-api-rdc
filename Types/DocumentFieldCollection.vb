Namespace kCura.WinEDDS
	Public Class DocumentFieldCollection

		Private _idIndex As System.Collections.Hashtable
		Private _nameIndex As System.Collections.Hashtable

		Public Function Item(ByVal fieldID As Int32) As DocumentField
			Return DirectCast(_idIndex.Item(fieldID), DocumentField)
		End Function

		Public Function Item(ByVal fieldName As String) As DocumentField
			Return DirectCast(_nameIndex.Item(fieldName), DocumentField)
		End Function

		Public Function Count() As Int32
			Return _nameIndex.Count
		End Function

		Public Sub Add(ByVal field As DocumentField)
			_idIndex.Add(field.FieldID, field)
			_nameIndex.Add(field.FieldName, field)
		End Sub

		Public Sub AddRange(ByVal fields As DocumentField())
			Dim field As DocumentField
			For Each field In fields
				_idIndex.Add(field.FieldID, field)
				_nameIndex.Add(field.FieldName, field)
			Next
		End Sub

		Public Function Names() As String()
			Dim i As Int32 = 0
			Dim o(_nameIndex.Keys.Count - 1) As Object
			Dim retval(_nameIndex.Keys.Count - 1) As String
			_nameIndex.Keys.CopyTo(o, 0)
			For i = 0 To o.Length - 1
				retval(i) = CType(o(i), String)
			Next
			Array.Sort(retval)
			Return retval
		End Function

		Public Function Exists(ByVal fieldID As Int32) As Boolean
			Return Not _idIndex.Item(fieldID) Is Nothing
		End Function

		Public Function Exists(ByVal fieldName As String) As Boolean
			Return Not _nameIndex.Item(fieldName) Is Nothing
		End Function

		Public ReadOnly Property GroupIdentifier() As DocumentField
			Get
				Dim ind As Int32
				Dim field As DocumentField
				For Each ind In _idIndex.Keys
					field = DirectCast(_idIndex(ind), DocumentField)
					If field.FieldCategory = DynamicFields.Types.FieldCategory.GroupIdentifier Then
						Return field
					End If
				Next
			End Get
		End Property

		Public Function IdentifierFields() As DocumentField()
			Dim df As DocumentField
			Dim al As New ArrayList
			For Each df In _idIndex.Values
				If df.FieldCategoryID = kCura.DynamicFields.Types.FieldCategory.Identifier Then
					al.Add(df)
				End If
			Next
			Return DirectCast(al.ToArray(GetType(DocumentField)), DocumentField())
		End Function

		Public Function IdentifierFieldNames() As String()
			Dim ids As DocumentField() = IdentifierFields()
			Dim retval(ids.Length - 1) As String
			Dim i As Int32
			For i = 0 To retval.Length - 1
				retval(i) = ids(i).FieldName
			Next
			System.Array.Sort(retval)
			Return retval
		End Function

		Public Function NamesForIdentifierDropdown() As String()
			Dim al As New ArrayList
			Dim field As DocumentField
			Dim fieldname As String
			For Each field In _idIndex.Values
				If ( _
				 field.FieldCategoryID <> 8 AndAlso _
				 field.FieldCategoryID <> 5 AndAlso _
				 field.FieldTypeID = kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Varchar _
				) Then
					al.Add(field.FieldName)
				End If
			Next
			Dim retval As String() = DirectCast(al.ToArray(GetType(String)), String())
			System.Array.Sort(retval)
			Return retval
		End Function

		Public Sub New()
			_idIndex = New System.Collections.Hashtable
			_nameIndex = New System.Collections.Hashtable
		End Sub
		Public Sub New(ByVal fields As DocumentField())
			Me.New()
			If Not fields Is Nothing Then Me.AddRange(fields)
		End Sub

		Public Class FieldComparer
			Implements IComparer
			Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements System.Collections.IComparer.Compare
				Dim lhs As DocumentField = DirectCast(x, DocumentField)
				Dim rhs As DocumentField = DirectCast(y, DocumentField)
			End Function
		End Class
	End Class
End Namespace