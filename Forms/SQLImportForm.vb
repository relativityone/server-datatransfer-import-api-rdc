Namespace kCura.EDDS.WinForm
  Public Class SQLImportForm
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New()
      MyBase.New()

      'This call is required by the Windows Form Designer.
      InitializeComponent()

      'Add any initialization after the InitializeComponent() call
      _application = kCura.EDDS.WinForm.Application.Instance
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
    Friend WithEvents _go As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
      Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(SQLImportForm))
      Me._go = New System.Windows.Forms.Button
      Me.SuspendLayout()
      '
      '_go
      '
      Me._go.Location = New System.Drawing.Point(4, 4)
      Me._go.Name = "_go"
      Me._go.Size = New System.Drawing.Size(280, 264)
      Me._go.TabIndex = 0
      Me._go.Text = "Go!"
      '
      'SQLImportForm
      '
      Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
      Me.ClientSize = New System.Drawing.Size(292, 273)
      Me.Controls.Add(Me._go)
      Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
      Me.Name = "SQLImportForm"
      Me.Text = "Import from SQL Database..."
      Me.ResumeLayout(False)

    End Sub

#End Region

		Friend WithEvents _application As kCura.EDDS.WinForm.Application

		Public SQLImportSettings As kCura.WinEDDS.SQLImportSettings

		Private Sub _go_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _go.Click
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			_application.ImportSQL(Me.SQLImportSettings)
			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Sub
	End Class
End Namespace