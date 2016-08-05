using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kCura.Relativity.ImportAPI.Data
{
	/// <summary>
	/// Represents a production set for use with an import job.
	/// </summary>
	public class ProductionSet : Artifact
	{
		// Or maybe the priority of this production set in the default view???
		/// <summary>
		/// Indicates the order in which to produce files.
		/// </summary>
		public int ProductionOrder { get; internal set; }
	}
}
