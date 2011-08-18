Namespace kCura.Relativity.DataReaderClient
	Public Class ImageSettings
		Inherits ImportSettingsBase

#Region "Properties"
		''' <summary>
		''' TODO: Remove. Only here for the NUnit project.
		''' </summary>
		Public Property ArtifactTypeId() As Int32

		''' <summary>
		''' Auto number Images yes/no
		''' </summary>
		Public Property AutoNumberImages() As Boolean

		''' <summary>
		''' Name of the field in the data source corresponding to the BatesNumber field
		''' </summary>
		Public Property BatesNumberField() As String

		''' <summary>
		''' '
		''' </summary>
		Public Property BeginBatesFieldArtifactID() As Int32

		''' <summary>
		''' Name of the field in the data source corresponding to the DocumentIdentifier field
		''' </summary>
		Public Property DocumentIdentifierField() As String

		''' <summary>
		''' Name of the field in the data source corresponding to the FileLocation field
		''' </summary>
		Public Property FileLocationField() As String

		''' <summary>
		''' Field name that contains ???
		''' </summary>
		Public Property FolderPathSourceFieldName() As String

		''' <summary>
		''' Set to true if you want the import to go into a production
		''' </summary>
		Public Property ForProduction() As Boolean

		''' <summary>
		''' Field name that contains a full path and filename to image files
		''' </summary>
		Public Property ImageFilePathSourceFieldName() As String

		''' <summary>
		''' If this is set then images will load into selected production
		''' </summary>
		Public Property ProductionArtifactID() As Int32
#End Region

	End Class
End Namespace