Namespace Relativity.Desktop.Client

	''' <summary>
	''' Represents a set of configuration values.
	''' </summary>
	Public Class Collection
		''' <summary>
		''' Gets or sets the hashtable of configuration values backing this collection.
		''' </summary>
		''' <returns>The configuration value of this collection.</returns>
		Public Property SectionHash As System.Collections.Hashtable

		''' <summary>
		''' Gets or sets the local time this Collection was most recently updated.
		''' </summary>
		''' <returns>The local time this Collection was most recently updated.</returns>
		Public Property LastRefresh As System.DateTime

		''' <summary>
		''' Gets or sets the lock object associated with this Collection.
		''' </summary>
		''' <returns>The lock object associated with this Collection.</returns>
		Public Property LockObject As System.Object

		''' <summary>
		''' Initializes a new, empty Collection.
		''' </summary>
		Public Sub New()
			SectionHash = New System.Collections.Hashtable
			LastRefresh = System.DateTime.MinValue
			LockObject = New System.Object
		End Sub
	End Class
End Namespace