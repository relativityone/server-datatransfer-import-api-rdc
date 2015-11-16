Namespace kCura.WinEDDS.Service
	Public Class ProductionManager
		Inherits kCura.EDDS.WebAPI.ProductionManagerBase.ProductionManager

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()

			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}ProductionManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

#Region " Shadow Functions "
		Public Shadows Function RetrieveProducedByContextArtifactID(ByVal caseContextArtifactID As Int32) As System.Data.DataSet
			Return RetryOnReLoginException(Function() MyBase.RetrieveProducedByContextArtifactID(caseContextArtifactID))
		End Function

		Public Shadows Function RetrieveImportEligibleByContextArtifactID(ByVal caseContextArtifactID As Int32) As System.Data.DataSet
			Return RetryOnReLoginException(Function() MyBase.RetrieveImportEligibleByContextArtifactID(caseContextArtifactID))
		End Function

		Public Shadows Function Read(ByVal caseContextArtifactID As Int32, ByVal productionArtifactID As Int32) As kCura.EDDS.WebAPI.ProductionManagerBase.ProductionInfo
			Return RetryOnReLoginException(Function() MyBase.Read(caseContextArtifactID, productionArtifactID))
		End Function

		Public Shadows Function RetrieveProducedWithSecurity(ByVal contextArtifactID As Int32) As System.Data.DataSet
			Return RetryOnReLoginException(Function() MyBase.RetrieveProducedByContextArtifactID(contextArtifactID))
		End Function

		Public Shadows Sub DoPostImportProcessing(ByVal contextArtifactID As Int32, ByVal productionArtifactID As Int32)
			RetryOnReLoginException(Sub() MyBase.DoPostImportProcessing(contextArtifactID, productionArtifactID))
		End Sub

		Public Shadows Sub DoPreImportProcessing(ByVal contextArtifactID As Int32, ByVal productionArtifactID As Int32)
			RetryOnReLoginException(Sub() MyBase.DoPreImportProcessing(contextArtifactID, productionArtifactID))
		End Sub

		Public Shadows Function MigrationJobExists(ByVal contextArtifactID As Int32, ByVal productionArtifactID As Int32) As Boolean
			Return RetryOnReLoginException(Function() MyBase.MigrationJobExists(contextArtifactID, productionArtifactID))
		End Function
#End Region

	End Class
End Namespace