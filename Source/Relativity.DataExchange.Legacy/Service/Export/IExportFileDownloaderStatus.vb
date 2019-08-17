Namespace kCura.WinEDDS.Service.Export
	Public Interface IExportFileDownloaderStatus
		Event TransferModeChangeEvent(tapiClient As Global.Relativity.DataExchange.Transfer.TapiClient)
		ReadOnly Property TransferMode As Global.Relativity.DataExchange.Transfer.TapiClient
	End Interface
End Namespace