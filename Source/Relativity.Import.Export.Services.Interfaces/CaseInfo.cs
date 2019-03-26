namespace Relativity.Import.Export.Services
{
	public class CaseInfo
	{
		public int ArtifactID { get; set; }

		public bool AsImportAllowed { get; set; }

		public string DownloadHandlerURL { get; set; }

		public bool EnableDataGrid { get; set; }

		public bool ExportAllowed { get; set; }

		public int MatterArtifactID { get; set; }

		public string Name { get; set; }

		public int RootArtifactID { get; set; }

		public int RootFolderID { get; set; }

		public int StatusCodeArtifactID { get; set; }

		public string DocumentPath { get; set; }
	}
}
