Namespace kCura.WinEDDS.Service.Replacement
    Public interface IFieldQuery
        Inherits IDisposable
        Shadows Function RetrieveAllMappable(ByVal caseContextArtifactID As Int32, ByVal artifactTypeID As Int32) As System.Data.DataSet
        Shadows Function RetrievePotentialBeginBatesFields(ByVal caseContextArtifactID As Int32) As System.Data.DataSet
        Shadows Function IsFieldIndexed(ByVal caseContextArtifactID As Int32, ByVal fieldArtifactID As Int32) As Boolean
        Shadows Function RetrieveAllAsDocumentFieldCollection(ByVal caseContextArtifactID As Int32, ByVal artifactTypeID As Int32) As DocumentFieldCollection
        Shadows Function RetrieveAllAsArray(ByVal caseContextArtifactID As Int32, ByVal artifactTypeID As Int32, Optional ByVal includeUnmappable As Boolean = False) As kCura.EDDS.WebAPI.DocumentManagerBase.Field()
    end interface
End NameSpace