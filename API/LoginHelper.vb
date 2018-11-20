Imports kCura.WinEDDS.Credentials

Namespace kCura.WinEDDS.Api
	Public Class LoginHelper
		Private Shared relativityManager As kCura.WinEDDS.Service.RelativityManager

		Public Shared Function LoginWindowsAuth(ByVal cookieContainer As System.Net.CookieContainer) As System.Net.NetworkCredential
			If cookieContainer Is Nothing Then Throw New ArgumentException("Cookie container not set")
			Dim myHttpWebRequest As System.Net.HttpWebRequest
			Dim cred As System.Net.NetworkCredential

			cred = DirectCast(System.Net.CredentialCache.DefaultCredentials, System.Net.NetworkCredential)

			Dim requestURIString As String = kCura.WinEDDS.Config.WebServiceURL & "RelativityManager.asmx"
			myHttpWebRequest = DirectCast(System.Net.WebRequest.Create(requestURIString), System.Net.HttpWebRequest)
			myHttpWebRequest.Credentials = cred
			relativityManager = New kCura.WinEDDS.Service.RelativityManager(cred, cookieContainer)

			If relativityManager.ValidateSuccesfulLogin Then
				CheckVersion(relativityManager)
				Initialize(relativityManager, kCura.WinEDDS.Config.WebServiceURL)
				Return cred
			End If
			Return Nothing
		End Function

		Public Shared Function LoginWindowsAuthTapi() As System.Net.NetworkCredential
			Dim provider As IntegratedAuthenticationOAuthCredentialsProvider = New IntegratedAuthenticationOAuthCredentialsProvider(relativityManager)
			Return provider.LoginWindowsAuthTapi()
		End Function

		Public Shared Function LoginUsernamePassword(ByVal username As String, ByVal password As String, ByVal cookieContainer As Net.CookieContainer) As System.Net.NetworkCredential
			Return LoginUsernamePassword(username, password, cookieContainer, kCura.WinEDDS.Config.WebServiceURL)
		End Function

		Public Shared Function LoginUsernamePassword(ByVal username As String, ByVal password As String, ByVal cookieContainer As Net.CookieContainer, ByVal webServiceUrl As String) As System.Net.NetworkCredential
			webServiceUrl = kCura.WinEDDS.Config.ValidateURIFormat(webServiceUrl)
			If cookieContainer Is Nothing Then Throw New ArgumentException("Cookie container not set")
			Dim credential As New Net.NetworkCredential(username, password)
			Dim userManager As New kCura.WinEDDS.Service.UserManager(credential, cookieContainer, webServiceUrl)
			Dim relativityManager As New kCura.WinEDDS.Service.RelativityManager(credential, cookieContainer, webServiceUrl)

			CheckVersion(relativityManager)
			If userManager.Login(username, password) Then
				Initialize(relativityManager, webServiceUrl)
				Return credential
			End If
			Return Nothing
		End Function

		Public Shared Function CreateRelativityVersionMismatchException(ByVal relativityVersion As String) As RelativityVersionMismatchException

			' Rely on the process executable to craft a more accurate exception message.
			Dim assembly As System.Reflection.Assembly = System.Reflection.Assembly.GetEntryAssembly()
			If assembly Is Nothing Then
				assembly = System.Reflection.Assembly.GetExecutingAssembly()
			End If

			Return CreateRelativityVersionMismatchException(relativityVersion, assembly)
		End Function

		Public Shared Function CreateRelativityVersionMismatchException(ByVal relativityVersion As String, ByVal assembly As System.Reflection.Assembly) As RelativityVersionMismatchException
			
			If assembly Is Nothing Then
				Throw New ArgumentNullException(NameOf(assembly))
			End If

			' Favor the supplied application name
			Dim applicationName As String = Config.ApplicationName
			If String.IsNullOrEmpty(applicationName) Then
				If assembly.GetName.Name.StartsWith("kCura.EDDS.WinForm", StringComparison.OrdinalIgnoreCase) Then
					applicationName = "Relativity Desktop Client"
				Else
					applicationName = "Import API"
				End If
			End If

			Dim clientVersion As String = assembly.GetName.Version.ToString()
			Return CreateRelativityVersionMismatchException(relativityVersion, clientVersion, applicationName)
		End Function

		Public Shared Function CreateRelativityVersionMismatchException(ByVal relativityVersion As String, ByVal clientVersion As String, ByVal applicationName As String) As RelativityVersionMismatchException

			' Because this is existing code, avoid arg checks and just supply a default value.
			Const UnknownVersion As String = "(unknown)"
			If String.IsNullOrEmpty(relativityVersion) Then
				relativityVersion = UnknownVersion
			End If

			If String.IsNullOrEmpty(clientVersion) Then
				clientVersion = UnknownVersion
			End If

			If String.IsNullOrEmpty(applicationName) Then
				applicationName = UnknownVersion
			End If

			Dim message As String = $"applicationName version of {applicationName} ({clientVersion _
				    }) is out of date. Please make sure you're running the correct version ({relativityVersion _
				}) or the correct Relativity WebService URL is specified."
			Return New RelativityVersionMismatchException(message, relativityVersion, clientVersion)
		End Function

		Private Shared Sub Initialize(ByVal relativityManager As kCura.WinEDDS.Service.RelativityManager, ByVal webServiceUrl As String)
			Dim locale As New System.Globalization.CultureInfo(System.Globalization.CultureInfo.CurrentCulture.LCID, True)
			locale.NumberFormat.CurrencySymbol = relativityManager.RetrieveCurrencySymbol
			System.Threading.Thread.CurrentThread.CurrentCulture = locale

			Dim userMan As kCura.WinEDDS.Service.UserManager = New kCura.WinEDDS.Service.UserManager(relativityManager.Credentials, relativityManager.CookieContainer, webServiceUrl)

			kCura.WinEDDS.Service.Settings.AuthenticationToken = userMan.GenerateDistributedAuthenticationToken()
		End Sub

		Private Shared Sub CheckVersion(relativityManager As Service.RelativityManager)
			Dim winVersionString As String = System.Reflection.Assembly.GetExecutingAssembly.FullName.Split(","c)(1).Split("="c)(1)
			Dim winRelativityVersion As String() = winVersionString.Split("."c)
			Dim relVersionString As String = relativityManager.RetrieveRelativityVersion
			Dim relativityWebVersion As String() = relVersionString.Split("."c)
			Dim match As Boolean = True
			Dim i As Int32
			For i = 0 To System.Math.Max(winRelativityVersion.Length - 1, relativityWebVersion.Length - 1)
				Dim winv As String = "*"
				Dim relv As String = "*"
				If i <= winRelativityVersion.Length - 1 Then winv = winRelativityVersion(i)
				If i <= relativityWebVersion.Length - 1 Then relv = relativityWebVersion(i)
				If Not (relv = "*" OrElse winv = "*" OrElse relv.ToLower = winv.ToLower) Then
					match = False
					Exit For
				End If
			Next
			If Not match Then
				Throw CreateRelativityVersionMismatchException(relVersionString)
			End If
		End Sub
	End Class
End Namespace

