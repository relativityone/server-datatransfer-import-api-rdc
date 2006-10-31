Namespace kCura.WinEDDS
	<Serializable()> Public Class DocumentInfo

#Region "Members"
		Private _fields() As DocumentField
		Private _files() As FileInfo
		Private _artifactID As Integer
		Private _artifactTypeID As Integer
		Private _parentArtifactID As NullableInt32
		Private _containerID As NullableInt32
		Private _accessControlListID As Integer
		Private _accessControlListIsInherited As Boolean
		Private _keywords As String
		Private _notes As String
		Private _textIdentifier As String
		Private _lastModifiedOn As Date
		Private _lastModifiedBy As Integer
		Private _createdBy As Integer
		Private _createdOn As Date
		Private _deleteFlag As Boolean
#End Region

#Region "Properties"

		Public Property Fields() As DocumentField()
			Get
				Return _fields
			End Get
			Set(ByVal value As DocumentField())
				_fields = value
			End Set
		End Property

		Public Property Files() As FileInfo()
			Get
				Return _files
			End Get
			Set(ByVal value As FileInfo())
				_files = value
			End Set
		End Property

		Public Property ArtifactID() As Integer
			Get
				Return _artifactID
			End Get
			Set(ByVal value As Integer)
				_artifactID = value
			End Set
		End Property

		Public Property ArtifactTypeID() As Integer
			Get
				Return _artifactTypeID
			End Get
			Set(ByVal value As Integer)
				_artifactTypeID = value
			End Set
		End Property

		Public Property ParentArtifactID() As NullableInt32
			Get
				Return _parentArtifactID
			End Get
			Set(ByVal value As NullableInt32)
				_parentArtifactID = value
			End Set
		End Property

		Public Property ContainerID() As NullableInt32
			Get
				Return _containerID
			End Get
			Set(ByVal value As NullableInt32)
				_containerID = value
			End Set
		End Property

		Public Property AccessControlListID() As Integer
			Get
				Return _accessControlListID
			End Get
			Set(ByVal value As Integer)
				_accessControlListID = value
			End Set
		End Property

		Public Property AccessControlListIsInherited() As Boolean
			Get
				Return _accessControlListIsInherited
			End Get
			Set(ByVal value As Boolean)
				_accessControlListIsInherited = value
			End Set
		End Property

		Public Property Keywords() As String
			Get
				Return _keywords
			End Get
			Set(ByVal value As String)
				_keywords = value
			End Set
		End Property

		Public Property Notes() As String
			Get
				Return _notes
			End Get
			Set(ByVal value As String)
				_notes = value
			End Set
		End Property

		Public Property TextIdentifier() As String
			Get
				Return _textIdentifier
			End Get
			Set(ByVal value As String)
				_textIdentifier = value
			End Set
		End Property

		Public Property LastModifiedOn() As Date
			Get
				Return _lastModifiedOn
			End Get
			Set(ByVal value As Date)
				_lastModifiedOn = value
			End Set
		End Property

		Public Property LastModifiedBy() As Integer
			Get
				Return _lastModifiedBy
			End Get
			Set(ByVal value As Integer)
				_lastModifiedBy = value
			End Set
		End Property

		Public Property CreatedBy() As Integer
			Get
				Return _createdBy
			End Get
			Set(ByVal value As Integer)
				_createdBy = value
			End Set
		End Property

		Public Property CreatedOn() As Date
			Get
				Return _createdOn
			End Get
			Set(ByVal value As Date)
				_createdOn = value
			End Set
		End Property

		Public Property DeleteFlag() As Boolean
			Get
				Return _deleteFlag
			End Get
			Set(ByVal value As Boolean)
				_deleteFlag = value
			End Set
		End Property

#End Region

		Public Function ToDisplayString() As String
			Dim sb As New System.Text.StringBuilder
			sb.AppendFormat("[ArtifactID: {0}]", Me.ArtifactID)
			Dim docField As DocumentField
			For Each docField In Fields
				'If Not IgnoreField(docField.FieldName) Then
				If Not docField.FieldCategoryID = kCura.DynamicFields.Types.FieldCategory.FullText AndAlso Not IgnoreField(docField.FieldName) Then
					sb.AppendFormat(" [{0}: {1}]", docField.FieldName, docField.Value)
				End If
			Next
			Dim file As FileInfo
			sb.Append("[Files:")
			For Each file In Files
				sb.Append(" " & file.Filename)
			Next
			sb.Append("]")
			Return sb.ToString
		End Function

		Private Shared IGNRORED_FIELDS As String() = {"System Created On", "System Created By", "System Last Modified On", "System Last Modified By"}
		Public Shared Function IgnoreField(ByVal fieldName As String) As Boolean
			Return System.Array.IndexOf(kCura.WinEDDS.DocumentInfo.IGNRORED_FIELDS, fieldName) <> -1
		End Function

	End Class
End Namespace
