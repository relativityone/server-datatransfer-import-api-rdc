Imports Relativity.Import.Export

Namespace kCura.WinEDDS.Service
	Public Class FileManager
		Inherits kCura.EDDS.WebAPI.FileManagerBase.FileManager

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()

			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}FileManager.asmx", AppSettings.Instance.WebApiServiceUrl)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

		Public Overloads Function CreateFile(ByVal caseContextArtifactID As Int32, ByVal documentArtifactID As Int32, ByVal filename As String, ByVal fileGuid As String, ByVal order As Int32, ByVal type As Int32) As String
			Dim fileDTO As New kCura.EDDS.WebAPI.FileManagerBase.File
			fileDTO.DocumentArtifactID = documentArtifactID
			fileDTO.Filename = filename
			fileDTO.Guid = fileGuid
			fileDTO.Order = order
			fileDTO.Type = type
			Return Me.Create(caseContextArtifactID, fileDTO)
		End Function

#Region " Translations "
		Public Shared Function DTOtoFileInfo(ByVal dto As kCura.EDDS.WebAPI.DocumentManagerBase.File) As FileInfo
			Dim file As New FileInfo
			file.DocumentArtifactID = dto.DocumentArtifactID
			file.Filename = dto.Filename
			file.Guid = dto.Guid
			file.Order = dto.Order
			file.Type = dto.Type
			Return file
		End Function

		Public Shared Function DTOsToFileInfo(ByVal dtos As kCura.EDDS.WebAPI.DocumentManagerBase.File()) As FileInfo()
			Dim files(dtos.Length - 1) As FileInfo
			Dim i As Int32
			For i = 0 To files.Length - 1
				files(i) = DTOtoFileInfo(dtos(i))
			Next
			Return files
		End Function

		'Public Shared Function DTOtoWebAPIFile(ByVal dto As Relativity.Core.DTO.File) As kCura.EDDS.WebAPI.DocumentManagerBase.File
		'	Dim file As New kCura.EDDS.WebAPI.DocumentManagerBase.File

		'	file.DocumentArtifactID = dto.DocumentArtifactID
		'	file.Filename = dto.Filename
		'	file.Guid = dto.Guid
		'	file.Order = dto.Order
		'	file.Type = dto.Type
		'	Return file
		'End Function

		'Public Shared Function DTOstoWebAPIFiles(ByVal dtos As Relativity.Core.DTO.File()) As kCura.EDDS.WebAPI.DocumentManagerBase.File()
		'	Dim files(dtos.Length - 1) As kCura.EDDS.WebAPI.DocumentManagerBase.File

		'	Dim i As Int32
		'	For i = 0 To files.Length - 1
		'		files(i) = DTOtoWebAPIFile(dtos(i))
		'	Next
		'	Return files
		'End Function

		'Public Shared Function DocumentWebAPIFiletoDTO(ByVal file As kCura.EDDS.WebAPI.DocumentManagerBase.File) As Relativity.Core.DTO.File
		'	Dim dto As New Relativity.Core.DTO.File

		'	dto.DocumentArtifactID = file.DocumentArtifactID
		'	dto.Filename = file.Filename
		'	dto.Guid = file.Guid
		'	dto.Order = file.Order
		'	dto.Type = file.Type
		'	Return dto
		'End Function

		'Public Shared Function FileWebAPIFiletoDTO(ByVal file As kCura.EDDS.WebAPI.FileManagerBase.File) As Relativity.Core.DTO.File
		'	Dim dto As New Relativity.Core.DTO.File

		'	dto.DocumentArtifactID = file.DocumentArtifactID
		'	dto.Filename = file.Filename
		'	dto.Guid = file.Guid
		'	dto.Order = file.Order
		'	dto.Type = file.Type
		'	Return dto
		'End Function

		'Public Shared Function WebAPIFilestoDTOs(ByVal files As kCura.EDDS.WebAPI.DocumentManagerBase.File()) As Relativity.Core.DTO.File()
		'	If files Is Nothing Then Return Nothing
		'	Dim dtos(files.Length - 1) As Relativity.Core.DTO.File

		'	Dim i As Int32
		'	For i = 0 To dtos.Length - 1
		'		dtos(i) = DocumentWebAPIFiletoDTO(files(i))
		'	Next
		'	Return dtos
		'End Function

		Public Shared Function WebAPIFileInfotoFileInfo(ByVal file As kCura.EDDS.WebAPI.FileManagerBase.FileInfoBase) As Relativity.FileInfoBase
			Dim fileInfo As New Relativity.FileInfoBase

			fileInfo.FileName = file.FileName
			fileInfo.FileGuid = file.FileGuid
			Return fileInfo
		End Function

		Public Shared Function WebAPIFileInfostoFileInfos(ByVal files As kCura.EDDS.WebAPI.FileManagerBase.FileInfoBase()) As Relativity.FileInfoBase()
			Dim fileInfos(files.Length - 1) As Relativity.FileInfoBase

			Dim i As Int32
			For i = 0 To fileInfos.Length - 1
				fileInfos(i) = WebAPIFileInfotoFileInfo(files(i))
			Next
			Return fileInfos
		End Function
