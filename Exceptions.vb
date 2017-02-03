Namespace kCura.EDDS.WinForm
	Public Class Exceptions
#Region " Exception "

		Public MustInherit Class RdcBaseException
			Inherits System.Exception
			Public Sub New(ByVal message As String)
				Me.New(message, Nothing)
			End Sub

			Protected Sub New(ByVal message As String, ByVal innerException As System.Exception)
				MyBase.New(message, innerException)
			End Sub
		End Class

		Public Class FlagException
			Inherits RdcBaseException
			Public Sub New(ByVal flag As String)
				MyBase.New(String.Format("Flag {0} is invalid", flag))
			End Sub
		End Class

		Public Class LoadFilePathException
			Inherits RdcBaseException
			Public Sub New()
				MyBase.New("No load file specified")
			End Sub

			Public Sub New(ByVal loadFilePath As String)
				MyBase.New(String.Format("The load file specified does not exist: " & loadFilePath))
			End Sub
		End Class

		Public Class SavedSettingsFilePathException
			Inherits RdcBaseException
			Public Sub New(ByVal loadFilePath As String)
				MyBase.New(String.Format("The saved settings file specified does not exist: " & loadFilePath))
			End Sub
		End Class

		Public Class CaseArtifactIdException
			Inherits RdcBaseException
			Public Sub New(ByVal caseArtifactID As String)
				Me.New(caseArtifactID, Nothing)
			End Sub

			Public Sub New(ByVal caseArtifactID As String, ByVal innerException As System.Exception)
				MyBase.New(String.Format("The case specified by the following ID does not exist: " & caseArtifactID), innerException)
			End Sub
		End Class

		Public Class UsernameException
			Inherits RdcBaseException
			Public Sub New()
				MyBase.New("Invalid or no username specified")
			End Sub
		End Class

		Public Class PasswordException
			Inherits RdcBaseException
			Public Sub New()
				MyBase.New("Invalid or no password specified")
			End Sub
		End Class

		Public Class ClientIDException
			Inherits RdcBaseException
			Public Sub New()
				MyBase.New("Invalid or no Client ID specified")
			End Sub
		End Class

		Public Class ClientSecretException
			Inherits RdcBaseException
			Public Sub New()
				MyBase.New("Invalid or no Client Secret specified")
			End Sub
		End Class

		Public Class MultipleCredentialException
			Inherits RdcBaseException

			Public Sub New()
				MyBase.New("Mutiple credentials specified.  Please only specify Username and Password or Client ID and Secret.")
			End Sub
		End Class

		Public Class ClientCrendentialsException
			Inherits RdcBaseException

			Public Sub New()
				MyBase.New("Invalid credentials specified. Please specify a valid ClientID and ClientSecret combination")
			End Sub
		End Class

		Public Class ConnectToIdentityServerException
			Inherits RdcBaseException

			Public Sub New()
				MyBase.New("Failed to connect to Identity server. Ensure your Identity server is running and accessible from this location.")
			End Sub
		End Class

		Public Class CredentialsException
			Inherits RdcBaseException
			Public Sub New()
				MyBase.New("Invalid credentials specified.  Please specify an active Relativity account's username and password.")
			End Sub
		End Class

		Public Class SavedSettingsRehydrationException
			Inherits RdcBaseException
			Public Sub New(ByVal path As String)
				Me.New(path, Nothing)
			End Sub

			Public Sub New(ByVal path As String, ByVal innerException As System.Exception)
				MyBase.New("The saved settings file specified is in an invalid format: " & path, innerException)
			End Sub
		End Class

		Public Class NoLoadTypeModeSetException
			Inherits RdcBaseException
			Public Sub New()
				MyBase.New("No load type or invalid load type set. Available options are ""image"", ""native"" or ""object""")
			End Sub
		End Class

		Public Class EncodingMisMatchException
			Inherits RdcBaseException
			Public Sub New(ByVal encoding As Int32, ByVal detectedEncoding As Int32)
				Me.New(encoding, detectedEncoding, Nothing)
			End Sub

			Public Sub New(ByVal encoding As Int32, ByVal detectedEncoding As Int32, ByVal innerException As System.Exception)
				MyBase.New(String.Format("The Encoding id - {0} - selected for your load file does not match the detected Encoding - {1}.  Please select the correct Encoding for your load file.", encoding, detectedEncoding, innerException))
			End Sub
		End Class

		Public Class EncodingException
			Inherits RdcBaseException
			Public Sub New(ByVal id As String, ByVal destination As String)
				Me.New(id, destination, Nothing)
			End Sub

			Public Sub New(ByVal id As String, ByVal destination As String, ByVal innerException As System.Exception)
				MyBase.New(String.Format("Invalid encoding set for {1}.  Encoding id '{0}' not supported.  Use -h:encoding for a list of supported encoding ids", id, destination), innerException)
			End Sub
		End Class

		Public Class FolderIdException
			Inherits RdcBaseException
			Public Sub New(ByVal id As String)
				MyBase.New(String.Format("There is no folder in the selected case with the id '{0}'", id))
			End Sub
		End Class

		Public Class InvalidPathLocationException
			Inherits RdcBaseException
			Public Sub New(ByVal path As String, ByVal type As String)
				Me.New(path, type, Nothing)
			End Sub

			Public Sub New(ByVal path As String, ByVal type As String, ByVal innerException As System.Exception)
				MyBase.New(String.Format("The {0} path {1} is invalid", type, path), innerException)
			End Sub
		End Class

		Public Class InvalidArtifactTypeException
			Inherits RdcBaseException
			Public Sub New(ByVal type As String)
				MyBase.New(String.Format("'{0}' is neither the name or ID of any dynamic object type in the system", type))
			End Sub
		End Class

		Public Class StartLineNumberException
			Inherits RdcBaseException
			Public Sub New(ByVal value As String)
				Me.New(value, Nothing)
			End Sub

			Public Sub New(ByVal value As String, ByVal innerException As System.Exception)
				MyBase.New(String.Format("The specified start line number is not valid: {0}.", value), innerException)
			End Sub
		End Class

		Public Class MustCopyFilesToRepositoryException
			Inherits RdcBaseException
			Public Sub New(ByVal path As String, ByVal type As String)
				Me.New()
			End Sub

			Public Sub New()
				MyBase.New("Files must be copied to the Repository in RelativityOne.")
			End Sub
		End Class

#End Region
	End Class
End Namespace
