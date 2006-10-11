Namespace kCura.WinEDDS.Service
	Public Class FolderManager
		Inherits kCura.EDDS.WebAPI.FolderManagerBase.FolderManager

		'Private _folderManager As New kCura.EDDS.Service.FolderManager
		'Private _identity As kCura.EDDS.EDDSIdentity

		Public Sub New(ByVal credentials As Net.NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer)
			'Public Sub New(ByVal credentials As Net.NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer, ByVal identity As kCura.EDDS.EDDSIdentity)
			MyBase.New()
			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}FolderManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
			'_identity = identity
		End Sub

		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

#Region " Translations "
		'Public Function DTOToWebAPIFolder(ByVal folderDTO As kCura.EDDS.DTO.Folder) As kCura.EDDS.WebAPI.FolderManagerBase.Folder
		'	Dim folder As New kCura.EDDS.WebAPI.FolderManagerBase.Folder

		'	folder.AccessControlListID = folderDTO.AccessControlListID
		'	folder.AccessControlListIsInherited = folderDTO.AccessControlListIsInherited
		'	folder.ArtifactID = folderDTO.ArtifactID
		'	folder.ArtifactTypeID = folderDTO.ArtifactTypeID
		'	folder.ContainerID = folderDTO.ContainerID
		'	folder.CreatedBy = folderDTO.CreatedBy
		'	folder.CreatedOn = folderDTO.CreatedOn
		'	folder.DeleteFlag = folderDTO.DeleteFlag
		'	folder.Keywords = folderDTO.Keywords
		'	folder.LastModifiedBy = folderDTO.LastModifiedBy
		'	folder.LastModifiedOn = folderDTO.LastModifiedOn
		'	folder.Name = folderDTO.Name
		'	folder.Notes = folderDTO.Notes
		'	folder.ParentArtifactID = folderDTO.ParentArtifactID
		'	folder.TextIdentifier = folderDTO.TextIdentifier
		'	Return folder
		'End Function
#End Region

#Region " Shadow Functions "
		Public Shadows Function RetrieveAllByCaseID(ByVal caseID As Int32) As System.Data.DataSet
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.RetrieveAllByCaseID(caseID)
			Else
				'Return _folderManager.ExternalRetrieveAllByCaseID(caseID, _identity)
			End If
		End Function

		Public Shadows Function Read(ByVal folderArtifactID As Int32) As kCura.EDDS.WebAPI.FolderManagerBase.Folder
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.Read(folderArtifactID)
			Else
				'Return Me.DTOToWebAPIFolder(_folderManager.Read(folderArtifactID, _identity))
			End If
		End Function

		Public Shadows Function Create(ByVal parentArtifactID As Int32, ByVal name As String) As Int32
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.Create(parentArtifactID, name)
			Else
				'Return _folderManager.Create(parentArtifactID, name, _identity)
			End If
		End Function

		Public Shadows Function Exists(ByVal artifactID As Int32, ByVal rootFolderID As Int32) As Boolean
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.Exists(artifactID, rootFolderID)
			Else
				'Return _folderManager.Exists(artifactID, _identity, rootFolderID)
			End If
		End Function
#End Region

	End Class
End Namespace