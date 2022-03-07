﻿Imports System.Net
Imports kCura.WinEDDS.Mapping
Imports kCura.WinEDDS.Service.Kepler
Imports kCura.WinEDDS.Service.Replacement
Imports Relativity.DataExchange
Imports Relativity.DataExchange.Logger
Imports Relativity.DataExchange.Service
Imports Relativity.DataExchange.Service.WebApiVsKeplerSwitch

Namespace kCura.WinEDDS.Service
	Public Class ManagerFactory

		Private Shared ReadOnly Lock As New Object

		''' <summary>
		''' Do not use. It is internal for testing purpose.
		''' </summary>
		Friend Shared _webApiVsKeplerLazy As Lazy(Of IWebApiVsKepler)

		''' <summary>
		''' Do not use. It is internal for testing purpose.
		''' </summary>
		Friend Shared _currentUrl As String

		''' <summary>
		''' Do not use. It is internal for testing purpose.
		''' </summary>
		Friend Shared _currentCredentials As NetworkCredential

		''' <summary>
		''' Do not use. It is internal for testing purpose.
		''' </summary>
		Friend Shared _connectionInfo As IServiceConnectionInfo

		''' <summary>
		''' Do not use. It is internal for testing purpose.
		''' </summary>
		Friend Shared Function Initialize(credentials As NetworkCredential, correlationIdFunc As Func(Of String), Optional url As String = Nothing) As IServiceConnectionInfo
			credentials.ThrowIfNull(nameof(credentials))
			SyncLock Lock
				If String.IsNullOrWhiteSpace(url) Then
					url = AppSettings.Instance.WebApiServiceUrl
				End If

				If url <> _currentUrl OrElse Not CredentialsAreEqual(credentials, _currentCredentials) Then
					Dim urlAsUri As Uri = New Uri(url)
					Dim connectionInfo As IServiceConnectionInfo = New KeplerServiceConnectionInfo(urlAsUri, credentials)

					Dim webApiVsKeplerLazy As Lazy(Of IWebApiVsKepler) = New Lazy(Of IWebApiVsKepler)(
						Function()
							Dim webApiVsKeplerFactory As WebApiVsKeplerFactory = New WebApiVsKeplerFactory(RelativityLogger.Instance)
							Return webApiVsKeplerFactory.Create(urlAsUri, credentials, correlationIdFunc)
						End Function)

					_connectionInfo = connectionInfo
					_currentUrl = url
					_currentCredentials = credentials
					_webApiVsKeplerLazy = webApiVsKeplerLazy
				End If

				_connectionInfo.ThrowIfNull(nameof(_connectionInfo))
				Return _connectionInfo
			End SyncLock
		End Function

		Friend Shared Sub InvalidateCache()
			SyncLock Lock
				_connectionInfo = Nothing
				_currentUrl = Nothing
				_currentCredentials = Nothing
				_webApiVsKeplerLazy = Nothing
			End SyncLock
		End Sub

		Public Shared Function CreateApplicationVersionService(instance As RelativityInstanceInfo, appSettings As IAppSettings, logger As Global.Relativity.Logging.ILog, correlationIdFunc As Func(Of String)) As IApplicationVersionService
			Initialize(CType(instance.Credentials, NetworkCredential), correlationIdFunc)
			If _webApiVsKeplerLazy.Value.UseKepler() Then
				Return New KeplerApplicationVersionService(instance, appSettings, logger)
			End If
			Return New ApplicationVersionService(instance, appSettings, logger)
		End Function

		Public Shared Function CreateAuditManager(ByVal credentials As NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer, correlationIdFunc As Func(Of String)) As IAuditManager
			Dim connectionInfo As IServiceConnectionInfo = Initialize(credentials, correlationIdFunc)
			If _webApiVsKeplerLazy.Value.UseKepler() Then
				Return New KeplerAuditManager(New KeplerServiceProxyFactory(connectionInfo), New KeplerExceptionMapper(), correlationIdFunc)
			End If
			Return New AuditManager(credentials, cookieContainer)
		End Function

		Public Shared Function CreateBulkImportManager(ByVal credentials As NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer, correlationIdFunc As Func(Of String)) As IBulkImportManager
			Dim connectionInfo As IServiceConnectionInfo = Initialize(credentials, correlationIdFunc)
			If _webApiVsKeplerLazy.Value.UseKepler() Then
				Return New KeplerBulkImportManager(New KeplerServiceProxyFactory(connectionInfo), New KeplerExceptionMapper(), credentials, correlationIdFunc)
			End If
			Return New BulkImportManager(credentials, cookieContainer)
		End Function

		Public Shared Function CreateCaseManager(ByVal credentials As NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer, correlationIdFunc As Func(Of String)) As ICaseManager
			Dim connectionInfo As IServiceConnectionInfo = Initialize(credentials, correlationIdFunc)
			If _webApiVsKeplerLazy.Value.UseKepler() Then
				Return New KeplerCaseManager(New KeplerServiceProxyFactory(connectionInfo), New KeplerExceptionMapper(), correlationIdFunc)
			End If
			Return New CaseManager(credentials, cookieContainer)
		End Function

		Public Shared Function CreateCodeManager(ByVal credentials As NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer, correlationIdFunc As Func(Of String)) As ICodeManager
			Dim connectionInfo As IServiceConnectionInfo = Initialize(credentials, correlationIdFunc)
			If _webApiVsKeplerLazy.Value.UseKepler() Then
				Return New KeplerCodeManager(New KeplerServiceProxyFactory(connectionInfo), New KeplerExceptionMapper(), correlationIdFunc)
			End If
			Return New CodeManager(credentials, cookieContainer)
		End Function

		Public Shared Function CreateDocumentManager(ByVal credentials As NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer, correlationIdFunc As Func(Of String)) As IDocumentManager
			Dim connectionInfo As IServiceConnectionInfo = Initialize(credentials, correlationIdFunc)
			If _webApiVsKeplerLazy.Value.UseKepler() Then
				Return New KeplerDocumentManager(New KeplerServiceProxyFactory(connectionInfo), New KeplerExceptionMapper(), correlationIdFunc)
			End If
			Return New DocumentManager(credentials, cookieContainer)
		End Function

		Public Shared Function CreateExportManager(ByVal credentials As NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer, correlationIdFunc As Func(Of String)) As IExportManager
			Dim connectionInfo As IServiceConnectionInfo = Initialize(credentials, correlationIdFunc)
			If _webApiVsKeplerLazy.Value.UseKepler() Then
				Return New KeplerExportManager(New KeplerServiceProxyFactory(connectionInfo), New KeplerExceptionMapper(), correlationIdFunc)
			End If
			Return New ExportManager(credentials, cookieContainer)
		End Function

		Public Shared Function CreateFieldManager(ByVal credentials As NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer, correlationIdFunc As Func(Of String)) As IFieldManager
			Dim connectionInfo As IServiceConnectionInfo = Initialize(credentials, correlationIdFunc)
			If _webApiVsKeplerLazy.Value.UseKepler() Then
				Return New KeplerFieldManager(New KeplerServiceProxyFactory(connectionInfo), New KeplerExceptionMapper(), correlationIdFunc)
			End If
			Return New FieldManager(credentials, cookieContainer)
		End Function

		Public Shared Function CreateFieldQuery(ByVal credentials As NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer, correlationIdFunc As Func(Of String)) As IFieldQuery
			Dim connectionInfo As IServiceConnectionInfo = Initialize(credentials, correlationIdFunc)
			If _webApiVsKeplerLazy.Value.UseKepler() Then
				Return New KeplerFieldQuery(New KeplerServiceProxyFactory(connectionInfo), New KeplerExceptionMapper(), correlationIdFunc)
			End If
			Return New FieldQuery(credentials, cookieContainer)
		End Function

		Public Shared Function CreateFileIO(ByVal credentials As NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer, correlationIdFunc As Func(Of String)) As IFileIO
			Dim connectionInfo As IServiceConnectionInfo = Initialize(credentials, correlationIdFunc)
			If _webApiVsKeplerLazy.Value.UseKepler() Then
				Return New KeplerFileIO(New KeplerServiceProxyFactory(connectionInfo), New KeplerExceptionMapper(), correlationIdFunc)
			End If
			Return New FileIO(credentials, cookieContainer)
		End Function

		Public Shared Function CreateFileIO(ByVal credentials As NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer, ByVal timeout As Int32, correlationIdFunc As Func(Of String)) As IFileIO
			Dim connectionInfo As IServiceConnectionInfo = Initialize(credentials, correlationIdFunc)
			If _webApiVsKeplerLazy.Value.UseKepler() Then
				Return New KeplerFileIO(New KeplerServiceProxyFactory(connectionInfo), New KeplerExceptionMapper(), correlationIdFunc)
			End If
			Return New FileIO(credentials, cookieContainer) With {
				.Timeout = timeout
				}
		End Function

		Public Shared Function CreateFolderManager(ByVal credentials As NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer, correlationIdFunc As Func(Of String)) As IFolderManager
			Dim connectionInfo As IServiceConnectionInfo = Initialize(credentials, correlationIdFunc)
			If _webApiVsKeplerLazy.Value.UseKepler() Then
				Return New KeplerFolderManager(New KeplerServiceProxyFactory(connectionInfo), New KeplerExceptionMapper(), correlationIdFunc)
			End If
			Return New FolderManager(credentials, cookieContainer)
		End Function

		Public Shared Function CreateObjectManager(ByVal credentials As NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer, correlationIdFunc As Func(Of String)) As IObjectManager
			Dim connectionInfo As IServiceConnectionInfo = Initialize(credentials, correlationIdFunc)
			If _webApiVsKeplerLazy.Value.UseKepler() Then
				Return New KeplerObjectManager(New KeplerServiceProxyFactory(connectionInfo), New KeplerExceptionMapper(), correlationIdFunc)
			End If
			Return New ObjectManager(credentials, cookieContainer)
		End Function

		Public Shared Function CreateObjectTypeManager(ByVal credentials As NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer, correlationIdFunc As Func(Of String)) As IObjectTypeManager
			Dim connectionInfo As IServiceConnectionInfo = Initialize(credentials, correlationIdFunc)
			If _webApiVsKeplerLazy.Value.UseKepler() Then
				Return New KeplerObjectTypeManager(New KeplerServiceProxyFactory(connectionInfo), New KeplerExceptionMapper(), correlationIdFunc)
			End If
			Return New ObjectTypeManager(credentials, cookieContainer)
		End Function

		Public Shared Function CreateProductionManager(ByVal credentials As NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer, correlationIdFunc As Func(Of String)) As IProductionManager
			Dim connectionInfo As IServiceConnectionInfo = Initialize(credentials, correlationIdFunc)
			If _webApiVsKeplerLazy.Value.UseKepler() Then
				Return New KeplerProductionManager(New KeplerServiceProxyFactory(connectionInfo), New KeplerExceptionMapper(), correlationIdFunc)
			End If
			Return New ProductionManager(credentials, cookieContainer)
		End Function

		Public Shared Function CreateRelativityManager(
		                                               ByVal credentials As NetworkCredential,
		                                               ByVal cookieContainer As System.Net.CookieContainer,
		                                               correlationIdFunc As Func(Of String),
		                                               Optional useKepler As Boolean? = Nothing) As IRelativityManager
			Dim connectionInfo As IServiceConnectionInfo = Initialize(credentials, correlationIdFunc)
			Dim useKeplerValue As Boolean = If(useKepler, _webApiVsKeplerLazy.Value.UseKepler())
			If useKeplerValue Then
				Return New KeplerRelativityManager(New KeplerServiceProxyFactory(connectionInfo), new KeplerExceptionMapper(), correlationIdFunc)
			End If
			Return New RelativityManager(credentials, cookieContainer)
		End Function

		Public Shared Function CreateSearchManager(ByVal credentials As NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer, correlationIdFunc As Func(Of String)) As ISearchManager
			Dim connectionInfo As IServiceConnectionInfo = Initialize(credentials, correlationIdFunc)
			If _webApiVsKeplerLazy.Value.UseKepler() Then
				Return New KeplerSearchManager(New KeplerServiceProxyFactory(connectionInfo), New KeplerExceptionMapper(), correlationIdFunc)
			End If
			Return New SearchManager(credentials, cookieContainer)
		End Function

		Public Shared Function CreateUserManager(ByVal credentials As NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer, correlationIdFunc As Func(Of String)) As IUserManager
			Dim connectionInfo As IServiceConnectionInfo = Initialize(credentials, correlationIdFunc)
			If _webApiVsKeplerLazy.Value.UseKepler() Then
				Return New KeplerUserManager(New KeplerServiceProxyFactory(connectionInfo), New KeplerExceptionMapper(), correlationIdFunc)
			End If
			Return New UserManager(credentials, cookieContainer)
		End Function

		Public Shared Function CreateUserManager(ByVal credentials As NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer, ByVal webServiceUrl As String, correlationIdFunc As Func(Of String)) As IUserManager
			Dim connectionInfo As IServiceConnectionInfo = Initialize(credentials, correlationIdFunc, webServiceUrl)
			If _webApiVsKeplerLazy.Value.UseKepler() Then
				Return New KeplerUserManager(New KeplerServiceProxyFactory(connectionInfo), New KeplerExceptionMapper(), correlationIdFunc)
			End If
			Return New UserManager(credentials, cookieContainer, webServiceUrl)
		End Function

		Private Shared Function CredentialsAreEqual(cred1 As NetworkCredential, cred2 As NetworkCredential) As Boolean
			If IsNothing(cred1) And IsNothing(cred2) Then
				Return True
			End If

			If IsNothing(cred1) Or IsNothing(cred2) Then
				Return False
			End If

			Return cred1.Domain = cred2.Domain And
				   cred1.UserName = cred2.UserName And
				   cred1.Password = cred2.Password
		End Function
	End Class
End Namespace