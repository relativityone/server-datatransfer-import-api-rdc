Imports System.Runtime.Serialization
Namespace kCura.WinEDDS
	<Serializable()> Public Class DocumentField

#Region "Members"
		Private _fieldName As String
		Private _fieldID As Int32
		Private _fieldTypeID As Int32
		Private _value As String
		Private _fieldCategoryID As Int32
		Private _codeTypeID As NullableTypes.NullableInt32
		Private _fileColumnIndex As Int32
		Private _fieldLength As NullableTypes.NullableInt32
		<NonSerialized()> Private _useUnicode As Boolean
#End Region

#Region "Properties"
		Public Property FieldName() As String
			Get
				Return _fieldName
			End Get
			Set(ByVal value As String)
				_fieldName = value
			End Set
		End Property

		Public Property FieldID() As Int32
			Get
				Return _fieldID
			End Get
			Set(ByVal value As Int32)
				_fieldID = value
			End Set
		End Property

		Public Property FieldTypeID() As Int32
			Get
				Return _fieldTypeID
			End Get
			Set(ByVal value As Int32)
				_fieldTypeID = value
			End Set
		End Property

		Public Property FieldCategoryID() As Int32
			Get
				Return _fieldCategoryID
			End Get
			Set(ByVal value As Int32)
				_fieldCategoryID = value
			End Set
		End Property

		Public Property FieldCategory() As kCura.DynamicFields.Types.FieldCategory
			Get
				Return CType(_fieldCategoryID, kCura.DynamicFields.Types.FieldCategory)
			End Get
			Set(ByVal value As kCura.DynamicFields.Types.FieldCategory)
				_fieldCategoryID = value
			End Set
		End Property

		Public Property Value() As String
			Get
				Return _value
			End Get
			Set(ByVal value As String)
				_value = value
			End Set
		End Property

		Public Property CodeTypeID() As NullableTypes.NullableInt32
			Get
				Return _codeTypeID
			End Get
			Set(ByVal value As NullableTypes.NullableInt32)
				_codeTypeID = value
			End Set
		End Property

		Public Property FileColumnIndex() As Int32
			Get
				Return _fileColumnIndex
			End Get
			Set(ByVal value As Int32)
				_fileColumnIndex = value
			End Set
		End Property

		Public Property FieldLength() As NullableTypes.NullableInt32
			Get
				Return _fieldLength
			End Get
			Set(ByVal value As NullableTypes.NullableInt32)
				_fieldLength = value
			End Set
		End Property

		Public Property UseUnicode() As Boolean
			Get
				Return _useUnicode
			End Get
			Set(ByVal value As Boolean)
				_useUnicode = value
			End Set
		End Property

		Public Function ToDisplayString() As String
			Return String.Format("DocumentField[{0},{1},{2},{3},'{4}']", FieldCategoryID, FieldID, FieldName, FieldTypeID, kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(CodeTypeID))
		End Function
		Public Overloads Function ToString() As String
			Return FieldName
		End Function
#End Region

#Region "Constructors"

		Public Sub New(ByVal fieldName As String, ByVal fieldID As Int32, ByVal fieldTypeID As Int32, ByVal fieldCategoryID As Int32, ByVal codeTypeID As NullableTypes.NullableInt32, ByVal fieldLength As NullableInt32, ByVal useUnicode As Boolean)
			MyBase.New()
			_fieldName = fieldName
			_fieldID = fieldID
			_fieldTypeID = fieldTypeID
			_fieldCategoryID = fieldCategoryID
			_codeTypeID = codeTypeID
			_fieldLength = fieldLength
			_useUnicode = useUnicode
		End Sub

		Public Sub New(ByVal docField As DocumentField)
			Me.New(docField.FieldName, docField.FieldID, docField.FieldTypeID, docField.FieldCategoryID, docField.CodeTypeID, docField.FieldLength, docField.UseUnicode)
		End Sub

#End Region

		Public Shared Function op_Equality(ByVal df1 As DocumentField, ByVal df2 As DocumentField) As Boolean
			Dim areEqual As Boolean
			If df1.CodeTypeID.IsNull Then
				If df2.CodeTypeID.IsNull Then
					areEqual = True
				Else
					areEqual = False
				End If
			Else
				If df2.CodeTypeID.IsNull Then
					areEqual = True
				Else
					areEqual = df1.CodeTypeID.Value = df2.CodeTypeID.Value
				End If
			End If
			areEqual = areEqual And df1.FieldCategoryID = df2.FieldCategoryID
			areEqual = areEqual And df1.FieldName = df2.FieldName
			areEqual = areEqual And df1.FieldTypeID = df2.FieldTypeID
			areEqual = areEqual And df1.Value = df2.Value
			Return areEqual
		End Function
	End Class

End Namespace
