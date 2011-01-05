Namespace kCura.Windows.Forms
	Public Class ListBox
		Inherits System.Windows.Forms.ListBox

		Private _highlightIndex As Int32 = -1
		Private _relativityHighlightColor As System.Drawing.Color
		Private _alternateColors As Boolean

		Public Property HighlightIndex() As Int32
			Get
				Return _highlightIndex
			End Get
			Set(ByVal value As Int32)
				_highlightIndex = value
			End Set
		End Property

		Public ReadOnly Property RelativityHighlightColor() As System.Drawing.Color
			Get
				Return _relativityHighlightColor
			End Get
		End Property

		Public Property AlternateColors() As Boolean
			Get
				Return _alternateColors
			End Get
			Set(ByVal value As Boolean)
				_alternateColors = value
			End Set
		End Property

		Public Sub New(ByVal relativityHighlightColor As System.Drawing.Color)
			_relativityHighlightColor = relativityHighlightColor
			Me.SetStyle( _
			 System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer Or System.Windows.Forms.ControlStyles.ResizeRedraw Or System.Windows.Forms.ControlStyles.UserPaint, True)
			Me.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
		End Sub

		Protected Overrides Sub OnDrawItem(ByVal e As System.Windows.Forms.DrawItemEventArgs)
			Dim rowBackColor As System.Drawing.Color = If(Me.AlternateColors AndAlso e.Index Mod 2 = 1, System.Drawing.Color.LightGray, e.BackColor)
			Dim newArgs As System.Windows.Forms.DrawItemEventArgs = e
			If Me.Items.Count > 0 Then
				newArgs = New System.Windows.Forms.DrawItemEventArgs(e.Graphics, e.Font, Me.GetItemRectangle(e.Index), e.Index, e.State, e.ForeColor, rowBackColor)
				newArgs.DrawBackground()
				newArgs.Graphics.DrawString(Me.Items(e.Index).ToString(), newArgs.Font, New System.Drawing.SolidBrush(If(newArgs.Index = _highlightIndex, Me.RelativityHighlightColor, newArgs.ForeColor)), New System.Drawing.PointF(e.Bounds.Left, e.Bounds.Top))
				newArgs.DrawFocusRectangle()
			End If
			MyBase.OnDrawItem(newArgs)
		End Sub

		Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)

			Dim region As New System.Drawing.Region(e.ClipRectangle)
			e.Graphics.FillRegion(New System.Drawing.SolidBrush(Me.BackColor), region)
			If Me.Items.Count > 0 Then
				For i As Int32 = 0 To Me.Items.Count - 1
					Dim rect As System.Drawing.Rectangle = Me.GetItemRectangle(i)
					If e.ClipRectangle.IntersectsWith(rect) Then
						If ((Me.SelectionMode = System.Windows.Forms.SelectionMode.One AndAlso Me.SelectedIndex = i) OrElse (Me.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple AndAlso Me.SelectedIndices.Contains(i)) OrElse Me.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended AndAlso Me.SelectedIndices.Contains(i)) Then
							OnDrawItem(New System.Windows.Forms.DrawItemEventArgs(e.Graphics, Me.Font, rect, i, System.Windows.Forms.DrawItemState.Selected, Me.ForeColor, Me.BackColor))
						Else
							OnDrawItem(New System.Windows.Forms.DrawItemEventArgs(e.Graphics, Me.Font, rect, i, System.Windows.Forms.DrawItemState.Default, Me.ForeColor, Me.BackColor))
						End If
						region.Complement(rect)
					End If
				Next
			End If
			MyBase.OnPaint(e)

		End Sub
	End Class
End Namespace


