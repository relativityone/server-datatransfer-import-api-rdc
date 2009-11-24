Imports kCura.DynamicFields.Types
Namespace kCura.WinEDDS.Api
	Public Class ArtifactField
		Inherits kCura.EDDS.Types.FieldInfoBase
		Private _value As Object
		Private _associatedObjectTypeID As Int32
		Public Property Value() As Object
			Get
				Return _value
			End Get
			Set(ByVal value As Object)
				_value = value
			End Set
		End Property

		Public Property AssociatedObjectTypeID() As Int32
			Get
				Return _associatedObjectTypeID
			End Get
			Set(ByVal value As Int32)
				_associatedObjectTypeID = value
			End Set
		End Property

		Public ReadOnly Property ValueAsString() As String
			Get
				If _value Is Nothing Then Return ""
				Return _value.ToString
			End Get
		End Property
		Public Sub New(ByVal displayName As String, ByVal artifactID As Int32, ByVal fieldTypeID As FieldTypeHelper.FieldType, ByVal fieldCategoryID As FieldCategory, ByVal codeTypeID As NullableTypes.NullableInt32, ByVal textLength As NullableInt32, ByVal associatedObjectTypeID As NullableInt32)
			MyBase.New()
			Me.DisplayName = displayName
			Me.ArtifactID = artifactID
			Me.Type = fieldTypeID
			Me.Category = fieldCategoryID
			If Not codeTypeID.IsNull Then Me.CodeTypeID = codeTypeID.Value
			If Not textLength.IsNull Then Me.TextLength = textLength.Value
			If Not associatedObjectTypeID.IsNull Then Me.AssociatedObjectTypeID = associatedObjectTypeID.Value
		End Sub

		Public Function Copy() As Api.ArtifactField
			Dim textl As NullableInt32 = NullableInt32.Null
			Return New Api.ArtifactField(Me.DisplayName, Me.ArtifactID, Me.Type, Me.Category, New NullableInt32(Me.CodeTypeID), New NullableInt32(Me.TextLength), New NullableInt32(Me.AssociatedObjectTypeID))
		End Function

		Friend Sub New(ByVal field As DocumentField)
			Me.ArtifactID = field.FieldID
			Me.Category = CType(field.FieldCategoryID, kCura.DynamicFields.Types.FieldCategory)
			If Not field.CodeTypeID.IsNull Then Me.CodeTypeID = field.CodeTypeID.Value
			Me.DisplayName = field.FieldName
			If Not field.FieldLength.IsNull Then Me.TextLength = field.FieldLength.Value
			If Not field.AssociatedObjectTypeID.IsNull Then Me.AssociatedObjectTypeID = field.AssociatedObjectTypeID.Value
			Me.Type = CType(field.FieldTypeID, kCura.DynamicFields.Types.FieldTypeHelper.FieldType)
		End Sub
		Friend Sub New(ByVal field As kCura.EDDS.WebAPI.DocumentManagerBase.Field)
			Me.ArtifactID = field.ArtifactID
			Me.Category = CType(field.FieldCategoryID, kCura.DynamicFields.Types.FieldCategory)
			If Not field.CodeTypeID.IsNull Then Me.CodeTypeID = field.CodeTypeID.Value
			Me.DisplayName = field.DisplayName
			If Not field.MaxLength.IsNull Then Me.TextLength = field.MaxLength.Value
			If Not field.AssociativeArtifactTypeID.IsNull Then Me.AssociatedObjectTypeID = field.AssociativeArtifactTypeID.Value
			Me.Type = CType(field.FieldTypeID, kCura.DynamicFields.Types.FieldTypeHelper.FieldType)
		End Sub
	End Class
End Namespace

