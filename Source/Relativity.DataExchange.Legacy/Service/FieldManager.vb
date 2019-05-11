Imports Relativity.DataExchange
Imports Relativity.DataExchange.Service

Namespace kCura.WinEDDS.Service
	Public Class FieldManager
		Inherits kCura.EDDS.WebAPI.FieldManagerBase.FieldManager
		Implements Export.IFieldManager

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

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()

			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			_query = New kCura.WinEDDS.Service.FieldQuery(credentials, Me.CookieContainer)
			Me.Url = String.Format("{0}FieldManager.asmx", AppSettings.Instance.WebApiServiceUrl)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub


#Region " Translations "
		Public Shared Function DTOtoDocumentField(ByVal dto As kCura.EDDS.WebAPI.DocumentManagerBase.Field) As DocumentField
			Dim retval As New DocumentField(dto.DisplayName, dto.ArtifactID, dto.FieldTypeID, dto.FieldCategoryID, dto.CodeTypeID, dto.MaxLength, dto.AssociativeArtifactTypeID, dto.UseUnicodeEncoding, dto.ImportBehavior, dto.EnableDataGrid)
			If retval.FieldCategoryID = FieldCategory.FullText Then
				retval.Value = System.Text.ASCIIEncoding.ASCII.GetString(DirectCast(dto.Value, Byte()))
			ElseIf retval.FieldTypeID = FieldType.Code OrElse retval.FieldTypeID = FieldType.MultiCode Then
				retval.Value = (DirectCast(dto.Value, Int32())).ToCsv().Replace(",", ";")
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

#End Region

#Region " Shadow Functions "
		Public Shadows Function Create(ByVal caseContextArtifactID As Int32, ByVal field As kCura.EDDS.WebAPI.FieldManagerBase.Field) As Int32
			Return RetryOnReLoginException(Function() MyBase.Create(caseContextArtifactID, field))
		End Function

		Public Shadows Function Read(ByVal caseContextArtifactID As Int32, ByVal fieldArtifactID As Int32) As kCura.EDDS.WebAPI.FieldManagerBase.Field Implements Export.IFieldManager.Read
			Return RetryOnReLoginException(Function() MyBase.Read(caseContextArtifactID, fieldArtifactID))
		End Function
#End Region

	End Class
End Namespace