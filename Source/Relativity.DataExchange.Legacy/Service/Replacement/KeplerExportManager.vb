Imports System.Web.Services.Protocols
Imports kCura.WinEDDS.Mapping
Imports Relativity.DataExchange.Service
Imports Relativity.DataTransfer.Legacy.SDK.ImportExport.V1
Imports Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models

Namespace kCura.WinEDDS.Service.Replacement
    Public Class KeplerExportManager
        Inherits KeplerManager
        Implements IExportManager

        Public Sub New(serviceProxyFactory As IServiceProxyFactory, typeMapper As ITypeMapper, exceptionMapper As IServiceExceptionMapper, correlationIdFunc As Func(Of String))
            MyBase.New(serviceProxyFactory, typeMapper, exceptionMapper, correlationIdFunc)
        End Sub

        Public Function RetrieveResultsBlockStartingFromIndex(appID As Integer, runId As Guid, artifactTypeID As Integer, avfIds() As Integer, chunkSize As Integer, displayMulticodesAsNested As Boolean, multiValueDelimiter As Char, nestedValueDelimiter As Char, textPrecedenceAvfIds() As Integer, index As Integer) As Object() Implements IExportManager.RetrieveResultsBlockStartingFromIndex, Export.IExportManager.RetrieveResultsBlockStartingFromIndex
            Return Execute(Async Function(s)
                Using service As IExportService = s.CreateProxyInstance(Of IExportService)
                                   Dim result As ExportDataWrapper = Await service.RetrieveResultsBlockStartingFromIndexAsync(appID, runId, artifactTypeID, avfIds, chunkSize, displayMulticodesAsNested, multiValueDelimiter, nestedValueDelimiter, textPrecedenceAvfIds, index, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                                   Return UnwrapAndRehydrateStrings(result)
                               End Using
                           End Function)
        End Function

        Public Function RetrieveResultsBlockForProductionStartingFromIndex(appID As Integer, runId As Guid, artifactTypeID As Integer, avfIds() As Integer, chunkSize As Integer, displayMulticodesAsNested As Boolean, multiValueDelimiter As Char, nestedValueDelimiter As Char, textPrecedenceAvfIds() As Integer, productionId As Integer, index As Integer) As Object() Implements IExportManager.RetrieveResultsBlockForProductionStartingFromIndex, Export.IExportManager.RetrieveResultsBlockForProductionStartingFromIndex
            Return Execute(Async Function(s)
                               Using service As IExportService = s.CreateProxyInstance(Of IExportService)
                                   Dim result As ExportDataWrapper = Await service.RetrieveResultsBlockForProductionStartingFromIndexAsync(appID, runId, artifactTypeID, avfIds, chunkSize, displayMulticodesAsNested, multiValueDelimiter, nestedValueDelimiter, textPrecedenceAvfIds, productionId, index, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                                   Return UnwrapAndRehydrateStrings(result)
                               End Using
                           End Function)
        End Function

        Public Function InitializeFolderExport(appID As Integer, viewArtifactID As Integer, parentArtifactID As Integer, includeSubFolders As Boolean, avfIds() As Integer, startAtRecord As Integer, artifactTypeID As Integer) As kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults Implements IExportManager.InitializeFolderExport, Export.IExportManager.InitializeFolderExport
            Return Execute(Async Function(s)
                               Using service As IExportService = s.CreateProxyInstance(Of IExportService)
                                   Dim result As Models.InitializationResults = Await service.InitializeFolderExportAsync(appID, viewArtifactID, parentArtifactID, includeSubFolders, avfIds, startAtRecord, artifactTypeID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                                   Return Map(Of kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults)(result)
                               End Using
                           End Function)
        End Function

        Public Function InitializeProductionExport(appID As Integer, productionArtifactID As Integer, avfIds() As Integer, startAtRecord As Integer) As kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults Implements IExportManager.InitializeProductionExport, Export.IExportManager.InitializeProductionExport
            Return Execute(Async Function(s)
                               Using service As IExportService = s.CreateProxyInstance(Of IExportService)
                                   Dim result As Models.InitializationResults = Await service.InitializeProductionExportAsync(appID, productionArtifactID, avfIds, startAtRecord, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                                   Return Map(Of kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults)(result)
                               End Using
                           End Function)
        End Function

        Public Function InitializeSearchExport(appID As Integer, searchArtifactID As Integer, avfIds() As Integer, startAtRecord As Integer) As kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults Implements IExportManager.InitializeSearchExport, Export.IExportManager.InitializeSearchExport
            Return Execute(Async Function(s)
                               Using service As IExportService = s.CreateProxyInstance(Of IExportService)
                                   Dim result As Models.InitializationResults = Await service.InitializeSearchExportAsync(appID, searchArtifactID, avfIds, startAtRecord, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                                   Return Map(Of kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults)(result)
                               End Using
                           End Function)
        End Function

        Public Function HasExportPermissions(appID As Integer) As Boolean Implements IExportManager.HasExportPermissions, Export.IExportManager.HasExportPermissions
            Return Execute(Async Function(s)
                               Using service As IExportService = s.CreateProxyInstance(Of IExportService)
                                   Return Await service.HasExportPermissionsAsync(appID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
            End Function)
        End Function

        Protected Overrides Function ConvertSoapExceptionToRelativityException(soapException As SoapException) As Exception
	        If soapException.Detail.SelectNodes("ExceptionType").Item(0).InnerText = "Relativity.Core.Exception.InsufficientAccessControlListPermissions" Then
		        Return New ExportManager.InsufficientPermissionsForExportException(soapException.Detail.SelectNodes("ExceptionMessage")(0).InnerText, soapException)
	        End If

	        Return MyBase.ConvertSoapExceptionToRelativityException(soapException)
        End Function

        Private Function UnwrapAndRehydrateStrings(wrapper As ExportDataWrapper) As Object()()
            If wrapper.Unwrap() Is Nothing
                Return Nothing
            End If
            
            Dim toScrub As Object()() = wrapper.Unwrap()
            
            For Each row As Object() In toScrub
                If row Is Nothing Then
                    Throw New System.Exception("Invalid (null) row retrieved from server")
                End If
                For i As Int32 = 0 To row.Length - 1
                    If TypeOf row(i) Is Byte() Then row(i) = System.Text.Encoding.Unicode.GetString(DirectCast(row(i), Byte()))
                Next
            Next

            Return toScrub
        End Function
    End Class
End Namespace
