Namespace kCura.WinEDDS.Service
	Public Class Helper
		Public Shared Sub AttemptReLogin(ByVal credentials As System.Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer, ByVal tryNumber As Int32)
			AttemptReLogin(credentials, cookieContainer, tryNumber)
		End Sub

		Public Shared Sub AttemptReLogin(ByVal credentials As System.Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer, ByVal tryNumber As Int32, ByVal webURL As String)
			System.Threading.Thread.CurrentThread.Join(CType((1000 * (tryNumber + 1)), Int32))
			Dim manager As New UserManager(DirectCast(credentials, System.Net.NetworkCredential), cookieContainer)
			manager.ServiceURL = webURL

			manager.AttemptReLogin()
		End Sub
	End Class
End Namespace