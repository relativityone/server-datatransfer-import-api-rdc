Imports System.IO
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Formatters.Binary

Imports NUnit.Framework

Namespace kCura.Relativity.DataReaderClient.NUnit.Exceptions

	<TestFixture()>
	Public Class ExceptionSerializationTests

		Public Shared ReadOnly Iterator Property TestCases() As IEnumerable
			Get
				Yield New ImportCredentialException("message", "username", "url")
				Yield New ImportSettingsException("setting")
				Yield New ImportSettingsException("setting", "additionalInfo")
				Yield New ImportSettingsConflictException("setting", "conflictingSettings", "message")
			End Get
		End Property
	
		<Test>
		<TestCaseSource(nameof(TestCases))>
		Public Sub ItShouldSerializeAndDeserializeTheDataReaderClientException(ByVal exception As Exception)
			Dim formatter As IFormatter = New BinaryFormatter()
			Using stream As MemoryStream = New MemoryStream()
				formatter.Serialize(stream, exception)
				stream.Seek(0, SeekOrigin.Begin)
				Dim deserializedException As Exception = CType(formatter.Deserialize(stream), Exception)
				Assert.IsNotNull(deserializedException)
			End Using
		End Sub
	End Class
End Namespace