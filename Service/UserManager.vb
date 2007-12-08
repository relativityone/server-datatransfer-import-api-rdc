Namespace kCura.WinEDDS.Service
	Public Class UserManager
		Inherits kCura.EDDS.WebAPI.UserManagerBase.UserManager

		Public Sub New(ByVal credentials As Net.NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()
			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}userManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

#Region " Shadow Methods "
		Public Shadows Function Login(ByVal emailAddress As String, ByVal password As String) As Boolean
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Try
					Return MyBase.Login(emailAddress, password)
				Catch ex As System.Exception
					Throw
				End Try
			End If
		End Function

		Public Shadows Function RetrieveAllAssignableInCase(ByVal caseContextArtifactID As Int32) As System.Data.DataSet
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.RetrieveAllAssignableInCase(caseContextArtifactID)
					Else
						'Return kCura.EDDS.Service.FileQuery.RetrieveFullTextFilesForDocuments(_identity, artifactID, documentArtifactIDs).ToDataSet()
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException Then
						If ex.ToString.IndexOf("NeedToReLoginException") <> -1 Then
							If tries < Config.MaxReloginTries Then
								tries += 1
								Try
									Dim creds As System.Net.NetworkCredential = DirectCast(Me.Credentials, System.Net.NetworkCredential)
									Dim userManager As New userManager(creds, Me.CookieContainer)
									userManager.Login(creds.UserName, creds.Password)
									kCura.WinEDDS.Service.Settings.AuthenticationToken = userManager.GetLatestAuthenticationToken()
								Catch
								End Try
							End If
						End If
						If tries >= Config.MaxReloginTries Then
							Throw
						End If
					Else
						Throw
					End If
				End Try
			End While
		End Function

#End Region

	End Class
End Namespace