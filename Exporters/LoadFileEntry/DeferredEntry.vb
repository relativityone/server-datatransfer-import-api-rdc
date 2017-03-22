Imports System.IO
Imports System.IO.Ports

Namespace kCura.WinEDDS.LoadFileEntry

	#Region "PartialEntry"

	Public Interface IPartialEntry
		Sub Write(ByRef fileWriter As System.IO.StreamWriter)
	End Interface

	Public Class StringPartialEntry
		Implements IPartialEntry
		Private ReadOnly _partialEntry As String

		Public Sub New(ByVal str As String)
			_partialEntry = str
		End Sub

		Public Sub Write(ByRef fileWriter As StreamWriter) Implements IPartialEntry.Write
			fileWriter.Write(_partialEntry)
		End Sub
	End Class

	Public Class LongTextWriteDeferredEntry
		Implements IPartialEntry
		Private ReadOnly _longTextPath As String
		Private ReadOnly _encoding As System.Text.Encoding
		Private ReadOnly _volumeManager As VolumeManager

		Public Sub New(ByVal longTextPath As String, ByVal encoding As System.Text.Encoding, ByVal volumeManager As VolumeManager)
			_longTextPath = longTextPath
			_encoding = encoding
			_volumeManager = volumeManager
		End Sub

		Public Sub Write(ByRef fileWriter As System.IO.StreamWriter) Implements IPartialEntry.Write
			_volumeManager.WriteLongTextFileToDatFile(fileWriter, _longTextPath, _encoding)
		End Sub
	End Class

	#End Region

	Public Class DeferredEntry
		Implements ILoadFileEntry
		Private ReadOnly _partialEntryList As ArrayList

		Public Sub New()
			_partialEntryList = New ArrayList()
		End Sub

		Public Sub AddStringEntry(ByVal partialEntry As String)
			_partialEntryList.Add(New StringPartialEntry(partialEntry))
		End Sub

		Public Sub AddPartialEntry(ByVal partialEntry As IPartialEntry)
			_partialEntryList.Add(partialEntry)
		End Sub

		Public Sub Write(ByRef fileWriter As System.IO.StreamWriter) Implements ILoadFileEntry.Write
			For Each partialEntry As IPartialEntry In _partialEntryList
				partialEntry.Write(fileWriter)
			Next
		End Sub
	End Class
End Namespace