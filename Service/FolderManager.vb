Namespace kCura.WinEDDS.Service
	Public Class FolderManager
		Inherits kCura.EDDS.WebAPI.FolderManagerBase.FolderManager

		Public Sub New(ByVal credentials As Net.NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()
			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}FolderManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

		Public Shared Function GetExportFriendlyFolderName(ByVal input As String) As String
			Return System.Text.RegularExpressions.Regex.Replace(input, "[\*\\\/\:\?\<\>\""\|\$]+", " ").Trim
		End Function

#Region " Translations "
		'Public Function DTOToWebAPIFolder(ByVal folderDTO As kCura.EDDS.DTO.Folder) As kCura.EDDS.WebAPI.FolderManagerBase.Folder
		'	Dim folder As New kCura.EDDS.WebAPI.FolderManagerBase.Folder

		'	folder.AccessControlListID = folderDTO.AccessControlListID
		'	folder.AccessControlListIsInherited = folderDTO.AccessControlListIsInherited
		'	folder.ArtifactID = folderDTO.ArtifactID
		'	folder.ArtifactTypeID = folderDTO.ArtifactTypeID
		'	folder.ContainerID = folderDTO.ContainerID
		'	folder.CreatedBy = folderDTO.CreatedBy
		'	folder.CreatedOn = folderDTO.CreatedOn
		'	folder.DeleteFlag = folderDTO.DeleteFlag
		'	folder.Keywords = folderDTO.Keywords
		'	folder.LastModifiedBy = folderDTO.LastModifiedBy
		'	folder.LastModifiedOn = folderDTO.LastModifiedOn
		'	folder.Name = folderDTO.Name
		'	folder.Notes = folderDTO.Notes
		'	folder.ParentArtifactID = folderDTO.ParentArtifactID
		'	folder.TextIdentifier = folderDTO.TextIdentifier
		'	Return folder
		'End Function
#End Region

#Region " Shadow Functions "
		Public Shadows Function RetrieveAllByCaseID(ByVal caseContextArtifactID As Int32) As System.Data.DataSet
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.RetrieveAllByCaseID(caseContextArtifactID)
					Else
						'Return _folderManager.ExternalRetrieveAllByCaseID(caseID, _identity)
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

		Public Shadows Function Read(ByVal caseContextArtifactID As Int32, ByVal folderArtifactID As Int32) As kCura.EDDS.WebAPI.FolderManagerBase.Folder
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.Read(caseContextArtifactID, folderArtifactID)
					Else
						'Return Me.DTOToWebAPIFolder(_folderManager.Read(folderArtifactID, _identity))
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

		Public Shadows Function Create(ByVal caseContextArtifactID As Int32, ByVal parentArtifactID As Int32, ByVal name As String) As Int32
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.Create(caseContextArtifactID, parentArtifactID, GetExportFriendlyFolderName(name))
					Else
						'Return _folderManager.Create(parentArtifactID, name, _identity)
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

		Public Shadows Function Exists(ByVal caseContextArtifactID As Int32, ByVal rootFolderID As Int32) As Boolean
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.Exists(caseContextArtifactID, rootFolderID)
					Else
						'Return _folderManager.Exists(artifactID, _identity, rootFolderID)
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
#End Region

	End Class
End Namespace