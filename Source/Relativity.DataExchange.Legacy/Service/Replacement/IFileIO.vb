Namespace kCura.WinEDDS.Service.Replacement
    Public interface IFileIO
        Inherits IDisposable
        Shadows Sub RemoveTempFile(ByVal caseContextArtifactID As Integer, ByVal fileName As String)
        Shadows Function ValidateBcpShare(ByVal appID As Int32) As Boolean
        Shadows Function GetBcpShareSpaceReport(ByVal appID As Int32) As String()()
        Shadows Function RepositoryVolumeMax() As Int32
        Shadows Function GetBcpSharePath(ByVal appID As Int32) As String
    end interface
End NameSpace