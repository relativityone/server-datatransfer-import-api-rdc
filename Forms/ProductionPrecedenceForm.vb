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
		Friend WithEvents Label1 As System.Windows.Forms.Label
		Friend WithEvents Label2 As System.Windows.Forms.Label
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
			Me.Label1 = New System.Windows.Forms.Label
			Me.Label2 = New System.Windows.Forms.Label
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
			Me._productions.KeepButtonsCentered = True
			Me._productions.LeftOrderControlsVisible = False
			Me._productions.Location = New System.Drawing.Point(8, 104)
			Me._productions.Name = "_productions"
			Me._productions.RightOrderControlVisible = True
			Me._productions.Size = New System.Drawing.Size(360, 280)
			Me._productions.TabIndex = 2
			'
			'Label1
			'
			Me.Label1.Location = New System.Drawing.Point(8, 88)
			Me.Label1.Name = "Label1"
			Me.Label1.Size = New System.Drawing.Size(144, 16)
			Me.Label1.TabIndex = 3
			Me.Label1.Text = "Available Productions"
			'
			'Label2
			'
			Me.Label2.Location = New System.Drawing.Point(196, 88)
			Me.Label2.Name = "Label2"
			Me.Label2.Size = New System.Drawing.Size(144, 16)
			Me.Label2.TabIndex = 4
			Me.Label2.Text = "Selected Productions"
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
			Me.ClientSize = New System.Drawing.Size(456, 389)
			Me.Controls.Add(Me._includeOriginals)
			Me.Controls.Add(Me._producedImages)
			Me.Controls.Add(Me._originalImages)
			Me.Controls.Add(Me.Label2)
			Me.Controls.Add(Me.Label1)
			Me.Controls.Add(Me._productions)
			Me.Controls.Add(Me._cancelButton)
			Me.Controls.Add(Me._okButton)
			Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
			Me.MaximizeBox = False
			Me.MaximumSize = New System.Drawing.Size(464, 416)
			Me.MinimizeBox = False
			Me.MinimumSize = New System.Drawing.Size(464, 416)
			Me.Name = "ProductionPrecedenceForm"
			Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
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
	End Class

End Namespace
