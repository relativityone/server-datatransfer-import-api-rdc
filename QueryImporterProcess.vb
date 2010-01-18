Namespace kCura.WinEDDS.ImportExtension
	Public Class QueryImporterProcess
        Inherits kCura.WinEDDS.ImportLoadFileProcess

        Private _serverName As String
        Private _databaseName As String
        Private _loginName As String
        Private _loginPassword As String
        Private _query As String

        Public Sub New(ByVal serverName As String, ByVal databaseName As String, ByVal loginName As String, ByVal loginPassword As String, ByVal query As String)
            _serverName = serverName
            _databaseName = databaseName
            _loginName = loginName
            _loginPassword = loginPassword
            _query = query
        End Sub

        Public Overrides Function GetImporter() As kCura.WinEDDS.BulkLoadFileImporter
            Return New QueryImporter(DirectCast(Me.LoadFile, kCura.WinEDDS.ImportExtension.SqlLoadFile))
            'Return New QueryImporter(Me.LoadFile)
        End Function

        Protected Overrides Sub Execute()
            MyBase.Execute()
            Dim tempdir As String = System.IO.Path.GetTempPath & "FlexMigrationFiles\"
            If System.IO.Directory.Exists(tempdir) Then System.IO.Directory.Delete(tempdir, True)
        End Sub

    End Class
End Namespace

