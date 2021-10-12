Imports System.Net
Imports System.Web.Services.Protocols
Imports kCura.WinEDDS.Service
Imports kCura.WinEDDS.Service.Replacement
Imports Relativity.DataExchange
Imports Relativity.Logging

Namespace kCura.WinEDDS
	Friend Class CertificateValidator
		ReadOnly _settings As IAppSettings
		ReadOnly _logger As ILog
		ReadOnly _correlationIdProvider As Func(Of String)
		ReadOnly _cookieContainer As CookieContainer

		Public Sub New(settings As IAppSettings, cookieContainer As CookieContainer, logger As ILog, correlationIdProvider As Func(Of String))
			_settings = settings
			_cookieContainer = cookieContainer
			_logger = logger
			_correlationIdProvider = correlationIdProvider
		End Sub

		''' <summary>
		''' Checks that the https certificate is trusted for this connection. Throws an exception if anything but trust failure happens
		''' </summary>
		''' <returns>True if the certificate is trusted. False otherwise.</returns>
		Public Function IsCertificateTrusted() As Boolean
			Dim credential As NetworkCredential = DirectCast(CredentialCache.DefaultCredentials, NetworkCredential)

			Dim exceptionFromKepler As Exception = Nothing
			Dim isKeplerCertificateValid As Boolean? = ValidateKeplerCertificate(credential, exceptionFromKepler)
			If isKeplerCertificateValid.HasValue Then
				Return isKeplerCertificateValid.Value
			End If

			Dim exceptionFromWebApi As Exception = Nothing
			Dim isWebApiCertificateTrusted As Boolean? = ValidateWebApiCertificate(credential, exceptionFromWebApi)
			If isWebApiCertificateTrusted.HasValue Then
				Return isWebApiCertificateTrusted.Value
			End If

			If _settings.UseKepler Then
				Throw exceptionFromKepler
			Else
				Throw exceptionFromWebApi
			End If
		End Function

		Private Function ValidateKeplerCertificate(credential As NetworkCredential, ByRef exceptionFromKepler As Exception) As Boolean?
			Dim keplerRelativityManager As IRelativityManager = ManagerFactory.CreateRelativityManager(credential, _cookieContainer, _correlationIdProvider, useKepler:=True)
			Try
				keplerRelativityManager.ValidateCertificate() ' We need to call a Kepler endpoint without multiple retries
				Return True
			Catch ex As SoapException When ex.Message = "The service endpoint denied the request."
				Me._logger.LogVerbose(ex, "Expected authorization exception was thrown when validating certificate.")
				Return True
			Catch ex As Exception
				Dim webException As WebException = TryCast(ex?.InnerException?.InnerException, WebException)
				If webException?.Status = WebExceptionStatus.TrustFailure Then
					Me._logger.LogInformation("Kepler certificate is untrusted.")
					Return False
				End If

				exceptionFromKepler = If(webException, ex)
				Me._logger.LogError(ex, "Unexpected error occurred when validating Kepler certificate.")
				Return Nothing
			End Try

		End Function

		Private Function ValidateWebApiCertificate(credential As NetworkCredential, ByRef exceptionFromWebApi As Exception) As Boolean?
			Dim webApiRelativityManager As IRelativityManager = ManagerFactory.CreateRelativityManager(credential, _cookieContainer, _correlationIdProvider, useKepler:=False)
			Try
				' Only if this line bombs do we say the cert is untrusted
				webApiRelativityManager.ValidateCertificate()
				Return True
			Catch ex As WebException
				If (ex.Status = WebExceptionStatus.TrustFailure) Then
					Me._logger.LogInformation("WebApi certificate is untrusted.")
					Return False
				Else
					Me._logger.LogError(ex, "Unexpected error occurred when validating WebApi certificate.")
					exceptionFromWebApi = ex
					Return Nothing
				End If
			End Try
		End Function
	End Class
End Namespace
