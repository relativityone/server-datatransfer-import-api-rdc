Imports System.Threading
Imports kCura.WinEDDS.Service
Imports Relativity.DataExchange

Namespace kCura.WinEDDS.Api

	''' <summary>
	''' Defines helper methods for authentication in Relativity
	''' </summary>
	Public Class LoginHelper
		Public Shared Function LoginUsernamePassword(ByVal username As String,
													 ByVal password As String,
													 ByVal cookieContainer As Net.CookieContainer,
													 ByVal runningContext As IRunningContext,
													 ByVal correlationIdFunc As Func(Of String)) As System.Net.NetworkCredential
			Return LoginUsernamePassword(
				username,
				password,
				cookieContainer,
				AppSettings.Instance.WebApiServiceUrl,
				runningContext,
				correlationIdFunc)
		End Function

		Public Shared Function LoginUsernamePassword(ByVal username As String,
													 ByVal password As String,
													 ByVal cookieContainer As Net.CookieContainer,
													 ByVal webServiceUrl As String,
													 ByVal runningContext As IRunningContext,
													 ByVal correlationIdFunc As Func(Of String)) As System.Net.NetworkCredential
			Return LoginUsernamePassword(
				username,
				password,
				cookieContainer,
				AppSettings.Instance.WebApiServiceUrl,
				CancellationToken.None,
				runningContext,
				New Global.Relativity.Logging.NullLogger(),
				correlationIdFunc)
		End Function

		Public Shared Function LoginUsernamePassword(ByVal username As String,
													 ByVal password As String,
													 ByVal cookieContainer As Net.CookieContainer,
													 ByVal webServiceUrl As String,
													 ByVal token As CancellationToken,
													 ByVal runningContext As IRunningContext,
													 ByVal logger As Global.Relativity.Logging.ILog,
													 ByVal correlationIdFunc As Func(Of String)) As System.Net.NetworkCredential
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
			Using userManager As kCura.WinEDDS.Service.Replacement.IUserManager = ManagerFactory.CreateUserManager(credentials, cookieContainer, webServiceUrl, correlationIdFunc)
				If Not userManager.Login(username, password) Then
					Return Nothing
				End If
			End Using

			Using relManager As kCura.WinEDDS.Service.Replacement.IRelativityManager = ManagerFactory.CreateRelativityManager(credentials, cookieContainer, correlationIdFunc)
				Initialize(relManager, webServiceUrl, credentials, cookieContainer, correlationIdFunc)
				ValidateVersionCompatibility(credentials, cookieContainer, webServiceUrl, token, runningContext, logger, correlationIdFunc)
				Return credentials
			End Using
		End Function

		Public Shared Sub ValidateVersionCompatibility(ByVal credential As System.Net.NetworkCredential,
														ByVal cookieContainer As Net.CookieContainer,
														ByVal webServiceUrl As String,
														ByVal token As CancellationToken,
														ByVal runningContext As IRunningContext,
														ByVal logger As Global.Relativity.Logging.ILog,
														ByVal correlationIdFunc As Func(Of String))
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
			ValidateVersionCompatibilityAsync(instanceInfo, token, runningContext, logger, correlationIdFunc).GetAwaiter().GetResult()
		End Sub

		Friend Shared Function ValidateVersionCompatibilityAsync(ByVal instanceInfo As RelativityInstanceInfo,
																 ByVal token As CancellationToken,
																 ByVal runningContext As IRunningContext,
																 ByVal logger As Global.Relativity.Logging.ILog,
																 ByVal correlationIdFunc As Func(Of String)) As System.Threading.Tasks.Task
			' Automatically throws RelativityNotSupportedException when the validation fails. 
			Dim compatibilityCheck As IImportExportCompatibilityCheck = New ImportExportCompatibilityCheck(
				instanceInfo,
				ManagerFactory.CreateApplicationVersionService(instanceInfo, AppSettings.Instance, logger, correlationIdFunc),
				runningContext,
				logger)
			Return compatibilityCheck.ValidateAsync(token)
		End Function

		Private Shared Sub Initialize(ByVal relativityManager As kCura.WinEDDS.Service.Replacement.IRelativityManager, ByVal webServiceUrl As String, ByVal credentials As Net.NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer, ByVal correlationIdFunc As Func(Of String))
			Dim locale As New System.Globalization.CultureInfo(System.Globalization.CultureInfo.CurrentCulture.LCID, True)
			locale.NumberFormat.CurrencySymbol = relativityManager.RetrieveCurrencySymbolV2()
			System.Threading.Thread.CurrentThread.CurrentCulture = locale

			Using userManager As kCura.WinEDDS.Service.Replacement.IUserManager = ManagerFactory.CreateUserManager(credentials, cookieContainer, webServiceUrl, correlationIdFunc)
				kCura.WinEDDS.Service.Settings.AuthenticationToken = userManager.GenerateDistributedAuthenticationToken()
			End Using
		End Sub
	End Class
End Namespace