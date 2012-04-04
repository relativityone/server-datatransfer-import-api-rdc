Imports NUnit.Framework
Imports kCura.WinEDDS.NUnit.TestObjectFactories

Namespace kCura.WinEDDS.NUnit

	<TestFixture()> Public Class ExportFileSerializer

		Protected Property QueryFieldFactory As QueryFieldFactory = New QueryFieldFactory
		Public Const CORRECT_FOLDER_PATH As String = "\ValidExport\Location\"
		Private _serializer As New kCura.WinEDDS.ExportFileSerializer With {.SettingsValidator = New MockExportSettingsValidator}

#Region " XML Builder - for further test creation "

		'<Test()>
		Public Sub not_a_test()
			Dim x As New kCura.WinEDDS.ExportFile(10)
			x.ArtifactID = 100
			x.AppendOriginalFileName = True
			x.LoadFilesPrefix = "Wah"
			x.NestedValueDelimiter = "\"c
			x.TypeOfExport = ExportFile.ExportType.ArtifactSearch
			x.FolderPath = "\\This\Is\A\Folder Path"
			x.ViewID = 666
			x.Overwrite = True
			x.RecordDelimiter = "þ"c
			x.QuoteDelimiter = "•"c
			x.NewlineDelimiter = ChrW(10)
			x.MultiRecordDelimiter = "ÿ"c
			x.ExportFullText = True
			x.ExportFullTextAsFile = True
			x.ExportNative = True
			x.LogFileFormat = WinEDDS.LoadFileType.FileFormat.IPRO_FullText
			x.RenameFilesToIdentifier = True
			x.IdentifierColumnName = "Control Number"
			x.LoadFileExtension = ".LFE"
			x.ExportImages = True
			x.ExportNativesToFileNamedFrom = WinEDDS.ExportNativeWithFilenameFrom.Production
			x.FilePrefix = "WANG_"
			x.TypeOfExportedFilePath = ExportFile.ExportedFilePathType.Prefix
			x.TypeOfImage = ExportFile.ImageType.Pdf
			x.AppendOriginalFileName = True
			x.LoadFileIsHtml = True
			x.MulticodesAsNested = True
			x.LoadFileEncoding = Nothing
			x.TextFileEncoding = System.Text.Encoding.UTF8
			x.VolumeDigitPadding = 656
			x.SubdirectoryDigitPadding = 657
			x.StartAtDocumentNumber = 658
			x.VolumeInfo = New Exporters.VolumeInfo With {.CopyFilesFromRepository = True, .SubdirectoryFullTextPrefix = "TXT", .SubdirectoryImagePrefix = "IMG", .SubdirectoryNativePrefix = "NAT", .SubdirectoryMaxSize = 10, .SubdirectoryStartNumber = 1, .VolumeMaxSize = 100, .VolumeStartNumber = 1000}
			x.SelectedTextFields = {QueryFieldFactory.GetExtractedTextField}
			x.ImagePrecedence = New Pair() {New Pair("1", "A1"), New Pair("2", "B2")}
			x.SelectedViewFields = New kCura.WinEDDS.ViewFieldInfo() {}
			Dim sw As New System.IO.StreamWriter("C:\TempExportFiles\" & System.Guid.NewGuid.ToString("N") & ".kwe")
			Dim serializer As New System.Runtime.Serialization.Formatters.Soap.SoapFormatter
			serializer.Serialize(sw.BaseStream, x)
			sw.Close()
		End Sub

#End Region

		'enums are being serialized as their underlying ints; ensure that existing enum ids aren't being changed
		'TODO: Move this to a separate test assembly
#Region "Export type serialization"
		<Test()> Public Sub ExportType_AncestorSearch()
			Assert.AreEqual(CInt(kCura.WinEDDS.ExportFile.ExportType.AncestorSearch), 3)
		End Sub

		<Test()> Public Sub ExportType_ArtifactSearch()
			Assert.AreEqual(CInt(kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch), 1)
		End Sub

		<Test()> Public Sub ExportType_ParentSearch()
			Assert.AreEqual(CInt(kCura.WinEDDS.ExportFile.ExportType.ParentSearch), 2)
		End Sub
		<Test()> Public Sub ExportType_Production()
			Assert.AreEqual(CInt(kCura.WinEDDS.ExportFile.ExportType.Production), 0)
		End Sub
		<Test()> Public Sub ExportType_Range()
			Dim min, max As Int32?
			Dim count As Int32 = 0
			For Each e As kCura.WinEDDS.ExportFile.ExportType In System.Enum.GetValues(GetType(kCura.WinEDDS.ExportFile.ExportType))
				If Not min.HasValue Then min = CInt(e)
				If Not max.HasValue Then max = CInt(e)
				min = System.Math.Min(min.Value, CInt(e))
				max = System.Math.Max(max.Value, CInt(e))
				count += 1
			Next
			Assert.AreEqual(4, count)
			Assert.AreEqual(3, max, "All export types should be tested")
			Assert.AreEqual(0, min, "All export types should be tested")
		End Sub
#End Region

#Region "ExportedFilePathType serialization"
		<Test()> Public Sub ExportedFilePathType_Relative()
			Assert.AreEqual(CInt(kCura.WinEDDS.ExportFile.ExportedFilePathType.Relative), 0)
		End Sub
		<Test()> Public Sub ExportedFilePathType_Absolute()
			Assert.AreEqual(CInt(kCura.WinEDDS.ExportFile.ExportedFilePathType.Absolute), 1)
		End Sub
		<Test()> Public Sub ExportedFilePathType_Prefix()
			Assert.AreEqual(CInt(kCura.WinEDDS.ExportFile.ExportedFilePathType.Prefix), 2)
		End Sub
		<Test()> Public Sub ExportedFilePathType_Range()
			Dim min, max As Int32?
			Dim count As Int32 = 0
			For Each e As kCura.WinEDDS.ExportFile.ExportedFilePathType In System.Enum.GetValues(GetType(kCura.WinEDDS.ExportFile.ExportedFilePathType))
				If Not min.HasValue Then min = CInt(e)
				If Not max.HasValue Then max = CInt(e)
				min = System.Math.Min(min.Value, CInt(e))
				max = System.Math.Max(max.Value, CInt(e))
				count += 1
			Next
			Assert.AreEqual(3, count)
			Assert.AreEqual(2, max, "All Exported File Path Types should be tested")
			Assert.AreEqual(0, min, "All Exported File Path Types should be tested")
		End Sub
#End Region

#Region "ImageType Serialization"
		<Test()> Public Sub ImageType_SinglePage()
			Assert.AreEqual(CInt(kCura.WinEDDS.ExportFile.ImageType.SinglePage), 0)
		End Sub
		<Test()> Public Sub ImageType_Pdf()
			Assert.AreEqual(CInt(kCura.WinEDDS.ExportFile.ImageType.Pdf), 2)
		End Sub
		<Test()> Public Sub ImageType_MultiPageTiff()
			Assert.AreEqual(CInt(kCura.WinEDDS.ExportFile.ImageType.MultiPageTiff), 1)
		End Sub
		<Test()> Public Sub ImageType_Range()
			Dim min, max As Int32?
			Dim count As Int32 = 0
			For Each e As kCura.WinEDDS.ExportFile.ImageType In System.Enum.GetValues(GetType(kCura.WinEDDS.ExportFile.ImageType))
				If Not min.HasValue Then min = CInt(e)
				If Not max.HasValue Then max = CInt(e)
				min = System.Math.Min(min.Value, CInt(e))
				max = System.Math.Max(max.Value, CInt(e))
				count += 1
			Next
			Assert.AreEqual(3, count)
			Assert.AreEqual(2, max, "All image types should be tested")
			Assert.AreEqual(0, min, "All image types should be tested")

		End Sub
#End Region


#Region " Test - Serialization not broken "

		<Category("MultipleDomain")>
		<Test()> Public Sub DeserializeExportFile_AllPrimitivesSet_7_2()
			Dim test As kCura.WinEDDS.ExportFile = _serializer.DeserializeExportFile(XDocument.Parse(AllPrimitivesSet_7_2))
			Compare(test)
			Assert.IsNull(test.VolumeInfo)
			Assert.IsNull(test.SelectedTextFields)
			Assert.IsNull(test.ImagePrecedence)
			Assert.IsNull(test.SelectedViewFields)
		End Sub

		<Category("MultipleDomain")>
		<Test()> Public Sub DeserializeExportFile_EverythingSet_7_2()
			Dim test As kCura.WinEDDS.ExportFile = _serializer.DeserializeExportFile(XDocument.Parse(EverythingSet_7_2))
			Compare(test)
			Assert.IsNotNull(test.VolumeInfo)
			Dim info As Exporters.VolumeInfo = test.VolumeInfo
			Assert.AreEqual(True, test.VolumeInfo.CopyFilesFromRepository)
			Assert.AreEqual(10, test.VolumeInfo.SubdirectoryMaxSize)
			Assert.AreEqual(1, test.VolumeInfo.SubdirectoryStartNumber)
			Assert.AreEqual("TXT", test.VolumeInfo.SubdirectoryFullTextPrefix(False))
			Assert.AreEqual("TEXT\TXT", test.VolumeInfo.SubdirectoryFullTextPrefix(True))
			Assert.AreEqual("NAT", info.SubdirectoryNativePrefix(False))
			Assert.AreEqual("NATIVES\NAT", info.SubdirectoryNativePrefix(True))
			Assert.AreEqual("IMG", info.SubdirectoryImagePrefix(False))
			Assert.AreEqual("IMAGES\IMG", info.SubdirectoryImagePrefix(True))
			Assert.AreEqual(100, info.VolumeMaxSize)
			Assert.AreEqual(1000, info.VolumeStartNumber)
			Assert.IsNull(info.VolumePrefix)

			Assert.IsNotNull(test.SelectedTextFields)
			Assert.AreEqual("ExtractedText", test.SelectedTextFields(0).AvfColumnName)
			Assert.AreEqual(1000187, test.SelectedTextFields(0).AvfId)
			Assert.AreEqual("Extracted Text", test.SelectedTextFields(0).DisplayName)
			Assert.AreEqual("1", test.ImagePrecedence(0).Value)
			Assert.AreEqual("2", test.ImagePrecedence(1).Value)

			Assert.IsNotNull(test.SelectedViewFields)
		End Sub

		<Category("MultipleDomain")>
		<Test()> Public Sub DeserializeExportFile_EverythingSet_7_4()
			Dim test As kCura.WinEDDS.ExportFile = _serializer.DeserializeExportFile(XDocument.Parse(EverythingSet_7_4))
			Compare(test)
			Assert.IsNotNull(test.VolumeInfo)
			Dim info As Exporters.VolumeInfo = test.VolumeInfo
			Assert.AreEqual(True, test.VolumeInfo.CopyFilesFromRepository)
			Assert.AreEqual(10, test.VolumeInfo.SubdirectoryMaxSize)
			Assert.AreEqual(1, test.VolumeInfo.SubdirectoryStartNumber)
			Assert.AreEqual("TXT", test.VolumeInfo.SubdirectoryFullTextPrefix(False))
			Assert.AreEqual("TEXT\TXT", test.VolumeInfo.SubdirectoryFullTextPrefix(True))
			Assert.AreEqual("NAT", info.SubdirectoryNativePrefix(False))
			Assert.AreEqual("NATIVES\NAT", info.SubdirectoryNativePrefix(True))
			Assert.AreEqual("IMG", info.SubdirectoryImagePrefix(False))
			Assert.AreEqual("IMAGES\IMG", info.SubdirectoryImagePrefix(True))
			Assert.AreEqual(100, info.VolumeMaxSize)
			Assert.AreEqual(1000, info.VolumeStartNumber)
			Assert.IsNull(info.VolumePrefix)

			Assert.IsNotNull(test.SelectedTextFields)
			Assert.AreEqual("ExtractedText", test.SelectedTextFields(0).AvfColumnName)
			Assert.AreEqual(1000187, test.SelectedTextFields(0).AvfId)
			Assert.AreEqual("Extracted Text", test.SelectedTextFields(0).DisplayName)
			Assert.AreEqual("1", test.ImagePrecedence(0).Value)
			Assert.AreEqual("2", test.ImagePrecedence(1).Value)

			Assert.IsNotNull(test.SelectedViewFields)
		End Sub

		<Category("MultipleDomain")>
		<Test()> Public Sub DeserializeExportFile_LogFileFormat_Unselected()
			Dim test As kCura.WinEDDS.ExportFile = _serializer.DeserializeExportFile(XDocument.Parse(AllPrimitivesSetExceptLogFileFormat_7_2))
			Assert.IsNull(test.LogFileFormat)
		End Sub

		<Category("MultipleDomain")>
		<Test()> Public Sub DeserializeExportFile_ImageType_Unselected()
			Dim test As kCura.WinEDDS.ExportFile = _serializer.DeserializeExportFile(XDocument.Parse(AllPrimitivesSetExceptImageType_7_2))
			Assert.IsNull(test.TypeOfImage)
		End Sub

		<Category("MultipleDomain")>
		<Test()> Public Sub DeserializeExportFile_ObjectTypeNameMismatch_ErrorExportFileType()
			Dim incomingExportFile As New kCura.WinEDDS.ExportFile(1000001) With {.ObjectTypeName = "Custodian"}
			_serializer.DeserializeExportFile(incomingExportFile, AllPrimitivesSetExceptImageType_7_2)
			Dim test As kCura.WinEDDS.ExportFile = _serializer.DeserializeExportFile(incomingExportFile, AllPrimitivesSetExceptImageType_7_2)
			Assert.AreEqual(GetType(kCura.WinEDDS.ErrorExportFile), test.GetType)
		End Sub

		<Category("MultipleDomain")>
		<Test()> Public Sub DeserializeExportFile_FolderPath_InvalidLocation()
			Dim incomingExportFile As New kCura.WinEDDS.ExportFile(1000001) With {.FolderPath = "Blah Blah Invalid", .ObjectTypeName = "Document"}
			_serializer.DeserializeExportFile(incomingExportFile, AllPrimitivesSetExceptImageType_7_2)
			Dim test As kCura.WinEDDS.ExportFile = _serializer.DeserializeExportFile(incomingExportFile, AllPrimitivesSetExceptImageType_7_2)
			Assert.AreEqual(String.Empty, test.FolderPath)
		End Sub

		<Category("MultipleDomain")>
		<Test()> Public Sub DeserializeExportFile_FolderPath_EmptyLocation()
			Dim incomingExportFile As New kCura.WinEDDS.ExportFile(1000001) With {.FolderPath = "Blah Blah Invalid", .ObjectTypeName = "Document"}
			_serializer.DeserializeExportFile(incomingExportFile, EmptyFolderPath)
			Dim test As kCura.WinEDDS.ExportFile = _serializer.DeserializeExportFile(incomingExportFile, EmptyFolderPath)
			Assert.AreEqual(String.Empty, test.FolderPath)
		End Sub

		<Category("MultipleDomain")>
		<Test()> Public Sub DeserializeExportFile_FolderPath_CorrectFolderPath()
			Dim incomingExportFile As New kCura.WinEDDS.ExportFile(1000001) With {.FolderPath = "Blah Blah Invalid", .ObjectTypeName = "Document"}
			_serializer.DeserializeExportFile(incomingExportFile, CorrectFolderPath)
			_serializer.SettingsValidator = New MockExportSettingsValidator
			Dim test As kCura.WinEDDS.ExportFile = _serializer.DeserializeExportFile(incomingExportFile, CorrectFolderPath)
			Assert.AreEqual(CORRECT_FOLDER_PATH, test.FolderPath)
		End Sub

		Protected Sub Compare(ByVal test As kCura.WinEDDS.ExportFile)

			Assert.IsTrue(test.ArtifactID = 100)
			Assert.AreEqual("Wah", test.LoadFilesPrefix)
			Assert.AreEqual("\"c, test.NestedValueDelimiter)
			Assert.AreEqual("ArtifactSearch", test.TypeOfExport.ToString)
			Assert.AreEqual("\\This\Is\A\Folder Path", test.FolderPath)
			Assert.AreEqual(666, test.ViewID)
			Assert.AreEqual(True, test.Overwrite)
			Assert.AreEqual("þ"c, test.RecordDelimiter)
			Assert.AreEqual("•"c, test.QuoteDelimiter)
			Assert.AreEqual(ChrW(10), test.NewlineDelimiter)
			Assert.AreEqual("ÿ"c, test.MultiRecordDelimiter)
			Assert.AreEqual(True, test.ExportFullText)
			Assert.AreEqual(True, test.ExportFullTextAsFile)
			Assert.AreEqual(True, test.ExportNative)
			Assert.AreEqual("IPRO_FullText", test.LogFileFormat.ToString)
			Assert.AreEqual(True, test.RenameFilesToIdentifier)
			Assert.AreEqual("Control Number", test.IdentifierColumnName)
			Assert.AreEqual(".LFE", test.LoadFileExtension)
			Assert.AreEqual(True, test.ExportImages)
			Assert.AreEqual("Production", test.ExportNativesToFileNamedFrom.ToString)
			Assert.AreEqual("WANG_", test.FilePrefix)
			Assert.AreEqual("Prefix", test.TypeOfExportedFilePath.ToString)
			Assert.AreEqual("Pdf", test.TypeOfImage.ToString)
			Assert.AreEqual(True, test.AppendOriginalFileName)
			Assert.AreEqual(True, test.LoadFileIsHtml)
			Assert.AreEqual(True, test.MulticodesAsNested)
			Assert.IsNull(test.LoadFileEncoding)
			Assert.AreEqual(65001, test.TextFileEncoding.CodePage)
			Assert.AreEqual(656, test.VolumeDigitPadding)
			Assert.AreEqual(657, test.SubdirectoryDigitPadding)
			Assert.AreEqual(658, test.StartAtDocumentNumber)
		End Sub

#End Region

#Region " Mocks "
		Public Class MockExportSettingsValidator
			Inherits kCura.WinEDDS.ExportFileSerializer.ExportSettingsValidator
			Public Overrides Function IsValidExportDirectory(ByVal path As String) As Boolean
				If String.IsNullOrEmpty(path) Then Return False
				If path = CORRECT_FOLDER_PATH Then Return True
				Return False
			End Function

		End Class
#End Region
	End Class
End Namespace
