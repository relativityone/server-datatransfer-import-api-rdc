Namespace kCura.Windows.Process
	''' <summary>
	''' Defines what it takes to be a runable process.
	''' </summary>
	Public Interface IRunable
		''' <summary>
		''' A unique identifier that will be assigned to the object when it's started.
		''' </summary>
		''' <value>
		''' A <see cref="System.Guid"> that identifies the object.</see>
		''' </value>
		Property ProcessID() As Guid

		''' <summary>
		''' The method that will be executed by the managing process runner.
		''' </summary>
		Sub StartProcess()
	End Interface
End Namespace
