Namespace kCura.WinEDDS.Service
	Public Class ExportManager
		Inherits kCura.EDDS.WebAPI.ExportManagerBase.ExportManager

		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()

			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}ExportManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

		Public Shadows Function InitializeFolderExport(ByVal appID As Int32, ByVal viewArtifactID As Int32, ByVal parentArtifactID As Int32, ByVal includeSubFolders As Boolean, ByVal avfIds As Int32(), ByVal startAtRecord As Int32, ByVal artifactTypeID As Int32) As kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Return MyBase.InitializeFolderExport(appID, viewArtifactID, parentArtifactID, includeSubFolders, avfIds, startAtRecord, artifactTypeID)
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

		Public Shadows Function InitializeProductionExport(ByVal appID As Int32, ByVal productionArtifactID As Int32, ByVal avfIds As Int32(), ByVal startAtRecord As Int32) As kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Return MyBase.InitializeProductionExport(appID, productionArtifactID, avfIds, startAtRecord)
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

		Public Shadows Function InitializeSearchExport(ByVal appID As Int32, ByVal searchArtifactID As Int32, ByVal avfIds As Int32(), ByVal startAtRecord As Int32) As kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Return MyBase.InitializeSearchExport(appID, searchArtifactID, avfIds, startAtRecord)
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

		Public Shadows Function RetrieveResultsBlock(ByVal appID As Int32, ByVal runId As Guid, ByVal artifactTypeID As Int32, ByVal avfIds As Int32(), ByVal chunkSize As Int32, ByVal displayMulticodesAsNested As Boolean, ByVal multiValueDelimiter As Char, ByVal nestedValueDelimiter As Char, ByVal textPrecedenceAvfIds As Int32()) As Object()
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Dim retval As Object() = MyBase.RetrieveResultsBlock(appID, runId, artifactTypeID, avfIds, chunkSize, displayMulticodesAsNested, multiValueDelimiter, nestedValueDelimiter, textPrecedenceAvfIds)
					If Not retval Is Nothing Then
						For Each row As Object() In retval
							For i As Int32 = 0 To row.Length - 1
								If TypeOf row(i) Is Byte() Then row(i) = System.Text.Encoding.Unicode.GetString(DirectCast(row(i), Byte()))
							Next
						Next
					End If
					Return retval
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

	End Class
End Namespace

