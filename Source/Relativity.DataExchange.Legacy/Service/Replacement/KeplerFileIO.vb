Imports System.Runtime.Caching
Imports System.Web.Services.Protocols
Imports kCura.WinEDDS.Mapping
Imports kCura.WinEDDS.Service
Imports kCura.WinEDDS.Service.Replacement
Imports Relativity.DataExchange.Service
Imports Relativity.DataTransfer.Legacy.SDK.ImportExport.V1

Public Class KeplerFileIO
    Inherits KeplerManager
    Implements IFileIO

    Private Shared BCPCache As New MemoryCache("BCPCache")

    Public Sub New(serviceProxyFactory As IServiceProxyFactory, typeMapper As ITypeMapper, exceptionMapper As IServiceExceptionMapper, correlationIdFunc As Func(Of String))
        MyBase.New(serviceProxyFactory, typeMapper, exceptionMapper, correlationIdFunc)
    End Sub

    Public Sub RemoveTempFile(caseContextArtifactID As Integer, fileName As String) Implements IFileIO.RemoveTempFile
        Execute(Async Function(s)
            Using service As IFileIOService = s.CreateProxyInstance(Of IFileIOService)
                        Await service.RemoveTempFileAsync(caseContextArtifactID, fileName, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                        Return True
                    End Using
                End Function)
    End Sub

    Public Function ValidateBcpShare(appID As Integer) As Boolean Implements IFileIO.ValidateBcpShare
        Return Execute(Async Function(s)
                           Using service As IFileIOService = s.CreateProxyInstance(Of IFileIOService)
                               Return Await service.ValidateBcpShareAsync(appID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                           End Using
                       End Function)
    End Function

    Public Function GetBcpShareSpaceReport(appID As Integer) As String()() Implements IFileIO.GetBcpShareSpaceReport
        Return Execute(Async Function(s)
                           Using service As IFileIOService = s.CreateProxyInstance(Of IFileIOService)
                               Return Await service.GetBcpShareSpaceReportAsync(appID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                           End Using
                       End Function)
    End Function

    Public Function RepositoryVolumeMax() As Integer Implements IFileIO.RepositoryVolumeMax
        Return Execute(Async Function(s)
                           Using service As IFileIOService = s.CreateProxyInstance(Of IFileIOService)
                               Return Await service.RepositoryVolumeMaxAsync(CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                           End Using
                       End Function)
    End Function

    Public Function GetBcpSharePath(appID As Integer) As String Implements IFileIO.GetBcpSharePath
        Dim cacheValue As Object = BCPCache.Get(appID.ToString())
        If cacheValue IsNot Nothing Then
            Return cacheValue.ToString()
        End If
        Return Execute(Async Function(s)
                           Using service As IFileIOService = s.CreateProxyInstance(Of IFileIOService)
                               Dim path As String = Await service.GetBcpSharePathAsync(appID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               BCPCache.Add(appID.ToString(), path, New CacheItemPolicy() With {.AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(60)})
                Return path
            End Using
        End Function)
    End Function

    Protected Overrides Function ConvertSoapExceptionToRelativityException(soapException As SoapException) As Exception
	    If soapException.Detail.SelectNodes("ExceptionType").Item(0).InnerText = "Relativity.Core.Exception.InsufficientAccessControlListPermissions" Then
		    Return New BulkImportManager.InsufficientPermissionsForImportException(soapException.Detail.SelectNodes("ExceptionMessage")(0).InnerText, soapException)
	    ElseIf soapException.Detail.SelectNodes("ExceptionType").Item(0).InnerText = "System.ArgumentException" Then
		    Return New ArgumentException(soapException.Detail.SelectNodes("ExceptionMessage")(0).InnerText, soapException)
	    End If

	    Return MyBase.ConvertSoapExceptionToRelativityException(soapException)
    End Function
End Class
