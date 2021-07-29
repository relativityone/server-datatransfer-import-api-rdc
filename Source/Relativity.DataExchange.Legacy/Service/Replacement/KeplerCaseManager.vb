Imports kCura.WinEDDS.Mapping
Imports Relativity.DataExchange.Service
Imports Relativity.DataTransfer.Legacy.SDK.ImportExport.V1

Namespace kCura.WinEDDS.Service.Replacement
    Public Class KeplerCaseManager
        Inherits KeplerManager
        Implements ICaseManager

        Public Sub New(serviceProxyFactory As IServiceProxyFactory, typeMapper As ITypeMapper, exceptionMapper As IServiceExceptionMapper, correlationIdFunc As Func(Of String))
            MyBase.New(serviceProxyFactory, typeMapper, exceptionMapper, correlationIdFunc)
        End Sub

        Public Function Read(caseArtifactID As Integer) As CaseInfo Implements Export.ICaseManager.Read, ICaseManager.Read
            Return Execute(Async Function(s)
                Using caseManager As ICaseService = s.CreateProxyInstance(Of ICaseService)
                                   Dim result As Models.CaseInfo = Await caseManager.ReadAsync(caseArtifactID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                                   Return Map(Of CaseInfo)(result)
                               End Using
                           End Function)
        End Function

        Public Function GetAllDocumentFolderPathsForCase(caseArtifactID As Integer) As String() Implements Export.ICaseManager.GetAllDocumentFolderPathsForCase, ICaseManager.GetAllDocumentFolderPathsForCase
            Return Execute(Async Function(s)
                               Using caseManager As ICaseService = s.CreateProxyInstance(Of ICaseService)
                                   Return Await caseManager.GetAllDocumentFolderPathsForCaseAsync(caseArtifactID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
                           End Function)
        End Function

        Public Function RetrieveAll() As DataSet Implements ICaseManager.RetrieveAll
            Return Execute(Async Function(s)
                               Using caseManager As ICaseService = s.CreateProxyInstance(Of ICaseService)
                                   Return Await caseManager.RetrieveAllEnabledAsync(CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
                           End Function)
        End Function

        Public Function GetAllDocumentFolderPaths() As String() Implements ICaseManager.GetAllDocumentFolderPaths
            Return Execute(Async Function(s)
                               Using caseManager As ICaseService = s.CreateProxyInstance(Of ICaseService)
                                   Return Await caseManager.GetAllDocumentFolderPathsAsync(CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
            End Function)
        End Function
    End Class
End Namespace
