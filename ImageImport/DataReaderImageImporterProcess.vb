Imports System.Data

Namespace kCura.WinEDDS.ImportExtension
	Public Class DataReaderImageImporterProcess
		Inherits kCura.WinEDDS.ImportImageFileProcess

		Private _sourceData As System.Data.DataTable

		Public Sub New(ByVal sourceData As System.Data.DataTable)
			Me.New(sourceData, kCura.WinEDDS.Config.WebServiceURL)
		End Sub

		Public Sub New(ByVal sourceData As DataTable, ByVal webURL As String)
			MyBase.New(webURL)
			_sourceData = sourceData
		End Sub

		'Private Function AddColumnIndexToName(ByVal dr As System.Data.IDataReader, ByVal columnName As String) As String
		'	'function: convert [columnName] to [columnName]([index]) if [columnName] is a column in DR.  Example: FileLocation -> FileLocation(3)
		'	'*Note: I want the result to be a 1-based index. DR uses a 0-based index.  Computers, man, computers.
		'	Dim retColumnName As String = columnName
		'	If Not retColumnName Is Nothing Then
		'		retColumnName = retColumnName.ToLower
		'		Try
		'			Dim iIndex As Integer = dr.GetOrdina(retColumnName)
		'			retColumnName = String.Concat(retColumnName, "(", iIndex + 1, ")")
		'		Catch ex As IndexOutOfRangeException
		'			Throw New IndexOutOfRangeException(String.Format("ERROR: {0} must be a field in the source data", columnName), ex)
		'		End Try
		'	End If
		'	Return retColumnName
		'End Function

		Protected Overrides Function GetImageFileImporter() As kCura.WinEDDS.BulkImageFileImporter

			Select Case ImageLoadFile.Overwrite.ToLower
				Case "append"
					ImageLoadFile.Overwrite = "none"
				Case "overlay"
					ImageLoadFile.Overwrite = "strict"
				Case Else
					ImageLoadFile.Overwrite = "both"
			End Select

			Return New DataReaderImageImporter(1003697, ImageLoadFile, New kCura.Windows.Process.Controller, System.Guid.NewGuid, _sourceData, ServiceURL)

			'Dim temp As DataReaderImporter = New DataReaderImporter(DirectCast(Me.LoadFile, kCura.WinEDDS.ImportExtension.DataReaderLoadFile))
			'Dim dr As System.Data.IDataReader = temp.SourceData
			''These settings need to have [columnName]([index])
			'LoadFile.FolderStructureContainedInColumn = AddColumnIndexToName(dr, LoadFile.FolderStructureContainedInColumn)
			'LoadFile.NativeFilePathColumn = AddColumnIndexToName(dr, LoadFile.NativeFilePathColumn)
			'temp.DestinationFolder = AddColumnIndexToName(dr, temp.DestinationFolder)
			'Return DirectCast(temp, kCura.WinEDDS.BulkLoadFileImporter)
		End Function

		Protected Overrides Sub Execute()
			MyBase.Execute()
			Dim tempdir As String = System.IO.Path.GetTempPath & "FlexMigrationFiles\"
			If System.IO.Directory.Exists(tempdir) Then System.IO.Directory.Delete(tempdir, True)
		End Sub

	End Class
End Namespace

