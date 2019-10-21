Imports System.Threading
Imports kCura.WinEDDS.Credentials
Imports Relativity.DataExchange

Namespace kCura.WinEDDS.Api

	''' <summary>
	''' Defines helper methods for authentication in Relativity
	''' </summary>
	Public Class LoginHelper
		Public Shared Function LoginUsernamePassword(ByVal username As String,
													 ByVal password As String,
													 ByVal cookieContainer As Net.CookieContainer) As System.Net.NetworkCredential
			Return LoginUsernamePassword(
				username,
				password,
				cookieContainer,
				AppSettings.Instance.WebApiServiceUrl)
		End Function

		Public Shared Function LoginUsernamePassword(ByVal username As String,
													 ByVal password As String,
													 ByVal cookieContainer As Net.CookieContainer,
													 ByVal webServiceUrl As String) As System.Net.NetworkCredential
			Return LoginUsernamePassword(
				username,
				password,
				cookieContainer,
				AppSettings.Instance.WebApiServiceUrl,
				CancellationToken.None,
				New Global.Relativity.Logging.NullLogger())
		End Function

		Public Shared Function LoginUsernamePassword(ByVal username As String,
													 ByVal password As String,
													 ByVal cookieContainer As Net.CookieContainer,
													 ByVal webServiceUrl As String,
													 ByVal token As CancellationToken,
													 ByVal logger As Global.Relativity.Logging.ILog) As System.Net.NetworkCredential
			If cookieContainer Is Nothing Then
				Throw New ArgumentException(NameOf(cookieContainer))
			End If

			If String.IsNullOrEmpty(webServiceUrl) Then
				Throw New ArgumentException(NameOf(webServiceUrl))
			End If

			If logger Is Nothing Then
				Throw New ArgumentException(NameOf(logger))
			End If

			webServiceUrl = AppSettings.Instance.ValidateUriFormat(webServiceUrl)

			' Note: Do NOT perform the compatibility check until all legacy initializations are complete.
			Dim credentials As New Net.NetworkCredential(username, password)
			Using userManager As New kCura.WinEDDS.Service.UserManager(credentials, cookieContainer, webServiceUrl)
				If Not userManager.Login(username, password) Then
					Return Nothing
				End If
			End Using

			Using relManager As New kCura.WinEDDS.Service.RelativityManager(credentials, cookieContainer, webServiceUrl)
				Initialize(relManager, webServiceUrl)
				ValidateVersionCompatibility(credentials, cookieContainer, webServiceUrl, token, logger)
				Return credentials
			End Using
		End Function

		''' <summary>
		''' This method retrieves bearer token using OAuth Implicit Flow and Integrated Windows Authentication.
		''' Implementation of implicit flow token provider requires to be run from interactive process.
		''' So it should be used only from Desktop or Console apps. After successful login, compatibility with
		''' Relativity instance is validated.
		''' </summary>
		''' <param name="cookieContainer">Cookie container</param>
		''' <param name="webServiceUrl">URL of RelativityWebApi service</param>
		''' <param name="token">cancellation token for compatibility check</param>
		''' <param name="logger">logger</param>
		''' <returns>Bearer token <see cref="Net.NetworkCredential"/></returns>
		Public Shared Function LoginWindowsAuth(ByVal cookieContainer As System.Net.CookieContainer,
													ByVal webServiceUrl As String,
													ByVal token As CancellationToken,
													ByVal logger As Global.Relativity.Logging.ILog) As System.Net.NetworkCredential
			' Windows Authentication is used only to authenticate in a implicit flow process which returns credentials used in the import process
			' RDC behavior is similar, it tries to authenticate in RelativityManager using Windows Authentication
			' but it always fails and fall back on implicit flow. Please see comment in Application.AttemptWindowsAuthentication
			' which was added in commit ba46946f (01 May 2019)
			Dim windowsAuthCredentials As System.Net.NetworkCredential = DirectCast(System.Net.CredentialCache.DefaultCredentials, System.Net.NetworkCredential)
			Using windowsAuthRelativityManager As New kCura.WinEDDS.Service.RelativityManager(windowsAuthCredentials, cookieContainer, webServiceUrl)
				Dim provider As IntegratedAuthenticationOAuthCredentialsProvider = New IntegratedAuthenticationOAuthCredentialsProvider(windowsAuthRelativityManager)
				Dim credentials As System.Net.NetworkCredential = provider.LoginWindowsAuth()
				ValidateVersionCompatibility(credentials, cookieContainer, webServiceUrl, token, logger)
				Return credentials
			End Using
		End Function

		Public Shared Sub ValidateVersionCompatibility(ByVal credential As System.Net.NetworkCredential,
													   ByVal cookieContainer As Net.CookieContainer,
													   ByVal webServiceUrl As String,
													   ByVal token As CancellationToken,
													   ByVal logger As Global.Relativity.Logging.ILog)
			If credential Is Nothing Then
				Throw New ArgumentException(NameOf(credential))
			End If

			If cookieContainer Is Nothing Then
				Throw New ArgumentException(NameOf(cookieContainer))
			End If

			If String.IsNullOrEmpty(webServiceUrl) Then
				Throw New ArgumentException(NameOf(webServiceUrl))
			End If

			If logger Is Nothing Then
				Throw New ArgumentException(NameOf(logger))
			End If

			' This method is executed synchronously.
			webServiceUrl = AppSettings.Instance.ValidateUriFormat(webServiceUrl)
			Dim instanceInfo As New RelativityInstanceInfo With
					{
					.Credentials = credential,
					.CookieContainer = cookieContainer,
					.WebApiServiceUrl = New Uri(webServiceUrl)
					}
			ValidateVersionCompatibilityAsync(instanceInfo, token, logger).GetAwaiter().GetResult()
		End Sub

		Friend Shared Function ValidateVersionCompatibilityAsync(ByVal instanceInfo As RelativityInstanceInfo,
																 ByVal token As CancellationToken,
																 ByVal logger As Global.Relativity.Logging.ILog) As System.Threading.Tasks.Task
			' Automatically throws RelativityNotSupportedException when the validation fails. 
			Dim compatibilityCheck As IImportExportCompatibilityCheck = New ImportExportCompatibilityCheck(
				instanceInfo,
				New kCura.WinEDDS.Service.ApplicationVersionService(instanceInfo, AppSettings.Instance, logger),
				logger)
			Return compatibilityCheck.ValidateAsync(token)
		End Function

		Private Shared Sub Initialize(ByVal relativityManager As kCura.WinEDDS.Service.RelativityManager, ByVal webServiceUrl As String)
			Dim locale As New System.Globalization.CultureInfo(System.Globalization.CultureInfo.CurrentCulture.LCID, True)
			locale.NumberFormat.CurrencySymbol = relativityManager.RetrieveCurrencySymbol()
			System.Threading.Thread.CurrentThread.CurrentCulture = locale

			Using userManager As kCura.WinEDDS.Service.UserManager = New kCura.WinEDDS.Service.UserManager(relativityManager.Credentials, relativityManager.CookieContainer, webServiceUrl)
				kCura.WinEDDS.Service.Settings.AuthenticationToken = userManager.GenerateDistributedAuthenticationToken()
			End Using
		End Sub
	End Class
End Namespace