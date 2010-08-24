Imports NUnit.Framework

Namespace kCura.Relativity.DataReaderClient.NUnit.HelperTests
	<TestFixture()> _
	Public Class SetupHelperTests

		<Test(), _
		Ignore(), _
		Category("InfrastructureTests")> _
		Public Sub RestoreDB_BackupExists_RestoreSucceeds()
			' Arrange
			Dim helper As New Helpers.SetupHelper()
			Dim successful As Boolean

			' Act
			successful = helper.RestoreDatabases()

			' Assert
			Assert.That(successful, [Is].True)
		End Sub

		<Test(), _
		Ignore(), _
		Category("InfrastructureTests")> _
		Public Sub ParentDirectoryName_ReturnsNUnitProjectDirectory()
			' Arrange
			Dim helper As New Helpers.SetupHelper()

			' Act
			Dim dir As String = helper.ParentDirectoryName()

			' Assert
			Assert.That(dir.ToLower(), [Is].EqualTo("C:\SourceCode\trunk\EDDS\kCura.Relativity.Client.NUnit".ToLower()))
		End Sub
	End Class
End Namespace
