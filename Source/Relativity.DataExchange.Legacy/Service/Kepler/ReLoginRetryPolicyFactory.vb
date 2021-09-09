Imports System.Net
Imports System.Threading.Tasks
Imports kCura.WinEDDS.Service
Imports Polly
Imports Relativity.DataExchange
Imports Relativity.DataExchange.Service
Imports Relativity.Logging
Imports Relativity.Services.Exceptions

Namespace kCura.WinEDDS.Service.Kepler 
	Public Class ReLoginRetryPolicyFactory 
		Implements IKeplerRetryPolicyFactory

		Private ReadOnly _logger As ILog
		Private ReadOnly _serviceProxyFactory As IServiceProxyFactory
		Private ReadOnly _onRetry As Action(Of Exception, TimeSpan, Integer, Context)
		Private ReadOnly _settings As IAppSettings
		Private _canRetryOnNotAuthorizedException As Boolean
		
		Public Sub New(settings As IAppSettings, serviceProxyFactory As IServiceProxyFactory, logger As ILog)
			Me._serviceProxyFactory = serviceProxyFactory
			Me._logger = logger
			Me._canRetryOnNotAuthorizedException = True
			Me._settings = settings
		End Sub

		Public Sub New(settings As IAppSettings, serviceProxyFactory As IServiceProxyFactory, logger As ILog, onRetry As Action(Of Exception, TimeSpan, Integer, Context))
			Me._serviceProxyFactory = serviceProxyFactory
			Me._logger = logger
			Me._canRetryOnNotAuthorizedException = True
			Me._onRetry = onRetry
			Me._settings = settings
		End Sub

		Public Function CreateRetryPolicy() As IAsyncPolicy Implements IKeplerRetryPolicyFactory.CreateRetryPolicy
			Return Policy _
				.Handle(Of NotAuthorizedException)(Function(exception) _canRetryOnNotAuthorizedException) _
				.WaitAndRetryAsync(Me._settings.MaxReloginTries, AddressOf GetWaitTimeBeforeRetry, AddressOf OnRetryAsync)
		End Function

		Public Function CreateRetryPolicy (Of T)() As IAsyncPolicy(Of T) Implements IKeplerRetryPolicyFactory.CreateRetryPolicy
			Return Policy(Of T) _
				.Handle(Of NotAuthorizedException)(Function(exception) _canRetryOnNotAuthorizedException) _
				.WaitAndRetryAsync(Me._settings.MaxReloginTries, AddressOf GetWaitTimeBeforeRetry, AddressOf OnRetryAsync)
		End Function

		Private Async Function OnRetryAsync(exception As Exception, timeSpan As TimeSpan, retryCount As Int32, context As Context) As Task
			If (Me._onRetry Is Nothing) Then
				Me._logger.LogWarning(exception, $"Call to Kepler service failed due to '{NameOf(NotAuthorizedException)}'. Refreshing token.")
			Else
				Me._onRetry.Invoke(exception, timeSpan, retryCount, context)
			End If
			
			Dim emptyCredentials As NetworkCredential = New NetworkCredential()
			Dim credentials As NetworkCredential = Await Helper.GetUpdatedCredentialsAsync(emptyCredentials).ConfigureAwait(False)
			If Not credentials Is emptyCredentials Then
				Me._serviceProxyFactory.UpdateCredentials(credentials)
			Else
				Me._canRetryOnNotAuthorizedException = False ' RelativityWebApiCredentialsProvider failed to refresh token
			End If
		End Function

		Private Function OnRetryAsync(Of TResult)(result As DelegateResult(Of TResult), timeSpan As TimeSpan, retryCount As Int32, context As Context) As Task
			Return OnRetryAsync(result.Exception, timeSpan, retryCount, context)
		End Function

		Private Shared Function GetWaitTimeBeforeRetry(retryNumber As Integer) As TimeSpan
			Dim numberOfSecondsToWait As Integer = Math.Max(0, retryNumber - 1)
			Return TimeSpan.FromSeconds(numberOfSecondsToWait)
		End Function
	End Class
End Namespace