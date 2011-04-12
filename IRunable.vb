Namespace kCura.Windows.Process
	''' <summary>
	''' Defines what it takes to be a runable process.
	''' </summary>
	''' <remarks></remarks>
	Public Interface IRunable
		''' <summary>
		''' A unique identifier that will be assigned to the object when it's started.
		''' </summary>
		''' <value>A <see cref="System.Guid"> that identifies the object.</see></value>
		''' <returns></returns>
		Property ProcessID() As Guid

		''' <summary>
		''' The method that will be executed by the managing process runner.
		''' </summary>
		''' <remarks></remarks>
		Sub StartProcess()
	End Interface
End Namespace
