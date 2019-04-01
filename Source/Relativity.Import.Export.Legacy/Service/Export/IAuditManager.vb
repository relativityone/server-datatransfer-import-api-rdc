Namespace kCura.WinEDDS.Service.Export
	Public Interface IAuditManager
		Function AuditExport(ByVal appID As Int32, ByVal isFatalError As Boolean, ByVal exportStats As kCura.EDDS.WebAPI.AuditManagerBase.ExportStatistics) As Boolean
	End Interface
End Namespace