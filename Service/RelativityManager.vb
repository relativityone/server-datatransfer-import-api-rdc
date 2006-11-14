Namespace kCura.WinEDDS.Service
	Public Class RelativityManager
		Inherits kCura.EDDS.WebAPI.RelativityManagerBase.RelativityManager

		Public Sub New(ByVal credentials As Net.NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()
			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}RelativityManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

		Public Shadows Function ValidateSuccesfulLogin() As Boolean
			Dim retVal As Boolean
			If kCura.WinEDDS.Config.UsesWebAPI Then
				retVal = MyBase.ValidateSuccessfulLogin
			Else
				'retVal = _productionManager.ExternalRetrieveProducedByContextArtifactID(contextArtifactID, _identity)
			End If
			Return retVal
		End Function
	End Class
End Namespace