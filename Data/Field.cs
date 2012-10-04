using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kCura.Relativity.ImportAPI.Enumeration;

namespace kCura.Relativity.ImportAPI.Data
{
	public class Field : Artifact
	{
		public int? AssociatedObjectTypeID { get; internal set; }

		public bool UseUnicode { get; internal set; }

		public FieldTypeEnum FieldTypeID { get; internal set; }

		public int? FieldLength { get; internal set; }

		public FieldCategoryEnum FieldCategory { get; internal set; }

		public IEnumerable<Guid> Guids { get; internal set; }

	}
}
