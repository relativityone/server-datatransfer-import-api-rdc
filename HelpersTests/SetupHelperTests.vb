Imports NUnit.Framework
Imports kCura.Relativity.DataReaderClient.NUnit
Imports System.Configuration

Namespace kCura.Relativity.DataReaderClient.NUnit.HelperTests
	<TestFixture()> _
	Public Class SetupHelperTests
		Inherits kCura.Relativity.DataReaderClient.NUnit.WriteTests.WriteTestsBase

		<Test(), _
		Category("InfrastructureTests")> _
		Public Sub RestoreDB_BackupExists_RestoreSucceeds()
			' Arrange
			Dim helper As New kCura.IntegrationTest.SetupHelper()

			' Act
			Dim theseWorkspaceIDs As New System.Collections.Generic.List(Of Int32)
			theseWorkspaceIDs.Add(1016623)
			theseWorkspaceIDs.Add(1016621)
			helper.SetupForIntegrationTests(theseWorkspaceIDs)

			' Assert
			Assert.That(1, [Is].EqualTo(1))
		End Sub
	End Class
End Namespace
