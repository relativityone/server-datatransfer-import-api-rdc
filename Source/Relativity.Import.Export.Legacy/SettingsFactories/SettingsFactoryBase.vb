Imports System.Security.Cryptography.X509Certificates
Imports System.Net
Imports System.Net.Security

Namespace kCura.WinEDDS
	Public MustInherit Class SettingsFactoryBase

		Public Enum OverwriteType
			Append
			Overlay
			AppendOverlay
		End Enum

		Private _credential As System.Net.NetworkCredential
		Private _cookieContainer As System.Net.CookieContainer
		Private _caseManager As kCura.WinEDDS.Service.CaseManager
		Private _folderManager As kCura.WinEDDS.Service.FolderManager
		Private _fieldManager As kCura.WinEDDS.Service.FieldManager
		Private _productionManager As kCura.WinEDDS.Service.ProductionManager

		Protected ReadOnly Property Credential() As System.Net.NetworkCredential
			Get
				Return _credential
			End Get
		End Property

		Protected ReadOnly Property CookieContainer() As System.Net.CookieContainer
			Get
				Return _cookieContainer
			End Get
		End Property

		Protected ReadOnly Property CaseManager() As kCura.WinEDDS.Service.CaseManager
			Get
				If _caseManager Is Nothing Then
					_caseManager = New Service.CaseManager(_credential, _cookieContainer)
				End If
				Return _caseManager
			End Get
		End Property

		Protected ReadOnly Property FolderManager() As kCura.WinEDDS.Service.FolderManager
			Get
				If _folderManager Is Nothing Then
					_folderManager = New Service.FolderManager(_credential, _cookieContainer)
				End If
				Return _folderManager
			End Get
		End Property

		Protected ReadOnly Property FieldManager() As kCura.WinEDDS.Service.FieldManager
			Get
				If _fieldManager Is Nothing Then
					_fieldManager = New Service.FieldManager(_credential, _cookieContainer)
				End If
				Return _fieldManager
			End Get
		End Property

		Protected ReadOnly Property ProductionManager() As kCura.WinEDDS.Service.ProductionManager
			Get
				If _productionManager Is Nothing Then
					_productionManager = New Service.ProductionManager(_credential, _cookieContainer)
				End If
				Return _productionManager
			End Get
		End Property

		Protected Sub New(ByVal login As String, ByVal password As String)
			Me.New(New System.Net.NetworkCredential(login, password))
		End Sub

		Protected Sub New(ByVal credential As System.Net.NetworkCredential)
			Me.new(credential, New System.Net.CookieContainer)
		End Sub

		Protected Sub New(ByVal credential As System.Net.NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer)

			_credential = credential

			If Config.SuppressCertificateCheckOnClient
				ServicePointManager.ServerCertificateValidationCallback = Function(sender As Object, certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors) True
			End If
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 Or SecurityProtocolType.Tls11 Or SecurityProtocolType.Tls Or SecurityProtocolType.Ssl3
			_cookieContainer = cookieContainer

			Dim relativityManager As New kCura.WinEDDS.Service.RelativityManager(_credential, _cookieContainer)

			Dim successfulLogin As Boolean = False
			Try
				successfulLogin = relativityManager.ValidateSuccesfulLogin()
			Catch ex As Exception
				successfulLogin = False
			End Try
			If Not successfulLogin Then
				If Not _credential.Password = "" Then
					Dim userManager As New kCura.WinEDDS.Service.UserManager(_credential, _cookieContainer)

					If userManager.Login(_credential.UserName, _credential.Password) Then
						kCura.WinEDDS.Service.Settings.AuthenticationToken = userManager.GenerateDistributedAuthenticationToken()
						Exit Sub
					End If
				End If
			Else
				Exit Sub
			End If
			Throw New InvalidCredentialsException
		End Sub

		Protected Sub SaveObject(ByVal location As String, ByVal settings As Object)
			Dim sw As New System.IO.StreamWriter(location)
			Dim serializer As New System.Runtime.Serialization.Formatters.Soap.SoapFormatter
			Try
				serializer.Serialize(sw.BaseStream, settings)
				sw.Close()
			Catch ex As System.Exception
				Throw New System.Exception("Settings object save failed", ex)
			End Try
		End Sub

		Public MustOverride Sub Save(ByVal location As String)

		''' <summary>
		''' The exception thrown when a login or credential failure occurs when attempting to access settings.
		''' </summary>
		<Serializable>
		Public Class InvalidCredentialsException
			Inherits System.Exception

			''' <summary>
			''' Initializes a new instance of the <see cref="InvalidCredentialsException"/> class.
			''' </summary>
			Public Sub New()
				MyBase.New()
			End Sub

			''' <summary>
			''' Initializes a new instance of the <see cref="InvalidCredentialsException"/> class.
			''' </summary>
			''' <param name="message">
			''' The error message that explains the reason for the exception.
			''' </param>
			Public Sub New(message As String)
				MyBase.New(message)
			End Sub

			''' <summary>
			''' Initializes a new instance of the <see cref="InvalidCredentialsException"/> class.
			''' </summary>
			''' <param name="message">
			''' The error message that explains the reason for the exception.
			''' </param>
			''' <param name="innerException">
			''' The exception that Is the cause of the current exception, Or a null reference (Nothing in Visual Basic) if no inner exception Is specified.
			''' </param>
			Public Sub New(message As String, innerException As Exception)
				MyBase.New(message, innerException)
			End Sub

			''' <inheritdoc />
			<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
			Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
				MyBase.New(info, context)
			End Sub
		End Class

	End Class
End Namespace