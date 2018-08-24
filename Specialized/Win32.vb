Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Runtime.InteropServices

Namespace Oli.Controls
	Friend Module Win32
		<DllImport("gdi32.dll", EntryPoint:="SetROP2", CallingConvention:=CallingConvention.StdCall)>
		Function SetROP2(ByVal hdc As IntPtr, ByVal fnDrawMode As Integer) As Integer

		End Function
		<DllImport("user32.dll", EntryPoint:="GetDC", CallingConvention:=CallingConvention.StdCall)>
		Function GetDC(ByVal hWnd As IntPtr) As IntPtr

		End Function
		<DllImport("user32.dll", EntryPoint:="ReleaseDC", CallingConvention:=CallingConvention.StdCall)>
		Function ReleaseDC(ByVal hWnd As IntPtr, ByVal hDC As IntPtr) As IntPtr

		End Function
		<DllImport("gdi32.dll", EntryPoint:="MoveToEx", CallingConvention:=CallingConvention.StdCall)>
		Function MoveToEx(ByVal hdc As IntPtr, ByVal x As Integer, ByVal y As Integer, ByVal lpPoint As IntPtr) As Boolean

		End Function
		<DllImport("gdi32.dll", EntryPoint:="LineTo", CallingConvention:=CallingConvention.StdCall)>
		Function LineTo(ByVal hdc As IntPtr, ByVal x As Integer, ByVal y As Integer) As Boolean

		End Function
		Public Const R2_NOT As Integer = 6
	End Module
End Namespace
