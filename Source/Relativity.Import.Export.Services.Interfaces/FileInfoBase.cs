namespace Relativity.Import.Export.Services
{
	public partial class FileInfoBase
	{
		private string _fileGuidField;

		private string _fileNameField;

		private string _identifierField;

		private string _locationField;

		public string _fileGuid
		{
			get
			{
				return this._fileGuidField;
			}
			set
			{
				this._fileGuidField = value;
			}
		}

		public string _fileName
		{
			get
			{
				return this._fileNameField;
			}
			set
			{
				this._fileNameField = value;
			}
		}

		public string _identifier
		{
			get
			{
				return this._identifierField;
			}
			set
			{
				this._identifierField = value;
			}
		}

		public string _location
		{
			get
			{
				return this._locationField;
			}
			set
			{
				this._locationField = value;
			}
		}
	}
}