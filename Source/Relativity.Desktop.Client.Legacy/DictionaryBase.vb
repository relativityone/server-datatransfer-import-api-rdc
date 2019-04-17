Imports Relativity.Import.Export

Namespace Relativity.Desktop.Client
	''' <summary>
	''' Base class for all DictionaryBase types. 
	''' 
	''' This class is responsible for ensuring data does not go stale. On value access, if the dictionary has not been accessed in a period 
	''' of time specified by <see cref="Relativity.Import.Export.IAppSettings.ValueRefreshThreshold"/>, the values will be refreshed by a call to the abstract UpdateValues().
	''' </summary>
	Public MustInherit Class DictionaryBase
		Implements IDictionary

		Protected _sectionName As String
		Protected _valuesCollection As Collection
#Region " Virtual Methods "

		''' <summary>
		''' Copies the DictionaryBase elements to a one-dimensional System.Array instance at the specified index.
		''' </summary>
		''' <param name="array">The array into which DictionaryEntry objects will be copied.</param>
		''' <param name="index">The zero-based index into the array at which copying begins.</param>
		''' <exception cref="ArgumentException">
		''' Possible reasons:
		'''		- Array is multidimensional
		'''		- The number of elements in the source DictionaryBase is greater than the 
		'''			available space from <see param="index"/> to the end of <see param="array"/>
		''' </exception>
		''' <exception cref="ArgumentNullException"><see param="array"/> is null.</exception>
		''' <exception cref="ArgumentOutOfRangeException"><see param="index"/> is outside the bounds of the array.</exception>
		''' <exception cref="InvalidCastException">The type of the DictionaryBase cannot be cast automatically 
		'''		to the type of <see param="array"/>.</exception>
		Public Sub CopyTo(ByVal array As System.Array, ByVal index As Integer) Implements System.Collections.ICollection.CopyTo
			Me.Section.CopyTo(array, index)
		End Sub

		''' <summary>
		''' The number of key/value pairs in this DictionaryBase.
		''' </summary>
		''' <returns>The number of key/value pairs in this DictionaryBase.</returns>
		Public ReadOnly Property Count() As Integer Implements System.Collections.ICollection.Count
			Get
				Return Me.Section.Count
			End Get
		End Property

		''' <summary>
		''' Gets a value indicating whether access to this DictionaryBase is synchronized (thread-safe).
		''' </summary>
		''' <returns>True if access to this DictionaryBase is synchronized; otherwise, false. Defaults to false.</returns>
		Public ReadOnly Property IsSynchronized() As Boolean Implements System.Collections.ICollection.IsSynchronized
			Get
				Return Me.Section.IsSynchronized
			End Get
		End Property

		''' <summary>
		''' Gets an object that can be used to synchronize access to this DictionaryBase.
		''' </summary>
		''' <returns>An object that can be used to synchronize access to this DictionaryBase.</returns>
		Public ReadOnly Property SyncRoot() As Object Implements System.Collections.ICollection.SyncRoot
			Get
				Return Me.Section.SyncRoot
			End Get
		End Property

		''' <summary>
		''' Adds an element with the specified key and value into this DictionaryBase
		''' </summary>
		''' <param name="key">The key of the element to add.</param>
		''' <param name="value">The value of the element to add.</param>
		''' <exception cref="ArgumentException">An element with <see param="key"/> already exists in this DictionaryBase.</exception>
		''' <exception cref="ArgumentNullException"><see param="key"/> is null.</exception>
		''' <exception cref="NotSupportedException">
		''' Possible reasons:
		'''		- The DictionaryBase is read-only, or
		'''		- The DictionaryBase has a fixed size.
		''' </exception>
		Public Sub Add(ByVal key As Object, ByVal value As Object) Implements System.Collections.IDictionary.Add
			Me.Section.Add(key, value)
		End Sub

		''' <summary>
		''' Removes all elements from this DictionaryBase.
		''' </summary>
		''' <exception cref="NotSupportedException">The DictionaryBase is read-only.</exception>
		Public Sub Clear() Implements System.Collections.IDictionary.Clear
			Me.Section.Clear()
		End Sub

		''' <summary>
		''' Determines whether this DictionaryBase contains a specific key.
		''' </summary>
		''' <param name="key">The key to lookup.</param>
		''' <returns>true if this DictionaryBase contains an element with the specified key; otherwise, false.</returns>
		''' <exception cref="ArgumentNullException"><see param="key"/> is null.</exception>
		Public Function Contains(ByVal key As Object) As Boolean Implements System.Collections.IDictionary.Contains
			Return Me.Section.ContainsKey(key)
		End Function

		''' <summary>
		''' Gets an IDictionaryEnumerator that iterates through this DictionaryBase.
		''' </summary>
		''' <returns>An IDictionaryEnumerator for this DictionaryBase.</returns>
		Public Function GetIDicionaryEnumerator() As System.Collections.IDictionaryEnumerator Implements System.Collections.IDictionary.GetEnumerator
			Return Me.Section.GetEnumerator()
		End Function

		''' <summary>
		''' Gets an IDictionaryEnumerator that iterates through this DictionaryBase.
		''' </summary>
		''' <returns>An IDictionaryEnumerator for this DictionaryBase.</returns>
		Public Function GetIEnumerableEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
			Return Me.Section.GetEnumerator()
		End Function

		''' <summary>
		''' Gets a value indicating whether the DictionaryBase has a fixed size.
		''' </summary>
		''' <returns>True is this DictionaryBase has a fixed size; otherwise, false. Defaults to false.</returns>
		Public ReadOnly Property IsFixedSize() As Boolean Implements System.Collections.IDictionary.IsFixedSize
			Get
				Return Me.Section.IsFixedSize
			End Get
		End Property

		''' <summary>
		''' Gets a value indicating whether this DictionaryBase is read-only.
		''' </summary>
		''' <returns>True if this DictionaryBase is read-only; otherwise, false. Defaults to false.</returns>
		Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.IDictionary.IsReadOnly
			Get
				Return Me.Section.IsReadOnly
			End Get
		End Property

		''' <summary>
		''' Gets an ICollection containing the keys in this DictionaryBase.
		''' </summary>
		''' <returns>An ICollection containing the keys in this DictionaryBase.</returns>
		Public ReadOnly Property Keys() As System.Collections.ICollection Implements System.Collections.IDictionary.Keys
			Get
				Return Me.Section.Keys()
			End Get
		End Property

		''' <summary>
		''' Removes the element with the specified key from this DictionaryBase.
		''' </summary>
		''' <param name="key">The key of the element to remove.</param>
		''' <exception cref="ArgumentNullException"><see param="key"/> is null.</exception>
		''' <exception cref="NotSupportedException">
		''' Possible reasons:
		'''		- The DictionaryBase is read-only, or
		'''		- The DictionaryBase has a fixed size.
		''' </exception>
		Public Sub Remove(ByVal key As Object) Implements System.Collections.IDictionary.Remove
			Me.Section.Remove(key)
		End Sub

		''' <summary>
		''' Gets an ICollection containing the values in this DictionaryBase.
		''' </summary>
		''' <returns>An ICollection containing the values in this DictionaryBase.</returns>
		Public ReadOnly Property Values() As System.Collections.ICollection Implements System.Collections.IDictionary.Values
			Get
				Return Me.Section.Values
			End Get
		End Property

#End Region

#Region " Constructors "
		''' <summary>
		''' Initializes a new instance of DictionaryBase with the given section name and collection of values.
		''' </summary>
		''' <param name="sectionName">The name of the section these configuration values represent.</param>
		''' <param name="valuesCollection">The collection of configuration values to be used.</param>
		Protected Sub New(ByVal sectionName As String, ByVal valuesCollection As Collection)
			_sectionName = sectionName
			_valuesCollection = valuesCollection
		End Sub
