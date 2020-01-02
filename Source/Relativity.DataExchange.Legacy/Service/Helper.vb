Imports kCura.WinEDDS.Credentials

Namespace kCura.WinEDDS.Service
	Public Class Helper

		'TODO: Precious tests! We needs it, we wants it! 
		Public Shared Sub AttemptReLogin(ByVal credentials As System.Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer, ByVal tryNumber As Int32, Optional ByVal retryOnFailure As Boolean = True)
			System.Threading.Thread.CurrentThread.Join(1000 * (tryNumber + 1))
			credentials = GetUpdatedCredentials(credentials)
			Dim manager As New UserManager(credentials, cookieContainer)

			manager.AttemptReLogin(retryOnFailure)
		End Sub

		Private Shared Function GetUpdatedCredentials(ByVal defaultCredentials As System.Net.ICredentials) As System.Net.NetworkCredential
			dim result As System.Net.NetworkCredential

			Try
				If(Not RelativityWebApiCredentialsProvider.Instance().CredentialsSet())
					result = DirectCast(defaultCredentials, System.Net.NetworkCredential)
				Else 
					dim updatedWebApiCredentials As System.Net.NetworkCredential = RelativityWebApiCredentialsProvider.Instance().GetCredentials()
					result = updatedWebApiCredentials
				End If
			Catch ex As Exception
				result = DirectCast(defaultCredentials, System.Net.NetworkCredential)
			End Try

			return result
		End Function
	End Class
End Namespace