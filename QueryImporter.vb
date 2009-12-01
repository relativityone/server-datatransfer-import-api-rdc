Namespace kCura.WinEDDS.ImportExtension
	Public Class QueryImporter
		Inherits kCura.WinEDDS.BulkLoadFileImporter

		Public Sub New(ByVal loadFile As kCura.WinEDDS.LoadFile)
			MyBase.New(loadFile, Nothing, 0, True, System.Guid.NewGuid, True)
		End Sub

		Protected Overrides Function GetArtifactReader() As kCura.WinEDDS.Api.IArtifactReader
			Dim connectionString As String = "data source=kcuraflex;initial catalog=FLEX1061368;persist security info=False;user id=FlexDBO;password=flex; workstation id=localhost;packet size=4096"
			Dim sql As String = "SELECT * FROM TaskExport"
			Dim context As New kCura.Data.RowDataGateway.Context(connectionString)
			Dim collection As New kCura.WinEDDS.Api.ArtifactFieldCollection
			Dim dt As System.Data.DataTable = context.ExecuteSQLStatementAsDataTable(sql, 10000)
			For Each field As kCura.EDDS.WebAPI.DocumentManagerBase.Field In New kCura.WinEDDS.Service.FieldQuery(_settings.Credentials, _settings.CookieContainer).RetrieveAllAsArray(_settings.CaseInfo.ArtifactID, _settings.ArtifactTypeID, True)
				field.Value = Nothing
				field.FieldCategory = CType(field.FieldCategoryID, kCura.EDDS.WebAPI.DocumentManagerBase.FieldCategory)
				collection.Add(New kCura.WinEDDS.Api.ArtifactField(field))
				If dt.Columns.Contains(field.DisplayName) Then
					_settings.FieldMap.Add(New kCura.WinEDDS.LoadFileFieldMap.LoadFileFieldMapItem(New kCura.WinEDDS.DocumentField(field.DisplayName, field.ArtifactID, field.FieldTypeID, field.FieldCategoryID, field.CodeTypeID, field.MaxLength, field.AssociativeArtifactTypeID, field.UseUnicodeEncoding), 0))
				End If
			Next
			Return New QueryReader(New QueryReaderInitializationArgs(collection, _settings.ArtifactTypeID), _settings, dt)
		End Function

	End Class
End Namespace

