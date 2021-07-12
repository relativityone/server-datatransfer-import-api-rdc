Imports Microsoft.VisualBasic.Devices

Namespace Helpers
	''' <summary>
	''' Provides information about current machine running import or export.
	''' </summary>
	Public Class ClientInformationHelper
		Private ReadOnly _computerInfo As ComputerInfo

		''' <summary>
		''' Initializes a new instance of <see cref="ClientInformationHelper"/> class.
		''' </summary>
		Public Sub New()
			Me._computerInfo = new ComputerInfo()
		End Sub

		''' <summary>
		''' Gets total amount of physical memory on the machine.
		''' </summary>
		''' <returns>Total amount of physical memory.</returns>
		Public ReadOnly Property TotalPhysicalMemory As ULong
			Get
				Return Me._computerInfo.TotalPhysicalMemory
			End Get
		End Property

		''' <summary>
		''' Gets total amount of free physical memory on the machine.
		''' </summary>
		''' <returns>The amount of free physical memory.</returns>
		Public ReadOnly Property AvailablePhysicalMemory As ULong
			Get
				Return Me._computerInfo.AvailablePhysicalMemory
			End Get
		End Property

		''' <summary>
		''' Gets name of operating system.
		''' </summary>
		''' <returns>Name of operating system.</returns>
		Public ReadOnly Property OperatingSystemName As String
			Get
				Return Me._computerInfo.OSFullName
			End Get
		End Property

		''' <summary>
		''' Gets version of operating system.
		''' </summary>
		''' <returns>Version of operating system.</returns>
		Public ReadOnly Property OperatingSystemVersion As String
			Get
				Return Me._computerInfo.OSVersion
			End Get
		End Property

		''' <summary>
		''' Gets a value indicating whether operating system is 64-bit operating system.
		''' </summary>
		''' <returns>True if OS is 64-bit; False otherwise.</returns>
		Public ReadOnly Property Is64BitOperatingSystem As Boolean
			Get
				Return Environment.Is64BitOperatingSystem
			End Get
		End Property

		''' <summary>
		''' Gets a value indicating whether the current process is 64-bit process.
		''' </summary>
		''' <returns>True if process is 64-bit; False otherwise.</returns>
		Public ReadOnly Property Is64BitProcess As Boolean
			Get
				Return Environment.Is64BitProcess
			End Get
		End Property

		''' <summary>
		''' Gets number of processors.
		''' </summary>
		''' <returns>Number of processors.</returns>
		Public ReadOnly Property CpuCount As Integer
			Get
				Return Environment.ProcessorCount
			End Get
		End Property
	End Class
End NameSpace