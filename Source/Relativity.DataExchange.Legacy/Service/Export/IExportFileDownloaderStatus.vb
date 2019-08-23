Imports System.Collections.Generic

Namespace kCura.WinEDDS.Service.Export
	Public Interface IExportFileDownloaderStatus
		Event TransferModesChangeEvent(ByVal sender As Object, ByVal args As Global.Relativity.DataExchange.Transfer.TapiMultiClientEventArgs)
		ReadOnly Property TransferModes As IList(Of Global.Relativity.DataExchange.Transfer.TapiClient)
	End Interface
End Namespace