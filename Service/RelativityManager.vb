Namespace kCura.WinEDDS.Service
	Public Class RelativityManager
		Inherits kCura.EDDS.WebAPI.RelativityManagerBase.RelativityManager

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer)
			Me.New(credentials, cookieContainer, kCura.WinEDDS.Config.WebServiceURL)
		End Sub

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer, ByVal webServiceUrl As String)
			MyBase.New()

			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}RelativityManager.asmx", webServiceUrl)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

		Public Shadows Function ValidateSuccesfulLogin() As Boolean
			Return MyBase.ValidateSuccessfulLogin
		End Function

		Public Shadows Function IsImportEmailNotificationEnabled() As Boolean
			Return RetryOnReLoginException(Function() MyBase.IsImportEmailNotificationEnabled())
		End Function

		Public Shadows Function RetrieveRdcConfiguration() As System.Data.DataSet
			Return RetryOnReLoginException(Function() MyBase.RetrieveRdcConfiguration())
		End Function
	End Class
End Namespace