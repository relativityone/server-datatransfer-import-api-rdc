
Namespace kCura.WinEDDS.Service.Export
	Public Interface IExportFileDownloader
		Event UploadModeChangeEvent(mode As String)
		Property UploaderType() As FileDownloader.FileAccessType
		Property FileHelper() As IFileHelper
		Function DownloadFullTextFile(ByVal localFilePath As String, ByVal artifactID As Int32, ByVal appID As String) As Boolean
		Function DownloadLongTextFile(ByVal localFilePath As String, ByVal artifactID As Int32, ByVal field As ViewFieldInfo, ByVal appId As String) As Boolean
		Function DownloadFileForDocument(ByVal localFilePath As String, ByVal remoteFileGuid As String, ByVal remoteLocation As String, ByVal artifactID As Int32, ByVal appID As String) As Boolean
		Function DownloadFileForDynamicObject(ByVal localFilePath As String, ByVal remoteLocation As String, ByVal artifactID As Int32, ByVal appID As String, ByVal fileID As Int32, ByVal fileFieldArtifactID As Int32) As Boolean
	End Interface
End Namespace