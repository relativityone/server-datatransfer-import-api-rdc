Imports System.Xml.Linq
Namespace kCura.WinEDDS
	Public Class ExportFileSerializer
		Public Function TransformExportFileXml(ByVal input As XDocument) As String
			Return input.ToString
		End Function

		Public Function DeserializeExportFile(ByVal currentExportFile As kCura.WinEDDS.ExportFile, ByVal xml As String) As kCura.WinEDDS.ExportFile
			Dim retval As New kCura.WinEDDS.ExportFile(currentExportFile.ArtifactTypeID)
			Dim deserialized As kCura.WinEDDS.ExportFile = Me.DeserializeExportFile(XDocument.Parse(xml))
			For Each p As System.Reflection.PropertyInfo In (From prop As System.Reflection.PropertyInfo In retval.GetType.GetProperties Where prop.CanWrite)
				p.SetValue(retval, p.GetValue(If(PropertyIsReadFromExisting(p), currentExportFile, deserialized), Nothing), Nothing)
			Next
			'TODO: test
			Select Case retval.TypeOfExport
				Case ExportFile.ExportType.AncestorSearch, ExportFile.ExportType.ParentSearch
					retval.ArtifactID = currentExportFile.ArtifactID
				Case ExportFile.ExportType.Production
					retval.ImagePrecedence = New Pair() {}
			End Select
			Return retval
		End Function

		Private Function PropertyIsReadFromExisting(ByVal p As System.Reflection.PropertyInfo) As Boolean
			For Each att As Attribute In p.GetCustomAttributes(GetType(ReadFromExisting), False)
				Return True
			Next
			Return False
		End Function

		Public Function DeserializeExportFile(ByVal xml As XDocument) As ExportFile
			Dim deserializer As New System.Runtime.Serialization.Formatters.Soap.SoapFormatter
			Dim cleansedInput As String = Me.TransformExportFileXml(xml)
			Dim sr As New System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(cleansedInput))
			Dim deserialized As kCura.WinEDDS.ExportFile = Nothing
			Try
				deserialized = DirectCast(deserializer.Deserialize(sr), WinEDDS.ExportFile)
			Catch
				Throw
			Finally
				sr.Close()
			End Try
			Return deserialized
		End Function

		Protected Sub SetAllSerializableProperties(ByVal retval As kCura.WinEDDS.ExportFile, ByVal deserialized As kCura.WinEDDS.ExportFile)
			retval.AppendOriginalFileName = deserialized.AppendOriginalFileName
			retval.ExportFullText = deserialized.ExportFullText
			retval.ExportFullTextAsFile = deserialized.ExportFullTextAsFile
			retval.ExportImages = deserialized.ExportImages
			retval.ExportNative = deserialized.ExportNative
			retval.ExportNativesToFileNamedFrom = deserialized.ExportNativesToFileNamedFrom
		End Sub
		Protected Sub SetAllCurrentlySelectedProperties(ByVal retval As kCura.WinEDDS.ExportFile, ByVal current As kCura.WinEDDS.ExportFile, ByVal deserialized As kCura.WinEDDS.ExportFile)
			retval.AllExportableFields = current.AllExportableFields
			retval.AppendOriginalFileName = deserialized.AppendOriginalFileName
			retval.ArtifactAvfLookup = current.ArtifactAvfLookup
			retval.ArtifactID = current.ArtifactID
			retval.CaseInfo = current.CaseInfo
			retval.CookieContainer = current.CookieContainer

		End Sub
	End Class
End Namespace

