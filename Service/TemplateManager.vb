Namespace kCura.WinEDDS.Service
	Public Class TemplateManager
		Inherits kCura.EDDS.WebAPI.TemplateManagerBase.TemplateManager

		Private ReadOnly _serviceURLPageFormat As String

#Region " Constructors "

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()

			_serviceURLPageFormat = "{0}TemplateManager.asmx"
			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.ServiceURL = kCura.WinEDDS.Config.WebServiceURL
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

#End Region

		Public Overridable Property ServiceURL As String
			Get
				Return Me.Url
			End Get
			Set(ByVal value As String)
				Me.Url = String.Format(_serviceURLPageFormat, value)
			End Set
		End Property

#Region " Shadow Methods "

		Public Shadows Function InstallTemplate(ByVal appsToOverride As Int32(), ByVal template As System.Xml.XmlNode, ByVal installationParameters As kCura.EDDS.WebAPI.TemplateManagerBase.ApplicationInstallationParameters) As kCura.EDDS.WebAPI.TemplateManagerBase.ApplicationInstallationResult
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Return MyBase.InstallTemplate(appsToOverride, template, installationParameters)
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

#End Region

#Region " Overrides Methods "

		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

#End Region

	End Class
End Namespace