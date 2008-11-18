Namespace kCura.WinEDDS
  Public Class ObjectTypeListItem

    Public Value As Int32
    Public Display As String

    Public Overrides Function ToString() As String
      Return Me.Display
    End Function

    Public Sub New(ByVal v As Int32, ByVal d As String)
      Me.Value = v
      Me.Display = d
    End Sub
  End Class
End Namespace