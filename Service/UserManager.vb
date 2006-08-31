Namespace kCura.WinEDDS.Service
	Public Class UserManager
		Inherits kCura.EDDS.WebAPI.UserManagerBase.UserManager

		Public Sub New(ByVal credentials As Net.NetworkCredential)
			MyBase.New()
			Me.Credentials = credentials
			Me.Url = String.Format("{0}userManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub
	End Class
End Namespace