using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kCura.Relativity.ImportAPI
{
	/// <summary>
	/// Representation of a workspace
	/// </summary>
	public class Workspace
	{

		internal Workspace()
		{
		}


		public int ArtifactID
		{
			get; internal set;
		}

		public String Name { get; internal set; }

		public int MatterArtifactID { get; internal set; }

		public int StatusCodeArtifactID { get; internal set; }

		public int RootFolderID { get; internal set; }

		public int RootArtifactID { get; internal set; }

		public String DownloadHandlerURL { get; internal set; }

		public String DocumentPath { get; internal set; }

		public override String ToString()
		{
			return Name;
		}
	}
}
