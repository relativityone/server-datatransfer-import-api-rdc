Namespace kCura.WinEDDS.ImportExtension
    Public Class QueryReaderInitializationArgs

        Private _allFields As kCura.WinEDDS.Api.ArtifactFieldCollection
        Private _artifactTypeID As Int32

        Public ReadOnly Property AllFields() As kCura.WinEDDS.Api.ArtifactFieldCollection
            Get
                Return _allFields
            End Get
        End Property

        Public ReadOnly Property ArtifactTypeID() As Int32
            Get
                Return _artifactTypeID
            End Get
        End Property

        Public Sub New(ByVal allFields As kCura.WinEDDS.Api.ArtifactFieldCollection, ByVal artifactTypeID As Int32)
            _allFields = allFields
            _artifactTypeID = artifactTypeID
        End Sub

    End Class
End Namespace

