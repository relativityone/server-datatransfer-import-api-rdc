Imports System.IO

Namespace kCura.WinEDDS.LoadFileEntry
	Public Interface ILongTextEntryWriter
		'TODO change name, because it's being used for field value also
		Sub WriteLongTextFileToDatFile(fileWriter As StreamWriter, longTextPath As String, encoding As Text.Encoding)
	End Interface
End Namespace

