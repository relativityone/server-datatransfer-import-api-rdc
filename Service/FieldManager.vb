Namespace kCura.WinEDDS.Service
	Public Class FieldManager
		Inherits kCura.EDDS.WebAPI.FieldManagerBase.FieldManager

		Private _query As kCura.WinEDDS.Service.FieldQuery

		'TODO: FIX THIS SHIZZITY!
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

#Region " Translations "
		Public Shared Function DTOtoDocumentField(ByVal dto As kCura.EDDS.WebAPI.DocumentManagerBase.Field) As DocumentField
			Dim retval As New DocumentField(dto.DisplayName, dto.ArtifactID, dto.FieldTypeID, dto.FieldCategoryID, dto.CodeTypeID, dto.MaxLength, dto.UseUnicodeEncoding)
			If retval.FieldCategoryID = kCura.DynamicFields.Types.FieldCategory.FullText Then
				retval.Value = System.Text.ASCIIEncoding.ASCII.GetString(DirectCast(dto.Value, Byte()))
			ElseIf retval.FieldTypeID = kCura.DynamicFields.Types.FieldTypeHelper.FieldType.Code OrElse retval.FieldTypeID = kCura.DynamicFields.Types.FieldTypeHelper.FieldType.MultiCode Then
				retval.Value = kCura.Utility.Array.IntArrayToCSV(DirectCast(dto.Value, Int32())).Replace(",", ";")
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

		'Public Shared Function DTOtoDocumentWebAPIField(ByVal dto As kCura.EDDS.DynamicFields.DTO.Field) As kCura.EDDS.WebAPI.DocumentManagerBase.Field
		'	Dim retVal As New kCura.EDDS.WebAPI.DocumentManagerBase.Field

		'	retVal.DisplayName = dto.DisplayName
		'	retVal.ArtifactID = dto.ArtifactID
		'	retVal.FieldTypeID = dto.FieldTypeID
		'	retVal.FieldCategoryID = dto.FieldCategoryID
		'	retVal.CodeTypeID = dto.CodeTypeID
		'	retVal.MaxLength = dto.MaxLength
		'	retVal.IsArtifactBaseField = dto.IsArtifactBaseField
		'	If retVal.FieldCategoryID = kCura.EDDS.Types.FieldCategory.FullText Then
		'		retVal.Value = System.Text.ASCIIEncoding.ASCII.GetString(DirectCast(dto.Value, Byte()))
		'	Else
		'		retVal.Value = dto.Value.ToString
		'	End If
		'	Return retVal
		'End Function

		'Public Shared Function DTOtoFieldWebAPIField(ByVal dto As kCura.EDDS.DynamicFields.DTO.Field) As kCura.EDDS.WebAPI.FieldManagerBase.Field
		'	Dim retVal As New kCura.EDDS.WebAPI.FieldManagerBase.Field

		'	retVal.DisplayName = dto.DisplayName
		'	retVal.ArtifactID = dto.ArtifactID
		'	retVal.FieldTypeID = dto.FieldTypeID
		'	retVal.FieldCategoryID = dto.FieldCategoryID
		'	retVal.CodeTypeID = dto.CodeTypeID
		'	retVal.MaxLength = dto.MaxLength
		'	retVal.IsArtifactBaseField = dto.IsArtifactBaseField
		'	If retVal.FieldCategoryID = kCura.EDDS.Types.FieldCategory.FullText Then
		'		retVal.Value = System.Text.ASCIIEncoding.ASCII.GetString(DirectCast(dto.Value, Byte()))
		'	Else
		'		retVal.Value = dto.Value.ToString
		'	End If
		'	Return retVal
		'End Function

		'Public Shared Function DTOsToWebAPIFields(ByVal dtos As kCura.EDDS.DynamicFields.DTO.Field()) As kCura.EDDS.WebAPI.DocumentManagerBase.Field()
		'	Dim documentFields(dtos.Length - 1) As kCura.EDDS.WebAPI.DocumentManagerBase.Field
		'	Dim i As Int32
		'	For i = 0 To documentFields.Length - 1
		'		documentFields(i) = DTOtoDocumentWebAPIField(dtos(i))
		'	Next
		'	Return documentFields
		'End Function

		'Public Shared Function WebAPIFieldtoDTO(ByVal field As kCura.EDDS.WebAPI.DocumentManagerBase.Field) As kCura.EDDS.DynamicFields.DTO.Field
		'	Dim retVal As New kCura.EDDS.DynamicFields.DTO.Field

		'	retVal.DisplayName = field.DisplayName
		'	retVal.ArtifactID = field.ArtifactID
		'	retVal.FieldTypeID = field.FieldTypeID
		'	retVal.FieldCategoryID = field.FieldCategoryID
		'	retVal.CodeTypeID = field.CodeTypeID
		'	retVal.MaxLength = field.MaxLength
		'	retVal.IsArtifactBaseField = field.IsArtifactBaseField
		'	retVal.Value = field.Value.ToString
		'	Return retVal
		'End Function

		'Public Shared Function WebAPIFieldtoDTO(ByVal field As kCura.EDDS.WebAPI.FieldManagerBase.Field) As kCura.EDDS.DynamicFields.DTO.Field
		'	Dim retVal As New kCura.EDDS.DynamicFields.DTO.Field

		'	retVal.DisplayName = field.DisplayName
		'	retVal.ArtifactID = field.ArtifactID
		'	retVal.FieldTypeID = field.FieldTypeID
		'	retVal.FieldCategoryID = field.FieldCategoryID
		'	retVal.CodeTypeID = field.CodeTypeID
		'	retVal.MaxLength = field.MaxLength
		'	retVal.IsArtifactBaseField = field.IsArtifactBaseField
		'	retVal.Value = field.Value.ToString
		'	Return retVal
		'End Function

		'Public Shared Function WebAPIFieldsToDTOs(ByVal fields As kCura.EDDS.WebAPI.DocumentManagerBase.Field()) As kCura.EDDS.DynamicFields.DTO.Field()
		'	Dim dtos(fields.Length - 1) As kCura.EDDS.DynamicFields.DTO.Field
		'	Dim i As Int32
		'	For i = 0 To dtos.Length - 1
		'		dtos(i) = WebAPIFieldtoDTO(fields(i))
		'	Next
		'	Return dtos
		'End Function
#End Region

#Region " Shadow Functions "
		Public Shadows Function Create(ByVal caseContextArtifactID As Int32, ByVal field As kCura.EDDS.WebAPI.FieldManagerBase.Field) As Int32
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.Create(caseContextArtifactID, field)
					Else
						'Return _fieldManager.ExternalCreate(Me.WebAPIFieldtoDTO(field), _identity)
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer)
					Else
						Throw
					End If
				End Try
			End While
		End Function

		Public Shadows Function Read(ByVal caseContextArtifactID As Int32, ByVal fieldArtifactID As Int32) As kCura.EDDS.WebAPI.FieldManagerBase.Field
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.Read(caseContextArtifactID, fieldArtifactID)
					Else
						'Return Me.DTOtoFieldWebAPIField(_fieldManager.Read(fieldArtifactID, _identity))
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer)
					Else
						Throw
					End If
				End Try
			End While
		End Function
#End Region

	End Class
End Namespace