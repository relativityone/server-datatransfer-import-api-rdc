Imports System.Collections.Generic
Imports kCura.Windows.Forms

Namespace kCura.EDDS.WinForm
	Public Class ProductionPrecedenceForm
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
		Friend WithEvents LabelAvailableProductions As System.Windows.Forms.Label
		Friend WithEvents LabelSelectedProductions As System.Windows.Forms.Label
		Friend WithEvents _productions As kCura.Windows.Forms.TwoListBox
		Friend WithEvents _okButton As System.Windows.Forms.Button
		Friend WithEvents _cancelButton As System.Windows.Forms.Button
		Friend WithEvents _originalImages As System.Windows.Forms.RadioButton
		Friend WithEvents _includeOriginals As System.Windows.Forms.CheckBox
		Friend WithEvents _producedImages As System.Windows.Forms.RadioButton

		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
			Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(ProductionPrecedenceForm))
			Me._okButton = New System.Windows.Forms.Button
			Me._cancelButton = New System.Windows.Forms.Button
			Me._productions = New kCura.Windows.Forms.TwoListBox
			Me.LabelAvailableProductions = New System.Windows.Forms.Label
			Me.LabelSelectedProductions = New System.Windows.Forms.Label
			Me._originalImages = New System.Windows.Forms.RadioButton
			Me._producedImages = New System.Windows.Forms.RadioButton
			Me._includeOriginals = New System.Windows.Forms.CheckBox
			Me.SuspendLayout()
			'
			'_okButton
			'
			Me._okButton.Location = New System.Drawing.Point(376, 332)
			Me._okButton.Name = "_okButton"
			Me._okButton.TabIndex = 0
			Me._okButton.Text = "OK"
			'
			'_cancelButton
			'
			Me._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me._cancelButton.Location = New System.Drawing.Point(376, 360)
			Me._cancelButton.Name = "_cancelButton"
			Me._cancelButton.TabIndex = 1
			Me._cancelButton.Text = "Cancel"
			'
			'_productions
			'
			Me._productions.AlternateRowColors = False
			Me._productions.KeepButtonsCentered = True
			Me._productions.LeftOrderControlsVisible = False
			Me._productions.Location = New System.Drawing.Point(8, 104)
			Me._productions.Name = "_productions"
			Me._productions.RightOrderControlVisible = True
			Me._productions.Size = New System.Drawing.Size(360, 280)
			Me._productions.TabIndex = 2
			'
			'LabelAvailableProductions
			'
			Me.LabelAvailableProductions.Location = New System.Drawing.Point(8, 88)
			Me.LabelAvailableProductions.Name = "LabelAvailableProductions"
			Me.LabelAvailableProductions.Size = New System.Drawing.Size(144, 16)
			Me.LabelAvailableProductions.TabIndex = 3
			Me.LabelAvailableProductions.Text = "Available Productions"
			'
			'LabelSelectedProductions
			'
			Me.LabelSelectedProductions.Name = "LabelSelectedProductions"
			Me.LabelSelectedProductions.Location = New System.Drawing.Point(180, 88)
			Me.LabelSelectedProductions.Size = New System.Drawing.Size(144, 16)
			Me.LabelSelectedProductions.TabIndex = 4
			Me.LabelSelectedProductions.Text = "Selected Productions"
			'
			'_originalImages
			'
			Me._originalImages.Checked = True
			Me._originalImages.Location = New System.Drawing.Point(8, 8)
			Me._originalImages.Name = "_originalImages"
			Me._originalImages.Size = New System.Drawing.Size(104, 20)
			Me._originalImages.TabIndex = 5
			Me._originalImages.TabStop = True
			Me._originalImages.Text = "Original Images"
			'
			'_producedImages
			'
			Me._producedImages.Location = New System.Drawing.Point(8, 32)
			Me._producedImages.Name = "_producedImages"
			Me._producedImages.Size = New System.Drawing.Size(116, 20)
			Me._producedImages.TabIndex = 6
			Me._producedImages.Text = "Produced Images"
			'
			'_includeOriginals
			'
			Me._includeOriginals.Location = New System.Drawing.Point(8, 56)
			Me._includeOriginals.Name = "_includeOriginals"
			Me._includeOriginals.Size = New System.Drawing.Size(376, 24)
			Me._includeOriginals.TabIndex = 7
			Me._includeOriginals.Text = "Include original images for documents that haven't been produced"
			'
			'ProductionPrecedenceForm
			'
			Me.AcceptButton = Me._okButton
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.CancelButton = Me._cancelButton
			Me.ClientSize = New System.Drawing.Size(456, 393)
			Me.Controls.Add(Me._includeOriginals)
			Me.Controls.Add(Me._producedImages)
			Me.Controls.Add(Me._originalImages)
			Me.Controls.Add(Me.LabelSelectedProductions)
			Me.Controls.Add(Me.LabelAvailableProductions)
			Me.Controls.Add(Me._productions)
			Me.Controls.Add(Me._cancelButton)
			Me.Controls.Add(Me._okButton)
			Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
			Me.MaximizeBox = True
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable
			Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
			Me.MinimumSize = New System.Drawing.Size(464, 370)
			Me.MinimizeBox = False
			Me.Name = "ProductionPrecedenceForm"
			Me.Text = "Pick Production Precedence"
			Me.ResumeLayout(False)

		End Sub

