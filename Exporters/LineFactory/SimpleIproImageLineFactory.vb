Imports System.Collections.Concurrent

Namespace kCura.WinEDDS.Exporters.LineFactory
	Public Class SimpleIproImageLineFactory
		Inherits ImageLineFactoryBase

#Region "Constructors"

		Public Sub New(ByVal batesNumber As String, ByVal pageNumber As Int32, ByVal fullFilePath As String, ByVal volumeName As String, ByVal imageType As ExportFile.ImageType)
			MyBase.New(batesNumber, pageNumber, fullFilePath, volumeName, imageType)
		End Sub

#End Region

#Region "Cell Contents"

		Private ReadOnly Property ImportCodeIdentifier() As String
			Get
				Return "IM"
			End Get
		End Property

		Private ReadOnly Property ImageKey() As String
			Get
				Return Me.BatesNumber
			End Get
		End Property

		Private ReadOnly Property DocumentDesignation() As String
			Get
				If Me.PageNumber = 1 Then
					Return "D"
				Else
					Return ""
				End If
			End Get
		End Property

		Private ReadOnly Property TiffFileOffset() As String
			Get
				Select Case Me.ImageType
					Case ExportFile.ImageType.MultiPageTiff
						Return Me.PageNumber.ToString
					Case ExportFile.ImageType.Pdf
						Return Me.PageNumber.ToString
					Case ExportFile.ImageType.SinglePage
						Return 0.ToString
				End Select
				Return Nothing
			End Get
		End Property

		Private ReadOnly Property VolumeIdentifier() As String
			Get
				Return "@" & Me.VolumeName
			End Get
		End Property

		Private ReadOnly Property DirectoryPath() As String
			Get
				Return System.IO.Path.GetDirectoryName(Me.FullFilePath)
			End Get
		End Property

		Private ReadOnly Property Filename() As String
			Get
				Return System.IO.Path.GetFileName(Me.FullFilePath)
			End Get
		End Property

		Private ReadOnly Property IproImageFileType() As String
			Get
				Select Case System.IO.Path.GetExtension(Me.FullFilePath).ToLower.Trim(".".ToCharArray)
					Case "pdf"
						Return 7.ToString
					Case "jpg", "jpeg"
						Return 4.ToString
					Case "tif", "tiff"
						Return 2.ToString
				End Select
				Return Nothing
			End Get
		End Property

#End Region

#Region "Virtual Method Implementation"

		Public Overrides Sub WriteLine(ByVal stream As System.IO.StreamWriter, ByVal linesToWriteOpt As ConcurrentDictionary(Of String, String))
			Dim lineToWrite As New System.Text.StringBuilder
			lineToWrite.Append(Me.ImportCodeIdentifier)
			lineToWrite.Append(",")
			lineToWrite.Append(Me.ImageKey)
			lineToWrite.Append(",")
			lineToWrite.Append(Me.DocumentDesignation)
			lineToWrite.Append(",")
			lineToWrite.Append(Me.TiffFileOffset)
			lineToWrite.Append(",")
			lineToWrite.Append(Me.VolumeIdentifier)
			lineToWrite.Append(";")
			lineToWrite.Append(Me.DirectoryPath)
			lineToWrite.Append(";")
			lineToWrite.Append(Me.Filename)
			lineToWrite.Append(";")
			lineToWrite.Append(Me.IproImageFileType)
			lineToWrite.Append(vbNewLine)
			linesToWriteOpt.TryAdd(Me.ImageKey, lineToWrite.ToString)
		End Sub

#End Region

	End Class
End Namespace

