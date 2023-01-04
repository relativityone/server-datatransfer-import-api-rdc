Imports kCura.WinEDDS.Service.Replacement
Imports Relativity.DataExchange
Namespace kCura.WinEDDS.Service
	Public Class RelativityManager
		Inherits kCura.EDDS.WebAPI.RelativityManagerBase.RelativityManager
		Implements Replacement.IRelativityManager

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer)
			Me.New(credentials, cookieContainer, AppSettings.Instance.WebApiServiceUrl)
		End Sub

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer, ByVal webServiceUrl As String)
			Me.New(credentials, cookieContainer, AppSettings.Instance.WebApiServiceUrl, AppSettings.Instance.WebApiOperationTimeout)
		End Sub

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer, ByVal webServiceUrl As String, ByVal timeout As Integer)
			MyBase.New()

			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}RelativityManager.asmx", AppSettings.Instance.ValidateUriFormat(webServiceUrl))
			Me.Timeout = timeout
		End Sub

		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

		Public Shadows Function RetrieveCurrencySymbol() As String Implements Replacement.IRelativityManager.RetrieveCurrencySymbol
			Return RetryOnReLoginException(Function() MyBase.RetrieveCurrencySymbol())
		End Function

		Public Shadows Function ValidateSuccessfulLogin() As Boolean Implements Replacement.IRelativityManager.ValidateSuccessfulLogin
			Return MyBase.ValidateSuccessfulLogin
		End Function

		Public Function ValidateCertificate() As Boolean Implements IRelativityManager.ValidateCertificate
			Return ValidateSuccessfulLogin()
		End Function

		Public Shadows Function IsImportEmailNotificationEnabled() As Boolean Implements Replacement.IRelativityManager.IsImportEmailNotificationEnabled
			Return RetryOnReLoginException(Function() MyBase.IsImportEmailNotificationEnabled())
		End Function

		Public Shadows Function RetrieveRdcConfiguration() As System.Data.DataSet Implements Replacement.IRelativityManager.RetrieveRdcConfiguration
			Return RetryOnReLoginException(Function() MyBase.RetrieveRdcConfiguration())
		End Function

		Public Shadows Function GetImportExportWebApiVersion() As String
			Return RetryOnReLoginException(Function() MyBase.GetImportExportWebApiVersion())
		End Function
	End Class
End Namespace