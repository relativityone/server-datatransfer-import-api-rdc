Imports System.Xml.Linq
Namespace kCura.WinEDDS
	Public Class ExportFileSerializer
		Public Function TransformExportFileXml(ByVal input As XDocument) As String
			Return input.ToString
		End Function
		Public Function DeserializeExportFile(ByVal currentExportFile As kCura.WinEDDS.ExportFile, ByVal xml As String) As kCura.WinEDDS.ExportFile
			Dim retval As New kCura.WinEDDS.ExportFile(currentExportFile.ArtifactTypeID)
			Dim deserializer As New System.Runtime.Serialization.Formatters.Soap.SoapFormatter
			Dim cleansedInput As String = Me.TransformExportFileXml(XDocument.Parse(xml))
			Dim sr As New System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(cleansedInput))
			Try
				retval = DirectCast(deserializer.Deserialize(sr), WinEDDS.ExportFile)
			Catch
				Throw
			Finally
				sr.Close()
			End Try
			Return retval
		End Function
	End Class
End Namespace

