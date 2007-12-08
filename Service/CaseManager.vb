Namespace kCura.WinEDDS.Service
	Public Class CaseManager
		Inherits kCura.EDDS.WebAPI.CaseManagerBase.CaseManager

		Public Sub New(ByVal credentials As Net.NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()
			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}CaseManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

		Public Shared Function ConvertToCaseInfo(ByVal toConvert As kCura.EDDS.WebAPI.CaseManagerBase.CaseInfo) As kCura.EDDS.Types.CaseInfo
			Dim c As New kCura.EDDS.Types.CaseInfo
			With toConvert
				c.ArtifactID = .ArtifactID
				c.MatterArtifactID = .MatterArtifactID
				c.Name = .Name
				c.RootArtifactID = .RootArtifactID
				c.RootFolderID = .RootFolderID
				c.StatusCodeArtifactID = .StatusCodeArtifactID
				c.DocumentPath = .DocumentPath
				c.DownloadHandlerURL = .DownloadHandlerURL
			End With
			Return c
		End Function

#Region " Shadow Functions "
		Public Shadows Function RetrieveAll() As System.Data.DataSet
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.RetrieveAll()
					Else
						'Return _caseManager.RetrieveAll(_identity).ToDataSet()
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException Then
						If ex.ToString.IndexOf("NeedToReLoginException") <> -1 Then
							If tries < Config.MaxReloginTries Then
								tries += 1
								Try
									Dim creds As System.Net.NetworkCredential = DirectCast(Me.Credentials, System.Net.NetworkCredential)
									Dim userManager As New userManager(creds, Me.CookieContainer)
									userManager.Login(creds.UserName, creds.Password)
									kCura.WinEDDS.Service.Settings.AuthenticationToken = userManager.GetLatestAuthenticationToken()
								Catch
								End Try
							End If
						End If
						If tries >= Config.MaxReloginTries Then
							Throw
						End If
					Else
						Throw
					End If
				End Try
			End While
		End Function

		Public Shadows Function Read(ByVal caseArtifactID As Int32) As kCura.EDDS.Types.CaseInfo
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				Try
					Return ConvertToCaseInfo(MyBase.Read(caseArtifactID))
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException Then
						If ex.ToString.IndexOf("NeedToReLoginException") <> -1 Then
							If tries < Config.MaxReloginTries Then
								tries += 1
								Try
									Dim creds As System.Net.NetworkCredential = DirectCast(Me.Credentials, System.Net.NetworkCredential)
									Dim userManager As New userManager(creds, Me.CookieContainer)
									userManager.Login(creds.UserName, creds.Password)
									kCura.WinEDDS.Service.Settings.AuthenticationToken = userManager.GetLatestAuthenticationToken()
								Catch
								End Try
							End If
						End If
						If tries >= Config.MaxReloginTries Then
							Throw
						End If
					Else
						Throw
					End If
				End Try
			End While
		End Function
#End Region

	End Class
End Namespace