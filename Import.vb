Imports Relativity

Namespace kCura.EDDS.WinForm

	Public Class Import
#Region " Input Validation "

		Private Function EnsureEncoding() As Boolean
			Dim determinedEncoding As System.Text.Encoding = kCura.WinEDDS.Utility.DetectEncoding(_loadFilePath, True).DeterminedEncoding
			If determinedEncoding Is Nothing Then Return True
			Return (determinedEncoding.Equals(SourceFileEncoding))
		End Function

#End Region

#Region " Run Import "

		Public Sub RunApplicationImport()
			Dim packageData As Byte()
			packageData = System.IO.File.ReadAllBytes(_loadFilePath)
			Dim importer As New kCura.WinEDDS.ApplicationDeploymentProcess(New Int32() {}, Nothing, packageData, _application.Credential, _application.CookieContainer, New Relativity.CaseInfo() {SelectedCaseInfo})
			Dim executor As New kCura.EDDS.WinForm.CommandLineProcessRunner(importer.ProcessObserver, importer.ProcessController, ErrorLoadFileLocation, ErrorReportFileLocation)
			_application.StartProcess(importer)
		End Sub

		Public Sub RunDynamicObjectImport(ByVal commandList As kCura.CommandLine.CommandList)
			Dim importer As New kCura.WinEDDS.ImportLoadFileProcess
			SelectedNativeLoadFile.SourceFileEncoding = SourceFileEncoding
			SelectedNativeLoadFile.ExtractedTextFileEncoding = ExtractedTextFileEncoding
			SelectedNativeLoadFile.SelectedCasePath = SelectedCasePath
			SelectedNativeLoadFile.CopyFilesToDocumentRepository = CopyFilesToDocumentRepository
			SelectedNativeLoadFile.DestinationFolderID = DestinationFolderID
			SelectedNativeLoadFile.StartLineNumber = StartLineNumber
			importer.LoadFile = SelectedNativeLoadFile
			importer.TimeZoneOffset = _application.TimeZoneOffset
			importer.BulkLoadFileFieldDelimiter = Config.BulkLoadFileFieldDelimiter
			importer.CloudInstance = Config.CloudInstance
			importer.ExecutionSource = Relativity.ExecutionSource.Rdc
			_application.SetWorkingDirectory(SelectedNativeLoadFile.FilePath)
			Dim executor As New kCura.EDDS.WinForm.CommandLineProcessRunner(importer.ProcessObserver, importer.ProcessController, ErrorLoadFileLocation, ErrorReportFileLocation)
			_application.StartProcess(importer)
		End Sub

		Public Sub RunNativeImport()
			If EnsureEncoding() Then
				Dim folderManager As New kCura.WinEDDS.Service.FolderManager(_application.Credential, _application.CookieContainer)
				If folderManager.Exists(SelectedCaseInfo.ArtifactID, SelectedCaseInfo.RootFolderID) Then
					Dim importer As New kCura.WinEDDS.ImportLoadFileProcess
					SelectedNativeLoadFile.SourceFileEncoding = SourceFileEncoding
					SelectedNativeLoadFile.ExtractedTextFileEncoding = ExtractedTextFileEncoding
					SelectedNativeLoadFile.SelectedCasePath = SelectedCasePath
					SelectedNativeLoadFile.CopyFilesToDocumentRepository = CopyFilesToDocumentRepository
					SelectedNativeLoadFile.DestinationFolderID = DestinationFolderID
					SelectedNativeLoadFile.StartLineNumber = StartLineNumber
					importer.LoadFile = SelectedNativeLoadFile
					importer.TimeZoneOffset = _application.TimeZoneOffset
					importer.BulkLoadFileFieldDelimiter = Config.BulkLoadFileFieldDelimiter
					importer.CloudInstance = Config.CloudInstance
					importer.ExecutionSource = Relativity.ExecutionSource.Rdc
					SelectedNativeLoadFile.ArtifactTypeID = Relativity.ArtifactType.Document
					_application.SetWorkingDirectory(SelectedNativeLoadFile.FilePath)
					Dim executor As New kCura.EDDS.WinForm.CommandLineProcessRunner(importer.ProcessObserver, importer.ProcessController, ErrorLoadFileLocation, ErrorReportFileLocation)
					_application.StartProcess(importer)
				End If
			Else
				Throw New EncodingMisMatchException(SourceFileEncoding.CodePage, kCura.WinEDDS.Utility.DetectEncoding(_loadFilePath, True).DeterminedEncoding.CodePage)
			End If
		End Sub

		Public Sub RunImageImport()
			If EnsureEncoding() Then
				Dim importer As New kCura.WinEDDS.ImportImageFileProcess
				SelectedImageLoadFile.CookieContainer = _application.CookieContainer
				SelectedImageLoadFile.Credential = _application.Credential
				SelectedImageLoadFile.SelectedCasePath = SelectedCasePath
				SelectedImageLoadFile.CaseDefaultPath = SelectedCaseInfo.DocumentPath
				SelectedImageLoadFile.CopyFilesToDocumentRepository = CopyFilesToDocumentRepository
				SelectedImageLoadFile.FullTextEncoding = ExtractedTextFileEncoding
				SelectedImageLoadFile.StartLineNumber = StartLineNumber
				importer.ImageLoadFile = SelectedImageLoadFile
				importer.CloudInstance = Config.CloudInstance
				importer.ExecutionSource = ExecutionSource.Rdc
				_application.SetWorkingDirectory(SelectedImageLoadFile.FileName)
				Dim executor As New kCura.EDDS.WinForm.CommandLineProcessRunner(importer.ProcessObserver, importer.ProcessController, ErrorLoadFileLocation, ErrorReportFileLocation)
				_application.StartProcess(importer)
			Else
				Throw New EncodingMisMatchException(SourceFileEncoding.CodePage, kCura.WinEDDS.Utility.DetectEncoding(_loadFilePath, True).DeterminedEncoding.CodePage)
			End If
		End Sub

#End Region

	End Class

End Namespace

