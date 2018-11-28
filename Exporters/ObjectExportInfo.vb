Namespace kCura.WinEDDS.Exporters
	Public Class ObjectExportInfo
		Private Const _ORIGINAL_FILE_NAME_PART_SEPARATOR As String = "_"
		Private _docCount As Int32 = 1

#Region " Member Accessors "

		Public Property ArtifactID() As Int32
		Public Property FileID() As Int32
		Public Property HasCountedNative() As Boolean = False
		Public Property HasCountedTextFile() As Boolean = False
		Public Property HasFullText() As Boolean
		Public Property IdentifierValue() As String = String.Empty
		Public Property Images() As System.Collections.ArrayList
		Public Property Metadata() As Object()
		Public Property Native() As Object
		Public Property NativeExtension() As String = String.Empty
		Public Property NativeFileGuid() As String = String.Empty
		Public Property NativeSourceLocation() As String = String.Empty
		Public Property NativeTempLocation() As String = String.Empty
		Public Property OriginalFileName() As String = String.Empty
		Public Property ProductionBeginBates() As String = String.Empty
		Public Property TotalFileSize() As Int64
		Public Property TotalNumberOfFiles() As Int64
		Public Property DestinationVolume() As String = String.Empty
		Friend Property CoalescedProductionID As Int32? = Nothing

#End Region

#Region " Calculated Accessors "

		Public ReadOnly Property ProductionBeginBatesFileName(ByVal appendToOriginal As Boolean, tryProductionBegBates As Boolean) As String
			Get
				Dim retval As String
				If tryProductionBegBates AndAlso String.IsNullOrWhiteSpace(ProductionBeginBates) Then
					retval = NativeFileName(appendToOriginal)
				ElseIf appendToOriginal Then
					retval = ProductionBeginBates & _ORIGINAL_FILE_NAME_PART_SEPARATOR & OriginalFileName
				Else
					If Not NativeExtension = "" Then
						retval = ProductionBeginBates & "." & NativeExtension
					Else
						retval = ProductionBeginBates
					End If
				End If
				Return kCura.Utility.File.Instance.ConvertIllegalCharactersInFilename(retval)
			End Get
		End Property

		Public ReadOnly Property NativeCount() As Int64
			Get
				If Me.NativeFileGuid = "" Then
					If Not Me.FileID = Nothing Or Me.FileID <> 0 Then
						Return 1
					Else
						Return 0
					End If
				Else
					Return 1
				End If
			End Get
		End Property

		Public ReadOnly Property ImageCount() As Int64
			Get
				If Me.Images Is Nothing Then Return 0
				Return Me.Images.Count
			End Get
		End Property

		Public ReadOnly Property DocCount() As Int32
			Get
				Dim retval As Int32 = _docCount
				If retval = 1 Then _docCount -= 1
				Return retval
			End Get
		End Property

#End Region

		Public Function NativeFileName(ByVal appendToOriginal As Boolean) As String
			Dim retval As String
			If appendToOriginal Then
				retval = IdentifierValue & _ORIGINAL_FILE_NAME_PART_SEPARATOR & OriginalFileName
			Else
				If Not NativeExtension = "" Then
					retval = IdentifierValue & "." & NativeExtension
				Else
					retval = IdentifierValue
				End If
			End If
			Return kCura.Utility.File.Instance.ConvertIllegalCharactersInFilename(retval)
		End Function

		Public Function FullTextFileName(ByVal nameFilesAfterIdentifier As Boolean, tryProductionBegBates As Boolean) As String
			Return FullTextFileName(nameFilesAfterIdentifier, tryProductionBegBates, False)
		End Function

		Public Function FullTextFileName(ByVal nameFilesAfterIdentifier As Boolean, tryProductionBegBates As Boolean, appendOriginalFilename As Boolean) As String
			Dim retval As String = GetFullTextIdentifierFileNamePart(nameFilesAfterIdentifier, tryProductionBegBates)

			If appendOriginalFilename Then
				retval = retval & _ORIGINAL_FILE_NAME_PART_SEPARATOR & OriginalFileName
			End If

			retval = kCura.Utility.File.Instance.ConvertIllegalCharactersInFilename(retval)
			retval = AppendExtensionToFullTextFileName(retval)
			Return retval
		End Function

		Private Function AppendExtensionToFullTextFileName(retval As String) As String
			Return FileNameHelper.AppendExtensionToFileWhenMissing(retval, ".txt")
		End Function

		Private Function GetFullTextIdentifierFileNamePart(nameFilesAfterIdentifier As Boolean, tryProductionBegBates As Boolean) As String
			Dim retval As String
			If tryProductionBegBates Then
				retval = If(String.IsNullOrWhiteSpace(ProductionBeginBates), IdentifierValue, ProductionBeginBates)
			ElseIf Not nameFilesAfterIdentifier Then
				retval = Me.ProductionBeginBates
			Else
				retval = Me.IdentifierValue
			End If

			Return If(retval, String.Empty)
		End Function
	End Class
End Namespace