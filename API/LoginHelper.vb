Namespace kCura.WinEDDS.Api

	Public Class LoginHelper
		Public Shared Function LoginWindowsAuth(ByVal cookieContainer As System.Net.CookieContainer) As System.Net.NetworkCredential
			If cookieContainer Is Nothing Then Throw New ArgumentException("Cookie container not set")
			Dim myHttpWebRequest As System.Net.HttpWebRequest
			Dim myHttpWebResponse As System.Net.HttpWebResponse
			Dim cred As System.Net.NetworkCredential
			Dim relativityManager As kCura.WinEDDS.Service.RelativityManager

			cred = DirectCast(System.Net.CredentialCache.DefaultCredentials, System.Net.NetworkCredential)
			'myHttpWebRequest = DirectCast(System.Net.WebRequest.Create(kCura.WinEDDS.Config.WebServiceURL & "\RelativityManager.asmx"), System.Net.HttpWebRequest)
			Dim requestURIString As String = kCura.WinEDDS.Config.WebServiceURL & "RelativityManager.asmx"
			myHttpWebRequest = DirectCast(System.Net.WebRequest.Create(requestURIString), System.Net.HttpWebRequest)
			myHttpWebRequest.Credentials = cred
			myHttpWebResponse = DirectCast(myHttpWebRequest.GetResponse(), System.Net.HttpWebResponse)
			relativityManager = New kCura.WinEDDS.Service.RelativityManager(cred, cookieContainer)
			If relativityManager.ValidateSuccesfulLogin Then
				CheckVersion(cred, cookieContainer)
				Initialize(relativityManager)
				Return cred
			End If
			Return Nothing
		End Function

		Public Shared Function LoginUsernamePassword(ByVal username As String, ByVal password As String, ByVal cookieContainer As Net.CookieContainer) As System.Net.NetworkCredential
			If cookieContainer Is Nothing Then Throw New ArgumentException("Cookie container not set")
			Dim credential As New Net.NetworkCredential(username, password)
			Dim userManager As New kCura.WinEDDS.Service.UserManager(credential, cookieContainer)
			Dim relativityManager As New kCura.WinEDDS.Service.RelativityManager(credential, cookieContainer)
			CheckVersion(credential, cookieContainer)
			If userManager.Login(username, password) Then
				Initialize(relativityManager)
				Return credential
			End If
			Return Nothing
		End Function

		Private Shared Sub Initialize(ByVal relativityManager As kCura.WinEDDS.Service.RelativityManager)
			Dim locale As New System.Globalization.CultureInfo(System.Globalization.CultureInfo.CurrentCulture.LCID, True)
			locale.NumberFormat.CurrencySymbol = relativityManager.RetrieveCurrencySymbol
			System.Threading.Thread.CurrentThread.CurrentCulture = locale
			kCura.WinEDDS.Service.Settings.AuthenticationToken = New kCura.WinEDDS.Service.UserManager(relativityManager.Credentials, relativityManager.CookieContainer).GenerateDistributedAuthenticationToken()
		End Sub

		Private Shared Sub CheckVersion(ByVal credential As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer)
			Dim relativityManager As New kCura.WinEDDS.Service.RelativityManager(credential, cookieContainer)
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
			If Not match Then Throw New RelativityVersionMismatchException(relVersionString)
		End Sub
	End Class
End Namespace

