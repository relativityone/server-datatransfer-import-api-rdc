﻿Imports kCura.WinEDDS.Api
Imports Relativity.Import.Export

Namespace kCura.WinEDDS.ImportExtension
	Public Class NativeFileValidated
		Implements IHasOixFileType

		Public Property OixFileID As FileIdInfo
		Public Function GetFileIDData() As FileIdInfo Implements Api.IHasOixFileType.GetFileIdInfo
			Return Me.OixFileID
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


	Public Class InjectableArtifactFieldCollection
		Inherits ArtifactFieldCollection
		Implements IInjectableFieldCollection

		Public ReadOnly Property FileName As IHasFileName Implements IInjectableFieldCollection.FileName
		Public ReadOnly Property FileSize As IHasFileSize Implements IInjectableFieldCollection.FileSize
		Public ReadOnly Property FileIdData As IHasOixFileType Implements IInjectableFieldCollection.FileIdInfo

		Private Sub New(fileName As String, fileData As FileIdInfo, fileSize As Long?)
			If (Not String.IsNullOrEmpty(fileName)) Then
				Me.FileName = New FileNamePopulated() With {.FileName = fileName}
			End If
			If (Not fileData Is Nothing) Then
				Me.FileIdData = New NativeFileValidated() With {.OixFileID = fileData}
			End If
			If (Not fileSize Is Nothing) Then
				Me.FileSize = New FileSizePopulated() With {.FileSize = fileSize.GetValueOrDefault(0)}
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

		Public Shared Function CreateFieldCollection(fileName As String, fileData As FileIdInfo, fileSize As Long?) As ArtifactFieldCollection
			Dim possibleRetVal As InjectableArtifactFieldCollection = New InjectableArtifactFieldCollection(fileName, fileData, fileSize)
			If (possibleRetVal.HasFileName() Or possibleRetVal.HasFileIdData() Or possibleRetVal.HasFileSize()) Then
				Return possibleRetVal
			End If
			Return New ArtifactFieldCollection()
		End Function
	End Class
End Namespace