Imports kCura.EDDS.WebAPI.FolderManagerBase
Imports kCura.WinEDDS.Mapping
Imports Relativity.DataExchange.Service
Imports Relativity.DataTransfer.Legacy.SDK.ImportExport.V1

Namespace kCura.WinEDDS.Service.Replacement
    Public Class KeplerFolderManager
        Inherits KeplerManager
        Implements IFolderManager

        Private _folderCreationCount As Int32 = 0

        Public Sub New(serviceProxyFactory As IServiceProxyFactory, exceptionMapper As IServiceExceptionMapper, correlationIdFunc As Func(Of String))
            MyBase.New(serviceProxyFactory, exceptionMapper, correlationIdFunc)
        End Sub

        Public ReadOnly Property CreationCount As Integer Implements IFolderManager.CreationCount
            Get
                Return _folderCreationCount
            End Get
        End Property

        Public Function Read(caseContextArtifactID As Integer, folderArtifactID As Integer) As Folder Implements IFolderManager.Read
            Return Execute(Async Function(s)
                Using service As IFolderService = s.CreateProxyInstance(Of IFolderService)
                                   Dim result As Models.Folder = Await service.ReadAsync(caseContextArtifactID, folderArtifactID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                                   Return KeplerTypeMapper.Map(result)
                               End Using
                           End Function)
        End Function

        Public Function ReadID(caseContextArtifactID As Integer, parentArtifactID As Integer, name As String) As Integer Implements IFolderManager.ReadID, IHierarchicArtifactManager.Read
            Return Execute(Async Function(s)
                               Using service As IFolderService = s.CreateProxyInstance(Of IFolderService)
                                   Return Await service.ReadIDAsync(caseContextArtifactID, parentArtifactID, name, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
                           End Function)
        End Function

        Public Function Create(caseContextArtifactID As Integer, parentArtifactID As Integer, name As String) As Integer Implements IFolderManager.Create, IHierarchicArtifactManager.Create
            Return Execute(Async Function(s)
                               Using service As IFolderService = s.CreateProxyInstance(Of IFolderService)
                                   Dim fName As String = GetExportFriendlyFolderName(name)
                                   Dim result As Integer = Await service.CreateAsync(caseContextArtifactID, parentArtifactID, fName, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                                   If result > 0 Then _folderCreationCount += 1
                                   Return result
                               End Using
                           End Function)
        End Function

        Public Function Exists(caseContextArtifactID As Integer, rootFolderID As Integer) As Boolean Implements IFolderManager.Exists
            Return Execute(Async Function(s)
                               Using service As IFolderService = s.CreateProxyInstance(Of IFolderService)
                                   Return Await service.ExistsAsync(caseContextArtifactID, rootFolderID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
                           End Function)
        End Function

        Public Function RetrieveIntitialChunk(caseContextArtifactID As Integer) As DataSet Implements IFolderManager.RetrieveIntitialChunk
            Return Execute(Async Function(s)
                               Using service As IFolderService = s.CreateProxyInstance(Of IFolderService)
                                   Return Await service.RetrieveInitialChunkAsync(caseContextArtifactID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
                           End Function)
        End Function

        Public Function RetrieveNextChunk(caseContextArtifactID As Integer, lastFolderID As Integer) As DataSet Implements IFolderManager.RetrieveNextChunk
            Return Execute(Async Function(s)
                               Using service As IFolderService = s.CreateProxyInstance(Of IFolderService)
                                   Return Await service.RetrieveNextChunkAsync(caseContextArtifactID, lastFolderID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
                           End Function)
        End Function

        Public Function RetrieveArtifacts(caseContextArtifactID As Integer, rootArtifactID As Integer) As DataSet Implements IFolderManager.RetrieveArtifacts, IHierarchicArtifactManager.RetrieveArtifacts
            Return Execute(Async Function(s)
                               Using service As IFolderService = s.CreateProxyInstance(Of IFolderService)
                                   Return Await service.RetrieveFolderAndDescendantsAsync(caseContextArtifactID, rootArtifactID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
            End Function)
        End Function
        
        Private Function GetExportFriendlyFolderName(ByVal input As String) As String
            Return System.Text.RegularExpressions.Regex.Replace(input, "[\*\\\/\:\?\<\>\""\|\$]+", " ").Trim
        End Function
    End Class
End Namespace
