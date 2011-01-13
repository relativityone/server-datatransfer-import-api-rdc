Imports System.Runtime.Serialization
Namespace kCura.WinEDDS
	<Serializable()> Public Class DocumentField
		Implements ISerializable

#Region "Properties"

		<NonSerialized()> Private _associatedObjectTypeID As Nullable(Of Int32)
		Public Property AssociatedObjectTypeID() As Nullable(Of Int32)
			Get
				Return _associatedObjectTypeID
			End Get
			Set(ByVal Value As Nullable(Of Int32))
				_associatedObjectTypeID = Value
			End Set
		End Property

		<NonSerialized()> Private _useUnicode As Boolean
		Public Property UseUnicode() As Boolean
			Get
				Return _useUnicode
			End Get
			Set(ByVal value As Boolean)
				_useUnicode = value
			End Set
		End Property

		<NonSerialized()> Private _importBehavior As kCura.EDDS.WebAPI.DocumentManagerBase.ImportBehaviorChoice
		Public Property ImportBehavior() As kCura.EDDS.WebAPI.DocumentManagerBase.ImportBehaviorChoice
			Get
				Return _importBehavior
			End Get
			Set(ByVal value As kCura.EDDS.WebAPI.DocumentManagerBase.ImportBehaviorChoice)
				_importBehavior = value
			End Set
		End Property

		Public Property FieldName() As String
		Public Property FieldID() As Int32
		Public Property FieldTypeID() As Int32
		Public Property FieldCategoryID() As Int32
		Public Property FieldCategory() As Relativity.FieldCategory
		Public Property Value() As String
		Public Property CodeTypeID() As Nullable(Of Int32)
		Public Property FileColumnIndex() As Int32
		Public Property FieldLength() As Nullable(Of Int32)

#End Region

#Region "Constructors"

		Private Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal Context As System.Runtime.Serialization.StreamingContext)
			Me.FieldName = info.GetString("_fieldName")
			Me.FieldID = info.GetInt32("_fieldID")
			Me.FieldTypeID = info.GetInt32("_fieldTypeID")
			Me.Value = info.GetString("_value")
			Me.FieldCategoryID = info.GetInt32("_fieldCategoryID")
			Me.FileColumnIndex = info.GetInt32("_fileColumnIndex")
		End Sub

		Public Sub New(ByVal fieldName As String, ByVal fieldID As Int32, ByVal fieldTypeID As Int32, ByVal fieldCategoryID As Int32, ByVal codeTypeID As Nullable(Of Int32), ByVal fieldLength As Nullable(Of Int32), ByVal associatedObjectTypeID As Nullable(Of Int32), ByVal useUnicode As Boolean, ByVal importBehavior As kCura.EDDS.WebAPI.DocumentManagerBase.ImportBehaviorChoice)
			MyBase.New()
			_FieldName = fieldName
			_FieldID = fieldID
			_FieldTypeID = fieldTypeID
			_FieldCategoryID = fieldCategoryID
			_CodeTypeID = codeTypeID
			_FieldLength = fieldLength
			_associatedObjectTypeID = associatedObjectTypeID
			_useUnicode = useUnicode
			_importBehavior = importBehavior
		End Sub

		Public Sub New(ByVal docField As DocumentField)
			Me.New(docField.FieldName, docField.FieldID, docField.FieldTypeID, docField.FieldCategoryID, docField.CodeTypeID, docField.FieldLength, docField.AssociatedObjectTypeID, docField.UseUnicode, docField.ImportBehavior)
		End Sub

#End Region

		Public Function ToDisplayString() As String
			Return String.Format("DocumentField[{0},{1},{2},{3},'{4}']", FieldCategoryID, FieldID, FieldName, FieldTypeID, kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(CodeTypeID))
		End Function

		Public Overrides Function ToString() As String
			Return FieldName
		End Function

		Public Function ToFieldInfo() As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo
			Dim retval As New kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo
			retval.ArtifactID = Me.FieldID
			retval.Category = CType(Me.FieldCategoryID, kCura.EDDS.WebAPI.BulkImportManagerBase.FieldCategory)
			If Not Me.CodeTypeID Is Nothing Then retval.CodeTypeID = Me.CodeTypeID.Value
			retval.DisplayName = Me.FieldName
			If Not Me.FieldLength Is Nothing Then retval.TextLength = Me.FieldLength.Value
			retval.Type = CType(Me.FieldTypeID, kCura.EDDS.WebAPI.BulkImportManagerBase.FieldType)
			retval.IsUnicodeEnabled = Me.UseUnicode
			Return retval
		End Function

		Public Sub GetObjectData(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext) Implements System.Runtime.Serialization.ISerializable.GetObjectData
			info.AddValue("_fieldName", Me.FieldName, GetType(String))
			info.AddValue("_fieldID", Me.FieldID, GetType(Int32))
			info.AddValue("_fieldTypeID", Me.FieldTypeID, GetType(Int32))
			info.AddValue("_value", Me.Value, GetType(String))
			info.AddValue("_fieldCategoryID", Me.FieldCategoryID, GetType(Int32))
			info.AddValue("_fileColumnIndex", Me.FileColumnIndex, GetType(Int32))
		End Sub

		Public Shared Function op_Equality(ByVal df1 As DocumentField, ByVal df2 As DocumentField) As Boolean
			Dim areEqual As Boolean
			If df1.CodeTypeID Is Nothing Then
				If df2.CodeTypeID Is Nothing Then
					areEqual = True
				Else
					areEqual = False
				End If
			Else
				If df2.CodeTypeID Is Nothing Then
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
