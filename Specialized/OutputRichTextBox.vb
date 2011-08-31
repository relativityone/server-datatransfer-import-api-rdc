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
		Public WithEvents DetailsLink As System.Windows.Forms.LinkLabel

    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
			Me.DetailsLink = New System.Windows.Forms.LinkLabel
			Me.TextBox = New System.Windows.Forms.TextBox
			Me.SuspendLayout()

			'
			'DetailsLink
			'
			Me.TextBox.Controls.Add(Me.DetailsLink)
			Me.DetailsLink.Size = New System.Drawing.Size(0, 0)
			Me.DetailsLink.Text = ""
			Me.DetailsLink.SendToBack()
			Me.DetailsLink.AutoSize = True

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
			Me.TextBox.ReadOnly = True
			Me.BackColor = Drawing.Color.White
			Me.Scrollbars = System.Windows.Forms.ScrollBars.None
      '
      'OutputRichTextBox
			'

      Me.Controls.Add(Me.TextBox)
      Me.Name = "OutputRichTextBox"
			Me.Size = New System.Drawing.Size(150, 144)
			Me.ResumeLayout(False)


    End Sub

#End Region

#Region " Properties "

		Public Overrides Property BackColor As System.Drawing.Color
			Get
				Return Me.TextBox.BackColor
			End Get
			Set(ByVal value As System.Drawing.Color)
				Me.TextBox.BackColor = value
			End Set
		End Property

		Public Property Scrollbars As System.Windows.Forms.ScrollBars
			Get
				Return Me.TextBox.ScrollBars
			End Get
			Set(ByVal value As System.Windows.Forms.ScrollBars)
				Me.TextBox.ScrollBars = value
			End Set
		End Property

		Public Property InSafeMode() As Boolean
			Get
				Return _inSafeMode
			End Get
			Set(ByVal Value As Boolean)
				_inSafeMode = Value
			End Set
		End Property

		Public Property AllowForSave As Boolean

#End Region

		Dim _inSafeMode As Boolean
		Dim _totalLineCount As Int32
		Dim _visibleLineCount As Int32
		Dim _totalOutput As New System.Collections.ArrayList
		Dim _tmpFileName As String

		Public Sub Reset()
			_totalOutput = New System.Collections.ArrayList
			TextBox.Text = ""
			Me.DetailsLink.SendToBack()
			Me.DetailsLink.Text = ""
		End Sub

		Public Sub Save(ByVal filePath As String)
			If AllowForSave Then
				System.IO.File.Move(_tmpFileName, filePath)
			End If
		End Sub

		Public Sub WriteLine(ByVal message As String)
			WriteLine(message, vbCrLf)
		End Sub

		Public Sub WriteLine(ByVal message As String, ByVal lineDelimiter As String)
			_totalLineCount = _totalLineCount + 1
			message = String.Format("{0}  {1}{2}", System.DateTime.Now.ToLongTimeString + " " + _totalLineCount.ToString("000000"), message.TrimEnd(vbCrLf.ToCharArray), lineDelimiter)

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

		Public Sub WriteErrorDetails()
			If Not Me.TextBox.IsDisposed Then
				Me.DetailsLink.Text = "More Details"
				Me.DetailsLink.BringToFront()
			End If
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
				If sb.Length > 2 Then
					TextBox.Text = sb.Remove(sb.Length - 2, 2).ToString()
				End If
			End If
		End Sub

		Private Sub TextBox_Resize1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox.Resize
			_visibleLineCount = CType(System.Math.Round((Me.Height / 15), 0), Int32)
			DumpOutput()
			PositionDetailsLink()
		End Sub

		Public Sub PositionDetailsLink()
			Me.DetailsLink.Location = Me.TextBox.GetPositionFromCharIndex(Me.TextBox.TextLength - 10)
		End Sub
	End Class
End Namespace