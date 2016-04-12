Namespace kCura.WinEDDS.Service
	Public Class RelativityManager
		Inherits kCura.EDDS.WebAPI.RelativityManagerBase.RelativityManager

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer)
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
			Return MyBase.ValidateSuccessfulLogin
		End Function

		Public Shadows Function IsImportEmailNotificationEnabled() As Boolean
			Return RetryOnReLoginException(Function() MyBase.IsImportEmailNotificationEnabled())
		End Function

		Public Shadows Function IsCoffeeInstance() As Boolean
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Return MyBase.IsCoffeeInstance()
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

		Public Shadows Function RetrieveRdcConfiguration() As System.Data.DataSet
			Return RetryOnReLoginException(Function() MyBase.RetrieveRdcConfiguration())
		End Function
	End Class
End Namespace