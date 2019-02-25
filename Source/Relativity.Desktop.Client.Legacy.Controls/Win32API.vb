Imports System.Runtime.InteropServices

Namespace kCura.Windows.Forms
	Public Class Win32API

#Region " Constants "

		Public Const MENU_CLASS As String = "#32768"
		Public Const HC_ACTION As Integer = 0
		Public Const WH_CALLWNDPROC As Integer = 4
		Public Const GWL_WNDPROC As Integer = -4

		Public Const WM_NCCALCSIZE As Integer = &H83
		Public Const WM_WINDOWPOSCHANGING As Integer = &H46
		Public Const WM_PAINT As Integer = &HF
		Public Const WM_CREATE As Integer = 1
		Public Const WM_NCCREATE As Integer = &H81
		Public Const WM_NCPAINT As Integer = 133
		Public Const WM_PRINT As Integer = 791
		Public Const WM_DESTROY As Integer = &H2
		Public Const WM_SHOWWINDOW As Integer = &H18
		Public Const WM_SHARED_MENU As Integer = 482
		Public Const WM_HSCROLL As Integer = &H114
		Public Const WM_VSCROLL As Integer = &H115

		Public Const SB_LINELEFT As Integer = 0
		Public Const SB_LINERIGHT As Integer = 1
		Public Const SB_PAGELEFT As Integer = 2
		Public Const SB_PAGERIGHT As Integer = 3
		Public Const SB_THUMBPOSITION As Integer = 4
		Public Const SB_THUMBTRACK As Integer = 5
		Public Const SB_LEFT As Integer = 6
		Public Const SB_RIGHT As Integer = 7
		Public Const SB_ENDSCROLL As Integer = 8
		Public Const SBS_HORZ As Integer = 0
		Public Const SBS_VERT As Integer = 1
		Public Const SIF_TRACKPOS As Integer = &H10
		Public Const SIF_RANGE As Integer = &H1
		Public Const SIF_POS As Integer = &H4
		Public Const SIF_PAGE As Integer = &H2
		Public Const SIF_ALL As Integer = SIF_RANGE Or SIF_PAGE Or SIF_POS Or SIF_TRACKPOS

#End Region

#Region " Delegates "

		Public Delegate Function WndProc(ByVal hwnd As IntPtr, ByVal msg As Integer, ByVal wparam As IntPtr, ByVal lparam As IntPtr) As Integer
		Public Delegate Function HookProc(ByVal code As Integer, ByVal wparam As IntPtr, ByRef cwp As CWPSTRUCT) As Integer

#End Region

#Region " Structures "

		<StructLayout(LayoutKind.Sequential)> _
		Public Structure CWPSTRUCT
			Public lparam As IntPtr
			Public wparam As IntPtr
			Public message As Integer
			Public hwnd As IntPtr
		End Structure

		<StructLayout(LayoutKind.Sequential)> _
		Public Structure WINDOWPOS
			Public hWnd As IntPtr
			Public hWndInsertAfter As IntPtr
			Public x As Integer
			Public y As Integer
			Public cx As Integer
			Public cy As Integer
			Public flags As Integer
		End Structure

		<StructLayout(LayoutKind.Sequential)> _
		Public Structure NCCALCSIZE_PARAMS
			Public rgrc0, rgrc1, rgrc2 As RECT
			Public lppos As IntPtr
		End Structure

		<StructLayout(LayoutKind.Sequential)> _
		Public Structure RECT
			Public Left As Integer
			Public Top As Integer
			Public Right As Integer
			Public Bottom As Integer
		End Structure

		<StructLayout(LayoutKind.Sequential)> _
		Public Structure ScrollInfoStruct
			Public cbSize As Integer
			Public fMask As Integer
			Public nMin As Integer
			Public nMax As Integer
			Public nPage As Integer
			Public nPos As Integer
			Public nTrackPos As Integer
		End Structure

#End Region

#Region " Function declarations "

		<DllImport("User32.dll", CharSet:=CharSet.Auto)> _
		Public Shared Function GetWindowRect(ByVal hWnd As IntPtr, ByRef rect As RECT) As Boolean
		End Function

		<DllImport("User32.dll", CharSet:=CharSet.Auto)> _
		Public Shared Function GetWindowDC(ByVal hWnd As IntPtr) As IntPtr
		End Function

		<DllImport("User32.dll", CharSet:=CharSet.Auto)> _
		Public Shared Function ReleaseDC(ByVal hWnd As IntPtr, ByVal hDC As IntPtr) As Integer
		End Function

		<DllImport("User32.dll", CharSet:=CharSet.Auto)> _
		Public Shared Function GetDlgItem(ByVal hDlg As IntPtr, ByVal nControlID As Integer) As IntPtr
		End Function

		<DllImport("User32.dll", CharSet:=CharSet.Auto)> _
		Public Shared Function SetWindowsHookEx(ByVal type As Integer, ByVal hook As HookProc, ByVal instance As IntPtr, ByVal threadID As Integer) As IntPtr
		End Function

		<DllImport("User32.dll", CharSet:=CharSet.Auto)> _
		Public Shared Function UnhookWindowsHookEx(ByVal hookHandle As IntPtr) As Boolean
		End Function

		<DllImport("User32.dll", CharSet:=CharSet.Auto)> _
		Public Shared Function GetWindowThreadProcessId(ByVal hwnd As IntPtr, ByVal ID As Integer) As Integer
		End Function

		<DllImport("User32.dll", CharSet:=CharSet.Auto)> _
		Public Shared Function GetClassName(ByVal hwnd As IntPtr, ByVal className As System.Text.StringBuilder, ByVal maxCount As Integer) As Integer
		End Function

		<DllImport("User32.dll", CharSet:=CharSet.Auto)> _
		Public Shared Function SetWindowLong(ByVal hwnd As IntPtr, ByVal index As Integer, ByVal wp As WndProc) As IntPtr
		End Function

		<DllImport("User32.dll", CharSet:=CharSet.Auto)> _
		Public Shared Function SetWindowLong(ByVal hwnd As IntPtr, ByVal index As Integer, ByVal dwNewLong As IntPtr) As IntPtr
		End Function

		<DllImport("User32.dll", CharSet:=CharSet.Auto)> _
		Public Shared Function CallNextHookEx(ByVal hookHandle As IntPtr, ByVal code As Integer, ByVal wparam As IntPtr, ByRef cwp As CWPSTRUCT) As Integer
		End Function

		<DllImport("User32.dll", CharSet:=CharSet.Auto)> _
		Public Shared Function CallWindowProc(ByVal wndProc As IntPtr, ByVal hwnd As IntPtr, ByVal msg As Integer, ByVal wparam As IntPtr, ByVal lparam As IntPtr) As Integer
		End Function

		<DllImport("user32.dll", CharSet:=CharSet.Auto)> _
		Public Shared Function GetScrollInfo(ByVal hwnd As IntPtr, ByVal nBar As Integer, ByRef lpsi As ScrollInfoStruct) As Integer
		End Function

#End Region

	End Class
End Namespace