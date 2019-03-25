Namespace kCura.WinEDDS.Service
	Public Class ObjectManager
		Inherits kCura.EDDS.WebAPI.ObjectManagerBase.ObjectManager

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
			Me.Url = String.Format("{0}ObjectManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

#End Region

#Region " Shadow Implementations "

		Public Shadows Function RetrieveArtifactIdOfMappedObject(ByVal caseContextArtifactID As Int32, ByVal textIdentifier As String, ByVal artifactTypeID As Int32) As System.Data.DataSet
			Return RetryOnReLoginException(Function() MyBase.RetrieveArtifactIdOfMappedObject(caseContextArtifactID, textIdentifier, artifactTypeID))
		End Function

		Public Shadows Function RetrieveTextIdentifierOfMappedObject(ByVal caseContextArtifactID As Int32, ByVal artifactId As Int32, ByVal artifactTypeID As Int32) As System.Data.DataSet
			Return RetryOnReLoginException(Function() MyBase.RetrieveTextIdentifierOfMappedObject(caseContextArtifactID, artifactId, artifactTypeID))
		End Function

		Public Shadows Function RetrieveArtifactIdOfMappedParentObject(ByVal caseContextArtifactID As Int32, ByVal textIdentifier As String, ByVal artifactTypeID As Int32) As System.Data.DataSet
			Return RetryOnReLoginException(Function() MyBase.RetrieveArtifactIdOfMappedParentObject(caseContextArtifactID, textIdentifier, artifactTypeID))
		End Function

		Public Shadows Sub Update(appArtifactID As Int32, artifact As kCura.EDDS.WebAPI.ObjectManagerBase.SimplifiedMaskDto)
			RetryOnReLoginException(Sub() MyBase.Update(appArtifactID, artifact))
		End Sub

#End Region

	End Class
End Namespace