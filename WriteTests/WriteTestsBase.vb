Imports System.Configuration

Namespace kCura.Relativity.DataReaderClient.NUnit.WriteTests
	Public MustInherit Class WriteTestsBase
		'Protected client As New ArtifactManagerClient(ConfigurationManager.AppSettings("endpointNameToUse").ToString())
		Protected token As String = Nothing
		'Protected apiOpt As New APIOptions(token)
	End Class
End Namespace
