Imports System.Collections.Generic

Namespace kCura.WinEDDS.FileNaming.CustomFileNaming
	Public Interface INativeFileNameViewFieldsHelper

		Function FilterFields(exportFile As ExportFile, fieldsIds As List(Of Integer)) As ViewFieldInfo()
		Sub PopulateNativeFileNameViewFields(ExportSettings As ExtendedExportFile)
	End Interface
End Namespace

