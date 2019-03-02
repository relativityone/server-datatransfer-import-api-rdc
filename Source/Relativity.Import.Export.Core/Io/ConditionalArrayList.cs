// ----------------------------------------------------------------------------
// <copyright file="ConditionalArrayList.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Io
{
	using System;
	using System.Collections;

	/// <summary>
	/// Represents a class object that conditionally builds an array.
	/// </summary>
	internal class ConditionalArrayList
	{
		private readonly ArrayList list = new ArrayList();
		private readonly bool saveData;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConditionalArrayList"/> class.
		/// </summary>
		/// <param name="saveData">
		/// <see langword="true" /> to save the data; otherwise, <see langword="false" />.
		/// </param>
		public ConditionalArrayList(bool saveData)
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
		public void Add(object value)
		{
			if (this.saveData)
			{
				this.list.Add(value);
			}
		}

		/// <summary>
		/// Converts the list to the specified array type.
		/// </summary>
		/// <param name="type">
		/// The type of array to create.
		/// </param>
		/// <returns>
		/// The <see cref="System.Array"/> instance.
		/// </returns>
		public System.Array ToArray(Type type)
		{
			return this.list.ToArray(type);
		}
	}
}