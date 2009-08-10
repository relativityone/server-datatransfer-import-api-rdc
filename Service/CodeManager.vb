Namespace kCura.WinEDDS.Service
	Public Class CodeManager
		Inherits kCura.EDDS.WebAPI.CodeManagerBase.CodeManager

		Public Sub New(ByVal credentials As Net.NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()
			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}CodeManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

		Public Function CreateNewCodeDTOProxy(ByVal codeTypeID As Int32, ByVal name As String, ByVal order As Int32, ByVal caseSystemID As Int32) As kCura.EDDS.WebAPI.CodeManagerBase.Code
			Dim code As New kCura.EDDS.WebAPI.CodeManagerBase.Code
			code.CodeType = codeTypeID
			code.IsActive = True
			code.Name = name
			code.Order = order
			code.Keywords = String.Empty
			code.Notes = String.Empty
			code.ParentArtifactID = New NullableTypes.NullableInt32(caseSystemID)
			code.ContainerID = New NullableTypes.NullableInt32(caseSystemID)
			Return code
		End Function

#Region " Shadow Functions "
		Public Shadows Function RetrieveCodesAndTypesForCase(ByVal caseContextArtifactID As Int32) As System.Data.DataSet
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.RetrieveCodesAndTypesForCase(caseContextArtifactID)
					Else
						'Return _codeManager.ExternalRetrieveCodesAndTypesForCase(caseContextArtifactID, _identity)
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
		End Function

		Public Shadows Function Create(ByVal caseContextArtifactID As Int32, ByVal code As kCura.EDDS.WebAPI.CodeManagerBase.Code) As Object
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Return MyBase.Create(caseContextArtifactID, code)
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
		End Function
#End Region

	End Class
End Namespace