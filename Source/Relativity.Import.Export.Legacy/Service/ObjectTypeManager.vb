Namespace kCura.WinEDDS.Service
	Public Class ObjectTypeManager
		Inherits kCura.EDDS.WebAPI.ObjectTypeManagerBase.ObjectTypeManager

#Region " Constructor and Initialization "
		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()

			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}ObjectTypeManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

#End Region

#Region " Shadow Implementations "

		Public Shadows Function RetrieveAllUploadable(ByVal caseContextArtifactID As Int32) As System.Data.DataSet
			Return RetryOnReLoginException(Function() MyBase.RetrieveAllUploadable(caseContextArtifactID))
		End Function

#End Region

	End Class
End Namespace
