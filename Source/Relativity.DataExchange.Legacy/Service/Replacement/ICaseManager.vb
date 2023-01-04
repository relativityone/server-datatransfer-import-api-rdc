Imports Relativity.DataExchange.Service

Namespace kCura.WinEDDS.Service.Replacement
    Public Interface ICaseManager
        Inherits Export.ICaseManager
        Inherits IDisposable
        Shadows Function Read(caseArtifactID As Integer) As CaseInfo
        Shadows Function GetAllDocumentFolderPathsForCase(caseArtifactID As Integer) As String()
        Shadows Function RetrieveAll() As System.Data.DataSet
        Shadows Function GetAllDocumentFolderPaths() As String()
    End Interface
End Namespace
