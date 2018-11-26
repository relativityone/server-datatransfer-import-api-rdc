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
		<TestCase("custom-suffix")>
		<TestCase(TempFileConstants.CodeLoadFileNameSuffix)>
		<TestCase(TempFileConstants.DataGridLoadFileNameSuffix)>
		<TestCase(TempFileConstants.ErrorsFileNameSuffix)>
		<TestCase(TempFileConstants.FullTextFileNameSuffix)>
		<TestCase(TempFileConstants.IProFileNameSuffix)>
		<TestCase(TempFileConstants.LongTextFileNameSuffix)>
		<TestCase(TempFileConstants.NativeLoadFileNameSuffix)>
		<TestCase(TempFileConstants.ObjectLoadFileNameSuffix)>
		Public Sub ShouldGetTheTempFileWithSuffix(suffix As String)
			WhenGettingTheTempFile(suffix)
			ThenTheTempFileExists()
			ThenTheTempFileIsNotLocked()
			ThenTheTempFileExists()
			ThenTheTempFileIsNotLocked()
		End Sub

		<Test>
		Public Sub ShouldGetTheTempFileWithoutTheSuffix()
			WhenGettingTheTempFileWithoutTheSuffix()
			ThenTheTempFileExists()
			ThenTheTempFileIsNotLocked()
		End Sub

		Private Sub WhenGettingTheTempFileWithoutTheSuffix()
			_tempFile = TempFileBuilder.GetTempFileName()
		End Sub

		Private Sub WhenGettingTheTempFile(suffix As String)
			_tempFile = TempFileBuilder.GetTempFileName(suffix)
		End Sub

		Private Sub ThenTheTempFileExists()
			FileAssert.Exists(_tempFile)
		End Sub

		Private Sub ThenTheTempFileIsNotLocked()
			' Sanity check to ensure the file can be modified.
			Using (System.IO.File.Open(_tempFile, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None))
			End Using
		End Sub
	End Class
End Namespace