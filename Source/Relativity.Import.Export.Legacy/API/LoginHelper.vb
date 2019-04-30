Imports System.Threading
Imports kCura.WinEDDS.Credentials
Imports Relativity.Import.Export
Imports Relativity.Logging

Namespace kCura.WinEDDS.Api
	Public Class LoginHelper
		''' <summary>
		''' The default application name used when a null or empty value is specified.
		''' </summary>
		Friend Const DefaultApplicationName As String = "Import API"

		''' <summary>
		''' The default version used when a null or empty value is specified.
		''' </summary>
		Friend Const DefaultUnknownVersion As String = "0.0.0.0"

		Private Shared ReadOnly _logger As ILog = RelativityLogFactory.CreateLog()

		Private Shared relativityManager As kCura.WinEDDS.Service.RelativityManager

		Public Shared Function LoginWindowsAuth(ByVal cookieContainer As System.Net.CookieContainer) As System.Net.NetworkCredential
			If cookieContainer Is Nothing Then Throw New ArgumentException("Cookie container not set")
			Dim myHttpWebRequest As System.Net.HttpWebRequest
			Dim cred As System.Net.NetworkCredential

			cred = DirectCast(System.Net.CredentialCache.DefaultCredentials, System.Net.NetworkCredential)

			Dim requestURIString As String = AppSettings.Instance.WebApiServiceUrl & "RelativityManager.asmx"
			myHttpWebRequest = DirectCast(System.Net.WebRequest.Create(requestURIString), System.Net.HttpWebRequest)
			myHttpWebRequest.Credentials = cred
			relativityManager = New kCura.WinEDDS.Service.RelativityManager(cred, cookieContainer)

			If relativityManager.ValidateSuccesfulLogin Then
				Initialize(relativityManager, AppSettings.Instance.WebApiServiceUrl)
				Return cred
			End If
			Return Nothing
		End Function

		Public Shared Function LoginWindowsAuthTapi() As System.Net.NetworkCredential
			Dim provider As IntegratedAuthenticationOAuthCredentialsProvider = New IntegratedAuthenticationOAuthCredentialsProvider(relativityManager)
			Return provider.LoginWindowsAuthTapi()
		End Function

		Public Shared Function LoginUsernamePassword(ByVal username As String, ByVal password As String, ByVal cookieContainer As Net.CookieContainer) As System.Net.NetworkCredential
			Return LoginUsernamePassword(username, password, cookieContainer, AppSettings.Instance.WebApiServiceUrl)
		End Function

		Public Shared Function LoginUsernamePassword(ByVal username As String,
													 ByVal password As String,
													 ByVal cookieContainer As Net.CookieContainer,
													 ByVal webServiceUrl As String) As System.Net.NetworkCredential
			webServiceUrl = AppSettings.Instance.ValidateUriFormat(webServiceUrl)
			If cookieContainer Is Nothing Then
				Throw New ArgumentException("Cookie container not set")
			End If

			' Note: Do NOT perform the compatibility check until all legacy initializations are complete.
			Dim credential As New Net.NetworkCredential(username, password)
			Using userManager As New kCura.WinEDDS.Service.UserManager(credential, cookieContainer, webServiceUrl)
				If Not userManager.Login(username, password) Then
					Return Nothing
				End If
			End Using

			Using relManager As New kCura.WinEDDS.Service.RelativityManager(credential, cookieContainer, webServiceUrl)
				Initialize(relManager, webServiceUrl)
				ValidateVersionCompatibility(credential, cookieContainer, webServiceUrl, CancellationToken.None)
				Return credential
			End Using
		End Function

		Public Shared Function CreateRelativityVersionMismatchMessage(ByVal relativityVersion As String, ByVal clientVersion As String, ByVal applicationName As String) As String

			' Because this is existing code, avoid arg checks and just supply a default value.
			If String.IsNullOrEmpty(relativityVersion) Then
				relativityVersion = DefaultUnknownVersion
			End If

			If String.IsNullOrEmpty(clientVersion) Then
				clientVersion = DefaultUnknownVersion
			End If

			If String.IsNullOrEmpty(applicationName) Then
				applicationName = DefaultApplicationName
			End If

			Dim message As String = $"Your version of {applicationName} ({clientVersion _
					}) is out of date. Please make sure you're running the correct version ({relativityVersion _
					}) or the correct Relativity WebService URL is specified."
			Return message
		End Function

		Public Shared Sub ValidateVersionCompatibility(ByVal credential As System.Net.NetworkCredential,
													   ByVal cookieContainer As Net.CookieContainer,
													   ByVal webServiceUrl As String,
													   ByVal token As CancellationToken)
			' This method is executed synchronously.
			Dim instanceInfo As New RelativityInstanceInfo With
					{
					.Credentials = credential,
					.CookieContainer = cookieContainer,
					.WebApiServiceUrl = New Uri(webServiceUrl)
					}
			ValidateVersionCompatibilityAsync(instanceInfo, token).GetAwaiter().GetResult()
		End Sub

		Friend Shared Function ValidateVersionCompatibilityAsync(ByVal instanceInfo As RelativityInstanceInfo, ByVal token As CancellationToken) As System.Threading.Tasks.Task
			' Automatically throws RelativityNotSupportedException when the validation fails. 
			Dim compatibilityCheck As IImportExportCompatibilityCheck = New ImportExportCompatibilityCheck(
				instanceInfo,
				New kCura.WinEDDS.Service.ApplicationVersionService(instanceInfo, AppSettings.Instance, _logger),
				_logger)
			Return compatibilityCheck.ValidateAsync(token)
		End Function

		Private Shared Sub Initialize(ByVal relativityManager As kCura.WinEDDS.Service.RelativityManager, ByVal webServiceUrl As String)
			Dim locale As New System.Globalization.CultureInfo(System.Globalization.CultureInfo.CurrentCulture.LCID, True)
			locale.NumberFormat.CurrencySymbol = relativityManager.RetrieveCurrencySymbol
			System.Threading.Thread.CurrentThread.CurrentCulture = locale

			Dim userMan As kCura.WinEDDS.Service.UserManager = New kCura.WinEDDS.Service.UserManager(relativityManager.Credentials, relativityManager.CookieContainer, webServiceUrl)

			kCura.WinEDDS.Service.Settings.AuthenticationToken = userMan.GenerateDistributedAuthenticationToken()
		End Sub
	End Class
End Namespace