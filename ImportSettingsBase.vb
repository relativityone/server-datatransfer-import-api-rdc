Imports System.Collections.Generic

Namespace kCura.Relativity.DataReaderClient
	Public Class ImportSettingsBase

		Friend Sub New()
			ExtractedTextFieldContainsFilePath = False

		End Sub

		''' <summary>
		''' Sets the level of auditing for the import job
		''' </summary>
		''' <value>FullAudit: default auditing
		''' NoSnapshot: no audit details for updates
		''' NoAudit: auditing is disabled</value>
		Public Property AuditLevel As kCura.EDDS.WebAPI.BulkImportManagerBase.ImportAuditLevel = WinEDDS.Config.AuditLevel

		''' <summary>
		''' ArtifactId of the destination Relativity workspace ('workspace' = 'case')
		''' </summary>
		Public Property CaseArtifactId() As Int32

		Public Property CopyFilesToDocumentRepository() As Boolean
			Get
				Select Case NativeFileCopyMode
					Case NativeFileCopyModeEnum.CopyFiles
						Return True
					Case NativeFileCopyModeEnum.SetFileLinks, Nothing
						Return False
					Case Else
						'TODO: Throw exception like in setup code?
						Return False
				End Select
			End Get
			Set(ByVal value As Boolean)
				If value = True Then
					NativeFileCopyMode = NativeFileCopyModeEnum.CopyFiles
				Else
					NativeFileCopyMode = NativeFileCopyModeEnum.SetFileLinks
				End If
			End Set
		End Property

		''' <summary>
		''' '
		''' </summary>
		Public Property DestinationFolderArtifactID() As Int32

		''' <summary>
		''' If True, force the value of <see cref="ExtractedTextEncoding"/> to always be used. In other words, skip
		''' reading the BOM and just use the value of <see cref="ExtractedTextEncoding"/>.
		''' </summary>
		Public Property DisableExtractedTextEncodingCheck() As Boolean?

		''' <summary>
		''' Enables or disables user permission checks per image
		''' </summary>
		''' <value>True: security checks are disabled
		''' False: security checks are enabled</value>
		Public Property DisableUserSecurityCheck As Boolean

		''' <summary>
		''' Sets the encoding of the extracted text files
		''' </summary>
		Public Property ExtractedTextEncoding() As Text.Encoding

		''' <summary>
		''' Sets whether the "Extracted Text" field contains paths to extracted text files or contains the actual extracted text
		''' </summary>
		Public Property ExtractedTextFieldContainsFilePath() As Boolean

		''' <summary>
		''' The key field that can only be set on Overwrite only
		''' </summary>
		''' <remarks>Also known as OverlayIdentifierFieldArtifactID; see ImportLoadFileProcess.AuditRun(Boolean,String)</remarks>
		Public Property IdentityFieldId() As Int32

		''' <summary>
		''' The maximum number of errors allowed before receiving a 'Maximum number of errors for display reached' message.
		''' </summary>
		''' <remarks>The default value is 999.</remarks>
		Public Property MaximumErrorCount As Int32?

		''' <summary>
		''' Sets whether native files are copied to the destination Relativity instance or whether they are used as links
		''' </summary>
		Public Property NativeFileCopyMode() As NativeFileCopyModeEnum

		''' <summary>
		''' Determines if records should be appended or overlayed
		''' </summary>
		Public Property OverwriteMode() As OverwriteModeEnum

		''' <summary>
		''' Field name which contains the unique identifier of a records parent object record
		''' </summary>
		Public Property ParentObjectIdSourceFieldName() As String

		''' <summary>
		''' Password to log into the destination Relativity instance
		''' </summary>
		Public Property RelativityPassword() As String

		''' <summary>
		''' Username to log into the destination Relativity instance
		''' </summary>
		Public Property RelativityUsername() As String

		''' <summary>
		''' '
		''' </summary>
		Public Property SelectedIdentifierFieldName() As String

		''' <summary>
		''' '
		''' </summary>
		Public Property SendEmailOnLoadCompletion() As Boolean

		''' <summary>
		''' URL of the web service to use
		''' </summary>
		Public Property WebServiceURL() As String

		''' <summary>
		''' '
		''' </summary>
		Public Property StartRecordNumber() As Int64

		''' <summary>
		''' '
		''' </summary>
		Public Property ObjectFieldIdListContainsArtifactId As IList(Of Int32)

		'Public Property Credential As System.Net.NetworkCredential


	End Class
End Namespace