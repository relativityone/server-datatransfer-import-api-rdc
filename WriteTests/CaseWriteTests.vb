Imports NUnit.Framework
Imports kCura.Relativity.DataReaderClient.NUnit
Imports System.Configuration

Namespace kCura.Relativity.DataReaderClient.NUnit.WriteTests
	<TestFixture()> _
	Public Class CaseWriteTests
		Inherits WriteTestsBase

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
