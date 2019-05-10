using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kCura.Relativity.ImportAPI.Enumeration
{
	/// <summary>
	/// Specifies the category of a field.
	/// </summary>
	public enum FieldCategoryEnum
	{
		/// <summary>
		/// The field does not fit in a more specific category.
		/// </summary>
		Generic = 0,
		/// <summary>
		/// The field holds descriptive text.
		/// </summary>
		FullText =1,
		/// <summary>
		/// The field identifies something, such as a page.
		/// </summary>
		Identifier = 2,
		// Only used internally.  This field's information actually exists on a single object field.
		/// <summary>
		/// The field is reflected.
		/// </summary>
		Reflected = 3,
		/// <summary>
		/// The field is used for a reviewer's comments.
		/// </summary>
		Comments = 4,
		/// <summary>
		/// The field is relational, enabling identification of duplicates and document families.
		/// </summary>
		Relational = 5,
		/// <summary>
		/// The field describes an aspect of a production.
		/// </summary>
		ProductionMarker = 6,
		/// <summary>
		/// The field is automatically generated.
		/// </summary>
		AutoCreate = 7,
		/// <summary>
		/// The field holds the size of a file.
		/// </summary>
		FileSize = 8,
		/// <summary>
		/// The field holds the name of a folder.
		/// </summary>
		FolderName = 9,
		/// <summary>
		/// The field holds metadata or information about a file.
		/// </summary>
		FileInfo = 10,
		/// <summary>
		/// The field represents the artifact's parent ID.
		/// </summary>
		ParentArtifact = 11,
		/// <summary>
		/// The field holds a markup set to be used in a production.
		/// </summary>
		MarkupSetMarker = 12,
		// Whatever this means ???
		/// <summary>
		/// The field holds a nonspecific system.
		/// </summary>
		GenericSystem = 13,
		// Only used internally.  This field's information actually exists on multiple object fields.
		/// <summary>
		/// The field is multireflected.
		/// </summary>
		MultiReflected = 14,
		/// <summary>
		/// The field represents the grouping mechanism for a batch.
		/// </summary>
		Batch = 15

	}

}
