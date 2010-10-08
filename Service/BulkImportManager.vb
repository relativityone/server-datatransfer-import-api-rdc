Namespace kCura.WinEDDS.Service
	Public Class BulkImportManager
		Inherits kCura.EDDS.WebAPI.BulkImportManagerBase.BulkImportManager

#Region "Constructors"

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()
			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}BulkImportManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

#End Region

#Region " Shadow Methods "
		Public Shadows Function BulkImportImage(ByVal appID As Int32, ByVal bulkFileName As String, ByVal uploadFullText As Boolean, ByVal overwrite As kCura.EDDS.WebAPI.BulkImportManagerBase.OverwriteType, ByVal destinationFolderArtifactID As Int32, ByVal repository As String, ByVal useBulk As Boolean, ByVal runID As String, ByVal keyFieldID As Int32, ByVal inRepository As Boolean) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Dim retval As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults = MyBase.BulkImportImage(appID, bulkFileName, uploadFullText, overwrite, destinationFolderArtifactID, repository, useBulk, runID, keyFieldID, inRepository)
					If retval.ErrorText <> "" Then Throw New BulkImportSqlException(retval.ErrorText)
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

		Public Shadows Function BulkImportProductionImage(ByVal appID As Int32, ByVal bulkFileName As String, ByVal uploadFullText As Boolean, ByVal overwrite As kCura.EDDS.WebAPI.BulkImportManagerBase.OverwriteType, ByVal destinationFolderArtifactID As Int32, ByVal repository As String, ByVal productionArtifactID As Int32, ByVal useBulk As Boolean, ByVal runID As String, ByVal productionKeyFieldArtifactID As Int32, ByVal inRepository As Boolean) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Dim retval As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults = MyBase.BulkImportProductionImage(appID, bulkFileName, uploadFullText, overwrite, destinationFolderArtifactID, repository, productionArtifactID, useBulk, runID, productionKeyFieldArtifactID, inRepository)
					If retval.ErrorText <> "" Then Throw New BulkImportSqlException(retval.ErrorText)
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

		Public Shadows Function GenerateImageErrorFiles(ByVal appID As Int32, ByVal importKey As String, ByVal writeHeader As Boolean, ByVal keyFieldId As Int32) As Relativity.MassImport.ErrorFileKey
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Dim retval As New Relativity.MassImport.ErrorFileKey
					With MyBase.GenerateImageErrorFiles(appID, importKey, writeHeader, keyFieldId)
						retval.LogKey = .LogKey
						retval.OpticonKey = .OpticonKey
					End With
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

		Public Shadows Function ImageRunHasErrors(ByVal appID As Int32, ByVal runId As String) As Boolean
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Return MyBase.ImageRunHasErrors(appID, runId)
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
		End Function

		Public Shadows Function BulkImportNative(ByVal appID As Int32, ByVal settings As kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo, ByVal inRepository As Boolean, ByVal includeExtractedTextEncoding As Boolean) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Dim retval As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults = MyBase.BulkImportNative(appID, settings, inRepository, includeExtractedTextEncoding)
					If retval.ErrorText <> "" Then Throw New BulkImportSqlException(retval.ErrorText)
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

		Public Shadows Function BulkImportObjects(ByVal appID As Int32, ByVal settings As kCura.EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo, ByVal inRepository As Boolean) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Dim retval As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults = MyBase.BulkImportObjects(appID, settings, inRepository)
					If retval.ErrorText <> "" Then Throw New BulkImportSqlException(retval.ErrorText)
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

		Public Shadows Function DisposeTempTables(ByVal appID As Int32, ByVal runId As String) As Object
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Return MyBase.DisposeTempTables(appID, runId)
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

		Public Class BulkImportSqlException
			Inherits System.Exception
			Public Sub New(ByVal errorText As String)
				MyBase.New(errorText)
			End Sub
		End Class
	End Class
End Namespace
