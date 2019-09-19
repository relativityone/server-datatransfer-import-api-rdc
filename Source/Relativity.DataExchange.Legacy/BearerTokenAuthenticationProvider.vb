Imports Relativity.DataExchange.Service
Imports Relativity.Transfer

Public Class BearerTokenAuthenticationProvider
	 Implements IAuthenticationTokenProvider

	Public Function GenerateToken() As String Implements IAuthenticationTokenProvider.GenerateToken
		Return System.Security.Claims.ClaimsPrincipal.Current.Claims.AccessToken()
	End Function
End Class
