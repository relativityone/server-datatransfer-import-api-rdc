Namespace kCura.WinEDDS.Service
	Public Class UserManager
		Inherits kCura.EDDS.WebAPI.UserManagerBase.UserManager

		Public Sub New(ByVal credentials As Net.NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()
			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}userManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

#Region " Shadow Methods "
		Public Shadows Function Login(ByVal emailAddress As String, ByVal password As String) As kCura.EDDS.EDDSIdentity
			If kCura.WinEDDS.Config.UsesWebAPI Then
				If Not MyBase.Login(emailAddress, password) Then
					Throw New kCura.WinEDDS.Exception.InvalidLoginException
				End If
				Return Nothing
			Else
				Dim loginManager As New kCura.EDDS.Service.LoginManager

				Try
					Dim id As kCura.EDDS.EDDSIdentity = loginManager.Login(emailAddress, password)
					If id Is Nothing Then Throw New kCura.WinEDDS.Exception.InvalidLoginException
					Return id
				Catch ex As System.Exception
					Throw New kCura.WinEDDS.Exception.InvalidLoginException
				End Try
			End If
		End Function
#End Region

	End Class
End Namespace