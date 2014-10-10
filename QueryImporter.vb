Namespace kCura.WinEDDS.ImportExtension
    Public Class QueryImporter
        Inherits kCura.WinEDDS.BulkLoadFileImporter

        Public Sub New(ByVal loadFile As kCura.WinEDDS.ImportExtension.SqlLoadFile)
            'Public Sub New(ByVal loadFile As kCura.WinEDDS.LoadFile)
            MyBase.New(loadFile, Nothing, 0, True, System.Guid.NewGuid, True)
        End Sub

        Protected Overrides Function GetArtifactReader() As kCura.WinEDDS.Api.IArtifactReader
            Dim thisSettings As kCura.WinEDDS.ImportExtension.SqlLoadFile = DirectCast(_settings, kCura.WinEDDS.ImportExtension.SqlLoadFile)
            Dim connectionstring As String = String.Format("data source={0};initial catalog={1};persist security info=False;user id={2};password={3}; workstation id=localhost;packet size=4096", thisSettings.ServerName, thisSettings.DatabaseName, thisSettings.LoginName, thisSettings.LoginPassword)
            Dim sql As String = thisSettings.Query
            Dim context As New kCura.Data.RowDataGateway.Context(connectionstring)
            Dim collection As New kCura.WinEDDS.Api.ArtifactFieldCollection
            Dim dt As System.Data.DataTable = context.ExecuteSQLStatementAsDataTable(sql, 10000)
            Dim s As New kCura.WinEDDS.Service.FieldQuery(_settings.Credentials, _settings.CookieContainer)
            For Each field As kCura.EDDS.WebAPI.DocumentManagerBase.Field In s.RetrieveAllAsArray(_settings.CaseInfo.ArtifactID, _settings.ArtifactTypeID, True)
                field.Value = Nothing
                field.FieldCategory = CType(field.FieldCategoryID, kCura.EDDS.WebAPI.DocumentManagerBase.FieldCategory)
                collection.Add(New kCura.WinEDDS.Api.ArtifactField(field))

                If dt.Columns.Contains(field.DisplayName) Then
                    _settings.FieldMap.Add(New kCura.WinEDDS.LoadFileFieldMap.LoadFileFieldMapItem(New kCura.WinEDDS.DocumentField(field.DisplayName, field.ArtifactID, field.FieldTypeID, field.FieldCategoryID, field.CodeTypeID, field.MaxLength, field.AssociativeArtifactTypeID, field.UseUnicodeEncoding), 0))
                End If
            Next
            Dim retval As New QueryReader(New QueryReaderInitializationArgs(collection, _settings.ArtifactTypeID), _settings, dt)
            Return retval
        End Function

    End Class
End Namespace

