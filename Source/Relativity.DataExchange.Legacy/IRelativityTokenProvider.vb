Namespace kCura.WinEDDS

	''' /// <summary>
	''' ///Provides method to obtain valid Relativity authentication access token
	''' /// </summary>
	Public Interface IRelativityTokenProvider

		Function GetToken() As String

	End Interface

End Namespace