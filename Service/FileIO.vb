Imports System.Collections.Generic

Namespace kCura.WinEDDS.Service
	Public Class FileIO
		Inherits kCura.EDDS.WebAPI.FileIOBase.FileIO

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer)
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

		Private _bcpShareDictionary As New Dictionary(Of Int32, String)
		Private Shared _syncedObject As New Object()
		Public Shadows Function GetBcpSharePath(ByVal appID As Int32) As String
			If (_bcpShareDictionary.ContainsKey(appID)) Then
				Return _bcpShareDictionary(appID)
			End If
			SyncLock (_syncedObject)
				Dim tries As Int32 = 0
				While tries < Config.MaxReloginTries
					tries += 1
					Try
						Dim retval As String = MyBase.GetBcpSharePath(appID)
						If String.IsNullOrEmpty(retval) Then
							Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
						Else
							_bcpShareDictionary.Add(appID, retval)
							Return retval
						End If
					Catch ex As System.Exception
						If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
							Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
						Else
							If TypeOf ex Is System.Web.Services.Protocols.SoapException Then
								Throw ParseExceptionForMoreInfo(ex)
							Else
								Throw ex
							End If
						End If
					End Try
				End While
			End SyncLock


			Throw New System.Exception("Unable to retrieve BCP share path from the server")
			Return Nothing
		End Function

		Public Shared Function ParseExceptionForMoreInfo(ByVal ex As Exception) As System.Exception
			Dim resultException As System.Exception = ex
			Dim detailedException As Relativity.SoapExceptionDetail = Nothing
			If TypeOf ex Is System.Web.Services.Protocols.SoapException Then
				Dim soapException As System.Web.Services.Protocols.SoapException = DirectCast(ex, System.Web.Services.Protocols.SoapException)
				Dim xs As New Xml.Serialization.XmlSerializer(GetType(Relativity.SoapExceptionDetail))
				Dim doc As New System.Xml.XmlDocument
				doc.LoadXml(soapException.Detail.OuterXml)
				Dim xr As Xml.XmlReader = doc.CreateNavigator.ReadSubtree
				detailedException = TryCast(xs.Deserialize(xr), Relativity.SoapExceptionDetail)
			End If

			If detailedException IsNot Nothing Then
				resultException = New CustomException(detailedException.ExceptionMessage, ex)
			End If

			Return resultException

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
			Return Nothing
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
			Return Nothing
		End Function
#End Region

		Public Class CustomException
			Inherits System.Exception

			Public Sub New(ByVal message As String, ByVal innerException As System.Exception)
				MyBase.New(message, innerException)
			End Sub

			Public Overrides Function ToString() As String
				Return MyBase.ToString + vbNewLine + MyBase.InnerException.ToString
			End Function
		End Class


	End Class
End Namespace