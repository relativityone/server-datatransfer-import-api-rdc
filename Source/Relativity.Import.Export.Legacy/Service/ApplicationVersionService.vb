Imports System.Globalization
Imports System.Net
Imports System.Net.Http
Imports System.Text
Imports System.Threading.Tasks
Imports kCura.WinEDDS.Service
Imports Relativity.Import.Export

Public Class ApplicationVersionService 
	Implements IApplicationVersionService

	Private Const InstanceDetailsServiceRelPath As String = "/Relativity.Rest/api/Relativity.Services.InstanceDetails.IInstanceDetailsModule/InstanceDetailsService/GetRelativityVersionAsync"

	Private ReadOnly _credentials As NetworkCredential
	Private ReadOnly _webApiUrl As String
	Private ReadOnly _restApiUrl As Uri

	Public Sub New(credentials As NetworkCredential, webApiUrl As String)
		_credentials = credentials
		_webApiUrl = webApiUrl

		Dim webServerBaseUrlString As String = New Uri(webApiUrl).GetLeftPart(UriPartial.Authority)
		Dim webServerBaseUrl As New Uri(webServerBaseUrlString)

		' create url to instance details kepler service
		_restApiUrl = New Uri(webServerBaseUrl, InstanceDetailsServiceRelPath)
	End Sub

	Public Async Function RetrieveRelativityVersion() As Task(Of Version) Implements IApplicationVersionService.RetrieveRelativityVersion

		Dim httpClientHelper As New HttpClientHelper()

		Dim authorizationHeader As String = GetAuthorizationHeader(_credentials)
		Dim httpResponse As HttpResponseMessage
		Try
			httpResponse = Await httpClientHelper.DoPost(_restApiUrl, authorizationHeader, string.Empty)

		Catch ex As Exception
			Throw New HttpServiceException($"Can not connect to Kepler service: {_restApiUrl.ToString()}", ex)
		End Try

		If Not httpResponse.IsSuccessStatusCode
			Throw New HttpServiceException(String.Format($"Can not retrieve Relativity version from Kepler servivce: {_restApiUrl.ToString()}. Staus code {httpResponse.StatusCode}"))
		End If

		' Relativity version is stored in additional quotation marks
		Dim relVersionString As String = httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult().TrimStart(""""c).TrimEnd(""""c)

		Return Version.Parse(relVersionString)
	End Function

	Public Async Function RetrieveImportExportWebApiVersion() As Task(Of Version) Implements IApplicationVersionService.RetrieveImportExportWebApiVersion
		Try
			Dim relativityManager As New RelativityManager(_credentials, New CookieContainer(), _webApiUrl)
			Dim ver As String = Await Task.Run(Function() relativityManager.GetImportExportWebApiVersion()).ConfigureAwait(false)
		Return Version.Parse(ver)
		Catch ex As Exception
			Throw New HttpServiceException("Can not connect to WebApi service", ex)
		End Try
	End Function

	Private Function GetAuthorizationHeader(cred As NetworkCredential) As String
		If _credentials.UserName = kCura.WinEDDS.Credentials.Constants.OAuthWebApiBearerTokenUserName Then
			Return string.Format(CultureInfo.InvariantCulture, "Bearer {0}", cred.Password)
		Else
			Dim plainUserPwd As String = String.Format(CultureInfo.InvariantCulture, "{0}:{1}", cred.UserName, cred.Password)
			Return string.Format(CultureInfo.InvariantCulture, "Basic {0}", Convert.ToBase64String(Encoding.ASCII.GetBytes(plainUserPwd)))
		End If
	End Function

End Class
