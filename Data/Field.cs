using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kCura.Relativity.ImportAPI.Data
{
	public class Field : Artifact
	{
		public int? AssociatedObjectTypeID { get; internal set; }

		public bool UseUnicode { get; internal set; }

		public int FieldTypeID { get; internal set; }

		public int? FieldLength { get; internal set; }


	}
}
