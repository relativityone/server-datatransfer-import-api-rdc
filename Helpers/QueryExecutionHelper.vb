Imports System.IO

Namespace kCura.Relativity.DataReaderClient.NUnit.Helpers
	Public Class QueryExecutionHelper
		Public Shared Sub RunSqlNonQuery(ByVal sqlToRun As String, ByVal forSetup As Boolean, ByVal conn As SqlClient.SqlConnection, ByVal logDirName As String)
			Try
				conn.Open()
				Dim command As New SqlClient.SqlCommand
				command.CommandText = sqlToRun.ToString()

				command.Connection = conn
				command.ExecuteNonQuery()
			Catch x As SqlClient.SqlException
				Helpers.FileHelper.SaveTextToFile(Date.Now.ToString() + ": The following SQL Query Failed: " + sqlToRun, _
				 logDirName, x.Message + vbNewLine + x.StackTrace)
				Throw
			Finally
				conn.Close()
			End Try

		End Sub

		Public Shared Function RunSqlQueryScalarResult(ByVal sqlToRun As String, ByVal forSetup As Boolean, ByVal conn As SqlClient.SqlConnection, ByVal logDirName As String) As Object
			Dim result As Object = Nothing
			Try
				conn.Open()
				Dim command As New SqlClient.SqlCommand

				Dim combinedSql As String = sqlToRun.ToString()
				command.CommandText = combinedSql

				command.Connection = conn
				result = command.ExecuteScalar()
			Catch x As SqlClient.SqlException
				Helpers.FileHelper.SaveTextToFile(Date.Now.ToString() + ": The following SQL Query Failed: " + sqlToRun, _
				 logDirName, x.Message + vbNewLine + x.StackTrace)
				Throw
			Finally
				conn.Close()
			End Try
			Return result
		End Function

	End Class
End Namespace
