Imports System.Net
Imports System.Threading.Tasks
Imports Polly
Imports Relativity.DataExchange
Imports Relativity.DataExchange.Service
Imports Relativity.Logging
Imports Relativity.Services.Exceptions

Namespace kCura.WinEDDS.Service.Kepler
	Public Class KeplerProxy
		Implements IKeplerProxy

		Private ReadOnly _logger As ILog
		Private ReadOnly _serviceProxyFactory As IServiceProxyFactory
		Private _canRetryOnNotAuthorizedException As Boolean

		Public Sub New(serviceProxyFactory As IServiceProxyFactory, logger As ILog)
			Me._serviceProxyFactory = serviceProxyFactory
			Me._logger = logger
			Me._canRetryOnNotAuthorizedException = True
		End Sub

		Public Function ExecuteAsync(Of T)(func As Func(Of IServiceProxyFactory, Task(Of T))) As Task(Of T) Implements IKeplerProxy.ExecuteAsync
			Return Policy(Of T) _
				.Handle(Of NotAuthorizedException)(Function(exception) _canRetryOnNotAuthorizedException) _
				.WaitAndRetryAsync(AppSettings.Instance.MaxReloginTries, AddressOf GetWaitTimeBeforeRetry, AddressOf OnRetryAsync) _
				.ExecuteAsync(Function() func(_serviceProxyFactory))
		End Function

		Public Function ExecuteAsync(func As Func(Of IServiceProxyFactory, Task)) As Task Implements IKeplerProxy.ExecuteAsync
			Return Policy _
				.Handle(Of NotAuthorizedException)(Function(exception) _canRetryOnNotAuthorizedException) _
				.WaitAndRetryAsync(AppSettings.Instance.MaxReloginTries, AddressOf GetWaitTimeBeforeRetry, AddressOf OnRetryAsync) _
				.ExecuteAsync(Function() func(_serviceProxyFactory))
		End Function

		Private Async Function OnRetryAsync(exception As Exception, timeSpan As TimeSpan) As Task
			Me._logger.LogWarning(exception, $"Call to Kepler service failed due to '{NameOf(NotAuthorizedException)}'. Refreshing token.")
			Dim emptyCredentials As NetworkCredential = New NetworkCredential()
			Dim credentials As NetworkCredential = Await Helper.GetUpdatedCredentialsAsync(emptyCredentials).ConfigureAwait(False)
			If Not credentials Is emptyCredentials Then
				Me._serviceProxyFactory.UpdateCredentials(credentials)
			Else
				Me._canRetryOnNotAuthorizedException = False ' RelativityWebApiCredentialsProvider failed to refresh token
			End If
		End Function

		Private Function OnRetryAsync(Of TResult)(result As DelegateResult(Of TResult), timeSpan As TimeSpan) As Task
			Return OnRetryAsync(result.Exception, timeSpan)
		End Function

		Private Shared Function GetWaitTimeBeforeRetry(retryNumber As Integer) As TimeSpan
			Dim numberOfSecondsToWait As Integer = Math.Max(0, retryNumber - 1)
			Return TimeSpan.FromSeconds(numberOfSecondsToWait)
		End Function
	End Class
End Namespace
