Namespace kCura.WinEDDS
	<Serializable()> Public Class FileInfo

#Region "Members"
		Private _guid As String
		Private _order As Integer
		Private _type As Integer
		Private _filename As String
		Private _documentArtifactID As Integer
#End Region

#Region "Properties"
		Public Property Guid() As String
			Get
				Return _guid
			End Get
			Set(ByVal value As String)
				_guid = value
			End Set
		End Property

		Public Property Order() As Integer
			Get
				Return _order
			End Get
			Set(ByVal value As Integer)
				_order = value
			End Set
		End Property

		Public Property Type() As Integer
			Get
				Return _type
			End Get
			Set(ByVal value As Integer)
				_type = value
			End Set
		End Property

		Public Property Filename() As String
			Get
				Return _filename
			End Get
			Set(ByVal value As String)
				_filename = value
			End Set
		End Property

		Public Property DocumentArtifactID() As Integer
			Get
				Return _documentArtifactID
			End Get
			Set(ByVal value As Integer)
				_documentArtifactID = value
			End Set
		End Property
#End Region

		Public Shared Function op_Equality(ByVal fi1 As FileInfo, ByVal fi2 As FileInfo) As Boolean
			Dim areEqual As Boolean
			areEqual = fi1.DocumentArtifactID = fi2.DocumentArtifactID
			areEqual = areEqual And fi1.Filename = fi2.Filename
			areEqual = areEqual And fi1.Order = fi2.Order
			areEqual = areEqual And fi1.Type = fi2.Type
			Return areEqual
		End Function
	End Class
End Namespace

