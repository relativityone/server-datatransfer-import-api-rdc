Namespace kCura.EDDS.WinForm
	Public Class LoadFilePreviewForm
		Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "
		Private _application As Application
		Private _formType As Int32
		Private _multiRecordDelimiter As Char
		Private _previewCodeCount As System.Collections.Specialized.HybridDictionary

		Public Sub New(ByVal formType As Int32, ByVal mutliRecordDelimiter As Char, ByVal previewCodeCount As System.Collections.Specialized.HybridDictionary)
			MyBase.New()

			'This call is required by the Windows Form Designer.
			InitializeComponent()

			'Add any initialization after the InitializeComponent() call
			_application = Application.Instance
			_formType = formType
			_multiRecordDelimiter = mutliRecordDelimiter
			_previewCodeCount = previewCodeCount

			Me.Text = "Relativity Desktop Client | Preview Load File"
			If _formType = kCura.EDDS.WinForm.LoadFilePreviewForm.FormType.Codes Then
				Me.Text = "Relativity Desktop Client | Preview Choices and Folders"
			End If
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
			Me.Text = "Relativity Desktop Client | Preview Choices and Folders"
			CType(Me._grid, System.ComponentModel.ISupportInitialize).EndInit()
			Me.ResumeLayout(False)

		End Sub

