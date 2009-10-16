Namespace kCura.WinEDDS.Service
	Public Class TemplateManager
		Inherits kCura.EDDS.WebAPI.TemplateManagerBase.TemplateManager

#Region " Constructors "

		Public Sub New(ByVal credentials As Net.NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()
			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}TemplateManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

#End Region

#Region " Shadow Methods "

		Public Shadows Function InstallTemplate(ByVal template As System.Xml.XmlNode, ByVal installationParameters As kCura.EDDS.WebAPI.TemplateManagerBase.ApplicationInstallationParameters) As kCura.EDDS.WebAPI.TemplateManagerBase.ApplicationInstallationResult
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Return MyBase.InstallTemplate(template, installationParameters)
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
		End Function

#End Region

	End Class
End Namespace