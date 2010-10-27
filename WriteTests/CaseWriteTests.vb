Imports NUnit.Framework
Imports kCura.Relativity.DataReaderClient.NUnit
Imports System.Configuration

Namespace kCura.Relativity.DataReaderClient.NUnit.WriteTests
	<TestFixture()> _
	Public Class CaseWriteTests
		Inherits WriteTestsBase
#Region "Private Variables"
#End Region

#Region "Setup/Tear down"
		''' <summary>
		''' Set up Artifact Test Cases
		''' </summary>
		<SetUp()> _
		Public Sub SetUp()
			Dim helper As New Helpers.SetupHelper()
			helper.SetupTestWithRestore()
		End Sub

		''' <summary>
		''' Tear down the test case
		''' </summary>
		''' <remarks></remarks>
		<TearDown()> _
		Public Sub TearDown()
			Dim helper As New Helpers.SetupHelper()
			helper.TearDownTest()
		End Sub
#End Region

#Region "Tests"
		<Test(), _
		Category("InfrastructureTests")> _
		Public Sub SanityTest1()
			Assert.That(1, [Is].EqualTo(1))
		End Sub

		<Test(), _
		 Category("InfrastructureTests")> _
		 Public Sub SanityTest2()
			Assert.That(1, [Is].EqualTo(1))
		End Sub

#End Region
	End Class
End Namespace