#End Region

#Region " Shadow Functions "
		Public Shadows Function RetrieveByProductionArtifactIDForProduction(ByVal caseContextArtifactID As Int32, ByVal productionArtifactID As Int32) As System.Data.DataSet
			Return RetryOnReLoginException(Function() MyBase.RetrieveByProductionArtifactIDForProduction(caseContextArtifactID, productionArtifactID))
		End Function

		Public Shadows Function RetrieveFileGuidsByDocumentArtifactIDAndProductionArtifactID(ByVal caseContextArtifactID As Int32, ByVal documentArtifactID As Int32, ByVal productionArtifactID As Int32) As String()
			Return RetryOnReLoginException(Function() MyBase.RetrieveFileGuidsByDocumentArtifactIDAndProductionArtifactID(caseContextArtifactID, documentArtifactID, productionArtifactID))
		End Function

		Public Shadows Function ReturnFileGuidsForOriginalImages(ByVal caseContextArtifactID As Int32, ByVal documentArtifactID As Int32) As String()
			Return RetryOnReLoginException(Function() MyBase.ReturnFileGuidsForOriginalImages(caseContextArtifactID, documentArtifactID))
		End Function

		Public Shadows Function Create(ByVal caseContextArtifactID As Int32, ByVal file As kCura.EDDS.WebAPI.FileManagerBase.File) As String
			Return RetryOnReLoginException(Function() MyBase.Create(caseContextArtifactID, file))
		End Function

		Public Shadows Sub CreateImages(ByVal caseContextArtifactID As Int32, ByVal files As kCura.EDDS.WebAPI.FileManagerBase.FileInfoBase(), ByVal documentArtifactID As Int32)
			RetryOnReLoginException(Sub() MyBase.CreateImages(caseContextArtifactID, files, documentArtifactID))
		End Sub

		Public Shadows Sub CreateProductionImages(ByVal caseContextArtifactID As Int32, ByVal files As kCura.EDDS.WebAPI.FileManagerBase.FileInfoBase(), ByVal documentArtifactID As Int32)
			RetryOnReLoginException(Sub() MyBase.CreateProductionImages(caseContextArtifactID, files, documentArtifactID))
		End Sub

		Public Overloads Sub CreateNatives(ByVal caseContextArtifactID As Int32, ByVal fileDTOs As kCura.EDDS.WebAPI.FileManagerBase.File())
			Dim documentArtifactIDs(fileDTOs.Length - 1) As Int32
			Dim files(fileDTOs.Length - 1) As kCura.EDDS.WebAPI.FileManagerBase.FileInfoBase
			Dim i As Int32
			For i = 0 To files.Length - 1
				Dim file As New kCura.EDDS.WebAPI.FileManagerBase.FileInfoBase
				file.FileGuid = fileDTOs(i).Guid
				file.FileName = fileDTOs(i).Filename
				documentArtifactIDs(i) = fileDTOs(i).DocumentArtifactID
				files(i) = file
			Next

			RetryOnReLoginException(Sub() MyBase.CreateNatives(caseContextArtifactID, files, documentArtifactIDs))
		End Sub

		Public Shadows Function GetRotation(ByVal caseContextArtifactID As Int32, ByVal guid As String) As Int32
			Return RetryOnReLoginException(Function() MyBase.GetRotation(caseContextArtifactID, guid))
		End Function

		Public Shadows Sub SetRotation(ByVal caseContextArtifactID As Int32, ByVal guid As String, ByVal rotation As Int32)
			RetryOnReLoginException(Sub() MyBase.SetRotation(caseContextArtifactID, guid, rotation))
		End Sub

		Public Shadows Function GetFullTextGuidsByDocumentArtifactIdAndType(ByVal caseContextArtifactID As Int32, ByVal documentArtifactID As Int32, ByVal TypeId As Int32) As String
			Return RetryOnReLoginException(Function() MyBase.GetFullTextGuidsByDocumentArtifactIdAndType(caseContextArtifactID, documentArtifactID, TypeId))
		End Function

		Public Shadows Function RetrieveNativeFileSize(ByVal caseContextArtifactID As Int32, ByVal guid As String) As Long
			Return RetryOnReLoginException(Function() MyBase.RetrieveNativesFileSize(caseContextArtifactID, guid))
		End Function
#End Region

	End Class
End Namespace