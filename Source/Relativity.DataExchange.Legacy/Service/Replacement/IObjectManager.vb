Namespace kCura.WinEDDS.Service.Replacement
    Public interface IObjectManager
        Inherits IDisposable
        Shadows Function RetrieveArtifactIdOfMappedObject(ByVal caseContextArtifactID As Int32, ByVal textIdentifier As String, ByVal artifactTypeID As Int32) As System.Data.DataSet
        Shadows Function RetrieveTextIdentifierOfMappedObject(ByVal caseContextArtifactID As Int32, ByVal artifactId As Int32, ByVal artifactTypeID As Int32) As System.Data.DataSet
        Shadows Function RetrieveArtifactIdOfMappedParentObject(ByVal caseContextArtifactID As Int32, ByVal textIdentifier As String, ByVal artifactTypeID As Int32) As System.Data.DataSet
    end interface
End NameSpace