Namespace kCura.WinEDDS
    Public interface IExporterStatusNotification
        Event FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception)
        Event StatusMessage(ByVal exportArgs As ExportEventArgs)
        Event FileTransferModeChangeEvent(ByVal mode As String)
    end interface
End NameSpace