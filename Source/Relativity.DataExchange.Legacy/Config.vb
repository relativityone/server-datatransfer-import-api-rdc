Imports System.Collections.Generic
Imports Relativity.DataExchange
Imports Relativity.DataExchange.Transfer

Namespace kCura.WinEDDS
	Public Class Config

		Private Const mainFormWindowHeightKey As String = "MainFormWindowHeight"
		Private Const mainFormWindowWidthKey As String = "MainFormWindowWidth"
		Private Shared ReadOnly _loadLock As New System.Object
		Private Shared _appSettingsDictionary As AppSettingsDictionary

		Public Shared ReadOnly Property ConfigSettings() As IDictionary
			Get
				If _appSettingsDictionary Is Nothing Then
					SyncLock _LoadLock
						If _appSettingsDictionary Is Nothing Then
							' This is a special dictionary that routes all changes to the settings singleton.
							_appSettingsDictionary = New AppSettingsDictionary(AppSettings.Instance)
						End If
					End SyncLock
				End If
				Return _appSettingsDictionary
			End Get
		End Property

		Public Shared ReadOnly Property MaxReloginTries() As Int32
			Get
				Return AppSettings.Instance.MaxReloginTries
			End Get
		End Property

		Public Shared ReadOnly Property WaitBeforeReconnect() As Int32      'Milliseconds
			Get
				Return AppSettings.Instance.WaitBeforeReconnect
			End Get
		End Property

		Public Shared ReadOnly Property FileTransferModeExplanationText(ByVal includeBulk As Boolean) As String
			Get
				Return TapiModeHelper.BuildDocText()
			End Get
		End Property

		Public Shared Function GetRegistryKeyValue(ByVal keyName As String) As String
			Return Global.Relativity.DataExchange.AppSettingsManager.GetRegistryKeyValue(keyName)
		End Function

		Private Shared Sub SetRegistryKeyValue(ByVal keyName As String, ByVal keyVal As String)
			Global.Relativity.DataExchange.AppSettingsManager.SetRegistryKeyValue(keyName, keyVal)
		End Sub

		Public Shared Function ValidateURIFormat(ByVal returnValue As String) As String
			Return AppSettings.Instance.ValidateUriFormat(returnValue)
		End Function

        'This sets the application name and is used to drive APM metrics and other reporting features. If not specified, the process name is used.
        Public Shared ReadOnly Property ApplicationName() As String
	        Get
		        Return AppSettings.Instance.ApplicationName
	        End Get
        End Property

        Friend Shared ReadOnly Property UsePipeliningForNativeAndObjectImports As Boolean
            Get
	            Return AppSettings.Instance.UsePipeliningForNativeAndObjectImports
            End Get
        End Property

        ''' <summary>
        ''' Please do not use or document - this is an internal toggle
        ''' </summary>
        ''' <returns></returns>
        Friend Shared ReadOnly Property LoadImportedFullTextFromServer As Boolean
            Get
	            Return AppSettings.Instance.LoadImportedFullTextFromServer
            End Get
        End Property
        Friend Shared ReadOnly Property DisableTextFileEncodingCheck As Boolean
            Get
	            Return AppSettings.Instance.DisableTextFileEncodingCheck
            End Get
        End Property

        Public Shared ReadOnly Property ProcessFormRefreshRate As Long
            Get
	            Return AppSettings.Instance.ProcessFormRefreshRate
            End Get
        End Property

		Public Shared ReadOnly Property ImportBatchMaxVolume() As Int32     'Volume in bytes
			Get
				Return AppSettings.Instance.ImportBatchMaxVolume
			End Get
		End Property

		Public Shared ReadOnly Property ImportBatchSize() As Int32      'Number of records
			Get
				Return AppSettings.Instance.ImportBatchSize
			End Get
		End Property

		Public Shared ReadOnly Property UseSynchronizedImportBatchMode() As Boolean      'Boolean
			Get
				Return AppSettings.Instance.UseSynchronizedImportBatchMode
			End Get
		End Property

		Public Shared ReadOnly Property JobCompleteBatchSize() As Int32     'Number of records
			Get
				Return AppSettings.Instance.JobCompleteBatchSize
			End Get
		End Property

		Public Shared ReadOnly Property HttpTimeoutSeconds() As Int32
			Get
				Return AppSettings.Instance.HttpTimeoutSeconds
			End Get
		End Property

		Public Shared ReadOnly Property WebAPIOperationTimeout() As Int32
			Get
				Return AppSettings.Instance.WebApiOperationTimeout
			End Get
		End Property

		Public Shared ReadOnly Property PermissionErrorsRetry() As Boolean
			Get
				Return AppSettings.Instance.PermissionErrorsRetry
			End Get
		End Property

		Public Shared ReadOnly Property BadPathErrorsRetry() As Boolean
			Get
				Return AppSettings.Instance.TapiBadPathErrorsRetry
			End Get
		End Property

		''' <summary>
		''' Overrides the default temp directory ordinarily provided by <see cref="System.IO.Path.GetTempPath"/>. This is 
		''' </summary>
		''' <value>
		''' The full path to the temp directory.
		''' </value>
		Public Shared ReadOnly Property TempDirectory() As String
			Get
				Return AppSettings.Instance.TempDirectory
			End Get
		End Property

		''' <summary>
		''' If True, Folders which are created in Append mode are created in the WebAPI.
		''' If False, Folders which are created in Append mode are created in RDC/ImportAPI.
		''' If the value is not set in the config file, True is returned.
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks>This property is part of the fix for Dominus# 1127879</remarks>
		Public Shared ReadOnly Property CreateFoldersInWebAPI() As Boolean
			Get
				Return AppSettings.Instance.CreateFoldersInWebApi
			End Get
		End Property

		Public Shared ReadOnly Property DynamicBatchResizingOn() As Boolean     'Allow or not to automatically decrease import batch size while import is in progress
			Get
				Return AppSettings.Instance.DynamicBatchResizingOn
			End Get
		End Property

		Public Shared ReadOnly Property DefaultMaximumErrorCount() As Int32
			Get
				Return AppSettings.Instance.DefaultMaxErrorCount
			End Get
		End Property

		Public Shared ReadOnly Property MinimumBatchSize() As Int32     'When AutoBatch is on. This is the lower ceiling up to which batch will decrease
			Get
				Return AppSettings.Instance.MinBatchSize
			End Get
		End Property

		Public Shared ReadOnly Property ExportBatchSize() As Int32      'Number of records
			Get
				Return AppSettings.Instance.ExportBatchSize
			End Get
		End Property

		Public Shared ReadOnly Property DisableImageTypeValidation() As Boolean
			Get
				Return AppSettings.Instance.DisableImageTypeValidation
			End Get
		End Property

		Public Shared ReadOnly Property DisableImageLocationValidation() As Boolean
			Get
				Return AppSettings.Instance.DisableImageLocationValidation
			End Get
		End Property

		Public Shared ReadOnly Property DisableNativeValidation() As Boolean
			Get
				Return AppSettings.Instance.DisableOutsideInFileIdentification
			End Get
		End Property

		Public Shared ReadOnly Property DisableNativeLocationValidation() As Boolean
			Get
				Return AppSettings.Instance.DisableThrowOnIllegalCharacters
			End Get
		End Property

		Public Shared ReadOnly Property CreateErrorForEmptyNativeFile() As Boolean
			Get
				Return AppSettings.Instance.CreateErrorForEmptyNativeFile
			End Get
		End Property

		Public Shared ReadOnly Property AuditLevel() As kCura.EDDS.WebAPI.BulkImportManagerBase.ImportAuditLevel
			Get
				Return DirectCast([Enum].Parse(GetType(kCura.EDDS.WebAPI.BulkImportManagerBase.ImportAuditLevel), AppSettings.Instance.AuditLevel), kCura.EDDS.WebAPI.BulkImportManagerBase.ImportAuditLevel)
			End Get
		End Property

		'This hidden configuration setting makes it easier to test WebUpload.  Testing is important.
		Public Shared ReadOnly Property ForceWebUpload() As Boolean
			Get
				Return AppSettings.Instance.ForceWebUpload
			End Get
		End Property

		'This is used to force the TAPI file share client.
		Public Shared ReadOnly Property TapiForceFileShareClient() As Boolean
			Get
				Return AppSettings.Instance.TapiForceFileShareClient
			End Get
		End Property

		'This is used to force the TAPI HTTP client.
		Public Shared ReadOnly Property TapiForceHttpClient() As Boolean
			Get
				Return AppSettings.Instance.TapiForceHttpClient
			End Get
		End Property

		'This is used to force the TAPI HTTP client only for BCP. This is a temp workaround until Aspera supports multiple file shares.
		Public Shared ReadOnly Property TapiForceBcpHttpClient() As Boolean
			Get
				Return AppSettings.Instance.TapiForceBcpHttpClient
			End Get
		End Property

		'This is used to specify the root folder where the Aspera BCP files are uploaded.
		Public Shared ReadOnly Property TapiAsperaBcpRootFolder() As String
			Get
				Return AppSettings.Instance.TapiAsperaBcpRootFolder
			End Get
		End Property

		'This is used to force the TAPI Aspera client.
		Public Shared ReadOnly Property TapiForceAsperaClient() As Boolean
			Get
				Return AppSettings.Instance.TapiForceAsperaClient
			End Get
		End Property

		'This is used to force a semi-colon delimited list of TAPI clients.
		Public Shared ReadOnly Property TapiForceClientCandidates() As String
			Get
				Return AppSettings.Instance.TapiForceClientCandidates
			End Get
		End Property

		' This sets a TAPI minimum data rate in Mbps units.
		Public Shared ReadOnly Property TapiMinDataRateMbps() As Int32
			Get
				Return AppSettings.Instance.TapiMinDataRateMbps
			End Get
		End Property

		' This sets a TAPI setting to submit APM metrics upon completion of the transfer job.
		Public Shared ReadOnly Property TapiSubmitApmMetrics() As Boolean
			Get
				Return AppSettings.Instance.TapiSubmitApmMetrics
			End Get
		End Property

		' This sets a TAPI target data rate in Mbps units.
		Public Shared ReadOnly Property TapiTargetDataRateMbps() As Int32
			Get
				Return AppSettings.Instance.TapiTargetDataRateMbps
			End Get
		End Property

		' This enables or disables preserving file timestamps.
		Public Shared ReadOnly Property TapiPreserveFileTimestamps() As Boolean
			Get
				Return AppSettings.Instance.TapiPreserveFileTimestamps
			End Get
		End Property

		' This sets a directory where TAPI client-specific transfer logs are stored.
		Public Shared ReadOnly Property TapiTransferLogDirectory() As String
			Get
				Return AppSettings.Instance.TapiTransferLogDirectory
			End Get
		End Property

		' This enables the feature that gives "trip 1 of x" progress for large files.
		Public Shared ReadOnly Property TapiLargeFileProgressEnabled() As Boolean
			Get
				Return AppSettings.Instance.TapiLargeFileProgressEnabled
			End Get
		End Property

		' This sets the max number of concurrent threads used by the TAPI job engine.
		Public Shared ReadOnly Property TapiMaxJobParallelism() As Int32
			Get
				Return AppSettings.Instance.TapiMaxJobParallelism
			End Get
		End Property

		' This sets the number of levels the Aspera doc root folder is relative to the file share where native files are stored.
		Public Shared ReadOnly Property TapiAsperaNativeDocRootLevels() As Int32
			Get
				Return AppSettings.Instance.TapiAsperaNativeDocRootLevels
			End Get
		End Property

		'This is used to disable certificates check of destination server on client. It should be enabled only when dealing with invalid certificates on test environments.
		Public Shared ReadOnly Property SuppressCertificateCheckOnClient() As Boolean
			Get
				Return AppSettings.Instance.SuppressServerCertificateValidation
			End Get
		End Property

		Public Shared ReadOnly Property LogConfigFile() As String
			Get
				Return AppSettings.Instance.LogConfigXmlFileName
			End Get
		End Property

		Public Shared Property ForceFolderPreview() As Boolean
			Get
				Return AppSettings.Instance.ForceFolderPreview
			End Get
			Set(ByVal value As Boolean)
				AppSettings.Instance.ForceFolderPreview = value
			End Set
		End Property

		Public Shared Property ObjectFieldIdListContainsArtifactId() As IList(Of Int32)
			Get
				Return AppSettings.Instance.ObjectFieldIdListContainsArtifactId
			End Get
			Set(ByVal value As IList(Of Int32))
				AppSettings.Instance.ObjectFieldIdListContainsArtifactId = value
			End Set
		End Property

        Public Shared Property WebServiceURL() As String
            Get
				Return AppSettings.Instance.WebApiServiceUrl
            End Get
            Set(ByVal value As String)
	            AppSettings.Instance.WebApiServiceUrl = value
            End Set
        End Property

        Public Shared Property MainFormWindowWidth As Integer
            Get
                Dim savedWidth As String = GetRegistryKeyValue(mainFormWindowWidthKey)
                If savedWidth <> Nothing Then
                    Return Convert.ToInt32(savedWidth)
                Else
                    Return Nothing
                End If
            End Get
            Set
                SetRegistryKeyValue(mainFormWindowWidthKey, Value.ToString())
            End Set
        End Property

        Public Shared Property MainFormWindowHeight As Integer
            Get
                Dim savedHeight As String = GetRegistryKeyValue(mainFormWindowHeightKey)
                If savedHeight <> Nothing Then
                    Return Convert.ToInt32(savedHeight)
                Else
                    Return Nothing
                End If
            End Get
            Set
                SetRegistryKeyValue(mainFormWindowHeightKey, Value.ToString())
            End Set
        End Property

        Public Shared Property ProgrammaticServiceURL() As String
            Get
	            Return AppSettings.Instance.ProgrammaticWebApiServiceUrl
            End Get
            Set(value As String)
                AppSettings.Instance.ProgrammaticWebApiServiceUrl = value
            End Set
        End Property

        Public Shared ReadOnly Property EnableSingleModeImport() As Boolean
            Get
				' 5/11/2019: Per Nick, this implementation was removed years ago and preserving the property for backwards compatibility only.
				Return False
            End Get
        End Property

        Public Shared ReadOnly Property WebBasedFileDownloadChunkSize() As Int32
            Get
				Return AppSettings.Instance.WebBasedFileDownloadChunkSize
            End Get
        End Property

		Public Shared ReadOnly Property EnableCaseSensitiveSearchOnImport As Boolean
			Get
				Return AppSettings.Instance.EnableCaseSensitiveSearchOnImport
			End Get
		End Property

		''' <summary>
		''' Gets the configurable retry options.
		''' </summary>
		''' <value>
		''' The <see cref="RetryOptions"/> value.
		''' </value>
		''' <remarks>
		''' There are several other retry candidate behaviors and change should be limited to this property.
		''' </remarks>
		Public Shared ReadOnly Property RetryOptions As Global.Relativity.DataExchange.Io.RetryOptions
			Get
				Return AppSettings.Instance.RetryOptions
			End Get
		End Property

		Public Shared ReadOnly Property UseSearchablePDF As Boolean
			Get
				Return AppSettings.Instance.UseSearchablePDF
			End Get
		End Property
	End Class
End Namespace