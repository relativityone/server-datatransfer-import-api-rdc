// ----------------------------------------------------------------------------
// <copyright file="ConditionalList.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Data
{
	using System.Collections.Generic;

	/// <summary>
	/// Represents a class object that conditionally builds an array.
	/// </summary>
	/// <typeparam name="T">The Type of the list.</typeparam>
	internal class ConditionalList<T>
	{
		private readonly List<T> list = new List<T>();
		private readonly bool saveData;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConditionalList{T}"/> class.
		/// </summary>
		/// <param name="saveData">
		/// <see langword="true" /> to save the data; otherwise, <see langword="false" />.
		/// </param>
		public ConditionalList(bool saveData)
		{
			this.saveData = saveData;
		}

		/// <summary>
		/// Gets the count.
		/// </summary>
		/// <value>
		/// The count.
		/// </value>
		public int Count => this.list.Count;

		/// <summary>
		/// Adds the specified object value to the list.
		/// </summary>
		/// <param name="value">
		/// The value to add to the list.
		/// </param>
		public void Add(T value)
		{
			if (this.saveData)
			{
				this.list.Add(value);
			}
		}

		/// <summary>
		/// Converts the list to the specified array type.
		/// </summary>
		/// <returns>
		/// The <see cref="System.Array"/> instance.
		/// </returns>
		public T[] ToArray()
		{
			return this.list.ToArray();
		}
	}
}