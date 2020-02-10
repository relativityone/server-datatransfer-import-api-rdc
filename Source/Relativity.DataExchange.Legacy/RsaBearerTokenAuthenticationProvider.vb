Imports kCura.WinEDDS
Imports Relativity.DataExchange.Service
Imports Relativity.Transfer

''' <summary>
''' This class obtains current claims principal token that can be used by Relativity Service Account hosted processes
''' </summary>
Friend Class RsaBearerTokenAuthenticationProvider
	Implements IRelativityTokenProvider

	Public Function GetToken() As String Implements IRelativityTokenProvider.GetToken
		Return System.Security.Claims.ClaimsPrincipal.Current.Claims.AccessToken()
	End Function
End Class
