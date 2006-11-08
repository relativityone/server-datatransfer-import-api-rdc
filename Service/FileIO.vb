Namespace kCura.WinEDDS.Service
	Public Class FileIO
		Inherits kCura.EDDS.WebAPI.FileIOBase.FileIO

		'Dim _externalIOManager As New kCura.EDDS.Service.ExternalIO

		Public Sub New(ByVal credentials As Net.NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()
			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}FileIO.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

#Region " Shadow Functions "
		Public Shadows Function BeginFill(ByVal caseContextArtifactID As Int32, ByVal b() As Byte, ByVal fileGuid As String) As String
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.BeginFill(caseContextArtifactID, b, fileGuid)
			Else
				'Return _externalIOManager.ExternalBeginFill(b, contextArtifactID, fileGuid)
			End If
		End Function

		Public Shadows Function FileFill(ByVal caseContextArtifactID As Int32, ByVal fileName As String, ByVal b() As Byte, ByVal contextArtifactID As Int32) As Boolean
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.FileFill(caseContextArtifactID, fileName, b)
			Else
				'Return _externalIOManager.ExternalFileFill(fileName, b, contextArtifactID)
			End If
		End Function

		Public Shadows Sub RemoveFill(ByVal caseContextArtifactID As Int32, ByVal fileName As String)
			If kCura.WinEDDS.Config.UsesWebAPI Then
				MyBase.RemoveFill(caseContextArtifactID, fileName)
			Else
				'_externalIOManager.ExternalRemoveFill(fileName, contextArtifactID)
			End If
		End Sub

		Public Shadows Function ReadFileAsString(ByVal path As String) As Byte()
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.ReadFileAsString(path)
			Else
				'Return _externalIOManager.ExternalReadFileAsString(path)
			End If
		End Function
#End Region

	End Class
End Namespace