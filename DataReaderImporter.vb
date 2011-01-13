Imports System.Collections.Generic

Namespace kCura.WinEDDS.ImportExtension
	Public Class DataReaderImporter
		Inherits kCura.WinEDDS.BulkLoadFileImporter

		Private _sourceReader As System.Data.IDataReader

		Public Sub New(ByVal loadFile As kCura.WinEDDS.ImportExtension.DataReaderLoadFile)
			MyBase.New(loadFile, Nothing, 0, True, System.Guid.NewGuid, True)
		End Sub

		Protected Overrides Function GetArtifactReader() As kCura.WinEDDS.Api.IArtifactReader
			Dim collection As New kCura.WinEDDS.Api.ArtifactFieldCollection
			Dim thisSettings As kCura.WinEDDS.ImportExtension.DataReaderLoadFile = DirectCast(_settings, kCura.WinEDDS.ImportExtension.DataReaderLoadFile)
			_sourceReader = thisSettings.DataReader
			Dim s As New kCura.WinEDDS.Service.FieldQuery(_settings.Credentials, _settings.CookieContainer)

			Dim allFields As List(Of [String]) = New List(Of [String])()
			For i As Integer = 0 To _sourceReader.FieldCount - 1
				allFields.Add(_sourceReader.GetName(i).ToLower())
			Next



			For Each field As kCura.EDDS.WebAPI.DocumentManagerBase.Field In s.RetrieveAllAsArray(_settings.CaseInfo.ArtifactID, _settings.ArtifactTypeID, True)
				field.Value = Nothing
				field.FieldCategory = CType(field.FieldCategoryID, kCura.EDDS.WebAPI.DocumentManagerBase.FieldCategory)
				field.DisplayName = field.DisplayName.ToLower
				collection.Add(New kCura.WinEDDS.Api.ArtifactField(field))

				Try
					If allFields.Contains(field.DisplayName.ToLower()) Then

						Dim isValidItemName As Integer = _sourceReader.GetOrdinal(field.DisplayName)

						'Do not add to field map if field.DisplayName is  _settings.FolderStructureContainedInColumn or _settings.NativeFilePathColumn
						If _settings.FolderStructureContainedInColumn Is Nothing Then					'AndAlso _settings.NativeFilePathColumn Is Nothing Then
							_settings.FieldMap.Add(New kCura.WinEDDS.LoadFileFieldMap.LoadFileFieldMapItem(New kCura.WinEDDS.DocumentField(field.DisplayName, field.ArtifactID, field.FieldTypeID, field.FieldCategoryID, field.CodeTypeID, field.MaxLength, field.AssociativeArtifactTypeID, field.UseUnicodeEncoding, field.ImportBehavior.Value), 0))
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
								_settings.FieldMap.Add(New kCura.WinEDDS.LoadFileFieldMap.LoadFileFieldMapItem(New kCura.WinEDDS.DocumentField(field.DisplayName, field.ArtifactID, field.FieldTypeID, field.FieldCategoryID, field.CodeTypeID, field.MaxLength, field.AssociativeArtifactTypeID, field.UseUnicodeEncoding, field.ImportBehavior.Value), 0))
							End If
						End If

					End If
				Catch ex As IndexOutOfRangeException
					'field.Displayname is not in the DataReader, forget about it and continue
				End Try
			Next
			Dim retval As New DataReaderReader(New DataReaderReaderInitializationArgs(collection, _settings.ArtifactTypeID), _settings, _sourceReader)
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

