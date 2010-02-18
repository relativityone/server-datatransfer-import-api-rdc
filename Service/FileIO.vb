Namespace kCura.WinEDDS.Service
	Public Class FileIO
		Inherits kCura.EDDS.WebAPI.FileIOBase.FileIO

		'Dim _externalIOManager As New kCura.EDDS.Service.ExternalIO

		Public Sub New(ByVal credentials As Net.NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()
			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}FileIO.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

#Region " Shadow Functions "
		Public Shadows Function BeginFill(ByVal caseContextArtifactID As Int32, ByVal b() As Byte, ByVal documentDirectory As String, ByVal fileGuid As String) As kCura.EDDS.WebAPI.FileIOBase.IoResponse
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Return MyBase.BeginFill(caseContextArtifactID, b, documentDirectory, fileGuid)
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

		Public Shadows Function FileFill(ByVal caseContextArtifactID As Int32, ByVal documentDirectory As String, ByVal fileName As String, ByVal b() As Byte, ByVal contextArtifactID As Int32) As kCura.EDDS.WebAPI.FileIOBase.IoResponse
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Return MyBase.FileFill(caseContextArtifactID, documentDirectory, fileName, b)
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

		Public Shadows Sub RemoveFill(ByVal caseContextArtifactID As Int32, ByVal documentDirectory As String, ByVal fileName As String)
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					MyBase.RemoveFill(caseContextArtifactID, documentDirectory, fileName)
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
		End Sub

		Public Shadows Function ReadFileAsString(ByVal path As String) As Byte()
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Return MyBase.ReadFileAsString(path)
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

		Public Shadows Function GetBcpSharePath(ByVal appID As Int32) As String
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then Return MyBase.GetBcpSharePath(appID)
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

		Public Shadows Function ValidateBcpShare(ByVal appID As Int32) As Boolean
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then Return MyBase.ValidateBcpShare(appID)
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
		End Function

		Public Shadows Function GetBcpShareSpaceReport(ByVal appID As Int32) As String()()
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then Return MyBase.GetBcpShareSpaceReport(appID)
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

		Public Shadows Function GetDefaultRepositorySpaceReport(ByVal appID As Int32) As String()()
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then Return MyBase.GetDefaultRepositorySpaceReport(appID)
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

		Public Shadows Function RepositoryVolumeMax() As Int32
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Return MyBase.RepositoryVolumeMax
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
		End Function
#End Region

	End Class
End Namespace