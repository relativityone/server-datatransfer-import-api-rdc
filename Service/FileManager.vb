Namespace kCura.WinEDDS.Service
	Public Class FileManager
		Inherits kCura.EDDS.WebAPI.FileManagerBase.FileManager

		Public Sub New(ByVal credentials As Net.NetworkCredential)
			MyBase.New()
			Me.Credentials = credentials
			Me.Url = String.Format("{0}FileManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

		Public Overloads Function CreateFile(ByVal documentArtifactID As Int32, ByVal filename As String, ByVal fileGuid As String, ByVal order As Int32, ByVal type As Int32) As String
			Dim fileDTO As New kCura.EDDS.WebAPI.FileManagerBase.File
			fileDTO.DocumentArtifactID = documentArtifactID
			fileDTO.Filename = filename
			fileDTO.Guid = fileGuid
			fileDTO.Order = order
			fileDTO.Type = type
			Return Create(fileDTO)
		End Function

		Public Overloads Sub CreateNatives(ByVal fileDTOs As kCura.EDDS.WebAPI.FileManagerBase.File())
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
			MyBase.CreateNatives(files, documentArtifactIDs)
		End Sub

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
	End Class
End Namespace