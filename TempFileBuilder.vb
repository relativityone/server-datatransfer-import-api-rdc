Namespace kCura.WinEDDS

	''' <summary>
	''' Provides static methods to build temporary files that are easier to debug and sort than <see cref="System.IO.Path.GetTempFileName"/> and employ unique file names to avoid collisions.
	''' </summary>
	''' <remarks>
	''' REL-135101: avoid failures when Temp is full.
	''' </remarks>
	Public Class TempFileBuilder

		''' <summary>
		''' The native load file name prefix constant.
		''' </summary>
		Public Const NativeLoadFileNamePrefix As String = "rel-native"

		''' <summary>
		''' The datagrid load file name prefix constant.
		''' </summary>
		Public Const DataGridLoadFileNamePrefix As String = "rel-datagrid"

		''' <summary>
		''' The code load file name prefix constant.
		''' </summary>
		Public Const CodeLoadFileNamePrefix As String = "rel-code"

		''' <summary>
		''' The RDO load file name prefix constant.
		''' </summary>
		Public Const ObjectLoadFileNamePrefix As String = "rel-object"

		''' <summary>
		''' The errors file name prefix constant.
		''' </summary>
		Public Const ErrorsFileNamePrefix As String = "rel-errors"

		''' <summary>
		''' The IPro file name prefix constant.
		''' </summary>
		Public Const IProFileNamePrefix As String = "rel-ipro"

		''' <summary>
		''' The full text file name prefix constant.
		''' </summary>
		Public Const FullTextFileNamePrefix As String = "rel-full-text"

		''' <summary>
		''' Gets a uniquely named, zero-byte temporary file on disk and returns the full path of that file.
		''' </summary>
		''' <returns>
		''' The full path of the temporary file.
		''' </returns>
		Public Shared Function GetTempFile() As String
			Return GetTempFile(Nothing)
		End Function

		''' <summary>
		''' Gets a uniquely named with a user-defined prefix, zero-byte temporary file on disk and returns the full path of that file.
		''' </summary>
		''' <param name="fileNamePrefix">
		''' Specify the prefix applied to the unique file name.
		''' </param>
		''' <returns>
		''' The full path of the temporary file.
		''' </returns>
		Public Shared Function GetTempFile(fileNamePrefix As String) As String
			Const Create As Boolean = True
			Return GetTempFile(fileNamePrefix, Create)
		End Function

		''' <summary>
		''' Gets a uniquely named with a user-defined prefix, optional zero-byte temporary file on disk and returns the full path of that file.
		''' </summary>
		''' <param name="fileNamePrefix">
		''' Specify the prefix applied to the unique file name.
		''' </param>
		''' <param name="create">
		''' <see langword="true" /> to create the file; otherwise, <see langword="false" />.
		''' </param>
		''' <returns>
		''' The full path of the temporary file.
		''' </returns>
		Public Shared Function GetTempFile(fileNamePrefix As String, create As Boolean) As String
			Const FileNameSeparator As String = "-"
			If String.IsNullOrEmpty(fileNamePrefix) Then
				fileNamePrefix = "rel-default"
			End If

			Dim tempDirectory As String = System.IO.Path.GetTempPath()
			Dim fileName As String = String.Join(
				FileNameSeparator,
				DateTime.Now.ToString($"yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture),
				Guid.NewGuid().ToString("D").ToUpperInvariant(),
				fileNamePrefix)
			Dim file As String = System.IO.Path.Combine(tempDirectory, System.IO.Path.ChangeExtension(fileName, "tmp"))
			If create Then
				Using (kCura.Utility.File.Instance.Create(file))
				End Using
			End If
			Return file
		End Function
	End Class
End Namespace