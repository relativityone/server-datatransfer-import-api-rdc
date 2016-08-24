﻿Namespace kCura.WinEDDS.Service.Export
	Public Interface IProductionManager
		Function Read(ByVal caseContextArtifactID As Int32, ByVal productionArtifactID As Int32) As kCura.EDDS.WebAPI.ProductionManagerBase.ProductionInfo

        Function RetrieveProducedByContextArtifactID(ByVal caseContextArtifactID As Int32) As System.Data.DataSet
	End Interface
End Namespace