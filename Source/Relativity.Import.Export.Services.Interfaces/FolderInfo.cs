namespace Relativity.Import.Export.Services
{
	public partial class FolderInfo
	{
		private int ArtifactIDField;

		private string NameField;

		private Relativity.Import.Export.Services.FolderInfo ParentField;

		private int ParentArtifactIDField;

		public int ArtifactID
		{
			get
			{
				return this.ArtifactIDField;
			}
			set
			{
				this.ArtifactIDField = value;
			}
		}

		public string Name
		{
			get
			{
				return this.NameField;
			}
			set
			{
				this.NameField = value;
			}
		}

		public Relativity.Import.Export.Services.FolderInfo Parent
		{
			get
			{
				return this.ParentField;
			}
			set
			{
				this.ParentField = value;
			}
		}

		public int ParentArtifactID
		{
			get
			{
				return this.ParentArtifactIDField;
			}
			set
			{
				this.ParentArtifactIDField = value;
			}
		}
	}
}