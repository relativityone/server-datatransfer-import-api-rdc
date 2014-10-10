Imports kCura.WinEDDS.ImportExtension

Friend Class StubDataReaderReader
	Inherits DataReaderReader

	Public Sub New(ByVal args As DataReaderReaderInitializationArgs, ByVal fieldMap As kCura.WinEDDS.LoadFile, ByVal reader As System.Data.IDataReader)
		MyBase.new(args, fieldMap, reader)
 End Sub

	Public Sub SetFieldValueTest(ByVal field As kCura.WinEDDS.Api.ArtifactField, ByVal value As Object)
		SetFieldValue(field, value)
 End Sub

	Public Sub SetFieldValueInvokerForTesting(ByVal idx As Integer, ByVal field As kCura.WinEDDS.Api.ArtifactField, ByVal displayName As String)
		SetFieldValueInvoker(idx, field, displayName)
	End Sub

	Public Sub SetCurrentLine(ByVal lineNum As Long)
		_currentLineNumber = lineNum
	End Sub
End Class