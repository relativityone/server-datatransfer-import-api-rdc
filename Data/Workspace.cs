using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kCura.Relativity.ImportAPI.Data
{
	/// <summary>
	/// Represents a workspace.
	/// </summary>
	public class Workspace : Artifact
	{

		internal Workspace()
		{
		}


		/// <summary>
		/// Represents the identifier for the matter associated with the workspace.
		/// </summary>
		public int MatterArtifactID { get; internal set; }

		/// <summary>
		/// Represents the identifier for the workspace's current status.
		/// </summary>
		public int StatusCodeArtifactID { get; internal set; }

		// Not sure of the difference between this and the RootArtifactID???
		/// <summary>
		/// Indicates the root folder of the workspace.
		/// </summary>
		public int RootFolderID { get; internal set; }

		/// <summary>
		/// Indicates the ArtifactID assigned to the root folder of the workspace. 
		/// </summary>
		public int RootArtifactID { get; internal set; }

		/// <summary>
		/// Represents the default URL used when downloading files.
		/// </summary>
		public String DownloadHandlerURL { get; internal set; }

		/// <summary>
		/// Indicates the path to the workspace's documents.
		/// </summary>
		public String DocumentPath { get; internal set; }

	}
}
