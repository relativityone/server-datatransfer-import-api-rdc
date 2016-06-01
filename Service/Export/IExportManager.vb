Namespace kCura.WinEDDS.Service.Export
	Public Interface IExportManager
		Function RetrieveResultsBlock(appID As Int32, runId As Guid, artifactTypeID As Int32, avfIds As Int32(), chunkSize As Int32, displayMulticodesAsNested As Boolean, multiValueDelimiter As Char, nestedValueDelimiter As Char, textPrecedenceAvfIds As Int32()) As Object()
		Function RetrieveResultsBlockForProduction(appID As Int32, runId As Guid, artifactTypeID As Int32, avfIds As Int32(), chunkSize As Int32, displayMulticodesAsNested As Boolean, multiValueDelimiter As Char, nestedValueDelimiter As Char, textPrecedenceAvfIds As Int32(), productionId As Int32) As Object()
		Function InitializeFolderExport(appID As Int32, viewArtifactID As Int32, parentArtifactID As Int32, includeSubFolders As Boolean, avfIds As Int32(), startAtRecord As Int32, artifactTypeID As Int32) As kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults
		Function InitializeProductionExport(appID As Int32, productionArtifactID As Int32, avfIds As Int32(), startAtRecord As Int32) As kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults
		Function InitializeSearchExport(appID As Int32, searchArtifactID As Int32, avfIds As Int32(), startAtRecord As Int32) As kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults
		Function HasExportPermissions(appID As Int32) As Boolean
	End Interface
End Namespace