Namespace kCura.WinEDDS.Service
	Public Class Helper

		'TODO: Precious tests! We needs it, we wants it! 
		Public Shared Sub AttemptReLogin(ByVal credentials As System.Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer, ByVal tryNumber As Int32, Optional ByVal retryOnFailure As Boolean = True)
			System.Threading.Thread.CurrentThread.Join(CType((1000 * (tryNumber + 1)), Int32))
			Dim manager As New UserManager(DirectCast(credentials, System.Net.NetworkCredential), cookieContainer)

			manager.AttemptReLogin(retryOnFailure)
		End Sub
	End Class
End Namespace