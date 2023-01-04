Imports System.Threading
Imports kCura.WinEDDS.Api
Imports kCura.WinEDDS.Credentials
Imports kCura.WinEDDS.Service
Imports Relativity.DataExchange

Public Class WindowsCredentialsLogin 
			''' <summary>
		''' This method retrieves bearer token using OAuth Implicit Flow and Integrated Windows Authentication.
		''' Implementation of implicit flow token provider requires to be run from interactive process.
		''' So it should be used only from Desktop or Console apps. After successful login, compatibility with
		''' Relativity instance is validated.
		''' </summary>
		''' <param name="cookieContainer">Cookie container</param>
		''' <param name="webServiceUrl">URL of RelativityWebApi service</param>
		''' <param name="token">cancellation token for compatibility check</param>
		''' <param name="runningContext">Object containing information about job context.</param>
		''' <param name="logger">logger</param>
		''' <returns>Bearer token <see cref="Net.NetworkCredential"/></returns>
		Public Shared Function LoginWindowsAuth(ByVal cookieContainer As System.Net.CookieContainer,
													ByVal webServiceUrl As String,
													ByVal token As CancellationToken,
													ByVal runningContext As IRunningContext,
													ByVal logger As Global.Relativity.Logging.ILog) As System.Net.NetworkCredential
		' Windows Authentication is used only to authenticate in a implicit flow process which returns credentials used in the import process
		' RDC behavior is similar, it tries to authenticate in RelativityManager using Windows Authentication
		' but it always fails and fall back on implicit flow. Please see comment in Application.AttemptWindowsAuthentication
		' which was added in commit ba46946f (01 May 2019)
		Dim windowsAuthCredentials As System.Net.NetworkCredential = DirectCast(System.Net.CredentialCache.DefaultCredentials, System.Net.NetworkCredential)
		Using windowsAuthRelativityManager As kCura.WinEDDS.Service.Replacement.IRelativityManager = ManagerFactory.CreateRelativityManager(windowsAuthCredentials, cookieContainer, AddressOf Global.Relativity.Desktop.Client.Application.Instance.GetCorrelationId)
			Dim provider As IntegratedAuthenticationOAuthCredentialsProvider = New IntegratedAuthenticationOAuthCredentialsProvider(windowsAuthRelativityManager)
			Dim credentials As System.Net.NetworkCredential = provider.LoginWindowsAuth()
			LoginHelper.ValidateVersionCompatibility(credentials, cookieContainer, webServiceUrl, token, runningContext, logger, AddressOf Global.Relativity.Desktop.Client.Application.Instance.GetCorrelationId)
			Return credentials
		End Using
	End Function
End Class
