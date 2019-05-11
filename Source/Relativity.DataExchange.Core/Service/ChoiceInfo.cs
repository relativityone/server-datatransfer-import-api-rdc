﻿//------------------------------------------------------------------------------
// <auto-generated>
// </auto-generated>
//------------------------------------------------------------------------------

namespace Relativity.DataExchange.Service
{
	using System;

	/// <summary>
	/// Represents a Relativity choice information data object. This class cannot be inherited, backwards compatibility isn't guaranteed, and should never be consumed by API users.
	/// </summary>
	/// <remarks>
	/// There's too much risk and too many expectation to make this type internal.
	/// </remarks>
	[Serializable]
	public sealed class ChoiceInfo
	{
		public int ArtifactID { get; set; }

		public int CodeTypeID { get; set; }

		public string Name { get; set; }

		public int Order { get; set; }

		public int ParentArtifactID { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ChoiceInfo"/> class.
		/// </summary>
		public ChoiceInfo()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChoiceInfo"/> class.
		/// </summary>
		/// <param name="row">
		/// The data row.
		/// </param>
		public ChoiceInfo(System.Data.DataRow row)
		{
			if (row == null)
			{
				throw new ArgumentNullException(nameof(row));
			}

			this.Order = System.Convert.ToInt32(row["Order"]);
			this.CodeTypeID = System.Convert.ToInt32(row["CodeTypeID"]);
			this.Name = System.Convert.ToString(row["Name"]);
			this.ArtifactID = System.Convert.ToInt32(row["ArtifactID"]);
			this.ParentArtifactID = System.Convert.ToInt32(row["ParentArtifactID"]);
		}
	}
}