Namespace kCura.WinEDDS.Service
	Public Class SearchManager
		Inherits kCura.EDDS.WebAPI.SearchManagerBase.SearchManager

		Public Sub New(ByVal credentials As Net.NetworkCredential)
			MyBase.New()
			Me.Credentials = credentials
			Me.Url = String.Format("{0}SearchManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

	End Class
End Namespace