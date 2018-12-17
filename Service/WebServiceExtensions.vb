Namespace kCura.WinEDDS.Service
    Public Module WebServiceExtensions
        Private _logger As Relativity.Logging.ILog = RelativityLogFactory.CreateLog(RelativityLogFactory.WinEDDSSubSystem)

        <System.Runtime.CompilerServices.Extension>
        Public Function RetryOnReLoginException(Of T)(ByVal input As System.Web.Services.Protocols.SoapHttpClientProtocol, serviceCall As Func(Of T), Optional ByVal retryOnFailure As Boolean = True) As T
            Dim tries As Int32 = 0
            While tries < Config.MaxReloginTries
                tries += 1
                Try
                    Return serviceCall()
                Catch ex As System.Exception
                    LogFailedServiceCall(ex, tries, retryOnFailure)

                    If retryOnFailure AndAlso TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
                        LogAttemptReLogin(ex)

                        Try
                            Helper.AttemptReLogin(input.Credentials, input.CookieContainer, tries, False)
                        Catch loginEx As System.Exception
                            LogFailedAttemptRelogin(loginEx)
                        End Try
                    Else
                        LogFailedExceptionCheck(ex)
                        Throw
                    End If
                End Try
            End While
            Return Nothing
        End Function

        <System.Runtime.CompilerServices.Extension>
        Public Sub RetryOnReLoginException(ByVal input As System.Web.Services.Protocols.SoapHttpClientProtocol, serviceCall As Action, Optional ByVal retryOnFailure As Boolean = True)
            Dim tries As Int32 = 0
            While tries < Config.MaxReloginTries
                tries += 1
                Try
                    serviceCall()
                    Exit Sub
                Catch ex As System.Exception
                    LogFailedServiceCall(ex, tries, retryOnFailure)

                    If retryOnFailure AndAlso TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
                        LogAttemptReLogin(ex)

                        Try
                            Helper.AttemptReLogin(input.Credentials, input.CookieContainer, tries, False)
                        Catch loginEx As System.Exception
                            LogFailedAttemptRelogin(loginEx)
                        End Try
                    Else
                        LogFailedExceptionCheck(ex)
                        Throw
                    End If
                End Try
            End While
        End Sub

        Private Sub LogFailedServiceCall(exception As Exception, tries As Int32, retryOnFailure As Boolean)
            Dim maxRetries As Int32 = Config.MaxReloginTries
            Dim token As String = kCura.WinEDDS.Service.Settings.AuthenticationToken
            Dim message As String = "An error occurred when retrying on a re-login exception"

            If retryOnFailure Then
                _logger.LogError(
                    exception,
                    message + " Currently on attempt {tries} out of {maxRetries}, using token {token}",
                    tries,
                    maxRetries,
                    token)
            Else
                _logger.LogError(
                    exception,
                    message + " Will not retry, using token {token}",
                    token)
            End If
        End Sub

        Private Sub LogAttemptReLogin(exception As Exception)
            _logger.LogError(exception, "Attempting re-login")
        End Sub

        Private Sub LogFailedExceptionCheck(exception As Exception)
            _logger.LogError(exception, "Will not attempt to retry")
        End Sub

        Private Sub LogFailedAttemptReLogin(exception As Exception)
            _logger.LogError(exception, "Failed to log in the user")
        End Sub
    End Module
End Namespace