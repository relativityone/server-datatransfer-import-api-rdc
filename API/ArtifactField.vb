Imports Relativity
Namespace kCura.WinEDDS.Api
	Public Class ArtifactField
		Inherits Relativity.FieldInfoBase
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
		Public Sub New(ByVal displayName As String, ByVal artifactID As Int32, ByVal fieldTypeID As FieldTypeHelper.FieldType, ByVal fieldCategoryID As FieldCategory, ByVal codeTypeID As Nullable(Of Int32), ByVal textLength As Nullable(Of Int32), ByVal associatedObjectTypeID As Nullable(Of Int32), ByVal enableDataGrid As Boolean)
			MyBase.New()
			Me.DisplayName = displayName
			Me.ArtifactID = artifactID
			Me.Type = fieldTypeID
			Me.Category = fieldCategoryID
			If Not codeTypeID Is Nothing Then Me.CodeTypeID = codeTypeID.Value
			If Not textLength Is Nothing Then Me.TextLength = textLength.Value
			If Not associatedObjectTypeID Is Nothing Then Me.AssociatedObjectTypeID = associatedObjectTypeID.Value
			Me.EnableDataGrid = enableDataGrid
		End Sub

		Public Function Copy() As Api.ArtifactField
			Return New Api.ArtifactField(Me.DisplayName, Me.ArtifactID, Me.Type, Me.Category, New Nullable(Of Int32)(Me.CodeTypeID), New Nullable(Of Int32)(Me.TextLength), New Nullable(Of Int32)(Me.AssociatedObjectTypeID), Me.EnableDataGrid)
		End Function

		Public Sub New(ByVal field As DocumentField)
			Me.ArtifactID = field.FieldID
			Me.Category = CType(field.FieldCategoryID, Relativity.FieldCategory)
			If Not field.CodeTypeID Is Nothing Then Me.CodeTypeID = field.CodeTypeID.Value
			Me.DisplayName = field.FieldName
			If Not field.FieldLength Is Nothing Then Me.TextLength = field.FieldLength.Value
			If Not field.AssociatedObjectTypeID Is Nothing Then Me.AssociatedObjectTypeID = field.AssociatedObjectTypeID.Value
			Me.Type = CType(field.FieldTypeID, Relativity.FieldTypeHelper.FieldType)
			Me.EnableDataGrid = field.EnableDataGrid
		End Sub

		Public Sub New(ByVal field As kCura.EDDS.WebAPI.DocumentManagerBase.Field)
			Me.ArtifactID = field.ArtifactID
			Me.Category = CType(field.FieldCategoryID, Relativity.FieldCategory)
			If Not field.CodeTypeID Is Nothing Then Me.CodeTypeID = field.CodeTypeID.Value
			Me.DisplayName = field.DisplayName
			If Not field.MaxLength Is Nothing Then Me.TextLength = field.MaxLength.Value
			If Not field.AssociativeArtifactTypeID Is Nothing Then Me.AssociatedObjectTypeID = field.AssociativeArtifactTypeID.Value
			Me.Type = CType(field.FieldTypeID, Relativity.FieldTypeHelper.FieldType)
			Me.EnableDataGrid = field.EnableDataGrid
		End Sub
	End Class
End Namespace

