Namespace kCura.WinEDDS

	''' <summary>
	''' Provides static methods to build temporary files that are easier to debug and sort than <see cref="System.IO.Path.GetTempFileName"/> and employ unique file names to avoid collisions.
	''' </summary>
	''' <remarks>
	''' REL-135101: avoid failures when Temp is full.
	''' </remarks>
	Public Class TempFileBuilder

		''' <summary>
		''' Gets a uniquely named, zero-byte temporary file on disk and returns the full path of that file.
		''' </summary>
		''' <returns>
		''' The full path of the temporary file.
		''' </returns>
		Public Shared Function GetTempFileName() As String
			Return GetTempFileName(Nothing)
		End Function

		''' <summary>
		''' Gets a uniquely named with a user-defined suffix, zero-byte temporary file on disk and returns the full path of that file.
		''' </summary>
		''' <param name="fileNameSuffix">
		''' Specify the suffix applied to the unique file name.
		''' </param>
		''' <returns>
		''' The full path of the temporary file.
		''' </returns>
		Public Shared Function GetTempFileName(fileNameSuffix As String) As String
			Const FileNameSeparator As String = "-"
			If String.IsNullOrEmpty(fileNameSuffix) Then
				fileNameSuffix = "rel-default"
			End If

			Dim tempDirectory As String = System.IO.Path.GetTempPath()
			Dim fileName As String = String.Join(
				FileNameSeparator,
				DateTime.Now.ToString($"yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture),
				Guid.NewGuid().ToString("D").ToUpperInvariant(),
				fileNameSuffix)
			Dim file As String = System.IO.Path.Combine(tempDirectory, System.IO.Path.ChangeExtension(fileName, "tmp"))
			Using (kCura.Utility.File.Instance.Create(file))
			End Using
			Return file
		End Function
	End Class
End Namespace