Imports System.Threading
Imports kCura.WinEDDS.Credentials
Imports Relativity.Import.Export

Namespace kCura.WinEDDS.Api
	Public Class LoginHelper

		Private Shared _windowsAuthRelativityManager As kCura.WinEDDS.Service.RelativityManager

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

		Public Shared Function LoginWindowsAuth(ByVal cookieContainer As System.Net.CookieContainer,
												ByVal webServiceUrl As String,
												ByVal token As CancellationToken,
												ByVal logger As Global.Relativity.Logging.ILog) As System.Net.NetworkCredential
			If cookieContainer Is Nothing Then
				Throw New ArgumentException(NameOf(cookieContainer))
			End If

			If String.IsNullOrEmpty(webServiceUrl) Then
				Throw New ArgumentException(NameOf(webServiceUrl))
			End If

			Dim credentials As System.Net.NetworkCredential = DirectCast(System.Net.CredentialCache.DefaultCredentials, System.Net.NetworkCredential)
			webServiceUrl = AppSettings.Instance.ValidateUriFormat(webServiceUrl)
			if (Not _windowsAuthRelativityManager Is Nothing)
				_windowsAuthRelativityManager.Dispose()
			End If

			_windowsAuthRelativityManager = New kCura.WinEDDS.Service.RelativityManager(credentials, cookieContainer, webServiceUrl)
			If _windowsAuthRelativityManager.ValidateSuccesfulLogin() Then
				Initialize(_windowsAuthRelativityManager, webServiceUrl)
				ValidateVersionCompatibility(credentials, cookieContainer, webServiceUrl, token, logger)
				Return credentials
			End If
			Return Nothing
		End Function

		Public Shared Function LoginWindowsAuthTapi(ByVal cookieContainer As System.Net.CookieContainer,
		                                            ByVal webServiceUrl As String,
		                                            ByVal token As CancellationToken,
		                                            ByVal logger As Global.Relativity.Logging.ILog) As System.Net.NetworkCredential
			' Commit 21e69fd0 introduced the expectation that LoginWindowsAuth was called right before this method.
			If _windowsAuthRelativityManager Is Nothing Then
				Throw New InvalidOperationException("This operation cannot be completed because a Windows authentication logic error exists.")
			End If

			Try
				Dim provider As IntegratedAuthenticationOAuthCredentialsProvider = New IntegratedAuthenticationOAuthCredentialsProvider(_windowsAuthRelativityManager)
				Dim credentials As System.Net.NetworkCredential = provider.LoginWindowsAuthTapi()
				ValidateVersionCompatibility(credentials, cookieContainer, webServiceUrl, token, logger)
				Return credentials
			Finally
				_windowsAuthRelativityManager.Dispose()
				_windowsAuthRelativityManager = Nothing
			End Try
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