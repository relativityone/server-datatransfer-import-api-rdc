using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kCura.Relativity.ImportAPI.Data
{
	/// <summary>
	/// Describes an item in Relativity.
	/// </summary>
	public abstract class Artifact
	{
		/// <summary>
		/// Represents a unique identifier for an Artifact.
		/// </summary>
		public int ArtifactID { get; internal set; }

		/// <summary>
		/// Represents the identifier for the type of item that the Artifact represents.
		/// </summary>
		public int ArtifactTypeId { get; internal set; }

		/// <summary>
		/// Represents the identifier for the parent of the artifact.
		/// </summary>
		public int ParentArtifactID { get; internal set; }

		/// <summary>
		/// Indicates the value of the Identifier field for the artifact.
		/// </summary>
		public String Name { get; internal set; }

		/// <summary>
		/// Returns a string that represents the Artifact.
		/// </summary>
		/// <returns></returns>
		public override String ToString()
		{
			return Name;
		}

	}
}
