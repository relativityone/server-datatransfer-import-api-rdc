Imports kCura.WinEDDS.Service.Export
Imports Relativity.Import.Export
Imports Relativity.Import.Export.Services

Namespace kCura.WinEDDS.Service
	Public Class CaseManager
		Inherits kCura.EDDS.WebAPI.CaseManagerBase.CaseManager
        Implements ICaseManager

        Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer)
	        Me.New(credentials, cookieContainer, AppSettings.Instance.WebApiServiceUrl, Settings.DefaultTimeOut)
		End Sub

        Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer, ByVal webApiServiceUrl As String, ByVal webApiOperationTimeout As Int32)
	        MyBase.New()
	        Me.Credentials = credentials
	        Me.CookieContainer = cookieContainer
	        Me.Url = String.Format("{0}CaseManager.asmx", webApiServiceUrl)
	        Me.Timeout = webApiOperationTimeout
        End Sub

		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

		Public Shared Function ConvertToCaseInfo(ByVal toConvert As kCura.EDDS.WebAPI.CaseManagerBase.CaseInfo) As CaseInfo
			Dim c As New CaseInfo
			With toConvert
				c.ArtifactID = .ArtifactID
				c.MatterArtifactID = .MatterArtifactID
				c.Name = .Name
				c.RootArtifactID = .RootArtifactID
				c.RootFolderID = .RootFolderID
				c.StatusCodeArtifactID = .StatusCodeArtifactID
				c.EnableDataGrid = .EnableDataGrid
				c.DocumentPath = .DocumentPath
				c.DownloadHandlerURL = .DownloadHandlerURL
			End With
			Return c
		End Function

#Region " Shadow Functions "
		Public Shadows Function RetrieveAll() As System.Data.DataSet
			Return RetryOnReLoginException(Function() MyBase.RetrieveAllEnabled())
		End Function

		Public Shadows Function Read(ByVal caseArtifactID As Int32) As CaseInfo Implements ICaseManager.Read
			Return RetryOnReLoginException(Function() ConvertToCaseInfo(MyBase.Read(caseArtifactID)))
		End Function

		Public Shadows Function GetAllDocumentFolderPathsForCase(ByVal caseArtifactID As Int32) As String() Implements ICaseManager.GetAllDocumentFolderPathsForCase
			Return RetryOnReLoginException(Function() MyBase.GetAllDocumentFolderPathsForCase(caseArtifactID))
		End Function
#End Region

	End Class
End Namespace