namespace Relativity.Import.Export.Services
{
	public partial class FolderInfo
	{
		public int ArtifactID { get; set; }

		public string Name { get; set; }

		public Relativity.Import.Export.Services.FolderInfo Parent { get; set; }

		public int ParentArtifactID { get; set; }
	}
}