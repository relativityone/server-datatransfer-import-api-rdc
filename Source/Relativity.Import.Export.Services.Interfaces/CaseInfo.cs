namespace Relativity.Import.Export.Services
{
	public partial class CaseInfo
	{
		private int _ArtifactIDField;

		private bool _AsImportAllowedField;

		private string _DownloadHandlerURLField;

		private bool _EnableDataGridField;

		private bool _ExportAllowedField;

		private int _MatterArtifactIDField;

		private string _NameField;

		private int _RootArtifactIDField;

		private int _RootFolderIDField;

		private int _StatusCodeArtifactIDField;

		private string _documentPathField;

		public int _ArtifactID
		{
			get
			{
				return this._ArtifactIDField;
			}
			set
			{
				this._ArtifactIDField = value;
			}
		}

		public bool _AsImportAllowed
		{
			get
			{
				return this._AsImportAllowedField;
			}
			set
			{
				this._AsImportAllowedField = value;
			}
		}

		public string _DownloadHandlerURL
		{
			get
			{
				return this._DownloadHandlerURLField;
			}
			set
			{
				this._DownloadHandlerURLField = value;
			}
		}

		public bool _EnableDataGrid
		{
			get
			{
				return this._EnableDataGridField;
			}
			set
			{
				this._EnableDataGridField = value;
			}
		}

		public bool _ExportAllowed
		{
			get
			{
				return this._ExportAllowedField;
			}
			set
			{
				this._ExportAllowedField = value;
			}
		}

		public int _MatterArtifactID
		{
			get
			{
				return this._MatterArtifactIDField;
			}
			set
			{
				this._MatterArtifactIDField = value;
			}
		}

		public string _Name
		{
			get
			{
				return this._NameField;
			}
			set
			{
				this._NameField = value;
			}
		}

		public int _RootArtifactID
		{
			get
			{
				return this._RootArtifactIDField;
			}
			set
			{
				this._RootArtifactIDField = value;
			}
		}

		public int _RootFolderID
		{
			get
			{
				return this._RootFolderIDField;
			}
			set
			{
				this._RootFolderIDField = value;
			}
		}

		public int _StatusCodeArtifactID
		{
			get
			{
				return this._StatusCodeArtifactIDField;
			}
			set
			{
				this._StatusCodeArtifactIDField = value;
			}
		}

		public string _documentPath
		{
			get
			{
				return this._documentPathField;
			}
			set
			{
				this._documentPathField = value;
			}
		}
	}
}
