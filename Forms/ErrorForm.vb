Namespace kCura.EDDS.WinForm
  Public Class ErrorForm
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Protected Sub New()
      MyBase.New()

      'This call is required by the Windows Form Designer.
      InitializeComponent()

      'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
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
    Friend WithEvents ErrorOutputBox As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
      Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(ErrorForm))
      Me.ErrorOutputBox = New System.Windows.Forms.TextBox
      Me.Label1 = New System.Windows.Forms.Label
      Me.SuspendLayout()
      '
      'ErrorOutputBox
      '
      Me.ErrorOutputBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                  Or System.Windows.Forms.AnchorStyles.Left) _
                  Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
      Me.ErrorOutputBox.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.ErrorOutputBox.Location = New System.Drawing.Point(0, 32)
      Me.ErrorOutputBox.MaxLength = 10000000
      Me.ErrorOutputBox.Multiline = True
      Me.ErrorOutputBox.Name = "ErrorOutputBox"
      Me.ErrorOutputBox.Size = New System.Drawing.Size(468, 248)
      Me.ErrorOutputBox.TabIndex = 0
      Me.ErrorOutputBox.Text = ""
      '
      'Label1
      '
      Me.Label1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                  Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
      Me.Label1.Location = New System.Drawing.Point(0, 4)
      Me.Label1.Name = "Label1"
      Me.Label1.Size = New System.Drawing.Size(464, 23)
      Me.Label1.TabIndex = 1
      Me.Label1.Text = "An error has occured in WinEDDS. "
      '
      'ErrorForm
      '
      Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
      Me.ClientSize = New System.Drawing.Size(468, 277)
      Me.Controls.Add(Me.Label1)
      Me.Controls.Add(Me.ErrorOutputBox)
      Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
      Me.Name = "ErrorForm"
      Me.Text = "WinEDDS Error..."
      Me.ResumeLayout(False)

    End Sub

#End Region

		Private _ex As System.Exception

    Private Sub ErrorForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
      ErrorOutputBox.Text = _ex.ToString
    End Sub

		Public Sub New(ByVal ex As System.Exception)
			MyBase.New()
			_ex = ex
		End Sub
	End Class
End Namespace