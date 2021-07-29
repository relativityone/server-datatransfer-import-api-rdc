Imports Relativity.DataExchange

Namespace kCura.WinEDDS.Service
	Public Class ObjectTypeManager
		Inherits kCura.EDDS.WebAPI.ObjectTypeManagerBase.ObjectTypeManager
		Implements Replacement.IObjectTypeManager

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
			Me.Url = String.Format("{0}ObjectTypeManager.asmx", AppSettings.Instance.WebApiServiceUrl)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

#End Region

#Region " Shadow Implementations "

		Public Shadows Function RetrieveAllUploadable(ByVal caseContextArtifactID As Int32) As System.Data.DataSet Implements Replacement.IObjectTypeManager.RetrieveAllUploadable
			Return RetryOnReLoginException(Function() MyBase.RetrieveAllUploadable(caseContextArtifactID))
		End Function

		Public Shadows Function RetrieveParentArtifactTypeID(ByVal caseContextArtifactID As Integer, ByVal artifactTypeID As Integer) As System.Data.DataSet Implements Replacement.IObjectTypeManager.RetrieveParentArtifactTypeID
			Return RetryOnReLoginException(Function() MyBase.RetrieveParentArtifactTypeID(caseContextArtifactID, artifactTypeID))
		End Function

#End Region

	End Class
End Namespace
