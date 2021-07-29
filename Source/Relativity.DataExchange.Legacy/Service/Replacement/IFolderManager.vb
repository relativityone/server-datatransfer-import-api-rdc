Namespace kCura.WinEDDS.Service.Replacement
    Public interface IFolderManager
        Inherits IDisposable
        Inherits IHierarchicArtifactManager
        ReadOnly Property CreationCount() As Integer
        Shadows Function Read(ByVal caseContextArtifactID As Int32, ByVal folderArtifactID As Int32) As kCura.EDDS.WebAPI.FolderManagerBase.Folder
        Shadows Function ReadID(ByVal caseContextArtifactID As Integer, ByVal parentArtifactID As Integer, ByVal name As String) As Integer
        Shadows Function Create(ByVal caseContextArtifactID As Int32, ByVal parentArtifactID As Int32, ByVal name As String) As Int32
        Shadows Function Exists(ByVal caseContextArtifactID As Int32, ByVal rootFolderID As Int32) As Boolean
        Shadows Function RetrieveIntitialChunk(ByVal caseContextArtifactID As Int32) As System.Data.DataSet
        Shadows Function RetrieveNextChunk(ByVal caseContextArtifactID As Int32, ByVal lastFolderID As Int32) As System.Data.DataSet
        Shadows Function RetrieveArtifacts(ByVal caseContextArtifactID As Integer, ByVal rootArtifactID As Integer) As System.Data.DataSet
    end interface
End NameSpace