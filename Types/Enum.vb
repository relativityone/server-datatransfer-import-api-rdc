Namespace kCura.WinEDDS

	''' <summary>
	''' Specifies the mode for importing files.
	''' </summary>
	Public Enum OverwriteModeEnum
		''' <summary>
		''' Import all files, even if this causes duplication. Faster than Append/Overlay mode.
		''' </summary>
		Append
		''' <summary>
		''' Update all files if new versions are made available by the import.
		''' </summary>
		Overlay
		''' <summary>
		''' Import all files.  Those that are duplicates will be updated to the new version of the file.
		''' </summary>
		AppendOverlay
	End Enum

	''' <summary>
	''' Specifies what to do about the native files when importing.
	''' </summary>
	Public Enum NativeFileCopyModeEnum
		''' <summary>
		''' Import documents without their native files.
		''' </summary>
		DoNotImportNativeFiles
		''' <summary>
		''' Copy native files into the workspace.
		''' </summary>
		CopyFiles
		''' <summary>
		''' Link to the native files but do not copy them.
		''' </summary>
		SetFileLinks
	End Enum

End Namespace