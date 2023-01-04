Imports Relativity.DataExchange.Helpers

Namespace Monitoring
	Public Class MetricJobStarted
		Inherits MetricJobBase

		''' <inheritdoc/>
		Public Overrides ReadOnly Property BucketName As String = TelemetryConstants.BucketName.METRIC_JOB_STARTED

		''' <summary>
		''' Gets total amount of physical memory on the machine.
		''' </summary>
		''' <returns>Total amount of physical memory.</returns>
		Public Property TotalPhysicalMemory As ULong
			Get
				Return GetValueOrDefault(Of ULong)(TelemetryConstants.KeyName.TOTAL_PHYSICAL_MEMORY)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.TOTAL_PHYSICAL_MEMORY) = value
			End Set
		End Property

		''' <summary>
		''' Gets total amount of free physical memory on the machine.
		''' </summary>
		''' <returns>The amount of free physical memory.</returns>
		Public Property AvailablePhysicalMemory As ULong
			Get
				Return GetValueOrDefault(Of ULong)(TelemetryConstants.KeyName.AVAILABLE_PHYSICAL_MEMORY)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.AVAILABLE_PHYSICAL_MEMORY) = value
			End Set
		End Property

		''' <summary>
		''' Gets name of operating system.
		''' </summary>
		''' <returns>Name of operating system.</returns>
		Public Property OperatingSystemName As String
			Get
				Return GetValueOrDefault(Of String)(TelemetryConstants.KeyName.OPERATING_SYSTEM_NAME)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.OPERATING_SYSTEM_NAME) = value
			End Set
		End Property

		''' <summary>
		''' Gets version of operating system.
		''' </summary>
		''' <returns>Version of operating system.</returns>
		Public Property OperatingSystemVersion As String
			Get
				Return GetValueOrDefault(Of String)(TelemetryConstants.KeyName.OPERATING_SYSTEM_VERSION)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.OPERATING_SYSTEM_VERSION) = value
			End Set
		End Property

		''' <summary>
		''' Gets a value indicating whether operating system is 64-bit operating system.
		''' </summary>
		''' <returns>True if OS is 64-bit; False otherwise.</returns>
		Public Property Is64BitOperatingSystem As Boolean
			Get
				Return GetValueOrDefault(Of Boolean)(TelemetryConstants.KeyName.IS_64_BIT_OS)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.IS_64_BIT_OS) = value
			End Set
		End Property

		''' <summary>
		''' Gets a value indicating whether the current process is 64-bit process.
		''' </summary>
		''' <returns>True if process is 64-bit; False otherwise.</returns>
		Public Property Is64BitProcess As Boolean
			Get
				Return GetValueOrDefault(Of Boolean)(TelemetryConstants.KeyName.IS_64_BIT_PROCESS)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.IS_64_BIT_PROCESS) = value
			End Set
		End Property

		''' <summary>
		''' Gets number of processors.
		''' </summary>
		''' <returns>Number of processors.</returns>
		Public Property CpuCount As Integer
			Get
				Return GetValueOrDefault(Of Integer)(TelemetryConstants.KeyName.CPU_COUNT)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.CPU_COUNT) = value
			End Set
		End Property

		''' <summary>
		''' Gets name of the assembly that uses import or export.
		''' </summary>
		''' <returns>Name of the assembly that uses import or export.</returns>
		Public Property CallingAssembly As String
			Get
				Return GetValueOrDefault(Of String)(TelemetryConstants.KeyName.CALLING_ASSEMBLY)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.CALLING_ASSEMBLY) = HashingHelper.CalculateSHA256Hash(value)
			End Set
		End Property
	End Class
End Namespace