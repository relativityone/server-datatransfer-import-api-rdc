Namespace kCura.WinEDDS.Service
	Public Class Helper
		Public Shared Sub AttemptReLogin(ByVal credentials As System.net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer)
			Dim manager As New UserManager(DirectCast(credentials, System.Net.NetworkCredential), cookieContainer)
			manager.AttemptReLogin()
		End Sub
	End Class
End Namespace