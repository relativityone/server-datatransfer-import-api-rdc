Namespace kCura.WinEDDS.Service.Export
	Public Interface IExportFileDownloaderStatus
		Event UploadModeChangeEvent(mode As String)
		Property UploaderType() As FileDownloader.FileAccessType
	End Interface
End Namespace
