Imports System.Collections.Generic

Namespace kCura.WinEDDS.ImportExtension
	Public Class DataReaderImporter
		Inherits kCura.WinEDDS.BulkLoadFileImporter

		Private _sourceReader As System.Data.IDataReader
		Public Property OnBehalfOfUserToken As String

		Private _temporaryLocalDirectory As String
		Public Property TemporaryLocalDirectory As String
			Get
				Return _temporaryLocalDirectory
			End Get
			Set(value As String)
				_temporaryLocalDirectory = value
			End Set
		End Property

		''' <summary>
		''' Constructs a new importer that loads the provided <paramref name="loadFile" />
		''' and responds to events fired on the provided <paramref name="controller" />.
		''' </summary>
		''' <param name="loadFile">Information about the data to load into the
		''' importer</param>
		''' <param name="controller">A controller that can send events to
		''' the process that is importing</param>
		''' <param name="bulkLoadFileFieldDelimiter">The field delimiter that
		''' was used to create the bulk load file. Line delimiters are a field
		''' delimiter followed by a new line.</param>
		Public Sub New(ByVal loadFile As kCura.WinEDDS.ImportExtension.DataReaderLoadFile, ByVal controller As kCura.Windows.Process.Controller, ByVal bulkLoadFileFieldDelimiter As String)
			Me.New(loadFile, controller, bulkLoadFileFieldDelimiter, Nothing, InitializeArtifactReader:=True)
		End Sub

		''' <summary>
		''' Constructs a new importer that loads the provided <paramref name="loadFile" />
		''' and responds to events fired on the provided <paramref name="controller" />.
		''' </summary>
		''' <param name="loadFile">Information about the data to load into the
		''' importer</param>
		''' <param name="controller">A controller that can send events to
		''' the process that is importing</param>
		''' <param name="bulkLoadFileFieldDelimiter">The field delimiter that
		''' was used to create the bulk load file. Line delimiters are a field
		''' delimiter followed by a new line.</param>
		''' <param name="temporaryLocalDirectory">This directory is used if kcuramarkerfilename is a field in the table.  
		''' Files are copied to this location and their names are changed before being imported to Relativity</param>
		''' <param name="initializeArtifactReader">If True, the ArtifactReader is created and initialized within the constructor.
		''' If False, you should initialize the artifact reader later by calling Initialize().</param>
		Public Sub New(loadFile As kCura.WinEDDS.ImportExtension.DataReaderLoadFile, controller As kCura.Windows.Process.Controller, bulkLoadFileFieldDelimiter As String, temporaryLocalDirectory As String, initializeArtifactReader As Boolean)
			MyBase.New(loadFile, controller, 0, True, True, System.Guid.NewGuid, True, bulkLoadFileFieldDelimiter, initializeArtifactReader)

			Me.OIFileIdColumnName = loadFile.OIFileIdColumnName
			Me.OIFileIdMapped = loadFile.OIFileIdMapped
			Me.OIFileTypeColumnName = loadFile.OIFileTypeColumnName
			Me.FileSizeMapped = loadFile.FileSizeMapped
			Me.FileSizeColumn = loadFile.FileSizeColumn

			If temporaryLocalDirectory IsNot Nothing Then
				Me.TemporaryLocalDirectory = temporaryLocalDirectory
			End If
		End Sub

		Public Overridable Sub Initialize()
			If _artifactReader Is Nothing Then
				Me.InitializeArtifactReader()
			End If
		End Sub

		Overrides Sub OnSettingsObjectCreate(settings As kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo)
			settings.OnBehalfOfUserToken = Me.OnBehalfOfUserToken
		End Sub

		''' <summary>
		''' This method has the side effect of populating various properties for _settings
		''' </summary>
		''' <returns></returns>
		Protected Overrides Function GetArtifactReader() As kCura.WinEDDS.Api.IArtifactReader
			Dim collection As New kCura.WinEDDS.Api.ArtifactFieldCollection
			Dim thisSettings As kCura.WinEDDS.ImportExtension.DataReaderLoadFile = DirectCast(_settings, kCura.WinEDDS.ImportExtension.DataReaderLoadFile)
			_sourceReader = thisSettings.DataReader
			Dim s As New kCura.WinEDDS.Service.FieldQuery(_settings.Credentials, _settings.CookieContainer)

			Dim allFields As List(Of [String]) = New List(Of [String])()
			For i As Integer = 0 To _sourceReader.FieldCount - 1
				allFields.Add(_sourceReader.GetName(i).ToLower())
			Next
			Dim columnAliases As New Dictionary(Of String, String)

			Dim columnIndex As Int32 = 0

			For Each field As kCura.EDDS.WebAPI.DocumentManagerBase.Field In s.RetrieveAllAsArray(_settings.CaseInfo.ArtifactID, _settings.ArtifactTypeID, True)
				field.Value = Nothing
				field.FieldCategory = CType(field.FieldCategoryID, kCura.EDDS.WebAPI.DocumentManagerBase.FieldCategory)
				field.DisplayName = field.DisplayName.ToLower
				collection.Add(New kCura.WinEDDS.Api.ArtifactField(field))

				Try
					Dim name As String = field.DisplayName.ToLower()
					Dim columnName As String = name
					If (columnAliases.ContainsKey(field.DisplayName)) Then
						columnName = columnAliases(name)
					End If
					If (allFields.Contains(field.DisplayName.ToLower()) OrElse columnAliases.ContainsKey(field.DisplayName)) Then

						Dim isValidItemName As Integer = _sourceReader.GetOrdinal(columnName)

						'Do not add to field map if field.DisplayName is  _settings.FolderStructureContainedInColumn or _settings.NativeFilePathColumn
						If _settings.FolderStructureContainedInColumn Is Nothing Then					'AndAlso _settings.NativeFilePathColumn Is Nothing Then
							_settings.FieldMap.Add(New kCura.WinEDDS.LoadFileFieldMap.LoadFileFieldMapItem(New kCura.WinEDDS.DocumentField(field.DisplayName, field.ArtifactID, field.FieldTypeID, field.FieldCategoryID, field.CodeTypeID, field.MaxLength, field.AssociativeArtifactTypeID, field.UseUnicodeEncoding, field.ImportBehavior, field.StorageLocation), columnIndex))
						Else
							Dim s_FolderStructureContainedInColumn As String
							Dim s_NativeFilePathColumn As String

							If _settings.FolderStructureContainedInColumn Is Nothing Then
								s_FolderStructureContainedInColumn = String.Empty
							Else
								s_FolderStructureContainedInColumn = _settings.FolderStructureContainedInColumn.ToLower
							End If

							If _settings.NativeFilePathColumn Is Nothing Then
								s_NativeFilePathColumn = String.Empty
							Else
								s_NativeFilePathColumn = _settings.NativeFilePathColumn.ToLower
							End If

							If Not field.DisplayName = s_FolderStructureContainedInColumn Then						'then AndAlso Not field.DisplayName = s_NativeFilePathColumn Then
								_settings.FieldMap.Add(New kCura.WinEDDS.LoadFileFieldMap.LoadFileFieldMapItem(New kCura.WinEDDS.DocumentField(field.DisplayName, field.ArtifactID, field.FieldTypeID, field.FieldCategoryID, field.CodeTypeID, field.MaxLength, field.AssociativeArtifactTypeID, field.UseUnicodeEncoding, field.ImportBehavior, field.StorageLocation), columnIndex))
							End If
						End If

					End If
				Catch ex As IndexOutOfRangeException
					'field.Displayname is not in the DataReader, forget about it and continue
				End Try
				columnIndex = columnIndex + 1
			Next
			Dim settings As New FileSettings() With {.IDColumnName = OIFileIdColumnName, .OIFileIdMapped = OIFileIdMapped, .TypeColumnName = OIFileTypeColumnName, .FileSizeColumn = FileSizeColumn, .FileSizeMapped = FileSizeMapped}
			Dim initalizationArgs As New DataReaderReaderInitializationArgs(collection, _settings.ArtifactTypeID) With {.TemporaryLocalDirectory = TemporaryLocalDirectory}
			Dim retval As New DataReaderReader(initalizationArgs, _settings, _sourceReader, settings)
			Return retval
		End Function

		Public ReadOnly Property SourceData() As System.Data.IDataReader
			Get
				Return _sourceReader
			End Get
		End Property

		Public Property DestinationFolder() As String
			Get
				Return _destinationFolder
			End Get
			Set(ByVal Value As String)
				_destinationFolder = Value
			End Set
		End Property

	End Class
End Namespace

