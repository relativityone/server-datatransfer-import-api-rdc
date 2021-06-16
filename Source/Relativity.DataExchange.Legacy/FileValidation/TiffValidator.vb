Imports System.IO

Namespace kCura.WinEDDS
	
	Public Class TiffValidator
		Implements ITiffValidator

		Delegate Function ReadShort(rdr As BinaryReader) As Short
		Delegate Function ReadLong(rdr As BinaryReader) As Long

		Public Function ValidateTiffTags(ByVal filePath As String) As ImageValidationResult Implements ITiffValidator.ValidateTiffTags
			Dim readshort As New ReadShort(AddressOf ReadLittleEndianShort)
			Dim readlong As New ReadLong(AddressOf ReadLittleEndianLong)

			' think default is 1 if none found
			Using rdr As New BinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				Dim bom As Integer = rdr.ReadInt16()
				If bom = &H4d4d Then
					readshort = AddressOf ReadBigEndianShort
					readlong = AddressOf ReadBigEndianLong
				End If

				' skip 2 bytes
				rdr.ReadInt16()
				Dim tagsOffset As Long = readlong(rdr)
				rdr.BaseStream.Position = tagsOffset

				Dim numEntries As Short = readshort(rdr)
				For i As Short = 0 To CShort(numEntries - 1)
					Dim bitPerPixel As Integer
					Dim newPosition As Long = rdr.BaseStream.Position + 12
					Dim tagID As Short = readshort(rdr)

					If tagID = 258 Then
						Dim varType As Integer = readshort(rdr)

						If varType <> 3 Then
							Return New ImageValidationResult(False, $"The TIFF image file {filePath} tag {tagID} has type that is not SHORT")
						End If

						Dim samplePerPixel As Long = readlong(rdr)

						If samplePerPixel > 1 Then
							rdr.BaseStream.Position = readlong(rdr)
							bitPerPixel = 0
							For j As Integer = 0 To CInt(samplePerPixel - 1)
								Dim bitPerSample As Integer = CInt(readshort(rdr))
								bitPerPixel += bitPerSample
							Next
						Else
							bitPerPixel = CInt(readshort(rdr))
						End If

						If bitPerPixel <> 1 Then
							Return New ImageValidationResult(False, $"The TIFF image file {filePath} is {bitPerPixel} bits. Only 1-bit TIFFs supported")
						End If

					ElseIf tagID = 259 Then
						Dim compression As Integer
						Dim varType As Integer = readshort(rdr)

						If varType <> 3 Then
							Return New ImageValidationResult(False, $"The TIFF image file {filePath} tag {tagID} has type that is not SHORT")
						End If

						rdr.BaseStream.Position = rdr.BaseStream.Position + 4

						compression = CInt(readshort(rdr))
						
						If compression <> 4 Then
							Return New ImageValidationResult(False, $"The TIFF image file {filePath} is not encoded in CCITT T.6")
						End If
					End If

					rdr.BaseStream.Position = newPosition
				Next
			End Using

			Return New ImageValidationResult(True, $"The TIFF image file {filePath} is valid")
		End Function

		Private Function ReadLittleEndianShort(rdr As BinaryReader) As Short
			Return rdr.ReadInt16()
		End Function
		Private Function ReadBigEndianShort(rdr As BinaryReader) As Short
			Dim s As Integer = CInt(rdr.ReadByte())
			s = (s << 8) Or rdr.ReadByte()
			Return CShort(s)
		End Function
		
		Private Function ReadLittleEndianLong(rdr As BinaryReader) As Long
			Return rdr.ReadInt32()
		End Function
		Private Function ReadBigEndianLong(rdr As BinaryReader) As Long
			Dim s As Long = CLng(rdr.ReadByte())
			s = (s << 8) Or rdr.ReadByte()
			s = (s << 8) Or rdr.ReadByte()
			s = (s << 8) Or rdr.ReadByte()
			Return s
		End Function
	End Class

End Namespace