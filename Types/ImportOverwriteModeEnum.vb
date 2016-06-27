Namespace kCura.WinEDDS

	''' <summary>
	''' Specifies the mode for importing files.
	''' These values come from kCura.Relativity.DataReaderClient.OverwriteModeEnum, but it is not referenced to prevent circular dependencies.
	''' </summary>
	Public Enum ImportOverwriteModeEnum
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

End Namespace