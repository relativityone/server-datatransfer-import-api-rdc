Imports System.IO
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Formatters.Binary

Imports NUnit.Framework

Namespace kCura.Relativity.DataReaderClient.NUnit.Exceptions

	<TestFixture()>
	Public Class ExceptionSerializationTests

		<Test>
		Public Sub ItShouldSerializeAndDeserializeTheDataReaderClientException()
	        SerializeAndDeserialize(new ImportCredentialException("message", "username", "url"))
			SerializeAndDeserialize(new ImportSettingsException("setting", "additionalInfo"))
			SerializeAndDeserialize(new ImportSettingsConflictException("setting", "conflictingSettings", "message"))
		End Sub	

		Private Sub SerializeAndDeserialize(ByVal exception As System.Exception)
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