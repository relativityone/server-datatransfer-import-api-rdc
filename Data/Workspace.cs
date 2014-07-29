using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kCura.Relativity.ImportAPI.Data
{
	/// <summary>
	/// Representation of a workspace
	/// </summary>
	public class Workspace : Artifact
	{

		internal Workspace()
		{
		}



		public int MatterArtifactID { get; internal set; }

		public int StatusCodeArtifactID { get; internal set; }

		public int RootFolderID { get; internal set; }

		public int RootArtifactID { get; internal set; }

		public String DownloadHandlerURL { get; internal set; }

		public String DocumentPath { get; internal set; }

	}
}
