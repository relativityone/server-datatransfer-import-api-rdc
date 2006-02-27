Namespace kCura.EDDS.WinForm
  Public Class LoadFilePreviewForm
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New()
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
    Friend WithEvents _grid As System.Windows.Forms.DataGrid
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
      Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(LoadFilePreviewForm))
      Me._grid = New System.Windows.Forms.DataGrid
      CType(Me._grid, System.ComponentModel.ISupportInitialize).BeginInit()
      Me.SuspendLayout()
      '
      '_grid
      '
      Me._grid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
      Me._grid.CaptionVisible = False
      Me._grid.DataMember = ""
      Me._grid.Dock = System.Windows.Forms.DockStyle.Fill
      Me._grid.HeaderForeColor = System.Drawing.SystemColors.ControlText
      Me._grid.Location = New System.Drawing.Point(0, 0)
      Me._grid.Name = "_grid"
      Me._grid.ReadOnly = True
      Me._grid.RowHeadersVisible = False
      Me._grid.Size = New System.Drawing.Size(720, 525)
      Me._grid.TabIndex = 0
      '
      'LoadFilePreviewForm
      '
      Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
      Me.ClientSize = New System.Drawing.Size(720, 525)
      Me.Controls.Add(Me._grid)
      Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
      Me.Name = "LoadFilePreviewForm"
      Me.Text = "Preview Load File..."
      CType(Me._grid, System.ComponentModel.ISupportInitialize).EndInit()
      Me.ResumeLayout(False)

    End Sub

#End Region

		Public Sub SetGridDataSource(ByVal dataSource As DataTable)
			Dim column As New System.Data.DataColumn
			Dim tablestyles As New DataGridTableStyle
			tablestyles.DataGrid = _grid
			_grid.DataSource = dataSource
		End Sub

	End Class
End Namespace