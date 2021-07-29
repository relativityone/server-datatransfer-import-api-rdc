Namespace kCura.WinEDDS.Service.Replacement
    Public interface IObjectTypeManager
        Inherits IDisposable
        Shadows Function RetrieveAllUploadable(ByVal caseContextArtifactID As Int32) As System.Data.DataSet
        Shadows Function RetrieveParentArtifactTypeID(ByVal caseContextArtifactID As Integer, ByVal artifactTypeID As Integer) As System.Data.DataSet
    end interface
End NameSpace