#End Region
		Private WithEvents _thrower As kCura.WinEDDS.ValueThrower

		Public DataSource As DataTable
		Public IsError As Boolean
		Private _erroredCellCollection As Generic.Dictionary(Of String, Generic.List(Of Integer))

		Public Enum FormType
			LoadFile = 1
			Codes = 2
		End Enum

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
			For Each column In ds.Columns
				Dim col As New HighlightErrorColumn
				col.MappingName = column.ColumnName
				col.HeaderText = column.ColumnName
				If (_erroredCellCollection.ContainsKey(column.ColumnName)) Then
					If (Not IsNothing(_erroredCellCollection(column.ColumnName))) Then
						For Each i In _erroredCellCollection(column.ColumnName)
							col.AddErrorRow(i)
						Next
					End If
				End If
				tablestyles.GridColumnStyles.Add(col)
			Next
			_grid.TableStyles.Add(tablestyles)
			DataSource = ds
			_grid.DataSource = ds
		End Sub

		Private Sub _thrower_OnEvent(ByVal value As Object) Handles _thrower.OnEvent
			Dim args As Object() = DirectCast(value, Object())
			If _formType = FormType.Codes Then
				Me.DataSource = _application.BuildFoldersAndCodesDataSource(DirectCast(args(0), ArrayList), _previewCodeCount)
			Else
				populateErroredCellCollection(DirectCast(args(0), ArrayList))
				Me.DataSource = _application.BuildLoadFileDataSource(DirectCast(args(0), ArrayList))
			End If
			Me.IsError = CType(args(1), Boolean)
			Me.Invoke(New HandleDataSourceDelegate(AddressOf HandleDataSource))
		End Sub

		Private Sub populateErroredCellCollection(ByVal al As ArrayList)
			_erroredCellCollection = New Generic.Dictionary(Of String, Generic.List(Of Integer))

			Dim column As New System.Data.DataColumn
			Dim rowNumber As Integer = 0
			Dim colNumber As Integer = 0
			Dim listOfRowWideErrors As Generic.List(Of Integer) = New Generic.List(Of Integer)

			For Each r In al
				If Not r Is Nothing Then
					If TypeOf r Is System.Exception Then
						' row-wide error
						listOfRowWideErrors.Add(rowNumber)
					Else
						colNumber = 0
						Dim c As Api.ArtifactField
						For Each c In DirectCast(r, Api.ArtifactField())
							If Not c Is Nothing Then
								If Not _erroredCellCollection.ContainsKey(c.DisplayName) Then
									_erroredCellCollection(c.DisplayName) = New Generic.List(Of Integer)
								End If
								' how is what was an exception now just a string?
								If TypeOf c.Value Is ErrorMessage Then
									' column error
									_erroredCellCollection(c.DisplayName).Add(rowNumber)
								End If
							End If
							colNumber += 1
						Next
					End If

				End If
				rowNumber += 1
			Next

			For Each i As Integer In listOfRowWideErrors
				For Each key In _erroredCellCollection.Keys
					_erroredCellCollection(key).Add(i)
				Next
			Next
		End Sub

		Public Sub HandleDataSource()
			Me.SetGridDataSource(Me.DataSource)
			If Me.IsError Then Me.Text = "Preview Errors"
		End Sub

		Delegate Sub HandleDataSourceDelegate()

	End Class

	Public Class HighlightErrorColumn
		Inherits DataGridColumnStyle

		Protected _listOfErrorRows As Generic.List(Of Integer)

		Public Sub AddErrorRow(ByVal rowNumber As Integer)
			_listOfErrorRows = If(_listOfErrorRows Is Nothing, New Generic.List(Of Integer)(), _listOfErrorRows)
			If (Not _listOfErrorRows Is Nothing) And rowNumber >= 0 And Not _listOfErrorRows.Contains(rowNumber) Then
				_listOfErrorRows.Add(rowNumber)
			End If
		End Sub

		Protected Function IsRowErrored(ByVal rowNumber As Integer) As Boolean
			If (Not (IsNothing(_listOfErrorRows))) Then
				Return _listOfErrorRows.Contains(rowNumber)
			End If
			Return False
		End Function


		Protected Overrides Sub Abort(ByVal rowNum As Integer)

		End Sub

		Protected Overrides Function Commit(ByVal dataSource As System.Windows.Forms.CurrencyManager, ByVal rowNum As Integer) As Boolean
			Return True
		End Function

		Protected Overloads Overrides Sub Edit(ByVal source As System.Windows.Forms.CurrencyManager, ByVal rowNum As Integer, ByVal bounds As System.Drawing.Rectangle, ByVal [readOnly] As Boolean, ByVal instantText As String, ByVal cellIsVisible As Boolean)

		End Sub

		Protected Overrides Function GetMinimumHeight() As Integer
			Return 20
		End Function

		Protected Overrides Function GetPreferredHeight(ByVal g As System.Drawing.Graphics, ByVal value As Object) As Integer
			Return 20
		End Function

		Protected Overrides Function GetPreferredSize(ByVal g As System.Drawing.Graphics, ByVal value As Object) As System.Drawing.Size
			Return New System.Drawing.Size(100, 20)
		End Function

		Protected Overloads Overrides Sub Paint(ByVal g As System.Drawing.Graphics, ByVal bounds As System.Drawing.Rectangle, ByVal source As System.Windows.Forms.CurrencyManager, ByVal rowNum As Integer)
			Me.Paint(g, bounds, source, rowNum, False)
		End Sub

		Protected Overloads Overrides Sub Paint(ByVal g As System.Drawing.Graphics, ByVal bounds As System.Drawing.Rectangle, ByVal source As System.Windows.Forms.CurrencyManager, ByVal rowNum As Integer, ByVal alignToRight As Boolean)
			'### NOTE: Brush Color decision is made in the other Paint(), so there's no need to duplicate it here, especially now that it's a check on error
			'			Dim cellcontents As String = Me.GetColumnValueAtRow(source, rowNum).ToString.ToLower
			'		If cellcontents.IndexOf("error") <> -1 Then
			'Me.Paint(g, bounds, source, rowNum, Brushes.White, Brushes.Red, alignToRight)
			'Else
			Me.Paint(g, bounds, source, rowNum, Brushes.White, Brushes.Black, alignToRight)
			'End If
		End Sub

		Protected Overloads Overrides Sub Paint(ByVal g As Graphics, ByVal bounds As Rectangle, ByVal [source] As CurrencyManager, ByVal rowNum As Integer, ByVal backBrush As Brush, ByVal foreBrush As Brush, ByVal alignToRight As Boolean)

			Dim rect As Rectangle = bounds
			g.FillRectangle(backBrush, rect)
			rect.Offset(0, 2)
			rect.Height -= 2
			Dim myForeBrush As Brush = foreBrush
			Dim cellcontents As String = Me.GetColumnValueAtRow(source, rowNum).ToString.ToLower
			'		If cellcontents.IndexOf("error") <> -1 Then
			If IsRowErrored(rowNum) Then
				myForeBrush = Brushes.Red
				'Me.Paint(g, bounds, source, rowNum, Brushes.White, Brushes.Red, alignToRight)
			Else
			End If
			g.DrawString(cellcontents, Me.DataGridTableStyle.DataGrid.Font, myForeBrush, RectangleF.FromLTRB(rect.X, rect.Y, rect.Right, rect.Bottom))
		End Sub
	End Class

End Namespace