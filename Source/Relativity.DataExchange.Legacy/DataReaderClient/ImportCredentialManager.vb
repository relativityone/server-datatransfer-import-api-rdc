﻿Imports System.Net
Imports System.Collections.Generic
Imports System.Threading
Imports Relativity.DataExchange

Friend Class ImportCredentialManager

	Private Shared CredentialCache As List(Of CredentialEntry)
	Private Shared _WebServiceURL As String

	Private Shared _lockObject As New System.Object

	Public Shared Property WebServiceURL As String
		Get
			Return _WebServiceURL
		End Get
		Set(value As String)
			If value Is Nothing Then
				value = String.Empty
			End If
			_WebServiceURL = value.Trim.ToLower
			AppSettings.Instance.ProgrammaticWebApiServiceUrl = value
		End Set
	End Property

	''' <summary>
	''' Authenticates in Relativity and returns credentials.
	''' When <paramref name="UserName"/> is empty then Integrated Windows Authentication is used.
	''' When <paramref name="UserName"/> is equal to <see cref="kCura.WinEDDS.Credentials.Constants.OAuthWebApiBearerTokenUserName"/>
	''' then bearer token authentication is used.
	''' Otherwise it uses standard username and password authentication
	'''  </summary>
	''' <param name="UserName">username or <see cref="kCura.WinEDDS.Credentials.Constants.OAuthWebApiBearerTokenUserName"/></param>
	''' <param name="Password">password or bearer token</param>
	''' <returns>Credentials <see cref="SessionCredentials"/></returns>
	Public Shared Function GetCredentials(ByVal UserName As String, ByVal Password As String) As SessionCredentials
		' this function needs to be thread safe so that multiple simultaneous threads could call it

		' data cleanup first
		If UserName Is Nothing Then
			UserName = String.Empty
		Else
			UserName = UserName.Trim
		End If

		If Password Is Nothing Then
			Password = String.Empty
		Else
			Password = Password.Trim()
		End If

		Dim retVal As SessionCredentials = Nothing
		Dim cachedCreds As Boolean = False

		SyncLock _lockObject
			' This section needs to be sync locked

			' 1. look for matching credentials in the cache, return if found
			Dim foundCred As CredentialEntry = FindCredentials(UserName, Password)
			If Not foundCred Is Nothing Then
				retVal = foundCred.SessionCredentials
				cachedCreds = True
			Else

				' 2. credential in cache not found, so actually log in and create credentials
				Dim logger As Relativity.Logging.ILog = RelativityLogFactory.CreateLog(RelativityLogFactory.DefaultSubSystem)
				Dim token As CancellationToken = CancellationToken.None
				Dim creds As NetworkCredential = Nothing
				Dim cookieMonster As New CookieContainer

				Try
					If String.IsNullOrEmpty(UserName) Then
						creds = kCura.WinEDDS.Api.LoginHelper.LoginWindowsAuth(cookieMonster, WebServiceURL, token, logger)
					Else
						creds = kCura.WinEDDS.Api.LoginHelper.LoginUsernamePassword(UserName.Trim(), Password.Trim(), cookieMonster, WebServiceURL, token, logger)
					End If
				Catch ex As kCura.WinEDDS.Exceptions.CredentialsNotSupportedException
					Throw
				Catch ex As kCura.WinEDDS.Exceptions.InvalidLoginException
					Throw
				Catch ex As RelativityNotSupportedException
					Throw
				Catch ex As Exception
					Throw New System.Exception("Unknown failure during authentication", ex)
				End Try

				' add credentials to cache and return session credentials to caller
				If Not creds Is Nothing Then
					retVal = AddCredentials(UserName, Password, creds, cookieMonster).SessionCredentials()
				End If
				cachedCreds = False
			End If
		End SyncLock

		If retVal Is Nothing Then
			Throw New System.Exception("Login failed")
		End If

		If retVal.Credentials Is Nothing Then
			If cachedCreds Then
				Throw New ImportCredentialException("Invalid credentials found in cache", UserName, WebServiceURL)
			Else
				Throw New ImportCredentialException("Invalid credentials received from Login", UserName, WebServiceURL)
			End If
		End If

		Return retVal
	End Function

	Private Shared Function FindCredentials(ByVal UserName As String, ByVal Password As String) As CredentialEntry
		If CredentialCache Is Nothing Then
			Return Nothing
		End If

		For Each ce As CredentialEntry In CredentialCache
			If ce.UserName = UserName And ce.PassWord = Password And ce.URL = WebServiceURL Then
				Return ce
			End If
		Next

		Return Nothing
	End Function

	Private Shared Function AddCredentials(ByVal UserName As String, ByVal Password As String, ByVal creds As NetworkCredential, ByVal cookieMonster As CookieContainer) As CredentialEntry
		If CredentialCache Is Nothing Then
			CredentialCache = New List(Of CredentialEntry)
		End If

		Dim ce As New CredentialEntry

		ce.UserName = UserName
		ce.PassWord = Password
		ce.Credentials = creds
		ce.TapiCredential = creds
		ce.CookieMonster = cookieMonster
		ce.URL = WebServiceURL

		CredentialCache.Add(ce)
		Return ce
	End Function

	Public Class SessionCredentials
		Public UserName As String
		Public CookieMonster As CookieContainer
		Public TapiCredential As NetworkCredential
		Private _Credentials As ICredentials

		Public Property Credentials As ICredentials
			Get
				Return _Credentials
			End Get
			Friend Set(value As ICredentials)
				If value Is Nothing Then
					Throw New System.Exception("Invalid property value.  Credentials cannot be null")
				End If
				_Credentials = value
			End Set
		End Property
	End Class

	Private Class CredentialEntry
		Public UserName As String
		Public PassWord As String
		Public URL As String
		Public CookieMonster As CookieContainer
		Public TapiCredential As NetworkCredential
		Private _Credentials As ICredentials

		Public Property Credentials As ICredentials
			Get
				Return _Credentials
			End Get
			Friend Set(value As ICredentials)
				If value Is Nothing Then
					Throw New System.Exception("Invalid property value.  Credentials cannot be null")
				End If
				_Credentials = value
			End Set
		End Property

		Public Function SessionCredentials() As SessionCredentials
			Dim sc As New SessionCredentials()
			sc.UserName = UserName
			sc.Credentials = Credentials
			sc.TapiCredential = TapiCredential
			sc.CookieMonster = CookieMonster
			Return sc
		End Function
	End Class
End Class
