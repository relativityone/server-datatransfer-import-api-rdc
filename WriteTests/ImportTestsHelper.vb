﻿Imports NUnit.Framework
Imports kCura.Relativity.DataReaderClient.NUnit
Imports System.Configuration
Imports System.IO
Imports System.Data.SqlClient
Imports kCura.Relativity.DataReaderClient

Namespace kCura.Relativity.DataReaderClient.NUnit.WriteTests
	Public Class ImportTestsHelper
		Private Shared ReadOnly INITIAL_CATELOG As String = "EDDS"
		Private Shared ReadOnly CONNECTION_STRING As String = "data source=localhost\integratedtests;initial catalog={0};persist security info=False;user id=EDDSdbo;password=edds; workstation id=localhost;packet size=4096;pooling=false"

		Public Shared Function ExecuteSQLStatementAsDataTable(ByVal sqlStatement As String, ByVal caseArtifactID As Int32) As System.Data.DataTable
			Dim dataAdapter As System.Data.SqlClient.SqlDataAdapter
			Dim dataTable As System.Data.DataTable
			Dim command As New SqlCommand
			command.CommandText = sqlStatement
			Dim initialCatalog As String = INITIAL_CATELOG
			If caseArtifactID > 0 Then initialCatalog &= caseArtifactID
			Dim connectionString As String = String.Format(CONNECTION_STRING, initialCatalog)
			command.Connection = New SqlConnection(connectionString)
			command.Connection.Open()
			dataAdapter = New System.Data.SqlClient.SqlDataAdapter(command)
			dataTable = New System.Data.DataTable
			dataAdapter.Fill(dataTable)
			command.Connection.Close()
			Return dataTable
		End Function

		Public Shared Sub ExecuteSQLStatement(ByVal sqlStatement As String, ByVal caseArtifactID As Int32)
			Dim command As New SqlCommand
			command.CommandText = sqlStatement
			Dim initialCatalog As String = INITIAL_CATELOG
			If caseArtifactID > 0 Then initialCatalog &= caseArtifactID
			Dim connectionString As String = String.Format(CONNECTION_STRING, initialCatalog)
			command.Connection = New SqlConnection(connectionString)
			command.Connection.Open()
			command.ExecuteNonQuery()
			command.Connection.Close()
		End Sub

		Public Shared Function ExecuteSQLStatementAsString(ByVal sqlStatement As String, ByVal caseArtifactID As Int32) As String
			Dim result As String
			Dim command As New SqlCommand
			command.CommandText = sqlStatement
			Dim initialCatalog As String = INITIAL_CATELOG
			If caseArtifactID > 0 Then initialCatalog &= caseArtifactID
			Dim connectionString As String = String.Format(CONNECTION_STRING, initialCatalog)
			command.Connection = New SqlConnection(connectionString)
			command.Connection.Open()
			result = DirectCast(command.ExecuteScalar, String)
			command.Connection.Close()
			Return result
		End Function

		Public Shared Function GetFileSize(ByVal fileName As String) As Int32
			Dim retval As Int32 = 0
			With New FileInfo(fileName)
				If .Exists Then retval = CType(.Length, Int32)
				.Delete()
			End With
			Return retval
		End Function

	End Class
End Namespace
