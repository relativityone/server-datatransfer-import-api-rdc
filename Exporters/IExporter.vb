Namespace kCura.WinEDDS
    Public interface IExporter
        Inherits IExporterStatusNotification
        Property DocumentsExported As Int32
        Property InteractionManager As Exporters.IUserNotification
        Function ExportSearch() As Boolean
    end interface
End NameSpace