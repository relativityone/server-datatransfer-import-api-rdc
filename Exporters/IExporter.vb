Namespace kCura.WinEDDS
    Public interface IExporter
        Inherits IExporterStatusNotification
        Property InteractionManager As Exporters.IUserNotification
        Function ExportSearch() As Boolean
    end interface
End NameSpace