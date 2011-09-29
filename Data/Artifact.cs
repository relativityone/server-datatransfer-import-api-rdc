using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kCura.Relativity.ImportAPI.Data
{
	public abstract class Artifact
	{

		public int ArtifactID { get; internal set; }

		public int ArtifactTypeId { get; internal set; }

		public int ParentArtifactID { get; internal set; }

		public String Name { get; internal set; }

		public override String ToString()
		{
			return Name;
		}

	}
}
