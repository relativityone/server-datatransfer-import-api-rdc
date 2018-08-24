Imports System
Imports System.Drawing
Imports System.Windows.Forms

Namespace Oli.Controls
    Public Class VisualCue
        Public Const NoVisualCue As Integer = -1
        Private _listBox As ListBox
        Private _index As Integer = NoVisualCue

        Public Sub New(ByVal listBox As ListBox)
            _listBox = listBox
        End Sub

        Public ReadOnly Property Index As Integer
            Get
                Return _index
            End Get
        End Property

        Public Sub Clear()
            If _index <> NoVisualCue Then
                Draw(_index)
                _index = NoVisualCue
            End If
        End Sub

        Public Sub Draw(ByVal itemIndex As Integer)
            Dim rect As Rectangle
            Dim l1p1, l1p2, l2p1, l2p2 As Point

            If _listBox.Sorted Then
                rect = _listBox.ClientRectangle
                rect = _listBox.RectangleToScreen(rect)
                l1p1 = New Point(rect.Left, rect.Top)
                l1p2 = New Point(rect.Left, rect.Bottom)
                l2p1 = New Point(rect.Left + 1, rect.Top)
                l2p2 = New Point(rect.Left + 1, rect.Bottom)
            Else

                If _listBox.Items.Count = 0 Then
                    rect = _listBox.ClientRectangle
                ElseIf itemIndex < _listBox.Items.Count Then
                    rect = _listBox.GetItemRectangle(itemIndex)
                Else
                    rect = _listBox.GetItemRectangle(_listBox.Items.Count - 1)
                    rect.Y += rect.Height
                End If

                rect.Y -= 1

                If rect.Y < _listBox.ClientRectangle.Y Then
                    rect.Y = _listBox.ClientRectangle.Y
                End If

                rect = _listBox.RectangleToScreen(rect)
                l1p1 = New Point(rect.Left, rect.Top)
                l1p2 = New Point(rect.Right, rect.Top)
                l2p1 = New Point(rect.Left, rect.Top + 1)
                l2p2 = New Point(rect.Right, rect.Top + 1)
            End If

            Dim hdc As IntPtr = Win32.GetDC(IntPtr.Zero)
            Win32.SetROP2(hdc, Win32.R2_NOT)
            Win32.MoveToEx(hdc, l1p1.X, l1p1.Y, IntPtr.Zero)
            Win32.LineTo(hdc, l1p2.X, l1p2.Y)
            Win32.MoveToEx(hdc, l2p1.X, l2p1.Y, IntPtr.Zero)
            Win32.LineTo(hdc, l2p2.X, l2p2.Y)
            Win32.ReleaseDC(IntPtr.Zero, hdc)
            _index = itemIndex
        End Sub
    End Class
End Namespace
