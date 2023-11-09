Imports kCura.WinEDDS.Api
Imports Relativity.DataExchange.Io

Namespace kCura.WinEDDS.ImportExtension
	Public Class NativeFileValidated
		Implements IHasOixFileType

		Public Property FileTypeInfo As IFileTypeInfo
		Public Function GetFileTypeIdInfo() As IFileTypeInfo Implements Api.IHasOixFileType.GetFileTypeIdInfo
			Return Me.FileTypeInfo
		End Function
	End Class

	Public Class FileSizePopulated
		Implements IHasFileSize

		Public Property FileSize() As Long
		Public Function GetFileSize() As Long Implements Api.IHasFileSize.GetFileSize
			Return FileSize
		End Function
	End Class

	Public Class FileNamePopulated
		Implements IHasFileName
		Public Property FileName() As String
		Public Function GetFileName() As String Implements IHasFileName.GetFileName
			Return FileName
		End Function
	End Class

	Public Class MetadataFileIdPopulated
		Implements IHasMetadataFileId
		Public Property MetadataFileId() As String
		Public Function GetMetadataFileId() As String Implements IHasMetadataFileId.GetMetadataFileId
			Return MetadataFileId
		End Function
	End Class


	Public Class InjectableArtifactFieldCollection
		Inherits ArtifactFieldCollection
		Implements IInjectableFieldCollection

		Public ReadOnly Property FileName As IHasFileName Implements IInjectableFieldCollection.FileName
		Public ReadOnly Property FileSize As IHasFileSize Implements IInjectableFieldCollection.FileSize
		Public ReadOnly Property FileIdData As IHasOixFileType Implements IInjectableFieldCollection.FileIdInfo
        Public ReadOnly Property MetadataFileId As IHasMetadataFileId Implements IInjectableFieldCollection.MetadataFileId

		Private Sub New(fileName As String, fileTypeInfo As IFileTypeInfo, fileSize As Long?, metadataFileId as String)
			If (Not String.IsNullOrEmpty(fileName)) Then
				Me.FileName = New FileNamePopulated() With {.FileName = fileName}
			End If
			If (Not fileTypeInfo Is Nothing) Then
				Me.FileIdData = New NativeFileValidated() With {.FileTypeInfo = fileTypeInfo}
			End If
			If (Not fileSize Is Nothing) Then
				Me.FileSize = New FileSizePopulated() With {.FileSize = fileSize.GetValueOrDefault(0)}
			End If
		    If (Not String.IsNullOrEmpty(metadataFileId)) Then
		        Me.MetadataFileId = New MetadataFileIdPopulated() With {.MetadataFileId = metadataFileId}
		    End If
		End Sub

		Public Function HasFileName() As Boolean Implements IInjectableFieldCollection.HasFileName
			Return Not FileName Is Nothing
		End Function

		Public Function HasFileSize() As Boolean Implements IInjectableFieldCollection.HasFileSize
			Return Not FileSize Is Nothing
		End Function

		Public Function HasFileIdData() As Boolean Implements IInjectableFieldCollection.HasFileIdInfo
			Return Not FileIdData Is Nothing
		End Function
		Public Function HasMetadataFileId() As Boolean Implements IInjectableFieldCollection.HasMetadataFileId
			Return Not MetadataFileId Is Nothing
		End Function

		Public Shared Function CreateFieldCollection(fileName As String, fileTypeInfo As IFileTypeInfo, fileSize As Long?, metadataFileId As String) As ArtifactFieldCollection
			Dim possibleRetVal As InjectableArtifactFieldCollection = New InjectableArtifactFieldCollection(fileName, fileTypeInfo, fileSize, metadataFileId)
			If (possibleRetVal.HasFileName() OrElse  possibleRetVal.HasFileIdData() OrElse  possibleRetVal.HasFileSize() OrElse possibleRetVal.HasMetadataFileId()) Then
				Return possibleRetVal
			End If
			Return New ArtifactFieldCollection()
		End Function
	End Class
End Namespace