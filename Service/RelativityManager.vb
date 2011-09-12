Namespace kCura.WinEDDS.Service
	Public Class RelativityManager
		Inherits kCura.EDDS.WebAPI.RelativityManagerBase.RelativityManager

		Private ReadOnly _serviceURLPageFormat As String

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer)
			Me.New(credentials, cookieContainer, kCura.WinEDDS.Config.WebServiceURL)
		End Sub

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer, ByVal webURL As String)
			MyBase.New()

			_serviceURLPageFormat = "{0}RelativityManager.asmx"
			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.ServiceURL = webURL
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

		Public Overridable Property ServiceURL As String
			Get
				Return Me.Url
			End Get
			Set(ByVal value As String)
				Me.Url = String.Format(_serviceURLPageFormat, value)
			End Set
		End Property

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

		Public Shadows Function IsImportEmailNotificationEnabled() As Boolean
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Return MyBase.IsImportEmailNotificationEnabled()
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries, ServiceURL)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

		Public Shadows Function RetrieveRdcConfiguration() As System.Data.DataSet
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Return MyBase.RetrieveRdcConfiguration()
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries, ServiceURL)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function
	End Class
End Namespace