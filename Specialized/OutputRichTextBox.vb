' TODO : Change namespace of this control
Namespace kCura.Windows.Forms
  Public Class OutputRichTextBox
    Inherits System.Windows.Forms.UserControl

#Region " Windows Form Designer generated code "

    Public Sub New()
      MyBase.New()

      'This call is required by the Windows Form Designer.
      InitializeComponent()

			'Add any initialization after the InitializeComponent() call
			ExtraSpace = ""
		End Sub

		'UserControl overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
      If disposing Then
        If Not (components Is Nothing) Then
          components.Dispose()
        End If
      End If
      MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents TextBox As System.Windows.Forms.TextBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
      Me.TextBox = New System.Windows.Forms.TextBox
      Me.SuspendLayout()
      '
      'TextBox
      '
      Me.TextBox.Dock = System.Windows.Forms.DockStyle.Fill
      Me.TextBox.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.TextBox.Location = New System.Drawing.Point(0, 0)
      Me.TextBox.Multiline = True
      Me.TextBox.Name = "TextBox"
      Me.TextBox.Size = New System.Drawing.Size(150, 144)
      Me.TextBox.TabIndex = 0
      Me.TextBox.Text = ""
      '
      'OutputRichTextBox
      '
      Me.Controls.Add(Me.TextBox)
      Me.Name = "OutputRichTextBox"
      Me.Size = New System.Drawing.Size(150, 144)
      Me.ResumeLayout(False)

    End Sub

#End Region

		Public ReadOnly Property DisplayTextBox As System.Windows.Forms.TextBox
			Get
				Return Me.TextBox
			End Get
		End Property
		Public Property ExtraSpace As String
		Public AllowForSave As Boolean

    Dim _inSafeMode As Boolean
    Dim _totalLineCount As Int32
    Dim _visibleLineCount As Int32
    Dim _totalOutput As New System.Collections.ArrayList
    Dim _tmpFileName As String

    Public Property InSafeMode() As Boolean
      Get
        Return _inSafeMode
      End Get
      Set(ByVal Value As Boolean)
        _inSafeMode = Value
      End Set
    End Property

    Public Sub Reset()
      _totalOutput = New System.Collections.ArrayList
      TextBox.Text = ""
    End Sub

    Public Sub Save(ByVal filePath As String)
      If AllowForSave Then
        System.IO.File.Move(_tmpFileName, filePath)
      End If
    End Sub

		Public Sub WriteLine(ByVal message As String)
			_totalLineCount = _totalLineCount + 1
			message = String.Format("{0}{1}  {2}{3}", ExtraSpace, System.DateTime.Now.ToLongTimeString + " " + _totalLineCount.ToString("000000"), message, vbCrLf)

			If AllowForSave Then
				If _tmpFileName = "" Then
					_tmpFileName = System.IO.Path.GetTempFileName()
				End If

				Dim sw As New System.IO.StreamWriter(_tmpFileName, True)
				sw.Write(message)
				sw.Close()
			End If

			If _totalOutput.Count = 100 Then _totalOutput.RemoveAt(0)
			_totalOutput.Add(message)
			DumpOutput()
		End Sub

		Private Sub DumpOutput()

      If _visibleLineCount > 0 Then
        Dim sb As New System.Text.StringBuilder
        Dim i As Int32
        Dim j As Int32
        i = _totalOutput.Count - _visibleLineCount
        If i < 0 Then i = 0
        For j = i To _totalOutput.Count - 1
          sb.Append(_totalOutput.Item(j))
        Next
        ' get rid of last line feed so it doesn't scroll
        If sb.Length > 2 Then
          TextBox.Text = sb.Remove(sb.Length - 2, 2).ToString()
        End If
      End If

		End Sub

		Private Sub TextBox_Resize1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox.Resize
			_visibleLineCount = CType(System.Math.Round((Me.Height / 15), 0), Int32)
			DumpOutput()
		End Sub

	End Class
End Namespace