#End Region

		''' <summary>
		''' Gets the underlying Hashtable backing this DictionaryBase representation.
		''' </summary>
		''' <returns>A hashtable containing a set of configuration values associated with a particular section name.</returns>
		Private ReadOnly Property Section As Hashtable
			Get
				SyncLock _valuesCollection.LockObject
					If DateTime.Now.Subtract(_valuesCollection.LastRefresh).TotalMilliseconds > AppSettings.Instance.ValueRefreshThreshold Then
						UpdateValues()
						_valuesCollection.LastRefresh = System.DateTime.Now
					End If
				End SyncLock
				Dim sectionHash As System.Collections.Hashtable = CType(_valuesCollection.SectionHash(_sectionName), System.Collections.Hashtable)
				Return If(sectionHash, New Hashtable()) ' when there is no config section, default to an empty hash table
			End Get
		End Property

		''' <summary>
		''' Gets the configuration value specified by the given key.
		''' </summary>
		''' <param name="key">The key of the element to get.</param>
		''' <returns>The element with the specified key.</returns>
		''' <exception cref="ConfigurationException">The provided key does not exist.</exception>
		Default Public Property Item(ByVal key As Object) As Object Implements System.Collections.IDictionary.Item
			Get
				Try
					Return Me.Section(key)
				Catch ex As Exception
					Throw New ConfigurationException(String.Format("Error retrieving '{0}' for section '{1}'", key.ToString, _sectionName), ex)
				End Try
			End Get
			Set(ByVal Value As Object)
				' DO NOTHING
			End Set
		End Property

		''' <summary>
		''' Base member for updating the underlying configuration values.
		''' </summary>
		Protected MustOverride Sub UpdateValues()
	End Class
End Namespace