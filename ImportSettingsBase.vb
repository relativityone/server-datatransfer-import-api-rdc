﻿Namespace kCura.Relativity.DataReaderClient
	Public Class ImportSettingsBase
		''' <summary>
		''' ArtifactId of the destination Relativity workspace ('workspace' = 'case')
		''' </summary>
		Public Property CaseArtifactId() As Int32

		<Obsolete("TODO: NativeFileCopyMode")>
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
		''' Sets the encoding of the extracted text files
		''' </summary>
		Public Property ExtractedTextEncoding() As Text.Encoding

		''' <summary>
		''' Sets whether the "Extracted Text" field contains paths to extracted text files or contains the actual extracted text
		''' </summary>
		Public Property ExtractedTextFieldContainsFilePath() As Boolean

		''' <summary>
		''' Sets whether native files are copied to the destination Relativity instance or whether they are used as links
		''' </summary>
		Public Property NativeFileCopyMode() As NativeFileCopyModeEnum

		''' <summary>
		''' Field name to identify matching records when overlaying records
		''' </summary>
		Public Property OverlayIdentifierSourceFieldName() As String

		''' <summary>
		''' Determines if records should be appended or overlayed
		''' </summary>
		Public Property OverwriteMode() As OverwriteModeEnum

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
		Public Property SendEmailOnLoadCompletion() As Boolean

		''' <summary>
		''' URL of the web service to use
		''' </summary>
		Public Property ServiceURL() As String
	End Class
End Namespace