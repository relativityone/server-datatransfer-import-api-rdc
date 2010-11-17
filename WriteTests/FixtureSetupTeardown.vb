Imports NUnit.Framework
Imports kCura.Relativity.DataReaderClient.NUnit.Helpers
Imports System.Configuration
Imports System.Data.SqlClient

Namespace kCura.Relativity.DataReaderClient.NUnit.WriteTests
	<SetUpFixture()> _
	Public Class FixtureSetupTeardown
		Inherits kCura.IntegrationTest.FixtureSetupTeardownBase

		Public Sub New()
			Dim theseWorkspaceIDs As New System.Collections.Generic.List(Of Int32)
			theseWorkspaceIDs.Add(1016621)
			theseWorkspaceIDs.Add(1016623)
			Me.WorkspaceIDs = theseWorkspaceIDs
		End Sub
	End Class
End Namespace
