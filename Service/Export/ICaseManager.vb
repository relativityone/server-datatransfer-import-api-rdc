Imports Relativity

Namespace kCura.WinEDDS.Service.Export
    Public Interface ICaseManager
        Inherits IDisposable
        Function Read(caseArtifactID As Integer) As CaseInfo
    End Interface
End Namespace
