Imports System.Threading.Tasks
Imports System.Web.Services.Protocols
Imports kCura.WinEDDS.Mapping
Imports kCura.WinEDDS.Service.Kepler
Imports Relativity.DataExchange
Imports Relativity.DataExchange.Service
Imports Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models
Imports Relativity.Logging
Imports Relativity.Services.Exceptions

Namespace kCura.WinEDDS.Service.Replacement
    Public MustInherit Class KeplerManager
        Implements IDisposable

        Private ReadOnly _serviceProxyFactory As IServiceProxyFactory
        Private ReadOnly _keplerProxy As IKeplerProxy
        Private ReadOnly _exceptionMapper As IServiceExceptionMapper
        
        ' Exception message that tells user that import of the current batch Is still in progress.
        Private Const BatchInProgressMessage As String = "Batch In Progress"

        Protected ReadOnly CorrelationIdFunc As Func(Of String)

        Public Sub New(serviceProxyFactory As IServiceProxyFactory, exceptionMapper As IServiceExceptionMapper, correlationIdFunc As Func(Of String))
            Me._serviceProxyFactory = serviceProxyFactory
            Me._keplerProxy = New KeplerProxy(serviceProxyFactory, Log.Logger)
            Me._exceptionMapper = exceptionMapper
            Me.CorrelationIdFunc = correlationIdFunc
        End Sub

        Protected Function Execute(Of T)(func As Func(Of IServiceProxyFactory, Task(Of T))) As T
            Return ExecuteAsync(func).GetAwaiter().GetResult()
        End Function

        Protected Function ExecuteWithoutRetries(Of T)(func As Func(Of IServiceProxyFactory, Task(Of T))) As T
            Return ExecuteWithoutRetriesAsync(func).GetAwaiter().GetResult()
        End Function

        Protected Function Execute(func As Func(Of IServiceProxyFactory, Task(Of DataSetWrapper))) As DataSet
            Return ExecuteAsync(func).GetAwaiter().GetResult().Unwrap()
        End Function

        Private Async Function ExecuteAsync(Of T)(func As Func(Of IServiceProxyFactory, Task(Of T))) As Task(Of T)
            Try
                Return Await Me._keplerProxy.ExecuteAsync(func).ConfigureAwait(False)
            Catch ex As ConflictException When ex.Message.Contains(BatchInProgressMessage) 
                Throw New ImportExportException($"Timeout occurred after {AppSettings.Instance.BatchInProgressNumberOfRetries} retries with {AppSettings.Instance.BatchInProgressWaitTimeInSeconds} seconds timeout. The server is still processing the request.")
            Catch serviceException As ServiceException
                Dim soapException As SoapException = _exceptionMapper.Map(serviceException)
                UnpackSoapException(soapException)
                Throw soapException
            Catch ex As Exception
                Throw
            End Try
        End Function

        Private Async Function ExecuteWithoutRetriesAsync(Of T)(func As Func(Of IServiceProxyFactory, Task(Of T))) As Task(Of T)
            Try
                Return Await Me._keplerProxy.ExecuteAsyncWithoutRetries(func).ConfigureAwait(False)
            Catch serviceException As ServiceException
                Dim soapException As SoapException = _exceptionMapper.Map(serviceException)
                UnpackSoapException(soapException)
                Throw soapException
            Catch ex As Exception
                Throw
            End Try
        End Function

        ''' <summary>
        ''' Each Kepler manager can unpack and handle SoapException in its own way.
        ''' Common SoapException logic can be added here.
        ''' </summary>
        ''' <param name="soapException">SoapException</param>
        Protected Overridable Function ConvertSoapExceptionToRelativityException(soapException As SoapException) As Exception
            Return Nothing
        End Function

        Private Sub UnpackSoapException(soapException As SoapException)
	        Dim exceptionToThrow As Exception = Nothing
	        Try
		        exceptionToThrow = ConvertSoapExceptionToRelativityException(soapException)
	        Catch
	        End Try
	        If Not exceptionToThrow Is Nothing Then Throw exceptionToThrow
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
	        Me._serviceProxyFactory?.Dispose()
        End Sub
    End Class
End Namespace
