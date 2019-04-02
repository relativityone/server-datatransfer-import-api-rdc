Imports Relativity.Import.Export.Services

Namespace kCura.WinEDDS.Service.Export
    Public Interface ICaseManager
        Inherits IDisposable
		Function Read(caseArtifactID As Integer) As CaseInfo
		Function GetAllDocumentFolderPathsForCase(caseArtifactID As Integer) As String()
	End Interface
End Namespace