#End Region

		Friend WithEvents _application As kCura.EDDS.WinForm.Application
		Private _precedenceList As Pair()
		Public PrecedenceTable As System.Data.DataTable
		Friend Property PrecedenceList() As Pair()
			Get
				Return _precedenceList
			End Get
			Set(ByVal value As Pair())
				_precedenceList = value
			End Set
		End Property

		Public ExportFile As WinEDDS.ExportFile

		Private Sub ProductionPrecedenceForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
			Dim row As System.Data.DataRow
			Dim activeValues As New System.Collections.ArrayList
			Dim item As Pair
			Dim firstTimeThrough As Boolean = True
			Dim hasOriginals As Boolean = False
			If Not _precedenceList Is Nothing Then
				For Each item In _precedenceList
					If item.Value = "-1" Then
						hasOriginals = True
						If firstTimeThrough Then
							Exit For						 'do nothing
						Else
							_producedImages.Checked = True
							_originalImages.Checked = False
							_includeOriginals.Checked = True
						End If
					Else
						If _producedImages.Checked = False Then
							_producedImages.Checked = True
						End If
						_productions.RightListBoxItems.Add(item)
						activeValues.Add(item.Value)
					End If
					If hasOriginals AndAlso _producedImages.Checked Then
						_includeOriginals.Checked = True
					End If
					firstTimeThrough = False
				Next
			End If
			For Each row In PrecedenceTable.Rows
				If Not activeValues.Contains(row("Value").ToString) Then
					_productions.LeftListBoxItems.Add(New Pair(row("Value").ToString, row("Display").ToString))
				End If
			Next
		End Sub

		Private Sub _okButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _okButton.Click
			Dim al As New System.Collections.ArrayList
			Dim item As Pair
			If _originalImages.Checked Then
				al.Add(New Pair("-1", "Original"))
			Else
				For Each item In _productions.RightListBoxItems
					al.Add(item)
				Next
				If _includeOriginals.Checked Then
					al.Add(New Pair("-1", "Original"))
				End If
			End If
			Me.Close()
			RaiseEvent PrecedenceOK(DirectCast(al.ToArray(GetType(Pair)), Pair()))
		End Sub

		Private Sub _cancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _cancelButton.Click
			Me.Close()
		End Sub

		Public Event PrecedenceOK(ByVal precedenceList As Pair())

		Private Sub _originalImages_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _originalImages.CheckedChanged
			If _originalImages.Checked Then
				_productions.Enabled = False
				_includeOriginals.Enabled = False
			Else
				_productions.Enabled = True
				_includeOriginals.Enabled = True
			End If
		End Sub

#Region "Resizing"
		'These member variables are populated with data needed to resize the controls

		'Avoid adjusting the layout if the size hasn't changed
		Private _layoutLastFormSize As Size

		' Used to keep track of whether we need to calculate the layout values.  In addition to
		' initial population, they may need to be populated later due to autoscaling.  Autoscaling
		' will change the distance between concrols which we would not expect to change.  If this
		' happens, the _layout info which contains the relative location of controls needs to be 
		' updated.
		Private _layoutReferenceDistance As Int32 = 0

		Private _layoutDifferenceList As List(Of RelativeLayoutData)

		Private Function CalcReferenceDistance() As Int32
			Return LabelSelectedProductions.Width
		End Function

		Private Sub OnForm_Layout(ByVal sender As Object, ByVal e As System.Windows.Forms.LayoutEventArgs) Handles MyBase.Layout
			'The reference distance should remain constant even if the dialog box is resized
			If _layoutReferenceDistance <> CalcReferenceDistance() Then
				InitializeLayout()
			Else
				AdjustLayout()
			End If
		End Sub

		Private Sub InitializeLayout()
			_layoutLastFormSize = Me.Size

			'Layout properties which are directly based on another layout property
			If _layoutDifferenceList Is Nothing Then
				_layoutDifferenceList = New List(Of RelativeLayoutData)

				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutPropertyType.Width, _productions, LayoutPropertyType.Width))
				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutPropertyType.Height, _productions, LayoutPropertyType.Height))

				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutPropertyType.Width, _okButton, LayoutPropertyType.Left))
				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutPropertyType.Height, _okButton, LayoutPropertyType.Top))
				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutPropertyType.Width, _cancelButton, LayoutPropertyType.Left))
				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutPropertyType.Height, _cancelButton, LayoutPropertyType.Top))

				_layoutDifferenceList.Add(New RelativeLayoutData(_productions, LayoutPropertyType.Right, LabelSelectedProductions, LayoutPropertyType.Left))
			End If

			_layoutDifferenceList.ForEach(Sub(x)
																			x.InitializeDifference()
																		End Sub)

			_layoutReferenceDistance = CalcReferenceDistance()

			AdjustColumnLabel()
		End Sub

		Public Sub AdjustLayout()
			If Not _layoutLastFormSize.Equals(Me.Size) Then
				For Each x As RelativeLayoutData In _layoutDifferenceList
					x.AdjustRelativeControlBasedOnDifference()
				Next

				_layoutLastFormSize = Me.Size
			End If

			AdjustColumnLabel()
		End Sub

		Private Sub AdjustColumnLabel()
			'Adjust the location of the label to be aligned with the left side of the Right ListBox

			'Get the absolute position of the Right ListBox of the TwoListBox in screen coordinates
			Dim absoluteListBoxLoc As Point = _productions.RightListBox.PointToScreen(New Point(0, 0))
			'Convert to a location relative to the Views group (_filtersBox)
			Dim relativeListBoxLoc As Point = Me.LabelSelectedProductions.Parent.PointToClient(absoluteListBoxLoc)
			'Adjust the location of the label
			Me.LabelSelectedProductions.Left = relativeListBoxLoc.X
		End Sub
#End Region
	End Class

End Namespace
