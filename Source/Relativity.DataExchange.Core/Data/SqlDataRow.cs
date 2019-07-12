// ----------------------------------------------------------------------------
// <copyright file="SqlDataRow.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Data
{
	using System;
	using System.Data;

	/// <summary>
	/// Represents a SQL data row. This class cannot be inherited, backwards compatibility isn't guaranteed, and should never be consumed by API users.
	/// </summary>
	public sealed class SqlDataRow
	{
		/// <summary>
		/// The data row view backing.
		/// </summary>
		private readonly DataRowView row;

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlDataRow"/> class.
		/// </summary>
		/// <param name="row">
		/// The data row view.
		/// </param>
		public SqlDataRow(DataRowView row)
		{
			if (row == null)
			{
				throw new ArgumentNullException(nameof(row));
			}

			this.row = row;
		}

		/// <summary>
		/// Gets or sets the <see cref="object"/> at the specified index.
		/// </summary>
		/// <param name="index">
		/// The index.
		/// </param>
		/// <returns>
		/// The <see cref="object"/> instance.
		/// </returns>
		public object this[int index]
		{
			get => this.row[index];
			set => this.row[index] = value;
		}

		/// <summary>
		/// Gets or sets the <see cref="object"/> with the specified field name.
		/// </summary>
		/// <param name="fieldName">
		/// Name of the field.
		/// </param>
		/// <returns>
		/// The <see cref="object"/> instance.
		/// </returns>
		public object this[string fieldName]
		{
			get => this.row[fieldName];
			set => this.row[fieldName] = value;
		}
	}
}