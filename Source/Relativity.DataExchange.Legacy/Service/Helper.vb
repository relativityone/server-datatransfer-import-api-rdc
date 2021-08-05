Imports System.Net
Imports System.Threading.Tasks
Imports kCura.WinEDDS.Credentials

Namespace kCura.WinEDDS.Service
	Public Class Helper

		'TODO: Precious tests! We needs it, we wants it! 
		Public Shared Sub AttemptReLogin(ByVal credentials As System.Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer, ByVal tryNumber As Int32, Optional ByVal retryOnFailure As Boolean = True)
			System.Threading.Thread.CurrentThread.Join(1000 * (tryNumber + 1))
			Dim newCredentials As NetworkCredential = GetUpdatedCredentials(credentials)

			' This is used only by legacy WebApi Soap services to re login (it is not used by any Kepler service) so we don't need to set correlation id.
			Dim manager As Replacement.IUserManager = ManagerFactory.CreateUserManager(newCredentials, cookieContainer, Nothing)

			manager.AttemptReLogin(retryOnFailure)
		End Sub

		Public Shared Async Function GetUpdatedCredentialsAsync(ByVal defaultCredentials As System.Net.ICredentials) As Task(Of System.Net.NetworkCredential)
			dim result As System.Net.NetworkCredential

			Try
				If(Not RelativityWebApiCredentialsProvider.Instance().CredentialsSet())
					result = DirectCast(defaultCredentials, System.Net.NetworkCredential)
				Else 
					dim updatedWebApiCredentials As System.Net.NetworkCredential = Await RelativityWebApiCredentialsProvider.Instance().GetCredentialsAsync()
					Dim originalCredential As NetworkCredential = DirectCast(defaultCredentials, System.Net.NetworkCredential)
					originalCredential.Password = updatedWebApiCredentials.Password
					result = updatedWebApiCredentials
				End If
			Catch ex As Exception
				result = DirectCast(defaultCredentials, System.Net.NetworkCredential)
			End Try

			return result
		End Function

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