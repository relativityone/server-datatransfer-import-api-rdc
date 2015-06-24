﻿Imports System.Collections.Generic

Namespace kCura.Relativity.DataReaderClient

	''' <summary>
	''' Defines the base class for the Settings and ImageSettings classes. Provides common properties.
	''' </summary>
	Public Class ImportSettingsBase

		Friend Sub New()
			ExtractedTextFieldContainsFilePath = False

		End Sub

		''' <summary>
		''' Indicates the level of auditing for an import job.
		''' </summary>
		''' <list type="bullet">
		'''		<listheader>
		'''			<description>This property can be set to one of the following values:</description>
		'''		</listheader>
		'''		<item>
		'''			<description>FullAudit - Includes create, update, and delete messages. Snapshot is also enabled, so all current field values (Audit Details) are captured for updates.
		'''				FullAudit is the default setting.</description>
		'''		</item>
		'''		<item>
		'''			<description>NoSnapshot - Includes create, update, and delete messages. Snapshot is disabled, so current field values (Audit Details) aren't captured for updates.</description>
		'''		</item>
		'''		<item>
		'''			<description>NoAudit - Auditing is disabled.</description>
		'''		</item>
		''' </list>
		Public Property AuditLevel As kCura.EDDS.WebAPI.BulkImportManagerBase.ImportAuditLevel = WinEDDS.Config.AuditLevel

		''' <summary>
		''' Indicates the ArtifactID of the workspace used as the destination for an import job. This property is required.
		''' </summary>
		Public Property CaseArtifactId() As Int32

		''' <summary>
		''' Gets or sets whether to copy files to the document repository.  If False, files will be linked instead.
		''' </summary>
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
		''' Gets or sets the name of the column that identifies the record identifier
		''' in DataGrid for documents.
		''' </summary>
		Public Property DataGridIDColumnName As String

		''' <summary>
		''' Indicates the Import Destination folder under which documents and objects, as well as folders containing documents or objects, are built.
		''' </summary>
		Public Property DestinationFolderArtifactID() As Int32

		''' <summary>
		''' Enables or disables encoding checks for each file.
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks>
		''' To use the <see cref="ExtractedTextEncoding"/> setting for all extracted text files during import, set this property to True.
		''' You can improve import performance by enabling this property, because it bypasses the process of directly detecting the encoding for each file. The default value is False, which indicates that the API will try to determine the encoding of the extracted text file. 
		''' Use this setting only when you are importing files that all have the same encoding. To avoid unexpected results, don't enable DisableExtractedTextEncodingCheck when you are running an import that uses several different encodings.</remarks>
		Public Property DisableExtractedTextEncodingCheck() As Boolean?

		''' <summary>
		''' Enables or disables user permission checks per document or object. 
		''' </summary>
		''' <remarks>Set this property to True if you want to disable security checks. By default, this property is set to False, so security checks are enabled.</remarks>
		Public Property DisableUserSecurityCheck As Boolean

		''' <summary>
		''' Sets the encoding of the extracted text files.
		''' </summary>
		''' <remarks>
		''' This setting will only be used if DisableExtractedTextEncodingCheck() is set to True.
		''' </remarks>
		Public Property ExtractedTextEncoding() As Text.Encoding

		''' <summary>
		''' Indicates whether the Extracted Text field contains a path to the extracted text file or contains the actual extracted text.
		''' </summary>
		Public Property ExtractedTextFieldContainsFilePath() As Boolean

		''' <summary>
		''' Indicates whether the Extracted Text field data will be loaded into SQL directly from its file path, rather than as part of a bulk load file.
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks>All Extracted Text files must exactly match the encoding of the Extracted Text field when this is set to True. 
		''' If extracted text is unicode enabled, the files need to be UTF-16 encoded,  otherwise they need to be ANSI.
		''' This setting will only be used when <see cref="ExtractedTextFieldContainsFilePath"/> is also set to True.</remarks>
		Public Property TextInSqlAccessibleFileShareLocation() As Boolean

		''' <summary>
		''' Represents a key field that is set only on Overwrite mode.
		''' </summary>
		''' <remarks>Also known as OverlayIdentifierFieldArtifactID; see ImportLoadFileProcess.AuditRun(Boolean,String)</remarks>
		Public Property IdentityFieldId() As Int32

		''' <summary>
		''' Determines the maximum number of errors displayed. This property is optional.
		''' </summary>
		''' <remarks>When error count reaches the maximum value, the following message is generated: “Maximum number of errors for display reached.” 
		''' For example, if you set this value to 100 for an import containing 150 bad rows, only 101 errors will be produced. 
		''' The first 100 errors are those generated by the bad rows, while the 101st error is the maximum errors message. 
		''' If you want to use this optional value, set it to greater than 0 and less than Int32.MaxValue. The default value is 999.</remarks>
		Public Property MaximumErrorCount As Int32?

		''' <summary>
		''' Indicates whether native files are used as links or copied to the Relativity instance designated as the destination.
		''' </summary>
		Public Property NativeFileCopyMode() As NativeFileCopyModeEnum

		''' <summary>
		''' Indicates whether records should be appended or overlayed. The default mode is Append.
		''' </summary>
		''' <remarks>To set this property, see <see cref="kCura.Relativity.DataReaderClient.OverwriteModeEnum">OverwriteModeEnum</see> for a list of values.</remarks>
		Public Property OverwriteMode() As OverwriteModeEnum

		''' <summary>
		''' Indicates the name of the Field that contains a unique identifier for the record of the parent object associated with the current record.
		''' </summary>
		Public Property ParentObjectIdSourceFieldName() As String

		''' <summary>
		''' Provides the password for logging in to the Relativity instance used as a destination.
		''' </summary>
		Public Property RelativityPassword() As String

		''' <summary>
		''' Provides the username for logging in to the Relativity instance used as a destination.
		''' </summary>
		Public Property RelativityUsername() As String

		''' <summary>
		''' Specifies the field to be used as an identifier while importing.
		''' </summary>
		''' <remarks>
		''' If this identifier cannot be resolved, the control number will be used in its place.
		''' </remarks>
		Public Property SelectedIdentifierFieldName() As String

		''' <summary>
		''' Gets or sets whether to send an email to notify you of load completion.
		''' </summary>
		Public Property SendEmailOnLoadCompletion() As Boolean

		''' <summary>
		''' Specifies the server where the Relativity WebAPI is installed. This property is optional.
		''' </summary>
		''' <remarks>This property defaults to the value set in the Windows registry by the Relativity Desktop Client.</remarks>
		Public Property WebServiceURL() As String

		''' <summary>
		''' Gets or sets the record number from which to start the import.
		''' </summary>
		Public Property StartRecordNumber() As Int64

		''' <summary>
		''' Identifies a list of single or multiple object Fields that should be mapped by ArtifactID instead of by name. 
		''' </summary>
		Public Property ObjectFieldIdListContainsArtifactId As IList(Of Int32)

		'Public Property Credential As System.Net.NetworkCredential

		''' <summary>
		''' Indicates the method for overlay imports with multiple choice and multiple object fields.
		''' </summary>
		''' <value>
		''' This property can be set to one of the following values:
		''' UseRelativityDefaults: each field will be imported based on its overlay behavior settings in Relativity.
		''' MergeAll: new imported values will be added to all imported fields.
		''' ReplaceAll: all the imported fields previous values will all be overwritten with the imported values.
		''' </value>
		''' <returns></returns>
		Public Property OverlayBehavior As kCura.EDDS.WebAPI.BulkImportManagerBase.OverlayBehavior = EDDS.WebAPI.BulkImportManagerBase.OverlayBehavior.UseRelativityDefaults


	End Class
End Namespace