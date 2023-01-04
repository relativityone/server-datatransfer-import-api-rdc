Imports System.Net
Imports System.Web.Services.Protocols
Imports kCura.EDDS.WebAPI.BulkImportManagerBase
Imports kCura.WinEDDS.Mapping
Imports Relativity.DataExchange.Service
Imports Relativity.DataTransfer.Legacy.SDK.ImportExport.V1

Namespace kCura.WinEDDS.Service.Replacement
    Public Class KeplerBulkImportManager
        Inherits KeplerManager
        Implements IBulkImportManager

        Private ReadOnly _credentials As NetworkCredential
        Private ReadOnly _cookieContainer As CookieContainer

        Public Sub New(serviceProxyFactory As IServiceProxyFactory, exceptionMapper As IServiceExceptionMapper, credentials As NetworkCredential, correlationIdFunc As Func(Of String))
            MyBase.New(serviceProxyFactory, exceptionMapper, correlationIdFunc)
            _credentials = credentials
        End Sub

        Public ReadOnly Property CookieContainer As CookieContainer Implements IBulkImportManager.CookieContainer
            Get
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property Credentials As ICredentials Implements IBulkImportManager.Credentials
            Get
                Return _credentials
            End Get
        End Property

        Public Function BulkImportImage(appID As Integer, settings As ImageLoadInfo, inRepository As Boolean) As MassImportResults Implements IBulkImportManager.BulkImportImage
            Return ExecuteImport(Function()
                Return Execute(Async Function(s)
                    Using importer As IBulkImportService = s.CreateProxyInstance(Of IBulkImportService)
                                       Dim result As Models.MassImportResults = Await importer.BulkImportImageAsync(appID, KeplerTypeMapper.Map(settings), inRepository, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                                       Return KeplerTypeMapper.Map(result)
                                   End Using
                               End Function)
                                 End Function)
        End Function

        Public Function BulkImportProductionImage(appID As Integer, settings As ImageLoadInfo, productionKeyFieldArtifactID As Integer, inRepository As Boolean) As MassImportResults Implements IBulkImportManager.BulkImportProductionImage
            Return ExecuteImport(Function()
                                     Return Execute(Async Function(s)
                                                        Using importer As IBulkImportService = s.CreateProxyInstance(Of IBulkImportService)
                                                            Dim result As Models.MassImportResults = Await importer.BulkImportProductionImageAsync(appID, KeplerTypeMapper.Map(settings), productionKeyFieldArtifactID, inRepository, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                                                            Return KeplerTypeMapper.Map(result)
                                                        End Using
                                                    End Function)
                                 End Function)
        End Function

        Public Function BulkImportNative(appID As Integer, settings As NativeLoadInfo, inRepository As Boolean, includeExtractedTextEncoding As Boolean) As MassImportResults Implements IBulkImportManager.BulkImportNative
            Return ExecuteImport(Function()
                                     Return Execute(Async Function(s)
                                                                      Using importer As IBulkImportService = s.CreateProxyInstance(Of IBulkImportService)
                                                                          Dim result As Models.MassImportResults = Await importer.BulkImportNativeAsync(appID, KeplerTypeMapper.Map(settings), inRepository, includeExtractedTextEncoding, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                                                                          Return KeplerTypeMapper.Map(result)
                                                                      End Using
                                                                  End Function)
                                 End Function)
        End Function

        Public Function BulkImportObjects(appID As Integer, settings As ObjectLoadInfo, inRepository As Boolean) As MassImportResults Implements IBulkImportManager.BulkImportObjects
            Return ExecuteImport(Function()
                                     Return Execute(Async Function(s)
                                                        Using importer As IBulkImportService = s.CreateProxyInstance(Of IBulkImportService)
                                                            Dim result As Models.MassImportResults = Await importer.BulkImportObjectsAsync(appID, KeplerTypeMapper.Map(settings), inRepository, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                                                            Return KeplerTypeMapper.Map(result)
                                                        End Using
                                                    End Function)
                                 End Function)
        End Function

        Public Function DisposeTempTables(appID As Integer, runId As String) As Object Implements IBulkImportManager.DisposeTempTables
            Return Execute(Async Function(s)
                               Using importer As IBulkImportService = s.CreateProxyInstance(Of IBulkImportService)
                                   Return Await importer.DisposeTempTablesAsync(appID, runId, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
                           End Function)
        End Function

        Public Function ImageRunHasErrors(artifactID As Integer, runId As String) As Boolean Implements IBulkImportManager.ImageRunHasErrors
            Return Execute(Async Function(s)
                               Using importer As IBulkImportService = s.CreateProxyInstance(Of IBulkImportService)
                                   Return Await importer.ImageRunHasErrorsAsync(artifactID, runId, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
                           End Function)
        End Function

        Public Function GenerateImageErrorFiles(appID As Integer, importKey As String, writeHeader As Boolean, keyFieldID As Integer) As Global.Relativity.DataExchange.Service.ErrorFileKey Implements IBulkImportManager.GenerateImageErrorFiles
            Return Execute(Async Function(s)
                               Using importer As IBulkImportService = s.CreateProxyInstance(Of IBulkImportService)
                                   Dim result As Models.ErrorFileKey = Await importer.GenerateImageErrorFilesAsync(appID, importKey, writeHeader, keyFieldID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                                   Return KeplerTypeMapper.Map(result)
                               End Using
                           End Function)
        End Function

        Public Function GenerateNonImageErrorFiles(appID As Integer, runID As String, artifactTypeID As Integer, writeHeader As Boolean, keyFieldID As Integer) As Global.Relativity.DataExchange.Service.ErrorFileKey Implements IBulkImportManager.GenerateNonImageErrorFiles
            Return Execute(Async Function(s)
                               Using importer As IBulkImportService = s.CreateProxyInstance(Of IBulkImportService)
                                   Dim result As Models.ErrorFileKey = Await importer.GenerateNonImageErrorFilesAsync(appID, runID, artifactTypeID, writeHeader, keyFieldID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                                   Return KeplerTypeMapper.Map(result)
                               End Using
                           End Function)
        End Function

        Public Function NativeRunHasErrors(appID As Integer, runId As String) As Boolean Implements IBulkImportManager.NativeRunHasErrors
            Return Execute(Async Function(s)
                               Using importer As IBulkImportService = s.CreateProxyInstance(Of IBulkImportService)
                                   Return Await importer.NativeRunHasErrorsAsync(appID, runId, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
                           End Function)
        End Function

        Public Function HasImportPermissions(appID As Integer) As Boolean Implements IBulkImportManager.HasImportPermissions
            Return Execute(Async Function(s)
                               Using importer As IBulkImportService = s.CreateProxyInstance(Of IBulkImportService)
                                   Return Await importer.HasImportPermissionsAsync(appID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
            End Function)
        End Function

        Private Function ExecuteImport(f As Func(Of MassImportResults)) As MassImportResults
	        Dim retval As MassImportResults = f()
	        Me.CheckResultsForException(retval)
	        Return retval
        End Function

        Private Sub CheckResultsForException(ByVal results As MassImportResults)
            If results.ExceptionDetail IsNot Nothing Then
                If results.ExceptionDetail.ExceptionMessage IsNot Nothing AndAlso results.ExceptionDetail.ExceptionMessage.Contains("Timeout expired") Then
                    Throw New BulkImportManager.BulkImportSqlTimeoutException(results.ExceptionDetail)
                ElseIf results.ExceptionDetail.ExceptionMessage IsNot Nothing AndAlso results.ExceptionDetail.ExceptionMessage.Contains("##InsufficientPermissionsForImportException##") Then
                    Throw New BulkImportManager.InsufficientPermissionsForImportException(results.ExceptionDetail)
                ElseIf results.ExceptionDetail.ExceptionMessage IsNot Nothing AndAlso results.ExceptionDetail.ExceptionMessage.Contains("Server stack limit has been reached") Then
                    results.ExceptionDetail.ExceptionMessage += " This exception is known to be thrown when you import too many fields. This limit can vary tough, depending on the hardware, and the code. Try to import less fields."
                    Throw New BulkImportManager.BulkImportSqlException(results.ExceptionDetail)
                Else
                    Throw New BulkImportManager.BulkImportSqlException(results.ExceptionDetail)
                End If
            End If
        End Sub

        Protected Overrides Function ConvertSoapExceptionToRelativityException(soapException As SoapException) As Exception
	        If soapException.Detail.SelectNodes("ExceptionType").Item(0).InnerText = "Relativity.Core.Exception.InsufficientAccessControlListPermissions" Then
		        Return New BulkImportManager.InsufficientPermissionsForImportException(soapException.Detail.SelectNodes("ExceptionMessage")(0).InnerText, soapException)
	        End If

            Return MyBase.ConvertSoapExceptionToRelativityException(soapException)
        End Function
    End Class
End Namespace