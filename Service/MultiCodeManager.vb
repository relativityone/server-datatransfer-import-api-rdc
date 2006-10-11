Namespace kCura.WinEDDS.Service
	Public Class MultiCodeManager
		Inherits kCura.EDDS.WebAPI.MultiCodeManagerBase.MultiCodeManager

		'Private _multiCodeManager As New kCura.Code.Service.MultiCodeManager

		Public Sub New(ByVal credentials As Net.NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()
			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}MultiCodeManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

#Region " Shadow Functions "
		Public Shadows Sub DeleteFromMultiCodeArtifactByMultiCodeID(ByVal artifactID As Int32, ByVal multiCodeID As Int32)
			If kCura.WinEDDS.Config.UsesWebAPI Then
				MyBase.DeleteFromMultiCodeArtifactByMultiCodeID(artifactID, multiCodeID)
			Else
				'_multiCodeManager.DeleteFromMultiCodeArtifactByMultiCodeID(artifactID, multiCodeID)
			End If
		End Sub

		Public Shadows Function CreateNewMultiCodeID(ByVal artifactID As Int32) As Int32
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.CreateNewMultiCodeID(artifactID)
			Else
				'Return _multiCodeManager.CreateNewMultiCodeID(artifactID)
			End If
		End Function

		Public Shadows Sub SetMultiCodeValues(ByVal multiCodeID As Int32, ByVal values As System.Collections.ArrayList)
			If kCura.WinEDDS.Config.UsesWebAPI Then
				MyBase.SetMultiCodeValues(multiCodeID, values.ToArray)
			Else
				'_multiCodeManager.SetMultiCodeValues(multiCodeID, values)
			End If
		End Sub
#End Region

	End Class
End Namespace