Namespace kCura.WinEDDS.Service.Replacement
    Public Interface IExportManager
        Inherits Export.IExportManager
        Inherits IDisposable
        Shadows Function RetrieveResultsBlockStartingFromIndex(appID As Int32, runId As Guid, artifactTypeID As Int32, avfIds As Int32(), chunkSize As Int32, displayMulticodesAsNested As Boolean, multiValueDelimiter As Char, nestedValueDelimiter As Char, textPrecedenceAvfIds As Int32(), index As Int32) As Object()
        Shadows Function RetrieveResultsBlockForProductionStartingFromIndex(appID As Int32, runId As Guid, artifactTypeID As Int32, avfIds As Int32(), chunkSize As Int32, displayMulticodesAsNested As Boolean, multiValueDelimiter As Char, nestedValueDelimiter As Char, textPrecedenceAvfIds As Int32(), productionId As Int32, index As Int32) As Object()
        Shadows Function InitializeFolderExport(appID As Int32, viewArtifactID As Int32, parentArtifactID As Int32, includeSubFolders As Boolean, avfIds As Int32(), startAtRecord As Int32, artifactTypeID As Int32) As kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults
        Shadows Function InitializeProductionExport(appID As Int32, productionArtifactID As Int32, avfIds As Int32(), startAtRecord As Int32) As kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults
        Shadows Function InitializeSearchExport(appID As Int32, searchArtifactID As Int32, avfIds As Int32(), startAtRecord As Int32) As kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults
        Shadows Function HasExportPermissions(appID As Int32) As Boolean
    End Interface
End Namespace