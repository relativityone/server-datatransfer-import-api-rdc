﻿//------------------------------------------------------------------------------
// <auto-generated>
// </auto-generated>
//------------------------------------------------------------------------------

namespace Relativity.Import.Export.Service
{
	using System;

	/// <summary>
	/// Represents a Relativity field information data object. This class cannot be inherited, backwards compatibility isn't guaranteed, and should never be consumed by API users.
	/// </summary>
	/// <remarks>
	/// There's too much risk and too many expectation to make this type internal.
	/// </remarks>
	[Serializable]
	public class FieldInfoBase
	{
		public FieldInfoBase()
		{
		}

		public int ArtifactID { get; set; }

		public FieldCategory Category { get; set; }

		public int CodeTypeID { get; set; }

		public string DisplayName { get; set; }

		public bool EnableDataGrid { get; set; }

		public int TextLength { get; set; }

		public FieldType Type { get; set; }
	}
}