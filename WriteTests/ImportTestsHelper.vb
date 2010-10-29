Imports NUnit.Framework
Imports kCura.Relativity.DataReaderClient.NUnit
Imports System.Configuration
Imports System.IO
Imports System.Data.SqlClient
Imports kCura.Relativity.DataReaderClient

Namespace kCura.Relativity.DataReaderClient.NUnit.WriteTests
	Public Class ImportTestsHelper

		Public Shared Function ExecuteSQLStatementAsDataTableAsDataReader(ByVal sqlStatement As String, ByVal caseArtifactID As Int32) As System.Data.IDataReader
			Return ExecuteReader(sqlStatement, caseArtifactID)
		End Function

		Private Shared Function ExecuteReader(ByVal sqlStatement As String, ByVal caseArtifactID As Int32) As IDataReader
			Dim command As New SqlCommand
			Dim resultReader As SqlDataReader
			command.CommandText = sqlStatement
			Dim connectionString = String.Format("data source=localhost\integratedtests;initial catalog={0};persist security info=False;user id=EDDSdbo;password=edds; workstation id=localhost;packet size=4096;pooling=false", "EDDS" & caseArtifactID)
			command.Connection = New SqlConnection(connectionString)
			command.Connection.Open()
			resultReader = command.ExecuteReader()
			'command.Connection.Close()
			Return resultReader
		End Function

		Public Shared Function ExecuteSQLStatementAsDataTable(ByVal sqlStatement As String, ByVal caseArtifactID As Int32) As System.Data.DataTable
			Dim dataAdapter As System.Data.SqlClient.SqlDataAdapter
			Dim dataTable As System.Data.DataTable
			Dim command As New SqlCommand
			command.CommandText = sqlStatement
			Dim initialCatalog As String = "EDDS"
			If caseArtifactID > 0 Then initialCatalog &= caseArtifactID
			Dim connectionString = String.Format("data source=localhost\integratedtests;initial catalog={0};persist security info=False;user id=EDDSdbo;password=edds; workstation id=localhost;packet size=4096;pooling=false", initialCatalog)
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
			Dim initialCatalog As String = "EDDS"
			If caseArtifactID > 0 Then initialCatalog &= caseArtifactID
			Dim connectionString = String.Format("data source=localhost\integratedtests;initial catalog={0};persist security info=False;user id=EDDSdbo;password=edds; workstation id=localhost;packet size=4096;pooling=false", initialCatalog)
			command.Connection = New SqlConnection(connectionString)
			command.Connection.Open()
			command.ExecuteNonQuery()
			command.Connection.Close()
		End Sub

		Public Shared Function ExecuteSQLStatementAsString(ByVal sqlStatement As String, ByVal caseArtifactID As Int32) As String
			Dim result As String
			Dim command As New SqlCommand
			command.CommandText = sqlStatement
			Dim initialCatalog As String = "EDDS"
			If caseArtifactID > 0 Then initialCatalog &= caseArtifactID
			Dim connectionString = String.Format("data source=localhost\integratedtests;initial catalog={0};persist security info=False;user id=EDDSdbo;password=edds; workstation id=localhost;packet size=4096;pooling=false", initialCatalog)
			command.Connection = New SqlConnection(connectionString)
			command.Connection.Open()
			result = DirectCast(command.ExecuteScalar, String)
			command.Connection.Close()
			Return result
		End Function

		Public Shared Function DetermineIfFileExists(ByVal fileName As String) As Boolean
			Dim FileInformain As New FileInfo(fileName)
			If FileInformain.Exists Then Return True
			Return False
		End Function

		Public Shared Function GetFileSize(ByVal fileName As String) As Int32
			Dim FileInformain As New FileInfo(fileName)
			If FileInformain.Exists Then Return CType(FileInformain.Length, Int32)
			Return 0
		End Function

		Public Shared Sub CopyDirectory(ByVal SourcePath As String, ByVal DestPath As String)
			Dim SourceDir As DirectoryInfo = New DirectoryInfo(SourcePath)
			Dim DestDir As DirectoryInfo = New DirectoryInfo(DestPath)
			If SourceDir.Exists Then
				If Not DestDir.Parent.Exists Then
					Throw New DirectoryNotFoundException("Destination directory does not exist: " + DestDir.Parent.FullName)
				End If

				If Not DestDir.Exists Then
					DestDir.Create()
				End If

				Dim ChildFile As FileInfo
				For Each ChildFile In SourceDir.GetFiles()
					If Not File.Exists(Path.Combine(DestDir.FullName, ChildFile.Name)) Then
						ChildFile.CopyTo(Path.Combine(DestDir.FullName, ChildFile.Name), False)
					End If
				Next

				Dim SubDir As DirectoryInfo
				For Each SubDir In SourceDir.GetDirectories()
					CopyDirectory(SubDir.FullName, Path.Combine(DestDir.FullName, SubDir.Name))
				Next
			Else
				Throw New DirectoryNotFoundException("Source directory does not exist: " + SourceDir.FullName)
			End If

		End Sub


		Public Enum OverwriteModeEnum
			none
			strict
			both
		End Enum


	End Class
End Namespace
