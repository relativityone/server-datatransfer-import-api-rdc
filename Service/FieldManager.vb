Namespace kCura.WinEDDS.Service
	Public Class FieldManager
		Inherits kCura.EDDS.WebAPI.FieldManagerBase.FieldManager

		Private _query As kCura.WinEDDS.Service.FieldQuery

		Public ReadOnly Property Query() As kCura.WinEDDS.Service.FieldQuery
			Get
				Return _query
			End Get
		End Property

		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

		Public Sub New(ByVal credentials As Net.NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()
			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}FieldManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
			_query = New kCura.WinEDDS.Service.FieldQuery(credentials, Me.CookieContainer)
		End Sub

		Public Shared Function DTOtoDocumentField(ByVal dto As kCura.EDDS.WebAPI.DocumentManagerBase.Field) As DocumentField
			Dim retval As New DocumentField(dto.DisplayName, dto.ArtifactID, dto.FieldTypeID, dto.FieldCategoryID, dto.CodeTypeID, dto.MaxLength)
			If retval.FieldTypeID = kCura.EDDS.Types.FieldCategory.FullText Then
				retval.Value = System.Text.ASCIIEncoding.ASCII.GetString(DirectCast(dto.Value, Byte()))
			Else
				retval.Value = dto.Value.ToString
			End If
			Return retval
		End Function

		Public Shared Function DTOsToDocumentField(ByVal dtos As kCura.EDDS.WebAPI.DocumentManagerBase.Field()) As DocumentField()
			Dim documentFields(dtos.Length - 1) As DocumentField
			Dim i As Int32
			For i = 0 To documentFields.Length - 1
				documentFields(i) = DTOtoDocumentField(dtos(i))
			Next
			Return documentFields
		End Function
	End Class
End Namespace