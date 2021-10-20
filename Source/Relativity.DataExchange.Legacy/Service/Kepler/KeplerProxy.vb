Imports System.Collections.Generic
Imports System.Threading
Imports System.Threading.Tasks
Imports Polly
Imports Relativity.DataExchange
Imports Relativity.DataExchange.Service
Imports Relativity.Logging

Namespace kCura.WinEDDS.Service.Kepler
	Public Class KeplerProxy
		Implements IKeplerProxy

		Private ReadOnly _serviceProxyFactory As IServiceProxyFactory
		Private ReadOnly _retryPolicyFactories As List(Of IKeplerRetryPolicyFactory)

		Public Sub New(serviceProxyFactory As IServiceProxyFactory, logger As ILog)
			Me._serviceProxyFactory = serviceProxyFactory

			_retryPolicyFactories = New List(Of IKeplerRetryPolicyFactory)()
			_retryPolicyFactories.Add(new RetryableErrorsRetryPolicyFactory(AppSettings.Instance, logger))
			_retryPolicyFactories.Add(new ReLoginRetryPolicyFactory(AppSettings.Instance, serviceProxyFactory, logger))
			_retryPolicyFactories.Add(new HttpErrorRetryPolicyFactory(AppSettings.Instance, logger))
		End Sub

		Public Sub New(serviceProxyFactory As IServiceProxyFactory, logger As ILog, onRetry As Action(Of Exception, TimeSpan, Integer, Context))
			Me._serviceProxyFactory = serviceProxyFactory

			_retryPolicyFactories = New List(Of IKeplerRetryPolicyFactory)()
			_retryPolicyFactories.Add(new RetryableErrorsRetryPolicyFactory(AppSettings.Instance, logger, onRetry))
			_retryPolicyFactories.Add(new ReLoginRetryPolicyFactory(AppSettings.Instance, serviceProxyFactory, logger, onRetry))
			_retryPolicyFactories.Add(new HttpErrorRetryPolicyFactory(AppSettings.Instance, logger, onRetry))
		End Sub

		Public Function ExecuteAsyncWithoutRetries(Of T)(func As Func(Of IServiceProxyFactory, Task(Of T))) As Task(Of T) Implements IKeplerProxy.ExecuteAsyncWithoutRetries
			Return func(_serviceProxyFactory)
		End Function

		Public Function ExecuteAsync(Of T)(func As Func(Of IServiceProxyFactory, Task(Of T))) As Task(Of T) Implements IKeplerProxy.ExecuteAsync
			Return Policy _
				.WrapAsync(_retryPolicyFactories.Select(Function(retryPolicyFactory) retryPolicyFactory.CreateRetryPolicy(Of T)()).ToArray()) _
				.ExecuteAsync(Function() func(_serviceProxyFactory))
		End Function

		Public Function ExecuteAsync (Of T)(context As Context, cancellationToken As CancellationToken, func As Func(Of Context,CancellationToken,IServiceProxyFactory,Task(Of T))) As Task(Of T) Implements IKeplerProxy.ExecuteAsync
			Return Policy _
				.WrapAsync(_retryPolicyFactories.Select(Function(retryPolicyFactory) retryPolicyFactory.CreateRetryPolicy(Of T)()).ToArray()) _
				.ExecuteAsync(Function(ctx As Context, ct As CancellationToken) func(context, cancellationToken, _serviceProxyFactory), context, cancellationToken)
		End Function

        Public Function ExecuteAsync(func As Func(Of IServiceProxyFactory, Task)) As Task Implements IKeplerProxy.ExecuteAsync
			Return Policy _
				.WrapAsync(_retryPolicyFactories.Select(Function(retryPolicyFactory) retryPolicyFactory.CreateRetryPolicy()).ToArray()) _
				.ExecuteAsync(Function() func(_serviceProxyFactory))
		End Function

        Public Function ExecutePostAsync(endpointAddress As String, body As String) As Task(Of String) Implements IKeplerProxy.ExecutePostAsync
            Return _serviceProxyFactory.ExecutePostAsync(endpointAddress, body)
        End Function
    End Class
End Namespace