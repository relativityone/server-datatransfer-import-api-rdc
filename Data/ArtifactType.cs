using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kCura.Relativity.ImportAPI.Data
{
	/// <summary>
	/// Represents an artifact type, which uniquely describes a category of artifact.
	/// </summary>
    public class ArtifactType
    {
		/// <summary>
		/// Represents the unique ID of the ArtifactType.
		/// </summary>
        public int ID { get; internal set; }
		
		/// <summary>
		/// Represents the name the ArtifactType has been given.
		/// </summary>
        public string Name { get; internal set; }

		/// <summary>
		/// Returns the name of the ArtifactType as its String representation.
		/// </summary>
		/// <returns></returns>
        public override String ToString()
        {
            return Name;
        }
    }
}
