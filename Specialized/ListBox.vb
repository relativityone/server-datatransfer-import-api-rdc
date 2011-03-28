Imports System
Imports System.Collections
Imports System.ComponentModel
Imports System.Drawing
Imports System.Data
Imports System.Windows.Forms
Imports System.Runtime.InteropServices
Imports System.Drawing.Drawing2D

Namespace kCura.Windows.Forms
	Public Class ListBox
		Inherits System.Windows.Forms.ListBox

#Region " Properties "

		Dim _relativityHighlightColor As System.Drawing.Color
		Public ReadOnly Property RelativityHighlightColor As System.Drawing.Color
			Get
				Return _relativityHighlightColor
			End Get
		End Property

		Public Property AlternateColors As Boolean

		Public Property HighlightIndex As Int32

#End Region

		Public Sub New(ByVal relativityHighlightColor As System.Drawing.Color)
			HighlightIndex = -1
			_relativityHighlightColor = relativityHighlightColor
			Me.SetStyle(System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer Or System.Windows.Forms.ControlStyles.ResizeRedraw Or System.Windows.Forms.ControlStyles.UserPaint, True)
			Me.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
		End Sub

#Region " Scrolling support "

		Public Property HorizontalScrollOffset As Integer
		Public Property VerticalScrollOffset As Integer

		<Category("Action")> _
		Public Event Scrolled(ByVal sender As Object, ByVal e As ScrollEventArgs)

		Protected Overrides Sub WndProc(ByRef msg As System.Windows.Forms.Message)
			If msg.Msg = Win32API.WM_HSCROLL OrElse msg.Msg = Win32API.WM_VSCROLL Then
				Dim si As New Win32API.ScrollInfoStruct With {.fMask = Win32API.SIF_ALL, .cbSize = Marshal.SizeOf(si)}

				If msg.WParam.ToInt32() = Win32API.SB_ENDSCROLL Then
					If msg.Msg = Win32API.WM_HSCROLL Then
						Win32API.GetScrollInfo(msg.HWnd, Win32API.SBS_HORZ, si)
						HorizontalScrollOffset = si.nPos

						RaiseEvent Scrolled(Me, New ScrollEventArgs(ScrollEventType.EndScroll, si.nPos, ScrollOrientation.HorizontalScroll))
					ElseIf msg.Msg = Win32API.WM_VSCROLL Then
						Win32API.GetScrollInfo(msg.HWnd, Win32API.SBS_VERT, si)
						VerticalScrollOffset = si.nPos

						RaiseEvent Scrolled(Me, New ScrollEventArgs(ScrollEventType.EndScroll, si.nPos, ScrollOrientation.VerticalScroll))
					End If
				End If
			End If

			MyBase.WndProc(msg)
		End Sub

#End Region

#Region " Owner draw support "

		Protected Overrides Sub OnDrawItem(ByVal e As System.Windows.Forms.DrawItemEventArgs)
			Dim newArgs As System.Windows.Forms.DrawItemEventArgs = e

			If Me.Items.Count > 0 Then
				Dim rowBackColor As System.Drawing.Color = If(Me.AlternateColors AndAlso e.Index Mod 2 = 1, System.Drawing.Color.LightGray, e.BackColor)

				' Create a copy of the original DrawItemEventArgs object (but with a new background color)
				newArgs = New System.Windows.Forms.DrawItemEventArgs(e.Graphics, e.Font, e.Bounds, e.Index, e.State, e.ForeColor, rowBackColor)

				' Create a brush with the highlight color if we have are drawing a highlighted item
				Dim brush As New System.Drawing.SolidBrush(If(newArgs.Index = _HighlightIndex, Me.RelativityHighlightColor, newArgs.ForeColor))

				' Adjust where the drawing will begin to account for any shift made by the scroll bar
				Dim newOffset As New System.Drawing.PointF(newArgs.Bounds.Left - HorizontalScrollOffset, newArgs.Bounds.Top)

				newArgs.DrawBackground()
				newArgs.Graphics.DrawString(Me.Items(e.Index).ToString, newArgs.Font, brush, newOffset)
				newArgs.DrawFocusRectangle()

				brush.Dispose()
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
						If ((Me.SelectionMode = System.Windows.Forms.SelectionMode.One AndAlso Me.SelectedIndex = i) OrElse _
						 (Me.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple AndAlso Me.SelectedIndices.Contains(i)) OrElse _
						 Me.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended AndAlso Me.SelectedIndices.Contains(i)) Then
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

#End Region

	End Class
End Namespace


