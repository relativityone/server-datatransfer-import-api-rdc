Namespace kCura.WinEDDS.Service
	Public Class BulkImportManager
		Inherits kCura.EDDS.WebAPI.BulkImportManagerBase.BulkImportManager

#Region "Constructors"

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()
			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.WebServiceURL = String.Format("{0}BulkImportManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

#End Region

		Private Sub CheckResultsForException(ByVal results As EDDS.WebAPI.BulkImportManagerBase.MassImportResults)
			If results.ExceptionDetail IsNot Nothing Then
				If results.ExceptionDetail.ExceptionMessage IsNot Nothing AndAlso results.ExceptionDetail.ExceptionMessage.Contains("Timeout expired") Then
					Throw New BulkImportSqlTimeoutException(results.ExceptionDetail)
				Else
					Throw New BulkImportSqlException(results.ExceptionDetail)
				End If
			End If
		End Sub

		Protected Overridable Property WebServiceURL As String
			Get
				Return Me.Url
			End Get
			Set(ByVal value As String)
				Me.Url = value
			End Set
		End Property

#Region " Shadow Methods "
		Public Shadows Function BulkImportImage(ByVal appID As Int32, ByVal bulkFileName As String, ByVal uploadFullText As Boolean, ByVal overwrite As kCura.EDDS.WebAPI.BulkImportManagerBase.OverwriteType, ByVal destinationFolderArtifactID As Int32, ByVal repository As String, ByVal useBulk As Boolean, ByVal runID As String, ByVal keyFieldID As Int32, ByVal inRepository As Boolean) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Dim retval As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults = Me.InvokeBulkImportImage(appID, bulkFileName, uploadFullText, overwrite, destinationFolderArtifactID, repository, useBulk, runID, keyFieldID, inRepository)
					Me.CheckResultsForException(retval)
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
					Dim retval As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults = Me.InvokeBulkImportProductionImage(appID, bulkFileName, uploadFullText, overwrite, destinationFolderArtifactID, repository, productionArtifactID, useBulk, runID, productionKeyFieldArtifactID, inRepository)
					Me.CheckResultsForException(retval)
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
					Dim retval As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults = Me.InvokeBulkImportNative(appID, settings, inRepository, includeExtractedTextEncoding)
					Me.CheckResultsForException(retval)
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
					Dim retval As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults = Me.InvokeBulkImportObjects(appID, settings, inRepository)
					Me.CheckResultsForException(retval)
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

#Region " Reduced Batch Size Shadow Methods "

		Public Shadows Function BulkImportImage(ByVal appID As Int32, ByVal bulkFileName As String, ByVal uploadFullText As Boolean, ByVal overwrite As kCura.EDDS.WebAPI.BulkImportManagerBase.OverwriteType, ByVal destinationFolderArtifactID As Int32, ByVal repository As String, ByVal useBulk As Boolean, ByVal runID As String, ByVal keyFieldID As Int32, ByVal inRepository As Boolean, ByVal startRecord As Int32, ByVal endRecord As Int32) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Dim retval As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults = Me.InvokeBulkImportImageWithRange(appID, bulkFileName, uploadFullText, overwrite, destinationFolderArtifactID, repository, useBulk, runID, keyFieldID, inRepository, startRecord, endRecord)
					Me.CheckResultsForException(retval)
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

		Public Shadows Function BulkImportProductionImage(ByVal appID As Int32, ByVal bulkFileName As String, ByVal uploadFullText As Boolean, ByVal overwrite As kCura.EDDS.WebAPI.BulkImportManagerBase.OverwriteType, ByVal destinationFolderArtifactID As Int32, ByVal repository As String, ByVal productionArtifactID As Int32, ByVal useBulk As Boolean, ByVal runID As String, ByVal productionKeyFieldArtifactID As Int32, ByVal inRepository As Boolean, ByVal startRecord As Int32, ByVal endRecord As Int32) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Dim retval As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults = Me.InvokeBulkImportProductionImageWithRange(appID, bulkFileName, uploadFullText, overwrite, destinationFolderArtifactID, repository, productionArtifactID, useBulk, runID, productionKeyFieldArtifactID, inRepository, startRecord, endRecord)
					Me.CheckResultsForException(retval)
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

#End Region

#Region " Webservice Wrapper Methods "

		Protected Overridable Function InvokeBulkImportImage(ByVal appID As Integer, ByVal bulkFileName As String, ByVal uploadFullText As Boolean, ByVal overwrite As kCura.EDDS.WebAPI.BulkImportManagerBase.OverwriteType, ByVal destinationFolderArtifactID As Integer, ByVal repository As String, ByVal useBulk As Boolean, ByVal runID As String, ByVal keyFieldID As Integer, ByVal inRepository As Boolean) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Return MyBase.BulkImportImage(appID, bulkFileName, uploadFullText, overwrite, destinationFolderArtifactID, repository, useBulk, runID, keyFieldID, inRepository)
		End Function

		Protected Overridable Function InvokeBulkImportImageWithRange(ByVal appID As Integer, ByVal bulkFileName As String, ByVal uploadFullText As Boolean, ByVal overwrite As kCura.EDDS.WebAPI.BulkImportManagerBase.OverwriteType, ByVal destinationFolderArtifactID As Integer, ByVal repository As String, ByVal useBulk As Boolean, ByVal runID As String, ByVal keyFieldID As Integer, ByVal inRepository As Boolean, ByVal startRecord As Int32, ByVal recordCount As Int32) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Return MyBase.BulkImportImageWithRange(appID, bulkFileName, uploadFullText, overwrite, destinationFolderArtifactID, repository, useBulk, runID, keyFieldID, inRepository, startRecord, recordCount)
		End Function

		Protected Overridable Function InvokeBulkImportProductionImage(ByVal appID As Integer, ByVal bulkFileName As String, ByVal uploadFullText As Boolean, ByVal overwrite As kCura.EDDS.WebAPI.BulkImportManagerBase.OverwriteType, ByVal destinationFolderArtifactID As Integer, ByVal repository As String, ByVal productionArtifactID As Integer, ByVal useBulk As Boolean, ByVal runID As String, ByVal productionKeyFieldArtifactID As Integer, ByVal inRepository As Boolean) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Return MyBase.BulkImportProductionImage(appID, bulkFileName, uploadFullText, overwrite, destinationFolderArtifactID, repository, productionArtifactID, useBulk, runID, productionKeyFieldArtifactID, inRepository)
		End Function

		Protected Overridable Function InvokeBulkImportProductionImageWithRange(ByVal appID As Integer, ByVal bulkFileName As String, ByVal uploadFullText As Boolean, ByVal overwrite As kCura.EDDS.WebAPI.BulkImportManagerBase.OverwriteType, ByVal destinationFolderArtifactID As Integer, ByVal repository As String, ByVal productionArtifactID As Integer, ByVal useBulk As Boolean, ByVal runID As String, ByVal productionKeyFieldArtifactID As Integer, ByVal inRepository As Boolean, ByVal startRecord As Int32, ByVal recordCount As Int32) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Return MyBase.BulkImportProductionImageWithRange(appID, bulkFileName, uploadFullText, overwrite, destinationFolderArtifactID, repository, productionArtifactID, useBulk, runID, productionKeyFieldArtifactID, inRepository, startRecord, recordCount)
		End Function

		Protected Overridable Function InvokeBulkImportNative(ByVal appID As Integer, ByVal settings As EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo, ByVal inRepository As Boolean, ByVal includeExtractedTextEncoding As Boolean) As EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Return MyBase.BulkImportNative(appID, settings, inRepository, includeExtractedTextEncoding)
		End Function

		Protected Overridable Function InvokeBulkImportObjects(ByVal appID As Integer, ByVal settings As EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo, ByVal inRepository As Boolean) As EDDS.WebAPI.BulkImportManagerBase.MassImportResults
			Return MyBase.BulkImportObjects(appID, settings, inRepository)
		End Function

#End Region

		Public Class BulkImportSqlException
			Inherits System.Exception

			Private Property DetailedException As EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail
			Public Sub New(ByVal exception As EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail)
				MyBase.New(exception.ExceptionMessage)
				Me.DetailedException = exception
			End Sub

			Public Overrides Function ToString() As String
				Return DetailedException.ExceptionFullText
			End Function
		End Class

		Public Class BulkImportSqlTimeoutException
			Inherits BulkImportSqlException
			Public Sub New(ByVal exception As EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail)
				MyBase.New(exception)
			End Sub
		End Class

	End Class
End Namespace
