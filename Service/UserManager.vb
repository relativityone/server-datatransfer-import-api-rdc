Namespace kCura.WinEDDS.Service
	Public Class UserManager
		Inherits kCura.EDDS.WebAPI.UserManagerBase.UserManager

		Private Shared _cookieSyncLock As New System.Object

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()

			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}UserManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

#Region " Shadow Methods "
		Public Shadows Function Login(ByVal emailAddress As String, ByVal password As String) As Boolean
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Try
					'ClearCookiesBeforeLogin call MUST be made before Login web method is called
					SyncLock _cookieSyncLock
						MyBase.ClearCookiesBeforeLogin()
						Return MyBase.Login(emailAddress, password)
					End SyncLock
				Catch ex As System.Exception
					Throw
				End Try
			End If
			Return Nothing
		End Function

		Public Shadows Function RetrieveAllAssignableInCase(ByVal caseContextArtifactID As Int32) As System.Data.DataSet
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.RetrieveAllAssignableInCase(caseContextArtifactID)
					Else
						'Return Relativity.Core.Service.FileQuery.RetrieveFullTextFilesForDocuments(_identity, artifactID, documentArtifactIDs).ToDataSet()
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

#End Region

		Public Function AttemptReLogin() As Boolean
			Try
				System.Threading.Thread.CurrentThread.Join(Config.WaitBeforeReconnect)
				Dim cred As System.Net.NetworkCredential = DirectCast(Me.Credentials, System.Net.NetworkCredential)
				If Not Me.Credentials Is System.Net.CredentialCache.DefaultCredentials Then Me.Login(cred.UserName, cred.Password)
				kCura.WinEDDS.Service.Settings.AuthenticationToken = Me.GenerateDistributedAuthenticationToken()
				Return True
			Catch ex As System.Exception
				Return False
			End Try
		End Function

	End Class
End Namespace