Imports System.Xml
Imports System.IO
Imports kCura.WinEDDS.Exceptions
Imports Ionic.Zip

Namespace kCura.WinEDDS

	''' <summary>
	''' Helper class designed to extract the XML application file from a RAP file
	''' </summary>
	''' <remarks></remarks>
	Public Class PackageHelper

		Public Function ExtractApplicationXML(packageData As Byte(), filepath As String) As XmlDocument
			Dim xmlDoc As XmlDocument

			If System.IO.Path.GetExtension(filepath).Equals(".rap", StringComparison.OrdinalIgnoreCase) Then
				'Treat it as a RAP file
				Using zip As ZipFile = ZipFile.Read(packageData)
					Using xmlStream As New MemoryStream()
						Dim xmlEntry As ZipEntry = zip.Item("application.xml")
						If xmlEntry Is Nothing Then Throw New InvalidPackageException()
						xmlEntry.Extract(xmlStream)
						xmlStream.Position = 0
						xmlDoc = New XmlDocument()
						xmlDoc.Load(xmlStream)
						xmlStream.Close()
					End Using
				End Using
			Else
				'Treat it as an XML file
				Using ms As New System.IO.MemoryStream(packageData)
					xmlDoc = New XmlDocument()
					xmlDoc.Load(ms)
					ms.Close()
				End Using
			End If
			Return xmlDoc
		End Function

	End Class

End Namespace
