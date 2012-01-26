Imports System.Configuration
Imports NUnit.Framework

Namespace kCura.Relativity.DataReaderClient.NUnit.WriteTests
	Public MustInherit Class WriteTestsBase

		<SetUp()> _
		Public Overridable Sub SetUp()
			Try
				Dim helper As New kCura.NUnit.Integration.SetupHelper
				For Each databaseName As String In DatabaseNames
					helper.RestoreSingleDatabaseFromTempBackup(databaseName)
				Next
				Dim retval As String = helper.RestoreRepositories()
				If retval <> String.Empty Then Throw New Exceptions.RepositoryException(String.Format("File Repository restore failed: {0}", retval))
			Catch ex As SqlClient.SqlException
				Throw New kCura.NUnit.Integration.Exceptions.DatabaseManagementException("Database restore failed." + ex.Message + vbNewLine + ex.StackTrace)
			End Try
		End Sub

		<TearDown()> _
		Public Overridable Sub TearDown()
		End Sub

		Protected Property DatabaseNames As System.Collections.Generic.List(Of String)


		Public Sub New()
			Dim theseDatabaseNames As New System.Collections.Generic.List(Of String)
			theseDatabaseNames.Add("EDDS1016623")
			Me.DatabaseNames = theseDatabaseNames
		End Sub
	End Class
End Namespace
