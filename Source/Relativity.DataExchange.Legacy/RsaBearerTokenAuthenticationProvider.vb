Imports Relativity.DataExchange.Service
Imports Relativity.Transfer

''' <summary>
''' This class represents method of refreshing the token credentials on the expiration event
''' Instance of this class is injected to Tapi
''' </summary>
Public Class RsaBearerTokenAuthenticationProvider
	 Implements IAuthenticationTokenProvider

	Public Function GenerateToken() As String Implements IAuthenticationTokenProvider.GenerateToken
		Return System.Security.Claims.ClaimsPrincipal.Current.Claims.AccessToken()
	End Function
End Class
