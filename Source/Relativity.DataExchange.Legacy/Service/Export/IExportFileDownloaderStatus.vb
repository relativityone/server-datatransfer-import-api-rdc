Namespace kCura.WinEDDS.Service.Export
	Public Interface IExportFileDownloaderStatus
		Event UploadModeChangeEvent(tapiClient As Global.Relativity.Import.Export.Transfer.TapiClient)
		Property UploaderType() As Global.Relativity.Import.Export.Transfer.TapiClient
	End Interface
End Namespace