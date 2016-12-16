﻿Imports kCura.EDDS.WinForm

Friend Class WinEDDSDTO

	'Moved from Startup.Vb to here
	Friend _loadFilePath As String
	Friend SelectedCaseInfo As Relativity.CaseInfo
	Friend SelectedNativeLoadFile As New kCura.WinEDDS.LoadFile
	Friend SelectedImageLoadFile As kCura.WinEDDS.ImageLoadFile
	Friend HasSetLoadFileLocation As Boolean = False
	Friend HasSetCaseInfo As Boolean = False
	Private HasSetLoadMode As Boolean = False
	Friend SourceFileEncoding As System.Text.Encoding
	Friend ExtractedTextFileEncoding As System.Text.Encoding
	Friend SelectedCasePath As String = ""
	Friend CopyFilesToDocumentRepository As Boolean = True
	Friend DestinationFolderID As Int32
	Private RunningDirectory As String = System.IO.Directory.GetCurrentDirectory
	Private ExportErrorReportFile As Boolean = False
	Private ExportErrorLoadFile As Boolean = False
	Friend ErrorReportFileLocation As String = ""
	Friend ErrorLoadFileLocation As String = ""
	Friend StartLineNumber As Int64

#Region " Input Validation "

	Friend Sub HandleConsoleAuthErrors(username As String, password As String, clientID As String, clientSecret As String)
		Dim usernameExists As Boolean = Not String.IsNullOrEmpty(username)
		Dim passwordExists As Boolean = Not String.IsNullOrEmpty(password)
		Dim clientIDExists As Boolean = Not String.IsNullOrEmpty(clientID)
		Dim clientSecretExists As Boolean = Not String.IsNullOrEmpty(clientSecret)

		If (usernameExists Or passwordExists) AndAlso (clientIDExists Or clientSecretExists) Then
			Throw New MultipleCredentialException
		End If

		If Not clientIDExists AndAlso Not clientSecretExists Then
			If Not usernameExists Then Throw New UsernameException
			If Not passwordExists Then Throw New PasswordException
		Else
			If Not clientIDExists Then Throw New ClientIDException
			If Not clientSecretExists Then Throw New ClientSecretException
		End If
	End Sub

	Friend Sub EnsureLoadFileLocation()
		If String.IsNullOrEmpty(_loadFilePath) Then Throw New LoadFilePathException
		If Not System.IO.File.Exists(_loadFilePath) Then Throw New LoadFilePathException(_loadFilePath)
	End Sub

	Friend Function EnsureEncoding() As Boolean
		Dim determinedEncoding As System.Text.Encoding = kCura.WinEDDS.Utility.DetectEncoding(_loadFilePath, True).DeterminedEncoding
		If determinedEncoding Is Nothing Then Return True
		Return (determinedEncoding.Equals(SourceFileEncoding))
	End Function

	Friend Sub SetFileLocation(ByVal path As String)
		_loadFilePath = path
		HasSetLoadFileLocation = Not String.IsNullOrEmpty(_loadFilePath)
	End Sub

	Friend Sub SetCaseInfo(ByVal caseID As String, ByRef application As kCura.EDDS.WinForm.Application)
		Try
			Dim caseManager As New kCura.WinEDDS.Service.CaseManager(application.Credential, application.CookieContainer)
			SelectedCaseInfo = caseManager.Read(Int32.Parse(caseID))
		Catch ex As Exception
			Throw New CaseArtifactIdException(caseID, ex)
		End Try
		If SelectedCaseInfo Is Nothing Then Throw New CaseArtifactIdException(caseID)
		application.RefreshSelectedCaseInfo(SelectedCaseInfo)
		HasSetCaseInfo = True
	End Sub

	Friend Sub SetExportErrorReportLocation(ByVal commandLine As kCura.CommandLine.CommandList)
		For Each command As kCura.CommandLine.Command In commandLine
			If command.Directive.ToLower.Replace("-", "").Replace("/", "") = "er" Then
				ExportErrorReportFile = True
				If command.Value Is Nothing OrElse command.Value = "" Then
					ErrorReportFileLocation = RunningDirectory.TrimEnd("\".ToCharArray) & "\ErrorReport_" & System.DateTime.Now.Ticks.ToString & ".csv"
				Else
					ErrorReportFileLocation = String.Copy(command.Value)
				End If
				Try
					If Not System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(ErrorReportFileLocation)) Then System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(ErrorReportFileLocation))
					System.IO.File.Create(ErrorReportFileLocation).Close()
					System.IO.File.Delete(ErrorReportFileLocation)
				Catch ex As Exception
					Throw New InvalidPathLocationException(ErrorReportFileLocation, "error report", ex)
				End Try
				'Return command.Value
			End If
		Next
		If Not ExportErrorReportFile Then ErrorReportFileLocation = ""
	End Sub

	Friend Sub SetExportErrorFileLocation(ByVal commandLine As kCura.CommandLine.CommandList)
		For Each command As kCura.CommandLine.Command In commandLine
			If command.Directive.ToLower.Replace("-", "").Replace("/", "") = "ef" Then
				ExportErrorLoadFile = True
				If command.Value Is Nothing OrElse command.Value = "" Then
					Dim extension As String = System.IO.Path.GetExtension(_loadFilePath)
					If extension Is Nothing Then extension = "txt"
					extension = extension.TrimStart(".".ToCharArray)
					ErrorLoadFileLocation = RunningDirectory.TrimEnd("\".ToCharArray) & "\ErrorLines_" & System.DateTime.Now.Ticks.ToString & "." & extension
				Else
					ErrorLoadFileLocation = String.Copy(command.Value)
				End If
				Try
					If Not System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(ErrorLoadFileLocation)) Then System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(ErrorLoadFileLocation))
					System.IO.File.Create(ErrorLoadFileLocation).Close()
					System.IO.File.Delete(ErrorLoadFileLocation)
				Catch ex As Exception
					Throw New InvalidPathLocationException(ErrorReportFileLocation, "error file", ex)
				End Try
				'Return command.Value
			End If
		Next
		If Not ExportErrorLoadFile Then ErrorLoadFileLocation = ""
	End Sub

	Friend Sub SetSavedMapLocation(ByVal path As String, ByVal currentLoadMode As LoadMode, ByRef application As kCura.EDDS.WinForm.Application)
		Try
			Select Case currentLoadMode
				Case LoadMode.Image
					If Not System.IO.File.Exists(path) Then Throw New SavedSettingsFilePathException(path)
					SelectedImageLoadFile = application.ReadImageLoadFile(path)
					If HasSetLoadFileLocation Then SelectedImageLoadFile.FileName = _loadFilePath
				Case LoadMode.Native, LoadMode.DynamicObject
					If Not System.IO.File.Exists(path) Then Throw New SavedSettingsFilePathException(path)
					Dim sr As New System.IO.StreamReader(path)
					Dim doc As String = sr.ReadToEnd
					sr.Close()
					Dim xmlDoc As New System.Xml.XmlDocument
					xmlDoc.LoadXml(doc)

					sr = New System.IO.StreamReader(path)
					Dim stringr As New System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(application.CleanLoadFile(xmlDoc)))
					Dim tempLoadFile As LoadFile
					Dim deserializer As New System.Runtime.Serialization.Formatters.Soap.SoapFormatter
					tempLoadFile = DirectCast(deserializer.Deserialize(stringr), LoadFile)
					sr.Close()
					If Not String.IsNullOrEmpty(_loadFilePath) Then
						tempLoadFile.FilePath = _loadFilePath
					Else
						_loadFilePath = tempLoadFile.FilePath
					End If
					tempLoadFile.OverwriteDestination = Utility.ConvertLegacyOverwriteDestinationValueToEnum(tempLoadFile.OverwriteDestination)
					tempLoadFile.ForceFolderPreview = False
					tempLoadFile.CaseInfo = SelectedCaseInfo
					tempLoadFile.CopyFilesToDocumentRepository = True           'LoadFile.CopyFilesToDocumentRepository
					tempLoadFile.SelectedCasePath = SelectedCaseInfo.DocumentPath           ''''''''''
					tempLoadFile.CaseDefaultPath = SelectedCaseInfo.DocumentPath
					tempLoadFile.Credentials = application.Credential
					tempLoadFile.DestinationFolderID = 35
					tempLoadFile.ExtractedTextFileEncoding = System.Text.Encoding.Unicode
					tempLoadFile.SourceFileEncoding = System.Text.Encoding.Default
					'TODO: Have ArtifactTypeID be passed in on command line, currently hardcoding to 10
					Dim artifactTypeID As Int32
					If currentLoadMode = LoadMode.Native Then
						artifactTypeID = Relativity.ArtifactType.Document
					Else
						artifactTypeID = tempLoadFile.ArtifactTypeID
					End If
					tempLoadFile.SelectedIdentifierField = application.CurrentFields(artifactTypeID, True).IdentifierFields(0)
					Dim mapItemToRemove As LoadFileFieldMap.LoadFileFieldMapItem = Nothing
					If tempLoadFile.GroupIdentifierColumn = "" AndAlso System.IO.File.Exists(tempLoadFile.FilePath) Then
						Dim fieldMapItem As kCura.WinEDDS.LoadFileFieldMap.LoadFileFieldMapItem
						For Each fieldMapItem In tempLoadFile.FieldMap
							If Not fieldMapItem.DocumentField Is Nothing AndAlso
							 fieldMapItem.NativeFileColumnIndex >= 0 AndAlso
							 fieldMapItem.DocumentField.FieldName.ToLower = "group identifier" Then
								tempLoadFile.GroupIdentifierColumn = application.GetColumnHeadersFromLoadFile(tempLoadFile, tempLoadFile.FirstLineContainsHeaders)(fieldMapItem.NativeFileColumnIndex)
								'mapItemToRemove = fieldMapItem
							End If
						Next
					End If
					If Not mapItemToRemove Is Nothing Then tempLoadFile.FieldMap.Remove(mapItemToRemove)

					For Each fieldMapItem As kCura.WinEDDS.LoadFileFieldMap.LoadFileFieldMapItem In tempLoadFile.FieldMap
						If Not fieldMapItem.DocumentField Is Nothing Then
							Try
								Dim thisField As DocumentField = application.CurrentFields(tempLoadFile.ArtifactTypeID).Item(fieldMapItem.DocumentField.FieldID)
								fieldMapItem.DocumentField.AssociatedObjectTypeID = thisField.AssociatedObjectTypeID
								fieldMapItem.DocumentField.UseUnicode = thisField.UseUnicode
								fieldMapItem.DocumentField.CodeTypeID = thisField.CodeTypeID
								fieldMapItem.DocumentField.FieldLength = thisField.FieldLength
								fieldMapItem.DocumentField.ImportBehavior = thisField.ImportBehavior
							Catch
							End Try
						End If
					Next
					SelectedNativeLoadFile = tempLoadFile
					SelectedNativeLoadFile.SelectedCasePath = SelectedCaseInfo.DocumentPath
					SelectedNativeLoadFile.Credentials = application.Credential
					SelectedNativeLoadFile.CookieContainer = application.CookieContainer
				Case LoadMode.Application
					If System.IO.File.Exists(path) Then Throw New InvalidOperationException("Load file is not supported for application imports")
			End Select
		Catch ex As Exception
			Throw New SavedSettingsRehydrationException(path, ex)
		End Try
	End Sub

	Friend Sub SetSourceFileEncoding(ByVal value As String)
		If value Is Nothing OrElse value = "" Then
			SourceFileEncoding = System.Text.Encoding.Default
		Else
			Try
				SourceFileEncoding = System.Text.Encoding.GetEncoding(Int32.Parse(value))
			Catch ex As Exception
				Throw New EncodingException(value, "source file", ex)
			End Try
		End If
	End Sub
	Friend Sub SetFullTextFileEncoding(ByVal value As String)
		If value Is Nothing OrElse value = "" Then
			ExtractedTextFileEncoding = System.Text.Encoding.Default
		Else
			Try
				ExtractedTextFileEncoding = System.Text.Encoding.GetEncoding(Int32.Parse(value))
			Catch ex As Exception
				Throw New EncodingException(value, "extracted text files", ex)
			End Try
		End If
	End Sub
	Friend Sub SetSelectedCasePath(ByVal value As String)
		If value Is Nothing OrElse value = "" Then
			SelectedCasePath = SelectedCaseInfo.DocumentPath
		Else
			SelectedCasePath = value.TrimEnd("\"c) & "\"
		End If
	End Sub
	Friend Sub SetCopyFilesToDocumentRepository(ByVal value As String)
		If value Is Nothing OrElse value = "" Then
			CopyFilesToDocumentRepository = True
		Else
			CopyFilesToDocumentRepository = True
			Try
				CopyFilesToDocumentRepository = Boolean.Parse(value)
			Catch ex As Exception
				Select Case value.ToLower.Substring(0, 1)
					Case "t", "y", "1"
						CopyFilesToDocumentRepository = True
					Case "f", "n", "0"
						CopyFilesToDocumentRepository = False
				End Select
				If value.ToLower = "on" Then
					CopyFilesToDocumentRepository = True
				ElseIf value.ToLower = "off" Then
					CopyFilesToDocumentRepository = False
				End If
			End Try
		End If
	End Sub

	Friend Sub SetDestinationFolderID(ByVal value As String, ByRef application As kCura.EDDS.WinForm.Application)
		If value Is Nothing OrElse value = "" Then
			DestinationFolderID = SelectedCaseInfo.RootFolderID
		Else
			Dim folderManager As New kCura.WinEDDS.Service.FolderManager(application.Credential, application.CookieContainer)
			Dim folderExists As Boolean = False
			Try
				folderExists = folderManager.Exists(SelectedCaseInfo.ArtifactID, Int32.Parse(value))
			Catch
			End Try
			If folderExists Then
				DestinationFolderID = Int32.Parse(value)
			Else
				Throw New FolderIdException(value)
			End If
		End If

	End Sub

	Friend Function SetLoadType(ByVal loadTypeString As String) As LoadMode
		Dim currentLoadMode As LoadMode
		Select Case loadTypeString.ToLower.Trim
			Case "i", "image"
				currentLoadMode = LoadMode.Image
				HasSetLoadMode = True
			Case "n", "native"
				currentLoadMode = LoadMode.Native
				HasSetLoadMode = True
			Case "o", "object"
				currentLoadMode = LoadMode.DynamicObject
				HasSetLoadMode = True
			Case "a", "application"
				currentLoadMode = LoadMode.Application
				HasSetLoadMode = True
		End Select
		If Not HasSetLoadMode Then Throw New NoLoadTypeModeSetException
		Return currentLoadMode
	End Function

	Friend Sub SetStartLineNumber(ByVal value As String)
		If value Is Nothing OrElse value = "" Then
			StartLineNumber = 0
		Else
			Try
				StartLineNumber = Int64.Parse(value)
				If StartLineNumber < 0 Then
					Throw New StartLineNumberException(value)
				End If
			Catch ex As Exception
				Throw New StartLineNumberException(value, ex)
			End Try
		End If
	End Sub

#End Region

End Class

