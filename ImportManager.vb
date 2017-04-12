Imports Relativity

Namespace kCura.EDDS.WinForm

	Public Class ImportManager

#Region " Input Validation "

		Private Function EnsureEncoding(ByVal importOptions As ImportOptions) As Boolean
			Dim determinedEncoding As System.Text.Encoding = kCura.WinEDDS.Utility.DetectEncoding(importOptions.LoadFilePath, True).DeterminedEncoding
			If determinedEncoding Is Nothing Then Return True
			Return (determinedEncoding.Equals(importOptions.SourceFileEncoding))
		End Function

#End Region

#Region " Run Import "

		Friend Sub RunApplicationImport(ByVal importOptions As ImportOptions)
			Dim packageData As Byte()
			packageData = System.IO.File.ReadAllBytes(importOptions.LoadFilePath)
			Dim importer As New kCura.WinEDDS.ApplicationDeploymentProcess(New Int32() {}, Nothing, packageData, _application.GetCredentials, _application.CookieContainer, New Relativity.CaseInfo() {importOptions.SelectedCaseInfo})
			Dim executor As New kCura.EDDS.WinForm.CommandLineProcessRunner(importer.ProcessObserver, importer.ProcessController, importOptions.ErrorLoadFileLocation, importOptions.ErrorReportFileLocation)
			_application.StartProcess(importer)
		End Sub

		Friend Sub RunDynamicObjectImport(ByVal importOptions As ImportOptions)
			Dim importer As New kCura.WinEDDS.ImportLoadFileProcess
			importOptions.SelectedNativeLoadFile.SourceFileEncoding = importOptions.SourceFileEncoding
			importOptions.SelectedNativeLoadFile.ExtractedTextFileEncoding = importOptions.ExtractedTextFileEncoding
			importOptions.SelectedNativeLoadFile.SelectedCasePath = importOptions.SelectedCasePath
			importOptions.SelectedNativeLoadFile.CopyFilesToDocumentRepository = importOptions.CopyFilesToDocumentRepository
			importOptions.SelectedNativeLoadFile.DestinationFolderID = importOptions.DestinationFolderID
			importOptions.SelectedNativeLoadFile.StartLineNumber = importOptions.StartLineNumber
			importer.LoadFile = importOptions.SelectedNativeLoadFile
			importer.TimeZoneOffset = _application.TimeZoneOffset
			importer.BulkLoadFileFieldDelimiter = Config.BulkLoadFileFieldDelimiter
			importer.CloudInstance = Config.CloudInstance
			importer.ExecutionSource = Relativity.ExecutionSource.Rdc
			_application.SetWorkingDirectory(importOptions.SelectedNativeLoadFile.FilePath)
			Dim executor As New kCura.EDDS.WinForm.CommandLineProcessRunner(importer.ProcessObserver, importer.ProcessController, importOptions.ErrorLoadFileLocation, importOptions.ErrorReportFileLocation)
			_application.StartProcess(importer)
		End Sub

		Friend Sub RunNativeImport(ByVal importOptions As ImportOptions)
			If EnsureEncoding(importOptions) Then
				Dim folderManager As New kCura.WinEDDS.Service.FolderManager(_application.GetCredentials, _application.CookieContainer)
				If folderManager.Exists(importOptions.SelectedCaseInfo.ArtifactID, importOptions.SelectedCaseInfo.RootFolderID) Then
					Dim importer As New kCura.WinEDDS.ImportLoadFileProcess
					importOptions.SelectedNativeLoadFile.SourceFileEncoding = importOptions.SourceFileEncoding
					importOptions.SelectedNativeLoadFile.ExtractedTextFileEncoding = importOptions.ExtractedTextFileEncoding
					importOptions.SelectedNativeLoadFile.SelectedCasePath = importOptions.SelectedCasePath
					importOptions.SelectedNativeLoadFile.CopyFilesToDocumentRepository = importOptions.CopyFilesToDocumentRepository
					importOptions.SelectedNativeLoadFile.DestinationFolderID = importOptions.DestinationFolderID
					importOptions.SelectedNativeLoadFile.StartLineNumber = importOptions.StartLineNumber
					importer.LoadFile = importOptions.SelectedNativeLoadFile
					importer.TimeZoneOffset = _application.TimeZoneOffset
					importer.BulkLoadFileFieldDelimiter = Config.BulkLoadFileFieldDelimiter
					importer.CloudInstance = Config.CloudInstance
					importer.ExecutionSource = Relativity.ExecutionSource.Rdc
					importOptions.SelectedNativeLoadFile.ArtifactTypeID = Relativity.ArtifactType.Document
					_application.SetWorkingDirectory(importOptions.SelectedNativeLoadFile.FilePath)
					Dim executor As New kCura.EDDS.WinForm.CommandLineProcessRunner(importer.ProcessObserver, importer.ProcessController, importOptions.ErrorLoadFileLocation, importOptions.ErrorReportFileLocation)
					_application.StartProcess(importer)
				End If
			Else
				Throw New Exceptions.EncodingMisMatchException(importOptions.SourceFileEncoding.CodePage, kCura.WinEDDS.Utility.DetectEncoding(importOptions.LoadFilePath, True).DeterminedEncoding.CodePage)
			End If
		End Sub

		Friend Sub RunImageImport(ByVal importOptions As ImportOptions)
			If EnsureEncoding(importOptions) Then
				Dim importer As New kCura.WinEDDS.ImportImageFileProcess
				importOptions.SelectedImageLoadFile.CookieContainer = _application.CookieContainer
				importOptions.SelectedImageLoadFile.Credential = _application.GetCredentials
				importOptions.SelectedImageLoadFile.SelectedCasePath = importOptions.SelectedCasePath
				importOptions.SelectedImageLoadFile.CaseDefaultPath = importOptions.SelectedCaseInfo.DocumentPath
				importOptions.SelectedImageLoadFile.CopyFilesToDocumentRepository = importOptions.CopyFilesToDocumentRepository
				importOptions.SelectedImageLoadFile.FullTextEncoding = importOptions.ExtractedTextFileEncoding
				importOptions.SelectedImageLoadFile.StartLineNumber = importOptions.StartLineNumber
				importer.ImageLoadFile = importOptions.SelectedImageLoadFile
				importer.CloudInstance = Config.CloudInstance
				importer.ExecutionSource = ExecutionSource.Rdc
				_application.SetWorkingDirectory(importOptions.SelectedImageLoadFile.FileName)
				Dim executor As New kCura.EDDS.WinForm.CommandLineProcessRunner(importer.ProcessObserver, importer.ProcessController, importOptions.ErrorLoadFileLocation, importOptions.ErrorReportFileLocation)
				_application.StartProcess(importer)
			Else
				Throw New Exceptions.EncodingMisMatchException(importOptions.SourceFileEncoding.CodePage, kCura.WinEDDS.Utility.DetectEncoding(importOptions.LoadFilePath, True).DeterminedEncoding.CodePage)
			End If
		End Sub

#End Region

	End Class

End Namespace

