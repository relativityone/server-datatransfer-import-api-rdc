Namespace kCura.EDDS.WinForm
  Public Class LoadFilePreviewForm
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "
		Private _application As Application
    Public Sub New()
      MyBase.New()

      'This call is required by the Windows Form Designer.
      InitializeComponent()

      'Add any initialization after the InitializeComponent() call
			_application = Application.Instance
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
			Me._grid.Size = New System.Drawing.Size(728, 533)
			Me._grid.TabIndex = 0
			'
			'LoadFilePreviewForm
			'
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.ClientSize = New System.Drawing.Size(728, 533)
			Me.Controls.Add(Me._grid)
			Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
			Me.Name = "LoadFilePreviewForm"
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
			Me.Text = "Preview Load File..."
			CType(Me._grid, System.ComponentModel.ISupportInitialize).EndInit()
			Me.ResumeLayout(False)

		End Sub

#End Region
		Private WithEvents _thrower As kCura.WinEDDS.ValueThrower
		Public DataSource As DataTable
		Public IsError As Boolean
		Public Property Thrower() As kCura.WinEDDS.ValueThrower
			Get
				Return _thrower
			End Get
			Set(ByVal value As kCura.WinEDDS.ValueThrower)
				_thrower = value
			End Set
		End Property

		Public Sub SetGridDataSource(ByVal ds As DataTable)
			Dim column As New System.Data.DataColumn
			Dim tablestyles As New DataGridTableStyle
			DataSource = ds
			tablestyles.DataGrid = _grid
			_grid.DataSource = ds
		End Sub

		Private Sub _thrower_OnEvent(ByVal value As Object) Handles _thrower.OnEvent
			Dim args As Object() = DirectCast(value, Object())
			Me.DataSource = _application.BuildLoadFileDataSource(DirectCast(args(0), ArrayList))
			Me.IsError = CType(args(1), Boolean)
			Me.Invoke(New HandleDataSourceDelegate(AddressOf HandleDataSource))
			'HandleDataSource(value)
		End Sub

		Public Sub HandleDataSource()
			Me.SetGridDataSource(Me.DataSource)
			If Me.IsError Then Me.Text = "Preview Errors"
			'Me.Show()
		End Sub
		'Public Sub SetGridDataSource(ByVal ds As DataTable)
		'	Dim column As New System.Data.DataColumn
		'	Dim tablestyles As New DataGridTableStyle
		'	DataSource = ds
		'	tablestyles.DataGrid = _grid
		'	_grid.DataSource = ds
		'End Sub

		'Private Sub _thrower_OnEvent(ByVal value As Object) Handles _thrower.OnEvent
		'	Dim args As Object() = DirectCast(value, Object())
		'	Me.Invoke(New HandleDataSourceDelegate(AddressOf HandleDataSource))
		'	HandleDataSource(value)
		'End Sub

		'Public Sub HandleDataSource(ByVal value As Object)
		'	Dim args As Object() = DirectCast(value, Object())
		'	Me.SetGridDataSource(_application.BuildLoadFileDataSource(DirectCast(args(0), ArrayList)))
		'	If CType(args(1), Boolean) Then
		'		Me.Text = "Preview Errors"
		'	End If
		'	'Me.Show()
		'End Sub

		Delegate Sub HandleDataSourceDelegate()

	End Class


End Namespace