Namespace kCura.WinEDDS.ImportExtension
	Public Class DataReaderImporterProcess
		Inherits kCura.WinEDDS.ImportLoadFileProcess

		Private _sourceData As System.Data.IDataReader

		Public Sub New(ByVal sourceData As System.Data.IDataReader)
			_sourceData = sourceData

			' Use the default value for the delimiter because as a public class,
			' users of this class may not know what value to set for this
			BulkLoadFileFieldDelimiter = Relativity.Constants.DEFAULT_FIELD_DELIMITER
		End Sub

		Private Function AddColumnIndexToName(ByVal dr As System.Data.IDataReader, ByVal columnName As String) As String
			'function: convert [columnName] to [columnName]([index]) if [columnName] is a column in DR.  Example: FileLocation -> FileLocation(3)
			'*Note: I want the result to be a 1-based index. DR uses a 0-based index.  Computers, man, computers.
			Dim retColumnName As String = columnName
			If Not retColumnName Is Nothing Then
				retColumnName = retColumnName.ToLower
				Try
					Dim iIndex As Integer = dr.GetOrdinal(retColumnName)
					retColumnName = String.Concat(retColumnName, "(", iIndex + 1, ")")
				Catch ex As IndexOutOfRangeException
					Throw New IndexOutOfRangeException(String.Format("ERROR: {0} must be a field in the source data", columnName), ex)
				End Try
			End If
			Return retColumnName
		End Function

		Public Overrides Function GetImporter() As kCura.WinEDDS.BulkLoadFileImporter
			Dim temp As DataReaderImporter = New DataReaderImporter(DirectCast(Me.LoadFile, kCura.WinEDDS.ImportExtension.DataReaderLoadFile), ProcessController, BulkLoadFileFieldDelimiter)
			Dim dr As System.Data.IDataReader = temp.SourceData
			'These settings need to have [columnName]([index])
			LoadFile.FolderStructureContainedInColumn = AddColumnIndexToName(dr, LoadFile.FolderStructureContainedInColumn)
			LoadFile.NativeFilePathColumn = AddColumnIndexToName(dr, LoadFile.NativeFilePathColumn)
			temp.DestinationFolder = AddColumnIndexToName(dr, temp.DestinationFolder)
			Return DirectCast(temp, kCura.WinEDDS.BulkLoadFileImporter)
		End Function

		Protected Overrides Sub Execute()
			MyBase.Execute()
			Dim tempdir As String = System.IO.Path.GetTempPath & "FlexMigrationFiles\"
			If System.IO.Directory.Exists(tempdir) Then System.IO.Directory.Delete(tempdir, True)
		End Sub

		Protected Overrides Sub Execute(ByVal webServiceURL As String)
			If String.IsNullOrEmpty(webServiceURL) Then
				Throw New ArgumentNullException("webServiceURL")
			End If

			Me.Execute()
		End Sub

	End Class
End Namespace

