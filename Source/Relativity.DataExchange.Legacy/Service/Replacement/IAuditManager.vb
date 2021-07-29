Namespace kCura.WinEDDS.Service.Replacement
    Public interface IAuditManager
        Inherits Export.IAuditManager
        Inherits IDisposable
        Shadows Function AuditImageImport(ByVal appID As Int32, ByVal runId As String, ByVal isFatalError As Boolean, ByVal importStats As kCura.EDDS.WebAPI.AuditManagerBase.ImageImportStatistics) As Boolean
        Shadows Function AuditObjectImport(ByVal appID As Int32, ByVal runId As String, ByVal isFatalError As Boolean, ByVal importStats As kCura.EDDS.WebAPI.AuditManagerBase.ObjectImportStatistics) As Boolean
        Shadows Function AuditExport(ByVal appID As Int32, ByVal isFatalError As Boolean, ByVal exportStats As kCura.EDDS.WebAPI.AuditManagerBase.ExportStatistics) As Boolean
        Shadows Sub DeleteAuditToken(ByVal token As String)
    end interface
End NameSpace