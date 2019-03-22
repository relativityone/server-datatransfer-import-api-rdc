namespace Relativity.Import.Export.Services
{
	public partial class ChoiceInfo
	{
		private int ArtifactIDField;

		private int CodeTypeIDField;

		private string NameField;

		private int OrderField;

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

		public int CodeTypeID
		{
			get
			{
				return this.CodeTypeIDField;
			}
			set
			{
				this.CodeTypeIDField = value;
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

		public int Order
		{
			get
			{
				return this.OrderField;
			}
			set
			{
				this.OrderField = value;
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