Namespace kCura.WinEDDS.Service
	Public Class FolderManager
		Inherits kCura.EDDS.WebAPI.FolderManagerBase.FolderManager
		Implements IHierarchicArtifactManager

		Private _folderCreationCount As Int32 = 0

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()

			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}FolderManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

		Public Shared Function GetExportFriendlyFolderName(ByVal input As String) As String
			Return System.Text.RegularExpressions.Regex.Replace(input, "[\*\\\/\:\?\<\>\""\|\$]+", " ").Trim
		End Function


#Region " Shadow Functions "
		Public Shadows Function RetrieveAllByCaseID(ByVal caseContextArtifactID As Int32) As System.Data.DataSet
			Return RetryOnReLoginException(Function() MyBase.RetrieveAllByCaseID(caseContextArtifactID))
		End Function

		Public Shadows Function Read(ByVal caseContextArtifactID As Int32, ByVal folderArtifactID As Int32) As kCura.EDDS.WebAPI.FolderManagerBase.Folder
			Return RetryOnReLoginException(Function() MyBase.Read(caseContextArtifactID, folderArtifactID))
		End Function

		Public Shadows Function ReadID(ByVal caseContextArtifactID As Integer, ByVal parentArtifactID As Integer, ByVal name As String) As Integer Implements IHierarchicArtifactManager.Read
			Return RetryOnReLoginException(Function() MyBase.ReadID(caseContextArtifactID, parentArtifactID, name))
		End Function

		Public Shadows Function Create(ByVal caseContextArtifactID As Int32, ByVal parentArtifactID As Int32, ByVal name As String) As Int32 Implements IHierarchicArtifactManager.Create
			Return RetryOnReLoginException(
				Function()
					Dim retval As Int32 = MyBase.Create(caseContextArtifactID, parentArtifactID, GetExportFriendlyFolderName(name))
					If retval > 0 Then _folderCreationCount += 1
					Return retval
				End Function)
		End Function

		Public Shadows Function Exists(ByVal caseContextArtifactID As Int32, ByVal rootFolderID As Int32) As Boolean
			Return RetryOnReLoginException(Function() MyBase.Exists(caseContextArtifactID, rootFolderID))
		End Function

		Public Shadows Function RetrieveIntitialChunk(ByVal caseContextArtifactID As Int32) As System.Data.DataSet
			Return RetryOnReLoginException(Function() MyBase.RetrieveIntitialChunk(caseContextArtifactID))
		End Function

		Public Shadows Function RetrieveNextChunk(ByVal caseContextArtifactID As Int32, ByVal lastFolderID As Int32) As System.Data.DataSet
			Return RetryOnReLoginException(Function() MyBase.RetrieveNextChunk(caseContextArtifactID, lastFolderID))
		End Function

#End Region

		Public Function RetrieveArtifacts(ByVal caseContextArtifactID As Integer, ByVal rootArtifactID As Integer) As System.Data.DataSet Implements IHierarchicArtifactManager.RetrieveArtifacts
			Return Me.RetrieveFolderAndDescendants(caseContextArtifactID, rootArtifactID)
		End Function

		Public ReadOnly Property CreationCount() As Integer
			Get
				Return _folderCreationCount
			End Get
		End Property

	End Class
End Namespace