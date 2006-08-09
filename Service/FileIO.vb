Namespace kCura.WinEDDS.Service
	Public Class FileIO
		Inherits kCura.EDDS.WebAPI.FileIOBase.FileIO
		Public Sub New(ByVal credentials As Net.NetworkCredential)
			MyBase.New()
			Me.Credentials = credentials
			Me.Url = String.Format("{0}FileIO.asmx", kCura.WinEDDS.Config.WebServiceURL)
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