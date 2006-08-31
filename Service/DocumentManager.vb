Imports kCura.Utility.NullableTypesHelper
Imports System.Runtime.Remoting.Lifetime
Namespace kCura.WinEDDS.Service
	Public Class DocumentManager
		Inherits kCura.EDDS.WebAPI.DocumentManagerBase.DocumentManager

		Public Sub New(ByVal credentials As Net.NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()
			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}DocumentManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

		Public Shadows Function ReadFromDocumentArtifactID(ByVal documentArtifactID As Int32) As kCura.EDDS.WebAPI.DocumentManagerBase.Document
			Dim doc As kCura.EDDS.WebAPI.DocumentManagerBase.Document = MyBase.Read(documentArtifactID)
			Dim field As kCura.EDDS.WebAPI.DocumentManagerBase.Field
			For Each field In doc.Fields
				If field.FieldCategoryID = kCura.EDDS.Types.FieldCategory.FullText Then
					field.Value = New NullableTypes.NullableString(System.Text.Encoding.Default.GetString(DirectCast(field.Value, Byte())))
					Exit For
				End If
			Next
			Return doc
		End Function

		Public Shared Function DTOtoDocumentInfo(ByVal dto As kCura.EDDS.WebAPI.DocumentManagerBase.Document) As DocumentInfo
			Dim doc As New DocumentInfo
			doc.AccessControlListID = dto.AccessControlListID
			doc.AccessControlListIsInherited = dto.AccessControlListIsInherited
			doc.ArtifactID = dto.ArtifactID
			doc.ArtifactTypeID = dto.ArtifactTypeID
			doc.ContainerID = dto.ContainerID
			doc.CreatedBy = dto.CreatedBy
			doc.CreatedOn = dto.CreatedOn
			doc.DeleteFlag = dto.DeleteFlag
			doc.Fields = FieldManager.DTOsToDocumentField(dto.Fields)
			doc.Files = FileManager.DTOsToFileInfo(dto.Files)
			doc.Keywords = dto.Keywords
			doc.LastModifiedBy = dto.LastModifiedBy
			doc.LastModifiedOn = dto.LastModifiedOn
			doc.Notes = dto.Notes
			doc.ParentArtifactID = dto.ParentArtifactID
			doc.TextIdentifier = dto.TextIdentifier
			Return doc
		End Function

	End Class
End Namespace