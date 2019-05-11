﻿Imports Relativity.DataExchange.Io

Namespace kCura.WinEDDS.Api
	Friend Class ExtendedFileTypeInfo
		Inherits FileTypeInfo
		Implements IHasSupportedByViewer

		Private ReadOnly _supportedByViewer As Boolean

		Public Sub New(fileType As String, supportedByViewer As Boolean)
			MyBase.new(0, fileType)
			_supportedByViewer = supportedByViewer
		End Sub

		Public Function SupportedByViewer() As Boolean Implements IHasSupportedByViewer.SupportedByViewer
			Return _supportedByViewer
		End Function
	End Class
End NameSpace