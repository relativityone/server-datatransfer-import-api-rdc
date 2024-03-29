Imports Relativity.DataExchange
Imports Relativity.DataExchange.Service

Namespace kCura.WinEDDS.Service
	Public Class UserManager
		Inherits kCura.EDDS.WebAPI.UserManagerBase.UserManager
		Implements IReLoginService
		Implements Replacement.IUserManager

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer)
			Me.New(credentials, cookieContainer, AppSettings.Instance.WebApiServiceUrl)
		End Sub

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer, ByVal webServiceUrl As String)
			MyBase.New()

			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}UserManager.asmx", AppSettings.Instance.ValidateUriFormat(webServiceUrl))
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

#Region " Shadow Methods "
		Public Shadows Function Login(ByVal emailAddress As String, ByVal password As String) As Boolean Implements Replacement.IUserManager.Login
			Return RetryOnReLoginException(Function() Me.LoginInternal(emailAddress, password))
		End Function

		Public Shadows Sub Logout() Implements Replacement.IUserManager.Logout
			MyBase.Logout()
		End Sub

		Private Function LoginInternal(ByVal emailAddress As String, ByVal password As String) As Boolean
			Try
				'ClearCookiesBeforeLogin call MUST be made before Login web method is called
				MyBase.ClearCookiesBeforeLogin()
				Return MyBase.Login(emailAddress, password)
			Catch ex As System.Exception
				Throw
			End Try
		End Function

		Public Shadows Function RetrieveAllAssignableInCase(ByVal caseContextArtifactID As Int32) As System.Data.DataSet Implements Replacement.IUserManager.RetrieveAllAssignableInCase
			Return RetryOnReLoginException(Function() Me.RetrieveAllAssignableInCaseInternal(caseContextArtifactID))
		End Function

		Private Function RetrieveAllAssignableInCaseInternal(ByVal caseContextArtifactID As Int32) As System.Data.DataSet
			Return MyBase.RetrieveAllAssignableInCase(caseContextArtifactID)
		End Function

		Public Shadows Function GenerateAuthenticationToken() As String
			Return RetryOnReLoginException(Function() MyBase.GenerateAuthenticationToken())
		End Function

		Public Shadows Function GenerateDistributedAuthenticationToken(Optional ByVal retryOnFailure As Boolean = True) As String Implements Replacement.IUserManager.GenerateDistributedAuthenticationToken
			Return RetryOnReLoginException(Function() MyBase.GenerateDistributedAuthenticationToken(), retryOnFailure)
		End Function

		Public Shadows Function LoggedIn() As Boolean Implements Replacement.IUserManager.LoggedIn
			Return MyBase.LoggedIn()
		End Function

#End Region

		Public Function AttemptReLogin(Optional ByVal retryOnFailure As Boolean = True) As Boolean Implements IReLoginService.AttemptReLogin
			Try
				System.Threading.Thread.CurrentThread.Join(AppSettings.Instance.WaitBeforeReconnect)
				Dim cred As System.Net.NetworkCredential = DirectCast(Me.Credentials, System.Net.NetworkCredential)
				If Not Me.Credentials Is System.Net.CredentialCache.DefaultCredentials Then
					If String.IsNullOrEmpty(cred.Password) Then
						Dim relativityManager As New kCura.WinEDDS.Service.RelativityManager(cred, Me.CookieContainer)

						relativityManager.ValidateSuccessfulLogin()
					Else
						Me.Login(cred.UserName, cred.Password)
					End If
				End If
				kCura.WinEDDS.Service.Settings.AuthenticationToken = Me.GenerateDistributedAuthenticationToken(retryOnFailure)
				Return True
			Catch ex As System.Exception
				Return False
			End Try
		End Function
	End Class
End Namespace