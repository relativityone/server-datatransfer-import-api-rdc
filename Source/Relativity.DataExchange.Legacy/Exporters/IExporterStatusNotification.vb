Namespace kCura.WinEDDS
    Public interface IExporterStatusNotification
        Event FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception)
        Event StatusMessage(ByVal exportArgs As ExportEventArgs)
        Event FileTransferModeChangeEvent(ByVal mode As Global.Relativity.DataExchange.Transfer.TapiClient)
    end interface
End NameSpace