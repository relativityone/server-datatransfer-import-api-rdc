Imports kCura.WinEDDS
Imports Relativity.Transfer

''' This class represents method of refreshing the token credentials on the expiration event
''' Instance of this class is injected to Tapi
Public Class AuthTokenProviderAdapter
	Implements IAuthenticationTokenProvider

	Private ReadOnly _relativityTokenProvider As IRelativityTokenProvider

	Public Sub New(relativityTokenProvider As IRelativityTokenProvider)
		_relativityTokenProvider = relativityTokenProvider
	End Sub

	Public Function GenerateToken() As String Implements IAuthenticationTokenProvider.GenerateToken
		Return _relativityTokenProvider.GetToken()
	End Function

End Class
