Imports NUnit.Framework

Namespace kCura.WinEDDS.NUnit
	<TestFixture>
	Public Class TempFileBuilderTests
		
		Private _tempFile As String

		<SetUp>
		Public Sub SetUp()
			_tempFile = String.Empty
		End Sub

		<TearDown>
		Public Sub Teardown()
			If (Not String.IsNullOrEmpty(_tempFile) AndAlso System.IO.File.Exists(_tempFile)) Then
				System.IO.File.Delete(_tempFile)
			End If
		End Sub

		<Test>
		<TestCase(Nothing)>
		<TestCase("custom-prefix")>
		<TestCase(TempFileBuilder.CodeLoadFileNamePrefix)>
		<TestCase(TempFileBuilder.DataGridLoadFileNamePrefix)>
		<TestCase(TempFileBuilder.ErrorsFileNamePrefix)>
		<TestCase(TempFileBuilder.FullTextFileNamePrefix)>
		<TestCase(TempFileBuilder.IProFileNamePrefix)>
		<TestCase(TempFileBuilder.NativeLoadFileNamePrefix)>
		<TestCase(TempFileBuilder.ObjectLoadFileNamePrefix)>
		Public Sub ShouldGetTheTempFileWithPrefix(prefix As String)
			WhenGettingTheTempFile(prefix, True)
			ThenTheTempFileExists()
			ThenTheTempFileIsNotLocked()
			ThenTheTempFileExists()
			ThenTheTempFileIsNotLocked()
		End Sub

		<Test>
		Public Sub ShouldGetTheTempFileButNotCreateIt()
			WhenGettingTheTempFile(TempFileBuilder.NativeLoadFileNamePrefix, False)
			ThenTheTempFileDoesNotExist()
		End Sub

		<Test>
		Public Sub ShouldGetTheTempFileWithoutThePrefix()
			WhenGettingTheTempFileWithoutThePrefix()
			ThenTheTempFileExists()
			ThenTheTempFileIsNotLocked()
		End Sub

		Private Sub WhenGettingTheTempFileWithoutThePrefix()
			_tempFile = TempFileBuilder.GetTempFile()
		End Sub

		Private Sub WhenGettingTheTempFile(prefix As String, create As Boolean)
			_tempFile = TempFileBuilder.GetTempFile(prefix, create)
		End Sub

		Private Sub ThenTheTempFileExists()
			FileAssert.Exists(_tempFile)
		End Sub

		Private Sub ThenTheTempFileDoesNotExist()
			FileAssert.DoesNotExist(_tempFile)
		End Sub

		Private Sub ThenTheTempFileIsNotLocked()
			Using (System.IO.File.Open(_tempFile, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None))
			End Using
		End Sub
	End Class
End Namespace