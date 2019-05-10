// ----------------------------------------------------------------------------
// <copyright file="SqlDataView.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Data
{
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Data;
	using System.Runtime.Serialization;

	/// <summary>
	/// Represents a SQL data view. This class cannot be inherited, backwards compatibility isn't guaranteed, and should never be consumed by API users.
	/// </summary>
	[Serializable]
	public sealed class SqlDataView : MarshalByRefObject, IEnumerable, ISerializable
	{
		/// <summary>
		/// The dataview backing.
		/// </summary>
		[NonSerialized]
		private readonly System.Data.DataView dataview;

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlDataView"/> class.
		/// </summary>
		/// <param name="table">
		/// The table containing the view data.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="table"/> is <see langword="null" />.
		/// </exception>
		public SqlDataView(DataTable table)
		{
			if (table == null)
			{
				throw new ArgumentNullException(nameof(table));
			}

			this.dataview = new System.Data.DataView(table);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlDataView"/> class.
		/// </summary>
		/// <param name="dataset">
		/// The data set containing the view data.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="dataset"/> is <see langword="null" />.
		/// </exception>
		public SqlDataView(DataSet dataset)
		{
			if (dataset == null)
			{
				throw new ArgumentNullException(nameof(dataset));
			}

			if (dataset.Tables.Count < 1)
			{
				throw new ArgumentException("The dataset must contain at least 1 table.", nameof(dataset));
			}

			this.dataview = new System.Data.DataView(dataset.Tables[0]);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlDataView"/> class.
		/// </summary>
		/// <param name="info">
		/// The serialization info.
		/// </param>
		/// <param name="context">
		/// The streaming context.
		/// </param>
		private SqlDataView(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}

			this.dataview = new System.Data.DataView();
			this.dataview.Table = (DataTable)info.GetValue(nameof(this.Table), typeof(DataTable));
			this.dataview.AllowDelete = info.GetBoolean("AllowDelete");
			this.dataview.AllowEdit = info.GetBoolean("AllowEdit");
			this.dataview.AllowNew = info.GetBoolean("AllowNew");
			this.dataview.ApplyDefaultSort = info.GetBoolean("ApplyDefaultSort");
			this.dataview.RowStateFilter = (DataViewRowState)info.GetValue("RowStateFilter", typeof(DataViewRowState));
			this.dataview.Site = (ISite)info.GetValue("Site", typeof(ISite));
		}

		/// <summary>
		/// Gets the underlying table.
		/// </summary>
		/// <value>
		/// The <see cref="DataTable"/> instance.
		/// </value>
		public DataTable Table => this.dataview.Table;

		/// <summary>
		/// Gets the record count.
		/// </summary>
		/// <value>
		/// The count.
		/// </value>
		public int Count => this.dataview.Count;

		/// <summary>
		/// Gets or sets the item at the specified index.
		/// </summary>
		/// <param name="index">
		/// The index to retrieve.
		/// </param>
		/// <returns>
		/// The item at the specified index.
		/// </returns>
		public SqlDataRow this[int index] => new SqlDataRow(this.dataview[index]);

		/// <summary>
		/// Gets an enumerator over this DataView.
		/// </summary>
		/// <returns>
		/// The <see cref="IEnumerator"/> instance.
		/// </returns>
		public IEnumerator GetEnumerator()
		{
			return this.dataview.GetEnumerator();
		}

		/// <summary>
		/// Gets the serialization info for this DataView.
		/// </summary>
		/// <param name="info">
		/// The serialization info to populate.
		/// </param>
		/// <param name="context">
		/// The streaming context.
		/// </param>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}

			info.AddValue("Table", this.Table);
			info.AddValue("AllowDelete", this.dataview.AllowDelete);
			info.AddValue("AllowEdit", this.dataview.AllowEdit);
			info.AddValue("AllowNew", this.dataview.AllowNew);
			info.AddValue("ApplyDefaultSort", this.dataview.ApplyDefaultSort);
			info.AddValue("RowStateFilter", this.dataview.RowStateFilter);
			info.AddValue("Site", this.dataview.Site);
		}
	}
}