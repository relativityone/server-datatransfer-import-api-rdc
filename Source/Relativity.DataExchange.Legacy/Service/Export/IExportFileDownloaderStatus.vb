Namespace kCura.WinEDDS.Service.Export
	Public Interface IExportFileDownloaderStatus
		Event UploadModeChangeEvent(tapiClient As Global.Relativity.DataExchange.Transfer.TapiClient)
		Property UploaderType() As Global.Relativity.DataExchange.Transfer.TapiClient
	End Interface
End Namespace