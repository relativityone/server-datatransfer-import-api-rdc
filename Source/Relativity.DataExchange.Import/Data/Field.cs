﻿using System;
using System.Collections.Generic;
using kCura.Relativity.ImportAPI.Enumeration;

namespace kCura.Relativity.ImportAPI.Data
{
	/// <summary>
	/// Represents a field, which is used to store data for documents and RDOs.
	/// </summary>
	public class Field : Artifact
	{
		/// <summary>
		/// Identifies the ObjectType associated with the field.
		/// </summary>
		public int? AssociatedObjectTypeID { get; internal set; }

		/// <summary>
		/// Indicates whether foreign language characters can be used.
		/// </summary>
		public bool UseUnicode { get; internal set; }

		/// <summary>
		/// Represents the ID of a FieldType used to determine the functionality and presentation of the field.
		/// </summary>
		public FieldTypeEnum FieldTypeID { get; internal set; }

		/// <summary>
		/// Indicates the length of a fixed-length text field.
		/// </summary>
		public int? FieldLength { get; internal set; }

		/// <summary>
		/// Provides the FieldCategory of the field definition.
		/// </summary>
		public FieldCategoryEnum FieldCategory { get; internal set; }

		/// <summary>
		/// Provides a list of GUIDs used to identify an Artifact.
		/// </summary>
		public IEnumerable<Guid> Guids { get; internal set; }

		/// <summary>
		/// Inidicates if the field should be written to Data Grid
		/// </summary>
		public bool EnableDataGrid { get; internal set; }
	}
}
