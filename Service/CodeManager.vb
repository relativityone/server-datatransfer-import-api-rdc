Namespace kCura.WinEDDS.Service
	Public Class CodeManager
		Inherits kCura.EDDS.WebAPI.CodeManagerBase.CodeManager

		Public Sub New(ByVal credentials As Net.NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()
			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}CodeManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

		Public Function CreateNewCodeDTOProxy(ByVal codeTypeID As Int32, ByVal name As String, ByVal order As Int32, ByVal caseID As Int32) As kCura.EDDS.WebAPI.CodeManagerBase.Code
			Dim code As New kCura.EDDS.WebAPI.CodeManagerBase.Code
			code.CodeType = codeTypeID
			code.IsActive = True			 'HACK: Hard-coded
			code.Name = name
			code.Order = order
			code.Keywords = String.Empty
			code.Notes = "Automatically generated by the kIT: " & System.DateTime.Now			 'HACK: Hard-coded
			code.ParentArtifactID = New NullableTypes.NullableInt32(caseID)
			code.ContainerID = New NullableTypes.NullableInt32(caseID)
			Return code
		End Function
	End Class
End Namespace