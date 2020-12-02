Imports System.Threading
Imports Relativity.DataExchange

Namespace kCura.WinEDDS.Api

	''' <summary>
	''' Defines helper methods for authentication in Relativity
	''' </summary>
	Public Class LoginHelper
		Public Shared Function LoginUsernamePassword(ByVal username As String,
													 ByVal password As String,
													 ByVal cookieContainer As Net.CookieContainer,
													 ByVal runningContext As IRunningContext) As System.Net.NetworkCredential
			Return LoginUsernamePassword(
				username,
				password,
				cookieContainer,
				AppSettings.Instance.WebApiServiceUrl,
				runningContext)
		End Function

		Public Shared Function LoginUsernamePassword(ByVal username As String,
													 ByVal password As String,
													 ByVal cookieContainer As Net.CookieContainer,
													 ByVal webServiceUrl As String,
													 ByVal runningContext As IRunningContext) As System.Net.NetworkCredential
			Return LoginUsernamePassword(
				username,
				password,
				cookieContainer,
				AppSettings.Instance.WebApiServiceUrl,
				CancellationToken.None,
				runningContext,
				New Global.Relativity.Logging.NullLogger())
		End Function

		Public Shared Function LoginUsernamePassword(ByVal username As String,
													 ByVal password As String,
													 ByVal cookieContainer As Net.CookieContainer,
													 ByVal webServiceUrl As String,
													 ByVal token As CancellationToken,
													 ByVal runningContext As IRunningContext,
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
				ValidateVersionCompatibility(credentials, cookieContainer, webServiceUrl, token, runningContext, logger)
				Return credentials
			End Using
		End Function

		Public Shared Sub ValidateVersionCompatibility(ByVal credential As System.Net.NetworkCredential,
														ByVal cookieContainer As Net.CookieContainer,
														ByVal webServiceUrl As String,
														ByVal token As CancellationToken,
														ByVal runningContext As IRunningContext,
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
			ValidateVersionCompatibilityAsync(instanceInfo, token, runningContext, logger).GetAwaiter().GetResult()
		End Sub

		Friend Shared Function ValidateVersionCompatibilityAsync(ByVal instanceInfo As RelativityInstanceInfo,
																 ByVal token As CancellationToken,
																 ByVal runningContext As IRunningContext,
																 ByVal logger As Global.Relativity.Logging.ILog) As System.Threading.Tasks.Task
			' Automatically throws RelativityNotSupportedException when the validation fails. 
			Dim compatibilityCheck As IImportExportCompatibilityCheck = New ImportExportCompatibilityCheck(
				instanceInfo,
				New kCura.WinEDDS.Service.ApplicationVersionService(instanceInfo, AppSettings.Instance, logger),
				runningContext,
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