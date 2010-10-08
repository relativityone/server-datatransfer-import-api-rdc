Namespace kCura.WinEDDS.Service
	Public Class FileManager
		Inherits kCura.EDDS.WebAPI.FileManagerBase.FileManager

		'Private _fileManager As New kCura.EDDS.Service.FileManager
		'Private _identity As kCura.EDDS.EDDSIdentity

		'Public Sub New(ByVal credentials As Net.NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer, ByVal identity As kCura.EDDS.EDDSIdentity)
		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()
			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}FileManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
			'_identity = identity
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

		'Public Shared Function DTOtoWebAPIFile(ByVal dto As kCura.EDDS.DTO.File) As kCura.EDDS.WebAPI.DocumentManagerBase.File
		'	Dim file As New kCura.EDDS.WebAPI.DocumentManagerBase.File

		'	file.DocumentArtifactID = dto.DocumentArtifactID
		'	file.Filename = dto.Filename
		'	file.Guid = dto.Guid
		'	file.Order = dto.Order
		'	file.Type = dto.Type
		'	Return file
		'End Function

		'Public Shared Function DTOstoWebAPIFiles(ByVal dtos As kCura.EDDS.DTO.File()) As kCura.EDDS.WebAPI.DocumentManagerBase.File()
		'	Dim files(dtos.Length - 1) As kCura.EDDS.WebAPI.DocumentManagerBase.File

		'	Dim i As Int32
		'	For i = 0 To files.Length - 1
		'		files(i) = DTOtoWebAPIFile(dtos(i))
		'	Next
		'	Return files
		'End Function

		'Public Shared Function DocumentWebAPIFiletoDTO(ByVal file As kCura.EDDS.WebAPI.DocumentManagerBase.File) As kCura.EDDS.DTO.File
		'	Dim dto As New kCura.EDDS.DTO.File

		'	dto.DocumentArtifactID = file.DocumentArtifactID
		'	dto.Filename = file.Filename
		'	dto.Guid = file.Guid
		'	dto.Order = file.Order
		'	dto.Type = file.Type
		'	Return dto
		'End Function

		'Public Shared Function FileWebAPIFiletoDTO(ByVal file As kCura.EDDS.WebAPI.FileManagerBase.File) As kCura.EDDS.DTO.File
		'	Dim dto As New kCura.EDDS.DTO.File

		'	dto.DocumentArtifactID = file.DocumentArtifactID
		'	dto.Filename = file.Filename
		'	dto.Guid = file.Guid
		'	dto.Order = file.Order
		'	dto.Type = file.Type
		'	Return dto
		'End Function

		'Public Shared Function WebAPIFilestoDTOs(ByVal files As kCura.EDDS.WebAPI.DocumentManagerBase.File()) As kCura.EDDS.DTO.File()
		'	If files Is Nothing Then Return Nothing
		'	Dim dtos(files.Length - 1) As kCura.EDDS.DTO.File

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
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.RetrieveByProductionArtifactIDForProduction(caseContextArtifactID, productionArtifactID)
					Else
						'Return kCura.EDDS.Service.FileQuery.RetrieveByProductionArtifactIDForProduction(productionArtifactID, _identity).ToDataSet()
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

		Public Shadows Function RetrieveFileGuidsByDocumentArtifactIDAndProductionArtifactID(ByVal caseContextArtifactID As Int32, ByVal documentArtifactID As Int32, ByVal productionArtifactID As Int32) As String()
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.RetrieveFileGuidsByDocumentArtifactIDAndProductionArtifactID(caseContextArtifactID, documentArtifactID, productionArtifactID)
					Else
						'Return kCura.EDDS.Service.FileQuery.RetrieveFileGuidsByProductionArtifactID(productionArtifactID, documentArtifactID, _identity)
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

		Public Shadows Function ReturnFileGuidsForOriginalImages(ByVal caseContextArtifactID As Int32, ByVal documentArtifactID As Int32) As String()
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.ReturnFileGuidsForOriginalImages(caseContextArtifactID, documentArtifactID)
					Else
						'Return kCura.EDDS.Service.FileQuery.ReturnFileGuidsForOriginalImages(_identity, documentArtifactID)
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

		Public Shadows Function Create(ByVal caseContextArtifactID As Int32, ByVal file As kCura.EDDS.WebAPI.FileManagerBase.File) As String
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.Create(caseContextArtifactID, file)
					Else
						'Return _fileManager.Create(Me.FileWebAPIFiletoDTO(file), _identity)
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

		Public Shadows Sub CreateImages(ByVal caseContextArtifactID As Int32, ByVal files As kCura.EDDS.WebAPI.FileManagerBase.FileInfoBase(), ByVal documentArtifactID As Int32)
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						MyBase.CreateImages(caseContextArtifactID, files, documentArtifactID)
						Exit Sub
					Else
						'_fileManager.ExternalCreateImages(WebAPIFileInfostoFileInfos(files), documentArtifactID, contextArtifactID, _identity)
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
		End Sub

		Public Shadows Sub CreateProductionImages(ByVal caseContextArtifactID As Int32, ByVal files As kCura.EDDS.WebAPI.FileManagerBase.FileInfoBase(), ByVal documentArtifactID As Int32)
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						MyBase.CreateProductionImages(caseContextArtifactID, files, documentArtifactID)
						Exit Sub
					Else
						'_fileManager.ExternalCreateImages(WebAPIFileInfostoFileInfos(files), documentArtifactID, contextArtifactID, _identity)
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
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
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						MyBase.CreateNatives(caseContextArtifactID, files, documentArtifactIDs)
					Else
						'_fileManager.ExternalCreateNatives(WebAPIFileInfostoFileInfos(files), documentArtifactIDs, _identity)
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
		End Sub

		Public Shadows Function GetRotation(ByVal caseContextArtifactID As Int32, ByVal guid As String) As Int32
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				Try
					tries += 1
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.GetRotation(caseContextArtifactID, guid)
					Else
						'Return New kCura.EDDS.Service.FileManager().Read(guid, artifactID, _identity).Rotation
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
		End Function

		Public Shadows Sub SetRotation(ByVal caseContextArtifactID As Int32, ByVal guid As String, ByVal rotation As Int32)
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						MyBase.SetRotation(caseContextArtifactID, guid, rotation)
						Exit Sub
					Else
						'_fileManager.ExternalSetRotation(artifactID, guid, rotation, _identity)
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
		End Sub

		Public Shadows Function GetFullTextGuidsByDocumentArtifactIdAndType(ByVal caseContextArtifactID As Int32, ByVal documentArtifactID As Int32, ByVal TypeId As Int32) As String
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.GetFullTextGuidsByDocumentArtifactIdAndType(caseContextArtifactID, documentArtifactID, TypeId)
					Else
						'Return _fileManager.ExternalGetFullTextGuidsByDocumentArtifactIdAndType(documentArtifactID, TypeId, _identity)
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
		End Function

		Public Shadows Function RetrieveNativeFileSize(ByVal caseContextArtifactID As Int32, ByVal guid As String) As Long
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.RetrieveNativesFileSize(caseContextArtifactID, guid)
					Else
						'Return _fileManager.ExternalRetrieveNativeFileSize(sourceDirectory, guid, _identity)
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
		End Function
#End Region

	End Class
End Namespace