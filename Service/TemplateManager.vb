Namespace kCura.WinEDDS.Service
	Public Class TemplateManager
		Inherits kCura.EDDS.WebAPI.TemplateManagerBase.TemplateManager

#Region " Constructors "

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()

			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}TemplateManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

#End Region

#Region " Shadow Methods "

		Public Shadows Function InstallTemplate(ByVal appsToOverride As Int32(), packageData As Byte(), ByVal installationParameters As kCura.EDDS.WebAPI.TemplateManagerBase.ApplicationInstallationParameters) As kCura.EDDS.WebAPI.TemplateManagerBase.ApplicationInstallationResult
			Return RetryOnReLoginException(Function() MyBase.InstallTemplate(appsToOverride, packageData, installationParameters))
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