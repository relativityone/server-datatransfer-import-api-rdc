Namespace kCura.WinEDDS
    Public interface IExporterStatusNotification
        Event FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception)
        Event StatusMessage(ByVal exportArgs As ExportEventArgs)
        Event FileTransferMultiClientModeChangeEvent(ByVal sender As Object, ByVal args As Global.Relativity.DataExchange.Transfer.TapiMultiClientEventArgs)
    End interface
End NameSpace